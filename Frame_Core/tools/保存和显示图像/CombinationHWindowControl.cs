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
    public class CombinationHWindowControl
    {
        string source = "CombinationHWindowControl";
        List<DisplayWindowClass> displayWindowClasses = new List<DisplayWindowClass>();
        int displayWindowClasses_Index = 0;

        public CombinationHWindowControl(string _source, List<DisplayWindowClass> _displayWindowClasses)
        {
            source = _source;
            displayWindowClasses = _displayWindowClasses;
        }
        /// <summary>
        /// 根据图像窗体队列依次获取图像窗体
        /// </summary>     
        /// <returns></returns>
        public DisplayWindowClass GetDisplayWindowClass()
        {

            DisplayWindowClass displayWindowClass = null;

            try
            {
                if (displayWindowClasses == null) return null;
                if (displayWindowClasses.Count <= 0) return null;

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
