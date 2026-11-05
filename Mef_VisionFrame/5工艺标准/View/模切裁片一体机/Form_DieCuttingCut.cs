using AntdUI;
using Frame_Core;
using Frame_Core.pluginParams;
using NPOI.OpenXmlFormats.Dml.Diagram;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public partial class Form_DieCuttingCut : FormCraftBase
    {
        static VM_CraftDieCuttingCut VM = new VM_CraftDieCuttingCut();
        CamCraft currentCamCraft = null;
        static List<DefectCraft_DieCuttingCut> currentDefectCraft = null;
        List<string> l_CraftItems;
        public Form_DieCuttingCut()
        {
            InitializeComponent();
            InitTableColumns__DefectCraft(dataGridView_Defect, this.Font);
            InitTableColumns_SizeCraftStation3();
            InitTableColumns_SizeCraft();
            InitTableColumns_GlobalVariableitem();
        }

        #region 虚方法重写
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="craftBase"></param>
        public override void Init(CraftBase craftBase)
        {
            VM.M_CraftParam = craftBase as CraftParam_DieCuttingCut;
            if (VM.M_CraftParam == null)
            {
                System.Windows.Forms.MessageBox.Show("参数类型加载错误");
                return;
            }

            VM.Init(VM.M_CraftParam);


            Electrode.Items.Clear();
            Electrode.Items.AddRange(Enum.GetNames(typeof(ElectrodeEnum)));

            //相机工艺初始化
            List<string> camNames = Service_Camera.GetCam_Names().ToList();
            l_CraftItems = camNames;
            l_CraftItems.Add("缺陷检测工位");
            l_CraftItems.Add("极片检测工位");
            l_CraftItems.Add("全局变量");
            CraftItem.Items.Clear();
            for (int i = 0; i < l_CraftItems.Count; i++)
            {
                CraftItem.Items.Add(new AntdUI.SegmentedItem() { Text = l_CraftItems[i] });
            }
        }

        /// <summary>
        /// 把参数显示到工艺界面
        /// </summary>
        public override void ParamToView()
        {
            if (VM.M_CraftParam == null) return;
            CraftType.Text = VM.M_CraftParam.CraftType.ToString();
            RecipeName.Text = VM.M_CraftParam.RecipeName;
            Electrode.SelectedIndex = (int)VM.M_CraftParam.Electrode;
            MachineNumber.Text = VM.M_CraftParam.MachineNumber;

            CraftItem.SelectIndex = 0;

        }

        /// <summary>
        /// 界面内容下发到参数
        /// </summary>
        public override void ViewToParam()
        {
            if (VM.M_CraftParam == null) return;

            VM.M_CraftParam.RecipeName = RecipeName.Text;
            VM.M_CraftParam.Electrode = (ElectrodeEnum)Electrode.SelectedIndex;
            VM.M_CraftParam.MachineNumber = MachineNumber.Text;

            ParamAndView_Select(false);
        }
        #endregion
        private bool ShowNameDialog(string title, string defaultName, out string text)
        {
            text = string.Empty;
            using (Form dialog = new Form())
            using (TextBox textBox = new TextBox())
            using (System.Windows.Forms.Button btnOk = new System.Windows.Forms.Button())
            using (System.Windows.Forms.Button btnCancel = new System.Windows.Forms.Button())
            {
                dialog.Text = title;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.FormBorderStyle = FormBorderStyle.FixedDialog;
                dialog.MinimizeBox = false;
                dialog.MaximizeBox = false;
                dialog.ClientSize = new Size(360, 112);
                dialog.Font = Font;

                textBox.Location = new Point(16, 18);
                textBox.Size = new Size(328, 26);
                textBox.Text = defaultName;
                textBox.SelectAll();

                btnOk.Text = "确定";
                btnOk.Location = new Point(176, 66);
                btnOk.Size = new Size(80, 30);
                btnOk.DialogResult = DialogResult.OK;

                btnCancel.Text = "取消";
                btnCancel.Location = new Point(264, 66);
                btnCancel.Size = new Size(80, 30);
                btnCancel.DialogResult = DialogResult.Cancel;

                dialog.Controls.Add(textBox);
                dialog.Controls.Add(btnOk);
                dialog.Controls.Add(btnCancel);
                dialog.AcceptButton = btnOk;
                dialog.CancelButton = btnCancel;

                // 输入校验示例：禁止空白确认
                dialog.FormClosing += (s, e) =>
                {
                    if (dialog.DialogResult == DialogResult.OK && string.IsNullOrWhiteSpace(textBox.Text))
                    {
                        MessageBox.Show("输入内容不能为空！");
                        e.Cancel = true; // 阻止弹窗关闭
                    }
                };

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    text = textBox.Text.Trim();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #region 缺陷工艺
        private static void InitTableColumns__DefectCraft(DataGridView dataGridView, Font font)
        {
            dataGridView.Columns.Clear();
            dataGridView.AutoGenerateColumns = false;

            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(31, 35, 41);
            dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 244, 255);
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.FromArgb(22, 119, 255);
            dataGridView.DefaultCellStyle.Font = font;

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ID",
                DataPropertyName = "ID",
                HeaderText = "ID",
                Width = 40,
                MinimumWidth = 40,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                ReadOnly = true,

            });

            //dataGridView.Columns.Add(new DataGridViewCheckBoxColumn
            //{
            //    Name = "Enable",
            //    DataPropertyName = "Enable",
            //    HeaderText = "启用",
            //    Width = 50,
            //    SortMode = DataGridViewColumnSortMode.NotSortable,
            //    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            //});

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                DataPropertyName = "Name",
                HeaderText = "检测项",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                ReadOnly = true,
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Describe",
                DataPropertyName = "Describe",
                HeaderText = "描述",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Condition",
                DataPropertyName = "Condition",
                HeaderText = "计算公式（面积单位：mm²，其他单位：mm）",
                Width = 500,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                ReadOnly = true,
            });

            dataGridView.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "DB",
                DataPropertyName = "DB",
                HeaderText = "剔片",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dataGridView.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "IsSevere",
                DataPropertyName = "IsSevere",
                HeaderText = "重度",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ContinuousCount",
                DataPropertyName = "ContinuousCount",
                HeaderText = "连续数量",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });


            dataGridView.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "BJ_Continuous",
                DataPropertyName = "BJ_Continuous",
                HeaderText = "报警",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dataGridView.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "TJ_Continuous",
                DataPropertyName = "TJ_Continuous",
                HeaderText = "停机",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AccumulateCount",
                DataPropertyName = "AccumulateCount",
                HeaderText = "累计数量",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });


            dataGridView.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "BJ_Accumulate",
                DataPropertyName = "BJ_Accumulate",
                HeaderText = "报警",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dataGridView.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "TJ_Accumulate",
                DataPropertyName = "TJ_Accumulate",
                HeaderText = "停机",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
        }
        private static DataTable BuildTable_DefectCraft(List<DefectCraft_DieCuttingCut> defectCrafts)
        {

            DataTable table = new DataTable();
            table.Columns.Add("ID");
            //table.Columns.Add("Enable");
            table.Columns.Add("Name");
            table.Columns.Add("Describe");
            table.Columns.Add("Condition");
            table.Columns.Add("DB");
            table.Columns.Add("IsSevere");
            table.Columns.Add("ContinuousCount");
            table.Columns.Add("BJ_Continuous");
            table.Columns.Add("TJ_Continuous");
            table.Columns.Add("AccumulateCount");
            table.Columns.Add("BJ_Accumulate");
            table.Columns.Add("TJ_Accumulate");

            foreach (var row in defectCrafts)
            {
                table.Rows.Add(
                    row.Id,
                    //row.Enable,
                    row.Name,
                    row.Describe,
                    row.GetConditionalFormat(),
                    row.DB,
                    row.IsSevere,
                    row.ContinuousCount,
                    row.BJ_Continuous,
                    row.TJ_Continuous,
                    row.AccumulateCount,
                    row.BJ_Accumulate,
                    row.TJ_Accumulate);
            }

            return table;
        }
        private static void GetBuildTable_DefectCraft(DataGridView dataGridView, List<DefectCraft_DieCuttingCut> defectCrafts)
        {
            int colIndex = 0;
            string rowID = "";
            string colName = "";
            try
            {
                for (int i = 0; i < dataGridView.Rows.Count; i++)
                {
                    rowID = defectCrafts[i].Id.ToString();
                    colIndex = 0;
                    defectCrafts[i].Id = Convert.ToInt32(dataGridView.Rows[i].Cells[colIndex++].Value);
                    //defectCrafts[i].Enable = Convert.ToBoolean(dataGridView.Rows[i].Cells[colIndex++].Value);
                    defectCrafts[i].Name = Convert.ToString(dataGridView.Rows[i].Cells[colIndex++].Value);
                    defectCrafts[i].Describe = Convert.ToString(dataGridView.Rows[i].Cells[colIndex++].Value);
                    colIndex++;//这列是条件格式
                    defectCrafts[i].DB = Convert.ToBoolean(dataGridView.Rows[i].Cells[colIndex++].Value);
                    defectCrafts[i].IsSevere = Convert.ToBoolean(dataGridView.Rows[i].Cells[colIndex++].Value);
                    defectCrafts[i].ContinuousCount = Convert.ToInt32(dataGridView.Rows[i].Cells[colIndex++].Value);
                    defectCrafts[i].BJ_Continuous = Convert.ToBoolean(dataGridView.Rows[i].Cells[colIndex++].Value);
                    defectCrafts[i].TJ_Continuous = Convert.ToBoolean(dataGridView.Rows[i].Cells[colIndex++].Value);
                    defectCrafts[i].AccumulateCount = Convert.ToInt32(dataGridView.Rows[i].Cells[colIndex++].Value);
                    defectCrafts[i].BJ_Accumulate = Convert.ToBoolean(dataGridView.Rows[i].Cells[colIndex++].Value);
                    defectCrafts[i].TJ_Accumulate = Convert.ToBoolean(dataGridView.Rows[i].Cells[colIndex++].Value);
                }
            }
            catch (Exception ex)
            {
                colIndex = colIndex - 1;
                if (colIndex < 0) return;
                if (dataGridView?.Columns?.Count > colIndex) colName = dataGridView?.Columns?[colIndex].HeaderText;
                string showMessage = $"行【{rowID}】列【{colName}】\r\n数据类型格式输入错误";
                MessageBox.Show(showMessage, "取消保存");
            }
        }
        private static void UpdateTable_DefectCrafts(DataGridView dataGridView, List<DefectCraft_DieCuttingCut> defectCrafts)
        {
            if (defectCrafts != null)
            {
                dataGridView.DataSource = BuildTable_DefectCraft(defectCrafts);
            }
            currentDefectCraft = defectCrafts;
        }
        private static void ShowCraftContextMenu_DefectCrafts(DataGridView dataGridView, List<DefectCraft_DieCuttingCut> defectCrafts, bool hasSelected, int index)
        {
            IContextMenuStripItem[] items =
            {
                new ContextMenuStripItem("编辑") { Tag = "edit",Enabled=hasSelected },
                //new ContextMenuStripItem("导出") { Tag = "export",Enabled=hasSelected },
                //new ContextMenuStripItem("加载") { Tag = "load",Enabled=hasSelected },
                new ContextMenuStripItem("置顶") { Tag = "top" ,Enabled=hasSelected},
                new ContextMenuStripItem("上移") { Tag = "moveUp" ,Enabled=hasSelected},
                new ContextMenuStripItem("下移") { Tag = "moveDown" ,Enabled=hasSelected},
                new ContextMenuStripItem("添加类型") { Tag = "create" },
                new ContextMenuStripItem("删除类型") { Tag = "remove" ,Enabled=hasSelected},
            };
            if (defectCrafts == null) return;

            AntdUI.ContextMenuStrip.open(dataGridView, item =>
            {
                string tag = item.Tag as string;
                if (tag == "edit")
                {
                    using (Form_DefectRules form = new Form_DefectRules(defectCrafts, index))
                    {
                        form.StartPosition = FormStartPosition.CenterParent;
                        form.Text = "编辑";
                        form.ShowDialog();
                        UpdateTable_DefectCrafts(dataGridView, defectCrafts);
                    }
                }
                if (tag == "top")
                {
                    VM.Top(defectCrafts, index);
                    UpdateTable_DefectCrafts(dataGridView, defectCrafts);
                }
                if (tag == "moveUp")
                {
                    VM.MoveUp(defectCrafts, index);
                    UpdateTable_DefectCrafts(dataGridView, defectCrafts);
                }
                if (tag == "moveDown")
                {
                    VM.MoveDown(defectCrafts, index);
                    UpdateTable_DefectCrafts(dataGridView, defectCrafts);
                }
                else if (tag == "create")
                {
                    DefectCraft_DieCuttingCut defectCraft = new DefectCraft_DieCuttingCut();

                    using (Form_DefectRules form = new Form_DefectRules(defectCraft))
                    {
                        form.Text = "添加类型";
                        DialogResult d = form.ShowDialog();
                        if (d == DialogResult.OK)
                        {
                            defectCraft.Describe = defectCraft.Name;
                            VM.AddDefect(defectCrafts, defectCraft);
                            UpdateTable_DefectCrafts(dataGridView, defectCrafts);
                        }
                    }
                }
                else if (tag == "remove")
                {
                    string name = defectCrafts?.Get_DefectNames(index);
                    DialogResult result = System.Windows.Forms.MessageBox.Show($"是否删除当前检测项:{name}？", "询问", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No)
                    {
                        return;
                    }

                    VM.DeleteDefect(defectCrafts, index);
                    UpdateTable_DefectCrafts(dataGridView, defectCrafts);
                }
            }, items, 0);
        }
        private void dataGridView_Defect_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = e.RowIndex;
            bool hasSelet = index >= 0;
            if (e.Button == MouseButtons.Right)
            {
                ShowCraftContextMenu_DefectCrafts(dataGridView_Defect, currentDefectCraft, hasSelet, index);
            }
        }
        #endregion


        #region 尺寸工艺
        private void InitTableColumns_SizeCraft()
        {
            dataGridView_Size.Columns.Clear();
            dataGridView_Size.AutoGenerateColumns = false;

            dataGridView_Size.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dataGridView_Size.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(31, 35, 41);
            dataGridView_Size.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView_Size.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 244, 255);
            dataGridView_Size.DefaultCellStyle.SelectionForeColor = Color.FromArgb(22, 119, 255);
            dataGridView_Size.DefaultCellStyle.Font = Font;

            dataGridView_Size.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ID",
                DataPropertyName = "ID",
                HeaderText = "ID",
                Width = 40,
                MinimumWidth = 40,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                ReadOnly = true,

            });

            //dataGridView_Size.Columns.Add(new DataGridViewCheckBoxColumn
            //{
            //    Name = "Enable",
            //    DataPropertyName = "Enable",
            //    HeaderText = "启用",
            //    Width = 50,
            //    SortMode = DataGridViewColumnSortMode.NotSortable,
            //    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            //});

            dataGridView_Size.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                DataPropertyName = "Name",
                HeaderText = "检测项",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                ReadOnly = true,
            });

            dataGridView_Size.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Describe",
                DataPropertyName = "Describe",
                HeaderText = "描述",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dataGridView_Size.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CalibrateCoeff",
                DataPropertyName = "CalibrateCoeff",
                HeaderText = "校准系数",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                ReadOnly = true,
            });

            dataGridView_Size.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MeasureOffset",
                DataPropertyName = "MeasureOffset",
                HeaderText = "测量偏移",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                ReadOnly = true,
            });

            dataGridView_Size.Columns.Add(new DataGridViewComboBoxColumn
            {
                Name = "CalcFormula",
                DataPropertyName = "CalcFormula",
                HeaderText = "计算公式",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                DataSource = Enum.GetValues(typeof(CalcFormulaEnum)).Cast<CalcFormulaEnum>()
                                                                    .Select(e => new
                                                                    {
                                                                        Text = e.ToString(),
                                                                        Value = (int)e
                                                                    }).ToList(),

                DisplayMember = "Text",  // 界面显示枚举文字
                ValueMember = "Value"    // 绑定后台int数值，和DataTable匹配

            });


            dataGridView_Size.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DataSource1",
                DataPropertyName = "DataSource1",
                HeaderText = "数据源1",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },

            });


            dataGridView_Size.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DataSource2",
                DataPropertyName = "DataSource2",
                HeaderText = "数据源2",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
            });

            dataGridView_Size.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StandardValue",
                DataPropertyName = "StandardValue",
                HeaderText = "标准值",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
            });

            dataGridView_Size.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DBTolLower",
                DataPropertyName = "DBTolLower",
                HeaderText = "下公差",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
            });

            dataGridView_Size.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DBTolUpper",
                DataPropertyName = "DBTolUpper",
                HeaderText = "上公差",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
            });

            dataGridView_Size.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DBContinuous",
                DataPropertyName = "DBContinuous",
                HeaderText = "连续",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
            });


            dataGridView_Size.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "DB",
                DataPropertyName = "DB",
                HeaderText = "剔片",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });



            dataGridView_Size.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ContinuousCount",
                DataPropertyName = "ContinuousCount",
                HeaderText = "连续数量",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
            });

            dataGridView_Size.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "BJ_Continuous",
                DataPropertyName = "BJ_Continuous",
                HeaderText = "报警",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dataGridView_Size.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "TJ_Continuous",
                DataPropertyName = "TJ_Continuous",
                HeaderText = "停机",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });


            dataGridView_Size.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AccumulateCount",
                DataPropertyName = "AccumulateCount",
                HeaderText = "累计数量",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
            });


            dataGridView_Size.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "BJ_Accumulate",
                DataPropertyName = "BJ_Accumulate",
                HeaderText = "报警",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dataGridView_Size.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "TJ_Accumulate",
                DataPropertyName = "TJ_Accumulate",
                HeaderText = "停机",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
        }
        private DataTable BuildTable_SizeCraft(List<SizeCraft_DieCuttingCut> l_Craft)
        {

            DataTable table = new DataTable();
            table.Columns.Add("ID");
            //table.Columns.Add("Enable");
            table.Columns.Add("Name");
            table.Columns.Add("Describe");
            table.Columns.Add("CalibrateCoeff");
            table.Columns.Add("MeasureOffset");
            table.Columns.Add("CalcFormula", typeof(int));
            table.Columns.Add("DataSource1");
            table.Columns.Add("DataSource2");
            table.Columns.Add("StandardValue");

            table.Columns.Add("DBTolLower");
            table.Columns.Add("DBTolUpper");
            table.Columns.Add("DBContinuous");
            table.Columns.Add("DB");

            table.Columns.Add("ContinuousCount");
            table.Columns.Add("BJ_Continuous");
            table.Columns.Add("TJ_Continuous");

            table.Columns.Add("AccumulateCount");
            table.Columns.Add("BJ_Accumulate");
            table.Columns.Add("TJ_Accumulate");

            foreach (var row in l_Craft)
            {
                table.Rows.Add(
                    row.Id,
                    //row.Enable,
                    row.Name,
                    row.Describe,
                    row.CalibrateCoeff,
                    row.MeasureOffset,
                    row.CalcFormula,
                    row.DataSource1,
                    row.DataSource2,
                    row.StandardValue,

                    row.DBTolLower,
                    row.DBTolUpper,
                    row.DBContinuous,
                    row.DBEnable,

                    row.ContinuousCount,
                    row.BJ_Continuous,
                    row.TJ_Continuous,

                    row.AccumulateCount,
                    row.BJ_Accumulate,
                    row.TJ_Accumulate);
            }

            return table;
        }
        private void GetBuildTable_SizeCraft(List<SizeCraft_DieCuttingCut> l_Craft)
        {
            int colIndex = 0;
            string rowID = "";
            string colName = "";
            try
            {
                for (int i = 0; i < dataGridView_Size.Rows.Count; i++)
                {
                    rowID = l_Craft[i].Id.ToString();
                    colIndex = 0;
                    l_Craft[i].Id = Convert.ToInt32(dataGridView_Size.Rows[i].Cells[colIndex++].Value);
                    //l_Craft[i].Enable = Convert.ToBoolean(dataGridView_Size.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].Name = Convert.ToString(dataGridView_Size.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].Describe = Convert.ToString(dataGridView_Size.Rows[i].Cells[colIndex++].Value);

                    l_Craft[i].CalibrateCoeff = Convert.ToDouble(dataGridView_Size.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].MeasureOffset = Convert.ToDouble(dataGridView_Size.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].CalcFormula = Convert.ToInt32(dataGridView_Size.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].DataSource1 = Convert.ToString(dataGridView_Size.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].DataSource2 = Convert.ToString(dataGridView_Size.Rows[i].Cells[colIndex++].Value);

                    l_Craft[i].StandardValue = Convert.ToDouble(dataGridView_Size.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].DBTolLower = Convert.ToDouble(dataGridView_Size.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].DBTolUpper = Convert.ToDouble(dataGridView_Size.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].DBContinuous = Convert.ToInt32(dataGridView_Size.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].DBEnable = Convert.ToBoolean(dataGridView_Size.Rows[i].Cells[colIndex++].Value);

                    l_Craft[i].ContinuousCount = Convert.ToInt32(dataGridView_Size.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].BJ_Continuous = Convert.ToBoolean(dataGridView_Size.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].TJ_Continuous = Convert.ToBoolean(dataGridView_Size.Rows[i].Cells[colIndex++].Value);

                    l_Craft[i].AccumulateCount = Convert.ToInt32(dataGridView_Size.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].BJ_Accumulate = Convert.ToBoolean(dataGridView_Size.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].TJ_Accumulate = Convert.ToBoolean(dataGridView_Size.Rows[i].Cells[colIndex++].Value);
                }
            }
            catch (Exception ex)
            {
                colIndex = colIndex - 1;
                if (colIndex < 0) return;
                if (dataGridView_Size?.Columns?.Count > colIndex) colName = dataGridView_Size?.Columns?[colIndex].HeaderText;
                string showMessage = $"行【{rowID}】列【{colName}】\r\n数据类型格式输入错误";
                MessageBox.Show(showMessage, "取消保存");
            }
        }
        private void UpdateTable_SizeCraft(List<SizeCraft_DieCuttingCut> l_Craft)
        {
            if (l_Craft != null)
            {
                dataGridView_Size.DataSource = BuildTable_SizeCraft(l_Craft);
            }
        }
        private void ShowCraftContextMenu_SizeCraft(bool hasSelected, int index)
        {
            IContextMenuStripItem[] items =
            {
                new ContextMenuStripItem("编辑") { Tag = "edit",Enabled=hasSelected },
                //new ContextMenuStripItem("导出") { Tag = "export",Enabled=hasSelected },
                //new ContextMenuStripItem("加载") { Tag = "load",Enabled=hasSelected },
                new ContextMenuStripItem("置顶") { Tag = "top" ,Enabled=hasSelected},
                new ContextMenuStripItem("上移") { Tag = "moveUp" ,Enabled=hasSelected},
                new ContextMenuStripItem("下移") { Tag = "moveDown" ,Enabled=hasSelected},
                new ContextMenuStripItem("添加") { Tag = "create" },
                new ContextMenuStripItem("删除") { Tag = "remove" ,Enabled=hasSelected},
            };

            AntdUI.ContextMenuStrip.open(dataGridView_Size, item =>
            {
                string tag = item.Tag as string;
                if (tag == "edit")
                {
                    using (Form_SizeEdit form = new Form_SizeEdit(currentCamCraft.l_SizeCraft, index))
                    {
                        form.StartPosition = FormStartPosition.CenterParent;
                        form.Text = "编辑";
                        form.ShowDialog();
                        UpdateTable_SizeCraft(currentCamCraft.l_SizeCraft);
                    }
                }
                if (tag == "top")
                {
                    VM.Top(currentCamCraft.l_SizeCraft, index);
                    UpdateTable_SizeCraft(currentCamCraft.l_SizeCraft);
                }
                if (tag == "moveUp")
                {
                    VM.MoveUp(currentCamCraft.l_SizeCraft, index);
                    UpdateTable_SizeCraft(currentCamCraft.l_SizeCraft);
                }
                if (tag == "moveDown")
                {
                    VM.MoveDown(currentCamCraft.l_SizeCraft, index);
                    UpdateTable_SizeCraft(currentCamCraft.l_SizeCraft);
                }
                else if (tag == "create")
                {
                    SizeCraft_DieCuttingCut craftTemp = new SizeCraft_DieCuttingCut();

                    if (!ShowNameDialog("添加类型", "", out string newName)) return;
                    craftTemp.Name = newName;
                    craftTemp.Describe = newName;
                    VM.AddSizeCraft(currentCamCraft.l_SizeCraft, craftTemp);
                    UpdateTable_SizeCraft(currentCamCraft.l_SizeCraft);
                }
                else if (tag == "remove")
                {
                    string name = currentCamCraft?.l_SizeCraft?.Get_SizeNames(index);
                    DialogResult result = System.Windows.Forms.MessageBox.Show($"是否删除当前检测项:{name}？", "询问", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No)
                    {
                        return;
                    }

                    VM.DeleteSizeCraft(currentCamCraft.l_SizeCraft, index);
                    UpdateTable_SizeCraft(currentCamCraft.l_SizeCraft);
                }
            }, items, 0);
        }
        private void dataGridView_Size_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = e.RowIndex;
            bool hasSelet = index >= 0;
            if (e.Button == MouseButtons.Right)
            {
                ShowCraftContextMenu_SizeCraft(hasSelet, index);
            }
        }
        #endregion

        #region 工位三尺寸工艺
        private void InitTableColumns_SizeCraftStation3()
        {
            dataGridView_SizeStation3.Columns.Clear();
            dataGridView_SizeStation3.AutoGenerateColumns = false;

            dataGridView_SizeStation3.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dataGridView_SizeStation3.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(31, 35, 41);
            dataGridView_SizeStation3.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView_SizeStation3.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 244, 255);
            dataGridView_SizeStation3.DefaultCellStyle.SelectionForeColor = Color.FromArgb(22, 119, 255);
            dataGridView_SizeStation3.DefaultCellStyle.Font = Font;

            dataGridView_SizeStation3.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ID",
                DataPropertyName = "ID",
                HeaderText = "ID",
                Width = 40,
                MinimumWidth = 40,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                ReadOnly = true,
            });

            //dataGridView_SizeStation3.Columns.Add(new DataGridViewCheckBoxColumn
            //{
            //    Name = "Enable",
            //    DataPropertyName = "Enable",
            //    HeaderText = "启用",
            //    Width = 50,
            //    SortMode = DataGridViewColumnSortMode.NotSortable,
            //    DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            //});

            dataGridView_SizeStation3.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                DataPropertyName = "Name",
                HeaderText = "检测项",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                ReadOnly = true,
            });

            dataGridView_SizeStation3.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Describe",
                DataPropertyName = "Describe",
                HeaderText = "描述",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dataGridView_SizeStation3.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CalibrateCoeff",
                DataPropertyName = "CalibrateCoeff",
                HeaderText = "校准系数",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                ReadOnly = true,
            });

            dataGridView_SizeStation3.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MeasureOffset",
                DataPropertyName = "MeasureOffset",
                HeaderText = "测量偏移",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                ReadOnly = true,
            });

            dataGridView_SizeStation3.Columns.Add(new DataGridViewComboBoxColumn
            {
                Name = "CalcFormula",
                DataPropertyName = "CalcFormula",
                HeaderText = "计算公式",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                DataSource = Enum.GetValues(typeof(CalcFormulaEnum)).Cast<CalcFormulaEnum>()
                                                                    .Select(e => new
                                                                    {
                                                                        Text = e.ToString(),
                                                                        Value = (int)e
                                                                    }).ToList(),

                DisplayMember = "Text",  // 界面显示枚举文字
                ValueMember = "Value"    // 绑定后台int数值，和DataTable匹配

            });

            dataGridView_SizeStation3.Columns.Add(new DataGridViewComboBoxColumn
            {
                Name = "CameraId1",
                DataPropertyName = "CameraId1",
                HeaderText = "所属相机",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                DataSource = Service_Camera.GetCam_Names().Select((text, idx) => new
                {
                    Text = text,
                    Value = idx
                }).ToList(),
                DisplayMember = "Text",  // 界面显示枚举文字
                ValueMember = "Value"    // 绑定后台int数值，和DataTable匹配
            });

            dataGridView_SizeStation3.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DataSource1",
                DataPropertyName = "DataSource1",
                HeaderText = "数据源1",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
            });

            dataGridView_SizeStation3.Columns.Add(new DataGridViewComboBoxColumn
            {
                Name = "CameraId2",
                DataPropertyName = "CameraId2",
                HeaderText = "所属相机",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                DataSource = Service_Camera.GetCam_Names().Select((text, idx) => new
                {
                    Text = text,
                    Value = idx
                }).ToList(),
                DisplayMember = "Text",  // 界面显示枚举文字
                ValueMember = "Value"    // 绑定后台int数值，和DataTable匹配
            });

            dataGridView_SizeStation3.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DataSource2",
                DataPropertyName = "DataSource2",
                HeaderText = "数据源2",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
            });

            dataGridView_SizeStation3.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "StandardValue",
                DataPropertyName = "StandardValue",
                HeaderText = "标准值",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
            });

            dataGridView_SizeStation3.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DBTolLower",
                DataPropertyName = "DBTolLower",
                HeaderText = "下公差",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
            });

            dataGridView_SizeStation3.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DBTolUpper",
                DataPropertyName = "DBTolUpper",
                HeaderText = "上公差",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
            });

            dataGridView_SizeStation3.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DBContinuous",
                DataPropertyName = "DBContinuous",
                HeaderText = "连续",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
            });


            dataGridView_SizeStation3.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "DB",
                DataPropertyName = "DB",
                HeaderText = "剔片",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });



            dataGridView_SizeStation3.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ContinuousCount",
                DataPropertyName = "ContinuousCount",
                HeaderText = "连续数量",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
            });

            dataGridView_SizeStation3.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "BJ_Continuous",
                DataPropertyName = "BJ_Continuous",
                HeaderText = "报警",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dataGridView_SizeStation3.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "TJ_Continuous",
                DataPropertyName = "TJ_Continuous",
                HeaderText = "停机",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });


            dataGridView_SizeStation3.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "AccumulateCount",
                DataPropertyName = "AccumulateCount",
                HeaderText = "累计数量",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
            });


            dataGridView_SizeStation3.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "BJ_Accumulate",
                DataPropertyName = "BJ_Accumulate",
                HeaderText = "报警",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dataGridView_SizeStation3.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "TJ_Accumulate",
                DataPropertyName = "TJ_Accumulate",
                HeaderText = "停机",
                Width = 50,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
        }
        private DataTable BuildTable_SizeCraftStation3(List<SizeCraftStation_DieCuttingCut> l_Craft)
        {

            DataTable table = new DataTable();
            table.Columns.Add("ID");
            //table.Columns.Add("Enable");
            table.Columns.Add("Name");
            table.Columns.Add("Describe");
            table.Columns.Add("CalibrateCoeff");
            table.Columns.Add("MeasureOffset");
            table.Columns.Add("CalcFormula", typeof(int));
            table.Columns.Add("CameraId1", typeof(int));
            table.Columns.Add("DataSource1");
            table.Columns.Add("CameraId2", typeof(int));
            table.Columns.Add("DataSource2");
            table.Columns.Add("StandardValue");

            table.Columns.Add("DBTolLower");
            table.Columns.Add("DBTolUpper");
            table.Columns.Add("DBContinuous");
            table.Columns.Add("DB");

            table.Columns.Add("ContinuousCount");
            table.Columns.Add("BJ_Continuous");
            table.Columns.Add("TJ_Continuous");

            table.Columns.Add("AccumulateCount");
            table.Columns.Add("BJ_Accumulate");
            table.Columns.Add("TJ_Accumulate");

            foreach (var row in l_Craft)
            {
                table.Rows.Add(
                    row.Id,
                    //row.Enable,
                    row.Name,
                    row.Describe,
                    row.CalibrateCoeff,
                    row.MeasureOffset,
                    row.CalcFormula,
                    row.CameraId1,
                    row.DataSource1,
                    row.CameraId2,
                    row.DataSource2,
                    row.StandardValue,

                    row.DBTolLower,
                    row.DBTolUpper,
                    row.DBContinuous,
                    row.DBEnable,

                    row.ContinuousCount,
                    row.BJ_Continuous,
                    row.TJ_Continuous,

                    row.AccumulateCount,
                    row.BJ_Accumulate,
                    row.TJ_Accumulate);
            }

            return table;
        }
        private void GetBuildTable_SizeCraftStation3(List<SizeCraftStation_DieCuttingCut> l_Craft)
        {
            int colIndex = 0;
            string rowID = "";
            string colName = "";
            try
            {
                for (int i = 0; i < dataGridView_SizeStation3.Rows.Count; i++)
                {
                    rowID = l_Craft[i].Id.ToString();
                    colIndex = 0;
                    l_Craft[i].Id = Convert.ToInt32(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);
                    //l_Craft[i].Enable = Convert.ToBoolean(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].Name = Convert.ToString(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].Describe = Convert.ToString(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);

                    l_Craft[i].CalibrateCoeff = Convert.ToDouble(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].MeasureOffset = Convert.ToDouble(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].CalcFormula = Convert.ToInt32(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].CameraId1 = Convert.ToInt32(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].DataSource1 = Convert.ToString(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].CameraId2 = Convert.ToInt32(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].DataSource2 = Convert.ToString(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);

                    l_Craft[i].StandardValue = Convert.ToDouble(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].DBTolLower = Convert.ToDouble(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].DBTolUpper = Convert.ToDouble(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].DBContinuous = Convert.ToInt32(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].DBEnable = Convert.ToBoolean(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);


                    l_Craft[i].ContinuousCount = Convert.ToInt32(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].BJ_Continuous = Convert.ToBoolean(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].TJ_Continuous = Convert.ToBoolean(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);


                    l_Craft[i].AccumulateCount = Convert.ToInt32(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].BJ_Accumulate = Convert.ToBoolean(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);
                    l_Craft[i].TJ_Accumulate = Convert.ToBoolean(dataGridView_SizeStation3.Rows[i].Cells[colIndex++].Value);
                }
            }
            catch (Exception ex)
            {
                colIndex = colIndex - 1;
                if (colIndex < 0) return;
                if (dataGridView_SizeStation3?.Columns?.Count > colIndex) colName = dataGridView_SizeStation3?.Columns?[colIndex].HeaderText;
                string showMessage = $"行【{rowID}】列【{colName}】\r\n数据类型格式输入错误";
                MessageBox.Show(showMessage, "取消保存");
            }
        }
        private void UpdateTable_SizeCraftStation3(List<SizeCraftStation_DieCuttingCut> l_Craft)
        {
            if (l_Craft != null)
            {
                dataGridView_SizeStation3.DataSource = BuildTable_SizeCraftStation3(l_Craft);
            }
        }
        private void ShowCraftContextMenu_SizeCraftStation3(bool hasSelected, int index)
        {
            IContextMenuStripItem[] items =
            {
                new ContextMenuStripItem("编辑") { Tag = "edit",Enabled=hasSelected },
                //new ContextMenuStripItem("导出") { Tag = "export",Enabled=hasSelected },
                //new ContextMenuStripItem("加载") { Tag = "load",Enabled=hasSelected },
                new ContextMenuStripItem("置顶") { Tag = "top" ,Enabled=hasSelected},
                new ContextMenuStripItem("上移") { Tag = "moveUp" ,Enabled=hasSelected},
                new ContextMenuStripItem("下移") { Tag = "moveDown" ,Enabled=hasSelected},
                new ContextMenuStripItem("添加") { Tag = "create" },
                new ContextMenuStripItem("删除") { Tag = "remove" ,Enabled=hasSelected},
            };

            AntdUI.ContextMenuStrip.open(dataGridView_SizeStation3, item =>
            {
                string tag = item.Tag as string;
                if (tag == "edit")
                {
                    using (Form_SizeStationEdit form = new Form_SizeStationEdit(VM.M_CraftParam.l_SizeCraftStation_3, index))
                    {
                        form.StartPosition = FormStartPosition.CenterParent;
                        form.Text = "编辑";
                        form.ShowDialog();
                        UpdateTable_SizeCraftStation3(VM.M_CraftParam.l_SizeCraftStation_3);
                    }
                }
                if (tag == "top")
                {
                    VM.Top(VM.M_CraftParam.l_SizeCraftStation_3, index);
                    UpdateTable_SizeCraftStation3(VM.M_CraftParam.l_SizeCraftStation_3);
                }
                if (tag == "moveUp")
                {
                    VM.MoveUp(VM.M_CraftParam.l_SizeCraftStation_3, index);
                    UpdateTable_SizeCraftStation3(VM.M_CraftParam.l_SizeCraftStation_3);
                }
                if (tag == "moveDown")
                {
                    VM.MoveDown(VM.M_CraftParam.l_SizeCraftStation_3, index);
                    UpdateTable_SizeCraftStation3(VM.M_CraftParam.l_SizeCraftStation_3);
                }
                else if (tag == "create")
                {
                    SizeCraftStation_DieCuttingCut craftTemp = new SizeCraftStation_DieCuttingCut();

                    if (!ShowNameDialog("添加类型", "", out string newName)) return;
                    craftTemp.Name = newName;
                    craftTemp.Describe = newName;
                    VM.AddSizeCraftStation_3(VM.M_CraftParam.l_SizeCraftStation_3, craftTemp);
                    UpdateTable_SizeCraftStation3(VM.M_CraftParam.l_SizeCraftStation_3);
                }
                else if (tag == "remove")
                {
                    string name = VM.M_CraftParam?.l_SizeCraftStation_3?.Get_SizeNames(index);
                    DialogResult result = System.Windows.Forms.MessageBox.Show($"是否删除当前检测项:{name}？", "询问", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No)
                    {
                        return;
                    }
                    VM.DeleteSizeCraftStation_3(VM.M_CraftParam.l_SizeCraftStation_3, index);
                    UpdateTable_SizeCraftStation3(VM.M_CraftParam.l_SizeCraftStation_3);
                }
            }, items, 0);
        }
        private void dataGridView_SizeStation3_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = e.RowIndex;
            bool hasSelet = index >= 0;
            if (e.Button == MouseButtons.Right)
            {
                ShowCraftContextMenu_SizeCraftStation3(hasSelet, index);
            }
        }
        #endregion

        #region 全局变量
        private void InitTableColumns_GlobalVariableitem()
        {
            dataGridView_GlobalVariable.Columns.Clear();
            dataGridView_GlobalVariable.AutoGenerateColumns = false;

            dataGridView_GlobalVariable.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dataGridView_GlobalVariable.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(31, 35, 41);
            dataGridView_GlobalVariable.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView_GlobalVariable.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 244, 255);
            dataGridView_GlobalVariable.DefaultCellStyle.SelectionForeColor = Color.FromArgb(22, 119, 255);
            dataGridView_GlobalVariable.DefaultCellStyle.Font = Font;

            dataGridView_GlobalVariable.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ID",
                DataPropertyName = "ID",
                HeaderText = "ID",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                ReadOnly = true,
            });

            dataGridView_GlobalVariable.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Name",
                DataPropertyName = "Name",
                HeaderText = "名称",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                ReadOnly = true,
            });

            dataGridView_GlobalVariable.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Describe",
                DataPropertyName = "Describe",
                HeaderText = "描述",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
            });



            dataGridView_GlobalVariable.Columns.Add(new DataGridViewComboBoxColumn
            {
                Name = "DataType",
                DataPropertyName = "DataType",
                HeaderText = "数据类型",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
                DataSource = Enum.GetValues(typeof(DataTypeEnum)).Cast<DataTypeEnum>()
                                                                    .Select(e => new
                                                                    {
                                                                        Text = e.ToString(),
                                                                        Value = (int)e
                                                                    }).ToList(),

                DisplayMember = "Text",  // 界面显示枚举文字
                ValueMember = "Value",   // 绑定后台int数值，和DataTable匹配
                ReadOnly = true,
            });

            dataGridView_GlobalVariable.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Value",
                DataPropertyName = "Value",
                HeaderText = "数值",
                Width = 100,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter },
            });
        }
        private DataTable BuildTable_GlobalVariableitem(List<GlobalVariableItem> l_GlobalVariable)
        {

            DataTable table = new DataTable();
            table.Columns.Add("ID");
            table.Columns.Add("Name");
            table.Columns.Add("Describe");
            table.Columns.Add("DataType", typeof(int));
            table.Columns.Add("Value");

            foreach (var row in l_GlobalVariable)
            {
                table.Rows.Add(
                    row.ID,
                    row.Name,
                    row.Describe,
                    row.DataType,
                    row.Value);
            }

            return table;
        }
        private void GetBuildTable_GlobalVariableitem(List<GlobalVariableItem> l_GlobalVariable)
        {
            try
            {
                for (int i = 0; i < dataGridView_GlobalVariable.Rows.Count; i++)
                {
                    GlobalVariableItem globalVariableItem = new GlobalVariableItem();

                    l_GlobalVariable[i].ID = Convert.ToInt32(dataGridView_GlobalVariable.Rows[i].Cells[0].Value);
                    l_GlobalVariable[i].Name = Convert.ToString(dataGridView_GlobalVariable.Rows[i].Cells[1].Value);
                    l_GlobalVariable[i].Describe = Convert.ToString(dataGridView_GlobalVariable.Rows[i].Cells[2].Value);

                    //校验数据类型是否和选择数据类型一致
                    globalVariableItem.DataType = (DataTypeEnum)dataGridView_GlobalVariable.Rows[i].Cells[3].Value;
                    globalVariableItem.Value = Convert.ToString(dataGridView_GlobalVariable.Rows[i].Cells[4].Value);
                    if (CheckInputData.CheckInputDataType(globalVariableItem.Value, globalVariableItem.DataType))
                    {
                        l_GlobalVariable[i].DataType = globalVariableItem.DataType;
                        l_GlobalVariable[i].Value = globalVariableItem.Value;
                    }
                    else
                    {
                        string showMessage = $"行【{l_GlobalVariable[i].ID}】\r\n数据类型和数值格式不匹配";
                        MessageBox.Show(showMessage, "取消保存");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void UpdateTable_GlobalVariableitem(List<GlobalVariableItem> l_GlobalVariable)
        {
            if (l_GlobalVariable != null)
            {
                dataGridView_GlobalVariable.DataSource = BuildTable_GlobalVariableitem(l_GlobalVariable);
            }
        }
        #endregion

        #region 参数显示与获取
        private void CraftItem_SelectIndexChanged(object sender, IntEventArgs e)
        {
            ParamAndView_Select(true);
        }
        private void ParamAndView_Select(bool showOrSave)
        {
            int index = CraftItem.SelectIndex;
            //显示参数
            if (showOrSave)
            {
                if (VM.M_CraftParam.Crafts.Count > index)
                {
                    currentCamCraft = VM.M_CraftParam.Crafts[index];
                }

                //显示对应工艺
                tabPage1.Visible = false;
                tabPage3.Visible = false;
                tabPage2.Visible = false;
                tabPage4.Visible = false;
                if (index == 0 || index == 1)
                {
                    //tabPage3.Visible = true;                
                    //UpdateTable_DefectCrafts(VM.M_CraftParam.Crafts[index].l_DefectCraft);

                    tabPage4.Visible = true;
                    UpdateTable_GlobalVariableitem(VM.M_CraftParam.Crafts[index].M_GlobalVariable.l_GlobalVariableItems);

                    tab_CraftTitle.SelectTab(tabPage4);
                }
                else if (index == 2)
                {
                    tabPage4.Visible = true;
                    UpdateTable_GlobalVariableitem(VM.M_CraftParam.Crafts[index].M_GlobalVariable.l_GlobalVariableItems);

                    tab_CraftTitle.SelectTab(tabPage4);
                }
                else if (index == 3 | index == 4 || index == 5)
                {

                    //tabPage1.Visible = true;                                           
                    //UpdateTable_SizeCraft(VM.M_CraftParam.Crafts[index].l_SizeCraft);

                    //tabPage3.Visible = true;
                    //UpdateTable_DefectCrafts(VM.M_CraftParam.Crafts[index].l_DefectCraft);

                    tabPage4.Visible = true;
                    UpdateTable_GlobalVariableitem(VM.M_CraftParam.Crafts[index].M_GlobalVariable.l_GlobalVariableItems);

                    tab_CraftTitle.SelectTab(tabPage4);
                }
                else if (index == 6)
                {
                    tabPage3.Visible = true;
                    UpdateTable_DefectCrafts(dataGridView_Defect, VM.M_CraftParam.l_DefectCraftStation_1);

                    tab_CraftTitle.SelectTab(tabPage3);
                }
                else if (index == 7)
                {
                    tabPage2.Visible = true;
                    UpdateTable_SizeCraftStation3(VM.M_CraftParam.l_SizeCraftStation_3);

                    tabPage3.Visible = true;
                    UpdateTable_DefectCrafts(dataGridView_Defect, VM.M_CraftParam.l_DefectCraftStation_3);

                    tab_CraftTitle.SelectTab(tabPage2);
                }
                else if (index == 8)
                {
                    tabPage4.Visible = true;
                    tab_CraftTitle.SelectTab(tabPage4);
                    UpdateTable_GlobalVariableitem(VM.M_CraftParam.M_GlobalVariable.l_GlobalVariableItems);
                }
            }
            //获取参数
            else
            {
                if (index == 0 || index == 1)
                {
                    //GetBuildTable_DefectCraft(VM.M_CraftParam.Crafts[index].l_DefectCraft);
                    GetBuildTable_GlobalVariableitem(VM.M_CraftParam.Crafts[index].M_GlobalVariable.l_GlobalVariableItems);
                }
                else if (index == 2)
                {
                    GetBuildTable_GlobalVariableitem(VM.M_CraftParam.Crafts[index].M_GlobalVariable.l_GlobalVariableItems);
                }
                else if (index == 3 | index == 4 || index == 5)
                {
                    //GetBuildTable_SizeCraft(VM.M_CraftParam.Crafts[index].l_SizeCraft);
                    //GetBuildTable_DefectCraft(VM.M_CraftParam.Crafts[index].l_DefectCraft);
                    GetBuildTable_GlobalVariableitem(VM.M_CraftParam.Crafts[index].M_GlobalVariable.l_GlobalVariableItems);
                }
                else if (index == 6)
                {
                    GetBuildTable_DefectCraft(dataGridView_Defect, VM.M_CraftParam.l_DefectCraftStation_1);
                }
                else if (index == 7)
                {
                    GetBuildTable_SizeCraftStation3(VM.M_CraftParam.l_SizeCraftStation_3);
                    GetBuildTable_DefectCraft(dataGridView_Defect, VM.M_CraftParam.l_DefectCraftStation_3);
                }
                else if (index == 8)
                {
                    GetBuildTable_GlobalVariableitem(VM.M_CraftParam.M_GlobalVariable.l_GlobalVariableItems);
                }
            }

        }
        #endregion

    }
}
