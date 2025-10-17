using HalconDotNet;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Frame_Core
{
    /// <summary>
    /// 清洗机EA产品类
    /// </summary>
    public class PFTCleanEAProductInfo : ProductBase
    {
        //尺寸计算项
        public int EAID = 0;
        //尺寸计算项
        public bool EAFinfished = false;


        #region 主相机产品
        /// <summary>
        /// 涂布区起始终止位置
        /// </summary>
        public RegionSE Coatrange = new RegionSE();
        /// <summary>
        /// EA起始终止位置
        /// </summary>
        public RegionSE EArange = new RegionSE();
        /// <summary>
        /// EA裁图起始终止位置
        /// </summary>
        public RegionSE CropEArange = new RegionSE();



        //分条产品队列
        //N条，
        public List<PFTSliceProduct> ListEASlicePD = new List<PFTSliceProduct>();


        //料头检测完成
        public bool BlHeadDone = false;
        //料尾检测完成
        public bool BlTileDone = false;

        //////待检测槽位编号
        ////public int curSlotIndex = 0;
        //////待检测槽位编号
        ////public int curSlShaveIndex = 0;
        ////待检测槽位编号
        //public int[] ArraycurShaveIndex = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };


        /// <summary>
        /// EALine抓边项 ,id+行
        /// </summary>
        public List<LineInfo> ListEAlines = new List<LineInfo>();
        /// <summary>
        /// EASize工艺检测项
        /// </summary>
        public List<SizeInfo> ListEASizes = new List<SizeInfo>();
        /// <summary>
        /// EADefect缺陷检 ，id+行的
        /// </summary>
        public List<NgTypeInfo> ListEADefects = new List<NgTypeInfo>();

        /// <summary>
        /// 当前EA的图像集合
        /// </summary>
        public HObject EAImageCollection = new HObject();

        /// <summary>
        /// 当前EA的完整拼图
        /// </summary>
        public HObject TotalEAImage = new HObject();
        /// <summary>
        /// 当前EA的完整拼图开始标志位
        /// </summary>
        public bool blStartTotalEAImage = false;

        /// <summary>
        /// 当前EA的完整拼图结束标志位
        /// </summary>
        public bool blEndTotalEAImage = false;
        #endregion


        public void InitDispose()
        {
            EAImageCollection?.DisposeEx();

            foreach (PFTSliceProduct item in ListEASlicePD)
            {
                item?.Dispose();
            }
            ListEASlicePD.Clear();


            ListEAlines.Clear();
            foreach (var item in ListEASizes)
            {
                item?.Dispose();
            }
            ListEASizes.Clear();
            foreach (var item in ListEADefects)
            {
                item?.Dispose();
            }
            ListEADefects.Clear();

        }


        public override void Dispose()
        {
            base.Dispose();
            //foreach (SlotProduct item in listEASlot)
            //{
            //    item?.Dispose();
            //}
            //listEASlot.Clear();

    

            foreach (PFTSliceProduct item in ListEASlicePD)
            {
                item?.Dispose();
            }
            ListEASlicePD.Clear();


            ListEAlines.Clear();
            foreach (var item in ListEASizes)
            {
                item?.Dispose();
            }
            ListEASizes.Clear();
            foreach (var item in ListEADefects)
            {
                item?.Dispose();
            }
            ListEADefects.Clear();

            EAImageCollection.DisposeEx();
            TotalEAImage.DisposeEx();
        }
    }

    /// <summary>
    /// 清洗机单帧产品类
    /// </summary>
    public class PFTImageProduct : ProductBase
    {
        public long nCurImageID = 0;
        public int nCurEAID = 0;
        public bool blSplitEA = false;

        public bool IsASide = true;

        //涂布侧基准边L+R
        public double EdgelineLeft = 1000;
        public double EdgelineRight = 2000;


        ////当前检查的槽位编号
        //public List<SlotProduct> listSlot = new List<SlotProduct>();
        ////当前检查的削薄区编号
        //public List<SlotProduct> listShave = new List<SlotProduct>();

        public List<PFTSliceProduct> ListImageSlicePD = new List<PFTSliceProduct>();


        /// <summary>
        /// EALine抓边项 ,id+行的
        /// </summary>
        public List<LineInfo> ListImagelines = new List<LineInfo>();
        /// <summary>
        /// EASize工艺检测项
        /// </summary>
        public List<SizeInfo> ListImageSizes = new List<SizeInfo>();
        /// <summary>
        /// EADefect缺陷检 ，id+行的
        /// </summary>
        public List<NgTypeInfo> ListImageDefects = new List<NgTypeInfo>();



        //涂布区域缺陷直接放Base里面的line和NgType。
        public override void Dispose()
        {
            base.Dispose();

            //foreach (SlotProduct item in listSlot)
            //{
            //    item.Dispose();
            //}
            //listSlot.Clear();
            //foreach (SlotProduct item in listShave)
            //{
            //    item.Dispose();
            //}
            //listShave.Clear();

            foreach (PFTSliceProduct item in ListImageSlicePD)
            {
                item.Dispose();
            }
            ListImageSlicePD.Clear();

            ListImagelines.Clear();
            foreach (var item in ListImageDefects)
            {
                item?.Dispose();
            }
            ListImageDefects.Clear();
            foreach (var item in ListImageSizes)
            {
                item?.Dispose();
            }
            ListImageSizes.Clear();


        }


    }
    /// <summary>
    /// 区域起始结束ID和行列
    /// </summary>
    public class RegionSE
    {
        public int StartImageID = 0;       //起始
        public double StarImageRow = 0;    //起始
        public double StarImageColumn = 0; //起始

        public int EndImageID = 0;        //终止
        public double EndImageRow = 0;    //终止
        public double EndImageColumn = 0; //终止

        public int MidImageID = 0;       //中心
        public double MidImageRow = 0;   //中心
        public double MidImageColumn = 0;//中心

        public double StartCol = 1000;//高
        public double EndCol = 2000;//宽

        public double Width = 1000;//高
        public double Height = 1000;//宽

    }

    /// <summary>
    /// 清洗机单槽位或削薄区的产品类。每个槽位包含一个裁图，四条抓边，一个缺陷检测。
    /// </summary>
    public class SlotProduct : ProductBase
    {
        /// <summary>
        /// 槽位/削薄区工艺
        /// </summary>
        public SlotInfo slotInfo = new SlotInfo();

        public SlottypeEnum Slottype = SlottypeEnum.slot;
        /// <summary>
        /// 该槽位/削薄区是否完成检测
        /// </summary>
        public bool IsFinished = false;
        /// <summary>
        /// 槽位裁图起始终止行
        /// </summary>
        public RegionSE Croprange = new RegionSE();

        /// <summary>
        /// 槽位槽位/削薄区Line抓边项，id+行的，转换成原图位置 -上边，下边，左边，右边。
        /// </summary>
        public List<LineInfo> Slotlines = new List<LineInfo>();
        /// <summary>
        /// 槽位/削薄区Size工艺检测项-TD定位距，MD定位距，TD尺寸，MD尺寸。
        /// </summary>
        public List<SizeInfo> SlotSizes = new List<SizeInfo>();
        /// <summary>
        /// EADefect缺陷检 ，id+行的，转换成原图位置
        /// </summary>
        public List<NgTypeInfo> SlotDefects = new List<NgTypeInfo>();
        public override void Dispose()
        {
            Slotlines.Clear();
            foreach (var item in SlotDefects)
            {
                item?.Dispose();
            }
            SlotDefects.Clear();
            foreach (var item in SlotSizes)
            {
                item?.Dispose();
            }
            SlotSizes.Clear();
            base.Dispose();
        }
    }

    /// <summary>
    /// 清洗机单帧产品类
    /// </summary>
    public class PFTSliceProduct : ProductBase
    {
        public long nCurImageID = 0;
        public int nCurEAID = 0;
        public bool blSplitEA = false;

        public bool IsASide = true;

        //待检测槽位编号
        public int curSlotIndex = 0;
        //待检测槽位编号
        public int curSlShaveIndex = 0;

        //涂布侧基准边L+R
        public double EdgeLeft = 1000;
        public double EdgeRight = 2000;



        //当前检查的槽位编号
        public List<SlotProduct> listSlot = new List<SlotProduct>();
        //当前检查的削薄区编号
        public List<SlotProduct> listShave = new List<SlotProduct>();

        /// <summary>
        /// EALine抓边项 ,id+行的
        /// </summary>
        public List<LineInfo> ListImagelines = new List<LineInfo>();
        /// <summary>
        /// EASize工艺检测项
        /// </summary>
        public List<SizeInfo> ListImageSizes = new List<SizeInfo>();
        /// <summary>
        /// EADefect缺陷检 ，id+行的
        /// </summary>
        public List<NgTypeInfo> ListImageDefects = new List<NgTypeInfo>();

        public ResultType resultSliceDefectNg = ResultType.OK;

        public ResultType resultSliceSizeNg
        {
            get 
            {
                return new ResultType[] { resultSliceConfigSizeNg, resultSliceGrooveSizeNg }.Max();
            }
        }

        public ResultType resultSliceConfigSizeNg = ResultType.OK;

        public ResultType resultSliceGrooveSizeNg = ResultType.OK;
        /// <summary>
        /// 这里这是用于AB合并的结果
        /// </summary>
        public ResultType resultSliceAllResult = ResultType.OK;


        public ResultType resultSliceToltalResult
        {
            get
            {
                return new ResultType[] { resultSliceDefectNg, resultSliceSizeNg }.Max();
            }
        }


        //涂布区域缺陷直接放Base里面的line和NgType。
        public override void Dispose()
        {
            base.Dispose();

            foreach (SlotProduct item in listSlot)
            {
                item.Dispose();
            }
            listSlot.Clear();
            foreach (SlotProduct item in listShave)
            {
                item.Dispose();
            }
            listShave.Clear();

            ListImagelines.Clear();
            foreach (var item in ListImageDefects)
            {
                item?.Dispose();
            }
            ListImageDefects.Clear();
            foreach (var item in ListImageSizes)
            {
                item?.Dispose();
            }
            ListImageSizes.Clear();


        }


    }
}
