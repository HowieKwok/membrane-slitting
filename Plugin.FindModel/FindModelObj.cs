using Frame_Core;
using Frame_Core.pluginParams;
using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plugin.FindModel
{
    [Category("模板匹配")]
    [DisplayName("模板匹配检测工具")]
    [Export("FindModelObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class FindModelObj : IPlugin
    {
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();
        public PluginParamsBase PluginParams { get; set; } = new ParamsFindModel();


        /// <summary>
        /// 模板图片
        /// </summary>
        private HObject m_hoModelImage = null;


        //private HObject m_tempInitage = null;
        /// <summary>
        /// 已创建的模板ID
        /// </summary>

        /// <summary>
        /// 模板region
        /// </summary>
        private HObject m_hoRegion;

        public event EventHandler<PluginExecuteCompletedEventArgs> ExecuteCompleted;

        public void DisPoseObj()
        {
          
        }


        public bool SigleExecuteObj(string modleName)
        {
            ParamsFindModel IParamsFindModel = (ParamsFindModel)PluginParams;
            try
            {
                IParamsFindModel.m_ho_OutImgShow?.Dispose();
                IParamsFindModel.m_ho_OutCircleRegion?.Dispose();
                IParamsFindModel.m_ho_OutRegion?.Dispose();
                IParamsFindModel.m_ho_OutCropRegion?.Dispose();
                IParamsFindModel.m_strGoalName = "None";

                HTuple Find_Row = 0, Find_Col = 0, Find_Angle = 0, Find_Score = 0;
                HOperatorSet.GetImageSize(IParamsFindModel.ho_Image, out HTuple width, out HTuple height);
                if (IParamsFindModel.m_fEdgeFindCol >= width)
                {
                    HOperatorSet.GenRectangle1(out IParamsFindModel.m_ho_OutCropRegion, 0, 0, height, width);
                    HOperatorSet.ReduceDomain(IParamsFindModel.ho_Image, IParamsFindModel.m_ho_OutCropRegion, out IParamsFindModel.m_ho_OutImgShow);
                }
                else
                {
                    if (IParamsFindModel.m_fEdgeFindEndCol > IParamsFindModel.m_fEdgeFindCol && IParamsFindModel.m_fEdgeFindEndCol < width)
                    {
                        //HOperatorSet.GenRectangle1(out IParamsFindModel.m_ho_OutCropRegion, 0, IParamsFindModel.m_fEdgeFindCol, height, IParamsFindModel.m_fEdgeFindEndCol);
                        //HOperatorSet.GenRectangle1(out IParamsFindModel.m_ho_OutCropRegion, IParamsFindModel.m_fEdgeFindRow, IParamsFindModel.m_fEdgeFindCol, height, IParamsFindModel.m_fEdgeFindEndCol);
                        HOperatorSet.GenRectangle1(out IParamsFindModel.m_ho_OutCropRegion, IParamsFindModel.m_fEdgeFindRow, IParamsFindModel.m_fEdgeFindCol, IParamsFindModel.m_fEdgeFindEndRow, IParamsFindModel.m_fEdgeFindEndCol);
                    }
                    else
                    {
                        HOperatorSet.GenRectangle1(out IParamsFindModel.m_ho_OutCropRegion, 0, IParamsFindModel.m_fEdgeFindCol, height, width);
                    }

                    HOperatorSet.ReduceDomain(IParamsFindModel.ho_Image, IParamsFindModel.m_ho_OutCropRegion, out IParamsFindModel.m_ho_OutImgShow);
                   
                }
                try
                {

                    FindModelParam findModelParam = IParamsFindModel.m_lFindParams.Find(x => x.m_strModelName == modleName);

                    HOperatorSet.SetShapeModelParam(findModelParam.ht_ModelID, "timeout", IParamsFindModel.m_nOverTime);

                    MatclTool.FindModelMethor(findModelParam, findModelParam.ht_ModelID, IParamsFindModel.m_ho_OutImgShow, out Find_Row, out Find_Col, out Find_Angle, out Find_Score);

                    if (Find_Score?.Length > 0)
                    {
                        IParamsFindModel.m_fScoreMatch = Find_Score;
                        //HOperatorSet.GenRectangle2(out IParamsFindModel.m_ho_OutRegion, IParamsFindModel.m_fCenterRowBase, IParamsFindModel.m_fCenterColBase, IParamsFindModel.m_fCenterAngleBase, IParamsFindModel.m_fCenterLength_X, IParamsFindModel.m_fCenterLength_Y);
                        //HOperatorSet.VectorAngleToRigid(0,0, 0, Find_Row, Find_Col, Find_Angle,out IParamsFindModel.m_HtuMatID);
                        //HOperatorSet.AffineTransRegion(IParamsFindModel.m_ho_OutRegion, out IParamsFindModel.m_ho_OutRegion, IParamsFindModel.m_HtuMatID, "nearest_neighbor");
                    }
                    
                


                }
                catch (Exception ee)
                {
                    IParamsFindModel.m_fScoreMatch = 0;
                   
                    HOperatorSet.GenRectangle2(out IParamsFindModel.m_ho_OutRegion, 10, 10, 0, 2, 2);
                    Log4netRecord.ErrorLog.Debug("找模板插件执行失败：" + ee.ToString());
                    return false;
                }

                if (Find_Col?.Length != 0)
                {
                    //.GenRectangle2(out IParamsFindModel.m_ho_OutRegion, Find_Row, Find_Col, Find_Angle, IParamsFindModel.m_fCenterLength_X, IParamsFindModel.m_fCenterLength_Y);
                    CreateModelParam createModelParam = IParamsFindModel.m_lCreateParams.Find(x => x.m_strModelName == modleName);
                    IParamsFindModel.m_strGoalName = modleName;
                    HOperatorSet.GenRectangle2(out IParamsFindModel.m_ho_OutRegion, createModelParam.m_fCenterRowBase, createModelParam.m_fCenterColBase, 0, createModelParam.m_fCenterLength_XBase, createModelParam.m_fCenterLength_YBase);
                    HOperatorSet.VectorAngleToRigid(createModelParam.m_fCenterRowBase, createModelParam.m_fCenterColBase, 0, Find_Row, Find_Col, Find_Angle, out IParamsFindModel.m_HtuMatID);
                    HOperatorSet.AffineTransRegion(IParamsFindModel.m_ho_OutRegion, out IParamsFindModel.m_ho_OutRegion, IParamsFindModel.m_HtuMatID, "nearest_neighbor");
                    if (!IParamsFindModel.m_bNotOffsetX)
                    {
                        IParamsFindModel.m_fOffsetX = (Find_Col - createModelParam.m_fCenterColBase);
                    }
                    else
                    {
                        IParamsFindModel.m_fOffsetX = -(Find_Col - createModelParam.m_fCenterColBase);
                    }

                    if (!IParamsFindModel.m_bNotOffsetY)
                    {
                        IParamsFindModel.m_fOffsetY= (Find_Row - createModelParam.m_fCenterRowBase);
                    }
                    else
                    {
                        IParamsFindModel.m_fOffsetY = -(Find_Row - createModelParam.m_fCenterRowBase);
                    }
                    if (!IParamsFindModel.m_bNotOffsetR)
                    {
                        IParamsFindModel.m_fOffsetR = (Find_Angle - 0);
                    }
                    else
                    {
                        IParamsFindModel.m_fOffsetR = -(Find_Angle - 0);
                    }

                    IParamsFindModel.m_fGoalX = Find_Col;
                    IParamsFindModel.m_fGoalY = Find_Row;
                    IParamsFindModel.m_fGoaR = Find_Angle;

                    return true;

                }
                else
                {
                    IParamsFindModel.m_fScoreMatch = 0;
                    HOperatorSet.GenRectangle2(out IParamsFindModel.m_ho_OutRegion, 10, 10, 0, 2, 2);
                    return false;
                }
              
            }
            catch (Exception ee)
            {
                Log4netRecord.ErrorLog.Debug("找模板插件执行失败：" + ee.ToString());
                return false;
            }


        }


        public bool ExecuteObj()
        {
            ParamsFindModel IParamsFindModel = (ParamsFindModel)PluginParams;
            IParamsFindModel.m_strGoalName = "None";
            try
            {
                IParamsFindModel.m_ho_OutImgShow?.Dispose();
                IParamsFindModel.m_ho_OutCircleRegion?.Dispose();
                IParamsFindModel.m_ho_OutRegion?.Dispose();
                IParamsFindModel.m_ho_OutCropRegion?.Dispose();


                HTuple Find_Row = 0, Find_Col = 0, Find_Angle = 0, Find_Score = 0;
                HOperatorSet.GetImageSize(IParamsFindModel.ho_Image, out HTuple width, out HTuple height);
                if (IParamsFindModel.m_fEdgeFindCol >= width)
                {
                    HOperatorSet.GenRectangle1(out IParamsFindModel.m_ho_OutCropRegion, 0, 0, height, width);
                    HOperatorSet.ReduceDomain(IParamsFindModel.ho_Image, IParamsFindModel.m_ho_OutCropRegion, out IParamsFindModel.m_ho_OutImgShow);
                }
                else
                {
                    if (IParamsFindModel.m_fEdgeFindEndCol > IParamsFindModel.m_fEdgeFindCol && IParamsFindModel.m_fEdgeFindEndCol < width)
                    {
                        HOperatorSet.GenRectangle1(out IParamsFindModel.m_ho_OutCropRegion, IParamsFindModel.m_fEdgeFindRow, IParamsFindModel.m_fEdgeFindCol, IParamsFindModel.m_fEdgeFindEndRow, IParamsFindModel.m_fEdgeFindEndCol);
                    }
                    else
                    {
                        HOperatorSet.GenRectangle1(out IParamsFindModel.m_ho_OutCropRegion, 0, IParamsFindModel.m_fEdgeFindCol, height, width);
                    }

                    HOperatorSet.ReduceDomain(IParamsFindModel.ho_Image, IParamsFindModel.m_ho_OutCropRegion, out IParamsFindModel.m_ho_OutImgShow);
                }
                try
                {

                    int IDum = IParamsFindModel.m_lFindParams.Count;
                    for (int i = 0; i < IDum; i++)
                    {

                        try
                        {
                            HOperatorSet.SetShapeModelParam(IParamsFindModel.m_lFindParams[i].ht_ModelID, "timeout", IParamsFindModel.m_nOverTime);
                            MatclTool.FindModelMethor(IParamsFindModel.m_lFindParams[i], IParamsFindModel.m_lFindParams[i].ht_ModelID, IParamsFindModel.m_ho_OutImgShow, out Find_Row, out Find_Col, out Find_Angle, out Find_Score);

                            if (Find_Score?.Length > 0)
                            {
                                IParamsFindModel.m_fScoreMatch = Find_Score;
                                IParamsFindModel.m_strGoalName = IParamsFindModel.m_lFindParams[i].m_strModelName;
                                break;
                            }
                        }
                        catch (Exception ee)
                        {
                            Log4netRecord.ErrorLog.Debug($"找模板插件执行失败：{IParamsFindModel.m_lFindParams[i].m_strModelName}__" + ee.ToString());
                            return false;
                        }

                      

                    }

                }
                catch (Exception ee)
                {
                    IParamsFindModel.m_fScoreMatch = 0;
                    HOperatorSet.GenRectangle2(out IParamsFindModel.m_ho_OutRegion, 10, 10, 0, 2, 2);
                    Log4netRecord.ErrorLog.Debug("找模板插件执行失败：" + ee.ToString());
                    return false;
                }

                if (Find_Col?.Length != 0)
                {
                    // HOperatorSet.GenRectangle2(out IParamsFindModel.m_ho_OutRegion, Find_Row, Find_Col, Find_Angle, IParamsFindModel.m_fCenterLength_X, IParamsFindModel.m_fCenterLength_Y);
                    CreateModelParam createModelParam = IParamsFindModel.m_lCreateParams.Find(x => x.m_strModelName == IParamsFindModel.m_strGoalName);
                    HOperatorSet.GenRectangle2(out IParamsFindModel.m_ho_OutRegion, createModelParam.m_fCenterRowBase, createModelParam.m_fCenterColBase, 0, createModelParam.m_fCenterLength_XBase, createModelParam.m_fCenterLength_YBase);
                    HOperatorSet.VectorAngleToRigid(createModelParam.m_fCenterRowBase, createModelParam.m_fCenterColBase, 0, Find_Row, Find_Col, Find_Angle, out IParamsFindModel.m_HtuMatID);
                    HOperatorSet.AffineTransRegion(IParamsFindModel.m_ho_OutRegion, out IParamsFindModel.m_ho_OutRegion, IParamsFindModel.m_HtuMatID, "nearest_neighbor");
                    //HOperatorSet.VectorAngleToRigid(0, 0, 0, Find_Row, Find_Col, Find_Angle, out IParamsFindModel.m_HtuMatID);
                    HOperatorSet.AreaCenter(IParamsFindModel.m_ho_OutRegion,out HTuple area,out Find_Row,out Find_Col);
                    //HOperatorSet.VectorAngleToRigid(createModelParam.m_fCenterRowBase, createModelParam.m_fCenterColBase, 0, Find_Row, Find_Col, Find_Angle, out IParamsFindModel.m_HtuMatID);
                    IParamsFindModel.m_fGoalX = Find_Col;
                    IParamsFindModel.m_fGoalY = Find_Row;
                    IParamsFindModel.m_fGoaR = Find_Angle;

                    if (!IParamsFindModel.m_bNotOffsetX)
                    {
                        IParamsFindModel.m_fOffsetX = (Find_Col - createModelParam.m_fCenterColBase);
                    }
                    else
                    {
                        IParamsFindModel.m_fOffsetX = -(Find_Col - createModelParam.m_fCenterColBase);
                    }

                    if (!IParamsFindModel.m_bNotOffsetY)
                    {
                        IParamsFindModel.m_fOffsetY = (Find_Row - createModelParam.m_fCenterRowBase);
                    }
                    else
                    {
                        IParamsFindModel.m_fOffsetY = -(Find_Row - createModelParam.m_fCenterRowBase);
                    }
                    if (!IParamsFindModel.m_bNotOffsetR)
                    {
                        IParamsFindModel.m_fOffsetR = (Find_Angle - 0);
                    }
                    else
                    {
                        IParamsFindModel.m_fOffsetR = -(Find_Angle - 0);
                    }
                    return true;

                }
                else
                {
                    IParamsFindModel.m_fScoreMatch = 0;
                    HOperatorSet.GenRectangle2(out IParamsFindModel.m_ho_OutRegion, 10, 10, 0, 2, 2);
                    return false;
                }

            }
            catch (Exception ee)
            {
                IParamsFindModel.m_fScoreMatch = 0;
                HOperatorSet.GenRectangle2(out IParamsFindModel.m_ho_OutRegion, 10, 10, 0, 2, 2);
                Log4netRecord.ErrorLog.Debug("找模板插件执行失败：" + ee.ToString());
                return false;
                //throw;
            }





        }

        public void GetResultParams(string varName, object obj)
        {
            //throw new NotImplementedException();
        }

        public void IniObj()
        {
            ParamsFindModel IParamsFindModel = (ParamsFindModel)PluginParams;
            //默认路径
            string filePath = Project.Instance.paramPathSet.ProcessPath;
            filePath=filePath.Replace("Process", "");
            filePath = filePath + moduleInfo.ProcessName;
            filePath = filePath + "\\NgMark";
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            };

            IParamsFindModel.m_strModelGroupPath = filePath;
            IParamsFindModel.Save();
            try
            {


                if (MatclTool.m_dicOptimization_Create.Count == 0)
                {
                    MatclTool.m_dicOptimization_Create.Add(0, "auto");
                    MatclTool.m_dicOptimization_Create.Add(1, "no_pregeneration");
                    MatclTool.m_dicOptimization_Create.Add(2, "none");
                    MatclTool.m_dicOptimization_Create.Add(3, "point_reduction_high");
                    MatclTool.m_dicOptimization_Create.Add(4, "point_reduction_low");
                    MatclTool.m_dicOptimization_Create.Add(5, "point_reduction_medium");
                    MatclTool.m_dicOptimization_Create.Add(6, "pregeneration");
                }

                if (MatclTool.m_dicMetric_Create.Count==0)
                {
                    MatclTool.m_dicMetric_Create.Add(0, "use_polarity");
                    MatclTool.m_dicMetric_Create.Add(1, "ignore_color_polarity");
                    MatclTool.m_dicMetric_Create.Add(2, "ignore_global_polarity");
                    MatclTool.m_dicMetric_Create.Add(3, "ignore_local_polarity");
                }
              

                if (MatclTool.m_dicSubPixel_FindModel.Count == 0)
                {
                    MatclTool.m_dicSubPixel_FindModel.Add(0, "none");
                    MatclTool.m_dicSubPixel_FindModel.Add(1, "interpolation");
                    MatclTool.m_dicSubPixel_FindModel.Add(2, "least_squares");
                    MatclTool.m_dicSubPixel_FindModel.Add(3, "least_squares_high");
                    MatclTool.m_dicSubPixel_FindModel.Add(4, "least_squares_very_high");
                    MatclTool.m_dicSubPixel_FindModel.Add(5, "max_deformation 1");
                    MatclTool.m_dicSubPixel_FindModel.Add(6, "max_deformation 2");
                    MatclTool.m_dicSubPixel_FindModel.Add(7, "max_deformation 3");
                    MatclTool.m_dicSubPixel_FindModel.Add(8, "max_deformation 4");
                    MatclTool.m_dicSubPixel_FindModel.Add(9, "max_deformation 5");
                    MatclTool.m_dicSubPixel_FindModel.Add(10, "max_deformation 6");
                }

                CreateModel();

                ExecuteObj();


            }
            catch (Exception ee)
            {

                string[] files = Directory.GetFiles(IParamsFindModel.m_strModelGroupPath, "*.bmp");
                //如果大于0，删除掉所有文件
                if (files.Length > 0)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        File.Delete(files[i]);
                    }
                }

                IParamsFindModel.m_lCreateParams.Clear();
                IParamsFindModel.m_lFindParams.Clear();
                IParamsFindModel.m_lCreateParams.Add(new CreateModelParam() { m_strModelName = "test" });
                IParamsFindModel.m_lFindParams.Add(new FindModelParam() { m_strModelName = "test" });
                HOperatorSet.GenImageGrayRamp(out m_hoModelImage, 1, 1, 128, 256, 256, 512, 512);
                string fileName = IParamsFindModel.m_strModelGroupPath + "\\test.bmp";
                HOperatorSet.WriteImage(m_hoModelImage, "bmp", 0, fileName);
            


                HOperatorSet.TupleRad(0, out HTuple m_nAngleStart_Create);
                HOperatorSet.TupleRad(360, out HTuple m_nAngleExtent_Create);
                HOperatorSet.GenImageGrayRamp(out m_hoModelImage, 1, 1, 128, 256, 256, 512, 512);
                HOperatorSet.GenRectangle2(out m_hoRegion, 0, 0, 0, 10, 10);
                HOperatorSet.CreateShapeModel(m_hoModelImage, 1, m_nAngleStart_Create, m_nAngleExtent_Create,
                   "auto", "auto", "use_polarity",
                   "auto", "auto", out IParamsFindModel.m_lFindParams.Find(p => p.m_strModelName == "test").ht_ModelID);

                ExecuteObj();
                Log4netRecord.ErrorLog.Debug("找模板插件初始化失败：" + ee.ToString());

            }

        }

        public bool CreateModel()
        {
            try
            {
                m_hoRegion?.Dispose();
                ParamsFindModel IParamsFindModel = (ParamsFindModel)PluginParams;
                //这段是用于防呆使用
                if (IParamsFindModel.m_lCreateParams.Count > 0)
                {
                    //预先遍历文件夹的图片名称跟参数文件m_lCreateParams的对应关系，没有找到对应的图片文件，则创建
                    string[] files = Directory.GetFiles(IParamsFindModel.m_strModelGroupPath, "*.bmp");

                    if (files.Length != IParamsFindModel.m_lCreateParams.Count)
                    {
                        if (files.Length > IParamsFindModel.m_lCreateParams.Count)
                        {
                            for (int i = 0; i < IParamsFindModel.m_lCreateParams.Count; i++)
                            {
                                string fileName = IParamsFindModel.m_strModelGroupPath+"\\"+ IParamsFindModel.m_lCreateParams[i].m_strModelName + ".bmp";
                                if (!File.Exists(fileName))
                                {
                                    IParamsFindModel.m_lCreateParams.RemoveAt(i);
                                    IParamsFindModel.m_lFindParams.RemoveAt(i);
                                    i--;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < files.Length; i++)
                            {
                                string fileName = Path.GetFileNameWithoutExtension(files[i]);
                                if (IParamsFindModel.m_lCreateParams.Find(p => p.m_strModelName == fileName) == null)
                                {
                                    IParamsFindModel.m_lCreateParams.Add(new CreateModelParam() { m_strModelName = fileName });
                                    IParamsFindModel.m_lFindParams.Add(new FindModelParam() { m_strModelName = fileName });
                                }
                            }
                        }
                    }

                    IParamsFindModel.Save();
                }
                else
                {

                    string[] files = Directory.GetFiles(IParamsFindModel.m_strModelGroupPath, "*.bmp");
                    //如果大于0，删除掉所有文件
                    if (files.Length > 0)
                    {

                        for (int i = 0; i < files.Length; i++)
                        {
                            string fileName = Path.GetFileNameWithoutExtension(files[i]);
                            IParamsFindModel.m_lCreateParams.Add(new CreateModelParam() { m_strModelName = fileName });
                            IParamsFindModel.m_lFindParams.Add(new FindModelParam() { m_strModelName = fileName });
                        }
                    
                    }
                    else
                    {
                        //重新生成新的参数文件，添加一个即可，并创建图片文件

                        IParamsFindModel.m_lCreateParams.Clear();
                        IParamsFindModel.m_lFindParams.Clear();
                        IParamsFindModel.m_lCreateParams.Add(new CreateModelParam() { m_strModelName = "test" });
                        IParamsFindModel.m_lFindParams.Add(new FindModelParam() { m_strModelName = "test" });
                        HOperatorSet.GenImageGrayRamp(out m_hoModelImage, 1, 1, 128, 256, 256, 512, 512);
                        string fileName = IParamsFindModel.m_strModelGroupPath + "\\test.bmp";
                        HOperatorSet.WriteImage(m_hoModelImage, "bmp", 0, fileName);
                    }

                
                }

                HOperatorSet.GenEmptyObj(out m_hoModelImage);

                HOperatorSet.GenRectangle2(out m_hoRegion, IParamsFindModel.m_fCenterRow, IParamsFindModel.m_fCenterCol, IParamsFindModel.m_fCenterAngle, IParamsFindModel.m_fCenterLength_X, IParamsFindModel.m_fCenterLength_Y);
                List<string> imagePaths = GetBmpFilePaths(IParamsFindModel.m_strModelGroupPath);
                IParamsFindModel.m_ho_OutRegion?.Dispose();
                IParamsFindModel.m_ho_OutCircleRegion?.Dispose();
                m_hoRegion?.Dispose();
                HOperatorSet.GenEmptyObj(out IParamsFindModel.m_ho_OutRegion);
                HOperatorSet.GenEmptyObj(out IParamsFindModel.m_ho_OutCircleRegion);

                foreach (var imagePath in imagePaths)
                {

                    HOperatorSet.ReadImage(out m_hoModelImage, imagePath);
                    //获取文件名称，与参数文件m_lCreateParams的对应关系，进行参数对应的操作
                    string fileName = Path.GetFileNameWithoutExtension(imagePath);
                    CreateModelParam createModelParam = IParamsFindModel.m_lCreateParams.Find(p => p.m_strModelName == fileName);
                    FindModelParam findModelParam = IParamsFindModel.m_lFindParams.Find(p => p.m_strModelName == fileName);
                    if (createModelParam == null)
                    {
                        createModelParam = new CreateModelParam() { m_strModelName = fileName };
                        IParamsFindModel.m_lCreateParams.Add(createModelParam);
                    }
                    if (findModelParam == null)
                    {
                        findModelParam = new FindModelParam() { m_strModelName = fileName };
                        IParamsFindModel.m_lFindParams.Add(findModelParam);
                    }



                    MatclTool.CreatModelMethor(createModelParam, m_hoModelImage, out findModelParam.ht_ModelID);

                }
                IParamsFindModel.Save();
                return true;
            }
            catch (Exception ee)
            {
                Log4netRecord.ErrorLog.Debug("找模板插件创建模板失败：" + ee.ToString());
                return false;

                // throw;
            }
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
            //throw new NotImplementedException();
        }

        private List<string> GetBmpFilePaths(string folderPath)
        {
            // 确保提供的路径存在
            if (!Directory.Exists(folderPath))
            {
                throw new DirectoryNotFoundException($"The directory {folderPath} was not found.");
            }

            // 搜索文件夹中的所有bmp文件
            string[] files = Directory.GetFiles(folderPath, "*.bmp", SearchOption.AllDirectories);

            // 将结果转换为列表并返回
            return new List<string>(files);
        }
    }
}
