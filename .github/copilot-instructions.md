# Copilot Instructions

## 项目指南
- 用户已调整方案：不生成单个独立 .cs，而是创建一个独立 Avalonia 项目，项目结构参考 Fluxen.DemoModules；项目引用 Fluxen.Tools.*、Fluxen.Abstractions、Fluxen.UI.Abstractions 及 HalconDotNet；工具名确定为 LineGeometryTool，定位为通用线工具，后续可扩展圆工具、矩形工具；首个 FluxenTool 命令为两线交点，并生成该命令绑定的 Avalonia 属性设置窗口。