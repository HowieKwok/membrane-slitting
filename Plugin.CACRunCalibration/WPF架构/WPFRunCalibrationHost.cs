using Frame_Core;
using System;

namespace Plugin.CACRunCalibration.WPF架构
{
    public class WPFRunCalibrationHost : WPFFormPluginBase
    {
        private readonly WPFRunCalibration view;

        public WPFRunCalibrationHost()
        {
            view = new WPFRunCalibration();
            PluginContent = view;
            Title = "CAC运行标定";
        }

        public override void Init()
        {
            base.Init();
            view.BindPlugin(pluginsObjBase);
        }

        public override void Btn_RUN_Click(object sender, EventArgs e)
        {
            view.Btn_RUN_Click(sender, e);
            base.Btn_RUN_Click(sender, e);
            view.AfterHostRun(sender, e);
        }

        public override void Btn_Save_Click(object sender, EventArgs e)
        {
            view.Btn_Save_Click(sender, e);
            base.Btn_Save_Click(sender, e);
            view.AfterHostSave(sender, e);
        }

        public override void Btn_Close_Click(object sender, EventArgs e)
        {
            view.Btn_Close_Click(sender, e);
            base.Btn_Close_Click(sender, e);
        }
    }
}
