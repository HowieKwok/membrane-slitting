const fs = require("fs");
const path = require("path");
const childProcess = require("child_process");

const root = process.cwd();
const buildDir = path.join(root, "_doc_work", "block_toy_manual_build");
const zipPath = path.join(root, "_doc_work", "block_toy_manual.zip");
const outPath = path.join(root, "机械龙工作台积木玩具搭建说明书_V0.1.docx");

function rm(target) {
  if (fs.existsSync(target)) fs.rmSync(target, { recursive: true, force: true });
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

function runPr(opts = {}) {
  const font = opts.font || "Microsoft YaHei";
  const bold = opts.bold ? "<w:b/><w:bCs/>" : "";
  const italic = opts.italic ? "<w:i/><w:iCs/>" : "";
  const color = opts.color ? `<w:color w:val="${opts.color}"/>` : "";
  const size = opts.size ? `<w:sz w:val="${opts.size}"/><w:szCs w:val="${opts.size}"/>` : "";
  return `<w:rPr><w:rFonts w:ascii="${font}" w:eastAsia="${font}" w:hAnsi="${font}" w:cs="${font}"/>${bold}${italic}${color}${size}</w:rPr>`;
}

function textRuns(text, opts = {}) {
  return String(text)
    .split("\n")
    .map((line, index) => {
      const br = index === 0 ? "" : "<w:br/>";
      return `<w:r>${runPr(opts)}${br}<w:t xml:space="preserve">${esc(line)}</w:t></w:r>`;
    })
    .join("");
}

function para(text, opts = {}) {
  const style = opts.style ? `<w:pStyle w:val="${opts.style}"/>` : "";
  const align = opts.align ? `<w:jc w:val="${opts.align}"/>` : "";
  const before = opts.before == null ? 0 : opts.before;
  const after = opts.after == null ? 120 : opts.after;
  const indent = opts.indent ? `<w:ind w:left="${opts.indent}" w:hanging="${opts.hanging || 0}"/>` : "";
  const keepNext = opts.keepNext ? "<w:keepNext/>" : "";
  const pageBreakBefore = opts.pageBreakBefore ? "<w:pageBreakBefore/>" : "";
  const shading = opts.shading ? `<w:shd w:fill="${opts.shading}"/>` : "";
  const border = opts.border
    ? `<w:pBdr><w:left w:val="single" w:sz="12" w:space="8" w:color="${opts.border}"/></w:pBdr>`
    : "";
  const spacing = `<w:spacing w:before="${before}" w:after="${after}" w:line="${opts.line || 300}" w:lineRule="auto"/>`;
  return `<w:p><w:pPr>${style}${keepNext}${pageBreakBefore}${spacing}${indent}${align}${shading}${border}</w:pPr>${textRuns(text, opts)}</w:p>`;
}

function heading(text, pageBreakBefore = false) {
  return para(text, {
    style: "Heading1",
    size: 28,
    bold: true,
    color: "1F4E79",
    before: pageBreakBefore ? 0 : 160,
    after: 120,
    keepNext: true,
    pageBreakBefore,
  });
}

function subheading(text) {
  return para(text, {
    style: "Heading2",
    size: 23,
    bold: true,
    color: "2F5597",
    before: 120,
    after: 80,
    keepNext: true,
  });
}

function bullet(text, level = 0) {
  const left = level === 0 ? 420 : 780;
  return para(`• ${text}`, { size: 20, indent: left, hanging: 260, after: 60 });
}

function step(text) {
  return para(text, { size: 20, indent: 360, hanging: 260, after: 70 });
}

function cell(content, opts = {}) {
  const width = opts.width || 1800;
  const fill = opts.fill ? `<w:shd w:fill="${opts.fill}"/>` : "";
  const vAlign = `<w:vAlign w:val="center"/>`;
  const margins =
    '<w:tcMar><w:top w:w="90" w:type="dxa"/><w:left w:w="110" w:type="dxa"/><w:bottom w:w="90" w:type="dxa"/><w:right w:w="110" w:type="dxa"/></w:tcMar>';
  const textOpts = {
    size: opts.size || 18,
    bold: Boolean(opts.bold),
    color: opts.color || "000000",
    align: opts.align || "left",
    after: 0,
    line: 260,
  };
  return `<w:tc><w:tcPr><w:tcW w:w="${width}" w:type="dxa"/>${fill}${vAlign}${margins}</w:tcPr>${para(content, textOpts)}</w:tc>`;
}

function table(headers, rows, widths, opts = {}) {
  const tableWidth = widths.reduce((sum, value) => sum + value, 0);
  const borders = `
    <w:tblBorders>
      <w:top w:val="single" w:sz="6" w:space="0" w:color="B7B7B7"/>
      <w:left w:val="single" w:sz="6" w:space="0" w:color="B7B7B7"/>
      <w:bottom w:val="single" w:sz="6" w:space="0" w:color="B7B7B7"/>
      <w:right w:val="single" w:sz="6" w:space="0" w:color="B7B7B7"/>
      <w:insideH w:val="single" w:sz="4" w:space="0" w:color="D9D9D9"/>
      <w:insideV w:val="single" w:sz="4" w:space="0" w:color="D9D9D9"/>
    </w:tblBorders>`;
  const tblPr = `<w:tblPr><w:tblW w:w="${tableWidth}" w:type="dxa"/><w:tblLayout w:type="fixed"/><w:jc w:val="center"/>${borders}<w:tblCellMar><w:top w:w="80" w:type="dxa"/><w:left w:w="100" w:type="dxa"/><w:bottom w:w="80" w:type="dxa"/><w:right w:w="100" w:type="dxa"/></w:tblCellMar></w:tblPr>`;
  const grid = `<w:tblGrid>${widths.map((w) => `<w:gridCol w:w="${w}"/>`).join("")}</w:tblGrid>`;
  const headerRow = `<w:tr><w:trPr><w:tblHeader/></w:trPr>${headers
    .map((h, i) =>
      cell(h, {
        width: widths[i],
        fill: opts.headerFill || "1F4E79",
        bold: true,
        color: "FFFFFF",
        align: i === 0 ? "left" : "center",
        size: 17,
      })
    )
    .join("")}</w:tr>`;
  const bodyRows = rows
    .map((row, r) => {
      return `<w:tr>${row
        .map((value, i) => {
          const isNumberLike = i === 2 || i === 3 || /^[0-9.+\-xX ]+$/.test(String(value));
          return cell(value, {
            width: widths[i],
            fill: r % 2 === 0 ? "FFFFFF" : "F8FAFC",
            align: isNumberLike ? "center" : "left",
            size: opts.bodySize || 17,
          });
        })
        .join("")}</w:tr>`;
    })
    .join("");
  return `${para(opts.caption || "", { size: 17, bold: true, color: "44546A", after: 45, keepNext: true })}<w:tbl>${tblPr}${grid}${headerRow}${bodyRows}</w:tbl>${para("", { after: 100 })}`;
}

function callout(title, text, fill = "EAF2F8", border = "2F5597") {
  return (
    para(title, { size: 21, bold: true, color: "1F4E79", shading: fill, border, before: 60, after: 30 }) +
    para(text, { size: 20, shading: fill, border, after: 120 })
  );
}

function pageBreak() {
  return '<w:p><w:r><w:br w:type="page"/></w:r></w:p>';
}

const docInfo = [
  ["文档名称", "机械龙工作台积木玩具搭建说明书", "版本", "V0.1 图片反推版"],
  ["适用对象", "设计师 / MOC 搭建设计评审", "单位基准", "1 stud = 8 mm"],
  ["模型类型", "兼容积木展示模型 / 概念样机", "预计颗粒", "约 260-320 件"],
  ["重要边界", "无实物规格、无 3D 源文件，尺寸为视觉比例反推", "安全状态", "非量产认证文件"],
];

const bomRows = [
  ["工作台", "Plate 6x12", "浅灰", "1", "桌面主板"],
  ["工作台", "Plate 4x12", "浅灰", "2", "桌面扩展"],
  ["工作台", "Plate 2x12 / 2x8", "浅灰", "5", "桌沿与压缝"],
  ["工作台", "Tile 2x8 / 2x6", "浅灰", "4", "外露桌沿收口"],
  ["工作台", "Brick 2x2 / 2x4", "深灰/浅灰", "20", "桌腿与承重柱"],
  ["工作台", "Plate 2x4", "深灰", "6", "腿部横向加固"],
  ["工作台", "Tile 或 Plate 4x6", "白色", "4", "桌面白色操作区"],
  ["控制屏", "Plate 4x8", "深灰", "1", "竖向屏幕主体"],
  ["控制屏", "Plate 1x8 / 1x6", "深灰", "4", "屏幕背部框架"],
  ["控制屏", "Round Plate / Tile 1x1", "浅灰/深灰", "18", "按钮与指示灯"],
  ["控制屏", "Slope 1x2 45°", "深灰", "4", "下半区操作键"],
  ["控制屏", "Modified Plate with Clip + Bar 4L", "深灰/浅灰", "6", "屏幕支撑"],
  ["椅座", "Plate 6x8", "深灰", "1", "座面基础"],
  ["椅座", "Plate 4x8", "深灰", "2", "座面加宽"],
  ["椅座", "Curved Slope 2x4", "深灰/黑", "6", "两侧弧形护板"],
  ["椅座", "Panel 6x6 或 6x8", "深灰", "3-4", "高靠背"],
  ["椅座", "Hinge Plate 1x2", "深灰", "6", "靠背后仰角"],
  ["椅座", "Small Wheel + Wheel Holder", "黑/浅灰", "8+4", "办公椅轮组"],
  ["椅座", "Round Brick 2x2 / Dish 4x4", "浅灰", "3", "中心柱与底部十字脚"],
  ["机械龙", "Plate 2x8 / 2x6 / 2x4", "橙色", "20", "身体主梁与分节外壳"],
  ["机械龙", "Plate 1x4 / 1x2", "橙色", "28", "装甲压缝与细节"],
  ["机械龙", "Tile 2x4 / 1x4", "橙色", "14", "顶部平滑装甲"],
  ["机械龙", "Wedge Plate 左/右", "橙色", "8 对", "龙身收腰与头部轮廓"],
  ["机械龙", "Curved Slope 2x2 / 2x4", "橙色", "14", "背弧与外壳弧面"],
  ["机械龙", "Slope 2x2 45°", "橙色", "6", "头部楔形轮廓"],
  ["机械龙", "Cone / Round Brick 1x1", "橙色", "12", "喷嘴与机械细节"],
  ["机械龙", "Cylinder 2x2", "橙色", "4", "头部与肩部圆筒"],
  ["机械龙", "Hinge Plate / Ball Joint", "橙/黑", "16", "脊柱可调结构"],
  ["机械龙", "Clip Plate + Bar 3L/4L", "橙色", "20", "爪、喷嘴、下挂机构"],
  ["机械龙", "Round Tile / Dish 2x2-4x4", "黑色", "6", "侧向关节与内侧黑弧"],
  ["连接件", "Technic Brick 1x2 with Hole", "深灰/橙", "8", "承重连接点"],
  ["连接件", "Technic Pin / Axle 4L/6L", "黑/蓝/浅灰", "16", "铰接与加固"],
  ["连接件", "Jumper Plate 1x2", "各色", "10", "半格错位与微调"],
];

const moduleRows = [
  ["桌面", "18 x 14 studs", "约 144 x 112 mm", "由多块 plate 压缝形成，外圈 tile 表现厚桌沿"],
  ["桌腿", "4 组，每组约 2 x 4 studs", "高约 58-65 mm", "腿底横向锁住，防止桌面扭转"],
  ["控制屏", "4 x 8 studs", "约 32 x 64 mm", "背后至少双点固定，正面做按钮阵列"],
  ["椅座", "12 x 10 studs", "约 96 x 80 mm", "中心柱承重，小轮只做视觉表现"],
  ["靠背", "6-8 studs 宽，高 10-12 studs", "后仰约 10-15°", "分段 panel 或 curved slope 模拟图片弧面"],
  ["机械龙", "长 24-28 studs", "约 192-224 mm", "6-7 个活动节形成向前伸展的 S 形曲线"],
  ["连接桥", "2 studs 宽，3 plates 厚", "约 16 mm 宽", "同时承担桌椅视觉连接和结构抗弯"],
];

const riskRows = [
  ["悬臂下垂", "橙色龙体长度较长，单铰链受力不足", "尾部、连接桥、椅背侧面三点受力；关键处加 Technic 轴"],
  ["桌面开裂", "大桌面由多块板件拼成，搬动时容易从中间打开", "桌面下方错缝压板，外圈 tile 只做饰面不承担主连接"],
  ["控制屏晃动", "竖板高度明显，正面按钮会增加重量", "背面增加 2 根 bar 或 bracket，底部至少 2 studs 插接"],
  ["轮组失稳", "小轮不适合承受完整模型重量", "真正承重放在中心柱和十字脚，轮子保持轻触地"],
  ["儿童安全", "喷嘴、圆锥、1x1 小件存在误吞风险", "建议标注 10+，量产前按目标市场做第三方安全检测"],
  ["非法连接", "弯曲装甲和夹杆可能出现过紧角度", "数字建模时检查干涉，避免强行弯折 clip 和 bar"],
];

const qualityRows = [
  ["外观还原", "机械龙头、橙色背弧、黑色圆关节、灰色工作台、竖屏、椅轮均可一眼识别"],
  ["装配顺序", "先桌体、再椅体、再连接桥、最后安装机械龙，避免后期无法压紧内部结构"],
  ["模块拆分", "桌子、屏幕、椅座、龙体建议可拆，便于包装和说明书分袋"],
  ["颜色控制", "主色橙色；结构灰；关节黑；高光白/透明黄。不要把近似色混成同一功能区"],
  ["说明书表达", "每步新增零件数控制在 3-8 件；活动角度用箭头或角度标注"],
  ["量产验证", "需要完成 3D 模型、碰撞检查、跌落/拉力/尖角/小物件测试后才能进入玩具认证"],
];

const body = [
  para("机械龙工作台", { size: 46, bold: true, color: "1F4E79", align: "center", before: 260, after: 60 }),
  para("积木玩具搭建说明书 / 图片反推 V0.1", { size: 26, bold: true, color: "44546A", align: "center", after: 220 }),
  table(["项目", "内容", "项目", "内容"], docInfo, [1900, 4600, 1900, 4600], { caption: "" }),
  callout(
    "设计师说明",
    "本文件按用户提供图片进行视觉比例反推，采用兼容积木常用单位体系编写。由于没有实物尺寸、CAD、零件库约束和真实受力数据，本文档属于 MOC 概念搭建说明，不冒充量产认证文件。下一阶段若要进入打样，应在 Studio/LDraw/Blender 或企业零件库中重建模型并做碰撞、重心和装配顺序校验。",
    "FFF2CC",
    "D6B656"
  ),
  para("版本日期：2026-05-07    编制：Codex", { size: 18, color: "666666", align: "right", after: 160 }),
  pageBreak(),

  heading("一、设计基准"),
  para("本方案按图片中的视觉层级进行模块化反推：左侧为灰色工作台和竖向控制屏，中部为橙色机械龙/机械臂，右侧为深灰色高靠背椅座与带轮机械底座。整体目标不是做静态摆件外壳，而是按积木玩具的可搭建逻辑拆成可连接、可分袋、可说明的结构。", { size: 20 }),
  bullet("单位：1 stud = 8 mm；1 brick 高约 9.6 mm；3 plates = 1 brick。"),
  bullet("推荐难度：中高阶 MOC，建议 10+；不建议给低龄儿童无监管玩耍。"),
  bullet("结构策略：桌椅为稳定底座，橙色机械龙为悬臂视觉主体，所有悬臂处必须通过铰链、Technic 轴或多点 plate 连接分散受力。"),
  bullet("零件名称：采用通用积木件描述，未绑定官方品牌编号，便于后续按供应链替换。"),
  table(["模块", "建议积木尺寸", "约合实物尺寸", "设计说明"], moduleRows, [1800, 2700, 2500, 6400], {
    caption: "表 1  图片反推尺寸建议",
    bodySize: 17,
  }),

  heading("二、零件清单 BOM", true),
  para("下表为首版可搭建清单，数量按视觉还原和结构冗余预留估算。打样时建议额外准备 5%-10% 的 1x2、1x4、clip、hinge、jumper 和 Technic pin，用于局部调角与压缝。", { size: 20 }),
  table(["模块", "通用零件", "颜色", "数量", "用途 / 备注"], bomRows, [1500, 3600, 1700, 1200, 5600], {
    caption: "表 2  首版搭建 BOM",
    bodySize: 16,
  }),

  heading("三、分步搭建说明", true),
  subheading("A. 灰色工作台"),
  step("1. 用 6x12、4x12 和 2x12 plate 拼出约 18 x 14 studs 的桌面，不要把长边接缝放在同一直线上。"),
  step("2. 桌面下方用 2x8、2x4 plate 横向压缝，至少形成两条贯通加强筋。"),
  step("3. 四条桌腿用 2x2 与 2x4 brick 叠到约 6 bricks 高，腿底用 2x4 plate 连接成门字形。"),
  step("4. 桌面外圈铺浅灰 tile，表现图片中的厚桌沿；中间放白色 plate/tile 作为操作台区域。"),
  subheading("B. 左侧控制屏"),
  step("1. 以深灰 4x8 plate 作为竖屏主体，背面用 1x8 和 1x6 plate 做外框。"),
  step("2. 正面上半区排布 1x1 round plate/tile 作为按钮和仪表，注意保留黑色顶边。"),
  step("3. 下半区使用 1x2 slope 和圆片形成控制键阵列。"),
  step("4. 屏幕底部通过 bracket 或 clip 插入桌面，背面再加两根 bar 支撑，避免前后晃动。"),
  subheading("C. 右侧椅座与底盘"),
  step("1. 用 6x8 plate 和 4x8 plate 叠出座面，深灰为主体，黑色 curved slope 表现两侧包覆感。"),
  step("2. 后方用 3-4 块 panel 或 curved slope 竖起高靠背，通过 hinge plate 形成约 10-15° 后仰。"),
  step("3. 座面下方用 round brick 做中心柱，再接十字脚。轮组安装在十字脚末端，轮子只做视觉件。"),
  step("4. 椅座前方预留 2 studs 宽的连接口，用来接橙色桌椅连接桥。"),
  subheading("D. 橙色机械龙背弧"),
  step("1. 从椅背右后侧开始布置 6-7 个 hinge 或 ball joint 节点，形成向前上方弯出的 S 形脊柱。"),
  step("2. 每节外侧覆盖橙色 curved slope 和 wedge plate，内侧保留黑色圆弧或黑色 dish 表现机械骨架。"),
  step("3. 尾部与椅背至少做两点连接：一个 Technic 轴承担拉力，一个 plate/clip 防止旋转。"),
  subheading("E. 龙身、龙头与喷嘴"),
  step("1. 用 2x8 plate 做龙身主梁，两侧用 wedge plate 收窄，顶部铺 tile 与 curved slope。"),
  step("2. 左右侧安装黑色 round tile 或 dish，模拟图片中的圆形机械关节。"),
  step("3. 头部用 slope、wedge plate 和 cylinder 做楔形轮廓，透明黄 1x1 stud 可作为眼部高光。"),
  step("4. 前端喷嘴用 bar、cone 和 round brick 组合，至少以 clip + stud 双方式固定，避免单点脱落。"),
  step("5. 下方 3-4 个机械爪/支架使用 clip plate 与 bar，不承担主重量，只负责视觉层次。"),
  subheading("F. 桌椅连接桥"),
  step("1. 用橙色 2x8 plate 两层叠加，中间错缝压 2x4 plate，形成约 2 studs 宽、3 plates 厚的梁。"),
  step("2. 一端固定在桌面下方加强筋，另一端接椅座前方接口。"),
  step("3. 连接桥是整件作品的视觉导线，也是抗弯件；打样时不得省略。"),

  heading("四、结构校核与玩具化要求", true),
  table(["风险点", "原因", "处理要求"], riskRows, [2500, 4300, 6800], {
    caption: "表 3  首版结构风险与处理要求",
    bodySize: 17,
  }),
  callout(
    "关键判断",
    "如果橙色机械龙安装后会明显下垂，优先增加尾部和连接桥的受力点，而不是把铰链夹得更紧。夹得过紧会造成零件疲劳，也不符合积木玩具的可重复装拆要求。",
    "EAF2F8",
    "2F5597"
  ),

  heading("五、交付与打样检查表"),
  table(["检查项", "通过标准"], qualityRows, [3000, 10400], {
    caption: "表 4  设计交付检查表",
    bodySize: 17,
  }),
  subheading("下一阶段建议"),
  bullet("建立数字模型：优先使用 Studio 或 LDraw 系零件库，确认所有连接合法且无明显干涉。"),
  bullet("输出正式说明书：每步新增零件 3-8 件，必要处标注旋转角度和分袋编号。"),
  bullet("补充真实规格：若提供目标成品长宽高、目标颗粒数、年龄段和成本上限，可把 V0.1 升级为 V1.0 打样版 BOM。"),
  bullet("量产前检测：按目标销售地区做小零件、尖角、拉力、跌落、材料和标识合规测试。"),
].join("");

const documentXml = `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<w:document xmlns:wpc="http://schemas.microsoft.com/office/word/2010/wordprocessingCanvas"
  xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
  xmlns:o="urn:schemas-microsoft-com:office:office"
  xmlns:r="http://schemas.openxmlformats.org/officeDocument/2006/relationships"
  xmlns:m="http://schemas.openxmlformats.org/officeDocument/2006/math"
  xmlns:v="urn:schemas-microsoft-com:vml"
  xmlns:wp14="http://schemas.microsoft.com/office/2010/wordprocessingDrawing"
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
      <w:pgMar w:top="760" w:right="560" w:bottom="720" w:left="560" w:header="360" w:footer="360" w:gutter="0"/>
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
    <w:rPr><w:b/><w:bCs/><w:color w:val="1F4E79"/><w:sz w:val="28"/><w:szCs w:val="28"/></w:rPr>
  </w:style>
  <w:style w:type="paragraph" w:styleId="Heading2">
    <w:name w:val="heading 2"/><w:basedOn w:val="Normal"/><w:next w:val="Normal"/><w:qFormat/>
    <w:pPr><w:keepNext/><w:spacing w:before="120" w:after="80"/></w:pPr>
    <w:rPr><w:b/><w:bCs/><w:color w:val="2F5597"/><w:sz w:val="23"/><w:szCs w:val="23"/></w:rPr>
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
  <dc:title>机械龙工作台积木玩具搭建说明书</dc:title>
  <dc:creator>Codex</dc:creator>
  <cp:lastModifiedBy>Codex</cp:lastModifiedBy>
  <dcterms:created xsi:type="dcterms:W3CDTF">2026-05-07T00:00:00Z</dcterms:created>
  <dcterms:modified xsi:type="dcterms:W3CDTF">2026-05-07T00:00:00Z</dcterms:modified>
</cp:coreProperties>`;

const appXml = `<?xml version="1.0" encoding="UTF-8" standalone="yes"?>
<Properties xmlns="http://schemas.openxmlformats.org/officeDocument/2006/extended-properties" xmlns:vt="http://schemas.openxmlformats.org/officeDocument/2006/docPropsVTypes">
  <Application>WPS Office compatible OpenXML</Application>
  <DocSecurity>0</DocSecurity>
  <ScaleCrop>false</ScaleCrop>
  <Company>Design Draft</Company>
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
