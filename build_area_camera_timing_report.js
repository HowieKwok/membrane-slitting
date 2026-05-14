const fs = require("fs");
const path = require("path");
const childProcess = require("child_process");

const root = process.cwd();
const buildDir = path.join(root, "_area_camera_timing_docx_build");
const zipPath = path.join(root, "_area_camera_timing_report.zip");
const outPath = path.join(root, "面阵相机延时一张取图后发送结果帧率计算详细报告.docx");

function rm(target) {
  if (fs.existsSync(target)) {
    fs.rmSync(target, { recursive: true, force: true });
  }
}

function ensureDir(target) {
  fs.mkdirSync(target, { recursive: true });
}

function esc(value) {
  return String(value)
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

function textRuns(text, opts = {}) {
  const lines = String(text).split("\n");
  return lines
    .map((line, index) => {
      const br = index === 0 ? "" : "<w:br/>";
      return `${br}<w:r>${runPr(opts)}<w:t xml:space="preserve">${esc(line)}</w:t></w:r>`;
    })
    .join("");
}

function runPr(opts = {}) {
  const color = opts.color ? `<w:color w:val="${opts.color}"/>` : "";
  const bold = opts.bold ? "<w:b/>" : "";
  const size = opts.size ? `<w:sz w:val="${opts.size}"/><w:szCs w:val="${opts.size}"/>` : "";
  const font = opts.font || "Microsoft YaHei";
  return `<w:rPr><w:rFonts w:ascii="${font}" w:eastAsia="${font}" w:hAnsi="${font}" w:cs="${font}"/>${bold}${color}${size}</w:rPr>`;
}

function para(text, opts = {}) {
  const align = opts.align ? `<w:jc w:val="${opts.align}"/>` : "";
  const style = opts.style ? `<w:pStyle w:val="${opts.style}"/>` : "";
  const before = opts.before || 0;
  const after = opts.after == null ? 120 : opts.after;
  const indent = opts.indent ? `<w:ind w:left="${opts.indent}"/>` : "";
  const keepNext = opts.keepNext ? "<w:keepNext/>" : "";
  const shading = opts.shading ? `<w:shd w:fill="${opts.shading}"/>` : "";
  const border =
    opts.borderTop || opts.borderBottom
      ? `<w:pBdr>${opts.borderTop ? '<w:top w:val="single" w:sz="8" w:space="1" w:color="2F5597"/>' : ""}${opts.borderBottom ? '<w:bottom w:val="single" w:sz="8" w:space="1" w:color="2F5597"/>' : ""}</w:pBdr>`
      : "";
  const pPr = `<w:pPr>${style}${keepNext}<w:spacing w:before="${before}" w:after="${after}" w:line="300" w:lineRule="auto"/>${indent}${align}${shading}${border}</w:pPr>`;
  return `<w:p>${pPr}${textRuns(text, opts)}</w:p>`;
}

function heading(text) {
  return para(text, { style: "Heading1", size: 28, bold: true, color: "1F4E79", after: 120, keepNext: true });
}

function subheading(text) {
  return para(text, { style: "Heading2", size: 23, bold: true, color: "2F5597", before: 80, after: 80, keepNext: true });
}

function formula(text) {
  return para(text, {
    font: "Consolas",
    size: 19,
    color: "1F1F1F",
    indent: 360,
    shading: "EAF2F8",
    before: 20,
    after: 80,
  });
}

function cell(content, opts = {}) {
  const width = opts.width || 2000;
  const fill = opts.fill ? `<w:shd w:fill="${opts.fill}"/>` : "";
  const vAlign = `<w:vAlign w:val="center"/>`;
  const tcPr = `<w:tcPr><w:tcW w:w="${width}" w:type="dxa"/>${fill}${vAlign}<w:tcMar><w:top w:w="90" w:type="dxa"/><w:left w:w="90" w:type="dxa"/><w:bottom w:w="90" w:type="dxa"/><w:right w:w="90" w:type="dxa"/></w:tcMar></w:tcPr>`;
  const align = opts.align || "center";
  const textOpts = {
    size: opts.size || 18,
    bold: opts.bold || false,
    color: opts.color || "000000",
    align,
    after: 0,
  };
  return `<w:tc>${tcPr}${para(content, textOpts)}</w:tc>`;
}

function table(headers, rows, widths) {
  const borders = `
    <w:tblBorders>
      <w:top w:val="single" w:sz="6" w:space="0" w:color="B7B7B7"/>
      <w:left w:val="single" w:sz="6" w:space="0" w:color="B7B7B7"/>
      <w:bottom w:val="single" w:sz="6" w:space="0" w:color="B7B7B7"/>
      <w:right w:val="single" w:sz="6" w:space="0" w:color="B7B7B7"/>
      <w:insideH w:val="single" w:sz="4" w:space="0" w:color="D9D9D9"/>
      <w:insideV w:val="single" w:sz="4" w:space="0" w:color="D9D9D9"/>
    </w:tblBorders>`;
  const tblGrid = `<w:tblGrid>${widths.map((w) => `<w:gridCol w:w="${w}"/>`).join("")}</w:tblGrid>`;
  const headerRow = `<w:tr>${headers
    .map((h, i) => cell(h, { width: widths[i], fill: "1F4E79", bold: true, color: "FFFFFF", size: 17 }))
    .join("")}</w:tr>`;
  const bodyRows = rows
    .map(
      (row, r) =>
        `<w:tr>${row
          .map((value, i) =>
            cell(value, {
              width: widths[i],
              fill: r % 2 === 0 ? "FFFFFF" : "F8FAFC",
              align: i === 0 || String(value).length > 18 ? "left" : "center",
              size: 17,
            })
          )
          .join("")}</w:tr>`
    )
    .join("");
  return `<w:tbl><w:tblPr><w:tblW w:w="0" w:type="auto"/><w:tblLayout w:type="fixed"/>${borders}</w:tblPr>${tblGrid}${headerRow}${bodyRows}</w:tbl>${para("", { after: 120 })}`;
}

function pageBreak() {
  return '<w:p><w:r><w:br w:type="page"/></w:r></w:p>';
}

const summaryRows = [
  ["工位二 V角位置检测", "A+C=80ms", "≥22.22fps", "≥26.67fps，建议 ≥27fps", "A≤125ms，满足", "取决于通讯 C：F≥1.2×1000/(125-C)", "30.7fps 满足"],
  ["工位三 尺寸检测", "A+C=60ms", "≥15.38fps", "≥18.46fps，建议 ≥19fps", "A≤125ms，满足", "取决于通讯 C：F≥1.2×1000/(125-C)", "30.7fps 满足"],
];

const paramRows = [
  ["面阵有效节拍", "T", "125ms", "按方案中“常规单片给面阵相机的时间是节拍的1/2”计算；240ppm 对应整机单片 250ms，面阵工位有效计算窗口取 125ms。"],
  ["取图时间", "G", "待反推", "包含曝光、传感器读出、传输、SDK等待到图像可用的完整时间。"],
  ["算法处理时间", "A", "包含在方案给定值内", "图像生成后到软件内部得到判定/坐标结果的时间。"],
  ["通讯时间", "C", "包含在方案给定值内", "软件发送结果到 PLC/设备端收到结果的时间；本报告以 PLC/设备端收到结果作为结果完成节点。"],
  ["处理+通讯时间", "A+C", "工位二 80ms", "方案总结中工位二照片处理+通讯时间，计算时必须包含通讯。"],
  ["处理+通讯时间", "A+C", "工位三 60ms", "方案总结中工位三照片处理+通讯时间，计算时必须包含通讯。"],
  ["帧率余量系数", "K", "1.2", "相机选型或方案承诺时，帧率至少按理论值乘 1.2。"],
  ["当前方案相机帧率", "F_current", "30.7fps", "方案中 MV-HSH4096-16UM 全幅帧率。"],
];

const calcRows = [
  ["工位二", "125 - 80 = 45ms", "1000 / 45 = 22.22fps", "22.22 × 1.2 = 26.67fps", "A≤80ms≤125ms", "Gmax = 125 - C", "Fselect≥1.2×1000/(125-C)"],
  ["工位三", "125 - 60 = 65ms", "1000 / 65 = 15.38fps", "15.38 × 1.2 = 18.46fps", "A≤60ms≤125ms", "Gmax = 125 - C", "Fselect≥1.2×1000/(125-C)"],
];

const currentRows = [
  ["工位二不延时", "30.7 / 22.22 = 1.38", "≥1.2", "满足，30.7fps 大于 26.67fps"],
  ["工位三不延时", "30.7 / 15.38 = 2.00", "≥1.2", "满足，30.7fps 大于 18.46fps"],
  ["延时一张取图后发送", "30.7 ≥ 1.2×1000/(125-C)", "需 C≤85.91ms", "工位二 C≤80ms、工位三 C≤60ms，因此按方案总耗时上限也满足"],
];

const timelineCompareRows = [
  ["不延时", "第 n 片触发，开始取图", "第 n 片图像完成，开始算法处理", "算法完成，开始通讯", "PLC/设备端收到第 n 片结果", "结果必须在 t=T 前收到", "G + A + C ≤ T"],
  ["延时一张取图后发送", "第 n 片触发，开始取图", "第 n 片图像完成，开始算法处理", "第 n+1 片图像完成后，开始发送第 n 片结果", "PLC/设备端收到第 n 片结果", "结果必须在下一张节拍内完成，即 t≤2T", "T + G + C ≤ 2T ⇒ G + C ≤ T"],
];

const noDelayTimelineRows = [
  ["1", "t = 0", "第 n 片触发", "相机开始曝光/读出/传输", "开始计取图时间 G"],
  ["2", "t = G", "第 n 片图像可用", "算法开始处理", "开始计 A"],
  ["3", "t = G + A", "第 n 片软件结果完成", "开始通讯到 PLC/设备端", "开始计 C"],
  ["4", "t = G + A + C", "PLC/设备端收到第 n 片结果", "这才算真正给到设备结果", "必须 ≤ T"],
  ["5", "t = T = 125ms", "当前片结果窗口截止", "若 G + A + C 超过 T，则结果不满足不延时", "Gmax = T - (A+C)"],
];

const delayTimelineRows = [
  ["1", "t = 0", "第 n 片触发", "相机开始取第 n 片", "开始计 G"],
  ["2", "t = G", "第 n 片图像可用", "算法开始处理", "开始计 A"],
  ["3", "t = G + A", "第 n 片软件结果完成", "结果先缓存，不立即通讯", "需在下一张图像生成前准备好，即 A≤T"],
  ["4", "t = T = 125ms", "第 n+1 片触发", "相机开始取下一张", "下一张同样需要 G"],
  ["5", "t = T + G", "第 n+1 片图像生成", "此时开始发送第 n 片结果", "开始计通讯时间 C"],
  ["6", "t = T + G + C", "PLC/设备端收到第 n 片结果", "通讯时间必须计入", "必须 ≤ 2T"],
  ["7", "化简", "T + G + C ≤ 2T", "延时一张取图后的核心约束", "G + C ≤ T；同时 A≤T"],
];

const commRows = [
  ["0ms", "125ms", "8.00fps", "9.60fps", "≥10fps", "仅理论通信为0时"],
  ["5ms", "120ms", "8.33fps", "10.00fps", "≥10fps", "10fps刚好满足1.2余量"],
  ["10ms", "115ms", "8.70fps", "10.43fps", "≥11fps", "通讯超过5ms后，10fps不再满足1.2余量"],
  ["20ms", "105ms", "9.52fps", "11.43fps", "≥12fps", "常规建议上取整"],
  ["30ms", "95ms", "10.53fps", "12.63fps", "≥13fps", "需留发送窗口"],
  ["40ms", "85ms", "11.76fps", "14.12fps", "≥15fps", "通讯越长，帧率要求越高"],
  ["60ms", "65ms", "15.38fps", "18.46fps", "≥19fps", "等同工位三保守上限 C=A+C"],
  ["80ms", "45ms", "22.22fps", "26.67fps", "≥27fps", "等同工位二保守上限 C=A+C"],
];

const body = [
  para("面阵相机延时一张取图后发送结果 / 帧率计算详细报告", {
    size: 38,
    bold: true,
    color: "1F4E79",
    align: "center",
    after: 160,
  }),
  para("适用范围：工位二 V角位置检测、工位三尺寸检测；仅讨论面阵相机", {
    size: 22,
    align: "center",
    color: "404040",
    after: 100,
  }),
  para("依据方案：HSXY2828-新宇-金羽-V角单片裁切机 V1.2    计算日期：2026-04-29", {
    size: 18,
    align: "center",
    color: "666666",
    after: 240,
  }),
  heading("一、计算结论"),
  para("本次计算按面阵相机有效节拍 T = 125ms 进行，不再按整机 250ms 作为面阵工位的结果窗口。客户口径为“延时一张取图，下一张图像生成后再发送上一张结果”，因此延时模式下必须满足下一张取图生成时间 G 加通讯发送时间 C 小于 125ms。帧率要求在理论值基础上至少乘 1.2 余量。", {
    size: 21,
    after: 80,
  }),
  table(
    ["工位", "处理+通讯", "不延时理论帧率", "不延时含1.2余量", "延时一片处理+通讯约束", "延时一片含1.2余量", "当前30.7fps校验"],
    summaryRows,
    [2300, 1300, 1900, 2500, 1900, 2500, 2000]
  ),
  para("结论口径：不延时输出时，工位二建议相机有效帧率 ≥27fps，工位三建议 ≥19fps。若采用“延时一张取图后再发送结果”，则不能只写固定 ≥10fps，必须结合通讯时间 C 计算：G + C ≤ 125ms，Fselect ≥ 1.2 × 1000 / (125 - C)。当 C≤5ms 时建议 ≥10fps；若 C 更长，帧率要求需按表上调。", {
    size: 21,
    bold: true,
    color: "1F4E79",
    borderTop: true,
    borderBottom: true,
    before: 80,
    after: 160,
  }),
  heading("二、计算前提与参数定义"),
  table(["参数", "符号", "数值", "说明"], paramRows, [2600, 1400, 2400, 9000]),
  heading("三、时序图对比"),
  para("下面的时序图按面阵有效节拍 T = 125ms 表达。G 表示完整取图时间，A 表示算法处理时间，C 表示通讯发送时间。延时模式的关键点是：下一张图像生成后才开始发送上一张结果，因此发送 C 必须落在下一张节拍剩余窗口内。", { size: 20, after: 80 }),
  table(
    ["场景", "t = 0", "t = G", "t = G + A", "t = G + A + C", "截止点", "核心约束"],
    timelineCompareRows,
    [1200, 2100, 2300, 2000, 2400, 2800, 2300]
  ),
  subheading("1. 不延时输出时序"),
  table(
    ["步骤", "时间点", "事件", "含义", "约束/计算"],
    noDelayTimelineRows,
    [900, 2200, 3100, 4500, 4200]
  ),
  subheading("2. 延时一张取图后发送结果时序"),
  table(
    ["步骤", "时间点", "事件", "含义", "约束/计算"],
    delayTimelineRows,
    [900, 2200, 3100, 4500, 4200]
  ),
  heading("四、公式推导"),
  subheading("1. 不延时给结果"),
  para("不延时表示当前片图像完成取图后，处理和通讯也必须在当前面阵有效节拍内完成。", { size: 20, after: 60 }),
  formula("T = 125ms\nG = 取图时间\nR = 处理时间 + 通讯时间\n不延时约束：G + R ≤ T\n最大允许取图时间：Gmax = T - R\n理论最低帧率：Fbase = 1000 / Gmax\n含余量帧率：Fselect ≥ Fbase × 1.2"),
  subheading("2. 延时一张取图后发送结果"),
  para("延时一张取图后发送结果表示：第 n 片结果先在软件内缓存；第 n+1 片触发并完成取图生成图片后，才开始发送第 n 片结果。因此下一张取图时间 G 和发送通讯时间 C 必须共同小于一个面阵有效节拍 T。", {
    size: 20,
    after: 60,
  }),
  formula("第 n 片触发：t = 0\n第 n 片图像完成：t = G\n第 n 片软件结果完成并缓存：t = G + A\n第 n+1 片触发：t = T\n第 n+1 片图像生成：t = T + G\n开始发送第 n 片结果：t = T + G\nPLC/设备端收到第 n 片结果：t = T + G + C\n延时一张约束：T + G + C ≤ 2T\n化简后：G + C ≤ T"),
  para("因此延时一张取图后再发送时，真正占用下一张节拍的是下一张图像生成时间 G 和结果发送通讯时间 C。算法处理 A 需要在发送前准备好，保守要求 A≤T；但帧率反推时，核心取图窗口为 Gmax = T - C。", {
    size: 20,
    after: 120,
  }),
  heading("五、工位二 / 工位三详细计算"),
  table(
    ["工位", "不延时 Gmax", "不延时 Fbase", "不延时 Fselect", "延时处理约束", "延时 Fbase", "延时 Fselect"],
    calcRows,
    [1500, 2200, 2300, 2600, 2400, 2000, 2200]
  ),
  heading("六、通讯时间 C 对延时一张帧率的影响"),
  para("延时一张取图后发送结果时，帧率不能脱离通讯时间 C 单独给值。计算公式如下：", { size: 20, after: 60 }),
  formula("G + C ≤ 125ms\nGmax = 125 - C\nFbase = 1000 / Gmax\nFselect ≥ Fbase × 1.2 = 1.2 × 1000 / (125 - C)"),
  table(
    ["通讯 C", "允许取图 Gmax", "理论 Fbase", "含1.2余量 Fselect", "建议写法", "说明"],
    commRows,
    [1300, 2200, 2100, 2500, 1800, 5200]
  ),
  heading("七、当前方案 30.7fps 余量校验"),
  table(["场景", "实际余量倍数", "要求", "判断"], currentRows, [2800, 3000, 1800, 7100]),
  heading("八、建议写入方案 / 回复客户的文字"),
  para("按面阵相机有效节拍 125ms 计算：不延时输出时，工位二相机有效帧率需 ≥22.22fps，考虑 1.2 倍余量后建议 ≥27fps；工位三相机有效帧率需 ≥15.38fps，考虑 1.2 倍余量后建议 ≥19fps。", {
    size: 20,
    after: 80,
  }),
  para("若客户采用“延时一张取图，下一张图像生成后再发送上一张结果”的方式，则发送通讯时间必须计入下一张节拍窗口。约束为 T + G + C ≤ 2T，即 G + C ≤ 125ms。相机帧率建议按 Fselect ≥ 1.2 × 1000 / (125 - C) 计算；例如通讯 C=5ms 时建议 ≥10fps，C=20ms 时建议 ≥12fps，C=40ms 时建议 ≥15fps。", {
    size: 20,
    bold: true,
    color: "1F4E79",
    after: 100,
  }),
  heading("九、工程注意事项"),
  para("1. 报告中的帧率必须理解为有效取图帧率，需包含曝光、读出、传输、SDK 等待到图像可处理的完整链路，不应只引用相机手册中的理想最大帧率。", { size: 19, after: 60 }),
  para("2. 延时一张取图后发送结果时，通讯 C 不能省略；如果只按 G≤125ms 推出 ≥10fps，会在 C>5ms 时不满足 1.2 倍帧率余量。", { size: 19, after: 60 }),
  para("3. 如果后续软件输出节点不是“下一张图像生成后开始发送”，而是更早或更晚的 PLC 取数节点，需按实际握手时刻重新代入时间轴。", { size: 19, after: 60 }),
  para("4. 工位二、工位三当前处理+通讯时间分别为 80ms、60ms，但方案未拆分算法 A 和通讯 C。若要给出严格帧率值，需要实测或约定通讯 C；若无法拆分，保守可将 C 按 A+C 上限估算，则工位二仍需 ≥27fps，工位三仍需 ≥19fps。", { size: 19, after: 60 }),
  para("5. 本报告只讨论工位二、三面阵相机。线扫相机行频直接影响运动方向采样精度，不能用结果延时一片来简单降低线扫成像行频。", { size: 19, after: 60 }),
].join("");

const documentXml = `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<w:document xmlns:wpc="http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas"
  xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
  xmlns:o="urn:schemas-microsoft-com:office:office"
  xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"
  xmlns:m="http://schemas.openxmlformats.org/officeDocument/2006/math"
  xmlns:v="urn:schemas-microsoft-com:vml"
  xmlns:wp14="http://schemas.microsoft.com/office/word/2010/wordprocessingDrawing"
  xmlns:wp="http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing"
  xmlns:w10="urn:schemas-microsoft-com:office:word"
  xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main"
  xmlns:w14="http://schemas.microsoft.com/office/word/2010/wordml"
  xmlns:wpg="http://schemas.microsoft.com/office/word/2010/wordprocessingGroup"
  xmlns:wpi="http://schemas.microsoft.com/office/word/2010/wordprocessingInk"
  xmlns:wne="http://schemas.microsoft.com/office/word/2006/wordml"
  xmlns:wps="http://schemas.microsoft.com/office/word/2010/wordprocessingShape"
  mc:Ignorable="w14 wp14">
  <w:body>
    ${body}
    <w:sectPr>
      <w:pgSz w:w="16838" w:h="11906" w:orient="landscape"/>
      <w:pgMar w:top="900" w:right="540" w:bottom="720" w:left="540" w:header="360" w:footer="360" w:gutter="0"/>
      <w:cols w:space="720"/>
      <w:docGrid w:linePitch="312"/>
    </w:sectPr>
  </w:body>
</w:document>`;

const stylesXml = `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<w:styles xmlns:w="http://schemas.openxmlformats.org/wordprocessingml/2006/main">
  <w:docDefaults>
    <w:rPrDefault>
      <w:rPr>
        <w:rFonts w:ascii="Microsoft YaHei" w:eastAsia="Microsoft YaHei" w:hAnsi="Microsoft YaHei" w:cs="Microsoft YaHei"/>
        <w:sz w:val="21"/><w:szCs w:val="21"/>
      </w:rPr>
    </w:rPrDefault>
    <w:pPrDefault><w:pPr><w:spacing w:after="120" w:line="300" w:lineRule="auto"/></w:pPr></w:pPrDefault>
  </w:docDefaults>
  <w:style w:type="paragraph" w:default="1" w:styleId="Normal">
    <w:name w:val="Normal"/><w:qFormat/>
    <w:rPr><w:rFonts w:ascii="Microsoft YaHei" w:eastAsia="Microsoft YaHei" w:hAnsi="Microsoft YaHei" w:cs="Microsoft YaHei"/></w:rPr>
  </w:style>
  <w:style w:type="paragraph" w:styleId="Heading1">
    <w:name w:val="heading 1"/><w:basedOn w:val="Normal"/><w:next w:val="Normal"/><w:qFormat/>
    <w:pPr><w:keepNext/><w:spacing w:before="160" w:after="120"/></w:pPr>
    <w:rPr><w:b/><w:color w:val="1F4E79"/><w:sz w:val="28"/><w:szCs w:val="28"/></w:rPr>
  </w:style>
  <w:style w:type="paragraph" w:styleId="Heading2">
    <w:name w:val="heading 2"/><w:basedOn w:val="Normal"/><w:next w:val="Normal"/><w:qFormat/>
    <w:pPr><w:keepNext/><w:spacing w:before="120" w:after="80"/></w:pPr>
    <w:rPr><w:b/><w:color w:val="2F5597"/><w:sz w:val="23"/><w:szCs w:val="23"/></w:rPr>
  </w:style>
</w:styles>`;

const relsRoot = `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument" Target="word/document.xml"/>
  <Relationship Id="rId2" Type="http://schemas.openxmlformats.org/package/2006/relationships/metadata/core-properties" Target="docProps/core.xml"/>
  <Relationship Id="rId3" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/extended-properties" Target="docProps/app.xml"/>
</Relationships>`;

const relsDoc = `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Relationships xmlns="http://schemas.openxmlformats.org/package/2006/relationships">
  <Relationship Id="rId1" Type="http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles" Target="styles.xml"/>
</Relationships>`;

const contentTypes = `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Types xmlns="http://schemas.openxmlformats.org/package/2006/content-types">
  <Default Extension="rels" ContentType="application/vnd.openxmlformats-package.relationships+xml"/>
  <Default Extension="xml" ContentType="application/xml"/>
  <Override PartName="/word/document.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.document.main+xml"/>
  <Override PartName="/word/styles.xml" ContentType="application/vnd.openxmlformats-officedocument.wordprocessingml.styles+xml"/>
  <Override PartName="/docProps/core.xml" ContentType="application/vnd.openxmlformats-package.core-properties+xml"/>
  <Override PartName="/docProps/app.xml" ContentType="application/vnd.openxmlformats-officedocument.extended-properties+xml"/>
</Types>`;

const coreXml = `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<cp:coreProperties xmlns:cp="http://schemas.openxmlformats.org/package/2006/metadata/core-properties" xmlns:dc="http://purl.org/dc/elements/1.1/" xmlns:dcterms="http://purl.org/dc/terms/" xmlns:dcmitype="http://purl.org/dc/dcmitype/" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
  <dc:title>面阵相机延时一片与不延时取图帧率计算详细报告</dc:title>
  <dc:creator>Codex</dc:creator>
  <cp:lastModifiedBy>Codex</cp:lastModifiedBy>
  <dcterms:created xsi:type="dcterms:W3CDTF">2026-04-29T00:00:00Z</dcterms:created>
  <dcterms:modified xsi:type="dcterms:W3CDTF">2026-04-29T00:00:00Z</dcterms:modified>
</cp:coreProperties>`;

const appXml = `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Properties xmlns="http://schemas.openxmlformats.org/officeDocument/2006/extended-properties" xmlns:vt="http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes">
  <Application>WPS Office compatible OpenXML</Application>
  <DocSecurity>0</DocSecurity>
  <ScaleCrop>false</ScaleCrop>
  <Company>Huashi Vision</Company>
  <LinksUpToDate>false</LinksUpToDate>
  <SharedDoc>false</SharedDoc>
  <HyperlinksChanged>false</HyperlinksChanged>
  <AppVersion>12.0000</AppVersion>
</Properties>`;

rm(buildDir);
rm(zipPath);
if (fs.existsSync(outPath)) fs.rmSync(outPath, { force: true });

ensureDir(path.join(buildDir, "_rels"));
ensureDir(path.join(buildDir, "word", "_rels"));
ensureDir(path.join(buildDir, "docProps"));

fs.writeFileSync(path.join(buildDir, "[Content_Types].xml"), contentTypes, "utf8");
fs.writeFileSync(path.join(buildDir, "_rels", ".rels"), relsRoot, "utf8");
fs.writeFileSync(path.join(buildDir, "word", "document.xml"), documentXml, "utf8");
fs.writeFileSync(path.join(buildDir, "word", "styles.xml"), stylesXml, "utf8");
fs.writeFileSync(path.join(buildDir, "word", "_rels", "document.xml.rels"), relsDoc, "utf8");
fs.writeFileSync(path.join(buildDir, "docProps", "core.xml"), coreXml, "utf8");
fs.writeFileSync(path.join(buildDir, "docProps", "app.xml"), appXml, "utf8");

childProcess.execFileSync(
  "powershell.exe",
  [
    "-NoProfile",
    "-Command",
    `Add-Type -AssemblyName System.IO.Compression.FileSystem; if (Test-Path -LiteralPath '${zipPath}') { Remove-Item -LiteralPath '${zipPath}' -Force }; [System.IO.Compression.ZipFile]::CreateFromDirectory('${buildDir}', '${zipPath}')`,
  ],
  { stdio: "inherit" }
);

fs.renameSync(zipPath, outPath);
console.log(outPath);
