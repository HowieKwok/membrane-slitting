using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class SlaveMachineParam : TCPClientParam
    {

        [Description("相机打标编号")]
        public int nCamDBIndex { set; get; } = 6;

        public BindingList<CoatingZone> Zones { get; } = new BindingList<CoatingZone>();

        public void ZonesInit()
        {
            if (Zones.Count != 12)
            {
                Zones?.Clear();
                for (int i = 0; i < 12; i++)
                {
                    Zones.Add(new CoatingZone() { ZoneNumber = i + 1 });
                }
                this.Save();
            }
        }
    }
}
