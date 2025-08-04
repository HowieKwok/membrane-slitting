#define IYCustomControl //自定义控件接口
using Frame_Core;
using LogTool;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using YYSD.UICore.Control;

namespace YYSD.UICore
{
    /// <summary>
    /// Form控件自动绑定 加载与修改保存
    /// </summary>
    public class YFormControlAutobind
    {
        /// <summary>
        /// Form控件自动加载
        /// </summary>
        /// <typeparam name="T">输入：对象类型</typeparam>
        /// <param name="frm">输入：Form句柄实例</param>
        /// <param name="Obj">输入：对象实例</param>
        /// <returns></returns>
        public static string Loading<T>(Form frm, T Obj)
        {
            string error = "";
            try
            {
                foreach (System.Windows.Forms.Control control in frm.Controls)
                {
                    string controlName = control.Name;
                    object value = GetPropertyValue<T>(Obj, controlName);
                    if (value == null) continue;
                    try
                    {
                        CheckBox cbx = control as CheckBox;
                        if (cbx != null)
                        {
                            cbx.Checked = (bool)value;
                            continue;
                        }
                        TextBox txt = control as TextBox;
                        if (txt != null)
                        {
                            txt.Text = (string)value;
                            continue;
                        }
                        NumericUpDown num = control as NumericUpDown;
                        if (num != null)
                        {
                            num.Value = Convert.ToDecimal(value);
                            continue;
                        }
                        ComboBox com = control as ComboBox;
                        if (com != null)
                        {
                            com.SelectedIndex = Convert.ToInt32(value);
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        error += $"ErrorInfo：control.name[{controlName}]control.Value{value.ToString()}\n{ex.ToString()}\n";
                        string errorMessage = string.Format("方法{0}执行 错误：", CommonApproach.getMethodName(ex));
                         Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                error += $"ErrorInfo：{ex.ToString()}\n";
                string errorMessage = string.Format("方法{0}执行 错误：", CommonApproach.getMethodName(ex));
                 Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
            }
            return error;
        }

        /// <summary>
        /// Form控件自动修改保存
        /// </summary>
        /// <typeparam name="T">输入：对象类型</typeparam>
        /// <param name="frm">输入：Form句柄实例</param>
        /// <param name="Obj">输入：对象实例</param>
        /// <returns></returns>
        public static string Saveing<T>(Form frm, T Obj)
        {
            string error = "";
            try
            {
                foreach (System.Windows.Forms.Control control in frm.Controls)
                {
                    string controlName = control.Name;
                    object value = null;
                    try
                    {
                        CheckBox cbx = control as CheckBox;
                        if (cbx != null)
                        {
                            value = cbx.Checked;
                        }
                        TextBox txt = control as TextBox;
                        if (txt != null)
                        {
                            value = txt.Text;
                        }
                        NumericUpDown num = control as NumericUpDown;
                        if (num != null)
                        {
                            if (num.DecimalPlaces > 0) value = (double)num.Value;
                            else value = (int)num.Value;
                        }
                        ComboBox com = control as ComboBox;
                        if (com != null)
                        {
                            value = com.SelectedIndex;
                        }
                        SetPropertyValue<T>(Obj, controlName, value);
                    }
                    catch (Exception ex)
                    {
                        error += $"ErrorInfo：control.name[{controlName}]control.Value{value.ToString()}\n{ex.ToString()}\n";
                        string errorMessage = string.Format("方法{0}执行 错误：", CommonApproach.getMethodName(ex));
                         Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                error += $"ErrorInfo：{ex.ToString()}\n";
                string errorMessage = string.Format("方法{0}执行 错误：", CommonApproach.getMethodName(ex));
                 Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
            }
            return error;
        }

        #region >>>Base Methods
        /// <summary>
        /// 获取类标量的所有属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objData"></param>
        /// <param name="MemberName"></param>
        /// <returns></returns>
        protected static PropertyInfo GetProperty<T>(T Obj, string MemberName)
        {
            PropertyInfo p = null;
            try
            {
                //Type type = typeof(T);
                //string className = type.Name;
                //PropertyInfo[] ProperInfo = typeof(T).GetProperties();
                //System.Reflection.PropertyInfo[] properties = type.GetProperties(); //获取所有属性
                //foreach (System.Reflection.PropertyInfo property in properties)
                //{
                //    object objVal = property.GetValue(Obj, null);//旧参数类中,属性中存放的值
                //    object[] obj_Description = property.GetCustomAttributes(typeof(DescriptionAttribute), true);//获取对应属性注释
                //    string str_description = ((DescriptionAttribute)obj_Description[0]).Description;
                //    string strValName = property.Name.ToString();
                //    if (property.Name == MemberName)
                //    {
                //        outobj = property;
                //        break;
                //    }
                //}
                ////outobj = type.GetProperty(MemberName);//设置类中的成员属性的值

                Type type = typeof(T);
                Assembly assembly = Assembly.Load(type.Assembly.FullName); //程序集名称
                Type type1 = assembly.GetType(type.FullName); //程序集名称.类名
                p = type.GetProperty(MemberName);
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("方法{0}执行 错误：", CommonApproach.getMethodName(ex));
                 Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
            }
            return p;
        }
        /// <summary>
        /// 获取类标量的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objData"></param>
        /// <param name="MemberName"></param>
        /// <returns></returns>
        protected static object GetPropertyValue<T>(T Obj, string MemberName)
        {
            object getVal = null;
            //object setVal = null;
            try
            {
                Type type = typeof(T);
                Assembly assembly = Assembly.Load(type.Assembly.FullName); //程序集名称
                Type type1 = assembly.GetType(type.FullName); //程序集名称.类名
                PropertyInfo p = type.GetProperty(MemberName);
                if (p != null)
                {
                    //p.SetValue(Obj, setVal); //设置类中的成员属性的值
                    getVal = p.GetValue(Obj); //获取类中的成员属性的值
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("方法{0}执行 错误：", CommonApproach.getMethodName(ex));
                 Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
            }
            return getVal;
        }
        /// <summary>
        /// 设置类标量的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objData"></param>
        /// <param name="MemberName"></param>
        /// <returns></returns>
        protected static object SetPropertyValue<T>(T Obj, string MemberName, object setVal)
        {
            object getVal = null;
            if (setVal == null) return getVal;
            try
            {
                //Type type = typeof(T);
                //string className = type.Name;
                //PropertyInfo[] ProperInfo = typeof(T).GetProperties();
                //System.Reflection.PropertyInfo[] properties = type.GetProperties(); //获取所有属性
                //PropertyInfo p = type.GetProperty(MemberName);
                //if (p != null)
                //{
                //    //p.SetValue(Obj, setVal); //设置类中的成员属性的值
                //    getVal = p.GetValue(Obj); //获取类中的成员属性的值
                //}

                Type type = Obj.GetType();
                Assembly assembly = Assembly.Load(type.Assembly.FullName); //程序集名称
                Type type1 = assembly.GetType(type.FullName); //程序集名称.类名
                PropertyInfo p = type.GetProperty(MemberName);
                if (p != null)
                {
                    getVal = p.GetValue(Obj); //获取类中的成员属性的值
                    if (setVal.GetType().Equals(getVal.GetType()))//防止设置数据混乱
                        p.SetValue(Obj, setVal); //设置类中的成员属性的值
                    getVal = p.GetValue(Obj); //获取类中的成员属性的值
                }
            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("方法{0}执行 错误：", CommonApproach.getMethodName(ex));
                 Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
            }
            return getVal;
        }
        /// <summary>
        /// 获取类标量的任意特性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="A"></typeparam>
        /// <param name="objData"></param>
        /// <param name="MemberName"></param>
        /// <returns></returns>
        protected static A GetAttribute<T, A>(T objData, string MemberName)
        {
            object obj = null;
            A getVal = (A)obj;
            try
            {
                Type type = typeof(T);
                Assembly assembly = Assembly.Load(type.Assembly.FullName); //程序集名称
                Type type1 = assembly.GetType(type.FullName); //程序集名称.类名
                PropertyInfo p = type.GetProperty(MemberName);
                if (p != null)
                {
                    object[] objs = p.GetCustomAttributes(typeof(A), true);//获取对应属性注释
                    if (objs.Length > 0) getVal = (A)objs[0];
                }

            }
            catch (Exception ex)
            {
                string errorMessage = string.Format("方法{0}执行 错误：", CommonApproach.getMethodName(ex));
                 Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
            }
            return getVal;
        }

        /// <summary>
        /// 获取变量自身的名字，如string name，使用后可以获得变量的名字“name”
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected static string GetVarName<T>(System.Linq.Expressions.Expression<Func<T>> exp)
        {
            ////测试例子
            //int b = 1;
            //string abc = "123";
            //string name1 = YReflectionHelper.GetVarName(() => abc);
            //string name2 = YReflectionHelper.GetVarName(() => b);

            return ((System.Linq.Expressions.MemberExpression)exp.Body).Member.Name;
        }
        /// <summary>
        /// c# 6.0 支持nameof方法去获取变量名
        /// </summary>
        private void GetVarName()
        {
            //string abc = "123";
            //string name = nameof(abc);
        }
        #endregion
    }
    /// <summary>
    /// Form控件自动绑定 加载与修改保存
    /// </summary>
    public class YControlAutobind : YFormControlAutobind
    {
        /// <summary>
        /// Form控件自动加载
        /// </summary>
        /// <typeparam name="T">输入：对象类型</typeparam>
        /// <typeparam name="A">输入：特性类型</typeparam>
        /// <param name="frm">输入：Form句柄实例</param>
        /// <param name="Obj">输入：对象实例</param>
        /// <returns></returns>
        public static string Loading<T, A>(Form frm, T Obj)
        {
            string error = "";
            try
            {
                LoadingControls<T, A>(frm.Controls, Obj);

            }
            catch (Exception ex)
            {
                error += $"ErrorInfo：{ex.ToString()}\n";
                string errorMessage = string.Format("方法{0}执行 错误：", CommonApproach.getMethodName(ex));
                 Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
            }
            return error;
        }
        /// <summary>
        /// Form控件自动修改保存
        /// </summary>
        /// <typeparam name="T">输入：对象类型</typeparam>
        /// <param name="frm">输入：Form句柄实例</param>
        /// <param name="Obj">输入：对象实例</param>
        /// <returns></returns>
        public static string Saveing<T, A>(Form frm, T Obj)
        {
            string error = "";
            try
            {
                SaveingControls<T, A>(frm.Controls, Obj);

            }
            catch (Exception ex)
            {
                error += $"ErrorInfo：{ex.ToString()}\n";
                string errorMessage = string.Format("方法{0}执行 错误：", CommonApproach.getMethodName(ex));
                 Log4netRecord.ErrorLog.Fatal(errorMessage + ex.Message);
            }
            return error;
        }


        private static void SaveingControls<T, A>(System.Windows.Forms.Control.ControlCollection controls, T obj)
        {
            foreach (System.Windows.Forms.Control control in controls)
            {
                if (control.Controls.Count > 0)
                {
                    // 递归遍历子控件
                    SaveingControls<T, A>(control.Controls, obj);
                }

                string controlName = control.Name;
                object value = null;
                    CheckBox cbx = control as CheckBox;
                    if (cbx != null)
                    {
                        value = cbx.Checked;
                    }
                    TextBox txt = control as TextBox;
                    if (txt != null)
                    {
                        value = txt.Text;
                    }
                    NumericUpDown num = control as NumericUpDown;
                    if (num != null)
                    {
                        if (num.DecimalPlaces > 0) value = (double)num.Value;
                        else value = (int)num.Value; ;
                    }
                    ComboBox com = control as ComboBox;
                    if (com != null)
                    {
                        value = com.SelectedIndex;
                    }
#if IYCustomControl
                    #region >>>自定义控件
                    YCheckBox ycbx = control as YCheckBox;
                    if (ycbx != null)
                    {
                        value = ycbx.value;
                    }
                    YTextBox ytxt = control as YTextBox;
                    if (ytxt != null)
                    {
                        value = ytxt.value;
                    }
                    YNumericUpDown ynum = control as YNumericUpDown;
                    if (ynum != null)
                    {
                        value = ynum.value;
                    }
                    YComboBox ycom = control as YComboBox;
                    if (ycom != null)
                    {
                        value = ycom.value;
                    }
                    #endregion
#endif

                    SetPropertyValue<T>(obj, controlName, value);
            }

        }



        private static void LoadingControls<T, A>(System.Windows.Forms.Control.ControlCollection controls, T obj)
        {
            foreach (System.Windows.Forms.Control control in controls)
            {
                if(control.Controls.Count>0)
                {
                    // 递归遍历子控件
                    LoadingControls<T, A>(control.Controls, obj);
                }

                string controlName = control.Name;
                PropertyInfo p = GetProperty<T>(obj, controlName);
                object value = null;
                string showName = null;
                if (p != null)
                {
                  #if IYCustomControl
                    if (typeof(A).Equals(typeof(IYCustomAttribute)))
                    {
                        object[] objs = p.GetCustomAttributes(typeof(IYCustomAttribute), true);//获取对应属性注释
                        if (objs.Length > 0) showName = ((IYCustomAttribute)objs[0]).Value.ToString();
                    }
                  #endif
                    value = p.GetValue(obj); //获取类中的成员属性的值
                }

                if (value == null) continue;

                // 对控件执行相应操作
              
                    CheckBox cbx = control as CheckBox;
                    if (cbx != null)
                    {
                        cbx.Text = (string)showName;
                        cbx.Checked = (bool)value;
                        continue;
                    }
                    TextBox txt = control as TextBox;
                    if (txt != null)
                    {
                        txt.Text = (string)value;
                        continue;
                    }
                    NumericUpDown num = control as NumericUpDown;
                    if (num != null)
                    {
                        num.Value = Convert.ToDecimal(value);
                        continue;
                    }
                    ComboBox com = control as ComboBox;
                    if (com != null)
                    {
                        com.SelectedIndex = Convert.ToInt32(value);
                        continue;
                    }
#if IYCustomControl
                    #region >>>自定义控件
                    YCheckBox ycbx = control as YCheckBox;
                    if (ycbx != null)
                    {
                        ycbx.name = showName;
                        ycbx.value = value;
                        continue;
                    }
                    YTextBox ytxt = control as YTextBox;
                    if (ytxt != null)
                    {
                        ytxt.name = showName;
                        ytxt.value = value;
                        continue;
                    }
                    YNumericUpDown ynum = control as YNumericUpDown;
                    if (ynum != null)
                    {
                        ynum.name = showName;
                        ynum.value = value;
                        continue;
                    }
                    YComboBox ycom = control as YComboBox;
                    if (ycom != null)
                    {
                        ycom.name = showName;
                        ycom.value = value;
                        continue;
                    }
                    #endregion
#endif 
                
            }
        }

    }
}
