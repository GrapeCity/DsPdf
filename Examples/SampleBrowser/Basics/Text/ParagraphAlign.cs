//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.Drawing;
using System.IO;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // This sample demonstrates paragraph alignment options
    // (top/center/justified/bottom for horizontal LTR text).
    public class ParagraphAlign
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;
            var tl = g.CreateTextLayout();
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 12;
            var borderColor = Color.FromArgb(217, 217, 217);

            var h = (page.Size.Height - 72) / 5;
            RectangleF bounds = new RectangleF(36, 36, page.Size.Width - 72, h);

            tl.MaxWidth = bounds.Width;
            tl.MaxHeight = bounds.Height;

            var para = Common.Util.LoremIpsum(1, 5, 5, 10, 12);

            // 1: ParagraphAlignment.Near
            tl.ParagraphAlignment = ParagraphAlignment.Near;
            tl.Append("ParagraphAlignment.Near: ");
            tl.Append(para);
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, bounds.Location);
            g.DrawRectangle(bounds, borderColor);

            // 2: ParagraphAlignment.Center
            bounds.Offset(0, h);
            tl.Clear();
            tl.ParagraphAlignment = ParagraphAlignment.Center;
            tl.Append("ParagraphAlignment.Center: ");
            tl.Append(para);
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, bounds.Location);
            g.DrawRectangle(bounds, borderColor);

            // 3: ParagraphAlignment.Justified
            bounds.Offset(0, h);
            tl.Clear();
            tl.ParagraphAlignment = ParagraphAlignment.Justified;
            tl.Append("ParagraphAlignment.Justified: ");
            tl.Append(para);
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, bounds.Location);
            g.DrawRectangle(bounds, borderColor);

            // 4: ParagraphAlignment.Distributed
            bounds.Offset(0, h);
            tl.Clear();
            tl.ParagraphAlignment = ParagraphAlignment.Distributed;
            tl.Append("ParagraphAlignment.Distributed: ");
            tl.Append(para);
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, bounds.Location);
            g.DrawRectangle(bounds, borderColor);

            // 5: ParagraphAlignment.Far
            bounds.Offset(0, h);
            tl.Clear();
            tl.ParagraphAlignment = ParagraphAlignment.Far;
            tl.Append("ParagraphAlignment.Far: ");
            tl.Append(para);
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, bounds.Location);
            g.DrawRectangle(bounds, borderColor);

            // Done:
            doc.Save(stream);
        }
    }
}
