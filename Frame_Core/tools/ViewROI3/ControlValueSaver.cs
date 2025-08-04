using EHolly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

public class ControlValueSaver
{
    /// <summary>
    /// 保存窗体中控件的值到字典
    /// </summary>
    /// <param name="form">要保存的窗体</param>
    /// <returns>包含控件名称和值的字典</returns>
    public static Dictionary<string, object> SaveControlValues(Form form)
    {
        var values = new Dictionary<string, object>();

        foreach (Control control in form.Controls)
        {
            SaveControlValue(control, values);
        }
        return values;
    }

   


    /// <summary>
    /// 递归保存控件及其子控件的值
    /// </summary>
    private static void SaveControlValue(Control control, Dictionary<string, object> values)
    {
        // 只保存有名称的控件
        if (!string.IsNullOrEmpty(control.Name))
        {
            object value = GetControlValue(control);
            if (value != null)
            if (value != null)
            {
                values[control.Name] = value;
            }
        }

        // 递归处理容器控件中的子控件
        if (control.HasChildren)
        {
            foreach (Control childControl in control.Controls)
            {
                SaveControlValue(childControl, values);
            }
        }
    }

    /// <summary>
    /// 根据控件类型获取控件的值
    /// </summary>
    private static object GetControlValue(Control control)
    {
        switch (control)
        {
            case TextBox textBox:
                return textBox.Text;

            case ComboBox comboBox:
                //return comboBox.SelectedItem ?? comboBox.Text;
                return comboBox.SelectedIndex;

            //case Label label:
            //    return label.Text;

            case CheckBox checkBox:
                return checkBox.Checked;

            case RadioButton radioButton:
                return radioButton.Checked;

            case NumericUpDown numericUpDown:
                return numericUpDown.Value;

            case DateTimePicker dateTimePicker:
                return dateTimePicker.Value;

            case ListBox listBox:
                return listBox.SelectedItem;

            //case CheckedListBox checkedListBox:
            //    var checkedItems = new List<object>();
            //    for (int i = 0; i < checkedListBox.Items.Count; i++)
            //    {
            //        if (checkedListBox.GetItemChecked(i))
            //        {
            //            checkedItems.Add(checkedListBox.Items[i]);
            //        }
            //    }
            //    return checkedItems;

            case RichTextBox richTextBox:
                return richTextBox.Text;

            case MaskedTextBox maskedTextBox:
                return maskedTextBox.Text;

            case TrackBar trackBar:
                return trackBar.Value;

            case DataGridView dataGridView:
                // 对于DataGridView，可能需要更复杂的处理
                return dataGridView.DataSource;

            // 添加更多控件类型的处理...

            default:
                // 对于不支持或不需保存的控件返回null
                return null;
        }
    }

    /// <summary>
    /// 从字典恢复控件的值
    /// </summary>
    /// <param name="form">要恢复的窗体</param>
    /// <param name="values">包含控件名称和值的字典</param>
    public static void RestoreControlValues(Form form, Dictionary<string, object> values)
    {
        foreach (Control control in form.Controls)
        {
            RestoreControlValue(control, values);
        }
    }

    public static void RestoreControlValues(TableLayoutPanel tap, Dictionary<string, object> values)
    {
        foreach (Control control in tap.Controls)
        {
            RestoreControlValue(control, values);
        }
    }


    /// <summary>
    /// 递归恢复控件及其子控件的值
    /// </summary>
    private static void RestoreControlValue(Control control, Dictionary<string, object> values)
    {
        if (!string.IsNullOrEmpty(control.Name) && values.TryGetValue(control.Name, out object value))
        {
            SetControlValue(control, value);
        }

        if (control.HasChildren)
        {
            foreach (Control childControl in control.Controls)
            {
                RestoreControlValue(childControl, values);
            }
        }
    }

    /// <summary>
    /// 根据控件类型设置控件的值
    /// </summary>
    private static void SetControlValue(Control control, object value)
    {
        if (value == null) return;

        try
        {
            switch (control)
            {
                case TextBox textBox:
                    textBox.Text = value.ToString();
                    break;
                //case Label label:
                //    label.Text = value.ToString();
                //    break;

                case ComboBox comboBox:
                    //comboBox.SelectedItem = value;
                    //if (comboBox.SelectedItem == null)
                    //    comboBox.Text = value.ToString();
                    comboBox.SelectedIndex = Convert.ToInt32(value);
                    break;

                case CheckBox checkBox:
                    checkBox.Checked = Convert.ToBoolean(value);
                    break;

                case RadioButton radioButton:
                    radioButton.Checked = Convert.ToBoolean(value);
                    break;

                case NumericUpDown numericUpDown:
                    numericUpDown.Value = Convert.ToDecimal(value);
                    break;

                case DateTimePicker dateTimePicker:
                    dateTimePicker.Value = Convert.ToDateTime(value);
                    break;

                case ListBox listBox:
                    listBox.SelectedItem = value;
                    break;

                //case CheckedListBox checkedListBox:
                //    if (value is List<object> checkedItems)
                //    {
                //        for (int i = 0; i < checkedListBox.Items.Count; i++)
                //        {
                //            checkedListBox.SetItemChecked(i, checkedItems.Contains(checkedListBox.Items[i]));
                //        }
                //    }
                //    break;

                case RichTextBox richTextBox:
                    richTextBox.Text = value.ToString();
                    break;

                case MaskedTextBox maskedTextBox:
                    maskedTextBox.Text = value.ToString();
                    break;

                case TrackBar trackBar:
                    trackBar.Value = Convert.ToInt32(value);
                    break;

                case DataGridView dataGridView:
                    dataGridView.DataSource = value;
                    break;

                    // 添加更多控件类型的处理...
            }
        }
        catch (Exception ex)
        {
            // 处理类型转换错误等异常
            Console.WriteLine($"设置控件 {control.Name} 值时出错: {ex.Message}");
        }
    }

    /// <summary>
    /// 将控件值保存到XML文件
    /// </summary>
    /// <param name="form">要保存的窗体</param>
    /// <param name="filePath">XML文件路径</param>
    public static void SaveToXml(Form form, string filePath)
    {
        try
        {
            var values = SaveControlValues(form);
            var serializer = new XmlSerializer(typeof(SerializableDictionary<string, object>));
            // 转换为可序列化的字典
            var serializableDict = new SerializableDictionary<string, object>();
            foreach (var kvp in values)
            {
                serializableDict.Add(kvp.Key, kvp.Value);
            }

            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, serializableDict);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"保存到XML文件失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }


    public static void SaveListXML(List<Dictionary<string, object>> LtDictionary, string filePath)
    {
        try
        {
            var serializer = new XmlSerializer(typeof(List<SerializableDictionary<string, object>>));
            //生成可序列化的字典集合
            List<SerializableDictionary<string, object>> LtSerializableDictionary = new List<SerializableDictionary<string, object>>();
            foreach (Dictionary<string, object> Dic in LtDictionary)
            {
                SerializableDictionary<string, object> keyValuePairs = new SerializableDictionary<string, object>();
                foreach (var kvp in Dic)
                {
                    keyValuePairs.Add(kvp.Key, kvp.Value);
                }
                LtSerializableDictionary.Add(keyValuePairs);
            }
            using (var writer = new StreamWriter(filePath))
            {
                serializer.Serialize(writer, LtSerializableDictionary);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"保存到XML文件失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    //public static void SaveDicMeasureRes(Dictionary<string, ColorObjAndStr> keyValuePairs, string filePath)
    //{
    //    try
    //    {
    //        var serializer = new XmlSerializer(typeof(SerializableDictionary<string, ColorObjAndStr>));
    //        //生成可序列化的字典集合
    //        SerializableDictionary<string, ColorObjAndStr> SerializableDictionary = new SerializableDictionary<string, ColorObjAndStr>();

    //            foreach (var kvp in keyValuePairs)
    //            {
    //                SerializableDictionary.Add(kvp.Key, kvp.Value);
    //            }
    //        using (var writer = new StreamWriter(filePath))
    //        {
    //            serializer.Serialize(writer, SerializableDictionary);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        MessageBox.Show($"保存到XML文件失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
    //    }
    //}

    //public static Dictionary<string, ColorObjAndStr>  LoadDicMeasureRes(string filePath)
    //{
    //    Dictionary<string, ColorObjAndStr> Dictionary = new Dictionary<string, ColorObjAndStr>();
    //    try
    //    {
    //        if (!File.Exists(filePath))
    //        {
    //            MessageBox.Show("指定的XML文件不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
    //            return Dictionary;
    //        }
    //        var serializer = new XmlSerializer(typeof(SerializableDictionary<string, ColorObjAndStr>));
    //        SerializableDictionary<string, ColorObjAndStr> serializableDict;
    //        using (var reader = new StreamReader(filePath))
    //        {
    //            serializableDict = (SerializableDictionary<string, ColorObjAndStr>)serializer.Deserialize(reader);
    //        }
    //        // 转换为普通字典
    //        foreach (var keyValuePairs in serializableDict)
    //        {
    //            Dictionary.Add(keyValuePairs.Key, keyValuePairs.Value);
    //        }
    //        return Dictionary;
    //    }
    //    catch (Exception ex)
    //    {
    //        MessageBox.Show($"从XML文件加载失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
    //        return Dictionary;
    //    }
    //}

        /// <summary>
        /// 从XML文件加载并恢复控件值
        /// </summary>
        /// <param name="form">要恢复的窗体</param>
        /// <param name="filePath">XML文件路径</param>
        public static void LoadFromXml(Form form, string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("指定的XML文件不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var serializer = new XmlSerializer(typeof(SerializableDictionary<string, object>));
            SerializableDictionary<string, object> serializableDict;

            using (var reader = new StreamReader(filePath))
            {
                serializableDict = (SerializableDictionary<string, object>)serializer.Deserialize(reader);
            }

            // 转换为普通字典
            var values = new Dictionary<string, object>();
            foreach (var kvp in serializableDict)
            {
                values.Add(kvp.Key, kvp.Value);
            }

            RestoreControlValues(form, values);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"从XML文件加载失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public static List<Dictionary<string, object>> LoadListXML(string filePath)
    {
        List<Dictionary<string, object>> LtDictionary = new List<Dictionary<string, object>>();
        try
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("指定的XML文件不存在", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return LtDictionary;
            }
            var serializer = new XmlSerializer(typeof(List<SerializableDictionary<string, object>>));
            List<SerializableDictionary<string, object>> LtserializableDict;
            using (var reader = new StreamReader(filePath))
            {
                LtserializableDict = (List<SerializableDictionary<string, object>>)serializer.Deserialize(reader);
            }
            // 转换为普通字典
            var values = new Dictionary<string, object>();
            foreach (SerializableDictionary<string, object> keyValuePairs in LtserializableDict)
            {
                Dictionary<string, object> dictionary = new Dictionary<string, object>();
                foreach (var item in keyValuePairs)
                {
                    dictionary.Add(item.Key, item.Value);
                }
                LtDictionary.Add(dictionary);
            }
            return LtDictionary;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"从XML文件加载失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return LtDictionary;
        }
    }


}

/// <summary>
/// 可序列化的字典类，用于XML序列化
/// </summary>
[Serializable]
[XmlRoot("ControlValues")]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
{
    public System.Xml.Schema.XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(System.Xml.XmlReader reader)
    {
        XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
        XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

        bool wasEmpty = reader.IsEmptyElement;
        reader.Read();

        if (wasEmpty)
            return;

        while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
        {
            reader.ReadStartElement("Item");

            reader.ReadStartElement("Key");
            TKey key = (TKey)keySerializer.Deserialize(reader);
            reader.ReadEndElement();

            reader.ReadStartElement("Value");
            TValue value = (TValue)valueSerializer.Deserialize(reader);
            reader.ReadEndElement();

            this.Add(key, value);

            reader.ReadEndElement();
            reader.MoveToContent();
        }
        reader.ReadEndElement();
    }

    public void WriteXml(System.Xml.XmlWriter writer)
    {
        XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
        XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));

        foreach (TKey key in this.Keys)
        {
            writer.WriteStartElement("Item");

            writer.WriteStartElement("Key");
            keySerializer.Serialize(writer, key);
            writer.WriteEndElement();

            writer.WriteStartElement("Value");
            TValue value = this[key];
            valueSerializer.Serialize(writer, value);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }
    }
}