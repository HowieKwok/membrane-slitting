using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class PLCAddress_DieCuttingCut
    {
        //public CommunicationData<short> 外观CCD结果请求 = new CommunicationData<short>();
        //public CommunicationData<short> 外观CCD_结果 = new CommunicationData<short>();
        //public CommunicationData<short> 外观CCD_连续NG信号 = new CommunicationData<short>();
        public CommunicationData<bool> 心跳 = new CommunicationData<bool>();
        //public CommunicationData<short> 尺寸CCD_结果 = new CommunicationData<short>();
        //public CommunicationData<short> 尺寸CCD_连续NG信号 = new CommunicationData<short>();
        //public CommunicationData<short> CCD尺寸结果请求 = new CommunicationData<short>();
        public CommunicationData<float> V角纠偏值 = new CommunicationData<float>();
        public CommunicationData<float> 裁切纠偏值 = new CommunicationData<float>();
        public CommunicationData<float> 有无极耳 = new CommunicationData<float>();
        public CommunicationData<short> 清除纠偏数据 = new CommunicationData<short>();
        public CommunicationData<short> 寻极耳中 = new CommunicationData<short>();
        //public CommunicationData<short> 裁切结果请求 = new CommunicationData<short>();
        //public CommunicationData<short> V角深度补偿 = new CommunicationData<short>();
        //public CommunicationData<short> V角Y外数据 = new CommunicationData<short>();
        //public CommunicationData<short> 空箔报警 = new CommunicationData<short>();
        //public CommunicationData<short> 白班接带 = new CommunicationData<short>();
        //public CommunicationData<short> 夜班接带 = new CommunicationData<short>();

        public PLCAddress_DieCuttingCut()
        {
            Init();
        }
        public  void Init()
        {
            //外观CCD结果请求.SetAddress("D2100").SetName("外观CCD结果请求");
            //外观CCD_结果.SetAddress("D2010").SetName("外观CCD_结果");
            //外观CCD_连续NG信号.SetAddress("D2204").SetName("外观CCD_连续NG信号");
            心跳.SetAddress("D1004").SetName("心跳");
            //尺寸CCD_结果.SetAddress("D2012").SetName("尺寸CCD_结果");
            //尺寸CCD_连续NG信号.SetAddress("D2206").SetName("尺寸CCD_连续NG信号");
            //CCD尺寸结果请求.SetAddress("D2102").SetName("CCD尺寸结果请求");
            V角纠偏值.SetAddress("D2000").SetName("V角纠偏值");
            裁切纠偏值.SetAddress("D2002").SetName("裁切纠偏值");
            有无极耳.SetAddress("D2004").SetName("有无极耳");
            清除纠偏数据.SetAddress("D1002").SetName("清除纠偏数据");
            寻极耳中.SetAddress("D2208").SetName("寻极耳中");
            //裁切结果请求.SetAddress("D2104").SetName("裁切结果请求");
            //V角深度补偿.SetAddress("D1010").SetName("V角深度补偿");
            //V角Y外数据.SetAddress("D1012").SetName("V角Y外数据");
            //空箔报警.SetAddress("D2208").SetName("空箔报警");
            //白班接带.SetAddress("D2300").SetName("白班接带");
            //夜班接带.SetAddress("D2302").SetName("夜班接带");
        }
    }

 
}
