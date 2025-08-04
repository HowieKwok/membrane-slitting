using LogTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using YYSD.UICore.Control;
using YYSD.UICore;

namespace Frame_Core
{
    public partial class Cam_Form : Form
    {
        CameraEnum[] camEnumNames = null;
        Dictionary<CameraEnum, CameraManager> cameraManager;
        public Cam_Form(Dictionary<CameraEnum, CameraManager> _CameraManager)
        {
            cameraManager = _CameraManager;
            InitializeComponent();
        }

        private void Cam_Form_Load(object sender, EventArgs e)
        {
            try
            {
                camEnumNames = cameraManager.Keys.ToArray();
                ShowdataGridView1();
                CamNameIsRepetition();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

        }

        private void Cam_Form_FormClosing(object sender, FormClosingEventArgs e)
        {
            EventCallback.CamFormClosingCallback();

        }

        private void save_cam_parmeter_Click(object sender, EventArgs e)
        {
            try
            {


                for (int i = 0; i < cameraManager.Count; i++)
                {
                    bool result = true;
                    int value = 0;
                    double double_value = 0;

                    if (dataGridView1.Rows[i].Cells[0].Value.ToString() != cameraManager[camEnumNames[i]].m_CamParam.CameraName)
                    {
                        if (MessageBox.Show("是否修改相机名称，修改后，参数文件需要重新设置！！！", "警告", MessageBoxButtons.YesNo) == DialogResult.No)
                        {
                            return;
                        }
                    }


                    result = int.TryParse(dataGridView1.Rows[i].Cells[5].Value.ToString(), out value);
                    if (!result)
                    {
                        System.Windows.Forms.MessageBox.Show("图像高度输入的数值格式错误，请输入数字");
                        return;
                    }


                    result = int.TryParse(dataGridView1.Rows[i].Cells[6].Value.ToString(), out value);
                    if (!result)
                    {
                        System.Windows.Forms.MessageBox.Show("图像宽度度输入的数值格式错误，请输入数字");
                        return;
                    }


                    result = double.TryParse(dataGridView1.Rows[i].Cells[7].Value.ToString(), out double_value);
                    if (!result)
                    {
                        System.Windows.Forms.MessageBox.Show("横向标定值输入的数值格式错误，请输入数字");
                        return;
                    }


                    result = double.TryParse(dataGridView1.Rows[i].Cells[8].Value.ToString(), out double_value);
                    if (!result)
                    {
                        System.Windows.Forms.MessageBox.Show("纵向标定值输入的数值格式错误，请输入数字");
                        return;
                    }
                    cameraManager[camEnumNames[i]].m_CamParam.backup(cameraManager[camEnumNames[i]].m_CamParam);

                    ControlTool.GetParamSave(dataGridView1, i, 0, ref cameraManager[camEnumNames[i]].m_CamParam.CameraName);
                    ControlTool.GetParamSave(dataGridView1, i, 1, ref cameraManager[camEnumNames[i]].m_CamParam.CameraType);
                    ControlTool.GetParamSave(dataGridView1, i, 2, ref cameraManager[camEnumNames[i]].m_CamParam.CameraSerial);
                    ControlTool.GetParamSave(dataGridView1, i, 3, ref cameraManager[camEnumNames[i]].m_CamParam.CameraIP);
                    ControlTool.GetParamSave(dataGridView1, i, 4, ref cameraManager[camEnumNames[i]].m_CamParam.PCIP);
                    ControlTool.GetParamSave(dataGridView1, i, 5, ref cameraManager[camEnumNames[i]].m_CamParam.ImageHeight);
                    ControlTool.GetParamSave(dataGridView1, i, 6, ref cameraManager[camEnumNames[i]].m_CamParam.ImageWidth);
                    ControlTool.GetParamSave(dataGridView1, i, 7, ref cameraManager[camEnumNames[i]].m_CamParam.CalibrationValue_X);
                    ControlTool.GetParamSave(dataGridView1, i, 8, ref cameraManager[camEnumNames[i]].m_CamParam.CalibrationValue_Y);
                    ControlTool.GetParamSave(dataGridView1, i, 9, ref cameraManager[camEnumNames[i]].m_CamParam.ToMainCamDistance);

                    cameraManager[camEnumNames[i]].m_CamParam.Save();

                    //修改记录设置
                    cameraManager[camEnumNames[i]].m_CamParam.FilePath = Project.Instance.logParam.LogSavePath + "\\ParamsChangeLog\\";             
                    cameraManager[camEnumNames[i]].m_CamParam.saveChangeLog(cameraManager[camEnumNames[i]].m_CamParam, String.Format("相机设置:{0} 中 {1}", Project.Instance.systemInfo.CurrentMaterial.ToString(), $"第{i+1}行"));
                }

                CamNameIsRepetition();


            }
            catch (Exception ex)
            {

                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }

        }

        private void ShowdataGridView1()
        {
            dataGridView1.Rows.Clear();


            for (int i = 0; i < cameraManager.Count; i++)
            {
                dataGridView1.Rows.Insert(i);
                ControlTool.SetParamShow(dataGridView1, i, 0, ref cameraManager[camEnumNames[i]].m_CamParam.CameraName);
                ControlTool.SetParamShow(dataGridView1, i, 1, ref cameraManager[camEnumNames[i]].m_CamParam.CameraType);
                ControlTool.SetParamShow(dataGridView1, i, 2, ref cameraManager[camEnumNames[i]].m_CamParam.CameraSerial);
                ControlTool.SetParamShow(dataGridView1, i, 3, ref cameraManager[camEnumNames[i]].m_CamParam.CameraIP);
                ControlTool.SetParamShow(dataGridView1, i, 4, ref cameraManager[camEnumNames[i]].m_CamParam.PCIP);
                ControlTool.SetParamShow(dataGridView1, i, 5, ref cameraManager[camEnumNames[i]].m_CamParam.ImageHeight);
                ControlTool.SetParamShow(dataGridView1, i, 6, ref cameraManager[camEnumNames[i]].m_CamParam.ImageWidth);
                ControlTool.SetParamShow(dataGridView1, i, 7, ref cameraManager[camEnumNames[i]].m_CamParam.CalibrationValue_X);
                ControlTool.SetParamShow(dataGridView1, i, 8, ref cameraManager[camEnumNames[i]].m_CamParam.CalibrationValue_Y);
                ControlTool.SetParamShow(dataGridView1, i, 9, ref cameraManager[camEnumNames[i]].m_CamParam.ToMainCamDistance);

            }
        }

        private void CamNameIsRepetition()
        {
            try
            {
                //相机名称不重复
                string[] camNames = new string[cameraManager.Count];
                int count = 0;
                foreach (var item in cameraManager)
                {
                    camNames[count] = item.Value.m_CamParam.CameraName;
                    count++;
                }

                camNames = ProcessDuplicateStrings(camNames);

                count = 0;
                foreach (var item in cameraManager)
                {
                    item.Value.m_CamParam.CameraName = camNames[count];
                    item.Value.m_CamParam.Save();
                    count++;
                }

                ShowdataGridView1();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        // 方法：处理重复字符串并返回修改后的新数组  
        private string[] ProcessDuplicateStrings(string[] inputArray)
        {
            if (inputArray == null) return inputArray;
            string[] newArray = new string[inputArray.Length];
            try
            {
                // 使用LINQ检测重复值  
                var duplicates = inputArray.GroupBy(item => item)
                                           .Where(group => group.Count() > 1)
                                           .Select(group => group.Key);
                int count = 0;
                // 遍历数组并修改第二个重复的值  
                foreach (var item in inputArray)
                {
                    newArray[count] = item;
                    if (duplicates.Contains(item))
                    {
                        newArray[count] = item + $"{count}";
                    }
                    count++;
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                for (int i = 0; i < newArray.Length; i++)
                {
                    newArray[i] = $"{i + 1}";
                }
            }
            return newArray;
        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                //if (e.ColumnIndex == -1 || e.RowIndex == -1) return;

                //if (e.ColumnIndex == 11)
                //{
                //    CameraEnum cameraEnum = (CameraEnum)e.RowIndex;
                //    cameraManager[cameraEnum].Camera.OutIO(cameraManager[cameraEnum].m_CamParam.OutputIndex);
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
    }
}
