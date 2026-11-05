using DevMgmt.Cameras.Models;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core
{
    public static class Service_Camera
    {
        private const string DefaultCameraName = "DefaultCameraName";
        private const double DefaultCalibrationValue = 1.0;
        private const double DefaultImageSize = 100.0;

        // 使用属性缓存，避免多次访问
        private static List<CameraDev> CameraDevs
            => Project.Instance?.M_CamParam?.l_CameraDevs;

        private static Dictionary<int, Tuple<CameraDev, CameraRelevancy>> CameraRelevancies
       => Project.Instance?.M_CameraRelevancies;

        // 添加索引有效性检查，返回可空类型
        private static CameraDev GetCameraDev(int index)
        {
            var devs = CameraDevs;
            if (devs == null || index < 0 || index >= devs.Count)
                return null;
            return devs[index];
        }

        // 安全获取相机参数
        private static CameraParameterBase GetCameraParam(int index)
        {
            var dev = GetCameraDev(index);
            return dev?.CameraParameter;
        }

        // 1. 获取所有相机名称
        public static string[] GetCam_Names()
        {
            var devs = CameraDevs;
            if (devs == null || devs.Count == 0)
                return new string[0];

            return devs
                .Select(d => d?.CameraParameter?.CameraName ?? DefaultCameraName)
                .ToArray();

        }

        // 2. 获取单个相机名称
        public static string GetCam_Name(int index)
        {
            var param = GetCameraParam(index);
            return param?.CameraName ?? DefaultCameraName;
        }

        // 3. 获取X方向标定值
        public static double GetCam_CalibrationValue_X(int index)
        {
            var param = GetCameraParam(index);
            return param?.CalibrationValue_X ?? DefaultCalibrationValue;
        }

        // 4. 获取Y方向标定值
        public static double GetCam_CalibrationValue_Y(int index)
        {
            var param = GetCameraParam(index);
            return param?.CalibrationValue_Y ?? DefaultCalibrationValue;
        }

        // 5. 获取图像宽度
        public static double GetCam_ImageWidth(int index)
        {
            var param = GetCameraParam(index);
            return param?.ImageWidth ?? DefaultImageSize;
        }

        // 6. 获取图像高度
        public static double GetCam_ImageHeight(int index)
        {
            var param = GetCameraParam(index);
            return param?.ImageHeight ?? DefaultImageSize;
        }

        public static CameraDev Get_CameraDev(int index)
        {
            return GetCameraDev(index);
        }

        public static List<CameraDev> Get_CameraDevs()
        {
            return CameraDevs;
        }

        public static Dictionary<int, Tuple<CameraDev, CameraRelevancy>> Get_AllCameraRelevancies()
        {
            return CameraRelevancies ?? new Dictionary<int, Tuple<CameraDev, CameraRelevancy>>();
        }

        public static List<CameraRelevancy> Get_AllCameraRelevancyList()
        {
            var relevancies = CameraRelevancies;
            if (relevancies == null || relevancies.Count == 0)
                return new List<CameraRelevancy>();

            return relevancies.Values
                .Select(tuple => tuple?.Item2)
                .Where(item => item != null)
                .ToList();
        }

        public static CameraRelevancy Get_CameraRelevancy(int index)
        {
            var relevancies = Project.Instance?.M_CameraRelevancies;
            if (relevancies == null || index < 0 || index >= relevancies.Count)
                return null;
            return relevancies[index]?.Item2;
        }

    }
}
