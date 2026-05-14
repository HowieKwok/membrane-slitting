# QR Board Mapping Tool

.NET Framework 4.7.2 WPF + HALCON 二维码棋盘格映射工具。

## 已实现

- 基于 `HSmartWindowControlWPF` 的通用 `HalconSmartViewer` 控件。
- 图像显示、适配窗口、鼠标拖拽平移、滚轮缩放、双击适配。
- 交互式 ROI：矩形、圆、多边形；多 ROI 会合并后用于识别。
- QR Code 识别，开放 `polarity`、`default_parameters`、`stop_after_result_num`。
- 棋盘格参数开放：行列数、X/Y 物理间距、物理原点、行/列方向反转。
- 支持已识别二维码参与映射，未识别或编号越界的点自动跳过。
- 二维码编号支持 `row-col`/`col-row` 双数字格式，也支持单序号格式。
- 支持仿射映射和透视映射，输出平均误差、RMS 误差、最大误差。
- 支持手动验证：输入像素坐标得到棋盘物理坐标，或反向输入物理坐标得到像素坐标。

## 主要文件

- `Controls/HalconSmartViewer.cs`：通用 HALCON SmartControl 封装。界面可直接用 `HalconSmartViewer`，已有 `HSmartWindowControlWPF` 时可直接传给 `HalconSmartControlController` 复用 ROI、显示、缩放、拖拽逻辑。
- `Services/QrBoardMapper.cs`：二维码识别、编号解析、点集生成、映射拟合入口。
- `Services/AffineBoardTransform.cs`：仿射/透视坐标转换和最小二乘拟合。
- `Models/BoardMappingModels.cs`：参数、点位结果、误差结果模型。
- `MainWindow.xaml` / `MainWindow.xaml.cs`：示例工具界面。

## 构建

使用 Visual Studio 2022 或 MSBuild 构建：

```bat
"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\amd64\MSBuild.exe" QrBoardMappingTool.csproj /t:Rebuild /p:Configuration=Release /p:Platform=AnyCPU
```

运行 `启动二维码棋盘映射工具.bat` 会设置默认 HALCON 环境变量并启动 `bin\Release\QrBoardMappingTool.exe`。
