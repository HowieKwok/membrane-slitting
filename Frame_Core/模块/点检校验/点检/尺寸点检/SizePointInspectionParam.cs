using Frame_Cor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 尺寸点检参数
    /// </summary>
    public class SizePointInspectionParam : XmlSerializationOperation
    {
        /// <summary>
        /// 点检图片路径
        /// </summary>
        public string PointCheckImgPath = @"E:\点检图像\尺寸点检图像";

        /// <summary>
        /// 点检算法
        /// </summary>
        public int SizePointInspectionArithmetic = (int)SizePointInspectionArithmeticEnum.矩形菲林片;


        /// <summary>
        /// 矩形菲林片参数
        /// </summary>
        public RectangleFilmParam p_RectangleFilm = new RectangleFilmParam();

        /// <summary>
        /// 线宽菲林片参数
        /// </summary>
        public LineWidthFilmParam p_LineWidthFilm = new LineWidthFilmParam();


        /// <summary>
        /// 点检工作参数
        /// </summary>
        public SizePointInspectionWorkParam sizePointInspectionWorkParam = new SizePointInspectionWorkParam();



        /// <summary>
        /// 线参数
        /// </summary>
        public List<Param_Line> LineParams = new List<Param_Line>();

        /// <summary>
        /// 尺寸逻辑参数
        /// </summary>
        public List<Param_SizeLogic_UserDefined> SizeLogicParams_UserDefineds = new List<Param_SizeLogic_UserDefined>();
    }
}
