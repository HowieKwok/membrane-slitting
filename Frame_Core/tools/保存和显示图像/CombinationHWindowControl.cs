using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 组合H窗口控制类
    /// </summary>
    public class CombinationHWindowControl<T>
    {
        string source = "CombinationHWindowControl";
        List<T> displayWindowClasses = new List<T>();
        int displayWindowClasses_Index = 0;

        public CombinationHWindowControl(string _source, List<T> _displayWindowClasses)
        {
            source = _source;
            displayWindowClasses = _displayWindowClasses;
        }
        /// <summary>
        /// 根据图像窗体队列依次获取图像窗体
        /// </summary>     
        /// <returns></returns>
        public T GetDisplayWindowClass()
        {

            T displayWindowClass = default(T);

            try
            {
                if (displayWindowClasses == null) return default(T);
                if (displayWindowClasses.Count <= 0) return default(T);

                if (displayWindowClasses_Index >= displayWindowClasses.Count || displayWindowClasses_Index < 0)
                {
                    displayWindowClasses_Index = 0;
                }

               
                for (int i = 0; i < displayWindowClasses.Count; i++)
                {
                    displayWindowClass = displayWindowClasses[0];
                    if (displayWindowClasses_Index == i)
                    {
                        displayWindowClass = displayWindowClasses[i];
                        break;
                    }

                }
                displayWindowClasses_Index++;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(source, ex);
            }
            return displayWindowClass;
        }
    }
}
