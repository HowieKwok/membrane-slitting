using Frame_Core;
using FrameCommunity.Data;
using LogTool;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static NPOI.HSSF.Util.HSSFColor;

namespace Mef_VisionFrame
{
    public partial class UC_DieCuttingCut : UCViewBase
    {
        public NewDisplayWindowClass DisplayWindowClass1_EAImage;
        public NewDisplayWindowClass DisplayWindowClass1_BigImage;
        public NewDisplayWindowClass DisplayWindowClass2_EAImage;
        public NewDisplayWindowClass DisplayWindowClass2_BigImage;
        public NewDisplayWindowClass DisplayWindowClass3;
        public NewDisplayWindowClass DisplayWindowClass4;
        public NewDisplayWindowClass DisplayWindowClass5;
        public NewDisplayWindowClass DisplayWindowClass6;

        public NewDisplayWindowClass DisplayWindowClassNGImage1;
        public NewDisplayWindowClass DisplayWindowClassNGImage2;
        public NewDisplayWindowClass DisplayWindowClassNGImage3;
        public NewDisplayWindowClass DisplayWindowClassNGImage4;
        public NewDisplayWindowClass DisplayWindowClassNGImage5;
        public NewDisplayWindowClass DisplayWindowClassNGImage6;
        public NewDisplayWindowClass DisplayWindowClassNGImage7;
        public NewDisplayWindowClass DisplayWindowClassNGImage8;
        public NewDisplayWindowClass DisplayWindowClassNGImage9;
        public NewDisplayWindowClass DisplayWindowClassNGImage10;
        public NewDisplayWindowClass DisplayWindowClassNGImage11;
        public NewDisplayWindowClass DisplayWindowClassNGImage12;

        public CombinationHWindowControl<NewDisplayWindowClass> CombinationHWindowControl_cam1;
        public CombinationHWindowControl<NewDisplayWindowClass> CombinationHWindowControl_cam2;
        public CombinationHWindowControl<NewDisplayWindowClass> CombinationHWindowControl_cam4;
        public CombinationHWindowControl<NewDisplayWindowClass> CombinationHWindowControl_cam5;
        public CombinationHWindowControl<NewDisplayWindowClass> CombinationHWindowControl_cam6;

        public BindingList<QualityData> DefectStation_BindingDatas { get; } = new BindingList<QualityData>();
        public BindingList<QualityData> SizeStation_BindingDatas { get; } = new BindingList<QualityData>();
        VM_DieCuttingCut VM;



        public UC_DieCuttingCut(VM_DieCuttingCut _VM)
        {


            InitializeComponent();
            VM = _VM;
            VM.Server.InitView -= InitView;
            VM.Server.InitView += InitView;

        
            VM.Server.thd_ImageShowAndSave.PerformWork -= ImageShowAndSave;
            VM.Server.thd_ImageShowAndSave.PerformWork += ImageShowAndSave;

            DisplayWindowClass1_EAImage = new NewDisplayWindowClass(FrontEAHalconView1);
            DisplayWindowClass1_BigImage = new NewDisplayWindowClass(FrontInitHalconView1);
            DisplayWindowClass2_EAImage = new NewDisplayWindowClass(FrontEAHalconView2);
            DisplayWindowClass2_BigImage = new NewDisplayWindowClass(FrontInitHalconView2);
            DisplayWindowClass3 = new NewDisplayWindowClass(FrontSizeHalconView);
            DisplayWindowClass4 = new NewDisplayWindowClass(FrontSizeHalconView1);
            DisplayWindowClass5 = new NewDisplayWindowClass(FrontSizeHalconView2);
            DisplayWindowClass6 = new NewDisplayWindowClass(FrontSizeHalconView3);


            DisplayWindowClassNGImage1 = new NewDisplayWindowClass(hWindowControl1);
            DisplayWindowClassNGImage2 = new NewDisplayWindowClass(hWindowControl2);
            DisplayWindowClassNGImage3 = new NewDisplayWindowClass(hWindowControl3);
            DisplayWindowClassNGImage4 = new NewDisplayWindowClass(hWindowControl4);
            DisplayWindowClassNGImage5 = new NewDisplayWindowClass(hWindowControl5);
            DisplayWindowClassNGImage6 = new NewDisplayWindowClass(hWindowControl6);
            DisplayWindowClassNGImage7 = new NewDisplayWindowClass(hWindowControl7);
            DisplayWindowClassNGImage8 = new NewDisplayWindowClass(hWindowControl8);
            DisplayWindowClassNGImage9 = new NewDisplayWindowClass(hWindowControl9);
            DisplayWindowClassNGImage10 = new NewDisplayWindowClass(hWindowControl10);
            DisplayWindowClassNGImage11 = new NewDisplayWindowClass(hWindowControl11);
            DisplayWindowClassNGImage12 = new NewDisplayWindowClass(hWindowControl12);

            CombinationHWindowControl_cam1 = new CombinationHWindowControl<NewDisplayWindowClass>("Cam1", new List<NewDisplayWindowClass>() { DisplayWindowClassNGImage1, DisplayWindowClassNGImage2, DisplayWindowClassNGImage3 });
            CombinationHWindowControl_cam2 = new CombinationHWindowControl<NewDisplayWindowClass>("Cam2", new List<NewDisplayWindowClass>() { DisplayWindowClassNGImage4, DisplayWindowClassNGImage5, DisplayWindowClassNGImage6 });
            CombinationHWindowControl_cam4 = new CombinationHWindowControl<NewDisplayWindowClass>("Cam4", new List<NewDisplayWindowClass>() { DisplayWindowClassNGImage7, DisplayWindowClassNGImage10 });
            CombinationHWindowControl_cam5 = new CombinationHWindowControl<NewDisplayWindowClass>("Cam5", new List<NewDisplayWindowClass>() { DisplayWindowClassNGImage8, DisplayWindowClassNGImage11 });
            CombinationHWindowControl_cam6 = new CombinationHWindowControl<NewDisplayWindowClass>("Cam6", new List<NewDisplayWindowClass>() { DisplayWindowClassNGImage9, DisplayWindowClassNGImage12 });

            var sizeName = VM.Data.Craft.Get_Station3_SizeCrafts();
            foreach (var item in sizeName)
            {
                var chart = GetChart(item.Name, item.Describe);
                if (chart != null)
                {                           
                    VM.Data.chartUIUser[item.Name] = chart;
                }
            }

        }
        private void UC_DieCuttingCut_Load(object sender, EventArgs e)
        {
            InitView();

            Dp_ALineCamEADetect.DataListView.MouseDoubleClick -= listView1_DoubleClick;
            Dp_ALineCamEADetect.DataListView.MouseDoubleClick += listView1_DoubleClick;
            Dp_BLineCamEADetect.DataListView.MouseDoubleClick -= listView1_DoubleClick;
            Dp_BLineCamEADetect.DataListView.MouseDoubleClick += listView1_DoubleClick;
            Dp_PoleDeleft.DataListView.MouseDoubleClick -= listView1_DoubleClick;
            Dp_PoleDeleft.DataListView.MouseDoubleClick += listView1_DoubleClick;
            tableLayoutPanel1.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(this.tableLayoutPanel1, true);
            tableLayoutPanel11.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(this.tableLayoutPanel11, true);


            Dg_DefectStationStatistics.SetDoubleBuffered(true).BindingSource(DefectStation_BindingDatas);
            Dg_SizeStationStatistics.SetDoubleBuffered(true).BindingSource(SizeStation_BindingDatas);

            tabs2.SelectedIndex = 0;
        }
        public override void RefreshView()
        {
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                this.Invoke((EventHandler)delegate
                {
                    FrontInitHalconView1.Title = Service_Camera.GetCam_Name(0) + "大图";
                    FrontEAHalconView1.Title = Service_Camera.GetCam_Name(0) + "EA图";
                    FrontInitHalconView2.Title = Service_Camera.GetCam_Name(1) + "大图";
                    FrontEAHalconView2.Title = Service_Camera.GetCam_Name(1) + "EA图";
                    FrontSizeHalconView.Title = Service_Camera.GetCam_Name(2);
                    FrontSizeHalconView1.Title = Service_Camera.GetCam_Name(3);
                    FrontSizeHalconView2.Title = Service_Camera.GetCam_Name(4);
                    FrontSizeHalconView3.Title = Service_Camera.GetCam_Name(5);

                    var sizeName = VM.Data.Craft.Get_Station3_SizeCrafts();
                    foreach (var item in sizeName)
                    {
                        if (VM.Data.chartUIUser.ContainsKey(item.Name))
                        {
                            var chart = VM.Data.chartUIUser[item.Name];
                            if (chart != null)
                            {
                                double up = item.StandardValue + item.DBTolUpper;
                                double down = item.StandardValue - item.DBTolLower;
                                chart.GetAxess(down, up, out double Min_Value, out double Max_Value);
                                chart.SetSignalPlotLine(item.StandardValue, Color.White, "当前值");
                                chart.SetHorizontalLine(item.StandardValue, Color.LimeGreen, "标准值", LineStyle.Dash);
                                chart.SetHorizontalLine(up, Color.Red, "上限", LineStyle.Dash);
                                chart.SetHorizontalLine(down, Color.Red, "下限", LineStyle.Dash);
                                chart.SetYAxisRange(Min_Value, Max_Value);
                          
                            }
                        }

                    }
                });
            });
        }
        public override void InitView()
        {
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                this.Invoke((EventHandler)delegate
                {
                    NDataPanelViewModel.Init(Dp_ALineCamEADetect, VM.Data.Station1_ALineCamTitle_Defect.ToArray(), 80, 15);
                    NDataPanelViewModel.Init(Dp_BLineCamEADetect, VM.Data.Station1_BLineCamTitle_Defect.ToArray(), 80, 15);
                    NDataPanelViewModel.Init(DP_DefectSendData, VM.Data.Station1_CamSendTitle_Defect.ToArray(), 80, 15);

                    NDataPanelViewModel.Init(Dp_Cut, VM.Data.Station2_CutCamTitle.ToArray(), 80, 25);
                    NDataPanelViewModel.Init(Dp_CutSend, VM.Data.Station2_CutCamSendTitle.ToArray(), 80, 25);

                    NDataPanelViewModel.Init(Dp_PoleSize, VM.Data.Station3_Title_Size.ToArray(), 80, 15);
                    NDataPanelViewModel.Init(Dp_PoleDeleft, VM.Data.Station3_Title_Defect.ToArray(), 80, 15);
                    NDataPanelViewModel.Init(DP_SizeSendData, VM.Data.Station3_CamSendTitle_Size.ToArray(), 80, 15);

                    Dg_DefectStationStatistics.SetColumnHeaders();
                    Dg_SizeStationStatistics.SetColumnHeaders();
                    DefectStation_BindingDatas.Clear();
                    SizeStation_BindingDatas.Clear();


                });
                RefreshView();
            });
        }

        public void ImageShowAndSave()
        {
            int camIndex = 0;
            string camName = Service_Camera.GetCam_Name(camIndex);
            ImageSaveParamConfig saveParamConfig = Project.Instance.M_NewImageSaveParamer.ImageSaveConfigItems.GetSaveParam(camIndex);

            #region //相机1图像显示与保存
            if (true)
            {
                //原图        
                Server_ImageProcess.SaveImagesAsync(Server_ImageProcess.ImageSaveCallback, VM.Data.Cam1_SourceImage_ImageSave, VM.Data.Cam1_SourceImage_ImageDispose);

                //大图
                Server_ImageProcess.ShowImagesAsync((showHalconWindow) => Server_ImageProcess.ImageShowCallback(DisplayWindowClass1_BigImage, showHalconWindow),
                                                  VM.Data.Cam1_BigImage_ImageShow, VM.Data.Cam1_BigImage_ImageSave);

                Server_ImageProcess.SaveImagesAsync(Server_ImageProcess.ImageSaveCallback, VM.Data.Cam1_BigImage_ImageSave, VM.Data.Cam1_BigImage_ImageDispose);


                //EA图
                Server_ImageProcess.ShowImagesAsync((showHalconWindow) => Server_ImageProcess.ImageShowCallback(DisplayWindowClass1_EAImage, showHalconWindow, VM.Data.Cam1_EAImage_ScreenshotSave, VM.Data.Cam1_EAImage_HighScreenshotQueue),
                                                  VM.Data.Cam1_EAImage_ImageShow, VM.Data.Cam1_EAImage_ImageSave);

                Server_ImageProcess.SaveImagesAsync(Server_ImageProcess.ImageSaveCallback, VM.Data.Cam1_EAImage_ImageSave, VM.Data.Cam1_EAImage_ImageDispose);

                Server_ImageProcess.HighScreenshotAsync((showHalconWindow) => Server_ImageProcess.HighScreenshotCallback(DisplayWindowClass1_EAImage, showHalconWindow, VM.Data.Cam1_EAImage_ScreenshotSave),
                                                  VM.Data.Cam1_EAImage_HighScreenshotQueue);

                Server_ImageProcess.SaveScreenshotAsync(Server_ImageProcess.ScreenshotSaveCallback, VM.Data.Cam1_EAImage_ScreenshotSave);

                //NG缺陷图
                Server_ImageProcess.ShowImagesAsync((showHalconWindow) => Server_ImageProcess.ImageShowCallback(CombinationHWindowControl_cam1.GetDisplayWindowClass(), showHalconWindow),
                                                  VM.Data.Cam1_NGDefectImage_ImageShow, VM.Data.Cam1_NGDefectImage_ImageSave);

                Server_ImageProcess.SaveImagesAsync(Server_ImageProcess.ImageSaveCallback, VM.Data.Cam1_NGDefectImage_ImageSave, VM.Data.Cam1_NGDefectImage_ImageDispose);
            }
            #endregion

            #region //相机2图像显示与保存
            if (true)
            {
                //原图        
                Server_ImageProcess.SaveImagesAsync(Server_ImageProcess.ImageSaveCallback, VM.Data.Cam2_SourceImage_ImageSave, VM.Data.Cam2_SourceImage_ImageDispose);

                //大图
                Server_ImageProcess.ShowImagesAsync((showHalconWindow) => Server_ImageProcess.ImageShowCallback(DisplayWindowClass2_BigImage, showHalconWindow),
                                                  VM.Data.Cam2_BigImage_ImageShow, VM.Data.Cam2_BigImage_ImageSave);

                Server_ImageProcess.SaveImagesAsync(Server_ImageProcess.ImageSaveCallback, VM.Data.Cam2_BigImage_ImageSave, VM.Data.Cam2_BigImage_ImageDispose);


                //EA图
                Server_ImageProcess.ShowImagesAsync((showHalconWindow) => Server_ImageProcess.ImageShowCallback(DisplayWindowClass2_EAImage, showHalconWindow, VM.Data.Cam2_EAImage_ScreenshotSave, VM.Data.Cam2_EAImage_HighScreenshotQueue),
                                                  VM.Data.Cam2_EAImage_ImageShow, VM.Data.Cam2_EAImage_ImageSave);

                Server_ImageProcess.SaveImagesAsync(Server_ImageProcess.ImageSaveCallback, VM.Data.Cam2_EAImage_ImageSave, VM.Data.Cam2_EAImage_ImageDispose);

                Server_ImageProcess.HighScreenshotAsync((showHalconWindow) => Server_ImageProcess.HighScreenshotCallback(DisplayWindowClass2_EAImage, showHalconWindow, VM.Data.Cam2_EAImage_ScreenshotSave),
                                                  VM.Data.Cam2_EAImage_HighScreenshotQueue);

                Server_ImageProcess.SaveScreenshotAsync(Server_ImageProcess.ScreenshotSaveCallback, VM.Data.Cam2_EAImage_ScreenshotSave);

                //NG缺陷图
                Server_ImageProcess.ShowImagesAsync((showHalconWindow) => Server_ImageProcess.ImageShowCallback(CombinationHWindowControl_cam2.GetDisplayWindowClass(), showHalconWindow),
                                                  VM.Data.Cam2_NGDefectImage_ImageShow, VM.Data.Cam2_NGDefectImage_ImageSave);

                Server_ImageProcess.SaveImagesAsync(Server_ImageProcess.ImageSaveCallback, VM.Data.Cam2_NGDefectImage_ImageSave, VM.Data.Cam2_NGDefectImage_ImageDispose);
            }
            #endregion

            #region //相机3图像显示与保存
            if (true)
            {
                //原图
                Server_ImageProcess.ShowImagesAsync((showHalconWindow) => Server_ImageProcess.ImageShowCallback(DisplayWindowClass3, showHalconWindow, VM.Data.Cam3_SourceImage_ScreenshotSave, VM.Data.Cam3_SourceImage_HighScreenshotQueue),
                                                  VM.Data.Cam3_SourceImage_ImageShow, VM.Data.Cam3_SourceImage_ImageSave);

                Server_ImageProcess.SaveImagesAsync(Server_ImageProcess.ImageSaveCallback, VM.Data.Cam3_SourceImage_ImageSave, VM.Data.Cam3_SourceImage_ImageDispose);

                Server_ImageProcess.HighScreenshotAsync((showHalconWindow) => Server_ImageProcess.HighScreenshotCallback(DisplayWindowClass3, showHalconWindow, VM.Data.Cam3_SourceImage_ScreenshotSave),
                                                  VM.Data.Cam3_SourceImage_HighScreenshotQueue);

                Server_ImageProcess.SaveScreenshotAsync(Server_ImageProcess.ScreenshotSaveCallback, VM.Data.Cam3_SourceImage_ScreenshotSave);

            }
            #endregion

            #region //相机4图像显示与保存
            if (true)
            {
                //原图
                Server_ImageProcess.ShowImagesAsync((showHalconWindow) => Server_ImageProcess.ImageShowCallback(DisplayWindowClass4, showHalconWindow, VM.Data.Cam4_SourceImage_ScreenshotSave, VM.Data.Cam4_SourceImage_HighScreenshotQueue),
                                                  VM.Data.Cam4_SourceImage_ImageShow, VM.Data.Cam4_SourceImage_ImageSave);

                Server_ImageProcess.SaveImagesAsync(Server_ImageProcess.ImageSaveCallback, VM.Data.Cam4_SourceImage_ImageSave, VM.Data.Cam4_SourceImage_ImageDispose);

                Server_ImageProcess.HighScreenshotAsync((showHalconWindow) => Server_ImageProcess.HighScreenshotCallback(DisplayWindowClass4, showHalconWindow, VM.Data.Cam4_SourceImage_ScreenshotSave),
                                                  VM.Data.Cam4_SourceImage_HighScreenshotQueue);

                Server_ImageProcess.SaveScreenshotAsync(Server_ImageProcess.ScreenshotSaveCallback, VM.Data.Cam4_SourceImage_ScreenshotSave);


                //NG缺陷图
                Server_ImageProcess.ShowImagesAsync((showHalconWindow) => Server_ImageProcess.ImageShowCallback(CombinationHWindowControl_cam4.GetDisplayWindowClass(), showHalconWindow),
                                                  VM.Data.Cam4_NGDefectImage_ImageShow, VM.Data.Cam4_NGDefectImage_ImageSave);

                Server_ImageProcess.SaveImagesAsync(Server_ImageProcess.ImageSaveCallback, VM.Data.Cam4_NGDefectImage_ImageSave, VM.Data.Cam4_NGDefectImage_ImageDispose);
            }
            #endregion

            #region //相机5图像显示与保存
            if (true)
            {
                //原图
                Server_ImageProcess.ShowImagesAsync((showHalconWindow) => Server_ImageProcess.ImageShowCallback(DisplayWindowClass5, showHalconWindow, VM.Data.Cam5_SourceImage_ScreenshotSave, VM.Data.Cam5_SourceImage_HighScreenshotQueue),
                                                  VM.Data.Cam5_SourceImage_ImageShow, VM.Data.Cam5_SourceImage_ImageSave);

                Server_ImageProcess.SaveImagesAsync(Server_ImageProcess.ImageSaveCallback, VM.Data.Cam5_SourceImage_ImageSave, VM.Data.Cam5_SourceImage_ImageDispose);

                Server_ImageProcess.HighScreenshotAsync((showHalconWindow) => Server_ImageProcess.HighScreenshotCallback(DisplayWindowClass5, showHalconWindow, VM.Data.Cam5_SourceImage_ScreenshotSave),
                                                  VM.Data.Cam5_SourceImage_HighScreenshotQueue);

                Server_ImageProcess.SaveScreenshotAsync(Server_ImageProcess.ScreenshotSaveCallback, VM.Data.Cam5_SourceImage_ScreenshotSave);



                //NG缺陷图
                Server_ImageProcess.ShowImagesAsync((showHalconWindow) => Server_ImageProcess.ImageShowCallback(CombinationHWindowControl_cam5.GetDisplayWindowClass(), showHalconWindow),
                                                  VM.Data.Cam5_NGDefectImage_ImageShow, VM.Data.Cam5_NGDefectImage_ImageSave);

                Server_ImageProcess.SaveImagesAsync(Server_ImageProcess.ImageSaveCallback, VM.Data.Cam5_NGDefectImage_ImageSave, VM.Data.Cam5_NGDefectImage_ImageDispose);
            }
            #endregion

            #region //相机6图像显示与保存
            if (true)
            {
                //原图
                Server_ImageProcess.ShowImagesAsync((showHalconWindow) => Server_ImageProcess.ImageShowCallback(DisplayWindowClass6, showHalconWindow, VM.Data.Cam6_SourceImage_ScreenshotSave, VM.Data.Cam6_SourceImage_HighScreenshotQueue),
                                                  VM.Data.Cam6_SourceImage_ImageShow, VM.Data.Cam6_SourceImage_ImageSave);

                Server_ImageProcess.SaveImagesAsync(Server_ImageProcess.ImageSaveCallback, VM.Data.Cam6_SourceImage_ImageSave, VM.Data.Cam6_SourceImage_ImageDispose);

                Server_ImageProcess.HighScreenshotAsync((showHalconWindow) => Server_ImageProcess.HighScreenshotCallback(DisplayWindowClass6, showHalconWindow, VM.Data.Cam6_SourceImage_ScreenshotSave),
                                                  VM.Data.Cam6_SourceImage_HighScreenshotQueue);

                Server_ImageProcess.SaveScreenshotAsync(Server_ImageProcess.ScreenshotSaveCallback, VM.Data.Cam6_SourceImage_ScreenshotSave);


                //NG缺陷图
                Server_ImageProcess.ShowImagesAsync((showHalconWindow) => Server_ImageProcess.ImageShowCallback(CombinationHWindowControl_cam6.GetDisplayWindowClass(), showHalconWindow),
                                                  VM.Data.Cam6_NGDefectImage_ImageShow, VM.Data.Cam6_NGDefectImage_ImageSave);

                Server_ImageProcess.SaveImagesAsync(Server_ImageProcess.ImageSaveCallback, VM.Data.Cam6_NGDefectImage_ImageSave, VM.Data.Cam6_NGDefectImage_ImageDispose);
            }
            #endregion
        }

        private void timer_Viewrefresh_Tick(object sender, EventArgs e)
        {
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                lb_num1.Text = VM.Data.Station1_StatisticalData.SumupCount.ToString();
                lb_OK1.Text = VM.Data.Station1_StatisticalData.OKCount.ToString();
                lb_ng1.Text = VM.Data.Station1_StatisticalData.NGCount.ToString();
                lb_yield1.Text = VM.Data.Station1_StatisticalData.Yield.ToString() + "%";



                lb_num3.Text = VM.Data.Station3_StatisticalData.SumupCount.ToString();
                lb_OK3.Text = VM.Data.Station3_StatisticalData.OKCount.ToString();
                lb_ng3.Text = VM.Data.Station3_StatisticalData.NGCount.ToString();
                lb_yield3.Text = VM.Data.Station3_StatisticalData.Yield.ToString() + "%";


                if (VM.Data.DefectStation_BindingDatasShow.Count > 0)
                {
                    DefectStation_BindingDatas.UdpateData(VM.Data.DefectStation_BindingDatasShow.Dequeue());
                }

                if (VM.Data.SizeStation_BindingDatasShow.Count > 0)
                {
                    SizeStation_BindingDatas.UdpateData(VM.Data.SizeStation_BindingDatasShow.Dequeue());
                }


                Station3DefectData_ShowAndSave();
            });
        }

        public void Station3DefectData_ShowAndSave()
        {
            ExecuteMethodClass.ExecuteMethod(() =>
            {
                //【】数据保存与显示
                var showData = VM.Data.Station3_Defect_DataShowQueue.DequeueBatch(1000);

                foreach (var item in showData)
                {
                    NDataPanelViewModel.Add(Dp_PoleDeleft, VM.Data.Station3_Title_Defect.ToArray(), item);
                }

            });
        }

        private MonitorForm _monitorForm;
        private MonitorForm V_monitorForm;
        private void divider4_DoubleClick(object sender, EventArgs e)
        {
            // 监控窗体全程只保留一个实例，重复点击只置前显示。
            if (_monitorForm == null || _monitorForm.IsDisposed)
            {
                _monitorForm = new MonitorForm(VM.Data.Cut_CorrectPosDeal, "裁切工位");
                _monitorForm.Show(this);
            }
            else
            {
                if (_monitorForm.WindowState == FormWindowState.Minimized)
                    _monitorForm.WindowState = FormWindowState.Normal;
                _monitorForm.Activate();
            }

            // 监控窗体全程只保留一个实例，重复点击只置前显示。
            if (V_monitorForm == null || V_monitorForm.IsDisposed)
            {
                V_monitorForm = new MonitorForm(VM.Data.V_CorrectPosDeal, "V裁切工位");
                V_monitorForm.Show(this);
            }
            else
            {
                if (V_monitorForm.WindowState == FormWindowState.Minimized)
                    V_monitorForm.WindowState = FormWindowState.Normal;
                V_monitorForm.Activate();
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (sender is ListView)
            {
                ListView dp = sender as ListView;

                // 使用 SelectedIndices 而不是 SelectedItems
                if (dp.SelectedIndices.Count > 0)
                {
                    // 获取第一个选中项的索引
                    int selectedIndex = dp.SelectedIndices[0];

                    // 通过索引获取 ListViewItem（这会触发 RetrieveVirtualItem 事件）
                    ListViewItem selectedItem = dp.Items[selectedIndex];

                    // 获取"是否保存"和"路径"列的索引
                    int saveIndex = selectedItem.SubItems.Count - 2;
                    int pathIndex = selectedItem.SubItems.Count - 1;

                    if (saveIndex >= 0 && pathIndex >= 0)
                    {
                        string saveStatus = selectedItem.SubItems[saveIndex].Text;
                        string imagePath = selectedItem.SubItems[pathIndex].Text;

                        if (saveStatus.Equals("True", StringComparison.OrdinalIgnoreCase))
                        {
                            if (File.Exists(imagePath))
                            {
                                // 使用默认程序打开图片
                                System.Diagnostics.Process.Start(new ProcessStartInfo(imagePath) { UseShellExecute = true });
                            }
                            else
                            {
                                MessageBox.Show("图片文件不存在！");
                            }
                        }
                    }
                }
            }


        }


        private ScotPotUIUser GetChart(string name, string describe)
        {
            switch (name)
            {
                case "单片长度":
                    return new ScotPotUIUser(formsPlot1, describe, 6, 8);
                case "带料高":
                    return new ScotPotUIUser(formsPlot2, describe, 6, 8);
                case "单片宽度":
                    return new ScotPotUIUser(formsPlot3, describe, 6, 8);
                case "极耳宽度":
                    return new ScotPotUIUser(formsPlot4, describe, 6, 8);
                case "极耳左肩宽":
                    return new ScotPotUIUser(formsPlot5, describe, 6, 8);
                case "极耳高度":
                    return new ScotPotUIUser(formsPlot6, describe, 6, 8);
                case "下左V宽X":
                    return new ScotPotUIUser(formsPlot7, describe, 6, 8);
                case "下右V宽X":
                    return new ScotPotUIUser(formsPlot8, describe, 6, 8);
                case "下左V深Y":
                    return new ScotPotUIUser(formsPlot9, describe, 6, 8);
                case "下右V深Y":
                    return new ScotPotUIUser(formsPlot10, describe, 6, 8);
                case "上左V宽X":
                    return new ScotPotUIUser(formsPlot11, describe, 6, 8);
                case "上左V深Y":
                    return new ScotPotUIUser(formsPlot12, describe, 6, 8);
                case "上右V宽X":
                    return new ScotPotUIUser(formsPlot13, describe, 6, 8);
                case "上右V深Y":
                    return new ScotPotUIUser(formsPlot14, describe, 6, 8);

            }
            return null;
        }
    }
}
