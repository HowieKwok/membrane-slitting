using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Extensions;
using HalconDotNet;
using YYSD.UICore.Control;
namespace Frame_Core
{
    [Serializable]
    public class ParamsMQDefects : PluginParamsBase
    {
        /// <summary>
        ///是否启用功能选择
        /// </summary>
        [NonSerialized]
        [Description("是否启用功能选择")]
        public bool ISEnabledFunctionSelection =false;


        /// <summary>
        /// 输入变量：原图
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_Image;


        /// <summary>
        /// 输入变量：搜索区域
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject DetectionROI;

        /// <summary>
        /// 检测区域左顶点X
        /// </summary>
        [ShowTextAttribute("检测区域左顶点X")]
        [Description("检测区域左顶点X")]

        public int Detection_Left_UP_X { get; set; } = 2000;



        /// <summary>
        /// 检测区域左顶点Y
        /// </summary>
        [ShowTextAttribute("检测区域左顶点Y")]
        [Description("检测区域左顶点Y")]

        public int Detection_Left_UP_Y { get; set; } = 100;


        /// <summary>
        /// 检测区域右底点X
        /// </summary>
        [ShowTextAttribute("检测区域右底点X")]
        [Description("检测区域右底点X")]

        public int Detection_Right_Down_X { get; set; } = 6000;



        /// <summary>
        /// 检测区域左底点Y
        /// </summary>
        [ShowTextAttribute("检测区域右底点Y")]
        [Description("检测区域右底点Y")]

        public int Detection_Right_Down_Y { get; set; } = 2000;


        /// <summary>
        /// 启动仿真区域
        /// </summary>
        [ShowTextAttribute("启动仿真区域")]
        [Description("启动仿真区域")]
        public bool cb_Enable_SimulationRegion { get; set; } = false;
        /// <summary>
        /// 输入变量：屏蔽区域
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ShieldROI;

        /// <summary>
        /// 横向标定
        /// </summary>
        [NonSerialized]
        public double WidthCali = 0.091496;
        /// <summary>
        /// 纵向标定
        /// </summary>
        [NonSerialized]
        public double HeightCali = 0.15072;






        /// <summary>
        /// 检测方法 0 膜区检测 1直边破损 2直边掉料  3直边余料 4AT检测 5极耳背光
        /// </summary>
        [ShowTextAttribute("检测方法 0 膜区检测 1直边破损 2直边掉料  3直边余料 4AT检测 5极耳背光")]
        [Description("检测方法 0 膜区检测 1直边破损 2直边掉料  3直边余料 4AT检测 5极耳背光")]
        public int com_DetectionMethod { get; set; } = 0;


        /// <summary>
        /// 极性选择 0表示阴极/正极 1表示阳极/负极
        /// </summary>
        [ShowTextAttribute("极性选择 0表示阴极/正极 1表示阳极/负极")]
        [Description("极性选择 0表示阴极/正极 1表示阳极/负极")]
        public int com_Cathode { get; set; } = 0;


        /// <summary>
        /// 启动分类器
        /// </summary>
        [ShowTextAttribute("启动分类器")]
        [Description("启动分类器")]
        public bool cb_Enable_Classify { get; set; } = false;


        /// <summary>
        /// 亮瑕疵最小阈值
        /// </summary>
        [ShowTextAttribute("亮瑕疵最小阈值")]
        [ControlIdAttribute(49)]
        [Description("亮瑕疵最小阈值")]
        public int num_WhiteThreshold_Min { set; get; } = 125;

        /// <summary>
        /// 亮瑕疵最大阈值
        /// </summary>
        [ShowTextAttribute("亮瑕疵最大阈值")]
        [Description("亮瑕疵最小阈值")]
        public int num_WhiteThreshold_Max { set; get; } = 255;



        /// <summary>
        /// 暗瑕疵最小阈值
        /// </summary>
        [ShowTextAttribute("暗瑕疵最小阈值")]
        [Description("暗瑕疵最小阈值")]
        public int num_BlackThreshold_Min { set; get; } = 0;

        /// <summary>
        /// 暗瑕疵最大阈值
        /// </summary>
        [ShowTextAttribute("暗瑕疵最大阈值")]
        [Description("暗瑕疵最大阈值")]
        public int num_BlackThreshold_Max { set; get; } = 25;



        /// <summary>
        /// 亮瑕疵过滤最小面积
        /// </summary>
        [ShowTextAttribute("亮瑕疵过滤最小面积")]
        [Description("亮瑕疵过滤最小面积")]
        public double num_MinArea_White { set; get; } = 1;

        /// <summary>
        /// 暗瑕疵过滤最小面积
        /// </summary>
        [ShowTextAttribute("暗瑕疵过滤最小面积")]
        [Description("暗瑕疵过滤最小面积")]
        public double num_MinArea_Black { set; get; } = 1;




        /// <summary>
        /// 未冷压灰度差异
        /// </summary>
        [ShowTextAttribute("未冷压灰度差异")]
        [Description("未冷压灰度差异")]
        public int num_WLY_GrayDifference { set; get; } = 10;



        /// <summary>
        /// 未冷压灰度低于
        /// </summary>
        [ShowTextAttribute("未冷压灰度低于")]

        [Description("未冷压灰度低于")]
        public int num_WLY_GrayUnder { set; get; } = 10;


        /// <summary>
        /// 是否启用未冷压
        /// </summary>
        [ShowTextAttribute("是否启用未冷压")]
        [Description("是否启用未冷压")]
        public bool cb_OpenDec_WLY { set; get; } = false;


        /// <summary>
        /// 缺陷进入分类器最大数量
        /// </summary>
        [ShowTextAttribute("缺陷进入分类器最大数量")]

        [Description("缺陷进入分类器最大数量")]
        public int num_NgMaxNum { set; get; } = 3;



        /// <summary>
        /// 总面积超过多少,进入接带漏金属判定
        /// </summary>
        [ShowTextAttribute("总面积超过多少,进入接带漏金属判定")]

        [Description("总面积超过多少,进入接带漏金属判定")]
        public double num_JDAreaNG { set; get; } = 100;



        /// <summary>
        /// 区域平均灰度大于亮瑕疵最小阈值的差值,进入接带漏金属判定
        /// </summary>
        [ShowTextAttribute("区域平均灰度大于亮瑕疵最小阈值的差值,进入接带漏金属判定")]

        [Description("区域平均灰度大于亮瑕疵最小阈值的差值,进入接带漏金属判定")]
        public int num_GrayDifference { set; get; } = 20;

      

        /// <summary>
        /// 接带宽度最小值
        /// </summary>
        [ShowTextAttribute("接带宽度最小值")]

        [Description("接带宽度最小值")]
        public double num_JDMinW { set; get; } = 20.0;

        /// <summary>
        /// 接带高度最大值
        /// </summary>
        [ShowTextAttribute("接带高度最大值")]

        [Description("接带高度最大值")]
        public double num_JDMaxH { set; get; } = 20.0;

        /// <summary>
        /// 边缘破损阈值
        /// </summary>
        [ShowTextAttribute("边缘破损阈值")]
        [Description("边缘破损阈值")]
        public int num_ZBPS_Threshold { set; get; } = 20;



        /// <summary>
        /// 边缘破损过滤最小面积
        /// </summary>
        [ShowTextAttribute("边缘破损过滤最小面积")]
        [Description("边缘破损过滤最小面积")]
        public double num_ZBPS_MinArea { set; get; } = 0.5;


        /// <summary>
        /// 边缘掉料阈值
        /// </summary>
        [ShowTextAttribute("边缘掉料阈值")]
        [Description("边缘掉料阈值")]
        public int num_BYDL_Threshold { set; get; } = 180;



        /// <summary>
        /// 边缘掉料过滤最小面积
        /// </summary>
        [ShowTextAttribute("边缘掉料过滤最小面积")]
        [Description("边缘掉料过滤最小面积")]
        public double num_BYDL_MinArea { set; get; } = 0.5;




        /// <summary>
        /// 直边余料最小阈值
        /// </summary>
        [ShowTextAttribute("直边余料最小阈值")]
        [Description("直边余料最小阈值")]
        public int num_ZBYL_Threshold_Min { set; get; } = 30;


        /// <summary>
        /// 直边余料最大阈值
        /// </summary>
        [ShowTextAttribute("直边余料最大阈值")]
        [Description("直边余料最大阈值")]
        public int num_ZBYL_Threshold_Max { set; get; } = 150;

        /// <summary>
        /// 直边余料过滤最小面积
        /// </summary>
        [ShowTextAttribute("直边余料过滤最小面积")]
        [Description("直边余料过滤最小面积")]
        public double num_ZBYL_MinArea { set; get; } = 0.5;

        /// <summary>
        /// 插件运行超时时间ms
        /// </summary>
        [ShowTextAttribute("插件运行超时时间ms")]
        [Description("插件运行超时时间ms")]
        public int num_timeOutPeriod { set; get; } = 500;


        /// <summary>
        /// 模板匹配超时时间(ms)
        /// </summary>
        [ShowTextAttribute("模板匹配超时时间(ms)")]
        [Description("模板匹配超时时间(ms)")]
        public int num_TimeOut { set; get; } = 30;

        /// <summary>
        /// AT漏金属最小阈值
        /// </summary>
        [ShowTextAttribute("AT漏金属最小阈值")]
        [Description("AT漏金属最小阈值")]
        public int num_AT_LouJinShu_Threshold_Min { get; set; } = 200;

        /// <summary>
        /// AT漏金属最大阈值
        /// </summary>
        [ShowTextAttribute("AT漏金属最大阈值")]
        [Description("AT漏金属最大阈值")]
        public int num_AT_LouJinShu_Threshold_Max { get; set; } = 255;
        /// <summary>
        /// AT漏金属过滤最小面积
        /// </summary>
        [ShowTextAttribute("AT漏金属过滤最小面积")]
        [Description("AT漏金属过滤最小面积")]
        public double num_AT_LouJinShu_MinArea { get; set; } = 0.5;



        /// <summary>
        /// AT破损最小阈值
        /// </summary>
        [ShowTextAttribute("AT破损最小阈值")]
        [Description("AT破损最小阈值")]
        public int num_AT_PoSun_Threshold_Min { get; set; } = 0;

        /// <summary>
        /// AT破损最大阈值
        /// </summary>
        [ShowTextAttribute("AT破损最大阈值")]
        [Description("AT破损最大阈值")]
        public int num_AT_PoSun_Threshold_Max { get; set; } = 30;
        /// <summary>
        /// AT破损过滤最小面积
        /// </summary>
        [ShowTextAttribute("AT破损过滤最小面积")]
        [Description("AT破损过滤最小面积")]
        public double num_AT_PoSun_MinArea { get; set; } = 0.5;



        /// <summary>
        /// 极耳带料最小阈值
        /// </summary>
        [ShowTextAttribute("极耳带料最小阈值")]
        [Description("极耳带料最小阈值")]
        public int num_JR_DaiLiao_Threshold_Min { get; set; } = 0;

        /// <summary>
        /// 极耳带料最大阈值
        /// </summary>
        [ShowTextAttribute("极耳带料最大阈值")]
        [Description("极耳带料最大阈值")]
        public int num_JR_DaiLiao_Threshold_Max { get; set; } = 150;
        /// <summary>
        /// 极耳带料过滤最小面积
        /// </summary>
        [ShowTextAttribute("极耳带料过滤最小面积")]
        [Description("极耳带料过滤最小面积")]
        public double num_JR_DaiLiao_MinArea { get; set; } = 0.5;

        /// <summary>
        /// 极耳带料检测 模板轮廓向左平移1
        /// </summary>
        [ShowTextAttribute("极耳带料检测 模板轮廓向右平移1")]
        [Description("极耳带料检测 模板轮廓向右平移1")]
        public double num_JR_DaiLiao_ModelXld_ShiftRight_1 { get; set; } = 1;

        /// <summary>
        /// 极耳带料检测 模板轮廓向左平移2
        /// </summary>
        [ShowTextAttribute("极耳带料检测 模板轮廓向右平移2")]
        [Description("极耳带料检测 模板轮廓向右平移2")]
        public double num_JR_DaiLiao_ModelXld_ShiftRight_2 { get; set; } = 10;


        /// <summary>
        /// 是否显示极耳带料检测区域
        /// </summary>
        [ShowTextAttribute("显示检测区域")]
        [Description("是否显示极耳带料检测区域")]
        public bool cb_Show_JR_DaiLiao_DetectionROI { set; get; } = true;

        /// <summary>
        /// 是否显示模板轮廓
        /// </summary>
        [ShowTextAttribute("显示模板轮廓")]
        [Description("是否显示模板轮廓")]
        public bool cb_Show_JR_ModelXld { set; get; } = true;


        /// <summary>
        /// 极耳破损最小阈值
        /// </summary>
        [ShowTextAttribute("极耳破损最小阈值")]
        [Description("极耳破损最小阈值")]
        public int num_JR_PoSun_Threshold_Min { get; set; } = 150;

        /// <summary>
        /// 极耳破损最大阈值
        /// </summary>
        [ShowTextAttribute("极耳破损最大阈值")]
        [Description("极耳破损最大阈值")]
        public int num_JR_PoSun_Threshold_Max { get; set; } = 255;
        /// <summary>
        /// 极耳破损过滤最小面积
        /// </summary>
        [ShowTextAttribute("极耳破损过滤最小面积")]
        [Description("极耳破损过滤最小面积")]
        public double num_JR_PoSun_MinArea { get; set; } = 0.5;




        /// <summary>
        /// 极耳破损检测 模板轮廓向左平移1
        /// </summary>
        [ShowTextAttribute("极耳破损检测 模板轮廓向左平移1")]
        [Description("极耳破损检测 模板轮廓向左平移1")]
        public double num_JR_PoSun_ModelXld_ShiftLeft_1 { get; set; } = 2;


        /// <summary>
        /// 是否显示极耳破损检测区域
        /// </summary>
        [ShowTextAttribute("显示检测区域")]
        [Description("是否显示极耳破损检测区域")]
        public bool cb_Show_JR_PoSun_DetectionROI { set; get; } = true;



        /// <summary>
        /// 输入图片压缩Width系数//输入变量
        /// </summary>
        [ShowTextAttribute("输入图片压缩Width系数")]
        [Description("输入图片压缩Width系数")]
        public double m_scaleWidth  = 1;

        /// <summary>
        /// 输入图片压缩Height系数//输入变量
        /// </summary>
        [ShowTextAttribute("输入图片压缩Height系数")]
        [Description("输入图片压缩Height系数")]
        public double m_scaleHeight = 1;
        /// <summary>
        /// 模板Region中心Row//输入变量
        /// </summary>
        [ShowTextAttribute("模板Region中心Row")]
        [Description("模板Region中心Row")]
        public double m_fCenterRow  = 100;

        /// <summary>
        /// 模板Region中心Col//输入变量
        /// </summary>
        [ShowTextAttribute("模板Region中心Col")]
        [Description("模板Region中心Col")]
        public double m_fCenterCol = 100;

        /// <summary>
        /// 模板Region中心Length_X`//输入变量
        /// </summary>
        [ShowTextAttribute("模板Region中心Length_X")]
        [Description("模板Region中心Length_X")]
        public double m_fCenterLength_X  = 50;

        /// <summary>
        /// 模板Region中心Length_Y`//输入变量
        /// </summary>
        [ShowTextAttribute("模板Region中心Length_Y")]
        [Description("模板Region中心Length_Y")]
        public double m_fCenterLength_Y  = 50;

        /// <summary>
        /// 模板Region中心Col//输入变量
        /// </summary>
        [ShowTextAttribute("模板Region中心Angle")]
        [Description("模板Region中心Angle")]
        public double m_fCenterAngle  = 0;





        /// <summary>
        /// 模板
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HTuple Model;



        /// <summary>
        /// 模板XLD 为了减少耗时,读取模板时赋值
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ModelContours;




        /// <summary>
        /// 输出变量：Ng区域
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject NgRegion;

        /// <summary>
        /// 输出变量：Ng类型
        /// </summary>
        [NonSerialized]

        public string NgString;



        /// <summary>
        /// 输出的缺陷队列
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public Queue<NgTypeInfo> detectInfos = new Queue<NgTypeInfo>();


        /// <summary>
        /// 输出变量：处理后的图像
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_ImageProcessed;


        /// <summary>
        /// 输出变量：匹配结果
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ModleXld;



        /// <summary>
        /// 输出变量：极耳带料检测区域
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject JRDaiLiaoXldRegion;


        /// <summary>
        /// 输出变量：极耳破损检测区域
        /// </summary>
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject JRPoSunRegion;


        /// <summary>
        /// 输入:极耳中心Y
        /// </summary>
        [ShowTextAttribute("极耳中心Y")]
        [Description("极耳中心Y")]
        public int num_JRCenter_Row { set; get; } = 1000;


        /// <summary>
        /// 输入:极耳中心X
        /// </summary>
        [ShowTextAttribute("极耳中心X")]
        [Description("极耳中心X")]
        public int num_JRCenter_column { set; get; } = 1000;


        /// <summary>
        /// 极耳宽度(mm)
        /// </summary>
        [ShowTextAttribute("极耳宽度(mm)")]
        [Description("极耳宽度(mm)")]
        public double num_JR_width { set; get; } = 30;


        /// <summary>
        /// 底部R角半径(mm)
        /// </summary>
        [ShowTextAttribute("底部R角半径(mm)")]
        [Description("底部R角半径(mm)")]
        public double num_bottomAngleR_ridus { set; get; } = 6;

        /// <summary>
        /// 底部R角角度
        /// </summary>
        [ShowTextAttribute("底部R角角度")]
        [Description("底部R角角度")]
        public double num_bottomAngleR_CompensationAngle { set; get; } = 90;

        /// <summary>
        /// 顶部R角半径(mm)
        /// </summary>
        [ShowTextAttribute("顶部R角半径(mm)")]
        [Description("顶部R角半径(mm)")]
        public double num_topAngleR_ridus { set; get; } = 1;

        /// <summary>
        /// 顶部R角角度
        /// </summary>
        [ShowTextAttribute("顶部R角角度")]
        [Description("顶部R角角度")]
        public double num_topAngleR_CompensationAngle { set; get; } = 90;



        /// <summary>
        /// 斜边角度
        /// </summary>
        [ShowTextAttribute("斜边角度")]
        [Description("斜边角度")]
        public double num_hypotenuse_angle { set; get; } = 90;

        /// <summary>
        /// 斜边长度(mm)
        /// </summary>
        [ShowTextAttribute("斜边长度(mm)")]
        [Description("斜边长度(mm)")]
        public double num_hypotenuse_length { set; get; } = 45;


        /// <summary>
        /// 极耳余料-极耳底部屏蔽
        /// </summary>
        [ShowTextAttribute("极耳余料-极耳底部屏蔽")]
        [Description("极耳余料-极耳底部屏蔽")]
        public double num_bottomAngleR_Shielded_Y { set; get; } = 0;


        /// <summary>
        /// 极耳余料-极耳中部屏蔽
        /// </summary>
        [ShowTextAttribute("极耳余料-极耳中部屏蔽")]
        [Description("极耳余料-极耳中部屏蔽")]
        public double num_hypotenuse_Shielded_Y { set; get; } = 0;

        /// <summary>
        /// 极耳余料-极耳顶部屏蔽
        /// </summary>
        [ShowTextAttribute("极耳余料-极耳顶部屏蔽")]
        [Description("极耳余料-极耳顶部屏蔽")]
        public double num_topAngleR_Shielded_Y { set; get; } = 0;


        /// <summary>
        /// 极耳余料-直边屏蔽
        /// </summary>
        [ShowTextAttribute("极耳余料-直边屏蔽")]
        [Description("极耳余料-直边屏蔽")]
        public double num_bottomAngleR_Shielded_X { set; get; } = 0;

        /// <summary>
        /// 极耳余料-顶边屏蔽
        /// </summary>
        [ShowTextAttribute("极耳余料-顶边屏蔽")]
        [Description("极耳余料-顶边屏蔽")]
        public double num_topAngleR_Shielded_X { set; get; } = 0;




        /// <summary>
        /// 极耳破损-极耳底部屏蔽
        /// </summary>
        [ShowTextAttribute("极耳破损-极耳底部屏蔽")]
        [Description("极耳破损-极耳底部屏蔽")]
        public double num_bottomAngleR_Shielded_Y1 { set; get; } = 0;

        /// <summary>
        /// 极耳破损-极耳中部屏蔽
        /// </summary>
        [ShowTextAttribute("极耳破损-极耳中部屏蔽")]
        [Description("极耳破损-极耳中部屏蔽")]
        public double num_hypotenuse_Shielded_Y1 { set; get; } = 0;

        /// <summary>
        /// 极耳破损-极耳顶部屏蔽
        /// </summary>
        [ShowTextAttribute("极耳破损-极耳顶部屏蔽")]
        [Description("极耳破损-极耳顶部屏蔽")]
        public double num_topAngleR_Shielded_Y1 { set; get; } = 0;


        /// <summary>
        /// 极耳破损-直边屏蔽
        /// </summary>
        [ShowTextAttribute("极耳破损-直边屏蔽")]
        [Description("极耳破损-直边屏蔽")]
        public double num_bottomAngleR_Shielded_X1 { set; get; } = 0;

        /// <summary>
        /// 极耳破损-顶边屏蔽
        /// </summary>
        [ShowTextAttribute("极耳破损-顶边屏蔽")]
        [Description("极耳破损-顶边屏蔽")]
        public double num_topAngleR_Shielded_X1 { set; get; } = 0;


        public ParamsMQDefects()
        {
            HOperatorSet.GenEmptyObj(out NgRegion);
            HOperatorSet.GenEmptyObj(out DetectionROI);
            HOperatorSet.GenEmptyObj(out ShieldROI);
            HOperatorSet.GenEmptyObj(out ho_ImageProcessed);
            HOperatorSet.GenEmptyObj(out ModleXld);
            HOperatorSet.GenEmptyObj(out JRDaiLiaoXldRegion);
            HOperatorSet.GenEmptyObj(out JRPoSunRegion);
            HOperatorSet.GenEmptyObj(out ModelContours);
        }









    }
}
