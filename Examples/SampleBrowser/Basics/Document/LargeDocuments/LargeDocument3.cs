using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // Generates a large PDF using a single TextLayout.
    // Unlike other 'large document' samples (StartEndDoc, LargeDocument2),
    // this sample actually paginates the whole document as a single sequence of paragraphs,
    // optimally and correctly filling each page with text. Split options such as
    // keeping at least 2 lines of paragraph on each page are also supported.
    // Due to the extra work, this sample takes significantly more time to complete.
    public class LargeDocument3
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
            for (int paraIdx = 0; paraIdx < N; ++paraIdx)
            {
                tl.AppendLine(Common.Util.LoremIpsum(1));
            }
            // Split and render TextLayout as shown in the PaginatedText sample:
            TextSplitOptions tso = new TextSplitOptions(tl)
            {
                MinLinesInFirstParagraph = 2,
                MinLinesInLastParagraph = 2,
            };
            tl.PerformLayout(true);
            // The loop splitting and rendering the layout:
            var tls = new TextLayoutSplitter(tl);
            for (var tlPage = tls.Split(tso); tlPage != null; tlPage = tls.Split(tso))
                doc.NewPage().Graphics.DrawTextLayout(tlPage, PointF.Empty);

            tl.Clear();
            // Insert a title page (cannot be done if using StartDoc/EndDoc):
            tl.FirstLineIndent = 0;
            tl.Append($"Large TextLayout\n{doc.Pages.Count} Pages/{N} Paragraphs of Lorem Ipsum\n\n", new TextFormat() { FontSize = 24, FontBold = true });
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
