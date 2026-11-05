using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public static class ExecuteMethodClass
    {
   
        

        /// <summary>
        /// 方法调用模板
        /// </summary>
        /// <param name="action"></param>
        /// <param name="methodName"></param>
        public static void ExecuteMethod(Action action, [CallerMemberName] string methodName = "")
        {
            //string className = MethodBase.GetCurrentMethod().DeclaringType?.FullName ?? "UnknownClass";
            try
            {
                // 执行传入的委托
                action();
            }
            catch (Exception ex)
            {
                // 构造完整的异常信息，包含类名、方法名和异常详情
                //string fullErrorMessage = $"[{className}.{methodName}] 发生异常: {ee.ToString()}";
                MessageNotify.AddErrorLog(ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        /// <summary>
        /// 方法调用模板
        /// </summary>
        /// <param name="action"></param>
        /// <param name="methodName"></param>
        public static void ExecuteMethodContainErrShow(Action action, [CallerMemberName] string methodName = "")
        {
            //string className = MethodBase.GetCurrentMethod().DeclaringType?.FullName ?? "UnknownClass";
            try
            {
                // 执行传入的委托
                action();
            }
            catch (Exception ex)
            {
                // 构造完整的异常信息，包含类名、方法名和异常详情
                //string fullErrorMessage = $"[{className}.{methodName}] 发生异常: {ee.ToString()}";
                MessageNotify.AddErrorLog(ex.Message + "\r\n" + ex.StackTrace);
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        public static T ExecuteMethod<T>(Func<T> func, [CallerMemberName] string methodName = "")
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                MessageNotify.AddErrorLog(ex.Message + "\r\n" + ex.StackTrace);
                return default(T); // 返回类型的默认值
            }
        }

     

        public static T ExecuteMethodContainErrShow<T>(Func<T> func, [CallerMemberName] string methodName = "")
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                MessageNotify.AddErrorLog(ex.Message + "\r\n" + ex.StackTrace);
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                return default(T); // 返回类型的默认值
            }
        }

        public static ExecuteResult<T> ExecuteMethod<T>(Func<ExecuteResult<T>> func, [CallerMemberName] string methodName = "")
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                MessageNotify.AddErrorLog(ex.Message + "\r\n" + ex.StackTrace);
                ExecuteResult<T>  executeResult = new ExecuteResult<T>();
                executeResult.IsSuccess = false;
                executeResult.Message = ex.Message;
                executeResult.Value = default(T);
                return executeResult; // 返回类型的默认值
            }
        }
    }
}
