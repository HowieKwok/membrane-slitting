using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;
using Frame_Core;
using System.Windows.Interop;
using System.Web;

namespace Mef_VisionFrame
{
    public partial class FMain : Form
    {
        internal static ConfigModel config = new ConfigModel();

        private string rootPath = string.Empty;
        private string ngFileName = "NG.csv";

        List<string> camName = new List<string>();
        List<string> camNameS = new List<string>();
        List<int> frames = new List<int>();
        List<string> PathList = new List<string>();
        public FMain()
        {
            InitializeComponent();
            camName.AddRange(Service_Camera.GetCam_Names());
            camNameS.AddRange(Service_Camera.GetCam_Names());
        }





        private void b_path_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(rootPath))
                    fb.SelectedPath = rootPath;
                if (fb.ShowDialog() == DialogResult.OK)
                {
                    rootPath = fb.SelectedPath;
                    t_path.Text = rootPath;

                    string file = Path.Combine(rootPath, ngFileName);
                    if (!File.Exists(file))
                    {
                        MessageBox.Show($"NG记录文件【{file}】不存在，请检查该目录下的NG记录表格是否存在！", "停止", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }

                    l_list.Items.Clear();
                    string line = string.Empty;
                    bool isFirst = true;
                    using (StreamReader sr = new StreamReader(file))
                    {
                        while (sr.Peek() >= 0)
                        {
                            line = sr.ReadLine();
                            if (isFirst)
                            {
                                isFirst = false;
                                string[] hd = line.Split(new char[] { ',' });
                                ListViewItem item = new ListViewItem(new string[] { hd[0], hd[1], hd[2], hd[3], hd[4], hd[5], hd[6], hd[7] });
                            }
                            else
                            {
                                string[] ss = line.Split(new char[] { ',' });
                                ListViewItem item = new ListViewItem(new string[] { ss[0], ss[1], ss[2], ss[3], ss[4], ss[5], ss[6], ss[7] });
                                l_list.Items.Add(item);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开膜卷记录出错：【{ex.Message}】", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void l_list_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (l_list.SelectedItems.Count > 0)
                {
                    PathList.Clear();
                    ListViewItem item = l_list.SelectedItems[0];
                    for (int i = 4; i < 13; i++)
                    {
                        PathList.Add(item.SubItems[i].Text);
                    }
                    //if (!int.TryParse(item.SubItems[5].Text, out int frameA))
                    //{
                    //    MessageBox.Show($"叠台A帧数为非整数，显示图像中止", "停止", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    //    return;
                    //}
                    //if (!int.TryParse(item.SubItems[6].Text, out int frameB))
                    //{
                    //    MessageBox.Show($"叠台B帧数为非整数，显示图像中止", "停止", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    //    return;
                    //}
                    // string shootName = item.SubItems[7].Text;

                    //for (int i = 0; i < 4; i++)
                    //{
                    //    frames.Add(13);
                    //}
                    //for (int i = 0; i < 4; i++)
                    //{
                    //    frames.Add(15);
                    //}
                    FormDataAnalysis fView = new FormDataAnalysis(rootPath, PathList);
                    string msg = fView.LoadHminImage();

                    if (string.IsNullOrEmpty(msg))
                    {
                        MessageBox.Show(msg, "停止", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        fView.Dispose();
                        return;
                    }
                    else
                    {
                        fView.Show();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"展示NG图像出错：【{ex.Message}】", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FMain_Load(object sender, EventArgs e)
        {
            try
            {
                string file = Path.Combine(Application.StartupPath, "Config.json");
                if (File.Exists(file))
                {
                    string json = File.ReadAllText(file);
                    config = JsonConvert.DeserializeObject<ConfigModel>(json);
                    rootPath = t_path.Text = config.RootPath;
                    if (config.MfromStartPos.X > 0 || config.MfromStartPos.Y > 0)
                        this.Location = config.MfromStartPos;
                }
            }
            catch { }
        }

        private void FMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                config.MfromStartPos = this.Location;
                string json = JsonConvert.SerializeObject(config);
                string file = Path.Combine(Application.StartupPath, "Config.json");
                File.WriteAllText(file, json);
            }
            catch { }
        }
    }
}
