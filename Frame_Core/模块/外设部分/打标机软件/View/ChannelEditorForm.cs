using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualBasic;
using System.Windows.Forms;


namespace Frame_Core
{
    public partial class ChannelEditorForm : Form
    {
        private readonly CoatingZone _zone;
        // 1. 预先生成 0~12 的可用 Id 列表
        private IEnumerable<int> FreeIds =>
            Enumerable.Range(1, 12).Except(_zone.Channels.Select(c => c.Id));

        private readonly ListBox _list = new ListBox()
        {
            Dock = DockStyle.Fill,
            SelectionMode = SelectionMode.MultiExtended
        };



        public ChannelEditorForm(CoatingZone zone)
        {
            _zone = zone;

            Text = $"分区 {zone.ZoneNumber} - 打标通道";
            Size = new Size(320, 300);
            StartPosition = FormStartPosition.CenterParent;

            RefreshList();

            var addBtn = new Button { Text = "添加", Dock = DockStyle.Bottom };
            var delBtn = new Button { Text = "删除", Dock = DockStyle.Bottom };
            var okBtn = new Button { Text = "关闭", Dock = DockStyle.Bottom };


            // 2. 添加通道
            addBtn.Click += (_, __) =>
            {
                string input =Interaction.InputBox("请输入通道号 (1-12):", "添加通道", "0");
                if (!int.TryParse(input, out int id) || id < 1 || id > 12)
                {
                    MessageBox.Show("只能输入 1~12 的整数！");
                    return;
                }

                if (_zone.Channels.Any(c => c.Id == id))
                {
                    MessageBox.Show($"通道 {id} 已存在！");
                    return;
                }

                _zone.Channels.Add(new MarkerChannel
                {
                    Id = id,
                    Name = $"CH{id}"
                });
                RefreshList();
            };

            delBtn.Click += (_, __) =>
            {
                if (_list.SelectedItem is MarkerChannel ch)
                {
                    _zone.Channels.Remove(ch);
                    RefreshList();
                }
            };

            _list.DoubleClick += (_, __) =>
            {
                if (_list.SelectedItem is MarkerChannel ch)
                {
                    string input = Interaction.InputBox("新通道号 (1-12):", "修改", ch.Id.ToString());
                    if (!int.TryParse(input, out int newId) || newId < 1 || newId > 12)
                    {
                        MessageBox.Show("只能输入 1~12 的整数！");
                        return;
                    }
                    if (newId != ch.Id && _zone.Channels.Any(c => c.Id == newId))
                    {
                        MessageBox.Show($"通道 {newId} 已存在！");
                        return;
                    }

                    ch.Id = newId;
                    ch.Name = $"CH{newId}";
                    RefreshList();
                }
            };

            okBtn.Click += (_, __) => DialogResult = DialogResult.OK;

            Controls.AddRange(new Control[] { _list, addBtn, delBtn, okBtn });
        }

        private void RefreshList()
        {
            _list.DataSource = null;
            _list.DataSource = _zone.Channels;
            _list.DisplayMember = "Name";
        }
    }
}
