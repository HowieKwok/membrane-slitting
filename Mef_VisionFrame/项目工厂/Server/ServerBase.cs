using Frame_Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mef_VisionFrame
{
    public abstract class ServerBase : AThreadManageBase
    {
        public ModelBase BaseData { get; }
        public ServerBase(ModelBase modelBase) { BaseData = modelBase; }

        public abstract event Action RefreshToolView;
        public abstract event Action RefreshView;
        public abstract event Action InitView;
        /// <summary>
        /// 仿真加载图像
        /// </summary>
        public abstract void SimLoadImage();

        public abstract List<StatusLabelItem>  GetStatusBarInfo();

        public abstract void RunModelSwitch();

        public abstract bool StartFilmRoll(string filmRollName);

        public abstract bool StopFilmRoll();

        public abstract void Reset();

        public abstract bool StartRun();

        public abstract bool StopRun();

        public abstract void DataSave();

        public abstract void DataProcess();

        public abstract void Communication();

        public abstract void ClearCutData(bool isSendPLC);

        public abstract void Udpate();


        /// <summary>
        /// 报警复位
        /// </summary>
        public abstract void AlarmReset();

        /// <summary>
        /// 发送停机报警
        /// </summary>
        public abstract void SendStopAlarm(int index);

        /// <summary>
        /// 发送停机报警
        /// </summary>
        public abstract void SendStopAlarm(string msg);

        /// <summary>
        /// 发送报警
        /// </summary>
        public abstract void SendAlarm(int index);

        /// <summary>
        /// 发送报警
        /// </summary>
        public abstract void SendAlarm(string msg);
    }
}
