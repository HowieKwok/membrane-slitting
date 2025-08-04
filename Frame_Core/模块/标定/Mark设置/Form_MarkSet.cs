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
    public partial class Form_MarkSet : Form
    {
        Dictionary<CameraEnum, CameraManager> cameraManager;

        Param_MarkSet param_MarkSet;

        bool setMarkBaseFlag = false;

        public Form_MarkSet(Dictionary<CameraEnum, CameraManager> _cameraManager)
        {
            InitializeComponent();
            cameraManager = _cameraManager;


        }

        private void Form_MarkSet_Load(object sender, EventArgs e)
        {
            try
            {
                cbx_CamSelect.Items.Clear();
                foreach (var item in cameraManager)
                {
                    cbx_CamSelect.Items.Add(item.Value.m_CamParam.CameraName.ToString());
                }
                cbx_CamSelect.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult result = System.Windows.Forms.MessageBox.Show($"设置当前检测到的Mark点为基准", "询问", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    return;
                }

                setMarkBaseFlag = true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }

        }


        private void cbx_CamSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                param_MarkSet = cameraManager[(CameraEnum)cbx_CamSelect.SelectedIndex].param_MarkSet;

                LoadParam();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }


        public void LoadParam()
        {
            try
            {
                ControlTool.SetParamShow(Nud_MarkX, ref param_MarkSet.BasePointX);
                ControlTool.SetParamShow(Nud_MarkY, ref param_MarkSet.BasePointY);
                ControlTool.SetParamShow(Nud_MarkR, ref param_MarkSet.BasePointR);
                ControlTool.SetParamShow(nud_MarkPosRange, ref param_MarkSet.MarkPosRange);
                ControlTool.SetParamShow(cbx_MarkPosRangeBJTJ, ref param_MarkSet.MarkPosRangeBJTJ);
                ControlTool.SetParamShow(nud_UnidentificationMarkNum, ref param_MarkSet.UnidentificationMarkNum);
                ControlTool.SetParamShow(cbx_UnidentificationMarkBJTJ, ref param_MarkSet.UnidentificationMarkBJTJ);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }

        public void SaveParam()
        {
            try
            {
                ControlTool.GetParamSave(Nud_MarkX, ref param_MarkSet.BasePointX);
                ControlTool.GetParamSave(Nud_MarkY, ref param_MarkSet.BasePointY);
                ControlTool.GetParamSave(Nud_MarkR, ref param_MarkSet.BasePointR);
                ControlTool.GetParamSave(nud_MarkPosRange, ref param_MarkSet.MarkPosRange);
                ControlTool.GetParamSave(cbx_MarkPosRangeBJTJ, ref param_MarkSet.MarkPosRangeBJTJ);
                ControlTool.GetParamSave(nud_UnidentificationMarkNum, ref param_MarkSet.UnidentificationMarkNum);
                ControlTool.GetParamSave(cbx_UnidentificationMarkBJTJ, ref param_MarkSet.UnidentificationMarkBJTJ);
                param_MarkSet.Save();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }
        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {

                //设置所有相机的基准点
                foreach (var item in cameraManager)
                {
                    if (setMarkBaseFlag)
                    {
                        item.Value.param_MarkSet.BasePointX = item.Value.param_MarkSet.m_DetectionBaseCenterX;
                        item.Value.param_MarkSet.BasePointY = item.Value.param_MarkSet.m_DetectionBaseCenterY;
                        item.Value.param_MarkSet.BasePointR = item.Value.param_MarkSet.m_DetectionBaseCenterR;
                        item.Value.param_MarkSet.Save();
                    }

                    //设置所有相机的参数
                    if (cbx_AllCamOneClickSetup.Checked)
                    {
                        ControlTool.GetParamSave(nud_MarkPosRange, ref item.Value.param_MarkSet.MarkPosRange);
                        ControlTool.GetParamSave(cbx_MarkPosRangeBJTJ, ref item.Value.param_MarkSet.MarkPosRangeBJTJ);
                        ControlTool.GetParamSave(nud_UnidentificationMarkNum, ref item.Value.param_MarkSet.UnidentificationMarkNum);
                        ControlTool.GetParamSave(cbx_UnidentificationMarkBJTJ, ref item.Value.param_MarkSet.UnidentificationMarkBJTJ);                      
                    }
                }
              
                if (setMarkBaseFlag)
                {
                    ControlTool.SetParamShow(Nud_MarkX, ref param_MarkSet.BasePointX);
                    ControlTool.SetParamShow(Nud_MarkY, ref param_MarkSet.BasePointY);
                    ControlTool.SetParamShow(Nud_MarkR, ref param_MarkSet.BasePointR);
                }
          

                //保存当前选择相机数据
                SaveParam();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.Message);
            }
        }
    }
}
