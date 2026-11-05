using AntdUI;
using Frame_Core;
using Frame_Core.pluginParams;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public partial class Form_SizeStationEdit : Form
    {
        List<SizeCraftStation_DieCuttingCut> Crafts = null;
        SizeCraftStation_DieCuttingCut currentCraft = null;
        public Form_SizeStationEdit(List<SizeCraftStation_DieCuttingCut> _Crafts, int index)
        {
            InitializeComponent();
            Init();
            Crafts = _Crafts;
            if (_Crafts != null)
            {

                select_Cam1.Items.Clear();
                select_Cam1.Items.AddRange(Service_Camera.GetCam_Names());

                select_Cam2.Items.Clear();
                select_Cam2.Items.AddRange(Service_Camera.GetCam_Names());

         
                select_CalcFormula.Items.Clear();
                select_CalcFormula.Items.AddRange(Enum.GetNames(typeof(CalcFormulaEnum)));

                // 提取所有名称，过滤空值
                string[] names = _Crafts.Select(x => x.Name).ToArray();
                select_detectionItem.Items.Clear();
                select_detectionItem.Items.AddRange(names);
                select_detectionItem.SelectedIndex = index;

            }
        }

        private void Form_SizeStationEdit_Load(object sender, EventArgs e)
        {

        }
        private void Init()
        {

        }

        private void ParamToView(SizeCraftStation_DieCuttingCut craft)
        {
            if (craft == null) return;
            CalibrateCoeff.Value = (decimal)craft.CalibrateCoeff;
            MeasureOffset.Value = (decimal)craft.MeasureOffset;
            EnableDataModel.Checked = craft.EnableDataModel;
            CompensateF.Value = (decimal)craft.CompensateF;
            CompensateO.Value = (decimal)craft.CompensateO;
            select_CalcFormula.SelectedIndex = craft.CalcFormula;
            select_Cam1.SelectedIndex = craft.CameraId1;
            select_Cam2.SelectedIndex = craft.CameraId2;
            input_DataSource1.Text = craft.DataSource1;
            input_DataSource2.Text = craft.DataSource2;

        }

        private void ViewToParam(SizeCraftStation_DieCuttingCut craft)
        {
            if (craft == null) return;
            currentCraft.CalibrateCoeff = (double)CalibrateCoeff.Value;
            currentCraft.MeasureOffset = (double)MeasureOffset.Value;
            currentCraft.EnableDataModel = EnableDataModel.Checked;
            currentCraft.CompensateF = (double)CompensateF.Value;
            currentCraft.CompensateO = (double)CompensateO.Value;
            craft.CalcFormula = select_CalcFormula.SelectedIndex;
            craft.CameraId1 = select_Cam1.SelectedIndex;
            craft.CameraId2 = select_Cam2.SelectedIndex;
            craft.DataSource1 = input_DataSource1.Text;
            craft.DataSource2 = input_DataSource2.Text;
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


        #region 数据源订阅
        private List<string> GetPluginParamResultName(int camID)
        {
            List<string> strings = new List<string>();
            try
            {
                List<Process> process = Project.Instance.Process流程插件管理者.GetCamProcess(camID);
                if (process == null || process.Count == 0) return strings;


                foreach (var item1 in process)
                {
                    foreach (var item in item1.m_ModuleObjList)
                    {
                        if (item.PluginParams is ParamsMeasureLine)
                        {
                            strings.Add(item.moduleInfo.ModuleName);
                        }
                        else if (item.PluginParams is ParamsReusableMeasureDetect)//凸点测量检测
                        {
                            strings.Add(item.moduleInfo.ModuleName);
                        }
                        else if (item.PluginParams is ParamsFindIntersection)//两线交点工具
                        {
                            strings.Add(item.moduleInfo.ModuleName + ".抓边1");
                            strings.Add(item.moduleInfo.ModuleName + ".抓边2");
                            strings.Add(item.moduleInfo.ModuleName + ".交点");
                        }

                        else if (item.PluginParams is ParamsCombPoint)//组合交点工具
                        {
                            strings.Add(item.moduleInfo.ModuleName + ".交点");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


            return strings;
        }


        private void input_DataSource1_Click(object sender, EventArgs e)
        {
            divider4.Text = label5.Text;
            dataSourceSelectEnum = DataSourceSelectEnum.DataSource1;
            var names = GetPluginParamResultName(select_Cam1.SelectedIndex);

            tree1.Items.Clear();
            for (int i = 0; i < names.Count; i++)
            {
                AntdUI.TreeItem treeItem = new AntdUI.TreeItem();
                treeItem.Text = names[i];
                tree1.Items.Add(treeItem);
            }
        }
        DataSourceSelectEnum dataSourceSelectEnum = DataSourceSelectEnum.None;
        private void input_DataSource2_Click(object sender, EventArgs e)
        {
            divider4.Text = label6.Text;
            dataSourceSelectEnum = DataSourceSelectEnum.DataSource2;
            var names = GetPluginParamResultName(select_Cam2.SelectedIndex);

            tree1.Items.Clear();
            for (int i = 0; i < names.Count; i++)
            {
                AntdUI.TreeItem treeItem = new AntdUI.TreeItem();
                treeItem.Text = names[i];
                tree1.Items.Add(treeItem);
            }


        }

        private void btn_SubscriptionData_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectText)) return;

            if (dataSourceSelectEnum == DataSourceSelectEnum.DataSource1)
            {
                input_DataSource1.Text = selectText;
            }
            else if (dataSourceSelectEnum == DataSourceSelectEnum.DataSource2)
            {
                input_DataSource2.Text = selectText;
            }
        }

        string selectText = "";
        private void tree1_SelectChanged(object sender, AntdUI.TreeSelectEventArgs e)
        {
            List<string> strings = GetTreeItemSelectText(e.Item);

            if (strings.Count > 0)
            {
                strings.Reverse();
                selectText = string.Join(".", strings);
            }
        }

        public static List<string> GetTreeItemSelectText(TreeItem treeItem)
        {
            if (treeItem == null) return new List<string>();
            List<string> strings = new List<string>();
            strings.Add(treeItem.Text);

            TreeItem parentItem = treeItem.PARENTITEM;
            strings.AddRange(GetTreeItemSelectText(parentItem));

            return strings;
        }

        #endregion

        private void select_Cam1_SelectedIndexChanged(object sender, IntEventArgs e)
        {
            input_DataSource1_Click(null, null);
        }

        private void select_Cam2_SelectedIndexChanged(object sender, IntEventArgs e)
        {
            input_DataSource2_Click(null, null);
        }
    }

    public enum DataSourceSelectEnum
    {
        None,
        DataSource1,
        DataSource2
    }

}
