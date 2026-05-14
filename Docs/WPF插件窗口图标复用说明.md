# WPF插件窗口图标复用说明

## 1. 图标为什么要在宿主窗口设置

当前 WPF 插件页面不是独立 Window，而是作为 `UserControl` 被宿主窗口承载：

```csharp
WPFFormPluginBase window = new WPFFormPluginBase();
object content = Activator.CreateInstance(wpfContentType);
window.pluginsObjBase = plugin;
window.PluginContent = content;
window.Init();
window.ShowDialog();
```

因此标题栏左上角图标属于 `WPFFormPluginBase`，不属于插件页面本身。

也就是说：

- `WPFBlobAnalysis.xaml` 里设置图标没有效果。
- `UserControl` 没有窗口标题栏，也没有 `Icon` 属性。
- 要改左上角标题栏图标，必须改 `Frame_Core.Plugin.PluginBase.WPF.WPFFormPluginBase`。

对应文件：

```text
Frame_Core\Plugin\PluginBase\WPF\WPFFormPluginBase.xaml
Frame_Core\Plugin\PluginBase\WPF\WPFFormPluginBase.xaml.cs
```

## 2. 推荐图标资源存放位置

建议把所有 WPF 插件弹窗通用资源统一放到 `Frame_Core` 下，便于后续插件复用：

```text
Frame_Core
└─ WPF
   └─ Resources
      └─ Icons
         ├─ hsvision_plugin.ico
         ├─ blob_analysis.ico
         ├─ find_model.ico
         └─ calibration.ico
```

如果暂时不想新建 `WPF` 目录，也可以放到：

```text
Frame_Core\Resources\Icons
```

图标文件建议使用 `.ico`，并包含常用尺寸：

```text
16x16
24x24
32x32
48x48
256x256
```

标题栏主要使用小尺寸图标，`.png` 在 WPF 中也能显示，但 `.ico` 对标题栏、任务栏、不同 DPI 更稳定。

## 3. Frame_Core.csproj 中加入资源

把图标作为 WPF Resource 编译进 `Frame_Core.dll`。

示例：

```xml
<ItemGroup>
  <Resource Include="WPF\Resources\Icons\hsvision_plugin.ico" />
  <Resource Include="WPF\Resources\Icons\blob_analysis.ico" />
</ItemGroup>
```

如果使用 `Frame_Core\Resources\Icons`，则写成：

```xml
<ItemGroup>
  <Resource Include="Resources\Icons\hsvision_plugin.ico" />
  <Resource Include="Resources\Icons\blob_analysis.ico" />
</ItemGroup>
```

注意：

- `Build Action` 必须是 `Resource`。
- 不建议用 `Content + Copy always`，因为插件 DLL 加载时路径容易变化。
- 资源放在 `Frame_Core` 中，所有插件都能通过 pack URI 引用。

## 4. 最简单的统一图标写法

如果所有 WPF 插件窗口都使用同一个 HSVISION 默认图标，可以直接在 `WPFFormPluginBase.xaml` 的 `Window` 节点加 `Icon`。

示例：

```xml
<Window x:Class="Frame_Core.WPFFormPluginBase"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        x:Name="Root"
        Title="PluginFormBase"
        Icon="/Frame_Core;component/WPF/Resources/Icons/hsvision_plugin.ico"
        Width="1008"
        Height="811">
```

这种方式最简单，适合所有插件统一使用一个图标。

缺点是：

- 每个插件不能单独配置自己的图标。
- 后续如果 Blob、模板匹配、标定插件想用不同图标，就需要改成代码动态加载。

## 5. 推荐的可复用写法：宿主窗口统一加载图标

更推荐在 `WPFFormPluginBase.xaml.cs` 中封装一个统一方法：

```csharp
private void ApplyWindowIcon()
{
    try
    {
        Icon = new BitmapImage(new Uri(
            "pack://application:,,,/Frame_Core;component/WPF/Resources/Icons/hsvision_plugin.ico",
            UriKind.Absolute));
    }
    catch
    {
        // 图标加载失败不能影响插件窗口打开。
        // 这里不抛异常，最多保留系统默认图标。
    }
}
```

需要添加命名空间：

```csharp
using System.Windows.Media.Imaging;
```

然后在 `Init()` 中调用：

```csharp
public virtual void Init()
{
    if (pluginsObjBase == null || pluginsObjBase.moduleInfo == null) return;

    Title = pluginsObjBase.moduleInfo.ModuleName;
    StatusText = "模块：" + pluginsObjBase.moduleInfo.ModuleName;

    ApplyWindowIcon();

    CommandManager.InvalidateRequerySuggested();
}
```

这样后续所有通过 `WPFFormPluginBase` 打开的 WPF 插件，都能自动使用 HSVISION 默认图标。

## 6. 支持每个插件自定义图标

如果不同插件需要显示不同图标，建议在宿主窗口中按模块名做映射。

示例：

```csharp
private void ApplyWindowIcon()
{
    string iconPath = GetPluginIconPath();

    try
    {
        Icon = new BitmapImage(new Uri(iconPath, UriKind.Absolute));
    }
    catch
    {
        // 单插件图标加载失败时，兜底使用 HSVISION 默认图标。
        try
        {
            Icon = new BitmapImage(new Uri(
                "pack://application:,,,/Frame_Core;component/WPF/Resources/Icons/hsvision_plugin.ico",
                UriKind.Absolute));
        }
        catch
        {
        }
    }
}

private string GetPluginIconPath()
{
    string moduleName = pluginsObjBase != null && pluginsObjBase.moduleInfo != null
        ? pluginsObjBase.moduleInfo.ModuleName
        : string.Empty;

    if (moduleName.Contains("Blob"))
    {
        return "pack://application:,,,/Frame_Core;component/WPF/Resources/Icons/blob_analysis.ico";
    }

    if (moduleName.Contains("模板") || moduleName.Contains("FindModel"))
    {
        return "pack://application:,,,/Frame_Core;component/WPF/Resources/Icons/find_model.ico";
    }

    if (moduleName.Contains("标定") || moduleName.Contains("Calibration"))
    {
        return "pack://application:,,,/Frame_Core;component/WPF/Resources/Icons/calibration.ico";
    }

    return "pack://application:,,,/Frame_Core;component/WPF/Resources/Icons/hsvision_plugin.ico";
}
```

这种方式不需要每个插件页面继承宿主窗口，也不需要改每个插件的 `UserControl`。

## 7. 更专业的插件接口扩展方式

如果后续插件越来越多，不建议一直在 `WPFFormPluginBase` 里写 `moduleName.Contains(...)`。

更好的做法是在 WPF 插件视图接口中扩展一个可选图标属性。

示例接口：

```csharp
public interface IPluginWpfView
{
    IPlugin PluginObject { get; }
    PluginParamsBase PluginParams { get; }

    string WindowIconResource { get; }

    void BeforeRun();
    void AfterRun(bool success, long elapsedMs);
    void BeforeSave();
    void AfterSave(bool success);
    void BeforeClose();
}
```

插件页面可以这样返回自己的图标：

```csharp
public string WindowIconResource
{
    get
    {
        return "pack://application:,,,/Frame_Core;component/WPF/Resources/Icons/blob_analysis.ico";
    }
}
```

宿主窗口读取：

```csharp
private void ApplyWindowIcon()
{
    string iconPath = null;

    IPluginWpfView view = PluginView;
    if (view != null)
    {
        iconPath = view.WindowIconResource;
    }

    if (string.IsNullOrWhiteSpace(iconPath))
    {
        iconPath = "pack://application:,,,/Frame_Core;component/WPF/Resources/Icons/hsvision_plugin.ico";
    }

    try
    {
        Icon = new BitmapImage(new Uri(iconPath, UriKind.Absolute));
    }
    catch
    {
    }
}
```

注意：

- 如果直接改接口，会影响所有已实现 `IPluginWpfView` 的插件。
- 为了不破坏旧插件，可以新建一个可选接口。

推荐可选接口：

```csharp
public interface IPluginWpfWindowOption
{
    string WindowIconResource { get; }
}
```

插件需要自定义图标时再实现：

```csharp
public partial class WPFBlobAnalysis : UserControl, IPluginWpfView, IPluginWpfWindowOption
{
    public string WindowIconResource
    {
        get
        {
            return "pack://application:,,,/Frame_Core;component/WPF/Resources/Icons/blob_analysis.ico";
        }
    }
}
```

宿主窗口读取：

```csharp
private void ApplyWindowIcon()
{
    string iconPath = null;

    IPluginWpfWindowOption option = PluginContent as IPluginWpfWindowOption;
    if (option != null)
    {
        iconPath = option.WindowIconResource;
    }

    if (string.IsNullOrWhiteSpace(iconPath))
    {
        iconPath = "pack://application:,,,/Frame_Core;component/WPF/Resources/Icons/hsvision_plugin.ico";
    }

    try
    {
        Icon = new BitmapImage(new Uri(iconPath, UriKind.Absolute));
    }
    catch
    {
    }
}
```

这种方式最适合后续封装插件 DLL：

- 旧插件不需要改。
- 新插件想改标题栏图标，只实现 `IPluginWpfWindowOption`。
- 图标加载逻辑仍然统一在 `WPFFormPluginBase`，不会分散到各插件页面。

## 8. 后续新插件复用步骤

新建 WPF 插件时按下面步骤做：

1. 插件页面继续写成 `UserControl`。

```csharp
public partial class WPFNewTool : UserControl, IPluginWpfView
{
}
```

2. 不要在插件页面里创建 Window。

插件页面只负责界面和参数绑定，弹窗由 `WPFFormPluginBase` 承载。

3. 如果使用 HSVISION 默认图标，不需要额外写代码。

4. 如果需要自定义图标，实现可选接口。

```csharp
public partial class WPFNewTool : UserControl, IPluginWpfView, IPluginWpfWindowOption
{
    public string WindowIconResource
    {
        get
        {
            return "pack://application:,,,/Frame_Core;component/WPF/Resources/Icons/new_tool.ico";
        }
    }
}
```

5. 把图标资源放到 `Frame_Core` 的公共资源目录，并在 `Frame_Core.csproj` 设置为 `Resource`。

## 9. 常见问题

### 9.1 为什么改 UserControl 没效果

因为 `UserControl` 是内容，不是窗口。

标题栏图标属于 `Window.Icon`，当前窗口是 `WPFFormPluginBase`。

### 9.2 为什么不要用绝对路径

不推荐：

```csharp
Icon = new BitmapImage(new Uri(@"E:\xxx\blob.ico"));
```

原因：

- 换电脑路径会失效。
- 插件 DLL 拷贝到 `Plugins` 目录后，资源路径不好维护。
- 工程发布后容易漏文件。

推荐：

```csharp
pack://application:,,,/Frame_Core;component/WPF/Resources/Icons/blob_analysis.ico
```

### 9.3 为什么图标加载失败不能抛异常

图标只是界面装饰，不应该影响插件窗口打开。

所以 `ApplyWindowIcon()` 必须 `try/catch`，加载失败就保留默认系统图标。

### 9.4 为什么建议放在 Frame_Core

`Frame_Core` 是插件公共框架层，WPF 宿主窗口也在这里。

图标资源放在这里有几个好处：

- 所有插件可复用。
- 宿主窗口直接加载，不依赖某个插件 DLL 是否存在。
- 后续新增插件不用重复写资源加载逻辑。

## 10. 建议最终方案

建议采用：

```text
WPFFormPluginBase 统一设置 HSVISION 默认图标
        +
IPluginWpfWindowOption 可选接口支持单插件自定义图标
        +
图标资源统一放 Frame_Core\WPF\Resources\Icons
```

这样既不破坏已有插件，又方便后续 DLL 插件复用。

