using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYSD.UICore.Control;

namespace Frame_Core
{
    [Serializable]
    public class Param_MsaLineWidth:XmlSerializationOperation
    {

        [ShowTextAttribute("ROI起始Row")]
        [Description("ROI起始Row")]
        public double fRoiStartRow { get; set; } = 0.0;

        [ShowTextAttribute("ROI起始Col")]
        [Description("ROI起始Col")]
        public double fRoiStartCol { get; set; } = 0.0;

        [ShowTextAttribute("ROI结束Row")]
        [Description("ROI结束Row")]
        public double fRoiEndRow { get; set; } = 10.0;

        [ShowTextAttribute("ROI结束Col")]
        [Description("ROI结束Col")]
        public double fRoiEndCol { get; set; } = 10.0;

        [ShowTextAttribute("X或Y方向")]
        [Description("X或Y方向")]
        public int iCobInputXorY { get; set; } = 0;

        [ShowTextAttribute("卡尺间隔")]
        [Description("卡尺间隔")]
        public  int iInputInterval { get; set; } = 30;

        /// <summary>
        /// 方向 L2R_New从左到右,U2B_New从上到下
        /// </summary>
        [ShowTextAttribute("卡尺方向")]
        [Description("卡尺方向")]
        public int  iCobDirection { get; set; } = 1;

        [ShowTextAttribute("卡尺ROI长度")]
        [Description("卡尺ROI长度")]
        public double fInputRoiWidthLen { get; set; } = 5.0;

        private int _iInputAmplitudeThreshold=30;
        
        [ShowTextAttribute("卡尺阈值")]
        [Description("卡尺阈值")]
        public int iInputAmplitudeThreshold
        {
            get { return _iInputAmplitudeThreshold; }
            set
            {
                if (value < 0 || value > 255)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 and 255.");
                }
                _iInputAmplitudeThreshold = value;
            }
        }
        [ShowTextAttribute("卡尺平滑系数")]
        [Description("卡尺平滑系数")]
        public double fInputSigma { get; set; } = 1.00;

        /// <summary>
        /// negative‘从亮到暗，‘positive’从暗到亮，‘all’。
        /// </summary>
        [ShowTextAttribute("明暗方向")]
        [Description("明暗方向")]
        public int iCobinputTransition { get; set; } = 0;


        /// <summary>
        ///'first'第一点,'last'最后一个,'all' 所有的边缘。
        /// </summary>
        [ShowTextAttribute("边缘选择")]
        [Description("边缘选择")]
        public int iCobinputPosition { get; set; } = 0;



        /// <summary>
        ///Y方向像素单元
        /// </summary>
        [ShowTextAttribute("像素单元Y")]
        [Description("像素单元Y")]
        public double fScaleY { get; set; } = 0.00949766;


        private int _iDetectmplitudeThreshold = 30;

        [ShowTextAttribute("纵向检测阈值")]
        [Description("纵向检测阈值")]
        public int iDetectmplitudeThreshold
        {
            get { return _iDetectmplitudeThreshold; }
            set
            {
                if (value < 0 || value > 255)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "Value must be between 0 and 255.");
                }
                _iDetectmplitudeThreshold = value;
            }
        }

        [ShowTextAttribute("纵向卡尺宽度")]
        [Description("纵向卡尺宽度")]
        public double fDetectRoIWidth { get; set; } = 2.0;

        [ShowTextAttribute("到开始点线段长度")]
        [Description("到开始点线段长度")]
        public double fToMidStartPointLength { get; set; } = 150.0;

        [ShowTextAttribute("到结束点线段长度")]
        [Description("到结束点线段长度")]
        public double fToMidEndPointLength { get; set; } = 3500;

        [ShowTextAttribute("纵向平滑系数")]
        [Description("纵向平滑系数")]
        public double fYInputSigma { get; set; } = 1.00;


    }
}
