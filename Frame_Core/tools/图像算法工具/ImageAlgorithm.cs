using HalconDotNet;
using LogTool;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Frame_Core
{
    /// <summary>
    /// 图像算法
    /// </summary>
    public static class ImageAlgorithm
    {
        /// <summary>
        /// 计算相机内参和外参
        /// </summary>
        /// <param name="ho_Images"></param>
        /// <param name="Focus">焦距</param>
        /// <param name="Kappa"> 畸变系数</param>
        /// <param name="Sx">传感器两个相邻单元水平距离</param>
        /// <param name="Sy">传感器两个相邻单元垂直距离</param>
        /// <param name="Dz">标定版的厚度</param>
        /// <param name="caltabFileName"></param>
        /// <param name="hv_CamInternalParam"></param>
        /// <param name="hv_ExternalParam"></param>
        public static void CalibCalculateCamInternalAndExternalParameters(HObject[] ho_Images, double Focus, double Kappa, double Sx, double Sy, double Dz, string caltabFileName, out HTuple hv_CamInternalParam, out HTuple hv_ExternalParam)
        {
            HOperatorSet.GetImageSize(ho_Images[0], out HTuple hv_Width, out HTuple hv_Height);

            HTuple hv_CameraParam = new HTuple();

            //设置摄像头内部参数的初始值
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                gen_cam_par_area_scan_division(Focus, Kappa, Sx, Sy, hv_Width / 2.0, hv_Height / 2.0,
                    hv_Width, hv_Height, out hv_CameraParam);
            }

            //创建一个所有校准数据的校准数据模型
            //包括图像坐标的校准标记和校准板
            HOperatorSet.CreateCalibData("calibration_object", 1, 1, out HTuple hv_CalibDataID);
            HOperatorSet.SetCalibDataCamParam(hv_CalibDataID, 0, new HTuple(), hv_CameraParam);

            //30mm标定板 这个文件不能错
            HOperatorSet.SetCalibDataCalibObject(hv_CalibDataID, 0, caltabFileName);

            for (int i = 0; i < ho_Images.Length; i++)
            {
                //读取和*找到校准板和存储观察
                //在校准数据模型中显示校准图像
                HOperatorSet.FindCalibObject(ho_Images[i], hv_CalibDataID, 0, 0, i, new HTuple(), new HTuple());

                //HOperatorSet.GetCalibDataObservContours(out HObject Caltab, hv_CalibDataID, "caltab", 0, 0, i);
                //HOperatorSet.GetCalibDataObservPoints(hv_CalibDataID, 0, 0, i, out HTuple Row, out HTuple Column, out HTuple Index, out HTuple StartPose);

            }

            //执行实际校准     
            HOperatorSet.CalibrateCameras(hv_CalibDataID, out HTuple hv_Errors);
            //获取校准后的相机内参     
            HOperatorSet.GetCalibData(hv_CalibDataID, "camera", 0, "params", out hv_CamInternalParam);
            //获取校准后的相机外参         
            HOperatorSet.GetCalibData(hv_CalibDataID, "calib_obj_pose", (new HTuple(0)).TupleConcat(1), "pose", out hv_ExternalParam);
            //考虑标定板厚度,没有可以不填。mm单位
            HOperatorSet.SetOriginPose(hv_ExternalParam, 0, 0, Dz, out hv_ExternalParam);
        }


        /// <summary>
        /// 找菲林片
        /// </summary>
        /// <param name="ho_Image"></param>
        /// <param name="paramMultiCamCalib"></param>
        /// <param name="ho_Region"></param>
        public static void FindFilmRectangleInfo(string source, HObject ho_Image, double calibrationX, double calibrationY, SizeCheckPointParam sizeCheckPointParam, out HObject preproccessRegion, out List<RectangleInfo> rectangleInfos)
        {
            rectangleInfos = new List<RectangleInfo>();
            preproccessRegion = null;
            try
            {
                //==========================================================
                //【1】找菲林片获取所有菲林片行列坐标
                HOperatorSet.GetImageSize(ho_Image, out HTuple imageWidth, out HTuple imageHeight);
                HTuple edgeX1 = sizeCheckPointParam.EdgeX1;
                HTuple edgeX2 = sizeCheckPointParam.EdgeX2;
                //HTuple MinArea = (int)(((sizeCheckPointParam.CalibBlockHeiht / 3) / sizeCheckPointParam.Hcali) * ((sizeCheckPointParam.CalibBlockWidth / 3) / sizeCheckPointParam.Wcail));     //LWL
                //HTuple MaxArea = (int)(((sizeCheckPointParam.CalibBlockHeiht + 3) / sizeCheckPointParam.Hcali) * ((sizeCheckPointParam.CalibBlockWidth + 3) / sizeCheckPointParam.Wcail));     //LWL


                HObject ho_WhiteRegion = null;

                HObject TestRoi, ConnectionROI, TestRoi1;
                HObject testimage;
                HOperatorSet.GenEmptyObj(out TestRoi);
                HOperatorSet.GenEmptyObj(out TestRoi1);
                HOperatorSet.GenEmptyObj(out ConnectionROI);
                HOperatorSet.GenEmptyObj(out testimage);
                HOperatorSet.GenEmptyObj(out ho_WhiteRegion);

                HTuple ROInum = 0;
                HTuple x1 = null, y1 = null, x2 = null, y2 = null;//方框坐标
                try
                {

                    double rectArea = sizeCheckPointParam.SideLengthPixX * sizeCheckPointParam.SideLengthPixY;

                    HOperatorSet.GenRectangle1(out TestRoi, 1, edgeX1, imageHeight - 1, edgeX2);//LWL

                    HOperatorSet.Union2(TestRoi, TestRoi1, out TestRoi);//***************
                    HOperatorSet.ReduceDomain(ho_Image, TestRoi, out testimage);
                    //提取校准片的框框
                    HTuple outt;
                    HOperatorSet.BinaryThreshold(testimage, out TestRoi, "max_separability", "light", out outt);
                    HOperatorSet.Connection(TestRoi, out ConnectionROI);
                    //HOperatorSet.SelectShape(ConnectionROI, out TestRoi, "area", "and", MinArea, MaxArea);
                    //HOperatorSet.SelectShapeStd(TestRoi, out TestRoi, "max_area", 70);//仅选取面积最大的区域*****************
                    HOperatorSet.SelectShape(TestRoi, out TestRoi, "area", "and", rectArea - 100, rectArea + 100);

                    ////在白色底中找黑色的菲林片
                    HOperatorSet.SmallestRectangle1(ConnectionROI, out y1, out x1, out y2, out x2);
                    HOperatorSet.GenRectangle1(out ho_WhiteRegion, y1, x1, y2, x2);
                    HOperatorSet.Union1(ho_WhiteRegion, out ho_WhiteRegion);//***********
                    HOperatorSet.ReduceDomain(testimage, ho_WhiteRegion, out ho_WhiteRegion);
                    HOperatorSet.Threshold(ho_WhiteRegion, out TestRoi, sizeCheckPointParam.CalibBlockMinThreshold, sizeCheckPointParam.CalibBlockMaxThreshold);
                    //HalconWinHandle.DispObj(TestRoi);
                    HOperatorSet.SmallestRectangle1(TestRoi, out y1, out x1, out y2, out x2);
                    HOperatorSet.ClosingRectangle1(TestRoi, out TestRoi, 10, 10);

                    HOperatorSet.FillUp(TestRoi, out TestRoi);
                    HOperatorSet.OpeningRectangle1(TestRoi, out TestRoi, 15, 15);
                    HOperatorSet.Connection(TestRoi, out TestRoi);
                    HTuple hv_Area = 0;
                    HOperatorSet.RegionFeatures(TestRoi, "area", out hv_Area);
                    //HOperatorSet.SelectShape(TestRoi, out TestRoi, "area", "and", MinArea, MaxArea);
                    HOperatorSet.SelectShapeStd(TestRoi, out TestRoi, "max_area", 70);//仅选取面积最大的区域*****************           
                    HOperatorSet.Union1(TestRoi, out TestRoi);//***********
                    HOperatorSet.ReduceDomain(ho_WhiteRegion, TestRoi, out TestRoi);
                    HOperatorSet.Connection(TestRoi, out TestRoi);//***********
                    HOperatorSet.CountObj(TestRoi, out ROInum);

                    preproccessRegion = TestRoi;

                    //如果没有找到校准片
                    if (ROInum < 1) return;
                    else
                    {
                        HTuple Index = 0;
                        HOperatorSet.SmallestRectangle1(TestRoi, out y1, out x1, out y2, out x2);
                        HOperatorSet.TupleSortIndex(y1, out Index);//ROW由小到大索引排序
                        for (int i = 0; i < Index.Length; i++)
                        {
                            int hv_i = Index[i] + 1;

                            HOperatorSet.SmallestRectangle1(TestRoi.SelectObj(hv_i), out y1, out x1, out y2, out x2);

                            if (y1.D > 20 && y2 < imageHeight - 20)//菲林片在中间
                            {
                                sizeCheckPointParam.LineModelParam.MeasureSelect = 2;
                                sizeCheckPointParam.LineModelParam.MeasureTransition = 2;
                                ImageAlgorithm.FindRectangleEdge(ho_Image, TestRoi.SelectObj(hv_i), sizeCheckPointParam.LineModelParam, out RectangleInfo rectangleInfo);
                                ImageAlgorithm.CalculateRectangleCenterLocation(ref rectangleInfo, calibrationX, calibrationY);
                                rectangleInfos.Add(rectangleInfo);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log4netRecord.ErrorLog.Error(source, ex);
                }
                finally
                {
                    //TestRoi.Dispose();
                    TestRoi1.Dispose();
                    ConnectionROI.Dispose();
                    testimage.Dispose();
                    ho_WhiteRegion.Dispose();
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(source, ex);
            }
            finally
            {

            }
        }

        /// <summary>
        /// 计算矩形的四条边的中心坐标
        /// </summary>
        /// <param name="rectangleInfo"></param>
        public static void CalculateRectangleCenterLocation(ref RectangleInfo rectangleInfo, double calibrationX, double calibrationY)
        {
            rectangleInfo.leftEdgeCenter_Row = (rectangleInfo.leftEdgeBeginRow + rectangleInfo.leftEdgeEndRow) / 2;
            rectangleInfo.leftEdgeCenter_Col = (rectangleInfo.leftEdgeBeginCol + rectangleInfo.leftEdgeEndCol) / 2;
            rectangleInfo.topEdgeCenter_Row = (rectangleInfo.topEdgeBeginRow + rectangleInfo.topEdgeEndRow) / 2;
            rectangleInfo.topEdgeCenter_Col = (rectangleInfo.topEdgeBeginCol + rectangleInfo.topEdgeEndCol) / 2;
            rectangleInfo.rightEdgeCenter_Row = (rectangleInfo.rightEdgeBeginRow + rectangleInfo.rightEdgeEndRow) / 2;
            rectangleInfo.rightEdgeCenter_Col = (rectangleInfo.rightEdgeBeginCol + rectangleInfo.rightEdgeEndCol) / 2;
            rectangleInfo.bottomEdgeCenter_Row = (rectangleInfo.bottomEdgeBeginRow + rectangleInfo.bottomEdgeEndRow) / 2;
            rectangleInfo.bottomEdgeCenter_Col = (rectangleInfo.bottomEdgeBeginCol + rectangleInfo.bottomEdgeEndCol) / 2;

            rectangleInfo.widthReality = Math.Abs(rectangleInfo.rightEdgeCenter_Col - rectangleInfo.leftEdgeCenter_Col) * calibrationX;
            rectangleInfo.heightReality = Math.Abs(rectangleInfo.bottomEdgeCenter_Row - rectangleInfo.topEdgeCenter_Row) * calibrationY;
        }



        /// <summary>
        /// 查找矩形的边缘坐标
        /// </summary>
        /// <param name="ho_Image"></param>
        /// <param name="ho_inputRectangle"></param>
        /// <param name="measureTransitionEnum"></param>
        /// <param name="rectangleInfo"></param>    
        public static void FindRectangleEdge(HObject ho_Image, HObject ho_inputRectangle, Param_LineModel param_LineModel, out RectangleInfo rectangleInfo)
        {
            rectangleInfo = new RectangleInfo();

            Get_Rectangle_point(ho_inputRectangle, out HTuple topLeft_Row, out HTuple topLeft_Col, out HTuple topRight_Row, out HTuple topRight_Col,
                out HTuple lowerLeft_Row, out HTuple lowerLeft_Col, out HTuple lowerRight_Row, out HTuple lowerRight_Col);


            //矩形左边缘坐标
            if (true)
            {
                
                
                FindLine(ho_Image, param_LineModel, topLeft_Row.D, topLeft_Col.D, lowerLeft_Row.D, lowerLeft_Col.D, 5, 5, out List<LineInfo1> lineInfos);

                if (lineInfos != null && lineInfos.Count > 0)
                {
                    rectangleInfo.leftEdgeBeginRow = lineInfos[0].RowBegin;
                    rectangleInfo.leftEdgeBeginCol = lineInfos[0].ColBegin;
                    rectangleInfo.leftEdgeEndRow = lineInfos[0].RowEnd;
                    rectangleInfo.leftEdgeEndCol = lineInfos[0].ColEnd;
                }
            }

            //矩形上边缘坐标
            if (true)
            {
                FindLine(ho_Image, param_LineModel, topRight_Row.D, topRight_Col.D, topLeft_Row.D, topLeft_Col.D, 5, 5, out List<LineInfo1> lineInfos);

                if (lineInfos != null && lineInfos.Count > 0)
                {
                    rectangleInfo.topEdgeBeginRow = lineInfos[0].RowBegin;
                    rectangleInfo.topEdgeBeginCol = lineInfos[0].ColBegin;
                    rectangleInfo.topEdgeEndRow = lineInfos[0].RowEnd;
                    rectangleInfo.topEdgeEndCol = lineInfos[0].ColEnd;
                }

            }

            //矩形右边缘坐标
            if (true)
            {
                FindLine(ho_Image, param_LineModel, lowerRight_Row.D, lowerRight_Col.D, topRight_Row.D, topRight_Col.D, 5, 5, out List<LineInfo1> lineInfos);

                if (lineInfos != null && lineInfos.Count > 0)
                {
                    rectangleInfo.rightEdgeBeginRow = lineInfos[0].RowBegin;
                    rectangleInfo.rightEdgeBeginCol = lineInfos[0].ColBegin;
                    rectangleInfo.rightEdgeEndRow = lineInfos[0].RowEnd;
                    rectangleInfo.rightEdgeEndCol = lineInfos[0].ColEnd;
                }
            }

            //矩形下边缘坐标
            if (true)
            {
                FindLine(ho_Image, param_LineModel, lowerLeft_Row.D, lowerLeft_Col.D, lowerRight_Row.D, lowerRight_Col.D, 5, 5, out List<LineInfo1> lineInfos);

                if (lineInfos != null && lineInfos.Count > 0)
                {
                    rectangleInfo.bottomEdgeBeginRow = lineInfos[0].RowBegin;
                    rectangleInfo.bottomEdgeBeginCol = lineInfos[0].ColBegin;
                    rectangleInfo.bottomEdgeEndRow = lineInfos[0].RowEnd;
                    rectangleInfo.bottomEdgeEndCol = lineInfos[0].ColEnd;
                }
            }
        }

        /// <su
        public static bool FindMarkCircle(HObject ho_Image, Param_CalibrationPlate param_Calib, out HTuple Row, out HTuple Column, out HObject ho_Circle, out HObject ho_Contours)
        {
            HTuple hv_Row = param_Calib.BasePointY;
            HTuple hv_Column = param_Calib.BasePointX;
            HTuple hv_Radius = param_Calib.MarkRadius;
            HTuple Hv_MeasureThreshold = param_Calib.MarkCircleModelParam.MesureThreshold;
            HTuple Hv_Length1 = param_Calib.MarkCircleModelParam.MeasureLength1;
            HTuple Hv_Length2 = param_Calib.MarkCircleModelParam.MeasureLength2;
            try
            {

                //HOperatorSet.GenCircleContourXld(out HObject ho_ContCircle, hv_Row, hv_Column, hv_Radius,
                //0, 6.28318, "positive", 1);
                HOperatorSet.CreateMetrologyModel(out HTuple hv_MetrologyHandle);
                HOperatorSet.AddMetrologyObjectGeneric(hv_MetrologyHandle, "circle", ((hv_Row.TupleConcat(
                hv_Column))).TupleConcat(hv_Radius), Hv_Length1, Hv_Length2, 1, Hv_MeasureThreshold, new HTuple(), new HTuple(),
                out HTuple hv_Index);
                HOperatorSet.ApplyMetrologyModel(ho_Image, hv_MetrologyHandle);

                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type",
                    "all_param", out HTuple hv_Parameter);

                HOperatorSet.GetMetrologyObjectResultContour(out ho_Circle, hv_MetrologyHandle,
                    "all", "all", 1.5);

                HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle,
                    "all", "all", out hv_Row, out hv_Column);
                HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
                Column = hv_Parameter[1];
                Row = hv_Parameter[0];
                return true;
            }
            catch (Exception ee)
            {
                Column = hv_Row;
                Row = hv_Column;
                ho_Contours = new HObject();
                ho_Circle = new HObject();
                return false;

            }
        }



        /// <summary>
        /// 生成标定版区域
        /// </summary>
        /// <param name="caltabTypeEnum"></param>
        /// <param name="diameter">标定板直径（单位mm）</param>
        /// <param name="centerDistance">标定板中心距离（单位mm）</param>
        /// <param name="caltabFileName"></param>
        public static void GenCaltab(CaltabTypeEnum caltabTypeEnum, double diameter, double centerDistance, out string caltabFileName)
        {
            string fileName = "";
            if (caltabTypeEnum == CaltabTypeEnum.矩形标记)
            {
                fileName = "caltab.descr";
                HTuple markDist = centerDistance / 1000;//换算成米,默认0.00375
                HTuple diameterRatio = diameter / centerDistance;//默认0.5
                HOperatorSet.GenCaltab(7, 7, markDist, diameterRatio, fileName, "caltab_30mm.ps");
            }
            else if (caltabTypeEnum == CaltabTypeEnum.六边形标记)
            {
                fileName = "calplate.cpd";
                HTuple hv_diameter = diameter / 1000;//换算成米,默认0.0015
                HOperatorSet.CreateCaltab(27, 31, hv_diameter,
                    ((((new HTuple(13)).TupleConcat(6)).TupleConcat(6)).TupleConcat(20)).TupleConcat(20),
                    ((((new HTuple(15)).TupleConcat(6)).TupleConcat(24)).TupleConcat(6)).TupleConcat(24),
                    "light_on_dark", fileName, "calplate.ps");
            }
            caltabFileName = fileName;
        }

        /// <summary>
        /// 计算图像坐标到世界坐标的转换
        /// </summary>
        /// <param name="cameraParam">相机内参</param>
        /// <param name="worldPose">相机外参</param>
        /// <param name="rows">图像位置的行坐标</param>
        /// <param name="cols">图像位置的列坐标</param>
        /// <param name="o_rows">世界坐标的行坐标</param>
        /// <param name="o_cols">世界坐标的列坐标</param>
        public static bool GetImageCoordToWorldCoord(HTuple cameraParam, HTuple worldPose, double[] rows, double[] cols, out double[] o_rows, out double[] o_cols)
        {
            o_rows = null;
            o_cols = null;
            try
            {
                HTuple hv_rows = new HTuple(rows);
                HTuple hv_cols = new HTuple(cols);

                HOperatorSet.ImagePointsToWorldPlane(cameraParam, worldPose, hv_rows, hv_cols, "mm", out HTuple hv_X, out HTuple hv_Y);

                o_cols = hv_X.DArr;
                o_rows = hv_Y.DArr;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 计算图像坐标到世界坐标的转换
        /// </summary>
        /// <param name="cameraParam">相机内参</param>
        /// <param name="worldPose">相机外参</param>
        /// <param name="row">图像位置的行坐标</param>
        /// <param name="col">图像位置的列坐标</param>
        /// <param name="o_row">世界坐标的行坐标</param>
        /// <param name="o_col">世界坐标的列坐标</param>
        public static bool GetImageCoordToWorldCoord(HTuple cameraParam, HTuple worldPose, double row, double col, out double o_row, out double o_col)
        {
            o_row = 0;
            o_col = 0;
            try
            {
                HTuple hv_rows = new HTuple(row);
                HTuple hv_cols = new HTuple(col);

                HOperatorSet.ImagePointsToWorldPlane(cameraParam, worldPose, hv_rows, hv_cols, "mm", out HTuple hv_X, out HTuple hv_Y);

                o_row = hv_X.D;
                o_col = hv_Y.D;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                return false;
            }
            return true;
        }

        // Chapter: Calibration / Camera Parameters
        // Short Description: Generate a camera parameter tuple for an area scan camera with distortions modeled by the division model. 
        public static void gen_cam_par_area_scan_division(HTuple hv_Focus, HTuple hv_Kappa, HTuple hv_Sx,
            HTuple hv_Sy, HTuple hv_Cx, HTuple hv_Cy, HTuple hv_ImageWidth, HTuple hv_ImageHeight,
            out HTuple hv_CameraParam)
        {



            // Local iconic variables 
            // Initialize local and output iconic variables 
            hv_CameraParam = new HTuple();
            //Generate a camera parameter tuple for an area scan camera
            //with distortions modeled by the division model.
            //
            hv_CameraParam.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_CameraParam = new HTuple();
                hv_CameraParam[0] = "area_scan_division";
                hv_CameraParam = hv_CameraParam.TupleConcat(hv_Focus, hv_Kappa, hv_Sx, hv_Sy, hv_Cx, hv_Cy, hv_ImageWidth, hv_ImageHeight);
            }


            return;
        }

        /// <summary>
        /// 创建变换矩阵
        /// </summary>
        /// <param name="ptxs"></param>
        /// <param name="ptys"></param>
        /// <param name="ptxd"></param>
        /// <param name="ptyd"></param>
        /// <param name="homMat2D"></param>
        /// <returns></returns>
        public static bool CreateTransformMatrix2d(double[] ptxs, double[] ptys, double[] ptxd, double[] ptyd, out HTuple homMat2D)
        {
            homMat2D = null;
            try
            {
                HOperatorSet.VectorToHomMat2d(ptxs, ptys, ptxd, ptyd, out homMat2D);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 变换矩阵反转
        /// </summary>
        /// <param name="homMat2D"></param>
        /// <param name="homMat2DInvert"></param>
        /// <returns></returns>
        public static bool TransformMatrix2dInvert(HTuple homMat2D, out HTuple homMat2DInvert)
        {
            homMat2DInvert = null;
            try
            {
                HOperatorSet.HomMat2dInvert(homMat2D, out homMat2DInvert);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取仿射变换后的坐标
        /// </summary>
        /// <param name="homMat2D"></param>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <param name="qx"></param>
        /// <param name="qy"></param>
        /// <returns></returns>
        public static bool GetAffinePoints2d(HTuple homMat2D, HTuple px, HTuple py, out double dqx, out double dqy)
        {
            dqx = 0;
            dqy = 0;
            bool result = false;
            try
            {
                if (homMat2D == null || homMat2D.Length == 0) return false;
                HOperatorSet.AffineTransPoint2d(homMat2D, px, py, out HTuple qx, out HTuple qy);
                dqx = qx.D;
                dqy = qy.D;
                result = true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                result= false;
            }
            return result;
        }

        /// <summary>
        /// 获取两个变换矩阵组合
        /// </summary>
        /// <param name="homMat2DLeft"></param>
        /// <param name="homMat2DRight"></param>
        /// <param name="homMat2DCompose"></param>
        /// <returns></returns>
        public static bool GetHomMat2dCompose(HTuple homMat2DLeft, HTuple homMat2DRight, out HTuple homMat2DCompose)
        {
            homMat2DCompose = null;
            try
            {
                if (homMat2DLeft == null || homMat2DLeft.Length == 0) return false;
                if (homMat2DRight == null || homMat2DRight.Length == 0) return false;
                HOperatorSet.HomMat2dCompose(homMat2DLeft, homMat2DRight, out homMat2DCompose);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 计算一个带旋转的矩阵
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <param name="homMat2DRotate"></param>
        /// <returns></returns>
        public static bool Calculate_HomMat2dRotate(double angle, double px, double py, out HTuple homMat2DRotate)
        {
            homMat2DRotate = null;
            try
            {
                HOperatorSet.HomMat2dIdentity(out HTuple homMat2DIdentity);
                HOperatorSet.TupleRad(angle, out HTuple rad);
                HOperatorSet.HomMat2dRotate(homMat2DIdentity, rad, px, py, out homMat2DRotate);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 找圆预处理
        /// </summary>
        /// <param name="ho_Image"></param>
        /// <param name="paramSingleCamCalib"></param>
        /// <param name="ho_CircleRegion"></param>
        public static void FindCirclePretreatment(HObject ho_Image, Param_CalibrationPlate paramSingleCamCalib, out HObject ho_CircleRegion)
        {
            ho_CircleRegion = null;
            try
            {
                HTuple circleArea = paramSingleCamCalib.Radius * paramSingleCamCalib.Radius * Math.PI;
                double minArea = circleArea * 0.8;
                double maxArea = circleArea * 1.2;
                HOperatorSet.Threshold(ho_Image, out ho_CircleRegion, paramSingleCamCalib.CirclMinThd, paramSingleCamCalib.CirclMaxThd);
                HOperatorSet.OpeningRectangle1(ho_CircleRegion, out ho_CircleRegion, 5, 5);
                HOperatorSet.ClosingRectangle1(ho_CircleRegion, out ho_CircleRegion, 5, 5);
                HOperatorSet.Connection(ho_CircleRegion, out ho_CircleRegion);
                HOperatorSet.SelectShape(ho_CircleRegion, out ho_CircleRegion, "area", "and", minArea, maxArea);

                int count = ho_CircleRegion.CountObj();
            }
            catch (Exception ex)
            {


                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
            }
            finally
            {

            }
        }

        public static bool FindCirclePretreatmentTemp(HObject ho_Image, Param_CalibrationPlate paramSingleCamCalib, out HObject ho_CircleRegion, out HObject rec, out HTuple Row, out HTuple col,bool flag=true)
        {
            ho_CircleRegion = null;
            rec = null;
            HOperatorSet.GenRectangle1(out HObject reduceRegion, paramSingleCamCalib.BasePointY - paramSingleCamCalib.MarkCircleModelParam.MeasureLength1, paramSingleCamCalib.BasePointX - paramSingleCamCalib.MarkCircleModelParam.MeasureLength2, paramSingleCamCalib.BasePointY + paramSingleCamCalib.MarkCircleModelParam.MeasureLength2, paramSingleCamCalib.BasePointX + paramSingleCamCalib.MarkCircleModelParam.MeasureLength2);
            HOperatorSet.ReduceDomain(ho_Image, reduceRegion, out HObject redImg);
            try
            {
                HTuple circleArea = paramSingleCamCalib.MarkRadius * paramSingleCamCalib.MarkRadius * Math.PI;
                double minArea = circleArea * 0.8;
                double maxArea = circleArea * 1.2;
                if(flag)
                {
                    HOperatorSet.Threshold(ho_Image, out ho_CircleRegion, paramSingleCamCalib.MarkCirclMinThd, paramSingleCamCalib.MarkCirclMaxThd);
                }
                else
                {
                    HOperatorSet.Threshold(redImg, out ho_CircleRegion, paramSingleCamCalib.MarkCirclMinThd, paramSingleCamCalib.MarkCirclMaxThd);
                }

                int count = ho_CircleRegion.CountObj();
                HOperatorSet.OpeningRectangle1(ho_CircleRegion, out ho_CircleRegion, 5, 5);
                HOperatorSet.FillUp(ho_CircleRegion, out ho_CircleRegion);
                HOperatorSet.Connection(ho_CircleRegion, out ho_CircleRegion);
                count = ho_CircleRegion.CountObj();
                HOperatorSet.SelectShape(ho_CircleRegion, out ho_CircleRegion, "circularity", "and", 0.85, 1);
                HOperatorSet.SelectShape(ho_CircleRegion, out ho_CircleRegion, "width", "and", 250, 1000);
                count = ho_CircleRegion.CountObj();
                if (count > 1 || count == 0)
                {
                    Row = 0;
                    col = 0;
                    ho_CircleRegion = new HObject();
                    rec = reduceRegion;
                    return false;
                }
                HOperatorSet.ShapeTrans(ho_CircleRegion, out ho_CircleRegion, "outer_circle");
                HOperatorSet.AreaCenter(ho_CircleRegion, out HTuple area, out Row, out col);
                rec = reduceRegion;
                redImg?.Dispose();
                //rec.Dispose();
                //rec=new HObject();
                //rec?.Dispose();
                return true;
            }
            catch (Exception ex)
            {

                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                Row = 0;
                col = 0;
                ho_CircleRegion = new HObject();
                rec = new HObject();
                return false;
            }
            finally
            {

            }
        }


        /// <summary>
        /// 找圆
        /// </summary>
        /// <param name="ho_Image"></param>
        /// <param name="paramCircleModel"></param>
        /// <param name="circleModel"></param>
        /// <param name="circleInfo"></param>
        public static void FindCircle(HObject ho_Image, Param_CircleModel paramCircleModel, out CircleInfo circleInfo)
        {
            circleInfo = null;
            HTuple hv_MetrologyHandle = null;
            try
            {
                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                //HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);

                HOperatorSet.AddMetrologyObjectCircleMeasure(hv_MetrologyHandle, paramCircleModel.center_Y, paramCircleModel.center_X, paramCircleModel.radius, paramCircleModel.MeasureLength1, paramCircleModel.MeasureLength2,
                             paramCircleModel.MesureSigma, paramCircleModel.MesureThreshold, new HTuple(), new HTuple(), out HTuple hv_Index);

                //1.拟合数量：默认值： 1 值的列表： 1，2，3，4
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_Index, "num_instances", paramCircleModel.MeasureNumInstances);
                //2.距离剔除：默认值： 3.5  值的列表： 0，1.0，2.0， 3.5，5.0
                //HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_Index, "distance_threshold", hv_MeasureDistThreshold);
                //3.明暗方向：值列表： 'all'，'negative'， 'positive'，'uniform'
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_Index, "measure_transition", GetAnotherNamerAttribute((MeasureTransitionEnum)paramCircleModel.MeasureTransition));
                //4.边缘选取：'all'，'first'，'last'
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_Index, "measure_select", GetAnotherNamerAttribute((MeasureSelectEnum)paramCircleModel.MeasureSelect));
                //5.Roi数量：'num_measures'
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_Index, "num_measures", paramCircleModel.RoiPointsNum);
                //6.最小得分：'min_score'
                //HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_Index, "min_score", hv_MeasureMinScore);
                //7.迭代次数: 默认值：-1  值的列表： 10，100，1000
                //HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_Index, "max_num_iterations", hv_MaxNumIterations);
                //8.插值类型：nearest_neighbor'灰度值是从最近像素的灰度值获得；'bilinear'，使用双线性插值 ；'bicubic'，则使用双三次插值。
                //HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_Index, "measure_interpolation", hv_MeasureInterpolation);
                //9.指定对测量结果的验证：默认值： “ false”  值列表： 'true'，'false'
                //HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_Index, "instances_outside_measure_regions", hv_MeasureIORegions);

                HOperatorSet.ApplyMetrologyModel(ho_Image, hv_MetrologyHandle);
                //HOperatorSet.GetMetrologyObjectMeasures(out HObject ho_Contours, hv_MetrologyHandle, "all", "all", out HTuple hv_CrossRow, out HTuple hv_CrossColumn);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "row", out HTuple hv_CenterRow);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "column", out HTuple hv_CenterCol);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "radius", out HTuple hv_Radius);

                //获取计量模型对象结果轮廓
                if (hv_Radius != null && hv_Radius.Length > 0)
                {
                    HOperatorSet.GetMetrologyObjectResultContour(out HObject ho_Contours, hv_MetrologyHandle, hv_Index, "all", 1.5);
                    HOperatorSet.GenCrossContourXld(out HObject ho_Cross, hv_CenterRow, hv_CenterCol, 10, (new HTuple(45)).TupleRad());
                    circleInfo = new CircleInfo();
                    circleInfo.center_X = hv_CenterCol[0].D;
                    circleInfo.center_Y = hv_CenterRow[0].D;
                    circleInfo.radius = hv_Radius[0].D;
                    circleInfo.ho_Contours = ho_Contours;
                    circleInfo.ho_Cross = ho_Cross;
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
            }
            try
            {

            }
            catch (Exception)
            {
                //清除测量模型
                //ho_Contours.Dispose();
                if (hv_MetrologyHandle != null) HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
            }
        }

        /// <summary>
        /// 找边
        /// </summary>
        /// <param name="ho_Image"></param>
        /// <param name="hv_MeasureRow1"></param>
        /// <param name="hv_MeasureColumn1"></param>
        /// <param name="hv_MeasureRow2"></param>
        /// <param name="hv_MeasureColumn2"></param>
        /// <param name="hv_MeasureLength1"></param>
        /// <param name="hv_MeasureLength2"></param>
        /// <param name="hv_MeasureSigma"></param>
        /// <param name="hv_MeasureThreshold"></param>
        /// <param name="hv_MeasureNumInstances"></param>
        /// <param name="hv_MeasureDistThreshold"></param>
        /// <param name="hv_MeasureTransition"></param>
        /// <param name="hv_MeasureSelect"></param>
        /// <param name="hv_MeasurePointsNum"></param>
        /// <param name="hv_MeasureMinScore"></param>
        /// <param name="hv_MaxNumIterations"></param>
        /// <param name="hv_MeasureInterpolation"></param>
        /// <param name="hv_MeasureIORegions"></param>
        /// <param name="lineInfos"></param>
        public static void FindLine(HObject ho_Image, HTuple hv_MeasureRow1, HTuple hv_MeasureColumn1, HTuple hv_MeasureRow2, HTuple hv_MeasureColumn2, HTuple hv_MeasureLength1, HTuple hv_MeasureLength2, HTuple hv_MeasureSigma, HTuple hv_MeasureThreshold,
                                    HTuple hv_MeasureNumInstances, HTuple hv_MeasureDistThreshold, HTuple hv_MeasureTransition, HTuple hv_MeasureSelect,
                                    HTuple hv_MeasurePointsNum, HTuple hv_MeasureMinScore, HTuple hv_MaxNumIterations, HTuple hv_MeasureInterpolation, HTuple hv_MeasureIORegions, out List<LineInfo1> lineInfos)
        {
            lineInfos = null;
            HTuple hv_MetrologyHandle = null;
            try
            {

                HOperatorSet.CreateMetrologyModel(out hv_MetrologyHandle);
                //HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);

                HOperatorSet.AddMetrologyObjectLineMeasure(hv_MetrologyHandle, hv_MeasureRow1, hv_MeasureColumn1, hv_MeasureRow2, hv_MeasureColumn2, hv_MeasureLength1, hv_MeasureLength2,
                             hv_MeasureSigma, hv_MeasureThreshold, new HTuple(), new HTuple(), out HTuple hv_Index);

                //1.拟合数量：默认值： 1 值的列表： 1，2，3，4
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_Index, "num_instances", hv_MeasureNumInstances);
                //2.距离剔除：默认值： 3.5  值的列表： 0，1.0，2.0， 3.5，5.0
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_Index, "distance_threshold", hv_MeasureDistThreshold);
                //3.明暗方向：值列表： 'all'，'negative'， 'positive'，'uniform'
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_Index, "measure_transition", hv_MeasureTransition);
                //4.边缘选取：'all'，'first'，'last'
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_Index, "measure_select", hv_MeasureSelect);
                //5.Roi数量：'num_measures'
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_Index, "num_measures", hv_MeasurePointsNum);
                //6.最小得分：'min_score'
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_Index, "min_score", hv_MeasureMinScore);
                //7.迭代次数: 默认值：-1  值的列表： 10，100，1000
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_Index, "max_num_iterations", hv_MaxNumIterations);
                //8.插值类型：nearest_neighbor'灰度值是从最近像素的灰度值获得；'bilinear'，使用双线性插值 ；'bicubic'，则使用双三次插值。
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_Index, "measure_interpolation", hv_MeasureInterpolation);
                //9.指定对测量结果的验证：默认值： “ false”  值列表： 'true'，'false'
                HOperatorSet.SetMetrologyObjectParam(hv_MetrologyHandle, hv_Index, "instances_outside_measure_regions", hv_MeasureIORegions);

                HOperatorSet.ApplyMetrologyModel(ho_Image, hv_MetrologyHandle);
                //HOperatorSet.GetMetrologyObjectMeasures(out ho_Contours, hv_MetrologyHandle, "all", "all", out hv_CrossRow, out hv_CrossColumn);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "row_begin", out HTuple hv_LineRowBegin);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "column_begin", out HTuple hv_LineColumnBegin);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "row_end", out HTuple hv_LineRowEnd);
                HOperatorSet.GetMetrologyObjectResult(hv_MetrologyHandle, "all", "all", "result_type", "column_end", out HTuple hv_LineColumnEnd);

                //获取计量模型对象结果轮廓
                if (hv_LineRowBegin != null && hv_LineRowBegin.Length > 0)
                {
                    lineInfos = new List<LineInfo1>();
                    for (int i = 0; i < hv_LineRowBegin.Length; i++)
                    {
                        lineInfos.Add(new LineInfo1());
                        lineInfos[i].RowBegin = hv_LineRowBegin[i].D;
                        lineInfos[i].ColBegin = hv_LineColumnBegin[i].D;
                        lineInfos[i].RowEnd = hv_LineRowEnd[i].D;
                        lineInfos[i].ColEnd = hv_LineColumnEnd[i].D;
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
            }

            try
            {
                //清除测量模型
                //ho_Contours.Dispose();
                if (hv_MetrologyHandle != null) HOperatorSet.ClearMetrologyModel(hv_MetrologyHandle);
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 找边
        /// </summary>
        /// <param name="ho_Image"></param>
        /// <param name="paramLineModel"></param>
        /// <param name="lineModel"></param>
        /// <param name="lineInfos"></param>
        public static void FindLine(HObject ho_Image, Param_LineModel paramLineModel, out List<LineInfo1> lineInfos)
        {
            lineInfos = null;
            try
            {
                HOperatorSet.TupleRad(paramLineModel.RoiAngle, out HTuple hv_RoiPhi);//将角度转换为弧度
                Rectangle2_ToLineModel(paramLineModel.RoiMidRow, paramLineModel.RoiMidColumn, hv_RoiPhi, paramLineModel.RoiLength1, paramLineModel.RoiLength2, paramLineModel.RoiPointsNum,
                   out double lineRow1, out double lineCol1, out double lineRow2, out double lineCol2, out double MeasureLength1, out double MeasureLength2);

                FindLine(ho_Image, lineRow1, lineCol1, lineRow2, lineCol2, MeasureLength1, MeasureLength2, paramLineModel.MesureSigma, paramLineModel.MesureThreshold,
                    paramLineModel.MeasureNumInstances, paramLineModel.MeasureDistThreshold, GetAnotherNamerAttribute((MeasureTransitionEnum)paramLineModel.MeasureTransition), GetAnotherNamerAttribute((MeasureSelectEnum)paramLineModel.MeasureSelect),
                    paramLineModel.RoiPointsNum, paramLineModel.MeasureMinScore, paramLineModel.MaxNumIterations, paramLineModel.MeasureInterpolation, paramLineModel.MeasureIORegions, out lineInfos);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
            }
        }

        /// <summary>
        /// 找边
        /// </summary>
        /// <param name="ho_Image"></param>
        /// <param name="paramLineModel"></param>
        /// <param name="lineModel"></param>
        /// <param name="lineInfos"></param>
        public static void FindLine(HObject ho_Image, Param_LineModel paramLineModel, double lineRow1, double lineCol1, double lineRow2, double lineCol2, double MeasureLength1, double MeasureLength2, out List<LineInfo1> lineInfos)
        {
            lineInfos = null;
            try
            {
                FindLine(ho_Image, lineRow1, lineCol1, lineRow2, lineCol2, MeasureLength1 , MeasureLength2, paramLineModel.MesureSigma, paramLineModel.MesureThreshold,
                    paramLineModel.MeasureNumInstances, paramLineModel.MeasureDistThreshold, GetAnotherNamerAttribute((MeasureTransitionEnum)paramLineModel.MeasureTransition), GetAnotherNamerAttribute((MeasureSelectEnum)paramLineModel.MeasureSelect),
                    paramLineModel.RoiPointsNum, paramLineModel.MeasureMinScore, paramLineModel.MaxNumIterations, paramLineModel.MeasureInterpolation, paramLineModel.MeasureIORegions, out lineInfos);
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
            }
        }

        /// <summary>
        /// 棋盘格找方格预处理
        /// </summary>
        /// <param name="ho_Image"></param>
        /// <param name="paramMultiCamCalib"></param>
        /// <param name="ho_Region"></param>
        public static void FindCheckerboardPretreatment(HObject ho_Image, double CheckerboardBoxSideLengthPix_X, double CheckerboardBoxSideLengthPix_Y, double CheckerboardMinThd, double CheckerboardMaxThd, out HObject ho_Region)
        {
            ho_Region = null;
            try
            {
                HTuple checkerboardArea = CheckerboardBoxSideLengthPix_X * CheckerboardBoxSideLengthPix_Y;
                double minArea = checkerboardArea * 0.8;
                double maxArea = checkerboardArea * 1.2;

                HOperatorSet.Threshold(ho_Image, out ho_Region, CheckerboardMinThd, CheckerboardMaxThd);

                HOperatorSet.OpeningRectangle1(ho_Region, out ho_Region, 20, 20);

                HOperatorSet.ClosingRectangle1(ho_Region, out ho_Region, 20, 20);

                HOperatorSet.Connection(ho_Region, out ho_Region);

                //HOperatorSet.FillUp(ho_Region, out ho_Region);

                HOperatorSet.ErosionRectangle1(ho_Region, out ho_Region, 10, 10);



                //HOperatorSet.Connection(ho_Region, out ho_Region);
                HOperatorSet.DilationRectangle1(ho_Region, out ho_Region, 10, 10);
                HOperatorSet.AreaCenter(ho_Region, out HTuple area, out HTuple row, out HTuple col);
                HOperatorSet.SelectShape(ho_Region, out ho_Region, "area", "and", minArea, maxArea);
                HOperatorSet.SelectShape(ho_Region, out ho_Region, "width", "and", CheckerboardBoxSideLengthPix_X-50<0? 0: CheckerboardBoxSideLengthPix_X - 50, CheckerboardBoxSideLengthPix_X+50);
                HOperatorSet.SelectShape(ho_Region, out ho_Region, "height", "and", CheckerboardBoxSideLengthPix_Y - 50 < 0 ? 0 : CheckerboardBoxSideLengthPix_Y - 50, CheckerboardBoxSideLengthPix_Y + 50);

                int count = ho_Region.CountObj();


            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
            }
            finally
            {

            }
        }

        /// <summary>
        /// 输入字段和类型，返回对应的属性值的AnotherNamerAttribute特性
        /// </summary>
        /// <param name="Language"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetAnotherNamerAttribute(object obj)
        {
            Type type = obj.GetType();
            FieldInfo fd = type.GetField(obj.ToString());
            if (fd == null) return string.Empty;
            object[] attrs = fd.GetCustomAttributes(typeof(AnotherNamerAttribute), false); //true还是false好像没区别
            string desp = string.Empty;
            AnotherNamerAttribute attr = attrs[0] as AnotherNamerAttribute;
            desp = attr.AnotherNamer;
            return desp;
        }

        /// <summary>
        /// 将HTuple转换为string[]
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static string[] HTupleToStringArray(HTuple tuple)
        {
            string[] stringArray = tuple.ToSArr();
            return stringArray;
        }

        /// <summary>
        /// 将HTuple转换为string
        /// </summary>
        /// <param name="tuple"></param>
        /// <returns></returns>
        public static string HTupleToString(HTuple tuple)
        {
            string[] stringArray = HTupleToStringArray(tuple);
            return string.Join(",", stringArray);
        }

        /// <summary>
        /// 将矩形1转换为线模型
        /// </summary>
        /// <param name="rectangleRow1"></param>
        /// <param name="rectangleCol1"></param>
        /// <param name="rectangleRow2"></param>
        /// <param name="rectangleCol2"></param>
        /// <param name="measureDirection"></param>
        /// <param name="RoiPointsNum"></param>
        /// <param name="lineRow1"></param>
        /// <param name="lineRow2"></param>
        /// <param name="lineCol1"></param>
        /// <param name="lineCol2"></param>
        /// <param name="MeasureLength1"></param>
        /// <param name="MeasureLength2"></param>
        public static void Rectangle1_ToLineModel(double rectangleRow1, double rectangleCol1, double rectangleRow2, double rectangleCol2, MeasureDirection measureDirection, int RoiPointsNum,
                                                  out double lineRow1, out double lineCol1, out double lineRow2, out double lineCol2, out double MeasureLength1, out double MeasureLength2)
        {
            Rectangle1ToRectangle2(rectangleRow1, rectangleCol1, rectangleRow2, rectangleCol2, measureDirection, out double rectangleMidRow, out double rectangleMidColumn, out double phi, out double rectangleLength1, out double rectangleLength2);
            Rectangle2_ToLineModel(rectangleMidRow, rectangleMidColumn, phi, rectangleLength1, rectangleLength2, RoiPointsNum, out lineRow1, out lineCol1, out lineRow2, out lineCol2, out MeasureLength1, out MeasureLength2);
        }

        /// <summary>
        /// 将矩形2转换为线模型
        /// </summary>
        /// <param name="rectangleMidRow"></param>
        /// <param name="rectangleMidColumn"></param>
        /// <param name="phi"></param>
        /// <param name="rectangleLength1"></param>
        /// <param name="rectangleLength2"></param>
        /// <param name="RoiPointsNum"></param>
        /// <param name="lineRow1"></param>
        /// <param name="lineRow2"></param>
        /// <param name="lineCol1"></param>
        /// <param name="lineCol2"></param>
        /// <param name="MeasureLength1"></param>
        /// <param name="MeasureLength2"></param>
        public static void Rectangle2_ToLineModel(double rectangleMidRow, double rectangleMidColumn, double phi, double rectangleLength1, double rectangleLength2, int RoiPointsNum,
                                                out double lineRow1, out double lineCol1, out double lineRow2, out double lineCol2, out double MeasureLength1, out double MeasureLength2)
        {

            //HOperatorSet.TupleRad(rectangleAngle, out HTuple hv_RoiPhi);//将角度转换为弧度
            //double rectangleAngleInDegrees = rectangleAngle * (180 / Math.PI);//将弧度转换为角度
            //HOperatorSet.TupleDeg();将弧度转换为角度

            lineRow1 = rectangleMidRow - rectangleLength2 * Math.Sin(phi) / 2;
            lineCol1 = rectangleMidColumn - rectangleLength2 * Math.Cos(phi) / 2;
            lineRow2 = rectangleMidRow + rectangleLength2 * Math.Sin(phi) / 2;
            lineCol2 = rectangleMidColumn + rectangleLength2 * Math.Cos(phi) / 2;

            RoiPointsNum = RoiPointsNum <= 0 ? 1 : RoiPointsNum;
            MeasureLength1 = rectangleLength1 / 2;
            MeasureLength2 = rectangleLength2 / RoiPointsNum / 2;//半高
        }

        /// <summary>
        /// 将矩形1转换为矩形2
        /// </summary>
        /// <param name="rectangleRow1"></param>
        /// <param name="rectangleCol1"></param>
        /// <param name="rectangleRow2"></param>
        /// <param name="rectangleCol2"></param>
        /// <param name="measureDirection"></param>
        /// <param name="rectangleMidRow"></param>
        /// <param name="rectangleMidColumn"></param>
        /// <param name="phi"></param>
        /// <param name="rectangleLength1"></param>
        /// <param name="rectangleLength2"></param>
        public static void Rectangle1ToRectangle2(double rectangleRow1, double rectangleCol1, double rectangleRow2, double rectangleCol2, MeasureDirection measureDirection,
                                               out double rectangleMidRow, out double rectangleMidColumn, out double phi, out double rectangleLength1, out double rectangleLength2)
        {
            switch (measureDirection)
            {
                case MeasureDirection.从左到右:
                    phi = -1.57; //-90;
                    break;
                case MeasureDirection.从右到左:
                    phi = 1.57;//90;
                    break;
                case MeasureDirection.从上到下:
                    phi = 0;
                    break;
                case MeasureDirection.从下到上:
                    phi = 3.14;//180;
                    break;
                default:
                    phi = 0;
                    break;
            }

            rectangleMidRow = (rectangleRow1 + rectangleRow2) / 2;
            rectangleMidColumn = (rectangleCol1 + rectangleCol2) / 2;
            //rectangleAngle = 0;       
            rectangleLength1 = (rectangleCol1 - rectangleCol2) / 2;
            rectangleLength2 = (rectangleRow1 - rectangleRow2) / 2;
        }

        /// <summary>
        /// 将线模型转换为矩形2
        /// </summary>
        /// <param name="lineRow1"></param>
        /// <param name="lineCol1"></param>
        /// <param name="lineRow2"></param>
        /// <param name="lineCol2"></param>
        /// <param name="MeasureLength2">半高</param>
        /// <param name="rectangleMidRow"></param>
        /// <param name="rectangleMidColumn"></param>
        /// <param name="phi"></param>
        /// <param name="rectangleLength1"></param>
        /// <param name="rectangleLength2"></param>      
        public static void LineModel_ToRectangle2(double lineRow1, double lineCol1, double lineRow2, double lineCol2, double MeasureLength2,
                                              out double rectangleMidRow, out double rectangleMidColumn, out double phi, out double rectangleLength1, out double rectangleLength2)
        {
            // 计算矩形中心点的坐标
            rectangleMidRow = (lineRow1 + lineRow2) / 2;
            rectangleMidColumn = (lineCol1 + lineCol2) / 2;

            // 计算线段的长度
            double deltaX = lineCol2 - lineCol1;
            double deltaY = lineRow2 - lineRow1;
            double lineLength = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            // 计算矩形的弧度
            phi = Math.Atan2(deltaY, deltaX);

            // 计算矩形的高度和宽度
            rectangleLength1 = lineLength / 2;
            rectangleLength2 = MeasureLength2;

        }


        /// <summary>
        /// 获取矩形的角点坐标（基于Phi）
        /// </summary>
        /// <param name="rectangleMidRow"></param>
        /// <param name="rectangleMidColumn"></param>
        /// <param name="phi"></param>
        /// <param name="rectangleHeight"></param>
        /// <param name="rectangleWidth"></param>
        /// <param name="topLeft_Row"></param>
        /// <param name="topLeft_Col"></param>
        /// <param name="topRight_Row"></param>
        /// <param name="topRight_Col"></param>
        /// <param name="lowerLeft_Row"></param>
        /// <param name="lowerLeft_Col"></param>
        /// <param name="lowerRight_Row"></param>
        /// <param name="lowerRight_Col"></param>
        public static void GetRectangleAngularPoint(double rectangleMidRow, double rectangleMidColumn, double phi, double rectangleHeight, double rectangleWidth,
                                                   out double topLeft_Row, out double topLeft_Col, out double topRight_Row, out double topRight_Col,
                                                   out double lowerLeft_Row, out double lowerLeft_Col, out double lowerRight_Row, out double lowerRight_Col)
        {
            // 计算矩形的宽度和高度
            double halfWidth = rectangleWidth;
            double halfHeight = rectangleHeight;

            // 计算矩形的四个角点相对于中心点的偏移
            double sinAngle = Math.Sin(phi);
            double cosAngle = Math.Cos(phi);

            // 计算矩形的四个角点坐标
            topLeft_Row = rectangleMidRow - halfHeight * cosAngle + halfWidth * sinAngle;
            topLeft_Col = rectangleMidColumn - halfWidth * cosAngle - halfHeight * sinAngle;

            topRight_Row = rectangleMidRow - halfHeight * cosAngle - halfWidth * sinAngle;
            topRight_Col = rectangleMidColumn + halfWidth * cosAngle - halfHeight * sinAngle;

            lowerLeft_Row = rectangleMidRow + halfHeight * cosAngle + halfWidth * sinAngle;
            lowerLeft_Col = rectangleMidColumn - halfWidth * cosAngle + halfHeight * sinAngle;

            lowerRight_Row = rectangleMidRow + halfHeight * cosAngle - halfWidth * sinAngle;
            lowerRight_Col = rectangleMidColumn + halfWidth * cosAngle + halfHeight * sinAngle;
        }


        /// <summary>
        /// 获取矩形的角点坐标（基于X,Y）
        /// </summary>
        /// <param name="ho_inputRectangle"></param>
        /// <param name="hv_TopLeft_Row"></param>
        /// <param name="hv_TopLeft_Col"></param>
        /// <param name="hv_TopRight_Row"></param>
        /// <param name="hv_TopRight_Col"></param>
        /// <param name="hv_LowerRight_Row"></param>
        /// <param name="hv_LowerRight_Col"></param>
        /// <param name="hv_LowerLeft_Row"></param>
        /// <param name="hv_LowerLeft_Col"></param>
        public static void Get_Rectangle_point(HObject ho_inputRectangle,
            out HTuple hv_TopLeft_Row, out HTuple hv_TopLeft_Col, out HTuple hv_TopRight_Row, out HTuple hv_TopRight_Col,
            out HTuple hv_LowerLeft_Row, out HTuple hv_LowerLeft_Col, out HTuple hv_LowerRight_Row, out HTuple hv_LowerRight_Col)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Phi = new HTuple(), hv_Length1 = new HTuple();
            HTuple hv_Length2 = new HTuple(), hv_Tmp = new HTuple();
            HTuple hv_Sin = new HTuple(), hv_Cos = new HTuple(), hv_TopLeft_X = new HTuple();
            HTuple hv_TopLeft_Y = new HTuple(), hv_TopRight_X = new HTuple();
            HTuple hv_TopRight_Y = new HTuple(), hv_LowerRight_X = new HTuple();
            HTuple hv_LowerRight_Y = new HTuple(), hv_LowerLeft_X = new HTuple();
            HTuple hv_LowerLeft_Y = new HTuple();
            // Initialize local and output iconic variables 
            hv_TopLeft_Row = new HTuple();
            hv_TopLeft_Col = new HTuple();
            hv_TopRight_Row = new HTuple();
            hv_TopRight_Col = new HTuple();
            hv_LowerRight_Row = new HTuple();
            hv_LowerRight_Col = new HTuple();
            hv_LowerLeft_Row = new HTuple();
            hv_LowerLeft_Col = new HTuple();
            //计算矩形的中间点、角度、及长宽（一半）
            hv_Row.Dispose(); hv_Column.Dispose(); hv_Phi.Dispose(); hv_Length1.Dispose(); hv_Length2.Dispose();
            HOperatorSet.SmallestRectangle2(ho_inputRectangle, out hv_Row, out hv_Column,
                out hv_Phi, out hv_Length1, out hv_Length2);

            if ((int)(new HTuple(((((hv_Phi.TupleDeg())).TupleAbs())).TupleGreater(45))) != 0)
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_Phi = (((hv_Phi.TupleDeg()
                            ) - (90 * (hv_Phi / (hv_Phi.TupleAbs()))))).TupleRad();
                        hv_Phi.Dispose();
                        hv_Phi = ExpTmpLocalVar_Phi;
                    }
                }
                hv_Tmp.Dispose();
                hv_Tmp = new HTuple(hv_Length1);
                hv_Length1.Dispose();
                hv_Length1 = new HTuple(hv_Length2);
                hv_Length2.Dispose();
                hv_Length2 = new HTuple(hv_Tmp);
            }

            hv_Sin.Dispose();
            HOperatorSet.TupleSin(hv_Phi, out hv_Sin);
            hv_Cos.Dispose();
            HOperatorSet.TupleCos(hv_Phi, out hv_Cos);

            //左上角
            hv_TopLeft_X.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TopLeft_X = ((-hv_Length1) * hv_Cos) - (hv_Length2 * hv_Sin);
            }
            hv_TopLeft_Y.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TopLeft_Y = ((-hv_Length1) * hv_Sin) + (hv_Length2 * hv_Cos);
            }
            hv_TopLeft_Row.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TopLeft_Row = hv_Row - hv_TopLeft_Y;
            }
            hv_TopLeft_Col.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TopLeft_Col = hv_Column + hv_TopLeft_X;
            }

            //右上角
            hv_TopRight_X.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TopRight_X = (hv_Length1 * hv_Cos) - (hv_Length2 * hv_Sin);
            }
            hv_TopRight_Y.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TopRight_Y = (hv_Length1 * hv_Sin) + (hv_Length2 * hv_Cos);
            }
            hv_TopRight_Row.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TopRight_Row = hv_Row - hv_TopRight_Y;
            }
            hv_TopRight_Col.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TopRight_Col = hv_Column + hv_TopRight_X;
            }

            //右下角
            hv_LowerRight_X.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_LowerRight_X = (hv_Length1 * hv_Cos) + (hv_Length2 * hv_Sin);
            }
            hv_LowerRight_Y.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_LowerRight_Y = (hv_Length1 * hv_Sin) - (hv_Length2 * hv_Cos);
            }
            hv_LowerRight_Row.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_LowerRight_Row = hv_Row - hv_LowerRight_Y;
            }
            hv_LowerRight_Col.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_LowerRight_Col = hv_Column + hv_LowerRight_X;
            }

            //左下角
            hv_LowerLeft_X.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_LowerLeft_X = ((-hv_Length1) * hv_Cos) + (hv_Length2 * hv_Sin);
            }
            hv_LowerLeft_Y.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_LowerLeft_Y = ((-hv_Length1) * hv_Sin) - (hv_Length2 * hv_Cos);
            }
            hv_LowerLeft_Row.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_LowerLeft_Row = hv_Row - hv_LowerLeft_Y;
            }
            hv_LowerLeft_Col.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_LowerLeft_Col = hv_Column + hv_LowerLeft_X;
            }

            hv_Row.Dispose();
            hv_Column.Dispose();
            hv_Phi.Dispose();
            hv_Length1.Dispose();
            hv_Length2.Dispose();
            hv_Tmp.Dispose();
            hv_Sin.Dispose();
            hv_Cos.Dispose();
            hv_TopLeft_X.Dispose();
            hv_TopLeft_Y.Dispose();
            hv_TopRight_X.Dispose();
            hv_TopRight_Y.Dispose();
            hv_LowerRight_X.Dispose();
            hv_LowerRight_Y.Dispose();
            hv_LowerLeft_X.Dispose();
            hv_LowerLeft_Y.Dispose();

            return;
        }


        /// <summary>
        /// 查找矩形的边缘坐标和角点坐标
        /// </summary>
        /// <param name="ho_Image"></param>
        /// <param name="ho_inputRectangle"></param>
        /// <param name="measureTransitionEnum"></param>
        /// <param name="leftEdgeBeginRow"></param>
        /// <param name="leftEdgeBeginCol"></param>
        /// <param name="leftEdgeEndRow"></param>
        /// <param name="leftEdgeEndCol"></param>
        /// <param name="topEdgeBeginRow"></param>
        /// <param name="topEdgeBeginCol"></param>
        /// <param name="topEdgeEndRow"></param>
        /// <param name="topEdgeEndCol"></param>
        /// <param name="rightEdgeBeginRow"></param>
        /// <param name="rightEdgeBeginCol"></param>
        /// <param name="rightEdgeEndRow"></param>
        /// <param name="rightEdgeEndCol"></param>
        /// <param name="bottomEdgeBeginRow"></param>
        /// <param name="bottomEdgeBeginCol"></param>
        /// <param name="bottomEdgeEndRow"></param>
        /// <param name="bottomEdgeEndCol"></param>
        /// <param name="leftTop_Row"></param>
        /// <param name="leftTop_Col"></param>
        /// <param name="rightTop_Row"></param>
        /// <param name="rightTop_Col"></param>
        /// <param name="leftBottom_Row"></param>
        /// <param name="leftBottom_Col"></param>
        /// <param name="rightBottom_Row"></param>
        /// <param name="rightBottom_Col"></param>
        public static bool FindRectangleEdge(HObject ho_Image, HObject ho_inputRectangle, MeasureTransitionEnum measureTransitionEnum,
            out double leftEdgeBeginRow, out double leftEdgeBeginCol, out double leftEdgeEndRow, out double leftEdgeEndCol,
            out double topEdgeBeginRow, out double topEdgeBeginCol, out double topEdgeEndRow, out double topEdgeEndCol,
            out double rightEdgeBeginRow, out double rightEdgeBeginCol, out double rightEdgeEndRow, out double rightEdgeEndCol,
            out double bottomEdgeBeginRow, out double bottomEdgeBeginCol, out double bottomEdgeEndRow, out double bottomEdgeEndCol,
            out double leftTop_Row, out double leftTop_Col, out double rightTop_Row, out double rightTop_Col,
            out double leftBottom_Row, out double leftBottom_Col, out double rightBottom_Row, out double rightBottom_Col)
        {
            bool result = true;
            leftEdgeBeginRow = 0;
            leftEdgeBeginCol = 0;
            leftEdgeEndRow = 0;
            leftEdgeEndCol = 0;
            topEdgeBeginRow = 0;
            topEdgeBeginCol = 0;
            topEdgeEndRow = 0;
            topEdgeEndCol = 0;
            rightEdgeBeginRow = 0;
            rightEdgeBeginCol = 0;
            rightEdgeEndRow = 0;
            rightEdgeEndCol = 0;
            bottomEdgeBeginRow = 0;
            bottomEdgeBeginCol = 0;
            bottomEdgeEndRow = 0;
            bottomEdgeEndCol = 0;

            leftTop_Row = 0;
            leftTop_Col = 0;
            rightTop_Row = 0;
            rightTop_Col = 0;
            leftBottom_Row = 0;
            leftBottom_Col = 0;
            rightBottom_Row = 0;
            rightBottom_Col = 0;

            try
            {
                Param_LineModel param_LineModel = new Param_LineModel();
                param_LineModel.MeasureTransition = (int)measureTransitionEnum;

                Get_Rectangle_point(ho_inputRectangle, out HTuple topLeft_Row, out HTuple topLeft_Col, out HTuple topRight_Row, out HTuple topRight_Col,
                    out HTuple lowerLeft_Row, out HTuple lowerLeft_Col, out HTuple lowerRight_Row, out HTuple lowerRight_Col);

                int lenght = 20;
                //矩形左边缘坐标
                if (true)
                {
                    FindLine(ho_Image, param_LineModel, topLeft_Row.D, topLeft_Col.D, lowerLeft_Row.D, lowerLeft_Col.D, lenght, lenght, out List<LineInfo1> lineInfos);

                    if (lineInfos != null && lineInfos.Count > 0)
                    {
                        leftEdgeBeginRow = lineInfos[0].RowBegin;
                        leftEdgeBeginCol = lineInfos[0].ColBegin;
                        leftEdgeEndRow = lineInfos[0].RowEnd;
                        leftEdgeEndCol = lineInfos[0].ColEnd;
                    }
                    else
                    {
                        result = false;
                    }
                }

                //矩形上边缘坐标
                if (true)
                {
                    FindLine(ho_Image, param_LineModel, topRight_Row.D, topRight_Col.D, topLeft_Row.D, topLeft_Col.D, lenght, lenght, out List<LineInfo1> lineInfos);

                    if (lineInfos != null && lineInfos.Count > 0)
                    {
                        topEdgeBeginRow = lineInfos[0].RowBegin;
                        topEdgeBeginCol = lineInfos[0].ColBegin;
                        topEdgeEndRow = lineInfos[0].RowEnd;
                        topEdgeEndCol = lineInfos[0].ColEnd;
                    }
                    else
                    {
                        result = false;
                    }

                }

                //矩形右边缘坐标
                if (true)
                {
                    FindLine(ho_Image, param_LineModel, lowerRight_Row.D, lowerRight_Col.D, topRight_Row.D, topRight_Col.D, lenght, lenght, out List<LineInfo1> lineInfos);

                    if (lineInfos != null && lineInfos.Count > 0)
                    {
                        rightEdgeBeginRow = lineInfos[0].RowBegin;
                        rightEdgeBeginCol = lineInfos[0].ColBegin;
                        rightEdgeEndRow = lineInfos[0].RowEnd;
                        rightEdgeEndCol = lineInfos[0].ColEnd;
                    }
                    else
                    {
                        result = false;
                    }
                }

                //矩形下边缘坐标
                if (true)
                {
                    FindLine(ho_Image, param_LineModel, lowerLeft_Row.D, lowerLeft_Col.D, lowerRight_Row.D, lowerRight_Col.D, lenght, lenght, out List<LineInfo1> lineInfos);

                    if (lineInfos != null && lineInfos.Count > 0)
                    {
                        bottomEdgeBeginRow = lineInfos[0].RowBegin;
                        bottomEdgeBeginCol = lineInfos[0].ColBegin;
                        bottomEdgeEndRow = lineInfos[0].RowEnd;
                        bottomEdgeEndCol = lineInfos[0].ColEnd;
                    }
                    else
                    {
                        result = false;
                    }
                }


                //计算角点坐标
                HTuple isOverlapping = null;
                //左上角
                HOperatorSet.IntersectionLines(new HTuple(leftEdgeBeginRow), new HTuple(leftEdgeBeginCol), new HTuple(leftEdgeEndRow), new HTuple(leftEdgeEndCol),
                                               new HTuple(topEdgeBeginRow), new HTuple(topEdgeBeginCol), new HTuple(topEdgeEndRow), new HTuple(topEdgeEndCol),
                                               out HTuple hv_leftTop_Row, out HTuple hv_leftTop_Col, out isOverlapping);
                if (hv_leftTop_Row != null && hv_leftTop_Row.Length > 0)
                {
                    leftTop_Row = hv_leftTop_Row.D;
                    leftTop_Col = hv_leftTop_Col.D;
                    leftTop_Row = double.IsNaN(leftTop_Row) ? 0 : leftTop_Row;
                    leftTop_Col = double.IsNaN(leftTop_Col) ? 0 : leftTop_Col;
                }

                //右上角
                HOperatorSet.IntersectionLines(new HTuple(rightEdgeBeginRow), new HTuple(rightEdgeBeginCol), new HTuple(rightEdgeEndRow), new HTuple(rightEdgeEndCol),
                                               new HTuple(topEdgeBeginRow), new HTuple(topEdgeBeginCol), new HTuple(topEdgeEndRow), new HTuple(topEdgeEndCol),
                                               out HTuple hv_rightTop_Row, out HTuple hv_rightTop_Col, out isOverlapping);
                if (hv_rightTop_Row != null && hv_rightTop_Row.Length > 0)
                {
                    rightTop_Row = hv_rightTop_Row.D;
                    rightTop_Col = hv_rightTop_Col.D;
                    rightTop_Row = double.IsNaN(rightTop_Row) ? 0 : rightTop_Row;
                    rightTop_Col = double.IsNaN(rightTop_Col) ? 0 : rightTop_Col;
                }

                //左下角
                HOperatorSet.IntersectionLines(new HTuple(leftEdgeBeginRow), new HTuple(leftEdgeBeginCol), new HTuple(leftEdgeEndRow), new HTuple(leftEdgeEndCol),
                                               new HTuple(bottomEdgeBeginRow), new HTuple(bottomEdgeBeginCol), new HTuple(bottomEdgeEndRow), new HTuple(bottomEdgeEndCol),
                                               out HTuple hv_leftBottom_Row, out HTuple hv_leftBottom_Col, out isOverlapping);
                if (hv_leftBottom_Row != null && hv_leftBottom_Row.Length > 0)
                {
                    leftBottom_Row = hv_leftBottom_Row.D;
                    leftBottom_Col = hv_leftBottom_Col.D;
                    leftBottom_Row = double.IsNaN(leftBottom_Row) ? 0 : leftBottom_Row;
                    leftBottom_Col = double.IsNaN(leftBottom_Col) ? 0 : leftBottom_Col;
                }

                //右下角
                HOperatorSet.IntersectionLines(new HTuple(rightEdgeBeginRow), new HTuple(rightEdgeBeginCol), new HTuple(rightEdgeEndRow), new HTuple(rightEdgeEndCol),
                                               new HTuple(bottomEdgeBeginRow), new HTuple(bottomEdgeBeginCol), new HTuple(bottomEdgeEndRow), new HTuple(bottomEdgeEndCol),
                                               out HTuple hv_rightBottom_Row, out HTuple hv_rightBottom_Col, out isOverlapping);
                if (hv_rightBottom_Row != null && hv_rightBottom_Row.Length > 0)
                {
                    rightBottom_Row = hv_rightBottom_Row.D;
                    rightBottom_Col = hv_rightBottom_Col.D;
                    rightBottom_Row = double.IsNaN(rightBottom_Row) ? 0 : rightBottom_Row;
                    rightBottom_Col = double.IsNaN(rightBottom_Col) ? 0 : rightBottom_Col;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
            }
            return result;
        }

        /// <summary>
        /// 计算四个角的角度和中心坐标
        /// </summary>
        /// <param name="leftUpRow"></param>
        /// <param name="leftUpCol"></param>
        /// <param name="rightUpRow"></param>
        /// <param name="rightUpCol"></param>
        /// <param name="leftDownRow"></param>
        /// <param name="leftDownCol"></param>
        /// <param name="rightDownRow"></param>
        /// <param name="rightDownCol"></param>
        /// <param name="centreRow"></param>
        /// <param name="centreCol"></param>
        /// <param name="leftUpAngle"></param>
        /// <param name="rightUpAngle"></param>
        /// <param name="leftDownAngle"></param>
        /// <param name="rightDownAngle"></param>
        /// <returns></returns>
        public static bool Calculate_X_Y_Angle(double leftUpRow, double leftUpCol, double rightUpRow, double rightUpCol, double leftDownRow, double leftDownCol, double rightDownRow, double rightDownCol,
                                        out double centreRow, out double centreCol, out double leftUpAngle, out double rightUpAngle, out double leftDownAngle, out double rightDownAngle)
        {
            centreRow = 0;
            centreCol = 0;
            leftUpAngle = 0;
            rightUpAngle = 0;
            leftDownAngle = 0;
            rightDownAngle = 0;
            try
            {
                centreRow = (leftUpRow + rightUpRow + leftDownRow + rightDownRow) / 4;
                centreCol = (leftUpCol + rightUpCol + leftDownCol + rightDownCol) / 4;

                HOperatorSet.AngleLl(leftUpRow, leftUpCol, leftDownRow, leftDownCol, leftUpRow, leftUpCol, rightUpRow, rightUpCol, out HTuple angle);
                HOperatorSet.TupleDeg(angle, out angle);
                leftUpAngle = angle.D;

                HOperatorSet.AngleLl(rightUpRow, rightUpCol, rightDownRow, rightDownCol, leftUpRow, leftUpCol, rightUpRow, rightUpCol, out angle);
                HOperatorSet.TupleDeg(angle, out angle);
                rightUpAngle = angle.D;

                HOperatorSet.AngleLl(leftUpRow, leftUpCol, leftDownRow, leftDownCol, leftDownRow, leftDownCol, rightDownRow, rightDownCol, out angle);
                HOperatorSet.TupleDeg(angle, out angle);
                leftDownAngle = angle.D;

                HOperatorSet.AngleLl(rightUpRow, rightUpCol, rightDownRow, rightDownCol, leftDownRow, leftDownCol, rightDownRow, rightDownCol, out angle);
                HOperatorSet.TupleDeg(angle, out angle);
                rightDownAngle = angle.D;

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                return false;
            }
            return true;
        }


        /// <summary>
        /// 计算水平方向线的锐角
        /// </summary>
        /// <param name="line1_Row1"></param>
        /// <param name="line1_Col1"></param>
        /// <param name="line1_Row2"></param>
        /// <param name="line1_Col2"></param>
        /// <param name="line2_Row1"></param>
        /// <param name="line2_Col1"></param>
        /// <param name="line2_Row2"></param>
        /// <param name="line2_Col2"></param>
        /// <param name="lineAngle"></param>
        /// <returns></returns>
        public static bool Calculate_LevelLine_AcuteAngle(double line_BeginRow, double line_BeginCol, double line_EndRow, double line_EndCol, out double lineAngle)
        {
            lineAngle = 0;
            try
            {
                double line_BeginRowTemp = 0;
                double line_BeginColTemp = 0;
                double line_EndRowTemp = 0;
                double line_EndColTemp = 0;
                if (line_BeginCol > line_EndCol)
                {
                    line_BeginRowTemp = line_EndRow;
                    line_BeginColTemp = line_EndCol;
                    line_EndRowTemp = line_BeginRow;
                    line_EndColTemp = line_BeginCol;
                }
                else
                {
                    line_BeginRowTemp = line_BeginRow;
                    line_BeginColTemp = line_BeginCol;
                    line_EndRowTemp = line_EndRow;
                    line_EndColTemp = line_EndCol;
                }
                HOperatorSet.AngleLl(0, 0, 0, 500, line_BeginRowTemp, line_BeginColTemp, line_EndRowTemp, line_EndColTemp, out HTuple angle);
                HOperatorSet.TupleDeg(angle, out angle);
                lineAngle = angle.D;

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 计算水平方向线的锐角
        /// </summary>
        /// <param name="line1_Row1"></param>
        /// <param name="line1_Col1"></param>
        /// <param name="line1_Row2"></param>
        /// <param name="line1_Col2"></param>
        /// <param name="line2_Row1"></param>
        /// <param name="line2_Col1"></param>
        /// <param name="line2_Row2"></param>
        /// <param name="line2_Col2"></param>
        /// <param name="lineAngle"></param>
        /// <returns></returns>
        public static bool Calculate_LevelLine_AcuteAngle(double line1_BeginRow, double line1_BeginCol, double line1_EndRow, double line1_EndCol, double line_BeginRow, double line_BeginCol, double line_EndRow, double line_EndCol, out double lineAngle)
        {
            lineAngle = 0;
            try
            {
                double line_BeginRowTemp = line_BeginRow;
                double line_BeginColTemp = line_BeginCol;
                double line_EndRowTemp = line_EndRow;
                double line_EndColTemp = line_EndCol;
                //if (line_BeginCol > line_EndCol)
                //{
                //    line_BeginRowTemp = line_EndRow;
                //    line_BeginColTemp = line_EndCol;
                //    line_EndRowTemp = line_BeginRow;
                //    line_EndColTemp = line_BeginCol;
                //}
 
                HOperatorSet.AngleLl(line1_BeginRow, line1_BeginCol, line1_EndRow, line1_EndCol, line_BeginRowTemp, line_BeginColTemp, line_EndRowTemp, line_EndColTemp, out HTuple angle);
                HOperatorSet.TupleDeg(angle, out angle);
                lineAngle = angle.D;

            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowA1"></param>
        /// <param name="columnA1"></param>
        /// <param name="rowA2"></param>
        /// <param name="columnA2"></param>
        /// <param name="rowB1"></param>
        /// <param name="columnB1"></param>
        /// <param name="rowB2"></param>
        /// <param name="columnB2"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static bool Calculate_Angle_ll(HTuple rowA1, HTuple columnA1, HTuple rowA2, HTuple columnA2, HTuple rowB2, HTuple columnB2, out HTuple angle)
        {
            angle = 0;
            try
            {
                HOperatorSet.AngleLl(rowA1, columnA1, rowA2, columnA2, rowA1, columnA1, rowB2, columnB2, out angle);
                HOperatorSet.TupleDeg(angle, out angle);
                return true;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Error(PopUpWindowsShow.ReplaceStringIgnoreCaseHalcon(ex.Message + ex.StackTrace));
                return false;
            }
        }

        // 计算直线与矩形的交点，并返回矩形内的线段
        public static bool ClipLineToRectangle(double x1, double y1, double x2, double y2,
            double rectLeft, double rectTop, double rectRight, double rectBottom,
            out double p1_X, out double p1_Y, out double p2_X, out double p2_Y)
        {
            p1_X = 0;
            p1_Y = 0;
            p2_X = 0;
            p2_Y = 0;

            try
            {
                // 参数方程 t = (x - x1) / (x2 - x1) = (y - y1) / (y2 - y1)
                double t1 = 0, t2 = 1;

                // 计算与左边界的交点
                if ((x2 - x1) != 0) // 避免除以零
                {
                    double t = (rectLeft - x1) / (x2 - x1);
                    if (t > t1 && t < t2)
                    {
                        if (t * (y2 - y1) + y1 >= rectTop && t * (y2 - y1) + y1 <= rectBottom)
                        {
                            t1 = t;
                        }
                    }
                }
                else if (x1 < rectLeft || x2 < rectLeft)
                {
                    // 如果线完全在左边界左侧，则没有交点
                    return false;
                }

                // 计算与右边界的交点
                if ((x2 - x1) != 0) // 避免除以零
                {
                    double t = (rectRight - x1) / (x2 - x1);
                    if (t > t1 && t < t2)
                    {
                        if (t * (y2 - y1) + y1 >= rectTop && t * (y2 - y1) + y1 <= rectBottom)
                        {
                            t2 = t;
                        }
                    }
                }
                else if (x1 > rectRight || x2 > rectRight)
                {
                    // 如果线完全在右边界右侧，则没有交点
                    return false;
                }

                // 计算与上边界的交点
                if ((y2 - y1) != 0) // 避免除以零
                {
                    double t = (rectTop - y1) / (y2 - y1);
                    if (t > t1 && t < t2)
                    {
                        if (t * (x2 - x1) + x1 >= rectLeft && t * (x2 - x1) + x1 <= rectRight)
                        {
                            t1 = t;
                        }
                    }
                }
                else if (y1 < rectTop || y2 < rectTop)
                {
                    // 如果线完全在上边界上方，则没有交点
                    return false;
                }

                // 计算与下边界的交点
                if ((y2 - y1) != 0) // 避免除以零
                {
                    double t = (rectBottom - y1) / (y2 - y1);
                    if (t > t1 && t < t2)
                    {
                        if (t * (x2 - x1) + x1 >= rectLeft && t * (x2 - x1) + x1 <= rectRight)
                        {
                            t2 = t;
                        }
                    }
                }
                else if (y1 > rectBottom || y2 > rectBottom)
                {
                    // 如果线完全在下边界下方，则没有交点
                    return false;
                }

                // 如果 t1 <= t2，则线段在矩形内有部分
                if (t1 <= t2)
                {
                    double x1Clipped = x1 + t1 * (x2 - x1);
                    double y1Clipped = y1 + t1 * (y2 - y1);
                    double x2Clipped = x1 + t2 * (x2 - x1);
                    double y2Clipped = y1 + t2 * (y2 - y1);
                    p1_X = x1Clipped;
                    p1_Y = y1Clipped;
                    p2_X = x2Clipped;
                    p2_Y = y2Clipped;
                    return true;
                }

                // 如果没有交点，则返回 null
                return false;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        /// <summary>
        /// 延长直线
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static LineInfo ExtendLin(LineInfo line1, double length)
        {
            try
            {
                double subRow = line1.RowEnd - line1.RowBegin;
                double subCol = line1.ColEnd - line1.ColBegin;

                // 计算线段的中点
                // Point middle = new Point((line1.RowBegink + End.X) / 2, (Start.Y + End.Y) / 2);

                double midRow = (line1.RowEnd + line1.RowBegin) / 2;
                double midCol = (line1.ColEnd + line1.ColBegin) / 2;
                // 计算线段的角度
                double angle = Math.Atan2(subRow, subCol);

                // 根据角度和长度计算新的起点和终点
                double SRow = (int)(midRow + length * Math.Sin(angle));
                double Scol = (int)(midCol + length * Math.Cos(angle));

                double ERow = (int)(midRow + length * Math.Sin(angle + Math.PI));
                double ECol = (int)(midCol + length * Math.Cos(angle + Math.PI));

                return new LineInfo { RowBegin = SRow, ColBegin = Scol, RowEnd = ERow, ColEnd = ECol };
            }
            catch
            {

            }
            return line1;

        }

    }
}
