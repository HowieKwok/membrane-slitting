using ControlBoard.板卡库;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace ControlBoard.Interface
{
    public interface IControlCard
    {
        /// <summary>
        /// 检查板卡是否连接
        /// </summary>
        /// <returns>0未连接，非0已连接</returns>
         int IsConnnected();

        /// <summary>
        /// 板卡初始化
        /// </summary>
        /// <returns></returns>
         int Board_Init();
        /// <summary>
        /// 板卡关闭
        /// </summary>
        /// <returns></returns>
         int Board_Close();

        /// <summary>
        /// 设置输出
        /// </summary>
        /// <param name="cardno">卡号</param>
        /// <param name="ioNo">位号</param>
        /// <param name="highOrLow">电平信号</param>
        /// <returns></returns>
         int Write_outbit(ushort cardno, ushort bitno, int on_off);

        /// <summary>
        /// 读取输入状态
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="ioNo"></param>
        /// <returns></returns>
         int  Read_Inbit(ushort cardno, ushort bitno);

        /// <summary>
        /// 读取输出点状态
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="ioNo"></param>
        /// <returns></returns>
         int Read_Outbit(ushort cardno, ushort m_PortNo);


        /// <summary>
        /// 设置点位滤波时间
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
         int Set_Filter(ushort cardno, double filter);



        /// <summary>
        /// 读取指定控制卡的全部通用输入口的电平状态
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="m_PortNo"></param>
        /// <returns></returns>
         int Read_Inport(ushort cardno, ushort m_PortNo);

        /// <summary>
        /// 读取指定控制卡的全部通用输出口的电平状态
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="m_PortNo"></param>
        /// <returns></returns>Write_outbit
         int Read_Outport(ushort cardno, ushort m_PortNo);

        /// <summary>
        /// 设置指定控制卡的全部通用输出口的电平状态
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="m_PortNo"></param>
        /// <param name="port_value"></param>
        /// <returns></returns>
         int Write_Outport(ushort cardno, ushort m_PortNo, uint port_value);


        #region 中断
        /// <summary>
        /// 允许指定控制卡的中断
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="funcIntHandler"></param>
        /// <param name="operate_data"></param>
        /// <returns></returns>
         int Int_Enable(ushort cardno, IOC0640_OPERATE funcIntHandler, IntPtr operate_data);

        
        /// <summary>
        /// 禁止指定控制卡的中断 
        /// </summary>
        /// <param name="cardno"></param>
        /// <returns></returns>
        int Int_Disable(ushort cardno);


       /* 参 数：cardno 指定控制卡号, 范围（0 － N - 1 ,N为卡数）          
        bitno:   
        输入口位号(1-47)
        enable 输入口中断使能，0-无效,1有效
        logic
        输入口中断触发逻辑，0－表示中断信号上升沿有效，1－表示中断信号下降沿有效。*/

        /// <summary>
        /// 设置/读取指定控制卡输入口的使能设置和触发方式。 
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="bitno"></param>
        /// <param name="enable"></param>
        /// <param name="logic"></param>
        /// <returns></returns>
         int Config_Intbitmode(ushort cardno, ushort bitno, ushort enable, ushort logic);


         int Config_Intbitmode(ushort cardno, ushort bitno, ushort[] enable, ushort[] logic);

        /// <summary>
        /// 读取指定控制卡输入口的中断状态 
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="bitno"></param>
        /// <returns></returns>
         int Read_Intbitstatus(ushort cardno, ushort bitno);

        /// <summary>
        /// 设置指定控制卡输入端口的使能
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="m_PortNo"></param>
        /// <param name="port_en"></param>
        /// <returns></returns>
         int  Config_Intporten(ushort cardno, ushort m_PortNo, uint port_en);

        /// <summary>
        /// 设置指定控制卡输入端口的触发方式
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="m_PortNo"></param>
        /// <param name="port_logic"></param>
        /// <returns></returns>
         int Config_Intportlogic(ushort cardno, ushort m_PortNo, uint port_logic);

        /// <summary>
        /// 读取指定控制卡输入端口的使能和触发方式
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="m_PortNo"></param>
        /// <param name="enable"></param>
        /// <param name="logic"></param>
        /// <returns></returns>
         int Read_Intportmode(ushort cardno, ushort m_PortNo, uint[] enable, uint[] logic);

        /// <summary>
        /// 读取指定控制卡输入端口的中断状态
        /// </summary>
        /// <param name="cardno"></param>
        /// <param name="m_PortNo"></param>
        /// <returns></returns>
         int  Read_Intportstatus(ushort cardno, ushort m_PortNo);
        #endregion
    }
}
