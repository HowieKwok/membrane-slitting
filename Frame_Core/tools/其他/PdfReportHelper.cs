using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;



using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Frame_Core
{

    public class PdfReportHelper
    {
        /// <summary>
        /// iTextSharp 5 版本生产报表
        /// </summary>
        public static byte[] BuildPdf(
            DateTime time,
            string prodNo,
            string rollNo,
            int stripCount,
            int totalEa,
            string permission,
            string workNo,
            List<(int stripNo, int ok, int ng)> items)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4.Rotate(), 20, 20, 20, 20);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                // 字体（Win 自带）
                BaseFont bf = BaseFont.CreateFont("C:\\Windows\\Fonts\\msyh.ttc,0", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                Font font = new Font(bf, 11);
                Font bold = new Font(bf, 11, Font.BOLD);
                Font titleFont = new Font(bf, 18, Font.BOLD);

                // 标题
                iTextSharp.text.Paragraph title = new iTextSharp.text.Paragraph("生产实绩报表", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                doc.Add(title);
                doc.Add(new iTextSharp.text.Paragraph("打印时间：" + time.ToString("yyyy-MM-dd HH:mm:ss"), font)
                {
                    Alignment = Element.ALIGN_CENTER
                });
                doc.Add(Chunk.NEWLINE);

                // 基本信息表
                PdfPTable info = new PdfPTable(4) { WidthPercentage = 80 };
                info.SetWidths(new float[] { 15, 35, 15, 35 });
                info.HorizontalAlignment = Element.ALIGN_CENTER;

                void AddInfoRow(string lab, string val)
                {
                    info.AddCell(new PdfPCell(new Phrase(lab, bold)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    info.AddCell(new PdfPCell(new Phrase(val, font)));
                }
                AddInfoRow("品种号", prodNo);
                AddInfoRow("膜卷号", rollNo);
                AddInfoRow("分条数", stripCount.ToString());
                AddInfoRow("总 EA 数", totalEa.ToString());
                AddInfoRow("权限等级", permission);
                AddInfoRow("工号", workNo);
                doc.Add(info);
                doc.Add(Chunk.NEWLINE);

                // 明细表
                PdfPTable detail = new PdfPTable(5) { WidthPercentage = 80 };
                detail.SetWidths(new float[] { 20, 20, 20, 20, 20 });
                detail.HorizontalAlignment = Element.ALIGN_CENTER;

                // 表头
                string[] headers = { "条号", "OK数", "NG数", "总数", "良率(%)" };
                foreach (var h in headers)
                    detail.AddCell(new PdfPCell(new Phrase(h, bold)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                int totOk = 0, totNg = 0;
                foreach (var (stripNo, ok, ng) in items)
                {
                    int sub = ok + ng;
                    double rate = sub == 0 ? 0 : Math.Round(ok * 100.0 / sub, 2);
                    detail.AddCell(new PdfPCell(new Phrase(stripNo.ToString(), font)));
                    detail.AddCell(new PdfPCell(new Phrase(ok.ToString(), font)));
                    detail.AddCell(new PdfPCell(new Phrase(ng.ToString(), font)));
                    detail.AddCell(new PdfPCell(new Phrase(sub.ToString(), font)));
                    detail.AddCell(new PdfPCell(new Phrase(rate.ToString("0.##"), font)));
                    totOk += ok; totNg += ng;
                }

                //// 汇总
                //int tot = totOk + totNg;
                //double totRate = tot == 0 ? 0 : Math.Round(totOk * 100.0 / tot, 2);
                //detail.AddCell(new PdfPCell(new Phrase("汇总", bold)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                //detail.AddCell(new PdfPCell(new Phrase(totOk.ToString(), bold)));
                //detail.AddCell(new PdfPCell(new Phrase(totNg.ToString(), bold)));
                //detail.AddCell(new PdfPCell(new Phrase(tot.ToString(), bold)));
                //detail.AddCell(new PdfPCell(new Phrase(totRate.ToString("0.##"), bold)));

                doc.Add(detail);
                doc.Close();
                return ms.ToArray();
            }
        }
    }
}

