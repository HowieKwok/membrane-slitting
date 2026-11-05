using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrameCommunity.DataOperation;

namespace Frame_Core
{
    public class NewImageSaveParamer : FrameCommunity.DataOperation.JsonSerializationOperation
    {
        /// <summary>
        /// 工艺（所属相机）
        /// </summary>
        private List<ImageSaveParamConfig> _ImageSaveConfigItems = new List<ImageSaveParamConfig>();
        public List<ImageSaveParamConfig> ImageSaveConfigItems
        {
            get => _ImageSaveConfigItems;
            set { _ImageSaveConfigItems = value; }
        }

        /// <summary>
        /// 读取此参数会执行
        /// </summary>
        public override void Init(int length)
        {
            if (ImageSaveConfigItems == null) ImageSaveConfigItems = new List<ImageSaveParamConfig>();
            while (ImageSaveConfigItems.Count < length)
            {
                ImageSaveConfigItems.Add(new ImageSaveParamConfig());
            }       
        }
    }
}
