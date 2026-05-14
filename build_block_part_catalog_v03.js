const fs = require("fs");
const path = require("path");

const root = process.cwd();
const outDir = path.join(root, "积木机械龙图文说明_V0.2");
const assetsDir = path.join(outDir, "part_assets_v03");
fs.mkdirSync(assetsDir, { recursive: true });

const parts = [
  ["P001", "Plate 6x12", "浅灰", 1, "工作台", "桌面主板", "plate", [6, 12]],
  ["P002", "Plate 4x12", "浅灰", 2, "工作台", "桌面扩展", "plate", [4, 12]],
  ["P003", "Plate 2x12", "浅灰", 2, "工作台", "长边桌沿", "plate", [2, 12]],
  ["P004", "Plate 2x8", "浅灰/橙色", 7, "工作台/连接桥", "压缝与连接桥", "plate", [2, 8]],
  ["P005", "Tile 2x8", "浅灰", 2, "工作台", "外露桌沿收口", "tile", [2, 8]],
  ["P006", "Tile 2x6", "浅灰", 2, "工作台", "外露桌沿收口", "tile", [2, 6]],
  ["P007", "Brick 2x2", "深灰/浅灰", 12, "工作台", "桌腿承重", "brick", [2, 2]],
  ["P008", "Brick 2x4", "深灰/浅灰", 8, "工作台", "桌腿承重", "brick", [2, 4]],
  ["P009", "Plate 2x4", "深灰/橙色", 12, "工作台/连接桥", "腿部加固与错缝", "plate", [2, 4]],
  ["P010", "Tile / Plate 4x6", "白色", 4, "工作台", "白色操作区", "tile", [4, 6]],
  ["P011", "Plate 4x8", "深灰", 3, "控制屏/椅座", "竖屏与座面", "plate", [4, 8]],
  ["P012", "Plate 1x8", "深灰", 2, "控制屏", "屏幕背框", "plate", [1, 8]],
  ["P013", "Plate 1x6", "深灰", 2, "控制屏", "屏幕背框", "plate", [1, 6]],
  ["P014", "Round Plate / Tile 1x1", "浅灰/深灰", 18, "控制屏", "按钮与指示灯", "round1", [1, 1]],
  ["P015", "Slope 1x2 45°", "深灰", 4, "控制屏", "操作键", "slope", [1, 2]],
  ["P016", "Clip Plate 1x1", "深灰/橙色", 20, "控制屏/机械龙", "夹杆连接", "clip", [1, 1]],
  ["P017", "Bar 3L / 4L", "浅灰/橙色", 14, "控制屏/机械龙", "支撑杆、喷嘴、爪", "bar", [4]],
  ["P018", "Plate 6x8", "深灰", 1, "椅座", "座面基础", "plate", [6, 8]],
  ["P019", "Curved Slope 2x4", "深灰/黑/橙色", 12, "椅座/机械龙", "护板与背弧外壳", "curved", [2, 4]],
  ["P020", "Panel 6x6 / 6x8", "深灰", 4, "椅座", "高靠背", "panel", [6, 8]],
  ["P021", "Hinge Plate 1x2", "深灰/橙色", 16, "椅座/机械龙", "可调角连接", "hinge", [1, 2]],
  ["P022", "Small Wheel", "黑色", 8, "椅座", "办公椅轮", "wheel", [1]],
  ["P023", "Wheel Holder", "浅灰", 4, "椅座", "轮子支架", "holder", [1]],
  ["P024", "Round Brick 2x2", "浅灰", 2, "椅座", "中心柱", "round2", [2, 2]],
  ["P025", "Dish 4x4", "浅灰/黑色", 3, "椅座/机械龙", "底部十字脚或黑色内弧", "dish", [4]],
  ["P026", "Plate 2x6", "橙色", 6, "机械龙", "身体分节", "plate", [2, 6]],
  ["P027", "Plate 1x4", "橙色", 12, "机械龙", "装甲压缝", "plate", [1, 4]],
  ["P028", "Plate 1x2", "橙色", 16, "机械龙", "细节与压缝", "plate", [1, 2]],
  ["P029", "Tile 2x4", "橙色", 6, "机械龙", "顶部平滑装甲", "tile", [2, 4]],
  ["P030", "Tile 1x4", "橙色", 8, "机械龙", "顶部平滑装甲", "tile", [1, 4]],
  ["P031", "Wedge Plate 左/右", "橙色", "8 对", "机械龙", "龙身收腰与头部轮廓", "wedge", [3, 4]],
  ["P032", "Curved Slope 2x2", "橙色", 8, "机械龙", "背弧小外壳", "curved", [2, 2]],
  ["P033", "Slope 2x2 45°", "橙色", 6, "机械龙", "头部楔形轮廓", "slope", [2, 2]],
  ["P034", "Cone 1x1", "橙色", 4, "机械龙", "喷嘴尖端", "cone", [1]],
  ["P035", "Round Brick 1x1", "橙色", 8, "机械龙", "喷嘴与机械细节", "round1tall", [1]],
  ["P036", "Cylinder 2x2", "橙色", 4, "机械龙", "头部与肩部圆筒", "cylinder", [2]],
  ["P037", "Ball Joint Socket", "橙色/黑色", 6, "机械龙", "脊柱球窝", "ballsocket", [1]],
  ["P038", "Ball Joint Arm", "橙色/黑色", 6, "机械龙", "脊柱球头臂", "ballarm", [1]],
  ["P039", "Round Tile / Dish 2x2-4x4", "黑色", 6, "机械龙", "侧向关节与内侧黑弧", "dish", [3]],
  ["P040", "Technic Brick 1x2 with Hole", "深灰/橙色", 8, "连接件", "承重点", "technicbrick", [1, 2]],
  ["P041", "Technic Pin", "黑/蓝", 12, "连接件", "铰接锁定", "pin", [1]],
  ["P042", "Technic Axle 4L / 6L", "黑/浅灰", 4, "连接件", "长向加固", "axle", [6]],
  ["P043", "Jumper Plate 1x2", "各色", 10, "连接件", "半格错位与微调", "jumper", [1, 2]],
  ["P044", "Transparent Stud 1x1", "透明黄", 2, "机械龙", "眼部高光", "round1", [1, 1]],
];

const colorMap = {
  "浅灰": "#b8bfc2",
  "深灰": "#5a6268",
  "橙色": "#ff8b1a",
  "黑色": "#111d26",
  "白色": "#ffffff",
  "透明黄": "#ffd64a",
  "浅灰/橙色": "#ff8b1a",
  "深灰/浅灰": "#8d9699",
  "深灰/橙色": "#ff8b1a",
  "深灰/黑": "#343a40",
  "深灰/黑/橙色": "#ff8b1a",
  "浅灰/深灰": "#9aa1a6",
  "深灰/橙色": "#ff8b1a",
  "黑/浅灰": "#2c3338",
  "浅灰/黑色": "#80888b",
  "橙色/黑色": "#ff8b1a",
  "深灰/橙色": "#ff8b1a",
  "黑/蓝": "#1e4e9a",
  "黑/浅灰": "#4c555a",
  "各色": "#9aa1a6",
};

function esc(s) {
  return String(s).replace(/[&<>"']/g, c => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[c]));
}

function studs(x, y, cols, rows, step = 24, fill = "#d5d9dc") {
  let out = "";
  for (let r = 0; r < rows; r++) for (let c = 0; c < cols; c++) {
    out += `<ellipse cx="${x + c * step}" cy="${y + r * step}" rx="7" ry="4.5" fill="${fill}" stroke="#61686c" stroke-width="1"/>`;
  }
  return out;
}

function baseSvg(title, inner) {
  return `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 360 240">
  <rect width="360" height="240" fill="#f7f8fa"/>
  ${inner}
  <text x="18" y="218" font-family="Microsoft YaHei, Arial" font-size="16" font-weight="700" fill="#1f4e79">${esc(title)}</text>
</svg>`;
}

function drawPart(part) {
  const [, name, color, , , , type, dims] = part;
  const fill = colorMap[color] || "#b8bfc2";
  const stroke = "#3b4145";
  const [a = 2, b = 4] = dims;
  const w = Math.min(230, 34 * b);
  const h = Math.min(120, 28 * a);
  const x = 68;
  const y = 62;
  const topFill = fill;
  const sideFill = fill === "#ffffff" ? "#d9dde0" : fill;
  const studFill = fill === "#ffffff" ? "#ffffff" : "#f0a04a";

  if (type === "plate" || type === "tile" || type === "jumper") {
    const studsSvg = type === "tile" ? "" : type === "jumper"
      ? `<ellipse cx="${x + w / 2}" cy="${y + h / 2}" rx="13" ry="8" fill="${studFill}" stroke="${stroke}"/>`
      : studs(x + 18, y + 18, Math.max(1, b), Math.max(1, a), Math.min(24, w / Math.max(1, b)), studFill);
    return baseSvg(name, `
      <polygon points="${x},${y+h} ${x+28},${y+h+22} ${x+w+28},${y+h+22} ${x+w},${y+h}" fill="${sideFill}" opacity=".75" stroke="${stroke}"/>
      <rect x="${x}" y="${y}" width="${w}" height="${h}" rx="3" fill="${topFill}" stroke="${stroke}" stroke-width="2"/>
      ${studsSvg}
      ${type === "tile" ? `<text x="${x+w/2}" y="${y+h/2+6}" text-anchor="middle" font-family="Arial" font-size="18" fill="#555">smooth</text>` : ""}
    `);
  }

  if (type === "brick") {
    return baseSvg(name, `
      <polygon points="${x},${y+92} ${x+28},${y+120} ${x+w+28},${y+120} ${x+w},${y+92}" fill="${sideFill}" opacity=".8" stroke="${stroke}"/>
      <rect x="${x}" y="${y}" width="${w}" height="92" rx="3" fill="${fill}" stroke="${stroke}" stroke-width="2"/>
      ${studs(x + 18, y + 18, Math.max(1, b), Math.max(1, a), Math.min(24, w / Math.max(1, b)), studFill)}
      <line x1="${x}" y1="${y+54}" x2="${x+w}" y2="${y+54}" stroke="#000" opacity=".12"/>
    `);
  }

  if (type === "slope") {
    return baseSvg(name, `
      <polygon points="88,154 260,154 260,74" fill="${fill}" stroke="${stroke}" stroke-width="2"/>
      <polygon points="88,154 116,180 288,180 260,154" fill="${sideFill}" opacity=".75" stroke="${stroke}"/>
      <polygon points="260,74 288,100 288,180 260,154" fill="${sideFill}" opacity=".55" stroke="${stroke}"/>
      ${studs(118, 138, Math.max(1, b), 1, 28, studFill)}
    `);
  }

  if (type === "curved") {
    return baseSvg(name, `
      <path d="M82 150 C118 82, 210 70, 274 112 L274 158 L82 158Z" fill="${fill}" stroke="${stroke}" stroke-width="2"/>
      <path d="M82 158 L112 184 L304 184 L274 158Z" fill="${sideFill}" opacity=".75" stroke="${stroke}"/>
      ${studs(118, 138, Math.max(1, b), 1, 28, studFill)}
    `);
  }

  if (type === "wedge") {
    return baseSvg(name, `
      <polygon points="92,70 275,112 275,160 92,160" fill="${fill}" stroke="${stroke}" stroke-width="2"/>
      <polygon points="92,160 120,186 303,186 275,160" fill="${sideFill}" opacity=".75" stroke="${stroke}"/>
      ${studs(128, 122, 4, 1, 32, studFill)}
      <text x="180" y="95" font-family="Arial" font-size="13" fill="#333">L/R pair</text>
    `);
  }

  if (type === "panel") {
    return baseSvg(name, `
      <rect x="105" y="44" width="150" height="140" rx="4" fill="${fill}" stroke="${stroke}" stroke-width="2"/>
      <line x1="135" y1="50" x2="135" y2="178" stroke="#000" opacity=".18"/>
      <line x1="180" y1="50" x2="180" y2="178" stroke="#000" opacity=".18"/>
      <line x1="225" y1="50" x2="225" y2="178" stroke="#000" opacity=".18"/>
      ${studs(132, 166, 4, 1, 28, "#848b8f")}
    `);
  }

  if (type === "round1" || type === "round1tall" || type === "round2" || type === "cylinder") {
    const r = type === "round2" || type === "cylinder" ? 58 : 36;
    const cy = type === "round1tall" || type === "cylinder" ? 105 : 125;
    const height = type === "round1tall" || type === "cylinder" ? 74 : 36;
    return baseSvg(name, `
      <ellipse cx="180" cy="${cy-height/2}" rx="${r}" ry="${r/2.5}" fill="${fill}" stroke="${stroke}" stroke-width="2"/>
      <rect x="${180-r}" y="${cy-height/2}" width="${r*2}" height="${height}" fill="${fill}" stroke="${stroke}" stroke-width="2"/>
      <ellipse cx="180" cy="${cy+height/2}" rx="${r}" ry="${r/2.5}" fill="${sideFill}" stroke="${stroke}" stroke-width="2"/>
      <ellipse cx="180" cy="${cy-height/2}" rx="${r/2.2}" ry="${r/4}" fill="${studFill}" stroke="${stroke}"/>
    `);
  }

  if (type === "cone") {
    return baseSvg(name, `
      <path d="M180 58 L112 162 L248 162Z" fill="${fill}" stroke="${stroke}" stroke-width="2"/>
      <ellipse cx="180" cy="162" rx="68" ry="24" fill="${sideFill}" stroke="${stroke}" stroke-width="2"/>
      <ellipse cx="180" cy="68" rx="20" ry="8" fill="${studFill}" stroke="${stroke}"/>
    `);
  }

  if (type === "dish") {
    return baseSvg(name, `
      <ellipse cx="180" cy="125" rx="92" ry="48" fill="${fill}" stroke="${stroke}" stroke-width="2"/>
      <ellipse cx="180" cy="118" rx="62" ry="28" fill="#f7f8fa" stroke="${stroke}" opacity=".55"/>
      <ellipse cx="180" cy="122" rx="22" ry="10" fill="${studFill}" stroke="${stroke}"/>
    `);
  }

  if (type === "clip") {
    return baseSvg(name, `
      <rect x="120" y="120" width="86" height="42" rx="4" fill="${fill}" stroke="${stroke}" stroke-width="2"/>
      <ellipse cx="146" cy="138" rx="13" ry="8" fill="${studFill}" stroke="${stroke}"/>
      <path d="M205 120 C260 92, 285 118, 254 145" fill="none" stroke="${fill}" stroke-width="18" stroke-linecap="round"/>
      <path d="M205 120 C260 92, 285 118, 254 145" fill="none" stroke="${stroke}" stroke-width="2"/>
      <line x1="238" y1="100" x2="238" y2="155" stroke="#f7f8fa" stroke-width="10"/>
    `);
  }

  if (type === "bar" || type === "axle" || type === "pin") {
    return baseSvg(name, `
      <rect x="84" y="112" width="192" height="28" rx="14" fill="${fill}" stroke="${stroke}" stroke-width="2"/>
      <ellipse cx="84" cy="126" rx="14" ry="14" fill="${sideFill}" stroke="${stroke}" stroke-width="2"/>
      <ellipse cx="276" cy="126" rx="14" ry="14" fill="${sideFill}" stroke="${stroke}" stroke-width="2"/>
      ${type === "axle" ? `<line x1="112" y1="126" x2="248" y2="126" stroke="#f7f8fa" stroke-width="4"/><line x1="180" y1="98" x2="180" y2="154" stroke="#f7f8fa" stroke-width="4"/>` : ""}
    `);
  }

  if (type === "wheel") {
    return baseSvg(name, `
      <circle cx="180" cy="122" r="72" fill="#111d26" stroke="${stroke}" stroke-width="2"/>
      <circle cx="180" cy="122" r="46" fill="#2d343a" stroke="#e6e6e6" stroke-width="6"/>
      <circle cx="180" cy="122" r="16" fill="#b8bfc2" stroke="${stroke}" stroke-width="2"/>
    `);
  }

  if (type === "holder") {
    return baseSvg(name, `
      <rect x="145" y="62" width="70" height="92" rx="6" fill="${fill}" stroke="${stroke}" stroke-width="2"/>
      <path d="M125 154 L235 154 L215 190 L145 190Z" fill="${sideFill}" stroke="${stroke}" stroke-width="2"/>
      <circle cx="180" cy="172" r="20" fill="#111d26" stroke="${stroke}"/>
    `);
  }

  if (type === "hinge") {
    return baseSvg(name, `
      <rect x="78" y="116" width="86" height="44" rx="4" fill="${fill}" stroke="${stroke}" stroke-width="2"/>
      <rect x="196" y="96" width="86" height="44" rx="4" fill="${fill}" stroke="${stroke}" stroke-width="2" transform="rotate(12 239 118)"/>
      <circle cx="180" cy="128" r="28" fill="${sideFill}" stroke="${stroke}" stroke-width="2"/>
      <circle cx="180" cy="128" r="12" fill="#f7f8fa" stroke="${stroke}"/>
    `);
  }

  if (type === "ballsocket") {
    return baseSvg(name, `
      <rect x="92" y="122" width="110" height="45" rx="4" fill="${fill}" stroke="${stroke}" stroke-width="2"/>
      <path d="M202 108 C260 88, 294 122, 266 164 C238 158, 224 148, 202 160Z" fill="${fill}" stroke="${stroke}" stroke-width="2"/>
      <circle cx="242" cy="132" r="22" fill="#f7f8fa" stroke="${stroke}" stroke-width="2"/>
    `);
  }

  if (type === "ballarm") {
    return baseSvg(name, `
      <rect x="124" y="112" width="112" height="28" rx="14" fill="${fill}" stroke="${stroke}" stroke-width="2"/>
      <circle cx="104" cy="126" r="28" fill="${fill}" stroke="${stroke}" stroke-width="2"/>
      <circle cx="256" cy="126" r="28" fill="${fill}" stroke="${stroke}" stroke-width="2"/>
      <circle cx="104" cy="126" r="10" fill="${studFill}" stroke="${stroke}"/>
      <circle cx="256" cy="126" r="10" fill="${studFill}" stroke="${stroke}"/>
    `);
  }

  if (type === "technicbrick") {
    return baseSvg(name, `
      <rect x="105" y="82" width="150" height="82" rx="4" fill="${fill}" stroke="${stroke}" stroke-width="2"/>
      ${studs(135, 104, 2, 1, 60, studFill)}
      <circle cx="180" cy="140" r="22" fill="#f7f8fa" stroke="${stroke}" stroke-width="3"/>
    `);
  }

  return baseSvg(name, `<rect x="90" y="80" width="180" height="90" fill="${fill}" stroke="${stroke}" stroke-width="2"/>`);
}

for (const part of parts) {
  fs.writeFileSync(path.join(assetsDir, `${part[0]}_${part[1].replace(/[\\/:*?"<>| ]+/g, "_")}.svg`), drawPart(part), "utf8");
}

const rows = parts.map(p => {
  const file = fs.readdirSync(assetsDir).find(f => f.startsWith(`${p[0]}_`));
  return `<tr>
    <td class="id">${esc(p[0])}</td>
    <td><img src="part_assets_v03/${esc(file)}" alt="${esc(p[1])}"></td>
    <td>${esc(p[1])}</td>
    <td>${esc(p[2])}</td>
    <td class="qty">${esc(p[3])}</td>
    <td>${esc(p[4])}</td>
    <td>${esc(p[5])}</td>
  </tr>`;
}).join("\n");

const html = `<!doctype html>
<html lang="zh-CN">
<head>
<meta charset="utf-8">
<title>机械龙工作台积木零件图册 V0.3</title>
<style>
  body{font-family:"Microsoft YaHei",Arial,sans-serif;margin:28px;color:#1f2328;line-height:1.45}
  h1{font-size:30px;color:#1f4e79;margin:0 0 6px}
  .meta{color:#555;margin-bottom:14px}
  .note{background:#fff2cc;border-left:6px solid #d6b656;padding:10px 14px;margin:14px 0 18px}
  table{border-collapse:collapse;width:100%;font-size:13px}
  th{background:#1f4e79;color:#fff}
  th,td{border:1px solid #c7cdd3;padding:7px 8px;vertical-align:middle}
  tr:nth-child(even) td{background:#f8fafc}
  img{width:150px;height:100px;object-fit:contain;display:block}
  .id{font-weight:700;color:#1f4e79;text-align:center;width:58px}
  .qty{text-align:center;font-weight:700}
  @media print{body{margin:10mm} tr{break-inside:avoid} img{width:130px;height:86px}}
</style>
</head>
<body>
<h1>机械龙工作台积木零件图册</h1>
<div class="meta">V0.3｜每个 BOM 零件类型单独绘图｜用于识别、采购核件和搭建对照</div>
<div class="note">说明：以下为通用兼容积木示意图，不是品牌官方零件编号图。组合项已经拆成独立零件类型；颜色按本模型建议色标注。</div>
<table>
  <tr><th>图号</th><th>零件图</th><th>通用零件名</th><th>颜色</th><th>数量</th><th>模块</th><th>用途/备注</th></tr>
  ${rows}
</table>
</body>
</html>`;

fs.writeFileSync(path.join(outDir, "机械龙工作台_零件图册_V0.3.html"), html, "utf8");
fs.writeFileSync(path.join(outDir, "机械龙工作台_零件图册_V0.3_兼容Word.doc"), html, "utf8");

const csv = [["图号", "通用零件名", "颜色", "数量", "模块", "用途/备注"], ...parts.map(p => [p[0], p[1], p[2], p[3], p[4], p[5]])]
  .map(r => r.map(v => `"${String(v).replace(/"/g, '""')}"`).join(",")).join("\r\n");
fs.writeFileSync(path.join(outDir, "机械龙工作台_BOM清单_带图号_V0.3.csv"), "\uFEFF" + csv, "utf8");

const xlsRows = [["图号", "通用零件名", "颜色", "数量", "模块", "用途/备注"], ...parts.map(p => [p[0], p[1], p[2], p[3], p[4], p[5]])]
  .map((r, i) => `<Row>${r.map(v => `<Cell${i === 0 ? ' ss:StyleID="Header"' : ""}><Data ss:Type="String">${esc(v)}</Data></Cell>`).join("")}</Row>`).join("\n");
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
 <Worksheet ss:Name="BOM带图号">
  <Table>
   <Column ss:Width="60"/><Column ss:Width="210"/><Column ss:Width="100"/><Column ss:Width="60"/><Column ss:Width="100"/><Column ss:Width="280"/>
   ${xlsRows}
  </Table>
  <WorksheetOptions xmlns="urn:schemas-microsoft-com:office:excel"><FreezePanes/><FrozenNoSplit/><SplitHorizontal>1</SplitHorizontal><TopRowBottomPane>1</TopRowBottomPane></WorksheetOptions>
 </Worksheet>
</Workbook>`;
fs.writeFileSync(path.join(outDir, "机械龙工作台_BOM清单_带图号_V0.3.xls"), "\uFEFF" + xls, "utf8");

console.log(outDir);
console.log(parts.length);
