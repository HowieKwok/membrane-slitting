using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    [Serializable]
    public class StackEACraft : CraftBase  //纠偏相机设置在纠偏界面设置。
    {

      public  StackEACraft()
        { 
            //Init();
        }
        #region 通用设置
        /// <summary>
        /// 极片尺寸CW
        /// </summary>
        [Description("极片尺寸CW")]
        public double EASizeCW { get; set; } = 80;

        /// <summary>
        /// 极片尺寸CW2
        /// </summary>
        [Description("极片尺寸CW2")]
        public double EASizeCW2 { get; set; } = 100;

        /// <summary>
        /// 极片尺寸CH
        /// </summary>
        [Description("极片尺寸CH")]
        public double EASizeCH { get; set; } = 80;

        /// <summary>
        /// 极片尺寸CH2
        /// </summary>
        [Description("极片尺寸CH2")]
        public double EASizeCH2 { get; set; } = 100;


        /// <summary>
        /// 极耳尺寸JRH
        /// </summary>
        [Description("极耳尺寸JRH")]
        public double JRH { get; set; } = 10;

        /// <summary>
        /// 极耳尺寸JRW
        /// </summary>
        [Description("极耳尺寸JRW")]
        public double JRW { get; set; } = 10;

        /// <summary>
        /// 台阶尺寸TagH
        /// </summary>
        [Description("台阶尺寸TagH")]
        public double JRTagH { get; set; } = 2;


        /// <summary>
        /// 极耳左边距JRL
        /// </summary>
        [Description("极耳左边距JRL")]
        public double JRL { get; set; } = 65;

        /// <summary>
        /// 极耳右边距JRR
        /// </summary>
        [Description("极耳右边距JRR")]
        public double JRR { get; set; } = 25;



        //缺陷检测区域，
        /// <summary>
        /// 凹版平滑系数
        /// </summary>
        [Description("凹版平滑系数")]
        public double colsingRadius { get; set; } = 100;

        /// <summary>
        /// 凸版平滑系数
        /// </summary>
        [Description("凸版平滑系数")]
        public double openingRadius { get; set; } = 100;



        /// <summary>
        /// 阳极槽位左边距离侧基准边
        /// </summary>
        [Description("槽位边距")]
        public double GrovED { get; set; } = 0;


        //线扫缺陷检测，区域缩进
        /// <summary>
        /// 左缩进
        /// </summary>
        [Description("左缩进")]
        public double LeftShaft1 { get; set; } = 80;

        /// <summary>
        /// 上缩进
        /// </summary>
        [Description("上缩进")]
        public double UpShaft1 { get; set; } = 100;

        /// <summary>
        /// 右缩进
        /// </summary>
        [Description("右缩进")]
        public double RightShaft1 { get; set; } = 80;

        /// <summary>
        /// 下缩进
        /// </summary>
        [Description("下缩进")]
        public double DownShaft1 { get; set; } = 100;

        /// <summary>
        /// 左缩进
        /// </summary>
        [Description("左缩进")]
        public double LeftShaft2 { get; set; } = 80;

        /// <summary>
        /// 上缩进
        /// </summary>
        [Description("上缩进")]
        public double UpShaft2 { get; set; } = 100;

        /// <summary>
        /// 右缩进
        /// </summary>
        [Description("右缩进")]
        public double RightShaft2 { get; set; } = 80;

        /// <summary>
        /// 下缩进
        /// </summary>
        [Description("下缩进")]
        public double DownShaft2 { get; set; } = 100;



        /// <summary>
        /// 放卷纠偏基准边
        /// </summary>
        [Description("放卷纠偏基准边")]
        public double UnwindBaseLineY { get; set; } = 100;

        /// <summary>
        /// 放卷纠偏补偿值mm
        /// </summary>
        [Description("放卷纠偏基准边")]
        public double UnwindOffsetY { get; set; } = 0;
        /// <summary>
        /// 放卷纠偏值取反标志位
        /// </summary>
        [Description("放卷纠偏值取反标志位")]
        public bool UnwindFlag { get; set; } = false;

        /// <summary>
        /// 放卷纠偏值取反标志位
        /// </summary>
        [Description("放卷纠偏值R取反标志位")]
        public bool UnwindFlagR { get; set; } = false;

        /// <summary>
        /// 模切纠偏基准边Y
        /// </summary>
        [Description("模切纠偏基准边Y")]
        public double DieCutBaseLineY { get; set; } = 100;

        /// <summary>
        /// 放卷纠偏补偿值mm
        /// </summary>
        [Description("放卷纠偏基准边Y")]
        public double DieCutOffsetY { get; set; } = 0;
        /// <summary>
        /// 模切纠偏值取反标志位
        /// </summary>
        [Description("模切纠偏值取反标志位")]
        public bool DieCutFlagY { get; set; } = false;

        /// <summary>
        /// 模切纠偏值取反标志位
        /// </summary>
        [Description("模切纠偏值Y_R取反标志位")]
        public bool DieCutFlagY_R { get; set; } = false;

        /// <summary>
        /// 模切纠偏基准边X
        /// </summary>
        [Description("模切纠偏基准边X")]
        public double DieCutBaseLineX { get; set; } = 100;

        /// <summary>
        /// 放卷纠偏补偿值mm
        /// </summary>
        [Description("放卷纠偏基准边X")]
        public double DieCutOffsetX { get; set; } = 0;

        /// <summary>
        /// 模切纠偏值取反标志位
        /// </summary>
        [Description("模切纠偏值取反标志位")]
        public bool DieCutFlagX { get; set; } = false;


        /// <summary>
        /// 模切纠偏值取反标志位
        /// </summary>
        [Description("模切纠偏值X_R取反标志位")]
        public bool DieCutFlagX_R { get; set; } = false;

        /// <summary>
        /// 模切纠偏值启用Tabneck纠偏闭环
        /// </summary>
        [Description("模切纠偏启用Tabneck纠偏闭环")]
        public bool EnTabneckClosedLoop { get; set; } = false;


        #endregion

        #region 分相机设置

        ///// <summary>
        ///// 每个相机极性
        ///// </summary>
        //[Description("极性队列")]
        //public List<ElectrodeEnum> ListCamElectrode { get; set; }

        /// <summary>
        /// 抓边队列(所有相机的抓边集合)
        /// </summary>
        [Description("抓边队列")]
        public List<List<LineInfo>> ListLines { get; set; }

        /// <summary>
        /// 工艺编辑队列(基于抓边匹配)
        /// </summary>
        [Description("工艺编辑队列")]
        public List<List<SizeItemInfo>> ListSizeEdits { get; set; }

        /// <summary>
        /// 缺陷检测项，预留
        /// </summary>
        [Description("缺陷检测项队列")]
        public List<List<DefectItemInfo>> ListDefectInfos { get; set; }

        #endregion
        public override void Init()
        {
            CraftName = "叠片EA检测";
            BaseCamera = CameraEnum.Cam1;
            electrode = ElectrodeEnum.阴极;
            materialCrafEnum = MaterialCrafEnum.叠片阴极双面制片AD;

            //初始化抓边
            if (ListLines == null)
            {
                ListLines = new List<List<LineInfo>>();
            }
            else { ListLines.Clear(); }
            //初始化尺寸检测项
            if (ListSizeEdits == null)
            {
                ListSizeEdits = new List<List<SizeItemInfo>>();
            }
            else { ListSizeEdits.Clear(); }
            //初始化缺陷检测项
            if (ListDefectInfos == null)
            {
                ListDefectInfos = new List<List<DefectItemInfo>>();
            }
            else { ListDefectInfos.Clear(); }

            for (int i = 0; i < CamNum; i++)
            {
                ListLines.Add(new List<LineInfo>());
                ListSizeEdits.Add(new List<SizeItemInfo>());
                ListDefectInfos.Add(new List<DefectItemInfo>());
            }

        }

    }

}
