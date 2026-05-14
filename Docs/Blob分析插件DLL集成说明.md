# Blob分析插件 DLL 集成说明

## 1. 本次落地范围

本次新增真实插件工程 `Plugin.BlobAnalysis`，目标是向现有框架靠齐，而不是继续停留在独立 Demo：

- `Frame_Core\pluginParams\ParamsBlobAnalysis.cs`：流程可序列化参数类，只放输入、输出、ROI、过滤条件等公共参数。
- `Frame_Core\WPF\IPluginWpfView.cs`：WPF 插件页面通用扩展接口，供 `WPFFormPluginBase` 调用。
- `Plugin.BlobAnalysis`：Blob 专用算法、WPF 页面、ViewModel、ViewROI 适配、MEF 插件入口。
- `Mef_VisionFrame\bin\Debug\Plugins\Plugin.BlobAnalysis.dll`：已编译输出的插件 DLL。

Blob 算法没有放进 `Frame_Core`，避免框架层耦合具体视觉算法。

## 2. 类放置原则

### Frame_Core

只放框架公共内容：

- `Frame_Core\pluginParams\ParamsBlobAnalysis.cs`
  - `ParamsBlobAnalysis : PluginParamsBase`
  - `BlobMaskRoiParam`
  - `BlobFilterRuleParam`
  - `BlobRegionResultParam`
- `Frame_Core\WPF\IPluginWpfView.cs`
  - WPF 插件页面生命周期接口。

以后其它 DLL 也可以复用 `IPluginWpfView`，不需要每个插件都写一套 Host。

### Plugin.BlobAnalysis

只放 Blob 专用内容：

- `BlobAnalysisObj.cs`：流程实际调用入口，实现 `IPlugin`，MEF 导出。
- `Core\BlobAnalysisEngine.cs`：HALCON Blob 分析核心算法。
- `Core\BlobAnalysisFollowTransformer.cs`：局部 ROI、屏蔽 ROI 的位置跟随变换。
- `Core\BlobParamsMapper.cs`：`ParamsBlobAnalysis` 和内部 WPF/算法模型之间互转。
- `Models\*.cs`：DLL 内部强类型模型，不暴露给主框架强依赖。
- `ViewModels\*.cs`：WPF 绑定层。
- `Services\HalconViewRoiAdapter.cs`：只调用 ViewROI，不修改 ViewROI。
- `WPF架构\WPFBlobAnalysis.xaml(.cs)`：插件配置页面。

## 3. 流程调用方式

流程运行时调用的是：

```csharp
BlobAnalysisObj.ExecuteObj();
```

执行链路：

1. 从 `PluginParams` 转成 `ParamsBlobAnalysis`。
2. `BlobParamsMapper.ToRuntime()` 转成算法层 `BlobAnalysisParams`。
3. `BlobAnalysisEngine.Run(p.ho_Image, runtime)` 执行 HALCON：灰度化、局部 ROI reduce、阈值、屏蔽 ROI difference、形态学、connection、select_shape、多条件过滤、排序、特征输出。
4. `BlobParamsMapper.ApplyResult()` 写回：
   - `m_bRunSuccess`
   - `m_bIsFind`
   - `m_nRegionCount`
   - `m_fElapsedMs`
   - `m_strRunMessage`
   - `m_ho_OutSelectedRegion`
   - `m_lOutRegionResults`

## 4. WPFFormPluginBase 转接规则

`WPFFormPluginBase` 是通用宿主窗口，插件页面不继承它，也不把具体插件逻辑写进基类。

当前架构分三层：

- 主程序或调试窗口负责创建插件 `UserControl`，调用 `BindPlugin(plugin)`，再把页面放进 `WPFFormPluginBase.PluginContent`。
- `WPFFormPluginBase` 只负责底部按钮、耗时显示、状态显示和窗口关闭转发。
- 插件页面自己负责提交界面参数、调用 DLL 执行逻辑、刷新显示、释放界面局部资源。

插件页面实现：

```csharp
public partial class WPFBlobAnalysis : UserControl, IPluginWpfView
```

主程序打开窗口时的关键链路：

```csharp
object content = Activator.CreateInstance(wpfContentType);
TryBindPlugin(content, plugin);

WPFFormPluginBase window = new WPFFormPluginBase();
window.pluginsObjBase = plugin;
window.PluginContent = content;
window.Init();
window.ShowDialog();
```

`BindPlugin()` 中必须把插件对象和框架参数类绑定到界面：

```csharp
public void BindPlugin(IPlugin plugin)
{
    PluginObject = plugin as BlobAnalysisObj;
    PluginParams = PluginObject == null ? null : PluginObject.PluginParams;

    ParamsBlobAnalysis p = PluginParams as ParamsBlobAnalysis;
    if (p == null) return;

    vm.BindFrameParams(p);
    ShowFrameImage(true);
}
```

这里的图像并不是复制出来的新对象。`vm.BindFrameParams(p)` 保存的是同一个 `ParamsBlobAnalysis` 引用，`vm.Image` 每次都从 `frameParams.ho_Image` 读取。因此窗口关闭后重新打开，只要流程参数类里仍有 `ho_Image`，新界面就能通过 `ShowFrameImage(true)` 把同一个输入图像重新显示出来。

宿主点击运行时：

1. `WPFFormPluginBase.Btn_RUN_Click()` 调用 `PluginView.RunFromHost()`。
2. 插件页面内部调用 `BeforeRun()`，提交 WPF 绑定和当前 ViewROI 到 `ParamsBlobAnalysis`。
3. 插件页面调用 `PluginObject.ExecuteObj()` 执行 DLL 逻辑。
4. 插件页面调用 `AfterRun()`，从 `ParamsBlobAnalysis` 输出字段刷新界面。
5. 插件页面返回 `PluginViewActionResult`，宿主只用它刷新耗时和状态栏。

宿主点击保存时：

1. `WPFFormPluginBase.Btn_Save_Click()` 计算框架参数文件路径。
2. 宿主调用 `PluginView.SaveFromHost(filePath)`。
3. 插件页面内部调用 `BeforeSave()`，只把界面当前值回写到参数类。
4. 插件页面调用 `PluginObject.saveParams()` 或 `PluginParams.Save()` 写 XML。
5. 插件页面返回 `PluginViewActionResult`，宿主刷新保存状态。

宿主关闭时：

- `WPFFormPluginBase.OnClosing()` 调用 `PluginView.CloseFromHost()`。
- `CloseFromHost()` 只提交界面当前值并释放界面局部资源，不自动写 XML。
- 是否保存参数文件只由显式“保存”按钮决定。

这样以后新增 DLL 不需要单独写 `WPFBlobAnalysisHost`，只需要 `UserControl + IPluginWpfView`。宿主是转接器，不参与具体算法和具体参数字段管理。
## 5. Blob 输出字段

每个 Blob 输出到 `BlobRegionResultParam`：

| 字段 | HALCON 来源 | 说明 |
| --- | --- | --- |
| `m_nIndex` | SelectObj 顺序 | 第几个 Blob |
| `m_fArea` | `AreaCenter` | 面积 |
| `m_fCenterRow` | `AreaCenter` | 质心 Row |
| `m_fCenterCol` | `AreaCenter` | 质心 Col |
| `m_fRect2Row` | `SmallestRectangle2` | 最小外接矩形中心 Row |
| `m_fRect2Col` | `SmallestRectangle2` | 最小外接矩形中心 Col |
| `m_fRa` / `m_fRb` | `EllipticAxis` | 等效椭圆长短轴 |
| `m_fLength1` / `m_fLength2` | `SmallestRectangle2` | 外接矩形半长边、半短边 |
| `m_fAspectRatio` | 长边 / 短边 | 长宽比 |
| `m_fPhi` / `m_fAngleDeg` | `EllipticAxis` | 弧度和角度 |
| `m_fCircularity` | `Circularity` | 圆度 |
| `m_fRectangularity` | `Rectangularity` | 矩形度 |
| `m_fMeanGray` / `m_fDeviationGray` | `Intensity` | 平均灰度、灰度偏差 |
| `m_fMinGray` / `m_fMaxGray` | `GrayFeatures` | 最小/最大灰度 |

## 6. 检测范围与屏蔽 ROI

- 全局检测：`m_nDetectMode = 0`。
- 局部检测：`m_nDetectMode = 1` 且 `m_bUseLocalRoi = true`。
- 屏蔽 ROI：`m_bUseMaskRoi = true` 时，遍历 `m_lMaskRoiParams`，对启用项生成 Region 后从阈值区域中扣除。
- 屏蔽 ROI 支持多个，读取 XML 前后必须清空再重建 List，避免重复累加。

## 7. 编译验证

已执行：

```powershell
MSBuild .\Plugin.BlobAnalysis\Plugin.BlobAnalysis.csproj /p:Configuration=Debug /p:Platform=AnyCPU
```

结果：编译通过。

输出 DLL：

```text
Mef_VisionFrame\bin\Debug\Plugins\Plugin.BlobAnalysis.dll
```
## 8. HObject 生命周期与内存释放边界

Blob 插件必须严格区分“流程数据对象”和“界面显示对象”。`ParamsBlobAnalysis` 是框架流程输入输出数据类，不是界面资源容器，不能在界面层随意释放它里面的对象。

### 8.1 输入图像

- `ParamsBlobAnalysis.ho_Image` 是插件输入图像，必须作为算法运行和界面显示的唯一图像来源。
- `BlobAnalysisObj.ExecuteObj()` 调用 `BlobAnalysisEngine.Run(p.ho_Image, runtime)`，不要使用界面层另存的图像副本。
- `BlobAnalysisViewModel.Image` 也只从 `frameParams.ho_Image` 返回，不维护第二份输入图像。
- 插件运行入口、ViewModel、WPF 页面、显示适配器都不释放 `ho_Image`。输入图像生命周期由流程、上游模块或主框架统一管理。

### 8.2 输出对象

`ParamsBlobAnalysis` 中的输出 HObject 是插件对外输出结果：

- `m_ho_OutRegion`：阈值或中间区域输出。
- `m_ho_OutSelectedRegion`：最终通过过滤和排序后的 Blob 区域。
- `m_ho_OutLocalRoiRegion`：局部检测 ROI 区域。
- `m_ho_OutMaskRoiRegion`：屏蔽 ROI 合并区域。

输出对象必须由插件运行入口统一释放：

```csharp
BlobParamsMapper.ReleaseOutputHObjects(p);
```

执行前释放上一轮输出，是为了避免本轮失败时外部仍读到旧 HObject，也避免重复运行造成对象累积。`BlobParamsMapper.ApplyResult()` 写入新输出前也会再次释放输出字段，保证写回动作本身是干净的。

### 8.3 算法临时对象

`BlobAnalysisEngine.Run()` 内部创建的 `gray`、`domain`、`reduced`、`thresholdRegion`、`maskedRegion`、`morphRegion`、`connected`、`selected`、`sorted` 等对象，必须在 `finally` 中释放。算法结果需要外传时，使用 `CopyObj` 写入 `BlobAnalysisResult`，再由调用者决定何时释放结果对象。

`BlobAnalysisObj.ExecuteObj()` 中，`BlobAnalysisResult` 是算法层临时结果。即使 `ApplyResult()` 写回输出时抛异常，也必须释放 result 自己持有的 HObject，所以用 `try/finally` 包住：

```csharp
BlobAnalysisResult result = null;
try
{
    result = engine.Run(p.ho_Image, runtime);
    BlobParamsMapper.ApplyResult(result, p);
}
finally
{
    result?.Dispose();
}
```

这里不会改变 `ParamsBlobAnalysis` 的输出逻辑。`ApplyResult()` 已经把需要对外输出的对象 `CopyObj` 到 `m_ho_Out*` 字段，`result.Dispose()` 释放的是算法临时结果，不释放参数类输出。

### 8.4 界面局部对象

WPF 页面只释放自己创建、自己持有的界面对象：

- `selectedRegion`：界面为了高亮当前选中 Blob 额外 `SelectObj` 出来的预览对象。
- `BlobAnalysisViewModel.lastResult`：界面本地运行或预览产生的临时结果。
- `HalconViewRoiAdapter.displayImage`：为了让 ViewROI 正确计算图像宽高而创建的显示 `HImage`。
- WPF 事件引用和显示适配器引用。

WPF 页面不释放：

- `ParamsBlobAnalysis.ho_Image`。
- `ParamsBlobAnalysis.m_ho_OutRegion`。
- `ParamsBlobAnalysis.m_ho_OutSelectedRegion`。
- `ParamsBlobAnalysis.m_ho_OutLocalRoiRegion`。
- `ParamsBlobAnalysis.m_ho_OutMaskRoiRegion`。

这些输出对象由插件运行入口和插件对象销毁逻辑释放。

## 9. 从零开发一个新的 WPF 视觉插件

后续扩展其它 DLL 插件时，建议按下面步骤开发。核心原则是：框架只放公共协议，插件只放业务算法和界面，不把具体算法塞进 `Frame_Core`。

### 9.1 新增参数类

在 `Frame_Core\pluginParams` 新增一个参数类，例如：

```csharp
[Serializable]
public class ParamsNewVisionTool : PluginParamsBase
{
    [NonSerialized]
    [XmlIgnore]
    public HObject ho_Image;

    public bool m_bIsEnable = true;
    public double m_fThresholdMin = 0;
    public double m_fThresholdMax = 255;

    [NonSerialized]
    [XmlIgnore]
    public HObject m_ho_OutRegion;

    [NonSerialized]
    [XmlIgnore]
    public bool m_bRunSuccess;

    [NonSerialized]
    [XmlIgnore]
    public string m_strRunMessage = string.Empty;
}
```

参数类只做数据承载：

- 可保存的参数使用普通字段或属性。
- 运行时 HObject、HTuple、结果列表使用 `[NonSerialized]` 和 `[XmlIgnore]`。
- 输入图像字段统一命名为 `ho_Image`，便于框架和其它插件理解。
- 输出 HObject 字段由插件自己负责释放，不交给界面释放。

### 9.2 新建插件工程

新增 `Plugin.NewVisionTool` 类库工程，目标框架与主程序一致。工程引用：

- `Frame_Core`
- `LogTool`
- `HalconDotNet`
- `PresentationCore`
- `PresentationFramework`
- `WindowsBase`
- `WindowsFormsIntegration`
- `System.Windows.Forms`

输出目录建议沿用插件目录：

```xml
<OutputPath>..\Mef_VisionFrame\bin\Debug\Plugins\</OutputPath>
```

### 9.3 插件入口类

插件入口实现 `IPlugin`，并使用 MEF 导出：

```csharp
[Category("新视觉工具")]
[DisplayName("新视觉工具")]
[Export("NewVisionToolObj", typeof(IPlugin))]
[PartCreationPolicy(CreationPolicy.NonShared)]
[Serializable]
public class NewVisionToolObj : IPlugin
{
    private readonly INewVisionToolEngine engine = new NewVisionToolEngine();

    public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();
    public PluginParamsBase PluginParams { get; set; } = new ParamsNewVisionTool();

    public bool ExecuteObj()
    {
        ParamsNewVisionTool p = PluginParams as ParamsNewVisionTool;
        if (p == null) return false;

        try
        {
            ReleaseOutputHObjects(p);

            if (!p.m_bIsEnable)
            {
                p.m_bRunSuccess = true;
                p.m_strRunMessage = "插件未启用，跳过执行";
                return true;
            }

            NewVisionToolResult result = null;
            try
            {
                result = engine.Run(p.ho_Image, ToRuntime(p));
                ApplyResult(result, p);
            }
            finally
            {
                result?.Dispose();
            }

            return p.m_bRunSuccess;
        }
        catch (Exception ex)
        {
            p.m_bRunSuccess = false;
            p.m_strRunMessage = ex.Message;
            ReleaseOutputHObjects(p);
            return false;
        }
    }
}
```

入口类负责流程语义：释放上一轮输出、检查启用状态、调用算法、写回输出、异常时清输出。不在入口类里写复杂 HALCON 算法，算法放到 `Core`。

### 9.4 算法层

算法层只接收输入图像和运行参数，返回结果对象。内部临时 HObject 必须在 `finally` 中释放：

```csharp
public sealed class NewVisionToolEngine : INewVisionToolEngine
{
    public NewVisionToolResult Run(HObject image, NewVisionToolParams p)
    {
        NewVisionToolResult result = new NewVisionToolResult();
        HObject gray = null;
        HObject region = null;

        try
        {
            if (image == null || !image.IsInitialized())
            {
                result.Success = false;
                result.Message = "输入图像为空";
                return result;
            }

            HOperatorSet.CountChannels(image, out HTuple channels);
            if (channels.I > 1) HOperatorSet.Rgb1ToGray(image, out gray);
            else gray = image.CopyObj(1, -1);

            HOperatorSet.Threshold(gray, out region, p.ThresholdMin, p.ThresholdMax);
            result.OutRegion = region.CopyObj(1, -1);
            result.Success = true;
            result.Message = "OK";
            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
            return result;
        }
        finally
        {
            gray?.Dispose();
            region?.Dispose();
        }
    }
}
```

算法层不释放外部传入的 `image`，因为输入图像不是算法层创建的。

### 9.5 Mapper 层

建议每个插件都有一个 Mapper，负责三件事：

- `ToRuntime()`：把框架参数类转成算法内部参数。
- `ApplyResult()`：把算法结果写回框架参数类。
- `ReleaseOutputHObjects()`：释放本插件写入参数类的输出 HObject。

这样入口、算法、界面不会互相穿透，后续维护比较稳。

### 9.6 WPF 页面

插件页面实现 `IPluginWpfView`。页面负责界面状态和调用链，宿主只负责转接按钮：

```csharp
public partial class WPFNewVisionTool : UserControl, IPluginWpfView
{
    private readonly NewVisionToolViewModel vm = new NewVisionToolViewModel();
    private bool viewDisposed;

    public NewVisionToolObj PluginObject { get; private set; }
    public PluginParamsBase PluginParams { get; private set; }

    IPlugin IPluginWpfView.PluginObject { get { return PluginObject; } }
    PluginParamsBase IPluginWpfView.PluginParams { get { return PluginParams; } }

    public void BindPlugin(IPlugin plugin)
    {
        PluginObject = plugin as NewVisionToolObj;
        PluginParams = PluginObject == null ? null : PluginObject.PluginParams;
        vm.BindFrameParams(PluginParams as ParamsNewVisionTool);
        ShowFrameImage(true);
    }

    public PluginViewActionResult RunFromHost()
    {
        Stopwatch watch = Stopwatch.StartNew();
        bool success = false;
        PluginViewActionResult actionResult = null;

        try
        {
            BeforeRun();
            success = PluginObject != null && PluginObject.ExecuteObj();
            watch.Stop();
            actionResult = success
                ? PluginViewActionResult.Ok("新视觉工具执行完成", watch.ElapsedMilliseconds)
                : PluginViewActionResult.Fail("新视觉工具执行失败", watch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            if (watch.IsRunning) watch.Stop();
            actionResult = PluginViewActionResult.Fail("新视觉工具执行异常：" + ex.Message, watch.ElapsedMilliseconds);
        }
        finally
        {
            if (watch.IsRunning) watch.Stop();
            AfterRun(success, watch.ElapsedMilliseconds);
        }

        return actionResult;
    }

    public PluginViewActionResult SaveFromHost(string filePath)
    {
        Stopwatch watch = Stopwatch.StartNew();
        bool success = false;

        try
        {
            BeforeSave();
            if (PluginObject != null)
            {
                PluginObject.moduleInfo.filePath = filePath;
                if (PluginObject.PluginParams != null) PluginObject.PluginParams.filePath = filePath;
                success = PluginObject.saveParams();
            }

            watch.Stop();
            return success
                ? PluginViewActionResult.Ok("参数已保存", watch.ElapsedMilliseconds)
                : PluginViewActionResult.Fail("参数保存失败", watch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            if (watch.IsRunning) watch.Stop();
            return PluginViewActionResult.Fail("参数保存异常：" + ex.Message, watch.ElapsedMilliseconds);
        }
        finally
        {
            if (watch.IsRunning) watch.Stop();
            AfterSave(success);
        }
    }

    public void CloseFromHost()
    {
        BeforeSave();
        DisposeView();
    }

    public void BeforeRun()
    {
        CommitBindings();
        vm.CommitToFrameParams();
    }

    public void AfterRun(bool success, long elapsedMs)
    {
        vm.LoadResultsFromParams(PluginParams as ParamsNewVisionTool);
        RefreshBindings();
    }

    public void BeforeSave()
    {
        CommitBindings();
        vm.CommitToFrameParams();
    }

    public void AfterSave(bool success)
    {
        RefreshBindings();
    }

    public void BeforeClose()
    {
        CloseFromHost();
    }
}
```

WPF 页面职责：绑定参数、提交 UI 输入、调用插件入口、刷新显示、释放界面局部资源。不要把算法写在按钮事件里，也不要绕过 `ParamsNewVisionTool` 使用私有输入图像。

建议页面内部按钮也复用同一套入口：

```csharp
private void OnRunClick(object sender, RoutedEventArgs e)
{
    RunFromHost();
}

private void OnSaveParamsClick(object sender, RoutedEventArgs e)
{
    SaveFromHost(PluginObject.moduleInfo.filePath);
}
```

这样无论点击基类底部按钮，还是插件页面内部调试按钮，执行链路都是同一套，不会出现两套内存释放和参数提交逻辑。

### 9.7 XAML 中文显示规范

XAML 中界面文字直接写中文，例如：

```xml
<TabItem Header="基础参数">
<Button Content="运行分析" />
<TextBlock Text="检测范围" />
```

不要把中文界面文字写成字符实体编码或十六进制编码。

直接中文便于现场调试、搜索、维护，也避免设计器里出现大量进制码影响阅读。源码文件统一使用 UTF-8 保存。

### 9.8 开发检查清单

新插件完成后至少检查以下项目：

- `PluginParams` 是否使用框架参数类，输入图像是否来自 `ParamsXXX.ho_Image`。
- `ExecuteObj()` 是否执行前释放上一轮输出。
- `ApplyResult()` 是否通过 `CopyObj` 写回输出 HObject。
- 算法临时 HObject 是否都在 `finally` 释放。
- `BlobAnalysisResult` 这类结果对象是否实现 `IDisposable`。
- 界面关闭时是否释放界面局部 HObject、显示适配器、事件引用。
- 界面是否没有释放 `ParamsXXX.ho_Image` 和 `ParamsXXX.m_ho_Out*`。
- XAML 是否直接使用中文，不使用字符实体编码或十六进制编码。
- Debug/Release 输出目录是否指向 `Mef_VisionFrame\bin\...\Plugins`。
- 主程序能否通过 MEF 正确加载插件 DLL。
## 10. 本次经验总结

这次 Blob 插件集成的核心经验不是某一个函数怎么写，而是要把框架、插件入口、参数类、界面层的责任边界固定下来。后续扩展其它插件时，先按下面这些规则检查，再写代码。

### 10.1 参数类是流程数据，不是界面资源管理器

`ParamsBlobAnalysis` 的定位是流程输入输出类，负责承载输入图像、运行参数和输出结果。它不是 WPF 页面，也不是显示控件，所以不要在参数类里写界面释放逻辑。

必须遵守：

- 输入图像统一从 `ParamsBlobAnalysis.ho_Image` 取。
- `BlobAnalysisViewModel.Image` 只返回 `frameParams.ho_Image`，不维护第二份图像。
- UI 层不释放 `ho_Image`。
- UI 层不释放 `m_ho_OutRegion`、`m_ho_OutSelectedRegion`、`m_ho_OutLocalRoiRegion`、`m_ho_OutMaskRoiRegion`。
- 输出 HObject 的释放由插件运行入口负责，执行前释放上一轮输出，插件销毁时释放最后一轮输出。

这条规则可以避免“关闭窗体把流程图像释放掉”“界面预览把输出结果释放掉”“下一模块读不到 Blob 输出”等问题。

### 10.2 输出 HObject 每次执行前释放，不放到界面关闭里释放

Blob 输出是插件对外结果，不是窗体局部变量。每次运行前释放上一轮输出，是为了保证本轮运行失败时不会继续暴露旧结果，也避免多次运行后 HObject 累积。

标准写法：

```csharp
public bool ExecuteObj()
{
    ParamsBlobAnalysis p = PluginParams as ParamsBlobAnalysis;
    if (p == null) return false;

    try
    {
        BlobParamsMapper.ReleaseOutputHObjects(p);

        BlobAnalysisResult result = null;
        try
        {
            result = engine.Run(p.ho_Image, runtime);
            BlobParamsMapper.ApplyResult(result, p);
        }
        finally
        {
            result?.Dispose();
        }

        return p.m_bRunSuccess;
    }
    catch
    {
        BlobParamsMapper.ReleaseOutputHObjects(p);
        return false;
    }
}
```

这里 `result.Dispose()` 释放的是算法临时结果。`ApplyResult()` 已经通过 `CopyObj` 把需要对外输出的对象复制到 `ParamsBlobAnalysis.m_ho_Out*`，所以释放 `result` 不会释放参数类输出。

### 10.3 `ApplyResult()` 抛异常时也必须释放算法临时结果

不能只在成功路径释放 `BlobAnalysisResult`。如果 `ApplyResult()` 写回输出时抛异常，`result` 里持有的 HObject 仍然需要释放。

因此 `result?.Dispose()` 必须在 `finally` 里，而不是放在 `ApplyResult()` 后面的普通语句里。

错误风险：

```csharp
BlobAnalysisResult result = engine.Run(...);
BlobParamsMapper.ApplyResult(result, p);
result.Dispose();
```

如果 `ApplyResult()` 抛异常，最后一行不会执行。

正确规则：算法临时结果一定用 `try/finally` 包住。

### 10.4 基类窗体只做转接，不写插件业务逻辑

`WPFFormPluginBase` 不应该知道 Blob 插件有哪些参数、有哪些 ROI、有哪些 HObject。基类只做三件事：

- 点击“执行”时调用 `PluginView.RunFromHost()`。
- 点击“保存”时计算保存路径，然后调用 `PluginView.SaveFromHost(filePath)`。
- 窗口关闭时调用 `PluginView.CloseFromHost()`。

耗时和状态文字由插件页面通过 `PluginViewActionResult` 返回给基类显示。这样后续新增 DLL 插件时，不需要为了每个插件改基类。

### 10.5 界面关闭要释放界面局部资源，但不能释放流程对象

WPF 页面关闭时释放的是页面自己创建、自己持有的资源：

- `selectedRegion`：页面为了高亮当前选中 Blob 额外 `SelectObj` 出来的对象。
- `BlobAnalysisViewModel.lastResult`：页面本地运行或预览产生的临时结果。
- `HalconViewRoiAdapter.displayImage`：Adapter 为 ViewROI 显示创建的 `HImage`。
- `HWindowControl`：页面创建的 WinForms/HALCON 显示控件。
- 页面事件和 Adapter 事件。

WPF 页面关闭时不释放：

- `ParamsBlobAnalysis.ho_Image`。
- `ParamsBlobAnalysis.m_ho_Out*`。
- 上游流程或其它模块传入的 HObject。

### 10.6 事件解绑和 HObject 释放同样重要

这次检查发现，内存问题不能只看 HObject。WPF、WinForms、HALCON 控件和 ViewROI 都可能通过事件引用把页面对象留住。

本插件中需要处理的事件包括：

```csharp
vm.PropertyChanged += OnViewModelPropertyChanged;
vm.AddFilterRequested = ShowAddFilterDialog;

mainDisplay.Redraw += DrawMainOverlay;
previewDisplay.Redraw += DrawPreviewOverlay;
resultDisplay.Redraw += DrawResultOverlay;
mainDisplay.RoiChanged += OnRoiChanged;

mainHalcon.HMouseMove += OnMainMouseMove;
mainHalcon.HMouseDown += OnMainMouseDown;

viewCtrl.ReDrawEvent += redrawHandler;
roiController.NotifyRCObserver += roiChangedHandler;
```

关闭时必须断开：

```csharp
mainHalcon.HMouseMove -= OnMainMouseMove;
mainHalcon.HMouseDown -= OnMainMouseDown;

vm.PropertyChanged -= OnViewModelPropertyChanged;
vm.AddFilterRequested = null;

viewCtrl.ReDrawEvent -= redrawHandler;
roiController.NotifyRCObserver -= roiChangedHandler;
```

不要用匿名委托绑定需要释放的事件。匿名委托无法可靠解绑，应该保存成字段，例如：

```csharp
private HWndCtrl.ReDrawDelegate redrawHandler;
private IconicDelegate roiChangedHandler;

redrawHandler = OnViewRoiRedraw;
roiChangedHandler = OnViewRoiChanged;
viewCtrl.ReDrawEvent += redrawHandler;
roiController.NotifyRCObserver += roiChangedHandler;
```

### 10.7 重新打开窗体后图像显示必须从参数类恢复

关闭窗体后重新打开，流程参数类里的 `ho_Image` 可能仍然存在。新界面必须主动从 `ParamsBlobAnalysis.ho_Image` 恢复显示，否则会出现“执行模块里有图像，但界面没有图像”的现象。

标准链路：

```csharp
public void BindPlugin(IPlugin plugin)
{
    PluginObject = plugin as BlobAnalysisObj;
    PluginParams = PluginObject == null ? null : PluginObject.PluginParams;

    ParamsBlobAnalysis p = PluginParams as ParamsBlobAnalysis;
    if (p == null) return;

    vm.BindFrameParams(p);
    ShowFrameImage(true);
}
```

`BindFrameParams()` 不是复制图像，而是保存同一个参数类引用。`vm.Image` 每次都从 `frameParams.ho_Image` 取图，所以图像转接发生在参数对象绑定这一刻。

### 10.8 内部调试按钮也要复用宿主入口

插件页面内部如果保留“运行分析”“保存参数”这类调试按钮，不能另写一条执行链路。否则会出现基类按钮一套逻辑、页面按钮一套逻辑，内存释放和参数提交不一致。

标准写法：

```csharp
private void OnRunClick(object sender, RoutedEventArgs e)
{
    RunFromHost();
}

private void OnSaveParamsClick(object sender, RoutedEventArgs e)
{
    SaveFromHost(PluginObject.moduleInfo.filePath);
}
```

正式集成时，DLL 页面里的保存/读取默认隐藏；调试时再显示。

### 10.9 文档和代码注释必须讲清所有权

视觉插件最容易出问题的是 HObject 所有权不清楚。每个插件至少要在代码注释和开发文档中写清楚：

- 谁创建这个 HObject。
- 谁负责释放这个 HObject。
- 这个 HObject 是流程输入、流程输出、算法临时对象，还是界面临时对象。
- 窗口关闭时释放哪些对象。
- 每次执行前释放哪些对象。
- 插件销毁时释放哪些对象。

只要所有权写清楚，后续扩展其它插件时就不容易把参数类、界面层、算法层混在一起。

## 11. 本插件最终内存管控结论

### 11.1 执行前

`BlobAnalysisObj.ExecuteObj()` 调用：

```csharp
BlobParamsMapper.ReleaseOutputHObjects(p);
```

释放上一轮输出：

- `m_ho_OutRegion`
- `m_ho_OutSelectedRegion`
- `m_ho_OutLocalRoiRegion`
- `m_ho_OutMaskRoiRegion`

不释放：

- `p.ho_Image`

### 11.2 执行中

`BlobAnalysisEngine.Run()` 内部创建的临时 HObject 在算法层 `finally` 释放。需要输出给外部的对象先 `CopyObj` 到结果对象，再由入口写回参数类。

### 11.3 写回结果时

`BlobParamsMapper.ApplyResult()` 写入 `ParamsBlobAnalysis.m_ho_Out*` 前会释放旧输出，再把新结果 `CopyObj` 到参数类输出字段。

### 11.4 写回异常时

`BlobAnalysisObj.ExecuteObj()` 使用 `try/finally` 保证 `BlobAnalysisResult result` 一定释放。即使 `ApplyResult()` 抛异常，算法临时结果也不会泄漏。

### 11.5 界面关闭时

`WPFBlobAnalysis.DisposeView()` 释放：

- `selectedRegion`
- `HalconViewRoiAdapter.displayImage`
- `BlobAnalysisViewModel.lastResult`
- 三个 `HWindowControl`
- 页面和 ViewROI 事件引用

`WPFBlobAnalysis.DisposeView()` 不释放：

- `ParamsBlobAnalysis.ho_Image`
- `ParamsBlobAnalysis.m_ho_OutRegion`
- `ParamsBlobAnalysis.m_ho_OutSelectedRegion`
- `ParamsBlobAnalysis.m_ho_OutLocalRoiRegion`
- `ParamsBlobAnalysis.m_ho_OutMaskRoiRegion`

### 11.6 插件销毁时

`BlobAnalysisObj.DisPoseObj()` 释放本插件生成的输出 HObject，不释放输入图像。

## 12. 后续插件开发强制检查项

新开发或迁移其它 WPF 视觉插件时，合并前必须检查：

- 参数类是否只做输入输出承载，没有混入界面释放逻辑。
- 输入图像是否统一来自 `ParamsXXX.ho_Image`。
- 页面是否没有维护第二份输入图像。
- 页面关闭是否没有释放 `ParamsXXX.ho_Image`。
- 页面关闭是否没有释放 `ParamsXXX.m_ho_Out*`。
- 执行入口是否在每次运行前释放上一轮输出 HObject。
- 算法临时 HObject 是否全部在 `finally` 释放。
- `ApplyResult()` 写回输出是否使用 `CopyObj`。
- 结果对象是否实现 `IDisposable`，入口是否用 `try/finally` 释放结果对象。
- 基类窗体是否只调用 `RunFromHost()`、`SaveFromHost()`、`CloseFromHost()`。
- 页面内部按钮是否复用同一套入口。
- 页面关闭时是否解绑 WPF 事件、WinForms/HALCON 控件事件、ViewROI 事件。
- 需要解绑的事件是否没有使用匿名委托，或者匿名委托不会形成页面持有链。
- 页面重新打开时是否从参数类恢复图像显示。
- XAML 是否直接写中文，不使用字符实体或十六进制编码。
- 文档是否说明 HObject 所有权和释放位置。
## 13. ViewROI 在界面层的接入和绑定规则

Blob 插件界面使用的是项目已有的 `ViewROI`，不是重新写一套 ROI 控件。ViewROI 只应该被界面层包装和调用，不应该把 ViewROI 代码改成某个插件专用逻辑。

本插件通过 `HalconViewRoiAdapter` 包一层，原因是 ViewROI 的 `HWndCtrl` 对图像、ROI、鼠标事件、自适应显示都有自己的状态。如果直接在 WPF 页面里到处操作 `HWndCtrl`，后续很容易出现图像不显示、ROI 坐标没回写、关闭窗体事件没解绑的问题。

### 13.1 ViewROI 的创建位置

WPF 页面里用 `WindowsFormsHost` 承载 `HWindowControl`，页面加载时创建三个显示窗口：

```csharp
private void InitHalconViews()
{
    if (mainDisplay != null) return;

    mainHalcon = CreateHalconControl();
    previewHalcon = CreateHalconControl();
    resultHalcon = CreateHalconControl();

    MainHost.Child = mainHalcon;
    PreviewHost.Child = previewHalcon;
    ResultHost.Child = resultHalcon;

    mainDisplay = new HalconViewRoiAdapter(mainHalcon, true);
    previewDisplay = new HalconViewRoiAdapter(previewHalcon, false);
    resultDisplay = new HalconViewRoiAdapter(resultHalcon, false);
}
```

规则：

- 主图窗口 `mainDisplay` 允许 ROI 操作，所以 `enableRoi = true`。
- 预览窗口和结果窗口只显示图像和结果，不允许编辑 ROI，所以 `enableRoi = false`。
- `HWindowControl` 由页面创建，也由页面关闭时释放。
- `ViewROI` 的鼠标缩放、平移、ROI 拖动由 `HWndCtrl` 和 `ROIController` 管理，插件页面只接收事件和回写参数。

### 13.2 图像绑定必须使用插件参数类的图像

ViewROI 显示的图像必须来自 `ParamsBlobAnalysis.ho_Image`，不能使用页面私有图像。

绑定链路如下：

```csharp
public void BindPlugin(IPlugin plugin)
{
    PluginObject = plugin as BlobAnalysisObj;
    PluginParams = PluginObject == null ? null : PluginObject.PluginParams;

    ParamsBlobAnalysis p = PluginParams as ParamsBlobAnalysis;
    if (p == null) return;

    vm.BindFrameParams(p);
    ShowFrameImage(true);
}
```

`vm.BindFrameParams(p)` 保存的是同一个 `ParamsBlobAnalysis` 引用：

```csharp
public void BindFrameParams(ParamsBlobAnalysis source)
{
    frameParams = source;
    parameters = BlobParamsMapper.ToRuntime(frameParams);
    UpdateImageInfo("流程输入图像");
    ReloadCollections();
    OnPropertyChanged(string.Empty);
}
```

`vm.Image` 每次都从参数类取图：

```csharp
public HObject Image
{
    get { return frameParams == null ? null : frameParams.ho_Image; }
}
```

因此，ViewROI 的底图绑定不是复制图像，而是通过 ViewModel 转接到同一个 `ParamsBlobAnalysis.ho_Image`。

### 13.3 重新打开窗体时必须主动恢复 ViewROI 底图

窗口关闭后再打开时，新的 WPF 页面和新的 `HWindowControl` 都是重新创建的，但流程参数类里的 `ho_Image` 可能仍然存在。此时如果不主动调用 `ShowFrameImage(true)`，执行模块里有图像，界面却没有底图。

所以两个位置都要尝试恢复显示：

```csharp
public void BindPlugin(IPlugin plugin)
{
    vm.BindFrameParams(p);
    ShowFrameImage(true);
}

private void InitHalconViews()
{
    // 创建 mainDisplay/previewDisplay/resultDisplay 后
    ShowFrameImage(true);
}
```

这样可以覆盖两种加载顺序：

- 先绑定参数，再创建 ViewROI 控件。
- 先创建 ViewROI 控件，再绑定参数。

`ShowFrameImage()` 必须容错：没有控件、没有图像、图像未初始化时直接返回。

```csharp
private void ShowFrameImage(bool fit)
{
    if (mainDisplay == null || vm.Image == null || !vm.Image.IsInitialized()) return;

    mainDisplay.ShowImage(vm.Image, fit);
    previewDisplay.ShowImage(vm.Image, fit);
    resultDisplay.ShowImage(vm.Image, fit);
    EnsureViewRoi();
}
```

### 13.4 ViewROI 显示图像时要转成 HImage

当前 ViewROI 的 `addIconicVar()` 对 `HImage` 支持最好。为了让 ViewROI 正确计算图像宽高、自适应范围和缩放范围，`HalconViewRoiAdapter.ShowImage()` 会把外部传入的 `HObject` 转成内部显示用 `HImage`。

```csharp
private void RefreshDisplayImage()
{
    displayImage?.Dispose();
    displayImage = null;
    if (image == null || !image.IsInitialized()) return;

    HOperatorSet.GetImagePointer1(image, out HTuple pointer, out HTuple type, out HTuple width, out HTuple height);
    displayImage = new HImage();
    displayImage.GenImage1(type, width, height, pointer);
}
```

这里的所有权必须分清楚：

- `image` 是外部传入的 `ParamsBlobAnalysis.ho_Image`，Adapter 不释放。
- `displayImage` 是 Adapter 自己创建的显示对象，Adapter 负责释放。

### 13.5 ViewROI ROI 参数的绑定方向

ROI 参数不是直接绑定到 ViewROI 控件属性，而是通过 ViewModel 和 `SaveCurrentViewRoi()` 双向同步。

界面参数到 ViewROI：

```csharp
private void EnsureViewRoi()
{
    if (mainDisplay == null || vm.Image == null || !vm.Image.IsInitialized()) return;

    mainDisplay.SetRoiOperate(vm.ShowViewRoi);
    if (!vm.ShowViewRoi)
    {
        mainDisplay.ClearRoi();
        return;
    }

    if (vm.EditRole == RoiEditRole.Mask)
    {
        RoiParam r = vm.SelectedMaskRoi.Model;
        mainDisplay.EnsureRectangleRoi(r.Row1, r.Col1, r.Row2, r.Col2);
    }
    else
    {
        mainDisplay.EnsureRectangleRoi(vm.LocalRow1, vm.LocalCol1, vm.LocalRow2, vm.LocalCol2);
    }
}
```

ViewROI 到界面参数：

```csharp
private void SaveCurrentViewRoi()
{
    if (mainDisplay == null || !mainDisplay.TryGetRectangleRoi(out double row1, out double col1, out double row2, out double col2)) return;

    if (vm.EditRole == RoiEditRole.Mask && vm.SelectedMaskRoi != null)
    {
        vm.SelectedMaskRoi.Row1 = row1;
        vm.SelectedMaskRoi.Col1 = col1;
        vm.SelectedMaskRoi.Row2 = row2;
        vm.SelectedMaskRoi.Col2 = col2;
    }
    else
    {
        vm.LocalRow1 = row1;
        vm.LocalCol1 = col1;
        vm.LocalRow2 = row2;
        vm.LocalCol2 = col2;
    }
}
```

运行和保存前必须调用：

```csharp
CommitBindings();
SaveCurrentViewRoi();
vm.CommitToFrameParams();
```

这样 ViewROI 上最后拖动的位置才会回写到 `ParamsBlobAnalysis`。

### 13.6 ViewROI 事件绑定规则

ViewROI 相关事件分两层。

页面订阅 Adapter：

```csharp
mainDisplay.Redraw += DrawMainOverlay;
previewDisplay.Redraw += DrawPreviewOverlay;
resultDisplay.Redraw += DrawResultOverlay;
mainDisplay.RoiChanged += OnRoiChanged;
```

Adapter 订阅 ViewROI：

```csharp
redrawHandler = OnViewRoiRedraw;
roiChangedHandler = OnViewRoiChanged;
viewCtrl.ReDrawEvent += redrawHandler;
roiController.NotifyRCObserver += roiChangedHandler;
```

不要用匿名委托绑定需要释放的 ViewROI 事件。要把委托保存成字段，关闭时才能解绑：

```csharp
if (redrawHandler != null) viewCtrl.ReDrawEvent -= redrawHandler;
if (roiChangedHandler != null) roiController.NotifyRCObserver -= roiChangedHandler;
```

页面直接绑定到 `HWindowControl` 的鼠标事件也必须解绑：

```csharp
mainHalcon.HMouseMove += OnMainMouseMove;
mainHalcon.HMouseDown += OnMainMouseDown;
```

关闭时：

```csharp
mainHalcon.HMouseMove -= OnMainMouseMove;
mainHalcon.HMouseDown -= OnMainMouseDown;
```

### 13.7 ViewROI 重绘职责

ViewROI 自己负责画图像和 ROI，插件页面只在 `Redraw` 回调里补画业务图形：

- 局部 ROI 和屏蔽 ROI 的跟随显示。
- Base 点、Current 点。
- Blob 输出区域。
- 当前选中 Blob 的高亮。
- Blob 编号文字。

不要在普通属性变化里直接到处 `DispObj`，否则 ViewROI 缩放、平移或重绘后会丢图。业务图形统一放到：

```csharp
private void DrawMainOverlay()
private void DrawPreviewOverlay()
private void DrawResultOverlay()
```

然后通过：

```csharp
mainDisplay.Repaint();
previewDisplay.Repaint();
resultDisplay.Repaint();
```

触发刷新。

### 13.8 ViewROI 与位置跟随

Blob 插件支持位置跟随时，界面上显示的 ROI 不一定是参数原始坐标，而是经过 `BlobAnalysisFollowTransformer` 变换后的显示坐标。

显示时：

```csharp
BlobAnalysisFollowTransformer transformer = new BlobAnalysisFollowTransformer(vm.Parameters);
RoiParam r = transformer.TransformRect(vm.Parameters.LocalRoi);
HOperatorSet.DispRectangle1(window, r.Row1, r.Col1, r.Row2, r.Col2);
```

注意区分：

- 参数类保存的是原始 ROI 参数。
- 界面显示可以画跟随后的 ROI。
- 用户正在编辑 ViewROI 时，回写的是当前界面编辑坐标。

如果后续插件也有跟随、旋转、模板坐标系转换，必须明确“保存坐标”和“显示坐标”是不是同一个坐标系。

### 13.9 ViewROI 关闭释放规则

`DisposeView()` 中释放顺序建议如下：

1. 释放页面临时 HObject，例如 `selectedRegion`。
2. 解绑页面直接绑定的 `HWindowControl` 鼠标事件。
3. Dispose 三个 `HalconViewRoiAdapter`，由 Adapter 解绑 ViewROI 事件并释放 `displayImage`。
4. 从 `WindowsFormsHost.Child` 移除 `HWindowControl`。
5. Dispose `HWindowControl`。
6. 释放 ViewModel 本地临时结果。
7. 解绑 ViewModel 事件。

示例：

```csharp
selectedRegion?.Dispose();
selectedRegion = null;

if (mainHalcon != null)
{
    mainHalcon.HMouseMove -= OnMainMouseMove;
    mainHalcon.HMouseDown -= OnMainMouseDown;
}

mainDisplay?.Dispose();
previewDisplay?.Dispose();
resultDisplay?.Dispose();

MainHost.Child = null;
PreviewHost.Child = null;
ResultHost.Child = null;

mainHalcon?.Dispose();
previewHalcon?.Dispose();
resultHalcon?.Dispose();

vm.ReleaseViewResources();
vm.PropertyChanged -= OnViewModelPropertyChanged;
vm.AddFilterRequested = null;
```

这里仍然不释放 `ParamsBlobAnalysis.ho_Image` 和 `ParamsBlobAnalysis.m_ho_Out*`。

### 13.10 ViewROI 开发检查清单

后续插件接入 ViewROI 时必须检查：

- ViewROI 是否只在界面层创建和包装，没有写入参数类。
- 显示图像是否来自 `ParamsXXX.ho_Image`。
- Adapter 是否只释放自己创建的显示对象，不释放外部输入图像。
- 窗口重新打开后是否主动从参数类恢复图像显示。
- ROI 从参数到界面是否通过 `EnsureViewRoi()` 刷新。
- ROI 从界面到参数是否在运行和保存前通过 `SaveCurrentViewRoi()` 回写。
- `CommitBindings()`、`SaveCurrentViewRoi()`、`vm.CommitToFrameParams()` 是否按顺序调用。
- ViewROI 事件是否使用可解绑委托。
- `HWindowControl` 鼠标事件是否关闭时解绑。
- 关闭页面时是否 Dispose Adapter 和 `HWindowControl`。
- 页面关闭时是否没有释放流程输入图像和输出 HObject。
- 业务叠加绘制是否放在 Redraw 回调里。
## 14. 执行后界面刷新机制

这里必须写清楚，否则后续插件会出现“插件已经执行成功，但是 WPFDebugProcessView 还是红色或未执行”的问题。

### 14.1 为什么点击执行后调试界面会刷新

现在刷新不是靠外层界面猜，也不是靠定时器刷。完整链路如下：

```text
用户点击插件基类窗体里的“执行”
  -> WPFFormPluginBase.Btn_RUN_Click() 或 FormPluginBase.Btn_RUN_Click()
  -> 基类窗体调用插件执行逻辑
       WPF：插件页面 RunFromHost()
       WinForm：pluginsObjBase.ExecuteObj()
  -> 插件 ExecuteObj()/RunFromHost() 把 m_bRunSuccess / m_strRunMessage / m_fElapsedMs 等状态写回 PluginParams
  -> 基类窗体显示本窗口耗时和状态
  -> 基类窗体 RaiseActionCompleted(result)
  -> PluginDialogService 转发通知
  -> WPFDebugProcessView.OnPluginActionCompleted()
  -> RefreshModuleList()
  -> DebugProcessCardFactory 重新创建模块卡片
  -> ModuleRunStateReader 重新读取 PluginParams
  -> 卡片右侧显示成功、失败、未执行
```

关键点：外层调试界面的红绿状态，最终读取的是插件参数类 `PluginParams`，不是直接相信 `PluginViewActionResult`，也不是依赖 `moduleInfo.CostTime`。`PluginViewActionResult` 只用于基类窗体本次按钮动作的状态显示和刷新通知，真正的流程状态必须写进参数类。

### 14.2 三个角色的职责

| 角色 | 职责 | 不能做什么 |
| --- | --- | --- |
| `WPFFormPluginBase` / `FormPluginBase` | 拥有执行按钮，调用插件执行，显示本窗体耗时和状态，触发 `ActionCompleted` | 不补写业务结果，不替插件维护 `PluginParams` 输出字段 |
| 插件页面或插件对象 | 提交界面参数，执行算法，把成功、失败、耗时、错误信息写回 `PluginParams` | 不直接操作 `WPFDebugProcessView`，不承担外层刷新通知 |
| `WPFDebugProcessView` | 订阅基类窗体动作完成事件，重新读取 `PluginParams` 并刷新模块卡片 | 不进入插件内部算法，不替插件判断业务成功条件 |

### 14.3 通用通知接口

通知能力是基类窗体能力，不属于具体插件页面，也不属于 WPF 专用能力：

```csharp
public interface IPluginActionNotifier
{
    event EventHandler<PluginViewActionResult> ActionCompleted;
}
```

`WPFFormPluginBase` 和 `FormPluginBase` 实现该接口。具体插件页面不实现该接口，避免插件耦合外层调试界面。

### 14.4 参数类必须写哪些状态

调试界面当前按下面顺序读取运行状态：

1. 优先读 `PluginParams.m_bRunSuccess`。
2. 如果没有这个字段，再兼容读取 `RunSuccess`、`IsRunSuccess`、`Success`。
3. 如果成功字段是 `true`，显示绿色“成功”。
4. 如果成功字段是 `false`，并且有执行证据，显示红色“失败”。
5. 如果没有执行证据，显示灰色“未执行”。

执行证据只来自 `PluginParams`，例如：`m_fElapsedMs > 0` 或 `m_strRunMessage` 非空。`moduleInfo.CostTime` 只做界面耗时显示，不参与状态灯判断。

新插件建议统一在参数类中提供：

```csharp
public bool m_bRunSuccess;
public double m_fElapsedMs;
public string m_strRunMessage;
```

### 14.5 调试界面为什么重新读取参数而不是直接用 result

原因有三个：流程框架的真实输入输出载体是 `PluginParams`；有些插件可能不是从插件窗体执行，而是流程运行调用；外层调试界面只做框架展示，不应该了解每个插件自己的业务字段。

因此，`WPFDebugProcessView.OnPluginActionCompleted()` 收到事件后只做一件事：

```csharp
Dispatcher.BeginInvoke(new Action(RefreshModuleList));
```

`RefreshModuleList()` 会重新生成模块卡片，卡片里的状态灯再通过 `ModuleRunStateReader` 读取插件参数状态。

### 14.6 订阅和解绑规则

`PluginDialogService` 打开插件窗口时订阅基类窗体的 `ActionCompleted`：

```csharp
window.ActionCompleted += handler;
form.ActionCompleted += handler;
```

窗口关闭后必须解绑：

```csharp
window.ActionCompleted -= handler;
form.ActionCompleted -= handler;
```

这样做是为了防止窗口被事件引用链持有，导致关闭后页面、ViewROI、Halcon 控件不能释放。

### 14.7 新插件开发检查清单

- WPF 插件页面是否实现 `IPluginWpfView`。
- WPF 页面内部执行按钮是否也调用 `RunFromHost()`，保证和基类执行按钮同一条逻辑。
- `RunFromHost()` 是否先提交绑定和 ROI，再调用 `ExecuteObj()`。
- WinForm 插件是否继续使用 `FormPluginBase` 通用执行按钮。
- `ExecuteObj()` 是否把成功、失败、耗时、错误信息写回参数类。
- 异常时是否也写 `m_bRunSuccess = false`。
- 外层刷新是否只通过基类窗体 `ActionCompleted`，不直接引用 `WPFDebugProcessView`。
- 插件页面是否只释放界面局部对象，不释放参数类输入图像和输出 HObject。
## 15. 框架使用说明、开发规范与经验总结

本章给后续插件开发者使用。目标不是只让 Blob 插件能跑，而是让后续测量、定位、缺陷、标定类插件都能按同一套框架扩展，不破坏主流程、不破坏参数载体、不破坏 HObject 生命周期。

### 15.1 框架边界

| 层级 | 代表类 | 主要职责 | 不应该做的事 |
| --- | --- | --- | --- |
| 流程层 | `Process`、`IPlugin`、`PluginParamsBase` | 管理流程、模块、参数输入输出、执行顺序 | 不依赖具体插件页面 |
| 插件对象层 | `BlobAnalysisObj`、其它 `XXXObj` | 实现 `ExecuteObj()`，读写 `PluginParams`，负责算法执行入口 | 不直接操作 WPF 调试界面 |
| 参数层 | `ParamsBlobAnalysis`、其它 `ParamsXXX` | 保存输入参数、输出结果、执行状态，是流程真实数据载体 | 不承担界面控件释放，不写 ViewROI 控件对象 |
| 界面层 | `WPFBlobAnalysis`、`FormXXX`、ViewModel、ViewROI Adapter | 参数绑定、ROI 编辑、预览显示、界面局部资源释放 | 不释放流程输入图像，不替插件对象维护业务输出 |

最关键的一句话：`PluginParams` 是框架的数据输入输出类，界面只是参数编辑和显示入口。后续插件不要把界面变量当成流程变量，也不要把流程输出交给界面关闭时处理。

### 15.2 使用说明：模块状态和耗时怎么看

`WPFDebugProcessView` 的模块列表卡片右侧显示运行状态：

- 绿色：`PluginParams.m_bRunSuccess == true`。
- 红色：`PluginParams.m_bRunSuccess == false`，并且存在执行证据。
- 灰色：未执行或参数类没有可判断的执行状态。

状态灯下面显示耗时：

```text
耗时：xx ms
```

耗时读取顺序：

1. 优先读取 `PluginParams.m_fElapsedMs`。
2. 如果参数里没有耗时，再退回显示 `moduleInfo.CostTime`。
3. 如果两个都没有，显示 `-- ms`。

注意：状态灯不靠 `moduleInfo.CostTime` 判断成功失败。`moduleInfo.CostTime` 只能做界面耗时显示兜底，不能代表业务执行成功。

### 15.3 点击执行后的刷新链路

无论 WPF 插件还是 WinForm 插件，刷新链路都统一走基类窗体：

```text
用户点击插件窗体底部“执行”
  -> WPFFormPluginBase.Btn_RUN_Click() 或 FormPluginBase.Btn_RUN_Click()
  -> 插件执行逻辑
       WPF：IPluginWpfView.RunFromHost()
       WinForm：IPlugin.ExecuteObj()
  -> 插件把运行状态写入 PluginParams
       m_bRunSuccess
       m_fElapsedMs
       m_strRunMessage
  -> 基类窗体显示本窗体耗时和状态
  -> 基类窗体 RaiseActionCompleted(result)
  -> PluginDialogService 转发事件
  -> WPFDebugProcessView.OnPluginActionCompleted()
  -> RefreshModuleList()
  -> DebugProcessCardFactory 重新创建模块卡片
  -> ModuleRunStateReader 重新读取 PluginParams
```

`PluginViewActionResult` 的作用是让基类窗体显示本次按钮动作结果，并触发外层刷新。它不是流程最终状态来源。流程最终状态必须写入 `PluginParams`。

### 15.4 WPF 插件开发步骤

从零开发一个新的 WPF 插件，按下面顺序做：

1. 新建插件项目 `Plugin.XXX`，引用 `Frame_Core` 和算法所需 DLL。
2. 新建参数类 `ParamsXXX : PluginParamsBase`，放输入参数、输出结果、运行状态。
3. 新建插件对象 `XXXObj : IPlugin`，实现 `ExecuteObj()`、`saveParams()`、`DisPoseObj()`。
4. 新建算法层 `XXXEngine`，算法层不引用 WPF，不引用 ViewROI。
5. 新建 ViewModel，把界面控件值和 `ParamsXXX` 双向同步。
6. 新建 WPF 页面，实现 `IPluginWpfView`。
7. WPF 页面内部按钮也调用 `RunFromHost()`，保证和基类窗体执行按钮同一条逻辑。
8. 在插件导出信息里声明插件名称、分类、窗体类型或 WPF 页面绑定入口。
9. 编译插件 DLL，放入主程序插件目录。
10. 在 `WPFDebugProcessView` 添加模块、打开窗体、执行，检查状态灯和耗时。

WPF 插件页面必须提供绑定入口：

```csharp
public void BindPlugin(IPlugin plugin)
{
    PluginObject = plugin;
    PluginParams = PluginObject == null ? null : PluginObject.PluginParams;
    vm.BindFrameParams(PluginParams as ParamsXXX);
}
```

这一步只是把页面 ViewModel 指向同一个参数对象，不复制图像，不新建流程参数。图像必须来自 `ParamsXXX.ho_Image`。

### 15.5 WinForm 插件开发步骤

现有 WinForm 插件继续继承 `FormPluginBase`，最小改动原则如下：

1. 插件窗体不要自己重新做一套外层刷新通知。
2. 公共执行按钮继续走 `FormPluginBase.Btn_RUN_Click()`。
3. 插件对象的 `ExecuteObj()` 必须把执行状态写回 `PluginParams`。
4. 如果插件窗体重写 `Btn_RUN_Click()`，必须确认最终会调用 `base.Btn_RUN_Click(sender, e)`，否则外层调试界面收不到刷新通知。
5. 不要让 WinForm 页面直接引用 `WPFDebugProcessView`。

WinForm 插件的核心仍然是 `ExecuteObj()`：

```csharp
public bool ExecuteObj()
{
    ParamsXXX p = PluginParams as ParamsXXX;
    Stopwatch watch = Stopwatch.StartNew();
    try
    {
        // 1. 释放上一轮输出 HObject。
        // 2. 检查输入图像和参数。
        // 3. 调用算法。
        // 4. 写回输出结果。
        p.m_bRunSuccess = true;
        p.m_strRunMessage = "执行完成";
        return true;
    }
    catch (Exception ex)
    {
        p.m_bRunSuccess = false;
        p.m_strRunMessage = ex.Message;
        return false;
    }
    finally
    {
        watch.Stop();
        p.m_fElapsedMs = watch.Elapsed.TotalMilliseconds;
    }
}
```

### 15.6 参数类开发规范

参数类是流程真实输入输出载体，必须稳定、清晰、可序列化。

建议每个插件参数类都包含：

```csharp
public class ParamsXXX : PluginParamsBase
{
    [NonSerialized]
    [XmlIgnore]
    public HObject ho_Image;

    [NonSerialized]
    [XmlIgnore]
    public HObject m_ho_OutRegion;

    [NonSerialized]
    [XmlIgnore]
    public HObject m_ho_OutImgShow;

    [NonSerialized]
    [XmlIgnore]
    public bool m_bRunSuccess;

    [NonSerialized]
    [XmlIgnore]
    public double m_fElapsedMs;

    [NonSerialized]
    [XmlIgnore]
    public string m_strRunMessage;
}
```

规则：

- 输入图像字段由流程或上游模块传入，插件不能随便释放。
- 输出 HObject 是本插件生成的结果，插件执行前释放上一轮，插件销毁时释放最后一轮。
- 运行状态写在参数类里，外层调试界面只读取参数类。
- 需要保存到 XML 的参数不要使用 `HObject`、`HTuple`、控件对象、事件对象。
- 运行态字段使用 `[NonSerialized]` 和 `[XmlIgnore]`。

### 15.7 HObject 内存管控规则

视觉插件必须先分清 HObject 所有权：

| 对象类型 | 示例 | 谁释放 |
| --- | --- | --- |
| 流程输入图像 | `ParamsXXX.ho_Image` | 上游创建者或流程框架释放，插件不释放 |
| 插件输出结果 | `ParamsXXX.m_ho_OutRegion`、`m_ho_OutImgShow` | 插件执行前释放上一轮，`DisPoseObj()` 释放最后一轮 |
| 算法临时对象 | 灰度图、阈值区域、连通域中间结果 | 算法层 `finally` 释放 |
| 界面显示临时对象 | ViewROI Adapter 内部 `displayImage`、页面选中区域 | 界面关闭时释放 |

执行入口必须遵循：

```text
执行前释放上一轮输出
  -> 本轮算法运行
  -> 写回新输出
  -> 无论 ApplyResult 是否异常，算法临时结果 finally 释放
```

不要把输出 HObject 的释放放到界面关闭才做。输出结果是流程数据，不是界面局部变量。

### 15.8 ViewROI 接入规则

ViewROI 只属于界面层，不能写进参数类。

必须遵守：

- ViewROI 显示图像来自 `ParamsXXX.ho_Image`。
- 打开窗体时主动从参数类恢复底图。
- 运行和保存前先 `CommitBindings()`，再 `SaveCurrentViewRoi()`，最后 `vm.CommitToFrameParams()`。
- ROI 坐标最终写入 `ParamsXXX` 的普通数值字段。
- ViewROI Adapter 可以创建自己的显示 `HImage`，但只能释放自己创建的显示对象。
- 关闭页面时解绑 ViewROI、HWindowControl、ViewModel 事件。
- 关闭页面时不释放参数类输入图像和输出 HObject。

重新打开窗体时，如果插件对象里已有 `ParamsXXX.ho_Image`，界面必须显示这个图像；如果界面没有显示，但执行模块里有图像，通常是页面绑定或 ViewROI 恢复链路断了，不是执行模块没有图。

### 15.9 代码注释要求

后续插件代码注释不要写空话，要写所有权和调用时机。

推荐注释风格：

```csharp
// 每次执行前释放上一轮输出，避免本轮失败时外部继续读取旧结果。
ReleaseOutputHObjects(p);

// 输入图像由流程传入，本插件只读取，不释放。
HObject image = p.ho_Image;

// ViewROI 是界面交互对象，运行前必须把当前 ROI 坐标回写到参数类。
SaveCurrentViewRoi();
```

不推荐注释：

```csharp
// 执行算法
// 保存参数
// 释放对象
```

这些注释没有说明边界，后续维护者仍然不知道谁负责释放、何时刷新、数据从哪里来。

### 15.10 调试排查顺序

如果点击执行后外层状态没有刷新，按下面顺序打断点：

1. `WPFFormPluginBase.Btn_RUN_Click()` 或 `FormPluginBase.Btn_RUN_Click()`。
2. WPF 插件的 `RunFromHost()`，或 WinForm 插件对象的 `ExecuteObj()`。
3. 插件是否写入 `PluginParams.m_bRunSuccess`、`m_fElapsedMs`、`m_strRunMessage`。
4. `RaiseActionCompleted(result)` 是否执行。
5. `PluginDialogService` 里订阅的 handler 是否触发。
6. `WPFDebugProcessView.OnPluginActionCompleted()` 是否进入。
7. `RefreshModuleList()` 是否重新创建模块卡片。
8. `ModuleRunStateReader.GetState()` 和 `GetElapsedText()` 是否读到参数。

如果第 1 步没进，说明插件没有使用基类执行按钮或重写后没调用 base。

如果第 4 步没进，说明基类执行链路被绕开。

如果第 8 步读不到状态，说明插件没有把状态写回参数类，或者当前调试的 DLL 不是最新编译结果。

### 15.11 Debug / Release 和文件编码经验

本项目经常在 Visual Studio 中切换 `Debug | Any CPU` 和 `Release | Any CPU`。如果调试的是 Release，但是只编译了 Debug，界面不会出现新改动。

Release DLL 被运行程序占用时，必须先关闭主程序再编译。常见现象是 `Frame_Core.dll`、相机 SDK DLL 或插件 DLL 被 `Mef_VisionFrame` 占用，导致复制失败。

文件编码也要注意：

- 中文源码建议统一 UTF-8。
- 修改前先确认文件没有乱码。
- 发现 `�`、`锛` 这类字符时，不要继续在坏文本上补代码，先恢复成干净中文。
- 绿盾、网盘同步、老项目 ANSI 文件可能导致工具按错编码读取。
- 文档和新增 WPF 代码建议保存为 UTF-8，避免后续 diff 和搜索乱码。

### 15.12 本次经验总结

这次梳理后，框架应该按下面原则继续演进：

- 基类窗体做通用按钮、耗时显示、事件通知。
- 插件对象做业务执行和参数写回。
- 参数类做流程数据输入输出。
- WPF 页面做参数编辑、ViewROI 交互和界面局部释放。
- 外层调试界面只订阅基类事件，然后重新读取 `PluginParams`。
- 状态和耗时必须能从 `PluginParams` 读到，不能只停留在页面变量。
- HObject 释放必须按所有权处理，不能靠“界面关了顺便释放所有东西”。
- 后续新增插件时，先照检查清单确认数据链路，再写界面美化。

一句话收束：插件开发的稳定性，核心不是某个按钮怎么写，而是数据所有权、执行入口、刷新通知、资源释放四条链路必须清楚。
### 15.13 耗时输出强制要求

后续所有插件都必须输出执行耗时。调试界面模块卡片右侧的耗时显示读取的是 `PluginParams.m_fElapsedMs`，所以插件执行入口必须在每次执行后写回该字段。

推荐写法：

```csharp
public bool ExecuteObj()
{
    ParamsXXX p = PluginParams as ParamsXXX;
    Stopwatch watch = Stopwatch.StartNew();
    try
    {
        // 执行算法并写回输出结果。
        p.m_bRunSuccess = true;
        p.m_strRunMessage = "执行完成";
        return true;
    }
    catch (Exception ex)
    {
        p.m_bRunSuccess = false;
        p.m_strRunMessage = ex.Message;
        return false;
    }
    finally
    {
        watch.Stop();
        p.m_fElapsedMs = watch.Elapsed.TotalMilliseconds;
    }
}
```

要求：

- 成功、失败、异常都必须写 `m_fElapsedMs`。
- 不要只在界面基类里显示耗时，流程执行不经过插件窗体，必须由插件 `ExecuteObj()` 写回参数类。
- `PluginViewActionResult.ElapsedMs` 只服务于插件窗体底部显示，不能替代 `PluginParams.m_fElapsedMs`。
- 调试界面 `WPFDebugProcessView` 只读取参数类显示耗时，不进入插件内部计算耗时。