using ClassifyCPU_onnx_test;
using EHolly;
using Extensions;
using Frame_Core;
using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Plugin.MQDefects
{
    /// <summary>
    /// 图像处理算法
    /// </summary>
    public class CoatingDefectsAlog
    {

        /// <summary>
        /// 输出的缺陷队列
        /// </summary>
        private Queue<NgTypeInfo> detectInfos = new Queue<NgTypeInfo>();

        /// <summary>
        /// 构建极耳区域
        /// </summary>
        public Detcet_JR build_JR_detect = new Detcet_JR();


        /// <summary>
        /// 分类器对象
        /// </summary>
        private ClassificationClient ClassDefect;

        /// <summary>
        /// 给分类器准备的数组
        /// </summary>
        private ClassificationDefectInfo[] DefectInfos = new ClassificationDefectInfo[31];
        /// <summary>
        /// 当前图像涂布区疑似缺陷个数
        /// </summary>
        private int NgImagePart_Count = 0;
        /// <summary>
        /// 存放给分类器的图片
        /// </summary>
        private List<HObject> ho_Images = new List<HObject>();




        /// <summary>
        /// 横向标定
        /// </summary>
        private double WidthCali = 0.091496;
        /// <summary>
        /// 纵向标定
        /// </summary>
        private double HeightCali = 0.15072;
        /// <summary>
        /// 面积标定
        /// </summary>
        private double AreaCali = 0.003;

        /// <summary>
        /// 图像宽度
        /// </summary>
        private int ImageWidth = 8192;
        /// <summary>
        /// 图像高度
        /// </summary>
        private int ImageHeight = 10000;
        



        /// <summary>
        /// 图像处理主体 检测区域
        /// </summary>
        /// <param name="Image"></param>
        public void Run(HObject Image,HObject DetectionROI , HObject ShieldROI, ParamsMQDefects MQDefects,
            ClassificationClient inputClassDefect, object[] inputs, out object[] outputs)
        {
            HTuple imageWidth = 0; HTuple imageHeight = 0;
            outputs = new object[2];
            try
            {

                //初始化 释放以前图像
                foreach (NgTypeInfo info in detectInfos)
                {
                    info.m_hImage?.Dispose();
                    info.m_hRegion?.Dispose();
                }
                detectInfos.Clear();

                HOperatorSet.Union1(DetectionROI, out DetectionROI);
                HOperatorSet.Union1(ShieldROI, out ShieldROI);

                WidthCali = MQDefects.WidthCali;
                HeightCali= MQDefects.HeightCali;
                AreaCali = WidthCali * HeightCali;
                HOperatorSet.GetImageSize(Image, out imageWidth, out imageHeight);
                ImageWidth = imageWidth.I;
                ImageHeight = imageHeight.I;
                ClassDefect = inputClassDefect;
                //检测方法
                int DetectionType = MQDefects.com_DetectionMethod;
                

                switch (DetectionType)
                {
                    //膜区检测
                    case 0:      
                        coatingDefects(Image, DetectionROI, ShieldROI, MQDefects);
                        if (MQDefects.cb_OpenDec_WLY)
                        {
                            //未冷压检测
                            WLY(Image, DetectionROI, ShieldROI, MQDefects);
                        }
                        break;
                    //直边破损
                    case 1:
                        ZBPS(Image, DetectionROI, ShieldROI, MQDefects);
                        break;
                    //边缘掉料
                    case 2:
                        BYDL(Image, DetectionROI, ShieldROI, MQDefects);
                        break;
                    //直边余料
                    case 3:
                        ZBYuLiao(Image, DetectionROI, ShieldROI, MQDefects);
                        break;
                    //AT9检测
                    case 4:
                        Detection_AT9(Image, DetectionROI, ShieldROI, MQDefects);
                        break;
                    //极耳背光
                    case 5:
                        Detection_JR(Image, DetectionROI, MQDefects);
                        break;

                }
                MQDefects.NgString = "";
                HOperatorSet.GenEmptyObj(out MQDefects.NgRegion);
                foreach (NgTypeInfo info in detectInfos)
                {
                    MQDefects.NgString += info.m_strNgTypeName.ToString() + ",";
                    HOperatorSet.Union2(MQDefects.NgRegion, info.m_hRegion,
                        out MQDefects.NgRegion);   
                }
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Fatal(ex.Message);
                //System.Windows.Forms.MessageBox.Show(ex.Message);
                
            }
            finally
            {
                outputs[0] = detectInfos;
            }
        }


        /// <summary>
        /// 检测极耳
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="MQDefects"></param>
        private void Detection_JR(HObject Image,HObject DetectionROI, ParamsMQDefects MQDefects)
        {
            HObject ModleXld;
            HOperatorSet.GenEmptyObj(out ModleXld);
            //极耳带料
            HObject JRDaiLiaoXldRegion;
            HOperatorSet.GenEmptyObj(out JRDaiLiaoXldRegion);

            //极耳破损
            HObject JRPoSunRegion;
            HOperatorSet.GenEmptyObj(out JRPoSunRegion);

            HObject Domain_Image;
            HOperatorSet.GenEmptyObj(out Domain_Image);
            MQDefects.JRDaiLiaoXldRegion?.Dispose();
            MQDefects.JRPoSunRegion?.Dispose();
            HOperatorSet.GenEmptyObj(out MQDefects.JRDaiLiaoXldRegion);
            HOperatorSet.GenEmptyObj(out MQDefects.JRPoSunRegion);


           
            //极耳底部R角部分
            HObject bottomAngleR_region;
            HOperatorSet.GenEmptyObj(out bottomAngleR_region);
            //极耳斜边部分
            HObject hypotenuse_region;
            HOperatorSet.GenEmptyObj(out hypotenuse_region);
            //极耳顶部R角部分
            HObject topAngleR_region;
            HOperatorSet.GenEmptyObj(out topAngleR_region);



            HObject Detect_remnants_region;
            HOperatorSet.GenEmptyObj(out Detect_remnants_region);
            HObject Detect_brokens_region;
            HOperatorSet.GenEmptyObj(out Detect_brokens_region);
            HOperatorSet.ReduceDomain(Image, DetectionROI, out Domain_Image);


            #region 2024.4.16  加入精定位极耳中心
            HObject JR_Top_Rectangle;
            HOperatorSet.GenEmptyObj(out JR_Top_Rectangle);
            HObject JR_Down_Rectangle;
            HOperatorSet.GenEmptyObj(out JR_Down_Rectangle);
            HObject JR_Top_Line;
            HOperatorSet.GenEmptyObj(out JR_Top_Line);
            HObject JR_Down_Line;
            HOperatorSet.GenEmptyObj(out JR_Down_Line);    
            #endregion
            try
            {
               
 
                int type = 2;
                if(type ==1)
                {
                    //查找模板
                    var isok = FindShapeModel(Domain_Image, MQDefects, MQDefects.Model, MQDefects.ModelContours, out ModleXld);
                    MQDefects.ModleXld = ModleXld.CopyObj(1, 1);
                    if (isok == 0)
                    {
                        //极耳带料
                        Find_JR_DaiLiao(Domain_Image, ModleXld, MQDefects, out JRDaiLiaoXldRegion);
                        MQDefects.JRDaiLiaoXldRegion = JRDaiLiaoXldRegion.CopyObj(1, 1);
                        //极耳破损
                        Find_JR_PoSun(Domain_Image, ModleXld, MQDefects, out JRPoSunRegion);
                        MQDefects.JRPoSunRegion = JRPoSunRegion.CopyObj(1, 1);
                    }
                    else
                    {
                        NgTypeInfo iNgInfo = new NgTypeInfo();
                        iNgInfo.m_hImage = Domain_Image.CopyObj(1, 1);//缺陷小图片
                        iNgInfo.m_hRegion = JRPoSunRegion.CopyObj(1, 1);
                        iNgInfo.m_strNgTypeName = Defct1Enum.极耳破损;//缺陷位置类型
                        iNgInfo.m_fNgTypArea = 99999;//缺陷面积
                        iNgInfo.m_fNgTypeAbsPixelPos = 1;//Y坐标
                        iNgInfo.m_fNgTypHeight = 99999;//缺陷高度\长度
                        iNgInfo.m_fNgTypWidth = 99999;//缺陷宽度
                        iNgInfo.sAttachInfo = "";//缺陷附加信息
                        string NGType = "极耳破损";
                        bool isExStd = ImageShow(iNgInfo, NGType);
                        switch (isok)
                        {
                            case -1:
                                MessageBox.Show("请先创建模板");

                                break;

                            case -2:


                                break;

                            case -3:


                                break;
                        }

                    }
                }
                else
                {
                    //是否左极耳
                    bool Is_Left =false;
                    //极耳中心Row行
                    HTuple JRCenter_Row = MQDefects.num_JRCenter_Row;
                    //极耳中心column列
                    HTuple JRCenter_column = MQDefects.num_JRCenter_column;

                    #region 2024.4.16 生成找极耳上下的卡尺
                    //找极耳上
                    FindLineParameter findLineParameter_JR_Top = new FindLineParameter
                    {
                        inputXorY = "X",
                        inputInterval = 10,
                        inputDirection = ExtensionsObject.GetEnumValueAtIndex<Direction>
                        (2).ToString(),
                        inputRoiWidthLen = 10,
                        inputSigma = 3,
                        inputAmplitudeThreshold = 30,

                        inputTransition = ExtensionsObject.GetEnumValueAtIndex<Transition>
                        (1).ToString(),
                        inputPosition = ExtensionsObject.GetEnumValueAtIndex<Position>
                        (0).ToString(),
                    };
                    //找极耳下
                    FindLineParameter findLineParameter_JR_Down = new FindLineParameter
                    {
                        inputXorY = "X",
                        inputInterval = 10,
                        inputDirection = ExtensionsObject.GetEnumValueAtIndex<Direction>
                        (3).ToString(),
                        inputRoiWidthLen = 10,
                        inputSigma = 3,
                        inputAmplitudeThreshold = 30,

                        inputTransition = ExtensionsObject.GetEnumValueAtIndex<Transition>
                        (1).ToString(),
                        inputPosition = ExtensionsObject.GetEnumValueAtIndex<Position>
                        (0).ToString(),
                    };
                  
                    //上下,使用顶点Y坐标减去固定值
                    double JR_Top_Rectangle_Y= MQDefects.num_JRCenter_Row - MQDefects.num_JR_width / HeightCali / 2;
                    double JR_Top_Rectangle_X = MQDefects.num_JRCenter_column + (MQDefects.num_bottomAngleR_ridus+MQDefects.num_hypotenuse_length) / WidthCali/2;

                    HOperatorSet.GenRectangle2(out JR_Top_Rectangle, JR_Top_Rectangle_Y, JR_Top_Rectangle_X
                            , 0,80,300);
                    double JR_Down_Rectangle_Y = MQDefects.num_JRCenter_Row + MQDefects.num_JR_width / HeightCali / 2;
                    double JR_Down_Rectangle_X = MQDefects.num_JRCenter_column + (MQDefects.num_bottomAngleR_ridus + MQDefects.num_hypotenuse_length) / WidthCali / 2;
                    HOperatorSet.GenRectangle2(out JR_Down_Rectangle, JR_Down_Rectangle_Y, JR_Down_Rectangle_X
                            , 0, 80, 300);
                    Line JR_Top_Line_Point = new Line();
                    Line JR_Down_Line_Point = new Line();
                    try
                    { 
                        GetTuBu(Image, JR_Top_Rectangle, findLineParameter_JR_Top, out JR_Top_Line, out JR_Top_Line_Point);
                        GetTuBu(Image, JR_Down_Rectangle, findLineParameter_JR_Down, out JR_Down_Line, out JR_Down_Line_Point);
                    }
                    catch
                    {
                        JR_Top_Line_Point.state = false;
                        JR_Down_Line_Point.state = false;
                    }

                    //重新对极耳中心Y赋值,当找直线成功时
                    if (JR_Top_Line_Point.state&& JR_Down_Line_Point.state)
                    {
                        JRCenter_Row = (JR_Top_Line_Point.RowBegin + JR_Down_Line_Point.RowBegin) / 2;
                    }
                    #endregion
                    //极耳宽度
                    HTuple JRWidth =MQDefects.num_JR_width/ HeightCali;
                    //底部R角半径
                    HTuple bottomAngleR_ridus = MQDefects.num_bottomAngleR_ridus/ HeightCali;
                    //顶部R角半径
                    HTuple topAngleR_ridus = MQDefects.num_topAngleR_ridus/ HeightCali;

                    //底部R角补偿角度（0 - 90）
                    HTuple bottomAngleR_CompensationAngle = MQDefects.num_bottomAngleR_CompensationAngle;
                    //顶部R角补偿角度（0 - 90）
                    HTuple topAngleR_CompensationAngle = MQDefects.num_topAngleR_CompensationAngle;
                    //斜边长度
                    HTuple hypotenuse_length = MQDefects.num_hypotenuse_length/ WidthCali;
                    //斜边角度
                    HTuple hypotenuse_angle = MQDefects.num_hypotenuse_angle;
                    string str = build_JR_detect.Build_Standard_JR(Is_Left, JRCenter_Row, JRCenter_column, JRWidth,
                                                                 bottomAngleR_ridus, topAngleR_ridus, bottomAngleR_CompensationAngle, 
                                                                 topAngleR_CompensationAngle, hypotenuse_length, hypotenuse_angle,
                                                                  out bottomAngleR_region, out hypotenuse_region, out topAngleR_region);
                    double hStandard_JRH_Pix = hypotenuse_length;//标准极耳高度 pix
                    double hStandard_JRW_Pix = JRWidth;//标准极耳宽度pix

                    //默认检余料、破损区域
                    HOperatorSet.GenRectangle1(out Detect_remnants_region, JRCenter_Row - hStandard_JRW_Pix / 2 - 100, JRCenter_column - hStandard_JRH_Pix, JRCenter_Row + hStandard_JRW_Pix / 2 + 100, JRCenter_column + hStandard_JRH_Pix);
                    HOperatorSet.GenRectangle1(out Detect_brokens_region, JRCenter_Row - hStandard_JRW_Pix / 2 - 100, JRCenter_column - hStandard_JRH_Pix, JRCenter_Row + hStandard_JRW_Pix / 2 + 100, JRCenter_column + hStandard_JRH_Pix);



                    //标准极耳余料检测
                    HTuple bottomAngleR_Shielded_X = MQDefects.num_bottomAngleR_Shielded_X / WidthCali;
                    HTuple bottomAngleR_Shielded_Y = MQDefects.num_bottomAngleR_Shielded_Y / HeightCali;
                    HTuple hypotenuse_Shielded_Y = MQDefects.num_hypotenuse_Shielded_Y / HeightCali;
                    HTuple topAngleR_Shielded_X = MQDefects.num_topAngleR_Shielded_X / WidthCali;
                    HTuple topAngleR_Shielded_Y = MQDefects.num_topAngleR_Shielded_Y / HeightCali;

                    //标准极耳破损检测
                    HTuple bottomAngleR_Shielded_X1 = MQDefects.num_bottomAngleR_Shielded_X1 / WidthCali;
                    HTuple bottomAngleR_Shielded_Y1 = -MQDefects.num_bottomAngleR_Shielded_Y1 / HeightCali;
                    HTuple hypotenuse_Shielded_Y1 = -MQDefects.num_hypotenuse_Shielded_Y1 / HeightCali;
                    HTuple topAngleR_Shielded_X1 = MQDefects.num_topAngleR_Shielded_X1 / WidthCali;
                    HTuple topAngleR_Shielded_Y1 = -MQDefects.num_topAngleR_Shielded_Y1 / HeightCali;



                    if (str == "标准极耳构建已完成")
                     {
                         Detect_remnants_region.Dispose();
                         build_JR_detect.Detect_remnants(bottomAngleR_region, hypotenuse_region, topAngleR_region, Is_Left, 0.3 * hStandard_JRW_Pix, 0.7 * hStandard_JRH_Pix,
                                                          bottomAngleR_Shielded_X, bottomAngleR_Shielded_Y, hypotenuse_Shielded_Y, topAngleR_Shielded_X, topAngleR_Shielded_Y,
                                                          out Detect_remnants_region);

                         Detect_brokens_region.Dispose();
                         build_JR_detect.Detect_brokens(bottomAngleR_region, hypotenuse_region, topAngleR_region, Is_Left, 0.3 * hStandard_JRW_Pix, 0.3 * hStandard_JRH_Pix,
                                                        bottomAngleR_Shielded_X1, bottomAngleR_Shielded_Y1, hypotenuse_Shielded_Y1, topAngleR_Shielded_X1, topAngleR_Shielded_Y1,
                                                               out Detect_brokens_region);
                     }

                 
                    MQDefects.JRDaiLiaoXldRegion = Detect_remnants_region.CopyObj(1, 1);
                    MQDefects.JRPoSunRegion = Detect_brokens_region.CopyObj(1, 1);
                    //极耳带料
                    Find_JR_DaiLiao(Image, MQDefects.JRDaiLiaoXldRegion, MQDefects);
                    //极耳破损
                    Find_JR_PoSun(Image, MQDefects.JRPoSunRegion, MQDefects);
   
                }


             

            }
            catch(Exception ex)
            {
                Log4netRecord.ErrorLog.Error(ex.Message);
            }
            finally
            {
                ModleXld?.Dispose();
                JRPoSunRegion?.Dispose();
                JRDaiLiaoXldRegion?.Dispose();
                Domain_Image?.Dispose();
                bottomAngleR_region?.Dispose();
                hypotenuse_region?.Dispose();
                topAngleR_region?.Dispose();
                Detect_remnants_region?.Dispose();
                Detect_brokens_region?.Dispose();
                JR_Top_Rectangle?.Dispose();
                JR_Down_Rectangle?.Dispose();
                JR_Top_Line?.Dispose();
                JR_Down_Line?.Dispose();
            }  
        }

        /// <summary>
        /// 获得涂布边线
        /// </summary>
        /// <param name="inputImage">输入图像</param>
        /// <param name="inputRegion">输入区域</param>
        /// <param name="inputFindLineParameter">卡尺参数</param>
        /// <param name="outputLengthenLine">输出延长线</param>
        /// <returns></returns>

        public bool GetTuBu(HObject inputImage, HObject inputRegion,
            FindLineParameter inputFindLineParameter, out HObject outputLengthenLine, out Line line)
        {
            lock(this)
            {
                line = new Line();
                HObject outputContour = null;
                HOperatorSet.GenEmptyObj(out outputContour);
                //outputLengthenLine = null;
                HOperatorSet.GenEmptyObj(out outputLengthenLine);
                HTuple OutPointRow;
                HTuple OutPointCol;
                //2024.3.11 增加判断是否可以拟合直
                bool isFind_Line = false;


                HTuple tempT1 = 0, tempT2 = 0, tempT3 = 0, tempT4 = 0, tempT5 = 0, tempT6 = 0;
                HTuple t1 = 0, t2 = 0, t3 = 0, t4 = 0, t5 = 0;

                try
                {

                    HOperatorSet.CountSeconds(out tempT1);


                    PublicAlogClass.Find_Line_ForRectangle(inputImage, inputRegion, out outputContour, inputFindLineParameter.inputXorY,
                        inputFindLineParameter.inputInterval, inputFindLineParameter.inputDirection, inputFindLineParameter.inputRoiWidthLen,
                        inputFindLineParameter.inputSigma, inputFindLineParameter.inputAmplitudeThreshold, inputFindLineParameter.inputTransition,
                        inputFindLineParameter.inputPosition, out OutPointRow, out OutPointCol);
                    HOperatorSet.CountSeconds(out tempT2);
                    t1 = (tempT2 - tempT1) * 1000;

                    HTuple Count = 0;
                    HOperatorSet.CountObj(outputContour, out Count);
                    #region  1.判断一下有无轮廓,没有的话就用 输出的点 
                    if (Count < 1)
                    {
                        if (OutPointRow.Length > 5)
                        {
                            HOperatorSet.GenContourPolygonXld(out outputContour, OutPointRow, OutPointCol);
                            isFind_Line = true;
                        }
                        else
                        {
                            //没有找到边
                            isFind_Line = false;
                        }
                    }
                    else
                    {

                        isFind_Line = true;
                    }
                    HTuple RowBegin, ColBegin, RowEnd, ColEnd, Nr, Nc, Dist;
                    if (isFind_Line)
                    {
                        HOperatorSet.FitLineContourXld(outputContour, "tukey", -1, 1, 5, 2,
                        out RowBegin, out ColBegin, out RowEnd, out ColEnd, out Nr, out Nc, out Dist);
                        if (RowBegin.Length > 0)
                        {
                            line.RowBegin = RowBegin[0].D;
                            line.ColBegin = ColBegin[0].D;
                            line.RowEnd = RowEnd[0].D;
                            line.ColEnd = ColEnd[0].D;
                            line.state = true;
                            HOperatorSet.GenContourPolygonXld(out outputLengthenLine, RowBegin.TupleConcat(RowEnd),
                           ColBegin.TupleConcat(ColEnd));
                        }
                        else
                        {
                            line.state = false;
                        }
                    }


                    #endregion

                    HOperatorSet.CountSeconds(out tempT3);
                    t2 = (tempT3 - tempT2) * 1000;

                    return true;

                }
                catch (Exception ex)
                {
                    line.state = false;
                    //取得当前方法类全名 包括命名空间    
                    var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                        System.Reflection.MethodBase.GetCurrentMethod().Name);
                    Log4netRecord.ErrorLog.Fatal(str + ex.Message);
                    return false;
                }
                finally
                {
                    outputContour?.Dispose();
                }
            }
          
        }


        /// <summary>
        /// 极耳带料
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="ModleXld"></param>
        /// <param name="MQDefects"></param>
        /// <param name="JRDaiLiaoXldRegion"></param>
        private void Find_JR_DaiLiao(HObject Image, HObject ModleXld,

         ParamsMQDefects MQDefects,out HObject JRDaiLiaoXldRegion)
        {
            HTuple Row1, Column1, Row2, Column2;
            HObject Domain_Image = null;
            HOperatorSet.GenEmptyObj(out Domain_Image);
            HObject NG_ROI = null;
            HOperatorSet.GenEmptyObj(out NG_ROI);
            //临时截图
            HObject ImagePart;
            HOperatorSet.GenEmptyObj(out ImagePart);
            HObject RegionConnectionOut;
            HOperatorSet.GenEmptyObj(out RegionConnectionOut);
            HTuple Area, RowCenter, Colcenter;
            HTuple tempArea, tempRowCenter, tempColcenter;

            //极耳带料检测区域
            HOperatorSet.GenEmptyObj(out JRDaiLiaoXldRegion);


            //极耳带料
            //平移XLD 1
            HObject JRDaiLiaoXld1;
            HOperatorSet.GenEmptyObj(out JRDaiLiaoXld1);
            //平移XLD 2
            HObject JRDaiLiaoXld2;
            HOperatorSet.GenEmptyObj(out JRDaiLiaoXld2);
            HObject JRDaiLiaoXldConcat;
            HOperatorSet.GenEmptyObj(out JRDaiLiaoXldConcat);
            HObject JRDaiLiaoXldUnion;
            HOperatorSet.GenEmptyObj(out JRDaiLiaoXldUnion);
            HObject JRDaiLiaoImageReduced;
            HOperatorSet.GenEmptyObj(out JRDaiLiaoImageReduced);
            try
            {
                //极耳余料

                HOperatorSet.GenParallelContourXld(ModleXld, out JRDaiLiaoXld1,
            "regression_normal", -MQDefects.num_JR_DaiLiao_ModelXld_ShiftRight_1/ WidthCali);

                HOperatorSet.GenParallelContourXld(ModleXld, out JRDaiLiaoXld2,
           "regression_normal",-MQDefects.num_JR_DaiLiao_ModelXld_ShiftRight_2/ WidthCali);

                //区域
                HOperatorSet.ConcatObj(JRDaiLiaoXld1, JRDaiLiaoXld2,
            out JRDaiLiaoXldConcat);
                //合并xld
                //HOperatorSet.UnionAdjacentContoursXld(JRDaiLiaoXldConcat, out JRDaiLiaoXldUnion,
                //50000, 100, "attr_keep");

                HOperatorSet.RegressContoursXld(JRDaiLiaoXldConcat, out JRDaiLiaoXldConcat, "drop", 1);

                HOperatorSet.UnionStraightContoursXld(JRDaiLiaoXldConcat, out JRDaiLiaoXldUnion,
                    50, (new HTuple(90)).TupleRad(), 50, "every", "maximum");




                //转区域  
                HOperatorSet.GenRegionContourXld(JRDaiLiaoXldUnion, out JRDaiLiaoXldRegion, "filled");
                //
                HOperatorSet.ReduceDomain(Image, JRDaiLiaoXldRegion, out JRDaiLiaoImageReduced);
                HOperatorSet.Threshold(JRDaiLiaoImageReduced, out NG_ROI, MQDefects.num_JR_DaiLiao_Threshold_Min, MQDefects.num_JR_DaiLiao_Threshold_Max);
                HOperatorSet.Connection(NG_ROI, out NG_ROI);
                HOperatorSet.SelectShape(NG_ROI, out NG_ROI, "area", "and", MQDefects.num_JR_DaiLiao_MinArea / AreaCali, 10e+11);
                HOperatorSet.AreaCenter(NG_ROI, out tempArea, out tempRowCenter, out tempColcenter);
                //最多3个极耳带料
                if (tempArea.Length < 3)
                {
                    for (int i = 0; i < tempArea.Length; i++)
                    {
                        HOperatorSet.SelectObj(NG_ROI, out RegionConnectionOut, i + 1);
                        HOperatorSet.AreaCenter(RegionConnectionOut, out Area, out RowCenter, out Colcenter);

                        HOperatorSet.SmallestRectangle1(RegionConnectionOut, out Row1, out Column1, out Row2, out Column2);

                        double w_f = (Column2.D - Column1.D);
                        double h_f = (Row2.D - Row1.D);

                        double Wf = w_f * WidthCali;
                        double Hf = h_f * HeightCali;
                        bool isOk = false;

                        if (!isOk)
                        {
                            CropImage(Image, RegionConnectionOut, out ImagePart);
                            NgTypeInfo iNgInfo = new NgTypeInfo();
                            iNgInfo.m_hImage = ImagePart.CopyObj(1, 1);//缺陷小图片
                            iNgInfo.m_hRegion = RegionConnectionOut.CopyObj(1, 1);
                            iNgInfo.m_strNgTypeName = Defct1Enum.极耳带料;//缺陷位置类型
                            iNgInfo.m_fNgTypArea = Area * AreaCali;//缺陷面积
                            iNgInfo.m_fNgTypeAbsPixelPos = Convert.ToInt64(RowCenter.D);//Y坐标
                            iNgInfo.m_fNgTypHeight = Hf;//缺陷高度\长度
                            iNgInfo.m_fNgTypWidth = Wf;//缺陷宽度
                            iNgInfo.sAttachInfo = "";//缺陷附加信息

                            string NGType = "极耳带料";
                            bool isExStd = ImageShow(iNgInfo, NGType);

                        }
                    }
                }
                //多于3个则,融合在一起
                else
                {
                    HOperatorSet.Union1(NG_ROI, out RegionConnectionOut);
                    HOperatorSet.AreaCenter(RegionConnectionOut, out Area, out RowCenter, out Colcenter);
                    HOperatorSet.SmallestRectangle1(RegionConnectionOut, out Row1, out Column1, out Row2, out Column2);
                    double w_f = (Column2.D - Column1.D);
                    double h_f = (Row2.D - Row1.D);
                    double Wf = w_f * WidthCali;
                    double Hf = h_f * HeightCali;
                    bool isOk = false;
                    if (!isOk)
                    {
                        CropImage(Image, RegionConnectionOut, out ImagePart);
                        NgTypeInfo iNgInfo = new NgTypeInfo();
                        iNgInfo.m_hImage = ImagePart.CopyObj(1, 1);//缺陷小图片
                        iNgInfo.m_hRegion = RegionConnectionOut.CopyObj(1, 1);
                        iNgInfo.m_strNgTypeName = Defct1Enum.极耳带料;//缺陷位置类型
                        iNgInfo.m_fNgTypArea = Area * AreaCali;//缺陷面积
                        iNgInfo.m_fNgTypeAbsPixelPos = Convert.ToInt64(RowCenter.D);//Y坐标
                        iNgInfo.m_fNgTypHeight = Hf;//缺陷高度\长度
                        iNgInfo.m_fNgTypWidth = Wf;//缺陷宽度
                        iNgInfo.sAttachInfo = "";//缺陷附加信息
                        string NGType = "极耳带料";
                        bool isExStd = ImageShow(iNgInfo, NGType);

                    }
                }
            }
            catch (Exception ex)
            {
                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log4netRecord.ErrorLog.Fatal(str + ex.Message);

            }
            finally
            {
                Domain_Image?.Dispose();
                NG_ROI?.Dispose();
                ImagePart?.Dispose();
                RegionConnectionOut?.Dispose();

                JRDaiLiaoXld1?.Dispose();
                JRDaiLiaoXldConcat?.Dispose();
                JRDaiLiaoXldUnion?.Dispose();
                JRDaiLiaoImageReduced?.Dispose();
                NG_ROI?.Dispose();
            }
        }

        /// <summary>
        /// 极耳破损
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="ModleXld"></param>
        /// <param name="MQDefects"></param>
        /// <param name="JRPoSunRegion"></param>
        private void Find_JR_PoSun(HObject Image, HObject ModleXld,

        ParamsMQDefects MQDefects, out HObject JRPoSunRegion)
        {
            HTuple Row1, Column1, Row2, Column2;
            HObject Domain_Image = null;
            HOperatorSet.GenEmptyObj(out Domain_Image);
            HObject NG_ROI = null;
            HOperatorSet.GenEmptyObj(out NG_ROI);
            //临时截图
            HObject ImagePart;
            HOperatorSet.GenEmptyObj(out ImagePart);
            HObject RegionConnectionOut;
            HOperatorSet.GenEmptyObj(out RegionConnectionOut);
            HTuple Area, RowCenter, Colcenter;
          

            //极耳检测区域
            HOperatorSet.GenEmptyObj(out JRPoSunRegion);


            HObject JRPoSunXld;
            HOperatorSet.GenEmptyObj(out JRPoSunXld);

            HObject JRPoSunImageReduced;
            HOperatorSet.GenEmptyObj(out JRPoSunImageReduced);

            try
            {
                //极耳破损
                HOperatorSet.GenParallelContourXld(ModleXld, out JRPoSunXld,
                "regression_normal", MQDefects.num_JR_PoSun_ModelXld_ShiftLeft_1/ WidthCali);

                HOperatorSet.GenRegionContourXld(JRPoSunXld, out JRPoSunRegion,
                    "filled");
                HOperatorSet.OpeningCircle(JRPoSunRegion, out JRPoSunRegion, 50.5);
                HOperatorSet.ReduceDomain(Image, JRPoSunRegion, out JRPoSunImageReduced);

                HOperatorSet.Threshold(JRPoSunImageReduced, out NG_ROI, MQDefects.num_JR_PoSun_Threshold_Min, MQDefects.num_JR_PoSun_Threshold_Max);

                HOperatorSet.Connection(NG_ROI, out NG_ROI);
                HOperatorSet.SelectShape(NG_ROI, out NG_ROI, "area", "and", MQDefects.num_JR_PoSun_MinArea / AreaCali, 10e+11);
   
                HOperatorSet.Union1(NG_ROI, out RegionConnectionOut);
                HOperatorSet.AreaCenter(RegionConnectionOut, out Area, out RowCenter, out Colcenter);
                HOperatorSet.SmallestRectangle1(RegionConnectionOut, out Row1, out Column1, out Row2, out Column2);
                double w_f = (Column2.D - Column1.D);
                double h_f = (Row2.D - Row1.D);
                double Wf = w_f * WidthCali;
                double Hf = h_f * HeightCali;
                bool isOk = false;
                if (!isOk)
                {
                    CropImage(Image, RegionConnectionOut, out ImagePart);
                    NgTypeInfo iNgInfo = new NgTypeInfo();
                    iNgInfo.m_hImage = ImagePart.CopyObj(1, 1);//缺陷小图片
                    iNgInfo.m_hRegion = RegionConnectionOut.CopyObj(1, 1);
                    iNgInfo.m_strNgTypeName = Defct1Enum.极耳破损;//缺陷位置类型
                    iNgInfo.m_fNgTypArea = Area * AreaCali;//缺陷面积
                    iNgInfo.m_fNgTypeAbsPixelPos = Convert.ToInt64(RowCenter.D);//Y坐标
                    iNgInfo.m_fNgTypHeight = Hf;//缺陷高度\长度
                    iNgInfo.m_fNgTypWidth = Wf;//缺陷宽度
                    iNgInfo.sAttachInfo = "";//缺陷附加信息
                    string NGType = "极耳破损";
                    bool isExStd = ImageShow(iNgInfo, NGType);
                }
               
            }
            catch (Exception ex)
            {
                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log4netRecord.ErrorLog.Fatal(str + ex.Message);

            }
            finally
            {
                Domain_Image?.Dispose();
                NG_ROI?.Dispose();
                ImagePart?.Dispose();
                RegionConnectionOut?.Dispose();

                JRPoSunXld?.Dispose();
                JRPoSunImageReduced?.Dispose();
                NG_ROI?.Dispose();
            }
        }


        /// <summary>
        /// 极耳带料
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="ModleXld"></param>
        /// <param name="MQDefects"></param>
        /// <param name="JRDaiLiaoXldRegion"></param>
        private void Find_JR_DaiLiao(HObject Image, HObject ModleXld,

         ParamsMQDefects MQDefects)
        {
            HTuple Row1, Column1, Row2, Column2;
            HObject Domain_Image = null;
            HOperatorSet.GenEmptyObj(out Domain_Image);
            HObject NG_ROI = null;
            HOperatorSet.GenEmptyObj(out NG_ROI);
            //临时截图
            HObject ImagePart;
            HOperatorSet.GenEmptyObj(out ImagePart);
            HObject RegionConnectionOut;
            HOperatorSet.GenEmptyObj(out RegionConnectionOut);
            HTuple Area, RowCenter, Colcenter;
            HTuple tempArea, tempRowCenter, tempColcenter;

            HObject JRDaiLiaoImageReduced;
            HOperatorSet.GenEmptyObj(out JRDaiLiaoImageReduced);
            try
            {
  
                HOperatorSet.ReduceDomain(Image, ModleXld, out JRDaiLiaoImageReduced);
                HOperatorSet.Threshold(JRDaiLiaoImageReduced, out NG_ROI, MQDefects.num_JR_DaiLiao_Threshold_Min, MQDefects.num_JR_DaiLiao_Threshold_Max);
                HOperatorSet.Connection(NG_ROI, out NG_ROI);
                HOperatorSet.SelectShape(NG_ROI, out NG_ROI, "area", "and", MQDefects.num_JR_DaiLiao_MinArea / AreaCali, 10e+11);
                HOperatorSet.AreaCenter(NG_ROI, out tempArea, out tempRowCenter, out tempColcenter);
                if(tempArea.Length==0)
                {
                    //无缺陷
                    return;
                }
                //最多3个极耳带料
                if (tempArea.Length < 3)
                {
                    for (int i = 0; i < tempArea.Length; i++)
                    {
                        HOperatorSet.SelectObj(NG_ROI, out RegionConnectionOut, i + 1);
                        HOperatorSet.AreaCenter(RegionConnectionOut, out Area, out RowCenter, out Colcenter);

                        HOperatorSet.SmallestRectangle1(RegionConnectionOut, out Row1, out Column1, out Row2, out Column2);      
                        double w_f = (Column2.D - Column1.D);
                        double h_f = (Row2.D - Row1.D);

                        double Wf = w_f * WidthCali;
                        double Hf = h_f * HeightCali;
                        bool isOk = false;

                        if (!isOk)
                        {
                            CropImage(Image, RegionConnectionOut, out ImagePart);
                            NgTypeInfo iNgInfo = new NgTypeInfo();
                            iNgInfo.m_hImage = ImagePart.CopyObj(1, 1);//缺陷小图片
                            iNgInfo.m_hRegion = RegionConnectionOut.CopyObj(1, 1);
                            iNgInfo.m_strNgTypeName = Defct1Enum.极耳带料;//缺陷位置类型
                            iNgInfo.m_fNgTypArea = Area * AreaCali;//缺陷面积
                            iNgInfo.m_fNgTypeAbsPixelPos = Convert.ToInt64(RowCenter.D);//Y坐标
                            iNgInfo.m_fNgTypHeight = Hf;//缺陷高度\长度
                            iNgInfo.m_fNgTypWidth = Wf;//缺陷宽度
                            iNgInfo.sAttachInfo = "";//缺陷附加信息

                            string NGType = "极耳带料";
                            bool isExStd = ImageShow(iNgInfo, NGType);

                        }
                    }
                }
                //多于3个则,融合在一起
                else
                {
                    HOperatorSet.Union1(NG_ROI, out RegionConnectionOut);
                    HOperatorSet.AreaCenter(RegionConnectionOut, out Area, out RowCenter, out Colcenter);
                    HOperatorSet.SmallestRectangle1(RegionConnectionOut, out Row1, out Column1, out Row2, out Column2);
                    double w_f = (Column2.D - Column1.D);
                    double h_f = (Row2.D - Row1.D);
                    double Wf = w_f * WidthCali;
                    double Hf = h_f * HeightCali;
                    bool isOk = false;
                    if (!isOk)
                    {
                        CropImage(Image, RegionConnectionOut, out ImagePart);
                        NgTypeInfo iNgInfo = new NgTypeInfo();
                        iNgInfo.m_hImage = ImagePart.CopyObj(1, 1);//缺陷小图片
                        iNgInfo.m_hRegion = RegionConnectionOut.CopyObj(1, 1);
                        iNgInfo.m_strNgTypeName = Defct1Enum.极耳带料;//缺陷位置类型
                        iNgInfo.m_fNgTypArea = Area * AreaCali;//缺陷面积
                        iNgInfo.m_fNgTypeAbsPixelPos = Convert.ToInt64(RowCenter.D);//Y坐标
                        iNgInfo.m_fNgTypHeight = Hf;//缺陷高度\长度
                        iNgInfo.m_fNgTypWidth = Wf;//缺陷宽度
                        iNgInfo.sAttachInfo = "";//缺陷附加信息
                        string NGType = "极耳带料";
                        bool isExStd = ImageShow(iNgInfo, NGType);

                    }
                }
            }
            catch (Exception ex)
            {
                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log4netRecord.ErrorLog.Fatal(str + ex.Message);

            }
            finally
            {
                Domain_Image?.Dispose();
                NG_ROI?.Dispose();
                ImagePart?.Dispose();
                RegionConnectionOut?.Dispose();
                JRDaiLiaoImageReduced?.Dispose();
                NG_ROI?.Dispose();
            }
        }

        /// <summary>
        /// 极耳破损
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="ModleXld"></param>
        /// <param name="MQDefects"></param>
        /// <param name="JRPoSunRegion"></param>
        private void Find_JR_PoSun(HObject Image, HObject ModleXld,

        ParamsMQDefects MQDefects)
        {
            HTuple Row1, Column1, Row2, Column2;
            HObject Domain_Image = null;
            HOperatorSet.GenEmptyObj(out Domain_Image);
            HObject NG_ROI = null;
            HOperatorSet.GenEmptyObj(out NG_ROI);
            //临时截图
            HObject ImagePart;
            HOperatorSet.GenEmptyObj(out ImagePart);
            HObject RegionConnectionOut;
            HOperatorSet.GenEmptyObj(out RegionConnectionOut);
            HTuple Area, RowCenter, Colcenter;

            HObject JRPoSunImageReduced;
            HOperatorSet.GenEmptyObj(out JRPoSunImageReduced);

            try
            {
               
                HOperatorSet.ReduceDomain(Image, ModleXld, out JRPoSunImageReduced);

                HOperatorSet.Threshold(JRPoSunImageReduced, out NG_ROI, MQDefects.num_JR_PoSun_Threshold_Min, MQDefects.num_JR_PoSun_Threshold_Max);

                HOperatorSet.Connection(NG_ROI, out NG_ROI);
                HOperatorSet.SelectShape(NG_ROI, out NG_ROI, "area", "and", MQDefects.num_JR_PoSun_MinArea / AreaCali, 10e+20);

                HOperatorSet.Union1(NG_ROI, out RegionConnectionOut);
                HOperatorSet.AreaCenter(RegionConnectionOut, out Area, out RowCenter, out Colcenter);
                if (Area.Length == 0)
                {
                    //无缺陷
                    return;
                }
                HOperatorSet.SmallestRectangle1(RegionConnectionOut, out Row1, out Column1, out Row2, out Column2);
                double w_f = (Column2.D - Column1.D);
                double h_f = (Row2.D - Row1.D);
                double Wf = w_f * WidthCali;
                double Hf = h_f * HeightCali;
                bool isOk = false;
                if (!isOk)
                {
                    CropImage(Image, RegionConnectionOut, out ImagePart);
                    NgTypeInfo iNgInfo = new NgTypeInfo();
                    iNgInfo.m_hImage = ImagePart.CopyObj(1, 1);//缺陷小图片
                    iNgInfo.m_hRegion = RegionConnectionOut.CopyObj(1, 1);
                    iNgInfo.m_strNgTypeName = Defct1Enum.极耳破损;//缺陷位置类型
                    iNgInfo.m_fNgTypArea = Area * AreaCali;//缺陷面积
                    iNgInfo.m_fNgTypeAbsPixelPos = Convert.ToInt64(RowCenter.D);//Y坐标
                    iNgInfo.m_fNgTypHeight = Hf;//缺陷高度\长度
                    iNgInfo.m_fNgTypWidth = Wf;//缺陷宽度
                    iNgInfo.sAttachInfo = "";//缺陷附加信息
                    string NGType = "极耳破损";
                    bool isExStd = ImageShow(iNgInfo, NGType);
                }

            }
            catch (Exception ex)
            {
                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log4netRecord.ErrorLog.Fatal(str + ex.Message);

            }
            finally
            {
                Domain_Image?.Dispose();
                NG_ROI?.Dispose();
                ImagePart?.Dispose();
                RegionConnectionOut?.Dispose();
                JRPoSunImageReduced?.Dispose();
                NG_ROI?.Dispose();
            }
        }




        /// <summary>
        /// 膜区检测
        /// </summary>
        /// <param name="Image">图像</param>
        /// <param name="DetectionROI">检测区域</param>
        /// <param name="ShieldROI">屏蔽区域</param>
        /// <param name="MQDefects">参数</param>
        /// /// <param name="WinHandle">显示窗口</param>
        private void coatingDefects (HObject Image, HObject DetectionROI,
            HObject ShieldROI, ParamsMQDefects MQDefects )
        {
           
            HObject ROI;
            HOperatorSet.GenEmptyObj(out ROI);

            //暗瑕疵
            HObject NG_Blackt;
            HOperatorSet.GenEmptyObj(out NG_Blackt);
            HObject ROI_Blackt;
            HObject Domain_Image_Blackt;
            HOperatorSet.GenEmptyObj(out ROI_Blackt);
            HOperatorSet.GenEmptyObj(out Domain_Image_Blackt);


            //亮瑕疵
            HObject NG_White;
            HOperatorSet.GenEmptyObj(out NG_White);
            HObject ROI_White;
            HObject Domain_Image_White;
            HOperatorSet.GenEmptyObj(out ROI_White);
            HOperatorSet.GenEmptyObj(out Domain_Image_White);

            //生成暗瑕疵检测区域
            HOperatorSet.Difference(DetectionROI, ShieldROI, out ROI_Blackt);
            HOperatorSet.ReduceDomain(Image, ROI_Blackt, out Domain_Image_Blackt);
            //生成亮瑕疵检测区域
            HOperatorSet.Difference(DetectionROI, ShieldROI, out ROI_White);
            HOperatorSet.ReduceDomain(Image, ROI_White, out Domain_Image_White);
           

            //面积 中心 Y X
            HTuple hv_area, hv_RowCenter, hv_Colcenter;
            //面积总和
            HTuple typeAreaMm;
            //亮瑕疵 Ng数量
            int NgNum_Whitethreshold = 0;
            //暗瑕疵 Ng数量
            int NgNum_Blackthreshold = 0;

            //检测出来的NG数目过多标志量
            bool NgNum_OverFlag = false;
            //Ng最多数量
            int NgMaxNum = MQDefects.num_NgMaxNum;
            //平均灰度值
            HTuple blackRegionGray =0;


            //缺陷小图
            HObject ho_ImagePart;
            HOperatorSet.GenEmptyObj(out ho_ImagePart);
            //最多几个进入分类器
            //int image_num =2;

            //检测区域X边缘
            HTuple hv_lineONEx = 0, hv_lineENDx = 0;

            //判判是否是接带还是漏金属的最小宽\高
            double JieDaiMaxH = MQDefects.num_JDMaxH;  
            double JieDaiMinW = MQDefects.num_JDMinW;
            //总面积超过多少作为接带
            double dJDAreaNG = MQDefects.num_JDAreaNG;
            //平均灰度大于亮瑕疵最小值多少才去判定是接带或者漏金属
            int grayDifference = MQDefects.num_GrayDifference;
            try
            {
                //获取图像宽高
                HTuple hv_Width, hv_Height;
                HOperatorSet.GetImageSize(Image, out hv_Width, out hv_Height);
                ImageWidth = (int)(hv_Width.D);
                ImageHeight = (int)(hv_Height.D);

                //获取区域
                HTuple row1, column1, row2, column2;
                HOperatorSet.SmallestRectangle1(DetectionROI, out row1, out column1, out row2, out column2);
                if(row1.Length>0)
                {
                    hv_lineONEx = column1.TupleMin();
                    hv_lineENDx = column2.TupleMax();
                }

                #region >>>输入参数
                int WhiteThreshold_Min = MQDefects.num_WhiteThreshold_Min;
                int WhiteThreshold_Max = MQDefects.num_WhiteThreshold_Max;

                int Blackthreshold_Min = MQDefects.num_BlackThreshold_Min;
                int Blackthreshold_Max = MQDefects.num_BlackThreshold_Max;

                double MinArea_White = MQDefects.num_MinArea_White / AreaCali;
                double MinArea_Black = MQDefects.num_MinArea_Black / AreaCali;
                double hv_Radius = 3;

               

                #endregion



                //========【】提取亮瑕疵与暗瑕疵
                NG_White.Dispose();
                NG_Blackt.Dispose();
                HOperatorSet.GenEmptyObj(out NG_White);
                HOperatorSet.GenEmptyObj(out NG_Blackt);
                HOperatorSet.Threshold(Domain_Image_White, out NG_White, WhiteThreshold_Min, WhiteThreshold_Max);
                HOperatorSet.Threshold(Domain_Image_Blackt, out NG_Blackt, Blackthreshold_Min, Blackthreshold_Max);

                #region  >>>缺陷筛选

                //========【】亮瑕疵 筛选
                HTuple Wthreshold_MaxPixArea = 0;
                HTuple Bthreshold_MaxPixArea = 0;
                //获取亮瑕疵面积
                HOperatorSet.AreaCenter(NG_White, out hv_area, out hv_RowCenter, out hv_Colcenter);
                typeAreaMm = hv_area * AreaCali;
                HOperatorSet.Connection(NG_White, out NG_White);
                //过滤下
                HOperatorSet.SelectShape(NG_White, out NG_White, "area", "and", MinArea_White, 10e+11);
                HOperatorSet.AreaCenter(NG_White, out hv_area, out hv_RowCenter, out hv_Colcenter);
                //计算下数量
                NgNum_Whitethreshold = hv_area.Length;
                if (NgNum_Whitethreshold > 0)
                {
                    //获得面积最大的序号
                    HOperatorSet.TupleMax(hv_area, out Wthreshold_MaxPixArea);
                }

                double JD_PixArea = dJDAreaNG / AreaCali;
                //当找到的亮瑕疵,最大面积也小于过滤面积时
                if (Wthreshold_MaxPixArea < MinArea_White)
                {
                    HOperatorSet.GenEmptyObj(out NG_White);
                    NgNum_Whitethreshold = 1;
                }
                //亮瑕疵数量大于10 或者  亮瑕疵数量大于10且最大面积大于100平方mm
                else if (NgNum_Whitethreshold > NgMaxNum || 
                    (Wthreshold_MaxPixArea > JD_PixArea))
                {
                    //存在面积过大,或者数量过多,全部融合
                    try
                    {

                        HOperatorSet.Union1(NG_White, out NG_White);
                    }
                    catch (Exception ex)
                    {
                         Log4netRecord.ErrorLog.Fatal(ex.Message + "\r\n 亮瑕疵检测错误");
                    }
                    finally
                    {

                    }
                }
                else
                {
                    HOperatorSet.SelectShape(NG_White, out NG_White, "area", "and", MinArea_White, 10e+11);
                    HOperatorSet.AreaCenter(NG_White, out hv_area, out hv_RowCenter, out hv_Colcenter);

                    NgNum_Whitethreshold = hv_area.Length;
                }

                //========【】暗瑕疵 筛选      
                HOperatorSet.AreaCenter(NG_Blackt, out hv_area, out hv_RowCenter, out hv_Colcenter);

                typeAreaMm = hv_area * AreaCali;
   
                HOperatorSet.Connection(NG_Blackt, out NG_Blackt);
                HOperatorSet.SelectShape(NG_Blackt, out NG_Blackt, "area", "and", MinArea_Black, 10e+11);
                HOperatorSet.AreaCenter(NG_Blackt, out hv_area, out hv_RowCenter, out hv_Colcenter);
                NgNum_Blackthreshold = hv_area.Length;
                if (NgNum_Blackthreshold > 0)
                {
                    HOperatorSet.TupleMax(hv_area, out Bthreshold_MaxPixArea);
                }

                if (Bthreshold_MaxPixArea < MinArea_Black)
                {
                    HOperatorSet.GenEmptyObj(out NG_Blackt);
                    NgNum_Blackthreshold = 1;
                }
                else if (NgNum_Blackthreshold > NgMaxNum)
                {
                    try
                    {
                        //数量过多,全部融合
                        HOperatorSet.Union1(NG_Blackt, out NG_Blackt);

                    }
                    catch (Exception ex)
                    {
                         Log4netRecord.ErrorLog.Fatal(ex + "\r\n 暗瑕疵检测错误");
                    }
                    finally
                    {

                    }
                }
                else
                {
                    HOperatorSet.SelectShape(NG_Blackt, out NG_Blackt, "area", "and", MinArea_Black, 10e+11);
                    HOperatorSet.AreaCenter(NG_Blackt, out hv_area, out hv_RowCenter, out hv_Colcenter);
                }

                
                //=====================================================================================================================================
                //========【】合并亮瑕疵与暗瑕疵
                //2023.11.14  如果不先各自融合,会面积会大,且分开后数量会多
                HOperatorSet.Union1(NG_White, out NG_White);
                HOperatorSet.Union1(NG_Blackt, out NG_Blackt);
                HOperatorSet.Union2(NG_White, NG_Blackt, out NG_White);
                

                //缺陷数量过多则融合为一个
                if (NgNum_Blackthreshold +NgNum_Whitethreshold <= NgMaxNum)
                {
                    NgNum_OverFlag = false;
                    HOperatorSet.ClosingCircle(NG_White, out NG_White, hv_Radius);
                    HOperatorSet.Connection(NG_White, out NG_White);
                    HOperatorSet.AreaCenter(NG_White, out hv_area, out hv_RowCenter, out hv_Colcenter);
                }
                else
                {
                    NgNum_OverFlag = true;
                    HOperatorSet.Union1(NG_White, out NG_White);
                    HOperatorSet.AreaCenter(NG_White, out hv_area, out hv_RowCenter, out hv_Colcenter);
                }


                #endregion

                //获取最大面积
                HTuple hv_areaMax = 0;
                if (hv_area.Length > 0)
                {
                    hv_areaMax = hv_area.TupleMax();
                }
                if (hv_Colcenter.Length > 0 && hv_areaMax > 0)
                {
        
                    typeAreaMm = hv_area * AreaCali;
                    HTuple hv_Row1, hv_Column1, hv_Row2, hv_Column2, hv_defectWidth, hv_defectLength, Gray, Areasum, w_f, h_f;
                    HOperatorSet.SmallestRectangle1(NG_White, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);
                    HOperatorSet.GrayFeatures(NG_White, Image, "mean", out blackRegionGray);//提取接带平均灰度值
                    hv_defectWidth = (hv_Column2 - hv_Column1);
                    hv_defectLength = (hv_Row2 - hv_Row1);
                    HOperatorSet.TupleMax(hv_defectWidth, out w_f);
                    HOperatorSet.TupleMax(hv_defectLength, out h_f);
                    double Wf = w_f * WidthCali;
                    double Hf = h_f * HeightCali;

                    bool boolWhite = false;
                    HOperatorSet.TupleMax(blackRegionGray, out Gray);
                    if (Gray.Length > 0)
                    {
                        //平均灰度大于 亮瑕疵最小 20时,判断 接带或者漏金属
                        if (Gray > WhiteThreshold_Min - grayDifference)
                        {
                            boolWhite = true;//注意：平均灰度会拉低整体，这里减10
                        }
                    }
                    //计算面积和
                    HOperatorSet.TupleSum(typeAreaMm, out Areasum);

                    //========面积大无需进入你分类器
                    if ((Areasum > dJDAreaNG) && boolWhite)
                    {  
                        HOperatorSet.Union1(NG_White, out NG_White);
                        //面积过大时,融合后再计算
                        HOperatorSet.SmallestRectangle1(NG_White, out hv_Row1, out hv_Column1, out hv_Row2, out hv_Column2);
                        HTuple area, row, column;
                        HOperatorSet.AreaCenter(NG_White, out area, out row, out column);
                        hv_defectWidth = (hv_Column2 - hv_Column1);
                        hv_defectLength = (hv_Row2 - hv_Row1);
                        HOperatorSet.TupleMax(hv_defectWidth, out w_f);
                        HOperatorSet.TupleMax(hv_defectLength, out h_f);
                        Wf = w_f * WidthCali;
                        Hf = h_f * HeightCali;

                        //是否接带
                        bool JieDai = true;
                        
                        if (Hf > JieDaiMaxH && Wf < JieDaiMinW)
                        {
                            JieDai = false;
                        }
                        else
                        {
                            JieDai = true;
                        }
                        //=======【】NG打标采集，小图显示存图
                        double dNgArea = Areasum;
                        double dNgHigh = Hf;
                        double dNgWidth = Wf;

                        NgTypeInfo iNgInfo = new NgTypeInfo();
                        zoomImage(Image, NG_White, out ho_ImagePart);
                       
                        iNgInfo.m_hImage = ho_ImagePart.CopyObj(1, 1);//缺陷小图片
                        iNgInfo.m_hRegion = NG_White.CopyObj(1, 1);
                        iNgInfo.m_strNgTypeName = Defct1Enum.接带;//缺陷位置类型
                        iNgInfo.m_fNgTypArea = dNgArea;//缺陷面积
                        iNgInfo.m_fNgTypeAbsPixelPos =Convert.ToInt64(row.D);//Y坐标
                        iNgInfo.m_fNgTypHeight = dNgHigh;//缺陷高度\长度
                        iNgInfo.m_fNgTypWidth = dNgWidth;//缺陷宽度

                        string NGType = "接带";
                        if (!JieDai)
                        {
                            iNgInfo.m_strNgTypeName = Defct1Enum.漏金属;//缺陷位置类型ID
                            NGType = "漏金属";
                        }
                        ImageShow(iNgInfo, NGType);
                    }
                    //【】正常不良进入分类
                    else
                    {
                        HTuple Index, num = 0;
                        HOperatorSet.TupleSortIndex(hv_area, out Index);//面积由小到大索引排序
                        num = Index.Length;
                        for (int i = num - 1; i >= 0; i--)
                        {
                            int hv_i = Index[i];//从面积大到小排列
                            //只取10个
                            if (num - i > NgMaxNum)
                            {
                                break;
                            }

                            //=====================================================================================================================================
                            //========【】
                            if (hv_area.TupleSelect(hv_i) < 1) continue;
                            double area1 = typeAreaMm.TupleSelect(hv_i);
                            double NgWidth1 = hv_defectWidth.TupleSelect(hv_i) * WidthCali;
                            double NgHeight1 = hv_defectLength.TupleSelect(hv_i) * HeightCali;
                            int defectIndex = hv_i + 1;
                            int iNG_Gray = 0;

                            ROI.Dispose();
                            HTuple hRegionGray;
                            HOperatorSet.SelectObj(NG_White, out ROI, defectIndex);
                            HOperatorSet.GrayFeatures(ROI, Image, "mean", out hRegionGray);//提取接待平均灰度值 
                            if (hRegionGray.Length > 0) iNG_Gray = hRegionGray.TupleSelect(0).TupleInt();
                            else continue;
                            //=====================================================================================================================================
                            //========【】截取小图
                            if (hv_Row1.TupleSelect(hv_i) < 100) hv_Row1[hv_i] = 100;
                            if (hv_Row2.TupleSelect(hv_i) < 101) hv_Row2[hv_i] = 101;
                            if (hv_Row1.TupleSelect(hv_i) > ImageHeight - 101) hv_Row1[hv_i] = ImageHeight - 101;
                            if (hv_Row2.TupleSelect(hv_i) > ImageHeight - 100) hv_Row2[hv_i] = ImageHeight - 100;

                            if (hv_Column1.TupleSelect(hv_i) - 80 < hv_lineONEx) hv_Column1[hv_i] = hv_lineONEx + 80;
                            if (hv_Column2.TupleSelect(hv_i) - 80 < hv_lineONEx) hv_Column2[hv_i] = hv_lineONEx + 80;
                            if (hv_Column1.TupleSelect(hv_i) + 80 > hv_lineENDx) hv_Column1[hv_i] = hv_lineENDx - 80;
                            if (hv_Column2.TupleSelect(hv_i) + 80 > hv_lineENDx) hv_Column2[hv_i] = hv_lineENDx - 80;

                            if (hv_Row1.TupleSelect(hv_i) + 0.5 > hv_Row2.TupleSelect(hv_i))
                                hv_Row2[hv_i] = hv_Row1.TupleSelect(hv_i) + 1;
                            if (hv_Column1.TupleSelect(hv_i) + 0.5 > hv_Column2.TupleSelect(hv_i))
                                hv_Column2[hv_i] = hv_Column1.TupleSelect(hv_i) + 1;

                            ho_ImagePart.Dispose();
                            HOperatorSet.CropPart(Image, out ho_ImagePart, hv_Row1.TupleSelect(
                                                    hv_i) - 80, hv_Column1.TupleSelect(hv_i) - 80, (hv_Column2.TupleSelect(
                                                    hv_i)) - (hv_Column1.TupleSelect(hv_i)) + 160, (hv_Row2.TupleSelect(hv_i)) - (hv_Row1.TupleSelect(
                                                    hv_i)) + 160);

                            //=====================================================================================================================================
                            //========【】压缩图片并返回图像前5个像素
                            DefectInfos[NgImagePart_Count].NgImage?.Dispose();
                            string GrayValue = ClassDefect.Imagecompression(ho_ImagePart, out DefectInfos[NgImagePart_Count].NgImage);
                            DefectInfos[NgImagePart_Count].NgROIIndex = (hv_i + 1);
                            DefectInfos[NgImagePart_Count].NgArea = area1;
                            DefectInfos[NgImagePart_Count].NgHigh = NgHeight1;
                            DefectInfos[NgImagePart_Count].NgWidth = NgWidth1;
                            DefectInfos[NgImagePart_Count].NgMeanGray = iNG_Gray;

                            //=====================================================================================================================================
                            //========【】
                            if (ho_Images.Count <NgMaxNum)
                                ho_Images.Add(DefectInfos[NgImagePart_Count].NgImage);//添加到图片    

                            NgImagePart_Count++;
                        }
                    }
                }
                //负极使用分类器    
                if (MQDefects.com_Cathode==1)
                {
                    defectClassification_NotCathode(NG_White);//阳极
                }
                //正极判断是否使用分类器
                else
                {
                    if (MQDefects.cb_Enable_Classify)
                    {
                        defectClassification_Cathode_AI(NG_White);
                    }
                    else
                    {
                        defectClassification_Cathode(NG_White);//阴极 传统版
                    }


                }
                NgImagePart_Count = 0;
                ho_Images.Clear();
            }
            catch (Exception ex) 
            {
                 Log4netRecord.ErrorLog.Fatal(ex.Message);
                
            }
            finally
            {
                NG_Blackt?.Dispose();
                ROI_Blackt?.Dispose();
                Domain_Image_Blackt?.Dispose();
                NG_White?.Dispose();
                ROI_White?.Dispose();
                Domain_Image_White?.Dispose();
                ho_ImagePart?.Dispose();
                ROI?.Dispose();
            }



        }

        /// <summary>
        /// 未冷压
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="middParam"></param>
        /// <param name="VisionSettingParam"></param>
        public void WLY(HObject Image, HObject DetectionROI,
            HObject ShieldROI, ParamsMQDefects MQDefects)
        {

            HTuple Row1, Column1, Row2, Column2;
            HTuple Area, RowCenter, Colcenter;
            HObject Domain_Image = null;
            HOperatorSet.GenEmptyObj(out Domain_Image);
            HObject ImagePart = null;
            HOperatorSet.GenEmptyObj(out ImagePart);


            HObject ROI = null;
            HOperatorSet.GenEmptyObj(out ROI);
            HObject oROI = null;
            HOperatorSet.GenEmptyObj(out oROI);

            try
            {
                HOperatorSet.Difference(DetectionROI, ShieldROI, out ROI);
                bool isok = true;
                Test_WLY(Image, ROI, MQDefects.num_WLY_GrayUnder,
                    MQDefects.num_WLY_GrayDifference
                    , out isok, out oROI);

                HOperatorSet.AreaCenter(oROI, out Area, out RowCenter, out Colcenter);
                HOperatorSet.SmallestRectangle1(oROI, out Row1, out Column1, out Row2, out Column2);

                if (Area.Length > 0)
                {
                    double w_f = (Column2.D - Column1.D);
                    double h_f = (Row2.D - Row1.D);

                    double Wf = w_f * WidthCali;
                    double Hf = h_f * HeightCali;

                    
                    zoomImage(Image, oROI, out ImagePart);
                    NgTypeInfo iNgInfo = new NgTypeInfo();
                    iNgInfo.m_hImage = ImagePart.CopyObj(1, 1);//缺陷小图片
                    iNgInfo.m_hRegion = oROI.CopyObj(1, 1);
                    iNgInfo.m_strNgTypeName = Defct1Enum.未冷压;//缺陷位置类型
                    iNgInfo.m_fNgTypArea = Area * AreaCali;//缺陷面积
                    iNgInfo.m_fNgTypeAbsPixelPos = Convert.ToInt64(RowCenter.D);//Y坐标
                    iNgInfo.m_fNgTypHeight = Hf;//缺陷高度\长度
                    iNgInfo.m_fNgTypWidth = Wf;//缺陷宽度
                    iNgInfo.sAttachInfo = "";//缺陷附加信息
                    string NGType = "末冷压";
                    bool isExStd = ImageShow(iNgInfo, NGType);
                }
            }
            catch (Exception ex)
            {
                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                 Log4netRecord.ErrorLog.Fatal(str + ex.Message);

            }
            finally
            {
                Domain_Image?.Dispose();
                ImagePart?.Dispose();
                ROI?.Dispose();
                oROI?.Dispose();
            }
        }


        /// <summary>
        /// 直边破损
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="DetectionROI"></param>
        /// <param name="ShieldROI"></param>
        /// <param name="MQDefects"></param>
        public void ZBPS(HObject Image, HObject DetectionROI,
            HObject ShieldROI, ParamsMQDefects MQDefects)
        {
            HTuple Lx=0;
            HTuple Rx=0;
            //直边破损的检测区域
            HObject FindPosunEdgeRect_L = null;
            HOperatorSet.GenEmptyObj(out FindPosunEdgeRect_L);

            //直边破损的检测区域
            HObject FindPosunEdgeRect_R = null;
            HOperatorSet.GenEmptyObj(out FindPosunEdgeRect_R);
            try
            {
                Find_posun(Image, DetectionROI, ShieldROI,
                    Lx, Rx, true, MQDefects);
               
              
            }
            catch (Exception ex)
            {
                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                 Log4netRecord.ErrorLog.Fatal(str + ex.Message);

            }

            finally
            {
                FindPosunEdgeRect_L?.Dispose();
                FindPosunEdgeRect_R?.Dispose();

            }
        }
        /// <summary>
        /// 直边破损检测
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="ho_edgeRect"></param>
        /// <param name="Shield_ROI"></param>
        /// <param name="Lx"></param>
        /// <param name="Rx"></param>
        /// <param name="isLeft"></param>
        /// <param name="MQDefects"></param>
        private void Find_posun(HObject Image,
            HObject ho_edgeRect, HObject Shield_ROI, HTuple Lx,
            HTuple Rx, bool isLeft, ParamsMQDefects MQDefects)
        {
            HTuple Row1, Column1, Row2, Column2;
            HObject Domain_Image = null;
            HOperatorSet.GenEmptyObj(out Domain_Image);
            HObject NG_ROI = null;
            HOperatorSet.GenEmptyObj(out NG_ROI);

            //临时截图
            HObject ImagePart;
            HOperatorSet.GenEmptyObj(out ImagePart);

            HObject RegionConnectionOut_temp;
            HOperatorSet.GenEmptyObj(out RegionConnectionOut_temp);


            HTuple Area, RowCenter, Colcenter;

            HTuple tempArea, tempRowCenter, tempColcenter;

            //是否距离边缘,像素判断
            double isEdgePixelValue = 8;

            try
            {

                HOperatorSet.Difference(ho_edgeRect, Shield_ROI, out ho_edgeRect);//根据输入直边破损检测ROI区，屏蔽极耳区域

                //=========【2】阈值提取直边破损区域
                HOperatorSet.ReduceDomain(Image, ho_edgeRect, out Domain_Image);

                //=========================================================================================================================================
                //=========【3】阈值提取直边破损区域
                HOperatorSet.Threshold(Domain_Image, out NG_ROI, 0, MQDefects.num_ZBPS_Threshold);//0-破损亮度 
                HOperatorSet.OpeningRectangle1(NG_ROI, out NG_ROI, 2, 3);//过滤噪点
                HOperatorSet.ClosingRectangle1(NG_ROI, out NG_ROI, 25, 25);//连接分散的瑕疵
                HOperatorSet.Connection(NG_ROI, out NG_ROI);
                HOperatorSet.SelectShape(NG_ROI, out NG_ROI, "area", "and", MQDefects.num_ZBPS_MinArea / AreaCali, 10e+11);
                HOperatorSet.AreaCenter(NG_ROI, out tempArea, out tempRowCenter, out tempColcenter);

                //最多10个直边破损,
                if (tempArea.Length < 10)
                {

                    for (int i = 0; i < tempArea.Length; i++)
                    {
                        HOperatorSet.SelectObj(NG_ROI, out RegionConnectionOut_temp, i + 1);

                        HOperatorSet.AreaCenter(RegionConnectionOut_temp, out Area, out RowCenter, out Colcenter);

                        HOperatorSet.SmallestRectangle1(RegionConnectionOut_temp, out Row1, out Column1, out Row2, out Column2);

                        double w_f = (Column2.D - Column1.D);
                        double h_f = (Row2.D - Row1.D);

                        double Wf = w_f * WidthCali;
                        double Hf = h_f * HeightCali;
                        bool isOk = false;
                        //判断是否在边缘
                        //if (isLeft)
                        //{
                        //    if ((Column1.D - Lx) < isEdgePixelValue)
                        //    {
                        //        isOk = false;
                        //    }
                        //}
                        //else
                        //{
                        //    if ((Rx - Column2.D) < isEdgePixelValue)
                        //    {
                        //        isOk = false;
                        //    }
                        //}
                        if (!isOk)
                        {
                            zoomImage(Image, RegionConnectionOut_temp, out ImagePart);
                            NgTypeInfo iNgInfo = new NgTypeInfo();
                            iNgInfo.m_hImage = ImagePart.CopyObj(1, 1);//缺陷小图片
                            iNgInfo.m_hRegion = RegionConnectionOut_temp.CopyObj(1, 1);
                            iNgInfo.m_strNgTypeName = Defct1Enum.直边破损;//缺陷位置类型
                            iNgInfo.m_fNgTypArea = Area* AreaCali;//缺陷面积
                            iNgInfo.m_fNgTypeAbsPixelPos = Convert.ToInt64(RowCenter.D);//Y坐标
                            iNgInfo.m_fNgTypHeight = Hf;//缺陷高度\长度
                            iNgInfo.m_fNgTypWidth = Wf;//缺陷宽度
                            iNgInfo.sAttachInfo = "";//缺陷附加信息
                            string NGType = "直边破损";
                            bool isExStd = ImageShow(iNgInfo, NGType);

                        }


                    }

                }
                //多于10个则,融合在一起
                else
                {

                    HOperatorSet.Union1(NG_ROI, out RegionConnectionOut_temp);

                    HOperatorSet.AreaCenter(RegionConnectionOut_temp, out Area, out RowCenter, out Colcenter);

                    HOperatorSet.SmallestRectangle1(RegionConnectionOut_temp, out Row1, out Column1, out Row2, out Column2);

                    double w_f = (Column2.D - Column1.D);
                    double h_f = (Row2.D - Row1.D);

                    double Wf = w_f * WidthCali;
                    double Hf = h_f * HeightCali;

                    bool isOk = false;
                    //判断是否在边缘
                    //if (isLeft)
                    //{
                    //    if ((Column1.D - Lx) < isEdgePixelValue)
                    //    {
                    //        isOk = false;
                    //    }
                    //}
                    //else
                    //{
                    //    if ((Rx - Column2.D) < isEdgePixelValue)
                    //    {
                    //        isOk = false;
                    //    }
                    //}
                    if (!isOk)
                    {

                        zoomImage(Image, RegionConnectionOut_temp, out ImagePart);
                        NgTypeInfo iNgInfo = new NgTypeInfo();
                        iNgInfo.m_hImage = ImagePart.CopyObj(1, 1);//缺陷小图片
                        iNgInfo.m_hRegion = RegionConnectionOut_temp.CopyObj(1, 1);
                        iNgInfo.m_strNgTypeName = Defct1Enum.直边破损;//缺陷位置类型
                        iNgInfo.m_fNgTypArea = Area* AreaCali;//缺陷面积
                        iNgInfo.m_fNgTypeAbsPixelPos = Convert.ToInt64(RowCenter.D);//Y坐标
                        iNgInfo.m_fNgTypHeight = Hf;//缺陷高度\长度
                        iNgInfo.m_fNgTypWidth = Wf;//缺陷宽度
                        iNgInfo.sAttachInfo = "";//缺陷附加信息
                        string NGType = "直边破损";
                        bool isExStd = ImageShow(iNgInfo, NGType);
                    }


                }

            }
            catch (Exception ex)
            {
                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                 Log4netRecord.ErrorLog.Fatal(str + ex.Message);

            }
            finally
            {
                Domain_Image?.Dispose();
                NG_ROI?.Dispose();
                ImagePart?.Dispose();
                RegionConnectionOut_temp?.Dispose();
            }
        }



        /// <summary>
        /// 边缘掉料
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="DetectionROI"></param>
        /// <param name="ShieldROI"></param>
        /// <param name="MQDefects"></param>
        public void BYDL(HObject Image, HObject DetectionROI,
            HObject ShieldROI, ParamsMQDefects MQDefects)
        {
            HTuple Lx=0;
            HTuple Rx=0;
            //边缘掉料的检测区域
            HObject FindPosunEdgeRect_L = null;
            HOperatorSet.GenEmptyObj(out FindPosunEdgeRect_L);

            //边缘掉料的检测区域
            HObject FindPosunEdgeRect_R = null;
            HOperatorSet.GenEmptyObj(out FindPosunEdgeRect_R);
            try
            {
                Find_BYDL(Image, DetectionROI, ShieldROI,
                    Lx, Rx, false, MQDefects);
            }
            catch (Exception ex)
            {
                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                 Log4netRecord.ErrorLog.Fatal(str + ex.Message);

            }

            finally
            {
                FindPosunEdgeRect_L?.Dispose();
                FindPosunEdgeRect_R?.Dispose();

            }
        }

        /// <summary>
        /// 边缘掉料检测
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="DetectionROI"></param>
        /// <param name="Shield_ROI"></param>
        /// <param name="Lx"></param>
        /// <param name="Rx"></param>
        /// <param name="isLeft"></param>
        /// <param name="MQDefects"></param>
        private void Find_BYDL(HObject Image,
            HObject DetectionROI, HObject Shield_ROI, HTuple Lx,
            HTuple Rx, bool isLeft, ParamsMQDefects MQDefects)
        {
            HTuple Row1, Column1, Row2, Column2;
            HObject Domain_Image = null;
            HOperatorSet.GenEmptyObj(out Domain_Image);
            HObject NG_ROI = null;
            HOperatorSet.GenEmptyObj(out NG_ROI);

            //临时截图
            HObject ImagePart;
            HOperatorSet.GenEmptyObj(out ImagePart);

            HObject RegionConnectionOut_temp;
            HOperatorSet.GenEmptyObj(out RegionConnectionOut_temp);


            HTuple Area, RowCenter, Colcenter;

            HTuple tempArea, tempRowCenter, tempColcenter;

            //是否距离边缘,像素判断
            double isEdgePixelValue = 15;

            try
            {
                HOperatorSet.Difference(DetectionROI, Shield_ROI, out DetectionROI);//根据输入边缘掉料检测ROI区，屏蔽极耳区域

                //=========================================================================================================================================
                //=========【2】阈值提取边缘掉料区域
                HOperatorSet.ReduceDomain(Image, DetectionROI, out Domain_Image);

                //=========================================================================================================================================
                //=========【3】阈值提取边缘掉料区域
                HOperatorSet.Threshold(Domain_Image, out NG_ROI, MQDefects.num_BYDL_Threshold, 255);
                HOperatorSet.OpeningRectangle1(NG_ROI, out NG_ROI, 2, 3);//过滤噪点
                HOperatorSet.ClosingRectangle1(NG_ROI, out NG_ROI, 25, 25);//连接分散的瑕疵
                HOperatorSet.Connection(NG_ROI, out NG_ROI);
                HOperatorSet.SelectShape(NG_ROI, out NG_ROI, "area", "and", MQDefects.num_BYDL_MinArea / AreaCali, 10e+11);
                HOperatorSet.AreaCenter(NG_ROI, out tempArea, out tempRowCenter, out tempColcenter);
                //最多10个边缘掉料,
                if (tempArea.Length < 10)
                {

                    for (int i = 0; i < tempArea.Length; i++)
                    {
                        HOperatorSet.SelectObj(NG_ROI, out RegionConnectionOut_temp, i + 1);

                        HOperatorSet.AreaCenter(RegionConnectionOut_temp, out Area, out RowCenter, out Colcenter);

                        HOperatorSet.SmallestRectangle1(RegionConnectionOut_temp, out Row1, out Column1, out Row2, out Column2);

                        double w_f = (Column2.D - Column1.D);
                        double h_f = (Row2.D - Row1.D);

                        double Wf = w_f * WidthCali;
                        double Hf = h_f * HeightCali;
                        bool isOk = false;
                        //判断是否在边缘
                        //if (isLeft)
                        //{
                        //    if ((Column1.D - Lx) < isEdgePixelValue)
                        //    {
                        //        isOk = false;
                        //    }
                        //}
                        //else
                        //{
                        //    if ((Rx - Column2.D) < isEdgePixelValue)
                        //    {
                        //        isOk = false;
                        ////    }
                        //}
                        if (!isOk)
                        {
                            //HOperatorSet.ReduceDomain(Image, RegionConnectionOut_temp, out Domain_Image);
                            //HOperatorSet.CropDomain(Domain_Image, out ImagePart);

                            zoomImage(Image, RegionConnectionOut_temp, out ImagePart);
                            NgTypeInfo iNgInfo = new NgTypeInfo();
                            iNgInfo.m_hImage = ImagePart.CopyObj(1, 1);//缺陷小图片
                            iNgInfo.m_hRegion = RegionConnectionOut_temp.CopyObj(1, 1);
                            iNgInfo.m_strNgTypeName = Defct1Enum.直边掉料;//缺陷位置类型
                            iNgInfo.m_fNgTypArea = Area* AreaCali;//缺陷面积
                            iNgInfo.m_fNgTypeAbsPixelPos = Convert.ToInt64(RowCenter.D);//Y坐标
                            iNgInfo.m_fNgTypHeight = Hf;//缺陷高度\长度
                            iNgInfo.m_fNgTypWidth = Wf;//缺陷宽度
                            iNgInfo.sAttachInfo = "";//缺陷附加信息

                            string NGType = "边缘掉料";
                            bool isExStd = ImageShow(iNgInfo, NGType);

                        }


                    }

                }
                //多于10个则,融合在一起
                else
                {

                    HOperatorSet.Union1(NG_ROI, out RegionConnectionOut_temp);

                    HOperatorSet.AreaCenter(RegionConnectionOut_temp, out Area, out RowCenter, out Colcenter);

                    HOperatorSet.SmallestRectangle1(RegionConnectionOut_temp, out Row1, out Column1, out Row2, out Column2);

                    double w_f = (Column2.D - Column1.D);
                    double h_f = (Row2.D - Row1.D);

                    double Wf = w_f * WidthCali;
                    double Hf = h_f * HeightCali;

                    bool isOk = false;
                    //判断是否在边缘
                    //if (isLeft)
                    //{
                    //    if ((Column1.D - Lx) < isEdgePixelValue)
                    //    {
                    //        isOk = false;
                    //    }
                    //}
                    //else
                    //{
                    //    if ((Rx - Column2.D) < isEdgePixelValue)
                    //    {
                    //        isOk = false;
                    //    }
                    //}
                    if (!isOk)
                    {
                        zoomImage(Image, RegionConnectionOut_temp, out ImagePart);
                        NgTypeInfo iNgInfo = new NgTypeInfo();
                        iNgInfo.m_hImage = ImagePart.CopyObj(1, 1);//缺陷小图片
                        iNgInfo.m_hRegion = RegionConnectionOut_temp.CopyObj(1, 1);
                        iNgInfo.m_strNgTypeName = Defct1Enum.直边掉料;//缺陷位置类型
                        iNgInfo.m_fNgTypArea = Area * AreaCali;//缺陷面积
                        iNgInfo.m_fNgTypeAbsPixelPos = Convert.ToInt64(RowCenter.D);//Y坐标
                        iNgInfo.m_fNgTypHeight = Hf;//缺陷高度\长度
                        iNgInfo.m_fNgTypWidth = Wf;//缺陷宽度
                        iNgInfo.sAttachInfo = "";//缺陷附加信息
                        string NGType = "边缘掉料";
                        bool isExStd = ImageShow(iNgInfo, NGType);

                    }
                }
            }
            catch (Exception ex)
            {
                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                 Log4netRecord.ErrorLog.Fatal(str + ex.Message);

            }
            finally
            {
                Domain_Image?.Dispose();
                NG_ROI?.Dispose();
                ImagePart?.Dispose();
                RegionConnectionOut_temp?.Dispose();
            }
        }



        /// <summary>
        /// 直边余料
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="DetectionROI"></param>
        /// <param name="ShieldROI"></param>
        /// <param name="MQDefects"></param>
        public void ZBYuLiao(HObject Image, HObject DetectionROI,
            HObject ShieldROI, ParamsMQDefects MQDefects)
        {
            HTuple Lx = 0;
            HTuple Rx = 0;
            //直边余料的检测区域
            HObject FindPosunEdgeRect_L = null;
            HOperatorSet.GenEmptyObj(out FindPosunEdgeRect_L);

            //直边余料的检测区域
            HObject FindPosunEdgeRect_R = null;
            HOperatorSet.GenEmptyObj(out FindPosunEdgeRect_R);
            try
            {
                Find_YuLiao(Image, DetectionROI, ShieldROI,
                    Lx, Rx, true, MQDefects);


            }
            catch (Exception ex)
            {
                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                 Log4netRecord.ErrorLog.Fatal(str + ex.Message);

            }

            finally
            {
                FindPosunEdgeRect_L?.Dispose();
                FindPosunEdgeRect_R?.Dispose();

            }
        }
        /// <summary>
        /// 直边余料检测
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="DetectionROI"></param>
        /// <param name="Shield_ROI"></param>
        /// <param name="Lx"></param>
        /// <param name="Rx"></param>
        /// <param name="isLeft"></param>
        /// <param name="MQDefects"></param>
        private void Find_YuLiao(HObject Image,
            HObject DetectionROI, HObject Shield_ROI, HTuple Lx,
            HTuple Rx, bool isLeft, ParamsMQDefects MQDefects)
        {
            HTuple Row1, Column1, Row2, Column2;
            HObject Domain_Image = null;
            HOperatorSet.GenEmptyObj(out Domain_Image);
            HObject NG_ROI = null;
            HOperatorSet.GenEmptyObj(out NG_ROI);

            //临时截图
            HObject ImagePart;
            HOperatorSet.GenEmptyObj(out ImagePart);

            HObject RegionConnectionOut_temp;
            HOperatorSet.GenEmptyObj(out RegionConnectionOut_temp);


            HTuple Area, RowCenter, Colcenter;

            HTuple tempArea, tempRowCenter, tempColcenter;

            //是否距离边缘,像素判断
            double isEdgePixelValue = 8;

            try
            {

                HOperatorSet.Difference(DetectionROI, Shield_ROI, out DetectionROI);//根据输入直边余

                //=========【2】阈值提取直边余料
                HOperatorSet.ReduceDomain(Image, DetectionROI, out Domain_Image);

                //=========================================================================================================================================
                //=========【3】阈值提取直边余料
                HOperatorSet.Threshold(Domain_Image, out NG_ROI, MQDefects.num_ZBYL_Threshold_Min, MQDefects.num_ZBYL_Threshold_Max);
                HOperatorSet.OpeningRectangle1(NG_ROI, out NG_ROI, 2, 3);//过滤噪点
                HOperatorSet.ClosingRectangle1(NG_ROI, out NG_ROI, 25, 25);//连接分散的瑕疵
                HOperatorSet.Connection(NG_ROI, out NG_ROI);
                HOperatorSet.SelectShape(NG_ROI, out NG_ROI, "area", "and", MQDefects.num_ZBYL_MinArea / AreaCali, 10e+11);
                HOperatorSet.AreaCenter(NG_ROI, out tempArea, out tempRowCenter, out tempColcenter);

                //最多10个直边破损,
                if (tempArea.Length < 10)
                {

                    for (int i = 0; i < tempArea.Length; i++)
                    {
                        HOperatorSet.SelectObj(NG_ROI, out RegionConnectionOut_temp, i + 1);

                        HOperatorSet.AreaCenter(RegionConnectionOut_temp, out Area, out RowCenter, out Colcenter);

                        HOperatorSet.SmallestRectangle1(RegionConnectionOut_temp, out Row1, out Column1, out Row2, out Column2);

                        double w_f = (Column2.D - Column1.D);
                        double h_f = (Row2.D - Row1.D);

                        double Wf = w_f * WidthCali;
                        double Hf = h_f * HeightCali;
                        bool isOk = false;
                        //判断是否在边缘
                        //if (isLeft)
                        //{
                        //    if ((Column1.D - Lx) < isEdgePixelValue)
                        //    {
                        //        isOk = false;
                        //    }
                        //}
                        //else
                        //{
                        //    if ((Rx - Column2.D) < isEdgePixelValue)
                        //    {
                        //        isOk = false;
                        //    }
                        //}
                        if (!isOk)
                        {
                            zoomImage(Image, RegionConnectionOut_temp, out ImagePart);
                            NgTypeInfo iNgInfo = new NgTypeInfo();
                            iNgInfo.m_hImage = ImagePart.CopyObj(1, 1);//缺陷小图片
                            iNgInfo.m_hRegion = RegionConnectionOut_temp.CopyObj(1, 1);
                            iNgInfo.m_strNgTypeName = Defct1Enum.直边余料;//缺陷位置类型
                            iNgInfo.m_fNgTypArea = Area * AreaCali;//缺陷面积
                            iNgInfo.m_fNgTypeAbsPixelPos = Convert.ToInt64(RowCenter.D);//Y坐标
                            iNgInfo.m_fNgTypHeight = Hf;//缺陷高度\长度
                            iNgInfo.m_fNgTypWidth = Wf;//缺陷宽度
                            iNgInfo.sAttachInfo = "";//缺陷附加信息
                            string NGType = "直边余料";
                            bool isExStd = ImageShow(iNgInfo, NGType);

                        }


                    }

                }
                //多于10个则,融合在一起
                else
                {

                    HOperatorSet.Union1(NG_ROI, out RegionConnectionOut_temp);

                    HOperatorSet.AreaCenter(RegionConnectionOut_temp, out Area, out RowCenter, out Colcenter);

                    HOperatorSet.SmallestRectangle1(RegionConnectionOut_temp, out Row1, out Column1, out Row2, out Column2);

                    double w_f = (Column2.D - Column1.D);
                    double h_f = (Row2.D - Row1.D);

                    double Wf = w_f * WidthCali;
                    double Hf = h_f * HeightCali;

                    bool isOk = false;
                    //判断是否在边缘
                    //if (isLeft)
                    //{
                    //    if ((Column1.D - Lx) < isEdgePixelValue)
                    //    {
                    //        isOk = false;
                    //    }
                    //}
                    //else
                    //{
                    //    if ((Rx - Column2.D) < isEdgePixelValue)
                    //    {
                    //        isOk = false;
                    //    }
                    //}
                    if (!isOk)
                    {

                        zoomImage(Image, RegionConnectionOut_temp, out ImagePart);
                        NgTypeInfo iNgInfo = new NgTypeInfo();
                        iNgInfo.m_hImage = ImagePart.CopyObj(1, 1);//缺陷小图片
                        iNgInfo.m_hRegion = RegionConnectionOut_temp.CopyObj(1, 1);
                        iNgInfo.m_strNgTypeName = Defct1Enum.直边余料;//缺陷位置类型
                        iNgInfo.m_fNgTypArea = Area* AreaCali;//缺陷面积
                        iNgInfo.m_fNgTypeAbsPixelPos = Convert.ToInt64(RowCenter.D);//Y坐标
                        iNgInfo.m_fNgTypHeight = Hf;//缺陷高度\长度
                        iNgInfo.m_fNgTypWidth = Wf;//缺陷宽度
                        iNgInfo.sAttachInfo = "";//缺陷附加信息
                        string NGType = "直边余料";
                        bool isExStd = ImageShow(iNgInfo, NGType);
                    }


                }

            }
            catch (Exception ex)
            {
                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                 Log4netRecord.ErrorLog.Fatal(str + ex.Message);

            }
            finally
            {
                Domain_Image?.Dispose();
                NG_ROI?.Dispose();
                ImagePart?.Dispose();
                RegionConnectionOut_temp?.Dispose();
            }
        }






        /// <summary>
        /// 检测横向条痕【eg:未冷压检测】
        /// Time:2023.09.15
        /// </summary>
        public void Test_WLY(HObject Img, HObject TestROI, double dMinGary,
            double dDevGary, out bool bResult, out HObject oROI)
        {
            bResult = true;
            HOperatorSet.GenEmptyObj(out oROI);
            HTuple MinGray = 0;
            HObject Domain_Image = null;
            HObject ROI = null;
            HTuple HorMaxGray = null, HorMinGray = null, HorMeanGray = null;
            bool bShowROI = false;
            try
            {
                HTuple t1, t2;
                double dTime;
                HOperatorSet.CountSeconds(out t1);


                HTuple ImgH, ImgW;
                HOperatorSet.GetImageSize(Img, out ImgW, out ImgH);
                HTuple row1, col, row2, col2;
                HOperatorSet.SmallestRectangle1(TestROI, out row1, out col, out row2, out col2);
                HTuple HorProjection, VertProjection;
                HOperatorSet.GrayProjections(TestROI, Img, "simple", out HorProjection, out VertProjection);//计算水平和垂直灰度值预测



                List<double> listGray1 = new List<double>();
                List<double> listGray2 = new List<double>();
                #region >>>取最大值取差异值
                //取最大值附近的灰度
                HTuple HorMaxGray_y;
                HOperatorSet.TupleMax(HorProjection, out HorMaxGray);
                HOperatorSet.TupleMin(HorProjection, out HorMinGray);
                HOperatorSet.TupleMean(HorProjection, out HorMeanGray);
                HOperatorSet.TupleFind(HorProjection, HorMaxGray, out HorMaxGray_y);



                HTuple HorProjection1, HorProjection2;
                int iTestHeight = 180;
                int iTestMove = 120;
                for (int i = 0; i < HorMaxGray_y.Length; i++)
                {
                    if (i >= 10) break;

                    double yIndex1 = HorMaxGray_y.TupleSelect(i).D - (iTestHeight + iTestMove);
                    double yIndex2 = HorMaxGray_y.TupleSelect(i).D - (iTestMove);
                    yIndex1 = HorMaxGray_y.TupleSelect(i).D - (iTestHeight + iTestMove);
                    yIndex2 = HorMaxGray_y.TupleSelect(i).D - (iTestMove);
                    if (yIndex1 < 0)
                    {
                        double py = 0 - yIndex1;
                        yIndex1 = yIndex1 + py;
                        yIndex2 = yIndex2 + py;
                    }
                    if (yIndex2 >= HorProjection.Length)
                    {
                        double py = yIndex2 - (HorProjection.Length - 1);
                        yIndex1 = yIndex1 - py;
                        yIndex2 = yIndex2 - py;
                    }
                    if (bShowROI)
                    {
                        HOperatorSet.GenRectangle1(out ROI, yIndex1, 1, yIndex1, ImgW - 1);
                        HOperatorSet.Intersection(ROI, TestROI, out ROI);
                        //HOperatorSet.DispObj(ROI, WinHandle);
                    }
                    HOperatorSet.TupleSelectRange(HorProjection, (int)yIndex1, (int)yIndex2, out HorProjection1);
                    yIndex1 = HorMaxGray_y.TupleSelect(i).D + (iTestMove);
                    yIndex2 = HorMaxGray_y.TupleSelect(i).D + (iTestHeight + iTestMove);
                    if (yIndex1 < 0)
                    {
                        double py = 0 - yIndex1;
                        yIndex1 = yIndex1 + py;
                        yIndex2 = yIndex2 + py;
                    }
                    if (yIndex2 >= HorProjection.Length)
                    {
                        double py = yIndex2 - (HorProjection.Length - 1);
                        yIndex1 = yIndex1 - py;
                        yIndex2 = yIndex2 - py;
                    }
                    if (bShowROI)
                    {
                        HOperatorSet.GenRectangle1(out ROI, yIndex1, 1, yIndex1, ImgW - 1);
                        HOperatorSet.Intersection(ROI, TestROI, out ROI);
                        //HOperatorSet.DispObj(ROI, WinHandle);
                    }
                    HOperatorSet.TupleSelectRange(HorProjection, (int)yIndex1, (int)yIndex2, out HorProjection2);




                    HTuple HorPro1MeanGray, HorPro2MeanGray;
                    HOperatorSet.TupleMean(HorProjection1, out HorPro1MeanGray);
                    HOperatorSet.TupleMean(HorProjection2, out HorPro2MeanGray);

                    listGray1.Add(HorPro1MeanGray.D);
                    listGray2.Add(HorPro2MeanGray.D);
                }
                #endregion


                List<double> listSubUpGray = new List<double>();
                List<double> listSubDoGray = new List<double>();
                #region >>>错位相减求取差异值
                //错位相减求取差异值
                int iSubHeight = 200;
                int step = 1;
                int iSubHorProHeight = HorProjection.Length - iSubHeight - 1;
                HTuple HorProjection3;
                HOperatorSet.TupleSelectRange(HorProjection, 0, iSubHorProHeight, out HorProjection1);
                HOperatorSet.TupleSelectRange(HorProjection, iSubHeight, iSubHeight + iSubHorProHeight, out HorProjection2);
                HOperatorSet.TupleSub(HorProjection2, HorProjection1, out HorProjection3);

                #region >>>错位相减求取差异值 最值
                HTuple Hor3MaxGray_y;
                HTuple HorPro3MinGray, HorPro3MaxGray;
                HOperatorSet.TupleMax(HorProjection3, out HorPro3MaxGray);
                HOperatorSet.TupleMin(HorProjection3, out HorPro3MinGray);
                if (Math.Abs(HorPro3MaxGray.D) > Math.Abs(HorPro3MinGray.D))
                    HOperatorSet.TupleFind(HorProjection3, HorPro3MaxGray, out Hor3MaxGray_y);
                else
                    HOperatorSet.TupleFind(HorProjection3, HorPro3MinGray, out Hor3MaxGray_y);

                for (int i = 0; i < Hor3MaxGray_y.Length; i++)
                {
                    if (i >= 10) break;

                    double ySubIndexUp1 = Hor3MaxGray_y.TupleSelect(i).D - (iTestHeight + iTestMove);
                    double ySubIndexUp2 = Hor3MaxGray_y.TupleSelect(i).D - (iTestMove);
                    if (ySubIndexUp1 < 0)
                    {
                        double py = 0 - ySubIndexUp1;
                        ySubIndexUp1 = ySubIndexUp1 + py;
                        ySubIndexUp2 = ySubIndexUp2 + py;
                    }
                    if (ySubIndexUp2 >= HorProjection3.Length)
                    {
                        double py = ySubIndexUp2 - (HorProjection.Length - 1);
                        ySubIndexUp1 = ySubIndexUp1 - py;
                        ySubIndexUp2 = ySubIndexUp2 - py;
                    }
                    double ySubIndexDo1 = Hor3MaxGray_y.TupleSelect(i).D + (iTestMove);
                    double ySubIndexDo2 = Hor3MaxGray_y.TupleSelect(i).D + (iTestHeight + iTestMove);
                    if (ySubIndexDo1 < 0)
                    {
                        double py = 0 - ySubIndexDo1;
                        ySubIndexDo1 = ySubIndexDo1 + py;
                        ySubIndexDo2 = ySubIndexDo2 + py;
                    }
                    if (ySubIndexDo2 >= HorProjection3.Length)
                    {
                        double py = ySubIndexDo2 - (HorProjection.Length - 1);
                        ySubIndexDo1 = ySubIndexDo1 - py;
                        ySubIndexDo2 = ySubIndexDo2 - py;
                    }
                    HOperatorSet.TupleSelectRange(HorProjection, (int)ySubIndexUp1, (int)ySubIndexUp2, out HorProjection1);
                    HOperatorSet.TupleSelectRange(HorProjection, (int)ySubIndexDo1, (int)ySubIndexDo2, out HorProjection2);
                    HTuple HorPro1MeanGray, HorPro2MeanGray;
                    HOperatorSet.TupleMean(HorProjection1, out HorPro1MeanGray);
                    HOperatorSet.TupleMean(HorProjection2, out HorPro2MeanGray);

                    listSubUpGray.Add(HorPro1MeanGray.D);
                    listSubDoGray.Add(HorPro2MeanGray.D);

                    if (bShowROI)
                    {
                        HOperatorSet.GenRectangle1(out ROI, ySubIndexUp1, 1, ySubIndexUp2, ImgW - 1);
                        HOperatorSet.Intersection(ROI, TestROI, out ROI);
                        //HOperatorSet.DispObj(ROI, WinHandle);

                        HOperatorSet.GenRectangle1(out ROI, ySubIndexDo1, 1, ySubIndexDo2, ImgW - 1);
                        HOperatorSet.Intersection(ROI, TestROI, out ROI);
                        //HOperatorSet.DispObj(ROI, WinHandle);
                    }
                }
                #endregion

                #region >>>错位相减求取差异值 极值
                HTuple function;
                HOperatorSet.CreateFunct1dArray(HorProjection3, out function);//根据灰度轮廓线创建1D函数
                HOperatorSet.SmoothFunct1dMean(function, 211, 10, out function);//对轮廓线平滑
                HTuple zeroCrossings;
                HOperatorSet.ZeroCrossingsFunct1d(function, out zeroCrossings);
                for (int i = 0; i < zeroCrossings.Length; i++)
                {
                    if (i >= 10) break;

                    double ySubIndexUp1 = zeroCrossings.TupleSelect(i).D - (iTestHeight + iTestMove);
                    double ySubIndexUp2 = zeroCrossings.TupleSelect(i).D - (iTestMove);
                    if (ySubIndexUp1 < 0)
                    {
                        double py = 0 - ySubIndexUp1;
                        ySubIndexUp1 = ySubIndexUp1 + py;
                        ySubIndexUp2 = ySubIndexUp2 + py;
                    }
                    if (ySubIndexUp2 >= HorProjection3.Length)
                    {
                        double py = ySubIndexUp2 - (HorProjection.Length - 1);
                        ySubIndexUp1 = ySubIndexUp1 - py;
                        ySubIndexUp2 = ySubIndexUp2 - py;
                    }
                    double ySubIndexDo1 = zeroCrossings.TupleSelect(i).D + (iTestMove);
                    double ySubIndexDo2 = zeroCrossings.TupleSelect(i).D + (iTestHeight + iTestMove);
                    if (ySubIndexDo1 < 0)
                    {
                        double py = 0 - ySubIndexDo1;
                        ySubIndexDo1 = ySubIndexDo1 + py;
                        ySubIndexDo2 = ySubIndexDo2 + py;
                    }
                    if (ySubIndexDo2 >= HorProjection3.Length)
                    {
                        double py = ySubIndexDo2 - (HorProjection.Length - 1);
                        ySubIndexDo1 = ySubIndexDo1 - py;
                        ySubIndexDo2 = ySubIndexDo2 - py;
                    }
                    HOperatorSet.TupleSelectRange(HorProjection, (int)ySubIndexUp1, (int)ySubIndexUp2, out HorProjection1);
                    HOperatorSet.TupleSelectRange(HorProjection, (int)ySubIndexDo1, (int)ySubIndexDo2, out HorProjection2);
                    HTuple HorPro1MeanGray, HorPro2MeanGray;
                    HOperatorSet.TupleMean(HorProjection1, out HorPro1MeanGray);
                    HOperatorSet.TupleMean(HorProjection2, out HorPro2MeanGray);

                    listSubUpGray.Add(HorPro1MeanGray.D);
                    listSubDoGray.Add(HorPro2MeanGray.D);

                    //bShowROI = true;
                    if (bShowROI)
                    {
                        HOperatorSet.GenRectangle1(out ROI, ySubIndexUp1, 1, ySubIndexUp2, ImgW - 1);
                        HOperatorSet.Intersection(ROI, TestROI, out ROI);
                        //HOperatorSet.DispObj(ROI, WinHandle);

                        HOperatorSet.GenRectangle1(out ROI, ySubIndexDo1, 1, ySubIndexDo2, ImgW - 1);
                        HOperatorSet.Intersection(ROI, TestROI, out ROI);
                        //HOperatorSet.DispObj(ROI, WinHandle);
                    }
                }
                #endregion
                #endregion




                #region >>>计算
                List<double> listGray = new List<double>();
                listGray.AddRange(listGray1);
                listGray.AddRange(listGray2);
                listGray.AddRange(listSubUpGray);
                listGray.AddRange(listSubDoGray);

                double dminGray = HorMeanGray.D;
                double dmaxGray = HorMeanGray.D;

                if (listGray.Count > 0)
                {
                    dminGray = listGray.Min();
                    dmaxGray = listGray.Max();
                }
                double devGary = dmaxGray - dminGray;
                bool isExStd = devGary > dDevGary || dminGray < dMinGary;



                if (isExStd)
                {
                    bResult = false;
                    HTuple threshold = dminGray + 5;
                    HOperatorSet.ReduceDomain(Img, TestROI, out Domain_Image);
                    HOperatorSet.Threshold(Domain_Image, out oROI, 0, threshold);


                    //HOperatorSet.SetColor(WinHandle, "red");
                    //HOperatorSet.DispObj(oROI, WinHandle);
                }


                #endregion

                HOperatorSet.CountSeconds(out t2);
                dTime = ((t2 - t1) * 1000).D;
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Fatal(ex + "HorMin：" + HorMinGray.ToString());
            }
            finally
            {
                Domain_Image?.Dispose();
            }
        }


        /// <summary>
        /// 负极 缺陷分类
        /// </summary>
        private void defectClassification_NotCathode(HObject NG_ROI)
        {
            HObject ho_RegionConnectionOut, dilaho_RegionConnectionOut;
            HOperatorSet.GenEmptyObj(out ho_RegionConnectionOut);
            HOperatorSet.GenEmptyObj(out dilaho_RegionConnectionOut);
            int[] resultArray = null;//存放模型给出的缺陷类别序号，其顺序与ho_Images中的图像排列顺序对应
            double[] scores = null;     //存放模型给出的缺陷类别概率，其顺序与ho_Images中的图像排列顺序对应
            try
            {
                //=======================================================================================================================
                //=======【】传入0张图片给服务器会导致服务器挂掉，有缺陷时才进入分类
                if (NgImagePart_Count > 0)
                {
                    //=======================================================================================================================
                    //=======【】
                    string classRst = "";//记录分类结果以及概率
                    double Times = 0;//分类时间

                    int status = ClassDefect.Classification_channel(ho_Images, out resultArray, out Times);
                    if (status < 0)
                    {
                         Log4netRecord.ErrorLog.Error("分类异常，异常结果：" + status.ToString());
                    }
                    classRst = string.Join(",", resultArray);//分类结果记录
                    //HOperatorSet.SetColor(WinHandle, "red");
                    //HOperatorSet.SetTposition(WinHandle, ImageHeight - 1150, 10);
                    //HOperatorSet.WriteString(WinHandle, $"ClientExeR：{Times}ms ({classRst})");




                    HTuple min_gray = 0, max_gray = 0, image_num = 10;
                    HTuple x1, y1, x2, y2;
                    //=======================================================================================================================
                    //=======【】
                    for (int i = 0; i < NgImagePart_Count; i++)
                    {
                        //=======【】
                        if (i >= image_num)
                        {
                            Array.Resize<int>(ref resultArray, i + 1);
                            resultArray[i] = -1;
                        }
                        ClassificationDefectInfo defectInfos = DefectInfos[i];
                        int defectIndex = defectInfos.NgROIIndex;
                        double area = defectInfos.NgArea;
                        double hv_ngWidth = defectInfos.NgWidth;
                        double hv_defectLength = defectInfos.NgHigh;
                        double NG_gray = defectInfos.NgMeanGray;



                        ho_RegionConnectionOut.Dispose();
                        //=======================================================================================================================
                        //=======【】传统算法判断
                        #region >>>临时二次判定防止漏杀漏金属
                        HOperatorSet.SelectObj(NG_ROI, out ho_RegionConnectionOut, defectIndex);
                        HOperatorSet.SmallestRectangle1(ho_RegionConnectionOut, out y1, out x1, out y2, out x2);
                        HOperatorSet.Threshold(defectInfos.NgImage, out dilaho_RegionConnectionOut, 0, 255);
                        HOperatorSet.GrayFeatures(dilaho_RegionConnectionOut, defectInfos.NgImage, "min", out min_gray);
                        HOperatorSet.GrayFeatures(dilaho_RegionConnectionOut, defectInfos.NgImage, "max", out max_gray);
                        #endregion

                        //=======================================================================================================================
                        //=======【】EV 阳极(EvPos)分类器（v3.15.0）返回           
                        //字符与缺陷类型对照表：
                        //0-------------白点
                        //1-------------表面裂纹
                        //2-------------黑点
                        //3-------------划痕
                        //4-------------极片破损
                        //5-------------接带
                        //6-------------漏金属
                        //7-------------气泡
                        //8-------------压痕
                        //9-------------褶皱

                        //=======================================================================================================================
                        //=======【】NG打标采集，小图显示存图
                        double dNgArea = area;
                        double dNgHigh = hv_defectLength;
                        double dNgWidth = hv_ngWidth;
                        NgTypeInfo iNgInfo = new NgTypeInfo();
                        HTuple tempArea, row, column;
                        HOperatorSet.AreaCenter(ho_RegionConnectionOut, out tempArea, out row, out column);
                        iNgInfo.m_hImage = defectInfos.NgImage.CopyObj(1, 1);//缺陷小图片
                        iNgInfo.m_hRegion = ho_RegionConnectionOut.CopyObj(1, 1);
                        iNgInfo.m_fNgTypArea = dNgArea;//缺陷面积
                        iNgInfo.m_fNgTypeAbsPixelPos = Convert.ToInt64(row.D);//Y坐标
                        iNgInfo.m_fNgTypHeight = dNgHigh;//缺陷高度\长度
                        iNgInfo.m_fNgTypWidth = dNgWidth;//缺陷宽度
                        iNgInfo.sAttachInfo = "";//缺陷附加信息

                        string NGType = "未分类";
                        switch (resultArray[i])
                        {
                            case 0:
                                iNgInfo.m_strNgTypeName = Defct1Enum.白点;//缺陷位置类型ID
                                iNgInfo.Diameter = getDiameter(ho_RegionConnectionOut) * HeightCali;
                                break;
                            case 1:
                                iNgInfo.m_strNgTypeName = Defct1Enum.表面裂纹;//缺陷位置类型ID
                                break;
                            case 2:
                                iNgInfo.m_strNgTypeName = Defct1Enum.黑点;//缺陷位置类型ID
                                iNgInfo.Diameter = getDiameter(ho_RegionConnectionOut) * HeightCali;
                                break;
                            case 3:
                                iNgInfo.m_strNgTypeName = Defct1Enum.划痕;//缺陷位置类型ID
                                break;
                            case 4:
                                iNgInfo.m_strNgTypeName = Defct1Enum.极片破损;//缺陷位置类型ID
                                break;
                            case 5:
                                iNgInfo.m_strNgTypeName = Defct1Enum.接带;
                                break;
                            case 6:
                                iNgInfo.m_strNgTypeName = Defct1Enum.漏金属;
                                break;
                            case 7:
                                iNgInfo.m_strNgTypeName = Defct1Enum.气泡;//缺陷位置类型ID
                                iNgInfo.Diameter = getDiameter(ho_RegionConnectionOut) * HeightCali;
                                break;
                            case 8:
                                iNgInfo.m_strNgTypeName = Defct1Enum.压痕;//缺陷位置类型ID
                                break;
                            case 9:
                                iNgInfo.m_strNgTypeName = Defct1Enum.褶皱;//缺陷位置类型ID
                                break;
                            case 10:
                                iNgInfo.m_strNgTypeName = Defct1Enum.其它;//其它
                                break;
                            default:
                                //其它算其它类型
                                iNgInfo.m_strNgTypeName = Defct1Enum.其它;
                                break;
                        }


                        bool isExStd = ImageShow(iNgInfo, NGType);
                         
                    }
                }
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Error(ex);
            }
            finally
            {
                ho_RegionConnectionOut.Dispose();
                dilaho_RegionConnectionOut.Dispose();
            }
        }
        /// <summary>
        /// 正极  缺陷分类 
        /// </summary>
        private void defectClassification_Cathode(HObject NG_ROI)
        {
            //==================================================================================================================================================
            //=======【】输入参数
            #region >>>输入参数
            int Whitethreshold = 150;
            int Blackthreshold = 20;
            double MinArea_White = 20;
            double MinArea_Black = 20;
            double hv_Radius = 9;


            double dJDAreaNG = 100;

            #endregion

            HObject ho_RegionConnectionOut, dilaho_RegionConnectionOut;
            HOperatorSet.GenEmptyObj(out ho_RegionConnectionOut);
            HOperatorSet.GenEmptyObj(out dilaho_RegionConnectionOut);

            int[] resultArray = new int[NgImagePart_Count];//存放模型给出的缺陷类别序号，其顺序与ho_Images中的图像排列顺序对应
            try
            {
                //======================================================================================================================
                //========【】
                if (NgImagePart_Count > 0) //传入0张图片给服务器会导致服务器挂掉，有缺陷时才进入分类
                {
                    HTuple x1, y1, x2, y2;
                    //======================================================================================================================
                    //========【】
                    for (int i = 0; i < NgImagePart_Count; i++)
                    {
                        bool isNowNG_DB = false;//已经确认打标标志位
                        resultArray[i] = 10;
                        //======================================================================================================================
                        //========【】分类器最大10张小图，若resultArray是分类器的结果，故其最大的长度只能为10   但缺陷小图可能大于10，所以避免数组越界
                        if (i >= 10)
                        {
                            Array.Resize<int>(ref resultArray, i + 1);
                            resultArray[i] = -1;
                        }
                        ClassificationDefectInfo defectInfos = DefectInfos[i];
                        int defectIndex = defectInfos.NgROIIndex;
                        double area = defectInfos.NgArea;
                        double hv_ngWidth = defectInfos.NgWidth;
                        double hv_defectLength = defectInfos.NgHigh;
                        double NG_gray = defectInfos.NgMeanGray;


                        ho_RegionConnectionOut.Dispose();
                        HOperatorSet.SelectObj(NG_ROI, out ho_RegionConnectionOut, defectIndex);
                        HOperatorSet.SmallestRectangle1(ho_RegionConnectionOut, out y1, out x1, out y2, out x2);

                        //=======================================================================================================================
                        //=======【】传统算法分类
                        #region >>>传统算法分类  2021.05.09  @Xuyongyi
                        //========================================================================================================================
                        //========== 【】
                        //返回字符与缺陷类型对照表：
                        //0 白点
                        //1 黑点
                        //2 裂纹
                        //3 漏金属
                        //4 破损
                        //5 气泡
                        //6 脱碳
                        //7 褶皱开裂
                        HTuple min_gray = 0, max_gray = 0;
                        HTuple hv_area, hv_RowCenter, hv_Colcenter;
                        dilaho_RegionConnectionOut.Dispose();
                        HOperatorSet.Threshold(defectInfos.NgImage, out dilaho_RegionConnectionOut, 0, 255);
                        HOperatorSet.GrayFeatures(dilaho_RegionConnectionOut, defectInfos.NgImage, "min", out min_gray);
                        HOperatorSet.GrayFeatures(dilaho_RegionConnectionOut, defectInfos.NgImage, "max", out max_gray);

                        //=======================================================================================================================
                        //=======【】计算区域圆形度
                        HTuple hCircularity;
                        HTuple Rectangle;
                        HOperatorSet.Circularity(ho_RegionConnectionOut, out hCircularity);//计算区域圆形度
                        HOperatorSet.Rectangularity(ho_RegionConnectionOut, out Rectangle);//标签矩形度

                        //=======================================================================================================================
                        //=======【】计算亮瑕疵面积
                        HTuple Wthreshold_MaxPixArea = 0;
                        if (max_gray > Whitethreshold)
                        {
                            dilaho_RegionConnectionOut.Dispose();
                            HOperatorSet.Threshold(defectInfos.NgImage, out dilaho_RegionConnectionOut, Whitethreshold - 10, 255);
                            HOperatorSet.AreaCenter(dilaho_RegionConnectionOut, out hv_area, out hv_RowCenter, out hv_Colcenter);
                            if (hv_area.Length > 0)
                            {
                                Wthreshold_MaxPixArea = hv_area.TupleMax();
                            }
                        }

                        //=======================================================================================================================
                        //=======【】                    
                        if (Wthreshold_MaxPixArea > 10 && max_gray > Whitethreshold)
                        {
                            resultArray[i] = 3;//漏金属
                        }
                        else if (hCircularity > 0.5)//圆度
                        {
                            resultArray[i] = 5;//气泡
                        }
                        else if (Rectangle > 0.8 && NG_gray > Whitethreshold)
                        {
                            resultArray[i] = 4;//标签
                        }
                        else if (min_gray < Blackthreshold + 5 && NG_gray < Whitethreshold)
                        {
                            resultArray[i] = 7;//褶皱开裂
                        }
                        else
                        {
                            resultArray[i] = 2;//裂纹
                        }

                        ////=======================================================================================================================
                        ////=======【】 
                        //if (Fang)
                        //{
                        //    HOperatorSet.SetFont(WinHandle, "-Courier New-10-*-*-*-*-1-");
                        //    HOperatorSet.SetColor(WinHandle, "green");
                        //    HOperatorSet.SetTposition(WinHandle, y2, x2);
                        //    HOperatorSet.WriteString(WinHandle, resultArray[i].ToString());
                        //    HOperatorSet.SetFont(WinHandle, "-Courier New-14-*-*-*-*-1-");
                        //}
                        #endregion


                        //=======================================================================================================================
                        //=======【】NG打标采集，小图显示存图
                        double dNgArea = area;
                        double dNgHigh = hv_defectLength;
                        double dNgWidth = hv_ngWidth;
                        NgTypeInfo iNgInfo = new NgTypeInfo();
                        HTuple tempArea, row, column;
                        HOperatorSet.AreaCenter(ho_RegionConnectionOut, out tempArea, out row, out column);
                        iNgInfo.m_hImage = defectInfos.NgImage.CopyObj(1, 1);//缺陷小图片
                        iNgInfo.m_hRegion = ho_RegionConnectionOut.CopyObj(1, 1);
                        iNgInfo.m_fNgTypArea = dNgArea;//缺陷面积
                        iNgInfo.m_fNgTypeAbsPixelPos = Convert.ToInt64(row.D);//Y坐标
                        iNgInfo.m_fNgTypHeight = dNgHigh;//缺陷高度\长度
                        iNgInfo.m_fNgTypWidth = dNgWidth;//缺陷宽度
                        iNgInfo.sAttachInfo = "";//缺陷附加信息

                        //0 白点
                        //1 黑点
                        //2 裂纹
                        //3 漏金属
                        //4 破损
                        //5 气泡
                        //6 脱碳
                        //7 褶皱开裂
                        string NGType = "未分类";
                        switch (resultArray[i])
                        {
                            case 0:
                                iNgInfo.m_strNgTypeName = Defct1Enum.白点;//缺陷位置类型ID
                                iNgInfo.Diameter = getDiameter(ho_RegionConnectionOut) * HeightCali;
                                break;
                            case 1:
                                iNgInfo.m_strNgTypeName = Defct1Enum.黑点;//缺陷位置类型ID
                                iNgInfo.Diameter = getDiameter(ho_RegionConnectionOut) * HeightCali;
                                break;
                            case 2:
                                iNgInfo.m_strNgTypeName = Defct1Enum.表面裂纹;//缺陷位置类型ID
                                break;
                            case 3:
                                iNgInfo.m_strNgTypeName = Defct1Enum.漏金属;//缺陷位置类型ID
                                break;

                            case 4:
                                iNgInfo.m_strNgTypeName = Defct1Enum.极片破损;//缺陷位置类型ID
                                break;
                            case 5:
                                iNgInfo.m_strNgTypeName = Defct1Enum.气泡;//

                                iNgInfo.Diameter = getDiameter(ho_RegionConnectionOut) * HeightCali;

                                break;
                           
                            case 7:
                                iNgInfo.m_strNgTypeName = Defct1Enum.褶皱;//缺陷位置类型ID
                                break;
                            case 6:
                            default:

                                iNgInfo.m_strNgTypeName = Defct1Enum.其它;

                                break;
                        }
                       
                        bool isExStd = ImageShow(iNgInfo, NGType);
            
                    }
                }
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Error(ex);
            }
            finally
            {
                ho_RegionConnectionOut.Dispose();
                dilaho_RegionConnectionOut.Dispose();
            }
        }



        /// <summary>
        /// 缩放截图
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="inputRegion"></param>
        /// <param name="Image"></param>
        private void zoomImage(HObject inputImage, HObject inputRegion, out HObject Image)
        {
            Image = null;
            HOperatorSet.GenEmptyObj(out Image);
            HObject Region = null;
            HOperatorSet.GenEmptyObj(out Region);
            HTuple row1, column1, row2, column2;
            try
            {
                HOperatorSet.DilationRectangle1(inputRegion, out Region, 50, 50);
                HOperatorSet.SmallestRectangle1(Region, out row1, out column1, out row2, out column2);
                HOperatorSet.GenRectangle1(out Region, row1, column1, row2, column2);
                HOperatorSet.ReduceDomain(inputImage, Region, out Image);
                HOperatorSet.CropDomain(Image, out Image);
                HOperatorSet.ZoomImageFactor(Image, out Image, 0.25, 0.25, "constant");

            }
            catch (Exception ex)
            {

                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                 Log4netRecord.ErrorLog.Error(ex);

            }
            finally
            {


            }
        }



        /// <summary>
        /// 无缩放截图
        /// </summary>
        /// <param name="inputImage"></param>
        /// <param name="inputRegion"></param>
        /// <param name="Image"></param>
        private void CropImage(HObject inputImage, HObject inputRegion, out HObject Image)
        {
            Image = null;
            HOperatorSet.GenEmptyObj(out Image);
            HObject Region = null;
            HOperatorSet.GenEmptyObj(out Region);
            HTuple row1, column1, row2, column2;
            try
            {
                HOperatorSet.DilationRectangle1(inputRegion, out Region, 50, 50);
                HOperatorSet.SmallestRectangle1(Region, out row1, out column1, out row2, out column2);
                HOperatorSet.GenRectangle1(out Region, row1, column1, row2, column2);
                HOperatorSet.ReduceDomain(inputImage, Region, out Image);
                HOperatorSet.CropDomain(Image, out Image);
            }
            catch (Exception ex)
            {

                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log4netRecord.ErrorLog.Error(ex);

            }
            finally
            {


            }
        }





        /// <summary>
        /// 正极 缺陷分类
        /// </summary>
        private void defectClassification_Cathode_AI(HObject NG_ROI)
        {
            HObject ho_RegionConnectionOut, dilaho_RegionConnectionOut;
            HOperatorSet.GenEmptyObj(out ho_RegionConnectionOut);
            HOperatorSet.GenEmptyObj(out dilaho_RegionConnectionOut);
            int[] resultArray = null;//存放模型给出的缺陷类别序号，其顺序与ho_Images中的图像排列顺序对应
            double[] scores = null;     //存放模型给出的缺陷类别概率，其顺序与ho_Images中的图像排列顺序对应
            try
            {
                //=======================================================================================================================
                //=======【】传入0张图片给服务器会导致服务器挂掉，有缺陷时才进入分类
                if (NgImagePart_Count > 0)
                {
                    //=======================================================================================================================
                    //=======【】
                    string classRst = "";//记录分类结果以及概率
                    double Times = 0;//分类时间

                    int status = ClassDefect.Classification_channel(ho_Images, out resultArray, out Times);
                    if (status < 0)
                    {
                         Log4netRecord.ErrorLog.Error("分类异常，异常结果：" + status.ToString());
                    }
                    classRst = string.Join(",", resultArray);//分类结果记录
                    //HOperatorSet.SetColor(WinHandle, "red");
                    //HOperatorSet.SetTposition(WinHandle, ImageHeight - 1150, 10);
                    //HOperatorSet.WriteString(WinHandle, $"ClientExeR：{Times}ms ({classRst})");




                    HTuple min_gray = 0, max_gray = 0, image_num = 10;
                    HTuple x1, y1, x2, y2;
                    //=======================================================================================================================
                    //=======【】
                    for (int i = 0; i < NgImagePart_Count; i++)
                    {
                        //=======【】
                        if (i >= image_num)
                        {
                            Array.Resize<int>(ref resultArray, i + 1);
                            resultArray[i] = -1;
                        }
                        ClassificationDefectInfo defectInfos = DefectInfos[i];
                        int defectIndex = defectInfos.NgROIIndex;
                        double area = defectInfos.NgArea;
                        double hv_ngWidth = defectInfos.NgWidth;
                        double hv_defectLength = defectInfos.NgHigh;
                        double NG_gray = defectInfos.NgMeanGray;



                        ho_RegionConnectionOut.Dispose();
                        //=======================================================================================================================
                        //=======【】传统算法判断
                        #region >>>临时二次判定防止漏杀漏金属
                        HOperatorSet.SelectObj(NG_ROI, out ho_RegionConnectionOut, defectIndex);
                        HOperatorSet.SmallestRectangle1(ho_RegionConnectionOut, out y1, out x1, out y2, out x2);
                        HOperatorSet.Threshold(defectInfos.NgImage, out dilaho_RegionConnectionOut, 0, 255);
                        HOperatorSet.GrayFeatures(dilaho_RegionConnectionOut, defectInfos.NgImage, "min", out min_gray);
                        HOperatorSet.GrayFeatures(dilaho_RegionConnectionOut, defectInfos.NgImage, "max", out max_gray);
                        #endregion

                        //=======================================================================================================================
                        //=======【】EV 阳极(EvPos)分类器（v3.15.0）返回           
                        //字符与缺陷类型对照表：
                        //0-------------白点
                        //1-------------表面裂纹
                        //2-------------黑点
                        //3-------------划痕
                        //4-------------极片破损
                        //5-------------接带
                        //6-------------漏金属
                        //7-------------气泡
                        //8-------------压痕
                        //9-------------褶皱

                        //=======================================================================================================================
                        //=======【】NG打标采集，小图显示存图
                        double dNgArea = area;
                        double dNgHigh = hv_defectLength;
                        double dNgWidth = hv_ngWidth;
                        NgTypeInfo iNgInfo = new NgTypeInfo();
                        HTuple tempArea, row, column;
                        HOperatorSet.AreaCenter(ho_RegionConnectionOut, out tempArea, out row, out column);
                        iNgInfo.m_hImage = defectInfos.NgImage.CopyObj(1, 1);//缺陷小图片
                        iNgInfo.m_hRegion = ho_RegionConnectionOut.CopyObj(1, 1);
                        iNgInfo.m_fNgTypArea = dNgArea;//缺陷面积
                        iNgInfo.m_fNgTypeAbsPixelPos = Convert.ToInt64(row.D);//Y坐标
                        iNgInfo.m_fNgTypHeight = dNgHigh;//缺陷高度\长度
                        iNgInfo.m_fNgTypWidth = dNgWidth;//缺陷宽度
                        iNgInfo.sAttachInfo = "";//缺陷附加信息

                        string NGType = "未分类";
                        switch (resultArray[i])
                        {
                            case 0:
                                iNgInfo.m_strNgTypeName = Defct1Enum.白点;//缺陷位置类型ID
                                iNgInfo.Diameter = getDiameter(ho_RegionConnectionOut) * HeightCali;
                                break;
                            case 1:
                                iNgInfo.m_strNgTypeName = Defct1Enum.表面裂纹;//缺陷位置类型ID
                                break;
                            case 2:
                                iNgInfo.m_strNgTypeName = Defct1Enum.黑点;//缺陷位置类型ID
                                iNgInfo.Diameter = getDiameter(ho_RegionConnectionOut) * HeightCali;
                                break;
                            case 3:
                                iNgInfo.m_strNgTypeName = Defct1Enum.划痕;//缺陷位置类型ID
                                break;
                            case 4:
                                iNgInfo.m_strNgTypeName = Defct1Enum.极片破损;//缺陷位置类型ID
                                break;
                            case 5:
                                iNgInfo.m_strNgTypeName = Defct1Enum.接带;
                                break;
                            case 6:
                                iNgInfo.m_strNgTypeName = Defct1Enum.漏金属;
                                break;
                            case 7:
                                iNgInfo.m_strNgTypeName = Defct1Enum.气泡;//缺陷位置类型ID
                                iNgInfo.Diameter = getDiameter(ho_RegionConnectionOut) * HeightCali;
                                break;
                            case 8:
                                iNgInfo.m_strNgTypeName = Defct1Enum.压痕;//缺陷位置类型ID
                                break;
                            case 9:
                                iNgInfo.m_strNgTypeName = Defct1Enum.褶皱;//缺陷位置类型ID
                                break;
                            case 10:
                                iNgInfo.m_strNgTypeName = Defct1Enum.其它;//其它
                                break;
                            default:
                                //其它算其它类型
                                iNgInfo.m_strNgTypeName = Defct1Enum.其它;
                                break;
                        }


                        bool isExStd = ImageShow(iNgInfo, NGType);

                    }
                }
            }
            catch (Exception ex)
            {
                 Log4netRecord.ErrorLog.Fatal(ex.Message);
            }
            finally
            {
                ho_RegionConnectionOut.Dispose();
                dilaho_RegionConnectionOut.Dispose();
            }
        }


        /// <summary>
        /// NG项加入队列
        /// </summary>
        private bool ImageShow(NgTypeInfo iNgInfo, string NGType)
        {
            bool OverSpec = false;
            try
            {
                detectInfos.Enqueue(iNgInfo);  
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(ex.Message);
            }

            return OverSpec;
        }

        /// <summary>
        /// 获取直径
        /// </summary>
        /// <param name="inputRegion"></param>
        /// <returns></returns>
        private double getDiameter(HObject inputRegion)
        {
            double Diameter = 0;
            HTuple row, column, radius;
            try
            {

                HOperatorSet.SmallestCircle(inputRegion, out row, out column, out radius);

                //HOperatorSet.GenCircle(out inputRegion, row,  column,  radius);
                //HOperatorSet.DispObj(inputRegion, WinHandle);
                Diameter = radius.D * 2;
                return Diameter;
            }
            catch (Exception ex)
            {

                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                 Log4netRecord.ErrorLog.Fatal(str + ex.Message);

            }
            finally
            {


            }


            return Diameter;
        }



        /// <summary>
        /// 检测AT9
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="DetectionROI"></param>
        /// <param name="Shield_ROI"></param>
        /// <param name="MQDefects"></param>
        public void Detection_AT9(HObject Image,
            HObject DetectionROI, HObject Shield_ROI,
             ParamsMQDefects MQDefects)
        {
            try
            {
                //AT漏金属
                Find_AT_LouJinShu( Image,
             DetectionROI,  Shield_ROI,
              MQDefects);

                //AT破损
                Find_AT_PoSun(Image,
             DetectionROI, Shield_ROI,
              MQDefects);

            }
            catch(Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal( ex.Message);
            }
            finally
            {

            }

        }


        /// <summary>
        /// AT漏金属检测
        /// </summary>
        /// <param name="Image">图像</param>
        /// <param name="DetectionROI">检测区域</param>
        /// <param name="Shield_ROI">屏蔽区域</param>
        /// <param name="MQDefects">参数</param>
        private void Find_AT_LouJinShu(HObject Image,
            HObject DetectionROI, HObject Shield_ROI,
             ParamsMQDefects MQDefects)
        {
            HTuple Row1, Column1, Row2, Column2;
            HObject Domain_Image = null;
            HOperatorSet.GenEmptyObj(out Domain_Image);
            HObject NG_ROI = null;
            HOperatorSet.GenEmptyObj(out NG_ROI);

            //临时截图
            HObject ImagePart;
            HOperatorSet.GenEmptyObj(out ImagePart);

            HObject RegionConnectionOut_temp;
            HOperatorSet.GenEmptyObj(out RegionConnectionOut_temp);


            HTuple Area, RowCenter, Colcenter;

            HTuple tempArea, tempRowCenter, tempColcenter;

            try
            {
                //检测区域 减去 屏蔽区域
                HOperatorSet.Difference(DetectionROI, Shield_ROI, out DetectionROI);

                //=========================================================================================================================================
                //=========【2】阈值提取区域
                HOperatorSet.ReduceDomain(Image, DetectionROI, out Domain_Image);

                //=========================================================================================================================================
                //=========【3】阈值提区域
                HOperatorSet.Threshold(Domain_Image, out NG_ROI, MQDefects.num_AT_LouJinShu_Threshold_Min,
                    MQDefects.num_AT_LouJinShu_Threshold_Max);
                HOperatorSet.Connection(NG_ROI, out NG_ROI);
                HOperatorSet.SelectShape(NG_ROI, out NG_ROI, "area", "and", MQDefects.num_AT_LouJinShu_MinArea / AreaCali, 10e+11);
                HOperatorSet.AreaCenter(NG_ROI, out tempArea, out tempRowCenter, out tempColcenter);
                //最多5个边缘掉料,
                if (tempArea.Length < 5)
                {
                    for (int i = 0; i < tempArea.Length; i++)
                    {
                        HOperatorSet.SelectObj(NG_ROI, out RegionConnectionOut_temp, i + 1);
                        HOperatorSet.AreaCenter(RegionConnectionOut_temp, out Area, out RowCenter, out Colcenter);

                        HOperatorSet.SmallestRectangle1(RegionConnectionOut_temp, out Row1, out Column1, out Row2, out Column2);

                        double w_f = (Column2.D - Column1.D);
                        double h_f = (Row2.D - Row1.D);

                        double Wf = w_f * WidthCali;
                        double Hf = h_f * HeightCali;
                        bool isOk = false;
                       
                        if (!isOk)
                        {
                            zoomImage(Image, RegionConnectionOut_temp, out ImagePart);
                            NgTypeInfo iNgInfo = new NgTypeInfo();
                            iNgInfo.m_hImage = ImagePart.CopyObj(1, 1);//缺陷小图片
                            iNgInfo.m_hRegion = RegionConnectionOut_temp.CopyObj(1, 1);
                            iNgInfo.m_strNgTypeName = Defct1Enum.AT漏金属;//缺陷位置类型
                            iNgInfo.m_fNgTypArea = Area * AreaCali;//缺陷面积
                            iNgInfo.m_fNgTypeAbsPixelPos = Convert.ToInt64(RowCenter.D);//Y坐标
                            iNgInfo.m_fNgTypHeight = Hf;//缺陷高度\长度
                            iNgInfo.m_fNgTypWidth = Wf;//缺陷宽度
                            iNgInfo.sAttachInfo = "";//缺陷附加信息

                            string NGType = "AT漏金属";
                            bool isExStd = ImageShow(iNgInfo, NGType);

                        }


                    }

                }
                //多于5个则,融合在一起
                else
                {
                    HOperatorSet.Union1(NG_ROI, out RegionConnectionOut_temp);
                    HOperatorSet.AreaCenter(RegionConnectionOut_temp, out Area, out RowCenter, out Colcenter);
                    HOperatorSet.SmallestRectangle1(RegionConnectionOut_temp, out Row1, out Column1, out Row2, out Column2);
                    double w_f = (Column2.D - Column1.D);
                    double h_f = (Row2.D - Row1.D);
                    double Wf = w_f * WidthCali;
                    double Hf = h_f * HeightCali;
                    bool isOk = false;
                    if (!isOk)
                    {
                        zoomImage(Image, RegionConnectionOut_temp, out ImagePart);
                        NgTypeInfo iNgInfo = new NgTypeInfo();
                        iNgInfo.m_hImage = ImagePart.CopyObj(1, 1);//缺陷小图片
                        iNgInfo.m_hRegion = RegionConnectionOut_temp.CopyObj(1, 1);
                        iNgInfo.m_strNgTypeName = Defct1Enum.AT漏金属;//缺陷位置类型
                        iNgInfo.m_fNgTypArea = Area * AreaCali;//缺陷面积
                        iNgInfo.m_fNgTypeAbsPixelPos = Convert.ToInt64(RowCenter.D);//Y坐标
                        iNgInfo.m_fNgTypHeight = Hf;//缺陷高度\长度
                        iNgInfo.m_fNgTypWidth = Wf;//缺陷宽度
                        iNgInfo.sAttachInfo = "";//缺陷附加信息
                        string NGType = "AT漏金属";
                        bool isExStd = ImageShow(iNgInfo, NGType);

                    }
                }
            }
            catch (Exception ex)
            {
                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log4netRecord.ErrorLog.Fatal(str + ex.Message);

            }
            finally
            {
                Domain_Image?.Dispose();
                NG_ROI?.Dispose();
                ImagePart?.Dispose();
                RegionConnectionOut_temp?.Dispose();
            }
        }

        /// <summary>
        /// AT破损检测
        /// </summary>
        /// <param name="Image">图像</param>
        /// <param name="DetectionROI">检测区域</param>
        /// <param name="Shield_ROI">屏蔽区域</param>
        /// <param name="MQDefects">参数</param>
        private void Find_AT_PoSun(HObject Image,
            HObject DetectionROI, HObject Shield_ROI,
             ParamsMQDefects MQDefects)
        {
            HTuple Row1, Column1, Row2, Column2;
            HObject Domain_Image = null;
            HOperatorSet.GenEmptyObj(out Domain_Image);
            HObject NG_ROI = null;
            HOperatorSet.GenEmptyObj(out NG_ROI);

            //临时截图
            HObject ImagePart;
            HOperatorSet.GenEmptyObj(out ImagePart);

            HObject RegionConnectionOut_temp;
            HOperatorSet.GenEmptyObj(out RegionConnectionOut_temp);


            HTuple Area, RowCenter, Colcenter;

            HTuple tempArea, tempRowCenter, tempColcenter;

            try
            {
                //检测区域 减去 屏蔽区域
                HOperatorSet.Difference(DetectionROI, Shield_ROI, out DetectionROI);

                //=========================================================================================================================================
                //=========【2】阈值提取区域
                HOperatorSet.ReduceDomain(Image, DetectionROI, out Domain_Image);

                //=========================================================================================================================================
                //=========【3】阈值提区域
                HOperatorSet.Threshold(Domain_Image, out NG_ROI, MQDefects.num_AT_PoSun_Threshold_Min,
                    MQDefects.num_AT_PoSun_Threshold_Max);
                HOperatorSet.Connection(NG_ROI, out NG_ROI);
                HOperatorSet.SelectShape(NG_ROI, out NG_ROI, "area", "and", MQDefects.num_AT_PoSun_MinArea / AreaCali, 10e+11);
                HOperatorSet.AreaCenter(NG_ROI, out tempArea, out tempRowCenter, out tempColcenter);
                //最多5个边缘掉料,
                if (tempArea.Length < 5)
                {
                    for (int i = 0; i < tempArea.Length; i++)
                    {
                        HOperatorSet.SelectObj(NG_ROI, out RegionConnectionOut_temp, i + 1);
                        HOperatorSet.AreaCenter(RegionConnectionOut_temp, out Area, out RowCenter, out Colcenter);

                        HOperatorSet.SmallestRectangle1(RegionConnectionOut_temp, out Row1, out Column1, out Row2, out Column2);

                        double w_f = (Column2.D - Column1.D);
                        double h_f = (Row2.D - Row1.D);

                        double Wf = w_f * WidthCali;
                        double Hf = h_f * HeightCali;
                        bool isOk = false;

                        if (!isOk)
                        {
                            zoomImage(Image, RegionConnectionOut_temp, out ImagePart);
                            NgTypeInfo iNgInfo = new NgTypeInfo();
                            iNgInfo.m_hImage = ImagePart.CopyObj(1, 1);//缺陷小图片
                            iNgInfo.m_hRegion = RegionConnectionOut_temp.CopyObj(1, 1);
                            iNgInfo.m_strNgTypeName = Defct1Enum.AT破损;//缺陷位置类型
                            iNgInfo.m_fNgTypArea = Area * AreaCali;//缺陷面积
                            iNgInfo.m_fNgTypeAbsPixelPos = Convert.ToInt64(RowCenter.D);//Y坐标
                            iNgInfo.m_fNgTypHeight = Hf;//缺陷高度\长度
                            iNgInfo.m_fNgTypWidth = Wf;//缺陷宽度
                            iNgInfo.sAttachInfo = "";//缺陷附加信息

                            string NGType = "AT破损";
                            bool isExStd = ImageShow(iNgInfo, NGType);

                        }


                    }

                }
                //多于5个则,融合在一起
                else
                {
                    HOperatorSet.Union1(NG_ROI, out RegionConnectionOut_temp);
                    HOperatorSet.AreaCenter(RegionConnectionOut_temp, out Area, out RowCenter, out Colcenter);
                    HOperatorSet.SmallestRectangle1(RegionConnectionOut_temp, out Row1, out Column1, out Row2, out Column2);
                    double w_f = (Column2.D - Column1.D);
                    double h_f = (Row2.D - Row1.D);
                    double Wf = w_f * WidthCali;
                    double Hf = h_f * HeightCali;
                    bool isOk = false;
                    if (!isOk)
                    {
                        zoomImage(Image, RegionConnectionOut_temp, out ImagePart);
                        NgTypeInfo iNgInfo = new NgTypeInfo();
                        iNgInfo.m_hImage = ImagePart.CopyObj(1, 1);//缺陷小图片
                        iNgInfo.m_hRegion = RegionConnectionOut_temp.CopyObj(1, 1);
                        iNgInfo.m_strNgTypeName = Defct1Enum.AT破损;//缺陷位置类型
                        iNgInfo.m_fNgTypArea = Area * AreaCali;//缺陷面积
                        iNgInfo.m_fNgTypeAbsPixelPos = Convert.ToInt64(RowCenter.D);//Y坐标
                        iNgInfo.m_fNgTypHeight = Hf;//缺陷高度\长度
                        iNgInfo.m_fNgTypWidth = Wf;//缺陷宽度
                        iNgInfo.sAttachInfo = "";//缺陷附加信息
                        string NGType = "AT破损";
                        bool isExStd = ImageShow(iNgInfo, NGType);

                    }
                }
            }
            catch (Exception ex)
            {
                //取得当前方法类全名 包括命名空间    
                var str = string.Format("类{0}  中的  {1}方法错误：", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName,
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                Log4netRecord.ErrorLog.Fatal(str + ex.Message);

            }
            finally
            {
                Domain_Image?.Dispose();
                NG_ROI?.Dispose();
                ImagePart?.Dispose();
                RegionConnectionOut_temp?.Dispose();
            }
        }







        /// <summary>
        /// 模板匹配  返回-1代表没模型 -2未匹配到 -3错误
        /// </summary>
        /// <param name="image"></param>
        /// <param name="PluginParams"></param>
        /// <param name="Model"></param>
        /// <param name="ModelXld"></param>
        public static int FindShapeModel(HObject image, ParamsMQDefects PluginParams, HTuple Model,HObject ModelContours,
            out HObject ModelXld)
        {
            HOperatorSet.GenEmptyObj(out ModelXld);
            //HObject ModelContours;
            //HOperatorSet.GenEmptyObj(out ModelContours);
            HTuple Row, Col, Angle, Score;
            HTuple HomMat2D;
            try
            {   HTuple AngleStart= (new HTuple(-0.1)).TupleRad();
                HTuple AngleExtent= (new HTuple(0.1)).TupleRad();
                HTuple MinScore = 0.2;
                //匹配个数
                HTuple NumMatches = 1;
                //最大重叠
                HTuple MaxOverlap = 0.5;
                //匹配精度
                HTuple SubPixel = "least_squares";
                //金字塔层数
                HTuple NumLevels = 0;
                //贪婪 的搜索启发式
                HTuple Greediness = 0.9;

                if(Model.Length<1)
                {
                    return -1;
                }
              
                HOperatorSet.FindShapeModel(image, Model, AngleStart,
                    AngleExtent, MinScore, NumMatches, MaxOverlap, SubPixel, NumLevels, Greediness, out Row, out Col, out Angle,
                 out Score);
                if(Row.Length>0)
                {
                    HOperatorSet.VectorAngleToRigid(0, 0, 0, Row, Col, Angle, out HomMat2D);
                    HOperatorSet.GetShapeModelContours(out ModelContours, Model, 1);
                    HOperatorSet.AffineTransContourXld(ModelContours, out ModelXld,
                        HomMat2D);
                }
                else
                {
                    return -2;
                }

                return 0;
            }
            catch(Exception ex) 
            {

                Log4netRecord.ErrorLog.Error(ex);
                return -3;
            }
            finally
            {
                //ModelContours?.Dispose();

            }
           


        }








    }
}
