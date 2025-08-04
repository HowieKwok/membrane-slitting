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

namespace Frame_Core
{
    public partial class FormCraftBase : Form
    {
        /// <summary>
        /// 窗体语言翻译
        /// </summary>
        public LanguageEnum FormLanguage;

        public CraftBase craftBase { get; set; }

        public FormCraftBase()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 把工艺显示到工艺界面
        /// </summary>
        public virtual void CraftToView()
        {

        }
        /// <summary>
        /// 从工艺界面刷新工艺参数
        /// </summary>
        /// <returns></returns>
        public virtual CraftBase CraftFromView()
        {
            return craftBase;
        }
        /// <summary>
        /// 保存工艺到本地
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public virtual bool SaveCraft(string Path)
        {
            //craftBase = CraftFromView();
            //craftBase.fileFullPath = Path;
            //return craftBase.Save();
            return false;
        }
        /// <summary>
        /// 从本地读取工艺
        /// </summary>
        /// <param name="Path"></param>
        /// <returns></returns>
        public virtual CraftBase ReadCraft(string Path)
        {
            //craftBase.fileFullPath = Path;
            //craftBase = (CraftBase)craftBase.Read();
            //if (craftBase == null)
            //{
            //    craftBase = new CraftBase();
            //    craftBase.materialCrafEnum = (MaterialCrafEnum)0;
            //}
            //craftBase.fileFullPath = Path;
            //return craftBase;
            return craftBase;
        }

        public virtual void ReSetCraft() { }

        /// <summary>
        /// 获取当前界面的工艺配方。true：返回new工艺，false：返回已有工艺。
        /// </summary>
        /// <param name="IsNew"></param>
        /// <returns></returns>
        public virtual CraftBase GetCraft() { return craftBase; }


    }
}
