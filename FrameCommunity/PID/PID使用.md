# BackSizeDeviationCompensator 变量、调用、调试、方向说明

## 1. 这个类到底做什么

`BackSizeDeviationCompensator` 只负责根据后段尺寸相机的测量值，输出一个可以加到原脚本 `SendDiff` 上的补偿增量：

```text
后段实际尺寸 actualSize
标准尺寸 standardSize
尺寸偏差 sizeError = actualSize - standardSize
经过异常值剔除、平均、比例、限幅
输出 xComp = CurrentDeviationCompensation
```

它不改原脚本的队列，不改 `Length`、`VLength`、`lastRunLength`。

原脚本仍然负责：

```text
相机到切刀距离缓存
相机到 V 刀距离缓存
SendDiff 计算
lastRunLength 推进缓存
输出实际纠偏量
```

类只给原脚本一个补偿值：

```csharp
double xComp = sizeComp.GetCurrentDeviationCompensation();
```

然后原脚本按你的逻辑接入：

```csharp
SendDiff = SendDiff + xComp;
double SendDiff2 = SendDiff - pic;
lastRunLength = lastRunLength + xComp;
```

---

## 2. 类属性详细说明

### Enabled

```csharp
public bool Enabled { get; set; }
```

是否启用后段尺寸反馈。

- `true`：启用后段尺寸反馈，会接收尺寸并更新补偿。
- `false`：关闭反馈，`Update` 不更新，当前补偿保持不变。

建议现场调试第一步先设为 `false`，确认原脚本正常；再设为 `true`。

---

### Gain

```csharp
public double Gain { get; set; }
```

尺寸偏差转换成 X 补偿的比例系数。

内部计算方向：

```csharp
step = averageSizeError * Gain;
CurrentDeviationCompensation += step;
```

其中：

```text
averageSizeError = 后段实际尺寸 - 标准尺寸
```

例如：

```text
标准尺寸 = 100.000
后段实际尺寸 = 100.080
尺寸偏差 = +0.080
Gain = 0.3
本轮理论补偿变化 = +0.024
```

方向判断方法：

```text
如果后段尺寸偏大，xComp 增加后，后段尺寸逐渐变小，Gain 方向正确。
如果后段尺寸偏大，xComp 增加后，后段尺寸更大，Gain 方向反了，改成负数。
```

不要一开始给太大。建议初始：

```csharp
Gain = 0.1 ~ 0.3;
```

---

### SampleCount

```csharp
public int SampleCount { get; set; }
```

多少个后段尺寸样本更新一次补偿。

比如：

```csharp
SampleCount = 5;
```

表示收集 5 个有效尺寸偏差后，才做一次：

```text
剔异常值 -> 求平均 -> 更新 xComp
```

建议：

```text
现场尺寸稳定：5
后段测量抖动大：10 ~ 20
想更快响应：3 ~ 5
```

注意：后段尺寸相机测的是已经裁切后的结果，不适合每片强修，所以不建议设置为 1。

---

### DeadBand

```csharp
public double DeadBand { get; set; }
```

死区。平均尺寸偏差小于这个值时，不再更新补偿。

例如：

```csharp
DeadBand = 0.02;
```

表示：

```text
|平均尺寸偏差| <= 0.02mm
认为尺寸已经够接近，不继续修，避免来回抖动。
```

建议按尺寸公差设置：

```text
公差很严：0.01 ~ 0.02
普通调试：0.02 ~ 0.05
```

---

### MaxSingleSizeError

```csharp
public double MaxSingleSizeError { get; set; }
```

单个尺寸偏差的最大有效范围，超过就认为是异常点，直接丢弃。

内部判断：

```csharp
if (Math.Abs(sizeError) > Math.Abs(MaxSingleSizeError))
{
    // 直接丢弃
}
```

例如：

```csharp
MaxSingleSizeError = 2.0;
```

表示后段尺寸偏差超过 2mm，认为这次测量不可信，不参与补偿。

建议：

```text
按现场正常最大波动的 2~3 倍设置。
正常偏差在 ±0.3mm 内，可以设 1.0mm。
正常偏差可能到 ±1mm，可以设 2.0mm。
```

---

### IqrFactor

```csharp
public double IqrFactor { get; set; }
```

成组样本的 IQR 异常值剔除倍数。

满 `SampleCount` 个样本后，类会用四分位距剔除离群点：

```text
Q1 = 25% 分位数
Q3 = 75% 分位数
IQR = Q3 - Q1
保留范围 = [Q1 - IqrFactor * IQR, Q3 + IqrFactor * IQR]
```

默认：

```csharp
IqrFactor = 1.5;
```

建议：

```text
1.5：常用，剔除比较严格
2.0：现场波动稍大
3.0：只剔特别离谱的点
```

---

### MaxStepPerUpdate

```csharp
public double MaxStepPerUpdate { get; set; }
```

每一轮更新时，xComp 最多变化多少。

例如：

```csharp
MaxOutputCompensation = 0.1;
```

即使平均偏差很大，单次也最多让补偿变化 0.1mm，防止突然大跳。

建议初始：

```text
0.03 ~ 0.05：保守
0.1：常规
0.2：响应快，但风险更大
```

---

### MaxTotalCompensation

```csharp
public double MaxTotalCompensation { get; set; }
```

累计 xComp 的最大绝对值。

例如：

```csharp

```

表示：

```text
-1.0 <= xComp <= 1.0
```

防止长期积分把补偿推到危险位置。

建议按设备允许的最大补偿范围设置。

---

### CurrentDeviationCompensation

```csharp
public double CurrentDeviationCompensation { get; private set; }
```

当前输出给脚本使用的 xComp。这个值不是累计值。

原脚本要加的就是这个值：

```csharp
double xComp = sizeComp.GetCurrentDeviationCompensation();
```

或者在 `Update` 后直接取：

```csharp
BackSizeDeviationResult fb = sizeComp.Update(actualSize, standardSize);
double xComp = fb.CurrentDeviationCompensation;
```

---

## 3. 方法详细说明

### Update(actualSize, standardSize)

```csharp
BackSizeDeviationResult fb = sizeComp.Update(actualSize, standardSize);
```

用于后段相机有新尺寸值时。

内部会计算：

```csharp
sizeError = actualSize - standardSize;
```

然后决定是否接收、是否凑满一组、是否更新 xComp。

---

### UpdateBySizeError(sizeError)

```csharp
BackSizeDeviationResult fb = sizeComp.UpdateBySizeError(sizeError);
```

如果外部已经算好尺寸偏差，可以直接传偏差。

但按当前方案，推荐后段只输出实际尺寸，前段调用 `Update(actualSize, standardSize)`。

---

### GetCurrentDeviationCompensation()

```csharp
double xComp = sizeComp.GetCurrentDeviationCompensation();
```

没有新后段尺寸时，也可以取当前已学习到的补偿值。

这个值不是累计保持值。类在每次 `Update` 开始时先清零；只有本轮真正完成有效计算时才重新输出非零 xComp。

---

### Reset()

```csharp
sizeComp.Reset();
```

清空样本缓存和当前补偿。

建议在以下场景调用：

```text
换型
换卷
重新标定
人工调整机构
清线
发现补偿方向/参数明显不对后重新调试
```

也可以指定初始补偿：

```csharp
sizeComp.Reset(0.05);
```

---

## 4. Result 输出字段说明

### Accepted

```csharp
fb.Accepted
```

本次尺寸样本是否被接收。

- `true`：样本进入缓存。
- `false`：样本被丢弃，比如未启用、NaN、超限。

---

### Updated

```csharp
fb.Updated
```

本次是否完成了一轮补偿更新。

例如 `SampleCount = 5`，通常每 5 个有效样本才会有一次 `Updated = true`。

---

### RawSizeError

```csharp
fb.RawSizeError
```

本次原始尺寸偏差：

```text
actualSize - standardSize
```

---

### AverageSizeError

```csharp
fb.AverageSizeError
```

凑满一组后，剔除异常值之后的平均尺寸偏差。

只有 `Updated = true` 时最有意义。

---

### Step

```csharp
fb.Step
```

本轮给 xComp 增加了多少。

关系：

```text
CurrentDeviationCompensation = 上一轮补偿 + Step
```

如果 `Step` 长期为 0，可能是：

```text
尺寸偏差在 DeadBand 内
Gain = 0
样本没有凑满
样本被异常剔除
```

---

### CurrentDeviationCompensation

```csharp
fb.CurrentDeviationCompensation
```

当前这一次允许加到原脚本的 xComp。若样本未凑满、在死区内或异常丢弃，则为 0。

---

### PendingSampleCount

```csharp
fb.PendingSampleCount
```

当前缓存了多少个有效尺寸样本。

如果一直达不到 `SampleCount`，说明后段尺寸有效信号没有持续给。

---

### RejectedOutlierCount

```csharp
fb.RejectedOutlierCount
```

本轮 IQR 剔除掉的异常点数量。

如果这个值经常很大，说明：

```text
后段尺寸测量波动大
SampleCount 太小
IqrFactor 太严格
MaxSingleSizeError 设置不合适
```

---

### Message

```csharp
fb.Message
```

调试信息，例如：

```text
样本已接收，等待凑满一组
补偿值已更新
平均偏差在死区内，补偿值保持不变
尺寸偏差超过单点上限，已丢弃
```

建议输出到脚本调试界面。

---

## 5. 脚本里完整调用步骤

### 第一步：引用命名空间

如果脚本环境允许 using：

```csharp
using VisionCorrectionAlgorithms;
```

如果不允许 using，就用完整类名。

---

### 第二步：在 ScriptClass 内部、Process 外部声明静态对象

```csharp
private static BackSizeDeviationCompensator sizeComp = new BackSizeDeviationCompensator
{
    Enabled = true,
    Gain = 0.3,
    SampleCount = 5,
    DeadBand = 0.02,
    MaxSingleSizeError = 2.0,
    IqrFactor = 1.5,
    MaxStepPerUpdate = 0.1,
    MaxTotalCompensation = 1.0
};
```

必须放在 `Process` 外面。否则每次执行都重新 new，补偿学习值会丢。

---

### 第三步：Process 里读取尺寸

```csharp
object inputSize = null;
scriptOperation.GetData("后段实际尺寸", ref inputSize);
double actualSize = Convert.ToDouble(inputSize);

object inputStd = null;
scriptOperation.GetData("标准尺寸", ref inputStd);
double standardSize = Convert.ToDouble(inputStd);
```

如果有有效信号：

```csharp
object inputHasSize = null;
scriptOperation.GetData("后段尺寸有效", ref inputHasSize);
int hasSize = Convert.ToInt16(inputHasSize);
```

---

### 第四步：更新补偿器

有有效信号时：

```csharp
BackSizeDeviationResult fb = new BackSizeDeviationResult();

if (hasSize == 1)
{
    fb = sizeComp.Update(actualSize, standardSize);
}

double xComp = sizeComp.GetCurrentDeviationCompensation();
```

如果每拍都有尺寸：

```csharp
BackSizeDeviationResult fb = sizeComp.Update(actualSize, standardSize);
double xComp = fb.CurrentDeviationCompensation;
```

---

### 第五步：插入到原脚本 SendDiff 计算后

原脚本位置：

```csharp
lastRunLength = SendDiff;//上次PLC走的距离pic +

double SendDiff2 = SendDiff - pic;
```

改成：

```csharp
lastRunLength = SendDiff;

// xComp 是后段尺寸反馈补偿增量
SendDiff = SendDiff + xComp;

double SendDiff2 = SendDiff - pic;

// lastRunLength 必须同步加 xComp，否则下一拍缓存推进不准
lastRunLength = lastRunLength + xComp;
```

注意：

```text
xComp 不是最终纠偏量。
SendDiff2 也不是 xComp。
SendDiff2 = 原脚本实时纠偏 + xComp。
lastRunLength = 原脚本实际走料长度 + xComp。
```

---

## 6. 调试输出建议

建议脚本输出这些调试量：

```csharp
scriptOperation.SetData("后段尺寸X补偿", xComp.ToString("f3"));
scriptOperation.SetData("后段尺寸偏差", fb.RawSizeError.ToString("f3"));
scriptOperation.SetData("后段尺寸平均偏差", fb.AverageSizeError.ToString("f3"));
scriptOperation.SetData("后段尺寸补偿步长", fb.Step.ToString("f3"));
scriptOperation.SetData("后段尺寸补偿更新", fb.Updated ? 1 : 0);
scriptOperation.SetData("后段尺寸样本缓存数", fb.PendingSampleCount);
scriptOperation.SetData("后段尺寸剔除异常数量", fb.RejectedOutlierCount);
scriptOperation.SetData("后段尺寸反馈信息", fb.Message);
scriptOperation.SetData("实际纠偏量", SendDiff2.ToString("f3"));
scriptOperation.SetData("主驱上次运行的实际长度", lastRunLength);
```

现场主要看：

```text
后段尺寸偏差
后段尺寸平均偏差
后段尺寸X补偿
后段尺寸补偿步长
实际纠偏量
主驱上次运行的实际长度
```

---

## 7. 调试流程

### 阶段 1：先不启用反馈

```csharp
Enabled = false;
```

确认原脚本原逻辑正常。

---

### 阶段 2：启用反馈但小增益

```csharp
Enabled = true;
Gain = 0.1;
MaxOutputCompensation = 0.03;

```

观察 30~100 片。

---

### 阶段 3：判断方向

如果：

```text
后段尺寸偏大，xComp 增加后，后段尺寸逐渐靠近标准
```

方向正确。

如果：

```text
后段尺寸偏大，xComp 增加后，后段尺寸更大
```

方向反了，改：

```csharp
Gain = -Gain;
```

---

### 阶段 4：调响应速度

如果修得太慢：

```text
适当增大 Gain
适当增大 MaxOutputCompensation
适当减小 SampleCount
```

如果来回振荡：

```text
减小 Gain
减小 MaxOutputCompensation
增大 SampleCount
增大 DeadBand
```

---

## 8. 推荐初始参数

保守启动：

```csharp
Gain = 0.1;
SampleCount = 10;
DeadBand = 0.02;
MaxSingleSizeError = 2.0;
IqrFactor = 1.5;
MaxOutputCompensation = 0.03;

```

确认方向后可以改成：

```csharp
Gain = 0.3;
SampleCount = 5;
MaxOutputCompensation = 0.1;

```

---

## 9. 常见问题

### xComp 一直是 0

检查：

```text
Enabled 是否 true
后段尺寸有效是否一直为 0
PendingSampleCount 是否没达到 SampleCount
DeadBand 是否设太大
Gain 是否为 0
尺寸偏差是否被 MaxSingleSizeError 丢弃
```

### xComp 经常顶到输出上限

可能原因：

```text
Gain 方向反了
标准尺寸给错
后段尺寸值单位不对
MaxOutputCompensation 太小或太大
设备本身有持续机械偏差
```

### RejectedOutlierCount 经常很大

可能原因：

```text
后段测量抖动大
IqrFactor 太小
SampleCount 太小
后段尺寸偶发识别错误
```

可以调：

```csharp
IqrFactor = 2.0 或 3.0;
SampleCount = 10 或 20;
```

### 输出补偿后缓存错位

检查是否同时做了：

```csharp
SendDiff = SendDiff + xComp;
lastRunLength = lastRunLength + xComp;
```

只加 `SendDiff` 不加 `lastRunLength`，队列推进会不一致。
---

## 10. 构造函数默认参数逐项说明

类默认参数如下：

```csharp
public BackSizeDeviationCompensator()
{
    Enabled = true;
    Gain = 0.3;
    SampleCount = 5;
    DeadBand = 0.02;
    MaxSingleSizeError = 2.0;
    IqrFactor = 1.5;
    MaxOutputCompensation = 0.1;
}
```

### Enabled = true

是否启用后段尺寸补偿。

```text
true  = 接收后段尺寸，允许输出 xComp
false = 不接收后段尺寸，xComp 始终输出 0
```

调试建议：

```text
第一次接入时，可以先设 false，确认原脚本运行正常。
确认原脚本正常后，再设 true 开始观察后段尺寸反馈。
```

---

### Gain = 0.3

尺寸偏差转换成 xComp 的比例。

计算关系：

```csharp
xComp = 平均尺寸偏差 * Gain;
```

其中：

```text
平均尺寸偏差 = 后段实际尺寸 - 标准尺寸
```

例子：

```text
后段实际尺寸 = 100.100
标准尺寸 = 100.000
平均尺寸偏差 = +0.100
Gain = 0.3
xComp = +0.030
```

调方向：

```text
尺寸偏大，补偿后尺寸往标准靠近：Gain 方向正确。
尺寸偏大，补偿后尺寸更大：Gain 方向反了，改成 -0.3。
```

建议范围：

```text
初次调试：0.05 ~ 0.1
方向确认后：0.1 ~ 0.3
响应太慢：适当增大
来回振荡：减小
```

---

### SampleCount = 5

每多少个有效尺寸样本计算一次 xComp。

```text
SampleCount = 5 表示收集 5 个有效尺寸偏差后，才做一次剔异常和平均。
```

注意：

```text
未凑满 SampleCount 前，xComp = 0。
```

建议范围：

```text
3 ~ 5：响应快，但抗抖差
5 ~ 10：常用
10 ~ 20：后段尺寸抖动大时使用
```

调试判断：

```text
PendingSampleCount 一直小于 SampleCount：说明后段尺寸有效信号不连续，或者尺寸输入没有每片进入。
```

---

### DeadBand = 0.02

死区，单位与尺寸单位一致，通常是 mm。

```text
|平均尺寸偏差| <= DeadBand 时，xComp = 0。
```

例子：

```text
DeadBand = 0.02
平均尺寸偏差 = +0.015
因为 0.015 <= 0.02，所以 xComp = 0
```

作用：

```text
避免尺寸已经接近标准时还继续修，导致来回抖动。
```

建议范围：

```text
0.01 ~ 0.02：精度要求高
0.02 ~ 0.05：常规
太大：补偿不动作
太小：容易来回抖
```

---

### MaxSingleSizeError = 2.0

单点尺寸偏差上限，单位通常是 mm。

判断逻辑：

```csharp
if (Math.Abs(actualSize - standardSize) > MaxSingleSizeError)
{
    // 认为是异常测量，丢弃，本次 xComp = 0
}
```

例子：

```text
MaxSingleSizeError = 2.0
某次后段尺寸偏差 = +3.5
这次样本直接丢弃，不进入 SampleCount 缓存
```

作用：

```text
防止后段相机误测、识别错边、通讯异常值把补偿打飞。
```

建议设置：

```text
按现场正常最大尺寸波动的 2~3 倍设置。
正常偏差 ±0.3mm：可设 1.0
正常偏差 ±0.8mm：可设 2.0
```

太小的影响：

```text
正常样本也被丢弃，补偿不更新。
```

太大的影响：

```text
异常值可能进入后续计算，只能依赖 IQR 再剔除。
```

---

### IqrFactor = 1.5

IQR 四分位距异常值剔除倍数。

凑满 `SampleCount` 后会做：

```text
Q1 = 25% 分位数
Q3 = 75% 分位数
IQR = Q3 - Q1
保留范围 = [Q1 - IqrFactor * IQR, Q3 + IqrFactor * IQR]
```

默认 `1.5` 是常用值。

建议范围：

```text
1.5：剔除较严格，常用
2.0：现场波动较大
3.0：只剔除特别离谱的点
```

调试判断：

```text
RejectedOutlierCount 经常很大：IqrFactor 可能太小，或者后段尺寸测量本身不稳定。
```

---

### MaxOutputCompensation = 0.1

单次输出的 xComp 最大绝对值，单位通常是 mm。

判断逻辑：

```csharp
xComp = averageError * Gain;
xComp = Clamp(xComp, -MaxOutputCompensation, MaxOutputCompensation);
```

例子：

```text
平均尺寸偏差 = +1.0
Gain = 0.3
理论 xComp = +0.3
MaxOutputCompensation = 0.1
最终 xComp = +0.1
```

作用：

```text
防止一次补偿太大，导致 SendDiff 和 lastRunLength 突然跳变。
```

建议范围：

```text
初次调试：0.02 ~ 0.05
常规：0.05 ~ 0.1
响应要快：0.1 ~ 0.2，但风险更大
```

注意：

```text
MaxOutputCompensation 不是累计上限。
这个类现在不做累计保持。
每次有效更新只输出当前这一轮的 xComp。
死区内、未凑满、异常时，xComp = 0。
```

---

## 11. 默认参数的整体含义

默认参数代表一种比较温和的补偿策略：

```text
每 5 个有效尺寸样本更新一次；
单个尺寸偏差超过 2mm 直接丢弃；
一组样本里再用 IQR 剔异常；
平均偏差小于 0.02mm 不补偿；
超出死区后按 0.3 比例转换；
单次最多输出 0.1mm 的 xComp。
```

也就是：

```text
稳定优先，避免后段偶发误测把前段定位纠偏打乱。
```

---

## 12. 推荐现场调试起步参数

更保守的首次接入参数：

```csharp
Enabled = true;
Gain = 0.1;
SampleCount = 10;
DeadBand = 0.02;
MaxSingleSizeError = 1.0;
IqrFactor = 2.0;
MaxOutputCompensation = 0.03;
```

确认方向正确后，可逐步改成：

```csharp
Gain = 0.2;
SampleCount = 5;
MaxOutputCompensation = 0.05;
```

如果仍然太慢，再考虑：

```csharp
Gain = 0.3;
MaxOutputCompensation = 0.1;
```

