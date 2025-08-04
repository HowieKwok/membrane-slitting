using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// OVH层数计算
    /// </summary>
    public class OVHPackgeNumLayerSim
    {
        /// <summary>
        /// 当前电芯编号
        /// </summary>
        public int CurrentPackageNum_StackTable = 0;

        /// <summary>
        /// 当前电芯层数
        /// </summary>
        public int CurrentLayer_StackTable = 0;


        public void CalculatePackgeNumLayer(int MaxPackageNum, int MaxLayer, int StartLayer, int step)
        {
            if (CurrentPackageNum_StackTable == 0 || CurrentPackageNum_StackTable > MaxPackageNum)
            {
                CurrentPackageNum_StackTable = 1;
            }

            if (CurrentLayer_StackTable == 0 || CurrentLayer_StackTable > MaxLayer)
            {
                CurrentLayer_StackTable = StartLayer;
            }
            else
            {
                CurrentLayer_StackTable += step;
            }

            if (CurrentLayer_StackTable > MaxLayer)
            {
                CurrentPackageNum_StackTable = CurrentPackageNum_StackTable + 1;
                CurrentLayer_StackTable = StartLayer;
            }


            if (CurrentPackageNum_StackTable > MaxPackageNum)
            {
                CurrentPackageNum_StackTable = 1;
            }
        }
    }
}
