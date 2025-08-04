using NPOI.OpenXmlFormats.Dml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YYSD.UICore.Control;

namespace Frame_Core
{
    /// <summary>
    /// 交点基础类
    /// </summary>
    public class CoorPoint
    {
        /// <summary>
        /// 交点名称
        /// </summary>
        public string strCrossName { get; set; } = "defaultCross";

        /// <summary>
        /// 当前相机
        /// </summary>
        public CameraEnum CCamera;
        /// <summary>
        /// 当前相机交点X
        /// </summary>
        public double CPointX { get; set; } = 0;
        /// <summary>
        /// 当前相机交点Y
        /// </summary>
        public double CPointY { get; set; } = 0;

        /// <summary>
        /// 找到角点标志
        /// </summary>
        public bool CPointFinish { get; set; } = false;

        /// <summary>
        /// 主相机
        /// </summary>
        public CameraEnum AffineCamera;
        /// <summary>
        /// 仿射到主相机交点X
        /// </summary>
        public double AffinePointX { get; set; } = 0;
        /// <summary>
        /// 仿射到主相机交点Y
        /// </summary>
        public double AffinePointY { get; set; } = 0;

        /// <summary>
        /// 主相机坐标获取成功
        /// </summary>
        public bool MainCamCoordGetSucceed { get; set; } = false;

        /// <summary>
        /// 交点偏移结果-初始NG。
        /// </summary>
        public bool bl_PointResult { get; set; } = false;

        /// <summary>
        /// 与基准片1的偏移X（电芯包第一片为基准片）
        /// </summary>
        public double OffsetX { get; set; } = -999;
        /// <summary>
        /// 交点X偏移结果-初始NG。
        /// </summary>
        public bool bl_OffsetX { get; set; } = false;

        /// <summary>
        /// 与基准片1的偏移Y（电芯包第一片为基准片）
        /// </summary>
        public double OffsetY { get; set; } = -999;
        /// <summary>
        /// 交点Y偏移结果-初始NG。
        /// </summary>
        public bool bl_OffsetY { get; set; } = false;

    }

    /// <summary>
    ///  单相机图像产品-单相机单次，ovh，预定位的测角点产品类。
    /// </summary>
    public class CamEACornerProduct : ProductBase
    {
        public int intCamEnum { get; set; } = 0;

        /// <summary>
        /// 二维码
        /// </summary>
        public string QRCode { get; set; } = "";

        /// <summary>
        /// 相机帧号
        /// </summary>
        public long CamFrame { get; set; } = 0;

        /// <summary>
        /// 上一个相机帧号
        /// </summary>
        public long LastCamFrame { get; set; } = 0;

       
        /// <summary>
        /// 第几个电芯包
        /// </summary>
        public int PackgeNum { get; set; } = 1;


        /// <summary>
        /// 归属工位  1为A叠台  2为B叠台
        /// </summary>
        public int AffiliationStation { get; set; } = 0;

        /// <summary>
        /// 第几个单片EA，绝对计数，单工位处理EA的总数
        /// </summary>
        public int EAIndex { get; set; } = 0;

        /// <summary>
        /// 上Tabneck值
        /// </summary>
        public double UpTabneckValue = 0;

        /// <summary>
        /// 下Tabneck值
        /// </summary>
        public double DownTabneckValue = 0;


        public double PoleWidth1 = -999;
        public double PoleWidth2 = -999;
        public double PoleHeight1 = -999;
        public double PoleHeight2 = -999;
        public double PoleEarW = -999;
        public double PoleEarH = -999;
        public double PoleEarD = -999;

        public double MarkSizeW = -999;

        public double MarkSizeH = -999;


        public double CurrenPoleWidth1 = -999;
        public double CurrenPoleWidth2 = -999;
        public double CurrenPoleHeight1 = -999;
        public double CurrenPoleHeight2 = -999;


        /// <summary>
        /// 单相机检测结果
        /// </summary>
        public bool CamFinish { get; set; } = false;

        /// <summary>
        /// 相机原图相对路径
        /// </summary>
        public string PrimitiveImgSaveRelativePath = "";

        /// <summary>
        /// 相机截图相对路径
        /// </summary>
        public string ScreenshotSaveRelativePath = "";

        /// <summary>
        /// 隔膜结果
        /// </summary>
        public bool DiaphragmResult { get; set; } = false;

        /// <summary>
        /// 极片结果
        /// </summary>
        public bool PolePieceResult { get; set; } = false;

        /// <summary>
        /// 极片尺寸结果
        /// </summary>
        public bool PolePieceSizeResult { get; set; } = false;

        /// <summary>
        /// 单个相机极片角点：隔膜点、角点1、角点2
        /// </summary>
        public List<CoorPoint> coorEaPoints { get; set; } = new List<CoorPoint>();

        /// <summary>
        /// 单个相机隔膜定位 ：（ 左上隔膜，右上隔膜，左下隔膜，右下隔膜）；
        /// </summary>
        public List<CoorPoint> coorCoverPoints { get; set; } = new List<CoorPoint>();

        /// <summary>
        /// 单个相机基准极耳边
        /// </summary>
        public List<LineInfo> BaseLines { get; set; } = new List<LineInfo>();

    }

    /// <summary>
    /// OVH电池产品
    /// </summary>
    public class OVHBatteryProduct
    {
        /// <summary>
        /// 二维码
        /// </summary>
        public string QRCode { get; set; } = "";
       
        public List<CamEACornerProduct> EACornerProduct_Cam1 { get; set; } = new List<CamEACornerProduct>();
        public List<CamEACornerProduct> EACornerProduct_Cam2 { get; set; } = new List<CamEACornerProduct>();
        public List<CamEACornerProduct> EACornerProduct_Cam3 { get; set; } = new List<CamEACornerProduct>();
        public List<CamEACornerProduct> EACornerProduct_Cam4 { get; set; } = new List<CamEACornerProduct>();
        public List<CamEACornerProduct> EACornerProduct_Cam5 { get; set; } = new List<CamEACornerProduct>();
        public List<CamEACornerProduct> EACornerProduct_Cam6 { get; set; } = new List<CamEACornerProduct>();
        public List<CamEACornerProduct> EACornerProduct_Cam7 { get; set; } = new List<CamEACornerProduct>();
        public List<CamEACornerProduct> EACornerProduct_Cam8 { get; set; } = new List<CamEACornerProduct>();


        public void Get()
        {

        }

        public void Dispose()
        {
            //for (int i = 0; i < EACornerProduct_Cam1.Count; i++)
            //{
            //    EACornerProduct_Cam1[i]?.Dispose();
            //}
            //for (int i = 0; i < EACornerProduct_Cam2.Count; i++)
            //{
            //    EACornerProduct_Cam2[i]?.Dispose();
            //}
            //for (int i = 0; i < EACornerProduct_Cam3.Count; i++)
            //{
            //    EACornerProduct_Cam3[i]?.Dispose();
            //}
            //for (int i = 0; i < EACornerProduct_Cam4.Count; i++)
            //{
            //    EACornerProduct_Cam4[i]?.Dispose();
            //}
            //for (int i = 0; i < EACornerProduct_Cam5.Count; i++)
            //{
            //    EACornerProduct_Cam5[i]?.Dispose();
            //}
            //for (int i = 0; i < EACornerProduct_Cam6.Count; i++)
            //{
            //    EACornerProduct_Cam6[i]?.Dispose();
            //}
            //for (int i = 0; i < EACornerProduct_Cam7.Count; i++)
            //{
            //    EACornerProduct_Cam7[i]?.Dispose();
            //}
            //for (int i = 0; i < EACornerProduct_Cam8.Count; i++)
            //{
            //    EACornerProduct_Cam8[i]?.Dispose();
            //}
        }
    }
    /// <summary>
    /// OVH工位产品
    /// </summary>
    public class OVHStationProduct
    {
        /// <summary>
        /// 第几个电芯包
        /// </summary>
        public int PackgeNum { get; set; } = 1;

        /// <summary>
        /// 当前层数
        /// </summary>
        public int CurrentLayer = 0;

        /// <summary>
        /// 归属工位  1为A工位  2为B工位
        /// </summary>
        public int AffiliationStation { get; set; } = 0;

        public CamEACornerProduct EAProduct_Cam1 { get; set; } = new CamEACornerProduct();

        public CamEACornerProduct EAProduct_Cam2 { get; set; } = new CamEACornerProduct();

        public CamEACornerProduct EAProduct_Cam3 { get; set; } = new CamEACornerProduct();

        public CamEACornerProduct EAProduct_Cam4 { get; set; } = new CamEACornerProduct();

        public void Dispose()
        {
            EAProduct_Cam1?.Dispose();
            EAProduct_Cam2?.Dispose();
            EAProduct_Cam3?.Dispose();
            EAProduct_Cam4?.Dispose();
        }
    }

    /// <summary>
    /// 预定位电池产品
    /// </summary>
    public class PretargetingBatteryProduct
    {
        /// <summary>
        /// 二维码
        /// </summary>
        public string QRCode { get; set; } = "";

        /// <summary>
        /// 单EA第几次拍照
        /// </summary>
        public int EAIndex { get; set; } = 0;

        /// <summary>
        /// EA开始
        /// </summary>
        public bool EAProductStart = false;

        /// <summary>
        /// 拍照结果
        /// </summary>
        public bool PhotoResult = false;

        /// <summary>
        /// 纠偏值X
        /// </summary>
        public float OffsetValueX { get; set; } = 0;

        /// <summary>
        /// 纠偏值Y
        /// </summary>
        public float OffsetValueY { get; set; } = 0;

        /// <summary>
        /// 纠偏值R
        /// </summary>
        public float OffsetValueR { get; set; } = 0;

        public CamEACornerProduct EAProduct_Cam1 { get; set; } = new CamEACornerProduct();

        public CamEACornerProduct EAProduct_Cam2 { get; set; } = new CamEACornerProduct();

        public CamEACornerProduct EAProduct_Cam3 { get; set; } = new CamEACornerProduct();

        public CamEACornerProduct EAProduct_Cam4 { get; set; } = new CamEACornerProduct();

   

        public void Init()
        {
            QRCode = "";
            EAIndex = 0;
            EAProductStart = false;
            PhotoResult = false;
            OffsetValueX = 0;
            OffsetValueY = 0;
            OffsetValueR = 0;
        }
    }

    /// <summary>
    /// EA单片基础类
    /// </summary>
    public class SliceEAProduct
    {

        public int EAIndex { get; set; } = 0;
        /// <summary>
        /// 单相机检测结果
        /// </summary>
        public bool EAFinish { get; set; } = false;
        /// <summary>
        /// 单片检测结果
        /// </summary>
        public bool SliceEAResult { get; set; } = false;

        /// <summary>
        /// 判断是否是阴极
        /// </summary>
        public bool bIsCathode { get; set; } = false;

        /// <summary>
        /// 01：当前EA单片序号
        /// </summary>
        public int EaIndex { get; set; } = 0;

        /// <summary>
        /// 电芯包所有单片数据。四个相机联合
        /// </summary>
        public List<CamEACornerProduct> ListCamPDs { get; set; } = new List<CamEACornerProduct>();
    }

    public class PackageProduct
    {
        /// <summary>
        /// 第几个电芯包
        /// </summary>
        public int PackageNum { get; set; } = 0;
        /// <summary>
        /// 单相机检测结果
        /// </summary>
        public bool PackageFinish { get; set; } = false;
        /// <summary>
        /// 代表电芯包结果
        /// </summary>
        public bool PackageResult { get; set; } = false;

        /// <summary>
        /// 电芯包编号
        /// </summary>
        public String PackageCode { get; set; } = "dafultPackage";

        /// <summary>
        /// 电芯包所有单片数据。1  +  N  + （N-1） +  1
        /// </summary>
        public List<SliceEAProduct> ListEAPDs { get; set; } = new List<SliceEAProduct>();

    }

    /// <summary>
    ///  单相机图像产品-单相机单次，纠偏，尺寸缺陷的stackEA产品类。
    /// </summary>
    public class CamEAStackProduct : ProductBase
    {
        public int intCamEnum { get; set; } = 0;


        public int EAIndex { get; set; } = 0;
        /// <summary>
        /// 单相机检测完成
        /// </summary>
        public bool CamFinish { get; set; } = false;

        /// <summary>
        /// 单相机检测结果
        /// </summary>
        public bool CamResult { get; set; } = false;

        /// <summary>
        /// 单个EA尺寸检测，
        /// </summary>
        public List<SizeItemInfo> Listsize { get; set; } = new List<SizeItemInfo>();

        /// <summary>
        /// 单个EA尺寸检测，
        /// </summary>
        public List<DefectItemInfo> Listdefect { get; set; } = new List<DefectItemInfo>();

    }
}
