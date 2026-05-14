# HALCON 模板匹配基准点与矩阵方向说明

本文档用于模板匹配 Demo 的基准点设计，重点说明创建模板时如何通过拖拽设置业务基准点，以及 `0,0,0`、`set_shape_model_origin`、`set_ncc_model_origin`、`vector_angle_to_rigid`、`affine_trans_*` 的正确用法。

参考本机 HALCON 文档目录：

`E:\halconPath\doc\html\reference\operators`

已核对的关键算子：

- `set_shape_model_origin.html`
- `get_shape_model_contours.html`
- `find_shape_model.html`
- `set_ncc_model_origin.html`
- `find_ncc_model.html`
- `vector_angle_to_rigid.html`
- `affine_trans_contour_xld.html`
- `affine_trans_region.html`
- `affine_trans_point_2d.html`
- `hom_mat2d_invert.html`

## 1. 先给结论

创建模板时就要设定业务基准点。这个基准点不是运行时临时补出来的点，而是模板定义的一部分，应当和模板文件、模板轮廓、创建参数一起保存。

推荐实现方式：

- 形状模板：创建 `shape_model` 后，使用拖拽点计算相对模板域重心的偏移，再调用 `SetShapeModelOrigin`。
- 灰度/NCC 模板：创建 `ncc_model` 后，使用拖拽点计算相对模板域重心的偏移，再调用 `SetNccModelOrigin`。
- 运行匹配后，`find_shape_model` / `find_ncc_model` 输出的 `Row`、`Column` 就按模型原点解释，可以作为当前业务基准点使用。

如果不调用 `SetShapeModelOrigin` / `SetNccModelOrigin`，则不要把拖拽出来的业务基准点直接塞进 `VectorAngleToRigid(20,20,0, Row, Col, Angle)`。这种写法会把 HALCON 模型轮廓坐标系和业务基准坐标系混掉。

## 2. 为什么 `0,0,0` 也能用

常见显示代码如下：

```csharp
HOperatorSet.GetShapeModelContours(out HObject modelContours, modelID, 1);

HOperatorSet.VectorAngleToRigid(
    0, 0, 0,
    row, column, angle,
    out HTuple homMat2D);

HOperatorSet.AffineTransContourXld(
    modelContours,
    out HObject contours,
    homMat2D);
```

这段代码能正确显示，是因为 `GetShapeModelContours` 返回的模型轮廓已经被归一化：模型参考点在轮廓坐标系中的位置是 `(0,0)`。

也就是说，`0,0,0` 不是随便填的，而是在说：

```text
模型轮廓当前位于“模型参考点 = 0,0,0”的坐标系中。
现在把它变换到匹配结果 Row/Column/Angle 所在的位置。
```

因此：

- 模型轮廓显示可以继续用 `0,0,0 -> Row/Column/Angle`。
- 但是业务基准点拖拽后，不能直接替换这三个 0。
- 真正要改变匹配输出基准点，应该在创建模板阶段设置模型 origin。

## 3. HALCON 模型原点的含义

`set_shape_model_origin` 的文档说明：模型 origin 是相对模型图像 domain 的重心指定的。

含义：

```text
OriginRow = 0, OriginCol = 0
表示模型图像 domain 的重心作为模型参考点。

OriginRow = -20, OriginCol = -40
表示模型参考点在 domain 重心的左上方。
```

`set_ncc_model_origin` 对 NCC 模板同理，也是相对 NCC 模型 domain 的重心指定。

所以拖拽点如果是图像绝对坐标，不能直接传给 `SetShapeModelOrigin` 或 `SetNccModelOrigin`，要先转成相对模板 domain 重心的偏移。

## 4. 创建模板时如何用拖拽基准点

创建模板推荐流程如下：

1. 用户在模板编辑界面画模板 ROI。
2. 根据模板 ROI 裁剪或 ReduceDomain 后创建模板。
3. 计算模板 domain 的重心，作为默认基准点。
4. 在图像窗口显示一个可拖拽基准点 ROI，例如十字点、小圆点或很小的矩形。
5. 用户拖拽基准点到业务需要的位置。
6. 拖拽结束后读取 `BaseRowAbs`、`BaseColAbs`。
7. 计算相对模板 domain 重心的偏移。
8. 调用 `SetShapeModelOrigin` 或 `SetNccModelOrigin`。
9. 保存模板文件和基准点参数。

伪代码：

```csharp
// 1. 模板 ROI 已经由 ViewROI 得到，并转换为 HALCON Region。
HOperatorSet.ReduceDomain(image, templateRegion, out HObject templateImage);

// 2. 创建形状模板。
HOperatorSet.CreateShapeModel(
    templateImage,
    numLevels,
    angleStart,
    angleExtent,
    angleStep,
    optimization,
    metric,
    contrast,
    minContrast,
    out HTuple modelID);

// 3. 计算模板 domain 重心。
// 注意：这里应使用创建模型时的 domain/region，而不是整张图。
HOperatorSet.AreaCenter(templateRegion, out HTuple area, out HTuple domainCenterRow, out HTuple domainCenterCol);

// 4. 用户拖拽得到图像绝对坐标。
double baseRowAbs = param.m_fBaseRow;
double baseColAbs = param.m_fBaseCol;

// 5. 转成相对 domain 重心的 origin 偏移。
double originRow = baseRowAbs - domainCenterRow.D;
double originCol = baseColAbs - domainCenterCol.D;

// 6. 设置 HALCON 模型原点。
HOperatorSet.SetShapeModelOrigin(modelID, originRow, originCol);

// 7. 重新取得模型轮廓。此时轮廓仍然归一化到模型参考点为 0,0。
HOperatorSet.GetShapeModelContours(out HObject modelContours, modelID, 1);
```

NCC 模板同理：

```csharp
HOperatorSet.CreateNccModel(
    templateImage,
    numLevels,
    angleStart,
    angleExtent,
    angleStep,
    metric,
    out HTuple nccModelID);

HOperatorSet.SetNccModelOrigin(nccModelID, originRow, originCol);
```

## 5. 运行匹配后的矩阵方向

`vector_angle_to_rigid(Row1, Column1, Angle1, Row2, Column2, Angle2, out HomMat2D)` 的含义是：

```text
把源姿态 Row1/Column1/Angle1
变换到目标姿态 Row2/Column2/Angle2
```

因此：

```csharp
// Base -> Current
HOperatorSet.VectorAngleToRigid(
    baseRow,
    baseCol,
    DegToRad(baseAngleDeg),
    currentRow,
    currentCol,
    currentAngleRad,
    out HTuple homMatBaseToCurrent);
```

用于：

- 模板轮廓显示
- 基准 ROI 跟随到当前图像
- 屏蔽区跟随到当前图像
- 业务基准点从模板侧变换到当前侧

反方向：

```csharp
HOperatorSet.HomMat2dInvert(homMatBaseToCurrent, out HTuple homMatCurrentToBase);
```

用于：

- 当前点修正回基准坐标
- 输出与基准的偏差
- 把运行点、测量点转换回模板基准侧

## 6. AffineTrans 输入对象必须是哪一侧

核心规则：

```text
HomMatBaseToCurrent 只能作用在 Base 侧对象上。
HomMatCurrentToBase 只能作用在 Current 侧对象上。
```

正确：

```csharp
HOperatorSet.AffineTransRegion(
    baseSearchRegion,
    out HObject currentSearchRegion,
    homMatBaseToCurrent,
    "nearest_neighbor");

HOperatorSet.AffineTransContourXld(
    baseModelContour,
    out HObject currentModelContour,
    homMatBaseToCurrent);
```

错误：

```csharp
// 错误：currentRegion 已经是当前图像坐标，再用 Base -> Current 会多变换一次。
HOperatorSet.AffineTransRegion(
    currentRegion,
    out HObject wrongRegion,
    homMatBaseToCurrent,
    "nearest_neighbor");
```

如果手上是当前侧对象，要回到基准侧：

```csharp
HOperatorSet.HomMat2dInvert(homMatBaseToCurrent, out HTuple homMatCurrentToBase);

HOperatorSet.AffineTransRegion(
    currentRegion,
    out HObject baseRegion,
    homMatCurrentToBase,
    "nearest_neighbor");
```

## 7. 两条实现路线

### 路线 A：推荐，创建模板时设置 HALCON 模型原点

适合业务上明确要求“匹配输出 Row/Column 就是业务基准点”的场景。

创建时：

```text
用户拖拽业务基准点
-> 计算相对模板 domain 重心的偏移
-> SetShapeModelOrigin / SetNccModelOrigin
-> 保存模板
```

运行时：

```text
FindShapeModel / FindNccModel 输出 Row/Column/Angle
= 当前业务基准姿态
```

轮廓显示：

```csharp
HOperatorSet.VectorAngleToRigid(
    0, 0, 0,
    row, column, angle,
    out HTuple homMatModelToCurrent);

HOperatorSet.AffineTransContourXld(
    modelContours,
    out HObject contours,
    homMatModelToCurrent);
```

优点：

- 后续理解简单。
- 匹配输出就是业务基准点。
- 偏差计算直接。

注意：

- `SetShapeModelOrigin` / `SetNccModelOrigin` 的参数不是图像绝对坐标，而是相对 domain 重心的偏移。
- `find_shape_model` 文档说明，搜索域边界判断仍与模型 domain 相关，设置不同 origin 不参与这部分边界判断。因此局部 ROI 不要卡得过紧。

### 路线 B：不修改 HALCON 模型原点，另存业务基准点局部坐标

适合不想影响原有模板匹配逻辑，只想额外输出业务基准点的场景。

创建时：

```text
保持 HALCON 默认模型原点
用户拖拽业务基准点
把业务基准点转换为模型局部坐标保存
```

运行时：

```text
FindShapeModel 输出 HALCON 模型原点当前姿态
用 0,0,0 -> Row/Column/Angle 得到 HomMatModelToCurrent
把业务基准局部点 AffineTransPoint2d 到当前图像
```

示例：

```csharp
// 匹配输出的是 HALCON 模型原点当前位置。
HOperatorSet.FindShapeModel(
    image,
    modelID,
    angleStart,
    angleExtent,
    minScore,
    numMatches,
    maxOverlap,
    subPixel,
    numLevels,
    greediness,
    out HTuple row,
    out HTuple col,
    out HTuple angle,
    out HTuple score);

// 模型局部坐标 -> 当前图像坐标。
HOperatorSet.VectorAngleToRigid(
    0, 0, 0,
    row[0], col[0], angle[0],
    out HTuple homMatModelToCurrent);

HOperatorSet.AffineTransPoint2d(
    homMatModelToCurrent,
    param.m_fBaseLocalRow,
    param.m_fBaseLocalCol,
    out HTuple currentBaseRow,
    out HTuple currentBaseCol);
```

优点：

- 原有 `0,0,0` 显示逻辑可以保持。
- 对旧 Demo 改动小。

缺点：

- 需要额外维护业务基准点局部坐标。
- 多模板、多 ROI 跟随时更容易混淆模型原点和业务基准点。

## 8. 参数类建议

建议参数类明确区分三类点：

```csharp
// HALCON 模型原点，传给 SetShapeModelOrigin / SetNccModelOrigin。
public double m_fModelOriginRow = 0.0;
public double m_fModelOriginCol = 0.0;

// 创建模板时用户拖拽的业务基准点，图像绝对坐标。
public double m_fBaseRow = 0.0;
public double m_fBaseCol = 0.0;
public double m_fBaseAngleDeg = 0.0;

// 路线 B 使用：业务基准点相对模型原点的局部坐标。
public double m_fBaseLocalRow = 0.0;
public double m_fBaseLocalCol = 0.0;
public double m_fBaseAngleOffsetDeg = 0.0;

// 运行时输出：当前图像中的业务基准点。
[NonSerialized]
[System.Xml.Serialization.XmlIgnore]
public double m_fCurrentBaseRow = 0.0;

[NonSerialized]
[System.Xml.Serialization.XmlIgnore]
public double m_fCurrentBaseCol = 0.0;

[NonSerialized]
[System.Xml.Serialization.XmlIgnore]
public double m_fCurrentBaseAngleDeg = 0.0;

// 矩阵输出。
[NonSerialized]
[System.Xml.Serialization.XmlIgnore]
public HTuple m_HomMatBaseToCurrent;

[NonSerialized]
[System.Xml.Serialization.XmlIgnore]
public HTuple m_HomMatCurrentToBase;
```

## 9. ViewROI 交互要求

模板编辑状态：

- 显示模板 ROI。
- 显示业务基准点 ROI。
- 允许拖拽业务基准点。
- 拖拽结束后同步 `m_fBaseRow`、`m_fBaseCol`、`m_fBaseAngleDeg`。
- 参数界面 TwoWay Binding 立即显示最新值。
- 点击保存模板时，同步保存模型文件与基准参数。

普通运行状态：

- 禁止误拖模板业务基准点。
- 只显示运行时推算出的当前业务基准点。
- ROI 跟随只能使用基准侧 ROI 加 `HomMatBaseToCurrent` 生成当前侧 ROI。

## 10. 验收用例

必须验证以下场景：

1. 使用默认基准点，不调用 `SetShapeModelOrigin`，轮廓显示正常。
2. 拖拽业务基准点后调用 `SetShapeModelOrigin`，匹配输出 Row/Col 跟随业务基准点变化。
3. 拖拽业务基准点到 `(20,20)`，确认不能直接用 `VectorAngleToRigid(20,20,0, Row, Col, Angle)` 变换 `GetShapeModelContours` 的轮廓。
4. ROI 跟随时，输入 Base 侧 ROI，输出 Current 侧 ROI。
5. 当前测量点回基准侧时，必须使用 `HomMatCurrentToBase`。
6. 局部 ROI 搜索区域适当外扩，避免模型本体因为边界判断被排除。

## 11. 最终推荐

本工具 Demo 推荐采用路线 A：

```text
创建模板时拖拽业务基准点
-> 计算相对模板 domain 重心的偏移
-> SetShapeModelOrigin / SetNccModelOrigin
-> 保存模板
-> 运行时 find_*_model 输出当前业务基准点
-> Base -> Current 用于显示和 ROI 跟随
-> Current -> Base 用于纠偏输出
```

这样界面、参数、输出都最容易解释，也最符合“模板基准点可以拖拽设定”的业务需求。
