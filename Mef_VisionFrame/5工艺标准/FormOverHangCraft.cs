using Frame_Core;
using Mef_VisionFrame._5工艺标准;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.OpenXmlFormats.Wordprocessing;
using NPOI.POIFS.NIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public partial class FormOverHangCraft : FormCraftBase
    {
        public OverHangCraft craft;
        FormSizeFix formSizeFix = new FormSizeFix();

        string str_LineName = "";  //当前配方名
        string str_OldLineName = "";//旧的配方名
        Rectangle Rectangle;

        public int SelectCamIndex = 0;
        public bool IsLinelistChanged = false;

        public MaterialCrafEnum material;

        public FormOverHangCraft(MaterialCrafEnum Tempmaterial)
        {
            InitializeComponent();
            craft = new OverHangCraft();
            craft.Init();
            material = Tempmaterial;
            List<string> camName = new List<string>();
            List<string> camName1 = new List<string>();
            List<string> camNameS = new List<string>();
            List<string> camNameS1 = new List<string>();

            foreach (KeyValuePair<CameraEnum, CameraManager> item in Project.Instance.cameraManager)
            {
                camName.Add(item.Value.m_CamParam.CameraName);
                camName1.Add(item.Value.m_CamParam.CameraName);
                camNameS.Add(item.Key.ToString());
                camNameS1.Add(item.Key.ToString());
            }

            CB_LastFloorMeasureTransition.DataSource = Enum.GetNames(typeof(MeasureTransition));
            comb_materialCraf.DataSource = Enum.GetNames(typeof(MaterialCrafEnum));
            //相机名称
            comb_BaseCam.DataSource = camName;
            comb_SelectCam.DataSource = camName1;
            Comb_CamS.DataSource = camNameS;
            Comb_CamS1.DataSource = camNameS1;

            comb_BelongCamera.DataSource = Enum.GetNames(typeof(CameraEnum));
            //comb_BaseCam.DataSource = Enum.GetNames(typeof(CameraEnum));
            //comb_SelectCam.DataSource = Enum.GetNames(typeof(CameraEnum));
            // Comb_EditorTypeP.DataSource = Enum.GetNames(typeof(EditorTypeEnum));

            Comb_EditorTypeS.DataSource = Enum.GetNames(typeof(EditorTypeEnum));
            Comb_EditorTypeS1.DataSource = Enum.GetNames(typeof(EditorTypeEnum));

            InitListView();
            //InitDataGridView(DataGridView_Point, typeof(PointEdit));
            //DataGridView_Point.Controls.Add(Comb_LineP);
            //DataGridView_Point.Controls.Add(Comb_EditorTypeP);

            InitDataGridView(DataGridView_Editor, typeof(SizeItemInfo));
            DataGridView_Editor.Controls.Add(Comb_LineS);
            DataGridView_Editor.Controls.Add(Comb_EditorTypeS);
            DataGridView_Editor.Controls.Add(Comb_CamS);

            InitDataGridView(DataGridView_EASize1, typeof(SizeItemInfo));
            DataGridView_EASize1.Controls.Add(Comb_LineS1);
            DataGridView_EASize1.Controls.Add(Comb_EditorTypeS1);
            DataGridView_EASize1.Controls.Add(Comb_CamS1);


            toolClass.AllNumericUpDownControlSubscribeMouseWheelEvent(this);


        }

        public override void CraftToView()
        {
            //显示基础参数
            nud_CamNum.Value = (int)craft.CamNum;
            linkLabel_CraftName.Text = craft.CraftName;

            //复选框
            comb_materialCraf.SelectedIndex = this.comb_materialCraf.FindString(material.ToString());
            comb_BaseCam.SelectedIndex = (int)craft.BaseCamera;

            //工艺
            nud_StakeCathodeCount.Value = (int)craft.StakeCathodeCount;
            nud_AAStandardDown.Value = (decimal)craft.AAStandardDown;
            nud_AAStandardUp.Value = (decimal)craft.AAStandardUp;
            nud_OffSetUpperX.Value = (decimal)craft.XBiasMax;
            nud_OffSetLowerX.Value = (decimal)craft.XBiasMin;
            nud_OffSetUpperY.Value = (decimal)craft.YBiasMax;
            nud_OffSetLowerY.Value = (decimal)craft.YBiasMin;
            nud_OffSetUpperR.Value = (decimal)craft.AngleBiasMax;
            nud_OffSetLowerR.Value = (decimal)craft.AngleBiasMin;

            cbx_EnAACCDB.Checked = craft.EnAACCDB;
            cbx_EnOVHSize.Checked = craft.EnOVHSize;

            switch (craft.LastFloorMeasureTransition)
            {
                case "negative":
                    CB_LastFloorMeasureTransition.SelectedIndex = 0;
                    // CB_MeasureTransition.Text = "从明到暗";
                    break;
                case "positive":
                    CB_LastFloorMeasureTransition.SelectedIndex = 1;
                    // CB_MeasureTransition.Text = "从暗到明";
                    break;
                case "all":
                    CB_LastFloorMeasureTransition.SelectedIndex = 2;
                    //CB_MeasureTransition.Text = "不区分明暗";
                    break;
                default:
                    break;
            }


            //添加抓边队列
            LoadLinetList();

            //添加尺寸检测项表格
            DataGridView_Editor.Rows.Clear();
            int SizeCount = craft.ListSizeEdits[SelectCamIndex].Count();
            for (int i = 0; i < SizeCount; i++)
            {
                AddGridData<SizeItemInfo>(DataGridView_Editor, craft.ListSizeEdits[SelectCamIndex][i], true);
            }

            //添加阴阳极EASize检测项表格
            DataGridView_EASize1.Rows.Clear();
            //初始化检测项
            if (craft.ListEASizes == null)
            {
                craft.ListEASizes = new List<SizeItemInfo>();
            }
            int SizeCount1 = craft.ListEASizes.Count();
            for (int i = 0; i < SizeCount1; i++)
            {
                AddGridData<SizeItemInfo>(DataGridView_EASize1, craft.ListEASizes[i], true);
            }

        }
        public override CraftBase CraftFromView()
        {
            //显示基础参数
            craft.CamNum = (int)nud_CamNum.Value;
            craft.materialCrafEnum = material;
            craft.BaseCamera = (CameraEnum)comb_BaseCam.SelectedIndex;

            //工艺
            craft.StakeCathodeCount = (int)nud_StakeCathodeCount.Value;
            craft.AAStandardDown = (double)nud_AAStandardDown.Value;
            craft.AAStandardUp = (double)nud_AAStandardUp.Value;
            craft.XBiasMax = (double)nud_OffSetUpperX.Value;
            craft.XBiasMin = (double)nud_OffSetLowerX.Value;
            craft.YBiasMax = (double)nud_OffSetUpperY.Value;
            craft.YBiasMin = (double)nud_OffSetLowerY.Value;
            craft.AngleBiasMax = (double)nud_OffSetUpperR.Value;
            craft.AngleBiasMin = (double)nud_OffSetLowerR.Value;

            craft.EnAACCDB = cbx_EnAACCDB.Checked;
            craft.EnOVHSize = cbx_EnOVHSize.Checked;

            //显示尺寸检测项
            int count0 = DataGridView_Editor.Rows.Count;
            for (int i = 0; i < count0; i++)
            {

                SizeItemInfo item = craft.ListSizeEdits[SelectCamIndex][i];
                PropertyInfo[] infos = typeof(SizeItemInfo).GetProperties();
                foreach (PropertyInfo pi in infos)
                {
                    var fieldIfno = typeof(SizeItemInfo).GetProperty(pi.Name).GetCustomAttribute<MyAttribute>();
                    if (fieldIfno.Is_ShowCol)
                    {
                        pi.SetValue(item, Convert.ChangeType(DataGridView_Editor[pi.Name, i].Value, pi.PropertyType), null);
                    }
                }
            }

            //显示尺寸检测项
            int count1 = DataGridView_EASize1.Rows.Count;
            for (int i = 0; i < count1; i++)
            {

                SizeItemInfo item = craft.ListEASizes[i];
                PropertyInfo[] infos = typeof(SizeItemInfo).GetProperties();
                foreach (PropertyInfo pi in infos)
                {
                    var fieldIfno = typeof(SizeItemInfo).GetProperty(pi.Name).GetCustomAttribute<MyAttribute>();
                    if (fieldIfno.Is_ShowCol)
                    {
                        pi.SetValue(item, Convert.ChangeType(DataGridView_EASize1[pi.Name, i].Value, pi.PropertyType), null);
                    }
                }
            }

            return craft;
        }

        public override bool SaveCraft(string Path)
        {
            if (craft.CraftName == Project.Instance.craftBase.CraftName)
            {
                if (IsLinelistChanged)
                {
                    System.Windows.MessageBox.Show("抓边队列已改变，正在重建抓边");
                    ReBuildLineProcess();
                }
                IsLinelistChanged = false;
            }

            craft.filePath = Path;
            return craft.Save();
        }
        public override CraftBase ReadCraft(string Path)
        {
            craft.filePath = Path;
            try
            {
                craft = craft.Read<OverHangCraft>();
            }
            catch (Exception ee)
            {
                craft = new OverHangCraft();
                craft.Init();
            }
            craft.filePath = Path;
            return craft;
        }

        public override void ReSetCraft()
        {
            craft = new OverHangCraft();
            craft.Init();
            GC.Collect();
        }
        public override CraftBase GetCraft()
        {
            return craft;
        }

        ////ListView表格操作方法
        private void InitListView()
        {
            lV_LineList.View = View.Tile;
            lV_LineList.TileSize = new System.Drawing.Size(lV_LineList.Size.Width - 10, 30);
            lV_LineList.Groups.Clear();
            lV_LineList.Items.Clear();
            //建立两个组
            ListViewGroup group1 = new ListViewGroup("隔膜边");
            ListViewGroup group2 = new ListViewGroup("物料抓边");
            lV_LineList.Groups.Add(group1);
            lV_LineList.Groups.Add(group2);

            //foreach (CameraEnum item in Enum.GetValues(typeof(CameraEnum)))
            //{
            //    ListViewGroup group1 = new ListViewGroup(item.ToString());
            //    lV_LineList.Groups.Add(group1);
            //}
        }

        /// <summary>
        /// 将抓边加载到抓边列表
        /// </summary>
        private void LoadLinetList()
        {
            lV_LineList.Items.Clear();
            foreach (LineInfo item in craft.ListLines[SelectCamIndex])
            {
                if (item.IsStandLine)
                {
                    ListViewItem item0 = new ListViewItem(item.StrLineName, lV_LineList.Groups[0]);
                    lV_LineList.Items.Add(item0);
                }
                else
                {
                    ListViewItem item0 = new ListViewItem(item.StrLineName, lV_LineList.Groups[1]);
                    lV_LineList.Items.Add(item0);
                }
            }
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
                if (fieldIfno.HeadName == "编号" || fieldIfno.HeadName == "名称" || fieldIfno.HeadName == "备注" || fieldIfno.HeadName == "所属相机")
                {
                    dgvC.ReadOnly = true;
                }
            }

            //DGView.Columns[0].ReadOnly = true;
            //DGView.Columns[1].ReadOnly = true;
            //DGView.Columns[2].ReadOnly = true;
            //DGView.Columns[3].ReadOnly = true;
        }

        /// <summary>
        /// 添加尺寸DataGridView数据
        /// </summary>
        /// <param name="DGView"></param>
        /// <param name="item"></param>
        public void AddSizeData(DataGridView DGView, PointEdit item, bool showAll)
        {
            if (item != null)
            {
                if (showAll == true) //启用了就添加
                {
                    int count = DGView.Rows.Count;
                    DGView.Rows.Add();
                    PropertyInfo[] infos = item.GetType().GetProperties();
                    foreach (PropertyInfo pi in infos)
                    {
                        var fieldIfno = typeof(PointEdit).GetProperty(pi.Name).GetCustomAttribute<MyAttribute>();
                        if (fieldIfno.Is_ShowCol)
                        {
                            DGView[pi.Name, count].Value = pi.GetValue(item, null);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 添加DataGridView数据
        /// </summary>
        /// <param name="DGView"></param>
        /// <param name="item"></param>
        public void AddGridData<T>(DataGridView DGView, object item0, bool showAll)
        {
            T item = (T)item0;
            if (item != null)
            {
                if (showAll == true) //启用了就添加
                {
                    int count = DGView.Rows.Count;
                    DGView.Rows.Add();
                    PropertyInfo[] infos = item.GetType().GetProperties();
                    foreach (PropertyInfo pi in infos)
                    {
                        var fieldIfno = typeof(T).GetProperty(pi.Name).GetCustomAttribute<MyAttribute>();
                        if (fieldIfno.Is_ShowCol)
                        {
                            DGView[pi.Name, count].Value = pi.GetValue(item, null);
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 重建抓边插件方法，叠台工位需要重写
        /// </summary>
        public void ReBuildLineProcess()
        {
            //Project.Instance.baseProDevice.INIPro();
            //foreach (ProcessServices item in Project.Instance.m_ProcessList)
            //{
            //    item.process.DeleteAllModuleObj();
            //    item.InitProcess();
            //}
            ////保存流程
            //string path = Project.Instance.paramPathSet.ProcessPath;
            //bool bl = Directory.Exists(path);
            //if (bl)
            //{
            //    DirectoryInfo di = new DirectoryInfo(path);
            //    di.Delete(true);
            //}
            //Project.Instance.SaveProject();
        }

        #region 抓边操作事件

        private void Btn_AddLine_Click(object sender, EventArgs e)
        {
            try
            {
                LineInfo lineInfo = new LineInfo();
                lineInfo.StrLineName = str_LineName = str_OldLineName = "新建抓边";
                lineInfo.m_camera = (CameraEnum)comb_SelectCam.SelectedIndex;
                if (Cb_IsStandLine.Checked)
                {
                    lineInfo.IsStandLine = true; // true为隔膜边
                }
                else { lineInfo.IsStandLine = false; }


                craft.ListLines[SelectCamIndex].Add(lineInfo);
                LoadLinetList();


                Rectangle = lV_LineList.GetItemRect(lV_LineList.Items.Count - 1);
                Rectangle.X = groupBox3.Left + lV_LineList.Left + Rectangle.X + 2;
                Rectangle.Y =/* groupBox3.Top +*/ lV_LineList.Top + Rectangle.Y + 2;
                Rectangle.Width = Rectangle.Width + 2;
                tB_ConfigName.Visible = true;
                tB_ConfigName.Bounds = Rectangle;
                tB_ConfigName.BringToFront();
                tB_ConfigName.Text = str_LineName;
                tB_ConfigName.Focus();

                IsLinelistChanged = true;
                Btn_AddLine.Enabled = false;
            }
            catch (Exception ee)
            {

            }
        }

        private void button_RebuildLine_Click(object sender, EventArgs e)
        {
            string str = $"是否重建抓边工艺?\r\n " + $"工艺生产型号:{craft.materialCrafEnum}\r\n" + $"工艺配方名:{craft.CraftName}\r\n" + "重建抓边会清空所有抓边，恢复初始化值";

            if (MessageBox.Show(str, "重建抓边", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //初始化抓边
                if (craft.ListLines == null)
                {
                    craft.ListLines = new List<List<LineInfo>>();
                }
                else { craft.ListLines.Clear(); }

                for (int i = 0; i < craft.CamNum; i++)
                {
                    craft.ListLines.Add(new List<LineInfo>());
                }
                LoadLinetList();
                GC.Collect();
            }
        }
        #endregion

        #region Point检测项操作事件

        //private void Btn_AddPointItem_Click(object sender, EventArgs e)
        //{
        //}

        //private void Btn_DeletePointItem_Click(object sender, EventArgs e)
        //{


        //}

        //private void btn_RebuildPointItem_Click(object sender, EventArgs e)
        //{

        //}

        #endregion

        #region Szie检测项操作事件
        private void Btn_AddSizeItem_Click(object sender, EventArgs e)
        {
            SizeItemInfo st = new SizeItemInfo();

            st.Index = GetSizeInsertId(SelectCamIndex);
            st.Name = Tb_Name.Text;
            st.Description = Tb_Description.Text;
            st.camera = (CameraEnum)comb_BelongCamera.SelectedIndex;
            st.camera1 = ((CameraEnum)comb_BelongCamera.SelectedIndex).ToString();
            st.camera2 = ((CameraEnum)comb_BelongCamera.SelectedIndex).ToString();


            craft.ListSizeEdits[SelectCamIndex].Add(st);
            //  AddSizeData(DataGridView_Editor, pt, true);
            AddGridData<SizeItemInfo>(DataGridView_Editor, st, true);
        }
        private void Btn_DeleteSizeItem_Click(object sender, EventArgs e)
        {
            if (DataGridView_Editor.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选择一条要删除的项！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                try
                {
                    //根据标签ID删除信息
                    // Dclass.ExecuteSql("delete from SMS_InGenerate where ID='" + this.dataGridView1.SelectedRows[0].Cells[0].Value.ToString().Trim() + "'");
                    int Index = (int)DataGridView_Editor.SelectedRows[0].Index;
                    craft.ListSizeEdits[SelectCamIndex].RemoveAt(Index);
                    DataGridView_Editor.Rows.RemoveAt(Index);

                    //DataRowView drv = DataGridView_Editor.SelectedRows[0].DataBoundItem as DataRowView;
                    //drv.Row.Table.Rows.Remove(drv.Row); // 将要删除的行移除，更新时不影响数据库
                    DataGridView_Editor.Refresh();//更新
                    MessageBox.Show("删除成功！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void btn_RebuildSizeItem_Click(object sender, EventArgs e)
        {
            string str = $"是否重建检测项工艺?\r\n " + $"工艺生产型号:{craft.materialCrafEnum}\r\n" + $"工艺配方名:{craft.CraftName}\r\n" + "重建检测项会清空所有检测项，恢复初始化值";

            if (MessageBox.Show(str, "重建检测项", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //初始化检测项
                if (craft.ListSizeEdits == null)
                {
                    craft.ListSizeEdits = new List<List<SizeItemInfo>>();
                }
                else { craft.ListSizeEdits.Clear(); }

                for (int i = 0; i < craft.CamNum; i++)
                {
                    craft.ListSizeEdits.Add(new List<SizeItemInfo>());
                }
                CraftToView();
                GC.Collect();
            }

        }

        #endregion

        #region 抓边队列重命名，删除菜单操作

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
                        int idx = craft.ListLines[SelectCamIndex].FindIndex(c => c.StrLineName == lV_LineList.FocusedItem.Text);
                        craft.ListLines[SelectCamIndex].RemoveAt(idx);
                        LoadLinetList();

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
                    if (str_LineName == "新建抓边")
                    {
                        System.Windows.Forms.MessageBox.Show($"抓边名不能为{str_LineName},请重新命名!!!", "提示");
                        return;
                    }
                    if (str_LineName == str_OldLineName)
                    {
                        tB_ConfigName.Visible = false;
                        LoadLinetList();
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
                        int idx = craft.ListLines[SelectCamIndex].FindIndex(c => c.StrLineName == str_OldLineName);
                        if (idx < 0)
                        {
                            // slice1to2Craft.CamerCraftList[(int)CurrentCam].ListLines.Add(new LineInfo { StrLineName = str_LineName });
                        }
                        else
                        {
                            craft.ListLines[SelectCamIndex][idx].StrLineName = str_LineName;
                        }

                    }
                    tB_ConfigName.Visible = false;
                    LoadLinetList();
                    Btn_AddLine.Enabled = true;
                }
                catch (Exception ee)
                { tB_ConfigName.Visible = false; }
            }
        }
        #endregion

        /// <summary>
        /// 检测项列头双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView_Editor_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int Index = (int)DataGridView_Editor.CurrentRow.Cells[0].Value;
            formSizeFix.size = craft.ListSizeEdits[SelectCamIndex][Index];
            formSizeFix.ShowSet();
            formSizeFix.StartPosition = FormStartPosition.CenterScreen;

            formSizeFix.ShowDialog(this);
            craft.ListSizeEdits[SelectCamIndex][Index] = formSizeFix.size;
            craft.Save();
            CraftToView();
        }

        #region datagridview_Point下拉选择抓边
        ///// <summary>
        ///// DataGridView编辑器单元格下拉选择框
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        ////private void DataGridView_Point_CurrentCellChanged(object sender, EventArgs e)
        ////{
        ////    if (DataGridView_Point.CurrentCell == null) return;
        ////    DataGridViewColumn column = DataGridView_Point.CurrentCell.OwningColumn;
        ////    //如果是要显示下拉列表的列的话
        ////    if (column.Name.Equals("EditorType"))
        ////    {
        ////        int columnIndex = DataGridView_Point.CurrentCell.ColumnIndex;
        ////        int rowIndex = DataGridView_Point.CurrentCell.RowIndex;
        ////        Rectangle rect = DataGridView_Point.GetCellDisplayRectangle(columnIndex, rowIndex, false);
        ////        Comb_EditorTypeP.Left = rect.Left;
        ////        Comb_EditorTypeP.Top = rect.Top;
        ////        Comb_EditorTypeP.Width = rect.Width;
        ////        Comb_EditorTypeP.Height = rect.Height;
        ////        //将单元格的内容显示为下拉列表的当前项
        ////        string consultingRoom = DataGridView_Point.Rows[rowIndex].Cells[columnIndex].Value.ToString();
        ////        int index = Comb_EditorTypeP.Items.IndexOf(consultingRoom);

        ////        Comb_EditorTypeP.SelectedIndex = index;
        ////        Comb_EditorTypeP.Visible = true;
        ////    }
        ////    else
        ////    {
        ////        Comb_EditorTypeP.Visible = false;
        ////    }
        ////    //如果是要显示下拉列表的列的话
        ////    if (column.Name.Equals("Line1") || column.Name.Equals("Line2"))
        ////    {

        ////        Comb_LineP.Items.Clear();
        ////        foreach (var item in craft.ListLines[SelectCamIndex])
        ////        {
        ////            Comb_LineP.Items.Add(item.StrLineName);
        ////        }
        ////        int columnIndex = DataGridView_Point.CurrentCell.ColumnIndex;
        ////        int rowIndex = DataGridView_Point.CurrentCell.RowIndex;
        ////        Rectangle rect = DataGridView_Point.GetCellDisplayRectangle(columnIndex, rowIndex, false);
        ////        Comb_LineP.Left = rect.Left;
        ////        Comb_LineP.Top = rect.Top;
        ////        Comb_LineP.Width = rect.Width;
        ////        Comb_LineP.Height = rect.Height;
        ////        //将单元格的内容显示为下拉列表的当前项
        ////        string consultingRoom = DataGridView_Point.Rows[rowIndex].Cells[columnIndex].Value.ToString();
        ////        int index = Comb_LineP.Items.IndexOf(consultingRoom);

        ////        Comb_LineP.SelectedIndex = index;
        ////        Comb_LineP.Visible = true;
        ////    }
        ////    else
        ////    {
        ////        Comb_LineP.Visible = false;

        ////    }
        ////}
        //private void Comb_LineP_DrawItem(object sender, DrawItemEventArgs e)
        //{
        //    e.DrawBackground();
        //    e.Graphics.DrawString(Comb_LineP.Items[e.Index].ToString(), e.Font, Brushes.Black,
        //        e.Bounds, StringFormat.GenericDefault);
        //}
        //private void Comb_LineP_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    if (DataGridView_Point.CurrentCell != null)
        //        DataGridView_Point.CurrentCell.Value = Comb_LineP.Items[Comb_LineP.SelectedIndex];

        //}
        //private void Comb_EditorTypeP_DrawItem(object sender, DrawItemEventArgs e)
        //{
        //    e.DrawBackground();
        //    e.Graphics.DrawString(Comb_EditorTypeP.Items[e.Index].ToString(), e.Font, Brushes.Black,
        //        e.Bounds, StringFormat.GenericDefault);
        //}
        //private void Comb_EditorTypeP_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        if (DataGridView_Point.CurrentCell != null)
        //            DataGridView_Point.CurrentCell.Value = Comb_EditorTypeP.Items[Comb_EditorTypeP.SelectedIndex];
        //    }
        //    catch (Exception ee)
        //    {

        //    }

        //}

        #endregion

        #region datagridview_Point限制输入只能是数字

        //private void DataGridView_Point_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        //{
        //    TextBox txP = e.Control as TextBox;
        //    txP.KeyPress -= new KeyPressEventHandler(txP_KeyPress);
        //    txP.KeyPress += new KeyPressEventHandler(txP_KeyPress);
        //}

        //private void txP_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '-' && e.KeyChar != '\b')
        //        e.Handled = true;
        //}

        //private void DataGridView_Point_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        //{
        //    int row = e.RowIndex;
        //    int col = e.ColumnIndex;
        //    if (DataGridView_Point.Columns[col] is DataGridViewCheckBoxColumn) return;

        //    string val = DataGridView_Point.Rows[row].Cells[col].Value.ToString();
        //    string pattern = "^-?\\d+$|^(-?\\d+)(\\.\\d+)?$";
        //    Match m = Regex.Match(val, pattern);   // 匹配正则表达式

        //    if (m.Success)
        //    {
        //        DataGridView_Point.Rows[row].Cells[col].Value = val;
        //    }
        //    else
        //    {
        //        string sss1 = Regex.Replace(val, "[^0-9A-Za-z]", "");
        //        System.Windows.MessageBox.Show("输入数据格式错误，请检查后重新输入");
        //        if (sss1 == "")
        //        {
        //            DataGridView_Point.Rows[row].Cells[col].Value = 10;
        //        }
        //        else
        //        {
        //            DataGridView_Point.Rows[row].Cells[col].Value = sss1;
        //        }
        //    }
        //    //将string中的数学公式计算成数值。
        //    //DataTable dt = new DataTable();
        //    //var Result = dt.Compute(val, "");//将运算字符串转换成表达式运算
        //    //DataGridView_Size.SelectedCells[0].Value = Result.ToString();
        //}

        #endregion

        #region datagridview_Size下拉选择抓边，下拉选择计算方式
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
                Comb_EditorTypeS.Left = rect.Left;
                Comb_EditorTypeS.Top = rect.Top;
                Comb_EditorTypeS.Width = rect.Width;
                Comb_EditorTypeS.Height = rect.Height;
                //将单元格的内容显示为下拉列表的当前项
                string consultingRoom = DataGridView_Editor.Rows[rowIndex].Cells[columnIndex].Value.ToString();
                int index = Comb_EditorTypeS.Items.IndexOf(consultingRoom);

                Comb_EditorTypeS.SelectedIndex = index;
                Comb_EditorTypeS.Visible = true;
            }
            else
            {
                Comb_EditorTypeS.Visible = false;
            }
            //如果是要显示下拉列表的列的话
            if (column.Name.Equals("Line1") || column.Name.Equals("Line2"))
            {
                int columnIndex = DataGridView_Editor.CurrentCell.ColumnIndex;
                int rowIndex = DataGridView_Editor.CurrentCell.RowIndex;

                Comb_LineS.Items.Clear();
                string cam = DataGridView_Editor.Rows[rowIndex].Cells[columnIndex - 1].Value.ToString();
                int intcam = Comb_CamS.Items.IndexOf(cam);
                foreach (var item in craft.ListLines[intcam])
                {
                    Comb_LineS.Items.Add(item.StrLineName);
                }

                Rectangle rect = DataGridView_Editor.GetCellDisplayRectangle(columnIndex, rowIndex, false);
                Comb_LineS.Left = rect.Left;
                Comb_LineS.Top = rect.Top;
                Comb_LineS.Width = rect.Width;
                Comb_LineS.Height = rect.Height;
                //将单元格的内容显示为下拉列表的当前项
                string consultingRoom = DataGridView_Editor.Rows[rowIndex].Cells[columnIndex].Value.ToString();
                int index = Comb_LineS.Items.IndexOf(consultingRoom);
                if (index == -1)
                {
                    index = 0;
                }
                Comb_LineS.SelectedIndex = index;
                Comb_LineS.Visible = true;
            }
            else
            {
                Comb_LineS.Visible = false;

            }

            //如果是要显示下拉列表的列的话
            if (column.Name.Equals("camera1") || column.Name.Equals("camera2"))
            {
                int columnIndex = DataGridView_Editor.CurrentCell.ColumnIndex;
                int rowIndex = DataGridView_Editor.CurrentCell.RowIndex;
                Rectangle rect = DataGridView_Editor.GetCellDisplayRectangle(columnIndex, rowIndex, false);
                Comb_CamS.Left = rect.Left;
                Comb_CamS.Top = rect.Top;
                Comb_CamS.Width = rect.Width;
                Comb_CamS.Height = rect.Height;
                //将单元格的内容显示为下拉列表的当前项
                string consultingRoom = DataGridView_Editor.Rows[rowIndex].Cells[columnIndex].Value.ToString();
                int index = Comb_CamS.Items.IndexOf(consultingRoom);

                Comb_CamS.SelectedIndex = index;
                Comb_CamS.Visible = true;
            }
            else
            {
                Comb_CamS.Visible = false;
            }
        }

        private void Comb_LineS_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.Graphics.DrawString(Comb_LineS.Items[e.Index].ToString(), e.Font, Brushes.Black,
                e.Bounds, StringFormat.GenericDefault);
        }
        private void Comb_LineS_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DataGridView_Editor.CurrentCell != null)
                DataGridView_Editor.CurrentCell.Value = Comb_LineS.Items[Comb_LineS.SelectedIndex];

        }
        private void Comb_EditorTypeS_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.Graphics.DrawString(Comb_EditorTypeS.Items[e.Index].ToString(), e.Font, Brushes.Black,
                e.Bounds, StringFormat.GenericDefault);
        }
        private void Comb_EditorTypeS_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (DataGridView_Editor.CurrentCell != null)
                    DataGridView_Editor.CurrentCell.Value = Comb_EditorTypeS.Items[Comb_EditorTypeS.SelectedIndex];
            }
            catch (Exception ee)
            {

            }

        }

        private void Comb_CamS_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.Graphics.DrawString(Comb_CamS.Items[e.Index].ToString(), e.Font, Brushes.Black,
                e.Bounds, StringFormat.GenericDefault);
        }
        private void Comb_CamS_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (DataGridView_Editor.CurrentCell != null)
                    DataGridView_Editor.CurrentCell.Value = Comb_CamS.Items[Comb_CamS.SelectedIndex];
            }
            catch (Exception ee)
            {
            }
        }
        #endregion

        #region datagridview_Size限制输入只能是数字
        private void DataGridView_Editor_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox txS = e.Control as TextBox;
            txS.KeyPress -= new KeyPressEventHandler(txS_KeyPress);
            txS.KeyPress += new KeyPressEventHandler(txS_KeyPress);
        }

        private void txS_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '-' && e.KeyChar != '\b')
                e.Handled = true;
        }

        private void DataGridView_Editor_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int col = e.ColumnIndex;
            if (DataGridView_Editor.Columns[col] is DataGridViewCheckBoxColumn) return;

            string val = DataGridView_Editor.Rows[row].Cells[col].Value.ToString();
            string pattern = "^-?\\d+$|^(-?\\d+)(\\.\\d+)?$";
            Match m = Regex.Match(val, pattern);   // 匹配正则表达式

            if (m.Success)
            {
                DataGridView_Editor.Rows[row].Cells[col].Value = val;
            }
            else
            {
                string sss1 = Regex.Replace(val, "[^0-9A-Za-z]", "");
                System.Windows.MessageBox.Show("输入数据格式错误，请检查后重新输入");
                if (sss1 == "")
                {
                    DataGridView_Editor.Rows[row].Cells[col].Value = 10;
                }
                else
                {
                    DataGridView_Editor.Rows[row].Cells[col].Value = sss1;
                }
            }
            //将string中的数学公式计算成数值。
            //DataTable dt = new DataTable();
            //var Result = dt.Compute(val, "");//将运算字符串转换成表达式运算
            //DataGridView_Size.SelectedCells[0].Value = Result.ToString();
        }

        #endregion

        #region 添加阴阳极极片尺寸表格。

        /// <summary>
        /// EAsize检测项列头双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView_EASize1_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int Index = (int)DataGridView_EASize1.CurrentRow.Cells[0].Value;
            formSizeFix.size = craft.ListEASizes[Index];
            formSizeFix.ShowSet();
            formSizeFix.StartPosition = FormStartPosition.CenterScreen;

            formSizeFix.ShowDialog(this);
            craft.ListEASizes[Index] = formSizeFix.size;
            craft.Save();
            CraftToView();
        }

        /// <summary>
        /// DataGridView编辑器单元格下拉选择框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView_EASize1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (DataGridView_EASize1.CurrentCell == null) return;

            DataGridViewColumn column = DataGridView_EASize1.CurrentCell.OwningColumn;
            //如果是要显示下拉列表的列的话
            if (column.Name.Equals("EditorType"))
            {
                int columnIndex = DataGridView_EASize1.CurrentCell.ColumnIndex;
                int rowIndex = DataGridView_EASize1.CurrentCell.RowIndex;
                Rectangle rect = DataGridView_EASize1.GetCellDisplayRectangle(columnIndex, rowIndex, false);
                Comb_EditorTypeS1.Left = rect.Left;
                Comb_EditorTypeS1.Top = rect.Top;
                Comb_EditorTypeS1.Width = rect.Width;
                Comb_EditorTypeS1.Height = rect.Height;
                //将单元格的内容显示为下拉列表的当前项
                string consultingRoom = DataGridView_EASize1.Rows[rowIndex].Cells[columnIndex].Value.ToString();
                int index = Comb_EditorTypeS1.Items.IndexOf(consultingRoom);

                Comb_EditorTypeS1.SelectedIndex = index;
                Comb_EditorTypeS1.Visible = true;
            }
            else
            {
                Comb_EditorTypeS1.Visible = false;
            }
            //如果是要显示下拉列表的列的话
            if (column.Name.Equals("Line1") || column.Name.Equals("Line2"))
            {
                int columnIndex = DataGridView_EASize1.CurrentCell.ColumnIndex;
                int rowIndex = DataGridView_EASize1.CurrentCell.RowIndex;

                Comb_LineS1.Items.Clear();
                string cam = DataGridView_EASize1.Rows[rowIndex].Cells[columnIndex - 1].Value.ToString();
                int intcam = Comb_CamS1.Items.IndexOf(cam);
                foreach (var item in craft.ListLines[intcam])
                {
                    Comb_LineS1.Items.Add(item.StrLineName);
                }

                Rectangle rect = DataGridView_EASize1.GetCellDisplayRectangle(columnIndex, rowIndex, false);
                Comb_LineS1.Left = rect.Left;
                Comb_LineS1.Top = rect.Top;
                Comb_LineS1.Width = rect.Width;
                Comb_LineS1.Height = rect.Height;
                //将单元格的内容显示为下拉列表的当前项
                string consultingRoom = DataGridView_EASize1.Rows[rowIndex].Cells[columnIndex].Value.ToString();
                int index = Comb_LineS1.Items.IndexOf(consultingRoom);
                if (index == -1)
                {
                    index = 0;
                }
                Comb_LineS1.SelectedIndex = index;
                Comb_LineS1.Visible = true;
            }
            else
            {
                Comb_LineS1.Visible = false;

            }

            //如果是要显示下拉列表的列的话
            if (column.Name.Equals("camera1") || column.Name.Equals("camera2"))
            {
                int columnIndex = DataGridView_EASize1.CurrentCell.ColumnIndex;
                int rowIndex = DataGridView_EASize1.CurrentCell.RowIndex;
                Rectangle rect = DataGridView_EASize1.GetCellDisplayRectangle(columnIndex, rowIndex, false);
                Comb_CamS1.Left = rect.Left;
                Comb_CamS1.Top = rect.Top;
                Comb_CamS1.Width = rect.Width;
                Comb_CamS1.Height = rect.Height;
                //将单元格的内容显示为下拉列表的当前项
                string consultingRoom = DataGridView_EASize1.Rows[rowIndex].Cells[columnIndex].Value.ToString();
                int index = Comb_CamS1.Items.IndexOf(consultingRoom);

                Comb_CamS1.SelectedIndex = index;
                Comb_CamS1.Visible = true;
            }
            else
            {
                Comb_CamS1.Visible = false;
            }
        }

        private void Comb_LineS1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.Graphics.DrawString(Comb_LineS1.Items[e.Index].ToString(), e.Font, Brushes.Black,
                e.Bounds, StringFormat.GenericDefault);
        }
        private void Comb_LineS1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DataGridView_EASize1.CurrentCell != null)
                DataGridView_EASize1.CurrentCell.Value = Comb_LineS1.Items[Comb_LineS1.SelectedIndex];

        }
        private void Comb_EditorTypeS1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.Graphics.DrawString(Comb_EditorTypeS1.Items[e.Index].ToString(), e.Font, Brushes.Black,
                e.Bounds, StringFormat.GenericDefault);
        }
        private void Comb_EditorTypeS1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (DataGridView_EASize1.CurrentCell != null)
                    DataGridView_EASize1.CurrentCell.Value = Comb_EditorTypeS1.Items[Comb_EditorTypeS1.SelectedIndex];
            }
            catch (Exception ee)
            {

            }

        }

        private void Comb_CamS1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.Graphics.DrawString(Comb_CamS1.Items[e.Index].ToString(), e.Font, Brushes.Black,
                e.Bounds, StringFormat.GenericDefault);
        }
        private void Comb_CamS1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (DataGridView_EASize1.CurrentCell != null)
                    DataGridView_EASize1.CurrentCell.Value = Comb_CamS1.Items[Comb_CamS1.SelectedIndex];
            }
            catch (Exception ee)
            {
            }
        }

        #region datagridview_Size限制输入只能是数字
        private void DataGridView_EASize1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox txS = e.Control as TextBox;
            txS.KeyPress -= new KeyPressEventHandler(txS_KeyPress);
            txS.KeyPress += new KeyPressEventHandler(txS_KeyPress);
        }

        private void DataGridView_EASize1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex;
            int col = e.ColumnIndex;
            if (DataGridView_EASize1.Columns[col] is DataGridViewCheckBoxColumn) return;

            string val = DataGridView_EASize1.Rows[row].Cells[col].Value.ToString();
            string pattern = "^-?\\d+$|^(-?\\d+)(\\.\\d+)?$";
            Match m = Regex.Match(val, pattern);   // 匹配正则表达式

            if (m.Success)
            {
                DataGridView_EASize1.Rows[row].Cells[col].Value = val;
            }
            else
            {
                string sss1 = Regex.Replace(val, "[^0-9A-Za-z]", "");
                System.Windows.MessageBox.Show("输入数据格式错误，请检查后重新输入");
                if (sss1 == "")
                {
                    DataGridView_EASize1.Rows[row].Cells[col].Value = 10;
                }
                else
                {
                    DataGridView_EASize1.Rows[row].Cells[col].Value = sss1;
                }
            }
            //将string中的数学公式计算成数值。
            //DataTable dt = new DataTable();
            //var Result = dt.Compute(val, "");//将运算字符串转换成表达式运算
            //DataGridView_Size.SelectedCells[0].Value = Result.ToString();
        }

        #endregion
        #endregion




        private void comb_SelectCam_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                SelectCamIndex = comb_SelectCam.SelectedIndex;

                if (SelectCamIndex >= craft.CamNum)
                {
                    comb_SelectCam.SelectedIndex = craft.CamNum - 1;
                    SelectCamIndex = craft.CamNum - 1;
                }
                CraftToView();
            }
            catch (Exception ee)
            {


            }
        }

        private void Btn_ReDefault_Click(object sender, EventArgs e)
        {
            string str = $"是否初始化工艺?\r\n " + $"工艺生产型号:{craft.materialCrafEnum}\r\n" + $"工艺配方名:{craft.CraftName}\r\n" + "初始化工艺会将工艺全部修改为预设默认值";

            if (MessageBox.Show(str, "初始化工艺", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //初始化抓边
                if (craft.ListLines == null)
                {
                    craft.ListLines = new List<List<LineInfo>>();
                }
                else { craft.ListLines.Clear(); }

                for (int i = 0; i < craft.CamNum; i++)
                {
                    craft.ListLines.Add(new List<LineInfo>());
                }

                //初始化检测项
                if (craft.ListSizeEdits == null)
                {
                    craft.ListSizeEdits = new List<List<SizeItemInfo>>();
                }
                else { craft.ListSizeEdits.Clear(); }

                for (int i = 0; i < craft.CamNum; i++)
                {
                    craft.ListSizeEdits.Add(new List<SizeItemInfo>());
                }

                //初始化EA尺寸检测项
                if (craft.ListEASizes == null)
                {
                    craft.ListEASizes = new List<SizeItemInfo>();
                }
                else { craft.ListEASizes.Clear(); }

                ReDefault();


                LoadLinetList();
                CraftToView();
                GC.Collect();
            }
        }
        public void ReDefault()
        {
            if (craft.materialCrafEnum == MaterialCrafEnum.叠片对齐度OVH)
            {
                craft.CamNum = 8;

                for (int i = 0; i < craft.CamNum; i++)
                {
                    //阳极叠台A，cam1234，上单面，下单面
                    if (i == 0)
                    {
                        //抓边名
                        List<string> strnames = new List<string> { "4X边", "4Y边" };
                        for (int j = 0; j < strnames.Count; j++)
                        {
                            LineInfo lineInfo = new LineInfo();
                            lineInfo.StrLineName = strnames[j];
                            lineInfo.m_camera = (CameraEnum)i;
                            lineInfo.IsStandLine = false;
                            craft.ListLines[i].Add(lineInfo);
                        }
                        //没有检测项。
                    }
                    else if (i == 1)
                    {
                        //抓边名
                        List<string> strnames = new List<string> { "1X边", "1Y边", "AS1片", "AS1膜" };
                        for (int j = 0; j < strnames.Count; j++)
                        {
                            LineInfo lineInfo = new LineInfo();
                            lineInfo.StrLineName = strnames[j];
                            lineInfo.m_camera = (CameraEnum)i;
                            lineInfo.IsStandLine = false;
                            craft.ListLines[i].Add(lineInfo);
                        }
                        //只有一个检测项，AS1。
                        //对齐度
                        List<string> Lstrnames = new List<string> { "AS1" };
                        List<string> LstrDesp = new List<string> { "AS1" };
                        List<EditorTypeEnum> Lstrtype = new List<EditorTypeEnum> { EditorTypeEnum.SAOH };
                        List<string> Line1Name = new List<string> { "AS1片" };
                        List<string> Line2Name = new List<string> { "AS1膜" };
                        for (int k = 0; k < Lstrnames.Count; k++)
                        {
                            SizeItemInfo sizeItem = new SizeItemInfo();
                            sizeItem.Index = GetSizeInsertId(i);
                            sizeItem.Name = Lstrnames[k];
                            sizeItem.Description = LstrDesp[k];
                            sizeItem.camera = (CameraEnum)i;
                            sizeItem.camera1 = ((CameraEnum)i).ToString();
                            sizeItem.camera2 = ((CameraEnum)i).ToString();

                            sizeItem.EditorType = Lstrtype[k].ToString();
                            sizeItem.Line1 = Line1Name[k];
                            sizeItem.Line2 = Line2Name[k];
                            craft.ListSizeEdits[i].Add(sizeItem);
                        }
                    }
                    else if (i == 2)
                    {
                        //抓边名
                        List<string> strnames = new List<string> { "5X边", "5Y边", "AS3片", "AS3膜" };
                        for (int j = 0; j < strnames.Count; j++)
                        {
                            LineInfo lineInfo = new LineInfo();
                            lineInfo.StrLineName = strnames[j];
                            lineInfo.m_camera = (CameraEnum)i;
                            lineInfo.IsStandLine = false;
                            craft.ListLines[i].Add(lineInfo);
                        }
                        //只有一个检测项，AS3。
                        //对齐度
                        List<string> Lstrnames = new List<string> { "AS3" };
                        List<string> LstrDesp = new List<string> { "AS3" };
                        List<EditorTypeEnum> Lstrtype = new List<EditorTypeEnum> { EditorTypeEnum.SAOH };
                        List<string> Line1Name = new List<string> { "AS3片" };
                        List<string> Line2Name = new List<string> { "AS3膜" };
                        for (int k = 0; k < Lstrnames.Count; k++)
                        {
                            SizeItemInfo sizeItem = new SizeItemInfo();
                            sizeItem.Index = GetSizeInsertId(i);
                            sizeItem.Name = Lstrnames[k];
                            sizeItem.Description = LstrDesp[k];
                            sizeItem.camera = (CameraEnum)i;
                            sizeItem.camera1 = ((CameraEnum)i).ToString();
                            sizeItem.camera2 = ((CameraEnum)i).ToString();
                            sizeItem.EditorType = Lstrtype[k].ToString();
                            sizeItem.Line1 = Line1Name[k];
                            sizeItem.Line2 = Line2Name[k];
                            craft.ListSizeEdits[i].Add(sizeItem);
                        }
                    }
                    else if (i == 3)
                    {
                        //抓边名
                        List<string> strnames = new List<string> { "2X边", "2Y边", "3X边", "3Y边", "Corner1边", "ARC边", "Corner2边", "AS2片", "AS2膜" };
                        for (int j = 0; j < strnames.Count; j++)
                        {
                            LineInfo lineInfo = new LineInfo();
                            lineInfo.StrLineName = strnames[j];
                            lineInfo.m_camera = (CameraEnum)i;
                            lineInfo.IsStandLine = false;
                            craft.ListLines[i].Add(lineInfo);
                        }

                        //只有一个检测项，AS2。
                        //对齐度
                        List<string> Lstrnames = new List<string> { "AS2" };
                        List<string> LstrDesp = new List<string> { "AS2" };
                        List<EditorTypeEnum> Lstrtype = new List<EditorTypeEnum> { EditorTypeEnum.SAOH };
                        List<string> Line1Name = new List<string> { "AS2片" };
                        List<string> Line2Name = new List<string> { "AS2膜" };
                        for (int k = 0; k < Lstrnames.Count; k++)
                        {
                            SizeItemInfo sizeItem = new SizeItemInfo();
                            sizeItem.Index = GetSizeInsertId(i);
                            sizeItem.Name = Lstrnames[k];
                            sizeItem.Description = LstrDesp[k];
                            sizeItem.camera = (CameraEnum)i;
                            sizeItem.camera1 = ((CameraEnum)i).ToString();
                            sizeItem.camera2 = ((CameraEnum)i).ToString();
                            sizeItem.EditorType = Lstrtype[k].ToString();
                            sizeItem.Line1 = Line1Name[k];
                            sizeItem.Line2 = Line2Name[k];
                            craft.ListSizeEdits[i].Add(sizeItem);
                        }
                    }


                    //阴极叠台，cam5678
                    else if (i == 4)
                    {
                        //抓边名
                        List<string> strnames = new List<string> { "4X边", "4Y边" };
                        for (int j = 0; j < strnames.Count; j++)
                        {
                            LineInfo lineInfo = new LineInfo();
                            lineInfo.StrLineName = strnames[j];
                            lineInfo.m_camera = (CameraEnum)i;
                            lineInfo.IsStandLine = false;
                            craft.ListLines[i].Add(lineInfo);
                        }
                        //检测项，"4X", "4Y"
                        //对齐度
                        List<string> Lstrnames = new List<string> { "4X", "4Y" };
                        List<string> LstrDesp = new List<string> { "4X", "4Y" };
                        List<EditorTypeEnum> Lstrtype = new List<EditorTypeEnum> { EditorTypeEnum.ACOH_X, EditorTypeEnum.ACOH_Y };
                        List<string> Line1Name = new List<string> { "4X边", "4Y边" };
                        List<string> Line2Name = new List<string> { "4X边", "4Y边" };
                        for (int k = 0; k < Lstrnames.Count; k++)
                        {
                            SizeItemInfo sizeItem = new SizeItemInfo();
                            sizeItem.Index = GetSizeInsertId(i);
                            sizeItem.Name = Lstrnames[k];
                            sizeItem.Description = LstrDesp[k];
                            sizeItem.camera = (CameraEnum)i;
                            sizeItem.camera1 = ((CameraEnum)i).ToString();
                            sizeItem.camera2 = ((CameraEnum)i).ToString();
                            sizeItem.EditorType = Lstrtype[k].ToString();
                            sizeItem.Line1 = Line1Name[k];
                            sizeItem.Line2 = Line2Name[k];
                            craft.ListSizeEdits[i].Add(sizeItem);
                        }



                    }
                    else if (i == 5)
                    {
                        List<string> strnames = new List<string> { "1X边", "1Y边" };
                        for (int j = 0; j < strnames.Count; j++)
                        {
                            LineInfo lineInfo = new LineInfo();
                            lineInfo.StrLineName = strnames[j];
                            lineInfo.m_camera = (CameraEnum)i;
                            lineInfo.IsStandLine = false;
                            craft.ListLines[i].Add(lineInfo);
                        }
                        //检测项， "1X", "1Y" 
                        //对齐度
                        List<string> Lstrnames = new List<string> { "1X", "1Y" };
                        List<string> LstrDesp = new List<string> { "1X", "1Y" };
                        List<EditorTypeEnum> Lstrtype = new List<EditorTypeEnum> { EditorTypeEnum.ACOH_X, EditorTypeEnum.ACOH_Y };
                        List<string> Line1Name = new List<string> { "1X边", "1Y边" };
                        List<string> Line2Name = new List<string> { "1X边", "1Y边" };
                        for (int k = 0; k < Lstrnames.Count; k++)
                        {
                            SizeItemInfo sizeItem = new SizeItemInfo();
                            sizeItem.Index = GetSizeInsertId(i);
                            sizeItem.Name = Lstrnames[k];
                            sizeItem.Description = LstrDesp[k];
                            sizeItem.camera = (CameraEnum)i;
                            sizeItem.camera1 = ((CameraEnum)i).ToString();
                            sizeItem.camera2 = ((CameraEnum)i).ToString();
                            sizeItem.EditorType = Lstrtype[k].ToString();
                            sizeItem.Line1 = Line1Name[k];
                            sizeItem.Line2 = Line2Name[k];
                            craft.ListSizeEdits[i].Add(sizeItem);
                        }
                    }
                    else if (i == 6)
                    {
                        //抓边名
                        List<string> strnames = new List<string> { "5X边", "5Y边" };
                        for (int j = 0; j < strnames.Count; j++)
                        {
                            LineInfo lineInfo = new LineInfo();
                            lineInfo.StrLineName = strnames[j];
                            lineInfo.m_camera = (CameraEnum)i;
                            lineInfo.IsStandLine = false;
                            craft.ListLines[i].Add(lineInfo);
                        }
                        //检测项， "5X", "5Y"
                        //对齐度
                        List<string> Lstrnames = new List<string> { "5X", "5Y" };
                        List<string> LstrDesp = new List<string> { "5X", "5Y" };
                        List<EditorTypeEnum> Lstrtype = new List<EditorTypeEnum> { EditorTypeEnum.ACOH_X, EditorTypeEnum.ACOH_Y };
                        List<string> Line1Name = new List<string> { "5X边", "5Y边" };
                        List<string> Line2Name = new List<string> { "5X边", "5Y边" };
                        for (int k = 0; k < Lstrnames.Count; k++)
                        {
                            SizeItemInfo sizeItem = new SizeItemInfo();
                            sizeItem.Index = GetSizeInsertId(i);
                            sizeItem.Name = Lstrnames[k];
                            sizeItem.Description = LstrDesp[k];
                            sizeItem.camera = (CameraEnum)i;
                            sizeItem.camera1 = ((CameraEnum)i).ToString();
                            sizeItem.camera2 = ((CameraEnum)i).ToString();
                            sizeItem.EditorType = Lstrtype[k].ToString();
                            sizeItem.Line1 = Line1Name[k];
                            sizeItem.Line2 = Line2Name[k];
                            craft.ListSizeEdits[i].Add(sizeItem);
                        }
                    }
                    else if (i == 7)
                    {
                        //抓边名
                        List<string> strnames = new List<string> { "2X边", "2Y边", "3X边", "3Y边", "Corner1边", "ARC边", "Corner2边" };
                        for (int j = 0; j < strnames.Count; j++)
                        {
                            LineInfo lineInfo = new LineInfo();
                            lineInfo.StrLineName = strnames[j];
                            lineInfo.m_camera = (CameraEnum)i;
                            lineInfo.IsStandLine = false;
                            craft.ListLines[i].Add(lineInfo);
                        }
                        //检测项， "2X", "2Y", "3X", "3Y", "Corner1", "ARC", "Corner2" 
                        //对齐度
                        List<string> Lstrnames = new List<string> { "2X", "2Y", "3X", "3Y", "Corner1", "ARC", "Corner2" };
                        List<string> LstrDesp = new List<string> { "2X", "2Y", "3X", "3Y", "Corner1", "ARC", "Corner2" };
                        List<EditorTypeEnum> Lstrtype = new List<EditorTypeEnum> {
                                                               EditorTypeEnum.ACOH_X,EditorTypeEnum.ACOH_Y, EditorTypeEnum.ACOH_X,EditorTypeEnum.ACOH_Y,
                                               EditorTypeEnum.ACOH_Cor,EditorTypeEnum.ACOH_Cor,EditorTypeEnum.ACOH_Cor};
                        List<string> Line1Name = new List<string> { "2X边", "2Y边", "3X边", "3Y边", "Corner1边", "ARC边", "Corner2边", };
                        List<string> Line2Name = new List<string> { "2X边", "2Y边", "3X边", "3Y边", "Corner1边", "ARC边", "Corner2边", };

                        for (int k = 0; k < Lstrnames.Count; k++)
                        {
                            SizeItemInfo sizeItem = new SizeItemInfo();
                            sizeItem.Index = GetSizeInsertId(i);
                            sizeItem.Name = Lstrnames[k];
                            sizeItem.Description = LstrDesp[k];
                            sizeItem.camera = (CameraEnum)i;
                            sizeItem.camera1 = ((CameraEnum)i).ToString();
                            sizeItem.camera2 = ((CameraEnum)i).ToString();
                            sizeItem.EditorType = Lstrtype[k].ToString();
                            sizeItem.Line1 = Line1Name[k];
                            sizeItem.Line2 = Line2Name[k];
                            craft.ListSizeEdits[i].Add(sizeItem);
                        }
                    }
                }
                //添加阴阳极极片尺寸EASize
                if (true)
                {
                    // 对齐度尺寸计算-A叠片
                    List<string> Lstrnames = new List<string> { "AW1", "AW2", "AH1", "AH2" };
                    List<string> LstrDesp = new List<string> { "A极片全宽", "A极片宽2", "A极片全高", "A极片高2" };
                    List<EditorTypeEnum> Lstrtype = new List<EditorTypeEnum> { EditorTypeEnum.线距离X, EditorTypeEnum.线距离X, EditorTypeEnum.线距离Y, EditorTypeEnum.线距离Y };
                    List<CameraEnum> Line1Cam = new List<CameraEnum> { CameraEnum.Cam3, CameraEnum.Cam3, CameraEnum.Cam2, CameraEnum.Cam2 };
                    List<string> Line1Name = new List<string> { "5X边", "5X边", "1Y边", "1Y边" };
                    List<CameraEnum> Line2Cam = new List<CameraEnum> { CameraEnum.Cam4, CameraEnum.Cam4, CameraEnum.Cam4, CameraEnum.Cam4 };
                    List<string> Line2Name = new List<string> { "2X边", "3X边", "3Y边", "2Y边" };
                    for (int k = 0; k < Lstrnames.Count; k++)
                    {
                        SizeItemInfo sizeItem = new SizeItemInfo();
                        sizeItem.Index = GetSizeInsertId();
                        sizeItem.Name = Lstrnames[k];
                        sizeItem.Description = LstrDesp[k];
                        sizeItem.camera = (CameraEnum)0;
                        sizeItem.camera1 = Line1Cam[k].ToString();
                        sizeItem.camera2 = Line2Cam[k].ToString();
                        sizeItem.Line1 = Line1Name[k];
                        sizeItem.Line2 = Line2Name[k];
                        sizeItem.EditorType = Lstrtype[k].ToString();
                        craft.ListEASizes.Add(sizeItem);
                    }
                    // 对齐度尺寸计算-B叠片
                    List<string> Lstrnames1 = new List<string> { "BW1", "BW2", "BH1", "BH2" };
                    List<string> LstrDesp1 = new List<string> { "B极片全宽", "B极片宽2", "B极片全高", "B极片高2" };
                    List<EditorTypeEnum> Lstrtype1 = new List<EditorTypeEnum> { EditorTypeEnum.线距离X, EditorTypeEnum.线距离X, EditorTypeEnum.线距离Y, EditorTypeEnum.线距离Y };
                    List<CameraEnum> Line1Cam1 = new List<CameraEnum> { CameraEnum.Cam3, CameraEnum.Cam3, CameraEnum.Cam2, CameraEnum.Cam2 };
                    List<string> Line1Name1 = new List<string> { "5X边", "5X边", "1Y边", "1Y边" };
                    List<CameraEnum> Line2Cam1 = new List<CameraEnum> { CameraEnum.Cam4, CameraEnum.Cam4, CameraEnum.Cam4, CameraEnum.Cam4 };
                    List<string> Line2Name1 = new List<string> { "2X边", "3X边", "3Y边", "2Y边" };
                    for (int k = 0; k < Lstrnames1.Count; k++)
                    {
                        SizeItemInfo sizeItem = new SizeItemInfo();
                        sizeItem.Index = GetSizeInsertId();
                        sizeItem.Name = Lstrnames1[k];
                        sizeItem.Description = LstrDesp1[k];
                        sizeItem.camera = (CameraEnum)4;
                        sizeItem.camera1 = Line1Cam1[k].ToString();
                        sizeItem.camera2 = Line2Cam1[k].ToString();
                        sizeItem.Line1 = Line1Name1[k];
                        sizeItem.Line2 = Line2Name1[k];
                        sizeItem.EditorType = Lstrtype1[k].ToString();
                        craft.ListEASizes.Add(sizeItem);
                    }
                }
            }
            else if (craft.materialCrafEnum == MaterialCrafEnum.叠片预定位PP)
            {
                craft.CamNum = 4;
                for (int i = 0; i < craft.CamNum; i++)
                {
                    //抓边名
                    List<string> strnames = new List<string> { "抓边1X", "抓边2Y" };
                    for (int j = 0; j < strnames.Count; j++)
                    {
                        LineInfo lineInfo = new LineInfo();
                        lineInfo.StrLineName = strnames[j];
                        lineInfo.m_camera = (CameraEnum)i;
                        lineInfo.IsStandLine = false;
                        craft.ListLines[i].Add(lineInfo);
                    }

                    SizeItemInfo st = new SizeItemInfo();
                    st.Index = 0;
                    st.Name = ((CameraEnum)i).ToString() + $"角点0";
                    st.Description = ((CameraEnum)i).ToString() + $"角点0";
                    st.camera = (CameraEnum)comb_SelectCam.SelectedIndex;
                    st.camera1 = ((CameraEnum)comb_SelectCam.SelectedIndex).ToString();
                    st.camera2 = ((CameraEnum)comb_SelectCam.SelectedIndex).ToString();
                    st.EditorType = EditorTypeEnum.线线交点.ToString();
                    st.Line1 = craft.ListLines[i][0].StrLineName;
                    st.Line2 = craft.ListLines[i][1].StrLineName;
                    craft.ListSizeEdits[i].Add(st);

                    if (i == 0)
                    {
                        //抓边名
                        List<string> strnames1 = new List<string> { "极耳左边", "极耳右边", "极耳顶边" };
                        for (int j = 0; j < strnames1.Count; j++)
                        {
                            LineInfo lineInfo = new LineInfo();
                            lineInfo.StrLineName = strnames1[j];
                            lineInfo.m_camera = (CameraEnum)i;
                            lineInfo.IsStandLine = false;
                            craft.ListLines[i].Add(lineInfo);
                        }

                        // 对齐度点位名        //尺寸计算
                        List<string> Lstrnames = new List<string> { "W1", "W2", "H1", "H2", "JRW", "JRH", "JRD" };
                        List<string> LstrDesp = new List<string> { "极片全宽", "极片宽2", "极片全高", "极片高2", "极耳宽", "极耳高", "极耳距边" };
                        List<EditorTypeEnum> Lstrtype = new List<EditorTypeEnum> {    EditorTypeEnum.线距离X, EditorTypeEnum.线距离X, EditorTypeEnum.线距离Y,EditorTypeEnum.线距离Y,
       EditorTypeEnum.线距离X, EditorTypeEnum.线距离Y, EditorTypeEnum.线距离X};

                        List<CameraEnum> Line1Cam = new List<CameraEnum> {   CameraEnum.Cam3, CameraEnum.Cam3, CameraEnum.Cam2, CameraEnum.Cam2,
       CameraEnum.Cam1, CameraEnum.Cam1, CameraEnum.Cam1};
                        List<string> Line1Name = new List<string> {   "抓边1X", "抓边1X",    "抓边2Y", "抓边2Y" ,
                             "极耳左边",  "极耳顶边",  "极耳右边"};

                        List<CameraEnum> Line2Cam = new List<CameraEnum> {  CameraEnum.Cam4, CameraEnum.Cam4, CameraEnum.Cam4, CameraEnum.Cam4,
       CameraEnum.Cam1, CameraEnum.Cam1, CameraEnum.Cam1};
                        List<string> Line2Name = new List<string> {    "抓边1X", "抓边3X",   "抓边4Y", "抓边2Y" ,
                             "极耳右边",   "抓边2Y",  "抓边1X"};

                        for (int k = 0; k < Lstrnames.Count; k++)
                        {
                            SizeItemInfo sizeItem = new SizeItemInfo();
                            sizeItem.Index = GetSizeInsertId(i);
                            sizeItem.Name = Lstrnames[k];
                            sizeItem.Description = LstrDesp[k];
                            sizeItem.camera = (CameraEnum)i;
                            sizeItem.camera1 = Line1Cam[k].ToString();
                            sizeItem.camera2 = Line2Cam[k].ToString();
                            sizeItem.Line1 = Line1Name[k];
                            sizeItem.Line2 = Line2Name[k];
                            sizeItem.EditorType = Lstrtype[k].ToString();
                            craft.ListSizeEdits[i].Add(sizeItem);
                        }
                    }

                    if (i == 3)
                    {
                        List<string> strnames1 = new List<string> { "抓边3X", "抓边4Y" };
                        for (int j = 0; j < strnames1.Count; j++)
                        {
                            LineInfo lineInfo = new LineInfo();
                            lineInfo.StrLineName = strnames1[j];
                            lineInfo.m_camera = (CameraEnum)i;
                            lineInfo.IsStandLine = false;
                            craft.ListLines[i].Add(lineInfo);
                        }
                        SizeItemInfo st1 = new SizeItemInfo();
                        st1.Index = 1;
                        st1.Name = ((CameraEnum)i).ToString() + $"角点1";
                        st1.Description = ((CameraEnum)i).ToString() + $"角点1";
                        st1.EditorType = EditorTypeEnum.线线交点.ToString();
                        st1.camera = (CameraEnum)comb_SelectCam.SelectedIndex;
                        st1.camera1 = ((CameraEnum)comb_SelectCam.SelectedIndex).ToString();
                        st1.camera2 = ((CameraEnum)comb_SelectCam.SelectedIndex).ToString();
                        st1.Line1 = craft.ListLines[i][2].StrLineName;
                        st1.Line2 = craft.ListLines[i][3].StrLineName;
                        craft.ListSizeEdits[i].Add(st1);
                    }

                }
            }
        }

        public int GetSizeInsertId(int cameraid)
        {
            bool flag = false;
            int id = 0;
            do
            {
                flag = true;
                foreach (SizeItemInfo pte in craft.ListSizeEdits[cameraid])
                {
                    if (pte.Index == id)
                    {
                        id++;
                        flag = false;
                        break;
                    }
                }

                if (flag == true)
                {
                    break;
                }
            } while (true);
            return id;
        }

        public int GetSizeInsertId()
        {
            bool flag = false;
            int id = 0;
            do
            {
                flag = true;
                foreach (SizeItemInfo pte in craft.ListEASizes)
                {
                    if (pte.Index == id)
                    {
                        id++;
                        flag = false;
                        break;
                    }
                }

                if (flag == true)
                {
                    break;
                }
            } while (true);
            return id;
        }

        private void CB_LastFloorMeasureTransition_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (CB_LastFloorMeasureTransition.SelectedIndex)
            {
                case 0:
                    craft.LastFloorMeasureTransition = "negative";
                    break;
                case 1:
                    craft.LastFloorMeasureTransition = "positive";
                    break;
                case 2:
                    craft.LastFloorMeasureTransition = "all";
                    break;
                default:
                    break;
            }
        }



        private void Btn_AddSizeItem1_Click(object sender, EventArgs e)
        {
            SizeItemInfo st = new SizeItemInfo();
            st.Index = GetSizeInsertId();
            st.Name = Tb_Name.Text;
            st.Description = Tb_Description.Text;
            st.camera = (CameraEnum)comb_BelongCamera.SelectedIndex;
            st.camera1 = ((CameraEnum)comb_BelongCamera.SelectedIndex).ToString();
            st.camera2 = ((CameraEnum)comb_BelongCamera.SelectedIndex).ToString();

            craft.ListEASizes.Add(st);
            AddGridData<SizeItemInfo>(DataGridView_EASize1, st, true);
        }

        private void Btn_DeleteSizeItem1_Click(object sender, EventArgs e)
        {
            if (DataGridView_Editor.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选择一条要删除的项！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                try
                {
                    //根据标签ID删除信息
                    // Dclass.ExecuteSql("delete from SMS_InGenerate where ID='" + this.dataGridView1.SelectedRows[0].Cells[0].Value.ToString().Trim() + "'");
                    int Index = (int)DataGridView_EASize1.SelectedRows[0].Index;
                    craft.ListEASizes.RemoveAt(Index);
                    DataGridView_EASize1.Rows.RemoveAt(Index);

                    //DataRowView drv = DataGridView_Editor.SelectedRows[0].DataBoundItem as DataRowView;
                    //drv.Row.Table.Rows.Remove(drv.Row); // 将要删除的行移除，更新时不影响数据库
                    DataGridView_EASize1.Refresh();//更新
                    MessageBox.Show("删除成功！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void btn_RebuildSizeItem1_Click(object sender, EventArgs e)
        {
            string str = $"是否重建检测项工艺?\r\n " + $"工艺生产型号:{craft.materialCrafEnum}\r\n" + $"工艺配方名:{craft.CraftName}\r\n" + "重建检测项会清空所有检测项，恢复初始化值";

            if (MessageBox.Show(str, "重建检测项", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //初始化检测项
                if (craft.ListEASizes == null)
                {
                    craft.ListEASizes = new List<SizeItemInfo>();
                }
                else { craft.ListEASizes.Clear(); }

                CraftToView();
                GC.Collect();
            }

        }
    }
}
