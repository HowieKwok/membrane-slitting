using Frame_Core;
using Mef_VisionFrame._5工艺标准;
using NLog.Time;


//using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public partial class FormStackEACraft : FormCraftBase
    {
        public StackEACraft craft;
        FormSizeFix formSizeFix = new FormSizeFix();
        public MaterialCrafEnum material;

        public FormStackEACraft(MaterialCrafEnum Tempmaterial)
        {
            InitializeComponent();
            toolClass.AllNumericUpDownControlSubscribeMouseWheelEvent(this);
            craft = new StackEACraft();
            craft.Init();
            material = Tempmaterial;

            List<string> camName = new List<string>();
            List<string> camNameS = new List<string>();

            foreach (var item in Project.Instance.cameraManager)
            {
                camName.Add(item.Value.m_CamParam.CameraName);
                camNameS.Add(item.Key.ToString());
            }
            comb_materialCraf.DataSource = Enum.GetNames(typeof(MaterialCrafEnum));
            //相机名称
            comb_SelectCam.DataSource = camName;
            // comb_SelectCam.DataSource = Enum.GetNames(typeof(CameraEnum));

            comb_materialCraf.DataSource = Enum.GetNames(typeof(MaterialCrafEnum));
            Comb_EditorType.DataSource = Enum.GetNames(typeof(EditorTypeEnum));
            comb_Electrode.DataSource = Enum.GetNames(typeof(ElectrodeEnum));
            Comb_CamS.DataSource = camNameS;


            InitListView();
            InitDataGridView(DataGridView_Editor, typeof(SizeItemInfo));
            InitDataGridView(DataGridView_Defect, typeof(DefectItemInfo));

            DataGridView_Editor.Controls.Add(Comb_EditorType);
            DataGridView_Editor.Controls.Add(Comb_Line);
            DataGridView_Editor.Controls.Add(Comb_CamS);

        }


        string str_LineName = "";  //当前配方名
        string str_OldLineName = "";//旧的配方名
        Rectangle Rectangle;

        public int SelectCamIndex = 0;

        public bool IsLinelistChanged = false;


        public override void CraftToView()
        {
            //基础参数
            nud_EASizeCW.Value = (decimal)craft.EASizeCW;
            nud_EASizeCW2.Value = (decimal)craft.EASizeCW2;
            nud_EASizeCH.Value = (decimal)craft.EASizeCH;
            nud_EASizeCH2.Value = (decimal)craft.EASizeCH2;

            nud_JRH.Value = (decimal)craft.JRH;
            nud_JRW.Value = (decimal)craft.JRW;
            nud_JRTagH.Value = (decimal)craft.JRTagH;

            nud_JRL.Value = (decimal)craft.JRL;
            nud_JRR.Value = (decimal)craft.JRR;


            //来料工艺 -阳极
            nud_GrovED.Value = (decimal)craft.GrovED;

            //区域缩进
            nud_colsingRadius.Value = (decimal)craft.colsingRadius;
            nud_openingRadius.Value = (decimal)craft.openingRadius;

            //线扫缺陷检测，区域缩进区域缩进
            nud_LeftShaft1.Value = (decimal)craft.LeftShaft1;
            nud_UpShaft1.Value = (decimal)craft.UpShaft1;
            nud_RightShaft1.Value = (decimal)craft.RightShaft1;
            nud_DownShaft1.Value = (decimal)craft.DownShaft1;

            nud_LeftShaft2.Value = (decimal)craft.LeftShaft2;
            nud_UpShaft2.Value = (decimal)craft.UpShaft2;
            nud_RightShaft2.Value = (decimal)craft.RightShaft2;
            nud_DownShaft2.Value = (decimal)craft.DownShaft2;

            //纠偏设置
            CB_UnwindFlag.Checked = (bool)craft.UnwindFlag;
            CB_UnwindFlagR.Checked = (bool)craft.UnwindFlagR;

            nud_UnwindBaseLineY.Value = (decimal)craft.UnwindBaseLineY;
            nud_UnwindOffsetY.Value = (decimal)craft.UnwindOffsetY;

            CB_DieCutFlagY.Checked = (bool)craft.DieCutFlagY;
            CB_DieCutFlagY_R.Checked= (bool)craft.DieCutFlagY_R;
            nud_DieCutBaseLineY.Value = (decimal)craft.DieCutBaseLineY;
            nud_DieCutOffsetY.Value = (decimal)craft.DieCutOffsetY;

            CB_DieCutFlagX.Checked = (bool)craft.DieCutFlagX;
            CB_DieCutFlagX_R.Checked = (bool)craft.DieCutFlagX_R;

            nud_DieCutBaseLineX.Value = (decimal)craft.DieCutBaseLineX;
            nud_DieCutOffsetX.Value = (decimal)craft.DieCutOffsetX;

            cbx_EnTabneckClosedLoop.Checked = (bool)craft.EnTabneckClosedLoop;

            //显示基础参数
            nud_CamNum.Value = (int)craft.CamNum;
            linkLabel_CraftName.Text = craft.CraftName;
            //复选框
            comb_materialCraf.SelectedIndex = this.comb_materialCraf.FindString(material.ToString());
            comb_Electrode.SelectedIndex = this.comb_Electrode.FindString(craft.electrode.ToString()); ;


            //添加抓边队列
            LoadLinetList();

            //添加尺寸检测项表格
            DataGridView_Editor.Rows.Clear();
            int SizeCount = craft.ListSizeEdits[SelectCamIndex].Count();
            for (int i = 0; i < SizeCount; i++)
            {
                // AddSizeData(DataGridView_Editor, craft.ListSizeEdits[SelectCamIndex][i], true);
                AddGridData<SizeItemInfo>(DataGridView_Editor, craft.ListSizeEdits[SelectCamIndex][i], true);
            }
            //添加缺陷检测项表格
            DataGridView_Defect.Rows.Clear();
            SizeCount = craft.ListDefectInfos[SelectCamIndex].Count();
            for (int i = 0; i < SizeCount; i++)
            {
                // AddSizeData(DataGridView_Editor, craft.ListSizeEdits[SelectCamIndex][i], true);
                AddGridData<DefectItemInfo>(DataGridView_Defect, craft.ListDefectInfos[SelectCamIndex][i], true);
            }
        }

        public override CraftBase CraftFromView()
        {

            //标准极耳工艺
            craft.EASizeCW = (double)nud_EASizeCW.Value;
            craft.EASizeCW2 = (double)nud_EASizeCW2.Value;
            craft.EASizeCH = (double)nud_EASizeCH.Value;
            craft.EASizeCH2 = (double)nud_EASizeCH2.Value;

            craft.JRH = (double)nud_JRH.Value;
            craft.JRW = (double)nud_JRW.Value;
            craft.JRTagH = (double)nud_JRTagH.Value;

            craft.JRL = (double)nud_JRL.Value;
            craft.JRR = (double)nud_JRR.Value;

            //来料工艺 -阳极槽位
            craft.GrovED = (double)nud_GrovED.Value;

            //区域缩进
            craft.colsingRadius = (double)nud_colsingRadius.Value;
            craft.openingRadius = (double)nud_openingRadius.Value;

            //线扫缺陷检测，区域缩进区域缩进
            craft.LeftShaft1 = (double)nud_LeftShaft1.Value;
            craft.UpShaft1 = (double)nud_UpShaft1.Value;
            craft.RightShaft1 = (double)nud_RightShaft1.Value;
            craft.DownShaft1 = (double)nud_DownShaft1.Value;

            craft.LeftShaft2 = (double)nud_LeftShaft2.Value;
            craft.UpShaft2 = (double)nud_UpShaft2.Value;
            craft.RightShaft2 = (double)nud_RightShaft2.Value;
            craft.DownShaft2 = (double)nud_DownShaft2.Value;


            //纠偏设置
            craft.UnwindFlag = CB_UnwindFlag.Checked;
            craft.UnwindFlagR = CB_UnwindFlagR.Checked;
            craft.UnwindBaseLineY = (double)nud_UnwindBaseLineY.Value;
            craft.UnwindOffsetY = (double)nud_UnwindOffsetY.Value;


            craft.DieCutFlagY = CB_DieCutFlagY.Checked;
            craft.DieCutFlagY_R = CB_DieCutFlagY_R.Checked;
            craft.DieCutBaseLineY = (double)nud_DieCutBaseLineY.Value;
            craft.DieCutOffsetY = (double)nud_DieCutOffsetY.Value;

            craft.DieCutFlagX = CB_DieCutFlagX.Checked;
            craft.DieCutFlagX_R = CB_DieCutFlagX_R.Checked;
            craft.DieCutBaseLineX = (double)nud_DieCutBaseLineX.Value;
            craft.DieCutOffsetX = (double)nud_DieCutOffsetX.Value;

            craft.EnTabneckClosedLoop = cbx_EnTabneckClosedLoop.Checked;



            //显示基础参数
            // craft.materialCrafEnum = MaterialCrafEnum.叠台对齐度;
            craft.CamNum = (int)nud_CamNum.Value;
            craft.materialCrafEnum = material;
            craft.electrode = (ElectrodeEnum)Enum.Parse(typeof(ElectrodeEnum), comb_Electrode.SelectedItem.ToString(), false);
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
            //显示缺陷检测项
            count0 = DataGridView_Defect.Rows.Count;
            for (int i = 0; i < count0; i++)
            {
                DefectItemInfo item = craft.ListDefectInfos[SelectCamIndex][i];
                PropertyInfo[] infos = typeof(DefectItemInfo).GetProperties();
                foreach (PropertyInfo pi in infos)
                {
                    var fieldIfno = typeof(DefectItemInfo).GetProperty(pi.Name).GetCustomAttribute<MyAttribute>();
                    if (fieldIfno.Is_ShowCol)
                    {
                        pi.SetValue(item, Convert.ChangeType(DataGridView_Defect[pi.Name, i].Value, pi.PropertyType), null);
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
                craft = craft.Read<StackEACraft>();
            }
            catch (Exception ee)
            {
                craft = new StackEACraft();
                craft.Init();
            }
            craft.filePath = Path;
            return craft;
        }

        public override void ReSetCraft()
        {
            craft = new StackEACraft();
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
            ListViewGroup group1 = new ListViewGroup("基准边");
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
                    lineInfo.IsStandLine = true; // true为基准边
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

        #region 检测项操作事件

        private void Btn_AddItem_Click(object sender, EventArgs e)
        {
            SizeItemInfo pt = new SizeItemInfo();

            //获取新不重复的id  如果已经存在  1,2,4   那么久获得的id 是 3
            bool flag = false;
            int id = 0;
            do
            {
                flag = true;

                foreach (SizeItemInfo pte in craft.ListSizeEdits[SelectCamIndex])
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
            pt.Index = id;
            pt.Name = Tb_Name.Text;
            pt.Description = Tb_Description.Text;
            pt.camera = (CameraEnum)comb_SelectCam.SelectedIndex;
            pt.camera1 = ((CameraEnum)comb_SelectCam.SelectedIndex).ToString();
            pt.camera2 = ((CameraEnum)comb_SelectCam.SelectedIndex).ToString();
            craft.ListSizeEdits[SelectCamIndex].Add(pt);
            //  AddSizeData(DataGridView_Editor, pt, true);
            AddGridData<SizeItemInfo>(DataGridView_Editor, pt, true);

        }

        private void Btn_DeleteItem_Click(object sender, EventArgs e)
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

        private void btn_RebuildItem_Click(object sender, EventArgs e)
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

        #region 缺陷项操作事件

        private void Btn_AddDefect_Click(object sender, EventArgs e)
        {
            DefectItemInfo pt = new DefectItemInfo();

            //获取新不重复的id  如果已经存在  1,2,4   那么久获得的id 是 3
            bool flag = false;
            int id = 0;
            do
            {
                flag = true;

                foreach (DefectItemInfo pte in craft.ListDefectInfos[SelectCamIndex])
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
            pt.Index = id;
            pt.Name = Tb_Name.Text;
            pt.Description = Tb_Description.Text;
            pt.camera = (CameraEnum)comb_SelectCam.SelectedIndex;

            craft.ListDefectInfos[SelectCamIndex].Add(pt);
            //  AddSizeData(DataGridView_Editor, pt, true);
            AddGridData<DefectItemInfo>(DataGridView_Defect, pt, true);
        }

        private void Btn_DeleteDefect_Click(object sender, EventArgs e)
        {
            if (DataGridView_Defect.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选择一条要删除的项！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                try
                {
                    //根据标签ID删除信息
                    // Dclass.ExecuteSql("delete from SMS_InGenerate where ID='" + this.dataGridView1.SelectedRows[0].Cells[0].Value.ToString().Trim() + "'");
                    int Index = (int)DataGridView_Defect.SelectedRows[0].Index;
                    craft.ListDefectInfos[SelectCamIndex].RemoveAt(Index);
                    DataGridView_Defect.Rows.RemoveAt(Index);

                    //DataRowView drv = DataGridView_Editor.SelectedRows[0].DataBoundItem as DataRowView;
                    //drv.Row.Table.Rows.Remove(drv.Row); // 将要删除的行移除，更新时不影响数据库
                    DataGridView_Defect.Refresh();//更新
                    MessageBox.Show("删除成功！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void btn_RebuildDefect_Click(object sender, EventArgs e)
        {
            string str = $"是否重建检测项工艺?\r\n " + $"工艺生产型号:{craft.materialCrafEnum}\r\n" + $"工艺配方名:{craft.CraftName}\r\n" + "重建检测项会清空所有检测项，恢复初始化值";

            if (MessageBox.Show(str, "重建检测项", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                //初始化检测项
                if (craft.ListDefectInfos == null)
                {
                    craft.ListDefectInfos = new List<List<DefectItemInfo>>();
                }
                else { craft.ListDefectInfos.Clear(); }

                for (int i = 0; i < craft.CamNum; i++)
                {
                    craft.ListDefectInfos.Add(new List<DefectItemInfo>());
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

        #region datagridview下拉选择抓边，下拉选择计算方式
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
            //如果是要显示下拉列表的列的话
            if (column.Name.Equals("Line1") || column.Name.Equals("Line2"))
            {
                int columnIndex = DataGridView_Editor.CurrentCell.ColumnIndex;
                int rowIndex = DataGridView_Editor.CurrentCell.RowIndex;

                Comb_Line.Items.Clear();
                string cam = DataGridView_Editor.Rows[rowIndex].Cells[columnIndex - 1].Value.ToString();
                int intcam = Comb_CamS.Items.IndexOf(cam);
                foreach (var item in craft.ListLines[intcam])
                {
                    Comb_Line.Items.Add(item.StrLineName);
                }

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
        private void Comb_EditorType_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.Graphics.DrawString(Comb_EditorType.Items[e.Index].ToString(), e.Font, Brushes.Black,
                e.Bounds, StringFormat.GenericDefault);
        }
        private void Comb_EditorType_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (DataGridView_Editor.CurrentCell != null)
                    DataGridView_Editor.CurrentCell.Value = Comb_EditorType.Items[Comb_EditorType.SelectedIndex];
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

        #region datagridview限制输入只能是数字

        private void DataGridView_Editor_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
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

        /// <summary>
        /// 选择相机
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        private void comb_Electrode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comb_Electrode.SelectedIndex == 0)
            {
                pictureBox2.Image = Properties.Resources.叠片阴极铝箔;
            }
            else
            {
                pictureBox2.Image = Properties.Resources.叠片阳极铜箔;
            }
        }

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

        /// <summary>
        /// 添加尺寸DataGridView数据
        /// </summary>
        /// <param name="DGView"></param>
        /// <param name="item"></param>
        public void AddSizeData(DataGridView DGView, SizeItemInfo item, bool showAll)
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
                        var fieldIfno = typeof(SizeItemInfo).GetProperty(pi.Name).GetCustomAttribute<MyAttribute>();
                        if (fieldIfno.Is_ShowCol)
                        {
                            DGView[pi.Name, count].Value = pi.GetValue(item, null);
                        }
                    }
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                if (comb_Electrode.SelectedIndex == 0)
                {
                    pictureBox2.Image = Properties.Resources.叠片阴极铝箔;
                }
                else
                {
                    pictureBox2.Image = Properties.Resources.叠片阳极铜箔;
                }
            }
            else
            {
                pictureBox2.Image = Properties.Resources.叠片尺寸示意;
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
                //初始化缺陷项
                if (craft.ListDefectInfos == null)
                {
                    craft.ListDefectInfos = new List<List<DefectItemInfo>>();
                }
                else { craft.ListDefectInfos.Clear(); }

                for (int i = 0; i < craft.CamNum; i++)
                {
                    craft.ListDefectInfos.Add(new List<DefectItemInfo>());
                }

                ReDefault();

                LoadLinetList();
                CraftToView();
                GC.Collect();
            }
        }


        public void ReDefault()
        {
            if (craft.materialCrafEnum == MaterialCrafEnum.叠片阳极单面制片CS)
            {
                craft.CamNum = 5;
                craft.electrode = ElectrodeEnum.阳极;
                for (int i = 0; i < craft.CamNum; i++)
                {
                    if (i == 0)  //线扫相机。cam0涂布面 ， cam1光箔面
                    {
                        //缺陷检测项
                        ReBuildCraftDefect(craft, i);


                        LineInfo lineInfo = new LineInfo();
                        lineInfo.StrLineName = $"涂布左基准";
                        lineInfo.m_camera = (CameraEnum)i;
                        lineInfo.IsStandLine = false;
                        craft.ListLines[i].Add(lineInfo);

                        LineInfo lineInfo1 = new LineInfo();
                        lineInfo1.StrLineName = $"涂布右基准";
                        lineInfo1.m_camera = (CameraEnum)i;
                        lineInfo1.IsStandLine = false;
                        craft.ListLines[i].Add(lineInfo1);

                        //LineInfo lineInfo2 = new LineInfo();
                        //lineInfo2.StrLineName = "槽位一上边";
                        //lineInfo2.m_camera = (CameraEnum)i;
                        //lineInfo2.IsStandLine = false;
                        //craft.ListLines[i].Add(lineInfo2);

                        //LineInfo lineInfo3 = new LineInfo();
                        //lineInfo3.StrLineName = "槽位一下边";
                        //lineInfo3.m_camera = (CameraEnum)i;
                        //lineInfo3.IsStandLine = false;
                        //craft.ListLines[i].Add(lineInfo3);

                        //LineInfo lineInfo4 = new LineInfo();
                        //lineInfo4.StrLineName = "槽位一左边";
                        //lineInfo4.m_camera = (CameraEnum)i;
                        //lineInfo4.IsStandLine = false;
                        //craft.ListLines[i].Add(lineInfo4);


                        //LineInfo lineInfo5 = new LineInfo();
                        //lineInfo5.StrLineName = ((CameraEnum)i).ToString() + $"槽位右边";
                        //lineInfo5.m_camera = (CameraEnum)i;
                        //lineInfo5.IsStandLine = false;
                        //craft.ListLines[i].Add(lineInfo5);
                    }
                    else if (i == 1) //线扫相机。cam0涂布面 ， cam1光箔面
                    {
                        //缺陷检测项
                        ReBuildCraftDefect(craft, i);


                        LineInfo lineInfo = new LineInfo();
                        lineInfo.StrLineName = "涂布左基准";
                        lineInfo.m_camera = (CameraEnum)i;
                        lineInfo.IsStandLine = false;
                        craft.ListLines[i].Add(lineInfo);

                        LineInfo lineInfo1 = new LineInfo();
                        lineInfo1.StrLineName = "涂布右基准";
                        lineInfo1.m_camera = (CameraEnum)i;
                        lineInfo1.IsStandLine = false;
                        craft.ListLines[i].Add(lineInfo1);

                    }
                    else if (i == 2) //放卷纠偏 -涂布边
                    {
                        LineInfo lineInfo = new LineInfo();
                        lineInfo.StrLineName = ((CameraEnum)i).ToString() + $"涂布抓边";
                        lineInfo.m_camera = (CameraEnum)i;
                        lineInfo.IsStandLine = false;
                        craft.ListLines[i].Add(lineInfo);

                        LineInfo lineInfoY = new LineInfo();
                        lineInfoY.StrLineName = ((CameraEnum)i).ToString() + $"基准边Y";
                        lineInfoY.m_camera = (CameraEnum)i;
                        lineInfoY.IsStandLine = true;
                        craft.ListLines[i].Add(lineInfoY);


                        SizeItemInfo sizeItemY = new SizeItemInfo();
                        sizeItemY.Name = "基准距离Y";
                        sizeItemY.Description = "基准距离Y";
                        sizeItemY.Index = GetSizeInsertId(i);
                        sizeItemY.camera = (CameraEnum)i;
                        sizeItemY.camera1 = ((CameraEnum)i).ToString();
                        sizeItemY.camera2 = ((CameraEnum)i).ToString();
                        sizeItemY.Line1 = craft.ListLines[i][0].StrLineName;
                        sizeItemY.Line2 = craft.ListLines[i][1].StrLineName;
                        sizeItemY.EditorType = EditorTypeEnum.线距离Y.ToString();
                        craft.ListSizeEdits[i].Add(sizeItemY);

                    }
                    else if (i == 3)//模切纠偏 -台阶边和切边
                    {
                        LineInfo lineInfo = new LineInfo();
                        lineInfo.StrLineName = ((CameraEnum)i).ToString() + $"抓边Y";
                        lineInfo.m_camera = (CameraEnum)i;
                        lineInfo.IsStandLine = false;
                        craft.ListLines[i].Add(lineInfo);

                        LineInfo lineInfo1 = new LineInfo();
                        lineInfo1.StrLineName = ((CameraEnum)i).ToString() + $"抓边X";
                        lineInfo1.m_camera = (CameraEnum)i;
                        lineInfo1.IsStandLine = false;
                        craft.ListLines[i].Add(lineInfo1);


                        LineInfo lineInfoY = new LineInfo();
                        lineInfoY.StrLineName = ((CameraEnum)i).ToString() + $"基准边Y";
                        lineInfoY.m_camera = (CameraEnum)i;
                        lineInfoY.IsStandLine = true;
                        craft.ListLines[i].Add(lineInfoY);

                        LineInfo lineInfoX = new LineInfo();
                        lineInfoX.StrLineName = ((CameraEnum)i).ToString() + $"基准边X";
                        lineInfoX.m_camera = (CameraEnum)i;
                        lineInfoX.IsStandLine = true;
                        craft.ListLines[i].Add(lineInfoX);

                        SizeItemInfo sizeItemY = new SizeItemInfo();
                        sizeItemY.Index = GetSizeInsertId(i);
                        sizeItemY.Name = "基准距离Y";
                        sizeItemY.Description = "基准距离Y";
                        sizeItemY.camera = (CameraEnum)i;
                        sizeItemY.camera1 = ((CameraEnum)i).ToString();
                        sizeItemY.camera2 = ((CameraEnum)i).ToString();
                        sizeItemY.Line1 = craft.ListLines[i][0].StrLineName;
                        sizeItemY.Line2 = craft.ListLines[i][2].StrLineName;
                        sizeItemY.EditorType = EditorTypeEnum.线距离Y.ToString();
                        craft.ListSizeEdits[i].Add(sizeItemY);

                        SizeItemInfo sizeItemX = new SizeItemInfo();
                        sizeItemX.Index = GetSizeInsertId(i);
                        sizeItemX.Name = "基准距离X";
                        sizeItemX.Description = "基准距离X";
                        sizeItemX.camera = (CameraEnum)i;
                        sizeItemX.camera1 = ((CameraEnum)i).ToString();
                        sizeItemX.camera2 = ((CameraEnum)i).ToString();
                        sizeItemX.Line1 = craft.ListLines[i][1].StrLineName;
                        sizeItemX.Line2 = craft.ListLines[i][3].StrLineName;
                        sizeItemX.EditorType = EditorTypeEnum.线距离X.ToString();
                        craft.ListSizeEdits[i].Add(sizeItemX);
                    }
                    else
                    {
                        //缺陷检测项
                        ReBuildCraftDefect(craft, i);

                        // "极耳台阶边","Lstep",
                        //抓边名
                        List<string> strnames = new List<string> { "LTabNeck1", "LTabNeck2" ,"极片边X", "极片边X1", "极片边X2", "极片边Y", "极片边Y1", "极片边Y2",
                                "极耳左边", "极耳右边", "极耳顶边"};
                        for (int j = 0; j < strnames.Count; j++)
                        {
                            LineInfo lineInfo = new LineInfo();
                            lineInfo.StrLineName = strnames[j];
                            lineInfo.m_camera = (CameraEnum)i;
                            lineInfo.IsStandLine = false;
                            craft.ListLines[i].Add(lineInfo);
                        }
                        // 对齐度点位名        //尺寸计算
                        List<string> Lstrnames = new List<string> {"TabNeckX",  "极片X1", "极片X2", "极片Y1","极片Y2",
                                "极耳X","极耳Y","极耳边距Y" };

                        List<string> LstrDesp = new List<string> { "TabNeck", "极片宽度1", "极片宽度2", "极片高度1", "极片高度2",
                              "极耳宽",  "极耳高", "极耳边距" };

                        List<EditorTypeEnum> Lstrtype = new List<EditorTypeEnum> {   EditorTypeEnum.线距离X,   EditorTypeEnum.线距离X,   EditorTypeEnum.线距离X,   EditorTypeEnum.线距离Y, EditorTypeEnum.线距离Y,
                       EditorTypeEnum.线距离X,  EditorTypeEnum.线距离Y, EditorTypeEnum.线距离Y};

                        List<string> Line1Name = new List<string> {
                                   "LTabNeck1",
                                   "极片边X",
                                   "极片边X",
                                   "极片边Y",
                                   "极片边Y",
                                   "极片边X",
                                   "极耳左边",
                                   "极片边Y",
                                   };/* "TabNeckX "    */  //TabNeckX

                        List<string> Line2Name = new List<string> {
                                 "LTabNeck2",
                                 "极片边X1",
                                 "极片边X2",
                                 "极片边Y1",
                                 "极片边Y2",
                                 "极耳顶边",
                                 "极耳右边",
                                 "极耳左边",
                                  };

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
            }
            else if ((craft.materialCrafEnum == MaterialCrafEnum.叠片阳极双面制片CD))
            {
                craft.CamNum = 7;
                craft.electrode = ElectrodeEnum.阳极;
                for (int i = 0; i < craft.CamNum; i++)
                {
                    if (i == 0 || i == 1) //线扫相机。cam0涂布面 ， cam1光箔面
                    {
                        //缺陷检测项
                        ReBuildCraftDefect(craft, i);

                        LineInfo lineInfo = new LineInfo();
                        lineInfo.StrLineName = "涂布左基准";
                        lineInfo.m_camera = (CameraEnum)i;
                        lineInfo.IsStandLine = false;
                        craft.ListLines[i].Add(lineInfo);

                        LineInfo lineInfo1 = new LineInfo();
                        lineInfo1.StrLineName = $"涂布右基准";
                        lineInfo1.m_camera = (CameraEnum)i;
                        lineInfo1.IsStandLine = false;
                        craft.ListLines[i].Add(lineInfo1);

                        //LineInfo lineInfo2 = new LineInfo();
                        //lineInfo2.StrLineName = "槽位一上边";
                        //lineInfo2.m_camera = (CameraEnum)i;
                        //lineInfo2.IsStandLine = false;
                        //craft.ListLines[i].Add(lineInfo2);

                        //LineInfo lineInfo3 = new LineInfo();
                        //lineInfo3.StrLineName = "槽位一下边";
                        //lineInfo3.m_camera = (CameraEnum)i;
                        //lineInfo3.IsStandLine = false;
                        //craft.ListLines[i].Add(lineInfo3);

                        //LineInfo lineInfo4 = new LineInfo();
                        //lineInfo4.StrLineName = "槽位一左边";
                        //lineInfo4.m_camera = (CameraEnum)i;
                        //lineInfo4.IsStandLine = false;
                        //craft.ListLines[i].Add(lineInfo4);

                        //LineInfo lineInfo5 = new LineInfo();
                        //lineInfo5.StrLineName = "槽位二上边";
                        //lineInfo5.m_camera = (CameraEnum)i;
                        //lineInfo5.IsStandLine = false;
                        //craft.ListLines[i].Add(lineInfo5);

                        //LineInfo lineInfo6 = new LineInfo();
                        //lineInfo6.StrLineName = "槽位二下边";
                        //lineInfo6.m_camera = (CameraEnum)i;
                        //lineInfo6.IsStandLine = false;
                        //craft.ListLines[i].Add(lineInfo6);

                        //LineInfo lineInfo7 = new LineInfo();
                        //lineInfo7.StrLineName = "槽位二左边";
                        //lineInfo7.m_camera = (CameraEnum)i;
                        //lineInfo7.IsStandLine = false;
                        //craft.ListLines[i].Add(lineInfo7);
                    }
                    else if (i == 2) //放卷纠偏 -涂布边
                    {
                        LineInfo lineInfo = new LineInfo();
                        lineInfo.StrLineName = ((CameraEnum)i).ToString() + $"涂布抓边";
                        lineInfo.m_camera = (CameraEnum)i;
                        lineInfo.IsStandLine = false;
                        craft.ListLines[i].Add(lineInfo);

                        LineInfo lineInfoY = new LineInfo();
                        lineInfoY.StrLineName = ((CameraEnum)i).ToString() + $"基准边Y";
                        lineInfoY.m_camera = (CameraEnum)i;
                        lineInfoY.IsStandLine = true;
                        craft.ListLines[i].Add(lineInfoY);


                        SizeItemInfo sizeItemY = new SizeItemInfo();
                        sizeItemY.Index = GetSizeInsertId(i);
                        sizeItemY.Name = "基准距离Y";
                        sizeItemY.Description = "基准距离Y";
                        sizeItemY.camera = (CameraEnum)i;
                        sizeItemY.camera1 = ((CameraEnum)i).ToString();
                        sizeItemY.camera2 = ((CameraEnum)i).ToString();

                        sizeItemY.Line1 = craft.ListLines[i][0].StrLineName;
                        sizeItemY.Line2 = craft.ListLines[i][1].StrLineName;
                        sizeItemY.EditorType = EditorTypeEnum.线距离Y.ToString();
                        craft.ListSizeEdits[i].Add(sizeItemY);

                    }
                    else if (i == 3) /*|| i == 4*///模切纠偏 -台阶边和切边
                    {
                        LineInfo lineInfo = new LineInfo();
                        lineInfo.StrLineName = ((CameraEnum)i).ToString() + $"抓边Y";
                        lineInfo.m_camera = (CameraEnum)i;
                        lineInfo.IsStandLine = false;
                        craft.ListLines[i].Add(lineInfo);

                        LineInfo lineInfo1 = new LineInfo();
                        lineInfo1.StrLineName = ((CameraEnum)i).ToString() + $"抓边X";
                        lineInfo1.m_camera = (CameraEnum)i;
                        lineInfo1.IsStandLine = false;
                        craft.ListLines[i].Add(lineInfo1);


                        LineInfo lineInfoY = new LineInfo();
                        lineInfoY.StrLineName = ((CameraEnum)i).ToString() + $"基准边Y";
                        lineInfoY.m_camera = (CameraEnum)i;
                        lineInfoY.IsStandLine = true;
                        craft.ListLines[i].Add(lineInfoY);

                        LineInfo lineInfoX = new LineInfo();
                        lineInfoX.StrLineName = ((CameraEnum)i).ToString() + $"基准边X";
                        lineInfoX.m_camera = (CameraEnum)i;
                        lineInfoX.IsStandLine = true;
                        craft.ListLines[i].Add(lineInfoX);

                        SizeItemInfo sizeItemY = new SizeItemInfo();
                        sizeItemY.Index = GetSizeInsertId(i);
                        sizeItemY.Name = "基准距离Y";
                        sizeItemY.Description = "基准距离Y";
                        sizeItemY.camera = (CameraEnum)i;
                        sizeItemY.camera1 = ((CameraEnum)i).ToString();
                        sizeItemY.camera2 = ((CameraEnum)i).ToString();

                        sizeItemY.Line1 = craft.ListLines[i][0].StrLineName;
                        sizeItemY.Line2 = craft.ListLines[i][2].StrLineName;
                        sizeItemY.EditorType = EditorTypeEnum.线距离Y.ToString();
                        craft.ListSizeEdits[i].Add(sizeItemY);

                        SizeItemInfo sizeItemX = new SizeItemInfo();
                        sizeItemX.Index = GetSizeInsertId(i);
                        sizeItemX.Name = "基准距离X";
                        sizeItemX.Description = "基准距离X";
                        sizeItemX.camera = (CameraEnum)i;
                        sizeItemX.camera1 = ((CameraEnum)i).ToString();
                        sizeItemX.camera2 = ((CameraEnum)i).ToString();
                        sizeItemX.Line1 = craft.ListLines[i][1].StrLineName;
                        sizeItemX.Line2 = craft.ListLines[i][3].StrLineName;
                        sizeItemX.EditorType = EditorTypeEnum.线距离X.ToString();
                        craft.ListSizeEdits[i].Add(sizeItemX);
                    }
                    //上tabneck相机
                    else if (i == 4)
                    {
                        //缺陷检测项
                        ReBuildCraftDefect(craft, i);
                        // "极耳台阶边","Lstep",
                        //抓边名
                        List<string> strnames = new List<string> { "LTabNeck1", "LTabNeck2" , "极片边X", "极片边X1", "极片边X2", "极片边Y", "极片边Y1", "极片边Y2",
                                "极耳左边", "极耳右边", "极耳顶边"};
                        for (int j = 0; j < strnames.Count; j++)
                        {
                            LineInfo lineInfo = new LineInfo();
                            lineInfo.StrLineName = strnames[j];
                            lineInfo.m_camera = (CameraEnum)i;
                            lineInfo.IsStandLine = false;
                            craft.ListLines[i].Add(lineInfo);
                        }
                        // 对齐度点位名        //尺寸计算
                        List<string> Lstrnames = new List<string> {"TabNeckX", "极片X1", "极片X2", "极片Y1","极片Y2",
                                "极耳X","极耳Y","极耳边距Y" };

                        List<string> LstrDesp = new List<string> { "TabNeck" , "极片宽度1", "极片宽度2", "极片高度1", "极片高度2",
                              "极耳宽",  "极耳高", "极耳边距"};

                        List<EditorTypeEnum> Lstrtype = new List<EditorTypeEnum> {   EditorTypeEnum.线距离X  , EditorTypeEnum.线距离X,   EditorTypeEnum.线距离X,   EditorTypeEnum.线距离Y, EditorTypeEnum.线距离Y,
                       EditorTypeEnum.线距离X,  EditorTypeEnum.线距离Y, EditorTypeEnum.线距离Y};

                        List<string> Line1Name = new List<string> {
                                   "LTabNeck1",
                                   "极片边X",
                                   "极片边X",
                                   "极片边Y",
                                   "极片边Y",
                                   "极片边X",
                                   "极耳左边",
                                   "极片边Y",
                                  };/* "TabNeckX "    */  //TabNeckX

                        List<string> Line2Name = new List<string> {
                                "LTabNeck2",
                                 "极片边X1",
                                 "极片边X2",
                                 "极片边Y1",
                                 "极片边Y2",
                                 "极耳顶边",
                                 "极耳右边",
                                 "极耳左边",
                                  };

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
                    //下tabneck相机
                    else
                    {




                        //"极片边X", "极片边X1", "极片边X2", "极片边Y", "极片边Y1", "极片边Y2",
                        //                       "极耳左边", "极耳右边", "极耳顶边", "极耳台阶边","Lstep",
                        //抓边名
                        List<string> strnames = new List<string> { "LTabNeck1", "LTabNeck2" };
                        for (int j = 0; j < strnames.Count; j++)
                        {
                            LineInfo lineInfo = new LineInfo();
                            lineInfo.StrLineName = strnames[j];
                            lineInfo.m_camera = (CameraEnum)i;
                            lineInfo.IsStandLine = false;
                            craft.ListLines[i].Add(lineInfo);
                        }
                        // 对齐度点位名        //尺寸计算
                        //"极片X1", "极片X2", "极片Y1","极片Y2",
                        //                       "极耳X","极耳Y","台阶X",
                        List<string> Lstrnames = new List<string> { "TabNeckX" };
                        //"极片宽度1",  "极片宽度2",  "极片高度1", "极片高度2",
                        //                     "极耳宽",  "极耳高", "台阶高",
                        List<string> LstrDesp = new List<string> { "TabNeck" };
                        //EditorTypeEnum.线距离X,   EditorTypeEnum.线距离X,   EditorTypeEnum.线距离Y, EditorTypeEnum.线距离Y,
                        //              EditorTypeEnum.线距离X,  EditorTypeEnum.线距离Y, EditorTypeEnum.线距离X, 
                        List<EditorTypeEnum> Lstrtype = new List<EditorTypeEnum> { EditorTypeEnum.线距离X };

                        //"极片边X",   /*  "极片边X1",   */  //极片X1
                        //                      "极片边X",   /*  "极片边X2",   */  //极片X2
                        //                      "极片边Y",   /*   "极片边Y1",  */  //极片Y1
                        //                      "极片边Y",   /*   "极片边Y2",  */  //极片Y2
                        //                      "极片边X",   /*   "极耳顶边",  */  //极耳X
                        //                      "极耳左边",  /*   "极耳右边",  */  //极耳Y
                        //                      "极片边X",   /*   "台阶X",     */  //台阶X
                        List<string> Line1Name = new List<string> {
                                              "LTabNeck1", };/* "TabNeckX "    */  //TabNeckX

                        //"极片边X1",
                        //                        "极片边X2",
                        //                        "极片边Y1",
                        //                        "极片边Y2",
                        //                        "极耳顶边",
                        //                        "极耳右边",
                        //                        "Lstep",
                        List<string> Line2Name = new List<string> {
                                                "LTabNeck2"    };

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
            }
            else if ((craft.materialCrafEnum == MaterialCrafEnum.叠片阴极双面制片AD))
            {
                craft.CamNum = 7;
                craft.electrode = ElectrodeEnum.阴极;
                for (int i = 0; i < craft.CamNum; i++)
                {
                    //  极片寻边示意图
                    //        涂布      AT  箔材
                    //  |**************|  |++++|
                    //  左            右  AT  箔材
                    if (i == 0 || i == 1) //阴极双涂，有AT抓边   
                    {
                        //缺陷检测项
                        ReBuildCraftDefect(craft, i);

                        LineInfo lineInfo = new LineInfo();
                        lineInfo.StrLineName = $"涂布左基准";
                        lineInfo.m_camera = (CameraEnum)i;
                        lineInfo.IsStandLine = false;
                        craft.ListLines[i].Add(lineInfo);

                        LineInfo lineInfo1 = new LineInfo();
                        lineInfo1.StrLineName = $"涂布右基准";
                        lineInfo1.m_camera = (CameraEnum)i;
                        lineInfo1.IsStandLine = false;
                        craft.ListLines[i].Add(lineInfo1);

                        LineInfo lineInfo2 = new LineInfo();
                        lineInfo2.StrLineName = $"AT边";
                        lineInfo2.m_camera = (CameraEnum)i;
                        lineInfo2.IsStandLine = false;
                        craft.ListLines[i].Add(lineInfo2);


                        //LineInfo lineInfo3 = new LineInfo();
                        //lineInfo3.StrLineName = ((CameraEnum)i).ToString() + $"箔材边";
                        //lineInfo3.m_camera = (CameraEnum)i;
                        //lineInfo3.IsStandLine = false;
                        //craft.ListLines[i].Add(lineInfo3);
                    }
                    else if (i == 2) //放卷纠偏 -涂布边
                    {
                        LineInfo lineInfo = new LineInfo();
                        lineInfo.StrLineName = ((CameraEnum)i).ToString() + $"涂布抓边";
                        lineInfo.m_camera = (CameraEnum)i;
                        lineInfo.IsStandLine = false;
                        craft.ListLines[i].Add(lineInfo);

                        LineInfo lineInfoY = new LineInfo();
                        lineInfoY.StrLineName = ((CameraEnum)i).ToString() + $"基准边Y";
                        lineInfoY.m_camera = (CameraEnum)i;
                        lineInfoY.IsStandLine = true;
                        craft.ListLines[i].Add(lineInfoY);


                        SizeItemInfo sizeItemY = new SizeItemInfo();
                        sizeItemY.Index = GetSizeInsertId(i);
                        sizeItemY.Name = "基准距离Y";
                        sizeItemY.Description = "基准距离Y";
                        sizeItemY.camera = (CameraEnum)i;
                        sizeItemY.camera1 = ((CameraEnum)i).ToString();
                        sizeItemY.camera2 = ((CameraEnum)i).ToString();
                        sizeItemY.Line1 = craft.ListLines[i][0].StrLineName;
                        sizeItemY.Line2 = craft.ListLines[i][1].StrLineName;
                        sizeItemY.EditorType = EditorTypeEnum.线距离Y.ToString();
                        craft.ListSizeEdits[i].Add(sizeItemY);

                    }
                    else if (i == 3)//模切纠偏 -台阶边和切边
                    {
                        LineInfo lineInfo = new LineInfo();
                        lineInfo.StrLineName = ((CameraEnum)i).ToString() + $"抓边Y";
                        lineInfo.m_camera = (CameraEnum)i;
                        lineInfo.IsStandLine = false;
                        craft.ListLines[i].Add(lineInfo);

                        //LineInfo lineInfo1 = new LineInfo();
                        //lineInfo1.StrLineName = ((CameraEnum)i).ToString() + $"抓边X";
                        //lineInfo1.m_camera = (CameraEnum)i;
                        //lineInfo1.IsStandLine = false;
                        //craft.ListLines[i].Add(lineInfo1);


                        LineInfo lineInfoY = new LineInfo();
                        lineInfoY.StrLineName = ((CameraEnum)i).ToString() + $"基准边Y";
                        lineInfoY.m_camera = (CameraEnum)i;
                        lineInfoY.IsStandLine = true;
                        craft.ListLines[i].Add(lineInfoY);

                        //LineInfo lineInfoX = new LineInfo();
                        //lineInfoX.StrLineName = ((CameraEnum)i).ToString() + $"基准边X";
                        //lineInfoX.m_camera = (CameraEnum)i;
                        //lineInfoX.IsStandLine = true;
                        //craft.ListLines[i].Add(lineInfoX);

                        SizeItemInfo sizeItemY = new SizeItemInfo();
                        sizeItemY.Index = GetSizeInsertId(i);
                        sizeItemY.Name = "基准距离Y";
                        sizeItemY.Description = "基准距离Y";
                        sizeItemY.camera = (CameraEnum)i;
                        sizeItemY.camera1 = ((CameraEnum)i).ToString();
                        sizeItemY.camera2 = ((CameraEnum)i).ToString();
                        sizeItemY.Line1 = craft.ListLines[i][0].StrLineName;
                        sizeItemY.Line2 = craft.ListLines[i][1].StrLineName;
                        sizeItemY.EditorType = EditorTypeEnum.线距离Y.ToString();
                        craft.ListSizeEdits[i].Add(sizeItemY);

                        //SizeItemInfo sizeItemX = new SizeItemInfo();
                        //sizeItemX.Name = "基准距离X";
                        //sizeItemX.Description = "基准距离X";
                        //sizeItemX.camera = (CameraEnum)i;
                        //sizeItemX.Line1 = craft.ListLines[i][1].StrLineName;
                        //sizeItemX.Line2 = craft.ListLines[i][3].StrLineName;
                        //sizeItemX.EditorType = EditorTypeEnum.线距离X.ToString();
                        //craft.ListSizeEdits[i].Add(sizeItemX);
                    }
                    //上tabneck相机
                    else if (i == 4)
                    {
                        //缺陷检测项
                        ReBuildCraftDefect(craft, i);
                        // "极耳台阶边","Lstep",
                        //抓边名
                        List<string> strnames = new List<string> { "LTabNeck1", "LTabNeck2" , "极片边X", "极片边X1", "极片边X2", "极片边Y", "极片边Y1", "极片边Y2",
                                "极耳左边", "极耳右边", "极耳顶边"};
                        for (int j = 0; j < strnames.Count; j++)
                        {
                            LineInfo lineInfo = new LineInfo();
                            lineInfo.StrLineName = strnames[j];
                            lineInfo.m_camera = (CameraEnum)i;
                            lineInfo.IsStandLine = false;
                            craft.ListLines[i].Add(lineInfo);
                        }
                        // 对齐度点位名        //尺寸计算
                        List<string> Lstrnames = new List<string> {"TabNeckX" ,  "极片X1", "极片X2", "极片Y1","极片Y2",
                                "极耳X","极耳Y","极耳边距Y"};

                        List<string> LstrDesp = new List<string> {  "TabNeck" ,"极片宽度1", "极片宽度2", "极片高度1", "极片高度2",
                              "极耳宽",  "极耳高", "极耳边距"};

                        List<EditorTypeEnum> Lstrtype = new List<EditorTypeEnum> {   EditorTypeEnum.线距离X,   EditorTypeEnum.线距离X,   EditorTypeEnum.线距离X,   EditorTypeEnum.线距离Y, EditorTypeEnum.线距离Y,
                       EditorTypeEnum.线距离X,  EditorTypeEnum.线距离Y, EditorTypeEnum.线距离Y};

                        List<string> Line1Name = new List<string> {
                                   "LTabNeck1",
                                   "极片边X",
                                   "极片边X",
                                   "极片边Y",
                                   "极片边Y",
                                   "极片边X",
                                   "极耳左边",
                                   "极片边Y",
                                   };/* "TabNeckX "    */  //TabNeckX

                        List<string> Line2Name = new List<string> {
                                 "LTabNeck2" ,
                                 "极片边X1",
                                 "极片边X2",
                                 "极片边Y1",
                                 "极片边Y2",
                                 "极耳顶边",
                                 "极耳右边",
                                 "极耳左边",
                                    };

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
                    //下tabneck相机
                    else
                    {
                        //"极片边X", "极片边X1", "极片边X2", "极片边Y", "极片边Y1", "极片边Y2",
                        //                       "极耳左边", "极耳右边", "极耳顶边", "极耳台阶边","Lstep",
                        //抓边名
                        List<string> strnames = new List<string> { "LTabNeck1", "LTabNeck2" };
                        for (int j = 0; j < strnames.Count; j++)
                        {
                            LineInfo lineInfo = new LineInfo();
                            lineInfo.StrLineName = strnames[j];
                            lineInfo.m_camera = (CameraEnum)i;
                            lineInfo.IsStandLine = false;
                            craft.ListLines[i].Add(lineInfo);
                        }
                        // 对齐度点位名        //尺寸计算
                        //"极片X1", "极片X2", "极片Y1","极片Y2",
                        //                       "极耳X","极耳Y","台阶X",
                        List<string> Lstrnames = new List<string> { "TabNeckX" };
                        //"极片宽度1",  "极片宽度2",  "极片高度1", "极片高度2",
                        //                     "极耳宽",  "极耳高", "台阶高",
                        List<string> LstrDesp = new List<string> { "TabNeck" };
                        //EditorTypeEnum.线距离X,   EditorTypeEnum.线距离X,   EditorTypeEnum.线距离Y, EditorTypeEnum.线距离Y,
                        //              EditorTypeEnum.线距离X,  EditorTypeEnum.线距离Y, EditorTypeEnum.线距离X, 
                        List<EditorTypeEnum> Lstrtype = new List<EditorTypeEnum> { EditorTypeEnum.线距离X };

                        //"极片边X",   /*  "极片边X1",   */  //极片X1
                        //                      "极片边X",   /*  "极片边X2",   */  //极片X2
                        //                      "极片边Y",   /*   "极片边Y1",  */  //极片Y1
                        //                      "极片边Y",   /*   "极片边Y2",  */  //极片Y2
                        //                      "极片边X",   /*   "极耳顶边",  */  //极耳X
                        //                      "极耳左边",  /*   "极耳右边",  */  //极耳Y
                        //                      "极片边X",   /*   "台阶X",     */  //台阶X
                        List<string> Line1Name = new List<string> {
                                              "LTabNeck1", };/* "TabNeckX "    */  //TabNeckX

                        //"极片边X1",
                        //                        "极片边X2",
                        //                        "极片边Y1",
                        //                        "极片边Y2",
                        //                        "极耳顶边",
                        //                        "极耳右边",
                        //                        "Lstep",
                        List<string> Line2Name = new List<string> {
                                                "LTabNeck2"    };

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
            }
        }

        /// <summary>
        /// DataGridView重新初始化槽位工艺
        /// </summary>
        /// <param name="craftInfo"></param>
        private void ReBuildCraftDefect(StackEACraft craftInfo, int CamID)
        {
            DefectItemInfo defect;
            craftInfo.ListDefectInfos[CamID].Clear();
            foreach (Defct1Enum sp in Enum.GetValues(typeof(Defct1Enum)))
            {

                defect = new DefectItemInfo();
                defect.camera = (CameraEnum)CamID;
                defect.Name = sp.ToString();
                defect.Index = (byte)sp;
                craftInfo.ListDefectInfos[CamID].Add(defect);
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
    }
}
