using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // Generates a large PDF.
    // This sample is identical to StartEndDoc, but does not use the StartDoc/EndDoc
    // method, thus using more memory, but probably improving the performance.
    public class LargeDocument2
    {
        public int CreatePDF(Stream stream)
        {
            // Number of pages to generate:
            const int N = Common.Util.LargeDocumentIterations;
            var start = DateTime.Now;
            var doc = new GcPdfDocument();
            // Prep a TextLayout to hold/format the text:
            var tl = new TextLayout()
            {
                MaxWidth = doc.PageSize.Width,
                MaxHeight = doc.PageSize.Height,
                MarginLeft = 72,
                MarginRight = 72,
                MarginTop = 72,
                MarginBottom = 72,
                FirstLineIndent = 36,
            };
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 12;
            // Generate the document:
            for (int pageIdx = 0; pageIdx < N; ++pageIdx)
            {
                tl.Append(Common.Util.LoremIpsum(1));
                tl.PerformLayout(true);
                doc.NewPage().Graphics.DrawTextLayout(tl, PointF.Empty);
                tl.Clear();
            }
            // Insert a title page (cannot be done if using StartDoc/EndDoc):
            tl.FirstLineIndent = 0;
            tl.Append($"Large Document\n{N} Pages of Lorem Ipsum\n\n", new TextFormat() { FontSize = 24, FontBold = true });
            tl.Append($"Generated on {DateTime.Now} in {DateTime.Now - start:m\\m\\ s\\s\\ fff\\m\\s}.", new TextFormat() { FontSize = 14, FontItalic = true });
            tl.TextAlignment = TextAlignment.Center;
            tl.PerformLayout(true);
            doc.Pages.Insert(0).Graphics.DrawTextLayout(tl, PointF.Empty);
            // Done:
            doc.Save(stream);
            return doc.Pages.Count;
        }
    }
}
