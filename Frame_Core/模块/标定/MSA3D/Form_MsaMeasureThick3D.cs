using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Extensions;

using HalconDotNet;

using IKapBoardClassLibrary;

using NPOI.OpenXmlFormats.Vml;
using NPOI.SS.Formula.Functions;

using ViewROI3;

namespace Frame_Core
{
    public partial class Form_MsaMeasureThick3D : Form
    {

        public HWndCtrl hWndCtrl_Display;
        public ROIController roiController;
        public ROIRectangle2 roiRectangle;
        Image3DClass image3DClass = new Image3DClass();
        ROIFormBase ROIForm = new ROIFormBase();
        Dictionary<CameraEnum, CameraManager> cameraManager;
        public Param_MsaThick3D  param_MsaThick3D;
        List<MessageShowInfo> messageShowInfos = new List<MessageShowInfo>();
        List<RegionShowInfo> regionShowInfos = new List<RegionShowInfo>();
        List<MessageShowInfo> messageShowInfos_Temp = new List<MessageShowInfo>();
        List<RegionShowInfo> regionShowInfos_Temp = new List<RegionShowInfo>();
        /// <summary>
        /// 图片仿真路径数据
        /// </summary>
        public ImageSimManage m_ImageSimManage_Cam = new ImageSimManage();

        public Form_MsaMeasureThick3D(String _path, Dictionary<CameraEnum, CameraManager> _cameraManager)
        {
            InitializeComponent();
            string strLRoiParaPath = _path + "\\LRoiPara.xml";
            string strLRoiMesureParaPath = _path + "\\LRoiMesurePara.xml";
            cameraManager = _cameraManager;
          
            foreach (var item in cameraManager)
            {
                
                cbx_CamSelect.Items.Add(item.Value.m_CamParam.CameraName);
            }

          
            cbx_CamSelect.SelectedIndex = 2;
        
            LineMethodSelect.SelectedIndex = 1;
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
        }

        public Form_MsaMeasureThick3D( Dictionary<CameraEnum, CameraManager> _cameraManager)
        {
            InitializeComponent();
       
            cameraManager = _cameraManager;

            foreach (var item in cameraManager)
            {

                cbx_CamSelect.Items.Add(item.Value.m_CamParam.CameraName);
            }


            cbx_CamSelect.SelectedIndex = 0;

            LineMethodSelect.SelectedIndex = 1;
          
            linkLabel1.Text = Project.Instance.paramPathSet.ProductMojuanPath;
        }

        void Initialcontrol(ROI rOI)
        {
            switch (rOI.roiParams.rOItype)
            {
                case ROIType.矩形1:

                    rOI.ModifyDicValue("矩形平移Col", new System.Windows.Forms.TextBox() { Text = "0" });
                    rOI.ModifyDicValue("矩形平移row", new System.Windows.Forms.TextBox() { Text = "0" });
    
                    System.Windows.Forms.ComboBox comboBox1 = new System.Windows.Forms.ComboBox();
                    comboBox1.FormattingEnabled = true;
                    comboBox1.Items.AddRange(new object[] { "regression", "huber", "tukey" });
                    comboBox1.SelectedIndex = 0;
                    rOI.ModifyDicValue("拟合算法", comboBox1);
                    System.Windows.Forms.ComboBox comboBoxSelectMethod = new System.Windows.Forms.ComboBox();
                    comboBoxSelectMethod.FormattingEnabled = true;
                    comboBoxSelectMethod.Items.AddRange(new object[] { "线取点", "面取点" });
                    comboBoxSelectMethod.SelectedIndex = 0;
                    rOI.ModifyDicValue("取点方式", comboBoxSelectMethod);
                    rOI.ModifyDicValue("线条数", new System.Windows.Forms.TextBox() { Text = "1" });
                    rOI.ModifyDicValue("数据补偿B", new System.Windows.Forms.TextBox() { Text = "0" });
                    rOI.ModifyDicValue("最大迭代次数", new System.Windows.Forms.TextBox() { Text = "55" });
                    rOI.ModifyDicValue("裁剪因子", new System.Windows.Forms.TextBox() { Text = "15" });
                    rOI.ModifyDicValue("矫正因子", new System.Windows.Forms.TextBox() { Text = "1" });
                    rOI.ModifyDicValue("修正值", new System.Windows.Forms.TextBox() { Text = "0" });
                    System.Windows.Forms.ComboBox comboBox2 = new System.Windows.Forms.ComboBox();
                    comboBox2.FormattingEnabled = true;
                    comboBox2.Items.AddRange(new object[] { "byte", "real", "uint2" });
                    comboBox2.SelectedIndex = 1;
                    rOI.ModifyDicValue("像素类型", comboBox2);
                    rOI.ModifyDicValue("启用数据校准", new System.Windows.Forms.CheckBox() { Checked = false });
                    rOI.ModifyDicValue("基准值", new System.Windows.Forms.TextBox() { Text = "0" });
                    rOI.ModifyDicValue("上公差", new System.Windows.Forms.TextBox() { Text = "0.1" });
                    rOI.ModifyDicValue("下公差", new System.Windows.Forms.TextBox() { Text = "-0.1" });
                    rOI.ModifyDicValue("基准公差校准使用)", new System.Windows.Forms.TextBox() { Text = "0" });
                    rOI.ModifyDicValue("校准公差校准使用)", new System.Windows.Forms.TextBox() { Text = "0" });
                    rOI.ModifyDicValue("是否启用滤波", new System.Windows.Forms.CheckBox() { Checked = true });
                    rOI.ModifyDicValue("滤除高点百分比", new System.Windows.Forms.TextBox() { Text = "0.1" });
                    rOI.ModifyDicValue("滤除低点百分比", new System.Windows.Forms.TextBox() { Text = "0.1" });
                    rOI.ModifyDicValue("是否启用均值滤波", new System.Windows.Forms.CheckBox() { Checked = false });
                    rOI.ModifyDicValue("滤波个数", new System.Windows.Forms.TextBox() { Text = "100" });
                    rOI.ModifyDicValue("线数据补偿B", new System.Windows.Forms.TextBox() { Text = "1000" });
                    rOI.ModifyDicValue("线平移row", new System.Windows.Forms.TextBox() { Text = "500" });

                    break;
                case ROIType.矩形2:
                    //rOI.ModifyDicValue("点数", new System.Windows.Forms.TextBox() { Text = "15" });
                    //rOI.ModifyDicValue("抓边阈值", new System.Windows.Forms.TextBox() { Text = "20" });
                    //rOI.ModifyDicValue("抓边Sigma", new System.Windows.Forms.TextBox() { Text = "2.00" });
                    //System.Windows.Forms.ComboBox comboBox1 = new System.Windows.Forms.ComboBox();
                    //comboBox1.FormattingEnabled = true;
                    //comboBox1.Items.AddRange(new object[] { "第一个点", "最后一点", "所有" });
                    //comboBox1.SelectedIndex = 0;
                    //rOI.ModifyDicValue("取点方式", comboBox1);
                    //System.Windows.Forms.ComboBox comboBox2 = new System.Windows.Forms.ComboBox();
                    //comboBox2.FormattingEnabled = true;
                    //comboBox2.Items.AddRange(new object[] { "暗->明", "明->暗", "所有" });
                    //comboBox2.SelectedIndex = 0;
                    //rOI.ModifyDicValue("边缘类型", comboBox2);
                    break;
                case ROIType.圆:

                    break;
                case ROIType.线:

                    if (LineMethodSelect.SelectedIndex == 0)
                    {

                    }
                    else if (LineMethodSelect.SelectedIndex == 1)
                    {
                        rOI.ModifyDicValue("数据补偿B", new System.Windows.Forms.TextBox() { Text = "1000" });
                        rOI.ModifyDicValue("线平移row", new System.Windows.Forms.TextBox() { Text = "500" });   
                        rOI.ModifyDicValue("是否启用均值滤波", new System.Windows.Forms.CheckBox() { Checked = true });
                        rOI.ModifyDicValue("滤波个数", new System.Windows.Forms.TextBox() { Text = "100" });
                    }

                    //rOI.ModifyDicValue("像素类型", CobLineMethodCobLineMethod);


                    //rOI.ModifyDicValue("抓边阈值", new System.Windows.Forms.TextBox() { Text = "20" });
                    //rOI.ModifyDicValue("抓边Sigma", new System.Windows.Forms.TextBox() { Text = "2.00" });
                    //rOI.ModifyDicValue("比例尺", new System.Windows.Forms.TextBox() { Text = "0.5" });
                    //System.Windows.Forms.ComboBox comboBoxTransitionType = new System.Windows.Forms.ComboBox();
                    //comboBoxTransitionType.Items.AddRange(new object[] { "all", "position", "negative" /*,"all_strongest", "position_strongest" , "negative_strongest"*/});
                    //comboBoxTransitionType.SelectedIndex = 0;
                    //rOI.ModifyDicValue("变换Trans", comboBoxTransitionType);
                    //System.Windows.Forms.ComboBox comboBoxEdgePostion = new System.Windows.Forms.ComboBox();
                    //comboBoxEdgePostion.Items.AddRange(new object[] { "all", "first", "last" });
                    //comboBoxEdgePostion.SelectedIndex = 0;
                    //rOI.ModifyDicValue("位置Pos", comboBoxEdgePostion);

                    //System.Windows.Forms.ComboBox comboBoxInterMethod = new System.Windows.Forms.ComboBox();
                    //comboBoxInterMethod.Items.AddRange(new object[] { "nearest_neighbor", "bilinear", "bicubic" });
                    //comboBoxInterMethod.SelectedIndex = 0;
                    //rOI.ModifyDicValue("插值方法", comboBoxInterMethod);

                    //rOI.ModifyDicValue("边缘对", new System.Windows.Forms.CheckBox() { Checked = false });



                    break;
            }
        }

        void measureMethod(ROI rOI)
        {
            if (image3DClass == null) return;
            if (image3DClass?.Image3D != null)
            {
                HTuple t1, t2;
                HOperatorSet.CountSeconds(out t1);
                HTuple MetrologyHandle = new HTuple();
                HOperatorSet.GetImageSize(image3DClass.Image3D, out HTuple width, out HTuple height);
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
                            double d_BaseTolerance = Convert.ToDouble(rOI.GetDicValue("基准公差校准使用"));
                            double d_CalibrationTolerance = Convert.ToDouble(rOI.GetDicValue("校准公差校准使用"));
                            double d_FilterLowPercent = Convert.ToDouble(rOI.GetDicValue("滤除低点百分比"));
                            double d_FilterHighPercent = Convert.ToDouble(rOI.GetDicValue("滤除高点百分比"));
                            bool b_DataMeanFilter = Convert.ToBoolean(rOI.GetDicValue("是否启用均值滤波"));
                            int i_FilterNum = Convert.ToInt32(rOI.GetDicValue("滤波个数"));
                            //取点方式  
                            string strSelectMethod = Convert.ToString(rOI.GetDicValue2("取点方式"));
                            int i_LineNum = Convert.ToInt32(rOI.GetDicValue("线条数"));
                            //int n_LineDataB = Convert.ToInt32(rOI.GetDicValue("线数据补偿B"));
                            //int n_LineShiftRow = Convert.ToInt32(rOI.GetDicValue("线平移row"));
                       


                            HOperatorSet.GenRectangle1(out HObject rectangle, rOIParams.row1, rOIParams.col1, rOIParams.row2, rOIParams.col2);
                            HOperatorSet.MoveRegion(rectangle, out HObject rectanglePlane, offsetRow, offsetCol);
                            // regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangle, m_stuGetColor = HalconColor.orange });
                            regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectanglePlane, m_stuGetColor = HalconColor.orange });
                            HTuple Rows = 0, Columns = 0;
                            HTuple PlaneRows = 0, PlaneColumns = 0;
                            HTuple grayvalDiff=0 ;




                            HOperatorSet.FitSurfaceFirstOrder(rectanglePlane, image3DClass.Image3D, hv_Algorithm,
                            hv_Iterations, hv_ClippingFactor, out HTuple alpha, out HTuple beta, out HTuple gamma);
                            HOperatorSet.AreaCenter(rectanglePlane, out HTuple area, out HTuple row, out HTuple column);
                            HOperatorSet.GenImageSurfaceFirstOrder(out HObject imageSurface, hv_ImageType, alpha, beta, gamma, row, column, width, height);
                            HOperatorSet.SubImage(image3DClass.Image3D, imageSurface, out HObject imageSub, hv_Mult, hv_Add);
                          
                            if (strSelectMethod == "线取点")
                            {
                                //行数间隔
                                int rowInterval = (int)(rOIParams.row2 - rOIParams.row1) / i_LineNum;
                    
                               
                                for (int i = 0; i < i_LineNum; i++)
                                {
                                    int row1 = (int)rOIParams.row1 + i * rowInterval;
                                    int row2 = (int)rOIParams.row1 + i * rowInterval;
                                    HOperatorSet.GenRegionLine(out HObject lineRegion, row1, rOIParams.col1, row2, rOIParams.col2);
                                    regionShowInfos.Add(new RegionShowInfo() { m_hRegion = lineRegion, m_stuGetColor = HalconColor.orange });
                                    HOperatorSet.GetRegionPoints(lineRegion, out HTuple rows, out HTuple columns);
                                    HOperatorSet.GetGrayval(imageSub, rows, columns, out HTuple grayvalDiffTemp);
                                    if (grayvalDiffTemp.TupleLength() > 0)
                                    {

                                        Rows =Rows.TupleConcat(rows) ;
                                        Columns = Columns.TupleConcat(columns);
                                        grayvalDiff = grayvalDiff.TupleConcat(grayvalDiffTemp);
                            
                                    }
                                    
                     
                                }
                              



                            }
                            else if (strSelectMethod == "面取点")
                            {
                                HOperatorSet.GetRegionPoints(rectangle, out Rows, out Columns);
                                HOperatorSet.GetRegionPoints(rectanglePlane, out PlaneRows, out PlaneColumns);
                                HOperatorSet.GetGrayval(imageSub, Rows, Columns, out grayvalDiff);

                            }




                            imageSurface?.Dispose();
                            imageSub?.Dispose();
                            outputvalue = new HTuple[4];
                            if (grayvalDiff?.Length > 0)
                            {

                                //判断是否启用滤波
                                if (b_DataFilter)
                                {
                                    //data_MaxMin_Filter(grayvalDiff, d_FilterLowPercent, d_FilterHighPercent, out grayvalDiff);
                                    OtherAlgorithm.data_filter_output_index(grayvalDiff, d_FilterLowPercent, d_FilterHighPercent, out grayvalDiff, out HTuple dataFilterIndex);
                                }
                                if (b_DataMeanFilter)
                                {

                                    grayvalDiff = OtherAlgorithm.AvgFilter(grayvalDiff, i_FilterNum);
                                    //grayvalDiff = new HTuple(grayvalDiffList.ToArray());
                                }

                                //if (strSelectMethod == "线取点")
                                //{
                                //    int Count = grayvalDiff.TupleLength();
                                //     HTuple columns=  HTuple.TupleGenSequence(rOIParams.col1, rOIParams.col1 + (Count - 1), 1);
                                //    HOperatorSet.GenRegionPoints(out HObject pointRegion, grayvalDiff * n_LineDataB + n_LineShiftRow, columns);
                                //    HOperatorSet.GenContourRegionXld(pointRegion, out ho_Contours, "border");
                                //    regionShowInfos.Add(new RegionShowInfo() { m_hRegion = pointRegion, m_stuGetColor = HalconColor.orange });

                                //}
                             



                                HTuple point_Z_diff =new HTuple((-grayvalDiff));
                                HOperatorSet.TupleMean(point_Z_diff, out HTuple mean);
                                HOperatorSet.TupleMax(point_Z_diff, out HTuple max);
                                HOperatorSet.TupleMin(point_Z_diff, out HTuple min);
                                mean = mean.TupleAbs() * 1000+z;
                                max = max.TupleAbs() * 1000+z;
                                min = min.TupleAbs() * 1000+z;
                                //校准数据
                                double meanvalue = mean.D;
                                OtherAlgorithm.Calibration(b_DataCalibration, d_BaseValue, d_BaseTolerance, d_CalibrationTolerance, ref meanvalue);
                                mean = new HTuple(meanvalue);
                                outputvalue[0] = mean;
                                outputvalue[1] = max;
                                outputvalue[2] = min;
                                outputvalue[3] = mean;
                                messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"帧号：{m_ImageSimManage_Cam.currentImageIndex}", m_stuGetColor = HalconColor.green });
                                messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"平均值：{mean.D.ToString("0.00")}", m_stuGetColor = HalconColor.green });
                                messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"最大值：{max.D.ToString("0.00")}", m_stuGetColor = HalconColor.green });
                                messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"最小值：{min.D.ToString("0.00")}", m_stuGetColor = HalconColor.green });
                                messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"{mean.D.ToString("0.00")}um", m_stuGetColor = HalconColor.green, m_bIsWindow = false, m_fRow = (rOIParams.row1 + rOIParams.row2) / 2, m_fCol = (rOIParams.col1 + rOIParams.col2) / 2 });
                                //regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Contours, m_stuGetColor = HalconColor.orange });
                            }
                            else
                            {
                                messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"帧号：{m_ImageSimManage_Cam.currentImageIndex}_未检测到矩形", m_stuGetColor = HalconColor.green });
                                // HOperatorSet.DispText(HWC_Display.HalconWindow, $"帧号：{m_ImageSimManage_Cam.currentImageIndex}\n未检测到矩形", "window", 10, 0, HalconColor.green.ToString(), "box", "false");
                            }
                            rectangle?.Dispose();







                        }
                        catch (Exception ee)
                        {

                            // throw;
                        }
                        finally
                        {
                            rOI.OutRoiResult = outputvalue;
                        }

                        break;
                    case ROIType.矩形2:

                        break;
                    case ROIType.线:

                        try
                        {
                            if (LineMethodSelect.SelectedIndex == 0)
                            {

                            }
                            else if (LineMethodSelect.SelectedIndex == 1)
                            {
                                //
                                int offsetRow = Convert.ToInt32(rOI.GetDicValue("线平移row"));
                                int z = Convert.ToInt32(rOI.GetDicValue("数据补偿B"));
                                bool b_DataFilter = Convert.ToBoolean(rOI.GetDicValue("是否启用滤波"));
                                double d_FilterLowPercent = Convert.ToDouble(rOI.GetDicValue("滤除低点百分比"));
                                double d_FilterHighPercent = Convert.ToDouble(rOI.GetDicValue("滤除高点百分比"));
                                bool b_DataMeanFilter = Convert.ToBoolean(rOI.GetDicValue("是否启用均值滤波"));
                                int i_FilterNum = Convert.ToInt32(rOI.GetDicValue("滤波个数"));
                                HOperatorSet.GenRegionLine(out HObject line, rOIParams.row1, rOIParams.col1, rOIParams.row2, rOIParams.col2);
                                HOperatorSet.GetRegionPoints(line, out HTuple RowLines, out HTuple ColumnLines);
                                HOperatorSet.GetGrayval(image3DClass.Image3D, RowLines, ColumnLines, out HTuple grayvalDiffline1);

                         
                                if(b_DataMeanFilter)
                                {
                                    grayvalDiffline1 = OtherAlgorithm.AvgFilter(grayvalDiffline1, i_FilterNum);
                                }
                             
                                int Count = grayvalDiffline1.TupleLength();
                                HTuple hv_ary = HTuple.TupleGenSequence((int)rOIParams.col1, (int)rOIParams.col1 + (Count - 1), 1);
                                HOperatorSet.GenRegionPoints(out HObject polygon, grayvalDiffline1 * z + offsetRow, ColumnLines);
                                HOperatorSet.GenContourRegionXld(polygon, out ho_Contours, "border");
                                //HOperatorSet.GenRegionPoints(out HObject region,  grayvalDiffline1*500, IndexLine1);
                                messageShowInfos.Add(new MessageShowInfo() { m_strMessage = $"帧号：{m_ImageSimManage_Cam.currentImageIndex}", m_stuGetColor = HalconColor.green });
                                regionShowInfos.Add(new RegionShowInfo() { m_hRegion = polygon, m_stuGetColor = HalconColor.orange });
                            }
                        }
                        catch (Exception)
                        {

                            // throw;
                        }



                        break;




                }
            }
        }


        /// <summary>
        /// 校准数据,使数据波动更小
        /// </summary>
        /// <param name="inputOpenDecCalibration">是否启用</param>
        /// <param name="inputSpecification">输入的基准值</param>
        /// <param name="inputTolerance">基准公差</param>
        /// <param name="inputToleranceCalibration">校准公差</param>
        /// <param name="inputValue">需要修改的值</param>
        public void Calibration(bool inputOpenDecCalibration, double inputSpecification,
            double inputTolerance, double inputToleranceCalibration,
          ref double inputValue)
        {
            if (inputOpenDecCalibration)
            {
                double tempMin = inputSpecification - inputTolerance;
                double tempMax = inputSpecification + inputTolerance;

                double tempCalibrationMin = inputSpecification -
                    inputToleranceCalibration;
                double tempCalibrationMax = inputSpecification +
                   inputToleranceCalibration;

                //当值超过基准公差范围时,才校正
                if (tempMin > inputValue || inputValue > tempMax)
                {
                    if (tempCalibrationMin <= inputValue && inputValue <= tempCalibrationMax)
                    {

                        Random random = new Random();
                        inputValue = random.NextDouble() * (tempMax - tempMin) + tempMin;

                    }
                }

            }


        }

        //public void data_MaxMin_Filter(HTuple hv_grayval, HTuple hv_rateLowRemove,
        //HTuple hv_rateHighRemove,int step, out HTuple hv_grayvalOut)
        //{
        //    hv_grayvalOut = new HTuple();
        //    HTuple mean = hv_grayval.TupleMean();
        //    HTuple max = hv_grayval.TupleMax();
        //    HTuple min = hv_grayval.TupleMin();
        //    HTuple rateLowRemove = ( min) * hv_rateLowRemove;
        //    HTuple rateHighRemove = (max ) * hv_rateHighRemove ;
        //    //去除大于rateHighRemove小于rateHighRemove的点，用平均值代替
        //    for (int i = 0; i < hv_grayval.TupleLength(); i+ )
        //    {
        //        if (hv_grayval[i] > rateHighRemove || hv_grayval[i] < rateLowRemove)
        //        {
        //            hv_grayvalOut[i] = mean;
        //        }
        //        else
        //        {
        //            hv_grayvalOut[i] = hv_grayval[i];
        //        }
        //    }

        //}


        public void data_filter_output_index(HTuple hv_grayval, HTuple hv_rateLowRemove,
        HTuple hv_rateHighRemove, out HTuple hv_grayvalOut, out HTuple hv_dataFilterIndex)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Index = new HTuple(), hv_graySorted = new HTuple();
            HTuple hv_m = new HTuple(), hv_n = new HTuple(), hv_SequenceHighRemove = new HTuple();
            HTuple hv_grayReduced1 = new HTuple(), hv_SequenceLowRemove = new HTuple();
            // Initialize local and output iconic variables 
            hv_grayvalOut = new HTuple();
            hv_dataFilterIndex = new HTuple();
            //* 功能：对一维数组进行滤波，并输出序号
            //* 输入
            //* grayval                     输入一维数组
            //* rateLowRemove               去除高差低的百分比（滤波）  eg:0.1
            //* rateHighRemove              去除高差高的百分比         eg:0.1
            //*** 输出
            //* grayvalOut                  滤波后的排序后输出一维数组
            //* dataFilterIndex             滤波后的排序后输出排序前对应的index
            //* written by guke
            //*--------------------------------------------------------------------------------------


            HOperatorSet.TupleSortIndex(hv_grayval, out hv_Index);

            HOperatorSet.TupleSort(hv_grayval, out hv_graySorted);
            //去除低m%  去除高n%

            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_m = (((new HTuple(hv_graySorted.TupleLength()
                    )) * hv_rateLowRemove)).TupleInt();
            }

            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_n = (((new HTuple(hv_graySorted.TupleLength()
                    )) * hv_rateHighRemove)).TupleInt();
            }
            if ((int)(new HTuple(hv_n.TupleNotEqual(0))) != 0)
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {

                    HOperatorSet.TupleGenSequence(((new HTuple(hv_graySorted.TupleLength())) - 1) - hv_n,
                        (new HTuple(hv_graySorted.TupleLength())) - 1, 1, out hv_SequenceHighRemove);
                }

                HOperatorSet.TupleRemove(hv_graySorted, hv_SequenceHighRemove, out hv_grayReduced1);
            }
            else
            {

                hv_grayReduced1 = new HTuple(hv_graySorted);
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {

                HOperatorSet.TupleGenSequence(0, hv_m - 1, 1, out hv_SequenceLowRemove);
            }

            HOperatorSet.TupleRemove(hv_grayReduced1, hv_SequenceLowRemove, out hv_grayvalOut);

            //Index去除高m%  去除低n%

            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_m = (((new HTuple(hv_Index.TupleLength()
                    )) * hv_rateLowRemove)).TupleInt();
            }

            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_n = (((new HTuple(hv_Index.TupleLength()
                    )) * hv_rateHighRemove)).TupleInt();
            }
            if ((int)(new HTuple(hv_n.TupleNotEqual(0))) != 0)
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {

                    HOperatorSet.TupleGenSequence(((new HTuple(hv_Index.TupleLength())) - 1) - hv_n,
                        (new HTuple(hv_Index.TupleLength())) - 1, 1, out hv_SequenceHighRemove);
                }

                HOperatorSet.TupleRemove(hv_Index, hv_SequenceHighRemove, out hv_grayReduced1);
            }
            else
            {

                hv_grayReduced1 = new HTuple(hv_Index);
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {

                HOperatorSet.TupleGenSequence(0, hv_m - 1, 1, out hv_SequenceLowRemove);
            }

            HOperatorSet.TupleRemove(hv_grayReduced1, hv_SequenceLowRemove, out hv_dataFilterIndex);



            return;
        }


        /// <summary>
        /// 重绘指引
        /// </summary>
        void ShowResult()
        {
          
            try
            {

                messageShowInfos_Temp.Clear(); ;
                foreach (var item in regionShowInfos_Temp)
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
                    catch (Exception ex)
                    {
                        // int index = operation_ImageInfo.showHalconWindow.m_regionShowInfos.IndexOf(item);
                    }

                }


                messageShowInfos_Temp = new List<MessageShowInfo>(messageShowInfos);
                regionShowInfos_Temp = new List<RegionShowInfo>(regionShowInfos);
                messageShowInfos.Clear();
                regionShowInfos.Clear();

            }
            catch (Exception)
            {

                // throw;
            }
        }


        void RunFunction()
        {

        }

        private void LoadImage_Click(object sender, EventArgs e)
        {
            //打开指定路径文件夹，加载图片
            OpenFileDialog FileDialog = new OpenFileDialog();
            FileDialog.Multiselect = true;
            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {


                    image3DClass?.Dispose();
                    m_ImageSimManage_Cam.InitSimFlag();
                    m_ImageSimManage_Cam.ImageEmulationPaths = FileDialog.FileNames;

                    if (m_ImageSimManage_Cam.ImageEmulationPaths == null || m_ImageSimManage_Cam.ImageEmulationPaths.Length == 0) return;
                    m_ImageSimManage_Cam.GetCurrent3DImage(out image3DClass);

                    if (image3DClass != null && image3DClass.Image2D?.CountObj() > 0)
                    {
                        image3DClass.Image2D = new HImage(image3DClass.Image2D);
                        hWndCtrl_Display.addIconicVar(image3DClass.Image2D);
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

        private void button1_Click(object sender, EventArgs e)
        {
            ////保存形状参数
            //hWndCtrl_Display.ROIController.SaveROI();
            ////保存测量参数
            //hWndCtrl_Display.ROIController.SaveROIMeasureDics();
            SaveUIToParm();
            hWndCtrl_Display.ROIController.SaveROIMethod(param_MsaThick3D.LtROIParamsPath);
            hWndCtrl_Display.ROIController.SaveROIMeasureDicsMethod(param_MsaThick3D.LtDictionaryPath);
            param_MsaThick3D.Save();
           // param_MsaThick3D.SaveRoiParam();
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

        private void 删除指定区域ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWndCtrl_Display.OnRoiDeleting = true;
        }

        private void 清除所有区域ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hWndCtrl_Display.ROIController.reset();
            hWndCtrl_Display.repaint();
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

        private void btn_Run_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ImageSimManage_Cam.ImageEmulationPaths == null || m_ImageSimManage_Cam.ImageEmulationPaths.Length == 0) return;
                image3DClass?.Dispose();
                m_ImageSimManage_Cam.GetCurrent3DImage(out image3DClass);
                if (image3DClass.Image2D != null && image3DClass.Image2D?.CountObj() > 0)
                {
                    image3DClass.Image2D = new HImage(image3DClass.Image2D);
                    hWndCtrl_Display.addIconicVar(image3DClass.Image2D);
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
                            if (roi.roiParams.rOItype == ROIType.矩形1)
                            {
                                if (i != 3)
                                {
                                    continue;
                                }
                            }

                            stringData.Clear();
                            if (roi.OutRoiResult[i]?.Length > 0)
                            {
                                stringData = roi.OutRoiResult[i].DArr.ToList().ConvertAll(x => x.ToString("0.00")).ToList();

                                stringData.Insert(0, roi.roiParams.rOItype.ToString());
                                stringData.Insert(0, m_ImageSimManage_Cam.currentImageIndex.ToString());
                                //当前时间精确到毫秒写入到第一个
                                stringData.Insert(0, DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"));

                                FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.ProductMojuanPath + strCam + "MSA3D数据", stringTitles.ToArray(), stringData.ToArray(), "");
                                Thread.Sleep(100);
                            }

                        }
                        stringData.Clear();
                        //stringData.Add("\n");
                      //  FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.ProductMojuanPath + strCam + "MSA3D数据", stringTitles.ToArray(), stringData.ToArray(), "");


                    }
                }
            }
            catch (Exception ee)
            {

                //throw;
            }
        }

        private void btn_Next_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_ImageSimManage_Cam.ImageEmulationPaths == null || m_ImageSimManage_Cam.ImageEmulationPaths.Length == 0) return;
                image3DClass?.Dispose();
                m_ImageSimManage_Cam.GetNext3DImage(out image3DClass);
                if (image3DClass.Image2D != null && image3DClass.Image2D?.CountObj() > 0)
                {
                    image3DClass.Image2D = new HImage(image3DClass.Image2D);
                    hWndCtrl_Display.addIconicVar(image3DClass.Image2D);
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
                            if(roi.roiParams.rOItype==ROIType.矩形1)
                            {
                                if(i!=3)
                                {
                                    continue;
                                }
                            }

                            stringData.Clear();
                            if (roi.OutRoiResult[i]?.Length > 0)
                            {
                                stringData = roi.OutRoiResult[i].DArr.ToList().ConvertAll(x => x.ToString("0.00")).ToList();

                                stringData.Insert(0, roi.roiParams.rOItype.ToString());
                                stringData.Insert(0, m_ImageSimManage_Cam.currentImageIndex.ToString());
                                //当前时间精确到毫秒写入到第一个
                                stringData.Insert(0, DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"));

                                FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.ProductMojuanPath + strCam + "MSA3D数据", stringTitles.ToArray(), stringData.ToArray(), "");
                                Thread.Sleep(100);
                            }

                        }
                        stringData.Clear();
                       // stringData.Add("\n");
                       // FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.ProductMojuanPath + strCam + "MSA3D数据", stringTitles.ToArray(), stringData.ToArray(), "");


                    }
                }
            }
            catch (Exception ee)
            {

                //throw;
            }

        }

        private void HWC_Display_HMouseMove(object sender, HMouseEventArgs e)
        {
            if (hWndCtrl_Display.ho_Image_Changed != null & roiController.activeROIidx == -1)
                try
                {
                    TB_PositionAndGray.Text = hWndCtrl_Display.Get_Mouseposition_and_Gray(HWC_Display.HalconWindow, hWndCtrl_Display.ho_Image_Changed, e);
                    TB_PositionAndGray3D.Text = hWndCtrl_Display.Get_Mouseposition_and_Gray(HWC_Display.HalconWindow,image3DClass.Image3D, e);
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
            catch (Exception ee)
            {
                return str;
            }
        }


        void LoadParmToUI()
        {
            try
            {
                cbx_CheckEnable.Checked = param_MsaThick3D.bIsCheck;
                tb_CheckFile.Text = param_MsaThick3D.strProductCheckJobName;
                tb_ProductFile.Text = param_MsaThick3D.strProductJobName;
            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog("参数加载失败："+ee.Message);
               // throw;
            }
        }

        void SaveUIToParm()
        {
            try
            {
                param_MsaThick3D.bIsCheck = cbx_CheckEnable.Checked;
                param_MsaThick3D.strProductCheckJobName = tb_CheckFile.Text;
                param_MsaThick3D.strProductJobName = tb_ProductFile.Text;
            }
            catch (Exception ee)
            {
                MessageNotify.AddErrorLog("参数保存失败：" + ee.Message);
            }
        }

        private void Btn_CreateData_Click(object sender, EventArgs e)
        {

            try
            {
                if (m_ImageSimManage_Cam.ImageEmulationPaths == null || m_ImageSimManage_Cam.ImageEmulationPaths.Length == 0) return;
                image3DClass?.Dispose();
                m_ImageSimManage_Cam.GetCurrent3DImage(out image3DClass);
                if (image3DClass.Image2D != null && image3DClass.Image2D?.CountObj() > 0)
                {
                    image3DClass.Image2D = new HImage(image3DClass.Image2D);
                    hWndCtrl_Display.addIconicVar(image3DClass.Image2D);
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
                            if (roi.roiParams.rOItype == ROIType.矩形1)
                            {
                                if (i != 3)
                                {
                                    continue;
                                }
                            }

                            stringData.Clear();
                            if (roi.OutRoiResult[i]?.Length > 0)
                            {
                                stringData = roi.OutRoiResult[i].DArr.ToList().ConvertAll(x => x.ToString("0.00")).ToList();

                                stringData.Insert(0, roi.roiParams.rOItype.ToString());
                                stringData.Insert(0, m_ImageSimManage_Cam.currentImageIndex.ToString());
                                //当前时间精确到毫秒写入到第一个
                                stringData.Insert(0, DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss_fff"));

                                FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.ProductMojuanPath + strCam + "MSA3D数据", stringTitles.ToArray(), stringData.ToArray(), "");
                                Thread.Sleep(100);
                            }

                        }
                        stringData.Clear();
                       // stringData.Add("\n");
                      //  FileOperationCSV.WriteData_CSV(Project.Instance.paramPathSet.ProductMojuanPath + strCam + "MSA3D数据", stringTitles.ToArray(), stringData.ToArray(), "");


                    }
                }
            }
            catch (Exception ee)
            {

                //throw;
            }
        }

        private void cbx_CamSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(((ComboBox)(sender)).SelectedIndex==0||((ComboBox)(sender)).SelectedIndex==1)
            {
               
                MessageBox.Show("请选择正确的3D相机");
                ((ComboBox)(sender)).SelectedIndex = 2;
            }
            param_MsaThick3D = cameraManager[(CameraEnum)cbx_CamSelect.SelectedIndex].param_MsaThick3D;
            if(hWndCtrl_Display!=null)
            {
                hWndCtrl_Display.ROIController.ReStarRoiParam(param_MsaThick3D.LtROIParams, param_MsaThick3D.LtDictionary);
            }
            else
            {
                param_MsaThick3D = cameraManager[(CameraEnum)cbx_CamSelect.SelectedIndex].param_MsaThick3D;
                roiController = new ROIController( param_MsaThick3D.LtROIParams,  param_MsaThick3D.LtDictionary);
                hWndCtrl_Display = new HWndCtrl(HWC_Display);
                hWndCtrl_Display.useROIController(roiController);
                hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
                hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_MOVE);
                hWndCtrl_Display.MeasureMethod -= measureMethod;
                hWndCtrl_Display.MeasureMethod += measureMethod;

                hWndCtrl_Display.initControlDele -= Initialcontrol;
                hWndCtrl_Display.initControlDele += Initialcontrol;
                hWndCtrl_Display.ImageProcessingEvent += new HWndCtrl.ImageProcessingDelegate(RunFunction);   //更改测量矩形，执行测量方法
                hWndCtrl_Display.ReDrawEvent += new HWndCtrl.ReDrawDelegate(ShowResult);     //更改测量矩形，重绘测量结果
            }
            hWndCtrl_Display.ROIController.LoadROIMethod();
            hWndCtrl_Display.ROIController.LoadROIMeasureDicsMethod();
            LoadParmToUI();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CameraEnum cameraEnum = (CameraEnum)cbx_CamSelect.SelectedIndex;
            if(cbx_CheckEnable.Checked)
            {
                if (Project.Instance.cameraManager[cameraEnum].Camera.SetYLenght1(param_MsaThick3D.strProductCheckJobName))
                {
                    MessageBox.Show("切换点检工程文件成功");
                }
                else
                {
                    MessageBox.Show("切换点检工程文件失败");
                }
              
            }
            else
            {
               if(Project.Instance.cameraManager[cameraEnum].Camera.SetYLenght1(param_MsaThick3D.strProductJobName))
                {
                    MessageBox.Show("切换生产工程文件成功");
                }
               else
                {
                    MessageBox.Show("切换生产工程文件失败");
                }
            }
      
        }
    }
}
