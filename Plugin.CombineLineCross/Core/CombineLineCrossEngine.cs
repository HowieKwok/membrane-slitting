using Frame_Core;
using Frame_Core.pluginParams;
using HalconDotNet;

namespace Plugin.CombineLineCross.Core
{
    /// <summary>
    /// 只根据外部输入的两条线段计算交点和夹角，不做抓边、不依赖图像。
    /// m_bRunSuccess 由插件 ExecuteObj 的 finally 统一赋值，是唯一运行成功标志。
    /// </summary>
    internal static class CombineLineCrossEngine
    {
        public static bool Run(ParamsCombPoint parameters)
        {
            if (parameters == null || parameters.LineInfo1 == null || parameters.LineInfo2 == null)
            {
                if (parameters != null) parameters.m_strRunMessage = "两条线段参数不能为空";
                return false;
            }

            ReleaseTuple(ref parameters.hv_IntersectionRow);
            ReleaseTuple(ref parameters.hv_IntersectionColumn);
            ReleaseTuple(ref parameters.hv_LineAngle);
            ReleaseTuple(ref parameters.hv_IsParallel);

            HTuple row = null;
            HTuple column = null;
            HTuple isOverlapping = null;
            HTuple angleRad = null;
            HTuple angleDeg = null;
            if(!parameters.LineInfo1.IsLineSuccessfull||!parameters.LineInfo2.IsLineSuccessfull)
            {
                bool success = false;
                parameters.hv_IntersectionRow = -999;
                parameters.hv_IntersectionColumn = -999;
                parameters.hv_LineAngle = -999;
                parameters.hv_IsParallel = 0;
                parameters.m_strRunMessage = success ? "交点计算成功" : $"Line1状态{parameters.LineInfo1.IsLineSuccessfull}&&Line2状态{parameters.LineInfo2.IsLineSuccessfull}两条直线状态失败，无唯一交点";
                return success;
            }

            try
            {
                // 与 Plugin.FindIntersection 保持一致，交点和夹角均由 HALCON 算子计算。
                HOperatorSet.IntersectionLines(
                    parameters.LineInfo1.RowBegin, parameters.LineInfo1.ColBegin,
                    parameters.LineInfo1.RowEnd, parameters.LineInfo1.ColEnd,
                    parameters.LineInfo2.RowBegin, parameters.LineInfo2.ColBegin,
                    parameters.LineInfo2.RowEnd, parameters.LineInfo2.ColEnd,
                    out row, out column, out isOverlapping);
                HOperatorSet.AngleLl(
                    parameters.LineInfo1.RowBegin, parameters.LineInfo1.ColBegin,
                    parameters.LineInfo1.RowEnd, parameters.LineInfo1.ColEnd,
                    parameters.LineInfo2.RowBegin, parameters.LineInfo2.ColBegin,
                    parameters.LineInfo2.RowEnd, parameters.LineInfo2.ColEnd,
                    out angleRad);
                HOperatorSet.TupleDeg(angleRad, out angleDeg);

                parameters.hv_IntersectionRow = row;
                parameters.hv_IntersectionColumn = column;
                parameters.hv_LineAngle = angleDeg;
                parameters.hv_IsParallel = isOverlapping;
                row = null;
                column = null;
                angleDeg = null;
                isOverlapping = null;

                bool success = parameters.hv_IsParallel.Length > 0 && parameters.hv_IsParallel[0].I == 0;
                parameters.m_strRunMessage = success ? "交点计算成功" : "两条直线平行或重合，无唯一交点";
                return success;
            }
            catch (HOperatorException ex)
            {
                parameters.m_strRunMessage = "HALCON 交点计算异常：" + ex.Message;
                return false;
            }
            finally
            {
                // 未转交给参数对象的 HALCON 元组必须在本次计算内释放。
                ReleaseTuple(ref row);
                ReleaseTuple(ref column);
                ReleaseTuple(ref isOverlapping);
                ReleaseTuple(ref angleRad);
                ReleaseTuple(ref angleDeg);
            }
        }

        private static void ReleaseTuple(ref HTuple tuple)
        {
            if (tuple != null)
            {
                tuple.Dispose();
                tuple = null;
            }
        }
    }
}
