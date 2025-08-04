using HalconDotNet;
using LogTool;
using NPOI.OpenXmlFormats.Vml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using YYSD.UICore.Control;

namespace Frame_Core
{
    [Serializable]
    public class Param_PretargetingCalib : XmlSerializationOperation
    {
        #region 弃用
        /// <summary>
        /// 标定点数量
        /// </summary>
        public int CalibrationPointNum = 9;
        /// <summary>
        /// 机械手坐标系标定偏移点
        /// </summary>
        public List<PointRotateInfo> MachineHandOffsetPoint;

        /// <summary>
        /// 图像坐标系标定点
        /// </summary>
        public List<PointRotateInfo> ImageCalibPoint;

        /// <summary>
        /// 机械手坐标系标定点
        /// </summary>
        public List<PointRotateInfo> MachineHandPoint;
        #endregion

        /// <summary>
        /// 棋盘格图片路径
        /// </summary>
        [Description("棋盘格图片路径")]
        public string CalibImgPath = "";

        #region 基准和补偿参数
        /// <summary>
        /// 机械手坐标系基准标定补偿值
        /// </summary>
        [Description("机械手坐标系基准标定补偿值")]
        public PointRotateInfo MachineHandBCValue = new PointRotateInfo();

        /// <summary>
        /// X轴位置取反
        /// </summary>

        [Description("X轴位置取反")]
        public bool NotBCValueX = false;
        /// <summary>
        /// Y轴位置取反
        /// </summary>
        [Description("Y轴位置取反")]
        public bool NotBCValueY = false;
        /// <summary>
        /// R轴位置取反
        /// </summary>
        [Description("R轴位置取反")]
        public bool NotBCValueR = false;

        /// <summary>
        /// 下电芯下单面偏移补偿
        /// </summary>
        [Description("下电芯下单面偏移补偿")]
        public PointRotateInfo MachineDownBCOffectValue = new PointRotateInfo();

        /// <summary>
        /// 下电芯上单面偏移补偿
        /// </summary>
        [Description("下电芯上单面偏移补偿")]
        public PointRotateInfo MachineUpBCOffectValue = new PointRotateInfo();

        /// <summary>
        /// 极片偏移补偿
        /// </summary>
        [Description("极片偏移补偿")]
        public PointRotateInfo MachineHandBCOffectValue = new PointRotateInfo();


        /// <summary>
        /// 上电芯上单面偏移补偿
        /// </summary>
        [Description("上电芯上单面偏移补偿")]
        public PointRotateInfo MachineUpBCOffectValue_Up = new PointRotateInfo();

        /// <summary>
        /// 上电芯下单面偏移补偿
        /// </summary>
        [Description("上电芯下单面偏移补偿")]
        public PointRotateInfo MachineDownBCOffectValue_Up = new PointRotateInfo();
        #endregion

        #region 基准设置
        /// <summary>
        /// 基准点X
        /// </summary>
        [Description("基准点X")]
        public double BasePointX = 0;

        /// <summary>
        /// 基准点Y
        /// </summary>
        [Description("基准点Y")]
        public double BasePointY = 0;

        public double BaseLineBeginRow = 0;
        public double BaseLineBeginCol = 0;
        public double BaseLineEndRow = 0;
        public double BaseLineEndCol = 0;

        /// <summary>
        /// 基准点X
        /// </summary>
        [Description("临时基准点X")]
        public double BasePointXTemp = 0;

        /// <summary>
        /// 基准点Y
        /// </summary>
        [Description("临时基准点Y")]
        public double BasePointYTemp = 0;

        public double BaseLineBeginRowTemp = 0;
        public double BaseLineBeginColTemp = 0;
        public double BaseLineEndRowTemp = 0;
        public double BaseLineEndColTemp = 0;

        /// <summary>
        /// 旋转中心X
        /// </summary>
        [Description("旋转中心X")]
        public double CenterOfRotationX = 0;

        /// <summary>
        /// 旋转中心Y
        /// </summary>
        [Description("旋转中心Y")]
        public double CenterOfRotationY = 0;

        /// <summary>
        /// 当前相机边位置信息
        /// </summary>
        public List<LineInfo> BaseLines;

        /// <summary>
        /// 当前相机边位置信息
        /// </summary>
        public List<LineInfo> BaseLinesTemp = new List<LineInfo>();
        #endregion

        #region 棋盘格标定

        /// <summary>
        /// 棋盘格标定点数量
        /// </summary>
        [Description("棋盘格标定点数量")]
        public int CheckerboardCalibPointNum = 4;

        /// <summary>
        /// 棋盘格最小阈值
        /// </summary>
        [Description("棋盘格最小阈值")]
        public double CheckerboardMinThd = 0;

        /// <summary>
        /// 棋盘格最大阈值
        /// </summary>
        [Description("棋盘格最大阈值")]
        public double CheckerboardMaxThd = 20;

        /// <summary>
        /// 棋盘格方格边长X（单位Pixel）
        /// </summary>
        [Description("棋盘格方格边长X（单位Pixel）")]
        public double CheckerboardBoxSideLengthPix_X = 5;

        /// <summary>
        /// 棋盘格方格边长Y（单位Pixel）
        /// </summary>
        [Description("棋盘格方格边长Y（单位Pixel）")]
        public double CheckerboardBoxSideLengthPix_Y = 5;

        /// <summary>
        /// 棋盘格方格边长（单位mm）
        /// </summary>
        [Description("棋盘格方格边长（单位mm）")]
        public double CheckerboardBoxSideLength = 5;

        /// <summary>
        /// 线模型参数
        /// </summary>
        [Description("线模型参数")]
        public Param_LineModel LineModelParam = new Param_LineModel();

        /// <summary>
        /// 棋盘格标定图像坐标
        /// </summary>
        [Description("棋盘格标定图像坐标")]
        public List<PointRotateInfo> CheckerboardCalibrationImageCoord;

        /// <summary>
        /// 棋盘格标定棋盘格坐标
        /// </summary>
        [Description("棋盘格标定棋盘格坐标")]
        public List<PointRotateInfo> CheckerboardCalibrationCheckerboardCoord;

        /// <summary>
        /// ROI起始Row
        /// </summary>
        [Description("棋盘格ROI起始Row")]
        public double CheckerboardfRoiStartRow = 0.0;

        /// <summary>
        /// ROI起始Col
        /// </summary>
        [Description("棋盘格ROI起始Col")]
        public double CheckerboardfRoiStartCol = 0.0;

        /// <summary>
        /// ROI结束Row
        /// </summary>
        [Description("棋盘格ROI结束Row")]
        public double CheckerboardfRoiEndRow = 10.0;

        /// <summary>
        /// ROI结束Col
        /// </summary>
        [Description("棋盘格ROI结束Col")]
        public double CheckerboardfRoiEndCol = 10.0;
        #endregion

        #region 平台标定   
        /// <summary>
        /// 平台平移标定点数量
        /// </summary>
        [Description("平台平移标定点数量")]
        public int PlatformTranslationCalibPointNum = 9;

        /// <summary>
        /// 平台平移标定图像坐标
        /// </summary>
        [Description("平台平移标定图像坐标")]
        public List<PointRotateInfo> PlatformTranslationCalibImageCoord;

        /// <summary>
        /// 平台平移标定平台坐标
        /// </summary>
        [Description("平台平移标定平台坐标")]
        public List<PointRotateInfo> PlatformTranslationCalibPlatformCoord;

        /// <summary>
        /// 平台旋转标定点数量
        /// </summary>
        [Description("平台旋转标定点数量")]
        public int PlatformRotateCalibPointNum = 5;

        /// <summary>
        /// 平台旋转标定图像坐标
        /// </summary>
        [Description("平台旋转标定图像坐标")]
        public List<PointRotateInfo> PlatformRotateCalibImageCoord;

        /// <summary>
        /// 平台旋转标定平台坐标
        /// </summary>
        [Description("平台旋转标定平台坐标")]
        public List<PointRotateInfo> PlatformRotateCalibPlatformCoord;
        #endregion

        #region 平台标定参数设置
        /// <summary>
        /// 模板图片文件名
        /// </summary>
        public string ModelImage_handle_PathName = System.Environment.CurrentDirectory + "\\Model_handle.bmp";

        /// <summary>
        /// 模板文件名
        /// </summary>
        public string Model_handle_PathName = System.Environment.CurrentDirectory + "\\Model_handle.tup";

        /// <summary>
        /// 模板图像
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject Model_Image = new HObject();

        /// <summary>
        /// 模板模型
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple Model_handle = new HTuple();

        private FindRectangleLineParam[] _findRectangleLineParams;

        /// <summary>
        /// 抓边参数类//输入变量
        /// </summary>
        [ShowTextAttribute("抓边参数类")]
        [Description("抓边参数类")]
        public FindRectangleLineParam[] findRectangleLineParams = new FindRectangleLineParam[2] { new FindRectangleLineParam(), new FindRectangleLineParam() };

        [ShowTextAttribute("ModelROI起始Row")]
        [Description("ModelROI起始Row")]
        public double fModelRoiStartRow = 0.0;

        [ShowTextAttribute("ModelROI起始Col")]
        [Description("ModelROI起始Col")]
        public double fModelRoiStartCol = 0.0;

        [ShowTextAttribute("ModelROI结束Row")]
        [Description("ModelROI结束Row")]
        public double fModelRoiEndRow = 10.0;

        [ShowTextAttribute("ModelROI结束Col")]
        [Description("ModelROI结束Col")]
        public double fModelRoiEndCol = 10.0;
        #endregion

        /// <summary>
        /// 平台变换矩阵文件名
        /// </summary>
        public string HomMat2DPlatform_PathName = System.Environment.CurrentDirectory + "\\HomMat2DPlatform.mat";

        /// <summary>
        /// 棋盘格变换矩阵文件名
        /// </summary>
        public string HomMat2D_PathName = System.Environment.CurrentDirectory + "\\HomMat2D.mat";

        /// <summary>
        /// 反转的变换矩阵文件名
        /// </summary>
        public string HomMat2DInvert_PathName = System.Environment.CurrentDirectory + "\\HomMat2DInvert.mat";

        /// <summary>
        /// 平台变化矩阵
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple HomMat2DPlatform = new HTuple();

        /// <summary>
        /// 变化矩阵
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple HomMat2D = new HTuple();

        /// <summary>
        /// 反转的变换矩阵
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple HomMat2DInvert = new HTuple();

        public void ReadCalibParam()
        {
            if (System.IO.File.Exists(HomMat2DPlatform_PathName))
            {
                HomMat2DPlatform = HTuple.ReadTuple(HomMat2DPlatform_PathName);
            }

            if (System.IO.File.Exists(HomMat2D_PathName))
            {
                HomMat2D = HTuple.ReadTuple(HomMat2D_PathName);
            }

            if (System.IO.File.Exists(HomMat2DInvert_PathName))
            {
                HomMat2DInvert = HTuple.ReadTuple(HomMat2DInvert_PathName);
            }
        }

        public void SavePlatformCalibParam()
        {
            HomMat2DPlatform?.WriteTuple(HomMat2DPlatform_PathName);
        }

        public void SaveCheckerboardCalibParam()
        {
            HomMat2D?.WriteTuple(HomMat2D_PathName);
            HomMat2DInvert?.WriteTuple(HomMat2DInvert_PathName);
        }

        public void ReadModelHandle()
        {
            try
            {
                if (System.IO.File.Exists(Model_handle_PathName))
                {
                    HOperatorSet.ReadShapeModel(Model_handle_PathName, out Model_handle);                  
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                System.Windows.Forms.MessageBox.Show($"加载模板失败,路径：{Model_handle_PathName}");
            }

        }

        public void SaveModelHandle()
        {
            string fileName = Model_handle_PathName;
            try
            {
                HOperatorSet.WriteShapeModel(Model_handle, fileName);                

                fileName = ModelImage_handle_PathName;
                if (Model_Image != null && Model_Image?.CountObj() > 0)
                    HOperatorSet.WriteImage(Model_Image, "bmp", 0, ModelImage_handle_PathName);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex);
                System.Windows.Forms.MessageBox.Show($"保存模板失败,路径：{fileName}");
            }
        }
    }
    [Serializable]
    public class FindRectangleLineParam : XmlSerializationOperation
    {

        [ShowTextAttribute("ROI起始Row")]
        [Description("ROI起始Row")]
        public string FindLineName { get; set; } = "默认抓边1";

        [ShowTextAttribute("ROI起始Row")]
        [Description("ROI起始Row")]
        public double fRoiStartRow = 0.0;

        [ShowTextAttribute("ROI起始Col")]
        [Description("ROI起始Col")]
        public double fRoiStartCol = 0.0;

        [ShowTextAttribute("ROI结束Row")]
        [Description("ROI结束Row")]
        public double fRoiEndRow = 10.0;

        [ShowTextAttribute("ROI结束Col")]
        [Description("ROI结束Col")]
        public double fRoiEndCol = 10.0;

        [ShowTextAttribute("X或Y方向")]
        [Description("X或Y方向")]
        public int iCobInputXorY = 0;

        [ShowTextAttribute("卡尺间隔")]
        [Description("卡尺间隔")]
        public int iInputInterval = 30;

        /// <summary>
        /// 方向 L2R_New从左到右,U2B_New从上到下
        /// </summary>
        [ShowTextAttribute("卡尺方向")]
        [Description("卡尺方向")]
        public int iCobDirection = 1;

        [ShowTextAttribute("卡尺ROI长度")]
        [Description("卡尺ROI长度")]
        public double fInputRoiWidthLen = 5.0;

        private int _iInputAmplitudeThreshold = 30;

        [ShowTextAttribute("卡尺阈值")]
        [Description("卡尺阈值")]
        public int iInputAmplitudeThreshold = 30;

        [ShowTextAttribute("卡尺平滑系数")]
        [Description("卡尺平滑系数")]
        public double fInputSigma = 1.00;

        /// <summary>
        /// negative‘从亮到暗，‘positive’从暗到亮，‘all’。
        /// </summary>
        [ShowTextAttribute("明暗方向")]
        [Description("明暗方向")]
        public int iCobinputTransition = 0;


        /// <summary>
        ///'first'第一点,'last'最后一个,'all' 所有的边缘。
        /// </summary>
        [ShowTextAttribute("边缘选择")]
        [Description("边缘选择")]
        public int iCobinputPosition = 0;


    }
}
