using Frame_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public class CorrectParam : XmlSerializationOperation
    {
        /// <summary>
        /// 纠偏通讯类型
        /// </summary>
        public int JPConnect_Type = 0;

        /// <summary>
        /// E+L纠偏器IP地址
        /// </summary>
        public string strELMachineIP = "127.0.0.1";

        /// <summary>
        /// E+L纠偏器端口号
        /// </summary>
        public int strELMachinePort = 5050;

        /// <summary>
        /// 电脑本地IP地址
        /// </summary>
        public string strLocalMachineIP = "127.0.0.1";

        /// <summary>
        /// 电脑本地端口号
        /// </summary>
        public int strLocalMachinePort = 5060;




        /// <summary>
        /// 启用自动纠偏
        /// </summary>
        public bool ENAutoJP = false;
        /// <summary>
        /// 纠偏发送符号反转
        /// </summary>
        public bool JP_Sign = false;
        /// <summary>
        /// 纠偏采样距离(单位：mm)
        /// </summary>
        public int Correct_distance = 1000;
        /// <summary>
        /// 纠偏比例系数
        /// </summary>
        public double JP_Pro = 0.5;//
        /// <summary>
        /// 纠偏逻辑
        /// </summary>
        public int CorrectLogic = 0;


        /// <summary>
        /// 采样最大值
        /// </summary>
        public double JP_MaxValue = 2;
        /// <summary>
        /// 采样最小值
        /// </summary>
        public double JP_MinValue = 0.1;

        /// <summary>
        /// 最大停机报警个数
        /// </summary>
        public int JP_Max_errNUM = 3;

        /// <summary>
        /// 最大停机报警使能
        /// </summary>
        public bool JP_Max_errEn = false;

        /// <summary>
        /// 纠偏符号连续相同报警个数
        /// </summary>
        public int Sign_LX_Num = 5;

        /// <summary>
        /// 纠偏符号连续相同报警使能
        /// </summary>
        public bool Sign_LY_En = false;

        /// <summary>
        /// 手动纠偏值
        /// </summary>
        public double Manu_JPValue = 0;

        /// <summary>
        /// 换卷N米开启自动纠偏(单位：M)
        /// </summary>
        public double JP_HJLength = 0;

        /// <summary>
        /// 换卷纠偏使能
        /// </summary>
        public bool EnHJJP = false;

    }
}
