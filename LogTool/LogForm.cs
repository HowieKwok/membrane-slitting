using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using static System.Net.Mime.MediaTypeNames;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace LogTool
{
    public partial class LogForm : Form
    {
        LogParam logParam;
        public LogForm(LogParam _logParam)
        {
            InitializeComponent();
            logParam = _logParam;
        }
        private void Test()
        {

            //this.FormBorderStyle = FormBorderStyle.None;
            //this.Dock = DockStyle.Fill;
            //this.TopLevel = false;
            //logParam.filePath = $"{System.Environment.CurrentDirectory}\\logParam";

            //LogToolControl.SaveErrorLog( "", new Exception(), logParam);

            //LogToolControl.JudgeDirectoryStaleData("C:\\Users\\pengjiaqing\\Desktop\\宽幅连续涂003\\1", 5);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
               

                //Test();
                LogParamManage.GetLogParam(logParam.filePath, ref logParam);
                tbx_LogSavePath.Text = logParam.LogSavePath;
                nud_SaveFileSize.Value = logParam.SaveFileSize;
                nud_LogSaveDay.Value = logParam.LogSaveDay;

                //=========================选择项控件
                //选择项控件
                this.Tv_Select.HideSelection = false;
                //自已绘制  
                this.Tv_Select.DrawMode = TreeViewDrawMode.OwnerDrawText;

                Tv_Select.Nodes.Clear();

                string[] directoryNames = Directory.GetDirectories(logParam.LogSavePath);

                //遍历保存日志路径所有文件夹
                foreach (var item in directoryNames)
                {
                    //添加文件夹名称
                    string directoryName = Path.GetFileName(item);
                    Tv_Select.Nodes.Add(directoryName);

                    string[] fileNames = Directory.GetFiles(item);
                    //遍历文件夹下的所有文件
                    foreach (var item1 in fileNames)
                    {
                        string fileName = Path.GetFileName(item1);
                        Tv_Select.Nodes[Tv_Select.Nodes.Count - 1].Nodes.Add(fileName);
                    }
                }

                richTextBox_LogInfo.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(richTextBox_LogInfo, true);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());

            }


        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                logParam.LogSavePath = tbx_LogSavePath.Text.Trim();
                logParam.SaveFileSize = (int)nud_SaveFileSize.Value;
                logParam.LogSaveDay = (int)nud_LogSaveDay.Value;
                LogParamManage.SaveLogParam(logParam.filePath, logParam);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

        }

        private void linkLabel_LogSavePath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (true)
            {
                try
                {
                    // 获取文件夹绝对路径    显示在 txtbox 控件里
                    System.Windows.Forms.FolderBrowserDialog folder = new System.Windows.Forms.FolderBrowserDialog();

                    if (folder.ShowDialog() == DialogResult.OK && folder.SelectedPath != "")
                    {
                        this.tbx_LogSavePath.Text = folder.SelectedPath;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

        }

      
        #region 选择项控件
        /// 绘制颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tv_Select_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            //e.DrawDefault = true; //我这里用默认颜色即可，只需要在TreeView失去焦点时选中节点仍然突显  
            //return;

            //or  自定义颜色  
            if ((e.State & TreeNodeStates.Selected) != 0)
            {
                //演示为绿底白字  
                e.Graphics.FillRectangle(Brushes.DodgerBlue, e.Node.Bounds);

                Font nodeFont = e.Node.NodeFont;
                if (nodeFont == null) nodeFont = ((TreeView)sender).Font;
                e.Graphics.DrawString(e.Node.Text, nodeFont, Brushes.White, Rectangle.Inflate(e.Bounds, 2, 0));
            }
            else
            {
                e.DrawDefault = true;
            }

            if ((e.State & TreeNodeStates.Focused) != 0)
            {
                using (Pen focusPen = new Pen(Color.Black))
                {
                    focusPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    Rectangle focusBounds = e.Node.Bounds;
                    focusBounds.Size = new Size(focusBounds.Width - 1,
                    focusBounds.Height - 1);
                    e.Graphics.DrawRectangle(focusPen, focusBounds);
                }
            }

        }

        private void Tv_Select_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            try
            {
                // 假设treeView1是你的TreeView控件，selectedNode是当前选中的节点
                if (Tv_Select.SelectedNode != null)
                {
                    //获取当前选中节点的名称
                    StringBuilder path = new StringBuilder();
                    TreeNode currentNode = Tv_Select.SelectedNode;
                    while (currentNode != null)
                    {
                        path.Insert(0, "\\" + currentNode.Text);
                        currentNode = currentNode.Parent;
                    }

                    //获取选中节点的路径
                    string nodePath = logParam.LogSavePath + path.ToString();
                    //string logInfo = File.ReadAllText(nodePath, Encoding.UTF8);

                    //读取文件内容显示                
                    richTextBox_LogInfo.Invoke((EventHandler)delegate
                    {

                        richTextBox_LogInfo.Clear();
                        if (File.Exists(nodePath))
                        {
                            using (StreamReader sr = new StreamReader(nodePath, Encoding.UTF8))
                            {
                                string fileContent = sr.ReadToEnd();
                                richTextBox_LogInfo.Text = fileContent;
                            }

                            //richTextBox_LogInfo.LoadFile(nodePath, RichTextBoxStreamType.PlainText,);
                            richTextBox_LogInfo.SelectionStart = richTextBox_LogInfo.Text.Length; //自动滚到底部
                            richTextBox_LogInfo.ScrollToCaret();
                        }


                    });

                }
            }
            catch (Exception)
            {


            }
        }

        #endregion


    }
}
