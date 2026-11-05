using BatteryLugClassLibrary;
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


namespace Plugin.EarBurr
{
    [Category("极耳毛刺")]
    [DisplayName("极耳毛刺")]
    [Export("EarBurrObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class EarBurrObj : IPlugin
    {
        public PluginParamsBase PluginParams { get; set; } = new ParamsEarBurr();
        ParamsEarBurr EarBurrParams;
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();
        public string grooveName;
        public bool isExistModel = false;
        BLClassLib Battery = new BLClassLib();

        public event EventHandler<PluginExecuteCompletedEventArgs> ExecuteCompleted;

        public bool ExecuteObj()
        {
            EarBurrParams = PluginParams as ParamsEarBurr; //里氏转换

            EarBurrParams.ho_ImageProcessed?.Dispose();
            HOperatorSet.GenEmptyObj(out EarBurrParams.ho_ImageProcessed);
            EarBurrParams.DefectCount = 0;
            foreach (NgTypeInfo item in EarBurrParams.List_Defect)
            {
                item.Dispose();
            }
            EarBurrParams.List_Defect.Clear();

            HObject ho_Rectangle2, ho_ImageReduce;
            HObject ho_TransRegion, ho_SelRegion;

            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_ImageReduce);


            HOperatorSet.GenEmptyObj(out ho_TransRegion);
            HOperatorSet.GenEmptyObj(out ho_SelRegion);

            HTuple DefectNum = 0;


            try
            {


                HTuple hv_Channels = 1;
                HTuple hv_RoiPhi;
                HOperatorSet.TupleRad(EarBurrParams.hv_RoiAngle, out hv_RoiPhi);

                HOperatorSet.GenRectangle2(out ho_Rectangle2, EarBurrParams.hv_RoiMidRow, EarBurrParams.hv_RoiMidColumn, -hv_RoiPhi, EarBurrParams.hv_RoiHeight / 2, EarBurrParams.hv_RoiWidth / 2);
                HOperatorSet.Difference(ho_Rectangle2, EarBurrParams.ho_ShieldRegion, out ho_Rectangle2);

                //图像预处理
                HOperatorSet.Rgb1ToGray(EarBurrParams.ImageInfo.ho_image, out EarBurrParams.ho_ImageProcessed);
                HOperatorSet.ReduceDomain(EarBurrParams.ho_ImageProcessed, ho_Rectangle2, out ho_ImageReduce);
                //毛刺检出算法DLL。
                if (EarBurrParams.Bl_IsHalfEar)
                {
                    Battery.EntryBattery_HalfEar(ho_ImageReduce,
                    EarBurrParams.hv_RminArea / EarBurrParams.Pix_X / EarBurrParams.Pix_Y,
                    EarBurrParams.hv_RminWidth / EarBurrParams.Pix_X,
                    EarBurrParams.hv_RminHeigth / EarBurrParams.Pix_Y,
                    EarBurrParams.hv_HminArea / EarBurrParams.Pix_X / EarBurrParams.Pix_Y,
                    EarBurrParams.hv_HminWidth / EarBurrParams.Pix_X,
                    EarBurrParams.hv_HminHeigth / EarBurrParams.Pix_Y,
                    EarBurrParams.hv_HyminArea / EarBurrParams.Pix_X / EarBurrParams.Pix_Y,
                    EarBurrParams.hv_HyminWidth / EarBurrParams.Pix_X,
                    EarBurrParams.hv_HyminHeigth / EarBurrParams.Pix_Y,
                    out HObject YL2_ROI, out HTuple hv_AreaValue, out HTuple hv_WidthValue, out HTuple hv_HeightValue);
                    HOperatorSet.ClosingCircle(YL2_ROI, out ho_TransRegion, 3.5);
                }
                else
                {
                    Battery.EntryBattery(ho_ImageReduce,
                    EarBurrParams.hv_RminArea / EarBurrParams.Pix_X / EarBurrParams.Pix_Y,
                    EarBurrParams.hv_RminWidth / EarBurrParams.Pix_X,
                    EarBurrParams.hv_RminHeigth / EarBurrParams.Pix_Y,
                    EarBurrParams.hv_HminArea / EarBurrParams.Pix_X / EarBurrParams.Pix_Y,
                    EarBurrParams.hv_HminWidth / EarBurrParams.Pix_X,
                    EarBurrParams.hv_HminHeigth / EarBurrParams.Pix_Y,
                    EarBurrParams.hv_HyminArea / EarBurrParams.Pix_X / EarBurrParams.Pix_Y,
                    EarBurrParams.hv_HyminWidth / EarBurrParams.Pix_X,
                    EarBurrParams.hv_HyminHeigth / EarBurrParams.Pix_Y,
                    out HObject YL2_ROI, out HTuple hv_AreaValue, out HTuple hv_WidthValue, out HTuple hv_HeightValue);
                    HOperatorSet.ClosingCircle(YL2_ROI, out ho_TransRegion, 3.5);
                }

                HOperatorSet.Connection(ho_TransRegion, out ho_SelRegion);
                //统计缺陷
                HOperatorSet.CountObj(ho_SelRegion, out DefectNum);
                for (int i = 0; i < DefectNum.I; i++)
                {
                    HOperatorSet.SelectObj(ho_SelRegion, out HObject ho_SsRegion, i + 1);
                    HOperatorSet.AreaCenter(ho_SsRegion, out HTuple area, out HTuple row, out HTuple column);
                    HOperatorSet.Circularity(ho_SsRegion, out HTuple Circularity);
                    HOperatorSet.SmallestRectangle2(ho_SsRegion, out HTuple row02, out HTuple column02, out HTuple phi, out HTuple length1, out HTuple length2);
                    HOperatorSet.SmallestRectangle1(ho_SsRegion, out HTuple row1, out HTuple column1, out HTuple row2, out HTuple column2);
                    double row11 = row1.D - 50 > 0 ? row1.D - 50 : row1.D;
                    double row22 = row2.D + 50 > EarBurrParams.ImageInfo.imageHeight ? row2.D : row2.D + 50;

                    double col11 = column1.D - 50 > 0 ? column1.D - 50 : column1.D;
                    double col22 = column2.D + 50 > EarBurrParams.ImageInfo.imageWidth ? column2.D : column2.D + 50;

                    HOperatorSet.CropRectangle1(EarBurrParams.ho_ImageProcessed, out HObject imagePart, row11, col11, row22, col22);
                    HOperatorSet.GetImageSize(imagePart, out HTuple hv_Width, out HTuple hv_Height);

                    if (length1.D != 0 || length2.D != 0)
                    {
                        EarBurrParams.List_Defect.Add(
                        new NgTypeInfo
                        {
                            m_hRegion = ho_SsRegion,
                            m_hImage = imagePart,
                            m_nImgIndex = EarBurrParams.ImageInfo.image_ID,
                            NgMidRow = row.D,
                            NgMidColumn = column.D,

                            m_fNgTypWidth = length1,
                            m_fNgTypHeight = length2,
                            m_fNgTypArea = area.D,
                            circularity = Circularity.D * 100,
                        });
                        EarBurrParams.DefectCount++;
                    }

                }

                EarBurrParams.bl_ModelRunFlag = true;
            }
            catch (Exception)
            {
                EarBurrParams.bl_ModelRunFlag = false;
            }
            finally
            {
                ho_Rectangle2.Dispose();
                ho_ImageReduce.Dispose();

                ho_TransRegion.Dispose();
                ho_SelRegion.Dispose();
            }

            return EarBurrParams.bl_ModelRunFlag;


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
            this.PluginParams = (ParamsEarBurr)param;
            EarBurrParams = PluginParams as ParamsEarBurr; //里氏转换
        }
        public void IniObj()
        {

        }

        public void DisPoseObj()
        {

        }
    }
}
