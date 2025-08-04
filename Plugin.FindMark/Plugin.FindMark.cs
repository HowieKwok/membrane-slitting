using Frame_Core;
using HalconDotNet;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace Plugin.FindMark
{
    [Category("查找Mark")]
    [DisplayName("查找Mark工具")]
    //[ExportMetadata("measure1", "IPlugin")]
    [Export("FindMarkObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class FindMarkObj : IPlugin
    {
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();
        public PluginParamsBase PluginParams { get; set; } = new ParamsFindMark();
        ParamsFindMark paramsFindMark;
        public string grooveName;
        public bool isExistModel = false;

        public bool ExecuteObj()
        {

            try
            {
                paramsFindMark = PluginParams as ParamsFindMark; //里氏转换
                paramsFindMark.m_ho_OutCropRegion?.Dispose();
                paramsFindMark.m_ho_OutRegion?.Dispose();
                paramsFindMark.m_ho_CropImg?.Dispose();
                paramsFindMark.m_ho_OutRecRegion?.Dispose();
                paramsFindMark.m_fGoalY = 0;
                paramsFindMark.m_fGoalX = 0;
                paramsFindMark.m_fGoalR = 0;
                paramsFindMark.bIsFindLineSucess = false;

                HOperatorSet.TupleRad(paramsFindMark.hv_RoiAngle, out HTuple Phi);
                HOperatorSet.GenRectangle2(out paramsFindMark.m_ho_OutCropRegion, paramsFindMark.hv_RoiMidRow, paramsFindMark.hv_RoiMidColumn, 0, paramsFindMark.hv_RoiHeight / 2, paramsFindMark.hv_RoiWidth / 2);

                if (paramsFindMark.MarkType == 0)
                {
                    paramsFindMark.m_fGoalY = paramsFindMark.m_fBaseCenterX;
                    paramsFindMark.m_fGoalX = paramsFindMark.m_fBaseCenterY;
                    paramsFindMark.m_fGoalR = paramsFindMark.m_fBaseCenterR;
                }
                else if (paramsFindMark.MarkType == 1)
                {

                    HOperatorSet.ReduceDomain(paramsFindMark.ho_Image, paramsFindMark.m_ho_OutCropRegion, out paramsFindMark.m_ho_CropImg);

                    HOperatorSet.Threshold(paramsFindMark.m_ho_CropImg, out paramsFindMark.m_ho_OutRegion, paramsFindMark.m_nCircleThreshMin, paramsFindMark.m_nCircleThreshMax);
                    HOperatorSet.OpeningCircle(paramsFindMark.m_ho_OutRegion, out paramsFindMark.m_ho_OutRegion, paramsFindMark.CircleOpenParam);

                    HOperatorSet.Connection(paramsFindMark.m_ho_OutRegion, out paramsFindMark.m_ho_OutRegion);

                    HOperatorSet.FillUp(paramsFindMark.m_ho_OutRegion, out paramsFindMark.m_ho_OutRegion);
                    HOperatorSet.SelectShape(paramsFindMark.m_ho_OutRegion, out paramsFindMark.m_ho_OutRegion, "circularity", "and", paramsFindMark.m_fCircularityMin, paramsFindMark.m_fCircularityMax);
                    HOperatorSet.SelectShape(paramsFindMark.m_ho_OutRegion, out paramsFindMark.m_ho_OutRegion, "width", "and", paramsFindMark.m_nCircleWidthMin, paramsFindMark.m_nCircleWidthMax);

                    // HOperatorSet.CropRectangle2(paramsFindMark.ho_Image, out paramsFindMark.m_ho_CropImg, paramsFindMark.m_fBaseCenterY, paramsFindMark.m_fBaseCenterX, Phi, paramsFindMark.hv_RoiHeight / 2, paramsFindMark.hv_RoiWidth / 2, "true", "constant");
                    if (paramsFindMark.m_ho_OutRegion?.CountObj() == null || paramsFindMark.m_ho_OutRegion?.CountObj() == 0 /*|| paramsFindMark.m_ho_OutRegion?.CountObj() > 1*/)
                    {
                        return false;
                    }

                    HOperatorSet.ShapeTrans(paramsFindMark.m_ho_OutRegion, out paramsFindMark.m_ho_OutRegion, "outer_circle");
                    HOperatorSet.AreaCenter(paramsFindMark.m_ho_OutRegion, out HTuple area, out HTuple row, out HTuple col);
                    paramsFindMark.m_fGoalY = row[0].D;
                    paramsFindMark.m_fGoalX = col[0].D;
                }
                else if (paramsFindMark.MarkType == 2)
                {
                    HOperatorSet.ReduceDomain(paramsFindMark.ho_Image, paramsFindMark.m_ho_OutCropRegion, out paramsFindMark.m_ho_CropImg);
                    HOperatorSet.Threshold(paramsFindMark.m_ho_CropImg, out paramsFindMark.m_ho_OutRecRegion, paramsFindMark.m_nRecThreshMin, paramsFindMark.m_nRecThreshMax);

                    HOperatorSet.OpeningRectangle1(paramsFindMark.m_ho_OutRecRegion, out paramsFindMark.m_ho_OutRecRegion, paramsFindMark.RectangleOpenParam, paramsFindMark.RectangleOpenParam);

                    HOperatorSet.Connection(paramsFindMark.m_ho_OutRecRegion, out paramsFindMark.m_ho_OutRecRegion);

                    HOperatorSet.SelectShape(paramsFindMark.m_ho_OutRecRegion, out paramsFindMark.m_ho_OutRecRegion, "width", "and", paramsFindMark.m_nRecWidthMin, paramsFindMark.m_nRecWidthMax);


                    HOperatorSet.SelectShape(paramsFindMark.m_ho_OutRecRegion, out paramsFindMark.m_ho_OutRecRegion, "height", "and", paramsFindMark.m_nRecWidthMin, paramsFindMark.m_nRecWidthMax);

                    if (paramsFindMark.m_ho_OutRecRegion?.CountObj() == null || paramsFindMark.m_ho_OutRecRegion?.CountObj() == 0 /*|| paramsFindMark.m_ho_OutRecRegion?.CountObj() > 1*/)
                    {
                        return false;
                    }

                    if (paramsFindMark.m_ho_OutRecRegion.CountObj() > 1)
                    {
                        return false;
                    }



                    HOperatorSet.SmallestRectangle2(paramsFindMark.m_ho_OutRecRegion, out HTuple row1, out HTuple col1, out HTuple phi1, out HTuple length1, out HTuple length2);
                    HOperatorSet.GenRectangle2(out paramsFindMark.m_ho_OutRecRegion, row1, col1, phi1, length1, length2);

                    try
                    {
                        ImageAlgorithm.Get_Rectangle_point(paramsFindMark.m_ho_OutRecRegion, out HTuple leftTop_Row, out HTuple leftTop_Col, out HTuple rightTop_Row,
                        out HTuple rightTop_Col, out HTuple leftBottom_Row, out HTuple leftBottom_Col, out HTuple rightBottom_Row, out HTuple rightBottom_Col);
                        HOperatorSet.DistancePl(leftTop_Row, leftTop_Col, rightTop_Row, rightTop_Col, rightBottom_Row, rightBottom_Col, out HTuple distW);
                        HOperatorSet.DistancePl(leftTop_Row, leftTop_Col, leftBottom_Row, leftBottom_Col, rightBottom_Row, rightBottom_Col, out HTuple distH);
                        paramsFindMark.m_fMarkWidth = distW[0].D* paramsFindMark.m_fPixelUnitX;
                        paramsFindMark.m_fMarkHeight = distH[0].D* paramsFindMark.m_fPixelUnitY;


                    }
                    catch (Exception ee)
                    {

                    }
                
                    
                    paramsFindMark.m_fGoalY = row1[0].D;
                    paramsFindMark.m_fGoalX = col1[0].D;
                    paramsFindMark.m_fGoalR = phi1[0].D;

                }

                else if (paramsFindMark.MarkType == 3)
                {
                    HOperatorSet.ReduceDomain(paramsFindMark.ho_Image, paramsFindMark.m_ho_OutCropRegion, out paramsFindMark.m_ho_CropImg);
                    HOperatorSet.Threshold(paramsFindMark.m_ho_CropImg, out paramsFindMark.m_ho_OutRecRegion, paramsFindMark.m_nRecThreshMin, paramsFindMark.m_nRecThreshMax);

                    HOperatorSet.OpeningRectangle1(paramsFindMark.m_ho_OutRecRegion, out paramsFindMark.m_ho_OutRecRegion, paramsFindMark.RectangleOpenParam, paramsFindMark.RectangleOpenParam);

                    HOperatorSet.Connection(paramsFindMark.m_ho_OutRecRegion, out paramsFindMark.m_ho_OutRecRegion);

                    HOperatorSet.SelectShape(paramsFindMark.m_ho_OutRecRegion, out paramsFindMark.m_ho_OutRecRegion, "width", "and", paramsFindMark.m_nRecWidthMin, paramsFindMark.m_nRecWidthMax);


                    HOperatorSet.SelectShape(paramsFindMark.m_ho_OutRecRegion, out paramsFindMark.m_ho_OutRecRegion, "height", "and", paramsFindMark.m_nRecWidthMin, paramsFindMark.m_nRecWidthMax);

                    if (paramsFindMark.m_ho_OutRecRegion?.CountObj() == null || paramsFindMark.m_ho_OutRecRegion?.CountObj() == 0 /*|| paramsFindMark.m_ho_OutRecRegion?.CountObj() > 1*/)
                    {
                        return false;
                    }

                    if (paramsFindMark.m_ho_OutRecRegion.CountObj() > 1)
                    {
                        return false;
                    }



                    HOperatorSet.SmallestRectangle2(paramsFindMark.m_ho_OutRecRegion, out HTuple row1, out HTuple col1, out HTuple phi1, out HTuple length1, out HTuple length2);
                    HOperatorSet.GenRectangle2(out paramsFindMark.m_ho_OutRecRegion, row1, col1, phi1, length1, length2);

                   
                    paramsFindMark.m_fGoalY = row1[0].D;
                    paramsFindMark.m_fGoalX = col1[0].D;
                    paramsFindMark.m_fGoalR = phi1[0].D;

                    try
                    {
                        bool bFindLineResult = ImageAlgorithm.FindRectangleEdge(paramsFindMark.ho_Image, paramsFindMark.m_ho_OutRecRegion, MeasureTransitionEnum.不区分明暗,
                       out double leftEdgeBeginRow, out double leftEdgeBeginCol, out double leftEdgeEndRow, out double leftEdgeEndCol,
                       out double topEdgeBeginRow, out double topEdgeBeginCol, out double topEdgeEndRow, out double topEdgeEndCol,
                       out double rightEdgeBeginRow, out double rightEdgeBeginCol, out double rightEdgeEndRow, out double rightEdgeEndCol,
                       out double bottomEdgeBeginRow, out double bottomEdgeBeginCol, out double bottomEdgeEndRow, out double bottomEdgeEndCol,
                       out double leftTop_Row, out double leftTop_Col, out double rightTop_Row, out double rightTop_Col,
                       out double leftBottom_Row, out double leftBottom_Col, out double rightBottom_Row, out double rightBottom_Col);

                        paramsFindMark.leftTop_Row = leftTop_Row;
                        paramsFindMark.leftTop_Col = leftTop_Col;
                        paramsFindMark.rightTop_Row = rightTop_Row;
                        paramsFindMark.rightTop_Col = rightTop_Col;
                        paramsFindMark.leftBottom_Row = leftBottom_Row;
                        paramsFindMark.leftBottom_Col = leftBottom_Col;
                        paramsFindMark.rightBottom_Row = rightBottom_Row;
                        paramsFindMark.rightBottom_Col = rightBottom_Col;

                        try
                        {
                            HOperatorSet.DistancePl(leftTop_Row, leftTop_Col, rightTop_Row, rightTop_Col, rightBottom_Row, rightBottom_Col, out HTuple distW);
                            HOperatorSet.DistancePl(leftTop_Row, leftTop_Col, leftBottom_Row, leftBottom_Col, rightBottom_Row, rightBottom_Col, out HTuple distH);
                            paramsFindMark.m_fMarkWidth = distW[0].D * paramsFindMark.m_fPixelUnitX;
                            paramsFindMark.m_fMarkHeight = distH[0].D * paramsFindMark.m_fPixelUnitY;
                        }
                        catch (Exception ee)
                        {

                        }


                        if (bFindLineResult)
                        {
                            HOperatorSet.IntersectionLines(leftTop_Row, leftTop_Col, rightBottom_Row, rightBottom_Col, rightTop_Row, rightTop_Col,
                               leftBottom_Row, leftBottom_Col, out HTuple x, out HTuple y, out HTuple dist);
                            HOperatorSet.AngleLx(leftBottom_Row, leftBottom_Col, rightBottom_Row, rightBottom_Col, out HTuple angle);
                            paramsFindMark.m_fGoalY = x[0].D;
                            paramsFindMark.m_fGoalX = y[0].D;
                            paramsFindMark.m_fGoalR = angle.D;
                            paramsFindMark.bIsFindLineSucess = true;
                        }
                        else
                        {
                            paramsFindMark.bIsFindLineSucess = false;
                        }
                    }
                    catch (Exception ee)
                    {
                        paramsFindMark.bIsFindLineSucess = false;
                    }

                }

        

                return true;
            }
            catch (Exception ee)
            {
                return false;

                // throw;
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
            this.PluginParams = (ParamsFindMark)param;
            paramsFindMark = PluginParams as ParamsFindMark; //里氏转换
        }

        public void IniReslut()
        {

        }
        public void IniObj()
        {

        }

        public void DisPoseObj()
        {

        }
    }
}
