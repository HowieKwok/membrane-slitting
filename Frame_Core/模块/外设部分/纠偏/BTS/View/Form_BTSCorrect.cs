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
    public partial class Form_BTSCorrect : Form
    {
        CorrectBTS_Control correctBTS_Control;

        public Form_BTSCorrect(CorrectBTS_Control _correctBTS_Control)
        {
            correctBTS_Control = _correctBTS_Control;
            InitializeComponent();
        }

        private void Form_BTSCorrect_Load(object sender, EventArgs e)
        {
            try
            {
                cbx_JPType.Items.Clear();
                cbx_JPType.Items.Add("UDP");

                cbx_CorrectLogic.Items.Clear();
                cbx_CorrectLogic.AddItems(Enum.GetNames(typeof(CorrectLogicEnum)), Enum.GetNames(typeof(CorrectLogicEnum)));

                ControlTool.SetParamShow(cbx_JPType, ref correctBTS_Control.correctParam.JPConnect_Type);
                ControlTool.SetParamShow(tbxELMachineIP, ref correctBTS_Control.correctParam.strELMachineIP);
                ControlTool.SetParamShow(tbxELMachinePort, ref correctBTS_Control.correctParam.strELMachinePort);
                ControlTool.SetParamShow(tbxLocalMachineIP, ref correctBTS_Control.correctParam.strLocalMachineIP);
                ControlTool.SetParamShow(tbxLocalMachinePort, ref correctBTS_Control.correctParam.strLocalMachinePort);

                ControlTool.SetParamShow(cbx_ENAutoJP, ref correctBTS_Control.correctParam.ENAutoJP);
                ControlTool.SetParamShow(cbx_JP_Sign, ref correctBTS_Control.correctParam.JP_Sign);
                ControlTool.SetParamShow(num_TJDistance, ref correctBTS_Control.correctParam.Correct_distance);
                ControlTool.SetParamShow(num_TJJP_Pro, ref correctBTS_Control.correctParam.JP_Pro);
                ControlTool.SetParamShow(cbx_CorrectLogic, ref correctBTS_Control.correctParam.CorrectLogic);

                ControlTool.SetParamShow(num_maxTJJPnum, ref correctBTS_Control.correctParam.JP_MaxValue);
                ControlTool.SetParamShow(num_minTJJPnum, ref correctBTS_Control.correctParam.JP_MinValue);
                ControlTool.SetParamShow(num_maxBJTJNum, ref correctBTS_Control.correctParam.JP_Max_errNUM);
                ControlTool.SetParamShow(num_TJJPSign_LX_num, ref correctBTS_Control.correctParam.Sign_LX_Num);
                ControlTool.SetParamShow(cbx_maxBJTJEn, ref correctBTS_Control.correctParam.JP_Max_errEn);
                ControlTool.SetParamShow(cbx_TJJPSign_LX_En, ref correctBTS_Control.correctParam.Sign_LY_En);

                ControlTool.SetParamShow(num_Correctdata, ref correctBTS_Control.correctParam.Manu_JPValue);

                ControlTool.SetParamShow(num_JP_HJLength, ref correctBTS_Control.correctParam.JP_HJLength);
                ControlTool.SetParamShow(cb_EnHJJP, ref correctBTS_Control.correctParam.EnHJJP);


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void Form_BTSCorrect_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                ControlTool.GetParamSave(cbx_JPType, ref correctBTS_Control.correctParam.JPConnect_Type);
                ControlTool.GetParamSave(tbxELMachineIP, ref correctBTS_Control.correctParam.strELMachineIP);
                ControlTool.GetParamSave(tbxELMachinePort, ref correctBTS_Control.correctParam.strELMachinePort);
                ControlTool.GetParamSave(tbxLocalMachineIP, ref correctBTS_Control.correctParam.strLocalMachineIP);
                ControlTool.GetParamSave(tbxLocalMachinePort, ref correctBTS_Control.correctParam.strLocalMachinePort);

                ControlTool.GetParamSave(cbx_ENAutoJP, ref correctBTS_Control.correctParam.ENAutoJP);
                ControlTool.GetParamSave(cbx_JP_Sign, ref correctBTS_Control.correctParam.JP_Sign);
                ControlTool.GetParamSave(num_TJDistance, ref correctBTS_Control.correctParam.Correct_distance);
                ControlTool.GetParamSave(num_TJJP_Pro, ref correctBTS_Control.correctParam.JP_Pro);
                ControlTool.GetParamSave(cbx_CorrectLogic, ref correctBTS_Control.correctParam.CorrectLogic);

                ControlTool.GetParamSave(num_maxTJJPnum, ref correctBTS_Control.correctParam.JP_MaxValue);
                ControlTool.GetParamSave(num_minTJJPnum, ref correctBTS_Control.correctParam.JP_MinValue);
                ControlTool.GetParamSave(num_maxBJTJNum, ref correctBTS_Control.correctParam.JP_Max_errNUM);
                ControlTool.GetParamSave(num_TJJPSign_LX_num, ref correctBTS_Control.correctParam.Sign_LX_Num);
                ControlTool.GetParamSave(cbx_maxBJTJEn, ref correctBTS_Control.correctParam.JP_Max_errEn);
                ControlTool.GetParamSave(cbx_TJJPSign_LX_En, ref correctBTS_Control.correctParam.Sign_LY_En);

                ControlTool.GetParamSave(num_Correctdata, ref correctBTS_Control.correctParam.Manu_JPValue);

                ControlTool.GetParamSave(num_JP_HJLength, ref correctBTS_Control.correctParam.JP_HJLength);
                ControlTool.GetParamSave(cb_EnHJJP, ref correctBTS_Control.correctParam.EnHJJP);

                correctBTS_Control.correctParam.Save();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }



        private void btnELConnect_Click(object sender, EventArgs e)
        {
            correctBTS_Control?.Init();
        }

        private void btnManuJP_Click(object sender, EventArgs e)
        {
            if (correctBTS_Control == null) return;
           
            correctBTS_Control.En_ManualOffset.value = (double)num_Correctdata.Value;
            correctBTS_Control.En_ManualOffset.execute = true;
        }
  
    }
}
