
using NPOI.HSSF.Record;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    [Serializable]
    public class CraftBase : JsonSerializationOperation
    {
        /// <summary>
        /// 工艺名称
        /// </summary>
        public string CraftName { get; set; } = "默认配方";

        /// <summary>
        /// 物料工艺类型
        /// </summary>
        public MaterialCrafEnum materialCrafEnum { get; set; } = (MaterialCrafEnum)0;

        /// <summary>
        /// 主相机,基准相机
        /// </summary>
        public CameraEnum BaseCamera { get; set; } = (CameraEnum)0;

        /// <summary>
        /// 产品极性
        /// </summary>
        public ElectrodeEnum electrode { get; set; } = (ElectrodeEnum)0;


        /// <summary>
        /// 产区
        /// </summary>
        public int factory { get; set; } = (int)FactoryEnum.北京纯锂;

        /// <summary>
        /// 相机数量
        /// </summary>
        [Description("相机数量")]
        public int CamNum { get; set; } = 8;


        public virtual void Init() { }
        /// <summary>
        /// 当前设备为主机设备
        /// </summary>
        public bool IsHostMachine { get; set; } = true;
    }
}
