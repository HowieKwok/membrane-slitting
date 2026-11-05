using Frame_Core;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Mef_VisionFrame
{
    public sealed class CorrecInfo
    {
        object _lock = new object();

        public string Name { get; set; } = "";
        /// <summary>
        /// 无极耳
        /// </summary>
        public bool NotTab { get; set; } = false;

        /// <summary>
        /// 寻极耳中
        /// </summary>
        public bool FindTab { get; set; } = false;

        public double DetectionValue { get; set; } = 0;

        public double Value { get; set; } = 0;

        /// <summary>
        /// 失败标志 0：成功 1：数值过大
        /// </summary>
        public int FailFlag = 0;


        public List<double> CorrectPosChange { get; } = new List<double>();

        public List<double> Displacement { get; } = new List<double>();
        public int nFrameID { get; set; } = -1;
        public int nMode { get; set; }
        public string ChangeText
        {
            get { return CorrectPosChange.Count == 0 ? "0.000" : string.Join(" -> ", CorrectPosChange.Select(x => x.ToString("0.000"))); }
        }

        public string DisplacementText
        {
            get { return Displacement.Count == 0 ? "0.000" : string.Join(" -> ", Displacement.Select(x => x.ToString("0.000"))); }
        }

       public CorrecInfo Copy()
        {
            return (CorrecInfo)MemberwiseClone();
        }


        public void UpdateData(CorrecInfo source)
        {          
            lock (_lock)
            {
                NotTab = source.NotTab;
                FindTab = source.FindTab;
                DetectionValue = source.DetectionValue;
                Value = source.Value;
                nFrameID = source.nFrameID;
                nMode = source.nMode;

                // 深拷贝列表数据
                CorrectPosChange.Clear();
                CorrectPosChange.AddRange(source.CorrectPosChange);

                Displacement.Clear();
                Displacement.AddRange(source.Displacement);
            }
        }
        /// <summary>
        /// 线程安全的获取快照
        /// </summary>
        public CorrecInfo GetSnapshot()
        {   
            lock (_lock)
            {
                return new CorrecInfo
                {
                    NotTab = NotTab,
                    FindTab = FindTab,
                    DetectionValue = DetectionValue,
                    Value = Value,
                    nFrameID = nFrameID,
                    nMode = nMode,
                 
                };
            }
        }

    
    }
}
