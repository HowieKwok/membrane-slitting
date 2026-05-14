const fs = require("fs");
const path = require("path");

const root = process.cwd();
const mdPath = path.join(root, "机械龙工作台积木玩具搭建说明书_V0.1.md");
const rtfPath = path.join(root, "机械龙工作台积木玩具搭建说明书_V0.1.rtf");
const docPath = path.join(root, "机械龙工作台积木玩具搭建说明书_V0.1_兼容Word.doc");

function rtfEscape(text) {
  let out = "";
  for (const ch of text) {
    const code = ch.codePointAt(0);
    if (ch === "\\") out += "\\\\";
    else if (ch === "{") out += "\\{";
    else if (ch === "}") out += "\\}";
    else if (code === 9) out += "\\tab ";
    else if (code >= 32 && code <= 126) out += ch;
    else {
      const signed = code > 32767 ? code - 65536 : code;
      out += `\\u${signed}?`;
    }
  }
  return out;
}

function lineToRtf(line) {
  if (/^# /.test(line)) return `\\fs40\\b ${rtfEscape(line.replace(/^# /, ""))}\\b0\\fs22\\par\\par`;
  if (/^## /.test(line)) return `\\fs30\\b\\cf1 ${rtfEscape(line.replace(/^## /, ""))}\\cf0\\b0\\fs22\\par`;
  if (/^### /.test(line)) return `\\fs25\\b ${rtfEscape(line.replace(/^### /, ""))}\\b0\\fs22\\par`;
  if (/^\|/.test(line)) return `\\fs18 ${rtfEscape(line)}\\fs22\\par`;
  if (/^\d+\. /.test(line)) return `\\li360 ${rtfEscape(line)}\\li0\\par`;
  if (/^- /.test(line)) return `\\li360 ${rtfEscape("• " + line.slice(2))}\\li0\\par`;
  if (line.trim() === "") return "\\par";
  return `${rtfEscape(line)}\\par`;
}

const md = fs.readFileSync(mdPath, "utf8");
const content = md.split(/\r?\n/).map(lineToRtf).join("\n");
const rtf = `{\\rtf1\\ansi\\deff0
{\\fonttbl{\\f0 Microsoft YaHei;}{\\f1 Consolas;}}
{\\colortbl;\\red31\\green78\\blue121;\\red68\\green68\\blue68;}
\\paperw16838\\paperh11906\\landscape\\margl720\\margr720\\margt720\\margb720
\\f0\\fs22
${content}
}`;

fs.writeFileSync(rtfPath, rtf, "utf8");
fs.writeFileSync(docPath, rtf, "utf8");
console.log(rtfPath);
console.log(docPath);
