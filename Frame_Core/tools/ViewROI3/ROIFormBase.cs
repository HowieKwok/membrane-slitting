using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ViewROI3
{
    public partial class ROIFormBase : Form
    {
       public ROI roi;
       public HWndCtrl hWndCtrl;
        public ROIFormBase(object obj, HWndCtrl hWnd)
        {
            InitializeComponent();
            roi = (ROI)obj;
            hWndCtrl = hWnd;
            MeasureEnable.Checked = roi.roiParams.MeasureEnable;
        }


        public void InitControl(InitControlDele initControldele)
        {
            if (roi.ControlDictionary.Count == 0)
            {
                initControldele?.Invoke(roi);
            }
           
        }


        public ROIFormBase()
        {
            InitializeComponent();
        }
        public virtual void ControlToValue()
        {
           
        }
     
        public void ROIParamObj_FormClosing(object sender, FormClosingEventArgs e)
        {
            //控件值保存到字典
            roi.ControlDictionary = TableLayoutPanelSerializer.SerializeToDictionary(this.tPControlMain);
        }

        private void ROIParamObj_Load(object sender, EventArgs e)
        {
           // TableLayoutPanelSerializer.DeserializeToTable(roi.ControlDictionary, this.tPControlMain);
     
            tPControlMain.BeginInvoke((MethodInvoker)delegate {
                TableLayoutPanelSerializer.DeserializeToTable(roi.ControlDictionary, this.tPControlMain); TableLayoutPanelSerializer.DeserializeToTable(roi.ControlDictionary, this.tPControlMain);
              
            });
            
        }

        private void btn_Domeasure_Click(object sender, EventArgs e)
        {
            //控件值保存到字典
            try
            {
                roi.ControlDictionary = TableLayoutPanelSerializer.SerializeToDictionary(this.tPControlMain);
                hWndCtrl.InitShow();
                tet_MeasureRes.Clear();

                if (roi.OutRoiResult?.Length > 0)
                {
                    for (int i = 0; i < roi.OutRoiResult.Length; i++)
                    {
                        if (roi.OutRoiResult[i]?.Length > 0)
                        {
                            tet_MeasureRes.AppendText(string.Join(",", roi.OutRoiResult[i].DArr.ToList().ConvertAll(x => x.ToString("0.00"))) + "\n");

                        }

                    }
                    //tet_MeasureRes.Text = string.Join(",", roi.OutRoiResult.DArr.ToList());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
           
        }

        private void MeasureEnable_CheckedChanged(object sender, EventArgs e)
        {
            roi.MeasureEnable = MeasureEnable.Checked;
            roi.roiParams.MeasureEnable = MeasureEnable.Checked;
        }

        private void tPControlMain_MouseMove(object sender, MouseEventArgs e)
        {
            //this.tPControlMain.AutoScroll = true;
        }

        private void tPControlMain_MouseLeave(object sender, EventArgs e)
        {
           // this.tPControlMain.AutoScroll = false;
        }

        private void tPControlMain_MouseDown(object sender, MouseEventArgs e)
        {
           // this.tPControlMain.AutoScroll = true;
        }

        private void ROIFormBase_MouseDown(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Middle)
            //{
            //    this.tPControlMain.AutoScroll = true;
            //}
            //if (e.Button == MouseButtons.Right)
            //{
            //    this.tPControlMain.AutoScroll = false;
            //}
        }
    }
}
