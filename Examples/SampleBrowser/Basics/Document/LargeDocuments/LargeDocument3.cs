//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
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
            var tl = new TextLayout(72)
            {
                MaxWidth = doc.PageSize.Width,
                MaxHeight = doc.PageSize.Height,
                MarginAll = 72,
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
            var fnt = Font.FromFile(Path.Combine("Resources", "Fonts", "yumin.ttf"));
            var tf0 = new TextFormat() { FontSize = 24, FontBold = true, Font = fnt };
            tl.Append(string.Format("Large Document\n{0} Pages of Lorem Ipsum\n\n", N), tf0);
            var tf1 = new TextFormat(tf0) { FontSize = 14, FontItalic = true };
            tl.Append(string.Format("Generated on {0} in {1:m\\m\\ s\\s\\ fff\\m\\s}.", DateTime.Now, DateTime.Now - start), tf1);
            tl.TextAlignment = TextAlignment.Center;
            tl.PerformLayout(true);
            doc.Pages.Insert(0).Graphics.DrawTextLayout(tl, PointF.Empty);
            // Done:
            doc.Save(stream);
            return doc.Pages.Count;
        }
    }
}
