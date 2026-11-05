using Frame_Core;
using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using ViewROI;
using YYSD.UICore;
using YYSD.UICore.Control;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
namespace Plugin.FindMark
{
    public partial class FormFindMark : FormPluginBase
    {

        bool bl_RoiExist = false;

        public HObject ho_Rectangle2;
        public HObject ho_SmallRectangle2;

        public HObject ho_Image = null;

        public HWndCtrl hWndCtrl_Display;
        public ROIController roiController = new ROIController();
        public ROIRectangle2 roiRectangle2 = new ROIRectangle2();
        public HWndCtrl hWndCtrl_Result__Display;

        public HObject ho_Image1 = null;

        /// <summary>
        /// 参数类
        /// </summary>
        public ParamsFindMark PluginParams { get; set; }

        /// <summary>
        /// 插件对象
        /// </summary>
        private FindMarkObj pluginsObj { get; set; }


        public FormFindMark()
        {
            InitializeComponent();
            hWndCtrl_Display = new HWndCtrl(HWC_Display);
            hWndCtrl_Display.useROIController(roiController);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_MOVE);
            hWndCtrl_Display.ImageProcessingEvent += new HWndCtrl.ImageProcessingDelegate(RunFunction);   //更改测量矩形，执行测量方法
            hWndCtrl_Display.ReDrawEvent += new HWndCtrl.ReDrawDelegate(ShowResult);     //更改测量矩形，重绘测量结果

            hWndCtrl_Result__Display = new HWndCtrl(hImg3);
            hWndCtrl_Result__Display.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            hWndCtrl_Result__Display.setViewState(HWndCtrl.MODE_VIEW_MOVE);

            List<string> List_MarkType = new List<string> { "固定值", "启用圆mark", "启用矩形mark", "启用矩形mark+找边" };
            comboBox_MarkType.DataSource = List_MarkType;

        }
        private void FormFindMark_Load(object sender, EventArgs e)
        {

            LoadStandImage();
        }
        /// <summary>
        /// 加载模板图，或者正在检测的图片
        /// </summary>
        private void LoadStandImage()
        {
            try
            {
                if (ho_Image1 != null)
                {
                    if (!pluginsObj.isExistModel)
                    {
                        PluginParams.ho_Image = ho_Image1;
                    }
                }
                HImage hImage = new HImage();
                HobjectToHimage(PluginParams.ho_Image, ref hImage);
                hWndCtrl_Display.addIconicVar(hImage);
            }
            catch (Exception)
            {

            }
            hWndCtrl_Display.resetAll();
            hWndCtrl_Display.repaint();

        }

        /// <summary>
        /// Hobject转为Himage
        /// </summary>
        /// <param name="hobject"></param>
        /// <param name="image"></param>
        private void HobjectToHimage(HObject hobject, ref HImage image)
        {
            HTuple pointer, type, width, height;
            HOperatorSet.GetImagePointer1(hobject, out pointer, out type, out width, out height);
            image.GenImage1(type, width, height, pointer);
        }

        /// <summary>
        /// 获取ROI参数并显示
        /// </summary>
        /// <param name="roiRectangle2"></param>
        /// <param name="DetectResidueParams"></param>
        public void GetRoiParams(ROIRectangle2 roiRectangle2, ParamsFindMark paramsFindMark)
        {
            paramsFindMark.hv_RoiPointsNum = 1;
            roiRectangle2.ROIName = "";

            paramsFindMark.hv_RoiMidRow = roiRectangle2.midR;
            paramsFindMark.hv_RoiMidColumn = roiRectangle2.midC;
            HTuple hv_Phi;
            HTuple hv_Angle;
            HOperatorSet.TupleRad(paramsFindMark.hv_RoiAngle, out hv_Phi);
            HOperatorSet.TupleDeg(roiRectangle2.phi, out hv_Angle);
            paramsFindMark.hv_RoiAngle = hv_Angle;
            paramsFindMark.hv_RoiWidth = roiRectangle2.length2 * 2;
            paramsFindMark.hv_RoiHeight = roiRectangle2.length1 * 2;

            paramsFindMark.hv_RoiStandardOffsetRow = paramsFindMark.hv_RoiMidRow - paramsFindMark.hv_RoiStandardRow;
            paramsFindMark.hv_RoiStandardOffsetColumn = paramsFindMark.hv_RoiMidColumn - paramsFindMark.hv_RoiStandardColumn;
        }

        /// <summary>
        /// ROI参数显示刷新
        /// </summary>
        /// <param name="roiRectangle2"></param>
        private void RefreshRoiParams(ROIRectangle2 roiRectangle2)
        {
            HTuple hv_Deg;
            NUD_RoiWidth.Value = Convert.ToDecimal(roiRectangle2.length2 * 2);
            NUD_RoiHeight.Value = Convert.ToDecimal(roiRectangle2.length1 * 2);
            HOperatorSet.TupleDeg(roiRectangle2.phi, out hv_Deg);
            NUD_RoiAngle.Value = Convert.ToDecimal(hv_Deg.D);
            NUD_RoiMidRow.Value = Convert.ToDecimal(roiRectangle2.midR);
            NUD_RoiMidColumn.Value = Convert.ToDecimal(roiRectangle2.midC);

            NUD_RoiStandardOffsetRow.Value = NUD_RoiMidRow.Value - NUD_RoiStandardRow.Value;
            NUD_RoiStandardOffsetColumn.Value = NUD_RoiMidColumn.Value - NUD_RoiStandardColumn.Value;
        }


        public override void Init()
        {
            pluginsObj = (FindMarkObj)pluginsObjBase;
            PluginParams = (ParamsFindMark)pluginsObj.PluginParams;

            // PluginParams.backup(PluginParams);
            YControlAutobind.Loading<ParamsFindMark, IYCustomAttribute>(this, PluginParams);//测试存修改记录
            SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
            ShowParamsToView(PluginParams);
        }

        /// <summary>
        /// ViewRoi回调函数：执行检测方法
        /// </summary>
        private void RunFunction()
        {

            try
            {
                if (roiController.activeROIidx == -1)
                {
                    if (roiRectangle2 != null)
                        RefreshRoiParams(roiRectangle2);

                    if (roiRectangle2 != null & PluginParams.ho_Image?.CountObj() != 0)
                    {
                        pluginsObj.ExecuteObj();
                        this.Invoke(new Action(() =>
                        {
                            lbMarkCenterX.Text = "Mark中心X：  " + PluginParams.m_fGoalX.ToString("0.000");
                            lbMarkCenterY.Text = "Mark中心Y：  " + PluginParams.m_fGoalY.ToString("0.000");
                            lbMarkCenterR.Text = "Mark中心R：  " + PluginParams.m_fGoalR.ToString("0.000");
                            lbMarkHeight.Text = "Mark宽度:   " + PluginParams.m_fMarkHeight.ToString("0.000");
                            lbMarkWidth.Text = "Mark高度:   " + PluginParams.m_fMarkWidth.ToString("0.000");

                        }));
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }

        }


        /// <summary>
        /// 加载所有参数到窗体，基础参数和抓边参数
        /// </summary>
        /// <param name="measLineParams"></param>
        private void ShowParamsToView(ParamsFindMark Params)
        {
            try
            {

                NUD_baseCenterX.Value = Convert.ToDecimal(Params.m_fBaseCenterX);
                NUD_baseCenterY.Value = Convert.ToDecimal(Params.m_fBaseCenterY);
                NUD_baseCenterR.Value = Convert.ToDecimal(Params.m_fBaseCenterR);
                NUD_CircleThreoldMin.Value = Convert.ToDecimal(Params.m_nCircleThreshMin);
                NUD_CircleThreoldMax.Value = Convert.ToDecimal(Params.m_nCircleThreshMax);
                NUD_CircleWidthMin.Value = Convert.ToDecimal(Params.m_nCircleWidthMin);
                NUD_CircleWidthMax.Value = Convert.ToDecimal(Params.m_nCircleWidthMax);
                NUD_CircularityMin.Value = Convert.ToDecimal(Params.m_fCircularityMin);
                NUD_CircularityMax.Value = Convert.ToDecimal(Params.m_fCircularityMax);
                nud_CircleOpenParam.Value = Convert.ToDecimal(Params.CircleOpenParam);
                NUD_RecThreoldMin.Value = Convert.ToDecimal(Params.m_nRecThreshMin);
                NUD_RecThreoldMax.Value = Convert.ToDecimal(Params.m_nRecThreshMax);
                NUD_RecWidthMin.Value = Convert.ToDecimal(Params.m_nRecWidthMin);
                NUD_RecWidthMax.Value = Convert.ToDecimal(Params.m_nRecWidthMax);
                NUD_PixelUnitX.Value = Convert.ToDecimal(Params.m_fPixelUnitX);
                NUD_PixelUnitY.Value = Convert.ToDecimal(Params.m_fPixelUnitY);
                //Cbx_findLine.Checked = Params.bIsEnFindLine;

                nud_RecOpenParam.Value = Convert.ToDecimal(Params.RectangleOpenParam);

                //cbx_IsEnableRec.Checked = Params.bIsEnRec;
                //cbx_CircleRecognize.Checked = Params.bIsEnCircleRecognize;

                comboBox_MarkType.SelectedIndex = Params.MarkType;
             


            }
            catch (Exception ee)
            {
                string str = "";
                str = "参数设置有误？？请检查\n" + ee.ToString();
                MessageBox.Show(str);


            }
        }



        /// <summary>
        /// ViewRoi函数：加载ROI
        /// </summary>
        /// <param name="DetectResidueParams">抓边工作类</param>
        /// <param name="roiRectangle2"></param>
        /// <param name="bl_RoiExist">ROI是否存在</param>
        /// <param name="roiController">Roi控制类</param>
        /// <param name="hWndCtrl">Roi显示类</param>
        private void SetRoiParams(ParamsFindMark paramsFindMark, ROIRectangle2 roiRectangle2, ROIController roiController, HWndCtrl hWndCtrl)
        {
            try
            {
                roiRectangle2.PointNum = paramsFindMark.hv_RoiPointsNum;
                roiRectangle2.ROIName = "";

                roiRectangle2.midR = paramsFindMark.hv_RoiMidRow;
                roiRectangle2.midC = paramsFindMark.hv_RoiMidColumn;
                HTuple hv_Phi;
                HOperatorSet.TupleRad(paramsFindMark.hv_RoiAngle, out hv_Phi);
                roiRectangle2.phi = hv_Phi.D;
                roiRectangle2.length2 = paramsFindMark.hv_RoiWidth / 2;
                roiRectangle2.length1 = paramsFindMark.hv_RoiHeight / 2;
                if (paramsFindMark.ho_Image == null)
                {
                    HOperatorSet.GenEmptyObj(out paramsFindMark.ho_Image);
                }
                if (paramsFindMark.ho_Image.CountObj() != 0)
                {
                    HTuple hv_Width;
                    HTuple hv_Height;
                    HOperatorSet.GetImageSize(paramsFindMark.ho_Image, out hv_Width, out hv_Height);
                    if (hv_Width < 1000)
                    {
                        roiController.size = roiRectangle2.size = 1;
                    }
                    else
                    {
                        roiController.size = roiRectangle2.size = hv_Width / 1000 + 1;
                    }

                    if (bl_RoiExist == false)
                    {
                        bl_RoiExist = true;
                        roiController.setROIShape(roiRectangle2);
                        roiController.roiMode.createROI(roiRectangle2.midC, roiRectangle2.midR);
                        roiController.ROIList.Add(roiController.roiMode);
                    }
                    roiController.activeROIidx = -1;
                    roiRectangle2.updateHandlePos();
                    hWndCtrl.repaint();
                }
            }
            catch { }
        }




        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void Btn_Save_Click(object sender, EventArgs e)
        {
            //新的实现逻辑
            string error = "";
            //视觉设置

            PluginParams.backup(PluginParams);
            RefreshParamsFromView(PluginParams);
           // PluginParams.FilePath = Project.Instance.logParam.LogSavePath + "\\ParamsChangeLog\\";
            error = YControlAutobind.Saveing<PluginParamsBase, IYCustomAttribute>(this, PluginParams);
            // 调用父窗体
            base.Btn_Save_Click(sender, e);
            PluginParams.saveChangeLog(PluginParams, String.Format("测试视觉设置:{0} 中 {1}", pluginsObj.moduleInfo.ProcessName, pluginsObj.moduleInfo.ModuleName));
            if (error != "") System.Windows.Forms.MessageBox.Show(error);

        }


        private void NUD_RoiNum_ValueChanged(object sender, EventArgs e)
        {
            GetRoiParams(roiRectangle2, PluginParams);
            PluginParams.hv_RoiPointsNum = Convert.ToInt32(NUD_RoiNum.Value);
            SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
        }

        private void NUD_RoiWidth_ValueChanged(object sender, EventArgs e)
        {
            GetRoiParams(roiRectangle2, PluginParams);
            PluginParams.hv_RoiWidth = Convert.ToInt32(NUD_RoiWidth.Value);
            SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
        }

        private void NUD_RoiHeight_ValueChanged(object sender, EventArgs e)
        {
            GetRoiParams(roiRectangle2, PluginParams);
            PluginParams.hv_RoiHeight = Convert.ToInt32(NUD_RoiHeight.Value);
            SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
        }

        private void NUD_RoiAngle_ValueChanged(object sender, EventArgs e)
        {
            GetRoiParams(roiRectangle2, PluginParams);
            PluginParams.hv_RoiAngle = Convert.ToInt32(NUD_RoiAngle.Value);
            SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
        }

        private void NUD_RoiMidRow_ValueChanged(object sender, EventArgs e)
        {
            GetRoiParams(roiRectangle2, PluginParams);
            PluginParams.hv_RoiMidRow = Convert.ToInt32(NUD_RoiMidRow.Value);
            SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
        }

        private void NUD_RoiMidColumn_ValueChanged(object sender, EventArgs e)
        {
            GetRoiParams(roiRectangle2, PluginParams);
            PluginParams.hv_RoiMidColumn = Convert.ToInt32(NUD_RoiMidColumn.Value);
            SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
        }


        private void RefreshParamsFromView(ParamsFindMark Params)
        {
            Params.m_fBaseCenterX = (double)NUD_baseCenterX.Value;
            Params.m_fBaseCenterY = (double)NUD_baseCenterY.Value;
            Params.m_fBaseCenterR = (double)NUD_baseCenterR.Value;

            Params.m_nCircleThreshMin = (int)NUD_CircleThreoldMin.Value;
            Params.m_nCircleThreshMax = (int)NUD_CircleThreoldMax.Value;
            Params.m_nCircleWidthMin = (int)NUD_CircleWidthMin.Value;
            Params.m_nCircleWidthMax = (int)NUD_CircleWidthMax.Value;
            Params.m_fCircularityMin = (double)NUD_CircularityMin.Value;
            Params.m_fCircularityMax = (double)NUD_CircularityMax.Value;
            Params.CircleOpenParam = (int)nud_CircleOpenParam.Value;
            Params.m_fPixelUnitX = (double)NUD_PixelUnitX.Value;
            Params.m_fPixelUnitY = (double)NUD_PixelUnitY.Value;
            Params.MarkType = comboBox_MarkType.SelectedIndex;

            //Params.bIsEnRec = cbx_IsEnableRec.Checked;
            //Params.bIsEnCircleRecognize = cbx_CircleRecognize.Checked;


            Params.RectangleOpenParam = (int)nud_RecOpenParam.Value;
            Params.m_nRecThreshMin = (int)NUD_RecThreoldMin.Value;
            Params.m_nRecThreshMax = (int)NUD_RecThreoldMax.Value;
            Params.m_nRecWidthMin = (int)NUD_RecWidthMin.Value;
            Params.m_nRecWidthMax = (int)NUD_RecWidthMax.Value;
           // Params.bIsEnFindLine = Cbx_findLine.Checked;

        }


        HObject cross ;
        public override void Btn_RUN_Click(object sender, EventArgs e)
        {
            try
            {
                cross?.Dispose();
                RefreshParamsFromView(PluginParams);

                // 调用父窗体
                //重置显示画面


                hWndCtrl_Display.addIconicVar(PluginParams.ho_Image);
                HOperatorSet.GetImageSize(PluginParams.ho_Image, out HTuple width, out HTuple height);
                HOperatorSet.SetSystem("tsp_width", width);
                HOperatorSet.SetSystem("tsp_height", height);


                base.Btn_RUN_Click(sender, e);

                


                lbMarkCenterX.Text = "Mark中心坐标X:   " + PluginParams.m_fGoalX.ToString("0.000");

                lbMarkCenterY.Text = "Mark中心坐标Y:   " + PluginParams.m_fGoalY.ToString("0.000");

                lbMarkCenterR.Text = "Mark中心坐标R:   " + PluginParams.m_fGoalR.ToString("0.000");

                lbMarkHeight.Text= "Mark宽度:   "+ PluginParams.m_fMarkHeight.ToString("0.000");

                lbMarkWidth.Text = "Mark高度:   " + PluginParams.m_fMarkWidth.ToString("0.000");


                ShowParamsToView(PluginParams);



            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(this.Name + ":" + ex.ToString());
            }


            hWndCtrl_Display.resetAll();
            hWndCtrl_Display.repaint();

            // FullScrean_Click(null,null);
            ; ;
        }

        /// <summary>
        /// ViewRoi回调函数：显示检测结果
        /// </summary>
        private void ShowResult()
        {
            try
            {
                GetRoiParams(roiRectangle2, PluginParams);
                ShowParamsToView(PluginParams);

                if (PluginParams.ho_Image != null)
                {
                    HOperatorSet.SetDraw(HWC_Display.HalconWindow, "margin");
                    HOperatorSet.SetColor(HWC_Display.HalconWindow, "orange");
                    if (PluginParams.MarkType == 0)
                    {

                    }
                    else if (PluginParams.MarkType == 1)
                    {
                        if (PluginParams.m_ho_OutRegion?.CountObj() != 0)
                        {
                            HOperatorSet.DispObj(PluginParams.m_ho_OutRegion, HWC_Display.HalconWindow);
                        }
                        HOperatorSet.DispObj(PluginParams.m_ho_OutCropRegion, HWC_Display.HalconWindow);
                    }
                    else if (PluginParams.MarkType == 2)
                    {
                        if (PluginParams.m_ho_OutRecRegion?.CountObj() != 0 && PluginParams.m_ho_OutRecRegion?.CountObj() != null)
                        {
                            HOperatorSet.DispObj(PluginParams.m_ho_OutRecRegion, HWC_Display.HalconWindow);
                        }
                        HOperatorSet.DispObj(PluginParams.m_ho_OutCropRegion, HWC_Display.HalconWindow);
                      
                    }
                    else if(PluginParams.MarkType == 3)
                    {
                        if (PluginParams.m_ho_OutRecRegion?.CountObj() != 0 && PluginParams.m_ho_OutRecRegion?.CountObj() != null)
                        {
                            HOperatorSet.DispObj(PluginParams.m_ho_OutRecRegion, HWC_Display.HalconWindow);
                        }
                        HOperatorSet.DispObj(PluginParams.m_ho_OutCropRegion, HWC_Display.HalconWindow);

                        if (PluginParams.bIsFindLineSucess)
                        {
                            HOperatorSet.DispCross(HWC_Display.HalconWindow, PluginParams.leftTop_Row, PluginParams.leftTop_Col, 100, 0);
                            HOperatorSet.DispCross(HWC_Display.HalconWindow, PluginParams.rightTop_Row, PluginParams.rightTop_Col, 100, 0);
                            HOperatorSet.DispCross(HWC_Display.HalconWindow, PluginParams.leftBottom_Row, PluginParams.leftBottom_Col, 100, 0);
                            HOperatorSet.DispCross(HWC_Display.HalconWindow, PluginParams.rightBottom_Row, PluginParams.rightBottom_Col, 100, 0);
                            HOperatorSet.DispCross(HWC_Display.HalconWindow, PluginParams.m_fGoalY, PluginParams.m_fGoalX, 100, 0);

                        }
                    }
                   

                }
            }
            catch (Exception)
            {

                //  throw;
            }
        }

        private void LoadImage_Click(object sender, EventArgs e)
        {
            //打开指定路径文件夹，加载图片
            OpenFileDialog FileDialog = new OpenFileDialog();
           // FileDialog.InitialDirectory = "E:\\";
            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    PluginParams.ho_Image = new HImage(FileDialog.FileName);
                    ho_Image = PluginParams.ho_Image;
                    hWndCtrl_Display.addIconicVar(PluginParams.ho_Image);

                    SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
                    hWndCtrl_Display.resetAll();
                    hWndCtrl_Display.repaint();

                }
                catch (Exception)
                {
                }
            }
        }

        private void FullScrean_Click(object sender, EventArgs e)
        {
            try
            {
                //重置显示画面
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();
                //hWndCtrl_Result__Display.resetAll();
                //hWndCtrl_Result__Display.repaint();
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
        bool bl_RoiExist1 = false;

        private void comboBox_MarkType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_MarkType.SelectedIndex == 1)
            {
                groupBox_Circle.Enabled = true;
                groupBox_rectangle.Enabled = false;
            }
            else
            {
                groupBox_Circle.Enabled = false;
                groupBox_rectangle.Enabled = true;
            }
        }
    }
}
