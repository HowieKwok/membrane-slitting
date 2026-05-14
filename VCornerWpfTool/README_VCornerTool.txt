VCorner WPF Tool
================

Run:
  Double-click:
  启动V角检测工具.bat

Published EXE:
  VCornerWpfTool\ConvexPeakWpfDemo.exe

Source code folder:
  ConvexPeakWpfDemo

User manual:
  V角检测工具使用说明书.md

Main source files:
  ConvexPeakWpfDemo\MainWindow.xaml
  ConvexPeakWpfDemo\MainWindow.xaml.cs
  ConvexPeakWpfDemo\HalconVCornerDetector.cs
  ConvexPeakWpfDemo\App.xaml.cs

Open source folder:
  打开V角检测源码目录.bat

Open publish folder:
  打开V角工具发布目录.bat

If the tool does not start, check:
  VCornerWpfTool_run.log

Self test:
  cd /d VCornerWpfTool
  ConvexPeakWpfDemo.exe --self-test

Self-test output:
  VCornerWpfTool\ConvexPeakWpfDemo_self_test_result.txt

View operation:
  鼠标滚轮：放大 / 缩小
  右键或中键拖动：平移图像
  左键拖拽：生成当前检测 ROI
  适应窗口：恢复整图显示
  1:1：按原图像素显示
  左下角与底部状态栏会显示当前鼠标 Row、Col、Gray、Zoom

ROI / template:
  手动 ROI 检测：取消勾选“检测前先模板匹配定位 ROI”，拖 ROI 后点“应用 ROI”，再点“检测当前”。
  批量自动定位：先在 OK 图上拖 ROI，点“ROI生成模板”，再勾选模板匹配并执行批量仿真。

Distance output:
  基准线来自 ROI 内主轮廓的最长直线段。
  DistanceX = P1->P2 向量在基准线平行方向上的投影。
  DistanceY = P1->P2 向量在基准线垂直方向上的投影。
  DistanceTotal = sqrt(DistanceX^2 + DistanceY^2)。
  像素当量 X/Y 会先把 Row/Col 转成物理坐标，再做平行/垂直投影。

Parameter debug order:
  1. ROI 先框准，只包含 V 角和两侧基准边，少放背景和无关边。
  2. 主轮廓断裂：降低 Canny Low/High，或增大“轮廓拼接”距离。
  3. 噪声轮廓太多：提高 Canny High，或增大“最小轮廓长度”。
  4. 有蓝色分段但无交点：调小“直线误差 1/2”，放宽“最小圆弧长度/半径范围”。
  5. 圆弧优先找不到：降低“曲率角阈值”或“最小连续点数”；误检则反向调高。
