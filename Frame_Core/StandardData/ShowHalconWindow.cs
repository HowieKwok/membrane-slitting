using HalconDotNet;
using NPOI.XSSF.Streaming.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Frame_Core
{
    /// <summary>
    /// 界面显示类
    /// </summary>
    public class ShowHalconWindow : IDisposable
    {
        public HObject m_hImage = null;
        public string WindowName = "DefaultWindow";
        public Image3DClass Image3DClass = null;
        public long m_nProductID = 0;
        public long m_nImageFrameID = 0;
        public bool bIs3D = false;
        public bool m_bIsNG = false;//NG为true
        public bool m_bIsDispose = false;


        public string m_strCamName = "Default";
        /// <summary>
        /// 0：原图；1：产品图；2：3D图 3：NG小图
        /// </summary>
        public int m_nMode = 0;
        public List<MessageShowInfo> m_messageShowInfos { get; set; }
        public List<RegionShowInfo> m_regionShowInfos { get; set; }
        public List<LineInfo> m_lineShowInfos { get; set; }

        public ImageSavePathInfo m_imageSavePathInfo = new ImageSavePathInfo();

        public ShowHalconWindow()
        {
            m_messageShowInfos = new List<MessageShowInfo>();
            m_regionShowInfos = new List<RegionShowInfo>();
            m_lineShowInfos = new List<LineInfo>();
        }

        public ShowHalconWindow Copy()
        {
            ShowHalconWindow info = new ShowHalconWindow();
            this.m_hImage?.CopyImage(out info.m_hImage);
            info.m_nProductID = this.m_nProductID;
            info.m_nImageFrameID = this.m_nImageFrameID;
            info.m_strCamName = m_strCamName;
            info.m_nMode = m_nMode;
            info.bIs3D = this.bIs3D;
            info.m_bIsNG = this.m_bIsNG;
            info.m_bIsDispose = m_bIsDispose;
            info.Image3DClass = Image3DClass?.Copy();
            info.m_messageShowInfos = new List<MessageShowInfo>(m_messageShowInfos);
            info.m_lineShowInfos = new List<LineInfo>(m_lineShowInfos);
            info.m_regionShowInfos = new List<RegionShowInfo>();
            info.m_imageSavePathInfo = m_imageSavePathInfo;
            foreach (var item in m_regionShowInfos)
            {
                RegionShowInfo regionShowInfo = new RegionShowInfo();
                item.m_hRegion?.CopyImage(out regionShowInfo.m_hRegion);
                regionShowInfo.m_stuGetColor = item.m_stuGetColor;
            }
            return info;

        }

        public void Dispose()
        {
            m_hImage?.Dispose();
            Image3DClass?.Dispose();
            if (m_regionShowInfos != null)
            {
                for (int i = 0; i < m_regionShowInfos.Count; i++)
                {
                    m_regionShowInfos[i].m_hRegion?.Dispose();
                }
                m_regionShowInfos.Clear();
            }

            m_messageShowInfos?.Clear();
            m_lineShowInfos?.Clear();

        }
    }



}
