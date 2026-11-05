using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameCommunity.公共类
{
    public enum DetectRangeMode { Global = 0, LocalRoi = 1 }
    public enum RoiEditRole { Local = 0, Mask = 1 }
    public enum RoiShapeType { Rectangle1 = 0, Rectangle2 = 1, Circle = 2 }

    public enum PartitionSplitDirection
    {
        /// <summary>沿 Rectangle1 的高度方向切分，分区宽度保持不变。</summary>
        LongSide = 0,

        /// <summary>沿 Rectangle1 的宽度方向切分，分区高度保持不变。</summary>
        WideSide = 1
    }

    public enum PartitionLengthUnit
    {
        /// <summary>分区长度按像素输入和保存。</summary>
        Pixel = 0,

        /// <summary>分区长度按毫米输入，运行前通过公共像素单量换算成像素。</summary>
        Millimeter = 1
    }
    public class EnumToltal
    {
     
    }
}
