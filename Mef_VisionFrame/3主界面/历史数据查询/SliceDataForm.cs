using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Mef_VisionFrame
{
    /// <summary>
    /// 片数据显示窗体
    /// </summary>
    public partial class SliceDataForm : Form
    {
        ProdDataRecord prodDataRecord;

        /// <summary>
        /// 小图显示
        /// </summary>
        MinImageShowForm minImageShowForm = new MinImageShowForm();   

        CamImageDataForm camImageDataForm1;

        CamImageDataForm camImageDataForm2;

        CamImageDataForm camImageDataForm3;

        CamImageDataForm camImageDataForm4;

        public SliceDataForm(ProdDataRecord _prodDataRecord)
        {
            InitializeComponent();
            prodDataRecord = _prodDataRecord;

            camImageDataForm1 = new CamImageDataForm(minImageShowForm) { FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill, TopLevel = false };
            camImageDataForm2 = new CamImageDataForm(minImageShowForm) { FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill, TopLevel = false };
            camImageDataForm3 = new CamImageDataForm(minImageShowForm) { FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill, TopLevel = false };
            camImageDataForm4 = new CamImageDataForm(minImageShowForm) { FormBorderStyle = FormBorderStyle.None, Dock = DockStyle.Fill, TopLevel = false };
        }

        private void SliceDataForm_Load(object sender, EventArgs e)
        {
            this.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(this, true);
            lb_SliceSelect.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(lb_SliceSelect, true);
            tableLayoutPanel1.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(tableLayoutPanel1, true);


            panel_Cam1.Controls.Clear();
            panel_Cam1.Controls.Add(camImageDataForm1);
            camImageDataForm1.Show();

            panel_Cam2.Controls.Clear();
            panel_Cam2.Controls.Add(camImageDataForm2);
            camImageDataForm2.Show();

            panel_Cam3.Controls.Clear();
            panel_Cam3.Controls.Add(camImageDataForm3);
            camImageDataForm3.Show();

            panel_Cam4.Controls.Clear();
            panel_Cam4.Controls.Add(camImageDataForm4);
            camImageDataForm4.Show();

            lb_SliceSelect.Items.Clear();
        }

        private void SliceDataForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            camImageDataForm1.Close();
            camImageDataForm2.Close();
            camImageDataForm3.Close();
            camImageDataForm4.Close();
            minImageShowForm.Close();
        }

        /// <summary>
        /// 添加片到ListBox
        /// </summary>
        /// <param name="sliceIndex"></param>
        public void AddSlice(string sliceIndex,bool result)
        {
            try
            {
                lb_SliceSelect.BeginInvoke(new Action(() =>
                {
                    lb_SliceSelect.Items.Add(sliceIndex);
                   
            

                    //只显示100行数据，并使可见到最后一行
                    if (lb_SliceSelect.Items.Count > 100)
                    {
                        lb_SliceSelect.Items.RemoveAt(0);
                    }

                    int iEnd = lb_SliceSelect.Items.Count - 1;
                    if (iEnd >= 0)
                        lb_SliceSelect.SelectedIndex = iEnd;

                }));            

            }
            catch (Exception)
            {

            }

        }
        object SelectSliceShowLock= new object();   
        /// <summary>
        /// 选择片数据显示
        /// </summary>
        /// <param name="prodDataRecord"></param>
        /// <param name="sliceIndex"></param>
        private void SelectSliceShow(ProdDataRecord _prodDataRecord, string sliceIndex)
        {
            try
            {
                lock (SelectSliceShowLock)
                {
                    if (_prodDataRecord == null) return;

                    camImageDataForm1.LoadImageShow(_prodDataRecord.Cam1_ImageSliceDatas.Find(p => p.SliceIndex == sliceIndex));

                    camImageDataForm2.LoadImageShow(_prodDataRecord.Cam2_ImageSliceDatas.Find(p => p.SliceIndex == sliceIndex));

                    camImageDataForm3.LoadImageShow(_prodDataRecord.Cam3_ImageSliceDatas.Find(p => p.SliceIndex == sliceIndex));

                    camImageDataForm4.LoadImageShow(_prodDataRecord.Cam4_ImageSliceDatas.Find(p => p.SliceIndex == sliceIndex));
                }
             
            }
            catch (Exception)
            {


            }
        }

        /// <summary>
        /// 双击切换片数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lb_SliceSelect_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                int index = lb_SliceSelect.SelectedIndex;
                if (index == -1) return;

                string sliceIndex = lb_SliceSelect.SelectedItem.ToString();
                SelectSliceShow(prodDataRecord, sliceIndex);

            }
            catch (Exception)
            {

            }
        }
    }
}
