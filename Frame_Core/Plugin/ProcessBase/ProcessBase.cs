using LogTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frame_Core.Plugin.ProcessBase
{
    public delegate void InitDelePlugins(Process process);
    public delegate void RunDelePlugins(Process process);
    public class ProcessBase
    {
        public CameraEnum cameraName;
        public   string m_strProcessName = string.Empty;//每个流程名称不能重复
        public event InitDelePlugins m_eventInitPlugins;
        public event RunDelePlugins m_eventRunPlugins;

        /// <summary>
        /// 图片保存参数
        /// </summary>
        public ImageSaveParamer m_ImageSaveParamer = new ImageSaveParamer();

         
        public bool InitImageSaveParamer(string path)
        {
            try
            {
                if (m_ImageSaveParamer == null) m_ImageSaveParamer = new ImageSaveParamer();
                m_ImageSaveParamer.filePath = path;
                m_ImageSaveParamer = (ImageSaveParamer)m_ImageSaveParamer.Read();
                m_ImageSaveParamer.filePath = path;
            }
            catch (Exception ex)
            {
                Log4netRecord.ErrorLog.Debug(ex);
                return false;
            }
            return true;
        }

        public void Init(Process process)
        {
            m_eventInitPlugins(process);
        }

        public void Run(Process process)
        {
            m_eventRunPlugins(process);
        }
    }
}
