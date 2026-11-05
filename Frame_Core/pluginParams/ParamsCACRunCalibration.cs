using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;
using YYSD.UICore.Control;

namespace Frame_Core.pluginParams
{
    [Serializable]
    public class ParamsCACRunCalibration : PluginParamsBase,IDisposable
    {
        #region 1.输入变量

        /// <summary>
        /// 输入变量：原图。
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        [ShowTextAttribute("输入-原图")]
        [Description("输入变量：待识别二维码/棋盘格的原始图像，由流程上游传入，不参与XML保存")]
        public HObject ho_Image;

        /// <summary>
        /// 输入变量：插件是否启用。
        /// </summary>
        [ShowTextAttribute("输入-是否启用")]
        [Description("输入变量：是否启用插件；false时跳过执行并返回true")]
        public bool m_bIsEnable = true;

        /// <summary>
        /// 输入变量：是否启用矩形ROI。
        /// </summary>
        [ShowTextAttribute("输入-启用ROI")]
        [Description("输入变量：二维码识别时是否只在ViewROI矩形区域内执行")]
        public bool m_bEnableRoi = true;

        /// <summary>
        /// 输入变量：是否显示ViewROI。
        /// </summary>
        [ShowTextAttribute("输入-显示ViewROI")]
        [Description("输入变量：是否在插件窗体中显示并允许编辑ViewROI矩形")]
        public bool m_bShowViewRoi = true;

        /// <summary>
        /// 输入变量：矩形ROI左上角行坐标。
        /// </summary>
        [ShowTextAttribute("输入-ROI Row1")]
        [Description("输入变量：矩形ROI第一点行坐标，单位pixel")]
        public double m_fRoiRow1 = 100;

        /// <summary>
        /// 输入变量：矩形ROI左上角列坐标。
        /// </summary>
        [ShowTextAttribute("输入-ROI Col1")]
        [Description("输入变量：矩形ROI第一点列坐标，单位pixel")]
        public double m_fRoiCol1 = 100;

        /// <summary>
        /// 输入变量：矩形ROI右下角行坐标。
        /// </summary>
        [ShowTextAttribute("输入-ROI Row2")]
        [Description("输入变量：矩形ROI第二点行坐标，单位pixel")]
        public double m_fRoiRow2 = 1200;

        /// <summary>
        /// 输入变量：矩形ROI右下角列坐标。
        /// </summary>
        [ShowTextAttribute("输入-ROI Col2")]
        [Description("输入变量：矩形ROI第二点列坐标，单位pixel")]
        public double m_fRoiCol2 = 1800;

        /// <summary>
        /// 输入变量：棋盘格行数。
        /// </summary>
        [ShowTextAttribute("输入-棋盘行数")]
        [Description("输入变量：棋盘格行数，用于将二维码序号映射到棋盘行索引")]
        public int m_nBoardRows = 10;

        /// <summary>
        /// 输入变量：棋盘格列数。
        /// </summary>
        [ShowTextAttribute("输入-棋盘列数")]
        [Description("输入变量：棋盘格列数，用于将二维码序号映射到棋盘列索引")]
        public int m_nBoardColumns = 10;

        /// <summary>
        /// 输入变量：棋盘X方向间距。
        /// </summary>
        [ShowTextAttribute("输入-X间距")]
        [Description("输入变量：棋盘格X方向物理间距，单位mm")]
        public double m_fSpacingX = 10;

        /// <summary>
        /// 输入变量：棋盘Y方向间距。
        /// </summary>
        [ShowTextAttribute("输入-Y间距")]
        [Description("输入变量：棋盘格Y方向物理间距，单位mm")]
        public double m_fSpacingY = 10;

        /// <summary>
        /// 输入变量：棋盘物理坐标原点X。
        /// </summary>
        [ShowTextAttribute("输入-原点X")]
        [Description("输入变量：棋盘物理坐标原点X，单位mm")]
        public double m_fOriginX = 0;

        /// <summary>
        /// 输入变量：棋盘物理坐标原点Y。
        /// </summary>
        [ShowTextAttribute("输入-原点Y")]
        [Description("输入变量：棋盘物理坐标原点Y，单位mm")]
        public double m_fOriginY = 0;

        /// <summary>
        /// 输入变量：二维码物理宽度。
        /// </summary>
        [ShowTextAttribute("输入-二维码宽度")]
        [Description("输入变量：二维码物理宽度，单位mm，用于中心点补偿")]
        public double m_fQrPhysicalWidth = 10;

        /// <summary>
        /// 输入变量：二维码物理高度。
        /// </summary>
        [ShowTextAttribute("输入-二维码高度")]
        [Description("输入变量：二维码物理高度，单位mm，用于中心点补偿")]
        public double m_fQrPhysicalHeight = 10;

        /// <summary>
        /// 输入变量：二维码序号起始值。
        /// </summary>
        [ShowTextAttribute("输入-序号起始值")]
        [Description("输入变量：二维码文本中提取到的序号起始值，例如首个二维码为1则填1")]
        public int m_nSerialBaseIndex = 1;

        /// <summary>
        /// 输入变量：每行二维码数量。
        /// </summary>
        [ShowTextAttribute("输入-每行二维码数")]
        [Description("输入变量：二维码序号换行计算用，每行包含的二维码数量")]
        public int m_nQrPerRow = 5;

        /// <summary>
        /// 输入变量：二维码列步距。
        /// </summary>
        [ShowTextAttribute("输入-二维码列步距")]
        [Description("输入变量：相邻二维码在棋盘列索引上的步距")]
        public int m_nQrColumnStep = 2;

        /// <summary>
        /// 输入变量：二维码行步距。
        /// </summary>
        [ShowTextAttribute("输入-二维码行步距")]
        [Description("输入变量：相邻二维码行在棋盘行索引上的步距")]
        public int m_nQrRowStep = 1;

        /// <summary>
        /// 输入变量：奇数二维码行列偏移。
        /// </summary>
        [ShowTextAttribute("输入-奇数行列偏移")]
        [Description("输入变量：奇数二维码行额外增加的棋盘列索引偏移")]
        public int m_nQrOddRowColumnOffset = 1;

        /// <summary>
        /// 输入变量：是否反转行轴。
        /// </summary>
        [ShowTextAttribute("输入-反转行轴")]
        [Description("输入变量：是否将棋盘行索引反向映射到物理Y坐标")]
        public bool m_bInvertRowAxis = false;

        /// <summary>
        /// 输入变量：是否反转列轴。
        /// </summary>
        [ShowTextAttribute("输入-反转列轴")]
        [Description("输入变量：是否将棋盘列索引反向映射到物理X坐标")]
        public bool m_bInvertColumnAxis = false;

        /// <summary>
        /// 输入变量：HALCON二维码极性。
        /// </summary>
        [ShowTextAttribute("输入-二维码极性")]
        [Description("输入变量： FindDataCode2d的polarity参数，界面使用ComboBox选择")]
        public string m_strQrPolarity = "dark_on_light";

        /// <summary>
        /// 输入变量：HALCON二维码识别参数。
        /// </summary>
        [ShowTextAttribute("输入-识别参数")]
        [Description("输入变量： CreateDataCode2dModel的default_parameters参数，界面使用ComboBox选择")]
        public string m_strQrDefaultParameters = "maximum_recognition";

        /// <summary>
        /// 输入变量：二维码中心来源。
        /// </summary>
        [ShowTextAttribute("输入-中心来源")]
        [Description("输入变量：二维码中心点来源，SmallestRectangleCenter或DataCodeCenter，界面使用ComboBox选择")]
        public string m_strCenterSource = "SmallestRectangleCenter";

        /// <summary>
        /// 输入变量：映射模型。
        /// </summary>
        [ShowTextAttribute("输入-映射模型")]
        [Description("输入变量：当前固定使用仿射矩阵，界面使用ComboBox选择")]
        public string m_strMappingModel = "Affine";

        /// <summary>
        /// 输入变量：最大识别数量。
        /// </summary>
        [ShowTextAttribute("输入-最大识别数量")]
        [Description("输入变量：FindDataCode2d stop_after_result_num参数")]
        public int m_nStopAfterResultNum = 97;

        /// <summary>
        /// 输入变量：最大允许误差。
        /// </summary>
        [ShowTextAttribute("输入-最大允许误差")]
        [Description("输入变量：允许的最大拟合误差，单位mm，用于界面提示和质量判断")]
        public double m_fMaxAllowedErrorMm = 0.2;

        /// <summary>
        /// 输入变量：待转换像素行坐标。
        /// </summary>
        [ShowTextAttribute("输入-像素Row")]
        [Description("输入变量：待转换的像素行坐标Row，单位pixel")]
        public double m_fInputPixelRow = 0;

        /// <summary>
        /// 输入变量：待转换像素列坐标。
        /// </summary>
        [ShowTextAttribute("输入-像素Col")]
        [Description("输入变量：待转换的像素列坐标Col，单位pixel")]
        public double m_fInputPixelCol = 0;

        /// <summary>
        /// 输入变量：矩阵保存路径。
        /// </summary>
        [ShowTextAttribute("输入-矩阵保存路径")]
        [Description("输入变量：矩阵XML保存/加载路径；为空时插件按流程路径自动生成")]
        public string m_strMatrixSavePath = string.Empty;

        #endregion

        #region 2.矩阵参数

        /// <summary>
        /// 矩阵参数：像素坐标到棋盘物理坐标的仿射矩阵，XML保存用。
        /// </summary>
        [ShowTextAttribute("矩阵-像素转棋盘")]
        [Description("矩阵参数：生成矩阵后的像素到棋盘仿射矩阵，List<double>仅用于XML保存；运行时会转HTuple")]
        public List<double> m_lPixelToBoardMatrix = new List<double>();

        /// <summary>
        /// 矩阵参数：棋盘物理坐标到像素坐标的仿射矩阵，XML保存用。
        /// </summary>
        [ShowTextAttribute("矩阵-棋盘转像素")]
        [Description("矩阵参数：生成矩阵后的棋盘到像素仿射矩阵，List<double>仅用于XML保存；运行时会转HTuple")]
        public List<double> m_lBoardToPixelMatrix = new List<double>();

        /// <summary>
        /// 运行时变量：像素到棋盘矩阵HTuple。
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        [ShowTextAttribute("运行时-像素转棋盘HTuple")]
        [Description("运行时变量：由m_lPixelToBoardMatrix转成的Tuple，不参与XML保存")]
        public HTuple m_hvPixelToBoardHomMat;

        /// <summary>
        /// 运行时变量：棋盘到像素矩阵HTuple。
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        [ShowTextAttribute("运行时-棋盘转像素HTuple")]
        [Description("运行时变量：由m_lBoardToPixelMatrix转成的Tuple，不参与XML保存")]
        public HTuple m_hvBoardToPixelHomMat;

        #endregion

        #region 3.输出变量

        /// <summary>
        /// 输出变量：棋盘物理坐标X。
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        [ShowTextAttribute("输出-棋盘X")]
        [Description("输出变量：像素坐标转换后的棋盘物理坐标X，单位mm")]
        public double m_fOutputBoardX = 0;

        /// <summary>
        /// 输出变量：棋盘物理坐标Y。
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        [ShowTextAttribute("输出-棋盘Y")]
        [Description("输出变量：像素坐标转换后的棋盘物理坐标Y，单位mm")]
        public double m_fOutputBoardY = 0;

        /// <summary>
        /// 输出变量：坐标转换是否成功。
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        [ShowTextAttribute("输出-转换成功")]
        [Description("输出变量：像素坐标转换是否成功；失败时查看运行信息")]
        public bool m_bTransformSuccess = false;

        /// <summary>
        /// 输出变量：是否存在有效矩阵。
        /// </summary>
        [ShowTextAttribute("输出-矩阵有效")]
        [Description("输出变量：当前是否存在可用于坐标转换的有效矩阵")]
        public bool m_bHasValidMatrix = false;

        /// <summary>
        /// 输出变量：矩阵状态文本。
        /// </summary>
        [ShowTextAttribute("输出-矩阵状态")]
        [Description("输出变量：矩阵状态显示文本，例如矩阵有效、无有效矩阵、矩阵报错")]
        public string m_strMatrixStateText = "无有效矩阵";

        /// <summary>
        /// 输出变量：矩阵自检/生成信息。
        /// </summary>
        [ShowTextAttribute("输出-矩阵信息")]
        [Description("输出变量：矩阵自检、加载、生成或报错的详细信息")]
        public string m_strMatrixCheckMessage = "未自检";

        /// <summary>
        /// 输出变量：最后保存时间。
        /// </summary>
        [ShowTextAttribute("输出-最后保存时间")]
        [Description("输出变量：参数或矩阵最后保存到本地的时间")]
        public string m_strLastSaveTime = string.Empty;

        /// <summary>
        /// 输出变量：检测到的二维码数量。
        /// </summary>
        [ShowTextAttribute("输出-识别数量")]
        [Description("输出变量：识别到的二维码数量")]
        public int m_nDetectedCount = 0;

        /// <summary>
        /// 输出变量：参与拟合的有效点数量。
        /// </summary>
        [ShowTextAttribute("输出-参与拟合数量")]
        [Description("输出变量：成功解析序号并参与矩阵拟合的有效二维码点数量")]
        public int m_nUsedCount = 0;

        /// <summary>
        /// 输出变量：平均拟合误差。
        /// </summary>
        [ShowTextAttribute("输出-平均误差")]
        [Description("输出变量：矩阵拟合平均误差，单位mm")]
        public double m_fAvgErrorMm = 0;

        /// <summary>
        /// 输出变量：RMS拟合误差。
        /// </summary>
        [ShowTextAttribute("输出-RMS误差")]
        [Description("输出变量：矩阵拟合RMS误差，单位mm")]
        public double m_fRmsErrorMm = 0;

        /// <summary>
        /// 输出变量：最大拟合误差。
        /// </summary>
        [ShowTextAttribute("输出-最大误差")]
        [Description("输出变量：矩阵拟合最大误差，单位mm")]
        public double m_fMaxErrorMm = 0;

        /// <summary>
        /// 输出变量：显示图像。
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        [ShowTextAttribute("输出-显示图像")]
        [Description("输出变量：插件用于显示的图像，不参与XML保存")]
        public HObject m_ho_OutImgShow;

        /// <summary>
        /// 输出变量：二维码轮廓。
        /// </summary>
        [NonSerialized]
        [XmlIgnore]
        [ShowTextAttribute("输出-二维码轮廓")]
        [Description("输出变量：识别到的二维码XLD轮廓，不参与XML保存")]
        public HObject m_ho_OutQrXlds;

        public void Dispose()
        {
            m_ho_OutImgShow?.Dispose();
            m_ho_OutQrXlds?.Dispose();
            //throw new NotImplementedException();
        }

        #endregion
    }
}
