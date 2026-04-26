using Frame_Core.pluginParams;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plugin.FindModel
{
    public class MatclTool
    {

        /// <summary>
        /// Optimization：输入参数，设置模板优化和模板创建方法。默认 'auto'，范围
        ///【'auto', 'no_pregeneration',
        ///'none'存储所有模型点,
        ///'point_reduction_high', 'point_reduction_low',
        ///'point_reduction_medium', 'pregeneration'
        ///】
        /// </summary>
        public static Dictionary<int, string> m_dicOptimization_Create = new Dictionary<int, string>();

        /// <summary>
        ///   Metric：输入参数，设置匹配方法。默认 'use_polarity'，范围
        ///【
        ///'ignore_color_polarity', 表示匹配过程忽略颜色对比度的变化，一般适用于多通道图像。如果不确定检测时会显示哪个通道的图像，可以选择此项。
        ///'ignore_global_polarity', 适用于全局对比度发生变化的情况，表示忽略全局对比度的变化，即匹配过程中的图像对比度可以与模板中的完全相反。例如，模板中是一个暗背景上有一个亮的目标，那么在检索时，即使目标对象很亮而背景很暗，也能匹配成功。
        ///'ignore_local_polarity', 表示忽略局部对比度的变化，如果图像因为光照改变等原因发生局部对比度变化，可以选择这一项。
        ///'use_polarity'表示匹配得到的图像必须和模板图像的对比度“方向”相同。例如：模板中是一个暗背景上有一个亮的目标，那么在检索时，只有符合匹配条件并且亮度比背景亮的目标才能匹配成功。
        ///】
        /// </summary>
        public static Dictionary<int, string> m_dicMetric_Create = new Dictionary<int, string>();

        public static Dictionary<int, string> m_dicSubPixel_FindModel = new Dictionary<int, string>();




        public static void CreatModelMethor(CreateModelParam createParamsD, HObject ho_image, out HTuple modelID)
        {
            HTuple hTuple = 0;
            HOperatorSet.TupleRad(createParamsD.m_nAngleStart_Create, out HTuple m_nAngleStart_Create);
            HOperatorSet.TupleRad(createParamsD.m_nAngleExtent_Create, out HTuple m_nAngleExtent_Create);
            if (createParamsD.m_nNumLevels_Create == 11 && createParamsD.m_nAngleStep_Create == 0)
            {
                HOperatorSet.CreateShapeModel(ho_image, "auto", m_nAngleStart_Create, m_nAngleExtent_Create,
             "auto", m_dicOptimization_Create[createParamsD.m_nOptimization_Create] , m_dicMetric_Create[createParamsD.m_nMetric_Create] ,
             "auto", "auto", out hTuple);
            }
            if (createParamsD.m_nNumLevels_Create != 11 && createParamsD.m_nAngleStep_Create == 0)
            {

                HOperatorSet.CreateShapeModel(ho_image, createParamsD.m_nNumLevels_Create, m_nAngleStart_Create, m_nAngleExtent_Create,
              "auto", m_dicOptimization_Create[createParamsD.m_nOptimization_Create], m_dicMetric_Create[createParamsD.m_nMetric_Create],
             "auto", "auto", out hTuple);

            }
            if (createParamsD.m_nNumLevels_Create == 11 && createParamsD.m_nAngleStep_Create != 0)
            {
                HOperatorSet.CreateShapeModel(ho_image, "auto", m_nAngleStart_Create, m_nAngleExtent_Create,
             createParamsD.m_nAngleStep_Create, m_dicOptimization_Create[createParamsD.m_nOptimization_Create], m_dicMetric_Create[createParamsD.m_nMetric_Create],
            "auto", "auto", out hTuple);
            }

            if (createParamsD.m_nNumLevels_Create != 11 && createParamsD.m_nAngleStep_Create != 0)
            {
                HOperatorSet.CreateShapeModel(ho_image, createParamsD.m_nNumLevels_Create, m_nAngleStart_Create, m_nAngleExtent_Create,
              createParamsD.m_nAngleStep_Create, m_dicOptimization_Create[createParamsD.m_nOptimization_Create], m_dicMetric_Create[createParamsD.m_nMetric_Create],
             "auto", "auto", out hTuple);
            }


            modelID = hTuple;
        }

        public static void FindModelMethor(FindModelParam findModelParam, HTuple modelID, HObject ho_image, out HTuple Find_Row, out HTuple Find_Col, out HTuple Find_Angle, out HTuple Find_Score)
        {
            HOperatorSet.TupleRad(findModelParam.m_nAngleStart_Find, out HTuple m_nAngleStart_Find);
            HOperatorSet.TupleRad(findModelParam.m_nAngleExtent_Find, out HTuple m_nAngleExtent_Find);
            HOperatorSet.FindShapeModel(ho_image, modelID, m_nAngleStart_Find, m_nAngleExtent_Find, findModelParam.m_fMinScore_Find,
                                               findModelParam.m_nNumMatches_Find, findModelParam.m_fMaxOverlap_Find,
                                               m_dicSubPixel_FindModel[findModelParam.m_nSubPixel_Find] , findModelParam.m_nNumLevels_Find,
                                               findModelParam.m_fGreediness_Find, out Find_Row, out Find_Col, out Find_Angle, out Find_Score);
            
        }


        public static void CreatNccModelMethor(CreateModelParam createParamsD, HObject ho_image, out HTuple modelID)
        {
            HTuple hTuple = 0;
            HOperatorSet.TupleRad(createParamsD.m_nAngleStart_Create, out HTuple m_nAngleStart_Create);
            HOperatorSet.TupleRad(createParamsD.m_nAngleExtent_Create, out HTuple m_nAngleExtent_Create);


            if (createParamsD.m_nNumLevels_Create == 11 && createParamsD.m_nAngleStep_Create == 0)
            {
                HOperatorSet.CreateNccModel(ho_image, "auto", m_nAngleStart_Create, m_nAngleExtent_Create, "auto", m_dicMetric_Create[createParamsD.m_nMetric_Create], out hTuple);
            }
            if (createParamsD.m_nNumLevels_Create != 11 && createParamsD.m_nAngleStep_Create == 0)
            {
                HOperatorSet.CreateNccModel(ho_image, createParamsD.m_nNumLevels_Create, m_nAngleStart_Create, m_nAngleExtent_Create, "auto", m_dicMetric_Create[createParamsD.m_nMetric_Create], out hTuple);
            }
            if (createParamsD.m_nNumLevels_Create == 11 && createParamsD.m_nAngleStep_Create != 0)
            {
                HOperatorSet.CreateNccModel(ho_image, "auto", m_nAngleStart_Create, m_nAngleExtent_Create, createParamsD.m_nAngleStep_Create, m_dicMetric_Create[createParamsD.m_nMetric_Create], out hTuple);
            }
            if (createParamsD.m_nNumLevels_Create != 11 && createParamsD.m_nAngleStep_Create != 0)
            {
                HOperatorSet.CreateNccModel(ho_image, createParamsD.m_nNumLevels_Create, m_nAngleStart_Create, m_nAngleExtent_Create, createParamsD.m_nAngleStep_Create, m_dicMetric_Create[createParamsD.m_nMetric_Create], out hTuple);
            }
            modelID = hTuple;
        }


        public static void FindNccModelMethor(FindModelParam findModelParam, HTuple modelID, HObject ho_image, out HTuple Find_Row, out HTuple Find_Col, out HTuple Find_Angle, out HTuple Find_Score)
        {
            HOperatorSet.TupleRad(findModelParam.m_nAngleStart_Find, out HTuple m_nAngleStart_Find);
            HOperatorSet.TupleRad(findModelParam.m_nAngleExtent_Find, out HTuple m_nAngleExtent_Find);
            HOperatorSet.FindNccModel(ho_image, modelID, m_nAngleStart_Find, m_nAngleExtent_Find, findModelParam.m_fMinScore_Find, findModelParam.m_nNumMatches_Find, findModelParam.m_fMaxOverlap_Find, "true"
                      , findModelParam.m_nNumLevels_Find, out Find_Row, out Find_Col, out Find_Angle, out Find_Score);
        }


    }
}
