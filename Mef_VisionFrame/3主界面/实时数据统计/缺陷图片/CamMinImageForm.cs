using Frame_Core;
using HalconDotNet;
using NPOI.OpenXmlFormats.Wordprocessing;
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
    public partial class CamMinImageForm : Form
    {
        MinImageShowForm minImageShowForm;
        List<HWindow> HWindowList = new List<HWindow>();

        public CamMinImageForm(MinImageShowForm _minImageShowForm)
        {
            InitializeComponent();
            minImageShowForm = _minImageShowForm;
        }

        private void CamMinImageForm_Load(object sender, EventArgs e)
        {
            HWindowList.Clear();
            HWindowList.Add(hWindowControl_DefectImage1.HalconWindow);
            HWindowList.Add(hWindowControl_DefectImage2.HalconWindow);
            HWindowList.Add(hWindowControl_DefectImage3.HalconWindow);
        }

        private void CamMinImageForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        /// <summary>
        /// 刷新窗体文本
        /// </summary>
        public void UpdateFormText(string camName)
        {
            gb_CamName.Invoke(new Action(() =>
            {
                gb_CamName.Text = camName;
            }));
        }


        int DefectImage1MouseDownCount = 0;
        int DefectImage2MouseDownCount = 0;
        int DefectImage3MouseDownCount = 0;
        HObject defectImage1 = new HObject();
        HObject defectImage2 = new HObject();
        HObject defectImage3 = new HObject();
        int listIndex = 1;

        /// <summary>
        /// 添加缺陷图显示
        /// </summary>
        /// <param name="defectImage"></param>
        public void AddDefectImage(NgTypeInfo ngTypeInfo)
        {
            try
            {
                if (ngTypeInfo == null) return;
                if (ngTypeInfo.m_hImage == null) return;
                if (!ngTypeInfo.m_hImage.IsInitialized()) return;

                HObject hImage;
                if (listIndex == 0)
                {
                    defectImage1.Dispose();
                    HOperatorSet.CopyObj(ngTypeInfo.m_hImage, out defectImage1, 1, 1);
                    hImage = defectImage1;
                }
                else if (listIndex == 1)
                {
                    defectImage2.Dispose();
                    HOperatorSet.CopyObj(ngTypeInfo.m_hImage, out defectImage2, 1, 1);
                    hImage = defectImage2;
                }
                else
                {
                    defectImage3.Dispose();
                    HOperatorSet.CopyObj(ngTypeInfo.m_hImage, out defectImage3, 1, 1);
                    hImage = defectImage3;
                }


                HTuple hv_Width = 0, hv_Height = 0;
                HOperatorSet.GetImageSize(hImage, out hv_Width, out hv_Height);
                HOperatorSet.ClearWindow(HWindowList[listIndex]);
                HOperatorSet.SetPart(HWindowList[listIndex], 0, 0, hv_Height - 1, hv_Width - 1);
                HOperatorSet.DispObj(hImage, HWindowList[listIndex]);
                HOperatorSet.SetColor(HWindowList[listIndex], "red");

                List<string> ngInfo = new List<string>();
                ngInfo.Add(ngTypeInfo.m_nImgIndex.ToString());
                ngInfo.Add(ngTypeInfo.m_strNgTypeName.ToString());
                ngInfo.Add("Area:" + ngTypeInfo.m_fNgTypArea.ToString("F0"));
                ngInfo.Add("Height:" + ngTypeInfo.m_fNgTypHeight.ToString("F0"));
                ngInfo.Add("Width:" + ngTypeInfo.m_fNgTypWidth.ToString("F0"));

                int FontLine = 20;
                for (int i = 0; i < ngInfo.Count; i++)
                {
                    HOperatorSet.SetColor(HWindowList[listIndex], HalconColor.red.ToString());
                    HOperatorSet.DispText(HWindowList[listIndex], ngInfo[i], "window", FontLine, 0, HalconColor.red.ToString(), "box", "false");
                    FontLine += 20;
                }


                listIndex++;
                if (listIndex >= HWindowList.Count) listIndex = 0;
            }
            catch (Exception)
            {

            }

        }

        /// <summary>
        /// 小图显示
        /// </summary>
        /// <param name="_images"></param>
        private void MinImageShow(HObject image)
        {
            try
            {
                //if (minImageShowForm == null) { return; }
                //if (image == null) { return; }

                //minImageShowForm.Close();
                //if (minImageShowForm.IsDisposed == true)
                //{
                //    minImageShowForm = new MinImageShowForm();
                //}
                //minImageShowForm.StartPosition = FormStartPosition.CenterScreen;
                //minImageShowForm.Show();

                //HObject Img = image;
                //if (Img != null)
                //    minImageShowForm.ImageShow(Img, "");
            }
            catch (Exception)
            {


            }
        }

        private void hWindowControl_DefectImage1_HMouseDown(object sender, HMouseEventArgs e)
        {
            try
            {
                DefectImage1MouseDownCount++;
                if (DefectImage1MouseDownCount >= 2)
                {
                    MinImageShow(defectImage1);

                }
            }
            catch (Exception)
            {

            }
        }

        private void hWindowControl_DefectImage2_HMouseDown(object sender, HMouseEventArgs e)
        {
            try
            {
                DefectImage2MouseDownCount++;
                if (DefectImage2MouseDownCount >= 2)
                {
                    MinImageShow(defectImage2);

                }
            }
            catch (Exception)
            {

            }
        }

        private void hWindowControl_DefectImage3_HMouseDown(object sender, HMouseEventArgs e)
        {
            try
            {
                DefectImage3MouseDownCount++;
                if (DefectImage3MouseDownCount >= 2)
                {
                    MinImageShow(defectImage3);

                }
            }
            catch (Exception)
            {

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DefectImage1MouseDownCount = 0;
            DefectImage2MouseDownCount = 0;
            DefectImage3MouseDownCount = 0;
        }
    }
}
