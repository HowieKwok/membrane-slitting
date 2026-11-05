# Camera SDK 拆分开发说明手册

本文用于指导将 `Frame_Core\模块\外设部分\相机SDK` 从 `Frame_Core` 中拆出为独立相机 DLL，并保留现有流程注册、图像队列、回调、WinForms/WPF 界面打开方式。

## 1. 改造目标

- 相机硬件逻辑从 `Frame_Core` 中独立出去。
- `Frame_Core` 继续作为业务编排层，不破坏现有主架构。
- 相机 DLL 统一管理 `cameraManagers`，并对 `Frame_Core` 开放读取。
- 流程仍然能注册相机图像队列和回调。
- 相机界面放在相机 DLL 内部，支持 WinForms 和 WPF 两种顶层窗口。
- 不使用 MEF。
- 不做 WinForms/WPF 控件嵌套。
- 执行文件通过 `Project.Instance` 或 `Frame_Core` 暴露的入口打开相机界面。

## 2. 推荐项目结构

```text
Mef_VisionFrame.exe
  -> Frame_Core.dll
      -> Frame_Common.dll
      -> LogTool.dll
      -> CameraSdk.dll

CameraSdk.dll
  -> Frame_Common.dll
  -> LogTool.dll
  -> 相机厂商 SDK DLL
  -> Halcon DLL
```

建议新增或调整为以下项目：

```text
Frame_Common
  公共数据类型、公共枚举、公共基础类型。

LogTool
  保留现有 Log4netRecord、日志路径切换、日志对象配置。

CameraSdk
  相机服务、相机管理器、各厂商相机类、WinForms/WPF 相机界面。

Frame_Core
  Project、流程、业务编排、流程注册相机队列和回调。

Mef_VisionFrame
  执行文件，只负责菜单点击和调用 Frame_Core 入口。
```

## 3. 依赖原则

`Frame_Common` 是最底层公共库，谁都可以引用它，但它不能反向引用任何上层项目。

```text
Frame_Common 不引用 Frame_Core
Frame_Common 不引用 CameraSdk
Frame_Common 不引用 Mef_VisionFrame
Frame_Common 不引用具体窗体
```

`LogTool` 可以作为独立公共日志库存在。`Log4netRecord` 不建议放入 `Frame_Common`，因为它会把具体日志框架、配置、甚至 UI 弹窗依赖带入最底层公共库。

`CameraSdk` 可以引用 `Frame_Common` 和 `LogTool`，但不应引用 `Frame_Core`。如果相机 DLL 反向引用 `Frame_Core`，就会形成架构回绕。

`Frame_Core` 可以引用 `CameraSdk`，并在 `Project` 中 new 出相机服务对象。

## 4. 公共库 Frame_Common 放什么

第一批建议抽到 `Frame_Common` 的类型：

- `CameraEnum`
- `RotationAngleEnum`
- `CameraParameter`
- `ImageInfo`
- `OtherInfo`
- `CropImageInfo`
- `QueuePackage<T>`
- `ListPackage<T>`，如果相机和业务都使用
- `MyAttribute`
- `XmlSerializationOperation`
- `FileSettingsLog`
- `CameraInitializeOptions`
- `CameraObj` 内部的纯数据结构，如 `SurfacePoint`、`SurfacePointRGB`、`Cam_Calibration`

不建议放入 `Frame_Common` 的内容：

- `Project.Instance`
- 具体窗体
- `CameraManager` 这类具体相机管理服务
- 相机厂商 SDK 调用
- `MessageBox`
- `Log4netRecord`
- 业务流程对象
- 主程序 UI 逻辑

## 5. CameraSdk.dll 放什么

`CameraSdk.dll` 是相机模块的真正拥有者，负责相机全生命周期。

建议包含：

```text
CameraSdk
  Control
    CameraObj.cs
    CameraManager.cs
    CameraBasler.cs
    CameraDalsaCameraLink.cs
    CameraDalsaCameraLink2.cs
    CameraDalsaGigEVision.cs
    CameraFLY.cs
    CameraHK.cs
    CameraHKRGB.cs
    CameraHIK.cs
    CameraLMI.cs
    HKSDK.cs
    IKBoardSDK.cs
    IKCXPBoard.cs

  Services
    CameraService.cs
    CameraWindowLauncher.cs

  ViewModels
    CameraSettingViewModel.cs
    CameraParameterViewModel.cs

  Views.WinForms
    Cam_Form.cs
    Cam_Form.Designer.cs
    Cam_Form.resx

  Views.Wpf
    CameraSettingWindow.xaml
    CameraSettingWindow.xaml.cs
```

第一阶段可以做成一个 `CameraSdk.dll`。后续如果项目变大，再拆成：

```text
CameraSdk.Core.dll
CameraSdk.WinForms.dll
CameraSdk.Wpf.dll
```

## 6. CameraService 设计

`CameraService` 写在 `CameraSdk.dll` 中。它是相机模块的统一中转器。

第一阶段建议直接让 `Frame_Core` 持有 `CameraSdk.CameraService`，最少改动，最容易落地。

```csharp
public class CameraService
{
    private readonly Dictionary<CameraEnum, CameraManager> _cameraManagers
        = new Dictionary<CameraEnum, CameraManager>();

    public IReadOnlyDictionary<CameraEnum, CameraManager> CameraManagers
        => _cameraManagers;

    public void Initialize(CameraInitializeOptions options)
    {
        _cameraManagers.Clear();

        for (int i = 0; i < options.CameraCount; i++)
        {
            CameraEnum cam = (CameraEnum)i;
            string paramPath = options.GetCameraParamPath(cam);

            var manager = new CameraManager
            {
                CameraNameEnum = cam
            };

            manager.InitCameraParam(paramPath);
            manager.IniCameraSystem(i);

            _cameraManagers.Add(cam, manager);
        }
    }

    public void ShowWinForms()
    {
        using (var form = new Cam_Form(_cameraManagers))
        {
            form.StartPosition = FormStartPosition.CenterScreen;
            form.ShowDialog();
        }
    }

    public void ShowWpf()
    {
        var window = new CameraSettingWindow(_cameraManagers);
        window.ShowDialog();
    }

    public void Close()
    {
        foreach (CameraManager manager in _cameraManagers.Values)
        {
            manager.Close();
        }
    }
}
```

注意：`CameraManagers` 对外建议暴露为 `IReadOnlyDictionary`，避免外部清空或替换内部状态。初始化、关闭、重建由 `CameraService` 自己控制。

## 7. Frame_Core 中如何持有相机服务

`Frame_Core.Project` 中增加相机服务对象。

第一阶段实用写法：

```csharp
public CameraSdk.CameraService CameraService { get; private set; }
```

初始化时：

```csharp
CameraService = new CameraSdk.CameraService();

CameraService.Initialize(new CameraInitializeOptions
{
    CameraCount = cameraCount,
    ParamRootPath = paramPathSet.CutOverlayProjectBasePath
});
```

如果希望执行文件完全不知道 `CameraSdk.CameraService` 类型，可以在 `Project` 里再包一层方法：

```csharp
public void ShowCameraSetting()
{
    CameraService.ShowWinForms();
}
```

执行文件只调用：

```csharp
Project.Instance.ShowCameraSetting();
```

## 8. 流程如何注册相机队列和回调

流程注册必须保留。图像能进流程，核心就是把相机队列和回调注册给 `baseProDevice`。

原逻辑本质不变，只是相机对象来源从 `Project.Instance.cameraManager` 改成 `Project.Instance.CameraService.CameraManagers`。

```csharp
foreach (var item in Project.Instance.CameraService.CameraManagers.Keys)
{
    CameraManager manager = Project.Instance.CameraService.CameraManagers[item];
    CameraObj camera = manager.Camera;

    if (!camera.bIs3D)
    {
        baseProDevice.m_DicCamOperaParam[(int)item].QueueGrabImage =
            camera.QueueGrabImage;
    }
    else
    {
        baseProDevice.m_DicCamOperaParam[(int)item].Queue3DImage =
            camera.QueueGrab3DImage;
    }

    camera.CamCallbackEvent =
        baseProDevice.m_DicCamOperaParam[(int)item].CallBackImgDataDeal;
}
```

这段注册逻辑建议继续放在 `Frame_Core`。原因是 `baseProDevice` 是流程对象，属于业务编排层，不应该让 `CameraSdk.dll` 反向引用 `Frame_Core`。

## 9. 执行文件如何打开相机界面

当前 `Mef_VisionFrame\Form_Main.cs` 中相机菜单类似这样：

```csharp
using (Cam_Form cam_Form = new Cam_Form(Project.Instance.cameraManager))
{
    cam_Form.StartPosition = FormStartPosition.CenterScreen;
    cam_Form.ShowDialog();
}
```

改造后建议改为：

```csharp
private void 相机toolStripMenuItem1_Click(object sender, EventArgs e)
{
    if (!Project.Instance.purviewManage.GetWhetherPurview(
        Project.Instance.userManage.userData.CurrentUser,
        ModelName.相机设置))
    {
        return;
    }

    Project.Instance.CameraService.ShowWinForms();
}
```

如果要切换 WPF：

```csharp
Project.Instance.CameraService.ShowWpf();
```

如果希望执行文件完全不引用 `CameraSdk.dll`，使用 `Project` 包装方法：

```csharp
Project.Instance.ShowCameraSetting();
```

## 10. WinForms/WPF 界面策略

界面放在 `CameraSdk.dll` 内部是可行的。WinForms 和 WPF 都作为顶层窗口打开，不互相嵌套。

WinForms 版本：

- 保留旧 `Cam_Form`
- 继续使用原有 DataGridView 参数编辑方式
- 代码逐步改为调用 `CameraService` 和 ViewModel

WPF 版本：

- 新建 `CameraSettingWindow`
- 使用 `CameraSettingViewModel`
- `CameraParameterViewModel` 专门包装旧 `CameraParameter`
- UI 只做绑定和显示，不直接做硬件逻辑

WPF 绑定示例：

```csharp
public class CameraParameterViewModel : INotifyPropertyChanged
{
    private readonly CameraParameter _model;

    public CameraParameterViewModel(CameraParameter model)
    {
        _model = model;
    }

    public string CameraName
    {
        get { return _model.CameraName; }
        set
        {
            if (_model.CameraName == value) return;
            _model.CameraName = value;
            OnPropertyChanged(nameof(CameraName));
        }
    }

    public void Save()
    {
        _model.Save();
    }
}
```

## 11. 日志策略

保留现有 `LogTool.Log4netRecord`。

建议：

- `Log4netRecord` 留在 `LogTool.dll`
- `Frame_Common` 不引用 `log4net.dll`
- `CameraSdk.dll` 可以引用 `LogTool.dll`
- 相机日志继续写 `Log4netRecord.CamLog`
- 错误日志继续写 `Log4netRecord.ErrorLog`

示例：

```csharp
Log4netRecord.CamLog.Info("相机初始化成功");
Log4netRecord.ErrorLog.Error(ex);
```

## 12. 相机 DLL 依赖清单

`CameraSdk.dll` 需要引用的相机相关 DLL：

```text
Basler.Pylon.dll
DALSA.SaperaLT.SapClassBasic.dll
FlyCapture2Managed_v100.dll
MvCameraControl.Net.dll
MvFGCtrlC.Net.dll
IKapBoardClassLibrary.dll
IKapC.NET.dll
GoSdkNet.dll
GoSdk.dll
kApiNet.dll
kApi.dll
halcondotnet.dll
halcondotnetxl.dll
halcon.dll
halconxl.dll
halconc.dll
```

`log4net.dll` 和 `NLog.dll` 不作为相机核心必需项列入相机 SDK 清单。它们属于日志库依赖，由 `LogTool.dll` 管理。

所有运行时需要的 DLL 最终应出现在：

```text
Mef_VisionFrame\bin\Debug
Mef_VisionFrame\bin\Release
```

## 13. 迁移步骤

第一步，新增 `Frame_Common` 项目。

第二步，移动公共类型到 `Frame_Common`。

第三步，修正 `Frame_Core` 对公共类型的 using 和引用。

第四步，新增 `CameraSdk` 项目。

第五步，把相机 `Control` 目录中的设备类移动到 `CameraSdk`。

第六步，把 `Camera_Parameter.cs` 改为引用 `Frame_Common` 中的公共基础类型。

第七步，把 `Cam_Form` 移动到 `CameraSdk` 的 WinForms 目录。

第八步，在 `CameraSdk` 中新增 `CameraService`。

第九步，在 `Frame_Core.Project` 中 new 出 `CameraService` 并调用 `Initialize`。

第十步，把流程注册代码改为从 `Project.Instance.CameraService.CameraManagers` 获取相机。

第十一步，把执行文件中打开相机窗体的代码改为 `Project.Instance.CameraService.ShowWinForms()` 或 `Project.Instance.ShowCameraSetting()`。

第十二步，再新增 WPF 界面和 ViewModel。

## 14. 注意事项

不要同时让 `Frame_Core` 持有旧 `cameraManager`，又让 `CameraSdk` 持有新 `cameraManagers`。相机状态只能有一个所有者。

第一阶段建议让 `CameraSdk.CameraService` 成为相机状态唯一所有者。

`Frame_Core` 可以读 `CameraManagers` 注册流程，但不负责清空、重建、销毁字典。

`CameraSdk` 不要引用 `Frame_Core`，否则会形成依赖回绕。

`Frame_Common` 不要放窗体、日志实现、相机厂商 SDK 调用。

执行文件可以直接引用 `Frame_Common`。这不影响架构。

如果执行文件直接访问 `Project.Instance.CameraService`，可能需要引用 `CameraSdk.dll`。如果不想让执行文件引用 `CameraSdk.dll`，就在 `Project` 里增加包装方法，例如 `ShowCameraSetting()`。

## 15. 最终调用链

初始化：

```text
Project.Instance
  -> new CameraSdk.CameraService()
  -> CameraService.Initialize(...)
```

流程注册：

```text
Frame_Core
  -> Project.Instance.CameraService.CameraManagers
  -> 注册 QueueGrabImage / Queue3DImage
  -> 注册 CamCallbackEvent
```

打开界面：

```text
Mef_VisionFrame 菜单点击
  -> Project.Instance.CameraService.ShowWinForms()
  或 Project.Instance.CameraService.ShowWpf()
```

关闭：

```text
Project.Instance.CameraService.Close()
```

