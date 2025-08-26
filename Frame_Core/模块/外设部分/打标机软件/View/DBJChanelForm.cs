using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    public partial class DBJChanelForm : Form
    {
        DBJCardManager ClassDBJCardManager;
       // private readonly ZoneManager _manager = new();
        private readonly DataGridView _dgv =new DataGridView() { Dock = DockStyle.Fill };
        TableLayoutPanel table = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        private readonly NumericUpDown _nud =new NumericUpDown
        {
            Minimum = 0,
            Maximum = 12,
            Value = 12,
            Dock = DockStyle.Top,
            Enabled = false,
            
            
        };

        public DBJChanelForm(DBJCardManager _DBJCardManager)
        {
            InitializeComponent();
            ClassDBJCardManager=_DBJCardManager;
            Text = "涂布分区打标通道管理";
            Size = new Size(600, 450);

            _nud.Value = _DBJCardManager.Zones().Count();

            _nud.ValueChanged += (_, __) => RebuildZones();


            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2
            };
            table.RowStyles.Add(new RowStyle(SizeType.Absolute, 30)); // 固定顶部高度
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // 剩余空间

            table.Controls.Add(_nud, 0, 0);   // 顶部行
            table.Controls.Add(_dgv, 0, 1);   // 填充行
            Controls.Add(table);
            //Controls.Add(_nud);
            //Controls.Add(_dgv);

            InitGrid();
            RebuildZones();

     
        }

        private void InitGrid()
        {
            _dgv.AutoGenerateColumns = false;
            _dgv.AllowUserToAddRows = false;
            _dgv.RowHeadersVisible = false;

            _dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ZoneNumber",
                HeaderText = "分区号",
                Width = 80,
                ReadOnly = true
            });

       

            _dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ChannelsText",
                HeaderText = "打标通道",
                DataPropertyName = "Channels",   // ← 关键
                Width = 260,
                ReadOnly = true
            });

            _dgv.Columns.Add(new DataGridViewButtonColumn
            {
                Text = "编辑",
                UseColumnTextForButtonValue = true,
                Width = 80
            });




            _dgv.CellFormatting += (_, e) =>
            {
                if (_dgv.Columns[e.ColumnIndex].Name == "ChannelsText" &&
                    e.Value is BindingList<MarkerChannel> list)
                {
                    e.Value = "[" + string.Join(",", list.Select(c => c.Id)) + "]";
                    e.FormattingApplied = true;
                }
            };

            _dgv.CellContentClick += (_, e) =>
            {
                if (e.RowIndex >= 0 && _dgv.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
                {
                    DBJCardParam dBJCardParam= (DBJCardParam)ClassDBJCardManager.tcpClientParam;
                    var zone = dBJCardParam.Zones[e.RowIndex];
                    using (ChannelEditorForm dlg = new ChannelEditorForm(zone)) 
                    {
                        dlg.ShowDialog();
                        _dgv.InvalidateRow(e.RowIndex);
                    }
                 
                }
            };
        }

        private void RebuildZones()
        {
            DBJCardManager dBJCardManager=ClassDBJCardManager;
            dBJCardManager.SetZoneCount((int)_nud.Value);
            BindGrid();
        }

        private void BindGrid() => _dgv.DataSource = ((DBJCardManager)(ClassDBJCardManager)).Zones();
    }
}
