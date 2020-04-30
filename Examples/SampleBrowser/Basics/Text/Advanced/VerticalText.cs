//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;

namespace GcPdfWeb.Samples.Basics
{
    // Demonstrates rendering of vertical text in LTR and RTL modes.
    // Also shows how to have text flow around rectangular objects.
    // See also JapaneseColumns.
    public class VerticalText
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            // Use Landscape orientation:
            page.Landscape = true;
            var g = page.Graphics;
            // Some sample texts in Japanese, English and Arabic:
            string text1 = "学校教育の「国語」で教えられる。";
            string text2 = " flow direction. ";
            string text3 = "النص العربي 12 + 34 = 46 مع الأرقام ";
            // Init font cache and get the required fonts:
            var fc = new FontCollection();
            fc.RegisterDirectory(Path.Combine("Resources", "Fonts"));
            var fYuMin = fc.FindFamilyName("Yu Mincho");
            var fTimes = fc.FindFamilyName("Times New Roman");
            var fArial = fc.FindFamilyName("Arial");
            // Create text formats:
            var tf1 = new TextFormat() { Font = fYuMin };
            var tf2 = new TextFormat() { Font = fTimes };
            var tf3 = new TextFormat() { Font = fArial };
            // Create TextLayout and set some options on it:
            var tl = g.CreateTextLayout();
            tl.FirstLineIndent = 36;
            tl.TextAlignment = TextAlignment.Justified;
            // This setting justifies the last line too:
            tl.LastLineIsEndOfParagraph = false;
            // Set all margins to 1":
            tl.MarginAll = tl.Resolution;
            tl.MaxWidth = page.Size.Width;
            tl.MaxHeight = page.Size.Height;
            // RTL layout:
            tl.RightToLeft = false;
            // Build a list of objects for the text to flow around:
            tl.ObjectRects = new List<ObjectRect>()
            {
                new ObjectRect(540, 100, 120, 160),
                new ObjectRect(100, 290, 170, 100),
                new ObjectRect(500, 350, 170, 100)
            };
            // Fill corresponding rectangels on page so that we can see them:
            foreach (var or in tl.ObjectRects)
                g.FillRectangle(or.ToRectangleF(), Color.PaleVioletRed);

            // Add text to layout:
            for (int i = 0; i < 3; i++)
            {
                tl.Append(text1, tf1);
                tl.Append("Horizontal Top To Bottom" + text2, tf2);
                tl.AppendLine(text3, tf3);
            }
            // Perform and draw first layout:
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, PointF.Empty);
            g.FillRectangle(tl.ContentRectangle, Color.FromArgb(20, Color.Red));

            // Create 2nd layout - vertical rotated counter-clockwise:
            var t = tl.ContentHeight;
            tl.Clear();
            tl.RotateSidewaysCounterclockwise = true;
            tl.FlowDirection = FlowDirection.VerticalLeftToRight;
            tl.MarginTop += t;
            // Add text to layout:
            for (int i = 0; i < 3; i++)
            {
                tl.Append(text1, tf1);
                tl.Append("Vertical Left To Right" + text2, tf2);
                tl.AppendLine(text3, tf3);
            }
            // Perform and draw second layout:
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, PointF.Empty);
            g.FillRectangle(tl.ContentRectangle, Color.FromArgb(20, Color.Green));

            // Create 3rd layout - vertical:
            tl.Clear();
            tl.FlowDirection = FlowDirection.VerticalRightToLeft;
            tl.RotateSidewaysCounterclockwise = false;
            // Add text to layout:
            for (int i = 0; i < 3; i++)
            {
                tl.Append(text1, tf1);
                tl.Append("Vertical Right To Left" + text2, tf2);
                tl.AppendLine(text3, tf3);
            }
            // Perform and draw third layout:
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, PointF.Empty);
            g.FillRectangle(tl.ContentRectangle, Color.FromArgb(20, Color.Blue));
            // Done:
            doc.Save(stream);
        }
    }
}
