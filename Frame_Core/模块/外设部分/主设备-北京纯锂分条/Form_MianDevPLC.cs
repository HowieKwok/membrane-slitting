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
    public partial class Form_MianDevPLC : Form
    {
        MDevPLCManage_Slice1to2 bIZ_Param_Slice1To2;
        public Form_MianDevPLC(MDevPLCManage_Slice1to2 _bIZ_Param_Slice1To2)
        {
            InitializeComponent();
            bIZ_Param_Slice1To2 = _bIZ_Param_Slice1To2;
        }

        private void Form_MianDevPLC_Load(object sender, EventArgs e)
        {
            LoadParam();
        }

        public void LoadParam()
        {
            try
            {
                cbx_ComType.Items.Clear();
                cbx_ComType.Items.Add("Fins");
                cbx_CorrectLogic.Items.Clear();
                cbx_CorrectLogic.AddItems(Enum.GetNames(typeof(CorrectLogicEnum)), Enum.GetNames(typeof(CorrectLogicEnum)));
              

                ControlTool.SetParamShow(cbx_ComType, ref bIZ_Param_Slice1To2.m_DevParam.ComType);
                ControlTool.SetParamShow(cbx_IsConnect, ref bIZ_Param_Slice1To2.m_DevParam.IsConnect);
                ControlTool.SetParamShow(tbx_Local_IP, ref bIZ_Param_Slice1To2.m_DevParam.LocalPC_IP);
                ControlTool.SetParamShow(tbx_Dev_IP, ref bIZ_Param_Slice1To2.m_DevParam.PLC_IP);
                ControlTool.SetParamShow(num_DevPort, ref bIZ_Param_Slice1To2.m_DevParam.DevPort);
                ControlTool.SetParamShow(nud_EncoderDiameter, ref bIZ_Param_Slice1To2.m_DevParam.EncoderDiameter);
                ControlTool.SetParamShow(nud_EncoderPulse, ref bIZ_Param_Slice1To2.m_DevParam.EncoderPulse);
                ControlTool.SetParamShow(nud_CorrentSleep, ref bIZ_Param_Slice1To2.m_DevParam.CorrentSleep);


                ControlTool.SetParamShow(cbx_ENAutoJP, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.ENAutoJP);
                ControlTool.SetParamShow(cbx_JP_Sign, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.JP_Sign);
                ControlTool.SetParamShow(num_TJDistance, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.Correct_distance);
                ControlTool.SetParamShow(num_TJJP_Pro, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.JP_Pro);
                ControlTool.SetParamShow(cbx_CorrectLogic, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.CorrectLogic);

                ControlTool.SetParamShow(num_maxTJJPnum, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.JP_MaxValue);
                ControlTool.SetParamShow(num_minTJJPnum, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.JP_MinValue);
                ControlTool.SetParamShow(num_maxBJTJNum, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.JP_Max_errNUM);
                ControlTool.SetParamShow(num_TJJPSign_LX_num, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.Sign_LX_Num);
                ControlTool.SetParamShow(cbx_maxBJTJEn, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.JP_Max_errEn);
                ControlTool.SetParamShow(cbx_TJJPSign_LX_En, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.Sign_LY_En);

                ControlTool.SetParamShow(num_Correctdata, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.Manu_JPValue);

                ControlTool.SetParamShow(num_JP_HJLength, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.JP_HJLength);
                ControlTool.SetParamShow(cb_EnHJJP, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.EnHJJP);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        public void SaveParam()
        {
            try
            {
                ControlTool.GetParamSave(cbx_ComType, ref bIZ_Param_Slice1To2.m_DevParam.ComType);
                ControlTool.GetParamSave(cbx_IsConnect, ref bIZ_Param_Slice1To2.m_DevParam.IsConnect);
                ControlTool.GetParamSave(tbx_Local_IP, ref bIZ_Param_Slice1To2.m_DevParam.LocalPC_IP);
                ControlTool.GetParamSave(tbx_Dev_IP, ref bIZ_Param_Slice1To2.m_DevParam.PLC_IP);
                ControlTool.GetParamSave(num_DevPort, ref bIZ_Param_Slice1To2.m_DevParam.DevPort);
                ControlTool.GetParamSave(nud_EncoderDiameter, ref bIZ_Param_Slice1To2.m_DevParam.EncoderDiameter);
                ControlTool.GetParamSave(nud_EncoderPulse, ref bIZ_Param_Slice1To2.m_DevParam.EncoderPulse);
                ControlTool.GetParamSave(nud_CorrentSleep, ref bIZ_Param_Slice1To2.m_DevParam.CorrentSleep);


                ControlTool.GetParamSave(cbx_ENAutoJP, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.ENAutoJP);
                ControlTool.GetParamSave(cbx_JP_Sign, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.JP_Sign);
                ControlTool.GetParamSave(num_TJDistance, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.Correct_distance);
                ControlTool.GetParamSave(num_TJJP_Pro, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.JP_Pro);
                ControlTool.GetParamSave(cbx_CorrectLogic, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.CorrectLogic);

                ControlTool.GetParamSave(num_maxTJJPnum, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.JP_MaxValue);
                ControlTool.GetParamSave(num_minTJJPnum, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.JP_MinValue);
                ControlTool.GetParamSave(num_maxBJTJNum, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.JP_Max_errNUM);
                ControlTool.GetParamSave(num_TJJPSign_LX_num, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.Sign_LX_Num);
                ControlTool.GetParamSave(cbx_maxBJTJEn, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.JP_Max_errEn);
                ControlTool.GetParamSave(cbx_TJJPSign_LX_En, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.Sign_LY_En);

                ControlTool.GetParamSave(num_Correctdata, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.Manu_JPValue);

                ControlTool.GetParamSave(num_JP_HJLength, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.JP_HJLength);
                ControlTool.GetParamSave(cb_EnHJJP, ref bIZ_Param_Slice1To2.m_DevParam.correctParam.EnHJJP);

                bIZ_Param_Slice1To2.m_DevParam.Save();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        bool codingWheelCylinderFinish = false;
        bool codingWheelCylinder = false;
        private void btn_Encoder_Click(object sender, EventArgs e)
        {
            if (codingWheelCylinderFinish) return;
           
            codingWheelCylinderFinish = true;
            bIZ_Param_Slice1To2.Write_CodingWheelCylinder(codingWheelCylinder);
            codingWheelCylinder = !codingWheelCylinder;

            codingWheelCylinderFinish = false;
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            SaveParam();
            WritePLCParam();
        }

        private void btn_connectPLC_Click(object sender, EventArgs e)
        {
            bIZ_Param_Slice1To2.Connect();
        }

        private void btnManuJP_Click(object sender, EventArgs e)
        {
            try
            {
            
                bIZ_Param_Slice1To2.Write_CorrectManualValue((float)num_Correctdata.Value);
            }
            catch (Exception)
            {

            }
         
        }

        private void btnHZJP_Click(object sender, EventArgs e)
        {
            bIZ_Param_Slice1To2.Write_CorrectToHome();
        }

        public void WritePLCParam()
        {
            try
            {
                bIZ_Param_Slice1To2.Write_EncoderDiameter((short)nud_EncoderDiameter.Value);
                bIZ_Param_Slice1To2.Write_EncoderPulse((short)nud_EncoderPulse.Value);
                bIZ_Param_Slice1To2.Write_CorrectEn(cbx_ENAutoJP.Checked);
                bIZ_Param_Slice1To2.Write_CorrectSleep((short)nud_CorrentSleep.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
          
        }

        private void Form_MianDevPLC_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }
    }
}
