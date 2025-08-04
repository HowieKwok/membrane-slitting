using BatteryLugClassLibrary;
using Frame_Core;
using HalconDotNet;
using Plugin.DetectDefect;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Plugin.HandEyeCalibration
{
    [Category("九点标定")]
    [DisplayName("九点标定")]
    [Export("HandEyeCalibrationObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class HandEyeCalibrationObj : IPlugin
    {
        public PluginParamsBase PluginParams { get; set; } = new ParamsHandEyeCalibration();
        ParamsHandEyeCalibration EarBurrParams;
        public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();
        public string grooveName;
        public bool isExistModel = false;
       

      
        /// <summary>
        /// 面积标定
        /// </summary>
        private double AreaCali = 0.003;


        public bool ExecuteObj()
        {
            EarBurrParams = PluginParams as ParamsHandEyeCalibration; //里氏转换

            EarBurrParams.ho_ImageProcessed?.Dispose();
            HOperatorSet.GenEmptyObj(out EarBurrParams.ho_ImageProcessed);


            EarBurrParams.outputRobotX = -1;
            EarBurrParams.outputRobotY = -1;
            try
            {
                HOperatorSet.AffineTransPoint2d(EarBurrParams.HomMat2D, EarBurrParams.inputImageX, EarBurrParams.inputImageY,
                    out HTuple outputRobotX, out HTuple outputRobotY);

                EarBurrParams.outputRobotX = outputRobotX.D;
                EarBurrParams.outputRobotY = outputRobotY.D;
                EarBurrParams.bl_ModelRunFlag = true;
            }
            catch (Exception ex)
            {
                EarBurrParams.bl_ModelRunFlag = false;
            }
            finally
            {
              
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
            this.PluginParams = (ParamsHandEyeCalibration)param;
            EarBurrParams = PluginParams as ParamsHandEyeCalibration; //里氏转换
        }
        public void IniObj()
        {
            InitHomMat();
        }
        /// <summary>
        /// 初始化矩阵
        /// </summary>
        public void InitHomMat()
        {
            try
            {
                ParamsHandEyeCalibration paramsHandEyeCalibration = (ParamsHandEyeCalibration)PluginParams;
                paramsHandEyeCalibration.HomMat2D = new HTuple();
                string directoryPath = Path.GetDirectoryName(Project.Instance.paramPathSet.ProcessPath);
                string ProcessPath = Path.Combine(directoryPath + "\\HomMat\\", moduleInfo.ProcessName);
                string modelFilePath = Path.Combine(ProcessPath, moduleInfo.ModuleName);
                modelFilePath += ".txt";

                if (File.Exists(modelFilePath))
                {
                    HOperatorSet.ReadTuple(modelFilePath, out paramsHandEyeCalibration.HomMat2D);

                    
                }
                else
                {
                    
                }
            }
            catch (Exception ex)
            {

            }

        }
        public void DisPoseObj()
        {

        }
    }
}
