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

using YYSD.UICore.Control;

namespace Frame_Core
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using Frame_Core.tools;

    public partial class FrmCsvViewer : Form
    {
        private readonly string _rootPath;
        private readonly ImageList _imgList = new ImageList();

        private const int ICON_FOLDER = 0;
        private const int ICON_CSV = 1;

     

   



        public FrmCsvViewer(string rootPath)
        {
            InitializeComponent();
            _rootPath = rootPath;
            Text = $"CSV 浏览器 – {Path.GetFileName(rootPath)}";
            Load += FrmCsvViewer_Load;
            treeView1.BeforeExpand += (s, e) => { /* 留空 */ };
        }

        private void FrmCsvViewer_Load(object sender, EventArgs e)
        {

            InitImageList();
            ApplyPrettyUI();
            BuildFullTreeExpandTwoLevels();
    
        }

        #region 图标绘制（纯代码，无依赖）
        private void InitImageList()
        {
            _imgList.ImageSize = new Size(16, 16);
            _imgList.Images.Add(MakeFolderIcon()); // 0
            _imgList.Images.Add(MakeDocIcon());    // 1
            treeView1.ImageList = _imgList;
        }

        private static Bitmap MakeFolderIcon()
        {
            var bmp = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.FillRectangle(Brushes.Khaki, 1, 5, 14, 10);
                g.FillRectangle(Brushes.Khaki, 2, 2, 10, 4);
                g.DrawRectangle(Pens.DarkKhaki, 1, 5, 13, 9);
                g.DrawRectangle(Pens.DarkKhaki, 2, 2, 9, 3);
            }
            return bmp;
        }

        private static Bitmap MakeDocIcon()
        {
            var bmp = new Bitmap(16, 16);
            using (var g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.Transparent);
                g.FillRectangle(Brushes.White, 2, 2, 12, 12);
                g.FillPolygon(Brushes.LightGray, new[] { new Point(12, 2), new Point(14, 2), new Point(14, 4) });
                g.DrawRectangle(Pens.Gray, 2, 2, 11, 11);
                g.DrawLine(Pens.Black, 4, 6, 10, 6);
                g.DrawLine(Pens.Black, 4, 9, 10, 9);
            }
            return bmp;
        }

  
        #endregion

        #region UI 美化
        private void ApplyPrettyUI()
        {
            BackColor = Color.FromArgb(246, 246, 246);
            splitContainer1.BackColor = BackColor;

            treeView1.BorderStyle = BorderStyle.None;
            treeView1.BackColor = Color.White;
            treeView1.ForeColor = Color.FromArgb(64, 64, 64);
            treeView1.ItemHeight = 22;
            treeView1.LineColor = Color.LightGray;

            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.BackgroundColor = Color.White;
            dataGridView1.DefaultCellStyle.Font = new Font("微软雅黑", 10F);
            dataGridView1.DefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(184, 215, 255);
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.Black;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(57, 111, 168);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersHeight = 32;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.RowTemplate.Height = 28;
            dataGridView1.EnableHeadersVisualStyles = false;

            // ★ 自动变宽
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            numericUpDown1.Font = new Font("微软雅黑", 9F);
            label1.Font = numericUpDown1.Font;
        }
        #endregion

        #region 业务逻辑（预扫+展开到二级+图标）
        private void BuildFullTreeExpandTwoLevels()
        {
            int days = (int)numericUpDown1.Value;
            DateTime minDate = DateTime.Today.AddDays(-days);

            treeView1.Nodes.Clear();
            if (!Directory.Exists(_rootPath)) return;

            var rootNode = treeView1.Nodes.Add(Path.GetFileName(_rootPath));
            rootNode.Tag = _rootPath;
            BuildAllNodes(rootNode, _rootPath, minDate);

            rootNode.Expand();
            foreach (TreeNode lvl1Node in rootNode.Nodes)
                lvl1Node.Expand();
        }

        private void BuildAllNodes(TreeNode parentNode, string dir, DateTime minDate)
        {
            foreach (var subDir in Directory.GetDirectories(dir).OrderBy(d => d))
            {
                var subNode = parentNode.Nodes.Add(Path.GetFileName(subDir));
                subNode.Tag = subDir;
                subNode.ImageIndex = ICON_FOLDER;
                subNode.SelectedImageIndex = ICON_FOLDER;
                BuildAllNodes(subNode, subDir, minDate);
            }

            foreach (var file in Directory.GetFiles(dir, "*.csv", SearchOption.TopDirectoryOnly)
                                          .Where(f => File.GetLastWriteTime(f) >= minDate)
                                          .OrderBy(f => f))
            {
                var fileNode = parentNode.Nodes.Add(Path.GetFileName(file));
                fileNode.Tag = file;
                fileNode.ImageIndex = ICON_CSV;
                fileNode.SelectedImageIndex = ICON_CSV;
            }
        }
        #endregion

        /* CSV 内容加载 */
        private async void TreeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag is string file && File.Exists(file))
            {
               // dataGridView1.Visible = false;
                try
                {
                    var data = await Task.Run(() =>
                    {
                        var lines = File.ReadAllLines(file, System.Text.Encoding.Default);
                        return lines.Select(l => l.Split(',')).ToList();
                    });

                    if (data.Count == 0) return;

                    dataGridView1.Columns.Clear();
                    foreach (var col in data[0])
                        dataGridView1.Columns.Add(col, col);
                    for (int i = 1; i < data.Count; i++)
                        dataGridView1.Rows.Add(data[i]);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("加载失败：" + ex.Message);
                }
                finally
                {
         
                   // dataGridView1.Visible = true;
                }
            }
        }

        /* 近 N 天刷新 */
        private void NumericUpDown1_ValueChanged(object sender, EventArgs e) => BuildFullTreeExpandTwoLevels();
    }
}
