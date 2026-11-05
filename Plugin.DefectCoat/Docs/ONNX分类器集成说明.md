# 传统分类器（ONNX）集成说明

## 1. 集成目标

把预留的“传统分类器”接口（`IModelDefectClassifier`）替换为真实 ONNX 推理：

- 分类器本身**不依赖 HALCON**（输入为 RGB 像素 `ModelImageInput`，预处理用 OpenCvSharp，与 Python 训练端一致）；
- 插件侧只做一件事：把检测裁出的 HALCON 小图 `candidate.CropImage` 转成 RGB 像素；
- 支持并行（多候选多线程）、多任务（多个插件对象实例共用同一模型互不干扰）。

## 2. 改动文件

| 文件 | 改动 |
| --- | --- |
| `DefectRuleClassifier\Core\Classifiers\OnnxModelClassifier.cs` | 新增：ONNX Runtime 推理器（每个实例独立会话） |
| `DefectRuleClassifier\Core\Classifiers\IModelDefectClassifier.cs` | 输入由 `DefectCandidate` 改为 `ModelImageInput`（RGB 像素） |
| `DefectRuleClassifier\Core\Models\ModelImageInput.cs` | 新增：与 HALCON 解耦的图像输入 |
| `DefectRuleClassifier\Core\Models\ModelLabelMapItem.cs` | 新增：序号→特征值标签 映射项 |
| `DefectRuleClassifier\Core\Models\NgDefectResult.cs` | 新增 `ClassIndex`（标签序号）、`Confidence` |
| `DefectRuleClassifier\Core\Classifiers\DefectClassifyDecision.cs` | 新增 `ClassIndex`、`Confidence` |
| `DefectRuleClassifier\Core\Classifiers\ModelClassifierConfig.cs` | 新增 `LabelsPath`、`IndexToFeatureLabel`（序号映射表）、`GetFeatureLabelByIndex` |
| `Plugin.DefectCoat\Core\Pipeline\DefectDetectPipeline.cs` | 模型分类并行化 + 序号映射 + 未匹配归入超限类别；新增 `BuildModelImageInput`（HALCON 小图→RGB） |
| `Plugin.DefectCoat\Models\DefectCoatObj.cs` | 占位替换为 `OnnxModelClassifier`，复制/释放新字段 |
| `DefectRuleClassifier\Views\RuleSettingsWindow.xaml` | “分类器分类”页：标签文件路径、序号→特征标签映射表、未匹配归入类别 |
| `Plugin.DefectCoat\Views\ClassifyResultWindow.xaml`、`WPF架构\DefectCoatUser.xaml` | 结果列表增加“标签序号”列 |
| `Plugin.DefectCoat\Plugin.DefectCoat.csproj` | 构建后自动复制 onnxruntime.dll / OpenCvSharpExtern.dll 到插件输出 |

## 3. 标签序号与映射（switch 语义）

- 模型输出序号 = `labels.txt` 行顺序（0 起）；
- 分类界面（“分类结果”窗口 / “输出结果”页）会显示“标签序号”；
- **序号绑定表**（与特征值分类参数解耦）：
  - “分类器工具”界面（规则设置 → 分类器分类 → 打开分类器工具）独立读取特征值分类
    规则文件中的标签（NgTypes 名称），用户把“模型序号 → 特征值标签”绑定并保存；
  - 绑定保存到分类器自己的配置 JSON（`IndexToFeatureLabel`），运行时只用自己的绑定表；
  - 判定规则：序号已绑定 → 输出绑定的特征值标签；未绑定（或绑定为空）→ 统一输出
    “未匹配归入类别”（默认 `涂布不良`）。
- **性能**：绑定表在加载/保存时预构建为数组缓存，推理时逐候选 **O(1) 查表**，
  不做逐候选遍历匹配。

## 4. 分类调度语义（各管各的）

- `EnableFeatureRuleClassify` 与 `EnableModelClassify` 是两个独立开关；
- 特征值分类关闭时，模型分类接管全部候选；
- 两者都开启时，特征值分类先按其规则分类，模型分类处理未被特征值命中的候选；
- AI 分类仍为整图预留接口。

## 5. 并行与多任务

- 每个 `DefectCoatObj` 持有独立的 `InferenceSession`：多个插件对象/模块即使指向同一 .onnx 也互不干扰；
- 单次分类内对多个候选使用 `Parallel.For` 并行推理（ONNX Runtime 会话线程安全）；
- 候选小图各自独立，转换与推理均无共享状态。

## 6. 内存与图像生命周期

- `candidate.CropImage` 归候选缺陷所有，插件转换时只读借用（若本身是 HImage 不释放；包装句柄用完即释放）；
- 分类结果 `NgDefectResult` 持有自己的 `CropImage` 副本，用于界面显示/后续模块读取；
- RGB `byte[]` 为转换时的临时拷贝，随调用结束可回收；OpenCV Mat 用 `using` 释放；
- 分类器只输出类别/序号/置信度/命中说明，不修改原图，也不生成新的图像文件；
- 显示用的“已分类/未分类/未达要求”区域由 `Params.ho_ClassifiedRegion` 等输出，宿主负责释放。

## 7. 部署

插件构建后自动把 `onnxruntime.dll`、`onnxruntime_providers_shared.dll`、`OpenCvSharpExtern.dll` 复制到
`Mef_VisionFrame\bin\Release\Plugins\`，无需手工部署原生库。

模型与标签文件路径在“分类规则设置 → 分类器分类”页配置：

- 分类器模型路径：`model.onnx`；
- 标签文件路径：`labels.txt`（顺序即序号）；
- 序号→特征标签：按实际模型类别填写；
- 未匹配归入类别：默认 `涂布不良`，需要时改为 `涂层不良`。

模型由 Python 训练端（PyTorch → ONNX）产出，推理端全程不需要 Python。

## 8. 分类结果窗口增强

“Show分类结果”窗口（ClassifyResultWindow）升级为完整 WPF 界面：

- 左侧结果表格：新增“标签序号”“置信度”列；
- 右侧“推理效果”预览：点击表格任意行，显示该缺陷小图，图上直接标注
  类别 / 标签序号 / 置信度，下方显示命中规则说明；
- 底部“续接训练”面板：填写数据集目录（默认 `E:\软件代码库\测试`）、轮数、
  勾选“续接上次训练”后点“开始续接训练”——后台隐藏进程运行
  `train_demo.py --resume --all-data`，日志实时显示，**不打开 Python 界面**；
- 训练完成后自动回调插件 `InitObj()` 重新加载新模型，继续推理即用新模型；
- 关闭窗口时若训练仍在进行会提示确认，避免后台任务被误终止。

数据集目录、模型输出目录、训练脚本路径由插件自动带出（模型路径所在目录即输出目录），
也可在窗口内直接修改。
