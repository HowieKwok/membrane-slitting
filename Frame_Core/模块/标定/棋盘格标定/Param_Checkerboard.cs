using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    [Serializable]
    public class Param_Checkerboard : XmlSerializationOperation
    {
        /// <summary>
        /// 棋盘格方格边长（单位mm）
        /// </summary>
        [Description("棋盘格方格边长（单位mm）")]
        public double CheckerboardBoxSideLength = 5;

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
        /// 标定点数量
        /// </summary>
        [Description("标定点数量")]
        public int CalibrationPointNum = 10;

        /// <summary>
        /// 线模型参数
        /// </summary>
        [Description("线模型参数")]
        public Param_LineModel LineModelParam = new Param_LineModel();


        /// <summary>
        /// 变换矩阵文件名
        /// </summary>
        [Description("变换矩阵文件名")]
        public string HomMat2D_PathName = System.Environment.CurrentDirectory + "\\HomMat2D.mat";

        /// <summary>
        /// 反转的变换矩阵文件名
        /// </summary>
        [Description("反转的变换矩阵文件名")]
        public string HomMat2DInvert_PathName = System.Environment.CurrentDirectory + "\\HomMat2DInvert.mat";

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
            if (System.IO.File.Exists(HomMat2D_PathName))
            {
                HomMat2D = HTuple.ReadTuple(HomMat2D_PathName);
            }

            if (System.IO.File.Exists(HomMat2DInvert_PathName))
            {
                HomMat2DInvert = HTuple.ReadTuple(HomMat2DInvert_PathName);
            }
        }

        public void SaveCalibParam()
        {
            HomMat2D?.WriteTuple(HomMat2D_PathName);
            HomMat2DInvert?.WriteTuple(HomMat2DInvert_PathName);
        }
    }
}
