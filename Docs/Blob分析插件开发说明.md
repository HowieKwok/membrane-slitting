# Blob 分析插件开发说明

## 1. 目标

新增一个 Blob 分析插件 DLL，用于对极耳、有无料、异常区域、脏污、破损等区域做连通域分析。

插件要符合现有软件框架：

- 运行入口：`BlobAnalysisObj : IPlugin`
- 参数类：`Frame_Core.pluginParams.ParamsBlobAnalysis : PluginParamsBase`
- 界面：WPF，参照 `Plugin.CACRunCalibration\WPF架构`
- 算法：HALCON 区域处理，算法类只放在 `Plugin.BlobAnalysis` DLL 内部，不放 `Frame_Core`
- 参数保存：走现有 `PluginParams.Save()`
- 输出结果：写回 `PluginParams`，流程运行时直接读取

后续创建插件目录建议：

```text
Plugin.BlobAnalysis
├─ BlobAnalysisObj.cs                         // IPlugin 运行入口，只做框架适配
├─ Application
│  ├─ BlobAnalysisService.cs                  // 应用服务，负责调度算法管线并回写参数
│  └─ BlobAnalysisContext.cs                  // 单次运行上下文，保存输入、临时对象、输出
├─ Domain
│  ├─ BlobAnalysisResult.cs                   // DLL 内部算法结果模型
│  ├─ BlobFeature.cs                          // 单个 Blob 特征模型
│  └─ BlobFeatureName.cs                      // 支持的过滤属性常量/枚举
├─ Algorithms
│  ├─ Pipeline
│  │  ├─ IBlobAnalysisPipeline.cs
│  │  └─ BlobAnalysisPipeline.cs              // 串联分割、形态学、过滤、特征提取
│  ├─ Segmentation
│  │  ├─ IBlobSegmenter.cs
│  │  └─ ThresholdBlobSegmenter.cs            // threshold 分割策略
│  ├─ Morphology
│  │  ├─ IBlobMorphologyProcessor.cs
│  │  └─ HalconMorphologyProcessor.cs         // opening/closing/fill_up
│  ├─ Filtering
│  │  ├─ IBlobRegionFilter.cs
│  │  └─ SelectShapeBlobRegionFilter.cs       // select_shape 多条件过滤
│  ├─ Features
│  │  ├─ IBlobFeatureExtractor.cs
│  │  └─ HalconBlobFeatureExtractor.cs        // 逐 Region 输出特征
│  └─ Roi
│     ├─ IBlobRoiBuilder.cs
│     └─ BlobRoiBuilder.cs                    // 全局/局部/屏蔽 ROI 统一处理
├─ Converters
│  ├─ BlobParamMapper.cs                      // 公共参数 <-> DLL 内部模型转换
│  └─ BlobFeatureTextConverter.cs             // WPF 显示用转换器
├─ ViewModels
│  ├─ BlobAnalysisViewModel.cs
│  ├─ BlobFilterItemViewModel.cs
│  └─ BlobResultItemViewModel.cs
├─ WPF架构
│  ├─ WPFBlobAnalysis.xaml
│  ├─ WPFBlobAnalysis.xaml.cs
│  └─ WPFBlobAnalysisHost.cs
└─ Properties
   └─ AssemblyInfo.cs
```

参数公共类不要散放在 `pluginParams` 根目录，后续统一新增文件夹归类：

```text
Frame_Core\pluginParams\BlobAnalysis
├─ ParamsBlobAnalysis.cs       // 主参数类，继承 PluginParamsBase
├─ BlobFilterParam.cs          // select_shape 多条件过滤参数
└─ BlobResultParam.cs          // 多 Region 逐个输出的特征结果
```

说明：

- 文件夹用于归类，避免后续插件越来越多时参数类散乱。
- 命名空间仍建议保持 `Frame_Core.pluginParams`，这样不破坏现有框架反序列化和引用方式。
- 如果后续框架允许子命名空间，也可以用 `Frame_Core.pluginParams.BlobAnalysis`，但需要确认 XML 兼容性后再改。
- `Frame_Core` 只放公共参数类和通用基础类，不能放 `BlobAnalysisService / BlobAnalysisPipeline / 算法策略` 这类插件业务代码。

## 2. 分层架构和设计模式

Blob 插件不能写成一个大类堆所有逻辑。后续 DLL 按下面分层：

| 层 | 目录 | 职责 |
| --- | --- | --- |
| 框架入口层 | `BlobAnalysisObj.cs` | 实现 `IPlugin`，只负责 `IniObj / ExecuteObj / saveParams / DisPoseObj` |
| 应用服务层 | `Application` | 组织一次运行，调用算法管线，做参数和结果回写 |
| 领域模型层 | `Domain` | DLL 内部算法模型，不依赖 WPF，不依赖框架显示 |
| 算法层 | `Algorithms` | HALCON 分割、ROI、过滤、特征提取策略 |
| 参数映射层 | `Converters` | 公共参数类和 DLL 内部模型互转 |
| WPF 层 | `WPF架构 / ViewModels` | 界面、ViewROI、绑定、命令、预览 |

使用的设计模式：

| 模式 | 用在哪里 | 目的 |
| --- | --- | --- |
| Facade 外观 | `BlobAnalysisService` | 对 `BlobAnalysisObj` 暴露简单 `Run(param)` 调用 |
| Pipeline 管线 | `BlobAnalysisPipeline` | 串联 ROI、分割、形态学、过滤、特征提取 |
| Strategy 策略 | `IBlobSegmenter / IBlobRegionFilter / IBlobFeatureExtractor` | 后续替换阈值方式、过滤方式、特征提取方式不影响入口 |
| Mapper 映射 | `BlobParamMapper` | 公共参数类和 DLL 内部模型互转，降低耦合 |
| MVVM | `BlobAnalysisViewModel` | WPF 绑定和命令管理，不污染公共参数类 |
| Adapter 适配 | ViewROI 调用封装 | ViewROI 只在界面层适配，算法层只读 ROI 坐标 |
| Disposable/Guard | HALCON 临时对象释放 | 集中释放 HObject，避免内存泄漏 |

依赖方向必须固定：

```text
WPF/ViewModel -> Application -> Algorithms -> HalconDotNet
BlobAnalysisObj -> Application
Application -> Frame_Core.pluginParams.BlobAnalysis
Frame_Core 不依赖 Plugin.BlobAnalysis
Algorithms 不依赖 WPF/ViewROI
```

### 核心调用链

```text
流程触发 ExecuteObj()
  -> BlobAnalysisObj.ExecuteObj()
  -> BlobAnalysisService.Run(ParamsBlobAnalysis)
  -> BlobParamMapper.FromParams(...)
  -> BlobAnalysisPipeline.Run(context)
       -> BlobRoiBuilder.Build(...)
       -> ThresholdBlobSegmenter.Segment(...)
       -> HalconMorphologyProcessor.Process(...)
       -> SelectShapeBlobRegionFilter.Filter(...)
       -> HalconBlobFeatureExtractor.Extract(...)
  -> BlobParamMapper.WriteBack(...)
  -> 下游流程读取 ParamsBlobAnalysis 输出
```

### 类职责说明

#### BlobAnalysisObj

只做框架入口，不写算法细节。

```csharp
public bool ExecuteObj()
{
    ParamsBlobAnalysis p = PluginParams as ParamsBlobAnalysis;
    return service.Run(p);
}
```

#### BlobAnalysisService

应用服务，负责一次运行的整体调度：

- 检查参数。
- 调 mapper 转成内部 context。
- 调 pipeline。
- 把结果写回 `ParamsBlobAnalysis`。
- 记录日志和异常。

#### BlobAnalysisPipeline

算法管线，只负责串流程，不写具体策略细节：

```csharp
public BlobAnalysisResult Run(BlobAnalysisContext context)
{
    roiBuilder.Build(context);
    segmenter.Segment(context);
    morphologyProcessor.Process(context);
    regionFilter.Filter(context);
    featureExtractor.Extract(context);
    return context.Result;
}
```

#### SelectShapeBlobRegionFilter

只负责多条件 `select_shape`：

- 读取 `context.FilterParams`。
- 组装 `Features / Operation / Min / Max`。
- 调 `HOperatorSet.SelectShape`。

#### HalconBlobFeatureExtractor

只负责逐 Region 特征输出：

- `AreaCenter`
- `SmallestRectangle2`
- `EllipticAxis`
- `Circularity`
- `Rectangularity`
- `Intensity`
- `GrayFeatures`

#### BlobParamMapper

只负责转换：

- `ParamsBlobAnalysis -> BlobAnalysisContext`
- `BlobAnalysisResult -> ParamsBlobAnalysis`
- `BlobResultParam <-> BlobResultItemViewModel`

不能在 mapper 里写 HALCON 算法。
## 3. HALCON 算法流程

基础流程：

```text
输入图像 ho_Image
  -> 可选局部 ROI / 屏蔽 ROI
  -> 灰度阈值 threshold
  -> 可选形态学处理 opening / closing / fill_up
  -> connection 连通域
  -> select_shape 多条件过滤
  -> sort_region 排序
  -> area_center / elliptic_axis / orientation_region / circularity / rectangularity / smallest_rectangle2 / intensity / gray_features
  -> 对过滤后的多个 Region 逐个输出特征列表和显示 Region
```

主要 HALCON 算子：

| 功能 | HALCON 算子 | 说明 |
| --- | --- | --- |
| 阈值分割 | `Threshold` | 按灰度范围提取区域 |
| 连通域 | `Connection` | 把区域拆成独立 Blob |
| 多条件筛选 | `SelectShape` | 按面积、圆形度、矩形度、宽高等过滤 |
| 面积和质心 | `AreaCenter` | 输出 `Area / Row / Column` |
| 长短轴和方向 | `EllipticAxis` | 输出 `Ra / Rb / Phi` |
| 区域方向 | `OrientationRegion` | 输出主方向角 |
| 圆形度 | `Circularity` | 输出圆形度 |
| 矩形度 | `Rectangularity` | 输出矩形度 |
| 最小外接矩形 | SmallestRectangle2 | 输出中心、角度、长短边 |
| 灰度特征 | Intensity / GrayFeatures | 基于原图和 Region 输出平均灰度、灰度偏差、最小灰度、最大灰度 |
| 排序 | `SortRegion` | 按面积、行、列等排序 |
| 计数 | `CountObj` | 输出 Blob 个数 |

## 4. 多条件过滤设计

这里不能只写死面积过滤，要设计成“过滤条件列表”。

界面提供 `添加过滤条件 / 删除过滤条件`，每条过滤条件包含：

- 是否启用
- HALCON 属性
- 最小值
- 最大值
- 过滤逻辑

参数类建议：

```csharp
[Serializable]
public class BlobFilterParam : XmlSerializationOperation
{
    [ShowTextAttribute("启用")]
    [Description("是否启用当前过滤条件")]
    public bool m_bEnable = true;

    [ShowTextAttribute("过滤属性")]
    [Description("select_shape 的 Features 参数")]
    public string m_strFeature = "area";

    [ShowTextAttribute("最小值")]
    [Description("当前属性过滤下限")]
    public double m_fMin = 0.0;

    [ShowTextAttribute("最大值")]
    [Description("当前属性过滤上限")]
    public double m_fMax = 999999999.0;

    [ShowTextAttribute("过滤方式")]
    [Description("and/or，默认 and")]
    public string m_strOperation = "and";
}
```

界面 ComboBox 的 `m_strFeature` 选项建议先覆盖常用 HALCON `select_shape` 属性：

| 显示文本 | HALCON Feature | 用途 |
| --- | --- | --- |
| 面积 | `area` | Blob 像素面积 |
| 质心Row | `row` | 质心行坐标 |
| 质心Col | `column` | 质心列坐标 |
| 宽度 | `width` | 外接框宽度 |
| 高度 | `height` | 外接框高度 |
| 圆形度 | `circularity` | 圆形目标过滤 |
| 矩形度 | `rectangularity` | 方形/矩形目标过滤 |
| 紧凑度 | `compactness` | 区域紧凑程度 |
| 凸性 | `convexity` | 区域凸性 |
| 外接矩形Row | `rect2_row` | 最小外接矩形中心 Row |
| 外接矩形Col | `rect2_col` | 最小外接矩形中心 Col |
| 外接矩形角度 | `rect2_phi` | 最小外接矩形角度 |
| 外接矩形长边 | `rect2_len1` | 最小外接矩形半长轴 |
| 外接矩形短边 | `rect2_len2` | 最小外接矩形半短轴 |
| 椭圆长轴 | `ra` | 等效椭圆长轴 |
| 椭圆短轴 | `rb` | 等效椭圆短轴 |
| 区域角度 | `phi` | 区域方向角 |

执行方式：

```csharp
List<BlobFilterParam> filters = p.m_lFilterParams.Where(x => x.m_bEnable).ToList();

HTuple features = new HTuple();
HTuple operations = new HTuple();
HTuple mins = new HTuple();
HTuple maxs = new HTuple();

foreach (BlobFilterParam f in filters)
{
    features = features.TupleConcat(f.m_strFeature);
    operations = operations.TupleConcat(f.m_strOperation);
    mins = mins.TupleConcat(f.m_fMin);
    maxs = maxs.TupleConcat(f.m_fMax);
}

HOperatorSet.SelectShape(connectedRegions, out selectedRegions, features, operations, mins, maxs);
```

说明：

- 如果所有条件都是 `and`，代表所有条件同时满足。
- 如果需要更复杂组合，第一版先限制为全 `and`，界面保留 `Operation` 字段。
- `select_shape` 支持一次传入多个 `Features / Operation / Min / Max`。
- 如果后续要做“面积满足或圆度满足”这种组合，可以拆成多次 `SelectShape` 后用 `Union2` 合并。

## 5. 参数类设计

```csharp
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using YYSD.UICore.Control;

namespace Frame_Core.pluginParams
{
    [Serializable]
    public class ParamsBlobAnalysis : PluginParamsBase
    {
        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject ho_Image;

        [ShowTextAttribute("启用检测")]
        [Description("是否启用 Blob 分析")]
        public bool m_bIsEnable = true;

        [ShowTextAttribute("阈值最小值")]
        [Description("Threshold 最小灰度")]
        public double m_fThresholdMin = 0;

        [ShowTextAttribute("阈值最大值")]
        [Description("Threshold 最大灰度")]
        public double m_fThresholdMax = 255;

        [ShowTextAttribute("启用局部ROI")]
        [Description("是否只在局部 ROI 内检测")]
        public bool m_bUseLocalRoi = false;

        [ShowTextAttribute("局部ROI Row1")]
        public double m_fLocalRoiRow1 = 0;

        [ShowTextAttribute("局部ROI Col1")]
        public double m_fLocalRoiCol1 = 0;

        [ShowTextAttribute("局部ROI Row2")]
        public double m_fLocalRoiRow2 = 1000;

        [ShowTextAttribute("局部ROI Col2")]
        public double m_fLocalRoiCol2 = 1000;

        [ShowTextAttribute("启用屏蔽ROI")]
        [Description("是否屏蔽不需要检测的区域")]
        public bool m_bUseIgnoreRoi = false;

        [ShowTextAttribute("形态学方式")]
        [Description("None/Open/Close/FillUp/OpenClose/CloseOpen")]
        public string m_strMorphologyMode = "None";

        [ShowTextAttribute("形态学半径")]
        public double m_fMorphologyRadius = 3.0;

        [ShowTextAttribute("排序方式")]
        [Description("area/row/column")]
        public string m_strSortFeature = "area";

        [ShowTextAttribute("排序方向")]
        [Description("true=升序，false=降序")]
        public bool m_bSortAscending = false;

        [ShowTextAttribute("过滤条件组")]
        [Description("select_shape 多条件过滤参数")]
        public List<BlobFilterParam> m_lFilterParams = new List<BlobFilterParam>();

        [ShowTextAttribute("最大输出数量")]
        [Description("输出前 N 个 Blob，0 表示全部输出")]
        public int m_nMaxOutputCount = 0;

        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public bool m_bBlobSuccess = false;

        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public int m_nBlobCount = 0;

        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public int m_nElapsedMs = 0;

        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject m_ho_OutRegion;

        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject m_ho_OutSelectedRegion;

        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public HObject m_ho_OutImgShow;

        [NonSerialized]
        [System.Xml.Serialization.XmlIgnore]
        public List<BlobResultParam> m_lBlobResults = new List<BlobResultParam>();
    }
}
```

Blob 输出结果建议单独定义：

```csharp
[Serializable]
public class BlobResultParam : XmlSerializationOperation
{
    public int m_nIndex = 0;
    public double m_fArea = 0;
    public double m_fCenterRow = 0;
    public double m_fCenterCol = 0;
    public double m_fRa = 0;
    public double m_fRb = 0;
    public double m_fPhi = 0;
    public double m_fAngleDeg = 0;
    public double m_fCircularity = 0;
    public double m_fRectangularity = 0;
    public double m_fRect2Row = 0;
    public double m_fRect2Col = 0;
    public double m_fRect2Phi = 0;
    public double m_fRect2Length1 = 0;
    public double m_fRect2Length2 = 0;
}
```

## 6. 输出字段

必须输出：

| 输出 | 字段建议 | 说明 |
| --- | --- | --- |
| 检测成功 | `m_bBlobSuccess` | 有符合条件 Blob 为 true |
| Blob 个数 | `m_nBlobCount` | 过滤后区域数量 |
| 检测耗时 | `m_nElapsedMs` | ms |
| 输出区域 | `m_ho_OutSelectedRegion` | 过滤后的多个 Region，可用于界面显示 |
| 显示图像 | `m_ho_OutImgShow` | 原图或局部检测图 |
| 结果列表 | `m_lBlobResults` | 多个 Region 的逐个特征输出 |
| 最大面积 | `m_fMaxArea` | 可选，方便流程直接判断 |
| 最大面积质心Row | `m_fMaxAreaRow` | 可选 |
| 最大面积质心Col | `m_fMaxAreaCol` | 可选 |

核心要求：过滤后不是只输出一个合并后的总区域，而是对每一个输出 Region 单独计算特征。过滤后有几个 Region，`m_lBlobResults` 就有几条记录。

每个 Region 输出建议：

| 特征 | 字段建议 | HALCON 来源 | 说明 |
| --- | --- | --- | --- |
| 序号 | `m_nIndex` | `SelectObj` 循环序号 | 第几个 Blob |
| 面积 | `m_fArea` | `AreaCenter` | 像素面积 |
| 质心Row | `m_fCenterRow` | `AreaCenter` | 区域质心 Row |
| 质心Col | `m_fCenterCol` | `AreaCenter` | 区域质心 Col |
| 中心Row | `m_fRect2Row` | `SmallestRectangle2` | 最小外接矩形中心 Row |
| 中心Col | `m_fRect2Col` | `SmallestRectangle2` | 最小外接矩形中心 Col |
| 长轴 | `m_fRa` | `EllipticAxis` | 等效椭圆长轴 |
| 短轴 | `m_fRb` | `EllipticAxis` | 等效椭圆短轴 |
| 长边 | `m_fLength1` | `SmallestRectangle2` | 外接矩形半长边，可按需要乘 2 输出全长 |
| 短边 | `m_fLength2` | `SmallestRectangle2` | 外接矩形半短边，可按需要乘 2 输出全宽 |
| 长宽比 | `m_fAspectRatio` | `Length1 / Length2` | 防止除 0，短边为 0 时输出 0 |
| 区域角度 | `m_fPhi` | `EllipticAxis` 或 `OrientationRegion` | 弧度 |
| 角度Deg | `m_fAngleDeg` | `Phi * 180 / PI` | 角度制，方便界面和流程查看 |
| 圆度 | `m_fCircularity` | `Circularity` | 越接近 1 越圆 |
| 矩形度 | `m_fRectangularity` | `Rectangularity` | 越接近 1 越像矩形 |
| 平均灰度 | `m_fMeanGray` | `Intensity` | 需要传入原图和当前 Region |
| 灰度偏差 | `m_fDeviationGray` | `Intensity` | 灰度标准差/偏差 |
| 最小灰度 | `m_fMinGray` | `GrayFeatures` | 可选 |
| 最大灰度 | `m_fMaxGray` | `GrayFeatures` | 可选 |

`BlobResultParam` 建议：

```csharp
[Serializable]
public class BlobResultParam : XmlSerializationOperation
{
    public int m_nIndex = 0;
    public double m_fArea = 0;
    public double m_fCenterRow = 0;
    public double m_fCenterCol = 0;
    public double m_fRect2Row = 0;
    public double m_fRect2Col = 0;
    public double m_fRa = 0;
    public double m_fRb = 0;
    public double m_fLength1 = 0;
    public double m_fLength2 = 0;
    public double m_fAspectRatio = 0;
    public double m_fPhi = 0;
    public double m_fAngleDeg = 0;
    public double m_fCircularity = 0;
    public double m_fRectangularity = 0;
    public double m_fMeanGray = 0;
    public double m_fDeviationGray = 0;
    public double m_fMinGray = 0;
    public double m_fMaxGray = 0;
}
```

逐 Region 计算示例：

```csharp
HOperatorSet.CountObj(selectedRegions, out HTuple count);
for (int i = 1; i <= count.I; i++)
{
    HOperatorSet.SelectObj(selectedRegions, out HObject singleRegion, i);

    HOperatorSet.AreaCenter(singleRegion, out HTuple area, out HTuple row, out HTuple col);
    HOperatorSet.EllipticAxis(singleRegion, out HTuple ra, out HTuple rb, out HTuple phi);
    HOperatorSet.SmallestRectangle2(singleRegion, out HTuple rectRow, out HTuple rectCol, out HTuple rectPhi, out HTuple len1, out HTuple len2);
    HOperatorSet.Circularity(singleRegion, out HTuple circularity);
    HOperatorSet.Rectangularity(singleRegion, out HTuple rectangularity);

    // 灰度特征必须使用原图，Region 本身没有灰度。
    HOperatorSet.Intensity(singleRegion, p.ho_Image, out HTuple meanGray, out HTuple deviationGray);
    HOperatorSet.GrayFeatures(singleRegion, p.ho_Image, "min", out HTuple minGray);
    HOperatorSet.GrayFeatures(singleRegion, p.ho_Image, "max", out HTuple maxGray);

    double aspectRatio = len2.D == 0 ? 0 : len1.D / len2.D;

    p.m_lBlobResults.Add(new BlobResultParam
    {
        m_nIndex = i,
        m_fArea = area.D,
        m_fCenterRow = row.D,
        m_fCenterCol = col.D,
        m_fRect2Row = rectRow.D,
        m_fRect2Col = rectCol.D,
        m_fRa = ra.D,
        m_fRb = rb.D,
        m_fLength1 = len1.D,
        m_fLength2 = len2.D,
        m_fAspectRatio = aspectRatio,
        m_fPhi = phi.D,
        m_fAngleDeg = phi.D * 180.0 / Math.PI,
        m_fCircularity = circularity.D,
        m_fRectangularity = rectangularity.D,
        m_fMeanGray = meanGray.D,
        m_fDeviationGray = deviationGray.D,
        m_fMinGray = minGray.D,
        m_fMaxGray = maxGray.D
    });

    singleRegion.Dispose();
}
```

注意：

- `m_ho_OutSelectedRegion` 保存过滤后的多个 Region，用于显示。
- `m_lBlobResults` 保存逐个 Region 的数值特征，用于流程判断和界面表格显示。
- 灰度特征必须基于 `p.ho_Image` 和当前 `singleRegion` 计算，不能只对 Region 算。
- 如果启用了局部 ROI，灰度仍然用原图同坐标区域，不要对裁剪后坐标造成偏移。
## 7. BlobAnalysisObj 运行入口

运行对象按现有框架写：

```csharp
[Category("区域分析")]
[DisplayName("Blob分析工具")]
[Export("BlobAnalysisObj", typeof(IPlugin))]
[PartCreationPolicy(CreationPolicy.NonShared)]
[Serializable]
public class BlobAnalysisObj : IPlugin
{
    public ModuleInfo moduleInfo { get; set; } = new ModuleInfo();
    public PluginParamsBase PluginParams { get; set; } = new ParamsBlobAnalysis();

    private readonly BlobAnalysisService service = new BlobAnalysisService();

    public void IniObj()
    {
        ParamsBlobAnalysis p = (ParamsBlobAnalysis)PluginParams;
        p.filePath = moduleInfo.filePath;
    }

    public bool ExecuteObj()
    {
        ParamsBlobAnalysis p = (ParamsBlobAnalysis)PluginParams;
        return service.Run(p);
    }

    public bool saveParams()
    {
        PluginParams.filePath = moduleInfo.filePath;
        return PluginParams.Save();
    }

    public void SetParams(PluginParamsBase param)
    {
        if (param != null) PluginParams = param;
    }

    public void DisPoseObj()
    {
        ParamsBlobAnalysis p = PluginParams as ParamsBlobAnalysis;
        if (p == null) return;
        p.m_ho_OutRegion?.Dispose();
        p.m_ho_OutSelectedRegion?.Dispose();
        p.m_ho_OutImgShow?.Dispose();
    }
}
```

注意：

- `ExecuteObj()` 不访问 WPF 控件。
- 输入图像只从 `p.ho_Image` 取。
- 输出只写回 `p`。
- HALCON 临时对象在 `BlobAnalysisContext.Dispose()` 和各算法策略的 `finally` 中释放。

## 8. WPF 界面设计

界面采用 Tab 页，避免参数挤在一起。

建议布局：

```text
左侧：HALCON 图像窗口
  - 原图显示
  - 局部 ROI 显示
  - 屏蔽 ROI 显示
  - 过滤后 Blob 区域显示

右侧：参数 Tab
  Tab1 基础参数
    - 启用检测
    - 阈值 Min/Max
    - 局部 ROI 开关和坐标
    - 屏蔽 ROI 开关和坐标

  Tab2 形态学
    - 形态学方式 ComboBox
    - 半径
    - 是否 FillUp

  Tab3 过滤条件
    - DataGrid 显示 List<BlobFilterParam>
    - 添加过滤条件
    - 删除过滤条件
    - Feature ComboBox
    - Operation ComboBox
    - Min/Max

  Tab4 输出结果
    - 检测成功
    - Blob数量
    - 耗时
    - DataGrid 显示 BlobResultParam 列表
```

ComboBox 选项：

```csharp
public string[] FeatureItems { get; } =
{
    "area",
    "row",
    "column",
    "width",
    "height",
    "circularity",
    "rectangularity",
    "compactness",
    "convexity",
    "rect2_row",
    "rect2_col",
    "rect2_phi",
    "rect2_len1",
    "rect2_len2",
    "ra",
    "rb",
    "phi"
};

public string[] OperationItems { get; } = { "and", "or" };
public string[] MorphologyItems { get; } = { "None", "Open", "Close", "FillUp", "OpenClose", "CloseOpen" };
public string[] SortFeatureItems { get; } = { "area", "row", "column" };
```

第一版建议 `Operation` 默认只用 `and`，界面保留字段，避免复杂逻辑一开始做乱。

## 9. ViewROI 交互设计

Blob 分析插件也要使用 ViewROI。ViewROI 的职责是界面交互，不是算法运行依赖。也就是说：

- 界面上用 ViewROI 拖拽、缩放、设置 ROI。
- ROI 坐标写回 `ParamsBlobAnalysis`。
- 流程运行时 `BlobAnalysisObj.ExecuteObj()` 只读取参数里的 ROI 坐标和开关，不依赖 ViewROI 控件是否打开。

### 检测范围切换

界面必须提供检测范围切换：

| 模式 | 参数建议 | 说明 |
| --- | --- | --- |
| 全局检测 | `m_strDetectRangeMode = "Global"` | 不使用局部 ROI，直接整图阈值和 Blob 分析 |
| 局部检测 | `m_strDetectRangeMode = "LocalRoi"` | 只在局部 ROI 内检测 |

也可以用 bool：

```csharp
public bool m_bUseLocalRoi = false;
```

但界面建议用 ComboBox 显示：

```csharp
public string[] DetectRangeModeItems { get; } = { "全局检测", "局部ROI检测" };
```

绑定时由转换器转成 `Global / LocalRoi`，或者直接保存中文也可以，但推荐保存英文枚举字符串，后续流程判断更稳定。

### ViewROI 用途切换

一个 ViewROI 可以复用成不同用途，不需要每种 ROI 都放一套控件。界面提供“ROI用途”切换：

| ROI用途 | 参数建议 | 说明 |
| --- | --- | --- |
| 局部检测ROI | `LocalRoi` | 限定 Blob 检测范围 |
| 屏蔽ROI | `IgnoreRoi` | 扣除不需要检测的区域 |
| 显示/预览ROI | `PreviewRoi` | 只用于界面预览，第一版可不做 |

ComboBox：

```csharp
public string[] RoiEditModeItems { get; } = { "局部检测ROI", "屏蔽ROI" };
```

切换逻辑：

```text
选择“局部检测ROI”
  -> 从参数 LocalRoi 坐标生成 ViewROI 矩形
  -> 用户拖拽 ViewROI
  -> MouseUp 或 ReDrawEvent 后写回 m_fLocalRoiRow1/Col1/Row2/Col2

选择“屏蔽ROI”
  -> 从参数 IgnoreRoi 坐标生成 ViewROI 矩形
  -> 用户拖拽 ViewROI
  -> 写回 m_fIgnoreRoiRow1/Col1/Row2/Col2
```

### 参数字段补充

`ParamsBlobAnalysis` 建议增加：

```csharp
[ShowTextAttribute("检测范围模式")]
[Description("Global=全局检测，LocalRoi=局部ROI检测")]
public string m_strDetectRangeMode = "Global";

[ShowTextAttribute("当前ROI编辑模式")]
[Description("LocalRoi=局部检测ROI，IgnoreRoi=屏蔽ROI")]
public string m_strRoiEditMode = "LocalRoi";

[ShowTextAttribute("启用局部ROI")]
public bool m_bUseLocalRoi = false;

[ShowTextAttribute("局部ROI Row1")]
public double m_fLocalRoiRow1 = 0;

[ShowTextAttribute("局部ROI Col1")]
public double m_fLocalRoiCol1 = 0;

[ShowTextAttribute("局部ROI Row2")]
public double m_fLocalRoiRow2 = 1000;

[ShowTextAttribute("局部ROI Col2")]
public double m_fLocalRoiCol2 = 1000;

[ShowTextAttribute("启用屏蔽ROI")]
public bool m_bUseIgnoreRoi = false;

[ShowTextAttribute("屏蔽ROI Row1")]
public double m_fIgnoreRoiRow1 = 0;

[ShowTextAttribute("屏蔽ROI Col1")]
public double m_fIgnoreRoiCol1 = 0;

[ShowTextAttribute("屏蔽ROI Row2")]
public double m_fIgnoreRoiRow2 = 0;

[ShowTextAttribute("屏蔽ROI Col2")]
public double m_fIgnoreRoiCol2 = 0;
```

### 算法运行时的 ROI 处理

运行时不直接使用 ViewROI，只按参数处理：

```csharp
HObject detectImage = null;
HObject localRegion = null;
HObject ignoreRegion = null;
HObject validRegion = null;

if (p.m_bUseLocalRoi || p.m_strDetectRangeMode == "LocalRoi")
{
    HOperatorSet.GenRectangle1(out localRegion,
        p.m_fLocalRoiRow1,
        p.m_fLocalRoiCol1,
        p.m_fLocalRoiRow2,
        p.m_fLocalRoiCol2);
}
else
{
    HOperatorSet.GetImageSize(p.ho_Image, out HTuple width, out HTuple height);
    HOperatorSet.GenRectangle1(out localRegion, 0, 0, height - 1, width - 1);
}

if (p.m_bUseIgnoreRoi)
{
    HOperatorSet.GenRectangle1(out ignoreRegion,
        p.m_fIgnoreRoiRow1,
        p.m_fIgnoreRoiCol1,
        p.m_fIgnoreRoiRow2,
        p.m_fIgnoreRoiCol2);

    HOperatorSet.Difference(localRegion, ignoreRegion, out validRegion);
}
else
{
    HOperatorSet.CopyObj(localRegion, out validRegion, 1, -1);
}

HOperatorSet.ReduceDomain(p.ho_Image, validRegion, out detectImage);
```

后续 `Threshold / Connection / SelectShape` 都在 `detectImage` 上做。

注意：

- 全局检测时仍可生成整图 `localRegion`，这样后续屏蔽 ROI 逻辑统一。
- 屏蔽 ROI 是从检测区域里扣除，不是单独检测。
- 如果以后需要多个屏蔽 ROI，可以把屏蔽 ROI 参数扩展成 `List<BlobRoiParam>`，用 `Union2` 合并后再 `Difference`。

### ViewROI 和输出显示

建议显示颜色：

| 对象 | 颜色 |
| --- | --- |
| 局部检测 ROI | 青色 |
| 屏蔽 ROI | 红色 |
| 过滤后 Blob Region | 绿色或橙色透明 |
| 当前选中 Blob | 黄色 |

ViewROI 显示文字：

- `局部ROI`
- `屏蔽ROI`

文字继续使用统一风格：灰色背景、白色加粗字体。
## 10. WPF 绑定解耦设计

WPF 绑定不要直接把公共参数类做成复杂界面对象。公共参数类 `ParamsBlobAnalysis / BlobFilterParam / BlobResultParam` 的职责是给流程保存和传递参数，应该尽量保持简单、可序列化、低耦合。

推荐做法是在 `Plugin.BlobAnalysis` DLL 内部新增 WPF 专用绑定类，例如：

```text
Plugin.BlobAnalysis
└─ ViewModels
   ├─ BlobAnalysisViewModel.cs
   ├─ BlobFilterItemViewModel.cs
   └─ BlobResultItemViewModel.cs
```

职责划分：

| 类 | 所属位置 | 职责 |
| --- | --- | --- |
| `ParamsBlobAnalysis` | `Frame_Core\pluginParams\BlobAnalysis` | 公共流程参数，保存输入输出，不依赖 WPF |
| `BlobFilterParam` | `Frame_Core\pluginParams\BlobAnalysis` | 公共过滤条件参数，可 XML 序列化 |
| `BlobResultParam` | `Frame_Core\pluginParams\BlobAnalysis` | 公共输出结果参数，可被流程读取 |
| `BlobAnalysisViewModel` | `Plugin.BlobAnalysis\ViewModels` | WPF 绑定对象，负责界面显示、命令、集合通知 |
| `BlobFilterItemViewModel` | `Plugin.BlobAnalysis\ViewModels` | 过滤条件界面项，支持 ComboBox、校验、双向绑定 |
| `BlobResultItemViewModel` | `Plugin.BlobAnalysis\ViewModels` | 输出结果界面项，支持 DataGrid 显示 |

这样做的好处：

- 公共参数类不需要实现 `INotifyPropertyChanged`。
- 公共参数类不依赖 WPF，不污染流程运行层。
- WPF 里面可以自由使用 `ObservableCollection`、命令、下拉框文本转换器。
- 后续 DLL 给主框架运行时，`BlobAnalysisObj.ExecuteObj()` 仍然只认公共参数类。
- 界面调整不会影响流程参数结构。

### 参数转换方向

打开界面或绑定插件时：

```text
ParamsBlobAnalysis -> BlobAnalysisViewModel
BlobFilterParam -> BlobFilterItemViewModel
BlobResultParam -> BlobResultItemViewModel
```

保存参数、运行前提交界面时：

```text
BlobAnalysisViewModel -> ParamsBlobAnalysis
BlobFilterItemViewModel -> BlobFilterParam
```

运行完成后刷新结果：

```text
BlobAnalysisObj.ExecuteObj()
  -> 写入 ParamsBlobAnalysis.m_lBlobResults
  -> WPF 调用 ReloadResultsFromParams()
  -> BlobResultParam -> BlobResultItemViewModel
```

如果需要实时传入，也可以在 ViewModel 的属性 setter 中同步回公共参数类，但建议只对基础参数和过滤条件这样做。输出结果由算法统一刷新，避免界面误改结果。

### 示例：过滤条件绑定对象

```csharp
public class BlobFilterItemViewModel : BindableBase
{
    private readonly BlobFilterParam source;

    public BlobFilterItemViewModel(BlobFilterParam source)
    {
        this.source = source;
    }

    public bool IsEnable
    {
        get { return source.m_bEnable; }
        set
        {
            if (source.m_bEnable == value) return;
            source.m_bEnable = value;
            RaisePropertyChanged();
        }
    }

    public string Feature
    {
        get { return source.m_strFeature; }
        set
        {
            if (source.m_strFeature == value) return;
            source.m_strFeature = value;
            RaisePropertyChanged();
        }
    }

    public double Min
    {
        get { return source.m_fMin; }
        set
        {
            if (Math.Abs(source.m_fMin - value) < 0.000001) return;
            source.m_fMin = value;
            RaisePropertyChanged();
        }
    }

    public double Max
    {
        get { return source.m_fMax; }
        set
        {
            if (Math.Abs(source.m_fMax - value) < 0.000001) return;
            source.m_fMax = value;
            RaisePropertyChanged();
        }
    }

    public BlobFilterParam ToParam()
    {
        return source;
    }
}
```

### 示例：ViewModel 与公共参数类同步

```csharp
public class BlobAnalysisViewModel : BindableBase
{
    private readonly ParamsBlobAnalysis param;

    public ObservableCollection<BlobFilterItemViewModel> Filters { get; }
        = new ObservableCollection<BlobFilterItemViewModel>();

    public ObservableCollection<BlobResultItemViewModel> Results { get; }
        = new ObservableCollection<BlobResultItemViewModel>();

    public BlobAnalysisViewModel(ParamsBlobAnalysis param)
    {
        this.param = param;
        LoadFromParam();
    }

    public void LoadFromParam()
    {
        Filters.Clear();
        foreach (BlobFilterParam item in param.m_lFilterParams)
            Filters.Add(new BlobFilterItemViewModel(item));

        ReloadResultsFromParam();
    }

    public void CommitToParam()
    {
        param.m_lFilterParams.Clear();
        foreach (BlobFilterItemViewModel item in Filters)
            param.m_lFilterParams.Add(item.ToParam());
    }

    public void ReloadResultsFromParam()
    {
        Results.Clear();
        foreach (BlobResultParam item in param.m_lBlobResults)
            Results.Add(new BlobResultItemViewModel(item));
    }
}
```

### WPF Host 调用方式

参照 `Plugin.CACRunCalibration\WPF架构`：

```csharp
public void BindPlugin(IPlugin plugin)
{
    PluginObject = plugin;
    PluginParams = plugin == null ? null : plugin.PluginParams;

    ParamsBlobAnalysis p = PluginParams as ParamsBlobAnalysis;
    ViewModel = new BlobAnalysisViewModel(p);
    DataContext = ViewModel;
}

public virtual void Btn_RUN_Click(object sender, EventArgs e)
{
    ViewModel.CommitToParam();
    PluginObject.ExecuteObj();
    ViewModel.ReloadResultsFromParam();
}

public virtual void Btn_Save_Click(object sender, EventArgs e)
{
    ViewModel.CommitToParam();
    PluginObject.saveParams();
}
```

这套方式能保证：WPF 界面在插件 DLL 内部完成绑定和交互，公共参数类只负责被流程传入、保存、读取、输出，整体耦合最低。
## 11. 公共可复用绑定基类

`BindableBase : INotifyPropertyChanged`、`RelayCommand : ICommand`、下拉框显示项等基础类不要在每个插件 DLL 里反复写。后续插件会越来越多，如果每个 DLL 都复制一份，会出现以下问题：

- 同名类到处都是，维护困难。
- 某个插件修了通知逻辑，其他插件没同步。
- WPF 绑定行为不一致，排查问题麻烦。
- DLL 封装时显得结构散乱。

建议统一放到公共类库里复用。

优先级建议：

1. 如果现有 `YYSD.UICore` 已经有可用的绑定基类、命令类、下拉项模型，优先直接引用现有类。
2. 如果现有公共库没有，则在 `Frame_Core` 下新增公共绑定目录。

建议目录：

```text
Frame_Core
└─ CommonBinding
   ├─ BindableBase.cs
   ├─ RelayCommand.cs
   ├─ ComboBoxItemModel.cs
   └─ ObservableObjectCollectionHelper.cs   // 可选
```

命名空间建议：

```csharp
namespace Frame_Core.CommonBinding
```

### BindableBase

```csharp
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Frame_Core.CommonBinding
{
    public abstract class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
```

### RelayCommand

```csharp
using System;
using System.Windows.Input;

namespace Frame_Core.CommonBinding
{
    public class RelayCommand : ICommand
    {
        private readonly Action<object> execute;
        private readonly Predicate<object> canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            this.execute = execute ?? throw new ArgumentNullException(nameof(execute));
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
```

### ComboBoxItemModel

```csharp
namespace Frame_Core.CommonBinding
{
    public class ComboBoxItemModel<T>
    {
        public string Text { get; set; }
        public T Value { get; set; }

        public override string ToString()
        {
            return Text;
        }
    }
}
```

### 插件 DLL 使用方式

插件内部 ViewModel 直接继承公共基类：

```csharp
using Frame_Core.CommonBinding;

public class BlobFilterItemViewModel : BindableBase
{
    // 只写业务属性，不再重复写 INotifyPropertyChanged 模板代码。
}
```

命令也直接用公共 `RelayCommand`：

```csharp
public ICommand AddFilterCommand { get; }

public BlobAnalysisViewModel(ParamsBlobAnalysis param)
{
    AddFilterCommand = new RelayCommand(_ => AddFilter());
}
```

### 注意

- 公共参数类 `ParamsBlobAnalysis / BlobFilterParam / BlobResultParam` 不建议继承 `BindableBase`，它们保持普通可序列化参数类。
- WPF 专用 ViewModel 才继承 `BindableBase`。
- 公共绑定基类只放通用 UI 能力，不放 Blob 业务逻辑。
- 后续模板匹配、Blob 分析、交点工具等 WPF 插件都复用同一套公共绑定基类。
## 12. Frame_Core 与插件 DLL 边界

这里要明确边界，避免后续封装 DLL 时把职责放乱。

`Frame_Core` 只放：

- 公共参数类：`ParamsBlobAnalysis / BlobFilterParam / BlobResultParam`
- 框架接口：`IPlugin / PluginParamsBase / ModuleInfo`
- 通用基础类：`BindableBase / RelayCommand`，前提是这些类会被多个插件复用
- 通用序列化、属性显示、公共工具

`Plugin.BlobAnalysis` DLL 内部放：

- `BlobAnalysisObj`
- `BlobAnalysisService`
- WPF View / ViewModel
- Blob 专用转换器
- Blob 专用显示逻辑
- Blob 专用算法管线和策略实现

不能放进 `Frame_Core` 的内容：

- `BlobAnalysisService`
- `BlobAnalysisViewModel`
- `BlobFilterItemViewModel`
- `BlobResultItemViewModel`
- Blob 专用 HALCON 算法管线和策略实现
- Blob 专用 UI 显示逻辑

原因：

- `Frame_Core` 是公共框架层，不能依赖具体插件业务。
- 算法类放进 `Frame_Core` 会导致框架和插件倒置依赖。
- 后续如果还有模板匹配、Blob、交点、测量、缺陷检测，每个算法都塞进 `Frame_Core`，主框架会越来越难维护。
- 插件 DLL 应该可以独立替换和升级，算法留在 DLL 内部更合理。

最终依赖方向应该是：

```text
Plugin.BlobAnalysis
  -> Frame_Core
  -> YYSD.UICore / HalconDotNet

Frame_Core 不反向依赖 Plugin.BlobAnalysis
```
## 13. 算法层职责

算法层放在 `Plugin.BlobAnalysis\Algorithms` 内部，不放 `Frame_Core`。

### IBlobAnalysisPipeline

管线接口：

```csharp
public interface IBlobAnalysisPipeline
{
    BlobAnalysisResult Run(BlobAnalysisContext context);
}
```

### IBlobRoiBuilder

负责全局检测、局部 ROI、屏蔽 ROI 的统一处理：

```csharp
public interface IBlobRoiBuilder
{
    void Build(BlobAnalysisContext context);
}
```

输出：

- `context.ValidRegion`
- `context.DetectImage`

### IBlobSegmenter

分割策略接口：

```csharp
public interface IBlobSegmenter
{
    void Segment(BlobAnalysisContext context);
}
```

第一版实现：`ThresholdBlobSegmenter`，内部调用 `HOperatorSet.Threshold`。

后续可扩展：

- 动态阈值分割
- 自动阈值分割
- 灰度范围多段分割

### IBlobMorphologyProcessor

形态学策略接口：

```csharp
public interface IBlobMorphologyProcessor
{
    void Process(BlobAnalysisContext context);
}
```

第一版实现：`HalconMorphologyProcessor`，根据参数选择：

- `None`
- `OpeningCircle`
- `ClosingCircle`
- `FillUp`
- `OpenClose`
- `CloseOpen`

### IBlobRegionFilter

过滤策略接口：

```csharp
public interface IBlobRegionFilter
{
    void Filter(BlobAnalysisContext context);
}
```

第一版实现：`SelectShapeBlobRegionFilter`。

职责：

1. 读取 `List<BlobFilterParam>`。
2. 组装 `Features / Operation / Min / Max`。
3. 调 `HOperatorSet.SelectShape`。
4. 输出 `context.SelectedRegions`。

### IBlobFeatureExtractor

特征提取接口：

```csharp
public interface IBlobFeatureExtractor
{
    void Extract(BlobAnalysisContext context);
}
```

第一版实现：`HalconBlobFeatureExtractor`。

职责：

1. 遍历 `context.SelectedRegions`。
2. 每个 Region 调 `SelectObj`。
3. 计算面积、中心、质心、长宽比、灰度、圆度、矩形度。
4. 输出 `List<BlobFeature>`。
5. 由 mapper 写回公共 `BlobResultParam`。

### BlobAnalysisContext

单次运行上下文，只在 DLL 内部使用：

```csharp
public sealed class BlobAnalysisContext : IDisposable
{
    public HObject InputImage { get; set; }
    public HObject DetectImage { get; set; }
    public HObject ValidRegion { get; set; }
    public HObject ThresholdRegion { get; set; }
    public HObject MorphologyRegion { get; set; }
    public HObject ConnectedRegions { get; set; }
    public HObject SelectedRegions { get; set; }
    public BlobAnalysisResult Result { get; } = new BlobAnalysisResult();

    public void Dispose()
    {
        // 只释放本次运行临时对象，不释放 InputImage。
    }
}
```
## 14. 内存释放要求

必须释放：

- `RegionThreshold`
- `RegionMorphology`
- `ConnectedRegions`
- `SelectedRegions` 的旧对象
- `LocalRegion`
- `IgnoreRegion`
- 临时 `ReducedImage`

不释放：

- 输入图像 `p.ho_Image`，它属于上游流程。

输出对象释放规则：

- 每次运行前释放旧的 `m_ho_OutRegion / m_ho_OutSelectedRegion / m_ho_OutImgShow`。
- 新输出对象交给参数对象持有。
- `DisPoseObj()` 中再次兜底释放。

## 15. 后续 DLL 落地步骤

第一步：只新增文件，不修改原插件。

1. 新建 `Plugin.BlobAnalysis` 工程。
2. 新增 `Frame_Core\pluginParams\BlobAnalysis` 文件夹，里面放 `ParamsBlobAnalysis.cs / BlobFilterParam.cs / BlobResultParam.cs`。
3. 写 `BlobAnalysisObj.cs`，实现 `IPlugin`。
4. 在 `Plugin.BlobAnalysis` DLL 内写 `Application / Algorithms / Domain / Converters` 分层代码，不写单体 `BlobAnalysisTool`（不再使用单体工具类）。
5. 写 WPF 界面 `WPFBlobAnalysis.xaml`。
6. 引用现有 `Frame_Core / HalconDotNet / LogTool / YYSD.UICore.Control`。
7. 编译输出 `Plugin.BlobAnalysis.dll` 到插件目录。
8. 在主程序中加载插件，测试 `IniObj / ExecuteObj / saveParams / DisPoseObj`。

第二步：补 UI 交互。

1. 图像窗口显示输入图。
2. ROI 由界面设置后写回参数。
3. DataGrid 添加/删除过滤条件。
4. 点击运行后显示过滤区域和结果表。

第三步：接入流程。

1. 上游图像写入 `ParamsBlobAnalysis.ho_Image`。
2. 流程触发 `BlobAnalysisObj.ExecuteObj()`。
3. 下游读取 `m_bBlobSuccess / m_nBlobCount / m_lBlobResults / m_ho_OutSelectedRegion`，其中 `m_lBlobResults` 是过滤后多个 Region 的逐个特征输出。

## 16. 注意事项

- 公共参数类统一放在 `Frame_Core\pluginParams\BlobAnalysis` 文件夹中归类，避免散放在 `pluginParams` 根目录。
- 参数类字段命名保持现有风格：`m_` 开头。
- 可序列化参数不要使用 `HObject / HTuple`。
- `HObject` 输出必须 `[NonSerialized] + [XmlIgnore]`。
- 过滤条件列表 XML 读取时要先清空原列表再赋值，避免重复累加。
- WPF 只做参数绑定、ViewROI 交互和预览，不做算法主体。
- WPF 绑定优先使用 DLL 内部 ViewModel/DTO，再同步到公共参数类，降低公共参数和界面耦合。
- `BindableBase / RelayCommand` 等基础绑定类统一放公共类库复用，不要每个插件 DLL 重复写。
- `BlobAnalysisService / BlobAnalysisPipeline / 各算法策略` 只能放在 `Plugin.BlobAnalysis` DLL 内部，不放 `Frame_Core`，避免框架层耦合具体 Blob 业务。
- 算法层不引用 WPF，也不引用 ViewROI；它只读取参数中的 ROI 坐标和开关。
- `BlobAnalysisObj.ExecuteObj()` 是实际流程入口，界面按钮只是手动触发预览。
