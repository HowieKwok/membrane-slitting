const fs = require("fs");
const path = require("path");

const root = process.cwd();
const outDir = path.join(root, "积木机械龙图文说明_V0.2");
fs.mkdirSync(outDir, { recursive: true });

const svg = `<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1400 900">
  <defs>
    <linearGradient id="bg" x1="0" y1="0" x2="0" y2="1">
      <stop offset="0" stop-color="#eeeeee"/>
      <stop offset="1" stop-color="#d7d7d7"/>
    </linearGradient>
    <linearGradient id="orangeTop" x1="0" y1="0" x2="0" y2="1">
      <stop offset="0" stop-color="#ff9d2e"/>
      <stop offset="1" stop-color="#f47a00"/>
    </linearGradient>
    <linearGradient id="grayTop" x1="0" y1="0" x2="0" y2="1">
      <stop offset="0" stop-color="#b9c0c2"/>
      <stop offset="1" stop-color="#8e9799"/>
    </linearGradient>
    <filter id="softShadow" x="-20%" y="-20%" width="140%" height="140%">
      <feDropShadow dx="0" dy="12" stdDeviation="12" flood-color="#000" flood-opacity=".22"/>
    </filter>
    <filter id="smallShadow" x="-20%" y="-20%" width="140%" height="140%">
      <feDropShadow dx="0" dy="5" stdDeviation="4" flood-color="#000" flood-opacity=".22"/>
    </filter>
  </defs>

  <rect width="1400" height="900" fill="url(#bg)"/>
  <ellipse cx="715" cy="745" rx="560" ry="86" fill="#000" opacity=".13"/>

  <!-- Work table -->
  <g filter="url(#softShadow)">
    <polygon points="80,465 500,325 700,420 270,575" fill="#b2bbbd" stroke="#646d70" stroke-width="3"/>
    <polygon points="270,575 700,420 700,470 270,625" fill="#919a9d" stroke="#646d70" stroke-width="3"/>
    <polygon points="80,465 270,575 270,625 80,515" fill="#858e91" stroke="#646d70" stroke-width="3"/>
    <polygon points="210,452 410,385 540,445 338,518" fill="#ffffff" stroke="#9ca3a7" stroke-width="2"/>
    <g fill="#d2d6d8" stroke="#6d7476" stroke-width="2">
      <ellipse cx="146" cy="466" rx="13" ry="8"/>
      <ellipse cx="202" cy="447" rx="13" ry="8"/>
      <ellipse cx="258" cy="428" rx="13" ry="8"/>
      <ellipse cx="314" cy="409" rx="13" ry="8"/>
      <ellipse cx="370" cy="390" rx="13" ry="8"/>
      <ellipse cx="426" cy="371" rx="13" ry="8"/>
      <ellipse cx="245" cy="524" rx="13" ry="8"/>
      <ellipse cx="304" cy="503" rx="13" ry="8"/>
      <ellipse cx="364" cy="481" rx="13" ry="8"/>
      <ellipse cx="424" cy="460" rx="13" ry="8"/>
    </g>
    <g>
      <polygon points="125,520 190,555 190,745 125,710" fill="#7f8785" stroke="#555"/>
      <polygon points="190,555 230,540 230,730 190,745" fill="#69716f" stroke="#555"/>
      <polygon points="505,450 565,480 565,666 505,636" fill="#7f8785" stroke="#555"/>
      <polygon points="565,480 608,462 608,648 565,666" fill="#69716f" stroke="#555"/>
      <polygon points="178,710 246,680 246,718 178,748" fill="#6f7775" stroke="#555"/>
      <polygon points="520,633 610,596 610,632 520,670" fill="#6f7775" stroke="#555"/>
    </g>
  </g>

  <!-- Control screen -->
  <g filter="url(#smallShadow)">
    <polygon points="180,238 330,178 330,420 180,482" fill="#454c4e" stroke="#1e2528" stroke-width="4"/>
    <polygon points="202,260 310,218 310,394 202,438" fill="#747d7f" stroke="#2d3437" stroke-width="3"/>
    <polygon points="330,178 352,195 352,438 330,420" fill="#273139" stroke="#1e2528" stroke-width="3"/>
    <g fill="#b9c0c2" stroke="#3b4245" stroke-width="3">
      <ellipse cx="230" cy="277" rx="15" ry="22" transform="rotate(-18 230 277)"/>
      <ellipse cx="268" cy="263" rx="15" ry="22" transform="rotate(-18 268 263)"/>
      <ellipse cx="303" cy="250" rx="15" ry="22" transform="rotate(-18 303 250)"/>
      <ellipse cx="230" cy="334" rx="12" ry="17" transform="rotate(-18 230 334)"/>
      <ellipse cx="268" cy="320" rx="12" ry="17" transform="rotate(-18 268 320)"/>
      <ellipse cx="303" cy="307" rx="12" ry="17" transform="rotate(-18 303 307)"/>
      <ellipse cx="231" cy="386" rx="12" ry="17" transform="rotate(-18 231 386)"/>
      <ellipse cx="267" cy="372" rx="12" ry="17" transform="rotate(-18 267 372)"/>
    </g>
  </g>

  <!-- Chair base and seat -->
  <g filter="url(#softShadow)">
    <polygon points="790,462 1030,375 1162,442 918,535" fill="#525a56" stroke="#1e2523" stroke-width="4"/>
    <polygon points="918,535 1162,442 1162,510 918,604" fill="#3e4744" stroke="#1e2523" stroke-width="4"/>
    <polygon points="790,462 918,535 918,604 790,532" fill="#48514e" stroke="#1e2523" stroke-width="4"/>
    <path d="M1042 390 C1060 292 1115 166 1220 72 L1342 152 L1340 482 L1195 525 C1175 408 1110 365 1042 390Z" fill="#5f6461" stroke="#262b2b" stroke-width="4"/>
    <path d="M1128 148 C1160 174 1194 197 1236 220" fill="none" stroke="#767b78" stroke-width="14"/>
    <path d="M1114 250 C1160 270 1203 288 1260 310" fill="none" stroke="#767b78" stroke-width="12"/>
    <path d="M1120 375 C1165 390 1210 405 1265 420" fill="none" stroke="#767b78" stroke-width="12"/>
    <polygon points="862,535 1020,472 1078,500 920,565" fill="#111d26" opacity=".94"/>
    <polygon points="908,604 980,578 980,670 908,700" fill="#c6c9c8" stroke="#707575" stroke-width="3"/>
    <path d="M944 680 L760 758" stroke="#c6c9c8" stroke-width="30" stroke-linecap="round"/>
    <path d="M944 680 L1110 750" stroke="#c6c9c8" stroke-width="30" stroke-linecap="round"/>
    <path d="M944 680 L944 805" stroke="#c6c9c8" stroke-width="30" stroke-linecap="round"/>
    <g>
      <circle cx="738" cy="768" r="34" fill="#111d26" stroke="#d7d7d7" stroke-width="6"/>
      <circle cx="1128" cy="758" r="34" fill="#111d26" stroke="#d7d7d7" stroke-width="6"/>
      <circle cx="944" cy="825" r="34" fill="#111d26" stroke="#d7d7d7" stroke-width="6"/>
      <circle cx="805" cy="730" r="28" fill="#111d26" stroke="#d7d7d7" stroke-width="5"/>
      <circle cx="1064" cy="714" r="28" fill="#111d26" stroke="#d7d7d7" stroke-width="5"/>
    </g>
  </g>

  <!-- Orange bridge -->
  <g filter="url(#smallShadow)">
    <polygon points="640,470 820,405 888,438 708,505" fill="#ff8b1a" stroke="#a65400" stroke-width="3"/>
    <polygon points="708,505 888,438 888,476 708,544" fill="#d96b00" stroke="#a65400" stroke-width="3"/>
    <polygon points="640,470 708,505 708,544 640,510" fill="#e37400" stroke="#a65400" stroke-width="3"/>
    <g fill="#ffa640" stroke="#a65400" stroke-width="2">
      <ellipse cx="685" cy="468" rx="12" ry="7"/>
      <ellipse cx="734" cy="451" rx="12" ry="7"/>
      <ellipse cx="783" cy="434" rx="12" ry="7"/>
      <ellipse cx="832" cy="417" rx="12" ry="7"/>
    </g>
  </g>

  <!-- Mechanical dragon / arm -->
  <g filter="url(#softShadow)">
    <path d="M875 456 C890 348 945 240 1030 164 C1068 128 1110 100 1150 74" fill="none" stroke="#ff8b1a" stroke-width="64" stroke-linecap="round"/>
    <path d="M875 456 C890 348 945 240 1030 164 C1068 128 1110 100 1150 74" fill="none" stroke="#a65400" stroke-width="5" stroke-linecap="round"/>
    <path d="M914 430 C950 352 1010 270 1088 214" fill="none" stroke="#111d26" stroke-width="34" stroke-linecap="round" opacity=".96"/>
    <g fill="url(#orangeTop)" stroke="#a65400" stroke-width="3">
      <polygon points="880,400 950,352 1010,382 940,432"/>
      <polygon points="930,318 1008,268 1066,298 990,350"/>
      <polygon points="997,235 1070,180 1128,210 1058,266"/>
      <polygon points="1050,150 1124,98 1186,130 1114,184"/>
    </g>
    <g fill="#111d26" stroke="#d6d6d6" stroke-width="4">
      <circle cx="890" cy="424" r="31"/>
      <circle cx="966" cy="322" r="28"/>
      <circle cx="1040" cy="232" r="26"/>
    </g>
    <g>
      <polygon points="615,312 780,255 920,310 756,374" fill="url(#orangeTop)" stroke="#a65400" stroke-width="4"/>
      <polygon points="756,374 920,310 918,370 756,432" fill="#e07000" stroke="#a65400" stroke-width="4"/>
      <polygon points="615,312 756,374 756,432 615,372" fill="#f27c00" stroke="#a65400" stroke-width="4"/>
      <polygon points="545,340 625,292 704,318 666,382 560,398" fill="#ff9a28" stroke="#a65400" stroke-width="4"/>
      <polygon points="542,356 408,376 390,410 558,398" fill="#ff8b1a" stroke="#a65400" stroke-width="4"/>
      <polygon points="548,383 416,430 426,462 575,420" fill="#ff8b1a" stroke="#a65400" stroke-width="4"/>
      <line x1="405" y1="394" x2="265" y2="410" stroke="#ff8b1a" stroke-width="22" stroke-linecap="round"/>
      <line x1="425" y1="446" x2="290" y2="484" stroke="#ff8b1a" stroke-width="18" stroke-linecap="round"/>
      <line x1="322" y1="398" x2="258" y2="392" stroke="#cc6500" stroke-width="8" stroke-linecap="round"/>
      <line x1="330" y1="476" x2="270" y2="492" stroke="#cc6500" stroke-width="7" stroke-linecap="round"/>
      <circle cx="650" cy="404" r="36" fill="#111d26" stroke="#d6d6d6" stroke-width="5"/>
      <ellipse cx="560" cy="333" rx="10" ry="6" fill="#ffd64a" stroke="#9a4d00"/>
      <g fill="#ffa640" stroke="#a65400" stroke-width="2">
        <ellipse cx="650" cy="316" rx="13" ry="7"/>
        <ellipse cx="698" cy="299" rx="13" ry="7"/>
        <ellipse cx="746" cy="282" rx="13" ry="7"/>
        <ellipse cx="794" cy="265" rx="13" ry="7"/>
      </g>
    </g>
    <g stroke="#ff8b1a" stroke-width="16" stroke-linecap="round" fill="none">
      <path d="M700 438 L650 500"/>
      <path d="M770 430 L736 510"/>
      <path d="M835 398 L825 485"/>
    </g>
    <g fill="#ff8b1a" stroke="#a65400" stroke-width="3">
      <polygon points="636,500 688,488 658,530"/>
      <polygon points="720,512 772,494 748,538"/>
      <polygon points="810,484 858,470 836,514"/>
    </g>
  </g>

  <text x="64" y="70" font-family="Microsoft YaHei, Arial" font-size="34" font-weight="700" fill="#1f4e79">机械龙工作台积木最终效果图</text>
  <text x="64" y="108" font-family="Microsoft YaHei, Arial" font-size="18" fill="#555">V0.3 展示示意图｜按图片反推，非最终 3D 渲染</text>
</svg>`;

const html = `<!doctype html>
<html lang="zh-CN">
<head>
<meta charset="utf-8">
<title>机械龙工作台最终效果图 V0.3</title>
<style>
  body{margin:0;background:#2c3034;font-family:"Microsoft YaHei",Arial,sans-serif}
  .wrap{max-width:1320px;margin:24px auto;background:#fff;padding:18px;box-shadow:0 16px 40px rgba(0,0,0,.32)}
  svg{width:100%;height:auto;display:block}
</style>
</head>
<body><div class="wrap">${svg}</div></body>
</html>`;

fs.writeFileSync(path.join(outDir, "机械龙工作台_最终效果图_V0.3.svg"), svg, "utf8");
fs.writeFileSync(path.join(outDir, "机械龙工作台_最终效果图_V0.3.html"), html, "utf8");

console.log(path.join(outDir, "机械龙工作台_最终效果图_V0.3.html"));
