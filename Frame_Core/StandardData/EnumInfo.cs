using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{

    public enum CameraEnum
    {
        Cam1 = 0,
        Cam2 = 1,
        Cam3 = 2,
        Cam4 = 3,
        Cam5 = 4,
        Cam6 = 5,
        Cam7 = 6,
        Cam8 = 7,
    }

    /// <summary>
    /// 浮点数高低位转换
    /// 0 = ABCD；
    /// 1 = BADC;
    /// 2 = CDAB;
    /// 3 = DCBA;
    /// </summary>
    public enum emDataFormat
    {
        ABCD,
        BADC,
        CDAB,
        DCBA
    }
    public enum MaterialCrafEnum
    {
        [Language("宽幅打孔机", "BottomEdge1", "france")]
        PFT清洗机,

    }
    public enum JRLineEnum
    {
        [Language("极耳底边1", "BottomEdge1", "france")]
        极耳底边1,
        [Language("极耳底边2", "BottomEdge2", "france")]
        极耳底边2,
        [Language("极耳斜边1", "hypotenuse1", "france")]
        极耳斜边1,
        [Language("极耳斜边2", "hypotenuse2", "france")]
        极耳斜边2,
        [Language("极耳顶边", "TopEdge", "france")]
        极耳顶边,
        [Language("台阶边", "StepEdge", "france")]
        台阶边,
        // AT边,
        [Language("底部R1边", "BottomR1edge", "france")]
        底部R1边,//必须放在底边之后
        [Language("底部R2边", "BottomR2edge", "france")]
        底部R2边,//必须放在底边之后
    }
    public enum HalconColor
    {
        green,
        red,
        blue,
        yellow,
        magenta,
        cyan,
        black,
        white,
        gray,
        orange,
        violet,
        brown,
        pink,
        gold,
    }
    /// <summary>
    /// 尺寸编辑项，计算类型枚举
    /// </summary>
    public enum EditorTypeEnum
    {
        线距离X,
        线距离Y,
        线线交点,
        线线夹角,

        基准偏移X,
        基准偏移Y,
        极片隔膜Y,

        ACOH_X,//distance_pl
        ACOH_Y,//distance_pl
        SAOH, //distance_ss
        ACOH_Cor,

    }



    /// <summary>
    /// 报警名称
    /// </summary>
    public enum AlarmName//报警名称
    {
        检测项报警停机,
        副机未返回心跳回应信号,
        副机有硬件连接异常,
        主机有硬件连接异常,
        图像回调异常请重启软件,
        打标未识别,
        图像队列过多请关闭保存图片,
        片数据处理异常,

        打标机软件连接失败,
        打标机出标故障报警,//（定位对射）     
        打标机标签纸故障报警,//（出标感应）,   
        打标机标签纸末料故障报警,//(标签纸定位超过5s),    
        打标机标签纸断带故障报警,//（断带）,      
        打标机伺服故障报警,//伺服报警     
        帧数相差过多报警,
        主设备连接失败,
        连续NG停机报警,
        自动换卷失败,

        采图不正常停机报警,
        持续感应出标异常报警,
        编码轮未压到位报警,
        打标机未到位报警,
        打标机未收到设备启动信号报警,
        PLC编码器无脉冲信号报警,
        编码器压轮未压到位磁性开关报警,
        激光出标异常,
        NG信号不在接收范围,
        内侧打标过多,
        NG异常停机距离,

        纠偏器连接失败,
        纠编闭环伺服原点回归异常报警,
        纠编闭环伺服故障报警,
        纠偏内侧极限位置报警,
        纠偏外侧极限位置报警,
        纠偏超最大值报警个数,
        纠偏量超同符号报警个数,
        纠偏器工作模式异常,
        纠偏极限位置报警,

        AT9抓边参数或偏移设置有误,
        涂层抓边参数或偏移设置有误,
        边缘参数或偏移设置有误,
        极耳区域抓边参数或偏移设置有误,

        裁图模块有问题联系模块开发人员,
        找极耳模块有问题联系模块开发人员,
        缺陷检测模块有问题联系模块开发人员,

        检测流程1有误,
        检测流程2有误,
        检测流程3有误,
        检测流程4有误,
        检测流程5有误,
        检测流程6有误,

        工位选择错误,
        产品已存在,
        产品异常,
        读取设备信息错误,
        OVH未找到第一片电芯,
        OVH未找到上一片电芯,
        OVH叠台A获取结果异常,
        OVH叠台A获取结果超时,
        OVH叠台B获取结果异常,
        OVH叠台B获取结果超时,
        OVH叠台未开始电芯,

        OVH连续N次未识别Mark,
        OVH_Mark中心位置波动X超范围,
        OVH_Mark中心位置波动Y超范围,
        磁盘报警,


        尺寸点检失败,

        缺陷点检失败,

        已到点检时间请进行点检操作,
        灰度波动报警,    
        连续灰度波动超出范围报警,    
        灰度调节失败报警,
     
        已到点检时间请进行尺寸点检操作,     
        已到点检时间请进行缺陷点检操作,

    }


    public enum SizeEnum
    {
        [Language("极片尺寸X", "PolePieceX", "PolePieceX")]
        极片尺寸X,//PolePieceX
        [Language("极片尺寸Y", "PolePieceY", "PolePieceY")]
        极片尺寸Y,//PolePieceY
        [Language("极片角度", "PolePieceAngle", "PolePieceAngle")]
        极片角度,//PolePieceAngle

        [Language("极耳中心到上Mark_Y", "EarCentreToUpMarkY", "EarCentreToUpMarkY")]
        极耳中心到上Mark_Y,//EarCentreToUpMarkY
        [Language("极耳中心到下Mark_Y", "EarCentreToDownMarkY", "EarCentreToDownMarkY")]
        极耳中心到下Mark_Y,//EarCentreToDownMarkY
        [Language("极耳顶宽Y", "EarTopWidth", "EarTopWidth")]
        极耳顶宽Y,//EarTopWidth
        [Language("极耳底宽Y", "EarBottomWidth", "EarBottomWidth")]
        极耳底宽Y,//EarBottomWidth
        [Language("极耳高度", "EarHeight", "EarHeight")]
        极耳高度,//EarHeight
        [Language("AT9宽度", "AT9", "AT9")]
        AT9宽度,//AT9
        [Language("极耳台阶高", "EarStepHeight", "EarStepHeight")]
        极耳台阶高,//EarStepHeight
        [Language("极耳上升沿角度", "EarRisingEdgeAngle", "EarRisingEdgeAngle")]
        极耳上升沿角度,//EarRisingEdgeAngle
        [Language("极耳下降沿角度", "EarFallingEdgeAngle", "EarFallingEdgeAngle")]
        极耳下降沿角度,//EarFallingEdgeAngle
        [Language("顶R1半径", "TopR1Radius", "TopR1Radius")]
        顶R1半径,//TopR1Radius
        [Language("底R2半径", "BottomR2Radius", "BottomR2Radius")]
        底R2半径,//BottomR2Radius
        [Language("上V角宽度", "UpVAngleWidth", "UpVAngleWidth")]
        上V角宽度,//UpVAngleWidth
        [Language("上V角深度", "UpVAngleDepth", "UpVAngleDepth")]
        上V角深度,//UpVAngleDepth
        [Language("下V角宽度", "DownVAngleWidth", "DownVAngleWidth")]
        下V角宽度,//DownVAngleWidth
        [Language("下V角深度", "DownVAngleDepth", "DownVAngleDepth")]
        下V角深度,//DownVAngleDepth
        [Language("涂层尺寸X", "CoatingSizeX", "CoatingSizeX")]
        涂层尺寸X,//CoatingSizeX
        [Language("分切宽度", "SlittingWidth", "largeurDeRefente")]
        分切宽度,
    }
    public enum Defct1Enum
    {
        [AnotherNamer("ZBPS")]
        [Language("直边破损", "Electrode Damage", "DommageAuBordDroit")]
        直边破损,//StraightEdgeBreakage
        [AnotherNamer("ZBDL")]
        [Language("边缘漏金属", "Edge Exposed Metal", "Métal exposé au bord")]
        直边掉料,//StraightEdgeDrop
        [AnotherNamer("ZBYL")]
        [Language("直边余料", "Irregular Scrap Drag-Out", "RésidusDuBordDroit")]
        直边余料,//StraightEdgeResidualMaterial
        [AnotherNamer("ATLJS")]
        [Language("AT漏金属", "Ceramic Area Foil Exposed", "ATManquantDeMétal")]
        AT漏金属,
        [AnotherNamer("ATPS")]
        [Language("AT破损", "Ceramic Damage", "DommageAT")]
        AT破损,
        [AnotherNamer("JD")]
        [Language("接带", "Tape", "JonctionDeBande")]
        接带,//TapeSplicing
        [AnotherNamer("LJS")]
        [Language("漏金属", "Coating Area Foil Exposed", "MétalManquant")]
        漏金属,//MetalLeakage
        [AnotherNamer("ZZ")]
        [Language("褶皱", "Wrinkle", "Ride")]
        褶皱,//Wrinkle
        [AnotherNamer("QP")]
        [Language("气泡", "Bubble", "Bulle")]
        气泡,//Bubble
        [AnotherNamer("HH")]
        [Language("划痕", "Scratch", "Égratignure")]
        划痕,//Scratch
        [AnotherNamer("YH")]
        [Language("压痕", "Surface Dent", "Enfoncement")]
        压痕,//Indentation
        [AnotherNamer("JPPS")]
        [Language("极片破损", "Material Damage", "DommageÀLÉlectrod")]
        极片破损,//PolePlateDamaged
        [AnotherNamer("BMLW")]
        [Language("表面裂纹", "Surface Crack", "FissureDeSurface")]
        表面裂纹,//SurfaceCrack
        [AnotherNamer("HD")]
        [Language("黑点", "Over Coating (Coating Non-Uniformity)", "PointNoir")]
        黑点,//DarkSpot
        [AnotherNamer("BD")]
        [Language("白点", "Under Coating（Coating Non-Uniformity）", "PointBlanc")]
        白点,//WhiteSpot
        [AnotherNamer("QT")]
        [Language("其它", "Others", "Autre")]
        其它,//Else
        [AnotherNamer("WLY")]
        [Language("未冷压", "Unpressed", "NonPresséÀFroid")]
        未冷压, //NoChillPressing
        [AnotherNamer("JEDL")]
        [Language("极耳带料", "Oversized Tab/Tab Damage", "MatièreSurLOreilleDeLÉlectrode")]
        极耳带料,
        [AnotherNamer("JEPS")]
        [Language("极耳破损", "Undersized Tab/Tab Damage", "DommageÀLOreilleDeLÉlectrode")]
        极耳破损,
        [AnotherNamer("AK")]
        [Language("凹坑", "Pit", "Pockmarks")]
        凹坑,
        [AnotherNamer("JPDL")]
        [Language("极片带料", "Material Tab Damage", "Dégât d'étiquette matérielle")]
        极片带料,
        [AnotherNamer("DMJQX")]
        [Language("大面积缺陷", "Large area defects", "Défauts à grande échelle")]
        大面积缺陷,
        [AnotherNamer("DMJJD")]
        [Language("大面积接带", "Large Area Tape", "Jonction De Bande à Grande Échelle")]
        大面积接带,
        [AnotherNamer("DMJLJS")]
        [Language("大面积漏金属", "Large Area Coating Area Foil Exposed", "Métal Manquant à Grande Échelle")]
        大面积漏金属,
        [AnotherNamer("AB")]
        [Language("暗斑", "Dark Spot‌", "Tache sombre")]
        暗斑,
        [AnotherNamer("AH")]
        [Language("暗痕", "Dark Streak", "Strie sombre‌")]
        暗痕,
        [AnotherNamer("TJPS")]
        [Language("台阶破损", "Step Damage", "Marche endommagée‌")]
        台阶破损,
        直边毛刺,
        极耳毛刺,
        端面A类毛刺,
        端面B类毛刺,
        大面A类毛刺,
        //胶带,

    }

    public enum ElectrodeEnum
    {
        阴极,
        阳极
    }


    public enum FactoryEnum
    {
        北京纯锂,
        嘉善兰钧
    }
    public enum ImgType
    {
        BigImage,
        SmallImage,
        ProductImage,
        CropImage,
    }

    public enum DefctEnum
    {
        残留,
        针孔,
        涂布缺陷

    }
    public enum CropImageEnum
    {
        头部图,
        侧基准图,
        槽位图
    }


    public enum UserEnum
    {
        操作员,
        技术师,
        工程师,
        超级管理员
    }

    /// <summary>
    /// 图片保存格式
    /// </summary>
    public enum ImageSaveFormat
    {
        BMP,
        JPEG20,
        JPEG40,
        TIF,
        TIFCompress
    }


    /// <summary>
    /// 图片保存选择
    /// </summary>
    public enum ImageSaveSelect
    {
        [Language("保存所有", "Save All", "Sauvegarder tout")]
        保存所有,
        [Language("保存OK", "Save OK", "Sauvegarder OK")]
        保存OK,
        [Language("保存NG", "Save NG", "Sauvegarder NG")]
        保存NG,
        [Language("不保存", "Do Not Save", "Sauvegarder NG")]
        不保存,
    }

    public enum LanguageEnum
    {
        Chinese,
        English,
        France
    }


    /// <summary>
    /// 深度图片保存格式
    /// </summary>
    public enum ImageDeepthSaveFormat
    {
        TIF,
        TIF_DAT,
        DAT,
    }


    public enum ModelName
    {
        [Language("保存设置", "Save Settings", "Sauvegarder les paramètres")]
        保存设置,
        [Language("仿真设置", "Simulation Settings", "Paramètres de simulation")]
        仿真设置,
        [Language("删图设置", "Delete Image Settings", "Paramètres de suppression d'image")]
        删图设置,
        [Language("调试设置", "Debug Settings", "Paramètres de débogage")]
        调试设置,
        [Language("工艺设置", "Process Settings", "Paramètres de processus")]
        工艺设置,
        [Language("相机设置", "Camera Settings", "Paramètres de processus")]
        相机设置,
        [Language("光源设置", "Light Source Settings", "Paramètres de source lumineuse")]
        光源设置,
        [Language("主设备通讯", "Main Equipment Communication", " Communication avec l'équipement principal")]
        主设备通讯,
        [Language("PC通讯设置", "PC Communication Settings", "Paramètres de communication PC")]
        PC通讯设置,
        [Language("打标机软件通讯", "Marking Machine Software Communication", "Communication logicielle de la machine de marquage")]
        打标机软件通讯,
        [Language("相机标定", "Camera Calibration", "Calibration de la caméra")]
        相机标定,
        //[Language("尺寸点检", "Dimension Inspection", "Inspection des dimensions")]
        //尺寸点检,
        [Language("点检", "Inspection", "Inspection")]
        点检,
        [Language("MES", "MES", "MES")]
        MES,
        [Language("控制卡", "ControlCard", "ControlCard")]
        控制卡,
        [Language("纠偏器:", "CorrectDevice:", "CorrectDevice:")]
        纠偏器,
        [Language("Mark设置", "Mark Set", "Mark Set")]
        Mark设置,
        [Language("MSA圆点设置", "MSA圆点设置", "MSA圆点设置")]
        MSA圆点设置,
        [Language("MSA线宽设置", "MSA线宽设置", "MSA线宽设置")]
        MSA棋盘格线宽设置,
        [Language("MSA测厚3D设置", "MSA测厚3D设置", "MSA测厚3D设置")]
        MSA测厚3D设置,
        [Language("MSA棋盘格线宽设置", "MSA棋盘格线宽设置", "MSA棋盘格线宽设置")]
        MSA线宽设置,
        [Language("磁盘设置", "磁盘设置", "磁盘设置")]
        磁盘设置,
        [Language("系统设置", "系统设置", "系统设置")]
        系统设置,
        [Language("主副机通讯", "主副机通讯", "主副机通讯")]
        主副机通讯,
        [Language("灰度自适应", "Grayscale Adaptation", "Adaptation des Niveaux de Gris")]
        灰度自适应,
    }
    /// <summary>
    /// 抓边枚举，明暗方向：
    /// </summary>
    public enum MeasureTransition
    {
        [AnotherNamer("negative")]
        [Language("从明到暗", "negative", "negative")]
        从明到暗,
        [AnotherNamer("positive")]
        [Language("从暗到明", "positive", "positive")]
        从暗到明,
        [AnotherNamer("all")]
        [Language("不区分明暗", "all", "all")]
        不区分明暗,
    }
    /// <summary>
    /// 抓边枚举，边缘选取：
    /// </summary>
    public enum MeasureLine
    {
        [AnotherNamer("first")]
        [Language("第一条边", "first", "first")]
        第一条边,
        [AnotherNamer("last")]
        [Language("最后一条边", "last", "last")]
        最后一条边,
        [AnotherNamer("all")]
        [Language("最强边", "all", "all")]
        最强边,
    }

    /// <summary>
    /// 极值枚举，极值方向选择：
    /// </summary>
    public enum PeakValleymum
    {
        [Language("无选择", "None", "None")]
        无选择,
        [Language("峰值", "Peak", "Peak")]
        峰值,
        [Language("谷值", "Valley", "Valley")]
        谷值,
        [Language("剔值平均", "RejectionAverage", "RejectionAverage")]
        剔值平均,
    }

    /// <summary>
    /// 抓边枚举，外侧点验证：
    /// </summary>
    public enum MeasureIORegions
    {
        [AnotherNamer("true")]
        [Language("是", "true", "true")]
        是,
        [AnotherNamer("false")]
        [Language("否", "false", "false")]
        否,
    }
    /// <summary>
    /// 抓边枚举，插值类型：
    /// </summary>
    public enum MeasureInterpolation
    {
        [AnotherNamer("nearest_neighbor")]
        [Language("最近邻域法", "nearest_neighbor", "nearest_neighbor")]
        最近邻域法,
        [AnotherNamer("bilinear")]
        [Language("双线性插值法", "bilinear", "bilinear")]
        双线性插值法,
        [AnotherNamer("bicubic")]
        [Language("双三次插值法", "bicubic", "bicubic")]
        双三次插值法,
    }


    public enum MeasureDirection
    {
        从左到右,
        从右到左,
        从上到下,
        从下到上
    }

    public enum StatusStripEnum
    {
        [Language("当前用户类型:", "CurrentUserType:", "CurrentUserType:")]
        当前用户类型,
        [Language("当前用户ID:", "CurrentTypeID:", "CurrentTypeID:")]
        当前用户ID,
        [Language("磁盘剩余:", "DiskSurplus:", "DiskSurplus:")]
        磁盘剩余,
        [Language("主设备PLC", "MainDevPLC", "MainDevPLC")]
        主设备PLC,
        [Language("纠偏器", "CorrectDevice", "CorrectDevice")]
        纠偏器,
        [Language("运动控制卡", "MotionControlCard", "MotionControlCard")]
        运动控制卡,
        [Language("IO控制卡", "IOControlCard", "IOControlCard")]
        IO控制卡,
    }

    public enum UserType
    {
        [Language("操作员", "Operator", "opérateur")]
        Operator, //操作员
        [Language("技术员", "Technician", "technicienne")]
        Technician,  //技术员
        [Language("工程师", "Engineer", "ingénieur")]
        Engineer,  //工程师
        [Language("管理员", "Admin", "administrateur")]
        Admin, //管理员
        [Language("超级管理员", "Supervisor", "Le super administrateur")]
        Supervisor,   //超级管理员
        [Language("默认权限", "Default", "Par défaut")]
        Null
    }

    public enum PLCTypeEnum
    {
        Omron,//欧姆龙
        Siemens1200,//西门子1200系列
        Siemens1500,//西门子1500系列
        Inovance,//汇川
    }
    public enum CommunicationProtocolEnum
    {
        Fins,
        CIP,
        S7,
        ModbusTCP,
        TCP,
        UDP,
    }

    public enum StopAlarmStateEnum
    {
        [Language("正常", "Normal", "Normal")]
        正常,
        [Language("停机", "Shutdown", "Arrêt")]
        停机,
        [Language("报警", "Alarm", "Alarme")]
        报警,
    }
    public enum PointInspectionStateEnum
    {
        [Language("已忽略", "Ignored", "Ignoré")]
        已忽略,
        [Language("未点检", "Not Inspection", "Non inspecté")]
        未点检,
        [Language("点检中", "Inspecting", "En cours d'inspection")]
        点检中,
        [Language("点检成功", "Point check successful", "Vérification ponctuelle réussie")]
        点检成功,
        [Language("点检失败", "Point check failed", "Vérification ponctuelle échouée")]
        点检失败,
    }
    public enum RotationAngleEnum
    {
        零度 = 0,
        九十度 = 90,
        负九十度 = -90,
        一百八十度 = 180,
        负一百八十度 = -180
    }
    public enum SortEnum
    {
        [Language("X_LToR", "X_LToR ", "X_LToR")]
        X,//从左到右
        [Language("Y_UToD", "Y_UToD ", "Y_UToD")]
        Y//从上到下
    }

    public enum SizePointInspectionArithmeticEnum
    {
        [Language("矩形菲林片", "矩形菲林片", "矩形菲林片")]
        矩形菲林片,
        [Language("线宽菲林片", "线宽菲林片", "线宽菲林片")]
        线宽菲林片,

    }

    public enum SlottypeEnum
    {
        slot,
        shave,
    }
    /// <summary>
    /// 左边,右边,上边,下边
    /// </summary>
    public enum DirectionEnum
    {
        左边,
        右边,
        上边,
        下边,
        左间隙边,
        右间隙边
    }

    /// <summary>
    /// TD定位距,MD定位距,TD尺寸,MD尺寸
    /// </summary>
    public enum MeasureSlotEnum
    {
        TD定位距,
        MD定位距,
        TD尺寸,
        MD尺寸,
        TD间距,
        MD间距,

        TD左间隙,
        TD右间隙,

        TD对齐,
        MD对齐,
        TD错位,
        MD错位,
      

    }
}
