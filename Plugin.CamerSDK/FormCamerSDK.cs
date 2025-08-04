using Frame_Core;
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

namespace Plugin.CamerSDK
{
    public partial class FormCamerSDK : FormPluginBase
    {
        private CamerSDkobj pluginsObj { get; set; }
        public ParamsCamerSDK PluginParams { get; set; }
        ImageInfo imageinfo;


        public FormCamerSDK()
        {
            InitializeComponent();
        }
        public override void Init()
        {
            pluginsObj = (CamerSDkobj)pluginsObjBase;
            PluginParams = (ParamsCamerSDK)pluginsObj.PluginParams;
            this.Text = pluginsObjBase.moduleInfo.ModuleName;
            //测试存参用
        }
        private void Btn_LoadLoacalImage_Click(object sender, EventArgs e)
        {
            //打开指定路径文件夹，加载图片
            OpenFileDialog FileDialog = new OpenFileDialog();
            FileDialog.Multiselect = true;
            FileDialog.InitialDirectory = System.Environment.CurrentDirectory;
            HTuple Hv_Width, Hv_Height;
            if (FileDialog.ShowDialog() == DialogResult.OK)
            {
                PluginParams.ImageList_OffLine.Clear();

                for (int i = 0; i < FileDialog.FileNames.Length; i++)
                {
                    imageinfo = new ImageInfo();
                    imageinfo.ho_image = new HImage(FileDialog.FileNames[i]);
                    imageinfo.image_ID = i;
                    imageinfo.cameraName = PluginParams.str_CamName;
                    HOperatorSet.GetImageSize(imageinfo.ho_image, out Hv_Width, out Hv_Height);
                    imageinfo.imageWidth = Hv_Width.D;
                    imageinfo.imageHeight = Hv_Height.D;
                    PluginParams.ImageList_OffLine.Add(imageinfo);
                }
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            pluginsObj.ExecuteObj();
            HTuple WindowID1 = hWindowControl1.HalconWindow;
            HTuple ImageHeight, ImageWidth;
            int index = pluginsObj.index;
            if (index == 0)
            {
                index = PluginParams.ImageList_OffLine.Count-1;
            }
            else
            {
                index -= 1;
            }
            if (PluginParams.ImageList_OffLine.Count > 0)
            {
                HOperatorSet.GetImageSize(PluginParams.ImageList_OffLine[index].ho_image, out ImageWidth, out ImageHeight);
                HOperatorSet.SetPart(WindowID1, 0, 0, ImageHeight, ImageWidth);
                HOperatorSet.DispObj(PluginParams.ImageList_OffLine[index].ho_image, WindowID1);
                Disp_Text(hWindowControl1, 100, 200, "green", "图片编号：" + index.ToString());
                label1.Text = PluginParams.str_CamName;
            }
        }

        public void Disp_Text(HWindowControl win, int Row, int Column, string color, string text)
        {
            try
            {
                //设置字体颜色
                win.HalconWindow.SetColor(color);
                //设置字体风格 

                HTuple hv_Font;
                HTuple hv_FontWithSize = null;
                // 获取halcon支持的字体
                HOperatorSet.QueryFont(hWindowControl1.HalconWindow, out hv_Font);
                //50表示字体大小，括号中的数字表示第几种字体
                hv_FontWithSize = hv_Font.TupleSelect(0) + "-20";//缺点在此，不知道想要的黑体排第几，所以括号里不知道填几
                win.HalconWindow.SetFont(hv_FontWithSize);

                // win.HalconWindow.SetFont("CourierNew-Bold-20");//用不了
                //设置显示的位置（坐标）
                win.HalconWindow.SetTposition(Row, Column);
                //设置显示的内容
                win.HalconWindow.WriteString(text);
            }
            catch (Exception ee)
            {
                System.Windows.Forms.MessageBox.Show(text);
            }
        }

    }
}
