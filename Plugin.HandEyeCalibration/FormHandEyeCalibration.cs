using Frame_Core;
using HalconAlgorithm;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewROI;
using static System.Net.Mime.MediaTypeNames;


namespace Plugin.HandEyeCalibration
{
    public partial class FormHandEyeCalibration : FormPluginBase
    {
        private HandEyeCalibrationObj pluginsObj { get; set; }
        public ParamsHandEyeCalibration PluginParams { get; set; }
        bool bl_RoiExist = false;

        public HObject ho_Image = null;

        public HWndCtrl hWndCtrl_Display;
        public ROIController roiController = new ROIController();
        public ROIRectangle2 roiRectangle2 = new ROIRectangle2();

        public int SelectIndex = 0;
        public FormHandEyeCalibration()
        {
            InitializeComponent();
            hWndCtrl_Display = new HWndCtrl(HWC_Display);
            //hWndCtrl_Display.useROIController(roiController);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_MOVE);
            //hWndCtrl_Display.ImageProcessingEvent += new HWndCtrl.ImageProcessingDelegate(RunFunction);   //更改测量矩形，执行测量方法
            //hWndCtrl_Display.ReDrawEvent += new HWndCtrl.ReDrawDelegate(ShowResult);     //更改测量矩形，重绘测量结果
        }

        public override void Init()
        {
            pluginsObj = (HandEyeCalibrationObj)pluginsObjBase;
            PluginParams = (ParamsHandEyeCalibration)pluginsObj.PluginParams;
            this.Text = pluginsObjBase.moduleInfo.ModuleName;


            SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
            ShowParamsToForm(PluginParams);
            //InitListView(listView1);

            if (ho_Image != null)
            {
                if (!pluginsObj.isExistModel)
                {
                    PluginParams.ImageInfo.ho_image = ho_Image;
                }
            }
            try
            {
                HImage hImage = new HImage();
                HobjectToHimage(PluginParams.ImageInfo.ho_image, ref hImage);
                hWndCtrl_Display.addIconicVar(hImage);
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();
            }
            catch (Exception) { }
            //测试存参用
        }
        #region 功能函数
        /// <summary>
        /// 延时刷新图标函数
        /// </summary>
        /// <param name="milliSecond">毫秒</param>
        private void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)
            {

                //可执行某无聊的操作
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
        /// ViewRoi函数：加载ROI
        /// </summary>
        /// <param name="Params">抓边工作类</param>
        /// <param name="roiRectangle2"></param>
        /// <param name="bl_RoiExist">ROI是否存在</param>
        /// <param name="roiController">Roi控制类</param>
        /// <param name="hWndCtrl">Roi显示类</param>
        private void SetRoiParams(ParamsHandEyeCalibration Params, ROIRectangle2 roiRectangle2, ROIController roiController, HWndCtrl hWndCtrl)
        {
            try
            {
            
                if (Params.ImageInfo.ho_image == null)
                {
                    HOperatorSet.GenEmptyObj(out Params.ImageInfo.ho_image);
                }
                if (Params.ImageInfo.ho_image.CountObj() != 0)
                {
                    HTuple hv_Width;
                    HTuple hv_Height;
                    HOperatorSet.GetImageSize(Params.ImageInfo.ho_image, out hv_Width, out hv_Height);
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
        /// 加载抓边参数到窗体
        /// </summary>
        /// <param name="measLineParams"></param>
        private void ShowParamsToForm(ParamsHandEyeCalibration Params)
        {
            try
            {
              
            }
            catch 
            { 
            
            }

        }


      

        /// <summary>
        /// ROI参数显示刷新
        /// </summary>
        /// <param name="roiRectangle2"></param>
        private void RefreshRoiParams(ROIRectangle2 roiRectangle2)
        {
            HTuple hv_Deg;
           
            HOperatorSet.TupleDeg(roiRectangle2.phi, out hv_Deg);
            

        }

        /// <summary>
        /// ViewRoi回调函数：执行检测方法
        /// </summary>
        private void RunFunction()
        {

            if (roiController.activeROIidx == -1)
            {
                if (roiRectangle2 != null)
                    RefreshRoiParams(roiRectangle2);

                if (roiRectangle2 != null & PluginParams.ImageInfo.ho_image.CountObj() != 0)
                {
                    Delay(10);
                    pluginsObj.ExecuteObj();
                }

                if (TLP_Image1.BackgroundImage != null)
                {
                    if (PluginParams.ImageInfo.ho_image.CountObj() != 0)
                    {
                        hWndCtrl_Display.ho_Image_Changed = PluginParams.ImageInfo.ho_image;
                    }
                }
                else if (TLP_Image2.BackgroundImage != null)
                {
                    if (PluginParams.ho_ImageProcessed.CountObj() != 0)
                    {
                        hWndCtrl_Display.ho_Image_Changed = PluginParams.ho_ImageProcessed;
                    }
                }
            }
        }

        /// <summary>
        /// ViewRoi回调函数：显示检测结果
        /// </summary>
        private void ShowResult()
        {
            ShowDamageToHWD(PluginParams, HWC_Display.HalconWindow, roiRectangle2.size);
        }
        /// <summary>
        /// 显示直线到窗体上
        /// </summary>
        /// <param name="Params"></param>
        /// <param name="hv_HWindowID"></param>
        private void ShowDamageToHWD(ParamsHandEyeCalibration Params, HTuple hv_HWindowID, HTuple hv_size)
        {
           
            if (roiController.activeROIidx == -1)
            {
                //显示缩略图到图形选项中
                if (Params.ImageInfo.ho_image.CountObj() != 0)
                {
                    PaintWindows.DisplayImage(Params.ImageInfo.ho_image, HWC_Image1);
                }
                else
                {
                    HOperatorSet.ClearWindow(HWC_Image1.HalconWindow);
                }

                if (Params.ho_ImageProcessed.CountObj() != 0)
                {
                    PaintWindows.DisplayImage(Params.ho_ImageProcessed, HWC_Image2);
                }
                else
                {
                    HOperatorSet.ClearWindow(HWC_Image2.HalconWindow);
                }
               

               
              
            }
        }

        #endregion

       

    


        #region 控件事件    

        private void LoadImage_Click(object sender, EventArgs e)
        {
            //打开指定路径文件夹，加载图片
            OpenFileDialog FileDialog = new OpenFileDialog();
            FileDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    PluginParams.ImageInfo.ho_image = new HImage(FileDialog.FileName);
                    ho_Image = PluginParams.ImageInfo.ho_image;
                    hWndCtrl_Display.addIconicVar(ho_Image);

                    SetRoiParams(PluginParams, roiRectangle2, roiController, hWndCtrl_Display);
                    hWndCtrl_Display.resetAll();
                    hWndCtrl_Display.repaint();
                    
                }
                catch (Exception)
                {
                }
            }
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



        #endregion

        #region 窗体控件重绘，防止闪烁
        [System.Runtime.InteropServices.DllImport("user32")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0xB;

        #endregion

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











        private void FormFormHandEyeCalibration_Load(object sender, EventArgs e)
        {
            AddTextBox(flowLayoutPanel_Image, PluginParams.ImageX, PluginParams.ImageY, ref textBoxesImageX, ref textBoxesImageY);
            AddTextBox(flowLayoutPanel_Robot, PluginParams.RobotX, PluginParams.RobotY, ref textBoxesRobotX, ref textBoxesRobotY);

            flowLayoutPanel_Image.Scroll -= FlowLayoutPanel1_Scroll;
            flowLayoutPanel_Robot.Scroll -= FlowLayoutPanel2_Scroll;
            flowLayoutPanel_Image.Scroll += FlowLayoutPanel1_Scroll;
            flowLayoutPanel_Robot.Scroll += FlowLayoutPanel2_Scroll;
        }
        private void FlowLayoutPanel1_Scroll(object sender, ScrollEventArgs e)
        {
            // 当 flowLayoutPanel1 的滚动条滚动时，调整 flowLayoutPanel2 的滚动位置
            flowLayoutPanel_Robot.AutoScrollPosition = new Point(flowLayoutPanel_Image.HorizontalScroll.Value, flowLayoutPanel_Image.VerticalScroll.Value);

        }

        private void FlowLayoutPanel2_Scroll(object sender, ScrollEventArgs e)
        {
            // 当 flowLayoutPanel2 的滚动条滚动时，调整 flowLayoutPanel1 的滚动位置
            flowLayoutPanel_Image.AutoScrollPosition = new Point(flowLayoutPanel_Robot.HorizontalScroll.Value, flowLayoutPanel_Robot.VerticalScroll.Value);
        }

        private TextBox[] textBoxesImageX = new TextBox[30]; // 数组用于存储 TextBox 控件的引用
        private TextBox[] textBoxesImageY = new TextBox[30]; // 数组用于存储 TextBox 控件的引用

        private TextBox[] textBoxesRobotX = new TextBox[30]; // 数组用于存储 TextBox 控件的引用
        private TextBox[] textBoxesRobotY = new TextBox[30]; // 数组用于存储 TextBox 控件的引用
        //private int[] values = new int[30];        // 数组用于存储 TextBox 输入的值

        private int number = 5;
        private void AddTextBox(FlowLayoutPanel flowLayoutPanel1  ,double[] XValue , 
            double[] YValue ,ref TextBox[] textBoxesX, ref TextBox[] textBoxesY)
        {
            try
            {
                flowLayoutPanel1.Controls.Clear();
                // 创建 TableLayoutPanel 控件
                TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
                tableLayoutPanel.Dock = DockStyle.Fill; // 填充整个窗体
                tableLayoutPanel.ColumnCount = 3;      // 两列，一列放 Label，一列放 TextBox
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F)); // 第一列占整个宽度的50%
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F)); // 第二列占整个宽度的50%
                tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F)); // 第二列占整个宽度的50%
                tableLayoutPanel.AutoSize = true;      // 自动调整大小
               

                // 将 TableLayoutPanel 添加到 FlowLayoutPanel 中
                flowLayoutPanel1.Controls.Add(tableLayoutPanel);
                // 设置 FlowLayoutPanel 的属性
                flowLayoutPanel1.AutoScroll = true;
                // 动态生成9组 Label 和 TextBox，并将它们添加到 TableLayoutPanel 中
                for (int i = 0; i < number; i++)
                {

                    // 创建 Label
                    Label label = new Label();
                    label.Text = $"第{i + 1}点:";
                    label.TextAlign = ContentAlignment.MiddleLeft; // 文本右对齐
                    label.Size = new Size(80, 20);
                    // 创建 TextBox
                    TextBox textBoxX = new TextBox();
                    textBoxX.Name = $"textBox{i + 1}";
                    textBoxX.Text = XValue[i].ToString();
                    textBoxX.Size = new Size(50, 20);

                    // 创建 TextBox
                    TextBox textBoxY = new TextBox();
                    textBoxY.Name = $"textBoxY{i + 1}";
                    textBoxY.Text = YValue[i].ToString();
                    textBoxY.Size = new Size(50, 20);

                    // 将 Label 和 TextBox 添加到 TableLayoutPanel 中的相应位置
                    tableLayoutPanel.Controls.Add(label, 0, i); // 第一列，第 i 行
                    tableLayoutPanel.Controls.Add(textBoxX, 1, i); // 第二列，第 i 行
                    tableLayoutPanel.Controls.Add(textBoxY, 2, i); // 第二列，第 i 行
                                                                   // 将 TextBox 添加到数组中
                    textBoxesX[i] = textBoxX;
                    textBoxesY[i] = textBoxY;


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
            
        }


        private void GettextBoxesValue(TextBox[] textBoxes,int number,ref double[] Value)
        {

            try
            {
                // 遍历 textBoxes 数组，获取每个 TextBox 的输入值并赋值到 values 数组
                for (int i = 0; i < number; i++)
                {
                    // 将 TextBox 的输入值转换为整数并赋值到 values 数组中
                    if (double.TryParse(textBoxes[i].Text, out double value))
                    {
                        Value[i] = value;
                    }
                    else
                    {
                        MessageBox.Show(textBoxes[i].Name+$"中的值无效，请输入。");
                        return; // 如果有任何一个 TextBox 的输入值无效，停止赋值操作
                    }

                }
            }
            catch(Exception ex) 
            {
                ShowErrorInfo(3, "请检查输入数据：" + ex.Message);

            }


        }

        private void but_FigureUpHomMat_Click(object sender, EventArgs e)
        {
            try
            {
                label_ErrMax.Text = "err";
                label_ErrMean.Text = "err";
                HTuple robotPointsX=new HTuple();
                HTuple robotPointsY = new HTuple();

                GettextBoxesValue(textBoxesImageX, number, ref PluginParams.ImageX);
                GettextBoxesValue(textBoxesImageY, number, ref PluginParams.ImageY);
                GettextBoxesValue(textBoxesRobotX, number, ref PluginParams.RobotX);
                GettextBoxesValue(textBoxesRobotY, number, ref PluginParams.RobotY);
                HTuple rbt_x = new HTuple();
                HOperatorSet.TupleGenConst(number, 0, out rbt_x);
                HTuple rbt_y = new HTuple();
                HOperatorSet.TupleGenConst(number, 0, out rbt_y);
                HTuple img_x = new HTuple();
                HOperatorSet.TupleGenConst(number, 0, out img_x);
                HTuple img_y = new HTuple();
                HOperatorSet.TupleGenConst(number, 0, out img_y);
                for (int i = 0; i < number; i++)
                {
                    img_x[i] = PluginParams.ImageX[i];
                    img_y[i] = PluginParams.ImageY[i];
                    rbt_x[i] = PluginParams.RobotX[i];
                    rbt_y[i] = PluginParams.RobotY[i];
                }
                HOperatorSet.VectorToHomMat2d(img_x, img_y, rbt_x, rbt_y, out PluginParams.HomMat2D);

                HOperatorSet.AffineTransPoint2d(PluginParams.HomMat2D, img_x, img_y, out  robotPointsX,
                out  robotPointsY);
                HTuple hv_err=0;
               
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_err = (((((robotPointsX - rbt_x)).TuplePow(
                        2)) + (((robotPointsY - rbt_y)).TuplePow(2)))).TupleSqrt();
                }
                HTuple hv_errMean= new HTuple();
                hv_errMean.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_errMean = hv_err.TupleMean()
                        ;

                    label_ErrMean.Text = hv_errMean.D.ToString("f2");

                }
                HTuple hv_errMax = 0;
                hv_errMax.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_errMax = hv_err.TupleMax()
                        ;
                    label_ErrMax.Text = hv_errMax.D.ToString("f2");
                }
                


            }
            catch(Exception ex)
            {

                MessageBox.Show("请检查输入数据："+ex.Message);
                ShowErrorInfo(3, "请检查输入数据：" + ex.Message);
            }
          
        }

        private void but_SaveHomMat_Click(object sender, EventArgs e)
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(Project.Instance.paramPathSet.ProcessPath);
                string ProcessPath = Path.Combine(directoryPath + "\\HomMat\\", pluginsObjBase.moduleInfo.ProcessName);

                string modelFilePath = Path.Combine(ProcessPath, pluginsObjBase.moduleInfo.ModuleName + ".txt");
                CommonApproach.DetectIsExist(modelFilePath);
                HOperatorSet.WriteTuple(PluginParams.HomMat2D, modelFilePath);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ShowErrorInfo(3, ex.Message);
            }
        }




        public override void Btn_RUN_Click(object sender, EventArgs e)
        {
            try
            {
                // 调用父窗体
                base.Btn_RUN_Click(sender, e);

                ShowErrorInfo(1, string.Format
                    ("输入图像坐标X:{0},Y:{0}  输出机械手坐标X:{2},Y:{3}", PluginParams.inputImageX, PluginParams.inputImageY
                    , PluginParams.outputRobotX.ToString("f2"), PluginParams.outputRobotY.ToString("f2")));

            }
            catch (Exception ex)
            {
                ShowErrorInfo(3, ex.Message);
            }
        }


        private static readonly object lockerUpdateError = new object();
        /// <summary>
        /// 显示报警信息（1表示绿色正常，2表示橘黄色警告，3表示红色报警，4表示蓝色不重要）
        /// </summary>
        /// <param name="Level">等级</param>
        /// <param name="Msg">信息</param>
        public void ShowErrorInfo(int Level, string Msg)
        {
            try
            {
                lock (lockerUpdateError)
                {
                    this.Invoke((EventHandler)delegate
                    {
                        //大于100行清空,函数可设定
                        if (rtx_ErrorInfo.Lines.Length >= 50)
                            rtx_ErrorInfo.Clear();
                        rtx_ErrorInfo.SelectionStart = rtx_ErrorInfo.Text.Length;    //设置插入符位置为文本框末
                        switch (Level)
                        {
                            //按照红绿灯的颜色显示，软件实现报警信息严重性提醒
                            case 1:
                                rtx_ErrorInfo.SelectionColor = Color.Green;//正常 绿色
                                break;
                            case 2:
                                rtx_ErrorInfo.SelectionColor = Color.Orange;//警告 橘黄色
                                break;
                            case 3:
                                rtx_ErrorInfo.SelectionColor = Color.Red;   //报警 红色
                                break;
                            default:
                                rtx_ErrorInfo.SelectionColor = Color.Blue;  //其它 不重要
                                break;
                        }

                        string DataTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fff");
                        rtx_ErrorInfo.AppendText(DataTime + "：" + Msg + "\r\n");    //输出文本，换行
                        rtx_ErrorInfo.ScrollToCaret();                               //滚动条滚到最新插入行
                    });
                }
            }
            catch (Exception)
            {
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                          System.Reflection.MethodBase.GetCurrentMethod().Name);
               
            }
        }

        private void Btn_Test_Click(object sender, EventArgs e)
        {
            try
            {
                double inputImageX = Convert.ToDouble(num_TestImageX.Value);
                double inputImageY = Convert.ToDouble(num_TestImageY.Value);
                HOperatorSet.AffineTransPoint2d(PluginParams.HomMat2D, inputImageX, inputImageY,
                    out HTuple outputRobotX, out HTuple outputRobotY);

                ShowErrorInfo(1, string.Format
                    ("输入图像坐标X:{0},Y:{0}  输出机械手坐标X:{2},Y:{3}", inputImageX.ToString("f2"), PluginParams.inputImageY.ToString("f2")
                    , outputRobotX.D.ToString("f2"), outputRobotY.D.ToString("f2")));

            }
            catch (Exception ex)
            {
                ShowErrorInfo(3, ex.Message);
            }
        }
    }
}
