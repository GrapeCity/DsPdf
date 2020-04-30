//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;

namespace GcPdfWeb.Samples.Basics
{
    // This sample demonstrates the difference between WordWrap and CharWrap
    // text wrapping modes.
    public class WordCharWrap
    {
        public int CreatePDF(Stream stream)
        {
            var str =
                "Lose nothing in your documents! GrapeCity Documents for PDF includes text and paragraph formatting, " +
                "special characters, multiple languages, RTL support, vertical and rotated text on all supported platforms.";

            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;

            var tl = g.CreateTextLayout();
            tl.Append(str);
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 12;
            tl.MaxWidth = 72 * 3;

            tl.WrapMode = WrapMode.WordWrap;
            tl.PerformLayout(true);

            var dy = tl.Lines[0].Height + 72 / 16;
            var rc = new RectangleF(72, 72 + dy, tl.MaxWidth.Value, 72 * 1.4F);

            g.DrawString("WrapMode.WordWrap:", tl.DefaultFormat, new PointF(rc.Left, rc.Top - dy));
            g.DrawTextLayout(tl, rc.Location);
            g.DrawRectangle(rc, Color.CornflowerBlue);

            rc.Offset(0, 72 * 2);
            tl.WrapMode = WrapMode.CharWrap;
            tl.PerformLayout(false);
            g.DrawString("WrapMode.CharWrap:", tl.DefaultFormat, new PointF(rc.Left, rc.Top - dy));
            g.DrawTextLayout(tl, rc.Location);
            g.DrawRectangle(rc, Color.CornflowerBlue);

            // Done:
            doc.Save(stream);
            return doc.Pages.Count;
        }
    }
}
