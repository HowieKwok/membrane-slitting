using NLog.Config;
using NLog.Time;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AntdUI;
using Frame_Core;

namespace Mef_VisionFrame
{
    public partial class FormCraftBase : AntdUI.Window
    {
        public FormCraftBase()
        {
            InitializeComponent();
           
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="craftBase"></param>
        public virtual void Init(CraftBase craftBase)
        {

        }

        /// <summary>
        /// 把参数显示到工艺界面
        /// </summary>
        public virtual void ParamToView()
        {

        }

        /// <summary>
        /// 界面内容下发到参数
        /// </summary>
        public virtual void ViewToParam()
        {

        }
    }
}
