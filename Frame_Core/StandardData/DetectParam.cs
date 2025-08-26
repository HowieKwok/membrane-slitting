using HalconDotNet;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
///定义标准：全局变量统一用m_开头
///用于检测使用的参数，

namespace Frame_Core
{
    /// <summary>
    /// 分类器用，描述缺陷信息
    /// </summary>
    public struct ClassificationDefectInfo
    {
        public HObject NgImage;//缺陷图小
        public int NgROIIndex;//缺陷ROI索引
        public int NgRowMin;//缺陷位置最小行坐标
        public int NgRowMax;//缺陷位置最大行坐标
        public int NgColMin;//缺陷位置最小行坐标
        public int NgColMax;//缺陷位置最大行坐标
        public double NgArea;//缺陷面积
        public double NgHigh;//缺陷高度\长度
        public double NgWidth;//缺陷宽度
        public double NgMeanGray;//平均灰度
        public string sAttachInfo;//缺陷附加信息
    }
    /// <summary>
    /// 缺陷NG信息，描述缺陷信息
    /// </summary>
    public class NgTypeInfo : IDisposable
    {

        public ImgType Type;
        private NgTypeInfo m_stdNgTypeInfoCopy;

        public CameraEnum cameraEnum { get; set; }

        //传出来的ID值从0开始
        public long m_nImgIndex { get; set; } = 0;

        public long m_iProductIndex { get; set; } = 0;


        public long m_iSliceIndex { get; set; } = 0;

        /// <summary>
        /// 结果类型
        /// </summary>
        public ResultType resultType = ResultType.NG;

        /// <summary>
        /// 图片：缺陷小图
        /// </summary>
        public HObject m_hImage;
        /// <summary>
        /// Ng类型
        /// </summary>
        public Defct1Enum m_strNgTypeName { get; set; } = Defct1Enum.其它;


        /// <summary>
        /// 图片：缺陷NgType区域
        /// </summary>
        public HObject m_hRegion { get; set; }

        /// <summary>
        /// 尺寸
        /// </summary>
        public double m_fMeasureSize { get; set; } = -1;

        /// <summary>
        /// NgType面积
        /// </summary>
        public double NgMidRow { get; set; } = -1;

        /// <summary>
        /// NgType面积
        /// </summary>
        public double NgMidColumn { get; set; } = -1;
        /// <summary>
        /// NgType面积
        /// </summary>
        public double m_fNgTypArea { get; set; } = -1;


        /// <summary>
        /// NgType高
        /// </summary>
        public double m_fNgTypHeight { get; set; } = -1;
        /// <summary>
        /// NgType宽
        /// </summary>
        public double m_fNgTypWidth { get; set; } = -1;
        /// <summary>
        /// NgType半径
        /// </summary>
        public double Diameter { get; set; } = 0;
        /// <summary>
        /// NgType绝对像素位置
        /// </summary>
        public long m_fNgTypeAbsPixelPos { get; set; } = 0;

        /// <summary>
        /// NgType圆度
        /// </summary>
        public double circularity { get; set; } = 0;
        /// <summary>
        /// 缺陷附加信息
        /// </summary>
        public string sAttachInfo { get; set; } = "";

        /// <summary>
        /// NG小图图片显示是否显示完成
        /// </summary>
        public bool m_bIsShowFinish = false;

        /// <summary>
        /// 图片是否保存完成
        /// </summary>
        public bool m_bIsSaveFinish = false;

        public NgTypeInfo()
        {

        }
        public NgTypeInfo Copy()
        {
            m_stdNgTypeInfoCopy = new NgTypeInfo();
            m_stdNgTypeInfoCopy.cameraEnum = cameraEnum;
            m_stdNgTypeInfoCopy.m_strNgTypeName = m_strNgTypeName;
            m_stdNgTypeInfoCopy.m_iProductIndex = m_iProductIndex;
            m_stdNgTypeInfoCopy.m_nImgIndex = m_nImgIndex;
            m_stdNgTypeInfoCopy.m_iSliceIndex = m_iSliceIndex; //分条数
            m_stdNgTypeInfoCopy.m_fMeasureSize = m_fMeasureSize;
            m_stdNgTypeInfoCopy.NgMidRow = NgMidRow;
            m_stdNgTypeInfoCopy.NgMidColumn = NgMidColumn;
            m_stdNgTypeInfoCopy.m_fNgTypArea = m_fNgTypArea;
            m_stdNgTypeInfoCopy.m_fNgTypHeight = m_fNgTypHeight;
            m_stdNgTypeInfoCopy.m_fNgTypWidth = m_fNgTypWidth;
            m_stdNgTypeInfoCopy.Diameter = Diameter;
            m_stdNgTypeInfoCopy.m_fNgTypeAbsPixelPos = m_fNgTypeAbsPixelPos;
            m_stdNgTypeInfoCopy.circularity = circularity;
            m_stdNgTypeInfoCopy.sAttachInfo = sAttachInfo;

            m_stdNgTypeInfoCopy.Dispose();
            return m_stdNgTypeInfoCopy;



        }
        public void Dispose()
        {
            m_hRegion?.Dispose();
            m_hImage?.Dispose();
        }
    }
    [Serializable]
    public class AlarmCraftInfo
    {
        public double UpTolerance = 0;
        public double DownTolerance = 0;
        public bool IsAlarm = false;
    }

    [Serializable]
    public class SizeInfo : IDisposable
    {
        public bool IsCalcued = false;
        /// <summary>
        /// 检测数据
        /// </summary>
        public double DataResult = -999;

        public SizeEnum sizeEnum = SizeEnum.AT9宽度;

        public int Index = 0;

        public string Name = "默认";

        public string SizeName = "默认";

        public string Description = "默认";
        /// <summary>
        /// 标准值
        /// </summary>
        public double StandardValue = 0;
        /// <summary>
        /// 上限值
        /// </summary>
        public double upperLimitValue = 0;
        /// <summary>
        /// 下限值
        /// </summary>
        public double lowerLimitValue = 0;

        /// <summary>
        /// 结果类型
        /// </summary>
        public ResultType resultType = ResultType.NG;

        /// <summary>
        /// 检测结果//true为NG
        /// </summary>
        public bool IsNG = true;

        public AlarmCraftInfo DBInfo = new AlarmCraftInfo();

        public AlarmCraftInfo BJInfo = new AlarmCraftInfo();

        public AlarmCraftInfo TJInfo = new AlarmCraftInfo();

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 图片总数据结果;单帧
    /// </summary>
    public class ImageDataResult : IDisposable
    {
        public bool m_bIsNG = true;//true为有瑕疵
        public long m_nImgIndex = 0;
        public long m_nJRId = 0;
        public double m_fExcuseTime = 0;
        public Queue<NgTypeInfo> m_qNgTypeInfos = new Queue<NgTypeInfo>();
        public Queue<SizeInfo> m_qNgSizeInfos = new Queue<SizeInfo>();
        public CoordDateResult m_coordDateResult = new CoordDateResult();
        public virtual void Dispose()
        {
            while (m_qNgTypeInfos.Count() > 0)
            {
                NgTypeInfo INgTypeInfo = m_qNgTypeInfos.Dequeue();
                INgTypeInfo?.Dispose();
            }

            while (m_qNgSizeInfos.Count() > 0)
            {
                SizeInfo INgSizeInfo = m_qNgSizeInfos.Dequeue();
                INgSizeInfo?.Dispose();
            }

        }
    }

    /// <summary>
    /// 坐标类
    /// </summary>
    public class CoordDateResult
    {

        public double fAngle = 0;
        public double fCol = 0;
        public double fRow = 0;
        public double fRobotCol = 0;
        public double fRobotRow = 0;

    }
}
