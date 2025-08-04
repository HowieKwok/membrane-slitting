using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

public static class TableLayoutPanelSerializer
{
    // 序列化当前TableLayoutPanel中的控件到字典
    public static Dictionary<string, Control> SerializeToDictionary(TableLayoutPanel tableLayoutPanel)
    {
        if (tableLayoutPanel == null)
            throw new ArgumentNullException(nameof(tableLayoutPanel));

        var controlsDictionary = new Dictionary<string, Control>();

        for (int i = 0; i < tableLayoutPanel.RowCount; i++)
        {
            if (i >= tableLayoutPanel.RowStyles.Count)
                break;

            // 第一列是Label（键）
            var keyControl = tableLayoutPanel.GetControlFromPosition(0, i) as Label;
            if (keyControl == null) continue;

            // 第二列是值控件
            var valueControl = tableLayoutPanel.GetControlFromPosition(1, i);
            if (valueControl == null) continue;

            controlsDictionary[keyControl.Text] = CloneControl(valueControl);
        }
        return controlsDictionary;
    }

    // 从字典反序列化到TableLayoutPanel
    public static void DeserializeToTable(Dictionary<string, Control> controlsDictionary, TableLayoutPanel tableLayoutPanel)
    {
        if (tableLayoutPanel == null)
            throw new ArgumentNullException(nameof(tableLayoutPanel));

        tableLayoutPanel.SuspendLayout();
        tableLayoutPanel.Controls.Clear();
        tableLayoutPanel.RowStyles.Clear();
  
        if (controlsDictionary == null)
        {
            tableLayoutPanel.RowCount = 0;
            tableLayoutPanel.ResumeLayout(true);
            return;
        }

        // 确保有2列
        tableLayoutPanel.ColumnCount = 2;
        tableLayoutPanel.ColumnStyles.Clear();
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // 第一列自动大小
        tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100F)); // 第二列填充剩余空间

        int rowIndex = 0;
        foreach (var kvp in controlsDictionary)
        {
            // 添加行样式（重要修复：确保每行高度自适应）
            tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            // 创建Label（键）
            var keyLabel = new Label
            {
                Text = kvp.Key,
                AutoSize = true,
                //Dock=DockStyle.Fill,
                Anchor = AnchorStyles.Left | AnchorStyles.Top, // 固定锚点
                Margin = new Padding(3, 6, 3, 6), // 统一边距
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            // 克隆值控件
            var valueControl = CloneControl(kvp.Value);
            //valueControl.Dock = DockStyle.Fill;
            valueControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top; // 水平拉伸

            // 特殊处理：最后一个控件取消底部边距
            if (rowIndex == controlsDictionary.Count - 1)
            {
                valueControl.Margin = new Padding(valueControl.Margin.Left,
                                                valueControl.Margin.Top,
                                                valueControl.Margin.Right,
                                                3); // 底部只保留3像素
            }

            // 添加到表格
            tableLayoutPanel.Controls.Add(keyLabel, 0, rowIndex);
            tableLayoutPanel.Controls.Add(valueControl, 1, rowIndex);

            rowIndex++;
        }

        tableLayoutPanel.RowCount = rowIndex;
        //tableLayoutPanel.Dock = DockStyle.Fill;
        tableLayoutPanel.HorizontalScroll.Value = 0;
        tableLayoutPanel.AutoScroll = true;
        tableLayoutPanel.ResumeLayout(true); // 重要：恢复布局并强制重绘
        


    }

    // 添加控件到字典和TableLayoutPanel
    public static Dictionary<string, Control> AddControl(Dictionary<string, Control> controlsDictionary, string key, Control valueControl, TableLayoutPanel tableLayoutPanel = null)
    {
        if (controlsDictionary == null)
            controlsDictionary = new Dictionary<string, Control>();

        if (controlsDictionary.ContainsKey(key))
        {
            throw new ArgumentException($"Key '{key}' already exists in the dictionary.");
        }

        var newDictionary = new Dictionary<string, Control>(controlsDictionary)
        {
            [key] = CloneControl(valueControl)
        };

        if (tableLayoutPanel != null)
        {
            DeserializeToTable(newDictionary, tableLayoutPanel);
        }

        return newDictionary;
    }

    // 从字典和TableLayoutPanel中移除控件
    public static Dictionary<string, Control> RemoveControl(Dictionary<string, Control> controlsDictionary, string key, TableLayoutPanel tableLayoutPanel = null)
    {
        if (controlsDictionary == null || !controlsDictionary.ContainsKey(key))
            return controlsDictionary;

        var newDictionary = new Dictionary<string, Control>(controlsDictionary);
        newDictionary.Remove(key);

        if (tableLayoutPanel != null)
        {
            DeserializeToTable(newDictionary, tableLayoutPanel);
        }

        return newDictionary;
    }

    // 更新字典中的控件
    public static Dictionary<string, Control> UpdateControl(Dictionary<string, Control> controlsDictionary, string key, Control newValueControl, TableLayoutPanel tableLayoutPanel = null)
    {
        if (controlsDictionary == null || !controlsDictionary.ContainsKey(key))
        {
            throw new KeyNotFoundException($"Key '{key}' not found in the dictionary.");
        }

        var newDictionary = new Dictionary<string, Control>(controlsDictionary)
        {
            [key] = CloneControl(newValueControl)
        };

        if (tableLayoutPanel != null)
        {
            DeserializeToTable(newDictionary, tableLayoutPanel);
        }

        return newDictionary;
    }
    //修改字典中的控件
    public static void ModifyControlDic(Dictionary<string, Control> controlsDictionary, string key, Control valueControl)
    {
        if (controlsDictionary.ContainsKey(key))
        {
            controlsDictionary[key] = CloneControl(valueControl);
        }
        else
        {
            controlsDictionary.Add(key, CloneControl(valueControl)); 
        }
 
    }


    // 深度克隆控件（简化版）
    private static Control CloneControl(Control source)
    {
        if (source == null) return null;

        Control clone;
        if (source is TextBox)
        {
            var textBox = new TextBox();
            textBox.Text = ((TextBox)source).Text;
            clone = textBox;
        }
        else if (source is ComboBox)
        {
            var comboBox = new ComboBox();
            comboBox.Items.AddRange(((ComboBox)source).Items.Cast<object>().ToArray());
            comboBox.SelectedIndex = ((ComboBox)source).SelectedIndex;
            clone = comboBox;
        }
        else if (source is CheckBox)
        {
            var checkBox = new CheckBox();
            checkBox.Checked = ((CheckBox)source).Checked;
            checkBox.Text = ((CheckBox)source).Text;
            clone = checkBox;
        }
        else if (source is NumericUpDown)
        {
            var numeric = new NumericUpDown();
            numeric.Value = ((NumericUpDown)source).Value;
            numeric.Maximum = ((NumericUpDown)source).Maximum;
            numeric.Minimum = ((NumericUpDown)source).Minimum;
            numeric.DecimalPlaces = ((NumericUpDown)source).DecimalPlaces;  
            clone = numeric;
        }
        else
        {
            // 默认处理
            clone = (Control)Activator.CreateInstance(source.GetType());
        }

        // 复制通用属性
        clone.Name = source.Name;
        //clone.Text = source.Text;
        //clone.Enabled = source.Enabled;
        //clone.Visible = source.Visible;
        //clone.Dock = source.Dock;
        //clone.Anchor = source.Anchor;
        //clone.Size = source.Size;
        //clone.Margin = source.Margin;
        //clone.Padding = source.Padding;

        return clone;
    }



    // 保存字典到文件
    public static void SaveToFile(Dictionary<string, Control> controlsDictionary, string filePath)
    {
        if (controlsDictionary == null)
            throw new ArgumentNullException(nameof(controlsDictionary));

        // 转换为可序列化的格式
        var serializableData = new SerializableControlDictionary();
        foreach (var kvp in controlsDictionary)
        {
            var control = new SerializableControl
            {
                ControlType = kvp.Value.GetType().AssemblyQualifiedName,
            };

            foreach (var prop in GetControlProperties(kvp.Value))
            {
                control.Properties.Add(new SerializableProperty
                {
                    Name = prop.Key,
                    Value = prop.Value
                });
            }

            serializableData.Add(kvp.Key, control);
        }

        // 序列化到XML
        var serializer = new XmlSerializer(typeof(SerializableControlDictionary));
        using (var writer = new StreamWriter(filePath))
        {
            serializer.Serialize(writer, serializableData);
        }
    }

    // 保存字典集合到文件
    public static void SaveLtDicToFile(List<Dictionary<string, Control>> ltcontrolsDictionary, string filePath)
    {
        if (ltcontrolsDictionary == null)
            throw new ArgumentNullException(nameof(ltcontrolsDictionary));

        // 转换为可序列化的格式
        var ltserializableData = new List<SerializableControlDictionary>();
        foreach (var controlsDictionary in ltcontrolsDictionary)
        {
            // 转换为可序列化的格式
            var serializableData = new SerializableControlDictionary();
            foreach (var kvp in controlsDictionary)
            {
                var control = new SerializableControl
                {
                    ControlType = kvp.Value.GetType().AssemblyQualifiedName,
                };

                foreach (var prop in GetControlProperties(kvp.Value))
                {
                    control.Properties.Add(new SerializableProperty
                    {
                        Name = prop.Key,
                        Value = prop.Value
                    });
                }
                serializableData.Add(kvp.Key, control);
            }
            ltserializableData.Add(serializableData);
        }
        // 序列化到XML
        var serializer = new XmlSerializer(typeof(List<SerializableControlDictionary>));
        using (var writer = new StreamWriter(filePath))
        {
            serializer.Serialize(writer, ltserializableData);
        }
    }


    // 从文件加载字典
    public static Dictionary<string, Control> LoadFromFile(string filePath, TableLayoutPanel tableLayoutPanel = null)
    {
        if (!File.Exists(filePath)) return null;

        // 反序列化XML
        var serializer = new XmlSerializer(typeof(SerializableControlDictionary));
        SerializableControlDictionary serializableData;
        using (var reader = new StreamReader(filePath))
        {
            serializableData = (SerializableControlDictionary)serializer.Deserialize(reader);
        }

        // 转换为控件字典
        var controlsDictionary = new Dictionary<string, Control>();
        foreach (var item in serializableData.Items)
        {
            var controlType = Type.GetType(item.Value.ControlType);
            if (controlType == null) continue;

            var control = (Control)Activator.CreateInstance(controlType);

            // 设置属性
            var properties = item.Value.Properties.ToDictionary(p => p.Name, p => p.Value);
            SetControlProperties(control, properties);

            controlsDictionary[item.Key] = control;
        }

        // 刷新界面（如果提供了TableLayoutPanel）
        if (tableLayoutPanel != null)
        {
            DeserializeToTable(controlsDictionary, tableLayoutPanel);
        }

        return controlsDictionary;
    }

    // 从文件加载字典集合
    public static List<Dictionary<string, Control>> LoadFromLtDicFile(string filePath)
    {
        if (!File.Exists(filePath)) return null;

        // 反序列化XML
        var serializer = new XmlSerializer(typeof(List<SerializableControlDictionary>));
        List<SerializableControlDictionary> ltserializableData;
        using (var reader = new StreamReader(filePath))
        {
            ltserializableData = (List<SerializableControlDictionary>)serializer.Deserialize(reader);
        }

        // 转换为控件字典
        var ltcontrolsDictionary = new List<Dictionary<string, Control>>();
        foreach (var serializableData in ltserializableData)
        {
            Dictionary<string, Control> controlsDictionary = new Dictionary<string, Control>();
            foreach (var item in serializableData.Items)
            {
                var controlType = Type.GetType(item.Value.ControlType);
                if (controlType == null) continue;

                var control = (Control)Activator.CreateInstance(controlType);

                // 设置属性
                var properties = item.Value.Properties.ToDictionary(p => p.Name, p => p.Value);
                SetControlProperties(control, properties);

                controlsDictionary[item.Key] = control;
            }
            ltcontrolsDictionary.Add(controlsDictionary);
        }
        return ltcontrolsDictionary;
    }


    // 获取控件的属性集合
    private static Dictionary<string, string> GetControlProperties(Control control)
    {
        var properties = new Dictionary<string, string>
        {
            ["Text"] = control.Text,
            ["Enabled"] = control.Enabled.ToString(),
            ["Visible"] = control.Visible.ToString(),
        };

        if (control is TextBox textBox)
        {
            properties["Text"] = textBox.Text;
        }
        else if (control is ComboBox comboBox)
        {
            properties["SelectedIndex"] = comboBox.SelectedIndex.ToString();
            properties["Items"] = string.Join("|", comboBox.Items.Cast<object>());
        }
        else if (control is CheckBox checkBox)
        {
            properties["Checked"] = checkBox.Checked.ToString();
        }
        else if (control is NumericUpDown numeric)
        {
            properties["Value"] = numeric.Value.ToString();
            properties.Add("DecimalPlaces", numeric.DecimalPlaces.ToString());
            properties.Add("Minimum", numeric.Minimum.ToString());
            properties.Add("Maximum", numeric.Maximum.ToString());
        }

        return properties;
    }

    // 设置控件的属性
    private static void SetControlProperties(Control control, Dictionary<string, string> properties)
    {
        if (properties.TryGetValue("Text", out var text))
            control.Text = text;

        if (properties.TryGetValue("Enabled", out var enabled))
            control.Enabled = bool.Parse(enabled);

        if (properties.TryGetValue("Visible", out var visible))
            control.Visible = bool.Parse(visible);

        if (control is TextBox textBox)
        {
            if (properties.TryGetValue("Text", out var textBoxText))
                textBox.Text = textBoxText;
        }
        else if (control is ComboBox comboBox)
        {
            if (properties.TryGetValue("Items", out var items))
            {
                comboBox.Items.AddRange(items.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
            }
            if (properties.TryGetValue("SelectedIndex", out var selectedIndex))
            {
                comboBox.SelectedIndex = int.Parse(selectedIndex);
            }
        }
        else if (control is CheckBox checkBox)
        {
            if (properties.TryGetValue("Checked", out var checkedValue))
            {
                checkBox.Checked = bool.Parse(checkedValue);
            }
        }
        else if (control is NumericUpDown numeric)
        {
            if (properties.TryGetValue("Value", out var value))
                numeric.Value = decimal.Parse(value);

            if (properties.TryGetValue("Minimum", out var min))
                numeric.Minimum = decimal.Parse(min);

            if (properties.TryGetValue("Maximum", out var max))
                numeric.Maximum = decimal.Parse(max);

            if (properties.TryGetValue("DecimalPlaces", out var decimalPlaces))
                numeric.DecimalPlaces = int.Parse(decimalPlaces);

            if (properties.TryGetValue("Increment", out var increment))
                numeric.Increment = decimal.Parse(increment);
        }
    }

    [Serializable]
    public class SerializableControlDictionary
    {
        [XmlArray("Controls")]
        [XmlArrayItem("Control")]
        public List<SerializableControlItem> Items { get; set; } = new List<SerializableControlItem>();

        public void Add(string key, SerializableControl value)
        {
            Items.Add(new SerializableControlItem { Key = key, Value = value });
        }

        public Dictionary<string, SerializableControl> ToDictionary()
        {
            return Items.ToDictionary(item => item.Key, item => item.Value);
        }
    }

    [Serializable]
    public class SerializableControlItem
    {
        [XmlAttribute("Key")]
        public string Key { get; set; }

        [XmlElement("Value")]
        public SerializableControl Value { get; set; }
    }

    [Serializable]
    public class SerializableControl
    {
        public string ControlType { get; set; }

        [XmlArray("Properties")]
        [XmlArrayItem("Property")]
        public List<SerializableProperty> Properties { get; set; } = new List<SerializableProperty>();
    }

    [Serializable]
    public class SerializableProperty
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlText]
        public string Value { get; set; }
    }
}