using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    public partial class FloatingMessageForm : Form
    {
        private readonly UC_Message _mainUC;   // 主窗体UC引用
        private readonly UC_Message _slaveUC;  // 弹窗内UC
        private bool _isSyncing = false;       // 防止循环触发

        #region 长度记忆（实例级别，关闭即清零）
        internal int LastDebugLen;
        internal int LastErrorLen;
        internal int LastCommLen;
        #endregion

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        private const int WM_SETREDRAW = 0x000B;
        private const int WM_EXITSIZEMOVE = 0x0232;
        private const int WM_ENTERSIZEMOVE = 0x0231;

        /* 拖动期间禁止 RichTextBox 重绘 */
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_ENTERSIZEMOVE:   // 开始拖动边框
                    FreezeRichBoxes(true);
                    break;

                case WM_EXITSIZEMOVE:    // 拖动结束
                    FreezeRichBoxes(false);
                    break;
            }
            base.WndProc(ref m);
        }

        private void FreezeRichBoxes(bool freeze)
        {
            foreach (var rtb in Controls.OfType<UC_Message>().SelectMany(uc =>
                 new[] { uc.rtbx_Debug, uc.rtbx_Error, uc.rtbx_Comm }))
            {
                SendMessage(rtb.Handle, WM_SETREDRAW, (IntPtr)(freeze ? 0 : 1), IntPtr.Zero);
                if (!freeze) rtb.Invalidate();   // 恢复后重绘一次
            }
        }



        public FloatingMessageForm(UC_Message mainUC)
        {
            InitializeComponent();
            Text = "日志监视器";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(800, 600);
            TopMost = true;          // 置顶

            _mainUC = mainUC;
            _slaveUC = new UC_Message { Dock = DockStyle.Fill, IsSlave = true };
            Controls.Add(_slaveUC);

            // 双向事件绑定
            _mainUC.TabIndexChanged += idx => SyncToSlave(idx);
            _slaveUC.TabIndexChanged += idx => SyncToMain(idx);
            // 轻量级工具窗口，拖动时系统重绘更少
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
        }




        #region 同步逻辑
        private void SyncToSlave(int idx)
        {
            if (_isSyncing) return;
            _isSyncing = true;
            _slaveUC.SetTabIndexSilent(idx);
            _isSyncing = false;
        }

        private void SyncToMain(int idx)
        {
            if (_isSyncing) return;
            _isSyncing = true;
            _mainUC.SetTabIndexSilent(idx);
            _isSyncing = false;
        }
        #endregion

        #region 增量刷新 + 用户拖动感知
        internal void RefreshRichBox(RichTextBox rtb, ref int lastLen, string newText)
        {
            if (newText.Length == lastLen) return;   // 无增量

            bool atBottom = IsRichBoxAtBottom(rtb);  // 记录刷新前位置

            /* 只在变长且要滚底时才 Suspend，其余直接赋值 */
            if (atBottom)
            {
                rtb.SuspendLayout();
                rtb.Text = newText;
                rtb.SelectionStart = rtb.TextLength;
                rtb.ScrollToCaret();
                rtb.ResumeLayout();
            }
            else
            {
                rtb.Text = newText;      // 用户看历史，不滚底
            }
            lastLen = newText.Length;
        }
        #endregion
        #region
        [DllImport("user32.dll")]
        private static extern int GetScrollPos(IntPtr hWnd, int nBar);
        private const int SB_VERT = 1;

        private static bool IsRichBoxAtBottom(RichTextBox rtb)
        {
            int pos = GetScrollPos(rtb.Handle, SB_VERT);
            int lineH = rtb.Font.Height;
            int totalLines = rtb.GetPositionFromCharIndex(rtb.TextLength - 1).Y / lineH;
            int visibleLines = rtb.Height / lineH;
            return pos >= totalLines - visibleLines - 1;
        }


        #endregion
    }
}
