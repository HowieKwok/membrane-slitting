using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mef_VisionFrame
{
    /// <summary>
    /// 业务处理基类接口
    /// </summary>
    public interface IBizWorkerBase
    {
        /// <summary>
        /// 线程管理
        /// </summary>
        AThreadManageBase ThreadManage { get; set; }

        /// <summary>
        /// 刷新工艺
        /// </summary>
        void UpdateCraft();

        /// <summary>
        /// 初始化窗体
        /// </summary>
        void InitForm();

        /// <summary>
        /// 清空窗体
        /// </summary>
        void ClearForm();

        /// <summary>
        /// 更新窗体
        /// </summary>
        void UpdateForm();

        /// <summary>
        /// 初始化变量
        /// </summary>
        void InitVariable();

        /// <summary>
        /// 图像显示保存
        /// </summary>
        void ImageShowSave();

        /// <summary>
        /// 释放图像内存并清空图像队列
        /// </summary>
        void DisposeImageAndClearImageQueue();

        /// <summary>
        /// 仿真加载图像
        /// </summary>
        void SimLoadImage(bool simAutoFlag, ref bool simRunFlag);

        /// <summary>
        /// 开始膜卷
        /// </summary>
        void StartMojuan(string strMojuan);


        /// <summary>
        /// 停止膜卷
        /// </summary>
        void StopMojuan();


        /// <summary>
        /// 开始运行
        /// </summary>
        void StartRun();


        /// <summary>
        /// 停止运行
        /// </summary>
        void StopRun();


        /// <summary>
        /// 报警复位
        /// </summary>
        void AlarmReset();

        /// <summary>
        /// 发送停机报警
        /// </summary>
        void SendStopAlarm(int index);

        /// <summary>
        /// 发送报警
        /// </summary>
        void SendAlarm(int index);

    }
}
