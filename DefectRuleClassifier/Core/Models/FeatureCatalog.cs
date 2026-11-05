using System.Collections.Generic;
using System.Linq;

namespace DefectRuleClassifier.Core.Models
{
    /// <summary>
    /// 特征目录。
    /// 每个 PicFeatureType 对应一个可被规则编辑器选择的特征项。
    /// </summary>
    public static class FeatureCatalog
    {
        public const string FeaturePrefix = "Feature.";

        private static readonly FeatureTypeOption[] Options =
        {
            Create(PicFeatureType.DefectGray, "缺陷区域灰度均值", "缺陷区域的平均灰度"),
            Create(PicFeatureType.BrightDarkDiff, "亮区暗区差值", "亮区最大灰度减去暗区最小灰度"),
            Create(PicFeatureType.DefectContourChange, "缺陷轮廓变化", "缺陷区域 Sobel 后最大边缘灰度"),
            Create(PicFeatureType.DarkGray, "暗区平均值", "暗区灰度均值"),
            Create(PicFeatureType.UltraDarkGray, "超暗区平均值", "暗区内 0~40 灰度均值"),
            Create(PicFeatureType.DarkContourChange, "暗区轮廓变化", "暗区 Sobel 后最大边缘灰度"),
            Create(PicFeatureType.BrightGray, "亮区平均值", "亮区灰度均值"),
            Create(PicFeatureType.UltraBrightGray, "超亮区平均值", "亮区内 180~255 灰度均值"),
            Create(PicFeatureType.BrightContourChange, "亮区轮廓变化", "亮区 Sobel 后最大边缘灰度"),
            Create(PicFeatureType.WidthHeightRate, "宽高比", "短边 / 长边"),
            Create(PicFeatureType.HorVerRate, "横纵比", "横向尺寸 / 纵向尺寸"),
            Create(PicFeatureType.DarkBrightAreaRatio, "暗亮面积比", "暗区面积 / 亮区面积"),
            Create(PicFeatureType.BrightOfBubbleRate, "气泡外环亮点占比", "气泡外环亮点面积占比"),
            Create(PicFeatureType.MinCoateRate, "最小涂层宽度占比", "原程序当前返回 -1"),
            Create(PicFeatureType.BrightDarkDistanceRate, "亮暗区距离比", "亮暗最小外接圆半径和中心距离"),
            Create(PicFeatureType.BrightDarkAreaRate, "亮暗区面积比", "亮区面积 / 暗区面积"),
            Create(PicFeatureType.BrightArea, "亮区面积", "亮区面积乘以横纵单量"),
            Create(PicFeatureType.DarkArea, "暗区面积", "暗区面积乘以横纵单量"),
            Create(PicFeatureType.DecarburizationBlackZone, "脱碳黑区", "暗区内 0~30 黑区最大连通域面积"),
            Create(PicFeatureType.DarkGradient, "暗区梯度", "暗区 Sobel 后最大边缘灰度"),
            Create(PicFeatureType.BubbleOuterRoundness, "气泡外环圆度", "气泡外环 Circularity"),
            Create(PicFeatureType.BubbleOuterDarkConcentricity, "气泡外环黑点同心度", "缺陷与外环交集面积 / 缺陷面积"),
            Create(PicFeatureType.DarkWidthHeightRate, "暗区长宽比", "暗区 Eccentricity anisometry"),
            Create(PicFeatureType.DefectGradient, "缺陷梯度", "缺陷区域 Sobel 后最大边缘灰度"),
            Create(PicFeatureType.DefectGradientCount, "大梯度点数量", "缺陷区域 Sobel 后 40~255 面积"),
            Create(PicFeatureType.DefectBright, "缺陷亮度", "缺陷区域灰度均值"),
            Create(PicFeatureType.BrightEdgeGradient, "亮区边缘梯度", "亮区边缘带 SobelAmp 最大值"),
            Create(PicFeatureType.DarkEdgeGradient, "暗区边缘梯度", "暗区边缘带灰度或梯度判定值"),
            Create(PicFeatureType.DarkBrightWidthHeightRate, "暗亮区域长宽比", "暗亮合并区 Eccentricity anisometry"),
            Create(PicFeatureType.BubbleDarkRoundness, "气泡暗区圆度", "区域 Circularity"),
            Create(PicFeatureType.Lengthness, "缺陷细长度", "rect2_len2 / rect2_len1"),
            Create(PicFeatureType.Convexity, "缺陷紧实度", "RegionFeatures(convexity)"),
            Create(PicFeatureType.Holes, "亮区孔洞数", "RegionFeatures(holes_num)"),
            Create(PicFeatureType.Rect2_len1, "缺陷长边", "RegionFeatures(rect2_len1)"),
            Create(PicFeatureType.Rect2_len2, "缺陷短边", "RegionFeatures(rect2_len2)"),
            Create(PicFeatureType.DefectRoundness, "缺陷圆度", "映射到 BubbleDarkRoundness(缺陷区)"),
            Create(PicFeatureType.LightAreaRate, "亮区面积占比", "亮区面积 / 缺陷总面积"),
            Create(PicFeatureType.DarkAreaRate, "暗区面积占比", "暗区面积 / 缺陷总面积")
        };

        public static IReadOnlyList<FeatureTypeOption> All => Options;

        public static string KeyOf(PicFeatureType type) => FeaturePrefix + type;

        public static FeatureTypeOption Get(PicFeatureType type)
        {
            return Options.FirstOrDefault(item => item.Value == type) ??
                   new FeatureTypeOption(type, type.ToString(), KeyOf(type), "未配置说明");
        }

        public static string GetDisplayNameByKey(string featureKey)
        {
            switch (featureKey)
            {
                case "area": return "面积";
                case "area_px": return "面积(px)";
                case "length": return "长度";
                case "width": return "宽度";
                case "height": return "高度";
                case "width_px": return "宽度(px)";
                case "height_px": return "高度(px)";
                case "mean_gray": return "缺陷区域灰度均值";
                case "row": return "中心行";
                case "column": return "中心列";
                case "polarity_id": return "极性编号";
            }

            FeatureTypeOption option = Options.FirstOrDefault(item => item.FeatureKey == featureKey);
            return option != null ? option.DisplayName : featureKey;
        }

        /// <summary>
        /// 返回规则编辑表格中显示的建议范围和单位说明。
        /// 这里不参与算法，只帮助配置人员知道 MinValue/MaxValue 应该按什么量纲填写。
        /// </summary>
        public static string GetRangeDescription(PicFeatureType type)
        {
            switch (type)
            {
                case PicFeatureType.DefectGray:
                case PicFeatureType.DarkGray:
                case PicFeatureType.UltraDarkGray:
                case PicFeatureType.BrightGray:
                case PicFeatureType.UltraBrightGray:
                case PicFeatureType.DefectBright:
                    return "建议范围：0~255，单位：灰度";

                case PicFeatureType.DefectContourChange:
                case PicFeatureType.DarkContourChange:
                case PicFeatureType.BrightContourChange:
                case PicFeatureType.DarkGradient:
                case PicFeatureType.DefectGradient:
                case PicFeatureType.BrightEdgeGradient:
                case PicFeatureType.DarkEdgeGradient:
                    return "建议范围：0~255，单位：梯度/边缘灰度";

                case PicFeatureType.WidthHeightRate:
                case PicFeatureType.HorVerRate:
                case PicFeatureType.DarkBrightAreaRatio:
                case PicFeatureType.BrightOfBubbleRate:
                case PicFeatureType.MinCoateRate:
                case PicFeatureType.BrightDarkDistanceRate:
                case PicFeatureType.BrightDarkAreaRate:
                case PicFeatureType.LightAreaRate:
                case PicFeatureType.DarkAreaRate:
                case PicFeatureType.DarkWidthHeightRate:
                case PicFeatureType.DarkBrightWidthHeightRate:
                case PicFeatureType.Lengthness:
                    return "建议范围：按比例填写，常用 0~1，部分特征可能大于 1";

                case PicFeatureType.BrightArea:
                case PicFeatureType.DarkArea:
                case PicFeatureType.DecarburizationBlackZone:
                case PicFeatureType.DefectGradientCount:
                    return "建议范围：>=0，单位：面积，已按横纵单量换算";

                case PicFeatureType.BubbleOuterRoundness:
                case PicFeatureType.BubbleDarkRoundness:
                case PicFeatureType.DefectRoundness:
                case PicFeatureType.Convexity:
                    return "建议范围：0~1，越接近 1 越圆或越紧实";

                case PicFeatureType.Holes:
                    return "建议范围：>=0，单位：孔洞数量";

                case PicFeatureType.Rect2_len1:
                case PicFeatureType.Rect2_len2:
                    return "建议范围：>=0，单位：像素，来自区域最小外接矩形";

                case PicFeatureType.BrightDarkDiff:
                    return "建议范围：0~255，单位：灰度差";

                case PicFeatureType.BubbleOuterDarkConcentricity:
                    return "建议范围：0~1，缺陷与外环重合占比";

                default:
                    return "建议范围：按实际调试特征值填写";
            }
        }

        private static FeatureTypeOption Create(PicFeatureType type, string displayName, string description)
        {
            return new FeatureTypeOption(type, displayName, KeyOf(type), description);
        }
    }
}
