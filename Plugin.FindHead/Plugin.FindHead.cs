using Frame_Core;
using FrameCommunity.Data;
using HalconAlgorithm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.FindHead
{
    [Category("尺寸测量")]
    [DisplayName("查找料头工具")]
    //[ExportMetadata("measure1", "IPlugin")]
    [Export("FindHeadObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    class FindHeadObj : IPlugin
    {
        public PluginParamsBase PluginParams { get; set; } = new ParamsFindHead();
        ParamsFindHead findHeadParams;
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();

        public object Locker_Cam = new object();
        public ImageInfo imageInfo_now = new ImageInfo();
        private findHeadOperation findheadOperation = new findHeadOperation();
        int headID = -1;
        double headLine = -1;

        public event EventHandler<PluginExecuteCompletedEventArgs> ExecuteCompleted;

        public bool ExecuteObj()
        {
            bool result = false;
            try
            {
                findHeadParams = PluginParams as ParamsFindHead; //里氏转换
                headID = -1;
                headLine = -1;
                findHeadParams.hv_GrayResult = null;
                findHeadParams.hv_GrayMean = null;
                findHeadParams.bl_ContainHeadLine = false;

                lock (Locker_Cam)
                {
                    if (findHeadParams.list_ImageInfo.Count == 0)
                    {
                        //Console.WriteLine("--未加载图片--！");
                        return false;
                    }
                    imageInfo_now = findHeadParams.list_ImageInfo.Find(c => c.image_ID == findHeadParams.ImageNowID);//依次拿出图像队列中的图像
                    if (imageInfo_now == null)
                    {
                        //Console.WriteLine("--无指定图片--！");
                        return false;
                    }
                    //if (findHeadParams.list_ImageInfo.Count()>3)
                    //{
                    //    findHeadParams.list_ImageInfo.RemoveAt(0);
                    //}
                }
                if (findHeadParams.Is_FindHeadOrTrail)
                {
                    if (findHeadParams.Is_ActiveFindHead || findHeadParams.Is_TestFindHead)
                    {
                        findHeadParams.Is_TestFindHead = false;//模块测试使用

                        findheadOperation.JudgeWhiteRegion(imageInfo_now.ho_image, findHeadParams.hv_RegionThreshold, findHeadParams.hv_RegionNum, findHeadParams.hv_ColumnStart, findHeadParams.hv_ColumnEnd, out findHeadParams.hv_GrayResult, out findHeadParams.hv_GrayMean);
                        findheadOperation.findHeadLineAndID(findHeadParams, findHeadParams.hv_GrayResult, out headID, out headLine);
                        if (headID < 0 || headLine < 0)
                        {
                            return false;
                        }
                        else
                        {
                            findHeadParams.bl_ContainHeadLine = true;
                            findHeadParams.headLineID = headID;
                            findHeadParams.headLine = headLine;
                            // Console.WriteLine("----主动找料头执行成功---！");
                            //GetResultParams(findHeadParams.str_ModelName, ResultLine);
                            //Console.WriteLine("抓边结果：起点行：{0}，起点列：{1}，终点行：{2}，终点列：{3}", findHeadParams.hv_LineRowBegin, findHeadParams.hv_LineColumnBegin, findHeadParams.hv_LineRowEnd, findHeadParams.hv_LineColumnEnd);
                            return true;
                        }
                    }
                    else
                    {
                        headID = findHeadParams.headLineID;
                        headLine = findHeadParams.headLine;
                        if (headID < 0 || headLine < 0)
                        {
                            return false;
                        }
                        else
                        {
                            double cellHeight = imageInfo_now.imageHeight / findHeadParams.hv_RegionNum;
                            double headLine_begin = headLine - cellHeight;
                            int headID_begin = headID;
                            while (headLine_begin < 0)
                            {
                                headLine_begin += imageInfo_now.imageHeight;
                                headID_begin -= 1;
                            }
                            while (headLine_begin > imageInfo_now.imageHeight)
                            {
                                headLine_begin -= imageInfo_now.imageHeight;
                                headID_begin += 1;
                            }
                            double headLine_end = headLine + cellHeight;
                            int headID_end = headID;
                            while (headLine_end < 0)
                            {
                                headLine_end += imageInfo_now.imageHeight;
                                headID_end -= 1;
                            }
                            while (headLine_end > imageInfo_now.imageHeight)
                            {
                                headLine_end -= imageInfo_now.imageHeight;
                                headID_end += 1;
                            }
                            if (true)
                            {

                                findHeadParams.ho_ImageHead = findheadOperation.splicingAndCrop.OutStitchingAndCropImage(findHeadParams.list_ImageInfo, headID_begin, headLine_begin, headID_end, headLine_end, findHeadParams.hv_ColumnStart, findHeadParams.hv_ColumnEnd);

                                double headLine_re = -1;
                                int headID_re = headID_begin;

                                findheadOperation.ImagePretreatment(findHeadParams); //找料头图像预处理

                                //findHeadParams.ho_ImageMeasureLine = findHeadParams.ho_ImageHead;
                                findHeadParams.hv_ImageHeadBeginID = headID_begin;
                                findHeadParams.hv_ImageHeadBeginRow = headLine_begin;
                                findHeadParams.hv_ImageHeadBeginCol = findHeadParams.hv_ColumnStart;
                                findHeadParams.hv_ImageHeadEndID = headID_end;
                                findHeadParams.hv_ImageHeadEndRow = headLine_end;
                                findHeadParams.hv_ImageHeadEndCol = findHeadParams.hv_ColumnEnd;

                                findheadOperation.RefindHeadLine(findHeadParams, out headLine_re);
                                if (headLine_re > 0)
                                {
                                    headLine_re = headLine_begin + headLine_re;
                                    if (headLine_re > imageInfo_now.imageHeight)
                                    {
                                        headID_re = headID_begin + 1;
                                        headLine_re = headLine_re - imageInfo_now.imageHeight;
                                    }
                                    findHeadParams.headLineID = headID_re;
                                    findHeadParams.headLine = headLine_re;
                                    findHeadParams.bl_ContainHeadLine = true;

                                    //Console.WriteLine("----被动找料头执行成功---！");
                                    //GetResultParams(findHeadParams.str_ModelName, ResultLine);
                                    //Console.WriteLine("抓边结果：起点行：{0}，起点列：{1}，终点行：{2}，终点列：{3}", findHeadParams.hv_LineRowBegin, findHeadParams.hv_LineColumnBegin, findHeadParams.hv_LineRowEnd, findHeadParams.hv_LineColumnEnd);

                                    return true;
                                }
                            }
                        }
                        return false;
                    }
                }
                else
                {
                    if (true || findHeadParams.Is_TestFindHead)
                    {
                        findHeadParams.Is_TestFindHead = false;//模块测试使用

                        findheadOperation.JudgeWhiteRegion(imageInfo_now.ho_image, findHeadParams.hv_RegionThreshold, findHeadParams.hv_RegionNum, findHeadParams.hv_ColumnStart, findHeadParams.hv_ColumnEnd, out findHeadParams.hv_GrayResult, out findHeadParams.hv_GrayMean);
                        findheadOperation.findTrailLineAndID(findHeadParams, findHeadParams.hv_GrayResult, out headID, out headLine);
                        if (headID < 0 || headLine < 0)
                        {
                            return false;
                        }
                        else
                        {
                            findHeadParams.bl_ContainHeadLine = true;
                            findHeadParams.headLineID = headID;
                            findHeadParams.headLine = headLine;
                            // Console.WriteLine("----主动找料头执行成功---！");
                            //GetResultParams(findHeadParams.str_ModelName, ResultLine);
                            //Console.WriteLine("抓边结果：起点行：{0}，起点列：{1}，终点行：{2}，终点列：{3}", findHeadParams.hv_LineRowBegin, findHeadParams.hv_LineColumnBegin, findHeadParams.hv_LineRowEnd, findHeadParams.hv_LineColumnEnd);
                            return true;
                        }
                    }
                }
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        public void TestExecuteObj()
        {
            try
            {
                double headLine_re;
                findheadOperation.RefindHeadLine(findHeadParams, out headLine_re);
            }
            catch (Exception)
            {


            }
        }

        public void GetResultParams(string varName, object obj)
        {
            Process proc = Project.Instance.GetProcessById(moduleInfo.ProcessID);
            proc.AddOutPutParam(moduleInfo.ModuleName, varName, obj);
        }

        public bool saveParams()
        {
            PluginParams.filePath = moduleInfo.filePath;
            if (PluginParams.Save())
            {
                return true;
            }
            return false;
        }

        public void SetParams(PluginParamsBase param)
        {
            this.PluginParams = (ParamsFindHead)param;
            findHeadParams = PluginParams as ParamsFindHead; //里氏转换
        }

        public void IniObj()
        {
        }

        public void DisPoseObj()
        {
        }
    }
}
