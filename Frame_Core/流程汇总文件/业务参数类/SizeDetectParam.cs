using DefectRuleClassifier.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core.业务参数类
{

    /// <summary>
    /// 尺寸检测业务参数
    /// </summary>
    public class SizeDetectParam : BaseProParam
    {




        public SizeDetectParam(string cameraName) : base(cameraName)
        {
        }
        public override void Dispose()
        {
         
            base.Dispose();
        }


    }
}
