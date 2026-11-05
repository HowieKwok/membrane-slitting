using Frame_Core;
using HalconDotNet;
using LogTool;
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

namespace Frame_Core
{
    public partial class SaveSetingForm : Form
    {
      
        public SaveSetingForm()
        {
            InitializeComponent();
         
        }

        private void BaseSetingForm_Load(object sender, EventArgs e)
        {
            try
            {

                string[] strs = Enum.GetNames(typeof(ImageSaveFormat));
                foreach (var item in strs)
                {
                    cbx_SaveImageFormat_Primitive.Items.Add(item);                 
                    cbx_SaveImageFormat_Screenshot.Items.Add(item);
                    cbx_SaveImageFormat_Min.Items.Add(item);
                    cbx_SaveImageFormat_ProdImg.Items.Add(item);
                    cbx_SaveImageFormat_ProdScreenshot.Items.Add(item);
                }
                strs = Enum.GetNames(typeof(ImageDeepthSaveFormat));
                foreach (var item in strs)
                {
                    cbx_SaveImageFormat_Deepth.Items.Add(item);
                }

                strs = Enum.GetNames(typeof(ImageSaveSelect));
                foreach (var item in strs)
                {
                    cbx_SaveImageSelect_Primitive.Items.Add(item);
                    cbx_SaveImageSelect_ProdImg.Items.Add(item);
                    cbx_SaveImageSelect_Screenshot.Items.Add(item);
                    cbx_SaveImageSelect_ProdScreenshot.Items.Add(item);
                }

                //加载图片保存参数
                //=========================选择项控件
                //选择项控件
                this.Tv_Select.HideSelection = false;
                //自已绘制  
                this.Tv_Select.DrawMode = TreeViewDrawMode.OwnerDrawText;


                //初始化相机节点
                Tv_Select.Nodes.Clear();
                foreach (var item in Service_Camera.GetCam_Names())
                {
                    Tv_Select.Nodes.Add(item);
                }

                //展开节点
                Tv_Select.ExpandAll();

                //初始节点
                Tv_Select.SelectedNode = Tv_Select.Nodes[0];

                foreach (var item in Service_Camera.Get_AllCameraRelevancyList())
                {
                    item.m_ImageSaveParamer.backup(item.m_ImageSaveParamer);
                    YControlAutobind.Loading<ImageSaveParamer, IYCustomAttribute>(this, item.m_ImageSaveParamer);//测试存修改记录
                }
            }
            catch (Exception)
            {


            }

        }


        private void BaseSetingForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        private void btn_SaveSystemInfo_Click(object sender, EventArgs e)
        {
            try
            {
                //保存图片设置参数
                foreach (var item in Service_Camera.Get_AllCameraRelevancies())
                {
                    int index = Tv_Select.SelectedNode.Index;
                    if ((int)item.Key == index)
                    {
                        ControlTool.GetParamSave(cbx_SaveImageFormat_Primitive, ref item.Value.Item2.m_ImageSaveParamer.SaveImageFormat_Primitive);
                        ControlTool.GetParamSave(cbx_SaveImageSelect_Primitive, ref item.Value.Item2.m_ImageSaveParamer.SaveImageSelect_Primitive);
                        ControlTool.GetParamSave(cbx_SaveImageFormat_Deepth, ref item.Value.Item2.m_ImageSaveParamer.SaveImageFormat_Deepth);
                        ControlTool.GetParamSave(chx_IsCompression, ref item.Value.Item2.m_ImageSaveParamer.IsCompression_Deepth);
                        ControlTool.GetParamSave(cbx_IsShowImage_Primitive, ref item.Value.Item2.m_ImageSaveParamer.IsShowImage_Primitive);
                        ControlTool.GetParamSave(cbx_IsShowImage_Product, ref item.Value.Item2.m_ImageSaveParamer.IsShowImage_Product);
                        ControlTool.GetParamSave(cbx_SaveImageFormat_Screenshot, ref item.Value.Item2.m_ImageSaveParamer.SaveImageFormat_Screenshot);
                        ControlTool.GetParamSave(cbx_SaveImageSelect_Screenshot, ref item.Value.Item2.m_ImageSaveParamer.SaveImageSelect_Screenshot);
                        ControlTool.GetParamSave(cbx_HighDefinition_Screenshot, ref item.Value.Item2.m_ImageSaveParamer.HighDefinition_Screenshot);
                        ControlTool.GetParamSave(cbx_SaveImageFormat_Min, ref item.Value.Item2.m_ImageSaveParamer.SaveImageFormat_Min);
                        ControlTool.GetParamSave(cbx_IsSaveImage_Min, ref item.Value.Item2.m_ImageSaveParamer.IsSaveImage_Min);
                        ControlTool.GetParamSave(cbx_IsShowImage_Min, ref item.Value.Item2.m_ImageSaveParamer.IsShowImage_Min);
                        ControlTool.GetParamSave(cbx_SaveImageFormat_ProdImg, ref item.Value.Item2.m_ImageSaveParamer.SaveImageFormat_ProdImg);
                        ControlTool.GetParamSave(cbx_SaveImageSelect_ProdImg, ref item.Value.Item2.m_ImageSaveParamer.SaveImageSelect_ProdImg);
                        ControlTool.GetParamSave(cbx_SaveImageFormat_ProdScreenshot, ref item.Value.Item2.m_ImageSaveParamer.SaveImageFormat_ProdScreenshot);
                        ControlTool.GetParamSave(cbx_SaveImageSelect_ProdScreenshot, ref item.Value.Item2.m_ImageSaveParamer.SaveImageSelect_ProdScreenshot);
                        ControlTool.GetParamSave(cbx_HighDefinition_ProdScreenshot, ref item.Value.Item2.m_ImageSaveParamer.HighDefinition_ProdScreenshot);
                        ControlTool.GetParamSave(nud_IsDebug, ref item.Value.Item2.m_ImageSaveParamer.IsDebug);
                        ControlTool.GetParamSave(cbx_EnNSaveImage, ref item.Value.Item2.m_ImageSaveParamer.EnNSaveImage);
                        ControlTool.GetParamSave(nud_NSaveImageNum, ref item.Value.Item2.m_ImageSaveParamer.NSaveImageNum);
                        item.Value.Item2.m_ImageSaveParamer.Save();

                        //修改记录设置
                        item.Value.Item2.m_ImageSaveParamer.FilePath = Project.Instance.logParam.LogSavePath + "\\ParamsChangeLog\\";
                        // M_craftBase.FilePath = (Application.StartupPath + "\\record\\参数修改记录\\视觉设置\\");
                        string error = YControlAutobind.Saveing<ImageSaveParamer, IYCustomAttribute>(this, item.Value.Item2.m_ImageSaveParamer);
                        // 保存修改记录，保存后备份
                        item.Value.Item2.m_ImageSaveParamer.saveChangeLog(item.Value.Item2.m_ImageSaveParamer, String.Format("相机保存设置:{0} 中 {1}", item.Key.ToString(), ""));
                        item.Value.Item2.m_ImageSaveParamer.backup(item.Value.Item2.m_ImageSaveParamer);
                        if (error != "") System.Windows.Forms.MessageBox.Show(error);
                        

                        break;
                    }
                }

            }
            catch (Exception)
            {


            }
        }



        #region 选择项控件
        /// 绘制颜色
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tv_Select_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            //e.DrawDefault = true; //我这里用默认颜色即可，只需要在TreeView失去焦点时选中节点仍然突显  
            //return;

            //or  自定义颜色  
            if ((e.State & TreeNodeStates.Selected) != 0)
            {
                //演示为绿底白字  
                e.Graphics.FillRectangle(Brushes.DodgerBlue, e.Node.Bounds);

                Font nodeFont = e.Node.NodeFont;
                if (nodeFont == null) nodeFont = ((TreeView)sender).Font;
                e.Graphics.DrawString(e.Node.Text, nodeFont, Brushes.White, Rectangle.Inflate(e.Bounds, 2, 0));
            }
            else
            {
                e.DrawDefault = true;
            }

            if ((e.State & TreeNodeStates.Focused) != 0)
            {
                using (Pen focusPen = new Pen(Color.Black))
                {
                    focusPen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    Rectangle focusBounds = e.Node.Bounds;
                    focusBounds.Size = new Size(focusBounds.Width - 1,
                    focusBounds.Height - 1);
                    e.Graphics.DrawRectangle(focusPen, focusBounds);
                }
            }

        }

        private void Tv_Select_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                foreach (var item in Service_Camera.Get_AllCameraRelevancies())
                {
                    int index = Tv_Select.SelectedNode.Index;
                    if ((int)item.Key == index)
                    {
                        ControlTool.SetParamShow(cbx_SaveImageFormat_Primitive, ref item.Value.Item2.m_ImageSaveParamer.SaveImageFormat_Primitive);
                        ControlTool.SetParamShow(cbx_SaveImageSelect_Primitive, ref item.Value.Item2.m_ImageSaveParamer.SaveImageSelect_Primitive);
                        ControlTool.SetParamShow(cbx_SaveImageFormat_Deepth, ref item.Value.Item2.m_ImageSaveParamer.SaveImageFormat_Deepth);
                        ControlTool.SetParamShow(chx_IsCompression, ref item.Value.Item2.m_ImageSaveParamer.IsCompression_Deepth);
                        ControlTool.SetParamShow(cbx_IsShowImage_Primitive, ref item.Value.Item2.m_ImageSaveParamer.IsShowImage_Primitive);
                        ControlTool.SetParamShow(cbx_IsShowImage_Product, ref item.Value.Item2.m_ImageSaveParamer.IsShowImage_Product);
                        ControlTool.SetParamShow(cbx_SaveImageFormat_Screenshot, ref item.Value.Item2.m_ImageSaveParamer.SaveImageFormat_Screenshot);
                        ControlTool.SetParamShow(cbx_SaveImageSelect_Screenshot, ref item.Value.Item2.m_ImageSaveParamer.SaveImageSelect_Screenshot);
                        ControlTool.SetParamShow(cbx_HighDefinition_Screenshot, ref item.Value.Item2.m_ImageSaveParamer.HighDefinition_Screenshot);
                        ControlTool.SetParamShow(cbx_SaveImageFormat_Min, ref item.Value.Item2.m_ImageSaveParamer.SaveImageFormat_Min);
                        ControlTool.SetParamShow(cbx_IsSaveImage_Min, ref item.Value.Item2.m_ImageSaveParamer.IsSaveImage_Min);
                        ControlTool.SetParamShow(cbx_IsShowImage_Min, ref item.Value.Item2.m_ImageSaveParamer.IsShowImage_Min);
                        ControlTool.SetParamShow(cbx_SaveImageFormat_ProdImg, ref item.Value.Item2.m_ImageSaveParamer.SaveImageFormat_ProdImg);
                        ControlTool.SetParamShow(cbx_SaveImageSelect_ProdImg, ref item.Value.Item2.m_ImageSaveParamer.SaveImageSelect_ProdImg);
                        ControlTool.SetParamShow(cbx_SaveImageFormat_ProdScreenshot, ref item.Value.Item2.m_ImageSaveParamer.SaveImageFormat_ProdScreenshot);
                        ControlTool.SetParamShow(cbx_SaveImageSelect_ProdScreenshot, ref item.Value.Item2.m_ImageSaveParamer.SaveImageSelect_ProdScreenshot);
                        ControlTool.SetParamShow(cbx_HighDefinition_ProdScreenshot, ref item.Value.Item2.m_ImageSaveParamer.HighDefinition_ProdScreenshot);
                        ControlTool.SetParamShow(nud_IsDebug, ref item.Value.Item2.m_ImageSaveParamer.IsDebug);
                        ControlTool.SetParamShow(cbx_EnNSaveImage, ref item.Value.Item2.m_ImageSaveParamer.EnNSaveImage);
                        ControlTool.SetParamShow(nud_NSaveImageNum, ref item.Value.Item2.m_ImageSaveParamer.NSaveImageNum);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
            }
        }

        #endregion

        private void cbx_HighDefinition_ProdScreenshot_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
