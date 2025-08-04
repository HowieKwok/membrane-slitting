using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Frame_Core;
using System.ComponentModel;
using HalconDotNet;
using HalconAlgorithm;
using System.Windows.Forms;


namespace Plugin.MeasureEar
{
    // [ExportMetadata("measure2", "IPlugin")]
    [Category("尺寸测量")]
    [DisplayName("测量极耳工具")]
    [Export("MeasEarObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class MeasEarObj : IPlugin
    {
        public PluginParamsBase PluginParams { get; set; } = new ParamsMeasureEar();
        ParamsMeasureLine measLineParams;

        ParamsMeasureLine LineParams1;
        ParamsMeasureLine LineParams2;
        ParamsMeasureEar measEarParams;
        FittingLine fittingLine = new FittingLine();
        FitCircle fitCircle = new FitCircle();
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();

        bool fitLine = true;

        public bool ExecuteObj()
        {
            measEarParams = PluginParams as ParamsMeasureEar; //里氏转换
            measEarParams.INiOutputParams();
            fitLine = true;
            double XYRatio = measEarParams.Pix_X / measEarParams.Pix_Y;
            double colShaft = measEarParams.JRCenterCol - measEarParams.JRShaft_Left;
            double MidJRRow = measEarParams.JRCenterRow;

            HObject ReduceImage1;
            HOperatorSet.GenEmptyObj(out ReduceImage1);
            HObject ReduceImage2;
            HOperatorSet.GenEmptyObj(out ReduceImage2);

            HObject MarkregionReduce1;
            HOperatorSet.GenEmptyObj(out MarkregionReduce1);
            HObject MarkregionReduce2;
            HOperatorSet.GenEmptyObj(out MarkregionReduce2);

            try
            {
                //JR图像裁切处理部分，上下贯穿裁出极耳，
                if (measEarParams.ho_Image.CountObj() != 0)
                {
                    HTuple w, h;
                    HOperatorSet.GetImageSize(measEarParams.ho_Image, out w, out h);
                    HOperatorSet.CropRectangle1(measEarParams.ho_Image, out measEarParams.ho_JRImage, 0, measEarParams.JRCenterCol - measEarParams.JRShaft_Left, h - 1, measEarParams.JRCenterCol + measEarParams.JRShaft_Left - 1);
                    HOperatorSet.ZoomImageFactor(measEarParams.ho_JRImage, out measEarParams.ho_ZoomJRImage, 1, XYRatio, "bilinear");//缩放图像，调整横纵当量一致，用于测R角半径，
                }
                //JR裁图后抓边。
                if (measEarParams.IsFitOneLine)
                {
                    JRLineEnum enumItem = measEarParams.JRLine;
                    measLineParams = measEarParams.array_MeasureLineParam[(int)enumItem];//拿取抓边数据
                    measLineParams.ho_Image = measEarParams.ho_JRImage;

                    FitOneLine(measLineParams);
                    fitLine = measLineParams.bl_FitSuccess;
                }
                else
                {
                    //逐条抓边
                    foreach (JRLineEnum enumItem in Enum.GetValues(typeof(JRLineEnum)))
                    {
                        while ((int)enumItem > measEarParams.array_MeasureLineParam.Length - 1)
                        {
                            Array.Resize(ref measEarParams.array_MeasureLineParam, measEarParams.array_MeasureLineParam.Length + 1);
                            measEarParams.array_MeasureLineParam[measEarParams.array_MeasureLineParam.Length - 1] = new ParamsMeasureLine();
                        }
                        measLineParams = measEarParams.array_MeasureLineParam[(int)enumItem];//拿取抓边数据
                        measLineParams.ho_Image = measEarParams.ho_JRImage;

                        if (measEarParams.IsBackImage)
                        {
                            if (enumItem == JRLineEnum.台阶边 /*|| enumItem == JRLineEnum.AT边*/)
                            {
                                continue;
                            }

                        }
                        if (!measEarParams.IsCathode)
                        {
                            //if (enumItem == JRLineEnum.AT边)
                            //{
                            //    continue;
                            //}
                        }
                        if (enumItem == JRLineEnum.底部R2边)
                        {
                            measLineParams.hv_RoiMidColumn = measEarParams.array_MeasureLineParam[(int)JRLineEnum.底部R1边].hv_RoiMidColumn;
                            measLineParams.hv_RoiHeight = measEarParams.array_MeasureLineParam[(int)JRLineEnum.底部R1边].hv_RoiHeight;
                        }

                        FitOneLine(measLineParams);
                        fitLine &= measLineParams.bl_FitSuccess;
                    }
                    //推测极耳四个角点坐标，还原成EA坐标;必须抓五条轮廓边
                    if (measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳底边1].bl_FitSuccess &&
                        measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳斜边1].bl_FitSuccess &&
                        measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳底边2].bl_FitSuccess &&
                        measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳斜边2].bl_FitSuccess &&
                        measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳顶边].bl_FitSuccess)

                    {
                        HTuple hv_IsParallel;
                        //底边1边和斜边1的交点
                        LineParams1 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳底边1];
                        LineParams2 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳斜边1];
                        HOperatorSet.IntersectionLl(LineParams1.hv_LineRowBegin, LineParams1.hv_LineColumnBegin, LineParams1.hv_LineRowEnd, LineParams1.hv_LineColumnEnd, LineParams2.hv_LineRowBegin, LineParams2.hv_LineColumnBegin, LineParams2.hv_LineRowEnd, LineParams2.hv_LineColumnEnd,
                            out measEarParams.hv_PointRow0, out measEarParams.hv_PointCol0, out hv_IsParallel);
                        // measEarParams.hv_PointCol0 += colShaft;
                        //底边2边和斜边2的交点
                        LineParams1 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳底边2];
                        LineParams2 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳斜边2];
                        HOperatorSet.IntersectionLl(LineParams1.hv_LineRowBegin, LineParams1.hv_LineColumnBegin, LineParams1.hv_LineRowEnd, LineParams1.hv_LineColumnEnd, LineParams2.hv_LineRowBegin, LineParams2.hv_LineColumnBegin, LineParams2.hv_LineRowEnd, LineParams2.hv_LineColumnEnd,
                             out measEarParams.hv_PointRow1, out measEarParams.hv_PointCol1, out hv_IsParallel);
                        // measEarParams.hv_PointCol1 += colShaft;
                        //顶边和斜边1的交点
                        LineParams1 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳顶边];
                        LineParams2 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳斜边1];
                        HOperatorSet.IntersectionLl(LineParams1.hv_LineRowBegin, LineParams1.hv_LineColumnBegin, LineParams1.hv_LineRowEnd, LineParams1.hv_LineColumnEnd, LineParams2.hv_LineRowBegin, LineParams2.hv_LineColumnBegin, LineParams2.hv_LineRowEnd, LineParams2.hv_LineColumnEnd,
                            out measEarParams.hv_PointRow2, out measEarParams.hv_PointCol2, out hv_IsParallel);
                        // measEarParams.hv_PointCol2 += colShaft;
                        //顶边边和斜边2的交点
                        LineParams1 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳顶边];
                        LineParams2 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳斜边2];
                        HOperatorSet.IntersectionLl(LineParams1.hv_LineRowBegin, LineParams1.hv_LineColumnBegin, LineParams1.hv_LineRowEnd, LineParams1.hv_LineColumnEnd, LineParams2.hv_LineRowBegin, LineParams2.hv_LineColumnBegin, LineParams2.hv_LineRowEnd, LineParams2.hv_LineColumnEnd,
                             out measEarParams.hv_PointRow3, out measEarParams.hv_PointCol3, out hv_IsParallel);
                        // measEarParams.hv_PointCol3 += colShaft;


                        //计算极耳底部宽度,
                        // measEarParams.dic_SizeItem[SizeEnum.极耳底宽Y] = Math.Abs(measEarParams.Pix_Y * (measEarParams.hv_PointRow0.D - measEarParams.hv_PointRow1.D));
                        //计算极耳顶部宽度,
                        measEarParams.dic_SizeItem[SizeEnum.极耳顶宽Y] = Math.Abs(measEarParams.Pix_Y * (measEarParams.hv_PointRow2.D - measEarParams.hv_PointRow3.D));

                        //计算极耳高度,
                        LineParams1 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳底边1];
                        LineParams2 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳顶边];
                        measEarParams.dic_SizeItem[SizeEnum.极耳高度] = Math.Abs(measEarParams.Pix_X * ((LineParams1.hv_LineColumnBegin.D + LineParams1.hv_LineColumnEnd.D) / 2 - (LineParams2.hv_LineColumnBegin.D + LineParams2.hv_LineColumnEnd.D) / 2));

                        //计算上斜边角度,
                        LineParams1 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳斜边1];
                        LineParams2 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳底边1];
                        HTuple Angle1;
                        HOperatorSet.AngleLl(LineParams1.hv_LineRowBegin * XYRatio, LineParams1.hv_LineColumnBegin, LineParams1.hv_LineRowEnd * XYRatio, LineParams1.hv_LineColumnEnd, LineParams2.hv_LineRowBegin * XYRatio, LineParams2.hv_LineColumnBegin, LineParams2.hv_LineRowEnd * XYRatio, LineParams2.hv_LineColumnEnd, out Angle1);
                        HOperatorSet.TupleDeg(Angle1, out Angle1);
                        measEarParams.dic_SizeItem[SizeEnum.极耳上升沿角度] = Math.Abs(Angle1.D);
                        //计算下斜边角度,
                        LineParams1 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳底边2];
                        LineParams2 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳斜边2];
                        HTuple Angle2;
                        HOperatorSet.AngleLl(LineParams1.hv_LineRowBegin * XYRatio, LineParams1.hv_LineColumnBegin, LineParams1.hv_LineRowEnd * XYRatio, LineParams1.hv_LineColumnEnd, LineParams2.hv_LineRowBegin * XYRatio, LineParams2.hv_LineColumnBegin, LineParams2.hv_LineRowEnd * XYRatio, LineParams2.hv_LineColumnEnd, out Angle2);
                        HOperatorSet.TupleDeg(Angle2, out Angle2);
                        measEarParams.dic_SizeItem[SizeEnum.极耳下降沿角度] = Math.Abs(Angle2.D);

                        //拟合极耳R1角。
                        HTuple row0, col0, row1, col1;
                        row0 = measEarParams.hv_PointRow0 * XYRatio - measEarParams.AngleShaftY;
                        row1 = measEarParams.hv_PointRow0 * XYRatio + measEarParams.AngleShaftY * 0.5;
                        col0 = measEarParams.hv_PointCol0 - measEarParams.AngleShaftX * 0.5;
                        col1 = measEarParams.hv_PointCol0 + measEarParams.AngleShaftX;
                        HOperatorSet.GenRectangle1(out ReduceImage1, row0, col0, row1, col1);
                        HOperatorSet.ReduceDomain(measEarParams.ho_ZoomJRImage, ReduceImage1, out ReduceImage1);
                        fitCircle.Get_Max_Circle(ReduceImage1, out measEarParams.ho_R1_XLD, 10, 3000, out measEarParams.R1Row, out measEarParams.R1Col, out measEarParams.R1Radius);

                        measEarParams.dic_SizeItem[SizeEnum.顶R1半径] = measEarParams.R1Radius.D * measEarParams.Pix_X;

                        //拟合极耳R2角。
                        HTuple row01, col01, row11, col11;
                        row01 = measEarParams.hv_PointRow1 * XYRatio - measEarParams.AngleShaftY * 0.5;
                        row11 = measEarParams.hv_PointRow1 * XYRatio + measEarParams.AngleShaftY;
                        col01 = measEarParams.hv_PointCol1 - measEarParams.AngleShaftX * 0.5;
                        col11 = measEarParams.hv_PointCol1 + measEarParams.AngleShaftX;
                        HOperatorSet.GenRectangle1(out ReduceImage2, row01, col01, row11, col11);
                        HOperatorSet.ReduceDomain(measEarParams.ho_ZoomJRImage, ReduceImage2, out ReduceImage2);
                        fitCircle.Get_Max_Circle(ReduceImage2, out measEarParams.ho_R2_XLD, 10, 3000, out measEarParams.R2Row, out measEarParams.R2Col, out measEarParams.R2Radius);

                        measEarParams.dic_SizeItem[SizeEnum.底R2半径] = measEarParams.R2Radius.D * measEarParams.Pix_X;

                        // //HOperatorSet.AreaCenter(measEarParams.ho_R1_XLD, out HTuple R1Area, out HTuple R1Row, out HTuple R1Col);
                        // //HOperatorSet.AreaCenter(measEarParams.ho_R2_XLD, out HTuple R2Area, out HTuple R2Row, out HTuple R2Col);
                        // double 极耳斜边1_Mid = (measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳斜边1].hv_LineRowBegin.D + measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳斜边1].hv_LineRowEnd.D)/2;
                        // double 极耳斜边2_Mid = (measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳斜边2].hv_LineRowBegin.D + measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳斜边2].hv_LineRowEnd.D) / 2;
                        // measEarParams.hv_MidOffset = (极耳斜边1_Mid + 极耳斜边2_Mid) / 2 - measEarParams.JRCenterRow;
                        //// measEarParams.hv_MidOffset= (R1Row + R2Row) / 2 - measEarParams.JRCenterRow;
                    }
                    //精定位极耳坐标，还原成EA坐标;必须抓两条极耳R角边
                    if (measEarParams.array_MeasureLineParam[(int)JRLineEnum.底部R1边].bl_FitSuccess &&
                        measEarParams.array_MeasureLineParam[(int)JRLineEnum.底部R2边].bl_FitSuccess)
                    {
                        //极耳中心偏差，取R1XLD和R2XLD的轮廓中心，然后求行坐标中值。
                        LineParams1 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.底部R1边];
                        LineParams2 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.底部R2边];
                        double mid1 = (LineParams1.hv_LineRowBegin.D + LineParams1.hv_LineRowEnd.D) * 0.50;
                        double mid2 = (LineParams2.hv_LineRowBegin.D + LineParams2.hv_LineRowEnd.D) * 0.50;
                        MidJRRow = (mid1 + mid2) * 0.50;
                        measEarParams.hv_MidOffset = MidJRRow - measEarParams.JRCenterRow;

                        //计算极耳底部宽度,
                        measEarParams.dic_SizeItem[SizeEnum.极耳底宽Y] = Math.Abs(mid2 - mid1) * measEarParams.Pix_Y;
                    }
                    //背光相机不检测台阶，阴极台阶为AT和台阶边。阳极台阶为极耳底边1和台阶边
                    if (!measEarParams.IsBackImage)
                    {
                        //阴极，台阶高为AT和台阶边差
                        //LineParams1 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.AT边];
                        //LineParams2 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.台阶边];
                        //if (measEarParams.IsCathode && LineParams1.bl_FitSuccess && LineParams2.bl_FitSuccess)
                        //{
                        //    measEarParams.dic_SizeItem[SizeEnum.极耳台阶高] = Math.Abs(measEarParams.Pix_X * ((LineParams1.hv_LineColumnBegin.D + LineParams1.hv_LineColumnEnd.D) / 2 - (LineParams2.hv_LineColumnBegin.D + LineParams2.hv_LineColumnEnd.D) / 2));
                        //}

                        //阳极，台阶高为极耳底边1和台阶边差
                        LineParams1 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳底边1];
                        LineParams2 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.台阶边];
                        if (!measEarParams.IsCathode && LineParams1.bl_FitSuccess && LineParams2.bl_FitSuccess)
                        {
                            measEarParams.dic_SizeItem[SizeEnum.极耳台阶高] = Math.Abs(measEarParams.Pix_X * ((LineParams1.hv_LineColumnBegin.D + LineParams1.hv_LineColumnEnd.D) / 2 - (LineParams2.hv_LineColumnBegin.D + LineParams2.hv_LineColumnEnd.D) / 2));
                        }
                    }
                    //阳极测mark孔尺寸,上mark孔
                    if (measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳底边1].bl_FitSuccess && measEarParams.Mark1Enable)
                    {
                        LineParams1 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳底边1];
                        double midcol1 = (LineParams1.hv_LineColumnBegin.D + LineParams1.hv_LineColumnEnd.D) * 0.50;
                        HTuple row01, col01, row11, col11;
                        row01 = MidJRRow - measEarParams.Mark1ShaftRow - measEarParams.MarkRegionY / 2;
                        row11 = MidJRRow - measEarParams.Mark1ShaftRow + measEarParams.MarkRegionY / 2;
                        col01 = midcol1 - measEarParams.MarkRegionX;
                        col11 = midcol1;

                        HOperatorSet.GenRectangle1(out measEarParams.ho_MarkregionRect1, row01, col01, row11, col11);
                        HOperatorSet.ReduceDomain(measEarParams.ho_JRImage, measEarParams.ho_MarkregionRect1, out MarkregionReduce1);
                        HOperatorSet.Threshold(MarkregionReduce1, out measEarParams.ho_Markregion1, measEarParams.MarkGrayMin, measEarParams.MarkGrayMax);
                        HOperatorSet.OpeningRectangle1(measEarParams.ho_Markregion1, out measEarParams.ho_Markregion1, 2, 1);
                        HTuple regionNum;
                        HOperatorSet.CountObj(measEarParams.ho_Markregion1, out regionNum);
                        if (regionNum > 0)
                        {
                            //有上下切边对不齐漏光风险，可以开闭运算消除
                            HOperatorSet.SmallestRectangle1(measEarParams.ho_Markregion1, out HTuple row1, out HTuple column1, out HTuple row2, out HTuple column2);
                            measEarParams.dic_SizeItem[SizeEnum.上V角宽度] = (row2 - row1) * measEarParams.Pix_Y;
                            measEarParams.dic_SizeItem[SizeEnum.上V角深度] = (column2 - column1) * measEarParams.Pix_X;
                        }
                    }
                    //阳极测mark孔尺寸,下mark孔
                    if (measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳底边2].bl_FitSuccess && measEarParams.Mark2Enable)
                    {
                        LineParams1 = measEarParams.array_MeasureLineParam[(int)JRLineEnum.极耳底边2];
                        double midcol1 = (LineParams1.hv_LineColumnBegin.D + LineParams1.hv_LineColumnEnd.D) * 0.50;
                        HTuple row01, col01, row11, col11;
                        row01 = MidJRRow + measEarParams.Mark2ShaftRow - measEarParams.MarkRegionY / 2;
                        row11 = MidJRRow + measEarParams.Mark2ShaftRow + measEarParams.MarkRegionY / 2;
                        col01 = midcol1 - measEarParams.MarkRegionX;
                        col11 = midcol1;

                        HOperatorSet.GenRectangle1(out measEarParams.ho_MarkregionRect2, row01, col01, row11, col11);
                        HOperatorSet.ReduceDomain(measEarParams.ho_JRImage, measEarParams.ho_MarkregionRect2, out MarkregionReduce2);
                        HOperatorSet.Threshold(MarkregionReduce2, out measEarParams.ho_Markregion2, measEarParams.MarkGrayMin, measEarParams.MarkGrayMax);
                        HOperatorSet.OpeningRectangle1(measEarParams.ho_Markregion2, out measEarParams.ho_Markregion2, 2, 1);
                        HTuple regionNum;
                        HOperatorSet.CountObj(measEarParams.ho_Markregion2, out regionNum);
                        if (regionNum > 0)
                        {
                            //有上下切边对不齐漏光风险，可以开闭运算消除

                            HOperatorSet.SmallestRectangle1(measEarParams.ho_Markregion2, out HTuple row1, out HTuple column1, out HTuple row2, out HTuple column2);
                            measEarParams.dic_SizeItem[SizeEnum.下V角宽度] = (row2 - row1) * measEarParams.Pix_Y;
                            measEarParams.dic_SizeItem[SizeEnum.下V角深度] = (column2 - column1) * measEarParams.Pix_X;
                        }
                    }

                }
                return fitLine;
            }
            catch (Exception ee)
            {
                return false;
            }
            finally
            {
                ReduceImage1.Dispose();
                ReduceImage2.Dispose();
                MarkregionReduce1.Dispose();
                MarkregionReduce2.Dispose();

            }
        }

        public bool saveParams()
        {
            PluginParams.filePath = moduleInfo.filePath /*+ "_Params"*/;
            if (PluginParams.Save())
            {
                return true;
            }
            return false;
        }

        public void SetParams(PluginParamsBase param)
        {
            this.PluginParams = param;
        }

        public void GetResultParams(string varName, object obj)
        {

        }

        public bool FitOneLine(ParamsMeasureLine measLineParams)
        {
            HTuple hv_MeasureRow1, hv_MeasureColumn1, hv_MeasureRow2, hv_MeasureColumn2, hv_RoiPhi;
            HTuple hv_RoiMidRow = measLineParams.hv_RoiMidRow;
            HTuple hv_RoiMidColumn = measLineParams.hv_RoiMidColumn;
            //    HObject ResultLine;
            // HOperatorSet.GenEmptyObj(out ResultLine);
            // HObject ho_Rectangle2, ho_ImagePart, ho_ImagePartProcessed;
            // HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            //  HOperatorSet.GenEmptyObj(out ho_ImagePart);
            // HOperatorSet.GenEmptyObj(out ho_ImagePartProcessed);

            HOperatorSet.GenEmptyObj(out measLineParams.ho_ImageProcessed);
            measLineParams.hv_MetrologyHandle = null;
            measLineParams.hv_Index = null;
            measLineParams.hv_CrossRow = null;
            measLineParams.hv_CrossColumn = null;
            measLineParams.hv_LineRowBegin = null;
            measLineParams.hv_LineColumnBegin = null;
            measLineParams.hv_LineRowEnd = null;
            measLineParams.hv_LineColumnEnd = null;
            measLineParams.bl_FitSuccess = false;

            HOperatorSet.TupleRad(measLineParams.hv_RoiAngle, out hv_RoiPhi);

            hv_MeasureRow1 = hv_RoiMidRow - measLineParams.hv_RoiHeight * Math.Sin(hv_RoiPhi) / 2;
            hv_MeasureColumn1 = hv_RoiMidColumn - measLineParams.hv_RoiHeight * Math.Cos(hv_RoiPhi) / 2;
            hv_MeasureRow2 = hv_RoiMidRow + measLineParams.hv_RoiHeight * Math.Sin(hv_RoiPhi) / 2;
            hv_MeasureColumn2 = hv_RoiMidColumn + measLineParams.hv_RoiHeight * Math.Cos(hv_RoiPhi) / 2;

            //抓边算法
            if (measLineParams.hv_RoiPointsNum > 1 & measLineParams.ho_Image.CountObj() != 0)
            {
                fittingLine.CreateLineModel(hv_MeasureRow1, hv_MeasureColumn1, hv_MeasureRow2, hv_MeasureColumn2, measLineParams.hv_RoiWidth / 2,
                         measLineParams.hv_RoiHeight / measLineParams.hv_RoiPointsNum / 2, measLineParams.hv_MeasureSigma, measLineParams.hv_MeasureThreshold, out measLineParams.hv_MetrologyHandle, out measLineParams.hv_Index);
                fittingLine.FindLine(measLineParams.ho_Image, measLineParams.hv_MetrologyHandle, measLineParams.hv_Index, measLineParams.hv_MeasureNumInstances, measLineParams.hv_MeasureDistThreshold, measLineParams.hv_MeasureTransition, measLineParams.hv_MeasureSelect,
                     measLineParams.hv_RoiPointsNum, measLineParams.hv_MeasureMinScore, measLineParams.hv_MaxNumIterations, measLineParams.hv_MeasureInterpolation, measLineParams.hv_MeasureIORegions, out measLineParams.hv_CrossRow,
                    out measLineParams.hv_CrossColumn, out measLineParams.hv_LineRowBegin, out measLineParams.hv_LineColumnBegin, out measLineParams.hv_LineRowEnd, out measLineParams.hv_LineColumnEnd);

                if (measLineParams.hv_LineRowBegin >= 0)
                {
                    measLineParams.bl_FitSuccess = true;
                }
                else
                {
                    measLineParams.bl_FitSuccess = false;
                }
            }
            else
            {
                measLineParams.bl_FitSuccess = false;
            }
            return measLineParams.bl_FitSuccess;
        }
        public void IniObj()
        {

        }

        public void DisPoseObj()
        {

        }

    }


}
