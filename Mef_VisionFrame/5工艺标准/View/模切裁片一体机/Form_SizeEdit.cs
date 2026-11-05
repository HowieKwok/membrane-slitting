using Frame_Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public partial class Form_SizeEdit : Form
    {
        List<SizeCraft_DieCuttingCut> Crafts = null;
        SizeCraft_DieCuttingCut currentCraft = null;
        public Form_SizeEdit(List<SizeCraft_DieCuttingCut> _Crafts, int index)
        {
            InitializeComponent();
            Init();
            Crafts = _Crafts;
            if (_Crafts != null)
            {
                // 提取所有名称，过滤空值
                string[] names = _Crafts.Select(x => x.Name).ToArray();
                select_detectionItem.Items.Clear();
                select_detectionItem.Items.AddRange(names);
                select_detectionItem.SelectedIndex = index;
            }
        }

        private void Init()
        {

        }

        private void ParamToView(SizeCraft_DieCuttingCut craft)
        {
            if (craft == null) return;
            CalibrateCoeff.Value = (decimal)craft.CalibrateCoeff;
            MeasureOffset.Value = (decimal)craft.MeasureOffset;
            EnableDataModel.Checked = craft.EnableDataModel;
            CompensateF.Value = (decimal)craft.CompensateF;
            CompensateO.Value = (decimal)craft.CompensateO;
        }

        private void ViewToParam(SizeCraft_DieCuttingCut craft)
        {
            if (craft == null) return;
            currentCraft.CalibrateCoeff = (double)CalibrateCoeff.Value;
            currentCraft.MeasureOffset = (double)MeasureOffset.Value;
            currentCraft.EnableDataModel = EnableDataModel.Checked;
            currentCraft.CompensateF = (double)CompensateF.Value;
            currentCraft.CompensateO = (double)CompensateO.Value;
        }


        private void select_detectionItem_SelectedIndexChanged(object sender, AntdUI.IntEventArgs e)
        {
            if (Crafts == null) return;
            int index = select_detectionItem.SelectedIndex;
            currentCraft = Crafts[index];
            ParamToView(currentCraft);
        }

        private void btn_FixReset_Click(object sender, EventArgs e)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show("确定要重置校准系数？", "询问", MessageBoxButtons.YesNo);
            if (result == DialogResult.No) return;
            CalibrateCoeff.Value = 1;
        }

        private void btn_AutoCalib_Click(object sender, EventArgs e)
        {
            try
            {
   
                if (CCDMeasureValue.Value == 0 || ArtificialMeasureValue.Value == 0)
                {
                    MessageBox.Show("请输入非0值");
                    return;
                }
                //标定值=(实际量测值/软件测量值)*标定值
                CalibrateCoeff.Value = Math.Round((ArtificialMeasureValue.Value / CCDMeasureValue.Value * (decimal)currentCraft.CalibrateCoeff), 6);
              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            ViewToParam(currentCraft);
        }
    }

}
