using HalconDotNet;
using LogTool;
using NPOI.SS.Formula.Functions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Frame_Core
{
    public class BaseProDevice
    {
        [DllImport("kernel32.dll")]
        protected static extern UIntPtr SetThreadAffinityMask(IntPtr hThread, UIntPtr dwThreadAffinityMask);

        //得到当前线程的handler
        [DllImport("kernel32.dll")]
        protected static extern IntPtr GetCurrentThread();
        protected static ulong SetCpuID(int id)
        {
            ulong cpuid = 0;
            if (id < 0 || id >= System.Environment.ProcessorCount)
            {
                id = 0;
            }
            cpuid |= 1UL << id;//0,1,2,3=>1,2,4,8
            return cpuid;
        }

        /// <summary>
        /// 相机个数
        /// </summary>
        public int m_nCamCount = 0;
        /// <summary>
        /// 流程个数
        /// </summary>
        public int m_nProcessCount = 0;



        public Dictionary<int, BaseProParam> m_DicCamOperaParam = new Dictionary<int, BaseProParam>();

        public Dictionary<int, BaseProParam> m_DicCamOperaParam_Sim = new Dictionary<int, BaseProParam>();



        //INIProduct初始化事件
        public event Action m_eventINIProduct;


        //初始化仿真事件
        public event Action m_eventInitSim;

        public virtual void INIPro()
        {

            //m_stdCraftInfo = Project.Instance.craftInfo;  //加载系统参数之后加载，选择配方后加载
            //m_CraftBase = Project.Instance.craftBase;
            //  m_stdCalibrationInfo = Project.Instance.calibrationInfo;//加载系统参数之后加载，修改标定参数后加载         
            //加载仿真图
        }

        public virtual void BuildProcessServiceList()
        {

        }

        public virtual bool INIProduct()
        {
            return true; ;
        }

        public virtual bool InitProduct_Sim()
        {
            return true; ;
        }


        #region 通用功能工具方法
        //功能块
        public void NormalizaLocation(int LineID, double Line, double ImageHeight, out int standID, out double StandLine)
        {
            while (Line < 0)
            {
                Line += ImageHeight;
                LineID -= 1;
            }
            while (Line > ImageHeight)
            {
                Line -= ImageHeight;
                LineID += 1;
            }
            standID = LineID;
            StandLine = Line;
        }

        /// <summary>
        /// 针对可变行做的位置计算，Line不能为负数，Line不能超过2*ImageHeight的行
        /// </summary>
        /// <param name="LineID"></param>
        /// <param name="Line"></param>
        /// <param name="ImageHeight1"></param>
        /// <param name="ImageHeight2"></param>
        /// <param name="standID"></param>
        /// <param name="StandLine"></param>
        public void NormalizaLocation2(int LineID, double Line, double ImageHeight, out int standID, out double StandLine)
        {

            if (Line < 0)
            {
                Log4netRecord.ErrorLog.Info("NormalizaLocation2():LineID<0");
            }
            while (Line > ImageHeight)
            {
                Line -= ImageHeight;
                LineID += 1;
            }
            standID = LineID;
            StandLine = Line;
        }


        //功能块
        public void NormalizaLocation(ref LineInfo lineInfo, double offsetLine, double ImageHeight)
        {
            NormalizaLocation(lineInfo.RowBeginID, lineInfo.RowBegin + offsetLine, ImageHeight, out lineInfo.RowBeginID, out lineInfo.RowBegin);
            NormalizaLocation(lineInfo.RowEndID, lineInfo.RowEnd + offsetLine, ImageHeight, out lineInfo.RowEndID, out lineInfo.RowEnd);

        }
        /// <summary>
        /// 用于计算帧图Y方向绝对位置以及跨帧Y绝对像素计算
        /// </summary>
        /// <param name="startID"></param>
        /// <param name="startPixel"></param>
        /// <param name="endID"></param>
        /// <param name="endPixel"></param>
        /// <param name="ImageHeight"></param>
        /// <returns></returns>
        public double GetOffsetPosPixel(int startID, double startPixel, int endID, double endPixel, double ImageHeight)
        {
            double OffsetPos = 0;
            if (startID > endID)
            {
                double y1 = ImageHeight - endPixel;
                double y2 = startPixel;
                OffsetPos = (Math.Abs((startID - endID)) - 1) * ImageHeight + (y1 + y2);
                OffsetPos = -OffsetPos;

            }
            if (startID < endID)
            {
                double y1 = ImageHeight - startPixel;
                double y2 = endPixel;
                OffsetPos = (Math.Abs((startID - endID)) - 1) * ImageHeight + (y1 + y2);

            }
            if (startID == endID)
            {
                OffsetPos = endPixel - startPixel;
            }
            return OffsetPos;
        }

        public int GetCamID(ProcessInfo info)
        {
            int id = 0;
            for (int i = 0; i < m_nCamCount; i++)
            {
                bool flag = info.ProcessName.Contains(((CameraEnum)i).ToString());
                if (flag)
                {
                    id = i; break;
                }
            }
            return id;
        }

        public bool CalculateLinesX(LineInfo lineInfo1, LineInfo lineInfo2, out double Val)
        {
            Val = -999;
            try
            {

                double line1midcol = (lineInfo1.ColBegin + lineInfo1.ColEnd) / 2;
                double line2midcol = (lineInfo2.ColBegin + lineInfo2.ColEnd) / 2;
                Val = Math.Abs(line1midcol - line2midcol); ;
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public bool CalculateLines(CameraEnum cameraEnum, LineInfo lineInfo1, LineInfo lineInfo2, string Edtype, double coefficient, double RealFixOffset, out double Val)
        {
            Val = -999;
            try
            {

                //double calibValueX = Project.Instance.M_CameraRelevancies[cameraEnum].m_CamParam.CalibrationValue_X;
                //double calibValueY = Project.Instance.M_CameraRelevancies[cameraEnum].m_CamParam.CalibrationValue_Y;
                //if (Edtype == EditorTypeEnum.线距离X.ToString())
                //{
                //    double line1midcol = (lineInfo1.ColBegin + lineInfo1.ColEnd) / 2;
                //    double line2midcol = (lineInfo2.ColBegin + lineInfo2.ColEnd) / 2;
                //    Val = line1midcol - line2midcol;
                //    Val = Val * calibValueX * coefficient + RealFixOffset;
                //}
                //else if (Edtype == EditorTypeEnum.线距离Y.ToString())
                //{
                //    double line1midrow = (lineInfo1.RowBegin + lineInfo1.RowEnd) / 2;
                //    double line2midrow = (lineInfo2.RowBegin + lineInfo2.RowEnd) / 2;
                //    Val = line1midrow - line2midrow;
                //    Val = Val * calibValueY * coefficient + RealFixOffset;
                //}
                //else if (Edtype == EditorTypeEnum.线线夹角.ToString())
                //{
                //    HOperatorSet.AngleLl(lineInfo1.RowBegin, lineInfo1.ColBegin, lineInfo1.RowEnd, lineInfo1.ColEnd, lineInfo2.RowBegin, lineInfo2.ColBegin, lineInfo2.RowEnd, lineInfo2.ColEnd, out HTuple angle);
                //    Val = angle.TupleDeg();
                //    Val = Val * coefficient + RealFixOffset;
                //}

                //Val = Math.Round(Val, 3);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 计算点线距离，模切工位使用此方法。
        /// </summary>
        /// <param name="cameraEnum"></param>
        /// <param name="lineInfo1"></param>
        /// <param name="lineInfo2"></param>
        /// <param name="Edtype"></param>
        /// <param name="coefficient"></param>
        /// <param name="RealFixOffset"></param>
        /// <param name="Val"></param>
        /// <returns></returns>
        public bool CalculateLinesPl(int camIndex, LineInfo lineInfo1, LineInfo lineInfo2, string Edtype, double coefficient, double RealFixOffset, out double Val)
        {
            Val = -999;
            try
            {

                double calibValueX = Service_Camera.GetCam_CalibrationValue_X(camIndex);
                double calibValueY = Service_Camera.GetCam_CalibrationValue_Y(camIndex);
                double line1midcol = (lineInfo1.ColBegin + lineInfo1.ColEnd) / 2;
                double line2midcol = (lineInfo2.ColBegin + lineInfo2.ColEnd) / 2;
                double line1midrow = (lineInfo1.RowBegin + lineInfo1.RowEnd) / 2;
                double line2midrow = (lineInfo2.RowBegin + lineInfo2.RowEnd) / 2;

                if (Edtype == EditorTypeEnum.线距离X.ToString())
                {
                    int b = line1midcol > line2midcol ? 1 : -1;
                    HOperatorSet.DistancePl(line1midrow, line1midcol, lineInfo2.RowBegin, lineInfo2.ColBegin, lineInfo2.RowEnd, lineInfo2.ColEnd, out HTuple dist);
                    Val = dist.D;
                    Val = Val * b;
                    Val = Val * calibValueX * coefficient + RealFixOffset;
                }
                else if (Edtype == EditorTypeEnum.线距离Y.ToString())
                {
                    int b = line1midrow > line2midrow ? 1 : -1;
                    HOperatorSet.DistancePl(line1midrow, line1midcol, lineInfo2.RowBegin, lineInfo2.ColBegin, lineInfo2.RowEnd, lineInfo2.ColEnd, out HTuple dist);
                    Val = dist.D;
                    Val = Val * b;
                    Val = Val * calibValueY * coefficient + RealFixOffset;
                }

                Val = Math.Round(Val, 3);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool CalculateLines(CameraRelevancy cameraRelevancy, LineInfo lineInfo1, LineInfo lineInfo2, string Edtype, out double Val)
        {
            Val = -999;
            try
            {
                double line1midcol = (lineInfo1.ColBegin + lineInfo1.ColEnd) / 2;
                double line1midrow = (lineInfo1.RowBegin + lineInfo1.RowEnd) / 2;
                double line2midcol = (lineInfo2.ColBegin + lineInfo2.ColEnd) / 2;
                double line2midrow = (lineInfo2.RowBegin + lineInfo2.RowEnd) / 2;

                if (cameraRelevancy.param_CalibrationPlate.hv_CameraParam == null || cameraRelevancy.param_CalibrationPlate.hv_CameraParam.Length == 0) return false;
                if (cameraRelevancy.param_CalibrationPlate.hv_WorldPose == null || cameraRelevancy.param_CalibrationPlate.hv_WorldPose.Length == 0) return false;
                bool p1 = ImageAlgorithm.GetImageCoordToWorldCoord(cameraRelevancy.param_CalibrationPlate.hv_CameraParam,
                          cameraRelevancy.param_CalibrationPlate.hv_WorldPose, lineInfo1.RowBegin, lineInfo1.ColBegin, out double RowBegin1, out double ColBegin1);

                bool p2 = ImageAlgorithm.GetImageCoordToWorldCoord(cameraRelevancy.param_CalibrationPlate.hv_CameraParam,
                      cameraRelevancy.param_CalibrationPlate.hv_WorldPose, lineInfo1.RowEnd, lineInfo1.ColEnd, out double RowEnd1, out double ColEnd1);

                bool p3 = ImageAlgorithm.GetImageCoordToWorldCoord(cameraRelevancy.param_CalibrationPlate.hv_CameraParam,
                        cameraRelevancy.param_CalibrationPlate.hv_WorldPose, lineInfo2.RowBegin, lineInfo2.ColBegin, out double RowBegin2, out double ColBegin2);

                bool p4 = ImageAlgorithm.GetImageCoordToWorldCoord(cameraRelevancy.param_CalibrationPlate.hv_CameraParam,
                      cameraRelevancy.param_CalibrationPlate.hv_WorldPose, lineInfo2.RowEnd, lineInfo2.ColEnd, out double RowEnd2, out double ColEnd2);

                if (Edtype == EditorTypeEnum.线距离X.ToString())
                {
                    if (p1 && p2)
                    {
                        double point1 = ((ColBegin1 + ColEnd1) / 2);
                        double point2 = ((ColBegin2 + ColEnd2) / 2);
                        Val = point1 - point2;

                    }

                }
                else if (Edtype == EditorTypeEnum.线距离Y.ToString())
                {
                    if (p1 && p2)
                    {
                        double point1 = ((RowBegin1 + RowEnd1) / 2);
                        double point2 = ((RowBegin2 + RowEnd2) / 2);
                        Val = point1 - point2;
                    }
                }
                Val = Math.Round(Val, 3);
                return true;


            }
            catch (Exception)
            {
                return false;
            }

        }


        /// <summary>
        /// 从抓边插件到lineinfo和显示
        /// </summary>
        /// <param name="measureLineParamsTile"></param>
        /// <param name="bIsSucessfull"></param>
        /// <param name="lineInfo"></param>
        /// <param name="lineName"></param>
        /// <param name="messageShowInfos"></param>
        /// <param name="lineShowInfos"></param>
        /// <param name="regionShowInfos"></param>
        /// <returns></returns>
        public bool DetectEdgeValueAssign(ParamsMeasureLine measureLineParamsTile, bool bIsSucessfull, LineInfo lineInfo, string lineName, List<MessageShowInfo> messageShowInfos, List<LineInfo> lineShowInfos, List<RegionShowInfo> regionShowInfos)
        {
            HTuple starTime = 0, stopTime = 0;
            HOperatorSet.CountSeconds(out starTime);
            bool IsExecuteSuccess = false;
            ParamsMeasureLine MeasureLineParamsTile;
            MeasureLineParamsTile = measureLineParamsTile;
            if (bIsSucessfull)
            {
                lineInfo.StrLineName = lineName;
                lineInfo.IsLineSuccessfull = true;
                lineInfo.RowBegin = MeasureLineParamsTile.hv_LineRowBegin;
                lineInfo.ColBegin = MeasureLineParamsTile.hv_LineColumnBegin;
                lineInfo.RowEnd = MeasureLineParamsTile.hv_LineRowEnd;
                lineInfo.ColEnd = MeasureLineParamsTile.hv_LineColumnEnd;
                lineInfo.m_nLineWidth = 10;
                lineInfo.Color = HalconColor.yellow;

                //显示抓边矩形ROI
                //HOperatorSet.TupleRad(-MeasureLineParamsTile.hv_RoiAngle, out HTuple rad);
                //HOperatorSet.GenRectangle2(out HObject rectangle, MeasureLineParamsTile.hv_RoiMidRow, MeasureLineParamsTile.hv_RoiMidColumn, rad, MeasureLineParamsTile.hv_RoiHeight / 2, MeasureLineParamsTile.hv_RoiWidth / 2);
                //regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangle, m_stuGetColor = HalconColor.blue });
                //显示抓边名称
                //messageShowInfos.Add(new MessageShowInfo { m_strMessage = lineName, m_fRow = MeasureLineParamsTile.hv_RoiMidRow, m_fCol = MeasureLineParamsTile.hv_RoiMidColumn, m_nFontSize = 10, m_stuGetColor = HalconColor.yellow, m_bIsWindow = false });

                lineShowInfos.Add(lineInfo);
            }
            else
            {

                lineInfo.StrLineName = lineName;
                lineInfo.IsLineSuccessfull = false;
                lineInfo.RowBegin = MeasureLineParamsTile.hv_RoiMidRow;
                lineInfo.ColBegin = MeasureLineParamsTile.hv_RoiMidColumn;
                lineInfo.RowEnd = MeasureLineParamsTile.hv_RoiMidRow + 2;
                lineInfo.ColEnd = MeasureLineParamsTile.hv_RoiMidColumn + 2;
                lineInfo.Color = HalconColor.red;
                if (messageShowInfos != null && lineShowInfos != null)
                {
                    messageShowInfos.Add(new MessageShowInfo { m_strMessage = lineName + ":抓边未成功", m_stuGetColor = HalconColor.orange });
                    lineShowInfos.Add(lineInfo);
                    HOperatorSet.TupleRad(-MeasureLineParamsTile.hv_RoiAngle, out HTuple rad);
                    HOperatorSet.GenRectangle2(out HObject rectangle, MeasureLineParamsTile.hv_RoiMidRow, MeasureLineParamsTile.hv_RoiMidColumn, rad, MeasureLineParamsTile.hv_RoiHeight / 2, MeasureLineParamsTile.hv_RoiWidth / 2);
                    regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangle, m_stuGetColor = HalconColor.blue });
                }

            }
            HOperatorSet.CountSeconds(out stopTime);
            if (Project.Instance.Is_bShowStationTiming)
            {
                messageShowInfos.Add(new MessageShowInfo { m_strMessage = lineName + ":抓边处理时长：" + ((stopTime - starTime) * 1000).D.ToString("0.00") + "ms", m_stuGetColor = HalconColor.green });

            }
            return IsExecuteSuccess;
        }

        public bool DetectEdgeValueAssign(ParamsMeasureLine measureLineParamsTile, bool bIsSucessfull, LineInfo lineInfo, List<MessageShowInfo> messageShowInfos, List<LineInfo> lineShowInfos, List<RegionShowInfo> regionShowInfos)
        {
            HTuple starTime = 0, stopTime = 0;

            bool IsExecuteSuccess = false;
            ParamsMeasureLine MeasureLineParamsTile;
            MeasureLineParamsTile = measureLineParamsTile;
            if (bIsSucessfull)
            {
                lineInfo.IsLineSuccessfull = true;
                lineInfo.RowBegin = MeasureLineParamsTile.hv_LineRowBegin;
                lineInfo.ColBegin = MeasureLineParamsTile.hv_LineColumnBegin;
                lineInfo.RowEnd = MeasureLineParamsTile.hv_LineRowEnd;
                lineInfo.ColEnd = MeasureLineParamsTile.hv_LineColumnEnd;
                lineInfo.m_nLineWidth = 10;
                lineInfo.Color = HalconColor.yellow;

                lineShowInfos.Add(lineInfo);
            }
            else
            {

                lineInfo.IsLineSuccessfull = false;
                lineInfo.RowBegin = MeasureLineParamsTile.hv_RoiMidRow;
                lineInfo.ColBegin = MeasureLineParamsTile.hv_RoiMidColumn;
                lineInfo.RowEnd = MeasureLineParamsTile.hv_RoiMidRow + 2;
                lineInfo.ColEnd = MeasureLineParamsTile.hv_RoiMidColumn + 2;
                lineInfo.Color = HalconColor.red;
                if (messageShowInfos != null && lineShowInfos != null)
                {
                    messageShowInfos.Add(new MessageShowInfo { m_strMessage = lineInfo.StrLineName + ":抓边未成功", m_stuGetColor = HalconColor.orange });
                    lineShowInfos.Add(lineInfo);
                    HOperatorSet.TupleRad(-MeasureLineParamsTile.hv_RoiAngle, out HTuple rad);
                    HOperatorSet.GenRectangle2(out HObject rectangle, MeasureLineParamsTile.hv_RoiMidRow, MeasureLineParamsTile.hv_RoiMidColumn, rad, MeasureLineParamsTile.hv_RoiHeight / 2, MeasureLineParamsTile.hv_RoiWidth / 2);
                    regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangle, m_stuGetColor = HalconColor.blue });
                }

            }

            if (Project.Instance.Is_bShowStationTiming)
            {
                messageShowInfos.Add(new MessageShowInfo { m_strMessage = lineInfo.StrLineName + ":抓边处理时长：" + (MeasureLineParamsTile.m_fElapsedMs.ToString("0.00")) + "ms", m_stuGetColor = HalconColor.green });

            }
            return IsExecuteSuccess;
        }

        public bool DetectEdgeValueAssign(ParamsMeasureLine measureLineParamsTile, bool bIsSucessfull, LineInfo lineInfo, ShowHalconWindow showHalconWindow)
        {
            HTuple starTime = 0, stopTime = 0;
            HOperatorSet.CountSeconds(out starTime);
            bool IsExecuteSuccess = false;
            ParamsMeasureLine MeasureLineParamsTile;
            MeasureLineParamsTile = measureLineParamsTile;
            if (bIsSucessfull)
            {
                lineInfo.IsLineSuccessfull = true;
                lineInfo.RowBegin = MeasureLineParamsTile.hv_LineRowBegin;
                lineInfo.ColBegin = MeasureLineParamsTile.hv_LineColumnBegin;
                lineInfo.RowEnd = MeasureLineParamsTile.hv_LineRowEnd;
                lineInfo.ColEnd = MeasureLineParamsTile.hv_LineColumnEnd;
                lineInfo.m_nLineWidth = 10;
                lineInfo.Color = HalconColor.yellow;

                showHalconWindow.m_lineShowInfos.Add(lineInfo.Copy());
            }
            else
            {

                lineInfo.IsLineSuccessfull = false;
                lineInfo.RowBegin = MeasureLineParamsTile.hv_RoiMidRow;
                lineInfo.ColBegin = MeasureLineParamsTile.hv_RoiMidColumn;
                lineInfo.RowEnd = MeasureLineParamsTile.hv_RoiMidRow + 2;
                lineInfo.ColEnd = MeasureLineParamsTile.hv_RoiMidColumn + 2;
                lineInfo.Color = HalconColor.red;
                if (showHalconWindow.m_messageShowInfos != null && showHalconWindow.m_lineShowInfos != null)
                {
                    showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo { m_strMessage = lineInfo.StrLineName + ":抓边未成功", m_stuGetColor = HalconColor.orange });
                    showHalconWindow.m_lineShowInfos.Add(lineInfo.Copy());
                    HOperatorSet.TupleRad(-MeasureLineParamsTile.hv_RoiAngle, out HTuple rad);
                    HOperatorSet.GenRectangle2(out HObject rectangle, MeasureLineParamsTile.hv_RoiMidRow, MeasureLineParamsTile.hv_RoiMidColumn, rad, MeasureLineParamsTile.hv_RoiHeight / 2, MeasureLineParamsTile.hv_RoiWidth / 2);
                    showHalconWindow.m_regionShowInfos.Add(new RegionShowInfo() { m_hRegion = rectangle, m_stuGetColor = HalconColor.blue });
                }

            }
            HOperatorSet.CountSeconds(out stopTime);
            if (Project.Instance.Is_bShowStationTiming)
            {
                showHalconWindow.m_messageShowInfos.Add(new MessageShowInfo { m_strMessage = lineInfo.StrLineName + ":抓边处理时长：" + (MeasureLineParamsTile.m_fElapsedMs.ToString("0.00")) + "ms", m_stuGetColor = HalconColor.green });

            }
            return IsExecuteSuccess;
        }



        /// <summary>
        /// 求线线交点
        /// </summary>
        /// <param name="lineInfo1"></param>
        /// <param name="lineInfo2"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool IntersectionLines(LineInfo lineInfo1, LineInfo lineInfo2, out double row, out double column)
        {
            try
            {
                HOperatorSet.IntersectionLines(lineInfo1.RowBegin, lineInfo1.ColBegin, lineInfo1.RowEnd, lineInfo1.ColEnd,
                             lineInfo2.RowBegin, lineInfo2.ColBegin, lineInfo2.RowEnd, lineInfo2.ColEnd, out HTuple rowCross,
                             out HTuple colCross, out HTuple isOverlapping1);
                try
                {
                    row = 0;
                    column = 0;
                    if (isOverlapping1 == 0)
                    {
                        row = rowCross.D;
                        column = colCross.D;
                        return true;
                    }
                    return false;
                }
                catch (Exception)
                {
                    row = 0;
                    column = 0;
                    return false;
                }



            }
            catch (Exception)
            {
                row = 0;
                column = 0;
                return false;
            }

        }

        #endregion


    }
}
