using ControlBoard.Interface;
using ControlBoard.板卡库;
using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//返回等于0表示成功，否则失败
namespace ControlBoard.板卡类型工具
{

  
    public class ControlIOC0640 : IControlCard
    {



        private int _Connected = 0;
        public int Board_Close()
        {
           IOC0640.ioc_board_close();
            Log4netRecord.ErrorLog.Error("卡已关闭");
            _Connected = 0;
            return 0;
        }

        public int Board_Init()
        {
             int ret = IOC0640.ioc_board_init();
            if (ret == 0)
            {            
                Log4netRecord.ErrorLog.Error("IO board init failed");
            }
            _Connected = ret;
            return ret;
        }

        /*功  能：设置/读取指定控制卡输入口的使能设置和触发方式。 
        参  数：cardno 指定控制卡号, 范围（0 － N - 1 ,N为卡数）          
        bitno:   
        输入口位号(1-47) 
        enable  输入口中断使能，0-无效,1有效 
        logic    
        输入口中断触发逻辑，0－表示中断信号上升沿有效，1－表示中断信号下降沿有效。 
        返回值：错误代码*/
        /// <summary>
        /// 置/读取指定控制卡输入口的使能设置和触发方式。
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="bitno"></param>
        /// <param name="enable"></param>
        /// <param name="logic"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int Config_Intbitmode(ushort cardno, ushort bitno, ushort enable, ushort logic)
        {
            int ret= (int)IOC0640.ioc_config_intbitmode(cardno, bitno, enable, logic);
            return ret;  // 返回值：0表示成功；1表示失败
        }

        public int Config_Intbitmode(ushort cardno, ushort bitno, ushort[] enable, ushort[] logic)
        {
            int ret = (int)IOC0640.ioc_config_intbitmode(cardno, bitno, enable, logic);
            return ret;  // 返回值：0表示成功；1表示失败
        }

        /*功  能：设置指定控制卡输入端口的使能 
        参  数：cardno        
        指定控制卡号, 范围（0 － N - 1 ,N为卡数） 
        m_PortNo   端口号,范围(0-1) 
        port_en     
        返回值：错误代码 
        m_PortNo 为0时,bit0–bit31 位值分别代表第1–32 号输入端口的中断使能 
        m_PortNo 为1 时,bit33–bit47 位值分别代表第33–47 号输入端口的中断使能位值,0－表示输入
        口中断禁止，*/
                /// <summary>
        /// 设置指定控制卡输入端口的使能
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="m_PortNo"></param>
        /// <param name="port_en"></param>
        /// <returns></returns>
        public int Config_Intporten(ushort cardno, ushort m_PortNo, uint port_en)
        {
            int ret=  (int)IOC0640.ioc_config_intporten(cardno, m_PortNo, port_en);

            return ret;
        }

        /*功  能：设置指定控制卡输入端口的触发方式 
        参  数：cardno       指定控制卡号, 范围（0 － N - 1 ,N为卡数） 
        m_PortNo  端口号,范围(0-1)。 
        port_logic     端口号为0时,bit0– bit31 位值分别代表第1–32 号输入端口的中断触发方式。 
                    端口号为1时,bit33–bit47 位值分别代表第33–47 号输入端口的中 
        断触发方式。 
                           位值,0－表示输入口上升沿有效，1－表示输入口下降沿有效。*/
        /// <summary>
        /// 设置指定控制卡输入端口的触发方式 
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="m_PortNo"></param>
        /// <param name="port_logic"></param>
        /// <returns></returns>
        public int Config_Intportlogic(ushort cardno, ushort m_PortNo, uint port_logic)
        {
            int ret = (int)IOC0640.ioc_config_intportlogic(cardno, m_PortNo, port_logic);

            return ret;

        }

        /*功  能：禁止指定控制卡的中断 
        参  数：cardno指定控制卡号, 范围（0 － N - 1 ,N为卡数） 
        返回值：错误代码 
        4.4.4  I/O 口中断设置函数*/
        /// <summary>
        /// 禁止指定控制卡的中断 
        /// </summary>
        /// <param name="cardno"></param>
        /// <returns></returns>
        public int Int_Disable(ushort cardno)
        {
            int ret = (int)IOC0640.ioc_int_disable(cardno);

            return ret;

        }

        /*功  能：允许指定控制卡的中断 
        参  数：cardno     指定控制卡号, 范围（0 － N - 1 ,N为卡数） 
            funcIntHandler    中断回调函数指针, 
        函数指针类型:   UINT (*IOC0640_OPERATE)(PVOID  operate_data) 
         operate_data  回调函数传递参数*/

        /// <summary>
        /// 允许指定控制卡的中断
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="funcIntHandler"></param>
        /// <param name="operate_data"></param>
        /// <returns></returns>
        public int Int_Enable(ushort cardno, IOC0640_OPERATE funcIntHandler, IntPtr operate_data)
        {
            int ret = (int)IOC0640.ioc_int_enable(cardno, funcIntHandler, operate_data);

            return ret;
        }

        public int IsConnnected()
        {
            return _Connected;
            //throw new NotImplementedException();
        }

        public int Read_Inbit(ushort cardno, ushort ioNo)
        {
            int ret = IOC0640.ioc_read_inbit(cardno, ioNo);
            return ret;  // 返回值：0表示低电平；1表示高电平
        }

        /*功  能：读取指定控制卡的全部通用输入口的电平状态 
        参  数：cardno   指定控制卡号, 范围（0 － N - 1 ,N为卡数） 
               m_PortNo      端口号,范围(0-1) 
        返回值：m_PortNo 为0时,bit0 – bit31位值分别代表第1 – 32号输入端口值。 
                 m_PortNo 为1时,bit0 – bit14位值分别代表第33 – 47号输入端口值。 */
        /// <summary>
        /// 读取指定控制卡的全部通用输入口的电平状态
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="m_PortNo"></param>
        /// <returns></returns>
        public int Read_Inport(ushort cardno, ushort m_PortNo)
        {
            int ret = IOC0640.ioc_read_inport(cardno, m_PortNo);
            return ret;
        }
        /// <summary>
        /// ：读取指定控制卡输入口的中断状态
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="bitno"></param>
        /// <returns></returns>
        public int Read_Intbitstatus(ushort cardno, ushort bitno)
        {
           int ret=  IOC0640.ioc_read_intbitstatus(cardno, bitno);
           return ret;
        }


        /*参  数：cardno 指定控制卡号, 范围（0 － N - 1 ,N为卡数） 
        m_PortNo  端口号,范围(0-1) 
        port_en    m_PortNo为0时,bit0–bit31 位值分别代表第1–32 号输入端口的中断使能。 
        m_PortNo为1时,bit33–bit47 位值分别代表第33–47号输入端口的中断使能。 
         位值,0－表示输入口中断禁止，1－表示输入口中断允许。 
        port_logic    端口号为0时,bit0– bit31 位值分别代表第1–32 号输入端口的中断触发方式。 
                        端口号为1时,bit33–bit47 位值分别代表第33–47 号输入端口的中断 
        触发方式。 
                  位值,0－表示输入口上升沿有效，1－表示输入口下降沿有效。 
        返回值：错误代码*/
        /// <summary>
        /// 读取指定控制卡输入端口的使能和触发方式 
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="m_PortNo"></param>
        /// <param name="enable"></param>
        /// <param name="logic"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public int Read_Intportmode(ushort cardno, ushort m_PortNo, uint[] enable, uint[] logic)
        {
            int ret=  (int)IOC0640.ioc_read_intportmode(cardno, m_PortNo, enable, logic);
            return ret;
        }
        /// <summary>
        /// 读取指定控制卡输入端口的中断状态
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="m_PortNo"></param>
        /// <returns></returns>
        public int Read_Intportstatus(ushort cardno, ushort m_PortNo)
        {
            int ret=  IOC0640.ioc_read_intportstatus(cardno, m_PortNo);
            return ret;
        }

        /*功  能：读取指定控制卡的某一位输出口的电平状态 
        参  数：   cardno   
        bitno  
        指定控制卡号, 范围（0 － N - 1 ,N为卡数） 
        指定输入口位号（取值范围：1－47） 
        返回值：0表示低电平；1表示高电平。 */
        /// <summary>
        /// 读取指定控制卡的某一位输出口的电平状态 
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="ioNo"></param>
        /// <returns></returns>
        public int Read_Outbit(ushort cardno, ushort ioNo)
        {
            int ret = IOC0640.ioc_read_outbit(cardno, ioNo);
            return ret;  // 返回值：0表示低电平；1表示高电平
        }


        /*功  能：读取指定控制卡的全部通用输出口的电平状态 
        参  数：cardno   指定控制卡号, 范围（0 － N - 1 ,N为卡数） 
               m_PortNo      端口号,范围(0-1) 
        返回值：m_PortNo 为0时,bit0 – bit31位值分别代表第1 – 32号输出端口值。 
                 m_PortNo 为1时,bit0 – bit15位值分别代表第33 – 48号输出端口值。 */
        /// <summary>
        /// 读取指定控制卡的全部通用输出口的电平状态 
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="m_PortNo"></param>
        /// <returns></returns>
        public int Read_Outport(ushort cardno, ushort m_PortNo)
        {
            int ret= IOC0640.ioc_read_outport(cardno, m_PortNo);
            return ret; 
        }



        public int Set_Filter(ushort cardno, double filter)
        {
            int ret= (int)IOC0640.ioc_set_filter(cardno, filter);
            return ret;
        }

        public int Write_outbit(ushort cardno, ushort ioNo, int highOrLow)
        {
            int ret = (int)IOC0640.ioc_write_outbit(cardno, ioNo, highOrLow);
            return ret;
        }

        public int Write_Outport(ushort cardno, ushort m_PortNo, uint port_value)
        {
            int ret = (int)IOC0640.ioc_write_outport(cardno, m_PortNo, port_value);
            return ret;
        }
    }
}
