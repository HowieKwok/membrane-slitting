using LogTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Frame_Core
{
    public partial class Light_Form : Form
    {
        LightManager[] lightManager;
        public Light_Form(LightManager[] _LightManager)
        {
            lightManager = _LightManager;
            InitializeComponent();
        }

        private void Light_Form_Load(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < lightManager.Length; i++)
                {
                    string pathTemp = lightManager[i].LightParam.filePath;
                    lightManager[i].LightParam = (LightPamater)lightManager[i].LightParam.Read();
                    lightManager[i].LightParam.filePath = pathTemp;
                }
           
                ShowdataGridView1();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void Light_Form_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void save_parmeter_Click(object sender, EventArgs e)
        {
            try
            {


                for (int i = 0; i < lightManager.Length; i++)
                {


                    bool result = false;
                    int value = 0;
                    lightManager[i].LightParam.LightName = dataGridView1.Rows[i].Cells[0].Value.ToString();
                    lightManager[i].LightParam.LightType = dataGridView1.Rows[i].Cells[1].EditedFormattedValue.ToString();

                    for (int j = 2; j <= 10; j++)
                    {
                        result = Int32.TryParse(dataGridView1.Rows[i].Cells[j].Value.ToString(), out value);
                        if (!result)
                        {
                            System.Windows.Forms.MessageBox.Show($"{lightManager[i].LightParam.LightName}-设置值错误，请设置为数值");
                            return;
                        }
                    }


                    lightManager[i].LightParam.LightCOM = Convert.ToInt32(dataGridView1.Rows[i].Cells[2].Value.ToString());
                    lightManager[i].LightParam.LightChannel1 = Convert.ToInt32(dataGridView1.Rows[i].Cells[3].Value.ToString());
                    lightManager[i].LightParam.LightChannel2 = Convert.ToInt32(dataGridView1.Rows[i].Cells[4].Value.ToString());
                    lightManager[i].LightParam.LightChannel3 = Convert.ToInt32(dataGridView1.Rows[i].Cells[5].Value.ToString());
                    lightManager[i].LightParam.LightChannel4 = Convert.ToInt32(dataGridView1.Rows[i].Cells[6].Value.ToString());
                    lightManager[i].LightParam.LightChannel5 = Convert.ToInt32(dataGridView1.Rows[i].Cells[7].Value.ToString());
                    lightManager[i].LightParam.LightChannel6 = Convert.ToInt32(dataGridView1.Rows[i].Cells[8].Value.ToString());
                    lightManager[i].LightParam.LightChannel7 = Convert.ToInt32(dataGridView1.Rows[i].Cells[9].Value.ToString());
                    lightManager[i].LightParam.LightChannel8 = Convert.ToInt32(dataGridView1.Rows[i].Cells[10].Value.ToString());
                    lightManager[i].LightParam.LightChannel1valueGroup[0] = Convert.ToInt16(DownLightValue1.Value);
                    lightManager[i].LightParam.LightChannel1valueGroup[1] = Convert.ToInt16(JpValue1.Value);
                    lightManager[i].LightParam.LightChannel1valueGroup[2] = Convert.ToInt16(UpLightValue1.Value);
                    lightManager[i].LightParam.LightChannel2valueGroup[0] = Convert.ToInt16(DownLightValue2.Value);
                    lightManager[i].LightParam.LightChannel2valueGroup[1] = Convert.ToInt16(JpValue2.Value);
                    lightManager[i].LightParam.LightChannel2valueGroup[2] = Convert.ToInt16(UpLightValue2.Value);
                    lightManager[i].LightParam.bIsControlChange = cbx_LightChange.Checked;
                    lightManager[i].LightParam.bIsEnableOverhang = cbx_EnableOverHang.Checked;
                    lightManager[i].LightParam.Save();
                    //lightManager[i].IniLightSystem();
                }
                UIDelegate.LightParamChange();
            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }
        private void ShowdataGridView1()
        {
            dataGridView1.Rows.Clear();
            for (int i = 0; i < lightManager.Length; i++)
            {
                dataGridView1.Rows.Insert(i);
                dataGridView1.Rows[i].Cells[0].Value = lightManager[i].LightParam.LightName;
                dataGridView1.Rows[i].Cells[1].Value = lightManager[i].LightParam.LightType;
                dataGridView1.Rows[i].Cells[2].Value = lightManager[i].LightParam.LightCOM;
                dataGridView1.Rows[i].Cells[3].Value = lightManager[i].LightParam.LightChannel1;
                dataGridView1.Rows[i].Cells[4].Value = lightManager[i].LightParam.LightChannel2;
                dataGridView1.Rows[i].Cells[5].Value = lightManager[i].LightParam.LightChannel3;
                dataGridView1.Rows[i].Cells[6].Value = lightManager[i].LightParam.LightChannel4;
                dataGridView1.Rows[i].Cells[7].Value = lightManager[i].LightParam.LightChannel5;
                dataGridView1.Rows[i].Cells[8].Value = lightManager[i].LightParam.LightChannel6;
                dataGridView1.Rows[i].Cells[9].Value = lightManager[i].LightParam.LightChannel7;
                dataGridView1.Rows[i].Cells[10].Value = lightManager[i].LightParam.LightChannel8;
                DownLightValue1.Value = Convert.ToDecimal(lightManager[i].LightParam.LightChannel1valueGroup[0]);
                JpValue1.Value = Convert.ToDecimal(lightManager[i].LightParam.LightChannel1valueGroup[1]);
                UpLightValue1.Value = Convert.ToDecimal(lightManager[i].LightParam.LightChannel1valueGroup[2]);
                DownLightValue2.Value = Convert.ToDecimal(lightManager[i].LightParam.LightChannel2valueGroup[0]);
                JpValue2.Value = Convert.ToDecimal(lightManager[i].LightParam.LightChannel2valueGroup[1]);
                UpLightValue2.Value = Convert.ToDecimal(lightManager[i].LightParam.LightChannel2valueGroup[2]);
                cbx_LightChange.Checked = lightManager[i].LightParam.bIsControlChange;
                cbx_EnableOverHang.Checked = lightManager[i].LightParam.bIsEnableOverhang;

            }
        }

        bool contrl_LightChange = false;
        private void btn_Light_Send_Click(object sender, EventArgs e)
        {
            if (contrl_LightChange) return;

            try
            {
                contrl_LightChange = true;
                for (int i = 0; i < lightManager.Length; i++)
                {

                    Thread.Sleep(5);
                    lightManager[i].Light.ChangeValue(1, lightManager[i].LightParam.LightChannel1);
                    Thread.Sleep(5);
                    lightManager[i].Light.ChangeValue(2, lightManager[i].LightParam.LightChannel2);
                    Thread.Sleep(5);
                    lightManager[i].Light.ChangeValue(3, lightManager[i].LightParam.LightChannel3);
                    Thread.Sleep(5);
                    lightManager[i].Light.ChangeValue(4, lightManager[i].LightParam.LightChannel4);

                    Thread.Sleep(5);
                    lightManager[i].Light.ChangeValue(5, lightManager[i].LightParam.LightChannel5);
                    Thread.Sleep(5);
                    lightManager[i].Light.ChangeValue(6, lightManager[i].LightParam.LightChannel6);
                    Thread.Sleep(5);
           
                    lightManager[i].Light.ChangeValue(7, lightManager[i].LightParam.LightChannel7);
                    Thread.Sleep(5);
                    lightManager[i].Light.ChangeValue(8, lightManager[i].LightParam.LightChannel8);

                }
                UIDelegate.LightParamChange();
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                MessageBox.Show(ex.ToString());
            }
            contrl_LightChange = false;
        }
    }
}
