using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using LogTool;

namespace Frame_Core
{
    public partial class Form_Thick3DSet : Form
    {

        private Thick3DManage[] thick3DManages;
        public Form_Thick3DSet(Thick3DManage[] thick3DManages)
        {
            InitializeComponent();
            this.thick3DManages = thick3DManages;
            cob_SelectCorrect.Items.Clear();
            for (int i = 0; i < thick3DManages.Count(); i++)
            {
                cob_SelectCorrect.Items.Add($"3D测厚纠偏{i+1}");
            }
            cob_SelectCorrect.SelectedIndex = 0;

        }

        private void Btn_SendData_Click(object sender, EventArgs e)
        {
            try
            {
                if(thick3DManages[cob_SelectCorrect.SelectedIndex].SendData!=null)
                {
                    int mode = (int)num_SelectMode.Value;
                    float num   = (float)num_SendData.Value;
                    if (thick3DManages[cob_SelectCorrect.SelectedIndex].correctParam.JP_Sign)
                    {
                        num=num*(-1);
                    }
      
                    thick3DManages[cob_SelectCorrect.SelectedIndex].SendData?.Invoke(mode,num);
                }
               
            }
            catch (Exception)
            {

                //throw;
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                UpDataCorrectParam(thick3DManages[cob_SelectCorrect.SelectedIndex].correctParam);
                thick3DManages[cob_SelectCorrect.SelectedIndex].correctParam.Save();

            }
            catch (Exception)
            {

                //throw;
            }
        }

        void UpDataUI(Param_Thick3DSet param)
        {
            try
            {

                ControlTool.SetParamShow(cbx_ENAutoJP, ref param.ENAutoJP);
                ControlTool.SetParamShow(cbx_JP_Sign, ref param.JP_Sign);
                ControlTool.SetParamShow(num_TJDistance, ref param.Correct_distance);
                ControlTool.SetParamShow(num_TJJP_Pro, ref param.JP_Pro);

                ControlTool.SetParamShow(num_maxTJJPnum, ref param.JP_MaxValue);
                ControlTool.SetParamShow(num_minTJJPnum, ref param.JP_MinValue);
                ControlTool.SetParamShow(num_maxBJTJNum, ref param.JP_Max_errNUM);
                ControlTool.SetParamShow(num_TJJPSign_LX_num, ref param.Sign_LX_Num);
                ControlTool.SetParamShow(num_stdValue, ref param.JP_StdValue);

                ControlTool.SetParamShow(num_JP_HJLength, ref param.JP_HJLength);
                ControlTool.SetParamShow(cb_EnHJJP, ref param.EnHJJP);

                ControlTool.SetParamShow(cbx_JP_Max_errEn, ref param.JP_Max_errEn);
                ControlTool.SetParamShow(cBx_Sign_LY_En, ref param.Sign_LY_En);


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        void UpDataCorrectParam(Param_Thick3DSet param)
        {
            try
            {
                ControlTool.GetParamSave(cbx_ENAutoJP, ref param.ENAutoJP);
                ControlTool.GetParamSave(cbx_JP_Sign, ref param.JP_Sign);
                ControlTool.GetParamSave(num_TJDistance, ref param.Correct_distance);
                ControlTool.GetParamSave(num_TJJP_Pro, ref param.JP_Pro);


                ControlTool.GetParamSave(num_maxTJJPnum, ref param.JP_MaxValue);
                ControlTool.GetParamSave(num_minTJJPnum, ref param.JP_MinValue);
                ControlTool.GetParamSave(num_maxBJTJNum, ref param.JP_Max_errNUM);
                ControlTool.GetParamSave(num_TJJPSign_LX_num, ref param.Sign_LX_Num);
                ControlTool.GetParamSave(num_stdValue, ref param.JP_StdValue);

                ControlTool.GetParamSave(num_JP_HJLength, ref param.JP_HJLength);
                ControlTool.GetParamSave(cb_EnHJJP, ref param.EnHJJP);

                ControlTool.GetParamSave(cbx_JP_Max_errEn, ref param.JP_Max_errEn);
                ControlTool.GetParamSave(cBx_Sign_LY_En, ref param.Sign_LY_En);

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void cob_SelectCorrect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(thick3DManages.Count() > 0)
            {
                UpDataUI(thick3DManages[cob_SelectCorrect.SelectedIndex].correctParam);
            }
        }
    }
}
