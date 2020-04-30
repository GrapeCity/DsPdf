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
    // This sample shows how to create gradient fills
    // using LinearGradientBrush and RadialGradientBrush.
    public class Gradients
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var g = doc.NewPage().Graphics;
            var testRectSize = new SizeF(72 * 4, 72);
            var dy = 12;
            // TextLayout to draw labels:
            var tl = g.CreateTextLayout();
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 18;
            tl.DefaultFormat.ForeColor = Color.Chartreuse;
            tl.MaxWidth = testRectSize.Width;
            tl.MaxHeight = testRectSize.Height;
            tl.TextAlignment = TextAlignment.Center;
            tl.ParagraphAlignment = ParagraphAlignment.Center;
            // Note 1:
            var rc = Common.Util.AddNote("Linear gradients using LinearGradientBrush:", doc.Pages.Last, new RectangleF(72, 36, 500, 100));
            // Text insertion point:
            PointF ip = new PointF(rc.Left, rc.Bottom + dy);
            // Local action to draw a gradient-filled rectangle:
            Action<Brush, string> drawSwatch = (b_, txt_) =>
            {
                var rect = new RectangleF(ip, testRectSize);
                // Fill the rectangle with a gradient brush:
                g.FillRectangle(rect, b_);
                // Draw a border, text info etc:
                g.DrawRectangle(rect, Color.Magenta);
                tl.Clear();
                tl.Append(txt_);
                tl.MaxHeight = testRectSize.Height;
                tl.MaxWidth = testRectSize.Width;
                tl.PerformLayout(true);
                g.DrawTextLayout(tl, ip);
                ip.Y += rect.Height + dy;
            };
            // LinearGradientBrush:
            // Horizontal gradient:
            LinearGradientBrush linearGradBrush = new LinearGradientBrush(Color.Red, Color.Blue);
            drawSwatch(linearGradBrush, $"Linear gradient\nfrom {linearGradBrush.StartPoint} to {linearGradBrush.EndPoint}");
            // Vertical gradient:
            linearGradBrush = new LinearGradientBrush(Color.Red, new PointF(0, 0), Color.Green, new PointF(0, 1));
            drawSwatch(linearGradBrush, $"Linear gradient\r\nfrom {linearGradBrush.StartPoint} to {linearGradBrush.EndPoint}");
            // Diagonal gradient (increase swatch height to better show diagonal):
            testRectSize.Height *= 2;
            linearGradBrush = new LinearGradientBrush(Color.Red, new PointF(0, 0), Color.Teal, new PointF(1, 1));
            drawSwatch(linearGradBrush, $"Linear gradient\r\nfrom {linearGradBrush.StartPoint} to {linearGradBrush.EndPoint}");
            // RadialGradientBrush
            rc = Common.Util.AddNote("Radial gradients using RadialGradientBrush:", doc.Pages.Last, new RectangleF(ip, new SizeF(500, 100)));
            ip.Y = rc.Bottom + dy;
            // Centered:
            RadialGradientBrush radialGradBrush = new RadialGradientBrush(Color.Orange, Color.Purple);
            drawSwatch(radialGradBrush, $"Radial gradient\r\nwith origin at {radialGradBrush.GradientOrigin}");
            // Center in bottom right corner:
            radialGradBrush = new RadialGradientBrush(Color.OrangeRed, Color.DarkBlue, new PointF(1, 1));
            drawSwatch(radialGradBrush, $"Radial gradient\r\nwith origin at {radialGradBrush.GradientOrigin}");
            // Done:
            doc.Save(stream);
        }
    }
}
