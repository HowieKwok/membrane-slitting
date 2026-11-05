using DevMgmt.Cameras.Models;
using Extensions;
using HalconDotNet;
using LogTool;
using NPOI.OpenXmlFormats.Vml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using ViewROI3;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Frame_Core
{
    public partial class Form_MsaCheckBoardLine : Form
    {

        public HWndCtrl hWndCtrl_Display;
        public ROIController roiController;
        public ROIRectangle2 roiRectangle;
        HObject ho_Image = null;
        ROIFormBase ROIForm = new ROIFormBase();
     

        List<MessageShowInfo> messageShowInfos = new List<MessageShowInfo>();
        List<RegionShowInfo> regionShowInfos = new List<RegionShowInfo>();
        List<MessageShowInfo> messageShowInfos_Temp = new List<MessageShowInfo>();
        List<RegionShowInfo> regionShowInfos_Temp = new List<RegionShowInfo>();

        /// <summary>
        /// 图片仿真路径数据
        /// </summary>
        public ImageSimManage m_ImageSimManage_Cam = new ImageSimManage();



        public Form_MsaCheckBoardLine(String _path)
        {
            InitializeComponent();
            string strLRoiParaPath= _path + "\\LRoiPara.xml";
            string strLRoiMesureParaPath = _path + "\\LRoiMesurePara.xml";
            cbx_CamSelect.Items.AddRange(Service_Camera.GetCam_Names());

            cbx_CamSelect.SelectedIndex = 0;
            cobSelect3D.SelectedIndex = 0;
            roiController = new ROIController(strLRoiParaPath, strLRoiMesureParaPath);
            hWndCtrl_Display = new HWndCtrl(HWC_Display);
            hWndCtrl_Display.useROIController(roiController);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_MOVE);
            hWndCtrl_Display.MeasureMethod -= measureMethod;
            hWndCtrl_Display.MeasureMethod += measureMethod;

            hWndCtrl_Display.ROIController.LoadROI();
            hWndCtrl_Display.ROIController.LoadROIMeasureDics();

            hWndCtrl_Display.initControlDele -= Initialcontrol;
            hWndCtrl_Display.initControlDele += Initialcontrol;
            hWndCtrl_Display.ImageProcessingEvent += new HWndCtrl.ImageProcessingDelegate(RunFunction);   //更改测量矩形，执行测量方法
            hWndCtrl_Display.ReDrawEvent += new HWndCtrl.ReDrawDelegate(ShowResult);     //更改测量矩形，重绘测量结果
            linkLabel1.Text = Project.Instance.paramPathSet.ProductMojuanPath;

            //OIForm = hWndCtrl_Display.formBase;
            // hWndCtrl_Display.formBase.Dock = DockStyle.Fill;
            // panel1.Controls.Add(hWndCtrl_Display.formBase);
        }


        //private void LoadImage_Click(object sender, EventArgs e)
        //{
        //    //打开指定路径文件夹，加载图片
        //    OpenFileDialog FileDialog = new OpenFileDialog();
        //    // FileDialog.InitialDirectory = "E:\\";
        //    if (FileDialog.ShowDialog() == DialogResult.OK)
        //    {
        //        try
        //        {
        //            ho_Image = new HImage(FileDialog.FileName);

        //            hWndCtrl_Display.addIconicVar(ho_Image);

        //            //SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
        //            hWndCtrl_Display.resetAll();
        //            hWndCtrl_Display.repaint();

        //        }
        //        catch (Exception)
        //        {
        //        }
        //    }
        //}


        private void LoadImage_Click(object sender, EventArgs e)
        {
            //打开指定路径文件夹，加载图片
            OpenFileDialog FileDialog = new OpenFileDialog();
            FileDialog.Multiselect = true;
            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {

                  
                    ho_Image?.Dispose();
                    m_ImageSimManage_Cam.InitSimFlag();
                    m_ImageSimManage_Cam.ImageEmulationPaths = FileDialog.FileNames;

                    if (m_ImageSimManage_Cam.ImageEmulationPaths == null || m_ImageSimManage_Cam.ImageEmulationPaths.Length == 0) return;
                    m_ImageSimManage_Cam.GetCurrentImage(out ho_Image, out string imgPath);

                    if (ho_Image != null && ho_Image?.CountObj() > 0)
                    {
                        ho_Image=new HImage(ho_Image);
                        hWndCtrl_Display.addIconicVar(ho_Image);
                        hWndCtrl_Display.resetAll();
                        hWndCtrl_Display.repaint();
                    }


                    //SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);


                }
                catch (Exception)
                {
                }
            }
        }

        void RunFunction()
        {

        }

      
        /// <summary>
        /// 重绘指引
        /// </summary>
        void ShowResult()
        {
            try
            {
                messageShowInfos_Temp.Clear(); ;
                foreach(var item in regionShowInfos_Temp)
                {
                    item.m_hRegion?.Dispose();
                }
                regionShowInfos_Temp.Clear();

                HOperatorSet.GetWindowExtents(HWC_Display.HalconWindow, out HTuple Row, out HTuple Col, out HTuple width, out HTuple height);
                int count = 0;
                int FontLine = 0;
                double colPos = 0;
                foreach (var item in messageShowInfos)
                {
                    if (item.m_bIsWindow)
                    {
                        if (count > height / 20)
                        {
                            count = 0;
                            colPos = width / 2;
                            FontLine = 0;
                        }
                        HOperatorSet.SetColor(HWC_Display.HalconWindow, item.m_stuGetColor.ToString());
                        
                        HOperatorSet.DispText(HWC_Display.HalconWindow, item.m_strMessage, "window", FontLine, colPos, item.m_stuGetColor.ToString(), "box", "false");

                        FontLine += item.m_nFontSize + 10;
                        count++;
                    }
                    else
                    {
                        HOperatorSet.SetColor(HWC_Display.HalconWindow, item.m_stuGetColor.ToString());
                        HOperatorSet.DispText(HWC_Display.HalconWindow, item.m_strMessage, "image", item.m_fRow, item.m_fCol, item.m_stuGetColor.ToString(), "box", "false");
                    }

                }
           
                    foreach (var item in regionShowInfos)
                    {
                        try
                        {

                            if ((item.m_hRegion?.CountObj() != null && item.m_hRegion?.CountObj() != 0))
                            {
                                HOperatorSet.SetColor(HWC_Display.HalconWindow, item.m_stuGetColor.ToString());
                                HOperatorSet.SetDraw(HWC_Display.HalconWindow, "margin");
                                HOperatorSet.DispObj(item.m_hRegion, HWC_Display.HalconWindow);
                            }

                        }
                        catch (Exception)
                        {
                           // int index = operation_ImageInfo.showHalconWindow.m_regionShowInfos.IndexOf(item);
                        }

                    }


                messageShowInfos_Temp=new List<MessageShowInfo>(messageShowInfos);
                regionShowInfos_Temp = new List<RegionShowInfo>(regionShowInfos); 
                messageShowInfos.Clear();
                regionShowInfos.Clear();

            }
            catch (Exception)
            {

               // throw;
            }
        }

        void Initialcontrol(ROI rOI)
        {
            switch(rOI.roiParams.rOItype)
            {
                case ROIType.矩形1:
                    if(cobSelect3D.SelectedIndex==1)
                    {

                        rOI.ModifyDicValue("矩形平移Col", new System.Windows.Forms.TextBox() { Text = "0" });
                        rOI.ModifyDicValue("矩形平移row", new System.Windows.Forms.TextBox() { Text = "0" });
                        System.Windows.Forms.ComboBox comboBox5= new System.Windows.Forms.ComboBox();
                        comboBox5.FormattingEnabled = true;
                        comboBox5.Items.AddRange(new object[] { "regression", "huber", "tukey" });
                        comboBox5.SelectedIndex = 0;
                        rOI.ModifyDicValue("拟合算法", comboBox5);
                        rOI.ModifyDicValue("数据补偿B", new System.Windows.Forms.TextBox() { Text = "0" });
                        rOI.ModifyDicValue("最大迭代次数", new System.Windows.Forms.TextBox() { Text = "55" });
                        rOI.ModifyDicValue("裁剪因子", new System.Windows.Forms.TextBox() { Text = "15" });
                        rOI.ModifyDicValue("矫正因子", new System.Windows.Forms.TextBox() { Text = "1" });
                        rOI.ModifyDicValue("修正值", new System.Windows.Forms.TextBox() { Text = "0" });
                        System.Windows.Forms.ComboBox comboBox6 = new System.Windows.Forms.ComboBox();
                        comboBox6.FormattingEnabled = true;
                        comboBox6.Items.AddRange(new object[] { "byte", "real", "uint2" });
                        comboBox6.SelectedIndex = 0;
                        rOI.ModifyDicValue("像素类型", comboBox6);
                        rOI.ModifyDicValue("启用数据校准", new System.Windows.Forms.CheckBox() { Checked = false });
                        rOI.ModifyDicValue("基准值", new System.Windows.Forms.TextBox() { Text = "0" });
                        rOI.ModifyDicValue("基准公差", new System.Windows.Forms.TextBox() { Text = "0" });
                        rOI.ModifyDicValue("校准公差", new System.Windows.Forms.TextBox() { Text = "0" });
                        rOI.ModifyDicValue("是否启用滤波", new System.Windows.Forms.CheckBox() { Checked = true });
                        rOI.ModifyDicValue("滤除高点百分比", new System.Windows.Forms.TextBox() { Text = "0.1" });
                        rOI.ModifyDicValue("滤除低点百分比", new System.Windows.Forms.TextBox() { Text = "0.1" });
                    }
                    else
                    {
                        rOI.ModifyDicValue("圆最小阈值", new System.Windows.Forms.TextBox() { Text = "0" });
                        rOI.ModifyDicValue("圆最大阈值", new System.Windows.Forms.TextBox() { Text = "20" });
                        rOI.ModifyDicValue("填充系数", new System.Windows.Forms.TextBox() { Text = "2.5" });
                        rOI.ModifyDicValue("比例尺Y", new System.Windows.Forms.TextBox() { Text = "0.5" });
                        rOI.ModifyDicValue("比例尺X", new System.Windows.Forms.TextBox() { Text = "0.5" });
                        rOI.ModifyDicValue("最小圆度", new System.Windows.Forms.TextBox() { Text = "0.5" });
                        rOI.ModifyDicValue("最大圆度", new System.Windows.Forms.TextBox() { Text = "1.0" });
                    }
         
                    break;
                case ROIType.矩形2:
                    rOI.ModifyDicValue("点数", new System.Windows.Forms.TextBox() { Text = "15" });
                    rOI.ModifyDicValue("抓边阈值", new System.Windows.Forms.TextBox() { Text = "20" });
                    rOI.ModifyDicValue("抓边Sigma", new System.Windows.Forms.TextBox() { Text = "2.00" });
                    System.Windows.Forms.ComboBox comboBox1 = new System.Windows.Forms.ComboBox();
                    comboBox1.FormattingEnabled = true;
                    comboBox1.Items.AddRange(new object[] { "第一个点", "最后一点", "所有" });
                    comboBox1.SelectedIndex = 0;
                    rOI.ModifyDicValue("取点方式", comboBox1);
                    System.Windows.Forms.ComboBox comboBox2 = new System.Windows.Forms.ComboBox();
                    comboBox2.FormattingEnabled = true;
                    comboBox2.Items.AddRange(new object[] { "暗->明", "明->暗", "所有" });
                    comboBox2.SelectedIndex = 0;
                    rOI.ModifyDicValue("边缘类型", comboBox2);
                    break;
                case ROIType.圆:
               
                    break;
                case ROIType.线:

                    rOI.ModifyDicValue("抓边阈值", new System.Windows.Forms.TextBox() { Text = "20" });
                    rOI.ModifyDicValue("抓边Sigma", new System.Windows.Forms.TextBox() { Text = "2.00" });
                    rOI.ModifyDicValue("比例尺", new System.Windows.Forms.TextBox() { Text = "0.5" });
                    System.Windows.Forms.ComboBox comboBoxTransitionType = new System.Windows.Forms.ComboBox();
                    comboBoxTransitionType.Items.AddRange(new object[] { "all", "position", "negative" /*,"all_strongest", "position_strongest" , "negative_strongest"*/});
                    comboBoxTransitionType.SelectedIndex = 0;
                    rOI.ModifyDicValue("变换Trans", comboBoxTransitionType);
                    System.Windows.Forms.ComboBox comboBoxEdgePostion = new System.Windows.Forms.ComboBox();
                    comboBoxEdgePostion.Items.AddRange(new object[] { "all", "first", "last"});
                    comboBoxEdgePostion.SelectedIndex = 0;
                    rOI.ModifyDicValue("位置Pos", comboBoxEdgePostion);

                    System.Windows.Forms.ComboBox comboBoxInterMethod = new System.Windows.Forms.ComboBox();
                    comboBoxInterMethod.Items.AddRange(new object[] { "nearest_neighbor", "bilinear", "bicubic" });
                    comboBoxInterMethod.SelectedIndex = 0;
                    rOI.ModifyDicValue("插值方法", comboBoxInterMethod);

                    rOI.ModifyDicValue("边缘对", new System.Windows.Forms.CheckBox() {Checked=false });



                    break;
            }
        }



        void measureMethod(ROI rOI)
        {
            if(ho_Image!=null)
            {
                HTuple t1, t2;
                HOperatorSet.CountSeconds(out t1);
                HTuple MetrologyHandle = new HTuple();
                HOperatorSet.GetImageSize(ho_Image, out HTuple width, out HTuple height);
                HTuple[] outputvalue = new HTuple[] { };
                //HObject outputobj = new OutPutObj();
                HObject ho_Contours = null;
                HOperatorSet.GenEmptyObj(out ho_Contours);
                ROIParams rOIParams = rOI.roiParams;
                switch (rOI.roiParams.rOItype)
                {
                    case ROIType.圆:
                       
                        break;
                    case ROIType.矩形1:
                        if(cobSelect3D.SelectedIndex==1)
                        {
                            try
                            {
                                int offsetCol = Convert.ToInt32(rOI.GetDicValue("矩形平移Col"));
                                int offsetRow = Convert.ToInt32(rOI.GetDicValue("矩形平移row"));
                                double z = Convert.ToDouble(rOI.GetDicValue("数据补偿B"));
                                int hv_Iterations = Convert.ToInt32(rOI.GetDicValue("最大迭代次数"));
                                int hv_ClippingFactor = Convert.ToInt32(rOI.GetDicValue("裁剪因子"));
                                double hv_Mult = Convert.ToDouble(rOI.GetDicValue("矫正因子"));
                                double hv_Add = Convert.ToDouble(rOI.GetDicValue("修正值"));

                                string hv_Algorithm = Convert.ToString(rOI.GetDicValue2("拟合算法"));
                                string hv_ImageType = Convert.ToString(rOI.GetDicValue2("像素类型"));
                                bool b_DataCalibration = Convert.ToBoolean(rOI.GetDicValue("启用数据校准"));
                                bool b_DataFilter = Convert.ToBoolean(rOI.GetDicValue("是否启用滤波"));
                                double d_BaseValue = Convert.ToDouble(rOI.GetDicValue("基准值"));
                                double d_BaseTolerance = Convert.ToDouble(rOI.GetDicValue("基准公差"));
                                double d_CalibrationTolerance = Convert.ToDouble(rOI.GetDicValue("校准公差"));
                                double d_FilterLowPercent = Convert.ToDouble(rOI.GetDicValue("滤除低点百分比"));
                                double d_FilterHighPercent = Convert.ToDouble(rOI.GetDicValue("滤除高点百分比"));


                                HOperatorSet.GenRectangle1(out HObject rectangle, rOIParams.row1, rOIParams.col1, rOIParams.row2, rOIParams.col2);
                                HOperatorSet.MoveRegion(rectangle, out HObject rectanglePlane, offsetRow, offsetCol);
                                // regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangle, m_stuGetColor = HalconColor.orange });
                                regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectanglePlane, m_stuGetColor = HalconColor.orange });
                                HOperatorSet.GetRegionPoints(rectangle, out HTuple Rows, out HTuple Columns);
                                HOperatorSet.GetRegionPoints(rectanglePlane, out HTuple PlaneRows, out HTuple PlaneColumns);


                                HOperatorSet.FitSurfaceFirstOrder(rectanglePlane, ho_Image, hv_Algorithm,
                                hv_Iterations, hv_ClippingFactor, out HTuple alpha, out HTuple beta, out HTuple gamma);
                                HOperatorSet.AreaCenter(rectanglePlane, out HTuple area, out HTuple row, out HTuple column);
                                HOperatorSet.GenImageSurfaceFirstOrder(out HObject imageSurface, hv_ImageType, alpha, beta, gamma, row, column, width, height);
                                HOperatorSet.SubImage(ho_Image, imageSurface, out HObject imageSub, hv_Mult, hv_Add);
                                HOperatorSet.GetGrayval(imageSub, Rows, Columns, out HTuple grayvalDiff);

                                imageSurface?.Dispose();
                                imageSub?.Dispose();
                                outputvalue = new HTuple[4];
                                if (grayvalDiff?.Length > 0)
                                {

                                    //判断是否启用滤波
                                    if (b_DataFilter)
                                    {
                                        OtherAlgorithm.data_filter_output_index(grayvalDiff, d_FilterLowPercent, d_FilterHighPercent, out grayvalDiff, out HTuple dataFilterIndex);
                                    }

                                    HTuple point_Z_diff = (-grayvalDiff).ToDArr();
                                    HOperatorSet.TupleMean(point_Z_diff, out HTuple mean);
                                    HOperatorSet.TupleMax(point_Z_diff, out HTuple max);
                                    HOperatorSet.TupleMin(point_Z_diff, out HTuple min);
                                    //校准数据
                                    double meanvalue = mean.D;
                                    OtherAlgorithm.Calibration(b_DataCalibration, d_BaseValue, d_BaseTolerance, d_CalibrationTolerance, ref meanvalue);
                                    mean = new HTuple(meanvalue);
                                    outputvalue[0] = mean * 1000;
                                    outputvalue[1] = max * 1000;
                                    outputvalue[2] = min * 1000;
                                    outputvalue[3] = ((mean * 1000).TupleAbs() + z).TupleAbs();
                                    messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"帧号：{m_ImageSimManage_Cam.currentImageIndex}", m_stuGetColor = HalconColor.green });
                                    messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"平均值：{string.Join(",", (mean * 1000).DArr.ToList().ConvertAll(x => x.ToString("0.00")))}", m_stuGetColor = HalconColor.green });
                                    messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"最大值：{string.Join(",", (max * 1000).DArr.ToList().ConvertAll(x => x.ToString("0.00")))}", m_stuGetColor = HalconColor.green });
                                    messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"最小值：{string.Join(",", (min * 1000).DArr.ToList().ConvertAll(x => x.ToString("0.00")))}", m_stuGetColor = HalconColor.green });
                                    messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"{string.Join(",", ((mean * 1000).TupleAbs() + z).TupleAbs().DArr.ToList().ConvertAll(x => x.ToString("0.00")))}um", m_stuGetColor = HalconColor.green, m_bIsWindow = false, m_fRow = (rOIParams.row1 + rOIParams.row2) / 2, m_fCol = (rOIParams.col1 + rOIParams.col2) / 2 });
                                    //regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Contours, m_stuGetColor = HalconColor.orange });
                                }
                                else
                                {
                                    messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"帧号：{m_ImageSimManage_Cam.currentImageIndex}_未检测到矩形", m_stuGetColor = HalconColor.green });
                                    // HOperatorSet.DispText(HWC_Display.HalconWindow, $"帧号：{m_ImageSimManage_Cam.currentImageIndex}\n未检测到矩形", "window", 10, 0, HalconColor.green.ToString(), "box", "false");
                                }
                                rectangle?.Dispose();







                            }
                            catch (Exception)
                            {

                                // throw;
                            }
                            finally
                            {
                                rOI.OutRoiResult = outputvalue;
                            }
                        }
                        else
                        {
                            try
                            {
                                HOperatorSet.GenRectangle1(out HObject rectangle, rOIParams.row1, rOIParams.col1, rOIParams.row2, rOIParams.col2);
                                HOperatorSet.DispObj(rectangle, hWndCtrl_Display.GetHWindow());
                                regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangle, m_stuGetColor = HalconColor.orange });
                                HOperatorSet.ReduceDomain(ho_Image, rectangle, out HObject ho_Image_Reduced);
                                int mingray = Convert.ToInt32(rOI.GetDicValue("圆最小阈值"));
                                int maxgray = Convert.ToInt32(rOI.GetDicValue("圆最大阈值"));
                                HOperatorSet.Threshold(ho_Image_Reduced, out ho_Image_Reduced, mingray, maxgray);

                                HOperatorSet.FillUp(ho_Image_Reduced, out ho_Image_Reduced);
                                double raduspix = Convert.ToDouble(rOI.GetDicValue("填充系数"));
                                HOperatorSet.ClosingCircle(ho_Image_Reduced, out ho_Image_Reduced, raduspix);
                                HOperatorSet.Connection(ho_Image_Reduced, out ho_Image_Reduced);
                                double circclusionmin = Convert.ToDouble(rOI.GetDicValue("最小圆度"));
                                double circclusionmax = Convert.ToDouble(rOI.GetDicValue("最大圆度"));

                                HOperatorSet.SelectShape(ho_Image_Reduced, out ho_Image_Reduced, "circularity",
                                 "and", circclusionmin, circclusionmax);
                                HOperatorSet.AreaCenter(ho_Image_Reduced, out HTuple area, out HTuple centerY1, out HTuple centerX1);
                                double ratioX = Convert.ToDouble(rOI.GetDicValue("比例尺X"));
                                double ratioY = Convert.ToDouble(rOI.GetDicValue("比例尺Y"));

                                HOperatorSet.SmallestCircle(ho_Image_Reduced, out HTuple centerX, out HTuple centerY, out HTuple radius);
                                //HOperatorSet.GenCircle(out ho_Image_Reduced, centerX, centerY, radius);
                                HOperatorSet.SetColor(HWC_Display.HalconWindow, HalconColor.orange.ToString());
                                //HTuple hTuple = new HTuple();
                                outputvalue = new HTuple[6];

                                if (centerX?.Length > 0)
                                {


                                    HTuple area2 = area * ratioX * ratioY;
                                    outputvalue[0] = (centerX);
                                    outputvalue[1] = (centerY);
                                    outputvalue[2] = (radius);
                                    outputvalue[3] = (radius * ratioY);
                                    outputvalue[4] = (radius * ratioY * 2);
                                    outputvalue[5] = (area2);

                                    messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"帧号：{m_ImageSimManage_Cam.currentImageIndex}", m_stuGetColor = HalconColor.green });
                                    messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"坐标X：{string.Join(",", centerX.DArr.ToList().ConvertAll(x => x.ToString("0.00")))}", m_stuGetColor = HalconColor.green });
                                    messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"坐标Y：{string.Join(",", centerY.DArr.ToList().ConvertAll(x => x.ToString("0.00")))}", m_stuGetColor = HalconColor.green });
                                    messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"半径(pixel)：{string.Join(",", radius.DArr.ToList().ConvertAll(x => x.ToString("0.00")))}", m_stuGetColor = HalconColor.green });
                                    messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"半径/mm：{string.Join(",", ((radius * ratioY)).DArr.ToList().ConvertAll(x => x.ToString("0.00")))}", m_stuGetColor = HalconColor.green });
                                    messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"直径/mm：{string.Join(",", ((radius * ratioY) * 2).DArr.ToList().ConvertAll(x => x.ToString("0.00")))}", m_stuGetColor = HalconColor.green });
                                    messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"面积/mm：{string.Join(",", (area2).DArr.ToList().ConvertAll(x => x.ToString("0.00")))}", m_stuGetColor = HalconColor.green });

                                    regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Image_Reduced, m_stuGetColor = HalconColor.orange });

                                    //HOperatorSet.DispText(HWC_Display.HalconWindow, $"帧号：{m_ImageSimManage_Cam.currentImageIndex}\n坐标X：{string.Join(",", centerX.DArr.ToList().ConvertAll(x => x.ToString("0.00")))}\n" +
                                    //    $"坐标Y：{string.Join(",", centerY.DArr.ToList().ConvertAll(x => x.ToString("0.00")))}\n半径(pixel)：{str}\n半径/mm：{string.Join(",", ((radius * ratioY)).DArr.ToList().ConvertAll(x => x.ToString("0.00")))}\n直径/mm：{string.Join(",", ((radius * ratioY)*2).DArr.ToList().ConvertAll(x => x.ToString("0.00")))}" +
                                    //    $"\n面积/mm：{string.Join(",", (area2).DArr.ToList().ConvertAll(x => x.ToString("0.00")))}", "window", 10, 0, HalconColor.green.ToString(), "box", "false");
                                }
                                else
                                {
                                    messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"帧号：{m_ImageSimManage_Cam.currentImageIndex}_未检测到圆", m_stuGetColor = HalconColor.green });
                                    // HOperatorSet.DispText(HWC_Display.HalconWindow, $"帧号：{m_ImageSimManage_Cam.currentImageIndex}\n未检测到圆", "window", 10, 0, HalconColor.green.ToString(), "box", "false");
                                }
                                rectangle?.Dispose();
                            }
                            catch (Exception)
                            {

                                // throw;
                            }
                            finally
                            {
                                rOI.OutRoiResult = outputvalue;
                            }
                        }
                        
                       
                        break;
                    case ROIType.矩形2:
                        try
                        {
                           
                            //创建测量句柄
                            HOperatorSet.CreateMetrologyModel(out MetrologyHandle);
                            //设置模型大小
                            HOperatorSet.SetMetrologyModelImageSize(MetrologyHandle, width, height);
                            //添加测量模型
                            HTuple hv_MeasureRow1 = rOIParams.midR - rOIParams.length1 * Math.Sin(rOIParams.phi);
                            HTuple hv_MeasureColumn1 = rOIParams.midC - rOIParams.length1 * Math.Cos(rOIParams.phi);
                            HTuple hv_MeasureRow2 = rOIParams.midR + rOIParams.length1 * Math.Sin(rOIParams.phi);
                            HTuple hv_MeasureColumn2 = rOIParams.midC + rOIParams.length1 * Math.Cos(rOIParams.phi);
                            double Sigma = Convert.ToDouble(rOI.GetDicValue("抓边Sigma"));
                            int Threshold = Convert.ToInt32(rOI.GetDicValue("抓边阈值"));
                            int PointNum = Convert.ToInt32(rOI.GetDicValue("点数"));
                            rOIParams.pointnum = PointNum;
                            HTuple hv_ParamName = new HTuple("measure_select", "measure_transition", "num_measures", "num_instances");
                            string Position = ExtensionsObject.GetEnumValueAtIndex<Position>(Convert.ToInt32(rOI.GetDicValue("取点方式"))).ToString();
                            string Transition = ExtensionsObject.GetEnumValueAtIndex<Transition>(Convert.ToInt32(rOI.GetDicValue("边缘类型"))).ToString();
                            HTuple hv_ParamValue = new HTuple(Position, Transition, PointNum, 1);
                            HOperatorSet.AddMetrologyObjectLineMeasure(MetrologyHandle, hv_MeasureRow1, hv_MeasureColumn1, hv_MeasureRow2, hv_MeasureColumn2,
                            rOIParams.length2, rOIParams.length1 / PointNum, Sigma, Threshold, hv_ParamName, hv_ParamValue, out HTuple index);
                           
                            
                            ////执行测量
                            HOperatorSet.ApplyMetrologyModel(ho_Image, MetrologyHandle);
                            //获取结果 get_metrology_object_result_contour()
                            HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, MetrologyHandle, "all", "all", out HTuple hv_CrossRow, out HTuple hv_CrossColumn);
                            HOperatorSet.SetColor(hWndCtrl_Display.GetHWindow(), "red");
                            HOperatorSet.GetMetrologyObjectResult(MetrologyHandle, "all", "all", "result_type", "row_begin", out HTuple hv_LineRowBegin);
                            HTuple hTuple = new HTuple();
                            outputvalue=new HTuple[] { hTuple };
                            outputvalue[0] = outputvalue[0].TupleConcat(hv_LineRowBegin);
                            HOperatorSet.GetMetrologyObjectResult(MetrologyHandle, "all", "all", "result_type", "column_begin", out HTuple hv_LineColumnBegin);
                            outputvalue[0] = outputvalue[0].TupleConcat(hv_LineColumnBegin);
                            HOperatorSet.GetMetrologyObjectResult(MetrologyHandle, "all", "all", "result_type", "row_end", out HTuple hv_LineRowEnd);
                            outputvalue[0] = outputvalue[0].TupleConcat(hv_LineRowEnd);
                            HOperatorSet.GetMetrologyObjectResult(MetrologyHandle, "all", "all", "result_type", "column_end", out HTuple hv_LineColumnEnd);
                            outputvalue[0] = outputvalue[0].TupleConcat(hv_LineColumnEnd);
                            if (outputvalue[0].Length == 4)
                            {
                                HOperatorSet.DispLine(hWndCtrl_Display.GetHWindow(), hv_LineRowBegin, hv_LineColumnBegin, hv_LineRowEnd, hv_LineColumnEnd);
                            }
                            HOperatorSet.DispCross(hWndCtrl_Display.GetHWindow(), hv_CrossRow, hv_CrossColumn, 50, 0.785);
                            HOperatorSet.GetMetrologyObjectResultContour(out ho_Contours, MetrologyHandle, "all", "all", 1.5);
                            HOperatorSet.GenRegionContourXld(ho_Contours, out ho_Contours, "filled");
                            regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Contours, m_stuGetColor = HalconColor.orange });
                           // HOperatorSet.DispObj(ho_Contours, hWndCtrl_Display.GetHWindow());
                        }
                        catch (Exception)
                        {
                           
                            //LogName.ErrorLog.Info(ex.Message);
                            //log.Error(ex.Message);
                        }
                        finally
                        {
                            HOperatorSet.ClearMetrologyModel(MetrologyHandle);
                            HOperatorSet.CountSeconds(out t2);
                            rOI.OutRoiObj = ho_Contours;
                            rOI.OutRoiResult = outputvalue;
                  
                        }
                        break;
                    case ROIType.线:
                        try
                        {
                            HTuple hv_AmplitudeThreshold = Convert.ToInt32(rOI.GetDicValue("抓边阈值"));
                            HTuple hv_RoiWidthLen2 = rOIParams.length2, hv_LineRowStart_Measure_01_0 = rOIParams.row1;
                            HTuple hv_LineColumnStart_Measure_01_0 = rOIParams.col1;
                            HTuple hv_LineRowEnd_Measure_01_0 = rOIParams.row2, hv_LineColumnEnd_Measure_01_0 = rOIParams.col2;
                            HTuple hv_TmpCtrl_Row = 0.5 * (hv_LineRowStart_Measure_01_0 + hv_LineRowEnd_Measure_01_0), hv_TmpCtrl_Column = 0.5 * (hv_LineColumnStart_Measure_01_0 + hv_LineColumnEnd_Measure_01_0);
                            HTuple hv_TmpCtrl_Dr = hv_LineRowStart_Measure_01_0 - hv_LineRowEnd_Measure_01_0, hv_TmpCtrl_Dc = hv_LineColumnEnd_Measure_01_0 - hv_LineColumnStart_Measure_01_0;
                            HTuple hv_TmpCtrl_Phi = hv_TmpCtrl_Dr.TupleAtan2(hv_TmpCtrl_Dc), hv_TmpCtrl_Len1 = 0.5 * ((((hv_TmpCtrl_Dr * hv_TmpCtrl_Dr) + (hv_TmpCtrl_Dc * hv_TmpCtrl_Dc))).TupleSqrt()
            );
                            HTuple hv_TmpCtrl_Len2 = new HTuple(hv_RoiWidthLen2), hv_MsrHandle_Measure_01_0 = new HTuple();
                            HTuple hv_Row_Measure_01_0 = new HTuple(), hv_Column_Measure_01_0 = new HTuple();
                            HTuple hv_Amplitude_Measure_01_0 = new HTuple();

                            HTuple hv_Distance_Measure_01_0 = new HTuple();

                            hv_MsrHandle_Measure_01_0.Dispose();
                            HOperatorSet.GenMeasureRectangle2(hv_TmpCtrl_Row, hv_TmpCtrl_Column, hv_TmpCtrl_Phi,
                                hv_TmpCtrl_Len1, hv_TmpCtrl_Len2, width, height, rOI.GetDicValue2("插值方法").ToString(), out hv_MsrHandle_Measure_01_0);

                            hv_Column_Measure_01_0.Dispose(); hv_Amplitude_Measure_01_0.Dispose(); hv_Distance_Measure_01_0.Dispose();

                            HOperatorSet.MeasurePos(ho_Image, hv_MsrHandle_Measure_01_0, 1, hv_AmplitudeThreshold, rOI.GetDicValue2("变换Trans").ToString(), rOI.GetDicValue2("位置Pos").ToString(), out hv_Row_Measure_01_0, out hv_Column_Measure_01_0, out hv_Amplitude_Measure_01_0, out hv_Distance_Measure_01_0);
                            HOperatorSet.DispCross(hWndCtrl_Display.GetHWindow(), hv_Row_Measure_01_0, hv_Column_Measure_01_0, 50, 0.785);
                            double scale = Convert.ToDouble(rOI.GetDicValue("比例尺"));
                            outputvalue = new HTuple[5] ;
                            outputvalue[0]= hv_Row_Measure_01_0;
                            outputvalue[1] = hv_Column_Measure_01_0;
                            outputvalue[2] = hv_Amplitude_Measure_01_0;
                            outputvalue[3] = hv_Distance_Measure_01_0* scale;
                            //线段总长度
                            if (outputvalue[0]?.Length!=null || outputvalue[0]?.Length!=0)
                            {
                                HOperatorSet.DistancePp(outputvalue[0][0], outputvalue[1][0], outputvalue[0][hv_Row_Measure_01_0.Length-1], outputvalue[1][hv_Column_Measure_01_0.Length - 1], out HTuple TitleLength);
                                outputvalue[4] = TitleLength * scale;
                               // HOperatorSet.DispText(HWC_Display.HalconWindow, $"帧号：{m_ImageSimManage_Cam.currentImageIndex}\n总长度：{outputvalue[4]}", "window", 10, 0, HalconColor.green.ToString(), "box", "false");
                                messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"帧号：{m_ImageSimManage_Cam.currentImageIndex}_总长度：{outputvalue[4]}", m_stuGetColor = HalconColor.green });
                            }
                            else
                            {
                                //HOperatorSet.DispText(HWC_Display.HalconWindow, $"帧号：{m_ImageSimManage_Cam.currentImageIndex}", "window", 10, 0, HalconColor.green.ToString(), "box", "false");
                                 messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"帧号：{m_ImageSimManage_Cam.currentImageIndex}", m_stuGetColor = HalconColor.green });
                            }

                        }
                        catch (Exception)
                        {

                            //throw;
                        }
                        finally
                        {
                            rOI.OutRoiResult = outputvalue;
                        }
                   

                         

                        break;




                }
            }
         

            //HTuple t1, t2;
            //HOperatorSet.CountSeconds(out t1);
            //HTuple MetrologyHandle = new HTuple();
            //HOperatorSet.GetImageSize(ho_Image, out HTuple width, out HTuple height);
            //HTuple outputvalue = new HTuple();
            ////HObject outputobj = new OutPutObj();
            //HObject ho_Contours = null;
            //HOperatorSet.GenEmptyObj(out ho_Contours);
           
        }

        private void ImageReset_Click(object sender, EventArgs e)
        {
            try
            {
                //重置显示画面
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();
                //
            }
            catch (Exception)
            {
            }
        }

        private void HWC_Display_HMouseMove(object sender, HMouseEventArgs e)
        {
            if (hWndCtrl_Display.ho_Image_Changed != null & roiController.activeROIidx == -1)
                try
                {
                    TB_PositionAndGray.Text = hWndCtrl_Display.Get_Mouseposition_and_Gray(HWC_Display.HalconWindow, hWndCtrl_Display.ho_Image_Changed, e);
                    LB_IMageSize.Text = ShowIMageSize(hWndCtrl_Display.ho_Image_Changed);
                }
                catch (Exception)
                {

                }
        }

        private string ShowIMageSize(HObject image)
        {
            string str = "Size  R:—   C:—";
            try
            {
                HTuple w, h;
                HOperatorSet.GetImageSize(image, out w, out h);
                str = String.Format("Size  R:{0}   C:{1}", h.ToString(), w.ToString()); //格式化字符串
                return str;
            }
            catch (Exception)
            {
                return str;
            }
        }

        private void 画矩形2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWndCtrl_Display.AddRectangle2();
        }

        private void 画圆ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWndCtrl_Display.AddCircle();
        }

        private void 画矩形1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWndCtrl_Display.AddRectangle1();
        }

        private void 画线ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWndCtrl_Display.AddLine();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //保存形状参数
            hWndCtrl_Display.ROIController.SaveROI();
            //保存测量参数
            hWndCtrl_Display.ROIController.SaveROIMeasureDics();
        }

        private void 删除指定区域ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWndCtrl_Display.OnRoiDeleting=true;
        }


        private void Btn_CreateData_Click(object sender, EventArgs e)
        {
            try
            {

                List<string> stringTitles = new List<string>();
                stringTitles.Add("时间");
                stringTitles.Add("帧号");
                stringTitles.Add("ROI类型");
                foreach (ROI roi in hWndCtrl_Display.ROIController.ROIList)
                {
                    string strCam = cbx_CamSelect.Text;


                    List<string> stringData = new List<string>();

                    if (roi.OutRoiResult?.Length > 0)
                    {
                        for (int i = 0; i < roi.OutRoiResult?.Length; i++)
                        {

                            stringData.Clear();
                            if (roi.OutRoiResult[i]?.Length > 0)
                            {
                                stringData = roi.OutRoiResult[i].DArr.ToList().ConvertAll(x => x.ToString()).ToList();

                                stringData.Insert(0, roi.roiParams.rOItype.ToString());
                                stringData.Insert(0, m_ImageSimManage_Cam.currentImageIndex.ToString());
                                //当前时间精确到毫秒写入到第一个
                                stringData.Insert(0, DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"));

                                FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.ProductMojuanPath + strCam + "MSA棋盘格线宽数据", stringTitles.ToArray(), stringData.ToArray(), "");
                                Thread.Sleep(100);
                            }

                        }
                        stringData.Clear();
                        stringData.Add("\n");
                        FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.ProductMojuanPath + strCam + "MSA棋盘格线宽数据", stringTitles.ToArray(), stringData.ToArray(), "");


                    }
                }




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            MessageBox.Show("数据保存成功！");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(Project.Instance.paramPathSet.ProductMojuanPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void 清除所有区域ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWndCtrl_Display.ROIController.reset();
            hWndCtrl_Display.repaint();
        }

        private void btn_Run_Click(object sender, EventArgs e)
        {
            try
            {
               if (m_ImageSimManage_Cam.ImageEmulationPaths == null || m_ImageSimManage_Cam.ImageEmulationPaths.Length == 0) return;
                ho_Image?.Dispose();
                m_ImageSimManage_Cam.GetCurrentImage(out ho_Image, out string imgPath);
                if (ho_Image != null && ho_Image?.CountObj() > 0)
                {
                    ho_Image = new HImage(ho_Image);
                    hWndCtrl_Display.addIconicVar(ho_Image);
                    hWndCtrl_Display.resetAll();
                    hWndCtrl_Display.repaint();
                }
                List<string> stringTitles = new List<string>();
                stringTitles.Add("时间");
                stringTitles.Add("帧号");
                stringTitles.Add("ROI类型");
                foreach (ROI roi in hWndCtrl_Display.ROIController.ROIList)
                {
                    string strCam = cbx_CamSelect.Text;


                    List<string> stringData = new List<string>();

                    if (roi.OutRoiResult?.Length > 0)
                    {
                        for (int i = 0; i < roi.OutRoiResult?.Length; i++)
                        {

                            stringData.Clear();
                            if (roi.OutRoiResult[i]?.Length > 0)
                            {
                                stringData = roi.OutRoiResult[i].DArr.ToList().ConvertAll(x => x.ToString()).ToList();

                                stringData.Insert(0, roi.roiParams.rOItype.ToString());
                                stringData.Insert(0, m_ImageSimManage_Cam.currentImageIndex.ToString());
                                //当前时间精确到毫秒写入到第一个
                                stringData.Insert(0, DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"));

                                FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.ProductMojuanPath + strCam + "MSA棋盘格线宽数据", stringTitles.ToArray(), stringData.ToArray(), "");
                                Thread.Sleep(100);
                            }

                        }
                        stringData.Clear();
                        stringData.Add("\n");
                        FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.ProductMojuanPath + strCam + "MSA棋盘格线宽数据", stringTitles.ToArray(), stringData.ToArray(), "");


                    }
                }

            }
            catch (Exception)
            {

               // throw;
            }
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ImageSimManage_Cam.ImageEmulationPaths == null || m_ImageSimManage_Cam.ImageEmulationPaths.Length == 0) return;
                ho_Image?.Dispose();
                m_ImageSimManage_Cam.GetNextImage(out ho_Image, out string imgPath);
                if (ho_Image != null && ho_Image?.CountObj() > 0)
                {
                    ho_Image = new HImage(ho_Image);
                    hWndCtrl_Display.addIconicVar(ho_Image);
                    hWndCtrl_Display.resetAll();
                    hWndCtrl_Display.repaint();
                }
                List<string> stringTitles = new List<string>();
                stringTitles.Add("时间");
                stringTitles.Add("帧号");
                stringTitles.Add("ROI类型");
                foreach (ROI roi in hWndCtrl_Display.ROIController.ROIList)
                {
                    string strCam = cbx_CamSelect.Text;

                   
                    List<string> stringData = new List<string>();
      
                    if (roi.OutRoiResult?.Length > 0)
                    {
                        for (int i = 0; i < roi.OutRoiResult?.Length; i++)
                        {
                    
                            stringData.Clear();
                            if (roi.OutRoiResult[i]?.Length > 0)
                            {
                                stringData = roi.OutRoiResult[i].DArr.ToList().ConvertAll(x => x.ToString()).ToList();
                          
                                stringData.Insert(0, roi.roiParams.rOItype.ToString());
                                stringData.Insert(0, m_ImageSimManage_Cam.currentImageIndex.ToString());
                                //当前时间精确到毫秒写入到第一个
                                stringData.Insert(0, DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"));
                             
                                FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.ProductMojuanPath + strCam + "MSA棋盘格线宽数据", stringTitles.ToArray(), stringData.ToArray(), "");
                                Thread.Sleep(100);
                            }

                        }
                        stringData.Clear();
                        stringData.Add("\n");
                        FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.ProductMojuanPath + strCam + "MSA棋盘格线宽数据", stringTitles.ToArray(), stringData.ToArray(), "");


                    }
                }
            }
            catch (Exception)
            {

                //throw;
            }
    
        }
    }
}
