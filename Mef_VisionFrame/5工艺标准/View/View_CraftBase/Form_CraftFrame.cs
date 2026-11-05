using AntdUI;
using Frame_Core;
using LogTool;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    public partial class Form_CraftFrame : Form
    {
        VM_Craft VM = null;
        FormCraftBase M_FormCraftBase = null;
        CraftBase CurrentCraftBase = new CraftBase();
        List<CraftRow> recipeInfo;

        public Form_CraftFrame(SystemInfo _systemInfo, CraftBase craftBase, string craftBasePath)
        {
            InitializeComponent();
            M_FormCraftBase = Craft_ViewService.GetView(M_FormCraftBase, _systemInfo.CraftType);
            VM = new VM_Craft(_systemInfo, craftBase, craftBasePath);
            InitMaterialSelect();
            InitTableColumns();

        }
        private void Form_CraftFrame_Load(object sender, EventArgs e)
        {
            comb_materialCraf.SelectedIndex = (int)VM.systemInfo.CraftType;
        }
        #region 私有工具
        private void InitMaterialSelect()
        {
            comb_materialCraf.Items.Clear();
            foreach (var name in Enum.GetValues(typeof(MaterialCrafEnum)))
            {
                string temp = ((MaterialCrafEnum)name).GetDescription();
                comb_materialCraf.Items.Add(new SelectItem(name));
            }
        }
        private void InitTableColumns()
        {
            tableCrafts.Columns.Clear();
            tableCrafts.AutoGenerateColumns = false;

            tableCrafts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            tableCrafts.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(31, 35, 41);
            tableCrafts.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            tableCrafts.DefaultCellStyle.SelectionBackColor = Color.FromArgb(230, 244, 255);
            tableCrafts.DefaultCellStyle.SelectionForeColor = Color.FromArgb(22, 119, 255);
            tableCrafts.DefaultCellStyle.Font = Font;

            tableCrafts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CraftName",
                DataPropertyName = "CraftName",
                HeaderText = "配方名",
                Width = 270,
                MinimumWidth = 220,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });

            tableCrafts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                DataPropertyName = "Status",
                HeaderText = "状态",
                Width = 80,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            tableCrafts.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ActionText",
                DataPropertyName = "ActionText",
                HeaderText = "操作",
                Width = 60,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });
        }

        private DataTable BuildCraftTable(List<CraftRow> rows)
        {
            DataTable table = new DataTable();
            table.Columns.Add("CraftName", typeof(string));
            table.Columns.Add("Status", typeof(string));
            table.Columns.Add("ActionText", typeof(string));

            foreach (CraftRow row in rows)
            {
                table.Rows.Add(row.CraftName, row.Status, row.ActionText);
            }


            return table;
        }
        private CraftRow GetSelectRecipeInfo(int rowIndex)
        {
            if (recipeInfo != null)
                if (rowIndex >= 0 && rowIndex < recipeInfo.Count)
                {
                    return recipeInfo[rowIndex];
                }

            return null;
        }
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
        private void ShowCraftContextMenu(CraftRow craftRow)
        {
            bool hasSelected = (craftRow != null);
            bool isCurrent = craftRow?.IsCurrent ?? false;
            string selectRecipeName = craftRow?.CraftName ?? "";
            IContextMenuStripItem[] items =
            {
                new ContextMenuStripItem("新建配方") { Tag = "create" },
                new ContextMenuStripItem("复制配方") { Tag = "copy", Enabled = hasSelected },
                new ContextMenuStripItem("重命名配方") { Tag = "rename", Enabled = hasSelected&& !isCurrent},
                new ContextMenuStripItem("移除配方") { Tag = "remove", Enabled = hasSelected && !isCurrent, Fore = Color.Red },
                new ContextMenuStripItemDivider(),
                new ContextMenuStripItem("打开配方文件位置") { Tag = "open" }
            };

            AntdUI.ContextMenuStrip.open(tableCrafts, item =>
            {
                string tag = item.Tag as string;

                try
                {
                    if (tag == "create")
                    {
                        if (!ShowNameDialog("新建配方", "", out string newName)) return;
                        VM.CreateRecipe((MaterialCrafEnum)comb_materialCraf.SelectedIndex, newName);
                        Update_TableCrafts((MaterialCrafEnum)comb_materialCraf.SelectedIndex, VM.systemInfo.RecipeName);
                    }
                    else if (tag == "copy")
                    {
                        bool result = VM.CopyRecipe((MaterialCrafEnum)comb_materialCraf.SelectedIndex, selectRecipeName, out string newName);
                        Update_TableCrafts((MaterialCrafEnum)comb_materialCraf.SelectedIndex, VM.systemInfo.RecipeName);

                        if (result)
                        {
                            string oldProcessPath = CraftExtension.GetProcessFilePath(Project.Instance.paramPathSet.CraftPath, (MaterialCrafEnum)comb_materialCraf.SelectedIndex, selectRecipeName);
                            string oldInstanceParamPath = CraftExtension.GetProcessInstanceFilePath(Project.Instance.paramPathSet.CraftPath, (MaterialCrafEnum)comb_materialCraf.SelectedIndex, selectRecipeName);
                            string newProcessPath = CraftExtension.GetProcessFilePath(Project.Instance.paramPathSet.CraftPath, (MaterialCrafEnum)comb_materialCraf.SelectedIndex, newName);
                            string newInstanceParamPath = CraftExtension.GetProcessInstanceFilePath(Project.Instance.paramPathSet.CraftPath, (MaterialCrafEnum)comb_materialCraf.SelectedIndex, newName);

                            Project.Instance.Process流程插件管理者.CopyRecipe(oldProcessPath, oldInstanceParamPath, newProcessPath, newInstanceParamPath);
                        }

                    }
                    else if (tag == "rename")
                    {
                        if (isCurrent)
                        {
                            MessageBox.Show("不能重命名正在生产的配方！");
                            return;
                        }

                        if (!ShowNameDialog("重命名配方", selectRecipeName, out string newName)) return;
                        VM.RenameRecipe((MaterialCrafEnum)comb_materialCraf.SelectedIndex, selectRecipeName, newName);
                        Update_TableCrafts((MaterialCrafEnum)comb_materialCraf.SelectedIndex, VM.systemInfo.RecipeName);

                        string oldProcessPath = CraftExtension.GetProcessFilePath(Project.Instance.paramPathSet.CraftPath, (MaterialCrafEnum)comb_materialCraf.SelectedIndex, selectRecipeName);
                        string oldInstanceParamPath = CraftExtension.GetProcessInstanceFilePath(Project.Instance.paramPathSet.CraftPath, (MaterialCrafEnum)comb_materialCraf.SelectedIndex, selectRecipeName);
                        string newProcessPath = CraftExtension.GetProcessFilePath(Project.Instance.paramPathSet.CraftPath, (MaterialCrafEnum)comb_materialCraf.SelectedIndex, newName);
                        string newInstanceParamPath = CraftExtension.GetProcessInstanceFilePath(Project.Instance.paramPathSet.CraftPath, (MaterialCrafEnum)comb_materialCraf.SelectedIndex, newName);
                        Project.Instance.Process流程插件管理者.RenameRecipe(oldProcessPath, oldInstanceParamPath, newProcessPath, newInstanceParamPath);
                    }
                    else if (tag == "remove")
                    {
                        if (isCurrent)
                        {
                            MessageBox.Show("不能删除正在生产的配方！");
                            return;
                        }

                        try
                        {
                            VM.DeleteRecipe((MaterialCrafEnum)comb_materialCraf.SelectedIndex, selectRecipeName);
                            Update_TableCrafts((MaterialCrafEnum)comb_materialCraf.SelectedIndex, VM.systemInfo.RecipeName);

                            string processPath = CraftExtension.GetProcessFileBasePath(Project.Instance.paramPathSet.CraftPath, (MaterialCrafEnum)comb_materialCraf.SelectedIndex, selectRecipeName);
                            if (Directory.Exists(processPath))
                            {
                                Directory.Delete(processPath,true);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"删除配方失败：{ex.Message}");
                        }

                        MessageBox.Show($"配方【{selectRecipeName}】已成功删除");
                    }
                    else if (tag == "open")
                    {
                        VM.OpenRecipeFolder((MaterialCrafEnum)comb_materialCraf.SelectedIndex);
                    }
                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Error("异常", ex);
                }

            }, items, 0);
        }

        private void Update_TableCrafts(MaterialCrafEnum craftType, string currentRecipeName)
        {
            string keyword = inputSearch.Text == null ? "" : inputSearch.Text.Trim();
            recipeInfo = VM.GetRecipeInfo(craftType, currentRecipeName, keyword);
            if (recipeInfo != null)
            {
                tableCrafts.DataSource = BuildCraftTable(recipeInfo);
            }
        }
        #endregion

        private void comb_materialCraf_SelectedIndexChanged(object sender, AntdUI.IntEventArgs e)
        {
            if (comb_materialCraf.SelectedIndex < 0) return;
            MaterialCrafEnum materialCrafEnum = (MaterialCrafEnum)comb_materialCraf.SelectedIndex;
            Update_TableCrafts(materialCrafEnum, VM.systemInfo.RecipeName);

            if (tableCrafts.CurrentCell != null && tableCrafts.Rows.Count > 0)
            {
                // 清空所有原有选中行
                //tableCrafts.ClearSelection();

                // 设置目标行为选中
                tableCrafts.Rows[tableCrafts.CurrentCell.RowIndex].Selected = true;

                // 设为当前激活单元格（高亮聚焦）
                //tableCrafts.CurrentCell = tableCrafts.Rows[tableCrafts.CurrentCell.RowIndex].Cells[0];

                M_FormCraftBase = Craft_ViewService.GetView(M_FormCraftBase, materialCrafEnum);
                panel_Main.Controls.Clear();
                panel_Main.Controls.Add(M_FormCraftBase);
                M_FormCraftBase.Show();

                CraftRow row = GetSelectRecipeInfo(tableCrafts.CurrentCell.RowIndex);
                Update_ParamToView(materialCrafEnum, row.CraftName);
            }
            else
            {
                panel_Main.Controls.Clear();
            }

        }
        private void tableCrafts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            CraftRow row = GetSelectRecipeInfo(e.RowIndex);
            if (CurrentCraftBase.RecipeName == VM.systemInfo.RecipeName) return;


            CurrentCraftBase = Craft_ModelService.GetCraft(VM.CraftBasePath, (MaterialCrafEnum)comb_materialCraf.SelectedIndex, row.CraftName);
            string str = "";
            str += $"是否切换配方?";
            str += $"\r\n 当前工艺：{VM.M_craftBase.CraftType}，当前配方：{VM.M_craftBase.RecipeName},工艺极性：{VM.M_craftBase.Electrode}";
            str += $"\r\n 切换工艺：{CurrentCraftBase.CraftType}，当前配方：{CurrentCraftBase.RecipeName},工艺极性：{CurrentCraftBase.Electrode}";

            if (System.Windows.Forms.MessageBox.Show(str, "请停机状态切换,切换后软件自动关闭", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Project.Instance.craftBase = CurrentCraftBase;
                VM.systemInfo.RecipeName = CurrentCraftBase.RecipeName;
                VM.systemInfo.CraftType = CurrentCraftBase.CraftType;
                VM.systemInfo.Save();
                this.Hide();

                // this.Close(); //关闭工艺设置界面
                Environment.Exit(0); //关闭整个工程
            }
        }

        private void tableCrafts_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = e.RowIndex;
            CraftRow row = GetSelectRecipeInfo(index);
            if (e.Button == MouseButtons.Right)
            {
                ShowCraftContextMenu(row);
            }
            else if (e.Button == MouseButtons.Left && index >= 0)
            {
                if (CurrentCraftBase.RecipeName == row.CraftName) return;

                MaterialCrafEnum materialCrafEnum = (MaterialCrafEnum)comb_materialCraf.SelectedIndex;
                Update_ParamToView(materialCrafEnum, row.CraftName);
            }
        }

        /// <summary>
        /// 加载工艺参数到窗体
        /// </summary>
        public void Update_ParamToView(MaterialCrafEnum craftType, string recipeName)
        {
            CurrentCraftBase = Craft_ModelService.GetCraft(VM.CraftBasePath, craftType, recipeName);

            M_FormCraftBase.Init(CurrentCraftBase);
            M_FormCraftBase.ParamToView();
        }

        private void btn_SaveCurrentCraft_Click(object sender, EventArgs e)
        {
            try
            {
                M_FormCraftBase.ViewToParam();
                CurrentCraftBase.SaveCraft(VM.CraftBasePath, CurrentCraftBase.CraftType, CurrentCraftBase.RecipeName);
                VM.M_craftBase = Craft_ModelService.GetCraft(VM.CraftBasePath, VM.systemInfo.CraftType, VM.systemInfo.RecipeName);
                Project.Instance.craftBase = VM.M_craftBase;
                MessageBox.Show("保存成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存失败");
            }
        }

        private void inputSearch_TextChanged(object sender, EventArgs e)
        {
            Update_TableCrafts((MaterialCrafEnum)comb_materialCraf.SelectedIndex, VM.systemInfo.RecipeName);
        }
    }
}
