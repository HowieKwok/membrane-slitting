using ControlBoard.Interface;
using ControlBoard.Model;
using ControlBoard.View;
using ControlBoard.板卡类型工具;
using FrameCommunity.DataOperation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
namespace ControlBoard.服务类
{
    public class ServiceCardManage
    {
        // 私有构造函数，防止外部 new
        private ServiceCardManage() { }

        // 静态只读锁对象，用于线程安全
        private static readonly object _lock = new object();


        public string strPath { get; set; } = "";

        // 静态实例变量
        private static ServiceCardManage _instance = null;

        // 公开的静态 Instance 属性
        public static ServiceCardManage Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new ServiceCardManage();
                        }
                    }
                }
                return _instance;
            }
        }

        // 实例字段（原静态字段改为实例字段）
        private IControlCard _controlCard { get; set; }
        public ParamControlCord Param控制卡 { get; set; }

        // 实例方法 外部可调用
        public IControlCard GetIControlCard()
        {
            return _controlCard;
        }

        public int InitServiceCard(ParamControlCord paramControlCord)
        {         
            Param控制卡 = paramControlCord;
            int ret = -1;
            switch (Param控制卡.ControlType)
            {
                case EnumControlType.IO0640:
                    _controlCard = new ControlIOC0640();
                    ret= _controlCard.Board_Init();
                    if(ret==0)
                    {
                        _controlCard.Set_Filter(paramControlCord.CardNo, paramControlCord.FilteTime);
                    }
                    return ret;
        
            }
            return ret;

           
        }


        MainView View ;
        public void ShowWindow()
        {
            View = new MainView(_controlCard, Param控制卡);
            View.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            View.ShowDialog();
            // 窗口显示逻辑
        }
    }
}
