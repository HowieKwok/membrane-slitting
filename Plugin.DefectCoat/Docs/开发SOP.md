# Plugin.DefectCoat 开发 SOP

## 1. 适用范围

本文档用于约束 `Plugin.DefectCoat` 涂布缺陷检测插件的后续维护、排查、参数迁移、ROI 绑定、界面调整、检测算法扩展和分类器接入。任何修改都必须先确认自己改的是界面层、适配层、参数层、检测层、分类层还是框架入口层，不允许为了临时修界面问题直接改检测主流程。

本 SOP 覆盖以下目录：

- `Plugin.DefectCoat\Models`：插件入口对象。
- `Plugin.DefectCoat\Core\Detection`：检测请求、检测结果、HALCON 阈值检测、ROI 位置跟随。
- `Plugin.DefectCoat\Core\Pipeline`：检测和分类调度。
- `Plugin.DefectCoat\Services`：HALCON 显示、ViewROI 适配、路径服务。
- `Plugin.DefectCoat\ViewModels`：界面绑定、命令、参数适配对象。
- `Plugin.DefectCoat\Views` 和 `Plugin.DefectCoat\WPF架构`：WPF 页面和窗口。
- `Plugin.DefectCoat\ViewROI`：HALCON ROI 交互控件源码，原则上只做注释，不改行为。
- `Frame_Core\pluginParams\ParamsDefectCoat.cs`、`CoatRoiParam.cs`、`CoatPartitionParam.cs`：可保存的公共参数类。

## 2. 基本原则

1. 先定位层级，再修改代码。
2. 参数保存和运行输入必须以 `ParamsDefectCoat` 为准。
3. WPF 界面不要直接绑定不存在或不参与运行的字段。
4. Rectangle1 ROI 只认 `Row1`、`Col1`、`Row2`、`Col2` 四个实际坐标。
5. `CenterRow`、`CenterCol`、`Length1`、`Length2`、`RoiWidth`、`RoiHeight` 只能作为派生兼容字段使用，不作为 Rectangle1 的主绑定源。
6. 位置跟随只在运行时通过 HALCON 仿射变换生成当前 Region，不能把旋转后的坐标写回原始 ROI 参数。
7. 宿主传入的 `ho_Image` 不由插件释放。
8. `bin`、`obj`、生成的 `.g.cs`、`.g.i.cs` 不允许手工维护。
9. 注释可以补充，逻辑不能顺手改。
10. 每次改完必须编译，并说明 warning 是否为既有代码。

## 3. 开发前检查

开发前必须执行以下检查：

```powershell
git -C "D:\BaiduNetdiskDownload\宽幅打孔新方案1107\宽幅打孔新方案1017\0703计划做流程插件管理修改计划\ProcessWideHoleLaser" status --short
```

检查重点：

- 是否已有用户未提交修改。
- 当前要改的文件是否已经被别人改过。
- `Plugin.DefectCoat` 是否处于未跟踪状态。当前仓库中该目录可能是未跟踪目录，不能用普通 `git diff` 证明文件没有变化，必须结合文件内容和编译结果核对。
- 不要执行 `git reset --hard`、`git checkout --` 这类会覆盖用户工作的命令。

## 4. 分层职责

### 4.1 插件入口层

文件：`Models\DefectCoatObj.cs`

职责：

- 实现框架 `IPlugin`。
- 持有 `ParamsDefectCoat`。
- 初始化检测 pipeline、特征值分类器、传统分类器占位、AI 占位。
- 执行 `ExecuteObj()`。
- 将检测和分类输出回写到 `PluginParams`。
- 保存、加载、释放运行态输出。

禁止事项：

- 不在入口对象里写 WPF 控件逻辑。
- 不在入口对象里释放宿主传入的输入图像。
- 不绕过 `ParamsDefectCoat` 私自保存检测参数。

### 4.2 参数层

文件：

- `Frame_Core\pluginParams\ParamsDefectCoat.cs`
- `Frame_Core\pluginParams\CoatRoiParam.cs`
- `Frame_Core\pluginParams\CoatPartitionParam.cs`

职责：

- 保存可序列化参数。
- 保存运行期输入输出 HObject 引用。
- 提供 XML 序列化所需的公开属性和字段。
- 提供释放运行输出的方法。

禁止事项：

- 参数类不能继承 WPF ViewModel 基类。
- 参数类不能引用窗口、控件、命令、ViewROI 适配器。
- 参数类不能承担界面刷新通知职责。

### 4.3 检测层

文件：

- `Core\Detection\HalconThresholdDefectDetector.cs`
- `Core\Detection\DefectRoiFollowTransformer.cs`
- `Core\Detection\DefectDetectRequest.cs`
- `Core\Detection\DefectDetectResult.cs`
- `Core\Detection\IDefectDetector.cs`

职责：

- 接收检测请求。
- 转灰度。
- 根据局部 ROI、屏蔽 ROI、分区和位置跟随生成 HALCON Region。
- 进行阈值、形态学、屏蔽扣除、连通域、面积筛选。
- 输出候选、预处理图、调试日志、检测 Region。

禁止事项：

- 修界面绑定问题时不能改检测算法。
- 不允许把位置跟随后的旋转 Region 转回 Rectangle1 外接矩形。
- 不允许在检测层释放 `Params.ho_Image`。

### 4.4 调度层

文件：`Core\Pipeline\DefectDetectPipeline.cs`

职责：

- 从 `ParamsDefectCoat` 创建 `DefectDetectRequest`。
- 调用检测器。
- 调用特征值分类器、传统分类器、AI 分类器。
- 处理候选数量超限。
- 生成最终 `NgDefectResult`。

禁止事项：

- 不把界面状态放进 pipeline。
- 不在 pipeline 中修改 WPF 绑定集合。
- 不在分类阶段重新检测图像。

### 4.5 界面层

文件：

- `WPF架构\DefectCoatUser.xaml`
- `WPF架构\DefectCoatUser.xaml.cs`
- `Views\ClassifyResultWindow.xaml`
- `Views\ClassifyResultWindow.xaml.cs`

职责：

- 展示控件。
- 承载 `HWindowControl`。
- 转发生命周期方法到 ViewModel。
- 不写检测算法。

禁止事项：

- XAML 不绑定不存在的参数字段。
- 界面 code-behind 不直接修改检测结果。
- 关闭页面只释放显示资源，不释放宿主仍可能需要的运行输出。

### 4.6 ViewModel 层

文件：

- `ViewModels\DefectCoatViewModel.cs`
- `ViewModels\CoatRoiParamViewModel.cs`
- `ViewModels\CoatPartitionParamViewModel.cs`
- `ViewModels\BindableBase.cs`
- `ViewModels\RelayCommand.cs`
- `ViewModels\PartitionDirectionOption.cs`

职责：

- 连接界面控件和 `ParamsDefectCoat`。
- 将 UI 编辑集合回写到参数集合。
- 将参数集合重建为 UI 绑定集合。
- 负责 ViewROI 和当前编辑 ROI 的双向同步。
- 负责状态文本、调试日志、结果表刷新。

禁止事项：

- 不在 ViewModel 中复制一套独立检测参数后忘记回写。
- 不用中心宽高覆盖 Rectangle1 四坐标。
- 不在 ViewModel 中释放宿主输入图像，除非 `OwnsInputImage=true`。

### 4.7 HALCON 显示适配层

文件：

- `Services\HalconViewRoiAdapter.cs`
- `Services\HalconPreviewAdapter.cs`
- `Services\HalconMouseInfoArgs.cs`

职责：

- 封装 `HWindowControl`、`HWndCtrl`、`ROIController`。
- 主图显示 ViewROI 和检测叠加。
- 预览窗口显示单图和叠加区域。
- 对外提供鼠标信息。

禁止事项：

- 适配层只封装显示和 ROI 读写，不做检测。
- 适配层不保存参数 XML。
- 适配层不释放由外部传入的业务 HObject。

### 4.8 ViewROI 源码层

文件：`ViewROI\*.cs`

职责：

- 提供 HALCON ROI 交互基础能力。

维护要求：

- 原则上不修改行为。
- 允许补充中文注释解释字段、事件、坐标顺序和拖动机制。
- 如果必须改行为，需要单独说明原因，并对所有使用 ViewROI 的插件做回归。

## 5. ROI 开发 SOP

### 5.1 Rectangle1 坐标约定

Rectangle1 只有四个主参数：

```text
Row1, Col1, Row2, Col2
```

约定：

- `Row1` 和 `Row2` 运行前会归一化为上/下边界。
- `Col1` 和 `Col2` 运行前会归一化为左/右边界。
- 界面显示和保存以四坐标为准。
- 中心、宽、高只从四坐标派生。

### 5.2 局部 ROI

局部检测 ROI 存在：

```text
ParamsDefectCoat.LocalRoi.Row1
ParamsDefectCoat.LocalRoi.Col1
ParamsDefectCoat.LocalRoi.Row2
ParamsDefectCoat.LocalRoi.Col2
```

界面绑定必须通过 ViewModel 的当前编辑 ROI 代理属性，不直接绑定不存在的 `Params.Row1`。

### 5.3 屏蔽 ROI

屏蔽 ROI 存在：

```text
ParamsDefectCoat.MaskRois[index].Row1
ParamsDefectCoat.MaskRois[index].Col1
ParamsDefectCoat.MaskRois[index].Row2
ParamsDefectCoat.MaskRois[index].Col2
```

切换选中的屏蔽 ROI 时必须同步：

1. 当前编辑对象。
2. 参数面板四坐标。
3. 主图 ViewROI 框。
4. 屏蔽 ROI 列表行。

### 5.4 ViewROI 回写

ViewROI 拖动后必须读取 `ROIRectangle1.getModelData()` 的四个值，然后写回当前编辑 ROI。

正确链路：

```text
ViewROI 拖动
  -> HalconViewRoiAdapter.TryGetActiveRectangle1(...)
  -> DefectCoatViewModel.PullViewRoiToEditModel()
  -> 当前 LocalRoi 或 SelectedMaskRoi.Model
  -> RefreshEditRoiBindings()
```

错误链路：

```text
ViewROI 拖动
  -> 只写 RoiCenterRow / RoiCenterColumn / RoiWidth / RoiHeight
  -> 再用这些旧字段覆盖 Row1 / Col1 / Row2 / Col2
```

## 6. 分区开发 SOP

分区只能基于局部检测 ROI 的 Rectangle1 四坐标切分。

开发步骤：

1. 读取 `Params.LocalRoi`。
2. 归一化 `Row1/Col1/Row2/Col2`。
3. 根据 `PartitionDirection` 判断沿 Row 还是 Col 切分。
4. 读取每个 `CoatPartitionParam.Length`。
5. 按顺序生成分区 Rectangle1。
6. 每个分区运行时再进入 `DefectRoiFollowTransformer.CreateRegion()`。

禁止事项：

- 不用位置跟随后的旋转外接矩形反切分。
- 不在禁用分区时移动后续分区位置。
- 不把分区 UI 的毫米显示值直接当像素运行。

## 7. 检测开发 SOP

检测算法改动必须写明影响面。最小步骤：

1. 确认输入图像是否有效。
2. 确认灰度图生命周期。
3. 确认 ROI Region 是否正确释放。
4. 确认屏蔽 ROI 是否参与位置跟随。
5. 确认 `AppendCandidates` 是否仍在候选数量硬上限判断之后。
6. 确认输出 `DefectDetectResult` 中 HObject 归属清楚。

任何检测算法变更必须验证：

- 无图像时失败信息明确。
- 全局/局部 ROI 行为正确。
- 屏蔽 ROI 能扣除候选。
- 分区数量为 1 时等价单区域检测。
- 候选超限时不逐个裁图。

## 8. 分类开发 SOP

分类接入顺序：

1. 特征值分类。
2. 传统分类器。
3. AI 分类。

规则：

- 特征值分类只按需计算当前规则使用的特征。
- 传统分类器使用候选小图。
- AI 分类使用整图和候选列表。
- 分类规则明细独立保存，主参数只保存 `RuleConfigPath`。
- 修改规则后必须重新 `InitObj()`。

## 9. HObject 生命周期 SOP

输入：

- `Params.ho_Image` 是运行输入。
- 宿主传入图像时 `OwnsInputImage=false`。
- 插件 Demo 自己读图时才可以设置 `OwnsInputImage=true`。

输出：

- `Params.ReleaseOutputHObjects()` 只释放运行输出。
- `NgDefectResult.Dispose()` 只释放单个缺陷持有的 Region、小图和子区域。
- 界面窗口关闭只释放显示资源。

禁止事项：

- 不在预览窗口关闭时释放 `Params.ho_Image`。
- 不释放不归当前对象拥有的 `HObject`。
- 不把同一个 HObject 同时交给多个对象负责释放。

## 10. 注释规范

### 10.1 类注释

每个类必须说明：

- 该类属于哪一层。
- 负责什么。
- 不负责什么。
- 和哪些类有上下游关系。

### 10.2 字段和属性注释

每个字段和属性必须说明：

- 保存什么。
- 单位是什么。
- 是否参与 XML 保存。
- 是否为运行态对象。
- 是否拥有释放权。
- 是否是派生兼容字段。

### 10.3 方法注释

核心方法必须说明：

- 调用入口。
- 输入输出。
- 是否改变参数对象。
- 是否创建或释放 HObject。
- 为什么这样处理。

### 10.4 禁止的注释

禁止写无意义注释：

```csharp
// 给变量赋值
// 调用方法
// 返回结果
```

注释必须解释业务边界、生命周期、坐标含义、保存关系或风险点。

## 11. 构建 SOP

优先使用解决方案目标编译，避免依赖项目 `Any CPU/x64` 映射问题：

```powershell
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" `
  "D:\BaiduNetdiskDownload\宽幅打孔新方案1107\宽幅打孔新方案1017\0703计划做流程插件管理修改计划\ProcessWideHoleLaser\Mef_VisionFrame.sln" `
  /t:Plugin_DefectCoat `
  /p:Configuration=Release `
  /p:Platform=x64 `
  /m `
  /v:minimal
```

不推荐直接编译插件 csproj 的 `Release|x64`，因为部分依赖项目只配置了 `Any CPU` 输出路径，单独编译会报 `BaseOutputPath/OutputPath` 配置错误。

## 12. 提交前检查清单

提交或交付前必须检查：

- 是否只改了本次任务允许的文件。
- 是否误改 `Core\Pipeline` 或检测算法。
- 是否误改 `ViewROI` 行为。
- 是否误改 `Frame_Core` 框架公共逻辑。
- XAML 是否还绑定不存在的参数。
- Rectangle1 是否仍以四坐标为主。
- 编译是否通过。
- 运行 warning 是否为既有 warning。
- 文档路径是否使用当前工程真实路径。

## 13. 常见问题处理

### 13.1 ROI 参数不随拖动刷新

检查：

```text
HalconViewRoiAdapter.TryGetActiveRectangle1
DefectCoatViewModel.PullViewRoiToEditModel
DefectCoatViewModel.RefreshEditRoiBindings
DefectCoatUser.xaml 的 EditRoiRow1/Col1/Row2/Col2 绑定
```

### 13.2 屏蔽 ROI 切换后图上框没变

检查：

```text
SelectedMaskRoi setter
EditRole setter
SyncViewFromEditRoi
```

### 13.3 编译 csproj 报 OutputPath 错

使用解决方案目标编译，不直接编插件 csproj：

```text
/t:Plugin_DefectCoat /p:Configuration=Release /p:Platform=x64
```

### 13.4 运行后图像被释放

检查：

```text
OwnsInputImage
ReleaseOwnedInputImage
ReleaseOwnedRuntimeHObjects
CloseFromHost / BeforeClose
```

## 14. 严禁越界修改清单

除非明确需求要求，否则不要修改：

- `Core\Pipeline\DefectDetectPipeline.cs` 的分类调度顺序。
- `Core\Detection\HalconThresholdDefectDetector.cs` 的候选超限前置逻辑。
- `Core\Detection\DefectRoiFollowTransformer.cs` 的仿射位置跟随方式。
- `Models\DefectCoatObj.cs` 的宿主输入图像所有权规则。
- `Frame_Core` 中与本插件无关的框架服务。
- `ViewROI` 中鼠标拖动、缩放、ROI 控制行为。

