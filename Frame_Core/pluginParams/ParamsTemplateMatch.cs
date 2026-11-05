using HalconDotNet;
using FrameCommunity.MaskRoiTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Frame_Core.pluginParams
{
    [Serializable]
    public class ParamsTemplateMatch : PluginParamsBase, IDisposable
    {
        public const int RoiRoleTemplate = 0;
        public const int RoiRoleLocal = 1;
        public const int RoiRoleMask = 2;

        [NonSerialized]
        [XmlIgnore]
        public HObject ho_Image;

        [NonSerialized]
        [XmlIgnore]
        public HObject ho_OutContours;

        [NonSerialized]
        [XmlIgnore]
        public HObject ho_OutMatchRoiRegion;

        [NonSerialized]
        [XmlIgnore]
        public HObject ho_OutMaskRoiRegion;

        [NonSerialized]
        [XmlIgnore]
        public HTuple hv_OutBaseToCurrentMatrix;

        [Description("判断是否启用")]
        public bool m_bIsEnable = true;

        [Description("启用局部ROI")]
        public bool m_bUseLocalRoi = false;

        [Description("显示ViewROI")]
        public bool m_bShowViewRoi = true;

        [Description("显示模板创建ROI")]
        public bool m_bShowTemplateRoi = true;

        [Description("显示局部搜索ROI")]
        public bool m_bShowLocalRoi = true;

        [Description("允许编辑模板创建ROI")]
        public bool m_bEditTemplateRoi = true;

        [Description("允许编辑局部搜索ROI")]
        public bool m_bEditLocalRoi = false;

        [Description("模板创建ROI形状类型")]
        public int m_nTemplateRoiShapeType = 13;

        [Description("局部搜索ROI形状类型")]
        public int m_nLocalRoiShapeType = 13;

        [Description("匹配后ROI跟随显示")]
        public bool m_bRoiFollowMatch = true;

        [Description("启用位置跟随")]
        public bool m_bUsePositionFollow = false;

        [Description("启用屏蔽ROI")]
        public bool m_bUseMaskRoi = false;

        [Description("显示屏蔽ROI")]
        public bool m_bShowMaskRoi = true;

        [Description("允许编辑屏蔽ROI")]
        public bool m_bEditMaskRoi = false;

        [Description("位置跟随基准行")]
        public double m_fFollowBaseRow = 0;

        [Description("位置跟随基准列")]
        public double m_fFollowBaseCol = 0;

        [Description("位置跟随基准角度")]
        public double m_fFollowBaseAngleDeg = 0;

        [Description("位置跟随当前行")]
        public double m_fFollowCurrentRow = 0;

        [Description("位置跟随当前列")]
        public double m_fFollowCurrentCol = 0;

        [Description("位置跟随当前角度")]
        public double m_fFollowCurrentAngleDeg = 0;

        [Description("模板根目录")]
        public string m_strTemplateRootDir = string.Empty;

        public double m_fLocalRoiRow1 = 0;
        public double m_fLocalRoiCol1 = 0;
        public double m_fLocalRoiRow2 = 3000;
        public double m_fLocalRoiCol2 = 4096;

        public TemplateCreateOptions CreateOptions { get; set; } = new TemplateCreateOptions();
        public TemplateFindOptions FindOptions { get; set; } = new TemplateFindOptions();
        public List<TemplateModelParam> Templates { get; set; } = new List<TemplateModelParam>();
        [XmlArrayItem("MaskRoiParam", typeof(MaskRoiParam))]
        public List<MaskRoiParam> m_lMaskRoiParams { get; set; } = new List<MaskRoiParam>();

        [NonSerialized]
        [XmlIgnore]
        public List<TemplateMatchResultParam> Results = new List<TemplateMatchResultParam>();

        [XmlIgnore]
        public string TemplateRootDir
        {
            get { return m_strTemplateRootDir; }
            set { m_strTemplateRootDir = value ?? string.Empty; }
        }

        public ParamsTemplateMatch()
        {
            HOperatorSet.GenEmptyObj(out ho_Image);
            Results = new List<TemplateMatchResultParam>();
            EnsureDefaultTemplates();
        }

        public void EnsureDefaultTemplates()
        {
            if (Templates == null) Templates = new List<TemplateModelParam>();
            if (Templates.Count > 0) return;

            //Templates.Add(new TemplateModelParam
            //{
            //    Name = "Template1",
            //    IsEnabled = true,
            //    MatchMode = TemplateMatchMode.Shape,
            //    RoiRow1 = 100,
            //    RoiCol1 = 100,
            //    RoiRow2 = 300,
            //    RoiCol2 = 300,
            //    BaseRow = 200,
            //    BaseCol = 200,
            //    BaseAngleDeg = 0
            //});
        }

        public void Dispose()
        {
            // Only release objects owned by this plugin output/model cache.
            // ho_Image is an input from upstream and must not be released here.
            if (ho_OutContours != null)
            {
                ho_OutContours.Dispose();
                ho_OutContours = null;
            }

            if (ho_OutMatchRoiRegion != null)
            {
                ho_OutMatchRoiRegion.Dispose();
                ho_OutMatchRoiRegion = null;
            }

            if (ho_OutMaskRoiRegion != null)
            {
                ho_OutMaskRoiRegion.Dispose();
                ho_OutMaskRoiRegion = null;
            }

            hv_OutBaseToCurrentMatrix = null;
            if (Templates != null)
            {
                foreach (TemplateModelParam template in Templates)
                {
                    if (template != null) template.Dispose();
                }
            }
        }
    }

    [Serializable]
    public enum TemplateMatchMode
    {
        Shape = 0,
        Gray = 1
    }

    [Serializable]
    public class TemplateCreateOptions
    {
        public TemplateMatchMode MatchMode { get; set; } = TemplateMatchMode.Shape;
        public int ModelCreatingMethod { get; set; } = 1;
        public int NumLevels { get; set; } = 3;
        public double AngleStartDeg { get; set; } = -180;
        public double AngleExtentDeg { get; set; } = 360;
        public double AngleStepDeg { get; set; } = 0;
        public string Optimization { get; set; } = "auto";
        public string Metric { get; set; } = "use_polarity";
        public int Contrast { get; set; } = 30;
        public int MinContrast { get; set; } = 10;
    }

    [Serializable]
    public class TemplateFindOptions
    {
        public double AngleStartDeg { get; set; } = -180;
        public double AngleExtentDeg { get; set; } = 360;
        public double MinScore { get; set; } = 0.5;
        public int NumMatches { get; set; } = 1;
        public double MaxOverlap { get; set; } = 0.5;
        public string SubPixel { get; set; } = "least_squares";
        public int NumLevels { get; set; } = 3;
        public double Greediness { get; set; } = 0.9;
        public int TimeoutMs { get; set; } = 5000;
    }

    [Serializable]
    public class TemplateModelParam : IDisposable
    {
        public bool IsEnabled { get; set; } = true;
        public bool IsCreated { get; set; } = false;
        public string Name { get; set; } = "Template1";
        public TemplateMatchMode MatchMode { get; set; } = TemplateMatchMode.Shape;
        public TemplateCreateOptions CreateOptions { get; set; } = new TemplateCreateOptions();
        public TemplateFindOptions FindOptions { get; set; } = new TemplateFindOptions();
        public double RoiRow1 { get; set; } = 0;
        public double RoiCol1 { get; set; } = 0;
        public double RoiRow2 { get; set; } = 100;
        public double RoiCol2 { get; set; } = 100;
        public int RoiShapeType { get; set; } = 13;
        public double BaseRow { get; set; } = 0;
        public double BaseCol { get; set; } = 0;
        public double BaseAngleDeg { get; set; } = 0;
        public double LastScore { get; set; } = 0;
        public string ModelFileName { get; set; } = string.Empty;

        [NonSerialized]
        [XmlIgnore]
        public HTuple ModelId;

        [NonSerialized]
        [XmlIgnore]
        public HObject CreateContours;

        public void SetRoi(double row1, double col1, double row2, double col2)
        {
            RoiRow1 = Math.Min(row1, row2);
            RoiCol1 = Math.Min(col1, col2);
            RoiRow2 = Math.Max(row1, row2);
            RoiCol2 = Math.Max(col1, col2);
        }

        public void Dispose()
        {
            if (CreateContours != null)
            {
                CreateContours.Dispose();
                CreateContours = null;
            }

            if (ModelId != null && ModelId.Length > 0)
            {
                try
                {
                    if (MatchMode == TemplateMatchMode.Gray)
                        HOperatorSet.ClearNccModel(ModelId);
                    else
                        HOperatorSet.ClearShapeModel(ModelId);
                }
                catch
                {
                }

                ModelId = null;
            }
        }
    }

    [Serializable]
    public class TemplateMatchResultParam
    {
        public string TemplateName { get; set; }
        public TemplateMatchMode MatchMode { get; set; }
        public int Index { get; set; }
        public double Row { get; set; }
        public double Col { get; set; }
        public double AngleDeg { get; set; }
        public double Scale { get; set; } = 1.0;
        public double Score { get; set; }
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
        public double OffsetRDeg { get; set; }
        public string MatrixText { get; set; }
        public string BaseToCurrentMatrixText { get; set; }
        public string CurrentToBaseMatrixText { get; set; }
        public bool IsSuccess { get; set; }
        public bool HasFollowRoi { get; set; }
        public double FollowRow1 { get; set; }
        public double FollowCol1 { get; set; }
        public double FollowRow2 { get; set; }
        public double FollowCol2 { get; set; }
        public string StatusText { get; set; }
        public string ErrorMessage { get; set; }
    }
}
