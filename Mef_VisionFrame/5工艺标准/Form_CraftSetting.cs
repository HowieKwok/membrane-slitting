using Frame_Core;
using LiveChartsCore.Drawing;
using LogTool;
using NPOI.SS.Formula.Functions;
using Plugin.MeasureEar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Windows.Media.Media3D;
using YYSD.UICore.Control;
using YYSD.UICore;
using System.Windows.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Security.Cryptography;

namespace Mef_VisionFrame
{
    public partial class Form_CraftSetting : Form
    {
        string str_ConfigName = "";  //当前配方名
        string str_OldConfigName = "";//旧的配方名
        string str_DesFileName = "";//复制的配方名
        Rectangle Rectangle;

        public CraftBase M_craftBase = new CraftBase();

        public FormCraftBase formCraftBase = new FormCraftBase();
        MaterialCrafEnum CurrentMateriarl = 0;

        string CurrentCraftName = "";
        string SelectCraftName = "";

        bool isOpen = false;

        public Form_CraftSetting()
        {
            InitializeComponent();
            toolClass.AllNumericUpDownControlSubscribeMouseWheelEvent(this);
            comb_materialCraf.DataSource = Enum.GetNames(typeof(MaterialCrafEnum));
            Ini_CraftSetting();

        }

        public void Ini_CraftSetting()
        {
            isOpen = true;
            if (Project.Instance.craftBase == null)
            {
                formCraftBase.ReSetCraft();
                M_craftBase = formCraftBase.GetCraft();
            }
            else
            {
                M_craftBase = Project.Instance.craftBase;
            }
            SelectCraftName = CurrentCraftName = M_craftBase.CraftName;
            CurrentMateriarl = M_craftBase.materialCrafEnum;
           
            if (CurrentMateriarl == (MaterialCrafEnum)comb_materialCraf.SelectedIndex)
            {
                SelectTargetItem(CurrentCraftName);
            }
            else
            {
                comb_materialCraf.SelectedIndex = (int)CurrentMateriarl;
            }
        }


        /// <summary>
        /// 将配方名加载到配方列表控件上
        /// </summary>
        private void LoadProjectList()
        {


            lV_ProjectList.View = View.List;
            lV_ProjectList.Items.Clear();

            string Path = Project.Instance.paramPathSet.CraftPath + CurrentMateriarl.ToString() + "\\";
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
            DirectoryInfo dir = new DirectoryInfo(Path);
            FileInfo[] finfo = dir.GetFiles();
            if (finfo.Length == 0)//没有工艺时默认创建一个
            {
                tSMI_CreateConfig_Click(null, null);
            }
            for (int i = 0; i < finfo.Length; i++)
            {
                if (/*finfo[i].Extension.ToLower() == ".cfts"||*/ finfo[i].Extension.ToLower() == ".json")
                {
                    string name = finfo[i].Name.Substring(0, finfo[i].Name.LastIndexOf('.'));
                    lV_ProjectList.Items.Add(name);
                }
            }

        }
        /// <summary>
        /// 根据类型和名称，选中配方
        /// </summary>
        /// <param name="Materiarl"></param>
        /// <param name="TargetCraftName"></param>
        public void SelectTargetItem(string TargetCraftName)
        {
            int index = 0;
            for (int i = 0; i < lV_ProjectList.Items.Count; i++)
            {
                if (lV_ProjectList.Items[i].Text == TargetCraftName)
                {
                    index = i;
                }
            }
            lV_ProjectList.Items[index].Selected = true;
            lV_ProjectList.FocusedItem = lV_ProjectList.Items[index]; // 设置焦点
            lV_ProjectList.EnsureVisible(0); // 确保项目可见
            SelectCraftName = lV_ProjectList.Items[index].Text;

            string path = Project.Instance.paramPathSet.CraftPath + CurrentMateriarl.ToString() + "\\" + SelectCraftName;
            M_craftBase = formCraftBase.ReadCraft(path);
            M_craftBase.CraftName = SelectCraftName;
            M_craftBase.backup(M_craftBase);
            YControlAutobind.Loading<CraftBase, IYCustomAttribute>(this, M_craftBase);//测试存修改记录

            formCraftBase.CraftToView();
            if (M_craftBase.CraftName != SelectCraftName)
            {
                M_craftBase.CraftName = SelectCraftName;
                formCraftBase.SaveCraft(path);
            }
        }

        #region 配方列表点击事件

        private void comb_materialCraf_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (true)
            {
                CurrentMateriarl = (MaterialCrafEnum)comb_materialCraf.SelectedIndex;
                isOpen = true;
                SwitchCraftForm(CurrentMateriarl);//切换界面，

                LoadProjectList();//刷新list表单
                SelectTargetItem(CurrentCraftName);
            }

        }
        private void lV_ProjectList_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                SelectCraftName = lV_ProjectList.FocusedItem.SubItems[0].Text;
                SelectTargetItem(SelectCraftName);
            }
            catch (Exception ee)
            {
            }
        }
        private void lV_ProjectList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string str = $"是否切换配方?\r\n " + $"工艺生产型号:{M_craftBase.materialCrafEnum}\r\n" + $"工艺配方名:{M_craftBase.CraftName}\r\n"
          + $"主相机:{M_craftBase.BaseCamera}\r\n" + $"工艺极性:{M_craftBase.electrode}\r\n";
            if (System.Windows.Forms.MessageBox.Show(str, "请停机状态切换,切换后软件自动关闭", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {

                Project.Instance.craftBase = M_craftBase;
                Project.Instance.systemInfo.CurrentCraft = M_craftBase.CraftName;
                Project.Instance.systemInfo.CurrentMaterial = CurrentMateriarl;
                Project.Instance.systemInfo.Save();
                this.Hide();

                // this.Close(); //关闭工艺设置界面
                Environment.Exit(0); //关闭整个工程
            }
        }
        #endregion

        #region 窗体控件重绘，防止闪烁
        ///<summary>
        ///窗体控件重绘，防止闪烁
        ///</summary>
        [DllImport("user32")]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, IntPtr lParam);
        private const int WM_SETREDRAW = 0xB;
        #endregion

        #region 配方菜单栏事件
        private void tSMI_CreateConfig_Click(object sender, EventArgs e)
        {
            SwitchCraftForm(CurrentMateriarl);
            formCraftBase.ReSetCraft();
            M_craftBase = formCraftBase.GetCraft();
            M_craftBase.CraftName = "新建配方";

            M_craftBase.filePath = Project.Instance.paramPathSet.CraftPath + CurrentMateriarl.ToString() + "\\" + M_craftBase.CraftName;
            formCraftBase.SaveCraft(M_craftBase.filePath);

            LoadProjectList();
            str_ConfigName = str_OldConfigName = M_craftBase.CraftName;
            Rectangle = lV_ProjectList.GetItemRect(lV_ProjectList.Items.Count - 1);
            Rectangle.X = gB_Config.Left + lV_ProjectList.Left + Rectangle.X + 2;
            Rectangle.Y = /*gB_Config.Top + */lV_ProjectList.Top + Rectangle.Y + 2;
            Rectangle.Width = Rectangle.Width + 2;
            tB_ConfigName.Visible = true;
            tB_ConfigName.Bounds = Rectangle;
            tB_ConfigName.BringToFront();
            tB_ConfigName.Text = str_ConfigName;
            tB_ConfigName.Focus();

        }

        private void tSMI_CopyConfig_Click(object sender, EventArgs e)
        {
            try
            {
                if (lV_ProjectList.FocusedItem != null)
                {
                    str_DesFileName = Project.Instance.paramPathSet.CraftPath + CurrentMateriarl.ToString() + "\\" + lV_ProjectList.FocusedItem.SubItems[0].Text;
                    File.Copy(str_DesFileName + ".json", str_DesFileName + "-复制.json");
                    LoadProjectList();

                    int index = 0;
                    for (int i = 0; i < lV_ProjectList.Items.Count; i++)
                    {
                        string strs = str_DesFileName.Substring(str_DesFileName.LastIndexOf('\\') + 1, str_DesFileName.Length - str_DesFileName.LastIndexOf('\\') - 1) + "-复制";
                        if (lV_ProjectList.Items[i].Text == strs)
                        {
                            index = i;
                        }
                    }
                    str_ConfigName = str_OldConfigName = lV_ProjectList.Items[index].Text;
                    Rectangle = lV_ProjectList.GetItemRect(index);
                    Rectangle.X = gB_Config.Left + lV_ProjectList.Left + Rectangle.X + 2;
                    Rectangle.Y = /*gB_Config.Top +*/ lV_ProjectList.Top + Rectangle.Y + 2;
                    Rectangle.Width = Rectangle.Width + 2;
                    tB_ConfigName.Visible = true;
                    tB_ConfigName.Bounds = Rectangle;
                    tB_ConfigName.BringToFront();
                    tB_ConfigName.Text = str_ConfigName;
                    tB_ConfigName.Focus();
                    LoadProjectList();
                    CopyFolder(str_DesFileName, Project.Instance.paramPathSet.CraftPath + CurrentMateriarl.ToString() + "\\" + str_ConfigName);

                }
                else
                {
                    MessageBox.Show("复制失败，未选中配方！");
                }

            }
            catch { }
        }

        private void tSMI_RemoveConfig_Click(object sender, EventArgs e)
        {
            try
            {
                if (lV_ProjectList.FocusedItem != null)
                {
                    string str = $"是否删除配方?\r\n " + $"工艺生产型号:{M_craftBase.materialCrafEnum}\r\n" + $"工艺配方名:{M_craftBase.CraftName}\r\n"
               + $"主相机:{M_craftBase.BaseCamera}\r\n" + $"工艺极性:{M_craftBase.electrode}\r\n";
                    if (System.Windows.Forms.MessageBox.Show(str, "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        File.Delete(Project.Instance.paramPathSet.CraftPath + CurrentMateriarl.ToString() + "\\" + lV_ProjectList.FocusedItem.SubItems[0].Text + ".json");
                        string path = Project.Instance.paramPathSet.CraftPath + CurrentMateriarl.ToString() + "\\" + lV_ProjectList.FocusedItem.SubItems[0].Text;
                        bool bl = Directory.Exists(path);
                        if (bl)
                        {
                            DirectoryInfo di = new DirectoryInfo(path);
                            di.Delete(true);
                        }
                        LoadProjectList();
                    }
                }
                else
                {
                    MessageBox.Show("删除失败，未选中配方！");
                }
            }
            catch (Exception ee)
            {
            }
        }

        private void tSMI_ReNameConfig_Click(object sender, EventArgs e)
        {
            if (lV_ProjectList.FocusedItem != null)
            {
                if (lV_ProjectList.FocusedItem.Text != CurrentCraftName)
                {
                    str_ConfigName = str_OldConfigName = lV_ProjectList.FocusedItem.Text;
                    Rectangle = lV_ProjectList.FocusedItem.Bounds;
                    Rectangle.X = gB_Config.Left + lV_ProjectList.Left + Rectangle.X + 2;
                    Rectangle.Y = /*gB_Config.Top + */lV_ProjectList.Top + Rectangle.Y + 2;
                    Rectangle.Width = Rectangle.Width + 2;
                    tB_ConfigName.Visible = true;
                    tB_ConfigName.Bounds = Rectangle;
                    tB_ConfigName.BringToFront();
                    tB_ConfigName.Text = str_ConfigName;
                    tB_ConfigName.Focus();
                }
                else
                {
                    MessageBox.Show("不能重命名正在生产的配方！");
                }
            }
            else
            {
                MessageBox.Show("重命名失败，未选中配方！");
            }
        }

        private void tB_ConfigName_TextChanged(object sender, EventArgs e)
        {
            str_ConfigName = tB_ConfigName.Text;
        }
        private void tB_ConfigName_KeyDown(object sender, KeyEventArgs e)
        {
            if ((int)e.KeyCode == 13)
            {
                try
                {
                    str_ConfigName = tB_ConfigName.Text;
                    if (str_ConfigName == "新建配方")
                    {
                        System.Windows.Forms.MessageBox.Show("配方名不能为新建配方,请重新命名!!!", "提示");
                        return;
                    }
                    if (str_ConfigName == str_OldConfigName)
                    {
                        tB_ConfigName.Visible = false;
                        LoadProjectList();
                        return;
                    }
                    for (int i = 0; i < lV_ProjectList.Items.Count; i++)
                    {
                        if (str_ConfigName == lV_ProjectList.Items[i].Text)
                        {
                            System.Windows.Forms.MessageBox.Show("配方名不能重复,请重新命名!!!", "提示");
                            return;
                        }
                    }

                    if (str_OldConfigName != str_ConfigName)
                    {
                        File.Copy(Project.Instance.paramPathSet.CraftPath + CurrentMateriarl.ToString() + "\\" + str_OldConfigName + ".json", Project.Instance.paramPathSet.CraftPath + CurrentMateriarl.ToString() + "\\" + str_ConfigName + ".json");
                        File.Delete(Project.Instance.paramPathSet.CraftPath + CurrentMateriarl.ToString() + "\\" + str_OldConfigName + ".json");
                        UpdateDirectoryName(Project.Instance.paramPathSet.CraftPath + CurrentMateriarl.ToString() + "\\" + str_OldConfigName, Project.Instance.paramPathSet.CraftPath + CurrentMateriarl.ToString() + "\\" + str_ConfigName);
                    }
                    tB_ConfigName.Visible = false;
                    LoadProjectList();
                    SelectTargetItem(str_ConfigName);
                }
                catch { tB_ConfigName.Visible = false; }
            }
        }

        #endregion

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SaveCurrentCraft_Click(object sender, EventArgs e)
        {
            if (/*lV_ProjectList.FocusedItem != null ||*/ SelectCraftName == M_craftBase.CraftName)
            {
                string str = $"是否保存工艺?\r\n " + $"工艺生产型号:{M_craftBase.materialCrafEnum}\r\n" + $"工艺配方名:{M_craftBase.CraftName}\r\n"
                + $"主相机:{M_craftBase.BaseCamera}\r\n" + $"工艺极性:{M_craftBase.electrode}\r\n";
                if (MessageBox.Show(str, "保存工艺", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    M_craftBase = formCraftBase.CraftFromView();
                    if (M_craftBase.CraftName == CurrentCraftName)
                    {
                        Project.Instance.craftBase = M_craftBase;
                        Project.Instance.baseProDevice.INIPro();
                        BIZ_Switchover.IBIZWorkerBase.UpdateCraft();
                    }

                    //Project.Instance.systemInfo.Save();//保存系统参数

                    string path = Project.Instance.paramPathSet.CraftPath + CurrentMateriarl.ToString() + "\\" + SelectCraftName;
                    M_craftBase.filePath = path;
                    formCraftBase.SaveCraft(path);


                    ////修改记录设置 20250919暂时取消保存修改记录，后续该修改方法改为序列化对比方法。
                    //M_craftBase.FilePath = (Project.Instance.logParam.LogSavePath + "\\ParamsChangeLog\\");
                    //string error = YControlAutobind.Saveing<CraftBase, IYCustomAttribute>(this, M_craftBase);
                    //// 保存修改记录，保存后备份
                    //M_craftBase.saveChangeLog(M_craftBase, String.Format("工艺视觉设置:{0} 中 {1}", CurrentMateriarl.ToString(), SelectCraftName));
                    //M_craftBase.backup(M_craftBase);
                    //if (error != "") System.Windows.Forms.MessageBox.Show(error);

                    formCraftBase.CraftToView();

                    //
                    //if (M_craftBase.CraftName == CurrentCraftName)
                    //{
                    //    Project.Instance.craftBase = M_craftBase;
                    //}
                    // SelectTargetItem(CurrentCraftName);//保存之后刷新一次参数，暂时屏蔽20240814zqm
                }
                else
                {
                    MessageBox.Show("保存已取消！");
                }
            }
            else
            {
                MessageBox.Show("保存失败，未选中配方！");
            }


        }

        public void SwitchCraftForm(MaterialCrafEnum Materiarl)
        {
            if (Materiarl != CurrentMateriarl || isOpen)//判断是否切换了工艺类型，若没有切换，直接刷新参数。
            {
                if (formCraftBase != null) formCraftBase?.Close();
                GC.Collect();
                switch (Materiarl)
                {
                    case MaterialCrafEnum.PFT清洗机:
                        formCraftBase = new FormPFTCleanCraft(Materiarl);
                        break;
                    default:
                        formCraftBase = new FormPFTCleanCraft(Materiarl);
                        break;
                }
                panel_Main.Controls.Clear();
                formCraftBase.FormBorderStyle = FormBorderStyle.None;
                formCraftBase.Dock = DockStyle.Fill;
                formCraftBase.TopLevel = false;
                panel_Main.Controls.Add(formCraftBase);
                formCraftBase.Show();
                isOpen = false;
            }
            //刷新参数
            //formCraftBase.CraftToView();

        }

        /// <summary>
        /// 方法一：复制文件夹下的所有文件，适合重命名复制
        /// </summary>
        /// <param name="sourceFolder">原文件路径</param>
        /// <param name="destFolder">目标文件路径</param>
        /// <returns></returns>
        public int CopyFolder(string sourceFolder, string destFolder)
        {
            try
            {
                //如果目标路径不存在,则创建目标路径
                if (!System.IO.Directory.Exists(destFolder))
                {
                    System.IO.Directory.CreateDirectory(destFolder);
                }
                //得到原文件根目录下的所有文件
                string[] files = System.IO.Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    string name = System.IO.Path.GetFileName(file);
                    string dest = System.IO.Path.Combine(destFolder, name);
                    System.IO.File.Copy(file, dest);//复制文件
                }
                //得到原文件根目录下的所有文件夹
                string[] folders = System.IO.Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    string name = System.IO.Path.GetFileName(folder);
                    string dest = System.IO.Path.Combine(destFolder, name);

                    CopyFolder(folder, dest);//构建目标路径,递归复制文件
                }
                return 1;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return 0;
            }

        }

        /// <summary>
        /// 方法二：复制文件夹及文件,完全复制文件夹，适合直接复制
        /// </summary>
        /// <param name="sourceFolder">原文件路径</param>
        /// <param name="destFolder">目标文件路径</param>
        /// <returns></returns>
        public int CopyFolder2(string sourceFolder, string destFolder)
        {
            try
            {
                string folderName = System.IO.Path.GetFileName(sourceFolder);
                string destfolderdir = System.IO.Path.Combine(destFolder, folderName);
                string[] filenames = System.IO.Directory.GetFileSystemEntries(sourceFolder);
                foreach (string file in filenames)// 遍历所有的文件和目录
                {
                    if (System.IO.Directory.Exists(file))
                    {
                        string currentdir = System.IO.Path.Combine(destfolderdir, System.IO.Path.GetFileName(file));
                        if (!System.IO.Directory.Exists(currentdir))
                        {
                            System.IO.Directory.CreateDirectory(currentdir);
                        }
                        CopyFolder2(file, destfolderdir);
                    }
                    else
                    {
                        string srcfileName = System.IO.Path.Combine(destfolderdir, System.IO.Path.GetFileName(file));
                        if (!System.IO.Directory.Exists(destfolderdir))
                        {
                            System.IO.Directory.CreateDirectory(destfolderdir);
                        }
                        System.IO.File.Copy(file, srcfileName);
                    }
                }

                return 1;
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
                return 0;
            }

        }


        /// 修改文件夹名称
        /// </summary>
        /// <param name="currentPath">当前文件夹路径</param>
        /// <param name="targetPath">目标文件夹路径（即修改后的文件夹）</param>
        //private void UpdateDirectoryName(string currentPath, string targetPath)
        //{
        //    if (!Directory.Exists(currentPath))
        //    {
        //        return;
        //    }

        //    if (Directory.Exists(targetPath))
        //    {
        //        return;
        //    }

        //    Directory.Move(currentPath, targetPath);
        //}
        private void UpdateDirectoryName(string currentPath, string targetPath)
        {
            if (!Directory.Exists(currentPath))
            {
                return;
            }
            if (Directory.Exists(targetPath))
                return;
            //throw new IOException("Target directory already exists.");

            if (Path.GetPathRoot(currentPath) != Path.GetPathRoot(targetPath))
            {
                // 跨磁盘：复制后删除
                CopyFolder2(currentPath, Path.GetDirectoryName(targetPath));
                Directory.Delete(currentPath, recursive: true);
            }
            else
            {
                Directory.Move(currentPath, targetPath);
            }
        }

        private void tSMI_OpenConfigFile_Click(object sender, EventArgs e)
        {
            string filePath = Project.Instance.paramPathSet.CraftPath + CurrentMateriarl.ToString() + "\\";
            //  string filePath = Project.Instance.paramPathSet.CraftPath;//文件（文件夹）路径
            System.Diagnostics.Process.Start(filePath);
        }
    }
}
