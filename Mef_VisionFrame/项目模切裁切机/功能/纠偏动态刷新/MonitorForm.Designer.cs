using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Mef_VisionFrame
{
    partial class MonitorForm
    {
        private FlowLayoutPanel Pieces = null; private FlowLayoutPanel CutPieces = null; private DataGridView BeforeTable = null; private DataGridView AfterTable = null; private Label CurrentValue = null; private Label RunLengthValue = null; private Label PieWidthValue = null; private Label PieCountValue = null;
        private void InitializeComponent()
        {
            Text = "极片裁切 · 纠偏监控台"; Width = 1180; Height = 680; BackColor = Color.FromArgb(9, 17, 31); Padding = new Padding(14);
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 3, ColumnCount = 2, BackColor = Color.FromArgb(9, 17, 31) }; root.RowStyles.Add(new RowStyle(SizeType.Absolute, 62)); root.RowStyles.Add(new RowStyle(SizeType.Percent, 62)); root.RowStyles.Add(new RowStyle(SizeType.Percent, 38)); root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70)); root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            var header = new Panel { Dock = DockStyle.Fill }; header.Controls.Add(new Label { Text = "极片裁切 · 纠偏监控台", ForeColor = Color.FromArgb(114, 216, 255), Font = new Font("Microsoft YaHei", 20, FontStyle.Bold), AutoSize = true, Location = new Point(4, 2) }); header.Controls.Add(new Label { Text = "CUTTING & CORRECTION LIVE MONITOR", ForeColor = Color.FromArgb(109, 137, 166), AutoSize = true, Location = new Point(6, 38) }); var parameters = new FlowLayoutPanel { Dock = DockStyle.Right, Width = 520, Padding = new Padding(0, 8, 0, 0) }; parameters.Controls.Add(ParameterCard("上片走的位置", out RunLengthValue)); parameters.Controls.Add(ParameterCard("片宽", out PieWidthValue)); parameters.Controls.Add(ParameterCard("片数", out PieCountValue)); header.Controls.Add(parameters); root.Controls.Add(header, 0, 0); root.SetColumnSpan(header, 2);
            // 极片数量超过可视宽度时，保持卡片尺寸并显示水平滚动条。
            Pieces = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, WrapContents = false, FlowDirection = FlowDirection.LeftToRight, BackColor = Color.FromArgb(17, 29, 49), Padding = new Padding(8) }; CutPieces = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoScroll = true, WrapContents = false, FlowDirection = FlowDirection.LeftToRight, BackColor = Color.FromArgb(16, 53, 72), Padding = new Padding(7) }; BeforeTable = CreateTable(); AfterTable = CreateTable();
            var flowPanel = Box("工艺流程：切前队列倒序；切后最新两片", Color.FromArgb(17, 29, 49)); var flow = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, Padding = new Padding(8, 35, 8, 8) }; flow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62)); flow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 58)); flow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38)); flow.Controls.Add(Labeled("切前来料（倒序）", Pieces, Color.FromArgb(135, 182, 217)), 0, 0); flow.Controls.Add(new Label { Text = "裁切\r\n->", ForeColor = Color.FromArgb(82, 212, 255), TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill, Font = new Font("Microsoft YaHei", 12, FontStyle.Bold) }, 1, 0); flow.Controls.Add(Labeled("切后成品（两片倒序）", CutPieces, Color.FromArgb(126, 240, 197)), 2, 0); flowPanel.Controls.Add(flow); root.Controls.Add(flowPanel, 0, 1);
            var value = Box("当前纠偏值", Color.FromArgb(16, 53, 72)); CurrentValue = new Label { Dock = DockStyle.Top, Height = 76, ForeColor = Color.FromArgb(99, 242, 196), Font = new Font("Microsoft YaHei", 25, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft }; value.Controls.Add(new Label { Text = "数值来自传入的 CorrectPosDeal", ForeColor = Color.FromArgb(139, 168, 191), AutoSize = true, Location = new Point(14, 120) }); value.Controls.Add(CurrentValue); root.Controls.Add(value, 1, 1);
            var tables = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 }; tables.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); tables.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50)); var beforeBox = Box("切前表格（倒序）", Color.FromArgb(17, 29, 49)); BeforeTable.Location = new Point(10, 36); BeforeTable.Size = new Size(440, 180); BeforeTable.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; beforeBox.Controls.Add(BeforeTable); var afterBox = Box("切后表格（最新10片，倒序）", Color.FromArgb(17, 29, 49)); AfterTable.Location = new Point(10, 36); AfterTable.Size = new Size(440, 180); AfterTable.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right; afterBox.Controls.Add(AfterTable); tables.Controls.Add(beforeBox, 0, 0); tables.Controls.Add(afterBox, 1, 0); root.Controls.Add(tables, 0, 2); root.SetColumnSpan(tables, 2); Controls.Add(root);
        }
        private static Panel Box(string title, Color color) { var p = new Panel { Dock = DockStyle.Fill, BackColor = color, Margin = new Padding(0, 0, 10, 10), Padding = new Padding(10) }; p.Controls.Add(new Label { Text = title, ForeColor = Color.FromArgb(215, 231, 248), Font = new Font("Microsoft YaHei", 11, FontStyle.Bold), AutoSize = true, Location = new Point(10, 9) }); return p; }
        private static Panel ParameterCard(string title, out Label value) { var card = new Panel { Width = 160, Height = 45, Margin = new Padding(5, 0, 5, 0), BackColor = Color.FromArgb(17, 35, 58) }; card.Controls.Add(new Label { Text = title, ForeColor = Color.FromArgb(139, 170, 194), AutoSize = true, Location = new Point(8, 5) }); value = new Label { ForeColor = Color.FromArgb(232, 245, 255), Font = new Font("Microsoft YaHei", 10, FontStyle.Bold), AutoSize = true, Location = new Point(8, 23) }; card.Controls.Add(value); return card; }
        private static DataGridView CreateTable()
        {
            var table = new DataGridView
            { Dock = DockStyle.Fill, ReadOnly = true, AutoGenerateColumns = false, RowHeadersVisible = false, ScrollBars = ScrollBars.Both, BackgroundColor = Color.FromArgb(10, 21, 38), BorderStyle = BorderStyle.None, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None, EnableHeadersVisualStyles = false };
            table.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "序号", DataPropertyName = "FrameId", Width = 65 });
            table.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "数值", DataPropertyName = "Value", Width = 85 });
            table.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "CorrectPosChange", DataPropertyName = "CorrectPosChange", Width = 300 });
            table.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "DisplacementText", DataPropertyName = "DisplacementText", Width = 300 });
            table.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 53, 80);
            table.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(214, 235, 250); table.DefaultCellStyle.BackColor = Color.FromArgb(16, 34, 57);
            table.DefaultCellStyle.ForeColor = Color.White;
            table.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(13, 27, 48);
            return table;
        }
        private static Panel Labeled(string title, Control body, Color color) { var p = new Panel { Dock = DockStyle.Fill, Padding = new Padding(0, 25, 0, 0) }; p.Controls.Add(body); p.Controls.Add(new Label { Text = title, ForeColor = color, Dock = DockStyle.Top, Height = 23 }); return p; }
    }
}
