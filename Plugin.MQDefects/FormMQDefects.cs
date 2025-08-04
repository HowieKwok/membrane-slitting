using Frame_Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YYSD.UICore.Control;
using YYSD.UICore;
using HalconDotNet;
using ViewROI;
using System.Threading;
using LogTool;
using System.IO;
using Extensions;
using System.Reflection;

namespace Plugin.MQDefects
{
    public partial class FormMQDefects : FormPluginBase
    {
        /// <summary>
        /// 插件对象
        /// </summary>
        private MQDefectsObj pluginsObj { get; set; }
        /// <summary>
        /// 参数类
        /// </summary>
        public ParamsMQDefects PluginParams { get; set; }
        /// <summary>
        /// 更新
        /// </summary>
        bool bl_RefreshObj = true;

        public HObject ho_Image = null;

        /// <summary>
        /// 显示窗口
        /// </summary>
        public HWndCtrl hWndCtrl_Display;


        /// <summary>
        /// 显示窗口
        /// </summary>
        public HWndCtrl hWndCtrl_Init_Display;

        /// <summary>
        /// 
        /// </summary>
        public ROIController roiController = new ROIController();
        /// <summary>
        /// 模板创建区域
        /// </summary>
        public ROIRectangle2 roiRectangle2 = new ROIRectangle2();

        /// <summary>
        /// 用双缓冲绘制窗口的所有子控件,防止闪烁
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // 用双缓冲绘制窗口的所有子控件
                return cp;
            }
        }

        public FormMQDefects()
        {
            InitializeComponent();
            hiddenTabPages = CloneTabControl(TB_Params);
            hWndCtrl_Display = new HWndCtrl(HWC_Display);
            hWndCtrl_Display.useROIController(roiController);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_MOVE);
            hWndCtrl_Display.ImageProcessingEvent += new HWndCtrl.ImageProcessingDelegate(RunFunction);   //更改测量矩形，执行测量方法
            hWndCtrl_Display.ReDrawEvent += new HWndCtrl.ReDrawDelegate(ShowResult);     //更改测量矩形，重绘测量结果


            hWndCtrl_Init_Display = new HWndCtrl(HWC_Image1);
            hWndCtrl_Init_Display.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            hWndCtrl_Init_Display.setViewState(HWndCtrl.MODE_VIEW_MOVE);

        }


        bool bl_RoiExist = false;
    

        /// <summary>
        /// ViewRoi回调函数：显示检测结果
        /// </summary>
        private void ShowResult()
        {
            try
            {

                HWC_Display.Invoke(new Action(() =>
                {


                    HOperatorSet.SetDraw(HWC_Display.HalconWindow, "margin");

                    if (PluginParams.com_DetectionMethod == 5)
                    {
                        //背光极耳
                        //if (PluginParams.cb_Show_JR_ModelXld)
                        //{
                        //    HOperatorSet.SetColor(HWC_Display.HalconWindow, "green");
                        //    //显示模型
                        //    HOperatorSet.DispRegion(PluginParams.ModleXld, HWC_Display.HalconWindow);
                        //}
                        if (PluginParams.cb_Show_JR_DaiLiao_DetectionROI)
                        {
                            HOperatorSet.SetColor(HWC_Display.HalconWindow, "blue");
                            HOperatorSet.DispRegion(PluginParams.JRDaiLiaoXldRegion, HWC_Display.HalconWindow);
                        }
                        if (PluginParams.cb_Show_JR_PoSun_DetectionROI)
                        {
                            HOperatorSet.SetColor(HWC_Display.HalconWindow, "yellow");
                            HOperatorSet.DispRegion(PluginParams.JRPoSunRegion, HWC_Display.HalconWindow);
                        }
                    }
                    else
                    {
                        HOperatorSet.SetColor(HWC_Display.HalconWindow, "blue");
                        HOperatorSet.DispRegion(PluginParams.ShieldROI, HWC_Display.HalconWindow);
                    }

                    HOperatorSet.SetColor(HWC_Display.HalconWindow, "green");
                    HOperatorSet.DispRegion(PluginParams.DetectionROI, HWC_Display.HalconWindow);
                    HOperatorSet.SetColor(HWC_Display.HalconWindow, "red");
                    HOperatorSet.DispRegion(PluginParams.NgRegion, HWC_Display.HalconWindow);

                }));
            }
            catch (Exception ee)
            {
                System.Windows.Forms.MessageBox.Show(ee.ToString());
                // throw;
            }
        }
        /// <summary>
        /// ViewRoi回调函数：执行检测方法
        /// </summary>
        private void RunFunction()
        {
            try
            {
                bl_RefreshObj = false;
                if (roiController.activeROIidx == -1)
                {
                    if (HWC_Image1.ImagePart != null)
                    {
                        if (PluginParams.ho_Image.CountObj() != 0)
                        {
                            hWndCtrl_Display.ho_Image_Changed = PluginParams.ho_Image;
                        }
                    }
                    else
                    {
                        if (PluginParams.ho_ImageProcessed.CountObj() != 0)
                        {
                            hWndCtrl_Display.ho_Image_Changed = PluginParams.ho_ImageProcessed;
                        }
                        else if (PluginParams.ho_Image.CountObj() != 0)
                        {
                            hWndCtrl_Display.ho_Image_Changed = PluginParams.ho_Image;
                        }
                    }
                }
            }
            catch { }
        }
        public override void Init()
        {
            pluginsObj = (MQDefectsObj)pluginsObjBase;
            //界面参数
            PluginParams = (ParamsMQDefects)pluginsObj.PluginParams;
            YControlAutobind.Loading<ParamsMQDefects, IYCustomAttribute>(this, PluginParams);
            LoadStandImage();

            if(!PluginParams.ISEnabledFunctionSelection)
            {
                com_DetectionMethod.Enabled = false;
                com_Cathode.Enabled= false;
            }

            //ShowROI_SearchJR(PluginParams.ho_Image, roiRectangle1, 200, 300);
        }
        private void FormMQDefects_Load(object sender, EventArgs e)
        {

        }



        /// <summary>
        /// 加载模板图，或者正在检测的图片
        /// </summary>
        private void LoadStandImage()
        {
            try
            {
                if (ho_Image != null)
                {
                    if (!pluginsObj.isExistModel)
                    {
                        PluginParams.ho_Image = ho_Image;
                    }
                }

                HImage hImage = new HImage();
                HobjectToHimage(PluginParams.ho_Image, ref hImage);
                hWndCtrl_Display.addIconicVar(hImage);
                
            }
            catch (Exception ex)
            {

            }
            finally
            {
                //SetRoiParams(PluginParams, roiRectangle2, roiController);
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();
                
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

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void Btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                //新的实现逻辑
                string error = "";
                //视觉设置
                PluginParams.backup(PluginParams);
                PluginParams.FilePath = Project.Instance.logParam.LogSavePath + "\\ParamsChangeLog\\"; ;
                error = YControlAutobind.Saveing<PluginParamsBase, IYCustomAttribute>(this, PluginParams);

                // 调用父窗体
                base.Btn_Save_Click(sender, e);
                PluginParams.saveChangeLog(PluginParams, String.Format("视觉设置:{0} 中 {1}", pluginsObj.moduleInfo.ProcessName, pluginsObj.moduleInfo.ModuleName));
                if (error != "") System.Windows.Forms.MessageBox.Show(error);

                if (PluginParams.Model.Length > 0)
                {
                    HOperatorSet.SetShapeModelParam(PluginParams.Model, "timeout", PluginParams.num_TimeOut);
                }
            }
            catch(Exception ex) 
            {

            }
           
        }
        /// <summary>
        /// 运行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void Btn_RUN_Click(object sender, EventArgs e)
        {
            try
            {
                // 调用父窗体
                base.Btn_RUN_Click(sender, e);

                HOperatorSet.ClearWindow(HWC_Display.HalconWindow);
                HOperatorSet.SetDraw(HWC_Display.HalconWindow, "margin");
                HOperatorSet.DispObj(PluginParams.ho_Image, HWC_Display.HalconWindow);

             
                if(PluginParams.com_DetectionMethod==5)
                {
                    //背光极耳
                    if (PluginParams.cb_Show_JR_ModelXld)
                    {
                        HOperatorSet.SetColor(HWC_Display.HalconWindow, "green");
                        //显示模型
                        HOperatorSet.DispObj(PluginParams.ModleXld, HWC_Display.HalconWindow);
                    }
                    if (PluginParams.cb_Show_JR_DaiLiao_DetectionROI)
                    {
                        HOperatorSet.SetColor(HWC_Display.HalconWindow, "blue");
                        HOperatorSet.DispObj(PluginParams.JRDaiLiaoXldRegion, HWC_Display.HalconWindow);
                    }
                    if (PluginParams.cb_Show_JR_PoSun_DetectionROI)
                    {
                        HOperatorSet.SetColor(HWC_Display.HalconWindow, "yellow");
                        HOperatorSet.DispObj(PluginParams.JRPoSunRegion, HWC_Display.HalconWindow);
                    }
                }
                else
                {
                   

                    HOperatorSet.SetColor(HWC_Display.HalconWindow, "blue");
                    HOperatorSet.DispObj(PluginParams.ShieldROI, HWC_Display.HalconWindow);

                   
                }

                HOperatorSet.SetColor(HWC_Display.HalconWindow, "green");
                HOperatorSet.DispObj(PluginParams.DetectionROI, HWC_Display.HalconWindow);
                HOperatorSet.SetColor(HWC_Display.HalconWindow, "red");
                HOperatorSet.DispObj(PluginParams.NgRegion, HWC_Display.HalconWindow);

                //获取字体
                HOperatorSet.SetFont(HWC_Display.HalconWindow, FontEnum.微软雅黑 + "-13");
                HOperatorSet.SetColor(HWC_Display.HalconWindow, "red");
                HOperatorSet.SetTposition(HWC_Display.HalconWindow, 150, 100);
                HOperatorSet.WriteString(HWC_Display.HalconWindow, PluginParams.NgString);

                hWndCtrl_Init_Display.addIconicVar(PluginParams.ho_Image);
                hWndCtrl_Init_Display.resetAll();
                hWndCtrl_Init_Display.repaint();
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Fatal(ex.Message);
            }
            finally
            {
                
            }
        }




        private void LoadImage_Click(object sender, EventArgs e)
        {
            //打开指定路径文件夹，加载图片
            OpenFileDialog FileDialog = new OpenFileDialog();
            //FileDialog.InitialDirectory = "E:\\";
            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    PluginParams.ho_Image = new HImage(FileDialog.FileName);
                    ho_Image = PluginParams.ho_Image;
                    hWndCtrl_Display.addIconicVar(PluginParams.ho_Image);


                    hWndCtrl_Display.resetAll();
                    hWndCtrl_Display.repaint();

                }
                catch (Exception ex)
                {
                    //取得当前方法类全名 包括命名空间    
                    var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                        System.Reflection.MethodBase.GetCurrentMethod().Name);
                     Log4netRecord.ErrorLog.Fatal(str + ex.Message);
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                }
            }
        }
        /// <summary>
        /// 鼠标移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HWC_Display_HMouseMove(object sender, HMouseEventArgs e)
        {
            if (hWndCtrl_Display.ho_Image_Changed != null)
            {
                try
                {
                    TB_PositionAndGray.Text = hWndCtrl_Display.Get_Mouseposition_and_Gray(HWC_Display.HalconWindow, hWndCtrl_Display.ho_Image_Changed, e);
                    LB_IMageSize.Text = ShowIMageSize(hWndCtrl_Display.ho_Image_Changed);
                }
                catch (Exception ex)
                {

                }
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
            catch (Exception ex)
            {
                return str;
            }
        }
        /// <summary>
        /// 检测方法发生变化时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void com_DetectionMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (com_DetectionMethod.SelectedIndex >= 0)
            {
                //检测方法 0 膜区检测 1直边破损 2直边掉料  3直边余料
                int camindex = com_DetectionMethod.SelectedIndex;
                try
                {

                    List<int> intList = new List<int>();
                    switch (camindex)
                    {
                        case 0:
                            cb_Enable_Classify.Enabled = true;
                            cb_Enable_Classify.Checked = PluginParams.cb_Enable_Classify;
                            intList.Add(camindex);
                            break;
                        case 5:
                            cb_Enable_Classify.Enabled = false;
                            cb_Enable_Classify.Checked = false;
                            intList.Add(camindex);
                            intList.Add(8);
                            break;
                        default:
                            cb_Enable_Classify.Enabled = false;
                            cb_Enable_Classify.Checked = false;
                            intList.Add(camindex);
                            break;

                    }

                    //使用模板匹配  则显示区域
                    if (camindex == 5)
                    {
                        SetRoiParams(PluginParams, roiRectangle2, roiController);
                        hWndCtrl_Display.resetAll();
                        hWndCtrl_Display.repaint();
                    }
            
                    intList.Add(9);
                    int[] indices = intList.ToArray();
                    //只显示指定选项卡
                    HideTabPagesExceptIndex(TB_Params, indices);
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message);
                }
            }
        }



        /// <summary>
        /// 用来存放选项卡的
        /// </summary>
        private List<TabPage> hiddenTabPages = new List<TabPage>();
        /// <summary>
        /// 拷贝选项卡
        /// </summary>
        /// <param name="newTabControl"></param>
        /// <returns></returns>
        private List<TabPage> CloneTabControl(TabControl newTabControl)
        {

            List<TabPage> oldTabControl = new List<TabPage>();

            foreach (TabPage tabPage in newTabControl.TabPages)
            {
                oldTabControl.Add(tabPage);
            }
            return oldTabControl;
        }

        /// <summary>
        /// 只显示指定选项卡
        /// </summary>
        /// <param name="tabControl"></param>
        /// <param name="index"></param>
        private void HideTabPagesExceptIndex(TabControl tabControl, int[] indices)
        {
            List<TabPage> tempHiddenTabPages = new List<TabPage>();

            // 先移除所有TabPages
            tabControl.TabPages.Clear();

            //后添加
            foreach (TabPage tabPage in hiddenTabPages)
            {
                if (indices.Contains(hiddenTabPages.IndexOf(tabPage)))
                {
                    tabControl.TabPages.Add(tabPage);
                }
            }
            this.VerticalScroll.Value = 0;
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
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Fatal(ex.Message);
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }





        /// <summary>
        /// ViewRoi函数：加载ROI
        /// </summary>
        /// <param name="measLineParams">抓边工作类</param>
        /// <param name="roiRectangle2"></param>
        /// <param name="bl_RoiExist">ROI是否存在</param>
        /// <param name="roiController">Roi控制类</param>
        /// <param name="hWndCtrl">Roi显示类</param>
        private void SetRoiParams(ParamsMQDefects paramsFindModel, ROIRectangle2 roiRectangle2, ROIController roiController)
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
            roiRectangle2.length2 = paramsFindModel.m_fCenterLength_X;
            roiRectangle2.length1 = paramsFindModel.m_fCenterLength_Y;
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
            catch (Exception ee)
            {

                //throw;
            }

        }
        /// <summary>
        /// 重置模板ROI区域
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void but_ResetROIRegion_Click(object sender, EventArgs e)
        {
            try
            {
                PluginParams.m_fCenterRow = 100;
                PluginParams.m_fCenterCol = 100;
                PluginParams.m_fCenterAngle = 0;
                roiRectangle2.phi = 0;
                PluginParams.m_fCenterLength_X = 50;
                PluginParams.m_fCenterLength_Y = 50;
                SetRoiParams(PluginParams, roiRectangle2, roiController);
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();
            }
            catch (Exception  ex)
            {
                MessageBox.Show(ex.Message);
                
            }

            
        }


        /// <summary>
        /// 生成模型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void but_CreateModel_Click(object sender, EventArgs e)
        {
            //模板截图
            HObject ModelImage = null;
            HOperatorSet.GenEmptyObj(out  ModelImage);
            //模板区域
            HObject ModelRegion = null;
            HOperatorSet.GenEmptyObj(out ModelRegion);
            HObject ModelXld;
            HOperatorSet.GenEmptyObj(out ModelXld);

            HTuple Model=new HTuple();
            try
            {
                GetRoiParams(roiRectangle2, PluginParams);
                //SetRoiParams(PluginParams, roiRectangle2, roiController);

                HOperatorSet.GenRectangle2(out ModelRegion, PluginParams.m_fCenterRow,
                    PluginParams.m_fCenterCol, PluginParams.m_fCenterAngle, PluginParams.m_fCenterLength_Y, PluginParams.m_fCenterLength_X);

                HOperatorSet.ReduceDomain(PluginParams.ho_Image, ModelRegion, out ModelImage);

                int MinContrast = 60;
                HOperatorSet.CreateShapeModel(ModelImage, "auto", (new HTuple(-90).TupleRad()), (new HTuple(90)).TupleRad()
                , "auto", "auto", "use_polarity", "auto", MinContrast, out Model);

                PluginParams.Model = Model;
                HOperatorSet.GetShapeModelContours(out PluginParams.ModelContours, Model, 1);
                HOperatorSet.SetShapeModelParam(PluginParams.Model, "timeout", PluginParams.num_TimeOut*10);
                var isok = CoatingDefectsAlog.FindShapeModel(PluginParams.ho_Image, PluginParams, Model,
                    PluginParams.ModelContours, out ModelXld);
                PluginParams.ModleXld = ModelXld.CopyObj(1, 1);
                if (isok == 0)
                {

                }
                HOperatorSet.ClearWindow(HWC_Display.HalconWindow);
                HOperatorSet.SetDraw(HWC_Display.HalconWindow, "margin");
                HOperatorSet.DispObj(PluginParams.ho_Image, HWC_Display.HalconWindow);
                HOperatorSet.SetColor(HWC_Display.HalconWindow, "green");
                HOperatorSet.DispObj(PluginParams.ModleXld, HWC_Display.HalconWindow);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
            finally
            {
                ModelImage?.Dispose();
                ModelRegion?.Dispose();
                ModelXld?.Dispose();
            }



        }


        public void GetRoiParams(ROIRectangle2 roiRectangle2, ParamsMQDefects paramsFindModel)
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
            paramsFindModel.m_fCenterLength_X = roiRectangle2.length2;
            paramsFindModel.m_fCenterLength_Y = roiRectangle2.length1;

        }

        /// <summary>
        /// 激活选择ROI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void but_SelectROI_Click(object sender, EventArgs e)
        {
            try
            {
                SetRoiParams(PluginParams, roiRectangle2, roiController);
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void but_SaveModel_Click(object sender, EventArgs e)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(Project.Instance.paramPathSet.ProcessPath);
                string ProcessPath = Path.Combine(directoryPath + "\\Model\\", pluginsObjBase.moduleInfo.ProcessName);

                string modelFilePath = Path.Combine(ProcessPath, pluginsObjBase.moduleInfo.ModuleName+ ".sbm");
                CommonApproach.DetectIsExist(modelFilePath);
                HOperatorSet.WriteShapeModel(PluginParams.Model, modelFilePath);
                HOperatorSet.GetShapeModelContours(out PluginParams.ModelContours, PluginParams.Model, 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           

        }

        private void cb_Enable_Classify_CheckedChanged(object sender, EventArgs e)
        {

        }

       
    }
}
