$ErrorActionPreference = 'Stop'

$outputDocx = Join-Path (Get-Location) '面阵相机延时一片与不延时取图帧率计算说明.docx'
$outputPdf = Join-Path (Get-Location) '面阵相机延时一片与不延时取图帧率计算说明_QA.pdf'

function Set-CellText {
    param(
        [Parameter(Mandatory=$true)] $Cell,
        [Parameter(Mandatory=$true)] [string] $Text,
        [int] $Bold = 0,
        [int] $Color = 0
    )
    $Cell.Range.Text = $Text
    $Cell.Range.Font.Name = 'Microsoft YaHei'
    $Cell.Range.Font.Size = 9.5
    $Cell.Range.Font.Bold = $Bold
    if ($Color -ne 0) { $Cell.Range.Font.Color = $Color }
    $Cell.VerticalAlignment = 1
}

function Add-Para {
    param(
        [Parameter(Mandatory=$true)] $Doc,
        [Parameter(Mandatory=$true)] [string] $Text,
        [int] $Size = 10,
        [int] $Bold = 0,
        [int] $SpaceAfter = 6,
        [int] $Color = 0
    )
    $p = $Doc.Paragraphs.Add()
    $p.Range.Text = $Text
    $p.Range.Font.Name = 'Microsoft YaHei'
    $p.Range.Font.Size = $Size
    $p.Range.Font.Bold = $Bold
    if ($Color -ne 0) { $p.Range.Font.Color = $Color }
    $p.Format.SpaceAfter = $SpaceAfter
    $p.Format.LineSpacingRule = 0
    $p.Range.InsertParagraphAfter()
    return $p
}

function Add-Heading {
    param(
        [Parameter(Mandatory=$true)] $Doc,
        [Parameter(Mandatory=$true)] [string] $Text
    )
    $p = Add-Para -Doc $Doc -Text $Text -Size 14 -Bold 1 -SpaceAfter 8 -Color 6299648
    $p.Range.Style = 'Heading 1'
}

function Add-Body {
    param(
        [Parameter(Mandatory=$true)] $Doc,
        [Parameter(Mandatory=$true)] [string] $Text
    )
    [void](Add-Para -Doc $Doc -Text $Text -Size 10.5 -Bold 0 -SpaceAfter 6)
}

function Add-FormulaBlock {
    param(
        [Parameter(Mandatory=$true)] $Doc,
        [Parameter(Mandatory=$true)] [string] $Text
    )
    $p = Add-Para -Doc $Doc -Text $Text -Size 9.5 -Bold 0 -SpaceAfter 8 -Color 3158064
    $p.Range.Font.Name = 'Consolas'
    $p.LeftIndent = 18
}

$word = New-Object -ComObject Word.Application
$word.Visible = $false
$word.DisplayAlerts = 0

try {
    $doc = $word.Documents.Add()
    $doc.PageSetup.TopMargin = $word.CentimetersToPoints(1.8)
    $doc.PageSetup.BottomMargin = $word.CentimetersToPoints(1.6)
    $doc.PageSetup.LeftMargin = $word.CentimetersToPoints(1.9)
    $doc.PageSetup.RightMargin = $word.CentimetersToPoints(1.9)

    $title = $doc.Paragraphs.Add()
    $title.Range.Text = '面阵相机延时一片与不延时取图 / 帧率计算说明'
    $title.Range.Font.Name = 'Microsoft YaHei'
    $title.Range.Font.Size = 20
    $title.Range.Font.Bold = 1
    $title.Range.Font.Color = 6299648
    $title.Alignment = 1
    $title.Format.SpaceAfter = 10
    $title.Range.InsertParagraphAfter()

    $sub = $doc.Paragraphs.Add()
    $sub.Range.Text = '适用范围：工位二 V角位置检测、工位三尺寸检测；仅讨论面阵相机。'
    $sub.Range.Font.Name = 'Microsoft YaHei'
    $sub.Range.Font.Size = 11
    $sub.Alignment = 1
    $sub.Format.SpaceAfter = 14
    $sub.Range.InsertParagraphAfter()

    $meta = $doc.Paragraphs.Add()
    $meta.Range.Text = '依据方案版本：HSXY2828-新宇-金羽-V角单片裁切机 V1.2    日期：2026-04-29'
    $meta.Range.Font.Name = 'Microsoft YaHei'
    $meta.Range.Font.Size = 9.5
    $meta.Alignment = 1
    $meta.Format.SpaceAfter = 18
    $meta.Range.InsertParagraphAfter()

    Add-Heading -Doc $doc -Text '一、结论'
    Add-Body -Doc $doc -Text '在 240ppm 条件下，单片节拍为 250ms，面阵相机每片触发 1 次，因此相机吞吐的理论最低触发频率为 4fps。'
    Add-Body -Doc $doc -Text '若结果不延时输出，工位二相机有效帧率需不低于 22.22fps，工位三相机有效帧率需不低于 15.38fps。当前方案 30.7fps 可以覆盖不延时要求，但工位二余量只有 4ms。'
    Add-Body -Doc $doc -Text '若客户允许结果延时一片输出，理论相机帧率只需满足每片一拍，即不低于 4fps；考虑曝光、传输、SDK取图等待、系统调度和现场波动，建议方案口径写为：面阵相机全幅有效帧率建议 ≥10fps，最低不可低于 6fps。'

    Add-Heading -Doc $doc -Text '二、基础参数'
    $range = $doc.Range($doc.Content.End - 1, $doc.Content.End - 1)
    $tbl = $doc.Tables.Add($range, 6, 4)
    $tbl.Style = 'Table Grid'
    $tbl.Range.Font.Name = 'Microsoft YaHei'
    $tbl.Range.Font.Size = 9.5
    $tbl.Borders.Enable = 1
    $headers = @('参数', '符号', '数值', '来源 / 说明')
    for ($i = 1; $i -le 4; $i++) {
        Set-CellText -Cell $tbl.Cell(1,$i) -Text $headers[$i-1] -Bold 1 -Color 16777215
        $tbl.Cell(1,$i).Shading.BackgroundPatternColor = 6299648
    }
    $rows = @(
        @('设备速度', 'P', '240ppm', '方案项目需求'),
        @('单片节拍', 'T_cycle', '60 / 240 = 0.25s = 250ms/片', '由设备速度换算'),
        @('面阵拍照次数', 'N', '1张/片', '工位二、三均为单相机/产品'),
        @('触发吞吐下限', 'F_piece', '1000 / 250 = 4fps', '每片至少拍到一张'),
        @('常规结果时间窗口', 'T_half', '250 / 2 = 125ms', '方案说明：常规单片给面阵相机的时间是节拍的1/2')
    )
    for ($r = 0; $r -lt $rows.Count; $r++) {
        for ($c = 0; $c -lt 4; $c++) {
            Set-CellText -Cell $tbl.Cell($r+2,$c+1) -Text $rows[$r][$c]
        }
    }
    $tbl.Columns.Item(1).Width = $word.CentimetersToPoints(3.0)
    $tbl.Columns.Item(2).Width = $word.CentimetersToPoints(2.4)
    $tbl.Columns.Item(3).Width = $word.CentimetersToPoints(5.0)
    $tbl.Columns.Item(4).Width = $word.CentimetersToPoints(6.6)
    $doc.Paragraphs.Add().Range.InsertParagraphAfter()

    Add-Heading -Doc $doc -Text '三、公式来源'
    Add-Body -Doc $doc -Text '视觉结果能否赶上节拍，核心看“取图时间 + 处理时间 + 通讯时间”是否小于允许结果时间窗口。'
    Add-FormulaBlock -Doc $doc -Text "1) 单片节拍：`nT_cycle = 60 / P = 60 / 240 = 0.25s = 250ms"
    Add-FormulaBlock -Doc $doc -Text "2) 每片触发吞吐下限：`nF_piece = N / T_cycle = 1 / 0.25 = 4fps"
    Add-FormulaBlock -Doc $doc -Text "3) 不延时允许时间：`nT_allow0 = T_cycle / 2 = 250 / 2 = 125ms"
    Add-FormulaBlock -Doc $doc -Text "4) 延时一片允许时间：`nT_allow1 = T_cycle / 2 + T_cycle = 125 + 250 = 375ms"
    Add-FormulaBlock -Doc $doc -Text "5) 最大允许取图时间：`nT_grab_max = T_allow - T_process_comm"
    Add-FormulaBlock -Doc $doc -Text "6) 由取图时间反推帧率：`nF_camera = 1000 / T_grab_max"
    Add-FormulaBlock -Doc $doc -Text "7) 最终帧率要求：`nF_required = max(F_camera, F_piece)"
    Add-Body -Doc $doc -Text '说明：延时一片降低的是结果输出时限压力，不降低“每片必须拍到图”的相机触发吞吐要求。'

    Add-Heading -Doc $doc -Text '四、不延时与延时一片对比'
    $range2 = $doc.Range($doc.Content.End - 1, $doc.Content.End - 1)
    $tbl2 = $doc.Tables.Add($range2, 5, 8)
    $tbl2.Style = 'Table Grid'
    $tbl2.Range.Font.Name = 'Microsoft YaHei'
    $tbl2.Range.Font.Size = 8.7
    $tbl2.Borders.Enable = 1
    $headers2 = @('工位', '处理+通讯', '不延时允许', '不延时最大取图', '不延时帧率要求', '延时一片允许', '延时一片理论帧率', '建议口径')
    for ($i = 1; $i -le 8; $i++) {
        Set-CellText -Cell $tbl2.Cell(1,$i) -Text $headers2[$i-1] -Bold 1 -Color 16777215
        $tbl2.Cell(1,$i).Shading.BackgroundPatternColor = 6299648
    }
    $rows2 = @(
        @('工位二 V角位置检测', '80ms', '125ms', '125 - 80 = 45ms', '1000 / 45 = 22.22fps', '375ms', 'max(1000/295, 4) = 4fps', '≥10fps'),
        @('工位三 尺寸检测', '60ms', '125ms', '125 - 60 = 65ms', '1000 / 65 = 15.38fps', '375ms', 'max(1000/315, 4) = 4fps', '≥10fps'),
        @('当前方案校验：工位二', '80ms', '125ms', '当前取图 41ms', '41 + 80 = 121ms', '余量 4ms', '延时后余量 254ms', '30.7fps 充足'),
        @('当前方案校验：工位三', '60ms', '125ms', '当前取图 41ms', '41 + 60 = 101ms', '余量 24ms', '延时后余量 274ms', '30.7fps 充足')
    )
    for ($r = 0; $r -lt $rows2.Count; $r++) {
        for ($c = 0; $c -lt 8; $c++) {
            Set-CellText -Cell $tbl2.Cell($r+2,$c+1) -Text $rows2[$r][$c]
        }
    }
    $widths = @(2.9, 1.8, 1.8, 2.9, 3.2, 1.8, 3.3, 2.0)
    for ($i = 1; $i -le 8; $i++) {
        $tbl2.Columns.Item($i).Width = $word.CentimetersToPoints($widths[$i-1])
    }
    $doc.Paragraphs.Add().Range.InsertParagraphAfter()

    Add-Heading -Doc $doc -Text '五、客户沟通建议口径'
    Add-Body -Doc $doc -Text '在 240ppm 条件下，单片节拍为 250ms。若结果不延时输出，工位二面阵相机有效帧率需 ≥22.22fps，工位三需 ≥15.38fps，因此当前 30.7fps 选型满足不延时要求。'
    Add-Body -Doc $doc -Text '若结果允许延时一片输出，算法处理和通讯结果可跨一片节拍缓存，理论相机帧率只需满足每片一拍，即 ≥4fps。现场考虑取图链路波动，建议面阵相机全幅有效帧率按 ≥10fps 配置，最低不低于 6fps。'
    Add-Body -Doc $doc -Text '注意：这里仅讨论工位二、三面阵相机。线扫相机的行频直接影响运动方向像素精度，不能用“结果延时一片”来简单降低成像行频要求。'

    $footer = $doc.Sections.Item(1).Footers.Item(1).Range
    $footer.Text = '面阵相机取图/帧率计算说明'
    $footer.Font.Name = 'Microsoft YaHei'
    $footer.Font.Size = 8
    $footer.ParagraphFormat.Alignment = 1

    $doc.SaveAs([ref]$outputDocx, [ref]16)
    $doc.ExportAsFixedFormat($outputPdf, 17)
    $pageCount = $doc.ComputeStatistics(2)
    $doc.Close([ref]$false)
}
finally {
    $word.Quit()
    [System.Runtime.InteropServices.Marshal]::ReleaseComObject($word) | Out-Null
}

Write-Host "DOCX=$outputDocx"
Write-Host "PDF=$outputPdf"
Write-Host "PAGES=$pageCount"
