using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ViewROI3;

namespace Frame_Core
{
    [Serializable]
    public class Param_MsaThick3D : XmlSerializationOperation
    {

        /// <summary>
        /// ROI参数列表
        /// </summary>

        [System.Xml.Serialization.XmlIgnore]
        public List<ROIParams> LtROIParams { get; set; } = new List<ROIParams>();

        /// <summary>
        /// ROI对应控件列表
        /// </summary>

        [System.Xml.Serialization.XmlIgnore]
        public List<Dictionary<string, Control>> LtDictionary { get; set; } = new List<Dictionary<string, Control>>();


        public bool bIsCheck { get; set; } = false;


        /// <summary>
        /// 3D生产工程文件名
        /// </summary>
        public string  strProductJobName { get; set; } = "Product3D.job";

        /// <summary>
        /// 3D点检工程文件名
        /// </summary>
        public string strProductCheckJobName { get; set; } = "ProductCheck3D.job";


        public string LtROIParamsPath { get; set; } = "";



        public string LtDictionaryPath { get; set; } = "";



        public void LoadRoiParam()
        {
            this.LtROIParams = SaveAndLoad_Data.Load_Data<List<ROIParams>>(LtROIParamsPath);
            if (LtROIParams == null)
            {
                LtROIParams = new List<ROIParams>();
            }

            LtDictionary?.Clear();
            LtDictionary = TableLayoutPanelSerializer.LoadFromLtDicFile(LtDictionaryPath);
            if (LtDictionary == null)
            {
                LtDictionary = new List<Dictionary<string, Control>>();
          
            }
        }




        public void SaveRoiParam()
        {
            SaveAndLoad_Data.Save_Data<List<ROIParams>>(LtROIParams, LtROIParamsPath);
            TableLayoutPanelSerializer.SaveLtDicToFile(LtDictionary, LtDictionaryPath);
        }

  




    }
}
