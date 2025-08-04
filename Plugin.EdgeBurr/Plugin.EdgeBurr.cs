using Frame_Core;
using HalconDotNet;
using Plugin.DetectDefect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.EdgeBurr
{
    [Category("直边毛刺")]
    [DisplayName("直边毛刺")]
    [Export("EdgeBurrObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class EdgeBurrObj : IPlugin
    {
        public PluginParamsBase PluginParams { get; set; } = new ParamsEdgeBurr();
        ParamsEdgeBurr EdgeBurrParams;
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();

        public string grooveName;
        public bool isExistModel = false;

        public bool ExecuteObj()
        {
            EdgeBurrParams = PluginParams as ParamsEdgeBurr; //里氏转换

            EdgeBurrParams.ho_ImageProcessed?.Dispose();
            HOperatorSet.GenEmptyObj(out EdgeBurrParams.ho_ImageProcessed);
            EdgeBurrParams.DefectCount = 0;
            foreach (NgTypeInfo item in EdgeBurrParams.List_Defect)
            {
                item.Dispose();
            }
            EdgeBurrParams.List_Defect.Clear();

            HObject ho_Rectangle2, ho_ImageReduce;
            HObject ho_Border, ho_SmoothBorder, ho_DifferBorder;
            HObject ho_DifferRegion, ho_TransRegion, ho_SelRegion;

            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_ImageReduce);

            HOperatorSet.GenEmptyObj(out ho_Border);
            HOperatorSet.GenEmptyObj(out ho_SmoothBorder);
            HOperatorSet.GenEmptyObj(out ho_DifferBorder);

            HOperatorSet.GenEmptyObj(out ho_DifferRegion);
            HOperatorSet.GenEmptyObj(out ho_TransRegion);
            HOperatorSet.GenEmptyObj(out ho_SelRegion);

            HTuple DefectNum = 0;


            try
            {
                HTuple hv_Channels = 1;
                HTuple hv_RoiPhi;
                HOperatorSet.TupleRad(EdgeBurrParams.hv_RoiAngle, out hv_RoiPhi);

                HOperatorSet.GenRectangle2(out ho_Rectangle2, EdgeBurrParams.hv_RoiMidRow, EdgeBurrParams.hv_RoiMidColumn, -hv_RoiPhi, EdgeBurrParams.hv_RoiHeight / 2, EdgeBurrParams.hv_RoiWidth / 2);
                HOperatorSet.Difference(ho_Rectangle2, EdgeBurrParams.ho_ShieldRegion, out ho_Rectangle2);

                //图像预处理
                HOperatorSet.Rgb1ToGray(EdgeBurrParams.ImageInfo.ho_image, out EdgeBurrParams.ho_ImageProcessed);

                //毛刺检出
                HOperatorSet.ReduceDomain(EdgeBurrParams.ho_ImageProcessed, ho_Rectangle2, out ho_ImageReduce);

                HOperatorSet.ThresholdSubPix(ho_ImageReduce, out ho_Border, EdgeBurrParams.hv_EdgeThreshold);
                HOperatorSet.SmoothContoursXld(ho_Border, out ho_SmoothBorder, EdgeBurrParams.hv_EdgeSmooth);
                if (EdgeBurrParams.Is_WhiteBurr)
                {
                    HOperatorSet.DifferenceClosedContoursXld(ho_Border, ho_SmoothBorder, out ho_DifferBorder);
                }
                else
                {
                    HOperatorSet.DifferenceClosedContoursXld(ho_SmoothBorder, ho_Border, out ho_DifferBorder);
                }
                HOperatorSet.GenRegionContourXld(ho_DifferBorder, out ho_DifferRegion, "filled");
                HOperatorSet.ShapeTrans(ho_DifferRegion, out ho_TransRegion, "convex");
                //区域筛选
                //HOperatorSet.CountObj(ho_SelRegion, out DefectNum);
                HOperatorSet.SelectShape(ho_TransRegion, out ho_SelRegion, "width", "and", EdgeBurrParams.hv_SelectWidth/ EdgeBurrParams.Pix_X, 99999999);//筛选宽度方向。
                //  HOperatorSet.CountObj(ho_SelRegion, out DefectNum);
                HOperatorSet.SelectShape(ho_SelRegion, out ho_SelRegion, "height", "and", EdgeBurrParams.hv_SelectHeight / EdgeBurrParams.Pix_Y, 99999999);//筛选高度方向。
                // HOperatorSet.CountObj(ho_SelRegion, out DefectNum);
                HOperatorSet.SelectShape(ho_SelRegion, out ho_SelRegion, "area", "and", EdgeBurrParams.hv_SelectAreaMin / EdgeBurrParams.Pix_X / EdgeBurrParams.Pix_Y, EdgeBurrParams.hv_SelectAreaMax / EdgeBurrParams.Pix_X / EdgeBurrParams.Pix_Y);//筛选面积。
                HOperatorSet.SelectShape(ho_SelRegion, out ho_SelRegion, "circularity", "and", EdgeBurrParams.hv_SelectCirMin / 100.0, EdgeBurrParams.hv_SelectCirMax / 100.0);//筛选圆度。

                //统计缺陷
                HOperatorSet.CountObj(ho_SelRegion, out DefectNum);

                for (int i = 0; i < DefectNum.I; i++)
                {
                    HOperatorSet.SelectObj(ho_SelRegion, out HObject ho_SsRegion, i + 1);
                    HOperatorSet.AreaCenter(ho_SsRegion, out HTuple area, out HTuple row, out HTuple column);
                    HOperatorSet.Circularity(ho_SsRegion, out HTuple Circularity);
                    HOperatorSet.SmallestRectangle1(ho_SsRegion, out HTuple row1, out HTuple column1, out HTuple row2, out HTuple column2);
                    double row11 = row1.D - 50 > 0 ? row1.D - 50 : row1.D;
                    double row22 = row2.D + 50 > EdgeBurrParams.ImageInfo.imageHeight ? row2.D : row2.D + 50;

                    double col11 = column1.D - 50 > 0 ? column1.D - 50 : column1.D;
                    double col22 = column2.D + 50 > EdgeBurrParams.ImageInfo.imageWidth ? column2.D : column2.D + 50;

                    HOperatorSet.CropRectangle1(EdgeBurrParams.ho_ImageProcessed, out HObject imagePart, row11, col11, row22, col22);
                    HOperatorSet.GetImageSize(imagePart, out HTuple hv_Width, out HTuple hv_Height);

                    EdgeBurrParams.List_Defect.Add(
                        new NgTypeInfo
                        {
                            m_hRegion = ho_SsRegion,
                            m_hImage = imagePart,
                            m_nImgIndex = EdgeBurrParams.ImageInfo.image_ID,
                            NgMidRow = row.D,
                            NgMidColumn = column.D,

                            m_fNgTypWidth = column2.D - column1.D,
                            m_fNgTypHeight = row2.D - row1.D,
                            m_fNgTypArea = area.D,
                            circularity = Circularity.D * 100,
                        });
                    EdgeBurrParams.DefectCount++;
                }

                EdgeBurrParams.bl_ModelRunFlag = true;
            }
            catch (Exception e)
            {

                EdgeBurrParams.bl_ModelRunFlag = false;
            }
            finally
            {
                ho_Rectangle2.Dispose();
                ho_ImageReduce.Dispose();

                ho_Border.Dispose();
                ho_SmoothBorder.Dispose();
                ho_DifferBorder.Dispose();

                ho_DifferRegion.Dispose();
                ho_TransRegion.Dispose();
                ho_SelRegion.Dispose();
            }

            return EdgeBurrParams.bl_ModelRunFlag;


        }

        public void GetResultParams(string varName, object obj)
        {
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
            this.PluginParams = (ParamsEdgeBurr)param;
            EdgeBurrParams = PluginParams as ParamsEdgeBurr; //里氏转换
        }
        public void IniObj()
        {



        }

        public void DisPoseObj()
        {

        }
    }
}
