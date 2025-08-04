using Frame_Core;
using Mef_VisionFrame._5工艺标准;
using NPOI.SS.Formula.Functions;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media.Effects;
using Match = System.Text.RegularExpressions.Match;


namespace Mef_VisionFrame
{
    public partial class FormSlice1to2Craft : FormCraftBase
    {

        FormSizeFix formSizeFix = new FormSizeFix();
        public Slice1to2Craft slice1to2Craft { get; set; } = new Slice1to2Craft();
        public SizeItemInfo size = new SizeItemInfo();
        public DefectItemInfo defect = new DefectItemInfo();
        public SizeItemEditor editor = new SizeItemEditor();
        CameraEnum CurrentCam = CameraEnum.Cam1;

        string str_LineName = "";  //当前配方名
        string str_OldLineName = "";//旧的配方名
        string str_DesFileName = "";//复制的配方名
        Rectangle Rectangle;

        public CraftBase M_craftBase = new CraftBase();
        public List<CraftBase> M_craftList = new List<CraftBase>();

        public bool IsLinelistChanged = false;

        public FormSlice1to2Craft()
        {
            InitializeComponent();
            toolClass.AllNumericUpDownControlSubscribeMouseWheelEvent(this);
            comb_materialCraf.DataSource = Enum.GetNames(typeof(MaterialCrafEnum));
            comb_BaseCam.DataSource = Enum.GetNames(typeof(CameraEnum));
            comb_Electrode.DataSource = Enum.GetNames(typeof(ElectrodeEnum));
            comb_SelectCam.DataSource = Enum.GetNames(typeof(CameraEnum));
            Comb_EditorType.DataSource = Enum.GetNames(typeof(EditorTypeEnum));
            cbx_factory.AddItems(Enum.GetNames(typeof(FactoryEnum)), new string[] { FactoryEnum.北京纯锂.ToString(), FactoryEnum.嘉善兰钧.ToString() });


            InitDataGridView(DataGridView_Defect, typeof(DefectItemInfo));
            InitDataGridView(DataGridView_Size, typeof(SizeItemInfo));
            //  DataGridView_Size.Columns[5].ReadOnly = true;
            InitDataGridView(DataGridView_Editor, typeof(SizeItemEditor));

            DataGridView_Editor.Controls.Add(Comb_EditorType);
            DataGridView_Editor.Controls.Add(Comb_Line);
        }
        public override void CraftToView()
        {
            //显示基础参数
            linkLabel_CraftName.Text = slice1to2Craft.CraftName;
            //复选框
            comb_materialCraf.SelectedIndex = this.comb_materialCraf.FindString(slice1to2Craft.materialCrafEnum.ToString());
            comb_BaseCam.SelectedIndex = this.comb_BaseCam.FindString(slice1to2Craft.BaseCamera.ToString());
            comb_Electrode.SelectedIndex = this.comb_Electrode.FindString(slice1to2Craft.electrode.ToString());
            cbx_factory.SetSelectedIndex(slice1to2Craft.factory);
            tbx_MachineID.Text = Project.Instance.systemInfo.MachineID;
            //勾选框
            //  cB_IsHostMachine.Checked = dieCutCraft.IsHostMachine;

            //标准极耳工艺
            nud_EASizeX.Value = (decimal)slice1to2Craft.GeneralCarft.EASizeX;
            nud_ATSizeX.Value = (decimal)slice1to2Craft.GeneralCarft.ATSizeX;
            nud_CoatSizeX.Value = (decimal)slice1to2Craft.GeneralCarft.CoatSizeX;
            nud_JRSizeX.Value = (decimal)slice1to2Craft.GeneralCarft.JRSizeX;

            CurrentCam = (CameraEnum)Enum.Parse(typeof(CameraEnum), comb_SelectCam.SelectedItem.ToString(), false);
            ShowCurrentCam(slice1to2Craft, CurrentCam);
        }
        public override CraftBase CraftFromView()
        {
            //显示基础参数
            slice1to2Craft.materialCrafEnum = MaterialCrafEnum.分条机1出2;

            //复选框
            slice1to2Craft.BaseCamera = (CameraEnum)Enum.Parse(typeof(CameraEnum), comb_BaseCam.SelectedItem.ToString(), false);
            slice1to2Craft.electrode = (ElectrodeEnum)Enum.Parse(typeof(ElectrodeEnum), comb_Electrode.SelectedItem.ToString(), false);
            slice1to2Craft.factory = cbx_factory.GetSelectedIndex();
            Project.Instance.systemInfo.MachineID = tbx_MachineID.Text.Trim();
            //勾选框
            // dieCutCraft.IsHostMachine = cB_IsHostMachine.Checked;

            //标准极耳工艺
            slice1to2Craft.GeneralCarft.EASizeX = (double)nud_EASizeX.Value;
            slice1to2Craft.GeneralCarft.ATSizeX = (double)nud_ATSizeX.Value;
            slice1to2Craft.GeneralCarft.CoatSizeX = (double)nud_CoatSizeX.Value;
            slice1to2Craft.GeneralCarft.JRSizeX = (double)nud_JRSizeX.Value;

            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Restart();
            // Thread.Sleep(5000); // 耗时操作
            RefreshCurrentCam(slice1to2Craft, CurrentCam);

            //stopWatch.Stop();

            //// 将经过的时间作为毫秒数
            //long mSeconds = stopWatch.ElapsedMilliseconds;
            //Console.WriteLine("RunTime(ms) " + mSeconds);

            return slice1to2Craft;

        }

        public override bool SaveCraft(string Path)
        {
            // CraftFromView();
            if (slice1to2Craft.CraftName == Project.Instance.craftBase.CraftName)
            {
                if (IsLinelistChanged)
                {
                    System.Windows.MessageBox.Show("抓边队列已改变，正在重建抓边");
                    ReBuildLineProcess();
                }
                IsLinelistChanged = false;
            }

            slice1to2Craft.fileFullPath = Path;
            return slice1to2Craft.Save();
        }
        public override CraftBase ReadCraft(string Path)
        {
            slice1to2Craft.filePath = Path;
            slice1to2Craft = (Slice1to2Craft)slice1to2Craft.Read<Slice1to2Craft>();
            if (slice1to2Craft == null)
            {
                slice1to2Craft = new Slice1to2Craft();
            }
            foreach (CameraEnum item in Enum.GetValues(typeof(CameraEnum)))
            {
                if (slice1to2Craft.CamerCraft[item].ListLines == null)
                {
                    slice1to2Craft.CamerCraft[item].ListLines = new List<LineInfo>();
                }
                if (slice1to2Craft.CamerCraft[item].ListsizeItemEditors == null)
                {
                    slice1to2Craft.CamerCraft[item].ListsizeItemEditors = new List<SizeItemEditor>();
                }
            }
            slice1to2Craft.fileFullPath = Path;
            return slice1to2Craft;
        }

        public override void ReSetCraft()
        {
            slice1to2Craft = new Slice1to2Craft();
            slice1to2Craft.Init();
            GC.Collect();
        }
        public override CraftBase GetCraft()
        {
            return slice1to2Craft;
        }


        ////DataGridView表格操作方法
        /// <summary>
        /// 初始化DataGridView表格
        /// </summary>
        /// <param name="DGView"></param>
        /// <param name="ty"></param>
        private void InitDataGridView(DataGridView DGView, Type ty)
        {
            DGView.Rows.Clear();
            DGView.Columns.Clear();
            //根据Header和所有单元格的内容自动调整行的高度
            DGView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            //设置对齐方式和字体
            DGView.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DGView.Font = new Font("黑体", 14);

            //  Type ty = size.GetType();
            PropertyInfo[] infos = ty.GetProperties();
            foreach (PropertyInfo pi in infos)
            {
                //var classIfno = ty.GetCustomAttribute<MyAttribute>();
                var fieldIfno = ty.GetProperty(pi.Name).GetCustomAttribute<MyAttribute>();
                //var fieldIfno = typeof(SizeItemInfo).GetProperty(pi.Name).GetCustomAttribute<MyAttribute>();
                //var methodIfno = typeof(SizeItemInfo).GetMethod(pi.Name).GetCustomAttribute<MyAttribute>();

                DataGridViewColumn dgvC;
                if (pi.PropertyType == typeof(Boolean))
                {
                    dgvC = new DataGridViewCheckBoxColumn();
                    // dgvC.Width = 60;
                }
                else
                {
                    dgvC = new DataGridViewTextBoxColumn();
                }
                dgvC.Name = pi.Name;
                dgvC.HeaderText = fieldIfno.HeadName;
                dgvC.SortMode = DataGridViewColumnSortMode.NotSortable;//禁用排序
                if (fieldIfno.Is_ShowCol)
                {
                    dgvC.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;  //设置所有列自适应宽度
                    DGView.Columns.Add(dgvC);
                }
            }

            DGView.Columns[0].ReadOnly = true;
            DGView.Columns[2].ReadOnly = true;
        }
        /// <summary>
        /// 添加尺寸DataGridView数据
        /// </summary>
        /// <param name="DGView"></param>
        /// <param name="item"></param>
        public void AddSizeData(DataGridView DGView, SizeItemInfo item, bool showAll)
        {
            if (item != null)
            {
                if (item.Enable == true || showAll == true) //启用了就添加
                {
                    int count = DGView.Rows.Count;
                    DGView.Rows.Add();
                    PropertyInfo[] infos = item.GetType().GetProperties();
                    foreach (PropertyInfo pi in infos)
                    {
                        var fieldIfno = typeof(SizeItemInfo).GetProperty(pi.Name).GetCustomAttribute<MyAttribute>();
                        if (fieldIfno.Is_ShowCol)
                        {
                            DGView[pi.Name, count].Value = pi.GetValue(item, null);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 添加缺陷DataGridView数据
        /// </summary>
        /// <param name="DGView"></param>
        /// <param name="item"></param>
        public void AddDefectData(DataGridView DGView, DefectItemInfo item, bool showAll)
        {
            if (item != null)
            {
                if (item.Enable == true || showAll == true) //启用了就添加
                {
                    int count = DGView.Rows.Count;
                    DGView.Rows.Add();
                    PropertyInfo[] infos = item.GetType().GetProperties();
                    foreach (PropertyInfo pi in infos)
                    {
                        var fieldIfno = typeof(DefectItemInfo).GetProperty(pi.Name).GetCustomAttribute<MyAttribute>();
                        if (fieldIfno.Is_ShowCol)
                        {
                            DGView[pi.Name, count].Value = pi.GetValue(item, null);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 添加尺寸编辑器DataGridView数据
        /// </summary>
        /// <param name="DGView"></param>
        /// <param name="item"></param>
        public void AddSizeEditorData(DataGridView DGView, SizeItemEditor item, bool showAll)
        {
            if (item != null)
            {
                if (item.Enable == true || showAll == true) //启用了就添加
                {
                    int a = item.Index;
                    int count = DGView.Rows.Count;
                    DGView.Rows.Add();
                    PropertyInfo[] infos = item.GetType().GetProperties();
                    foreach (PropertyInfo pi in infos)
                    {
                        var fieldIfno = typeof(SizeItemEditor).GetProperty(pi.Name).GetCustomAttribute<MyAttribute>();
                        if (fieldIfno.Is_ShowCol)
                        {
                            DGView[pi.Name, count].Value = pi.GetValue(item, null);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 将配方名加载到配方列表控件上
        /// </summary>
        private void LoadProjectList()
        {
            lV_LineList.View = View.Tile;
            lV_LineList.TileSize = new System.Drawing.Size(lV_LineList.Size.Width - 10, 30);
            lV_LineList.Groups.Clear();
            lV_LineList.Items.Clear();
            //建立两个组
            ListViewGroup group1 = new ListViewGroup("基准边");
            ListViewGroup group2 = new ListViewGroup(" 插件抓边");
            lV_LineList.Groups.Add(group1);
            lV_LineList.Groups.Add(group2);


            int LineCount = slice1to2Craft.CamerCraft[CurrentCam].ListLines.Count;
            for (int i = 0; i < LineCount; i++)
            {
                if (slice1to2Craft.CamerCraft[CurrentCam].ListLines[i].IsStandLine)
                {
                    ListViewItem item = new ListViewItem(slice1to2Craft.CamerCraft[CurrentCam].ListLines[i].StrLineName, group1);
                    lV_LineList.Items.Add(item);
                }
                else
                {
                    ListViewItem item = new ListViewItem(slice1to2Craft.CamerCraft[CurrentCam].ListLines[i].StrLineName, group2);
                    lV_LineList.Items.Add(item);
                }
            }



        }
        #region 配方菜单栏事件

        private void tSMI_CreateStandLine_Click(object sender, EventArgs e)
        {
            slice1to2Craft.CamerCraft[CurrentCam].ListLines.Add(new LineInfo { StrLineName = "新建基准边", IsStandLine = true });
            LoadProjectList();
            str_LineName = str_OldLineName = "新建基准边";
            Rectangle = lV_LineList.GetItemRect(lV_LineList.Items.Count - 1);
            Rectangle.X = groupBox3.Left + lV_LineList.Left + Rectangle.X + 2;
            Rectangle.Y = groupBox3.Top + lV_LineList.Top + Rectangle.Y + 2;
            Rectangle.Width = Rectangle.Width + 2;
            tB_ConfigName.Visible = true;
            tB_ConfigName.Bounds = Rectangle;
            tB_ConfigName.BringToFront();
            tB_ConfigName.Text = str_LineName;
            tB_ConfigName.Focus();
        }
        private void tSMI_CreateLine_Click(object sender, EventArgs e)
        {
            try
            {
                slice1to2Craft.CamerCraft[CurrentCam].ListLines.Add(new LineInfo { StrLineName = "新建抓边", IsStandLine = false });
                LoadProjectList();
                str_LineName = str_OldLineName = "新建抓边";
                Rectangle = lV_LineList.GetItemRect(lV_LineList.Items.Count - 1);
                Rectangle.X = groupBox3.Left + lV_LineList.Left + Rectangle.X + 2;
                Rectangle.Y = groupBox3.Top + lV_LineList.Top + Rectangle.Y + 2;
                Rectangle.Width = Rectangle.Width + 2;
                tB_ConfigName.Visible = true;
                tB_ConfigName.Bounds = Rectangle;
                tB_ConfigName.BringToFront();
                tB_ConfigName.Text = str_LineName;
                tB_ConfigName.Focus();

                IsLinelistChanged = true;
            }
            catch (Exception ee)
            {

            }
        }
        private void tSMI_RenameLine_Click(object sender, EventArgs e)
        {
            str_LineName = str_OldLineName = lV_LineList.FocusedItem.Text;
            Rectangle = lV_LineList.FocusedItem.Bounds;
            Rectangle.X = groupBox3.Left + lV_LineList.Left + Rectangle.X + 2;
            Rectangle.Y = /*gB_Config.Top + */lV_LineList.Top + Rectangle.Y + 2;
            Rectangle.Width = Rectangle.Width + 2;
            tB_ConfigName.Visible = true;
            tB_ConfigName.Bounds = Rectangle;
            tB_ConfigName.BringToFront();
            tB_ConfigName.Text = str_LineName;
            tB_ConfigName.Focus();

            IsLinelistChanged = true;
        }

        private void tSMI_DeleteLine_Click(object sender, EventArgs e)
        {
            try
            {
                if (lV_LineList.FocusedItem != null)
                {
                    string str = $"是否删除抓边?\r\n " + $"抓边名称:{lV_LineList.FocusedItem.Text}\r\n";
                    if (System.Windows.Forms.MessageBox.Show(str, "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        int idx = slice1to2Craft.CamerCraft[CurrentCam].ListLines.FindIndex(c => c.StrLineName == lV_LineList.FocusedItem.Text);
                        slice1to2Craft.CamerCraft[CurrentCam].ListLines.RemoveAt(idx);
                        LoadProjectList();

                        IsLinelistChanged = true;
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("删除失败，未选中配方！");
                }
            }
            catch (Exception ee)
            {
            }
        }
        private void tB_ConfigName_TextChanged(object sender, EventArgs e)
        {
            str_LineName = tB_ConfigName.Text;
        }
        private void tB_ConfigName_KeyDown(object sender, KeyEventArgs e)
        {
            if ((int)e.KeyCode == 13)
            {
                try
                {
                    str_LineName = tB_ConfigName.Text;
                    if (str_LineName == "新建抓边" || str_LineName == "新建基准边")
                    {
                        System.Windows.Forms.MessageBox.Show($"抓边名不能为{str_LineName},请重新命名!!!", "提示");
                        return;
                    }
                    if (str_LineName == str_OldLineName)
                    {
                        tB_ConfigName.Visible = false;
                        LoadProjectList();
                        return;
                    }
                    for (int i = 0; i < lV_LineList.Items.Count; i++)
                    {
                        if (str_LineName == lV_LineList.Items[i].Text)
                        {
                            System.Windows.Forms.MessageBox.Show("配方名不能重复,请重新命名!!!", "提示");
                            return;
                        }
                    }

                    if (str_OldLineName != str_LineName)
                    {
                        int idx = slice1to2Craft.CamerCraft[CurrentCam].ListLines.FindIndex(c => c.StrLineName == str_OldLineName);
                        if (idx < 0)
                        {
                            slice1to2Craft.CamerCraft[CurrentCam].ListLines.Add(new LineInfo { StrLineName = str_LineName });
                        }
                        else
                        {
                            slice1to2Craft.CamerCraft[CurrentCam].ListLines[idx].StrLineName = str_LineName;
                        }

                    }
                    tB_ConfigName.Visible = false;
                    LoadProjectList();
                }
                catch (Exception ee)
                { tB_ConfigName.Visible = false; }
            }
        }

        #endregion



        /// <summary>
        /// 刷新表格数据到队列
        /// </summary>
        /// <param name="craftInfo"></param>
        public void RefreshCurrentCam(Slice1to2Craft craftInfo, CameraEnum Cam)
        {
            //复选框

            //勾选框
            craftInfo.CamerCraft[Cam].IsShowAllItems = cB_ShowAllItems.Checked;
            craftInfo.CamerCraft[Cam].IsEnableAllSize = cB_EnableAllSize.Checked;
            craftInfo.CamerCraft[Cam].IsEnableAllDefect = cB_EnableAllDefect.Checked;
            craftInfo.CamerCraft[Cam].IsLeft = cB_IsLeftJR.Checked;
            //极片尺寸
            craftInfo.CamerCraft[Cam].BaseSideX = (double)nud_BaseSide.Value;
            craftInfo.CamerCraft[Cam].GrayValue = (double)nud_MeanGrayCoat.Value;

            int count0 = DataGridView_Size.Rows.Count;
            int count1 = DataGridView_Defect.Rows.Count;
            int count2 = DataGridView_Editor.Rows.Count;
            // if (tabControl2.SelectedTab == tabPage_Size)//刷新尺寸表格
            {
                //craftInfo.dicCamCraft[Cam].SizeInfoList.Clear();
                for (int i = 0; i < count0; i++)
                {
                    string a = DataGridView_Size.Rows[i].Cells[2].Value.ToString();
                    SizeItemInfo item = craftInfo.CamerCraft[Cam].SizeInfoList.Find(c => c.Name == a);
                    PropertyInfo[] infos = typeof(SizeItemInfo).GetProperties();
                    //PropertyInfo[] infos = item.GetType().GetProperties();
                    foreach (PropertyInfo pi in infos)
                    {
                        var fieldIfno = typeof(SizeItemInfo).GetProperty(pi.Name).GetCustomAttribute<MyAttribute>();
                        // Type tp = pi.PropertyType;
                        if (fieldIfno.Is_ShowCol)
                        {
                            pi.SetValue(item, Convert.ChangeType(DataGridView_Size[pi.Name, i].Value, pi.PropertyType), null);
                        }
                    }
                }
            }
            // else if (tabControl2.SelectedTab == tabPage_Defect) //刷新缺陷表格
            {
                // craftInfo.dicCamCraft[Cam].DefectInfoList.Clear();
                for (int i = 0; i < count1; i++)
                {
                    string a = DataGridView_Defect.Rows[i].Cells[2].Value.ToString();
                    DefectItemInfo item = craftInfo.CamerCraft[Cam].DefectInfoList.Find(c => c.Name == a);
                    PropertyInfo[] infos = item.GetType().GetProperties();
                    foreach (PropertyInfo pi in infos)
                    {
                        var fieldIfno = typeof(DefectItemInfo).GetProperty(pi.Name).GetCustomAttribute<MyAttribute>();
                        if (fieldIfno.Is_ShowCol)
                        {
                            pi.SetValue(item, Convert.ChangeType(DataGridView_Defect[pi.Name, i].Value, pi.PropertyType), null);
                        }
                    }
                }
            }
            // else if (tabControl2.SelectedTab == tabPage_SizeEditor) //刷新尺寸编辑器表格
            {
                for (int i = 0; i < count2; i++)
                {
                    string a = DataGridView_Editor.Rows[i].Cells[2].Value.ToString();
                    SizeItemEditor item = craftInfo.CamerCraft[Cam].ListsizeItemEditors.Find(c => c.Name == a);
                    PropertyInfo[] infos = typeof(SizeItemEditor).GetProperties();
                    //PropertyInfo[] infos = item.GetType().GetProperties();
                    foreach (PropertyInfo pi in infos)
                    {
                        var fieldIfno = typeof(SizeItemEditor).GetProperty(pi.Name).GetCustomAttribute<MyAttribute>();
                        // Type tp = pi.PropertyType;
                        if (fieldIfno.Is_ShowCol)
                        {
                            pi.SetValue(item, Convert.ChangeType(DataGridView_Editor[pi.Name, i].Value, pi.PropertyType), null);
                        }
                    }
                }
            }



            for (int i = 0; i < craftInfo.CamerCraft[Cam].SizeInfoList.Count; i++)
            {
                craftInfo.CamerCraft[Cam].ListsizeItemEditors[i].Enable = craftInfo.CamerCraft[Cam].SizeInfoList[i].Enable;
            }
        }

        public void ShowCurrentCam(Slice1to2Craft craftInfo, CameraEnum Cam)
        {
            if (craftInfo == null) return;
            if (craftInfo.CamerCraft == null) return;
      
            //复选框

            //勾选框
            cB_ShowAllItems.Checked = craftInfo.CamerCraft[Cam].IsShowAllItems;
            cB_EnableAllSize.Checked = craftInfo.CamerCraft[Cam].IsEnableAllSize;
            cB_EnableAllDefect.Checked = craftInfo.CamerCraft[Cam].IsEnableAllDefect;
            cB_IsLeftJR.Checked = craftInfo.CamerCraft[Cam].IsLeft;

            //极片尺寸
            nud_BaseSide.Value = (decimal)craftInfo.CamerCraft[Cam].BaseSideX;
            nud_MeanGrayCoat.Value = (decimal)craftInfo.CamerCraft[Cam].GrayValue;

            var SizeName = Enum.GetNames(typeof(SizeEnum));
            var DectectName = Enum.GetNames(typeof(Defct1Enum));
            ////显示尺寸工艺表格
            if (tabControl2.SelectedTab == tabPage_Size)
            {
                DataGridView_Size.Rows.Clear();

                List<string> strings = new List<string>();
                foreach (var item in SizeName)
                {
                    strings.Add(item.ToString());
                }
                if (craftInfo.CamerCraft[Cam].SizeInfoList.Count > 0)
                {
                    foreach (var item in craftInfo.CamerCraft[Cam].SizeInfoList)
                    {
                        if (SizeName.Contains(item.Name))
                        {
                            strings.Remove(item.Name);
                        }
                    }
                    //修改枚举之后自动补充检测项
                    foreach (var item in strings)
                    {
                        Enum.TryParse(item, out SizeEnum sizeEnumValue);
                        SizeItemInfo sizeData = new SizeItemInfo();
                        sizeData.Name = item;
                        sizeData.Index = (byte)sizeEnumValue; // 1
                        craftInfo.CamerCraft[Cam].SizeInfoList.Add(sizeData);

                        SizeItemEditor sizeEditor = new SizeItemEditor();
                        sizeEditor.Name = item;
                        sizeEditor.Index = (byte)sizeEnumValue; // 1
                        craftInfo.CamerCraft[Cam].ListsizeItemEditors.Add(sizeEditor);
                    }
                    craftInfo.Save();
                }
                int SizeCount = craftInfo.CamerCraft[Cam].SizeInfoList.Count();
                for (int i = 0; i < SizeCount; i++)
                {
                    AddSizeData(DataGridView_Size, craftInfo.CamerCraft[Cam].SizeInfoList[i], craftInfo.CamerCraft[Cam].IsShowAllItems);
                }
            }
            else if (tabControl2.SelectedTab == tabPage_SizeEditor)
            {
                foreach (SizeItemEditor item in slice1to2Craft.CamerCraft[CurrentCam].ListsizeItemEditors)
                {
                    if (item.Line1 == str_OldLineName)
                    {
                        item.Line1 = str_LineName;
                    }
                    if (item.Line2 == str_OldLineName)
                    {
                        item.Line2 = str_LineName;
                    }
                }
                //加载检测项编辑器表格
                DataGridView_Editor.Rows.Clear();

                List<string> stringseditor = new List<string>();

                foreach (var item in SizeName)
                {
                    stringseditor.Add(item.ToString());
                }
                if (craftInfo.CamerCraft[Cam].ListsizeItemEditors.Count > 0)
                {
                    foreach (var item in craftInfo.CamerCraft[Cam].ListsizeItemEditors)
                    {
                        if (SizeName.Contains(item.Name))
                        {
                            stringseditor.Remove(item.Name);
                        }
                    }
                    //修改枚举之后自动补充检测项
                    foreach (var item in stringseditor)
                    {
                        Enum.TryParse(item, out SizeEnum sizeEnumValue);
                        SizeItemEditor sizeEditor = new SizeItemEditor();
                        sizeEditor.Name = item;
                        sizeEditor.Index = (byte)sizeEnumValue; // 1
                        craftInfo.CamerCraft[Cam].ListsizeItemEditors.Add(sizeEditor);
                    }
                    craftInfo.Save();
                }

                int SizeCount0 = slice1to2Craft.CamerCraft[CurrentCam].ListsizeItemEditors.Count();
                for (int i = 0; i < SizeCount0; i++)
                {
                    AddSizeEditorData(DataGridView_Editor, craftInfo.CamerCraft[Cam].ListsizeItemEditors[i], craftInfo.CamerCraft[Cam].IsShowAllItems);
                }
            }
            else if (tabControl2.SelectedTab == tabPage_Defect)
            {
                //显示缺陷工艺表格
                DataGridView_Defect.Rows.Clear();

                List<string> stringsDefect = new List<string>();
                foreach (var item in DectectName)
                {
                    stringsDefect.Add(item.ToString());
                }
                if (craftInfo.CamerCraft[Cam].DefectInfoList.Count > 0)
                {
                    foreach (var item in craftInfo.CamerCraft[Cam].DefectInfoList)
                    {
                        if (DectectName.Contains(item.Name))
                        {
                            stringsDefect.Remove(item.Name);
                        }
                    }
                    //修改枚举之后自动补充检测项
                    foreach (var item in stringsDefect)
                    {
                        Enum.TryParse(item, out Defct1Enum dectectEnumValue);
                        DefectItemInfo defectData = new DefectItemInfo();
                        defectData.Name = item;
                        defectData.Index = (byte)dectectEnumValue;
                        craftInfo.CamerCraft[Cam].DefectInfoList.Add(defect);
                    }
                    craftInfo.Save();
                }
                int DefectCount = craftInfo.CamerCraft[Cam].DefectInfoList.Count();
                for (int i = 0; i < DefectCount; i++)
                {
                    AddDefectData(DataGridView_Defect, craftInfo.CamerCraft[Cam].DefectInfoList[i], craftInfo.CamerCraft[Cam].IsShowAllItems);
                }
            }
            LoadProjectList();
        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {

            ShowCurrentCam(slice1to2Craft, CurrentCam);
        }


        public void EnableAllSize(Slice1to2Craft craftInfo, CameraEnum Cam)
        {
            foreach (SizeItemInfo item in craftInfo.CamerCraft[Cam].SizeInfoList)
            {
                if (craftInfo.CamerCraft[Cam].IsEnableAllSize)
                {
                    item.Enable = true;
                }
                else
                {
                    item.Enable = false;
                }
            }
        }

        public void EnableAllDefect(Slice1to2Craft craftInfo, CameraEnum Cam)
        {
            foreach (DefectItemInfo item in craftInfo.CamerCraft[Cam].DefectInfoList)
            {
                if (craftInfo.CamerCraft[Cam].IsEnableAllDefect)
                {
                    item.Enable = true;
                }
                else
                {
                    item.Enable = false;
                }
            }
        }

        private void comb_SelectCam_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentCam = (CameraEnum)Enum.Parse(typeof(CameraEnum), comb_SelectCam.SelectedItem.ToString(), false);
            ShowCurrentCam(slice1to2Craft, CurrentCam);
        }
        private void cB_ShowAllItems_Click(object sender, EventArgs e)
        {
            slice1to2Craft.CamerCraft[CurrentCam].IsShowAllItems = cB_ShowAllItems.Checked;
            ShowCurrentCam(slice1to2Craft, CurrentCam);
        }
        private void cB_EnableAllSize_Click(object sender, EventArgs e)
        {
            slice1to2Craft.CamerCraft[CurrentCam].IsEnableAllSize = cB_EnableAllSize.Checked;
            EnableAllSize(slice1to2Craft, CurrentCam);
            ShowCurrentCam(slice1to2Craft, CurrentCam);
        }

        private void cB_EnableAllDefect_Click(object sender, EventArgs e)
        {
            slice1to2Craft.CamerCraft[CurrentCam].IsEnableAllDefect = cB_EnableAllDefect.Checked;
            EnableAllDefect(slice1to2Craft, CurrentCam);
            ShowCurrentCam(slice1to2Craft, CurrentCam);
        }


        private void btn_RebuildCraft_Click(object sender, EventArgs e)
        {
            if (System.Windows.Forms.MessageBox.Show("是否重建配方？", "参数恢复初始化状态", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (CameraEnum item in Enum.GetValues(typeof(CameraEnum)))
                {
                    ReBuildCraftSize(slice1to2Craft, item);
                    ReBuildCraftDefect(slice1to2Craft, item);
                    ReBuildCraftSizeEditor(slice1to2Craft, item);
                }
                ShowCurrentCam(slice1to2Craft, CurrentCam);
            }
        }
        /// <summary>
        /// DataGridView重新初始化定位距工艺
        /// </summary>
        /// <param name="craftInfo"></param>
        private void ReBuildCraftSize(Slice1to2Craft craftInfo, CameraEnum Cam)
        {
            SizeItemInfo size;
            craftInfo.CamerCraft[Cam].IsEnableAllSize = true;
            craftInfo.CamerCraft[Cam].SizeInfoList.Clear();
            foreach (SizeEnum sp in Enum.GetValues(typeof(SizeEnum)))
            {
                size = new SizeItemInfo();
                size.Name = sp.ToString();
                size.Index = (byte)sp; // 1
                craftInfo.CamerCraft[Cam].SizeInfoList.Add(size);
            }

        }
        /// <summary>
        /// DataGridView重新初始化槽位工艺
        /// </summary>
        /// <param name="craftInfo"></param>
        private void ReBuildCraftDefect(Slice1to2Craft craftInfo, CameraEnum Cam)
        {
            DefectItemInfo defect;
            craftInfo.CamerCraft[Cam].IsEnableAllDefect = true;
            craftInfo.CamerCraft[Cam].DefectInfoList.Clear();
            foreach (Defct1Enum sp in Enum.GetValues(typeof(Defct1Enum)))
            {
                defect = new DefectItemInfo();
                defect.Name = sp.ToString();
                defect.Index = (byte)sp;
                craftInfo.CamerCraft[Cam].DefectInfoList.Add(defect);
            }

        }

        /// <summary>
        /// DataGridView重新初始化定位距编辑器
        /// </summary>
        /// <param name="craftInfo"></param>
        private void ReBuildCraftSizeEditor(Slice1to2Craft craftInfo, CameraEnum Cam)
        {
            SizeItemEditor sizeeditor;
            craftInfo.CamerCraft[Cam].ListsizeItemEditors.Clear();
            foreach (SizeEnum sp in Enum.GetValues(typeof(SizeEnum)))
            {
                sizeeditor = new SizeItemEditor();
                sizeeditor.Name = sp.ToString();
                sizeeditor.Index = (byte)sp; // 1
                craftInfo.CamerCraft[Cam].ListsizeItemEditors.Add(sizeeditor);
            }

        }

        /// <summary>
        /// DataGridView编辑器单元格下拉选择框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView_Editor_CurrentCellChanged(object sender, EventArgs e)
        {
            if (DataGridView_Editor.CurrentCell == null) return;
            DataGridViewColumn column = DataGridView_Editor.CurrentCell.OwningColumn;
            //如果是要显示下拉列表的列的话
            if (column.Name.Equals("EditorType"))
            {
                int columnIndex = DataGridView_Editor.CurrentCell.ColumnIndex;
                int rowIndex = DataGridView_Editor.CurrentCell.RowIndex;
                Rectangle rect = DataGridView_Editor.GetCellDisplayRectangle(columnIndex, rowIndex, false);
                Comb_EditorType.Left = rect.Left;
                Comb_EditorType.Top = rect.Top;
                Comb_EditorType.Width = rect.Width;
                Comb_EditorType.Height = rect.Height;
                //将单元格的内容显示为下拉列表的当前项
                string consultingRoom = DataGridView_Editor.Rows[rowIndex].Cells[columnIndex].Value.ToString();
                int index = Comb_EditorType.Items.IndexOf(consultingRoom);

                Comb_EditorType.SelectedIndex = index;
                Comb_EditorType.Visible = true;
            }
            else
            {
                Comb_EditorType.Visible = false;
            }
            if (column.Name.Equals("Line1") || column.Name.Equals("Line2"))
            {

                Comb_Line.Items.Clear();
                foreach (var item in slice1to2Craft.CamerCraft[CurrentCam].ListLines)
                {
                    Comb_Line.Items.Add(item.StrLineName);
                }
                int columnIndex = DataGridView_Editor.CurrentCell.ColumnIndex;
                int rowIndex = DataGridView_Editor.CurrentCell.RowIndex;
                Rectangle rect = DataGridView_Editor.GetCellDisplayRectangle(columnIndex, rowIndex, false);
                Comb_Line.Left = rect.Left;
                Comb_Line.Top = rect.Top;
                Comb_Line.Width = rect.Width;
                Comb_Line.Height = rect.Height;
                //将单元格的内容显示为下拉列表的当前项
                string consultingRoom = DataGridView_Editor.Rows[rowIndex].Cells[columnIndex].Value.ToString();
                int index = Comb_Line.Items.IndexOf(consultingRoom);

                Comb_Line.SelectedIndex = index;
                Comb_Line.Visible = true;
            }
            else
            {
                Comb_Line.Visible = false;

            }
        }

        private void ComB_EditorType_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.Graphics.DrawString(Comb_EditorType.Items[e.Index].ToString(), e.Font, Brushes.Black,
                e.Bounds, StringFormat.GenericDefault);
        }

        private void ComB_EditorType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DataGridView_Editor.CurrentCell != null)
                DataGridView_Editor.CurrentCell.Value = Comb_EditorType.Items[Comb_EditorType.SelectedIndex];
        }


        private void Comb_Line_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.Graphics.DrawString(Comb_Line.Items[e.Index].ToString(), e.Font, Brushes.Black,
                e.Bounds, StringFormat.GenericDefault);
        }

        private void Comb_Line_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DataGridView_Editor.CurrentCell != null)
                DataGridView_Editor.CurrentCell.Value = Comb_Line.Items[Comb_Line.SelectedIndex];

        }

        private void DataGridView_Size_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int Index = (int)DataGridView_Size.CurrentRow.Cells[0].Value;
            formSizeFix.size = slice1to2Craft.CamerCraft[CurrentCam].SizeInfoList[Index];
            formSizeFix.ShowSet();
            formSizeFix.StartPosition = FormStartPosition.CenterScreen;

            formSizeFix.ShowDialog(this);
            slice1to2Craft.CamerCraft[CurrentCam].SizeInfoList[Index] = formSizeFix.size;
            slice1to2Craft.Save();
            ShowCurrentCam(slice1to2Craft, CurrentCam);
        }

        private void button_RebuildProcess_Click(object sender, EventArgs e)
        {
            if (slice1to2Craft.CraftName != Project.Instance.craftBase.CraftName)
            {
                System.Windows.MessageBox.Show($"当前应用配方为：{Project.Instance.craftBase.CraftName}\r\n" + $"重建抓边配方为：{slice1to2Craft.CraftName}\r\n" + "配方不匹配，禁止重建");
            }
            else
            {
                if (System.Windows.Forms.MessageBox.Show("是否重建抓边？", "抓边应用，且恢复初始化值", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    ReBuildLineProcess();
                }
            }

        }

        public void ReBuildLineProcess()
        {
            Project.Instance.baseProDevice.INIPro();
            foreach (ProcessServices item in Project.Instance.m_ProcessList)
            {
                item.process.DeleteAllModuleObj();
                item.InitProcess();
            }
            //保存流程
            string path = Project.Instance.paramPathSet.ProcessPath;
            bool bl = Directory.Exists(path);
            if (bl)
            {
                DirectoryInfo di = new DirectoryInfo(path);
                di.Delete(true);
            }
            Project.Instance.SaveProject();
        }

        private void DataGridView_Size_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox tx = e.Control as TextBox;
            tx.KeyPress -= new KeyPressEventHandler(tx_KeyPress);
            tx.KeyPress += new KeyPressEventHandler(tx_KeyPress);
        }

        private void tx_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '-' && e.KeyChar != '\b')
                e.Handled = true;
        }

        private void DataGridView_Size_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int col = e.ColumnIndex;
            if (DataGridView_Size.Columns[col] is DataGridViewCheckBoxColumn) return;
                   
            string val = DataGridView_Size.Rows[row].Cells[col].Value.ToString();
            string pattern = "^-?\\d+$|^(-?\\d+)(\\.\\d+)?$";
            Match m = Regex.Match(val, pattern);   // 匹配正则表达式

            if (m.Success)
            {
                DataGridView_Size.Rows[row].Cells[col].Value = val;
            }
            else
            {
                string sss1 = Regex.Replace(val, "[^0-9A-Za-z]", "");
                System.Windows.MessageBox.Show("输入数据格式错误，请检查后重新输入");
                if (sss1 == "")
                {
                    DataGridView_Size.Rows[row].Cells[col].Value = 10;
                }
                else
                {
                    DataGridView_Size.Rows[row].Cells[col].Value = sss1;
                }
            }
            //将string中的数学公式计算成数值。
            //DataTable dt = new DataTable();
            //var Result = dt.Compute(val, "");//将运算字符串转换成表达式运算
            //DataGridView_Size.SelectedCells[0].Value = Result.ToString();
        }
    }
}
