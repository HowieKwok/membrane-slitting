using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    /// <summary>
    /// 尺寸检测项
    /// </summary>
    [Serializable]
    public class SizeItemInfo
    {
        [My("编号")]
        [Description("编号")]
        public int Index { get; set; } = 0;//名称
        [My("启用")]
        [Description("启用")]
        public bool Enable { get; set; } = true;//打标
        [My("名称")]
        [Description("名称")]
        public string Name { get; set; } = "名称";//名称   

        [My("备注")]
        [Description("备注")]
        public string Description { get; set; } = "备注";

        [My("所属相机")]
        [Description("所属相机")]
        public CameraEnum camera { get; set; } = 0;


        [My("打孔编号1")]
        [Description("打孔编号1")]
        public int ShaveIndex1 { get; set; } = 0;//名称
        [My("当前相机")]
        [Description("当前相机")]
        public string camera1 { get; set; } = ((CameraEnum)0).ToString();

        [My("抓边1")]
        [Description("抓边1")]
        public string Line1 { get; set; } = "默认抓边1";

        [My("打孔编号2")]
        [Description("打孔编号2")]
        public int ShaveIndex2 { get; set; } = 0;//名称
        [My("目标相机")]
        [Description("目标相机")]
        public string camera2 { get; set; } = ((CameraEnum)0).ToString();

        [My("抓边2")]
        [Description("抓边2")]
        public string Line2 { get; set; } = "默认抓边2";

        [My("线线计算类型")]
        [Description("线线计算类型")]
        public string EditorType { get; set; } = EditorTypeEnum.线距离X.ToString();//线线计算类型 EditorTypeEnum转int
         
        //--------------边缘选择-------------------------------------

        //--------------标准值--------------------------------------
        [My("标准值")]
        [Description("标准值")]
        public double StDValue { get; set; } = 10;//标准值

        //[My("实际测量值", false)]
        //[Description("实际测量值")]
        //public double StD_Real { get; set; } = 0;//实际测量值
        [My("校准系数")]
        [Description("校准系数")]
        public double StD_RealFixRito { get; set; } = 1;//测量值校准
        [My("测量校准偏移")]
        [Description("测量校准偏移")]
        public double StD_RealFixOffset { get; set; } = 0;//实际测量值

        //--------------顶级模式--------------------------------------
        [My("启用顶级模式", false)]
        [Description("启用顶级模式")]
        public bool EnableTopModel { get; set; } = false;//打标
        [My("顶级模式系数", false)]
        [Description("顶级模式系数")]
        public double StD_TopModelRito { get; set; } = 1;//顶级模式系数
        [My("顶级模式偏移", false)]
        [Description("顶级模式偏移")]
        public double StD_TopModelOffset { get; set; } = 0;//顶级模式偏移

        //--------------打标--------------------------------------
        [My("打标下公差", true)]
        [Description("打标下公差")]
        public double StD_Down { get; set; } = -1;//标准值
        [My("打标上公差", true)]
        [Description("打标上公差")]
        public double StD_Up { get; set; } = 1;//标准值

        [My("累计DB", true)]
         public int CountNGDB { get; set; } = 1;//累计NG打标
        [My("打标", true)]
        [Description("打标")]
        public bool DB { get; set; } = false;//打标

        //--------------报警------------------------------------------
        [My("报警下公差", true)]
        [Description("报警下公差")]
        public double StD_BJ_Down { get; set; } = -1;//标准值
        [My("报警上公差", true)]
        [Description("报警上公差")]
        public double StD_BJ_Up { get; set; } = 1;//标准值
        [My("EA数量报警", true)]
        [Description("EA数量报警")]
        public int CountNGBJ { get; set; } = 1;//累计NG报警
        [My("报警", true)]
        [Description("报警")]
        public bool BJ { get; set; } = false;//报警

        //--------------停机------------------------------------------
        [My("停机下公差", false)]
        [Description("停机下公差")]
        public double StD_TJ_Down { get; set; } = -1;//标准值
        [My("停机上公差", false)]
        [Description("停机上公差")]
        public double StD_TJ_Up { get; set; } = 1;//标准值
        [My("EA累计停机", false)]
        [Description("EA累计停机")]
        public int CountNGTJ { get; set; } = 1;//累计NG停机
        [My("停机", false)]
        [Description("停机")]
        public bool TJ { get; set; } = false;//停机

        [My("所属分条")]
        [Description("所属分条")]
        public double[] ArrayBelongSlice { get; set; } = new double[8];//所属分条

        /// <summary>
        /// 序列化时调用
        /// </summary>
        /// <param name="context"></param>
        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {

        }
    }
    /// <summary>
    /// 缺陷检测项
    /// </summary>
    [Serializable]

    public class DefectItemInfo
    {
        [My("编号")]
        [Description("编号")]
        public int Index { get; set; } = 0;//名称
        [My("启用")]
        [Description("启用")]
        public bool Enable { get; set; } = true;//打标
        [My("名称")]
        [Description("名称")]
        public string Name { get; set; } = "名称";//名称
        [My("备注")]
        [Description("备注")]
        public string Description { get; set; } = "备注";
        [My("所属相机")]
        [Description("所属相机")]
        public CameraEnum camera { get; set; } = 0;
        [My("启用面积")]
        [Description("启用面积")]
        public bool EnableArea { get; set; } = true;//打标
        [My("面积")]
        [Description("面积")]
        public double Area { get; set; } = 1;//面积


        [My("启用宽度")]
        [Description("启用宽度")]
        public bool EnableWidth { get; set; } = true;//打标
        [My("宽或逻辑")]
        [Description("宽或逻辑")]
        public bool WidthLogic { get; set; } = true;//宽或逻辑
        [My("宽度")]
        [Description("宽度")]
        public double Width { get; set; } = 1;//宽度

        [My("启用高度")]
        [Description("启用高度")]
        public bool EnableHeight { get; set; } = true;//打标
        [My("高或逻辑")]
        [Description("高或逻辑")]
        public bool HeightLogic { get; set; } = true;//高或逻辑
        [My("高度")]
        [Description("高度")]
        public double Height { get; set; } = 1;//高度


        [My("打标")]
        [Description("打标")]
        public bool DB { get; set; } = false;//打标
        [My("累计DB")]
        [Description("累计DB")]
        public int CountNGDB { get; set; } = 1;//累计NG打标

        [My("报警")]
        [Description("报警")]
        public bool BJ { get; set; } = false;//报警
        [My("累计BJ")]
        [Description("累计BJ")]
        public int CountNGBJ { get; set; } = 1;//累计NG报警

        [My("停机")]
        [Description("停机")]
        public bool TJ { get; set; } = false;//停机
        [My("累计TJ")]
        [Description("累计TJ")]
        public int CountNGTJ { get; set; } = 1;//累计NG停机

    }

    /// <summary>
    /// 尺寸检测项-编辑器
    /// </summary>
    [Serializable]
    public class SizeItemEditor
    {
        [My("编号")]
        public int Index { get; set; } = 0;//名称
        [My("启用")]
        public bool Enable { get; set; } = true;//打标
        [My("名称")]
        public string Name { get; set; } = "名称";//名称


        [My("默认值")]
        public double DefaultValue { get; set; } = -999;//默认值

        [My("线线计算类型")]
        public string EditorType { get; set; } = EditorTypeEnum.线距离X.ToString();//线线计算类型 EditorTypeEnum转int

        [My("抓边1名称")]
        public string Line1 { get; set; } = "抓边1";//抓边1名称

        [My("抓边2名称")]
        public string Line2 { get; set; } = "抓边2";//抓边2名称


    }

    /// <summary>
    /// 叠台对齐度工位专用工艺类。
    /// </summary>
    [Serializable]
    public class PointEdit
    {
        [My("编号")]
        public int Index { get; set; } = 0;//编号
        [My("名称")]
        public string Name { get; set; } = "名称";
        [My("备注")]
        public string Description { get; set; } = "备注";

        [My("所属相机")]
        public CameraEnum camera { get; set; } = 0;
        [My("抓边1")]
        public string Line1 { get; set; } = "默认抓边1";
        [My("抓边2")]
        public string Line2 { get; set; } = "默认抓边2";
        [My("线线计算类型")]
        public string EditorType { get; set; } = EditorTypeEnum.线距离X.ToString();//线线计算类型 EditorTypeEnum转int

        [My("偏移下限")]
        public double XbiasMin { get; set; } = -0.3;

        [My("偏移上限")]
        public double XbiasMax { get; set; } = 0.3;

    }

    /// <summary>
    /// 检测项标准枚举
    /// </summary>
    public class MyAttribute : Attribute
    {
        /// <summary>
        /// 代码
        /// </summary>
        public string HeadName { get; set; }

        // public string ColumnType { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public bool Is_ShowCol { get; set; }

        //   public string[] ComoBoxItem { get; set; }
        public MyAttribute()
        {
            this.HeadName = "未命名项目";
            // this.ColumnType = "TextBox";
            this.Is_ShowCol = true;
        }
        public MyAttribute(string headName)
        {
            this.HeadName = headName;
            // this.ColumnType = "TextBox";
            this.Is_ShowCol = true;
        }
        public MyAttribute(string headName, bool is_Ignore)
        {
            this.HeadName = headName;
            //this.ColumnType = "TextBox";
            this.Is_ShowCol = is_Ignore;
        }

    }

    //EA通讯条码定义
    //YH:校验指令； 01/02：阴极/阳极  F/M/E：开始/中段/结束； 01：产品当前序号 ；EaCode：虚拟条码；
    //"YH;01;F;01;EaCode"

    // SEaType : 00
    // bIsCathode : false 
    // SEaIndex : 9
    // StrEACode : eaNo662
    // PackageCode : pkNo032 
    // CRC : ck021
    /// <summary>
    /// EA条码信息
    /// </summary>
    public class EaCodeInfo
    {
        /// <summary>
        /// SEaType  00：开始-单阳SCS / 01：过程-双阴AD / 10：过程-双阳CD / 11：结束-单阳ECS
        /// </summary>
        public int SEaType { get; set; } = 0;
        /// <summary>
        /// bIsCathode：是否是阴极
        /// </summary>
        public bool bIsCathode = false;
        /// <summary>
        /// SEaIndex：当前EA单片序号
        /// </summary>
        public int SEaIndex { get; set; } = 0;
        /// <summary>
        /// EaCode：虚拟条码 设备通讯的EA单片条码，每个单片一个码
        /// </summary>
        public string StrEACode { get; set; } = "virtualEA";
        /// <summary>
        /// PackageCode：电芯包编号
        /// </summary>
        public string PackageCode { get; set; } = "dafultPackage";
        /// <summary>
        /// CRC:校验指令；
        /// </summary>
        public string CRC { get; set; } = "";
    }
}



