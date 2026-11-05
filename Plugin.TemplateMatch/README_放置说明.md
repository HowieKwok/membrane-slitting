# Plugin.TemplateMatch 草稿

这是只放在 test 下的可复制版本，不动正式源码。

## 结构
- `Plugin.TemplateMatch.cs`：插件入口，只有生命周期和参数初始化。
- `ViewModels/TemplateMatchViewModel.cs`：全部界面逻辑。
- `Core/TemplateMatchEngine.cs`：算法总入口，内部用策略模式切换形状/NCC。
- `Core/TemplateStorageService.cs`：模板保存、预加载、删除。
- `Controls/NumericBox.cs`：沿用 MeasureLine 的数值控件风格。
- `WPF架构/TemplateMatchUser.xaml`：空壳界面，只绑定。

## 模板路径
`ProcessPath/Plugin.TemplateMatch/模板名/`

目录里放：
- 模型文件
- 模板图片
- 预览图
- meta

删除模板时会连同模板图片一起删掉整个模板目录。

## 复制方式
1. 把 `test/Plugin.TemplateMatch` 整个文件夹复制到正式插件区。
2. 把 `test/Frame_Core/pluginParams/ParamsTemplateMatch.cs` 复制到正式 `Frame_Core/pluginParams/`，并在 `Frame_Core.csproj` 里加入编译项。
3. 把插件名、命名空间、导出名统一成 `Plugin.TemplateMatch`。
4. 把 `csproj` 的 HALCON 引用路径改成你本机实际路径。
5. 在宿主里把该插件按普通 `IPlugin` 加载。

## 注意
- 界面层不要再写算法。
- 算法执行期的临时 `HObject` 已在代码里标注释放点。
- 保存/删除模板都走 `TemplateStorageService`，不要散写到 ViewModel。
