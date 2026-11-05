using Frame_Core;
using Frame_Core.pluginParams;

using HalconDotNet;
using LogTool;
using Plugin.MeasureEar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewROI;

using YYSD.UICore;
using YYSD.UICore.Control;
using static System.Net.Mime.MediaTypeNames;
namespace Plugin.FindModel
{
    public partial class FormFindModel : FormPluginBase
    {

        /// <summary>
        /// 插件对象
        /// </summary>
        private FindModelObj pluginsObj { get; set; }

        private Queue<MessageShowInfo> messageShowInfos { get; set; } = new Queue<MessageShowInfo>();


        /// <summary>
        /// 参数类
        /// </summary>
        public ParamsFindModel PluginParams { get; set; }


        public HObject ho_ImageResult = null;

        public HObject ho_ImgModel = null;

        /// <summary>
        /// 0默认中文//1.英文//2.法文
        /// </summary>
        public int LanguageMode = 0;

        /// <summary>
        /// 显示窗口
        /// </summary>
        public HWndCtrl hWndCtrl_Display;
        public HWndCtrl hWndCtrl_Init_Display;
        public HWndCtrl hWndCtrl_Template_Display;
        public HWndCtrl hWndCtrl_Result__Display;

        public ROIController roiController = new ROIController();

        public ROIRectangle2 roiRectangle2 = new ROIRectangle2();


        public FormFindModel()
        {
            InitializeComponent();
            hWndCtrl_Display = new HWndCtrl(HWC_Display);
            hWndCtrl_Display.useROIController(roiController);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_MOVE);
            hWndCtrl_Display.ImageProcessingEvent += new HWndCtrl.ImageProcessingDelegate(RunFunction);   //更改测量矩形，执行测量方法
            hWndCtrl_Display.ReDrawEvent += new HWndCtrl.ReDrawDelegate(ShowResult);     //更改测量矩形，重绘测量结果

            hWndCtrl_Init_Display = new HWndCtrl(HWC_Image1);
            hWndCtrl_Init_Display.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            hWndCtrl_Init_Display.setViewState(HWndCtrl.MODE_VIEW_MOVE);

            hWndCtrl_Template_Display = new HWndCtrl(HWC_Image2);
            hWndCtrl_Template_Display.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            hWndCtrl_Template_Display.setViewState(HWndCtrl.MODE_VIEW_MOVE);

            hWndCtrl_Result__Display = new HWndCtrl(HWC_Image3);
            hWndCtrl_Result__Display.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            hWndCtrl_Result__Display.setViewState(HWndCtrl.MODE_VIEW_MOVE);


            ho_ImageResult?.Dispose();
            ho_ImgModel?.Dispose();


        }




        #region 窗体控件重绘，防止闪烁
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0xB;
        #endregion


        public override void Init()
        {
            pluginsObj = (FindModelObj)pluginsObjBase;
            PluginParams = (ParamsFindModel)pluginsObj.PluginParams;

            // PluginParams.backup(PluginParams);
            YControlAutobind.Loading<ParamsFindModel, IYCustomAttribute>(this, PluginParams);//测试存修改记录
            this.Text = pluginsObjBase.moduleInfo.ModuleName;




            if (MatclTool.m_dicMetric_Create.Count > 0)
            {
                foreach (var item in MatclTool.m_dicMetric_Create)
                {
                    Cob_Metric_Create.Items.Add(item.Value);
                }
            }

            if (MatclTool.m_dicOptimization_Create.Count > 0)
            {
                foreach (var item in MatclTool.m_dicOptimization_Create)
                {
                    Cob_Optimization_Create.Items.Add(item.Value);
                }
            }

            if (MatclTool.m_dicSubPixel_FindModel.Count > 0)
            {
                foreach (var item in MatclTool.m_dicSubPixel_FindModel)
                {
                    Cob_SubPixel_Find.Items.Add(item.Value);
                }
            }

            if (PluginParams.m_lCreateParams.Count > 0)
            {
                foreach (var item in PluginParams.m_lCreateParams)
                {
                    cob_MatchInfo.Items.Add(item.m_strModelName);
                }
                cob_MatchInfo.SelectedIndex = 0;
            }

            LoadStandImage();
            ShowParamsToView(PluginParams, cob_MatchInfo.Text);
            LoadTemplateImg(PluginParams, cob_MatchInfo.Text);


        }



        /// <summary>
        /// 加载所有参数到窗体，基础参数和抓边参数
        /// </summary>
        /// <param name="measLineParams"></param>
        private void ShowParamsToView(ParamsFindModel Params, string moduleName)
        {
            try
            {
                CreateModelParam createParams = Params.m_lCreateParams.Find(x => x.m_strModelName == moduleName);
                FindModelParam findModelParams = Params.m_lFindParams.Find(x => x.m_strModelName == moduleName);

                Cob_Metric_Create.SelectedIndex = createParams.m_nMetric_Create;
                Cob_Optimization_Create.SelectedIndex = createParams.m_nOptimization_Create;
                Cob_SubPixel_Find.SelectedIndex = findModelParams.m_nSubPixel_Find;


                NUD_OverTime.Value = Convert.ToDecimal(Params.m_nOverTime);
                NUD_RoiMidRow.Value = Convert.ToDecimal(Params.m_fCenterRow);
                NUD_RoiMidCol.Value = Convert.ToDecimal(Params.m_fCenterCol);
                NUD_RoiMidAngle.Value = Convert.ToDecimal(Params.m_fCenterAngle);
                NUD_RoiLength_X.Value = Convert.ToDecimal(Params.m_fCenterLength_X);
                NUD_RoiLength_Y.Value = Convert.ToDecimal(Params.m_fCenterLength_Y);
                NUD_Crop_Pos_C.Value = Convert.ToDecimal(Params.m_fEdgeFindCol);
                nud_Crop_C_End.Value = Convert.ToDecimal(Params.m_fEdgeFindEndCol);

                NUD_Crop_Pos_R.Value = Convert.ToDecimal(Params.m_fEdgeFindRow);
                NUD_Crop_Pos_R_End.Value = Convert.ToDecimal(Params.m_fEdgeFindEndRow);



                NUD_NumLevels_Create.Value = Convert.ToDecimal(createParams.m_nNumLevels_Create);
                NUD_AngleStart_Create.Value = Convert.ToDecimal(createParams.m_nAngleStart_Create);
                NUD_AngleExtend_Create.Value = Convert.ToDecimal(createParams.m_nAngleExtent_Create);
                NUD_AngleStep_Create.Value = Convert.ToDecimal(createParams.m_nAngleStep_Create);

                nud_BaseModelX.Value = Convert.ToDecimal(createParams.m_fCenterColBase);
                nud_BaseModelY.Value = Convert.ToDecimal(createParams.m_fCenterRowBase);
                nud_BaseModelR.Value = Convert.ToDecimal(createParams.m_fCenterAngleBase);

                NUD_Greediness_Find.Value = Convert.ToDecimal(findModelParams.m_fGreediness_Find);
                NUD_NumLevels_Find.Value = Convert.ToDecimal(findModelParams.m_nNumLevels_Find);
                NUD_AngleExtent_Find.Value = Convert.ToDecimal(findModelParams.m_nAngleExtent_Find);
                NUD_MinScore_Find.Value = Convert.ToDecimal(findModelParams.m_fMinScore_Find);
                NUD_NumMatches_Find.Value = Convert.ToDecimal(findModelParams.m_nNumMatches_Find);
                NUD_MaxOverlap_Find.Value = Convert.ToDecimal(findModelParams.m_fMaxOverlap_Find);

                nud_OffsetX.Value = Convert.ToDecimal(Params.m_fOffsetX);
                nud_OffsetY.Value = Convert.ToDecimal(Params.m_fOffsetY);
                nud_OffsetR.Value = Convert.ToDecimal(Params.m_fOffsetR);

            

                nud_GoalX.Value = Convert.ToDecimal(Params.m_fGoalX);
                nud_GoalY.Value = Convert.ToDecimal(Params.m_fGoalY);
                nud_GoalR.Value = Convert.ToDecimal(Params.m_fGoaR);


                Cbx_NotOfsetX.Checked = Params.m_bNotOffsetX;
                Cbx_NotOfsetY.Checked = Params.m_bNotOffsetY;
                Cbx_NotOfsetR.Checked = Params.m_bNotOffsetR;
                Cbx_IsEnble.Checked = Params.m_bIsEnable;

                tb_GoalName.Text = Params.m_strGoalName;

            }
            catch (Exception ee)
            {
                string str = "";
                switch (LanguageMode)
                {
                    case 0:
                        str = "参数设置有误？？请检查\n" + ee.ToString();
                        break;
                    case 1:
                        str = "The parameter settings are incorrect. Please check\n" + ee.ToString();
                        break;
                    case 2:
                        str = "Les paramètres sont incorrects. Veuillez vérifier\n" + ee.ToString();
                        break;
                }
                MessageBox.Show(str);


            }
        }

        /// <summary>
        /// 从界面获取刷新基础极耳参数
        /// </summary>
        /// <param name="Params"></param>
        private void RefreshParamsFromView(ParamsFindModel Params, string moduleName)
        {
            try
            {
                CreateModelParam createParams = Params.m_lCreateParams.Find(x => x.m_strModelName == moduleName);
                FindModelParam findModelParams = Params.m_lFindParams.Find(x => x.m_strModelName == moduleName);
                Params.m_fCenterRow = Convert.ToDouble(NUD_RoiMidRow.Value);
                Params.m_fCenterAngle = Convert.ToDouble(NUD_RoiMidAngle.Value);
                Params.m_fCenterLength_X = Convert.ToDouble(NUD_RoiLength_X.Value);
                Params.m_fCenterLength_Y = Convert.ToDouble(NUD_RoiLength_Y.Value);
                Params.m_fEdgeFindCol = Convert.ToDouble(NUD_Crop_Pos_C.Value);
                Params.m_fEdgeFindEndCol = Convert.ToDouble(nud_Crop_C_End.Value);
                Params.m_fEdgeFindRow = Convert.ToDouble(NUD_Crop_Pos_R.Value);
                Params.m_fEdgeFindEndRow = Convert.ToDouble(NUD_Crop_Pos_R_End.Value);
                Params.m_nOverTime = Convert.ToInt16(NUD_OverTime.Value);

                Params.m_bNotOffsetX = Cbx_NotOfsetX.Checked;
                Params.m_bNotOffsetY = Cbx_NotOfsetY.Checked;
                Params.m_bNotOffsetR = Cbx_NotOfsetR.Checked;
                Params.m_bIsEnable= Cbx_IsEnble.Checked;

                createParams.m_nOptimization_Create = Cob_Optimization_Create.SelectedIndex;

                createParams.m_nMetric_Create = Cob_Metric_Create.SelectedIndex;
                createParams.m_nNumLevels_Create = Convert.ToInt16(NUD_NumLevels_Create.Value);
                createParams.m_nAngleStart_Create = Convert.ToInt16(NUD_AngleStart_Create.Value);
                createParams.m_nAngleExtent_Create = Convert.ToInt16(NUD_AngleExtend_Create.Value);
                createParams.m_nAngleStep_Create = Convert.ToDouble(NUD_AngleStep_Create.Value);

                findModelParams.m_nSubPixel_Find = Cob_SubPixel_Find.SelectedIndex;
                findModelParams.m_fGreediness_Find = Convert.ToDouble(NUD_Greediness_Find.Value);
                findModelParams.m_nNumLevels_Find = Convert.ToInt16(NUD_NumLevels_Find.Value);
                findModelParams.m_nAngleStart_Find = Convert.ToInt16(NUD_AngleStart_Find.Value);
                findModelParams.m_nAngleExtent_Find = Convert.ToInt16(NUD_AngleExtent_Find.Value);
                findModelParams.m_nNumMatches_Find = Convert.ToInt16(NUD_NumMatches_Find.Value);
                findModelParams.m_fMaxOverlap_Find = Convert.ToDouble(NUD_MaxOverlap_Find.Value);
                findModelParams.m_fMinScore_Find = Convert.ToDouble(NUD_MinScore_Find.Value);




            }
            catch (Exception ee)
            {
                string str = "";
                switch (LanguageMode)
                {
                    case 0:
                        str = "参数设置有误？？请检查\n" + ee.ToString();
                        break;
                    case 1:
                        str = "The parameter settings are incorrect. Please check\n" + ee.ToString();
                        break;
                    case 2:
                        str = "Les paramètres sont incorrects. Veuillez vérifier\n" + ee.ToString();
                        break;
                }
                MessageBox.Show(str);

                //throw;
            }
 ;


        }
        /// <summary>
        /// ViewRoi回调函数：显示检测结果
        /// </summary>
        private void ShowResult()
        {
            try
            {
                int FontLine = 0;

                GetRoiParams(roiRectangle2, PluginParams);
                ShowParamsToView(PluginParams, cob_MatchInfo.Text);

                if (messageShowInfos.Count() > 0)
                {
                    foreach (var item in messageShowInfos)
                    {
                        HOperatorSet.SetColor(HWC_Display.HalconWindow, item.m_stuGetColor.ToString());
                        HOperatorSet.DispText(HWC_Display.HalconWindow, item.m_strMessage, "window", FontLine, 0, item.m_stuGetColor.ToString(), "box", "false");
                        FontLine += 20;
                    }

                }
                HOperatorSet.SetDraw(HWC_Display.HalconWindow, "margin");
                HOperatorSet.SetColor(HWC_Display.HalconWindow, "orange");
                HOperatorSet.DispObj(PluginParams.m_ho_OutRegion, HWC_Display.HalconWindow);
                HOperatorSet.DispObj(PluginParams.m_ho_OutCropRegion, HWC_Display.HalconWindow);



            }
            catch (Exception)
            {

                // throw;
            }

        }

        public void GetRoiParams(ROIRectangle2 roiRectangle2, ParamsFindModel paramsFindModel)
        {
            HTuple hv_Angle;
            paramsFindModel.m_fCenterRow = roiRectangle2.midR;
            paramsFindModel.m_fCenterCol = roiRectangle2.midC;
            HOperatorSet.TupleRad(paramsFindModel.m_fCenterAngle, out hv_Angle);
            HOperatorSet.TupleDeg(roiRectangle2.phi, out hv_Angle);
            if (hv_Angle < 0)
            {
                hv_Angle = hv_Angle.D % 360;
                hv_Angle = 360 + hv_Angle.D;

            }
            else
            {
                hv_Angle.D = hv_Angle.D % 360;
            }
            paramsFindModel.m_fCenterAngle = hv_Angle;
            paramsFindModel.m_fCenterLength_X = roiRectangle2.length1;
            paramsFindModel.m_fCenterLength_Y = roiRectangle2.length2;

        }

        /// <summary>
        /// ViewRoi函数：加载ROI
        /// </summary>
        /// <param name="measLineParams">抓边工作类</param>
        /// <param name="roiRectangle2"></param>
        /// <param name="bl_RoiExist">ROI是否存在</param>
        /// <param name="roiController">Roi控制类</param>
        /// <param name="hWndCtrl">Roi显示类</param>
        private void SetRoiParams(ParamsFindModel paramsFindModel, ROIRectangle2 roiRectangle2, ROIController roiController)
        {
            HTuple hv_Phi;
            HTuple hv_Width;
            HTuple hv_Height;
            roiRectangle2.PointNum = 1;
            roiRectangle2.ROIName = "";
            roiRectangle2.midR = paramsFindModel.m_fCenterRow;
            roiRectangle2.midC = paramsFindModel.m_fCenterCol;
            HOperatorSet.TupleRad(paramsFindModel.m_fCenterAngle, out hv_Phi);
            roiRectangle2.phi = hv_Phi.D;
            roiRectangle2.length2 = paramsFindModel.m_fCenterLength_Y;
            roiRectangle2.length1 = paramsFindModel.m_fCenterLength_X;
            try
            {
                if (paramsFindModel.ho_Image == null)
                {
                    HOperatorSet.GenImageGrayRamp(out paramsFindModel.ho_Image, 1, 1, 128, 256, 256, 1024, 1024);
                }
                if (paramsFindModel.ho_Image != null)
                {
                    if (paramsFindModel.ho_Image.CountObj() != 0)
                    {
                        HOperatorSet.GetImageSize(paramsFindModel.ho_Image, out hv_Width, out hv_Height);
                        if (hv_Width < 1000)
                        {
                            roiController.size = roiRectangle2.size = 1;
                        }
                        else
                        {
                            roiController.size = roiRectangle2.size = hv_Width / 1000 + 1;
                        }
                        roiController.setROIShape(roiRectangle2);
                        roiController.roiMode.createROI(roiRectangle2.midC, roiRectangle2.midR);
                        roiController.ROIList.Add(roiController.roiMode);
                        roiController.activeROIidx = -1;
                        roiRectangle2.updateHandlePos();
                    }
                }
            }
            catch (Exception)
            {

                //throw;
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

        private void RunFunction()
        {
            try
            {

            }
            catch (Exception)
            {

                //  throw;
            }
        }
        string m_strImgPath = AppDomain.CurrentDomain.BaseDirectory;
        private void LoadImage_Click(object sender, EventArgs e)
        {
            //打开指定路径文件夹，加载图片

            using (OpenFileDialog FileDialog = new OpenFileDialog())
            {
                // 尝试从设置中获取最后打开的目录

                FileDialog.InitialDirectory = m_strImgPath;
                if (FileDialog.ShowDialog() == DialogResult.OK)
                {
                    PluginParams.ho_Image?.Dispose();
                    m_strImgPath = FileDialog.FileName;
                    PluginParams.ho_Image = new HImage(m_strImgPath);

                    hWndCtrl_Display.addIconicVar(PluginParams.ho_Image);
                    hWndCtrl_Display.resetAll();
                    hWndCtrl_Display.repaint();

                    hWndCtrl_Init_Display.addIconicVar(PluginParams.ho_Image);
                    hWndCtrl_Init_Display.resetAll();
                    hWndCtrl_Init_Display.repaint();


                }


            }
        }

        private void LoadStandImage()
        {
            try
            {

                hWndCtrl_Display.addIconicVar(PluginParams.ho_Image);
                // if()
            }
            catch (Exception)
            {

                // throw;
            }
            SetRoiParams(PluginParams, roiRectangle2, roiController);
            hWndCtrl_Display.resetAll();
            hWndCtrl_Display.repaint();
        }

        /// <summary>
        /// 加载模板图片
        /// </summary>
        private void LoadTemplateImg(ParamsFindModel Params, string moduleName)
        {
            try
            {
                ho_ImgModel?.Dispose();
                string[] files = Directory.GetFiles(Params.m_strModelGroupPath, "*.bmp");
                var matchingFiles = files.Where(file => Path.GetFileName(file).Contains(moduleName));
                foreach (var item in matchingFiles)
                {
                    HOperatorSet.ReadImage(out ho_ImgModel, item);
                }


                hWndCtrl_Template_Display.addIconicVar(ho_ImgModel);
                hWndCtrl_Template_Display.resetAll();
                hWndCtrl_Template_Display.repaint();
                //  HOperatorSet.GetImageSize(ho_ImgModel, out HTuple w, out HTuple h);

            }
            catch (Exception)
            {
                ho_ImgModel?.Dispose();

                HOperatorSet.GenImageGrayRamp(out ho_ImgModel, 1, 1, 128, 256, 256, 512, 512);

                hWndCtrl_Template_Display.addIconicVar(ho_ImgModel);
                hWndCtrl_Template_Display.resetAll();
                hWndCtrl_Template_Display.repaint(); ;

            }
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
        private void Btn_SaveModel_Click(object sender, EventArgs e)
        {

            try
            {
                RefreshParamsFromView(PluginParams, cob_MatchInfo.Text);
                CreateModelParam createParams = PluginParams.m_lCreateParams.Find(x => x.m_strModelName == cob_MatchInfo.Text);
                createParams.m_fCenterLength_XBase = PluginParams.m_fCenterLength_X;
                createParams.m_fCenterLength_YBase = PluginParams.m_fCenterLength_Y;
                createParams.m_fCenterAngleBase = 0;
                createParams.m_fCenterRowBase = PluginParams.m_fCenterRow;
                createParams.m_fCenterColBase = PluginParams.m_fCenterCol;
                PluginParams.Save();
                if (!pluginsObj.CreateModel())
                {
                    string str = "";
                    switch (LanguageMode)
                    {
                        case 0:
                            str = "创建模板失败";
                            break;
                        case 1:
                            str = "Template creation failed";
                            break;
                        case 2:
                            str = "Échec de la création du modèle";
                            break;
                    }
                    MessageBox.Show(str);
                }
                else
                {
                    // 创建模板成功
                    string str = "";
                    switch (LanguageMode)
                    {
                        case 0:
                            str = "创建模板成功";
                            break;
                        case 1:
                            str = "Template creation successful";
                            break;
                        case 2:
                            str = "Création du modèle réussie";
                            break;
                    }
                    MessageBox.Show(str);




                }



            }
            catch (Exception ee)
            {
                string str = "";
                switch (LanguageMode)
                {
                    case 0:
                        str = "创建模板失败";
                        break;
                    case 1:
                        str = "Template creation failed";
                        break;
                    case 2:
                        str = "Échec de la création du modèle";
                        break;
                }
                MessageBox.Show(str + "\n" + ee.ToString());
                //throw;
            }

            //pluginsObj.IniObj();

        }
        private void Btn_CropModel_Click(object sender, EventArgs e)
        {

            try
            {
                GetRoiParams(roiRectangle2, PluginParams);
                SetRoiParams(PluginParams, roiRectangle2, roiController);
                ShowParamsToView(PluginParams, cob_MatchInfo.Text);

                ho_ImgModel?.Dispose();
                HOperatorSet.GenRectangle2(out HObject rectangle, PluginParams.m_fCenterRow, PluginParams.m_fCenterCol, 0, PluginParams.m_fCenterLength_X, PluginParams.m_fCenterLength_Y);
                HOperatorSet.SmallestRectangle1(rectangle, out HTuple row, out HTuple col, out HTuple row2, out HTuple col2);
                //HOperatorSet.ReduceDomain(PluginParams.ho_Image, rectangle, out ho_ImgModel);
                HOperatorSet.CropRectangle2(PluginParams.ho_Image, out ho_ImgModel, PluginParams.m_fCenterRow, PluginParams.m_fCenterCol, 0, PluginParams.m_fCenterLength_X, PluginParams.m_fCenterLength_Y, "true", "constant");
                rectangle?.Dispose();
                hWndCtrl_Template_Display.addIconicVar(ho_ImgModel);
                hWndCtrl_Template_Display.resetAll();
                hWndCtrl_Template_Display.repaint();

                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "bmp(*.bmp)|*.bmp";
                saveFileDialog.InitialDirectory = PluginParams.m_strModelGroupPath;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (Directory.Exists(PluginParams.m_strModelGroupPath))
                    {
                        HOperatorSet.WriteImage(ho_ImgModel, "bmp", 0, saveFileDialog.FileName);
                    }
                    string fileName = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);

                    if (PluginParams.m_lCreateParams.Find(x => x.m_strModelName == fileName) == null)
                    {
                        PluginParams.m_lCreateParams.Add(new CreateModelParam() { m_strModelName = fileName });
                        PluginParams.m_lFindParams.Add(new FindModelParam() { m_strModelName = fileName });
                        cob_MatchInfo.Items.Add(fileName);
                        cob_MatchInfo.SelectedIndex = cob_MatchInfo.Items.IndexOf(fileName);
                    }
                    else
                    {
                        cob_MatchInfo.SelectedIndex = cob_MatchInfo.Items.IndexOf(fileName);
                    }

                    PluginParams.m_strModelPath = saveFileDialog.FileName;
                    PluginParams.Save();
                    ShowParamsToView(PluginParams, fileName);

                    string str = "";
                    switch (LanguageMode)
                    {
                        case 0:
                            str = "截取模板参数成功";
                            break;
                        case 1:
                            str = "Template parameter extraction successful";
                            break;
                        case 2:
                            str = "Extraction du paramètre de modèle réussi";
                            break;
                    }
                    MessageBox.Show(str);
                }

            }
            catch (Exception ee)
            {

                string str = "";
                switch (LanguageMode)
                {
                    case 0:
                        str = "文件保存失败!!!!";
                        break;
                    case 1:
                        str = "File save failed!!!!";
                        break;
                    case 2:
                        str = "Échec de l'enregistrement du fichier";
                        break;
                }
                MessageBox.Show(str + "\n" + ee.ToString());
            }

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
            RefreshParamsFromView(PluginParams, cob_MatchInfo.Text);
            PluginParams.FilePath = Project.Instance.logParam.LogSavePath + "\\ParamsChangeLog\\";
            error = YControlAutobind.Saveing<PluginParamsBase, IYCustomAttribute>(this, PluginParams);
            // 调用父窗体
            base.Btn_Save_Click(sender, e);
            PluginParams.saveChangeLog(PluginParams, String.Format("测试视觉设置:{0} 中 {1}", pluginsObj.moduleInfo.ProcessName, pluginsObj.moduleInfo.ModuleName));
            if (error != "") System.Windows.Forms.MessageBox.Show(error);

        }

        private void ImageReset_Click(object sender, EventArgs e)
        {
            hWndCtrl_Display.resetAll();
            hWndCtrl_Display.repaint();
            hWndCtrl_Init_Display.resetAll();
            hWndCtrl_Init_Display.repaint();
            hWndCtrl_Result__Display.resetAll();
            hWndCtrl_Result__Display.repaint(); ;
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

        public override void Btn_RUN_Click(object sender, EventArgs e)
        {
            try
            {
                RefreshParamsFromView(PluginParams, cob_MatchInfo.Text);

                // 调用父窗体
                PluginParams.m_fScoreCircularity = 0;
                HOperatorSet.GetImageSize(PluginParams.ho_Image, out HTuple width, out HTuple height);
                HOperatorSet.SetSystem("tsp_width", width);
                HOperatorSet.SetSystem("tsp_height", height);
                base.Btn_RUN_Click(sender, e);
                messageShowInfos.Clear();
                ho_ImageResult?.Dispose();
                hWndCtrl_Display.addIconicVar(PluginParams.ho_Image);

                HOperatorSet.SmallestRectangle1(PluginParams.m_ho_OutRegion, out HTuple row, out HTuple col,
                       out HTuple row2, out HTuple col2);
                HOperatorSet.CropPart(PluginParams.ho_Image, out ho_ImageResult, row, col,
                    col2 - col,row2 - row);
                hWndCtrl_Init_Display.addIconicVar(PluginParams.ho_Image);
                hWndCtrl_Result__Display.addIconicVar(ho_ImageResult);
                if (PluginParams.m_fScoreMatch != 0)
                {
                    messageShowInfos.Enqueue(new MessageShowInfo() { m_strMessage = "success", m_stuGetColor = HalconColor.yellow });
                    messageShowInfos.Enqueue(new MessageShowInfo() { m_strMessage = $"Matching Score{PluginParams.m_fScoreMatch}", m_stuGetColor = HalconColor.yellow });
                    ;
                }
                else
                {
                    messageShowInfos.Enqueue(new MessageShowInfo() { m_strMessage = "no success", m_stuGetColor = HalconColor.red });
                }
                ShowParamsToView(PluginParams, cob_MatchInfo.Text);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(this.Name + ":" + ex.ToString());
            }

            hWndCtrl_Display.resetAll();
            hWndCtrl_Display.repaint();
            hWndCtrl_Init_Display.resetAll();
            hWndCtrl_Init_Display.repaint();
            hWndCtrl_Result__Display.resetAll();
            hWndCtrl_Result__Display.repaint(); ;
        }


        private void HWC_Display_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (PluginParams.ho_Image != null)
            {
                hWndCtrl_Display.addIconicVar(PluginParams.ho_Image);
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();
            }
        }

        private void HWC_Display_HMouseDown(object sender, HMouseEventArgs e)
        {


        }



        private void FormFindModel_Load(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void cob_MatchInfo_SelectedValueChanged(object sender, EventArgs e)
        {
            cob_MatchInfo.Text = cob_MatchInfo.SelectedItem.ToString();
            ShowParamsToView(PluginParams, cob_MatchInfo.SelectedItem.ToString());
            LoadTemplateImg(PluginParams, cob_MatchInfo.SelectedItem.ToString());
        }

        private void btn_singleExcuse_Click(object sender, EventArgs e)
        {
            try
            {
                PluginParams.m_fScoreCircularity = 0;
                HOperatorSet.GetImageSize(PluginParams.ho_Image, out HTuple width, out HTuple height);
                HOperatorSet.SetSystem("tsp_width", width);
                HOperatorSet.SetSystem("tsp_height", height);
                pluginsObj.SigleExecuteObj(cob_MatchInfo.Text);
                messageShowInfos.Clear();
                ho_ImageResult?.Dispose();
                hWndCtrl_Display.addIconicVar(PluginParams.ho_Image);

                HOperatorSet.SmallestRectangle1(PluginParams.m_ho_OutRegion, out HTuple row, out HTuple col,
                         out HTuple row2, out HTuple col2);
                HOperatorSet.CropPart(PluginParams.ho_Image, out ho_ImageResult, row, col,
                col2 - col, row2 - row);
                hWndCtrl_Init_Display.addIconicVar(PluginParams.ho_Image);
                hWndCtrl_Result__Display.addIconicVar(ho_ImageResult);

                if (PluginParams.m_fScoreMatch != 0)
                {
                    messageShowInfos.Enqueue(new MessageShowInfo() { m_strMessage = "success", m_stuGetColor = HalconColor.yellow });
                    messageShowInfos.Enqueue(new MessageShowInfo() { m_strMessage = $"Matching Score{PluginParams.m_fScoreMatch}", m_stuGetColor = HalconColor.yellow });
                    ;
                }
                else
                {
                    messageShowInfos.Enqueue(new MessageShowInfo() { m_strMessage = "no success", m_stuGetColor = HalconColor.red });
                }
                ShowParamsToView(PluginParams, cob_MatchInfo.Text);
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();
                hWndCtrl_Init_Display.resetAll();
                hWndCtrl_Init_Display.repaint();
                hWndCtrl_Result__Display.resetAll();
                hWndCtrl_Result__Display.repaint(); ;

            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
                // throw;
            }

        }

        private void btn_DeleteModel_Click(object sender, EventArgs e)
        {
            string modelName = cob_MatchInfo.Text;
            if (MessageBox.Show("Are you sure to delete the model " + modelName + "?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (cob_MatchInfo.Items.Count < 2)
                {
                    //当前只剩一个模板，无法继续删除，系统默认自动保留一个模板
                    string strInfo = "";
                    switch (LanguageMode)
                    {
                        case 0:
                            strInfo = "当前只剩一个模板，无法继续删除，系统默认自动保留一个模板";
                            break;
                        case 1:
                            strInfo = "There is only one template left and it cannot be deleted. The system automatically retains one template.";
                            break;
                        case 2:
                            strInfo = "Il n'y a qu'un modèle restant et il ne peut pas être supprimé. Le système conserve un modèle par défaut.";
                            break;
                    }
                    MessageBox.Show(strInfo);
                    return;
                }
                string modelPath = Path.Combine(PluginParams.m_strModelGroupPath, modelName + ".bmp");
                if (File.Exists(modelPath))
                {
                    File.Delete(modelPath);
                }
                PluginParams.m_lCreateParams.RemoveAll(x => x.m_strModelName == modelName);
                PluginParams.m_lFindParams.RemoveAll(x => x.m_strModelName == modelName);
                cob_MatchInfo.Items.Remove(modelName);
                PluginParams.Save();
                if (cob_MatchInfo.Items.Count > 0)
                {
                    cob_MatchInfo.SelectedIndex = 0;
                    ShowParamsToView(PluginParams, cob_MatchInfo.Text);
                }
                else
                {
                    cob_MatchInfo.SelectedIndex = -1;
                }

                string str = "";
                switch (LanguageMode)
                {
                    case 0:
                        str = "模板删除成功";
                        break;
                    case 1:
                        str = "Template deletion successful";
                        break;
                    case 2:
                        str = "Suppression du modèle réussie";
                        break;
                }
                MessageBox.Show(str);
            }
        }

        private void FormFindModel_Shown(object sender, EventArgs e)
        {
            this.Invoke(new Action(() =>
            {
                Btn_RUN_Click(null, null);
            }));

        }
    }
}
