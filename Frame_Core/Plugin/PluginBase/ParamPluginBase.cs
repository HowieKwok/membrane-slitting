using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Frame_Core
{
    [Serializable]
    public class PluginParamsBase :XmlSerializationOperation
    {

        [NonSerialized]
        [XmlIgnore]
        public bool m_bRunSuccess = false;

        [NonSerialized]
        [XmlIgnore]
        public double m_fElapsedMs = 0;

        [NonSerialized]
        [XmlIgnore]
        public string m_strRunMessage = string.Empty;

    }
}
