using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core.业务参数类
{
    /// <summary>
    /// 切刀定位业务参数
    /// </summary>
    public class CutCorrectParam : BaseProParam
    {

        public LineInfo LastLineInfo = new LineInfo();

        public CutCorrectParam(string cameraName) : base(cameraName)
        {
        }
        public override void Dispose()
        {
            LastLineInfo = new LineInfo();
            base.Dispose();
        }
    }
}
