$ErrorActionPreference = 'Stop'

$out = Join-Path (Get-Location) '机械龙工作台积木玩具搭建说明书_V0.1_正式版.docx'
if (Test-Path -LiteralPath $out) { Remove-Item -LiteralPath $out -Force }

function Add-Para {
    param(
        [Parameter(Mandatory=$true)] $Doc,
        [Parameter(Mandatory=$true)] [string] $Text,
        [double] $Size = 10.5,
        [int] $Bold = 0,
        [int] $After = 6,
        [int] $Color = 0,
        [string] $Style = ''
    )
    $p = $Doc.Paragraphs.Add()
    $p.Range.Text = $Text
    $p.Range.Font.Name = 'Microsoft YaHei'
    $p.Range.Font.Size = $Size
    $p.Range.Font.Bold = $Bold
    if ($Color -ne 0) { $p.Range.Font.Color = $Color }
    if ($Style -ne '') { $p.Range.Style = $Style }
    $p.Format.SpaceAfter = $After
    $p.Format.LineSpacingRule = 0
    $p.Range.InsertParagraphAfter()
    return $p
}

function Add-Heading {
    param([Parameter(Mandatory=$true)] $Doc, [Parameter(Mandatory=$true)] [string] $Text)
    [void](Add-Para -Doc $Doc -Text $Text -Size 14 -Bold 1 -After 8 -Color 8404992 -Style 'Heading 1')
}

function Set-Cell {
    param($Cell, [string] $Text, [int] $Bold = 0, [double] $Size = 9, [int] $Color = 0)
    $Cell.Range.Text = $Text
    $Cell.Range.Font.Name = 'Microsoft YaHei'
    $Cell.Range.Font.Size = $Size
    $Cell.Range.Font.Bold = $Bold
    if ($Color -ne 0) { $Cell.Range.Font.Color = $Color }
    $Cell.VerticalAlignment = 1
}

function Add-Table {
    param(
        [Parameter(Mandatory=$true)] $Doc,
        [Parameter(Mandatory=$true)] [string[]] $Headers,
        [Parameter(Mandatory=$true)] [object[]] $Rows
    )
    $range = $Doc.Range($Doc.Content.End - 1, $Doc.Content.End - 1)
    $table = $Doc.Tables.Add($range, $Rows.Count + 1, $Headers.Count)
    $table.Borders.Enable = 1
    $table.Range.Font.Name = 'Microsoft YaHei'
    $table.Range.Font.Size = 9
    $table.Rows.Item(1).Shading.BackgroundPatternColor = 12611584
    for ($c = 1; $c -le $Headers.Count; $c++) {
        Set-Cell $table.Cell(1, $c) $Headers[$c - 1] 1 9 16777215
    }
    for ($r = 0; $r -lt $Rows.Count; $r++) {
        $row = $Rows[$r]
        for ($c = 0; $c -lt $Headers.Count; $c++) {
            Set-Cell $table.Cell($r + 2, $c + 1) ([string]$row[$c]) 0 8.5
        }
    }
    $table.AutoFitBehavior(1)
    $Doc.Paragraphs.Add().Range.InsertParagraphAfter()
}

$bom = @(
    @('工作台', 'Plate 6x12', '浅灰', '1', '桌面主板'),
    @('工作台', 'Plate 4x12', '浅灰', '2', '桌面扩展'),
    @('工作台', 'Plate 2x12 / 2x8', '浅灰', '5', '桌沿与压缝'),
    @('工作台', 'Tile 2x8 / 2x6', '浅灰', '4', '外露桌沿收口'),
    @('工作台', 'Brick 2x2 / 2x4', '深灰/浅灰', '20', '桌腿与承重柱'),
    @('工作台', 'Plate 2x4', '深灰', '6', '腿部横向加固'),
    @('工作台', 'Tile 或 Plate 4x6', '白色', '4', '桌面白色操作区'),
    @('控制屏', 'Plate 4x8', '深灰', '1', '竖向屏幕主体'),
    @('控制屏', 'Round Plate / Tile 1x1', '浅灰/深灰', '18', '按钮与指示灯'),
    @('控制屏', 'Slope 1x2 45°', '深灰', '4', '下半区操作键'),
    @('椅座', 'Plate 6x8', '深灰', '1', '座面基础'),
    @('椅座', 'Plate 4x8', '深灰', '2', '座面加宽'),
    @('椅座', 'Curved Slope 2x4', '深灰/黑', '6', '两侧弧形护板'),
    @('椅座', 'Panel 6x6 或 6x8', '深灰', '3-4', '高靠背'),
    @('椅座', 'Hinge Plate 1x2', '深灰', '6', '靠背后仰角'),
    @('椅座', 'Small Wheel + Holder', '黑/浅灰', '8+4', '办公椅轮组'),
    @('机械龙', 'Plate 2x8 / 2x6 / 2x4', '橙色', '20', '身体主梁与分节外壳'),
    @('机械龙', 'Plate 1x4 / 1x2', '橙色', '28', '装甲压缝与细节'),
    @('机械龙', 'Tile 2x4 / 1x4', '橙色', '14', '顶部平滑装甲'),
    @('机械龙', 'Wedge Plate 左/右', '橙色', '8 对', '龙身收腰与头部轮廓'),
    @('机械龙', 'Curved Slope 2x2 / 2x4', '橙色', '14', '背弧与外壳弧面'),
    @('机械龙', 'Slope 2x2 45°', '橙色', '6', '头部楔形轮廓'),
    @('机械龙', 'Cone / Round Brick 1x1', '橙色', '12', '喷嘴与机械细节'),
    @('机械龙', 'Hinge / Ball Joint', '橙/黑', '16', '脊柱可调结构'),
    @('机械龙', 'Clip Plate + Bar 3L/4L', '橙色', '20', '爪、喷嘴、下挂机构'),
    @('连接件', 'Technic Brick / Pin / Axle', '深灰/黑/蓝', '24', '承重连接与铰接加固'),
    @('连接件', 'Jumper Plate 1x2', '各色', '10', '半格错位与微调')
)

$modules = @(
    @('桌面', '18 x 14 studs', '约 144 x 112 mm', '多块 plate 错缝压接，外圈 tile 表现厚桌沿'),
    @('控制屏', '4 x 8 studs', '约 32 x 64 mm', '背面双点固定，正面做按钮阵列'),
    @('椅座', '12 x 10 studs', '约 96 x 80 mm', '中心柱承重，小轮只做视觉表现'),
    @('靠背', '宽 6-8 studs，高 10-12 studs', '后仰约 10-15°', '用 panel 或 curved slope 做分段弧面'),
    @('机械龙', '长 24-28 studs', '约 192-224 mm', '6-7 个活动节形成向前伸展 S 曲线'),
    @('连接桥', '2 studs 宽，3 plates 厚', '约 16 mm 宽', '连接桌椅并承担抗弯')
)

$word = New-Object -ComObject Word.Application
$word.Visible = $false
$word.DisplayAlerts = 0

try {
    $doc = $word.Documents.Add()
    $doc.PageSetup.Orientation = 1
    $doc.PageSetup.TopMargin = $word.CentimetersToPoints(1.4)
    $doc.PageSetup.BottomMargin = $word.CentimetersToPoints(1.4)
    $doc.PageSetup.LeftMargin = $word.CentimetersToPoints(1.5)
    $doc.PageSetup.RightMargin = $word.CentimetersToPoints(1.5)

    [void](Add-Para -Doc $doc -Text '机械龙工作台积木玩具搭建说明书' -Size 24 -Bold 1 -After 4 -Color 8404992)
    [void](Add-Para -Doc $doc -Text 'V0.1 图片反推版｜按兼容积木玩具标准编写｜2026-05-07' -Size 11 -Bold 1 -After 12 -Color 5592405)
    [void](Add-Para -Doc $doc -Text '设计边界：本文件依据图片进行视觉比例反推，默认 1 stud = 8 mm。由于没有真实尺寸、CAD、零件库约束和实物受力数据，本文档为 MOC 概念搭建说明，不冒充量产认证文件。若进入打样，需要补做 3D 建模、碰撞检查、重心校验和安全测试。' -Size 10.5 -After 10)

    Add-Heading $doc '一、设计基准'
    [void](Add-Para -Doc $doc -Text '主题为橙色机械龙/机械臂伸向灰色工作台，右侧为深灰高靠背椅座和带轮机械底座。预计颗粒数约 260-320 件，推荐难度为中高阶 MOC，建议 10+。')
    [void](Add-Para -Doc $doc -Text '结构策略：工作台和椅座作为稳定底座；机械龙作为悬臂视觉主体；尾部、连接桥、椅背侧面至少三点受力，关键处用 Technic 轴或多点 plate 加固。')
    Add-Table -Doc $doc -Headers @('模块','建议积木尺寸','约合实物尺寸','设计说明') -Rows $modules

    Add-Heading $doc '二、零件清单 BOM'
    [void](Add-Para -Doc $doc -Text '下表为首版可搭建清单。数量按视觉还原和结构冗余预留估算，打样时建议额外准备 5%-10% 的 1x2、1x4、clip、hinge、jumper 和 Technic pin。')
    Add-Table -Doc $doc -Headers @('模块','通用零件','颜色','数量','用途 / 备注') -Rows $bom

    Add-Heading $doc '三、搭建说明'
    [void](Add-Para -Doc $doc -Text 'A. 工作台：用 6x12、4x12 和 2x12 plate 拼出桌面；下方用 2x8、2x4 plate 横向压缝；四条桌腿用 2x2 与 2x4 brick 叠到约 6 bricks 高；桌面外圈铺浅灰 tile，中间放白色操作区。')
    [void](Add-Para -Doc $doc -Text 'B. 控制屏：以深灰 4x8 plate 为竖屏主体，背面加 1x8 和 1x6 plate 做外框；正面用 1x1 round plate/tile 排按钮；底部通过 bracket 或 clip 插入桌面，背后再加两根 bar 支撑。')
    [void](Add-Para -Doc $doc -Text 'C. 椅座：用 6x8 和 4x8 plate 叠出座面，黑色 curved slope 做两侧包覆；靠背用 panel 或 curved slope 竖起，通过 hinge plate 后仰约 10-15°；底部中心柱承重，轮组只做视觉。')
    [void](Add-Para -Doc $doc -Text 'D. 机械龙背弧：从椅背右后侧开始布置 6-7 个 hinge 或 ball joint 节点，形成向前上方弯出的 S 形脊柱；外侧覆盖橙色 curved slope 和 wedge plate，内侧用黑色 dish 或圆弧表现机械骨架。')
    [void](Add-Para -Doc $doc -Text 'E. 龙头与喷嘴：用 2x8 plate 做龙身主梁，两侧 wedge plate 收窄；头部由 slope、wedge plate、cylinder 组成楔形轮廓；透明黄 1x1 stud 可作为眼部高光；喷嘴用 bar、cone、round brick 组合，采用 clip + stud 双固定。')
    [void](Add-Para -Doc $doc -Text 'F. 桌椅连接桥：用橙色 2x8 plate 两层叠加，中间错缝压 2x4 plate，形成 2 studs 宽、3 plates 厚的梁。一端固定在桌面下方加强筋，另一端接椅座前方接口。')

    Add-Heading $doc '四、结构校核'
    [void](Add-Para -Doc $doc -Text '1. 悬臂下垂：不要靠夹紧铰链硬撑，应增加尾部、连接桥和椅背侧面的受力点。')
    [void](Add-Para -Doc $doc -Text '2. 桌面开裂：大桌面必须错缝压板，外圈 tile 只做饰面，不承担主连接。')
    [void](Add-Para -Doc $doc -Text '3. 控制屏晃动：竖屏底部至少 2 studs 插接，背部增加两根支撑杆。')
    [void](Add-Para -Doc $doc -Text '4. 玩具安全：喷嘴、圆锥、1x1 小件存在误吞风险，建议标注 10+；量产前按目标市场做小零件、尖角、拉力、跌落和材料检测。')
    [void](Add-Para -Doc $doc -Text '5. 说明书标准：每步新增零件控制在 3-8 件；活动角度用箭头或角度标注；桌子、屏幕、椅座、龙体建议拆成独立分袋。')

    Add-Heading $doc '五、下一阶段'
    [void](Add-Para -Doc $doc -Text '建议用 Studio/LDraw/Blender 或企业零件库建立数字模型，检查非法连接、碰撞、重心和装配顺序。若提供目标成品长宽高、年龄段、成本上限和目标颗粒数，可升级为 V1.0 打样版 BOM 与分步图说明书。')

    $doc.SaveAs([ref]$out, [ref]16)
    $doc.Close()
}
finally {
    $word.Quit()
}

Write-Host $out
