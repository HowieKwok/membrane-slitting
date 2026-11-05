using FrameCommunity;
using NPOI.SS.Formula.Functions;
using NPOI.XSSF.Streaming.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mef_VisionFrame
{
    public class CorrectLogic
    {
        object _lock = new object();
        bool FindTabState = false;

        private CorrecInfo SendCutCorrecInfo { get; set; } = new CorrecInfo();
        private CorrecInfo SendVCorrecInfo { get; set; } = new CorrecInfo();

        private List<CorrecInfo> Cut_CorrecInfo { get; } = new List<CorrecInfo>();
        private List<CorrecInfo> V_CorrecInfo { get; } = new List<CorrecInfo>();

        public void AddCut_CorrecInfo(int currentFrameID, int cutNum, int vNum, bool notTab, bool findTabState, double cutOffsetValue, double vOffsetValue)
        {
            FindTabState = findTabState;

            if (FindTabState)
            {
                SendCutCorrecInfo.Value = cutOffsetValue;
                SendVCorrecInfo.Value = vOffsetValue;
                SendVCorrecInfo.NotTab = notTab;
                return;
            }

            lock (_lock)
            {
                //剃片
                var cutCorrecInfoTemp = GetCut_CorrecInfo(currentFrameID, cutNum);
                var vCorrecInfoTemp = GetV_CorrecInfo(currentFrameID, vNum);

                //裁切刀
                CorrecInfo correcInfo = new CorrecInfo();
                correcInfo.nFrameID = currentFrameID;
                correcInfo.NotTab = notTab;
                correcInfo.Value = cutOffsetValue;
                Cut_CorrecInfo.Add(correcInfo);

                //V切刀
                CorrecInfo vCorrecInfo = new CorrecInfo();
                vCorrecInfo.nFrameID = currentFrameID;
                vCorrecInfo.NotTab = notTab;
                vCorrecInfo.Value = vOffsetValue;
                V_CorrecInfo.Add(vCorrecInfo);


                //位移
                for (int i = 0; i < Cut_CorrecInfo.Count; i++)
                {
                    Cut_CorrecInfo[i].Value -= cutCorrecInfoTemp.currentCorrecInfo.Value;
                }

                for (int i = 0; i < V_CorrecInfo.Count; i++)
                {
                    V_CorrecInfo[i].Value -= cutCorrecInfoTemp.currentCorrecInfo.Value;
                }

                //发送下一片的值
                {
                    SendCutCorrecInfo = cutCorrecInfoTemp.nextCorrecInfo;
                    vCorrecInfoTemp.nextCorrecInfo.Value -= cutCorrecInfoTemp.nextCorrecInfo.Value;
                    SendVCorrecInfo = vCorrecInfoTemp.nextCorrecInfo;
                }
                  
            }

        }


        public void GetCorrectData(out CorrecInfo cutCorrecInfo, out CorrecInfo vCorrecInfo)
        {
            lock (_lock)
            {
                cutCorrecInfo = SendCutCorrecInfo;
                vCorrecInfo = SendVCorrecInfo;
            }
        }

        private (CorrecInfo currentCorrecInfo, CorrecInfo nextCorrecInfo) GetCut_CorrecInfo(int currentFrameID, int num)
        {
            var targetFrameID = currentFrameID - num;
            CorrecInfo currentCorrecInfo = null;

            // 移除所有小于目标帧ID
            while (Cut_CorrecInfo.Count > 0 && Cut_CorrecInfo[0].nFrameID < targetFrameID)
            {
                Cut_CorrecInfo.RemoveAt(0);
            }

            // 检查是否存在目标帧
            if (Cut_CorrecInfo.Count > 0 && Cut_CorrecInfo[0].nFrameID == targetFrameID)
            {
                currentCorrecInfo = Cut_CorrecInfo[0];
                Cut_CorrecInfo.RemoveAt(0);
            }

            // 获取下一个矫正信息
            var nextCorrecInfo = Cut_CorrecInfo.Find(p => p.nFrameID == targetFrameID + 1) ?? new CorrecInfo();

            return (currentCorrecInfo ?? new CorrecInfo(), nextCorrecInfo);
        }

        private (CorrecInfo currentCorrecInfo, CorrecInfo nextCorrecInfo) GetV_CorrecInfo(int currentFrameID, int num)
        {
            var targetFrameID = currentFrameID - num;
            CorrecInfo currentCorrecInfo = null;

            // 移除所有小于目标帧ID
            while (V_CorrecInfo.Count > 0 && V_CorrecInfo[0].nFrameID < targetFrameID)
            {
                V_CorrecInfo.RemoveAt(0);
            }

            // 检查是否存在目标帧
            if (V_CorrecInfo.Count > 0 && V_CorrecInfo[0].nFrameID == targetFrameID)
            {
                currentCorrecInfo = V_CorrecInfo[0];
                V_CorrecInfo.RemoveAt(0);
            }

            // 获取下一个矫正信息
            var nextCorrecInfo = V_CorrecInfo.Find(p => p.nFrameID == targetFrameID + 1) ?? new CorrecInfo();

            return (currentCorrecInfo ?? new CorrecInfo(), nextCorrecInfo);

        }

        public void Clera()
        {
            lock (_lock)
            {
                SendCutCorrecInfo = new CorrecInfo();
                SendVCorrecInfo = new CorrecInfo();
                Cut_CorrecInfo.Clear();
                V_CorrecInfo.Clear();
            }
        }
    }
}
