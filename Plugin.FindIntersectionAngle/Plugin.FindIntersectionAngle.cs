using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frame_Core;
using System.ComponentModel.Composition;
using System.ComponentModel;
using HalconDotNet;
using HalconAlgorithm;
using LogTool;
using System.Windows.Forms;

namespace Plugin.FindIntersectionAngle
{
    [Category("尺寸测量")]
    [DisplayName("两线交点角度工具")]
    [Export("FindIntersectionAngleObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class FindIntersectionAngleObj : IPlugin
    {
        public PluginParamsBase PluginParams { get; set; } = new ParamsFindIntersectionAngle();
        FittingLine fittingLine = new FittingLine();
        ParamsFindIntersectionAngle findParams;

        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();
        public bool isExistModel = false;

        public bool ExecuteObj()
        {
            findParams = PluginParams as ParamsFindIntersectionAngle;
            if (findParams == null)
            {
                Log4netRecord.ErrorLog.Debug("两线交点模块：参数转换失败");
                return false;
            }

            // 如果指定图像来源模块，则加载图像
            if (findParams.ImageSourceIndex == 1)
            {
                SetInPutParams(findParams.ImageSourceModule, findParams.ImageSourceName);
            }

            // 重置输出变量
            ResetOutputParams();

            try
            {
                // 校验图像有效性
                if (findParams.ho_Image == null || findParams.ho_Image.CountObj() == 0)
                {
                    Log4netRecord.ErrorLog.Debug("两线交点模块：输入图像无效");
                    return false;
                }

                // ====== 第一条直线抓边 ======
                bool line1Ok = FitSingleLine(
                    findParams.hv_Line1_RoiMidRow, findParams.hv_Line1_RoiMidColumn,
                    findParams.hv_Line1_RoiAngle, findParams.hv_Line1_RoiHeight, findParams.hv_Line1_RoiWidth,
                    findParams.hv_Line1_RoiPointsNum, findParams.hv_Line1_MeasureSigma, findParams.hv_Line1_MeasureThreshold,
                    findParams.hv_Line1_MeasureNumInstances, findParams.hv_Line1_MeasureDistThreshold,
                    findParams.hv_Line1_MeasureTransition, findParams.hv_Line1_MeasureSelect,
                    findParams.hv_Line1_MeasureMinScore, findParams.hv_Line1_MaxNumIterations,
                    findParams.hv_Line1_MeasureInterpolation, findParams.hv_Line1_MeasureIORegions,
                    out findParams.hv_Line1_CrossRow, out findParams.hv_Line1_CrossColumn,
                    out findParams.hv_Line1_RowBegin, out findParams.hv_Line1_ColumnBegin,
                    out findParams.hv_Line1_RowEnd, out findParams.hv_Line1_ColumnEnd);

                findParams.bl_Line1_FitSuccess = line1Ok;

                // ====== 第二条直线抓边 ======
                bool line2Ok = FitSingleLine(
                    findParams.hv_Line2_RoiMidRow, findParams.hv_Line2_RoiMidColumn,
                    findParams.hv_Line2_RoiAngle, findParams.hv_Line2_RoiHeight, findParams.hv_Line2_RoiWidth,
                    findParams.hv_Line2_RoiPointsNum, findParams.hv_Line2_MeasureSigma, findParams.hv_Line2_MeasureThreshold,
                    findParams.hv_Line2_MeasureNumInstances, findParams.hv_Line2_MeasureDistThreshold,
                    findParams.hv_Line2_MeasureTransition, findParams.hv_Line2_MeasureSelect,
                    findParams.hv_Line2_MeasureMinScore, findParams.hv_Line2_MaxNumIterations,
                    findParams.hv_Line2_MeasureInterpolation, findParams.hv_Line2_MeasureIORegions,
                    out findParams.hv_Line2_CrossRow, out findParams.hv_Line2_CrossColumn,
                    out findParams.hv_Line2_RowBegin, out findParams.hv_Line2_ColumnBegin,
                    out findParams.hv_Line2_RowEnd, out findParams.hv_Line2_ColumnEnd);

                findParams.bl_Line2_FitSuccess = line2Ok;

                // ====== 计算两直线交点 ======
                if (line1Ok && line2Ok)
                {
                    CalculateIntersection();
                    PublishResultParams();
                }
                else
                {
                    findParams.bl_IntersectionSuccess = false;
                    if (!line1Ok)
                    Log4netRecord.ErrorLog.Debug("两线交点角度模块：直线1拟合失败");
                    if (!line2Ok)
                    Log4netRecord.ErrorLog.Debug("两线交点角度模块：直线2拟合失败");
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug("两线交点角度模块执行异常：" + ex.ToString());
                findParams.bl_IntersectionSuccess = false;
                return false;
            }

            return findParams.bl_IntersectionSuccess;
        }

        /// <summary>
        /// 对单条直线执行抓边拟合
        /// </summary>
        private bool FitSingleLine(
            double roiMidRow, double roiMidColumn, double roiAngle,
            double roiHeight, double roiWidth, int roiPointsNum,
            double measureSigma, int measureThreshold,
            int measureNumInstances, double measureDistThreshold,
            string measureTransition, string measureSelect,
            double measureMinScore, int maxNumIterations,
            string measureInterpolation, string measureIORegions,
            out HTuple crossRow, out HTuple crossColumn,
            out HTuple lineRowBegin, out HTuple lineColumnBegin,
            out HTuple lineRowEnd, out HTuple lineColumnEnd)
        {
            crossRow = null;
            crossColumn = null;
            lineRowBegin = null;
            lineColumnBegin = null;
            lineRowEnd = null;
            lineColumnEnd = null;

            HTuple hv_MetrologyHandle = null;
            HTuple hv_Index = null;

            try
            {
                // 校验参数有效性
                if (roiPointsNum < 2)
                {
                    Log4netRecord.ErrorLog.Debug("两线交点模块：采样密度不能小于2");
                    return false;
                }
                if (roiHeight <= 0 || roiWidth <= 0)
                {
                    Log4netRecord.ErrorLog.Debug("两线交点模块：ROI宽度和高度必须大于0");
                    return false;
                }
                if (measureSigma <= 0)
                {
                    Log4netRecord.ErrorLog.Debug("两线交点模块：平滑系数必须大于0");
                    return false;
                }

                // 角度转弧度
                HTuple hv_RoiPhi;
                HOperatorSet.TupleRad(roiAngle, out hv_RoiPhi);

                // 计算ROI的起止点
                HTuple hv_MeasureRow1 = roiMidRow - roiHeight * Math.Sin(hv_RoiPhi) / 2;
                HTuple hv_MeasureColumn1 = roiMidColumn - roiHeight * Math.Cos(hv_RoiPhi) / 2;
                HTuple hv_MeasureRow2 = roiMidRow + roiHeight * Math.Sin(hv_RoiPhi) / 2;
                HTuple hv_MeasureColumn2 = roiMidColumn + roiHeight * Math.Cos(hv_RoiPhi) / 2;

                // 创建直线测量模型
                fittingLine.CreateLineModel(
                    hv_MeasureRow1, hv_MeasureColumn1,
                    hv_MeasureRow2, hv_MeasureColumn2,
                    roiWidth / 2, roiHeight / roiPointsNum / 2,
                    measureSigma, measureThreshold,
                    out hv_MetrologyHandle, out hv_Index);

                if (hv_MetrologyHandle == null)
                {
                    Log4netRecord.ErrorLog.Debug("两线交点模块：创建测量模型失败");
                    return false;
                }

                // 执行直线拟合
                fittingLine.FindLine(
                    findParams.ho_Image, hv_MetrologyHandle, hv_Index,
                    measureNumInstances, measureDistThreshold,
                    measureTransition, measureSelect,
                    roiPointsNum, measureMinScore, maxNumIterations,
                    measureInterpolation, measureIORegions,
                    out crossRow, out crossColumn,
                    out lineRowBegin, out lineColumnBegin,
                    out lineRowEnd, out lineColumnEnd);

                // 判断拟合是否成功
                if (lineRowBegin != null && lineRowBegin.Length > 0 && lineRowBegin >= 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug("两线交点模块-单线拟合异常：" + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 计算两直线交点
        /// </summary>
        private void CalculateIntersection()
        {
            try
            {
                HTuple hv_Row, hv_Column, hv_IsOverlapping;

                HOperatorSet.IntersectionLines(
                    findParams.hv_Line1_RowBegin, findParams.hv_Line1_ColumnBegin,
                    findParams.hv_Line1_RowEnd, findParams.hv_Line1_ColumnEnd,
                    findParams.hv_Line2_RowBegin, findParams.hv_Line2_ColumnBegin,
                    findParams.hv_Line2_RowEnd, findParams.hv_Line2_ColumnEnd,
                    out hv_Row, out hv_Column, out hv_IsOverlapping);

                findParams.hv_IntersectionRow = hv_Row;
                findParams.hv_IntersectionColumn = hv_Column;
                findParams.hv_IsParallel = hv_IsOverlapping;
                CalculateLineAngle();

                // hv_IsOverlapping == 0 表示有唯一交点
                if (hv_IsOverlapping != null && hv_IsOverlapping.I == 0)
                {
                    findParams.bl_IntersectionSuccess = true;
                }
                else
                {
                    findParams.bl_IntersectionSuccess = false;
                    Log4netRecord.ErrorLog.Debug("两线交点模块：两直线平行或重合，无唯一交点");
                }
            }
            catch (Exception ex)
            {
                findParams.bl_IntersectionSuccess = false;
                Log4netRecord.ErrorLog.Debug("两线交点模块-交点计算异常：" + ex.ToString());
            }
        }

        /// <summary>
        /// 计算两条拟合线之间的锐角夹角，单位为度。
        /// </summary>
        private void CalculateLineAngle()
        {
            double v1Row = findParams.hv_Line1_RowEnd.D - findParams.hv_Line1_RowBegin.D;
            double v1Col = findParams.hv_Line1_ColumnEnd.D - findParams.hv_Line1_ColumnBegin.D;
            double v2Row = findParams.hv_Line2_RowEnd.D - findParams.hv_Line2_RowBegin.D;
            double v2Col = findParams.hv_Line2_ColumnEnd.D - findParams.hv_Line2_ColumnBegin.D;

            double len1 = Math.Sqrt(v1Row * v1Row + v1Col * v1Col);
            double len2 = Math.Sqrt(v2Row * v2Row + v2Col * v2Col);
            if (len1 <= double.Epsilon || len2 <= double.Epsilon)
            {
                findParams.hv_LineAngle = null;
                return;
            }

            double cos = (v1Row * v2Row + v1Col * v2Col) / (len1 * len2);
            cos = Math.Max(-1.0, Math.Min(1.0, cos));
            double angle = Math.Acos(Math.Abs(cos)) * 180.0 / Math.PI;
            findParams.hv_LineAngle = new HTuple(angle);
        }

        /// <summary>
        /// 重置所有输出参数
        /// </summary>
        private void ResetOutputParams()
        {
            findParams.hv_Line1_CrossRow = null;
            findParams.hv_Line1_CrossColumn = null;
            findParams.hv_Line1_RowBegin = null;
            findParams.hv_Line1_ColumnBegin = null;
            findParams.hv_Line1_RowEnd = null;
            findParams.hv_Line1_ColumnEnd = null;
            findParams.bl_Line1_FitSuccess = false;

            findParams.hv_Line2_CrossRow = null;
            findParams.hv_Line2_CrossColumn = null;
            findParams.hv_Line2_RowBegin = null;
            findParams.hv_Line2_ColumnBegin = null;
            findParams.hv_Line2_RowEnd = null;
            findParams.hv_Line2_ColumnEnd = null;
            findParams.bl_Line2_FitSuccess = false;

            findParams.hv_IntersectionRow = null;
            findParams.hv_IntersectionColumn = null;
            findParams.hv_LineAngle = null;
            findParams.hv_IsParallel = null;
            findParams.bl_IntersectionSuccess = false;
        }

        private void PublishResultParams()
        {
            GetResultParams("IntersectionRow", findParams.hv_IntersectionRow);
            GetResultParams("IntersectionColumn", findParams.hv_IntersectionColumn);
            GetResultParams("LineAngle", findParams.hv_LineAngle);
            GetResultParams("IsParallel", findParams.hv_IsParallel);

            GetResultParams("PointRow", findParams.hv_IntersectionRow);
            GetResultParams("PointColumn", findParams.hv_IntersectionColumn);
            GetResultParams("交点行", findParams.hv_IntersectionRow);
            GetResultParams("交点列", findParams.hv_IntersectionColumn);
            GetResultParams("两线夹角", findParams.hv_LineAngle);
            GetResultParams("是否平行", findParams.hv_IsParallel);
        }

        public bool saveParams()
        {
            PluginParams.filePath = moduleInfo.filePath;
            return PluginParams.Save();
        }

        public void SetParams(PluginParamsBase param)
        {
            this.PluginParams = (ParamsFindIntersectionAngle)param;
            findParams = PluginParams as ParamsFindIntersectionAngle;
        }

        public void GetResultParams(string varName, object obj)
        {
            Process proc = Project.Instance.GetProcessById(moduleInfo.ProcessID);
            if (proc == null)
            {
                Log4netRecord.ErrorLog.Debug("两线交点角度模块：发布输出失败，未找到指定流程");
                return;
            }
            proc.AddOutPutParam(moduleInfo.ModuleName, varName, obj);
        }

        public void SetInPutParams(string module, string valueName)
        {
            try
            {
                Process proc = Project.Instance.GetProcessById(moduleInfo.ProcessID);
                if (proc == null)
                {
                    Log4netRecord.ErrorLog.Debug("两线交点模块：未找到指定流程");
                    return;
                }
                object imgObj = proc.GetOutPutParamByName(module, valueName);
                if (imgObj != null)
                {
                    findParams.ho_Image = (HImage)imgObj;
                }
                else
                {
                    Log4netRecord.ErrorLog.Debug("两线交点模块：未获取到图像源 " + module + "/" + valueName);
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug("两线交点模块-设置输入参数异常：" + ex.ToString());
            }
        }

        public void IniObj()
        {
            findParams = PluginParams as ParamsFindIntersectionAngle;
        }

        public void DisPoseObj()
        {
        }
    }
}

