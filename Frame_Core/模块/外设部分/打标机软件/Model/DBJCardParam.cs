using Frame_Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 打标机板卡参数类
    /// </summary>
    public class DBJCardParam : TCPClientParam
    {
      
        [Description("自动打标")]
        public bool AutoDB { set; get; }

        public BindingList<CoatingZone> Zones { get; } = new BindingList<CoatingZone>();


        public DBJCardParam()
        {
            //Init();
        }
        public void ZonesInit()
        {
            if(Zones.Count!=12)
            {
                Zones?.Clear();
                for (int i = 0; i < 12; i++)
                {
                    Zones.Add(new CoatingZone() { ZoneNumber=i+1});
                }
                this.Save();
            }
        }
        public override void Init()
        {
            ComType = 0;
            IsConnect = true;
            LocalPC_IP = "127.0.0.1";
            PLC_IP = "127.0.0.1";
            DevPort = 2000;
            AutoDB = false;

        }

    }

}
