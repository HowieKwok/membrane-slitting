using System;
using System.Runtime.InteropServices;

namespace GTN
{
	public class mc_la
	{
		public const short LA_AXIS_NUM = 8;
		public const short LA_WORK_AXIS_NUM = 6;
		public const short LA_MACHINE_AXIS_NUM = 5;
		public const short AXIS_LIMIT_NONE = 0;
		public const short AXIS_LIMIT_MAX_VEL = 1;
		public const short AXIS_LIMIT_MAX_ACC = 2;
		public const short AXIS_LIMIT_MAX_DV = 4;
		public const short KIN_MSG_BUFFER_SIZE = 32;
		
		//工件坐标系下轨迹是否限制速度模式
		public enum EWorkLimitMode
		{
			WORK_LIMIT_INVALID=0,		//不限制
			WORK_LIMIT_VALID=1,			//限制生效
		};
		
		//设置的速度定义规则
		public enum EVelSettingDef
		{
			NORMAL_DEF_VEL=0,			//输入为轴坐标系所有轴的合成速度
			NUM_DEF_VEL=1,				//以NUM系统的规则定义
			CUT_DEF_VEL=2,				//速度为切削速度
		};
		
		//设置的加速度定义规则
		public enum EAccSettingDef
		{
			NORMAL_DEF_ACC=0,             //输入即输出
			LONG_AXIS_ACC=1,                //长轴最大速度
		};
		
		//机床类型
		public enum EMachineMode
		{
			NORMAL_THREE_AXIS=0,		//标准三轴机床模式
			MULTI_AXES=1,					//多轴联动模式
			FIVE_AXIS=2,					//五轴机床模式，轴坐标系为主，工件坐标系为辅
			FIVE_AXIS_WORK=3,				//五轴机床模式，工件坐标系为主，轴坐标系为辅
			ROBOT=4,
		};
        //前瞻参数结构体
        public struct TLookAheadParameter
        {
            public int lookAheadNum;                    //前瞻段数
            public double time;                     //时间常数
            public double radiusRatio;					//曲率限制调节参数
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public double[] vMax;           //各轴的最大速度
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public double[] aMax;           //各轴的最大加速度
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public double[] DVMax;          //各轴的最大速度变化量（在时间常数内）
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public double[] scale;          //各轴的脉冲当量
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public short[] axisRelation;    //输入坐标和内部坐标的对应关系
                                            //[MarshalAs(UnmanagedType.ByValTStr,SizeConst=128)]
            public string machineCfgFileName;       //机床配置文件名
        };

        //////////////////////////////////////
        public struct RC_KIN_CONFIG
		{
			public short RobotType;
			public short reserved1;
		
			public short KinParUse1;
            public short KinParUse2;
            public short KinParUse3;
            public short KinParUse4;
            public short KinParUse5;
            public short KinParUse6;
            public short KinParUse7;
            public short KinParUse8;
            public short KinParUse9;
            public short KinParUse10;
            public short KinParUse11;
            public short KinParUse12;
            public short KinParUse13;
            public short KinParUse14;
            public short KinParUse15;
            public short KinParUse16;
            public short KinParUse17;
            public short KinParUse18;
			public double KinPar1;
            public double KinPar2;
            public double KinPar3;
            public double KinPar4;
            public double KinPar5;
            public double KinPar6;
            public double KinPar7;
            public double KinPar8;
            public double KinPar9;
            public double KinPar10;
            public double KinPar11;
            public double KinPar12;
            public double KinPar13;
            public double KinPar14;
            public double KinPar15;
            public double KinPar16;
            public double KinPar17;
            public double KinPar18;
			public short KinLimitUse1;
            public short KinLimitUse2;
            public short KinLimitUse3;
            public short KinLimitUse4;
            public short KinLimitUse5;
            public short KinLimitUse6;
            public short KinLimitUse7;
            public short KinLimitUse8;
            public short KinLimitUse9;
            public short KinLimitUse10;
            public short KinLimitUse11;
            public short KinLimitUse12;
			public double KinLimitMin1;
            public double KinLimitMin2;
            public double KinLimitMin3;
            public double KinLimitMin4;
            public double KinLimitMin5;
            public double KinLimitMin6;
            public double KinLimitMin7;
            public double KinLimitMin8;
            public double KinLimitMin9;
            public double KinLimitMin10;
            public double KinLimitMin11;
            public double KinLimitMin12;
			public double KinLimitMax1;
            public double KinLimitMax2;
            public double KinLimitMax3;
            public double KinLimitMax4;
            public double KinLimitMax5;
            public double KinLimitMax6;
            public double KinLimitMax7;
            public double KinLimitMax8;
            public double KinLimitMax9;
            public double KinLimitMax10;
            public double KinLimitMax11;
            public double KinLimitMax12;
			public double KinLimitMinShift1;
            public double KinLimitMinShift2;
            public double KinLimitMinShift3;
            public double KinLimitMinShift4;
            public double KinLimitMinShift5;
            public double KinLimitMinShift6;
            public double KinLimitMinShift7;
            public double KinLimitMinShift8;
            public double KinLimitMinShift9;
            public double KinLimitMinShift11;
            public double KinLimitMinShift12;
			public double KinLimitMaxShift1;
            public double KinLimitMaxShift2;
            public double KinLimitMaxShift3;
            public double KinLimitMaxShift4;
            public double KinLimitMaxShift5;
            public double KinLimitMaxShift6;
            public double KinLimitMaxShift7;
            public double KinLimitMaxShift8;
            public double KinLimitMaxShift9;
            public double KinLimitMaxShift10;
            public double KinLimitMaxShift11;
		    public double KinLimitMaxShift12;

			public short AxisUse1;
            public short AxisUse2;
            public short AxisUse3;
            public short AxisUse4;
            public short AxisUse6;
            public short AxisUse7;
            public short AxisUse8;
			public char AxisPosSignSwitch1;
            public char AxisPosSignSwitch2;
            public char AxisPosSignSwitch3;
            public char AxisPosSignSwitch4;
            public char AxisPosSignSwitch5;
            public char AxisPosSignSwitch6;
            public char AxisPosSignSwitch7;
            public char AxisPosSignSwitch8;
			public double AxisPosOffset1;
            public double AxisPosOffset2;
            public double AxisPosOffset3;
            public double AxisPosOffset4;
            public double AxisPosOffset5;
            public double AxisPosOffset6;
            public double AxisPosOffset7;
            public double AxisPosOffset8;
		
			public short CartUnitUse1;
            public short CartUnitUse2;
            public short CartUnitUse3;
            public short CartUnitUse4;
            public short CartUnitUse5;
            public short CartUnitUse6;
			public char CartPosKCSSignSwitch1;
            public char CartPosKCSSignSwitch2;
            public char CartPosKCSSignSwitch3;
            public char CartPosKCSSignSwitch4;
            public char CartPosKCSSignSwitch5;
            public char CartPosKCSSignSwitch6;
			public short reserved21;
            public short reserved22;
            public short reserved23;
			public double CartPosKCSOffset1;
            public double CartPosKCSOffset2;
            public double CartPosKCSOffset3;
            public double CartPosKCSOffset4;
            public double CartPosKCSOffset5;
            public double CartPosKCSOffset6;
		};
		
		public struct RC_ERROR_INTERFACE
		{
			public char Error;
			public short ErrorID;
			public string Message;
		};
		
		public struct RC_MSG_BUFFER_ELEMENT
		{
			public short ErrorID;
			public string Message;
			public string LogTime;
			public int InternalID;
		};
		
		public struct RC_MSG_BUFFER 
		{
			public short LastMsgIndex;
			public RC_MSG_BUFFER_ELEMENT MsgElement1;
            public RC_MSG_BUFFER_ELEMENT MsgElement2;
            public RC_MSG_BUFFER_ELEMENT MsgElement3;
            public RC_MSG_BUFFER_ELEMENT MsgElement4;
            public RC_MSG_BUFFER_ELEMENT MsgElement5;
            public RC_MSG_BUFFER_ELEMENT MsgElement6;
            public RC_MSG_BUFFER_ELEMENT MsgElement7;
            public RC_MSG_BUFFER_ELEMENT MsgElement8;
            public RC_MSG_BUFFER_ELEMENT MsgElement9;
            public RC_MSG_BUFFER_ELEMENT MsgElement10;
            public RC_MSG_BUFFER_ELEMENT MsgElement11;
            public RC_MSG_BUFFER_ELEMENT MsgElement12;
            public RC_MSG_BUFFER_ELEMENT MsgElement13;
            public RC_MSG_BUFFER_ELEMENT MsgElement14;
            public RC_MSG_BUFFER_ELEMENT MsgElement15;
            public RC_MSG_BUFFER_ELEMENT MsgElement16;
            public RC_MSG_BUFFER_ELEMENT MsgElement17;
            public RC_MSG_BUFFER_ELEMENT MsgElement18;
            public RC_MSG_BUFFER_ELEMENT MsgElement19;
            public RC_MSG_BUFFER_ELEMENT MsgElement20;
            public RC_MSG_BUFFER_ELEMENT MsgElement21;
            public RC_MSG_BUFFER_ELEMENT MsgElement22;
            public RC_MSG_BUFFER_ELEMENT MsgElement23;
            public RC_MSG_BUFFER_ELEMENT MsgElement24;
            public RC_MSG_BUFFER_ELEMENT MsgElement25;
            public RC_MSG_BUFFER_ELEMENT MsgElement26;
            public RC_MSG_BUFFER_ELEMENT MsgElement27;
            public RC_MSG_BUFFER_ELEMENT MsgElement28;
            public RC_MSG_BUFFER_ELEMENT MsgElement29;
            public RC_MSG_BUFFER_ELEMENT MsgElement30;
            public RC_MSG_BUFFER_ELEMENT MsgElement31;
            public RC_MSG_BUFFER_ELEMENT MsgElement32;
			public int LastMsgID;
		};
		
		//旋转轴范围设置
		public struct TRotationAxisRange
		{
			public int primaryAxisRangeOn;              //第一旋转轴限定范围是否生效，0：不生效，1：生效
			public int slaveAxisRangeOn;                //第二旋转轴限定范围是否生效，0：不生效，1：生效
			public double maxPrimaryAngle;              //第一旋转轴最大值
			public double minPrimaryAngle;              //第一旋转轴最小值
			public double maxSlaveAngle;                //第二旋转轴最大值
			public double minSlaveAgnle;                //第二旋转轴最小值
		};
		
		//选解参数
		public enum EGroupSelect
		{
			Continuous=0,
			Group_1,
			Group_2,
		};
		
		public struct TPos
		{
			public double machinePos1;
            public double machinePos2;
            public double machinePos3;
            public double machinePos4;
            public double machinePos5;
			public double workPos1;
            public double workPos2;
            public double workPos3;
            public double workPos4;
            public double workPos5;
            public double workPos6;
		};
		
		//速度规划模式
		public enum EVelMode
		{
			T_CURVE=0,
			S_CURVE=1,
			S_CURVE_NEW=2,                  //根据加加速度、最大加速度进行S曲线速度前瞻，2015.11.16
		
			VEL_MODE_MAX=0x10000,         //确保长度为4Byte
		};
		
		//////////////////////////////////
		public enum OptimizeState
        {
            OPT_OFF=0,
            OPT_ON=1
        };

		public enum OptimizeMethod
        {
            NO_OPT=0,
            OPT_BLENDING=1, 
            OPT_CIRCLEFITTING=2,
            OPT_CUBICSPLINE=3,
            OPT_BSPLINE=4
        };
		
		public enum ErrorID
        {
            INIT_ERROR=1,		//没有进行参数初始化
			PASSWORD_ERROR=2,		//密码错误，请在固高运动控制平台上运行
			INDATA_ERROR=3,		//输入数据错误（检查圆弧数据是否正确）
			PRE_PROCESS_ERROR=4,	//
			TOOL_RADIUS_COMPENSATE_ERROR_INOUT=5,		//刀具半径补偿错误：进入/结束刀补处不能是圆弧
			TOOL_RADIUS_COMPENSATE_ERROR_NOCROSS=6,	//刀具半径补偿错误：数据不合理，无法计算交点
			USERDATA_ERROR=7,
		};
		
		//轨迹优化参数结构体
		public struct TOptimizeParamUser 
		{
			public OptimizeState usePathOptimize;	//是否使用路径优化：OPT_OFF:不使用	OPT_ON:使用
		
			public float tolerance;				//公差(suggest: rough:0.1, pre-finish:0.05, finish:0.01)
		
			public OptimizeMethod optimizeMethod;	//选择曲线优化方式
		
			public OptimizeState keepLargeArc;		//是否保留大圆弧：OPT_OFF：不保留， OPT_ON：保留
		
			public float blendingMinError;			//blending的最小设定误差
		
			public float blendingMaxAngle;			//blending的最大角度限制（即当线段向量角度大于该角度时，不做blending，单位：度）
		
		};
		
		public struct TErrorInfo 
		{
			public ErrorID errorID;		//错误号(INIT_ERROR:未初始化参数；PRE_PROCESS_ERROR:预处理模块错误；
									//TOOL_RADIUS_COMPENSATE_ERROR:刀具半径补偿错误；)
			public int errorRowNum;		//错误行号
		};

        public struct TPreStartPos
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = LA_AXIS_NUM)]
            public double[] Pos;
        };

        public struct TBufIoDelayData 
			{
                public ushort doType;
                public ushort doMask;
                public ushort doValue;
                public ushort delayTime;
                public short fifo;
			};

			public struct TBufDoBitPulseData
			{
                public short doType;
                public short doIndex;
                public ushort highLevelTime;
                public ushort lowLevelTime;
                public int pulseNum;
                public short firstLevel;
                public short fifo;
			};

			public struct TBufDaData
			{
                public short channel;
                public short daValue;
                public short fifo;
			};

			public struct TBufLaserData
			{
                public short channel;
                public short fifo;
                public short source;
                public double laserPower;
                public double ratio;
                public double minPower;
                public double maxPower;
                public short tableId;
			};

			public struct TBufGearData
			{
                public short axis;
                public double deltaPos;
                public short fifo;
                public short smoothFlag;
                public short accPercent;
                public short decPercent;
			};

			public struct TBufMoveData
			{
                public short axis;
                public double pos;
                public double vel;
                public short modal;
                public short fifo;
			};

			public struct TBufSegNumData
			{
                public int segNum;
                public short fifo;
			};
		
        //Look ahead Ex
        [DllImport("gts.dll")]
        public static extern short  GTN_SetupLookAheadCrd(short core,short crd,EMachineMode machineMode);
				[DllImport("gts.dll")]
        public static extern short  GTN_SetFollowAxisParaEx(short core,short crd, ref short pAxisLimitMode, ref double pVmax, ref double pAmax, ref double pDVmax);
        [DllImport("gts.dll")]
        public static extern short  GTN_SetVelDefineModeLa(short core,short crd,EVelSettingDef velDefMode);
        [DllImport("gts.dll")]
        public static extern short  GTN_SetAxisLimitModeLa(short core,short crd,ref int pAxisLimitMode);
        [DllImport("gts.dll")]
        public static extern short  GTN_SetWorkLimitModeLa(short core,short crd,EWorkLimitMode workLimitMode);
        [DllImport("gts.dll")]
        public static extern short  GTN_SetAxisVelValidModeLa(short core,short crd,int velValidAxis);
        [DllImport("gts.dll")]
        public static extern short  GTN_SetVelSmoothMode(short core,int crd,int smoothMode);
        [DllImport("gts.dll")]
        public static extern short  GTN_SetArcAllowErrorLa(short core,short crd, double error);
        [DllImport("gts.dll")]
       
        public static extern short GTN_InitLookAheadEx(short core, short crd, ref TLookAheadParameter pLookAheadPara, short fifo, short motionMode, ref TPreStartPos pPreStartPos);

        [DllImport("gts.dll")]
        public static extern short  GTN_PrintLACmdLa(short core,short crd,int printFlag, int clearFile);

        [DllImport("gts.dll")]
        public static extern short  GTN_LnXYEx(short core,short crd,double x,double y,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_LnXYG0Ex(short core,short crd,double x,double y,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_LnXYZEx(short core,short crd,double x,double y,double z,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_LnXYZG0Ex(short core,short crd,double x,double y,double z,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_LnXYZAEx(short core,short crd,double x,double y,double z,double a,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_LnXYZAG0Ex(short core,short crd,double x,double y,double z,double a,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_LnXYZACEx(short core,short crd,ref double pPos,short posMask,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_LnXYZACG0Ex(short core,short crd,ref double pPos,short posMask,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_LnXYZACUVWEx(short core,short crd,ref double pPos,short posMask,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_LnXYZACUVWG0Ex(short core,short crd,ref double pPos,short posMask,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_ArcXYREx(short core,short crd,double x,double y,double radius,short circleDir,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_ArcYZREx(short core,short crd,double y,double z,double radius,short circleDir,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_ArcZXREx(short core,short crd,double z,double x,double radius,short circleDir,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_ArcXYCEx(short core,short crd,double x,double y,double xCenter,double yCenter,short circleDir,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_ArcYZCEx(short core,short crd,double y,double z,double yCenter,double zCenter,short circleDir,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_ArcZXCEx(short core,short crd,double z,double x,double zCenter,double xCenter,short circleDir,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_ArcXYZEx(short core,short crd,double x,double y,double z,double interX,double interY,double interZ,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_HelixXYRZEx(short core,short crd,double x,double y,double z,double radius,short circleDir,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_HelixXYCZEx(short core,short crd,double x,double y,double z,double xCenter,double yCenter,short circleDir,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_BufDelayEx(short core,short crd,ushort delayTime,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_BufGearEx(short core,short crd,short gearAxis,double deltaPos,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_BufGearPercentEx(short core,short crd,short gearAxis,double deltaPos,short accPercent,short decPercent,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_BufMoveEx(short core,short crd,short moveAxis,double pos,double vel,double acc,short modal,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_BufIOEx(short core,short crd,ushort doType,ushort doMask,ushort doValue,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_BufDAEx(short core,short crd,short chn,short daValue,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_SetUserSegNumEx(short core,short crd,int segNum,short fifo);

        //Add By lin.ga 20150330
        [DllImport("gts.dll")]
        public static extern short  GTN_BufLaserOnEx(short core,short crd,short fifo,short channel);
        [DllImport("gts.dll")]
        public static extern short  GTN_BufLaserOffEx(short core,short crd,short fifo,short channel);
        [DllImport("gts.dll")]
        public static extern short  GTN_BufLaserPrfCmdEx(short core,short crd,double laserPower,short fifo,short channel);
        [DllImport("gts.dll")]
        public static extern short  GTN_BufLaserFollowRatioEx(short core,short crd,double ratio,double minPower,double maxPower,short fifo,short channel);
        [DllImport("gts.dll")]
        public static extern short  GTN_BufLaserFollowModeEx(short core,short crd,short source ,short fifo,short channel);
        [DllImport("gts.dll")]
        public static extern short  GTN_BufLaserFollowSplineEx(short core,short crd,short tableId,double minPower,double maxPower,short fifo,short channel);
        [DllImport("gts.dll")]
        public static extern short  GTN_BufLaserFollowOffEx(short core,short crd,short fifo,short channel);
        [DllImport("gts.dll")]
        public static extern short  GTN_BufDisableDoBitPulseEx(short core,short crd,short doType,short doIndex,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_BufEnableDoBitPulseEx(short core,short crd,short doType,short doIndex,ushort highLevelTime,ushort lowLevelTime,int pulseNum,short firstLevel,short fifo);
        [DllImport("gts.dll")]
        public static extern short GTN_CrdDataEx(short core,short crd, System.IntPtr pCrdData, short fifo);  //调用时传入 IntPtr.Zero GTN_CrdDataEx(1, System.IntPtr.Zero, 0);
        [DllImport("gts.dll")]
        public static extern short  GTN_CrdDataExEx(short core,short crd,System.IntPtr pCrdData,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_GetLookAheadSegCountEx(short core,short crd,out int pSegCount,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_GetMotionTimeEx(short core,short crd,out double pTime,short fifo);

        /////////////////////////////////////////////////////////////////////////////////////////
        /////////////////lin.ga 20150701 Add PathOpt Fuction/////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////
        [DllImport("gts.dll")]
        public static extern short  GTN_SetPathOptPara(short core,short crd, ref TOptimizeParamUser optPrm,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_GetPathOptErrorInfo(short core,short crd,ref TErrorInfo errorInfo,short fifo);


        //5 Axis
        [DllImport("gts.dll")]
        public static extern short  GTN_CrdRTCPOn(short core,short crd,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_CrdRTCPOff(short core,short crd,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_UpdateMachineBuildingFile(short core,short crd, int update);
        [DllImport("gts.dll")]
        public static extern short  GTN_InitialMachineBuilding(short core,short crd, string pMachineCfgFileName,ref double machineCoordCenter, ref double workCoordCenter, double toolLength);
        [DllImport("gts.dll")]
        public static extern short  GTN_SetRotationAxisRange(short core,short crd, ref TRotationAxisRange pRotationAxisRange);
        [DllImport("gts.dll")]
        public static extern short  GTN_SetInverseSolutionSelectPara(short core,short crd, EGroupSelect groupSelect, int priorAxisSet);
        [DllImport("gts.dll")]
        public static extern short  GTN_MachineForwardTrans(short core,short crd, ref double pMachinePos, out double pWorkPos);
        [DllImport("gts.dll")]
        public static extern short  GTN_MachineRTCPTrans(short core,short crd, ref double pInputPos, ref double pMachinePos, ref double pWorkPos);
        [DllImport("gts.dll")]
        public static extern short  GTN_MachineTransformation(short core,short crd, int posType, ref double pPrePos, ref double pPos, ref int pPosNum, ref TPos pReturnPos);
        [DllImport("gts.dll")]
        public static extern short  GTN_SetCompToolLength(short core,short crd, double compToolLength);
        [DllImport("gts.dll")]
        public static extern short  GTN_SetCompWorkCoordOffset(short core,short crd, ref double pCompWorkOffset);
        [DllImport("gts.dll")]
        public static extern short  GTN_SetNonlinearErrorControl(short core,short crd, int enable, double nonlinearError);
        [DllImport("gts.dll")]
        public static extern short  GTN_EnableDiscreateArc(short core,short crd,short enable,double arcError);
        [DllImport("gts.dll")]
        public static extern short  GTN_StartXYCMachineMode(short core,short crd, short dir, double contactAngle, double rotationAngle, ref double pTranslation, double aValue,short fifo);//启动XYC模式
        [DllImport("gts.dll")]
        public static extern short  GTN_EndXYCMachineMode(short core,short crd,short fifo);                                                   //退出XYC模式
        [DllImport("gts.dll")]
        public static extern short  GTN_SetWorkCrdPlane(short core,short crd, short enable, ref double pNormVector, ref double pPoint,short fifo);    //设置玻璃加工平面参数
        [DllImport("gts.dll")]
        public static extern short  GTN_SetStartPointProcessMode(short core,short crd, short enable, double z,short fifo);                      //预处理第一段xyc数据
        [DllImport("gts.dll")]
        public static extern short  GTN_InitialMachineBuildingEx(short core,short crd, string pMachineCfgFileName,ref double machineCoordCenter, ref double workCoordCenter, double toolLength);
        [DllImport("gts.dll")]
        public static extern short  GTN_SetWorkCrdLaserFollowMode(short core,short crd,short enbale,short fifo,short chn);
        [DllImport("gts.dll")]
        public static extern short  GTN_ArcXYRACEx(short core,short crd,double x,double y,double a,double c,double radius,short circleDir,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_ArcXYCACEx(short core,short crd,double x,double y,double a,double c,double xCenter,double yCenter,short circleDir,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short  GTN_ArcXYZACEx(short core,short crd,double x,double y,double z,double a,double c,double interX,double interY,double interZ,double interA,double interC,double synVel,double synAcc,int segNum,short override2,short fifo);
        [DllImport("gts.dll")]
        public static extern short GTN_ArcZXRACEx(short core, short crd, double z, double x, double a, double c, double radius, short circleDir, double synVel, double synAcc, int segNum, short override2, short fifo);
        [DllImport("gts.dll")]
        public static extern short GTN_ArcYZRACEx(short core, short crd, double y, double z, double a, double c, double radius, short circleDir, double synVel, double synAcc, int segNum, short override2, short fifo);
        [DllImport("gts.dll")]
        public static extern short GTN_ArcYZCACEx(short core, short crd, double y, double z, double a, double c, double yCenter, double zCenter, short circleDir, double synVel, double synAcc, int segNum, short override2, short fifo);
        [DllImport("gts.dll")]
        public static extern short GTN_ArcZXCACEx(short core, short crd, double z, double x, double a, double c, double zCenter, double xCenter, short circleDir, double synVel, double synAcc, int segNum, short override2, short fifo);


        //缓冲区Glink指令
        [DllImport("gts.dll")]
        public static extern short GT_BufSetGLinkDoEx(short crd, short slaveno, ushort offset, ref byte pData, ushort bytelength, short fifo);
        [DllImport("gts.dll")]
        public static extern short GT_BufSetGLinkDoBitEx(short crd, short slaveno, short doIndex, byte value, short fifo);
        [DllImport("gts.dll")]
        public static extern short GT_BufSetGLinkAoEx(short crd, short slaveno, ushort channel, short pData, short fifo);

        [DllImport("gts.dll")]
        public static extern short GTN_BufSetGLinkDoEx(short core, short crd, short slaveno, ushort offset, ref byte pData, ushort bytelength, short fifo);
        [DllImport("gts.dll")]
        public static extern short GTN_BufSetGLinkDoBitEx(short core, short crd, short slaveno, short doIndex, byte value, short fifo);
        [DllImport("gts.dll")]
        public static extern short GTN_BufSetGLinkAoEx(short core, short crd, short slaveno, ushort channel, short pData, short fifo);


        public struct TWatchCondition
        {

            public GTN.mc.TWatchVar var;
            public ushort condition;
            public double value;
        }

        public struct TWaitTimeout
        {

            public Int32 time;
            public short mode;
            public short reserve1;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public double[] reserve2;
        }

        //缓冲区等待watchvar功能
        [DllImport("gts.dll")]
        public static extern short GT_BufWaitWatchVarEx(short crd,ref TWatchCondition pWaitCondition,ref TWaitTimeout pWaitTimeout, long segNum, short fifo);
        [DllImport("gts.dll")]
        public static extern short GTN_BufWaitWatchVarEx(short core, short crd,ref TWatchCondition pWaitCondition,ref TWaitTimeout pWaitTimeout, long segNum, short fifo);

        [DllImport("gts.dll")]
        public static extern short GTN_BufDoBitEx(short core, short crd, ushort doType, ushort index, short value, short fifo);
    }
}
