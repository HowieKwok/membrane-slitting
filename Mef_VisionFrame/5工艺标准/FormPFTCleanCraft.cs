using Frame_Core;
using IKapBoardClassLibrary;
using LogTool;
using Mef_VisionFrame._5工艺标准;
using NLog.Time;
using NPOI.SS.Formula.Functions;



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
    public partial class FormPFTCleanCraft : FormCraftBase
    {
        public PFTCleanCraft craft;
        FormSizeFix formSizeFix = new FormSizeFix();
        public MaterialCrafEnum material;

        public FormPFTCleanCraft(MaterialCrafEnum Tempmaterial)
        {
            InitializeComponent();
            toolClass.AllNumericUpDownControlSubscribeMouseWheelEvent(this);
            craft = new PFTCleanCraft();
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
            InitDataGridView(DataGridView_ESize, typeof(SizeItemInfo));
            InitDataGridView(DataGridView_Defect, typeof(DefectItemInfo));

            InitDataGridView(DataGridView_SlotInfo, typeof(SlotInfo));

            DataGridView_ESize.Controls.Add(Comb_EditorType);
            DataGridView_ESize.Controls.Add(Comb_Line);
            DataGridView_ESize.Controls.Add(Comb_CamS);

        }


        string str_LineName = "";  //当前配方名
        string str_OldLineName = "";//旧的配方名
        Rectangle Rectangle;

        public int SelectCamIndex = 0;

        public bool IsLinelistChanged = false;


        public override void CraftToView()
        {
            //基础EA参数
            nud_EASizeTD.Value = (decimal)craft.EASizeTD;
            nud_EASizeMD.Value = (decimal)craft.EASizeMD;
            nud_EACropShiledMD.Value = (decimal)craft.EACropShiledMD;
            nud_CoatSizeMD_A.Value = (decimal)craft.CoatSizeMD_A;
            nud_CoatSizeMD_B.Value = (decimal)craft.CoatSizeMD_B;

            //料头参数
            CB_IsBurrHead.Checked = craft.IsBurrHead;
            nud_HeadMis_AtoB.Value = (decimal)craft.HeadMis_AtoB;
            //涂布屏蔽区参数
            nud_CoatShieldMDHead.Value = (decimal)craft.CoatShieldMDHead;
            nud_CoatShieldMDTile.Value = (decimal)craft.CoatShieldMDTile;
            nud_CoatShieldTDLeft.Value = (decimal)craft.CoatShieldTDLeft;
            nud_CoatShieldTDRight.Value = (decimal)craft.CoatShieldTDRight;

            //槽位、削薄区参数
            nud_SlotNum_A.Value = (decimal)craft.SlotNum_A;
            nud_SlotNum_B.Value = (decimal)craft.SlotNum_B;
            nud_ShaveNum_A.Value = (decimal)craft.ShaveNum_A;
            nud_ShaveNum_B.Value = (decimal)craft.ShaveNum_B;


            //显示基础参数
            nud_CamNum.Value = (int)craft.CamNum;
            linkLabel_CraftName.Text = craft.CraftName;
            //复选框
            comb_materialCraf.SelectedIndex = this.comb_materialCraf.FindString(material.ToString());
            comb_Electrode.SelectedIndex = this.comb_Electrode.FindString(craft.electrode.ToString()); ;


            //添加抓边队列
            LoadLinetList();

            //添加尺寸检测项表格
            DataGridView_ESize.Rows.Clear();
            List<SizeItemInfo> Camsizes = craft.ListEASizes.FindAll(i => i.camera == (CameraEnum)comb_SelectCam.SelectedIndex);
            int SizeCount = Camsizes.Count();
            if (!CB_ShowByCam.Checked)
            {
                SizeCount = craft.ListEASizes.Count();
                Camsizes = craft.ListEASizes;
            }
            //int SizeCount = craft.ListEASizes.Count();
            for (int i = 0; i < SizeCount; i++)
            {
                AddGridData<SizeItemInfo>(DataGridView_ESize, Camsizes[i], true);
            }
            //添加缺陷检测项表格
            DataGridView_Defect.Rows.Clear();
            List<DefectItemInfo> Camdefects = craft.ListDefectInfos.FindAll(i => i.camera == (CameraEnum)comb_SelectCam.SelectedIndex);
            SizeCount = Camdefects.Count();
            if (!CB_ShowByCam.Checked)
            {
                SizeCount = craft.ListDefectInfos.Count();
                Camdefects = craft.ListDefectInfos;
            }

            for (int i = 0; i < SizeCount; i++)
            {
                AddGridData<DefectItemInfo>(DataGridView_Defect, Camdefects[i], true);


            }

            //添加槽位检测项表格
            DataGridView_SlotInfo.Rows.Clear();
            SizeCount = craft.ListSlotInfos.Count();
            UIService.Instance.AddAction(() => { });
            for (int i = 0; i < SizeCount; i++)
            {
                AddGridData<SlotInfo>(DataGridView_SlotInfo, craft.ListSlotInfos[i], true);
            }

        }

        public override CraftBase CraftFromView()
        {
            //标准极耳工艺
            craft.EASizeTD = (double)nud_EASizeTD.Value;
            craft.EASizeMD = (double)nud_EASizeMD.Value;
            craft.EACropShiledMD = (double)nud_EACropShiledMD.Value;

            craft.CoatSizeMD_A = (double)nud_CoatSizeMD_A.Value;
            craft.CoatSizeMD_B = (double)nud_CoatSizeMD_B.Value;
            //料头参数
            craft.IsBurrHead = CB_IsBurrHead.Checked;
            craft.HeadMis_AtoB = (double)nud_HeadMis_AtoB.Value;

            //涂布屏蔽区参数
            craft.CoatShieldMDHead = (double)nud_CoatShieldMDHead.Value;
            craft.CoatShieldMDTile = (double)nud_CoatShieldMDTile.Value;
            craft.CoatShieldTDLeft = (double)nud_CoatShieldTDLeft.Value;
            craft.CoatShieldTDRight = (double)nud_CoatShieldTDRight.Value;


            //槽位、削薄区参数
            craft.SlotNum_A = (int)nud_SlotNum_A.Value;
            craft.SlotNum_B = (int)nud_SlotNum_B.Value;
            craft.ShaveNum_A = (int)nud_ShaveNum_A.Value;
            craft.ShaveNum_B = (int)nud_ShaveNum_B.Value;


            //显示基础参数
            // craft.materialCrafEnum = MaterialCrafEnum.叠台对齐度;
            craft.CamNum = (int)nud_CamNum.Value;
            craft.materialCrafEnum = material;
            craft.electrode = (ElectrodeEnum)Enum.Parse(typeof(ElectrodeEnum), comb_Electrode.SelectedItem.ToString(), false);
            //显示尺寸检测项

            List<SizeItemInfo> Camsizes = craft.ListEASizes.FindAll(i => i.camera == (CameraEnum)comb_SelectCam.SelectedIndex);
            // int SizeCount = Camsizes.Count();
            if (!CB_ShowByCam.Checked)
            {
                // SizeCount = craft.ListEASizes.Count();
                Camsizes = craft.ListEASizes;
            }
            int count0 = DataGridView_ESize.Rows.Count;
            for (int i = 0; i < count0; i++)
            {
                SizeItemInfo item = Camsizes[i];
                PropertyInfo[] infos = typeof(SizeItemInfo).GetProperties();
                foreach (PropertyInfo pi in infos)
                {
                    var fieldIfno = typeof(SizeItemInfo).GetProperty(pi.Name).GetCustomAttribute<MyAttribute>();
                    if (fieldIfno.Is_ShowCol)
                    {
                        pi.SetValue(item, Convert.ChangeType(DataGridView_ESize[pi.Name, i].Value, pi.PropertyType), null);
                    }
                }
            }
            //显示缺陷检测项
            List<DefectItemInfo> Camdefects = craft.ListDefectInfos.FindAll(i => i.camera == (CameraEnum)comb_SelectCam.SelectedIndex);
            // SizeCount = Camdefects.Count();
            if (!CB_ShowByCam.Checked)
            {
                //  SizeCount = craft.ListDefectInfos.Count();
                Camdefects = craft.ListDefectInfos;
            }
            count0 = DataGridView_Defect.Rows.Count;
            for (int i = 0; i < count0; i++)
            {
                DefectItemInfo item = Camdefects[i];
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

            //显示槽位检测项
            count0 = DataGridView_SlotInfo.Rows.Count;
            for (int i = 0; i < count0; i++)
            {

                SlotInfo item = craft.ListSlotInfos[i];
                PropertyInfo[] infos = typeof(SlotInfo).GetProperties();
                foreach (PropertyInfo pi in infos)
                {
                    var fieldIfno = typeof(SlotInfo).GetProperty(pi.Name).GetCustomAttribute<MyAttribute>();
                    if (fieldIfno.Is_ShowCol)
                    {
                        pi.SetValue(item, Convert.ChangeType(DataGridView_SlotInfo[pi.Name, i].Value, pi.PropertyType), null);
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
                craft = craft.Read<PFTCleanCraft>();
            }
            catch (Exception ee)
            {
                craft = new PFTCleanCraft();
                craft.Init();
            }
            craft.filePath = Path;
            return craft;
        }

        public override void ReSetCraft()
        {
            craft = new PFTCleanCraft();
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
            DGView.BeginInvoke(new MethodInvoker(() =>
            {
                try
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
                        DGView.Update();
                    }
                }
                catch (Exception)
                {

                }

            }));


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
            pt.Index = id;
            pt.Name = Tb_Name.Text;
            pt.Description = Tb_Description.Text;
            pt.camera = (CameraEnum)comb_SelectCam.SelectedIndex;
            pt.camera1 = ((CameraEnum)comb_SelectCam.SelectedIndex).ToString();
            pt.camera2 = ((CameraEnum)comb_SelectCam.SelectedIndex).ToString();
            craft.ListEASizes.Add(pt);
            //  AddSizeData(DataGridView_Editor, pt, true);
            AddGridData<SizeItemInfo>(DataGridView_ESize, pt, true);

        }

        private void Btn_DeleteItem_Click(object sender, EventArgs e)
        {
            if (DataGridView_ESize.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选择一条要删除的项！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                try
                {
                    //根据标签ID删除信息
                    // Dclass.ExecuteSql("delete from SMS_InGenerate where ID='" + this.dataGridView1.SelectedRows[0].Cells[0].Value.ToString().Trim() + "'");

                    int Indexc = (int)DataGridView_ESize.SelectedRows[0].Cells[0].Value;
                    craft.ListEASizes.Remove(craft.ListEASizes.Find(c => c.Index == Indexc));
    
                    int Index = (int)DataGridView_ESize.SelectedRows[0].Index;
                    DataGridView_ESize.Rows.RemoveAt(Index);

                    //DataRowView drv = DataGridView_Editor.SelectedRows[0].DataBoundItem as DataRowView;
                    //drv.Row.Table.Rows.Remove(drv.Row); // 将要删除的行移除，更新时不影响数据库
                    DataGridView_ESize.Refresh();//更新
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
                if (craft.ListEASizes == null)
                {
                    craft.ListEASizes = new List<SizeItemInfo>();
                }
                else { craft.ListEASizes.Clear(); }

                for (int i = 0; i < craft.CamNum; i++)
                {
                    craft.ListEASizes.Add(new SizeItemInfo());
                }
                CraftToView();
                GC.Collect();
            }
        }
        private void Btn_Forwardtem_Click(object sender, EventArgs e)
        {
            if (DataGridView_ESize.SelectedRows.Count == 0)
            {
                MessageBox.Show("请选择一条要前移的项！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                try
                {
                    int Index = (int)DataGridView_ESize.SelectedRows[0].Index;
                    if (Index != 0)
                    {
                        SizeItemInfo temp = craft.ListEASizes[Index - 1];
                        craft.ListEASizes[Index - 1] = craft.ListEASizes[Index];
                        craft.ListEASizes[Index] = temp;

                        var temprow = DataGridView_ESize.Rows[Index - 1];
                        DataGridView_ESize.Rows.RemoveAt(Index - 1);
                        DataGridView_ESize.Rows.Insert(Index, temprow);
                    }
                    DataGridView_ESize.Refresh();//更新
                    MessageBox.Show("前移一位！");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
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

                foreach (DefectItemInfo pte in craft.ListDefectInfos)
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

            craft.ListDefectInfos.Add(pt);
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
                    craft.ListDefectInfos.RemoveAt(Index);
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
                    craft.ListDefectInfos = new List<DefectItemInfo>();
                }
                else { craft.ListDefectInfos.Clear(); }

                for (int i = 0; i < craft.CamNum; i++)
                {
                    craft.ListDefectInfos.Add(new DefectItemInfo());
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
            if (DataGridView_ESize.CurrentCell == null) return;

            DataGridViewColumn column = DataGridView_ESize.CurrentCell.OwningColumn;
            //如果是要显示下拉列表的列的话
            if (column.Name.Equals("EditorType"))
            {
                int columnIndex = DataGridView_ESize.CurrentCell.ColumnIndex;
                int rowIndex = DataGridView_ESize.CurrentCell.RowIndex;
                Rectangle rect = DataGridView_ESize.GetCellDisplayRectangle(columnIndex, rowIndex, false);
                Comb_EditorType.Left = rect.Left;
                Comb_EditorType.Top = rect.Top;
                Comb_EditorType.Width = rect.Width;
                Comb_EditorType.Height = rect.Height;
                //将单元格的内容显示为下拉列表的当前项
                string consultingRoom = DataGridView_ESize.Rows[rowIndex].Cells[columnIndex].Value.ToString();
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
                int columnIndex = DataGridView_ESize.CurrentCell.ColumnIndex;
                int rowIndex = DataGridView_ESize.CurrentCell.RowIndex;

                Comb_Line.Items.Clear();
                string cam = DataGridView_ESize.Rows[rowIndex].Cells[columnIndex - 1].Value.ToString();
                int intcam = Comb_CamS.Items.IndexOf(cam);
                foreach (var item in craft.ListLines[intcam])
                {
                    Comb_Line.Items.Add(item.StrLineName);
                }

                Rectangle rect = DataGridView_ESize.GetCellDisplayRectangle(columnIndex, rowIndex, false);
                Comb_Line.Left = rect.Left;
                Comb_Line.Top = rect.Top;
                Comb_Line.Width = rect.Width;
                Comb_Line.Height = rect.Height;
                //将单元格的内容显示为下拉列表的当前项
                string consultingRoom = DataGridView_ESize.Rows[rowIndex].Cells[columnIndex].Value.ToString();
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
                int columnIndex = DataGridView_ESize.CurrentCell.ColumnIndex;
                int rowIndex = DataGridView_ESize.CurrentCell.RowIndex;
                Rectangle rect = DataGridView_ESize.GetCellDisplayRectangle(columnIndex, rowIndex, false);
                Comb_CamS.Left = rect.Left;
                Comb_CamS.Top = rect.Top;
                Comb_CamS.Width = rect.Width;
                Comb_CamS.Height = rect.Height;
                //将单元格的内容显示为下拉列表的当前项
                try
                {
                    string consultingRoom = DataGridView_ESize.Rows[rowIndex].Cells[columnIndex].Value.ToString();
                    int index = Comb_CamS.Items.IndexOf(consultingRoom);
                    Comb_CamS.SelectedIndex = index;
                    Comb_CamS.Visible = true;
                }
                catch (Exception ex)
                {
                }
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
            if (DataGridView_ESize.CurrentCell != null)
                DataGridView_ESize.CurrentCell.Value = Comb_Line.Items[Comb_Line.SelectedIndex];

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
                if (DataGridView_ESize.CurrentCell != null)
                    DataGridView_ESize.CurrentCell.Value = Comb_EditorType.Items[Comb_EditorType.SelectedIndex];
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
                if (DataGridView_ESize.CurrentCell != null)
                    DataGridView_ESize.CurrentCell.Value = Comb_CamS.Items[Comb_CamS.SelectedIndex];
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
            if (DataGridView_ESize.Columns[col] is DataGridViewCheckBoxColumn) return;

            string val = DataGridView_ESize.Rows[row].Cells[col].Value.ToString();
            string pattern = "^-?\\d+$|^(-?\\d+)(\\.\\d+)?$";
            System.Text.RegularExpressions.Match m = Regex.Match(val, pattern);   // 匹配正则表达式

            if (m.Success)
            {
                DataGridView_ESize.Rows[row].Cells[col].Value = val;
            }
            else
            {
                string sss1 = Regex.Replace(val, "[^0-9A-Za-z]", "");
                System.Windows.MessageBox.Show("输入数据格式错误，请检查后重新输入");
                if (sss1 == "")
                {
                    DataGridView_ESize.Rows[row].Cells[col].Value = 10;
                }
                else
                {
                    DataGridView_ESize.Rows[row].Cells[col].Value = sss1;
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
                Log4netRecord.ErrorLog.Error(ee);

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
            int Index = (int)DataGridView_ESize.CurrentRow.Cells[0].Value;
            formSizeFix.size = craft.ListEASizes[Index];
            formSizeFix.ShowSet();
            formSizeFix.StartPosition = FormStartPosition.CenterScreen;

            formSizeFix.ShowDialog(this);
            craft.ListEASizes[Index] = formSizeFix.size;
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
                //初始化槽位信息
                if (craft.ListSlotInfos == null)
                {
                    craft.ListSlotInfos = new List<SlotInfo>();
                }
                else { craft.ListSlotInfos.Clear(); }

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
                if (craft.ListEASizes == null)
                {
                    craft.ListEASizes = new List<SizeItemInfo>();
                }
                else { craft.ListEASizes.Clear(); }

                //初始化缺陷项
                if (craft.ListDefectInfos == null)
                {
                    craft.ListDefectInfos = new List<DefectItemInfo>();
                }
                else { craft.ListDefectInfos.Clear(); }

                ReDefault();


                LoadLinetList();
                CraftToView();
                GC.Collect();
            }
        }


        public void ReDefault()
        {
            craft.CamNum = 4;
            //添加左右侧测基准边插件
            for (int i = 0; i < craft.CamNum; i++)
            {
                LineInfo lineInfo3 = new LineInfo();
                lineInfo3.StrLineName = $"涂布料头边";
                lineInfo3.m_camera = (CameraEnum)i;
                lineInfo3.IsStandLine = false;
                craft.ListLines[i].Add(lineInfo3);

                LineInfo lineInfo2 = new LineInfo();
                lineInfo2.StrLineName = $"涂布料尾边";
                lineInfo2.m_camera = (CameraEnum)i;
                lineInfo2.IsStandLine = false;
                craft.ListLines[i].Add(lineInfo2);

                LineInfo lineInfo = new LineInfo();
                lineInfo.StrLineName = $"涂布左基准边";
                lineInfo.m_camera = (CameraEnum)i;
                lineInfo.IsStandLine = false;
                craft.ListLines[i].Add(lineInfo);

                LineInfo lineInfo1 = new LineInfo();
                lineInfo1.StrLineName = $"涂布右基准边";
                lineInfo1.m_camera = (CameraEnum)i;
                lineInfo1.IsStandLine = false;
                craft.ListLines[i].Add(lineInfo1);
            }
            int index = 0;
            for (int i = 0; i < craft.SlotNum_A; i++)
            {
                craft.ListSlotInfos.Add(new SlotInfo() { InfoID = index, InfoName = $"A面槽位{i}", IsASide = true });
                index++;
                AddSlotLine($"A面槽位{i}", 0);
            }
            for (int i = 0; i < craft.ShaveNum_A; i++)
            {
                craft.ListSlotInfos.Add(new SlotInfo() { InfoID = index, InfoName = $"A面削薄{i}", IsASide = true });
                index++;
                AddSlotLine($"A面削薄{i}", 0);
            }

            for (int i = 0; i < craft.SlotNum_B; i++)
            {
                craft.ListSlotInfos.Add(new SlotInfo() { InfoID = index, InfoName = $"B面槽位{i}", IsASide = false });
                index++;
                AddSlotLine($"B面槽位{i}", 1);
            }
            for (int i = 0; i < craft.ShaveNum_B; i++)
            {
                craft.ListSlotInfos.Add(new SlotInfo() { InfoID = index, InfoName = $"B面削薄{i}", IsASide = false });
                index++;
                AddSlotLine($"B面削薄{i}", 1);
            }



        }

        public void AddSlotLine(string InfoName, int camID)
        {
            int count = Enum.GetValues(typeof(DirectionEnum)).Length;
            //添加A面槽位边插件
            for (int j = 0; j < count; j++)
            {
                LineInfo lineInfo = new LineInfo();
                lineInfo.StrLineName = InfoName + ((DirectionEnum)j).ToString();
                lineInfo.m_camera = (CameraEnum)camID;
                lineInfo.IsStandLine = false;
                craft.ListLines[camID].Add(lineInfo);
            }
        }


        /// <summary>
        /// DataGridView重新初始化槽位尺寸工艺
        /// </summary>
        /// <param name="craftInfo"></param>
        private void ReBuildCraftSize(PFTCleanCraft craftInfo)
        {
            craftInfo.ListEASizes.Clear();
            int index = 0;
            string name = $"A面槽位";
            SizeItemInfo size0;
            CameraEnum cam01 = CameraEnum.Cam1;
            CameraEnum cam02 = CameraEnum.Cam2;
            for (int i = 0; i < craft.SlotNum_A; i++)
            {
                name = $"A面槽位{i}";

                size0 = new SizeItemInfo();
                size0.Name = name + "定位距";
                size0.Description = size0.Name;
                size0.Index = index;
                size0.camera = cam01;
                size0.camera1 = cam01.ToString();
                size0.Line1 = $"涂布料头边";
                size0.camera2 = cam01.ToString();
                size0.Line2 = name + DirectionEnum.上边.ToString();
                size0.EditorType = EditorTypeEnum.线距离Y.ToString();
                craftInfo.ListEASizes.Add(size0);
                index++;

                size0 = new SizeItemInfo();
                size0.Name = name + "TD尺寸";
                size0.Description = size0.Name;
                size0.Index = index;
                size0.camera = cam01;
                size0.camera1 = cam01.ToString();
                size0.Line1 = name + DirectionEnum.上边.ToString();
                size0.camera2 = cam01.ToString();
                size0.Line2 = name + DirectionEnum.下边.ToString();
                size0.EditorType = EditorTypeEnum.线距离Y.ToString();
                craftInfo.ListEASizes.Add(size0);
                index++;

                size0 = new SizeItemInfo();
                size0.Name = name + "MD尺寸";
                size0.Description = size0.Name;
                size0.Index = index;
                size0.camera = cam01;
                size0.camera1 = cam01.ToString();
                size0.Line1 = name + DirectionEnum.左边.ToString();
                size0.camera2 = cam01.ToString();
                size0.Line2 = name + DirectionEnum.右边.ToString();
                size0.EditorType = EditorTypeEnum.线距离X.ToString();
                craftInfo.ListEASizes.Add(size0);
                index++;
            }
            for (int i = 0; i < craft.ShaveNum_A; i++)
            {
                name = $"A面削薄{i}";

                size0 = new SizeItemInfo();
                size0.Name = name + "定位距";
                size0.Description = size0.Name;
                size0.Index = index;
                size0.camera = cam01;
                size0.camera1 = cam01.ToString();
                size0.Line1 = $"涂布料头边";
                size0.camera2 = cam01.ToString();
                size0.Line2 = name + DirectionEnum.上边.ToString();
                size0.EditorType = EditorTypeEnum.线距离Y.ToString();
                craftInfo.ListEASizes.Add(size0);
                index++;

                size0 = new SizeItemInfo();
                size0.Name = name + "TD尺寸";
                size0.Description = size0.Name;
                size0.Index = index;
                size0.camera = cam01;
                size0.camera1 = cam01.ToString();
                size0.Line1 = name + DirectionEnum.上边.ToString();
                size0.camera2 = cam01.ToString();
                size0.Line2 = name + DirectionEnum.下边.ToString();
                size0.EditorType = EditorTypeEnum.线距离Y.ToString();
                craftInfo.ListEASizes.Add(size0);
                index++;

                size0 = new SizeItemInfo();
                size0.Name = name + "MD尺寸";
                size0.Description = size0.Name;
                size0.Index = index;
                size0.camera = cam01;
                size0.camera1 = cam01.ToString();
                size0.Line1 = name + DirectionEnum.左边.ToString();
                size0.camera2 = cam01.ToString();
                size0.Line2 = name + DirectionEnum.右边.ToString();
                size0.EditorType = EditorTypeEnum.线距离X.ToString();
                craftInfo.ListEASizes.Add(size0);
                index++;
            }
            for (int i = 0; i < craft.SlotNum_B; i++)
            {
                name = $"B面槽位{i}";

                size0 = new SizeItemInfo();
                size0.Name = name + "定位距";
                size0.Description = size0.Name;
                size0.Index = index;
                size0.camera = cam02;
                size0.camera1 = cam02.ToString();
                size0.Line1 = $"涂布料头边";
                size0.camera2 = cam02.ToString();
                size0.Line2 = name + DirectionEnum.上边.ToString();
                size0.EditorType = EditorTypeEnum.线距离Y.ToString();
                craftInfo.ListEASizes.Add(size0);
                index++;

                size0 = new SizeItemInfo();
                size0.Name = name + "TD尺寸";
                size0.Description = size0.Name;
                size0.Index = index;
                size0.camera = cam02;
                size0.camera1 = cam02.ToString();
                size0.Line1 = name + DirectionEnum.上边.ToString();
                size0.camera2 = cam02.ToString();
                size0.Line2 = name + DirectionEnum.下边.ToString();
                size0.EditorType = EditorTypeEnum.线距离Y.ToString();
                craftInfo.ListEASizes.Add(size0);
                index++;

                size0 = new SizeItemInfo();
                size0.Name = name + "MD尺寸";
                size0.Description = size0.Name;
                size0.Index = index;
                size0.camera = cam02;
                size0.camera1 = cam02.ToString();
                size0.Line1 = name + DirectionEnum.左边.ToString();
                size0.camera2 = cam02.ToString();
                size0.Line2 = name + DirectionEnum.右边.ToString();
                size0.EditorType = EditorTypeEnum.线距离X.ToString();
                craftInfo.ListEASizes.Add(size0);
                index++;

            }
            for (int i = 0; i < craft.ShaveNum_B; i++)
            {
                name = $"B面削薄{i}";

                size0 = new SizeItemInfo();
                size0.Name = name + "定位距";
                size0.Description = size0.Name;
                size0.Index = index;
                size0.camera = cam02;
                size0.camera1 = cam02.ToString();
                size0.Line1 = $"涂布料头边";
                size0.camera2 = cam02.ToString();
                size0.Line2 = name + DirectionEnum.上边.ToString();
                size0.EditorType = EditorTypeEnum.线距离Y.ToString();
                craftInfo.ListEASizes.Add(size0);
                index++;

                size0 = new SizeItemInfo();
                size0.Name = name + "TD尺寸";
                size0.Description = size0.Name;
                size0.Index = index;
                size0.camera = cam02;
                size0.camera1 = cam02.ToString();
                size0.Line1 = name + DirectionEnum.上边.ToString();
                size0.camera2 = cam02.ToString();
                size0.Line2 = name + DirectionEnum.下边.ToString();
                size0.EditorType = EditorTypeEnum.线距离Y.ToString();
                craftInfo.ListEASizes.Add(size0);
                index++;

                size0 = new SizeItemInfo();
                size0.Name = name + "MD尺寸";
                size0.Description = size0.Name;
                size0.Index = index;
                size0.camera = cam02;
                size0.camera1 = cam02.ToString();
                size0.Line1 = name + DirectionEnum.左边.ToString();
                size0.camera2 = cam02.ToString();
                size0.Line2 = name + DirectionEnum.右边.ToString();
                size0.EditorType = EditorTypeEnum.线距离X.ToString();
                craftInfo.ListEASizes.Add(size0);
                index++;

            }

            CameraEnum cam03 = CameraEnum.Cam3;
            CameraEnum cam04 = CameraEnum.Cam4;

            size0 = new SizeItemInfo();
            size0.Name = cam03.ToString() + "3D深度";
            size0.Description = size0.Name;
            size0.Index = index;
            size0.camera = cam03;
            size0.camera1 = cam03.ToString();
            size0.Line1 = $"涂布料头边";
            size0.camera2 = cam03.ToString();
            size0.Line2 = $"涂布料头边";
            size0.EditorType = EditorTypeEnum.线距离Y.ToString();
            craftInfo.ListEASizes.Add(size0);
            index++;


            size0 = new SizeItemInfo();
            size0.Name = cam04.ToString() + "3D深度";
            size0.Description = size0.Name;
            size0.Index = index;
            size0.camera = cam04;
            size0.camera1 = cam04.ToString();
            size0.Line1 = $"涂布料头边";
            size0.camera2 = cam04.ToString();
            size0.Line2 = $"涂布料头边";
            size0.EditorType = EditorTypeEnum.线距离Y.ToString();
            craftInfo.ListEASizes.Add(size0);
            index++;

            ReBuildCraftDefect(craft);
        }



        /// <summary>
        /// DataGridView重新初始化槽位工艺
        /// </summary>
        /// <param name="craftInfo"></param>
        private void ReBuildCraftDefect(PFTCleanCraft craftInfo)
        {
            DefectItemInfo defect;
            craftInfo.ListDefectInfos.Clear();
            int index = 0;
            CameraEnum cam01 = CameraEnum.Cam1;
            CameraEnum cam02 = CameraEnum.Cam2;
            foreach (Defct1Enum sp in Enum.GetValues(typeof(Defct1Enum)))
            {
                defect = new DefectItemInfo();
                defect.camera = cam01;
                defect.Name = sp.ToString();
                defect.Index = index;
                craftInfo.ListDefectInfos.Add(defect);
                index++;
            }
            foreach (Defct1Enum sp in Enum.GetValues(typeof(Defct1Enum)))
            {
                defect = new DefectItemInfo();
                defect.camera = cam02;
                defect.Name = sp.ToString();
                defect.Index = index;
                craftInfo.ListDefectInfos.Add(defect);
                index++;
            }
        }

        public int GetSizeInsertId(int cameraid)
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

        private void Btn_ReDefaultSize_Click(object sender, EventArgs e)
        {
            ReBuildCraftSize(craft);
            CraftToView();
        }

        private void CB_ShowByCam_CheckedChanged(object sender, EventArgs e)
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
                Log4netRecord.ErrorLog.Error(ee);
            }
        }
    }
}
