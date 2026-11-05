using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;

namespace DevMgmt.Cameras.Models
{
    public enum CamTypeEnum
    {
        灰点面阵=0,
        Basler黑白面阵=100,
        Basler彩色面阵 = 200,
        海康黑白面阵 =300,
        海康彩色面阵=400,           
        海康U口黑白相机 =500,
        海康U口彩色相机=600,
        海康线扫 = 700,
        DALSA采集卡线扫 =900,
        埃科采集卡黑白线扫=1000,
        埃科采集卡彩色线扫=1100,
        埃科CXP黑白相机=1200,
        埃科CXP彩色相机=1300,
    }

   
}
