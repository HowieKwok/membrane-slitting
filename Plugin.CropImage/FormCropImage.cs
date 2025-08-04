using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Frame_Core;
using HalconDotNet;
using ViewROI;
using System.IO;
using HalconAlgorithm;

namespace Plugin.CropImage
{
    public partial class FormCropImage : FormPluginBase
    {
        public FormCropImage()
        {
            InitializeComponent();
            hWndCtrl_Display = new HWndCtrl(HWC_Display);
            hWndCtrl_Display.useROIController(roiController);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_ZOOM);
            hWndCtrl_Display.setViewState(HWndCtrl.MODE_VIEW_MOVE);
        }
        public override void Init()
        {
            pluginsObj = (CropImageObj)pluginsObjBase;
            PluginParams = (ParamsCropImage)pluginsObj.PluginParams;
            this.Text = pluginsObjBase.moduleInfo.ModuleName;

            LoadStandImage();
            INI_Params();
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_SmallRectangle2);
            //测试存参用
        }
        private CropImageObj pluginsObj { get; set; }
        public ParamsCropImage PluginParams { get; set; }
        ImageInfo imageinfo;

        public HObject ho_Image1 = null;
        public HWndCtrl hWndCtrl_Display;

        public ROIController roiController = new ROIController();
        public ROIRectangle2 roiRectangle1 = new ROIRectangle2();
        public ROIRectangle2 roiRectangle2 = new ROIRectangle2();
        bool bl_RoiExist = false;

        public HObject ho_Rectangle2;
        public HObject ho_SmallRectangle2;


        private void INI_Params()
        {
            try
            {
                num_ColBegin.Value = (decimal)PluginParams.colBegin;
                num_CropWidth.Value = (decimal)PluginParams.cropWidth;
                num_CropHeight.Value = (decimal)PluginParams.cropHeight;
            }
            catch (Exception ee)
            {
            }

        }
        private void LoadImage_Click(object sender, EventArgs e)
        {
            //打开指定路径文件夹，加载图片
            OpenFileDialog FileDialog = new OpenFileDialog();
            FileDialog.InitialDirectory = "E:\\";
            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    PluginParams.ho_Image = new HImage(FileDialog.FileName);
                    ho_Image1 = PluginParams.ho_Image;
                    hWndCtrl_Display.addIconicVar(PluginParams.ho_Image);

                    // SetRoiParams(PluginParams, roiRectangle2, roiController);
                    hWndCtrl_Display.resetAll();
                    hWndCtrl_Display.repaint();

                }
                catch (Exception)
                {
                }
            }
        }

        private void LoadStandImage()
        {
            if (PluginParams.ho_Image.CountObj() == 0)
            {
                string standImagePath = "";
                if (File.Exists(standImagePath))
                {
                    HOperatorSet.ReadImage(out PluginParams.ho_Image, standImagePath);
                }
            }
            try
            {
                HImage hImage = new HImage();
                HobjectToHimage(PluginParams.ho_Image, ref hImage);
                hWndCtrl_Display.addIconicVar(hImage);
            }
            catch { }
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

        private void num_ColBegin_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.colBegin = Convert.ToDouble(num_ColBegin.Value);
            ShowROI();

        }

        private void num_CropWidth_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.cropWidth = Convert.ToInt32(num_CropWidth.Value);
            ShowROI();

        }

        private void num_CropHeight_ValueChanged(object sender, EventArgs e)
        {
            PluginParams.cropHeight = Convert.ToInt32(num_CropHeight.Value);
            ShowROI();

        }

        private void ShowROI()
        {
            HTuple hv_Phi;
            HOperatorSet.TupleRad(0, out hv_Phi);
            HTuple hv_Width, hv_Height;
            HOperatorSet.GetImageSize(PluginParams.ho_Image, out hv_Width, out hv_Height);

            roiRectangle1.PointNum = 1;
            roiRectangle1.ROIName = "裁图区1";
            roiRectangle1.midC = PluginParams.colBegin + PluginParams.cropWidth / 2;
            roiRectangle1.midR = hv_Height - PluginParams.cropHeight / 2;
            roiRectangle1.phi = hv_Phi.D;
            roiRectangle1.length2 = PluginParams.cropHeight / 2;
            roiRectangle1.length1 = PluginParams.cropWidth / 2;

            roiRectangle2.PointNum = 1;
            roiRectangle2.ROIName = "裁图区2";
            roiRectangle2.midC = PluginParams.colBegin + PluginParams.cropWidth / 2;
            roiRectangle2.midR = PluginParams.cropHeight / 2;
            roiRectangle2.phi = hv_Phi.D;
            roiRectangle2.length2 = PluginParams.cropHeight / 2;
            roiRectangle2.length1 = PluginParams.cropWidth / 2;

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

                roiController.setROIShape(roiRectangle1);
                roiController.roiMode.createROI(roiRectangle1.midC, roiRectangle1.midR);
                roiController.ROIList.Add(roiController.roiMode);
            }

            roiController.activeROIidx = -1;
            roiRectangle2.updateHandlePos();
            roiRectangle1.updateHandlePos();
            hWndCtrl_Display.repaint();
        }

        private void ShowMiniImage(HObject himage0, HObject himage1, HObject himage2, HObject himage3, HObject himage4)
        {
            ////显示缩略图到图像选项
            if (himage0 == null) return;
            if (himage0.CountObj() != 0)
            {
                PaintWindows.DisplayImage(himage0, HWC_Image0);
            }
            else
            {
                HOperatorSet.ClearWindow(HWC_Image0.HalconWindow);
            }
            //
            if (himage1 == null) return;
            if (himage1.CountObj() != 0)
            {
                PaintWindows.DisplayImage(himage1, HWC_Image1);
            }
            else
            {
                HOperatorSet.ClearWindow(HWC_Image1.HalconWindow);
            }
            //
            if (himage2 == null) return;
            if (himage2.CountObj() != 0)
            {
                PaintWindows.DisplayImage(himage2, HWC_Image2);
            }
            else
            {
                HOperatorSet.ClearWindow(HWC_Image2.HalconWindow);
            }
            //
            if (himage3 == null) return;
            if (himage3.CountObj() != 0)
            {
                PaintWindows.DisplayImage(himage3, HWC_Image3);
                HTuple hv_width, hv_heigth;
                HObject RegionLines = new HObject();
                HOperatorSet.GetImageSize(himage3, out hv_width, out hv_heigth);
                HOperatorSet.GenRegionLine(out RegionLines, hv_heigth / 2, 1, hv_heigth / 2, hv_width);
                HOperatorSet.DispLine(HWC_Image3.HalconWindow, hv_heigth / 2, 1, hv_heigth / 2, hv_width);
                // PaintWindows.DisplayXLD(RegionLines, himage3, HWC_Image3);
            }
            else
            {
                HOperatorSet.ClearWindow(HWC_Image3.HalconWindow);
            }
            //
            if (himage4 == null) return;
            if (himage4.CountObj() != 0)
            {
                PaintWindows.DisplayImage(himage4, HWC_Image4);
            }
            else
            {
                HOperatorSet.ClearWindow(HWC_Image4.HalconWindow);
            }
        }




        private void button1_Click(object sender, EventArgs e)
        {
            pluginsObj.ExecuteObj();
            ShowMiniImage(PluginParams.ho_CropImage_Lastbottom, PluginParams.ho_CropIamge_top, PluginParams.ho_CropIamge_bottom, PluginParams.ho_TiledIamge, PluginParams.ho_Image);
        }

        private void FullScrean_Click(object sender, EventArgs e)
        {

            //重置显示画面
            hWndCtrl_Display.resetAll();
            hWndCtrl_Display.repaint();
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

        private void HWC_Image0_HMouseDown(object sender, HMouseEventArgs e)
        {
            if (PluginParams.ho_CropImage_Lastbottom.CountObj() != 0)
            {
                hWndCtrl_Display.bl_RoiOperateFlag = false;
                //显示原图
                HImage hImage = new HImage();
                HobjectToHimage(PluginParams.ho_CropImage_Lastbottom, ref hImage);
                hWndCtrl_Display.addIconicVar(hImage);
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();
                LB_IMageSize.Text = ShowIMageSize(hImage);
            }
        }

        private void HWC_Image1_HMouseDown(object sender, HMouseEventArgs e)
        {
            if (PluginParams.ho_CropIamge_top.CountObj() != 0)
            {
                hWndCtrl_Display.bl_RoiOperateFlag = false;
                //显示原图
                HImage hImage = new HImage();
                HobjectToHimage(PluginParams.ho_CropIamge_top, ref hImage);
                hWndCtrl_Display.addIconicVar(hImage);
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();
                LB_IMageSize.Text = ShowIMageSize(hImage);
            }
        }

        private void HWC_Image2_HMouseDown(object sender, HMouseEventArgs e)
        {
            if (PluginParams.ho_CropIamge_bottom.CountObj() != 0)
            {
                hWndCtrl_Display.bl_RoiOperateFlag = false;
                //显示原图
                HImage hImage = new HImage();
                HobjectToHimage(PluginParams.ho_CropIamge_bottom, ref hImage);
                hWndCtrl_Display.addIconicVar(hImage);
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();
                LB_IMageSize.Text = ShowIMageSize(hImage);
            }
        }

        private void HWC_Image3_HMouseDown(object sender, HMouseEventArgs e)
        {
            if (PluginParams.ho_TiledIamge.CountObj() != 0)
            {
                hWndCtrl_Display.bl_RoiOperateFlag = false;
                //显示原图
                HImage hImage = new HImage();
                HobjectToHimage(PluginParams.ho_TiledIamge, ref hImage);
                hWndCtrl_Display.addIconicVar(hImage);
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();
                LB_IMageSize.Text = ShowIMageSize(hImage);
            }
        }

        private void HWC_Image4_HMouseDown(object sender, HMouseEventArgs e)
        {
            if (PluginParams.ho_Image.CountObj() != 0)
            {
                hWndCtrl_Display.bl_RoiOperateFlag = false;
                //显示原图
                HImage hImage = new HImage();
                HobjectToHimage(PluginParams.ho_Image, ref hImage);
                hWndCtrl_Display.addIconicVar(hImage);
                hWndCtrl_Display.resetAll();
                hWndCtrl_Display.repaint();
                LB_IMageSize.Text = ShowIMageSize(hImage);
            }
        }

    }
}
