using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frame_Core
{
    public static class Model_LineToLineDist
    {
        public static void FindAllLine(CameraEnum cameraEnum, HObject ho_Image, Param_LineToLineDist param_LineToLineDist, out List<LineInfo1> lineInfo1s)
        {
            lineInfo1s = new List<LineInfo1>();
            try
            {
                for (int i = 0; i < param_LineToLineDist.LineParams.Count; i++)
                {
                    ImageAlgorithm.FindLine(cameraEnum, ho_Image, param_LineToLineDist.LineParams[i], out LineInfo1 lineInfo1);
                    lineInfo1.StrLineName = param_LineToLineDist.LineParams[i].LineName;
                    lineInfo1s.Add(lineInfo1);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
            }
        }


        public static void FindAllLine_2(CameraEnum cameraEnum, HObject ho_Image, Param_LineToLineDist param_LineToLineDist, out List<LineInfo1> lineInfo1s)
        {
            lineInfo1s = new List<LineInfo1>();
            try
            {
                for (int i = 0; i < param_LineToLineDist.LineParams.Count; i++)
                {
                    ImageAlgorithm.FindLine(cameraEnum, ho_Image, param_LineToLineDist.LineParams[i], out List<LineInfo1> lineInfo1s_Temp);

                    for (int j = 0; j < lineInfo1s_Temp.Count; j++)
                    {
                        lineInfo1s.Add(lineInfo1s_Temp[j]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
            }
        }
        public static void FindAllLine(CameraEnum cameraEnum, HObject ho_Image, Param_LineToLineDist param_LineToLineDist, out List<LineInfo1> lineInfo1s, ref List<RegionShowInfo> regionShowInfos)
        {
            lineInfo1s = new List<LineInfo1>();
            try
            {
                for (int i = 0; i < param_LineToLineDist.LineParams.Count; i++)
                {
                    ImageAlgorithm.FindLine(cameraEnum, ho_Image, param_LineToLineDist.LineParams[i], out LineInfo1 lineInfo1, out HObject ho_Contours);
                    lineInfo1s.Add(lineInfo1);
                    regionShowInfos.Add(new RegionShowInfo() { m_hRegion = ho_Contours });
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
            }
        }
    

        public static void SizeLogicCalculate_2(CameraEnum cameraEnum, List<LineInfo1> lineInfo1s, Param_LineToLineDist param_LineToLineDist, out List<SizeInfo_P> sizeInfos)
        {
            sizeInfos = new List<SizeInfo_P>();
            try
            {
                double calibValueX = Service_Camera.GetCam_CalibrationValue_X((int)cameraEnum);
                double calibValueY = Service_Camera.GetCam_CalibrationValue_Y((int)cameraEnum);
                for (int i = 0; i < param_LineToLineDist.SizeLogicParams_UserDefineds.Count; i++)
                {
                    double CalibCompensateValue = 0;
                    double ScaleCalibValue = param_LineToLineDist.SizeLogicParams_UserDefineds[i].CoefficientValue;

                    SizeInfo_P sizeInfo = new SizeInfo_P();
                    //sizeInfo.SizeID = param_LineToLineDist.SizeLogicParams_UserDefineds[i].SizeID;
                    sizeInfo.name = param_LineToLineDist.SizeLogicParams_UserDefineds[i].Name;

                    string line1Name = param_LineToLineDist.SizeLogicParams_UserDefineds[i].Line1Name;
                    string line2Name = param_LineToLineDist.SizeLogicParams_UserDefineds[i].Line2Name;
                    double standardValue = param_LineToLineDist.SizeLogicParams_UserDefineds[i].StandardValue;
                    double toleranceValue = param_LineToLineDist.SizeLogicParams_UserDefineds[i].ToleranceValue;
                    double coefficientValue = param_LineToLineDist.SizeLogicParams_UserDefineds[i].CoefficientValue;

                    if (param_LineToLineDist.LineParams?.Where(p => p.LineName == line1Name).Count() > 0) line1Name = $"{line1Name}-[1]";
                    if (param_LineToLineDist.LineParams?.Where(p => p.LineName == line2Name).Count() > 0) line2Name = $"{line2Name}-[1]";
                    LineInfo1 lineInfo1 = lineInfo1s.Find(p => p.StrLineName == line1Name);
                    LineInfo1 lineInfo2 = lineInfo1s.Find(p => p.StrLineName == line2Name);

                    if (param_LineToLineDist.SizeLogicParams_UserDefineds[i].SizeCalculateLogic == (int)SizeComputationalLogicEnum.X)
                    {
                        if (lineInfo1 != null && lineInfo2 != null)
                        {
                            if (lineInfo1.IsLineSuccessfull && lineInfo2.IsLineSuccessfull)
                            {
                                double line1midcol = (lineInfo1.ColBegin + lineInfo1.ColEnd) / 2;
                                double line2midcol = (lineInfo2.ColBegin + lineInfo2.ColEnd) / 2;
                                sizeInfo.value = line2midcol - line1midcol;
                                sizeInfo.value = sizeInfo.value * calibValueX * ScaleCalibValue + CalibCompensateValue;
                                sizeInfo.executeResult.IsSuccess = true;

                                sizeInfo.result = Math.Abs(sizeInfo.value - standardValue) <= toleranceValue;
                            }
                        }
                        else
                        {
                            sizeInfo.result = false;
                            sizeInfo.executeResult.IsSuccess = false;
                            sizeInfo.executeResult.Message = (lineInfo1 == null ? "Edge1Name not found" : "");
                            sizeInfo.executeResult.Message += " | ";
                            sizeInfo.executeResult.Message += (lineInfo2 == null ? "Edge2Name not found" : "");
                        }
                    }
                    else /*if (param_LineToLineDist.SizeLogicParams_UserDefineds[i].SizeCalculateLogic == (int)SizeComputationalLogicEnum.Y)*/
                    {
                        if (lineInfo1 != null && lineInfo2 != null)
                        {
                            if (lineInfo1.IsLineSuccessfull && lineInfo2.IsLineSuccessfull)
                            {
                                double line1midrow = (lineInfo1.RowBegin + lineInfo1.RowEnd) / 2;
                                double line2midrow = (lineInfo2.RowBegin + lineInfo2.RowEnd) / 2;
                                sizeInfo.value = line2midrow - line1midrow;
                                sizeInfo.value = sizeInfo.value * calibValueY * ScaleCalibValue + CalibCompensateValue;
                                sizeInfo.executeResult.IsSuccess = true;

                                sizeInfo.result = Math.Abs(sizeInfo.value - standardValue) <= toleranceValue;
                            }
                        }
                        else
                        {
                            sizeInfo.result = false;
                            sizeInfo.executeResult.IsSuccess = false;
                            sizeInfo.executeResult.Message = (lineInfo1 == null ? "Edge1Name not found" : "");
                            sizeInfo.executeResult.Message += " | ";
                            sizeInfo.executeResult.Message += (lineInfo2 == null ? "Edge2Name not found" : "");
                        }

                    }

                    sizeInfo.value = Math.Round(sizeInfo.value, 2);
                    sizeInfos.Add(sizeInfo);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
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
        public static void DetectEdgeValueAssign(CameraEnum cameraEnum, List<LineInfo1> lineInfo1s,  List<MessageShowInfo> messageShowInfos,  List<LineInfo> lineShowInfos,  List<RegionShowInfo> regionShowInfos)
        {
            try
            {
                if (lineInfo1s == null) return;
                for (int i = 0; i < lineInfo1s.Count; i++)
                {
                    if (lineInfo1s[i].IsLineSuccessfull)
                    {
                        if (lineShowInfos != null)
                        {
                            lineShowInfos.Add(new LineInfo() { RowBegin = lineInfo1s[i].RowBegin, ColBegin = lineInfo1s[i].ColBegin, RowEnd = lineInfo1s[i].RowEnd, ColEnd = lineInfo1s[i].ColEnd, Color = HalconColor.green });
                        }
                    }
                    else
                    {
                        if (messageShowInfos != null)
                        {
                            string str1 = "抓边未成功";
                            messageShowInfos.Add(new MessageShowInfo { m_strMessage = lineInfo1s[i].StrLineName + $":{str1}", m_stuGetColor = HalconColor.red });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
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
        public static void DetectEdgeValueAssign(CameraEnum cameraEnum, List<SizeInfo_P> sizeInfos,  List<MessageShowInfo> messageShowInfos)
        {
            try
            {
                if (sizeInfos == null) return;
                if (messageShowInfos == null) return;
                for (int i = 0; i < sizeInfos.Count; i++)
                {
                    HalconColor halconColor = HalconColor.green;
                    if (!sizeInfos[i].executeResult.IsSuccess || !sizeInfos[i].result) halconColor = HalconColor.red;
                    MessageShowInfo messageShowInfo = new MessageShowInfo() { m_strMessage = sizeInfos[i].name + ":" + sizeInfos[i].value.ToString("0.000") + $"   {sizeInfos[i].executeResult.Message}", m_stuGetColor = halconColor };
                    messageShowInfos.Add(messageShowInfo);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
            }
        }



        public static void FindAllLine(CameraEnum cameraEnum, HObject ho_Image, List<Param_Line> lineParams, out List<LineInfo1> lineInfo1s)
        {
            lineInfo1s = new List<LineInfo1>();
            try
            {
                for (int i = 0; i < lineParams.Count; i++)
                {
                    ImageAlgorithm.FindLine(cameraEnum, ho_Image, lineParams[i], out List<LineInfo1> lineInfo1s_Temp);

                    for (int j = 0; j < lineInfo1s_Temp.Count; j++)
                    {
                        lineInfo1s.Add(lineInfo1s_Temp[j]);
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
            }
        }

        public static void SizeLogicCalculate(CameraEnum cameraEnum, List<LineInfo1> lineInfo1s, List<Param_Line> lineParams, List<Param_SizeLogic_UserDefined> param_SizeLogic_UserDefineds, out List<SizeInfo_P> sizeInfos)
        {
            sizeInfos = new List<SizeInfo_P>();
            try
            {
                double calibValueX = Service_Camera.GetCam_CalibrationValue_X((int)cameraEnum);
                double calibValueY = Service_Camera.GetCam_CalibrationValue_Y((int)cameraEnum);
                for (int i = 0; i < param_SizeLogic_UserDefineds.Count; i++)
                {
                    double CalibCompensateValue = 0;
                    double ScaleCalibValue = param_SizeLogic_UserDefineds[i].CoefficientValue;

                    SizeInfo_P sizeInfo = new SizeInfo_P();
                    //sizeInfo.SizeID =param_SizeLogic_UserDefineds[i].SizeID;
                    sizeInfo.name = param_SizeLogic_UserDefineds[i].Name;

                    string line1Name = param_SizeLogic_UserDefineds[i].Line1Name;
                    string line2Name = param_SizeLogic_UserDefineds[i].Line2Name;
                    double standardValue = param_SizeLogic_UserDefineds[i].StandardValue;
                    double toleranceValue = param_SizeLogic_UserDefineds[i].ToleranceValue;
                    double coefficientValue = param_SizeLogic_UserDefineds[i].CoefficientValue;


                    if (lineParams?.Where(p => p.LineName == line1Name).Count() > 0) line1Name = $"{line1Name}-[1]";
                    if (lineParams?.Where(p => p.LineName == line2Name).Count() > 0) line2Name = $"{line2Name}-[1]";
                    LineInfo1 lineInfo1 = lineInfo1s.Find(p => p.StrLineName == line1Name);
                    LineInfo1 lineInfo2 = lineInfo1s.Find(p => p.StrLineName == line2Name);

                    if (param_SizeLogic_UserDefineds[i].SizeCalculateLogic == (int)SizeComputationalLogicEnum.X)
                    {
                        if (lineInfo1 != null && lineInfo2 != null)
                        {
                            if (lineInfo1.IsLineSuccessfull && lineInfo2.IsLineSuccessfull)
                            {
                                double line1midcol = (lineInfo1.ColBegin + lineInfo1.ColEnd) / 2;
                                double line2midcol = (lineInfo2.ColBegin + lineInfo2.ColEnd) / 2;
                                sizeInfo.value = line2midcol - line1midcol;
                                sizeInfo.value = sizeInfo.value * calibValueX * ScaleCalibValue + CalibCompensateValue;
                                sizeInfo.executeResult.IsSuccess = true;

                                sizeInfo.result = Math.Abs(sizeInfo.value - standardValue) <= toleranceValue;
                            }
                        }
                        else
                        {
                            sizeInfo.result = false;
                            sizeInfo.executeResult.IsSuccess = false;
                            sizeInfo.executeResult.Message = (lineInfo1 == null ? "Edge1Name not found" : "");
                            sizeInfo.executeResult.Message += " | ";
                            sizeInfo.executeResult.Message += (lineInfo2 == null ? "Edge2Name not found" : "");
                        }
                    }
                    else /*if (param_LineToLineDist.SizeLogicParams_UserDefineds[i].SizeCalculateLogic == (int)SizeComputationalLogicEnum.Y)*/
                    {
                        if (lineInfo1 != null && lineInfo2 != null)
                        {
                            if (lineInfo1.IsLineSuccessfull && lineInfo2.IsLineSuccessfull)
                            {
                                double line1midrow = (lineInfo1.RowBegin + lineInfo1.RowEnd) / 2;
                                double line2midrow = (lineInfo2.RowBegin + lineInfo2.RowEnd) / 2;
                                sizeInfo.value = line2midrow - line1midrow;
                                sizeInfo.value = sizeInfo.value * calibValueY * ScaleCalibValue + CalibCompensateValue;
                                sizeInfo.executeResult.IsSuccess = true;

                                sizeInfo.result = Math.Abs(sizeInfo.value - standardValue) <= toleranceValue;
                            }
                        }
                        else
                        {
                            sizeInfo.result = false;
                            sizeInfo.executeResult.IsSuccess = false;
                            sizeInfo.executeResult.Message = (lineInfo1 == null ? "Edge1Name not found" : "");
                            sizeInfo.executeResult.Message += " | ";
                            sizeInfo.executeResult.Message += (lineInfo2 == null ? "Edge2Name not found" : "");
                        }

                    }

                    sizeInfo.value = Math.Round(sizeInfo.value, 3);
                    sizeInfos.Add(sizeInfo);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(cameraEnum.ToString(), ex);
            }
        }


    }
}
