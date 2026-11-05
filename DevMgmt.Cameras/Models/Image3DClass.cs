using HalconDotNet;

using LogTool;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DevMgmt.Cameras.Models
{
    ////与存图序列化有关，勿动!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    ////!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!2020.12.20（叶）
    ////!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    //[Serializable]      //可序列化标志
    //public class Image3DClass
    //{
    //    [Description("3D深度数据")]
    //    public HObject Image3D;

    //    [Description("bmp图像")]
    //    public HObject Image2D;

    //    //2022-06-06-,lfh
    //    [Description("亮度图")]
    //    public HObject Image_Light;

    //    [Description("3D信息")]
    //    public CameraObj.Cam_Calibration Info3D;

    //    [Description("其它信息")]
    //    public Dictionary<string, string> Info;

    //    /// <summary>
    //    /// 当前图像ID
    //    /// </summary>
    //    public long image_ID = 0;

    //    public Image3DClass(HObject image3d, HObject image2d, CameraObj.Cam_Calibration info3d)
    //    {
    //        Image3D = image3d;
    //        Image2D = image2d;
    //        Info3D = info3d;
    //        Info = new Dictionary<string, string>();
    //    }

    //    public Image3DClass()
    //    {

    //        Image3D = null;
    //        Image2D = null;
    //        Image_Light = null;
    //        Info3D = new CameraObj.Cam_Calibration();
    //        Info = new Dictionary<string, string>();
    //    }

    //    public Image3DClass Copy()
    //    {
    //        Image3DClass info = new Image3DClass();
    //        Image3D?.CopyImage(out info.Image3D);
    //        Image2D?.CopyImage(out info.Image2D);
    //        Image_Light?.CopyImage(out info.Image_Light);
    //        info.Info3D = new CameraObj.Cam_Calibration(Info3D) ;
    //        info.Info = new Dictionary<string, string>(Info) ;
    //        return info;
    //    }

    //    public Image3DClass(Image3DClass image3d)
    //    {
    //        Image3D = new HObject(image3d.Image3D);
    //        Image2D = new HObject(image3d.Image2D);
    //        if (image3d.Image_Light != null)
    //            Image_Light = new HObject(image3d.Image_Light);
    //        Info3D = new CameraObj.Cam_Calibration(image3d.Info3D);
    //        Info = new Dictionary<string, string>(image3d.Info);
    //    }

    //    public void Dispose()
    //    {
    //        if (Image3D != null)
    //            Image3D.DisposeEx();
    //        if (Image2D != null)
    //            Image2D.DisposeEx();
    //        if (Image_Light != null)
    //            Image_Light.DisposeEx();
    //        Info3D.Clear();
    //        Info.Clear();
    //    }

    //    public void SaveImage3D(string SavePath, string strCode)
    //    {
    //        try
    //        {
    //            //2022-06-20,lhf 屏蔽，因为有的型号二维码长度小于24
    //            //if (strCode.Length < 24)
    //            //{
    //            //    strCode = ("ERROR").PadRight(24, '0');
    //            //}
    //           // Common.CheckFileNameError(strCode, out strCode);
    //            string time = DateTime.Now.ToString("yyyy.MM.dd HH.mm.ss.fff");
    //            //string filepath = SavePath + "/" + time + " " + strCode.Substring(0, 24) + "_" + ".dat";
    //            string filepath = SavePath + "/" + time + " " + strCode + ".dat";
    //            //if (!System.IO.File.Exists(filepath))
    //            //{
    //            //    System.IO.File.Create(filepath).Close();//不存在则创建,并释放对创建文件的占用
    //            //}
    //            SaveAndLoad_Data.Save(this, filepath);
    //        }
    //        catch (Exception ex)
    //        {
    //            Log4netRecord.ErrorLog.Fatal(ex.Message + ex.StackTrace);
    //        }
    //    }

    //    //2022-06-06,lfh
    //    public void Save_Image(string strPath, string strCode)
    //    {
    //        try
    //        {

    //            string strTime = "";
    //            string strName = "";
    //            string strFilePath = "";

    //          //  Common.CheckFileNameError(strCode, out strCode);
    //            strTime = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff");
    //            //亮度图
    //            strName = strTime + " " + strCode + "_" + ".bmp";
    //            strFilePath = strPath + "/" + strName;
    //            //这有问题

    //            if (Image_Light.IsInitialized() && Image_Light != null)
    //            {

    //                HOperatorSet.WriteImage(Image_Light, "bmp", 0, strFilePath);
    //            }
    //            //tif图
    //            strName = strTime + " " + strCode + "_" + ".tif";
    //            strFilePath = strPath + "/" + strName;
    //            if (Image3D.IsInitialized() && Image3D != null)
    //            {

    //                HOperatorSet.WriteImage(Image3D, "tiff", 0, strFilePath);
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Log4netRecord.ErrorLog.Fatal(ex.Message + ex.StackTrace);
    //        }
    //    }
    //    /// <summary>
    //    /// 保存3D图
    //    /// </summary>
    //    /// <param name="strPath">路径</param>
    //    /// <param name="strCode">条码</param>
    //    /// <param name="_2DType">2D图保存类型:0=jpeg 1=bmp 2=png</param>
    //    /// <param name="CompressTiffImage">是否压缩tiff图</param>
    //    /// <param name="_3DType">3D图保存类型:3=tif 5=tif+dat 6=dat</param>
    //    public void Save_Image_Test(string strPath, string strCode, int _2DType,
    //        bool CompressTiffImage, int _3DType = 3)
    //    {
    //        try
    //        {
    //            string imagetype = "jpeg";
    //            switch (_2DType)
    //            {
    //                case 0: imagetype = "jpeg"; break;
    //                case 1: imagetype = "bmp"; break;
    //                case 2: imagetype = "png fastest"; break;
    //                default: imagetype = "jpeg"; break;
    //            }


    //            Log4netRecord.CamLog.Fatal("进入3D存图");

    //            string strTime = "";
    //            string strName = "";
    //            string strFilePath = "";

    //           // Common.CheckFileNameError(strCode, out strCode);
    //            strTime = DateTime.Now.ToString("yyyy.MM.dd HH.mm.ss.fff");
    //            //亮度图
    //            //strName = strTime + " " + strCode + ".bmp";
    //            strName = strTime + " " + strCode;
    //            strFilePath = strPath + "/" + strName;
    //            ////这有问题

    //            if (Image_Light.IsInitialized() && Image_Light != null)
    //            {
    //                Log4netRecord.CamLog.Fatal("进入3D存图:保存亮度图");
    //                HOperatorSet.WriteImage(Image_Light, imagetype, 0, strFilePath);
    //            }
    //            //tif图
    //            strName = strTime + " " + strCode + ".tif";
    //            strFilePath = strPath + "/" + strName;
    //            if (Image3D.IsInitialized() && Image3D != null)
    //            {
    //               if (_3DType == 3 || _3DType == 5)
    //                {
    //                    Log4netRecord.CamLog.Fatal("进入3D存图:保存深度图");
    //                    if (CompressTiffImage)
    //                    {
    //                        //压缩后
    //                        HOperatorSet.WriteImage(Image3D, "tiff deflate 1", 0, strFilePath);
    //                    }
    //                    else
    //                    {
    //                        //不压缩
    //                        HOperatorSet.WriteImage(Image3D, "tiff", 0, strFilePath);
    //                    }

    //               }
    //                 else
    //                {
    //                    Log4netRecord.CamLog.Fatal("进入3D存图:未选择保存深度图");
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            Log4netRecord.ErrorLog.Fatal(ex.Message + ex.StackTrace);
    //        }
    //    }
    //}

    //public class Image3DMethod
    //{
    //    /// <summary>
    //    /// 修改字典中的值
    //    /// </summary>
    //    /// <param name="info"></param>
    //    /// <param name="key"></param>
    //    /// <param name="value"></param>
    //    public static void DicModify(ref Dictionary<string, string> info, string key, string value)
    //    {
    //        if (!info.ContainsKey(key))
    //        {
    //            info.Add(key, value);
    //        }
    //        else
    //        {
    //            info[key] = value;
    //        }
    //    }

    //}
}
