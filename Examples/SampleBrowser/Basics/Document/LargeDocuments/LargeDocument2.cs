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
    // Generates a large PDF.
    // This sample is identical to StartEndDoc, but does not use the StartDoc/EndDoc method.
    public class LargeDocument2
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
            for (int pageIdx = 0; pageIdx < N; ++pageIdx)
            {
                tl.Append(Common.Util.LoremIpsum(1));
                tl.PerformLayout(true);
                doc.NewPage().Graphics.DrawTextLayout(tl, PointF.Empty);
                tl.Clear();
            }
            // Insert a title page (cannot be done if using StartDoc/EndDoc):
            tl.FirstLineIndent = 0;
            var fnt = Font.FromFile(Path.Combine("Resources", "Fonts", "yumin.ttf"));
            var tf0 = new TextFormat() { Font = fnt, FontSize = 24, FontBold = true };
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
