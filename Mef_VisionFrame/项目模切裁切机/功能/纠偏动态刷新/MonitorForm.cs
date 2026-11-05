using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public partial class MonitorForm : Form
    {
        private readonly CorrectPosDeal _deal;
        private readonly BindingList<PieceRow> _beforeRows = new BindingList<PieceRow>();
        private readonly BindingList<PieceRow> _afterRows = new BindingList<PieceRow>();
        private CorrecInfo _pendingCutPiece;
        private bool _refreshQueued;

        public MonitorForm(CorrectPosDeal deal, string windowName)
        {
            _deal = deal;
            InitializeComponent();
            Text = string.IsNullOrWhiteSpace(windowName)
                ? "极片裁切 · 纠偏监控台"
                : windowName + " - 极片裁切 · 纠偏监控台";
            BeforeTable.DataSource = _beforeRows;
            AfterTable.DataSource = _afterRows;

            // 卡片只在窗体创建时初始化；高速刷新时只修改已有标签文本。
            InitializeCards();
            RefreshValues(new CorrecInfo());
            _deal.Updated += QueueRefresh;
            FormClosed += MonitorForm_FormClosed;
     
        }

        private void QueueRefresh(CorrectPosDeal deal, CorrecInfo cutPiece)
        {
            _pendingCutPiece = cutPiece;
            if (_refreshQueued || IsDisposed) return;
            _refreshQueued = true;
            try { BeginInvoke((Action)ProcessQueuedRefresh); }
            catch (InvalidOperationException) { _refreshQueued = false; }
        }

        private void ProcessQueuedRefresh()
        {
            if (IsDisposed) return;
            _refreshQueued = false;
            RefreshValues(_pendingCutPiece);
        }

        private void MonitorForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _deal.Updated -= QueueRefresh;
        }

        private void InitializeCards()
        {
            Pieces.Controls.Clear();
            CutPieces.Controls.Clear();
            foreach (CorrecInfo item in _deal.correcInfos.AsEnumerable().Reverse())
                Pieces.Controls.Add(CreateCard(false));
            CutPieces.Controls.Add(CreateCard(true));
            CutPieces.Controls.Add(CreateCard(true));
        }

        private void RefreshValues(CorrecInfo cutPiece)
        {
            RunLengthValue.Text = _deal.fRunLength.ToString("0.000") + " mm";
            PieWidthValue.Text = _deal.fPieWidth.ToString("0.000") + " mm";
            PieCountValue.Text = _deal.nPieNum.ToString();
            CurrentValue.Text = _deal.CorrectValue.ToString("+0.000;-0.000;0.000") + " mm";
            UpdateCardTexts(Pieces, _deal.correcInfos.AsEnumerable().Reverse().ToList(), "来料极片");
            UpdateCardTexts(CutPieces, _deal.CutPieces, "切后极片");
            RefreshTables(cutPiece);
        }

        private static void UpdateCardTexts(FlowLayoutPanel panel, IList<CorrecInfo> items, string title)
        {
            for (int i = 0; i < panel.Controls.Count; i++)
            {
                Label label = (Label)panel.Controls[i].Tag;
                if (i < items.Count)
                {
                    CorrecInfo item = items[i];
                    label.Text = string.Format("{0}\r\n编号 {1}\r\n{2:0.000} mm\r\n变化 {3}", title, item.nFrameID, item.Value, item.ChangeText);
                }
                else label.Text = title + "\r\n等待裁切...";
            }
        }

        private void RefreshTables(CorrecInfo cutPiece)
        {
            _beforeRows.Clear();
            foreach (CorrecInfo item in _deal.correcInfos.AsEnumerable().Reverse())
                _beforeRows.Add(new PieceRow(item));
            if (cutPiece != null && (cutPiece.nFrameID != -1 || cutPiece.CorrectPosChange.Count > 0))
            {
                _afterRows.Insert(0, new PieceRow(cutPiece));
                while (_afterRows.Count > 10) _afterRows.RemoveAt(_afterRows.Count - 1);
            }
        }

        private static Control CreateCard(bool cut)
        {
            Panel card = new Panel { Width = 116, Height = 94, Margin = new Padding(4), BackColor = cut ? Color.FromArgb(23, 85, 72) : Color.FromArgb(16, 41, 66), BorderStyle = BorderStyle.FixedSingle };
            Label label = new Label { Dock = DockStyle.Fill, Padding = new Padding(7), Font = new Font("Microsoft YaHei", 8.5f), ForeColor = Color.FromArgb(220, 245, 255) };
            card.Tag = label;
            card.Controls.Add(label);
            return card;
        }
    }

    public sealed class PieceRow
    {
        public PieceRow(CorrecInfo info) { FrameId = info.nFrameID; Value = info.Value.ToString("0.000"); CorrectPosChange = info.ChangeText; DisplacementText=info.DisplacementText;  }
        public int FrameId { get; private set; }
        public string Value { get; private set; }
        public string CorrectPosChange { get; private set; }

        public string DisplacementText { get; private set; }
    }
}
