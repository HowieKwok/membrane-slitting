const fs = require("fs");
const path = require("path");

const root = process.cwd();
const outDir = path.join(root, "积木机械龙图文说明_V0.2");
const assetsDir = path.join(outDir, "assets");

fs.mkdirSync(assetsDir, { recursive: true });

const bom = [
  ["模块", "通用零件", "颜色", "数量", "用途/备注"],
  ["工作台", "Plate 6x12", "浅灰", "1", "桌面主板"],
  ["工作台", "Plate 4x12", "浅灰", "2", "桌面扩展"],
  ["工作台", "Plate 2x12 / 2x8", "浅灰", "5", "桌沿与压缝"],
  ["工作台", "Tile 2x8 / 2x6", "浅灰", "4", "外露桌沿收口"],
  ["工作台", "Brick 2x2 / 2x4", "深灰/浅灰", "20", "桌腿与承重柱"],
  ["工作台", "Plate 2x4", "深灰", "6", "腿部横向加固"],
  ["工作台", "Tile 或 Plate 4x6", "白色", "4", "桌面白色操作区"],
  ["控制屏", "Plate 4x8", "深灰", "1", "竖向屏幕主体"],
  ["控制屏", "Plate 1x8 / 1x6", "深灰", "4", "屏幕背框"],
  ["控制屏", "Round Plate / Tile 1x1", "浅灰/深灰", "18", "按钮与指示灯"],
  ["控制屏", "Slope 1x2 45°", "深灰", "4", "下半区操作键"],
  ["控制屏", "Clip Plate + Bar 4L", "深灰/浅灰", "6", "屏幕背部支撑"],
  ["椅座", "Plate 6x8", "深灰", "1", "座面基础"],
  ["椅座", "Plate 4x8", "深灰", "2", "座面加宽"],
  ["椅座", "Curved Slope 2x4", "深灰/黑", "6", "两侧弧形护板"],
  ["椅座", "Panel 6x6 或 6x8", "深灰", "4", "高靠背"],
  ["椅座", "Hinge Plate 1x2", "深灰", "6", "靠背后仰角"],
  ["椅座", "Small Wheel + Holder", "黑/浅灰", "8+4", "办公椅轮组"],
  ["椅座", "Round Brick 2x2 / Dish 4x4", "浅灰", "3", "中心柱与十字脚"],
  ["机械龙", "Plate 2x8 / 2x6 / 2x4", "橙色", "20", "身体主梁与分节外壳"],
  ["机械龙", "Plate 1x4 / 1x2", "橙色", "28", "装甲压缝与细节"],
  ["机械龙", "Tile 2x4 / 1x4", "橙色", "14", "顶部平滑装甲"],
  ["机械龙", "Wedge Plate 左/右", "橙色", "8 对", "龙身收腰与头部轮廓"],
  ["机械龙", "Curved Slope 2x2 / 2x4", "橙色", "14", "背弧与外壳弧面"],
  ["机械龙", "Slope 2x2 45°", "橙色", "6", "头部楔形轮廓"],
  ["机械龙", "Cone / Round Brick 1x1", "橙色", "12", "喷嘴与机械细节"],
  ["机械龙", "Cylinder 2x2", "橙色", "4", "头部与肩部圆筒"],
  ["机械龙", "Hinge / Ball Joint", "橙/黑", "16", "脊柱可调结构"],
  ["机械龙", "Clip Plate + Bar 3L/4L", "橙色", "20", "爪、喷嘴、下挂机构"],
  ["机械龙", "Round Tile / Dish 2x2-4x4", "黑色", "6", "侧向关节与内侧黑弧"],
  ["连接件", "Technic Brick 1x2 with Hole", "深灰/橙", "8", "承重点"],
  ["连接件", "Technic Pin / Axle 4L/6L", "黑/蓝/浅灰", "16", "铰接与加固"],
  ["连接件", "Jumper Plate 1x2", "各色", "10", "半格错位与微调"],
];

function esc(s) {
  return String(s).replace(/[&<>"']/g, c => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[c]));
}

function tableHtml(rows, cls = "") {
  return `<table class="${cls}">${rows.map((r, i) => `<tr>${r.map(v => `<${i === 0 ? "th" : "td"}>${esc(v)}</${i === 0 ? "th" : "td"}>`).join("")}</tr>`).join("")}</table>`;
}

function svgFrame(title, body) {
  return `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1100 620" role="img" aria-label="${esc(title)}">
  <defs>
    <marker id="arrow" viewBox="0 0 10 10" refX="9" refY="5" markerWidth="7" markerHeight="7" orient="auto-start-reverse">
      <path d="M0,0 L10,5 L0,10 z" fill="#1f4e79"/>
    </marker>
    <filter id="shadow" x="-10%" y="-10%" width="120%" height="120%"><feDropShadow dx="0" dy="3" stdDeviation="2" flood-opacity=".18"/></filter>
  </defs>
  <rect width="1100" height="620" fill="#f6f7f9"/>
  <text x="34" y="46" font-family="Microsoft YaHei, Arial" font-size="28" font-weight="700" fill="#1f4e79">${esc(title)}</text>
  ${body}
</svg>`;
}

function studRow(x, y, count, color = "#9aa1a6") {
  let s = "";
  for (let i = 0; i < count; i++) s += `<ellipse cx="${x + i * 32}" cy="${y}" rx="10" ry="6" fill="${color}" stroke="#60666a"/>`;
  return s;
}

const diagrams = [
  ["01_总装关系图.svg", svgFrame("总装关系图：先看模块位置和受力方向", `
    <rect x="70" y="315" width="390" height="55" rx="4" fill="#aeb4b8" stroke="#596168" filter="url(#shadow)"/>
    <rect x="95" y="370" width="55" height="145" fill="#7d8586" stroke="#4b5153"/>
    <rect x="340" y="370" width="55" height="145" fill="#7d8586" stroke="#4b5153"/>
    <rect x="165" y="285" width="150" height="72" fill="#ffffff" stroke="#9aa1a6"/>
    ${studRow(112, 333, 9)}
    <rect x="88" y="145" width="118" height="160" fill="#4d555b" stroke="#222" filter="url(#shadow)"/>
    <rect x="104" y="162" width="86" height="126" fill="#6f777b" stroke="#2c3236"/>
    ${studRow(126, 190, 3, "#c4c8ca")}${studRow(126, 236, 3, "#c4c8ca")}
    <rect x="450" y="334" width="180" height="38" fill="#ff8b1a" stroke="#9a4d00"/>
    <rect x="665" y="305" width="210" height="72" rx="6" fill="#525a56" stroke="#222" filter="url(#shadow)"/>
    <path d="M760 300 C770 220, 805 145, 880 78 L1010 150 L1015 385 L930 385 C905 300, 845 270, 760 300Z" fill="#5e625f" stroke="#222"/>
    <path d="M705 300 C735 215, 780 150, 835 108 C855 130, 875 150, 895 182 C850 225, 810 285, 800 370" fill="none" stroke="#ff8b1a" stroke-width="34" stroke-linecap="round"/>
    <path d="M705 300 C735 215, 780 150, 835 108 C855 130, 875 150, 895 182 C850 225, 810 285, 800 370" fill="none" stroke="#c05f00" stroke-width="3"/>
    <rect x="535" y="232" width="260" height="54" rx="20" fill="#ff8b1a" stroke="#9a4d00"/>
    <polygon points="505,240 545,210 615,220 615,282 545,292" fill="#ff8b1a" stroke="#9a4d00"/>
    <circle cx="575" cy="286" r="24" fill="#0d1b24" stroke="#777"/>
    <line x1="505" y1="260" x2="365" y2="260" stroke="#ff8b1a" stroke-width="18" stroke-linecap="round"/>
    <line x1="505" y1="278" x2="392" y2="298" stroke="#ff8b1a" stroke-width="14" stroke-linecap="round"/>
    <line x1="468" y1="455" x2="612" y2="370" stroke="#1f4e79" stroke-width="3" marker-end="url(#arrow)"/>
    <text x="285" y="420" font-family="Microsoft YaHei" font-size="22" fill="#1f4e79">橙色连接桥：结构抗弯件</text>
    <text x="690" y="455" font-family="Microsoft YaHei" font-size="22" fill="#1f4e79">椅座承重，轮子只作视觉</text>
    <text x="492" y="190" font-family="Microsoft YaHei" font-size="22" fill="#1f4e79">机械龙三点受力</text>
  `)],
  ["02_工作台和控制屏.svg", svgFrame("步骤 1-2：工作台与控制屏", `
    <rect x="120" y="285" width="470" height="70" fill="#b8bfc2" stroke="#596168" filter="url(#shadow)"/>
    <rect x="145" y="355" width="70" height="150" fill="#7d8586" stroke="#4b5153"/>
    <rect x="475" y="355" width="70" height="150" fill="#7d8586" stroke="#4b5153"/>
    <rect x="245" y="215" width="190" height="70" fill="#fff" stroke="#8b9296"/>
    ${studRow(155, 318, 12)}
    <text x="630" y="190" font-family="Microsoft YaHei" font-size="23" font-weight="700" fill="#333">压缝规则</text>
    <text x="630" y="232" font-family="Microsoft YaHei" font-size="20" fill="#333">1. 大板不要直线拼缝</text>
    <text x="630" y="268" font-family="Microsoft YaHei" font-size="20" fill="#333">2. 下方两条 2x8 加强筋</text>
    <text x="630" y="304" font-family="Microsoft YaHei" font-size="20" fill="#333">3. 桌腿底部横向锁住</text>
    <rect x="150" y="102" width="125" height="170" fill="#4d555b" stroke="#222" filter="url(#shadow)"/>
    <rect x="168" y="122" width="88" height="128" fill="#6f777b" stroke="#2c3236"/>
    ${studRow(190, 150, 2, "#cfd3d5")}${studRow(190, 194, 2, "#cfd3d5")}${studRow(190, 238, 2, "#cfd3d5")}
    <line x1="275" y1="190" x2="370" y2="235" stroke="#1f4e79" stroke-width="3" marker-end="url(#arrow)"/>
    <text x="385" y="235" font-family="Microsoft YaHei" font-size="20" fill="#1f4e79">背面加 bar 支撑</text>
  `)],
  ["03_椅座底盘.svg", svgFrame("步骤 3：椅座、高靠背与轮组", `
    <rect x="390" y="305" width="255" height="78" rx="6" fill="#555b58" stroke="#222" filter="url(#shadow)"/>
    <rect x="420" y="270" width="195" height="40" fill="#111d26" stroke="#222"/>
    <path d="M635 292 L862 112 L998 184 L1000 420 L850 420 L825 318 L690 360Z" fill="#656967" stroke="#222"/>
    <path d="M675 308 C710 260, 740 215, 765 160" fill="none" stroke="#858a86" stroke-width="18"/>
    <rect x="485" y="383" width="55" height="72" fill="#c7c9c8" stroke="#777"/>
    <line x1="512" y1="455" x2="380" y2="515" stroke="#c7c9c8" stroke-width="24" stroke-linecap="round"/>
    <line x1="512" y1="455" x2="642" y2="515" stroke="#c7c9c8" stroke-width="24" stroke-linecap="round"/>
    <line x1="512" y1="455" x2="512" y2="555" stroke="#c7c9c8" stroke-width="24" stroke-linecap="round"/>
    <circle cx="360" cy="523" r="26" fill="#111d26"/><circle cx="660" cy="523" r="26" fill="#111d26"/><circle cx="512" cy="570" r="26" fill="#111d26"/>
    <text x="100" y="170" font-family="Microsoft YaHei" font-size="22" fill="#333">靠背用 hinge 后仰 10-15°</text>
    <line x1="345" y1="170" x2="650" y2="285" stroke="#1f4e79" stroke-width="3" marker-end="url(#arrow)"/>
    <text x="100" y="228" font-family="Microsoft YaHei" font-size="22" fill="#333">中心柱承重，轮子不承重</text>
    <line x1="345" y1="228" x2="512" y2="455" stroke="#1f4e79" stroke-width="3" marker-end="url(#arrow)"/>
  `)],
  ["04_机械龙分节.svg", svgFrame("步骤 4-5：机械龙分节脊柱与头部", `
    <path d="M240 435 C300 300, 430 195, 585 140 C655 115, 735 135, 800 185" fill="none" stroke="#ff8b1a" stroke-width="38" stroke-linecap="round" filter="url(#shadow)"/>
    <path d="M240 435 C300 300, 430 195, 585 140 C655 115, 735 135, 800 185" fill="none" stroke="#9a4d00" stroke-width="3"/>
    ${[0,1,2,3,4,5,6].map((i)=>`<circle cx="${255+i*75}" cy="${420-70*Math.sin(i/6*Math.PI)}" r="20" fill="#111d26" stroke="#ddd"/>`).join("")}
    <rect x="660" y="245" width="245" height="62" rx="22" fill="#ff8b1a" stroke="#9a4d00"/>
    <polygon points="595,250 670,218 730,242 725,314 650,328" fill="#ff8b1a" stroke="#9a4d00"/>
    <circle cx="680" cy="316" r="27" fill="#0d1b24" stroke="#777"/>
    <line x1="600" y1="274" x2="445" y2="274" stroke="#ff8b1a" stroke-width="17" stroke-linecap="round"/>
    <line x1="610" y1="298" x2="475" y2="332" stroke="#ff8b1a" stroke-width="14" stroke-linecap="round"/>
    <rect x="540" y="210" width="140" height="34" fill="#ff8b1a" stroke="#9a4d00"/>
    <text x="100" y="105" font-family="Microsoft YaHei" font-size="22" fill="#333">6-7 个活动节形成 S 形曲线</text>
    <line x1="375" y1="105" x2="415" y2="295" stroke="#1f4e79" stroke-width="3" marker-end="url(#arrow)"/>
    <text x="760" y="110" font-family="Microsoft YaHei" font-size="22" fill="#333">喷嘴：clip + stud 双固定</text>
    <line x1="815" y1="125" x2="545" y2="276" stroke="#1f4e79" stroke-width="3" marker-end="url(#arrow)"/>
  `)],
  ["05_连接桥和受力.svg", svgFrame("步骤 6：连接桥与最终受力校核", `
    <rect x="130" y="345" width="315" height="52" fill="#aeb4b8" stroke="#596168"/>
    <rect x="515" y="355" width="230" height="58" fill="#555b58" stroke="#222"/>
    <rect x="400" y="360" width="160" height="34" fill="#ff8b1a" stroke="#9a4d00" filter="url(#shadow)"/>
    <text x="408" y="340" font-family="Microsoft YaHei" font-size="19" fill="#9a4d00">2 studs 宽，3 plates 厚</text>
    <path d="M645 330 C680 250, 750 160, 850 110" fill="none" stroke="#ff8b1a" stroke-width="32" stroke-linecap="round"/>
    <rect x="505" y="250" width="220" height="50" rx="18" fill="#ff8b1a" stroke="#9a4d00"/>
    <circle cx="560" cy="310" r="25" fill="#111d26"/>
    <circle cx="642" cy="330" r="12" fill="#1f4e79"/>
    <circle cx="560" cy="378" r="12" fill="#1f4e79"/>
    <circle cx="430" cy="377" r="12" fill="#1f4e79"/>
    <text x="110" y="165" font-family="Microsoft YaHei" font-size="22" font-weight="700" fill="#333">三处必须受力</text>
    <text x="110" y="212" font-family="Microsoft YaHei" font-size="20" fill="#333">1. 椅背/尾部 Technic 轴</text>
    <text x="110" y="248" font-family="Microsoft YaHei" font-size="20" fill="#333">2. 椅座前口连接桥</text>
    <text x="110" y="284" font-family="Microsoft YaHei" font-size="20" fill="#333">3. 桌面下方加强筋</text>
    <line x1="355" y1="210" x2="642" y2="330" stroke="#1f4e79" stroke-width="3" marker-end="url(#arrow)"/>
    <line x1="355" y1="248" x2="560" y2="378" stroke="#1f4e79" stroke-width="3" marker-end="url(#arrow)"/>
    <line x1="355" y1="284" x2="430" y2="377" stroke="#1f4e79" stroke-width="3" marker-end="url(#arrow)"/>
  `)],
];

for (const [name, svg] of diagrams) fs.writeFileSync(path.join(assetsDir, name), svg, "utf8");

const figureHtml = diagrams.map(([name], idx) => `
  <figure>
    ${fs.readFileSync(path.join(assetsDir, name), "utf8")}
    <figcaption>图 ${idx + 1}：${esc(name.replace(/^\d+_/, "").replace(".svg", ""))}</figcaption>
  </figure>`).join("\n");

const html = `<!doctype html>
<html lang="zh-CN">
<head>
<meta charset="utf-8">
<title>机械龙工作台积木玩具图文搭建说明书 V0.2</title>
<style>
  body{font-family:"Microsoft YaHei",Arial,sans-serif;margin:32px;color:#1f2328;line-height:1.6;background:#fff}
  .page{max-width:1180px;margin:auto}
  h1{font-size:32px;color:#1f4e79;margin:0 0 6px}
  h2{font-size:22px;color:#1f4e79;border-bottom:2px solid #d9e4f2;padding-bottom:4px;margin-top:26px}
  h3{font-size:18px;margin-bottom:6px}
  .meta{color:#555;margin-bottom:18px}
  .note{background:#fff2cc;border-left:6px solid #d6b656;padding:12px 16px;margin:16px 0}
  figure{break-inside:avoid;margin:24px 0;border:1px solid #d9dee3;padding:12px;background:#fff}
  figure svg{width:100%;height:auto;display:block}
  figcaption{font-size:13px;color:#555;text-align:center;margin-top:6px}
  table{border-collapse:collapse;width:100%;margin:12px 0 18px;font-size:13px}
  th{background:#1f4e79;color:#fff;text-align:left}
  th,td{border:1px solid #c7cdd3;padding:7px 8px;vertical-align:top}
  tr:nth-child(even) td{background:#f8fafc}
  .steps td:first-child{width:110px;font-weight:700;color:#1f4e79}
  @media print{body{margin:12mm} figure{page-break-inside:avoid} h2{page-break-after:avoid}}
</style>
</head>
<body><div class="page">
<h1>机械龙工作台积木玩具图文搭建说明书</h1>
<div class="meta">V0.2 图文版｜按图片反推｜单位基准：1 stud = 8 mm｜BOM 已单独输出</div>
<div class="note"><b>设计边界：</b>没有真实规格和 3D 源文件，本说明书采用示意图指导模块搭建，不冒充官方零件说明书。进入打样前，需要用 Studio/LDraw/企业零件库重建模型并做碰撞、重心、非法连接和安全测试。</div>
<h2>一、总装图与模块顺序</h2>
${figureHtml}
<h2>二、分步搭建说明</h2>
${tableHtml([
  ["步骤", "搭建内容", "关键检查"],
  ["1 工作台", "拼 18 x 14 studs 桌面；下方两条 2x8/2x4 压缝；四腿约 6 bricks 高；中间放白色操作区。", "桌面不能一条直缝贯穿；腿底要横向锁住。"],
  ["2 控制屏", "4x8 深灰 plate 做竖屏；正面装圆形按钮；背面加 bar 或 bracket 支撑。", "底部至少 2 studs 插接，背面至少两点支撑。"],
  ["3 椅座", "6x8+4x8 plate 做座面；两侧 curved slope；靠背 hinge 后仰 10-15°；轮组装在十字脚末端。", "中心柱承重，轮子不承重。"],
  ["4 背弧", "从椅背右后侧起 6-7 个 hinge/ball joint，外覆橙色 curved slope 和 wedge。", "尾部至少双点固定，不能单铰链悬空。"],
  ["5 龙头", "2x8 主梁加 wedge 收窄；头部 slope/cylinder 组合；喷嘴 bar+cone，clip+stud 双固定。", "喷嘴和黑色圆关节是视觉识别点，不能省。"],
  ["6 连接桥", "橙色 2x8 plate 两层叠加，中间错缝压 2x4，形成 2 studs 宽、3 plates 厚梁。", "连接桌面加强筋和椅座前口，同时承担抗弯。"],
], "steps")}
<h2>三、结构校核</h2>
${tableHtml([
  ["风险点", "处理要求"],
  ["机械龙下垂", "增加尾部、椅背侧面和连接桥三点受力，不要靠夹紧铰链硬撑。"],
  ["桌面开裂", "桌面下方错缝压板，外圈 tile 只做饰面。"],
  ["控制屏晃动", "底部双点固定，背部加两根支撑杆。"],
  ["轮组失稳", "小轮只做视觉，承重放在中心柱和十字脚。"],
  ["安全风险", "1x1 小件、喷嘴、圆锥件需标注 10+；量产前做第三方安全测试。"],
])}
</div></body></html>`;

fs.writeFileSync(path.join(outDir, "机械龙工作台图文搭建说明书_V0.2.html"), html, "utf8");
fs.writeFileSync(path.join(outDir, "机械龙工作台图文搭建说明书_V0.2_兼容Word.doc"), html, "utf8");

const csv = bom.map(r => r.map(v => `"${String(v).replace(/"/g, '""')}"`).join(",")).join("\r\n");
fs.writeFileSync(path.join(outDir, "机械龙工作台_BOM清单_V0.2.csv"), "\uFEFF" + csv, "utf8");

const rowsXml = bom.map((r, i) =>
  `<Row>${r.map(v => `<Cell${i === 0 ? ' ss:StyleID="Header"' : ""}><Data ss:Type="String">${esc(v)}</Data></Cell>`).join("")}</Row>`
).join("\n");
const xls = `<?xml version="1.0" encoding="UTF-8"?>
<?mso-application progid="Excel.Sheet"?>
<Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet"
 xmlns:o="urn:schemas-microsoft-com:office:office"
 xmlns:x="urn:schemas-microsoft-com:office:excel"
 xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">
 <Styles>
  <Style ss:ID="Default"><Font ss:FontName="Microsoft YaHei" ss:Size="10"/></Style>
  <Style ss:ID="Header"><Font ss:FontName="Microsoft YaHei" ss:Size="10" ss:Bold="1" ss:Color="#FFFFFF"/><Interior ss:Color="#1F4E79" ss:Pattern="Solid"/></Style>
 </Styles>
 <Worksheet ss:Name="BOM清单">
  <Table>
   <Column ss:Width="90"/><Column ss:Width="210"/><Column ss:Width="90"/><Column ss:Width="60"/><Column ss:Width="260"/>
   ${rowsXml}
  </Table>
  <WorksheetOptions xmlns="urn:schemas-microsoft-com:office:excel"><FreezePanes/><FrozenNoSplit/><SplitHorizontal>1</SplitHorizontal><TopRowBottomPane>1</TopRowBottomPane></WorksheetOptions>
 </Worksheet>
</Workbook>`;
fs.writeFileSync(path.join(outDir, "机械龙工作台_BOM清单_V0.2.xls"), "\uFEFF" + xls, "utf8");

console.log(outDir);
