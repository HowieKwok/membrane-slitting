using Frame_Core;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using YYSD.UICore.Control;
using YYSD.UICore;
using ClassifyCPU_onnx_test;
using HalconDotNet;
using System.Collections.Generic;
using LogTool;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Reflection;
using static System.Windows.Forms.AxHost;

namespace Plugin.MQDefects
{
    [Category("瑕疵检测")]
    [DisplayName("模切瑕疵检测工具")]
    [Export("MQDefectsObj", typeof(IPlugin))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Serializable]
    public class MQDefectsObj : IPlugin
    {
        /// <summary>
        /// 模块信息
        /// </summary>
        public ModuleInfo moduleInfo { get; set; }=new ModuleInfo();
        /// <summary>
        /// 视觉设置参数
        /// </summary>
        public PluginParamsBase PluginParams { get ; set  ; }=new ParamsMQDefects();


        public bool isExistModel = true;
        /// <summary>
        /// 导入模型
        /// </summary>
        private ClassificationClient ClassDefect = null;

        /// <summary>
        /// 检测算法
        /// </summary>
        private CoatingDefectsAlog CoatingAlog;




        


        /// <summary>
        /// 执行
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool ExecuteObj()
        {
            try
            {
                ParamsMQDefects paramsMQDefects = (ParamsMQDefects)PluginParams;

                //初始化 释放以前图像
                foreach (NgTypeInfo info in paramsMQDefects.detectInfos)
                {
                    info.m_hImage?.Dispose();
                    info.m_hRegion?.Dispose();
                }
                paramsMQDefects.detectInfos.Clear();
                paramsMQDefects.currentExistingDefect = true;

                object[] inputs = new object[2];

                object[] outputs = null;

                HObject ShieldROI;
                HOperatorSet.GenEmptyObj(out ShieldROI);
                HObject DetectionROI;
                HOperatorSet.GenEmptyObj(out DetectionROI);
               
                if (paramsMQDefects.cb_Enable_SimulationRegion)
                {
                    HOperatorSet.GenRectangle1(out  DetectionROI,
                        paramsMQDefects.Detection_Left_UP_Y, paramsMQDefects.Detection_Left_UP_X,
                        paramsMQDefects.Detection_Right_Down_Y, paramsMQDefects.Detection_Right_Down_X);
                    paramsMQDefects.DetectionROI= DetectionROI;
                    paramsMQDefects.ShieldROI = ShieldROI;
                }
                else
                {
                    //对检测区域与屏蔽区域赋值
                    DetectionROI= paramsMQDefects.DetectionROI;
                    ShieldROI = paramsMQDefects.ShieldROI;
                }

                CoatingAlog.Run(paramsMQDefects.ho_Image, DetectionROI, ShieldROI, paramsMQDefects,
                    ClassDefect, inputs, out outputs);

                paramsMQDefects.detectInfos = (Queue<NgTypeInfo>)outputs[0];


                return true;
            }
            catch(Exception ex)
            {
                Log4netRecord.ErrorLog.Fatal(moduleInfo.ModuleName + ex.Message);
                return false;
            }
            finally
            {


            }
            
        }

        /// <summary>
        /// 分类器是否初始化
        /// </summary>
        bool isInitClassifier = false;

        public event EventHandler<PluginExecuteCompletedEventArgs> ExecuteCompleted;

        /// <summary>
        /// 初始化模型
        /// </summary>
        /// <param name="isInitModel">false 重新载入模型</param>
        public void InitModel(bool isInitModel)
        {
            try
            {
                ParamsMQDefects paramsMQDefects = (ParamsMQDefects)PluginParams;
                if (paramsMQDefects.cb_Enable_Classify)
                {
                    if (!isInitModel)
                    {
                        string path = Application.StartupPath + "\\AIModel\\";
                        path += "model.onnx";
                        ////极性选择 0表示阴极 / 正极 1表示阳极 / 负极
                        //if (paramsMQDefects.com_Cathode == 0)
                        //{
                        //    path += "正极\\model.onnx";
                        //}
                        //else
                        //{
                        //    path += "负极\\model.onnx";

                        //}
                        int stat = ClassDefect.init(path);
                        if (stat == 0) paramsMQDefects.InitClassifierfinish = true;
                        else paramsMQDefects.InitClassifierfinish = false;
                    }
                }
               
            }
            catch(Exception)
            {
                //System.Windows.Forms.MessageBox.Show(ex.Message);
            }
            finally
            {
                isInitClassifier = true;
            }
        }

        /// <summary>
        /// 初始化模型
        /// </summary>
        public void InitShapeModel()
        {
            try
            {
                ParamsMQDefects paramsMQDefects = (ParamsMQDefects)PluginParams;
                paramsMQDefects.Model = new HTuple();
                string directoryPath = Path.GetDirectoryName(Project.Instance.paramPathSet.ProcessPath);
                string ProcessPath = Path.Combine(directoryPath + "\\Model\\", moduleInfo.ProcessName);
                string modelFilePath = Path.Combine(ProcessPath, moduleInfo.ModuleName);
                modelFilePath += ".sbm";

                if (File.Exists(modelFilePath))
                {
                    HOperatorSet.ReadShapeModel(modelFilePath, out paramsMQDefects.Model);

                    HOperatorSet.SetShapeModelParam(paramsMQDefects.Model, "timeout", paramsMQDefects.num_TimeOut);
                    HOperatorSet.GetShapeModelContours(out paramsMQDefects.ModelContours, paramsMQDefects.Model, 1);
                }
                else
                {

                }         
            }
            catch(Exception)
            {

            }
           
        }




        /// <summary>
        /// 获取模块参数
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void GetResultParams(string varName, object obj)
        {
           
        }
        /// <summary>
        /// 保存参数
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool saveParams()
        {
            PluginParams.filePath = moduleInfo.filePath;
            if (PluginParams.Save())
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="param"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void SetParams(PluginParamsBase param)
        {
          this.PluginParams = param;
        }



        /// <summary>
        /// 构造函数
        /// </summary>
        public MQDefectsObj()
        {
            
        }

        public void IniObj()
        {
            //类器初始化
            ClassDefect = new ClassificationClient();
            //图像处理算法
            CoatingAlog = new CoatingDefectsAlog();
            InitModel(isInitClassifier);
            InitShapeModel();


        }

        public void DisPoseObj()
        {

        }
    }
}
