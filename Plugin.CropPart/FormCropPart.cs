using HalconAlgorithm;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ViewROI;
using YYSD.UICore.Control;
using YYSD.UICore;
using Frame_Core;

namespace Plugin.CropPart
{
    public partial class FormCropPart : FormPluginBase
    {
        private CropPartObj pluginsObj { get; set; }
        private Process proc { get; set; }
        public ParamsCropPart PluginParams { get; set; }

        public HObject ho_Image = null;
        public HWndCtrl hWndCtrl_Display;
        public ROIController roiController = new ROIController();
        public ROIRectangle2 roiRectangle2 = new ROIRectangle2();

        public bool IsLoaded = false;
        public FormCropPart()
        {
            InitializeComponent();
            hWndCtrl_Display = new HWndCtrl(HWC_Display);
            hWndCtrl_Display.useROIController(roiController);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_MOVE);
            hWndCtrl_Display.ImageProcessingEvent += new HWndCtrl.ImageProcessingDelegate(RunFunction);   //更改测量矩形，执行测量方法
            hWndCtrl_Display.ReDrawEvent += new HWndCtrl.ReDrawDelegate(ShowResult);     //更改测量矩形，重绘测量结果
        }
        public override void Init()
        {
            pluginsObj = (CropPartObj)pluginsObjBase;
            PluginParams = (ParamsCropPart)pluginsObj.PluginParams;
            //PluginParams.backup(PluginParams);

            proc = Project.Instance.GetProcessByName(pluginsObj.moduleInfo.ProcessName);
            this.Text = pluginsObjBase.moduleInfo.ModuleName;
            ShowModuleParams(PluginParams);
            //测试存参用
            if (ho_Image != null)
            {
                PluginParams.Image_part = ho_Image;
            }
            try
            {
                HImage hImage = new HImage();
                HobjectToHimage(PluginParams.Image_part, ref hImage);
                hWndCtrl_Display.addIconicVar(hImage);
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();
            }
            catch (Exception ee) { }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void Btn_Save_Click(object sender, EventArgs e)
        {
            //新的实现逻辑
            //string error = "";
            //视觉设置
            PluginParams.FilePath = (Application.StartupPath + "\\record\\参数修改记录\\视觉设置\\");
           // error = YControlAutobind.Saveing<PluginParamsBase, IYCustomAttribute>(this, PluginParams);
            // 调用父窗体
            base.Btn_Save_Click(sender, e);
            PluginParams.saveChangeLog(PluginParams, String.Format("测试视觉设置:{0} 中 {1}", pluginsObj.moduleInfo.ProcessName, pluginsObj.moduleInfo.ModuleName));
            PluginParams.backup(PluginParams);
           // if (error != "") System.Windows.Forms.MessageBox.Show(error);

        }

        private void RunFunction()
        {

        }

        /// <summary>
        /// ViewRoi回调函数：显示检测结果
        /// </summary>
        private void ShowResult()
        {
            if (roiController.activeROIidx == -1)
            {
                HObject ho_Rectangle2, ho_Point;

                HOperatorSet.GenEmptyObj(out ho_Rectangle2);
                HOperatorSet.GenEmptyObj(out ho_Point);

                //显示缩略图到图形选项中
                if (PluginParams.Image_part.CountObj() != 0)
                {
                    PaintWindows.DisplayImage(PluginParams.Image_part, HWC_Image1);
                }
                else
                {
                    HOperatorSet.ClearWindow(HWC_Image1.HalconWindow);
                }
            }
        }
        /// <summary>
        /// 加载抓边参数到窗体
        /// </summary>
        /// <param name="ParamsFH"></param>
        private void ShowModuleParams(ParamsCropPart ParamsFH)
        {
            try
            {
                NUD_PartCenterID.Value = Convert.ToDecimal(ParamsFH.PartCenterID);
                NUD_PartCenterRow.Value = Convert.ToDecimal(ParamsFH.PartCenterRow);
                NUD_PartCenterCol.Value = Convert.ToDecimal(ParamsFH.PartCenterCol);

                NUD_PartSizeRow.Value = Convert.ToDecimal(ParamsFH.PartSizeRow);
                NUD_PartSizeCol.Value = Convert.ToDecimal(ParamsFH.PartSizeCol);

                NUD_CropShaftRow.Value = Convert.ToDecimal(ParamsFH.CropShaftRow);
                NUD_CropShaftCol.Value = Convert.ToDecimal(ParamsFH.CropShaftCol);

                IsLoaded = true;
            }
            catch (Exception ee)
            {
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

        private void NUD_PartCenterID_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            PluginParams.PartCenterID = Convert.ToInt32(NUD_PartCenterID.Value);
        }

        private void NUD_PartCenterRow_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            PluginParams.PartCenterRow = Convert.ToDouble(NUD_PartCenterRow.Value);
        }

        private void NUD_PartCenterCol_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            PluginParams.PartCenterCol = Convert.ToDouble(NUD_PartCenterCol.Value);
        }

        private void NUD_PartSizeRow_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            PluginParams.PartSizeRow = Convert.ToDouble(NUD_PartSizeRow.Value);
        }

        private void NUD_PartSizeCol_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            PluginParams.PartSizeCol = Convert.ToDouble(NUD_PartSizeCol.Value);
        }

        private void NUD_CropShaftRow_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            PluginParams.CropShaftRow = Convert.ToDouble(NUD_CropShaftRow.Value);
        }

        private void NUD_CropShaftCol_ValueChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            PluginParams.CropShaftCol = Convert.ToDouble(NUD_CropShaftCol.Value);
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
            catch (Exception ee)
            {
                return str;
            }
        }

        private void FullScrean_Click(object sender, EventArgs e)
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

    }
}
