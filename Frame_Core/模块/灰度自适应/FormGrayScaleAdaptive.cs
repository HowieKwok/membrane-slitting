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

namespace Frame_Core
{
    public partial class FormGrayScaleAdaptive : Form
    {
        CameraEnum currentCamera = CameraEnum.Cam1;
        CameraManager currentCameraManager;
        Dictionary<CameraEnum, CameraManager> cameraManager;
        public FormGrayScaleAdaptive(Dictionary<CameraEnum, CameraManager> _cameraManager)
        {
            InitializeComponent();
            cameraManager = _cameraManager;
        }

        private void FormGrayScaleAdaptive_Load(object sender, EventArgs e)
        {
            try
            {


                //=========================选择项控件
                //选择项控件
                this.Tv_Select.HideSelection = false;
                //自已绘制  
                this.Tv_Select.DrawMode = TreeViewDrawMode.OwnerDrawText;


                //初始化相机节点
                Tv_Select.Nodes.Clear();
                foreach (var item in cameraManager)
                {
                    if(item.Key==CameraEnum.Cam1|| item.Key == CameraEnum.Cam2) 
                    {
                        Tv_Select.Nodes.Add(item.Key.ToString());
                    }
                   
                }

                //展开节点
                Tv_Select.ExpandAll();
                //初始节点
                Tv_Select.SelectedNode = Tv_Select.Nodes[0];

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        private void FormGrayScaleAdaptive_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

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

                foreach (var item in cameraManager)
                {
                    if (Tv_Select.SelectedNode.Text == item.Key.ToString())
                    {
                        currentCamera = item.Key;
                        currentCameraManager = item.Value;
                        break;
                    }
                }


                ControlTool.SetParamShow(cb_Enable, ref currentCameraManager.m_GrayScaleAdaptationParameter.Enable);
                ControlTool.SetParamShow(nud_TargetGray, ref currentCameraManager.m_GrayScaleAdaptationParameter.TargetGray);
                ControlTool.SetParamShow(nud_GrayRangeMin, ref currentCameraManager.m_GrayScaleAdaptationParameter.GrayRangeMin);
                ControlTool.SetParamShow(nud_GrayRangeMax, ref currentCameraManager.m_GrayScaleAdaptationParameter.GrayRangeMax);
                ControlTool.SetParamShow(nud_ContinuousNFrameOutOfScopeGray, ref currentCameraManager.m_GrayScaleAdaptationParameter.ContinuousNFrameOutOfScopeGray);
                ControlTool.SetParamShow(nud_AdjustmentGrayTolerance, ref currentCameraManager.m_GrayScaleAdaptationParameter.AdjustmentGrayTolerance);
                ControlTool.SetParamShow(cb_ContinuousNFrameOutOfScopeGrayAlarm, ref currentCameraManager.m_GrayScaleAdaptationParameter.ContinuousNFrameOutOfScopeGrayAlarm);
                ControlTool.SetParamShow(nud_TargetGrayTolerance, ref currentCameraManager.m_GrayScaleAdaptationParameter.TargetGrayTolerance);
                ControlTool.SetParamShow(nud_SingleAdjustmentRatio, ref currentCameraManager.m_GrayScaleAdaptationParameter.SingleAdjustmentRatio);
                ControlTool.SetParamShow(nud_AdjustFactor, ref currentCameraManager.m_GrayScaleAdaptationParameter.AdjustFactor);
                ControlTool.SetParamShow(nud_IntervalFrame, ref currentCameraManager.m_GrayScaleAdaptationParameter.IntervalFrame);
                ControlTool.SetParamShow(nud_AdjustFinishNum, ref currentCameraManager.m_GrayScaleAdaptationParameter.AdjustFinishNum);
                ControlTool.SetParamShow(cb_NNotAdjustFinishAlarm, ref currentCameraManager.m_GrayScaleAdaptationParameter.NNotAdjustFinishAlarm);
                ControlTool.SetParamShow(nud_GrayBodong, ref currentCameraManager.m_GrayScaleAdaptationParameter.GrayBodong);
                ControlTool.SetParamShow(cb_GrayBodongAlarm, ref currentCameraManager.m_GrayScaleAdaptationParameter.GrayBodongAlarm);
                ControlTool.SetParamShow(nud_LightSourcePort, ref currentCameraManager.m_GrayScaleAdaptationParameter.LightSourcePort);
                ControlTool.SetParamShow(nud_LightSourceChannel, ref currentCameraManager.m_GrayScaleAdaptationParameter.LightSourceChannel);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                GetFOrmParam(ref currentCameraManager.m_GrayScaleAdaptationParameter);
                currentCameraManager.m_GrayScaleAdaptationParameter.Save();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                MessageBox.Show(ex.ToString());
            }
        }

        public void GetFOrmParam(ref GrayScaleAdaptationParameter grayScaleAdaptationParameter)
        {
            try
            {
                ControlTool.GetParamSave(cb_Enable, ref grayScaleAdaptationParameter.Enable);
                ControlTool.GetParamSave(nud_TargetGray, ref grayScaleAdaptationParameter.TargetGray);
                ControlTool.GetParamSave(nud_GrayRangeMin, ref grayScaleAdaptationParameter.GrayRangeMin);
                ControlTool.GetParamSave(nud_GrayRangeMax, ref grayScaleAdaptationParameter.GrayRangeMax);
                ControlTool.GetParamSave(nud_ContinuousNFrameOutOfScopeGray, ref grayScaleAdaptationParameter.ContinuousNFrameOutOfScopeGray);
                ControlTool.GetParamSave(nud_AdjustmentGrayTolerance, ref grayScaleAdaptationParameter.AdjustmentGrayTolerance);
                ControlTool.GetParamSave(cb_ContinuousNFrameOutOfScopeGrayAlarm, ref grayScaleAdaptationParameter.ContinuousNFrameOutOfScopeGrayAlarm);
                ControlTool.GetParamSave(nud_TargetGrayTolerance, ref grayScaleAdaptationParameter.TargetGrayTolerance);
                ControlTool.GetParamSave(nud_SingleAdjustmentRatio, ref grayScaleAdaptationParameter.SingleAdjustmentRatio);
                ControlTool.GetParamSave(nud_AdjustFactor, ref grayScaleAdaptationParameter.AdjustFactor);
                ControlTool.GetParamSave(nud_IntervalFrame, ref grayScaleAdaptationParameter.IntervalFrame);
                ControlTool.GetParamSave(nud_AdjustFinishNum, ref grayScaleAdaptationParameter.AdjustFinishNum);
                ControlTool.GetParamSave(cb_NNotAdjustFinishAlarm, ref grayScaleAdaptationParameter.NNotAdjustFinishAlarm);
                ControlTool.GetParamSave(nud_GrayBodong, ref grayScaleAdaptationParameter.GrayBodong);
                ControlTool.GetParamSave(cb_GrayBodongAlarm, ref grayScaleAdaptationParameter.GrayBodongAlarm);
                ControlTool.GetParamSave(nud_LightSourcePort, ref currentCameraManager.m_GrayScaleAdaptationParameter.LightSourcePort);
                ControlTool.GetParamSave(nud_LightSourceChannel, ref currentCameraManager.m_GrayScaleAdaptationParameter.LightSourceChannel);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
