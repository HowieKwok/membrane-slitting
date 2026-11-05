using Basler.Pylon;
using DevMgmt.Cameras.Models;
using DevMgmt.Cameras.SDK;
using FrameCommunity;
using FrameCommunity.MessageNotification;

using FrameCommunity.错误代码;
using HalconDotNet;
using LogTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace DevMgmt.Cameras
{
    public class CameraManagerViewModel
    {
        public ICommand SaveCommand { get; }

        public CameraManagerViewModel(CamParam _Params)
        {
            Params = _Params;
            SaveCommand = new RelayCommand<object>(_ => SaveParamClick());
        }

        #region 参数类操作
    
        public CamParam Params;

        public void AddCam(CamTypeEnum camTypeEnum)
        {
            var existingNames = Params.l_CameraDevs
           .Where(c => !string.IsNullOrEmpty(c.CameraParameter?.CameraName))
           .Select(c => c.CameraParameter.CameraName)
           .ToList();

            var ids = Params.l_CameraDevs
        .Where(c => !string.IsNullOrEmpty(c.CameraParameter?.CameraName))
        .Select(c => c.CameraParameter.ID)
        .ToList();

            CameraDev cameraDev = new CameraDev() { CameraParameter = new CameraParameterBase() { DevType = (int)camTypeEnum } };

            cameraDev.CameraParameter.ID = MessageHelper.FindMinimumId(ids);
            cameraDev.CameraParameter.CameraName = MessageHelper.GenerateUniqueName(existingNames, camTypeEnum.ToString());

            Params.l_CameraDevs.Add(cameraDev);

            for (int i = 0; i < Params.l_CameraDevs.Count; i++)
            {
                Params.l_CameraDevs[i].CameraParameter.ID = i;
            }
        }

        public void RemoveCam()
        {
            if (Params.l_CameraDevs.Count > 0) Params.l_CameraDevs.RemoveAt(Params.l_CameraDevs.Count - 1);
           
        }

        public void RenameCam(int id, string newName)
        {
            if (Params.l_CameraDevs.Where(p => p.CameraParameter.ID == id).Count() > 0)
            {
                var existingNames = Params.l_CameraDevs
          .Where(c => !string.IsNullOrEmpty(c.CameraParameter?.CameraName))
          .Select(c => c.CameraParameter.CameraName)
          .ToList();

                if (MessageHelper.TryRename(existingNames, newName, out string resultName))
                {
                    CameraDev cameraDev = Params.l_CameraDevs.Find(p => p.CameraParameter.ID == id);
                    cameraDev.CameraParameter.CameraName = resultName;
                }
            }

            for (int i = 0; i < Params.l_CameraDevs.Count; i++)
            {
                Params.l_CameraDevs[i].CameraParameter.ID = i;
            }

        }


        /// <summary>
        /// 根据相机类型动态设置参数面板字段显示/隐藏
        /// 纯操作内置 ControlDisplay 字典，不再使用任何特性辅助类
        /// </summary>
        /// <param name="cameraParameter">相机参数实体</param>
        public void SetParamShowWindow(CameraParameterBase cameraParameter)
        {
            if (cameraParameter == null)
            {
                ErrorWorks.ErrorMessageShow("cameraParameter", ErrorWorks.GetErrorInfo((uint)ErrorCodeEnum.参数为空));
                return;
            }

            try
            {
                // 1. 全局重置：所有字段先统一隐藏，避免旧状态残留
                foreach (var cfg in cameraParameter.ControlDisplay.Values)
                {
                    cfg.IsShow = false;
                }

                CamTypeEnum camType = (CamTypeEnum)cameraParameter.DevType;

                // 2. 通用必显示字段（所有相机类型都展示）
                SetFieldIsShow(cameraParameter, nameof(CameraParameterBase.DevType), true);
                SetFieldIsShow(cameraParameter, nameof(CameraParameterBase.EmulationPath), true);
                SetFieldIsShow(cameraParameter, nameof(CameraParameterBase.CalibrationValue_X), true);
                SetFieldIsShow(cameraParameter, nameof(CameraParameterBase.CalibrationValue_Y), true);

                // 3. 面阵类相机专属字段
                var areaScanList = new List<CamTypeEnum>
        {
            CamTypeEnum.灰点面阵,
            CamTypeEnum.Basler黑白面阵,
            CamTypeEnum.Basler彩色面阵,
            CamTypeEnum.海康黑白面阵,
            CamTypeEnum.海康彩色面阵,
            CamTypeEnum.海康U口黑白相机,
            CamTypeEnum.海康U口彩色相机
        };

                if (areaScanList.Contains(camType))
                {
                    SetFieldIsShow(cameraParameter, nameof(CameraParameterBase.CameraSerial), true);
                    SetFieldIsShow(cameraParameter, nameof(CameraParameterBase.CameraIP), true);
                    SetFieldIsShow(cameraParameter, nameof(CameraParameterBase.PCIP), true);
                    SetFieldIsShow(cameraParameter, nameof(CameraParameterBase.ImageWidth), true);
                    SetFieldIsShow(cameraParameter, nameof(CameraParameterBase.ImageHeight), true);
                }

                // 4. 海康线扫/采集卡类相机专属字段
                var HKlineScanList = new List<CamTypeEnum>
        {
            CamTypeEnum.海康线扫,
         
        };

                if (HKlineScanList.Contains(camType))
                {
                    SetFieldIsShow(cameraParameter, nameof(CameraParameterBase.ConfigFiles), true);
                    SetFieldIsShow(cameraParameter, nameof(CameraParameterBase.CardSerial), true);
                    SetFieldIsShow(cameraParameter, nameof(CameraParameterBase.CameraSerial), true);
                    SetFieldIsShow(cameraParameter, nameof(CameraParameterBase.Port), true);
                    SetFieldIsShow(cameraParameter, nameof(CameraParameterBase.ImageWidth), true);
                    SetFieldIsShow(cameraParameter, nameof(CameraParameterBase.ImageHeight), true);
                }

                // 5. 线扫/采集卡类相机专属字段
                var lineScanList = new List<CamTypeEnum>
        {      
            CamTypeEnum.DALSA采集卡线扫,
            CamTypeEnum.埃科采集卡黑白线扫,
            CamTypeEnum.埃科采集卡彩色线扫,
            CamTypeEnum.埃科CXP黑白相机,
            CamTypeEnum.埃科CXP彩色相机
        };

                if (lineScanList.Contains(camType))
                {
                    SetFieldIsShow(cameraParameter, nameof(CameraParameterBase.ConfigFiles), true);
                    SetFieldIsShow(cameraParameter, nameof(CameraParameterBase.CardModel), true);
                    SetFieldIsShow(cameraParameter, nameof(CameraParameterBase.Port), true);
                    SetFieldIsShow(cameraParameter, nameof(CameraParameterBase.ImageWidth), true);
                    SetFieldIsShow(cameraParameter, nameof(CameraParameterBase.ImageHeight), true);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"配置参数面板异常：{ex.Message}");
            }
        }

        /// <summary>
        /// 内部辅助方法：设置单个字段显示状态
        /// </summary>
        /// <param name="param">参数实体</param>
        /// <param name="propName">属性名</param>
        /// <param name="isShow">是否显示</param>
        private void SetFieldIsShow(CameraParameterBase param, string propName, bool isShow)
        {
            if (param.ControlDisplay.TryGetValue(propName, out var displayConfig))
            {
                displayConfig.IsShow = isShow;
            }
        }

        public void SaveParamClick()
        {
            try
            {
                Params.Save();             
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("保存失败：" + ex.Message);
            }
        }

        #endregion

    
 
    }
}
