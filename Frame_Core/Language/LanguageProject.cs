using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{

    public static class DoubleExtensions
    {
        public static bool Between(this double value, double lower, double upper)
        {
            return value >= lower && value <= upper;
        }

       
    }
    /// <summary>
    /// 语言项目
    /// </summary>

    public static class LanguageProject
    {
        /// <summary>
        /// 获取尺寸枚举翻译
        /// </summary>
        public static string GetSizeEnumTranslate(SizeEnum sizeEnum, LanguageEnum LanguageEnum)
        {
            string name = "";
            if (LanguageEnum == LanguageEnum.English)
            {
                switch (sizeEnum)
                {
                    case SizeEnum.极片尺寸X:
                        name = "PolePieceX";
                        break;
                    case SizeEnum.极片尺寸Y:
                        name = "PolePieceY";
                        break;
                    case SizeEnum.极片角度:
                        name = "PolePieceAngle";
                        break;
                    case SizeEnum.极耳中心到上Mark_Y:
                        name = "EarCentreToUpMarkY";
                        break;
                    case SizeEnum.极耳中心到下Mark_Y:
                        name = "EarCentreToDownMarkY";
                        break;
                    case SizeEnum.极耳顶宽Y:
                        name = "EarTopWidth";
                        break;
                    case SizeEnum.极耳底宽Y:
                        name = "EarBottomWidth";
                        break;
                    case SizeEnum.极耳高度:
                        name = "EarHeight";
                        break;
                    case SizeEnum.AT9宽度:
                        name = "AT9";
                        break;
                    case SizeEnum.极耳台阶高:
                        name = "EarStepHeight";
                        break;
                    case SizeEnum.极耳上升沿角度:
                        name = "EarRisingEdgeAngle";
                        break;
                    case SizeEnum.极耳下降沿角度:
                        name = "EarFallingEdgeAngle";
                        break;
                    case SizeEnum.顶R1半径:
                        name = "TopR1Radius";
                        break;
                    case SizeEnum.底R2半径:
                        name = "BottomR2Radius";
                        break;
                    case SizeEnum.上V角宽度:
                        name = "UpVAngleWidth";
                        break;
                    case SizeEnum.上V角深度:
                        name = "UpVAngleDepth";
                        break;
                    case SizeEnum.下V角宽度:
                        name = "DownVAngleWidth";
                        break;
                    case SizeEnum.下V角深度:
                        name = "DownVAngleDepth";
                        break;
                    case SizeEnum.涂层尺寸X:
                        name = "CoatingSizeX";
                        break;

                }
            }
            else
            {
                name = sizeEnum.ToString();
            }

            return name;
        }

        /// <summary>
        /// 获取缺陷枚举翻译
        /// </summary>
        public static string GetDefct1EnumEnumTranslate(Defct1Enum defct1Enum, LanguageEnum LanguageEnum)
        {
            string name = "";
            if (LanguageEnum == LanguageEnum.English)
            {
                switch (defct1Enum)
                {
                    case Defct1Enum.直边破损:
                        name = "StraightEdgeBreakage";
                        break;
                    case Defct1Enum.直边掉料:
                        name = "StraightEdgeDrop";
                        break;
                    case Defct1Enum.直边余料:
                        name = "StraightEdgeResidualMaterial";
                        break;
                    case Defct1Enum.接带:
                        name = "TapeSplicing";
                        break;
                    case Defct1Enum.漏金属:
                        name = "MetalLeakage";
                        break;
                    case Defct1Enum.褶皱:
                        name = "Wrinkle";
                        break;
                    case Defct1Enum.气泡:
                        name = "Bubble";
                        break;
                    case Defct1Enum.划痕:
                        name = "Scratch";
                        break;
                    case Defct1Enum.压痕:
                        name = "Indentation";
                        break;
                    case Defct1Enum.极片破损:
                        name = "PolePlateDamaged";
                        break;
                    case Defct1Enum.表面裂纹:
                        name = "SurfaceCrack";
                        break;
                    case Defct1Enum.黑点:
                        name = "DarkSpot";
                        break;
                    case Defct1Enum.白点:
                        name = "WhiteSpot";
                        break;
                    case Defct1Enum.其它:
                        name = "Else";
                        break;
                    case Defct1Enum.未冷压:
                        name = "NoChillPressing";
                        break;

                }
            }
            else
            {
                name = defct1Enum.ToString();
            }

            return name;
        }

        #region 枚举和combox翻译。
        /// <summary>
        /// 输入字段和类型，返回对应语言的翻译特性
        /// </summary>
        /// <param name="Language"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetLangeuageAttribute(object obj, LanguageEnum Language)
        {
            Type type = obj.GetType();
            FieldInfo fd = type.GetField(obj.ToString());
            if (fd == null) return string.Empty;
            object[] attrs = fd.GetCustomAttributes(typeof(LanguageAttribute), false); //true还是false好像没区别
            string desp = string.Empty;
            LanguageAttribute attr = attrs[0] as LanguageAttribute;
            switch (Language)
            {
                case LanguageEnum.Chinese:
                    desp = attr.StrChinese;
                    break;
                case LanguageEnum.English:
                    desp = attr.StrEnglish;
                    break;
                case LanguageEnum.France:
                    desp = attr.StrFrance;
                    break;
                default:
                    desp = attr.StrChinese;
                    break;
            }
            return desp;
        }
        /// <summary>
        /// 绑定将枚举的翻译绑定combox
        /// </summary>
        /// <typeparam name="T">枚举类</typeparam>
        /// <param name="combox">Combox</param>
        /// <param name="language">语言枚举</param>
        public static void ReBindCombox<T>(ComboBox combox, LanguageEnum language)
        {
            List<string> strlist = new List<string>();
            foreach (var item in Enum.GetValues(typeof(T)))
            {
                strlist.Add(GetLangeuageAttribute(item, language));
            }
            combox.DataSource = strlist;
        }

        public static T? MatchStringToEnum<T>(string input) where T : struct
        {
            try
            {
                //if (!typeof(T).IsEnum)
                //{
                //    throw new ArgumentException("T must be an enumerated type");
                //}

                foreach (T value in Enum.GetValues(typeof(T)))
                {
                    FieldInfo fieldInfo = typeof(T).GetField(value.ToString());
                    LanguageAttribute attr = (LanguageAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(LanguageAttribute));

                    if (attr != null)
                    {
                        if (attr.StrEnglish == input || attr.StrFrance == input || attr.StrChinese == input)
                        {
                            return value;
                        }
                    }
                }
            }
            catch
            {
                // 在这里可以记录异常信息，如果需要
                // 例如: Log.Error(ex.ToString());
                return null;
            }

            // 如果没有找到匹配项，也返回null
            return null;
        }

        #endregion


        #region 窗体翻译
        public static void ChangeLanguage(LanguageEnum _Language, ContainerControl ctr)
        {
            string culture = "ZH-CN";
            switch (_Language)
            {
                case LanguageEnum.Chinese:
                    culture = "ZH-CN";
                    break;
                case LanguageEnum.English:
                    culture = "en";
                    break;
                case LanguageEnum.France:
                    culture = "fr";
                    break;
                default:
                    culture = "ZH-CN";
                    break;
            }
            // 设置当前线程的CultureInfo和UICulture信息
            Thread.CurrentThread.CurrentCulture = new CultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);

            // 应用资源更改
            ComponentResourceManager resources = new ComponentResourceManager(ctr.GetType());
            // resources.ApplyResources(this, "$this"); //这句话会造成界面闪烁

            // 从当前窗体开始遍历控件
            TraverseControls(ctr, resources);

            // 强制窗体重绘
            ctr.Refresh();
        }
        /// <summary>
        /// 遍历控件
        /// </summary>
        /// <param name="control"></param>
        /// <param name="resources"></param>
        public static void TraverseControls(Control control, ComponentResourceManager resources)
        {
            foreach (Control subControl in control.Controls)
            {
                //在这里处理每个控件，例如打印控件名
                if (subControl is Panel || subControl is TableLayoutPanel || subControl is SplitContainer)
                {
                    //这些控件不应用资源
                }
                else
                {
                    resources.ApplyResources(subControl, subControl.Name);
                }

                // 递归遍历子控件
                if (subControl is MenuStrip) //menustrip
                {
                    MenuStrip menuStrip = (MenuStrip)subControl;
                    TraverseMenuStrip(menuStrip, resources);
                }
                else if (subControl is ToolStrip)//toolstrip
                {
                    ToolStrip toolStrip = (ToolStrip)subControl;
                    TraverseToolStrip(toolStrip, resources);
                }
                else if (subControl is StatusStrip)//statusstrip
                {
                    StatusStrip statusStrip = (StatusStrip)subControl;
                    TraverseStatusStrip(statusStrip, resources);
                }
                else if (subControl is TabControl)//statusstrip
                {
                    TabControl TabControl = (TabControl)subControl;
                    TraverseTabControl(TabControl, resources);
                }
                else
                {
                    TraverseControls(subControl, resources);
                }
            }
        }
        /// <summary>
        /// 遍历菜单栏menustrip
        /// </summary>
        /// <param name="menuStrip"></param>
        /// <param name="resources"></param>
        public static void TraverseMenuStrip(MenuStrip menuStrip, ComponentResourceManager resources)
        {
            foreach (ToolStripItem item in menuStrip.Items)
            {
                if (item is ToolStripMenuItem toolStripMenuItem)
                {
                    // 打印当前菜单项的文本
                    resources.ApplyResources(item, item.Name);

                    // 如果有子菜单项，则递归遍历子菜单项
                    foreach (ToolStripMenuItem item1 in toolStripMenuItem.DropDownItems)
                    {
                        resources.ApplyResources(item1, item1.Name);
                    }

                }
            }
        }
        /// <summary>
        /// 遍历菜单栏toolstrip
        /// </summary>
        /// <param name="menuStrip"></param>
        /// <param name="resources"></param>
        public static void TraverseToolStrip(ToolStrip toolStrip, ComponentResourceManager resources)
        {
            foreach (ToolStripItem item in toolStrip.Items)
            {
                if (item is ToolStripButton)//只翻译第一层button类型
                {
                    // 打印当前菜单项的文本
                    resources.ApplyResources(item, item.Name);
                }
            }
        }

        /// <summary>
        /// 遍历菜单栏toolstrip
        /// </summary>
        /// <param name="menuStrip"></param>
        /// <param name="resources"></param>
        public static void TraverseStatusStrip(StatusStrip statusStrip, ComponentResourceManager resources)
        {
            foreach (ToolStripItem item in statusStrip.Items)
            {
                if (item is ToolStripLabel)//只翻译第一层Lab类型
                {
                    // 打印当前菜单项的文本
                    resources.ApplyResources(item, item.Name);
                }
            }
        }

        /// <summary>
        /// 遍历TabControl
        /// </summary>
        /// <param name="menuStrip"></param>
        /// <param name="resources"></param>
        public static void TraverseTabControl(TabControl TabControl, ComponentResourceManager resources)
        {
            foreach (TabPage item in TabControl.TabPages)
            {
                //// 打印当前菜单项的文本
                resources.ApplyResources(item, item.Name);
                TraverseControls(item, resources);
            }
        }

        #endregion
    }

    /// <summary>
    /// 枚举绑定的翻译特性
    /// </summary>
    public class LanguageAttribute : Attribute
    {
        /// <summary>
        /// 中文释意
        /// </summary>
        public string StrChinese { get; set; }

        /// <summary>
        /// 英文释意
        /// </summary>
        public string StrEnglish { get; set; }

        /// <summary>
        /// 法文释意
        /// </summary>
        public string StrFrance { get; set; }

        public LanguageAttribute()
        {
            this.StrChinese = "中文";
            this.StrEnglish = "English";
            this.StrFrance = "France";
        }
        public LanguageAttribute(string StrChinese)
        {
            this.StrChinese = StrChinese;
        }
        public LanguageAttribute(string StrChinese, string StrEnglish)
        {
            this.StrChinese = StrChinese;
            this.StrEnglish = StrEnglish;
        }
        public LanguageAttribute(string StrChinese, string StrEnglish, string StrFrance)
        {
            this.StrChinese = StrChinese;
            this.StrEnglish = StrEnglish;
            this.StrFrance = StrFrance;
        }
    }
}
