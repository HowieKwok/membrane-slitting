using Frame_Core;
using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Mef_VisionFrame
{
    public sealed class CorrectPosDeal
    {
        public double fRunLength { get; set; } = 100;
        public double fPieWidth { get; set; } = 100;
        public int nPieNum { get; set; } = 5;

        public double CorrectValue { get; set; }

        /// <summary>
        /// 输出的纠偏值
        /// </summary>
        CorrecInfo info { get; set; } = new CorrecInfo();

        public List<CorrecInfo> correcInfos { get; } = new List<CorrecInfo>();
        public List<CorrecInfo> CutPieces { get; } = new List<CorrecInfo>();

        object objLock = new object();


        // Update 完成队列计算后通知界面刷新。
        public event Action<CorrectPosDeal, CorrecInfo> Updated;

        public CorrectPosDeal(int _nPieNum, double _fPieWidth)
        {
            Init(_nPieNum, _fPieWidth);
        }


        /// <summary>
        /// 纠偏值写入，刷新数据，返回false就说明必须清理队列，队列值异常
        /// </summary>
        /// <param name="frameId"></param>
        /// <param name="value"></param>
        /// <param name="notTab"></param>
        /// <returns></returns>
        public bool RefreshValue(int frameId, double value, bool notTab, bool findTab)
        {
            lock (objLock)
            {
                CorrecInfo added = new CorrecInfo
                {
                    nFrameID = frameId,
                    DetectionValue = value,
                    Value = value + fPieWidth * nPieNum - fRunLength,
                    nMode = 1,
                    NotTab = notTab,
                    FindTab = findTab

                };
                added.CorrectPosChange.Add(added.Value);
                added.Displacement.Add(fRunLength);


                correcInfos.Add(added);

                CorrectValue = value;

                // 切后展示只保存从原队列移出的裁片，最新裁片在最前。
                if (correcInfos.Count > nPieNum)
                {
                    //片数异常，此时输出异常，需要强制清理队列
                    return false;
                }

                Action<CorrectPosDeal, CorrecInfo> handler = Updated;
                if (handler != null) handler(this, info);
                return true;
            }

        }



        public bool RefreshValue(int frameId, double value, bool notTab, bool findTab,double correct=0)
        {
            lock (objLock)
            {
                try
                {
                   


                    fRunLength = fPieWidth;
              
                    correcInfos[1].Value = correct + fPieWidth;
                    correcInfos[1].nFrameID = frameId;
                    correcInfos[1].NotTab = notTab;
                    correcInfos[1].FindTab = findTab;

                    return true;
                }
                catch (Exception ee)
                {
                    MessageNotify.AddErrorLog("RefreshValue()报错"+ee.Message);

                    return false;
                }
           
              
            }
        }

        /// <summary>
        /// 获取队列第一片信息
        /// </summary>
        /// <returns></returns>
        public CorrecInfo GetCorrecInfo()
        {
            lock (objLock)
            {
                CorrecInfo correcInfo = correcInfos[0];
                //MessageNotify.AddCommLog($"获取{Name}，帧号：{correcInfo?.nFrameID}");
                return correcInfo;
            }

        }



        /// <summary>
        /// 获取纠偏值，用于回调使用
        /// </summary>
        /// <returns></returns>
        public CorrecInfo GetOffsetValueAndRemove(bool changeflag = false, double lastRun = 0, double correct = 0)
        {
            lock (objLock)
            {
                info = new CorrecInfo();
                if (correcInfos[0].Value <= 0.4 * fPieWidth)
                {
                    info = correcInfos[0];
                }
                else
                {
                    info.Value = 0;
                    info.FailFlag = 1;
                    Log4netRecord.ErrorLog.Debug($"当前队列数为：{correcInfos.Count}_纠偏值为：{correcInfos[0].Value}，数据存在异常");

                }

                if (changeflag)
                {
                    info.Value = correct - info.Value;
                }

                correcInfos.Remove(info);


                fRunLength = fPieWidth + info.Value;
                if (changeflag)
                {
                    fRunLength = lastRun;
                }
                foreach (CorrecInfo item in correcInfos)
                {
                    item.Value -= fRunLength;
                    item.CorrectPosChange.Add(item.Value);
                    item.Displacement.Add(fRunLength);
                }
                CutPieces.Insert(0, info);
                while (CutPieces.Count > 2) CutPieces.RemoveAt(CutPieces.Count - 1);
                Action<CorrectPosDeal, CorrecInfo> handler = Updated;
                if (handler != null) handler(this, info);

                return info;
            }
        }

        public bool Remove()
        {
            lock (objLock)
            {
                if (correcInfos.Count == (nPieNum + 1))
                {
                    correcInfos.RemoveAt(0);
                    return true;
                }
                else
                {
                    //数据无对应，有问题
                    Log4netRecord.ErrorLog.Debug($"当前数量为{correcInfos.Count}，设定片数为{nPieNum}");
                    return false;
                }


            }
        }

        /// <summary>
        /// 复位缓存数据
        /// </summary>
        public void ResetValues()
        {
            lock (objLock)
            {
                correcInfos?.Clear();

                for (int i = 0; i < nPieNum; i++)
                    correcInfos.Add(new CorrecInfo { Value = i * fPieWidth, nFrameID = -(i + 1) });
            }

        }

        public void Init(int _nPieNum, double _fPieWidth)
        {
            nPieNum = _nPieNum;
            fPieWidth = _fPieWidth;
            fRunLength = _fPieWidth;
            ResetValues();
        }


        public CorrecInfo Update(int frameId, double value)
        {
            info = new CorrecInfo();
            if (correcInfos[0].Value <= 0.2 * fPieWidth)
            {
                info = correcInfos[0];
                correcInfos.Remove(info);
                fRunLength = fPieWidth + info.Value;
                foreach (CorrecInfo item in correcInfos)
                {
                    item.Value -= fRunLength;
                    item.CorrectPosChange.Add(item.Value);
                    item.Displacement.Add(fRunLength);
                }
            }

            CorrecInfo added = new CorrecInfo
            {
                nFrameID = frameId,
                DetectionValue = value,
                Value = value + fPieWidth * nPieNum - fRunLength,
                nMode = 1
            };
            added.CorrectPosChange.Add(added.Value);
            added.Displacement.Add(fRunLength);
            correcInfos.Add(added);
            CorrectValue = value;

            // 切后展示只保存从原队列移出的裁片，最新裁片在最前。
            CutPieces.Insert(0, info);
            while (CutPieces.Count > 2) CutPieces.RemoveAt(CutPieces.Count - 1);

            Action<CorrectPosDeal, CorrecInfo> handler = Updated;
            if (handler != null) handler(this, info);
            return info;
        }
    }
}
