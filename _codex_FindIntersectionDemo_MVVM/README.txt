FindIntersection WPF MVVM Demo

放置内容：
1. FormFindIntersectionDemo.cs
   WPF 模块列表 demo，参考 UC_ProcessSetting/PluginsInfo 的模块列表逻辑，从 PluginService.pluginsDictionary 读取插件信息。

2. FindIntersectionDemoDialog.cs
   继承 WPFFormPluginBase，使用 ShowDialog 模态打开。
   注意：这里按你的要求假设 Frame_Core 已经存在 WPFFormPluginBase。

3. FindIntersectionWpfView.xaml / FindIntersectionWpfView.xaml.cs
   科幻风 WPF UI，参数区三页，Halcon 显示区用 WindowsFormsHost 承载 HWindowControl。
   ViewROI 使用 HWndCtrl + ROIController + ROIRectangle2。

4. FindIntersectionDemoViewModel.cs
   MVVM 参数模型，界面参数双向绑定，ROI 拖动后回写 ViewModel。

5. Plugin.FindIntersection.csproj
   这是已挂载上述 demo 文件和 WPF 引用的项目文件副本，不会自动覆盖原项目。

合并注意：
- 如果你的 Frame_Core 里 WPFFormPluginBase 命名空间不是 Frame_Core，请改 FindIntersectionDemoDialog.cs 的 using。
- 如果要把 demo 放回 Plugin.FindIntersection，需要同步复制这 5 个文件，并把 csproj 中新增 Reference / Compile / Page 合进去。
- 原 MEF 入口 Plugin.FindIntersection.cs 没改，原业务逻辑不动。
