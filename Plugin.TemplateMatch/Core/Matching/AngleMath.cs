namespace Plugin.TemplateMatch.Core.Matching
{
    /// <summary>
    /// 角度工具，统一做角度与弧度转换，避免算法代码里到处散落换算表达式。
    /// </summary>
    internal static class AngleMath
    {
        public static double DegToRad(double deg)
        {
            return deg * System.Math.PI / 180.0;
        }

        public static double RadToDeg(double rad)
        {
            return rad * 180.0 / System.Math.PI;
        }
    }
}
