using Frame_Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;

namespace Mef_VisionFrame._5工艺标准
{
    public partial class FormSizeFix : Form
    {
        public FormSizeFix()
        {
            InitializeComponent();
            toolClass.AllNumericUpDownControlSubscribeMouseWheelEvent(this);
        }
        public SizeItemInfo size = new SizeItemInfo();
        bool FirstCalib = true;
        public void ShowSet()
        {
            linkLabel_SizeItem.Text = size.Name;
            TextBox_Desp.Text = size.Description;


            nud_CCDMeasureValue.Value = 1;
            nud_ArtificialMeasureValue.Value = 1;

            nud_CalibValue.Value = (decimal)size.StD_RealFixRito;//旧校准系数
            nud_CalibValueNew.Value = (decimal)size.StD_RealFixRito;//新校准系数
            nud_CalibCompensateValue.Value = (decimal)size.StD_RealFixOffset;//校准补偿值
            nud_DownCellDownOffset.Value = (decimal)size.StD_DownCellDownOffset;
            nud_DownCellUpOffset.Value = (decimal)size.StD_DownCellUpOffset;
            nud_UpCellDownOffset.Value = (decimal)size.StD_UpCellDownOffset;
            nud_UpCellUpOffset.Value = (decimal)size.StD_UpCellUpOffset;

            nud_DownCellDownOffsetAA.Value = (decimal)size.StD_DownCellDownOffsetAA;
            nud_DownCellUpOffsetAA.Value = (decimal)size.StD_DownCellUpOffsetAA;
            nud_UpCellDownOffsetAA.Value = (decimal)size.StD_UpCellDownOffsetAA;
            nud_UpCellUpOffsetAA.Value = (decimal)size.StD_UpCellUpOffsetAA;

            nud_DownCellDownOffsetGM.Value = (decimal)size.StD_DownCellDownOffsetGM;
            nud_DownCellUpOffsetGM.Value = (decimal)size.StD_DownCellUpOffsetGM;
            nud_UpCellDownOffsetGM.Value = (decimal)size.StD_UpCellDownOffsetGM;
            nud_UpCellUpOffsetGM.Value = (decimal)size.StD_UpCellUpOffsetGM;

            //顶级模式系数
            CB_EnableTopModel.Checked = size.EnableTopModel;
            nud_Factor.Value = (decimal)size.StD_TopModelRito;
            nud_Offset.Value = (decimal)size.StD_TopModelOffset;
            if (size.EnableTopModel)
            {
                nud_Factor.Enabled = true;
                nud_Offset.Enabled = true;
            }
            else
            {
                nud_Factor.Enabled = false;
                nud_Offset.Enabled = false;
            }

            FirstCalib = true;
            btn_AutoCalib.BackColor = Color.Green;
        }
        public void closeSet()
        {
            size.Description = TextBox_Desp.Text;

            size.StD_RealFixRito = (double)nud_CalibValueNew.Value;//新校准系数
            size.StD_RealFixOffset = (double)nud_CalibCompensateValue.Value;//校准补偿值
            size.StD_DownCellDownOffset = (double)nud_DownCellDownOffset.Value;
            size.StD_DownCellUpOffset = (double)nud_DownCellUpOffset.Value;
            size.StD_UpCellDownOffset = (double)nud_UpCellDownOffset.Value;
            size.StD_UpCellUpOffset = (double)nud_UpCellUpOffset.Value;

            size.StD_DownCellDownOffsetAA = (double)nud_DownCellDownOffsetAA.Value;
            size.StD_DownCellUpOffsetAA = (double)nud_DownCellUpOffsetAA.Value;
            size.StD_UpCellDownOffsetAA = (double)nud_UpCellDownOffsetAA.Value;
            size.StD_UpCellUpOffsetAA = (double)nud_UpCellUpOffsetAA.Value;

            size.StD_DownCellDownOffsetGM = (double)nud_DownCellDownOffsetGM.Value;
            size.StD_DownCellUpOffsetGM = (double)nud_DownCellUpOffsetGM.Value;
            size.StD_UpCellDownOffsetGM = (double)nud_UpCellDownOffsetGM.Value;
            size.StD_UpCellUpOffsetGM = (double)nud_UpCellUpOffsetGM.Value;


            //顶级模式系数
            size.EnableTopModel = CB_EnableTopModel.Checked;
            size.StD_TopModelRito = (double)nud_Factor.Value;
            size.StD_TopModelOffset = (double)nud_Offset.Value;
        }

        private void FormSizeFix_FormClosing(object sender, FormClosingEventArgs e)
        {
            closeSet();
        }

        private void btn_AutoCalib_Click(object sender, EventArgs e)
        {
            try
            {
                if (FirstCalib == false)
                {
                    MessageBox.Show("关闭弹窗继续校准");
                    FirstCalib = true;
                    btn_AutoCalib.BackColor = Color.Green;
                    return;
                }
                if (nud_CCDMeasureValue.Value == 0 || nud_ArtificialMeasureValue.Value == 0)
                {
                    MessageBox.Show("请输入非0值");
                    return;
                }
                //标定值=(实际量测值/软件测量值)*标定值
                nud_CalibValueNew.Value = Math.Round((nud_ArtificialMeasureValue.Value / nud_CCDMeasureValue.Value * (decimal)size.StD_RealFixRito), 6);
                FirstCalib = false;
                btn_AutoCalib.BackColor = Color.Gray;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void btn_FixReset_Click(object sender, EventArgs e)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show("确定要重置校准系数？", "询问", MessageBoxButtons.YesNo);
            if (result == DialogResult.No) return;

            nud_CalibValueNew.Value = 1;
        }
        private void CB_EnableTopModel_CheckedChanged(object sender, EventArgs e)
        {
            size.EnableTopModel = CB_EnableTopModel.Checked;
            if (size.EnableTopModel)
            {
                nud_Factor.Enabled = true;
                nud_Offset.Enabled = true;
            }
            else
            {
                nud_Factor.Enabled = false;
                nud_Offset.Enabled = false;
            }
        }


    }
}
