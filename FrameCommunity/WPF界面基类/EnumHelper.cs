using FrameCommunity.语言;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FrameCommunity
{
    public static class EnumHelper
    {
        public static ObservableCollection<ComboBoxEnumItem> GetEnumComboBoxItems(
     Type enumType,
     Func<string, string> translate = null)
        {
            if (enumType == null) throw new ArgumentNullException(nameof(enumType));
            if (!enumType.IsEnum) throw new ArgumentException("必须是枚举类型", nameof(enumType));

            var items = enumType
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(field =>
                {
                    var uiDisplay = field
                        .GetCustomAttributes(typeof(UiDisplayAttribute), false)
                        .FirstOrDefault() as UiDisplayAttribute;

                    var key = uiDisplay != null ? uiDisplay.Key : field.Name;
                    var text = translate != null ? translate(key) : key;

                    return new ComboBoxEnumItem
                    {
                        Value = Convert.ToInt32(field.GetValue(null)),
                        Key = key,
                        Text = text
                    };
                })
                .ToList();

            return new ObservableCollection<ComboBoxEnumItem>(items);
        }
    }
}
