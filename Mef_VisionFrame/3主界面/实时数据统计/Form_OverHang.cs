using AntdUI;
using AntdUI.In;
using Frame_Core;
using HalconDotNet;
using LogTool;
using NPOI.OpenXmlFormats.Dml.Diagram;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public partial class Form_OverHang : Form
    {
        public static int MaxFormDataAnalysisNum = 6;  
        public static int CurrentFormDataAnalysisNum = 0;
        bool isShow = false;
        public Form_OverHang()
        {
            InitializeComponent();
        }

        private void Form_OverHang_Load(object sender, EventArgs e)
        {

            UpdateForm();
        }

        public void UpdateForm()
        {
            TLPanal_CamMain.Invoke(new Action(() =>
            {
                if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.A面极片复检工位)
                {
                    TLPanal_CamMain.SetColumn(TLPanal_CamMain1, 0);
                    TLPanal_CamMain.SetColumn(TLPanal_CamMain2, 1);

                    TLPanal_CamMain1.SetRow(TLPanal_Cam4, 0);
                    TLPanal_CamMain1.SetColumn(TLPanal_Cam4, 0);
                    TLPanal_CamMain1.SetRowSpan(TLPanal_Cam4, 1);
                    TLPanal_CamMain1.SetColumnSpan(TLPanal_Cam4, 1);

                    TLPanal_CamMain1.SetRow(TLPanal_Cam3, 0);
                    TLPanal_CamMain1.SetColumn(TLPanal_Cam3, 1);
                    TLPanal_CamMain1.SetRowSpan(TLPanal_Cam3, 1);
                    TLPanal_CamMain1.SetColumnSpan(TLPanal_Cam3, 1);

                    TLPanal_CamMain1.SetRow(TLPanal_Cam2, 1);
                    TLPanal_CamMain1.SetColumn(TLPanal_Cam2, 0);
                    TLPanal_CamMain1.SetRowSpan(TLPanal_Cam2, 1);
                    TLPanal_CamMain1.SetColumnSpan(TLPanal_Cam2, 1);

                    TLPanal_CamMain1.SetRow(TLPanal_Cam1, 1);
                    TLPanal_CamMain1.SetColumn(TLPanal_Cam1, 1);
                    TLPanal_CamMain1.SetRowSpan(TLPanal_Cam1, 1);
                    TLPanal_CamMain1.SetColumnSpan(TLPanal_Cam1, 1);

                    TLPanal_CamMain2.SetRow(TLPanal_Cam8, 0);
                    TLPanal_CamMain2.SetColumn(TLPanal_Cam8, 0);
                    TLPanal_CamMain2.SetRowSpan(TLPanal_Cam8, 1);
                    TLPanal_CamMain2.SetColumnSpan(TLPanal_Cam8, 1);

                    TLPanal_CamMain2.SetRow(TLPanal_Cam7, 0);
                    TLPanal_CamMain2.SetColumn(TLPanal_Cam7, 1);
                    TLPanal_CamMain2.SetRowSpan(TLPanal_Cam7, 1);
                    TLPanal_CamMain2.SetColumnSpan(TLPanal_Cam7, 1);

                    TLPanal_CamMain2.SetRow(TLPanal_Cam6, 1);
                    TLPanal_CamMain2.SetColumn(TLPanal_Cam6, 0);
                    TLPanal_CamMain2.SetRowSpan(TLPanal_Cam6, 1);
                    TLPanal_CamMain2.SetColumnSpan(TLPanal_Cam6, 1);

                    TLPanal_CamMain2.SetRow(TLPanal_Cam5, 1);
                    TLPanal_CamMain2.SetColumn(TLPanal_Cam5, 1);
                    TLPanal_CamMain2.SetRowSpan(TLPanal_Cam5, 1);
                    TLPanal_CamMain2.SetColumnSpan(TLPanal_Cam5, 1);
                }
                else if (Project.Instance.systemInfo.ComStationsanwahl == (int)ComStationsanwahlEnum.B面极片复检工位)
                {

                    TLPanal_CamMain.SetColumn(TLPanal_CamMain1, 1);
                    TLPanal_CamMain.SetColumn(TLPanal_CamMain2, 0);

                    TLPanal_CamMain1.SetRow(TLPanal_Cam1, 0);
                    TLPanal_CamMain1.SetColumn(TLPanal_Cam1, 0);
                    TLPanal_CamMain1.SetRowSpan(TLPanal_Cam1, 1);
                    TLPanal_CamMain1.SetColumnSpan(TLPanal_Cam1, 1);

                    TLPanal_CamMain1.SetRow(TLPanal_Cam2, 0);
                    TLPanal_CamMain1.SetColumn(TLPanal_Cam2, 1);
                    TLPanal_CamMain1.SetRowSpan(TLPanal_Cam2, 1);
                    TLPanal_CamMain1.SetColumnSpan(TLPanal_Cam2, 1);

                    TLPanal_CamMain1.SetRow(TLPanal_Cam3, 1);
                    TLPanal_CamMain1.SetColumn(TLPanal_Cam3, 0);
                    TLPanal_CamMain1.SetRowSpan(TLPanal_Cam3, 1);
                    TLPanal_CamMain1.SetColumnSpan(TLPanal_Cam3, 1);

                    TLPanal_CamMain1.SetRow(TLPanal_Cam4, 1);
                    TLPanal_CamMain1.SetColumn(TLPanal_Cam4, 1);
                    TLPanal_CamMain1.SetRowSpan(TLPanal_Cam4, 1);
                    TLPanal_CamMain1.SetColumnSpan(TLPanal_Cam4, 1);


                    TLPanal_CamMain2.SetRow(TLPanal_Cam5, 0);
                    TLPanal_CamMain2.SetColumn(TLPanal_Cam5, 0);
                    TLPanal_CamMain2.SetRowSpan(TLPanal_Cam5, 1);
                    TLPanal_CamMain2.SetColumnSpan(TLPanal_Cam5, 1);

                    TLPanal_CamMain2.SetRow(TLPanal_Cam6, 0);
                    TLPanal_CamMain2.SetColumn(TLPanal_Cam6, 1);
                    TLPanal_CamMain2.SetRowSpan(TLPanal_Cam6, 1);
                    TLPanal_CamMain2.SetColumnSpan(TLPanal_Cam6, 1);

                    TLPanal_CamMain2.SetRow(TLPanal_Cam7, 1);
                    TLPanal_CamMain2.SetColumn(TLPanal_Cam7, 0);
                    TLPanal_CamMain2.SetRowSpan(TLPanal_Cam7, 1);
                    TLPanal_CamMain2.SetColumnSpan(TLPanal_Cam7, 1);

                    TLPanal_CamMain2.SetRow(TLPanal_Cam8, 1);
                    TLPanal_CamMain2.SetColumn(TLPanal_Cam8, 1);
                    TLPanal_CamMain2.SetRowSpan(TLPanal_Cam8, 1);
                    TLPanal_CamMain2.SetColumnSpan(TLPanal_Cam8, 1);
                }
            }));
        }

        /// <summary>
        /// 保存窗口截图
        /// </summary>
        /// <param name="userControl"></param>
        /// <param name="saveDirectoryPath"></param>
        /// <param name="saveFileName"></param>
        public void SaveWindowScreenshot(string imageSaveBasePath, string imageSaveRelativePath, string saveFileName, string source)
        {
            ExecuteResult executeResult = new ExecuteResult();
            try
            {
                string savePath = imageSaveBasePath + imageSaveRelativePath;

                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);

                this.Invoke(new Action(() =>
                {
                    HTuple time1;
                    HTuple time2;
                    //计时            
                    HOperatorSet.CountSeconds(out time1);

                    Bitmap bit = new Bitmap(panel1.Width, panel1.Height);//实例化一个和窗体一样大的bitmap
                    Graphics g = Graphics.FromImage(bit);
                    g.CompositingQuality = CompositingQuality.HighQuality;//质量设为最高
                    g.CopyFromScreen(panel1.Left, panel1.Top, 0, 0, new Size(panel1.Width, panel1.Height));//保存整个窗体为图片              
                    g.CopyFromScreen(panel1.PointToScreen(Point.Empty), Point.Empty, panel1.Size);//只保存某个控件（这里是panel游戏区）                 
                    bit.Save(savePath + saveFileName);//默认保存格式为PNG，保存成jpg格式质量不是很好
                                                      //计时            
                    HOperatorSet.CountSeconds(out time2);
                    double hTime3 = ((time2 - time1) * 1000).D;//算法总耗时
                                                               //Console.WriteLine("截图耗时：" + hTime3.ToString());
                }));

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(source, ex);
            }
        }


        private void listView10_DoubleClick(object sender, EventArgs e)
        {
            if (isShow) return;
            isShow =true;
            try
            {
                if (listView10.SelectedItems.Count > 0)
                {
                    //限制窗口数量
                    if (CurrentFormDataAnalysisNum >= MaxFormDataAnalysisNum)
                    {
                        isShow = false;
                        MessageBox.Show($"当前窗口数量已达到最大限制，请关闭其他窗口后再试。\r\n最大窗口数量：{MaxFormDataAnalysisNum}\r\n当前窗口数量：{CurrentFormDataAnalysisNum}");
                        return;
                    }

                    string rootPath = Project.Instance.paramPathSet.ProductMojuanPath;
                    List<string> PathList = new List<string>();
                    ListViewItem item = listView10.SelectedItems[0];

                    string str = $"时间:{item.SubItems[1].Text}\r\n" + $"电芯编号:{item.SubItems[1].Text}\r\n" + $"电芯:{item.SubItems[2].Text}\r\n" + $"极片层数:{item.SubItems[3].Text}\r\n" + $"结果:{item.SubItems[4].Text}\r\n";
                    using (FormMessagebox customMessageBox = new FormMessagebox(str, "打开NG历史图像"))
                    {
                        customMessageBox.StartPosition = FormStartPosition.CenterParent;
                        DialogResult result = customMessageBox.ShowDialog();

                        if (result == DialogResult.OK)
                        {
                            for (int i = 5; i < 13; i++)
                            {
                                PathList.Add(item.SubItems[i].Text);
                            }
                        }
                        else if (result == DialogResult.Cancel)
                        {
                            for (int i = 13; i < 21; i++)
                            {
                                PathList.Add(item.SubItems[i].Text);
                            }
                        }
                        PathList.Add(item.SubItems[21].Text);
                     
                        FormDataAnalysis fView = new FormDataAnalysis(rootPath, PathList, FormCloseCountCallBack);
                        fView.StartPosition = FormStartPosition.CenterParent;
                        string msg = fView.LoadHminImage();
                       
                        if (string.IsNullOrEmpty(msg))
                        {
                            MessageBox.Show(msg, "停止", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            fView.Dispose();                         
                            return;
                        }
                        else
                        {
                            fView.Text = str;
                            fView.Show();
                            CurrentFormDataAnalysisNum++;
                        }
                    }


                    //string str = $"时间:{item.SubItems[1].Text}\r\n" + $"电芯编号:{item.SubItems[1].Text}\r\n" + $"电芯:{item.SubItems[2].Text}\r\n" + $"极片层数:{item.SubItems[3].Text}\r\n" + $"结果:{item.SubItems[4].Text}\r\n";
                    //if (System.Windows.Forms.MessageBox.Show(str, "打开NG历史图像", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    //{
                    //    for (int i = 5; i < 13; i++)
                    //    {
                    //        PathList.Add(item.SubItems[i].Text);
                    //    }
                    //}
                    //else
                    //{
                    //    for (int i = 13; i < 21; i++)
                    //    {
                    //        PathList.Add(item.SubItems[i].Text);
                    //    }
                    //}
                    //PathList.Add(item.SubItems[21].Text);

                    //FormDataAnalysis fView = new FormDataAnalysis(rootPath, PathList);
                    //string msg = fView.LoadHminImage();

                    //if (string.IsNullOrEmpty(msg))
                    //{
                    //    MessageBox.Show(msg, "停止", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    //    fView.Dispose();
                    //    return;
                    //}
                    //else
                    //{
                    //    fView.Text = str;
                    //    fView.Show();
                    //}
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"展示NG图像出错：【{ex.Message}】", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            isShow = false;
        }

        public void FormCloseCountCallBack()
        {
            CurrentFormDataAnalysisNum--;
        }
    }
}
