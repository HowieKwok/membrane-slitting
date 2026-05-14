using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using YYSD.UICore.Control;

namespace Frame_Core.pluginParams
{
    [Serializable]
    public class ParamsBlobAnalysis : PluginParamsBase
    {
        [NonSerialized]
        [XmlIgnore]
        public HObject ho_Image;

        [ShowTextAttribute("判断是否启用")]
        [Description("判断是否启用")]
        public bool m_bIsEnable = true;

        [ShowTextAttribute("超时时间")]
        [Description("超时时间，单位ms")]
        public int m_nOverTime = 1000;

        [ShowTextAttribute("检测模式")]
        [Description("0=全图检测，1=局部ROI检测")]
        public int m_nDetectMode = 1;

        [ShowTextAttribute("启用局部ROI")]
        [Description("启用后只在局部ROI内做Blob分析")]
        public bool m_bUseLocalRoi = true;

        [ShowTextAttribute("启用屏蔽ROI")]
        [Description("启用后扣除多个屏蔽ROI")]
        public bool m_bUseMaskRoi = true;

        [ShowTextAttribute("显示ViewROI")]
        [Description("是否显示ViewROI编辑框")]
        public bool m_bShowViewRoi = true;

        [ShowTextAttribute("启用位置跟随")]
        [Description("启用Base到Current的位置跟随")]
        public bool m_bUsePositionFollow = false;

        public double m_fLocalRoiRow1 = 1000;
        public double m_fLocalRoiCol1 = 800;
        public double m_fLocalRoiRow2 = 2600;
        public double m_fLocalRoiCol2 = 3600;

        public double m_fThresholdMin = 0;
        public double m_fThresholdMax = 180;
        public int m_nMorphMode = 4;
        public double m_fMorphRadius = 3.0;
        public int m_nSortFeature = 0;
        public int m_nSortDirection = 1;

        public double m_fBaseRow = 0;
        public double m_fBaseCol = 0;
        public double m_fBaseAngle = 0;
        public double m_fCurrentRow = 0;
        public double m_fCurrentCol = 0;
        public double m_fCurrentAngle = 0;

        [ShowTextAttribute("屏蔽ROI列表")]
        [Description("多个屏蔽ROI")]
        public List<BlobMaskRoiParam> m_lMaskRoiParams = new List<BlobMaskRoiParam>();

        [ShowTextAttribute("过滤条件列表")]
        [Description("多个SelectShape过滤条件")]
        public List<BlobFilterRuleParam> m_lFilterRules = new List<BlobFilterRuleParam>();

        [NonSerialized]
        [XmlIgnore]
        public HObject m_ho_OutRegion;

        [NonSerialized]
        [XmlIgnore]
        public HObject m_ho_OutSelectedRegion;

        [NonSerialized]
        [XmlIgnore]
        public HObject m_ho_OutLocalRoiRegion;

        [NonSerialized]
        [XmlIgnore]
        public HObject m_ho_OutMaskRoiRegion;

        [NonSerialized]
        [XmlIgnore]
        public List<BlobRegionResultParam> m_lOutRegionResults = new List<BlobRegionResultParam>();


        [NonSerialized]
        [XmlIgnore]
        public bool m_bIsFind = false;

        [NonSerialized]
        [XmlIgnore]
        public int m_nRegionCount = 0;



        [NonSerialized]
        [XmlIgnore]
        public HTuple m_HtuFollowMat2D;

        public ParamsBlobAnalysis()
        {
            //m_lMaskRoiParams.Add(new BlobMaskRoiParam { m_nDisplayNo = 1, m_bIsEnable = true, m_strRoiName = "屏蔽ROI-1", m_nRoiShape = 0, m_fRow1 = 1300, m_fCol1 = 2100, m_fRow2 = 1600, m_fCol2 = 2600, m_fCenterRow = 1450, m_fCenterCol = 2350, m_fLength1 = 250, m_fLength2 = 150, m_fRadius = 80 });
            //m_lFilterRules.Add(new BlobFilterRuleParam { m_bIsEnable = true, m_strFeature = "area", m_fMin = 200, m_fMax = 999999, m_strOperation = "and" });
            //m_lFilterRules.Add(new BlobFilterRuleParam { m_bIsEnable = true, m_strFeature = "circularity", m_fMin = 0.3, m_fMax = 1.0, m_strOperation = "and" });
            //m_lFilterRules.Add(new BlobFilterRuleParam { m_bIsEnable = true, m_strFeature = "rectangularity", m_fMin = 0.45, m_fMax = 1.0, m_strOperation = "and" });
        }
    }

    [Serializable]
    public class BlobMaskRoiParam : XmlSerializationOperation
    {
        public int m_nDisplayNo = 1;
        public bool m_bIsEnable = true;
        public string m_strRoiName = "屏蔽ROI";
        public int m_nRoiShape = 0;
        public double m_fRow1 = 0;
        public double m_fCol1 = 0;
        public double m_fRow2 = 100;
        public double m_fCol2 = 100;
        public double m_fCenterRow = 50;
        public double m_fCenterCol = 50;
        public double m_fAngle = 0;
        public double m_fLength1 = 50;
        public double m_fLength2 = 20;
        public double m_fRadius = 30;
    }

    [Serializable]
    public class BlobFilterRuleParam : XmlSerializationOperation
    {
        public bool m_bIsEnable = true;
        public string m_strFeature = "area";
        public double m_fMin = 0;
        public double m_fMax = 999999;
        public string m_strOperation = "and";
    }

    [Serializable]
    public class BlobRegionResultParam : XmlSerializationOperation
    {
        public int m_nIndex = 0;
        public double m_fArea = 0;
        public double m_fCenterRow = 0;
        public double m_fCenterCol = 0;
        public double m_fRect2Row = 0;
        public double m_fRect2Col = 0;
        public double m_fRa = 0;
        public double m_fRb = 0;
        public double m_fLength1 = 0;
        public double m_fLength2 = 0;
        public double m_fAspectRatio = 0;
        public double m_fPhi = 0;
        public double m_fAngleDeg = 0;
        public double m_fCircularity = 0;
        public double m_fRectangularity = 0;
        public double m_fMeanGray = 0;
        public double m_fDeviationGray = 0;
        public double m_fMinGray = 0;
        public double m_fMaxGray = 0;
    }
}
