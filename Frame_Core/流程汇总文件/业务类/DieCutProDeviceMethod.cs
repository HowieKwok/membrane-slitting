using Frame_Core;
using Frame_Core.pluginParams;
using FrameCommunity.Data;
using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core.业务类
{
    partial class DieCutProDevice : BaseProDevice
    {

        /// <summary>
        /// 执行粗定位插件并梳理结果，生成基准点、边线数据及可视化十字标记。
        /// </summary>
        /// <param name="imageInfo">当前处理的图像信息</param>
        /// <param name="plugin">已初始化的粗定位插件实例（需实现 IPlugin）</param>
        /// <param name="processID">当前流程ID</param>
        /// <param name="crossPointInfos">结果列表（引用类型），用于存储交点、线条等数据</param>
        /// <param name="showWindow">显示窗口对象，用于添加十字线和边线的可视化元素</param>
        /// <param name="moduleName">模块名称（用于显示命名），若为 null 则使用 plugin.moduleInfo.ModuleName</param>
        /// <returns>返回插件执行结果（true/false）</returns>
        public bool ProcessCoarseLocation(
            ImageInfo imageInfo,
            IPlugin plugin,
            int processID,
            List<OutCrossGroup> crossPointInfos,
            ShowHalconWindow showWindow, out OutCrossGroup PointCross,
            string moduleName = null, bool defaultFalg = false)
        {
            PointCross = new OutCrossGroup();
            if (plugin == null)
                return false;

            // 获取参数并设置图像和禁用位置跟随
            var paramsFindIntersection = plugin.PluginParams as ParamsFindIntersection;
            if (paramsFindIntersection == null)
                return false;

            paramsFindIntersection.ho_Image = imageInfo.ho_image;
            paramsFindIntersection.m_bUsePositionFollow = false;

            // 模块名（若未指定则使用插件自身的名称）
            string modName = moduleName ?? plugin.moduleInfo.ModuleName;

            // 初始化基准点（默认当前设定值）
            PointInfo pointBase = new PointInfo()
            {
                Col = paramsFindIntersection.m_fCurrentCol,
                Row = paramsFindIntersection.m_fCurrentRow
            };

            // 执行插件
            bool success = plugin.ExecuteObj();

            // 构建两条边线信息
            LineInfo lineInfo1, lineInfo2;

            if (success)
            {
                // 执行成功：使用实际交点坐标
                pointBase.Col = paramsFindIntersection.hv_IntersectionColumn;
                pointBase.Row = paramsFindIntersection.hv_IntersectionRow;

                lineInfo1 = new LineInfo()
                {
                    ColBegin = paramsFindIntersection.hv_Line1_ColumnBegin,
                    RowBegin = paramsFindIntersection.hv_Line1_RowBegin,
                    ColEnd = paramsFindIntersection.hv_Line1_ColumnEnd,
                    RowEnd = paramsFindIntersection.hv_Line1_RowEnd,
                    IsLineSuccessfull = paramsFindIntersection.bl_Line1_FitSuccess,
                    StrLineName = $"{modName}_抓边1",
                    strMoudleName = modName,
                    nProcessID = processID
                };

                lineInfo2 = new LineInfo()
                {
                    ColBegin = paramsFindIntersection.hv_Line2_ColumnBegin,
                    RowBegin = paramsFindIntersection.hv_Line2_RowBegin,
                    ColEnd = paramsFindIntersection.hv_Line2_ColumnEnd,
                    RowEnd = paramsFindIntersection.hv_Line2_RowEnd,
                    IsLineSuccessfull = paramsFindIntersection.bl_Line2_FitSuccess,
                    StrLineName = $"{modName}_抓边2",
                    strMoudleName = modName,
                    nProcessID = processID
                };
            }
            else
            {
                // 执行失败：分别判断每条边是否拟合成功
                if (paramsFindIntersection.bl_Line1_FitSuccess)
                {
                    lineInfo1 = new LineInfo()
                    {
                        ColBegin = paramsFindIntersection.hv_Line1_ColumnBegin,
                        RowBegin = paramsFindIntersection.hv_Line1_RowBegin,
                        ColEnd = paramsFindIntersection.hv_Line1_ColumnEnd,
                        RowEnd = paramsFindIntersection.hv_Line1_RowEnd,
                        IsLineSuccessfull = true,
                        StrLineName = $"{modName}_抓边1",
                        strMoudleName = modName,
                        nProcessID = processID
                    };
                }
                else
                {
                    lineInfo1 = new LineInfo()
                    {
                        ColBegin = paramsFindIntersection.hv_Line1_RoiMidColumn,
                        RowBegin = paramsFindIntersection.hv_Line1_RoiMidRow,
                        ColEnd = paramsFindIntersection.hv_Line1_RoiMidColumn + 200,
                        RowEnd = paramsFindIntersection.hv_Line1_RoiMidRow + 200,
                        IsLineSuccessfull = false,
                        StrLineName = $"{modName}_抓边1",
                        strMoudleName = modName,
                        nProcessID = processID
                    };
                }

                if (paramsFindIntersection.bl_Line2_FitSuccess)
                {
                    lineInfo2 = new LineInfo()
                    {
                        ColBegin = paramsFindIntersection.hv_Line2_ColumnBegin,
                        RowBegin = paramsFindIntersection.hv_Line2_RowBegin,
                        ColEnd = paramsFindIntersection.hv_Line2_ColumnEnd,
                        RowEnd = paramsFindIntersection.hv_Line2_RowEnd,
                        IsLineSuccessfull = true,
                        StrLineName = $"{modName}_抓边2",
                        strMoudleName = modName,
                        nProcessID = processID
                    };
                }
                else
                {
                    lineInfo2 = new LineInfo()
                    {
                        ColBegin = paramsFindIntersection.hv_Line2_RoiMidColumn,   // 修正为 Line2 的 ROI 中点
                        RowBegin = paramsFindIntersection.hv_Line2_RoiMidRow,
                        ColEnd = paramsFindIntersection.hv_Line2_RoiMidColumn + 200,
                        RowEnd = paramsFindIntersection.hv_Line2_RoiMidRow + 200,
                        IsLineSuccessfull = false,
                        StrLineName = $"{modName}_抓边2",
                        strMoudleName = modName,
                        nProcessID = processID
                    };
                }
            }
            //默认给出错误坐标
            if (defaultFalg)
            {
                pointBase.Col = -999;
                pointBase.Row = -999;

            }
            // 封装结果并添加到列表中
            OutCrossGroup outCrossGroup = new OutCrossGroup()
            {
                nProcessID = processID,
                ModuleName = modName,
                LineInfo1 = lineInfo1,
                LineInfo2 = lineInfo2,
                point = pointBase
            };
            crossPointInfos.Add(outCrossGroup);

            // 生成十字交叉线并添加到显示窗口
            HOperatorSet.GenCrossContourXld(out HObject cross, pointBase.Row, pointBase.Col, 10, 45);
            showWindow.m_regionShowInfos.Add(new RegionShowInfo()
            {
                m_hRegion = cross,
                m_stuGetColor = HalconColor.gold
            });
            showWindow.m_lineShowInfos.Add(lineInfo1.Copy());
            showWindow.m_lineShowInfos.Add(lineInfo2.Copy());
            PointCross = outCrossGroup;
            return success;
        }
        public bool ProcessCoarseLocation(
           ImageInfo imageInfo,
           IPlugin plugin,
           int processID,
           List<OutCrossGroup> crossPointInfos,
           ShowHalconWindow showWindow, out OutCrossGroup PointCross, PointInfo Point基准,
           PointInfo Point跟随,
           string moduleName = null, bool defaultFalg = false, bool follow = false)
        {
            PointCross = new OutCrossGroup();
            if (plugin == null)
                return false;

            // 获取参数并设置图像
            var paramsFindIntersection = plugin.PluginParams as ParamsFindIntersection;
            if (paramsFindIntersection == null)
                return false;

            paramsFindIntersection.ho_Image = imageInfo.ho_image;
            paramsFindIntersection.m_bUsePositionFollow = follow;
            paramsFindIntersection.m_fBaseCol = Point基准.Col;
            paramsFindIntersection.m_fBaseRow = Point基准.Row;
            paramsFindIntersection.m_fBaseAngle = 0;
            paramsFindIntersection.m_fCurrentCol = Point跟随.Col;
            paramsFindIntersection.m_fCurrentRow = Point跟随.Row;
            paramsFindIntersection.m_fCurrentAngle = 0;


            // 模块名（若未指定则使用插件自身的名称）
            string modName = moduleName ?? plugin.moduleInfo.ModuleName;

            // 初始化当前点（默认当前设定值）
            PointInfo pointCurrent = new PointInfo()
            {
                Col = paramsFindIntersection.m_fCurrentCol,
                Row = paramsFindIntersection.m_fCurrentRow
            };

            // 执行插件
            bool success = plugin.ExecuteObj();

            // 构建两条边线信息
            LineInfo lineInfo1, lineInfo2;

            if (success)
            {
                // 执行成功：使用实际交点坐标
                pointCurrent.Col = paramsFindIntersection.hv_IntersectionColumn;
                pointCurrent.Row = paramsFindIntersection.hv_IntersectionRow;
                pointCurrent.bIsSuccess = success;
                lineInfo1 = new LineInfo()
                {
                    ColBegin = paramsFindIntersection.hv_Line1_ColumnBegin,
                    RowBegin = paramsFindIntersection.hv_Line1_RowBegin,
                    ColEnd = paramsFindIntersection.hv_Line1_ColumnEnd,
                    RowEnd = paramsFindIntersection.hv_Line1_RowEnd,
                    IsLineSuccessfull = paramsFindIntersection.bl_Line1_FitSuccess,
                    StrLineName = $"{modName}_抓边1",
                    strMoudleName = modName,
                    nProcessID = processID
                };

                lineInfo2 = new LineInfo()
                {
                    ColBegin = paramsFindIntersection.hv_Line2_ColumnBegin,
                    RowBegin = paramsFindIntersection.hv_Line2_RowBegin,
                    ColEnd = paramsFindIntersection.hv_Line2_ColumnEnd,
                    RowEnd = paramsFindIntersection.hv_Line2_RowEnd,
                    IsLineSuccessfull = paramsFindIntersection.bl_Line2_FitSuccess,
                    StrLineName = $"{modName}_抓边2",
                    strMoudleName = modName,
                    nProcessID = processID
                };
            }
            else
            {
                // 执行失败：分别判断每条边是否拟合成功
                if (paramsFindIntersection.bl_Line1_FitSuccess)
                {
                    lineInfo1 = new LineInfo()
                    {
                        ColBegin = paramsFindIntersection.hv_Line1_ColumnBegin,
                        RowBegin = paramsFindIntersection.hv_Line1_RowBegin,
                        ColEnd = paramsFindIntersection.hv_Line1_ColumnEnd,
                        RowEnd = paramsFindIntersection.hv_Line1_RowEnd,
                        IsLineSuccessfull = true,
                        StrLineName = $"{modName}_抓边1",
                        strMoudleName = modName,
                        nProcessID = processID
                    };
                }
                else
                {
                    lineInfo1 = new LineInfo()
                    {
                        ColBegin = paramsFindIntersection.hv_Line1_RoiMidColumn,
                        RowBegin = paramsFindIntersection.hv_Line1_RoiMidRow,
                        ColEnd = paramsFindIntersection.hv_Line1_RoiMidColumn + 200,
                        RowEnd = paramsFindIntersection.hv_Line1_RoiMidRow + 200,
                        IsLineSuccessfull = false,
                        StrLineName = $"{modName}_抓边1",
                        strMoudleName = modName,
                        nProcessID = processID
                    };
                }

                if (paramsFindIntersection.bl_Line2_FitSuccess)
                {
                    lineInfo2 = new LineInfo()
                    {
                        ColBegin = paramsFindIntersection.hv_Line2_ColumnBegin,
                        RowBegin = paramsFindIntersection.hv_Line2_RowBegin,
                        ColEnd = paramsFindIntersection.hv_Line2_ColumnEnd,
                        RowEnd = paramsFindIntersection.hv_Line2_RowEnd,
                        IsLineSuccessfull = true,
                        StrLineName = $"{modName}_抓边2",
                        strMoudleName = modName,
                        nProcessID = processID
                    };
                }
                else
                {
                    lineInfo2 = new LineInfo()
                    {
                        ColBegin = paramsFindIntersection.hv_Line2_RoiMidColumn,   // 修正为 Line2 的 ROI 中点
                        RowBegin = paramsFindIntersection.hv_Line2_RoiMidRow,
                        ColEnd = paramsFindIntersection.hv_Line2_RoiMidColumn + 200,
                        RowEnd = paramsFindIntersection.hv_Line2_RoiMidRow + 200,
                        IsLineSuccessfull = false,
                        StrLineName = $"{modName}_抓边2",
                        strMoudleName = modName,
                        nProcessID = processID
                    };
                }

                //默认给出错误坐标
                if (defaultFalg)
                {
                    pointCurrent.Col = -999;
                    pointCurrent.Row = -999;


                }
            }
 
            // 封装结果并添加到列表中
            OutCrossGroup outCrossGroup = new OutCrossGroup()
            {
                nProcessID = processID,
                ModuleName = modName,
                LineInfo1 = lineInfo1,
                LineInfo2 = lineInfo2,
                point = pointCurrent,
                bIsSuccess=pointCurrent.bIsSuccess
            };
            crossPointInfos.Add(outCrossGroup);

            // 生成十字交叉线并添加到显示窗口
            HOperatorSet.GenCrossContourXld(out HObject cross, pointCurrent.Row, pointCurrent.Col, 10, 45);
            showWindow.m_regionShowInfos.Add(new RegionShowInfo()
            {
                m_hRegion = cross,
                m_stuGetColor = HalconColor.gold
            });
            showWindow.m_lineShowInfos.Add(lineInfo1.Copy());
            showWindow.m_lineShowInfos.Add(lineInfo2.Copy());
            PointCross = outCrossGroup;
            return success;
        }


        public bool ProcessCoarseLocation(
          ImageInfo imageInfo,
          IPlugin plugin,
          int processID,
          ShowHalconWindow showWindow, out OutCrossGroup PointCross, PointInfo Point基准,
          PointInfo Point跟随,
          string moduleName = null, bool defaultFalg = false, bool follow = false)
        {
            PointCross = new OutCrossGroup();
            if (plugin == null)
                return false;

            // 获取参数并设置图像
            var paramsFindIntersection = plugin.PluginParams as ParamsFindIntersection;
            if (paramsFindIntersection == null)
                return false;

            paramsFindIntersection.ho_Image = imageInfo.ho_image;
            paramsFindIntersection.m_bUsePositionFollow = follow;
            paramsFindIntersection.m_fBaseCol = Point基准.Col;
            paramsFindIntersection.m_fBaseRow = Point基准.Row;
            paramsFindIntersection.m_fBaseAngle = 0;
            paramsFindIntersection.m_fCurrentCol = Point跟随.Col;
            paramsFindIntersection.m_fCurrentRow = Point跟随.Row;
            paramsFindIntersection.m_fCurrentAngle = 0;


            // 模块名（若未指定则使用插件自身的名称）
            string modName = moduleName ?? plugin.moduleInfo.ModuleName;

            // 初始化当前点（默认当前设定值）
            PointInfo pointCurrent = new PointInfo()
            {
                Col = paramsFindIntersection.m_fCurrentCol,
                Row = paramsFindIntersection.m_fCurrentRow
            };

            // 执行插件
            bool success = plugin.ExecuteObj();

            // 构建两条边线信息
            LineInfo lineInfo1, lineInfo2;

            if (success)
            {
                // 执行成功：使用实际交点坐标
                pointCurrent.Col = paramsFindIntersection.hv_IntersectionColumn;
                pointCurrent.Row = paramsFindIntersection.hv_IntersectionRow;
                pointCurrent.bIsSuccess = success;
                lineInfo1 = new LineInfo()
                {
                    ColBegin = paramsFindIntersection.hv_Line1_ColumnBegin,
                    RowBegin = paramsFindIntersection.hv_Line1_RowBegin,
                    ColEnd = paramsFindIntersection.hv_Line1_ColumnEnd,
                    RowEnd = paramsFindIntersection.hv_Line1_RowEnd,
                    IsLineSuccessfull = paramsFindIntersection.bl_Line1_FitSuccess,
                    StrLineName = $"{modName}_抓边1",
                    strMoudleName = modName,
                    nProcessID = processID
                };

                lineInfo2 = new LineInfo()
                {
                    ColBegin = paramsFindIntersection.hv_Line2_ColumnBegin,
                    RowBegin = paramsFindIntersection.hv_Line2_RowBegin,
                    ColEnd = paramsFindIntersection.hv_Line2_ColumnEnd,
                    RowEnd = paramsFindIntersection.hv_Line2_RowEnd,
                    IsLineSuccessfull = paramsFindIntersection.bl_Line2_FitSuccess,
                    StrLineName = $"{modName}_抓边2",
                    strMoudleName = modName,
                    nProcessID = processID
                };
            }
            else
            {
                // 执行失败：分别判断每条边是否拟合成功
                if (paramsFindIntersection.bl_Line1_FitSuccess)
                {
                    lineInfo1 = new LineInfo()
                    {
                        ColBegin = paramsFindIntersection.hv_Line1_ColumnBegin,
                        RowBegin = paramsFindIntersection.hv_Line1_RowBegin,
                        ColEnd = paramsFindIntersection.hv_Line1_ColumnEnd,
                        RowEnd = paramsFindIntersection.hv_Line1_RowEnd,
                        IsLineSuccessfull = true,
                        StrLineName = $"{modName}_抓边1",
                        strMoudleName = modName,
                        nProcessID = processID
                    };
                }
                else
                {
                    lineInfo1 = new LineInfo()
                    {
                        ColBegin = paramsFindIntersection.hv_Line1_RoiMidColumn,
                        RowBegin = paramsFindIntersection.hv_Line1_RoiMidRow,
                        ColEnd = paramsFindIntersection.hv_Line1_RoiMidColumn + 200,
                        RowEnd = paramsFindIntersection.hv_Line1_RoiMidRow + 200,
                        IsLineSuccessfull = false,
                        StrLineName = $"{modName}_抓边1",
                        strMoudleName = modName,
                        nProcessID = processID
                    };
                }

                if (paramsFindIntersection.bl_Line2_FitSuccess)
                {
                    lineInfo2 = new LineInfo()
                    {
                        ColBegin = paramsFindIntersection.hv_Line2_ColumnBegin,
                        RowBegin = paramsFindIntersection.hv_Line2_RowBegin,
                        ColEnd = paramsFindIntersection.hv_Line2_ColumnEnd,
                        RowEnd = paramsFindIntersection.hv_Line2_RowEnd,
                        IsLineSuccessfull = true,
                        StrLineName = $"{modName}_抓边2",
                        strMoudleName = modName,
                        nProcessID = processID
                    };
                }
                else
                {
                    lineInfo2 = new LineInfo()
                    {
                        ColBegin = paramsFindIntersection.hv_Line2_RoiMidColumn,   // 修正为 Line2 的 ROI 中点
                        RowBegin = paramsFindIntersection.hv_Line2_RoiMidRow,
                        ColEnd = paramsFindIntersection.hv_Line2_RoiMidColumn + 200,
                        RowEnd = paramsFindIntersection.hv_Line2_RoiMidRow + 200,
                        IsLineSuccessfull = false,
                        StrLineName = $"{modName}_抓边2",
                        strMoudleName = modName,
                        nProcessID = processID
                    };
                }

                //默认给出错误坐标
                if (defaultFalg)
                {
                    pointCurrent.Col = -999;
                    pointCurrent.Row = -999;
                    pointCurrent.bIsSuccess = false;

                }
            }
 
            // 封装结果并添加到列表中
            OutCrossGroup outCrossGroup = new OutCrossGroup()
            {
                nProcessID = processID,
                ModuleName = modName,
                LineInfo1 = lineInfo1,
                LineInfo2 = lineInfo2,
                point = pointCurrent,
                bIsSuccess= pointCurrent.bIsSuccess
            };
         

            // 生成十字交叉线并添加到显示窗口
            HOperatorSet.GenCrossContourXld(out HObject cross, pointCurrent.Row, pointCurrent.Col, 10, 45);
            showWindow.m_regionShowInfos.Add(new RegionShowInfo()
            {
                m_hRegion = cross,
                m_stuGetColor = HalconColor.gold
            });
            showWindow.m_lineShowInfos.Add(lineInfo1.Copy());
            showWindow.m_lineShowInfos.Add(lineInfo2.Copy());
            PointCross = outCrossGroup;
            return success;
        }

        public bool ExecuteBoardCalibration(
        double inputPixelCol, double inputPixelRow,
        IPlugin plugin,
        ParamsCACRunCalibration param,
        out double outputBoardX, out double outputBoardY,
        out string errorMessage,
        string processId = "", string moduleName = "")
        {
            param.m_fInputPixelCol = inputPixelCol;
            param.m_fInputPixelRow = inputPixelRow;
            if (plugin.ExecuteObj())
            {
                outputBoardX = param.m_fOutputBoardX;
                outputBoardY = param.m_fOutputBoardY;
                errorMessage = null;
                return true;
            }
            else
            {
                outputBoardX = outputBoardY = 0;
                errorMessage = param.m_strRunMessage;
                Log4netRecord.ErrorLog.Debug($"流程{processId}&&{moduleName}---{errorMessage}");
                return false;
            }
        }
    }
}
