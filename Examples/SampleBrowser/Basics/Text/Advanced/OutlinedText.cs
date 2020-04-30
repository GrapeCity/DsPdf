//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;

namespace GcPdfWeb.Samples.Basics
{
    // This sample shows how to render text with stroked glyph outlines,
    // and with glyphs filled using solid or gradient brushes.
    public class OutlinedText
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;

            var rc = Common.Util.AddNote(
                "This sample shows how to draw text with stroked glyph outlines, " +
                "and how to fill glyphs with solid or gradient brushes.",
                page);

            var tl = g.CreateTextLayout();

            // Set text flow and other layout properties:
            tl.MaxWidth = page.Size.Width;
            tl.MaxHeight = page.Size.Height;
            tl.MarginAll = 72;
            tl.MarginTop = rc.Bottom + 36;

            var rcBack = new RectangleF(tl.MarginLeft, tl.MarginTop, tl.MaxWidth.Value - tl.MarginLeft - tl.MarginRight, tl.MaxHeight.Value - tl.MarginTop - tl.MarginBottom);
            g.DrawImage(Image.FromFile(Path.Combine("Resources", "Images", "purples.jpg")), rcBack, null, ImageAlign.StretchImage);

            TextFormat tf0 = new TextFormat()
            {
                ForeColor = Color.LemonChiffon,
                Hollow = true,
                Font = Font.FromFile(Path.Combine("Resources", "Fonts", "GOTHICB.TTF")),
                FontSize = 48,
            };
            tl.AppendLine("Hollow Text", tf0);

            TextFormat tf1 = new TextFormat()
            {
                StrokePen = Color.DarkMagenta,
                FillBrush = new SolidBrush(Color.Yellow),
                Font = Font.FromFile(Path.Combine("Resources", "Fonts", "FoglihtenNo07.otf")),
                FontSize = 48,
            };
            tl.AppendLine("Outlined Text", tf1);

            var grad0 = new LinearGradientBrush(Color.Red, new PointF(0, 0), Color.Blue, new PointF(1, 0));
            TextFormat tf2 = new TextFormat()
            {
                FillBrush = grad0,
                Font = Font.FromFile(Path.Combine("Resources", "Fonts", "cambriab.ttf")),
                FontSize = 48,
            };
            tl.AppendLine("Gradient Fill", tf2);

            var grad1 = new LinearGradientBrush(Color.Red, Color.Purple);
            grad1.GradientStops = new List<GradientStop>();
            grad1.GradientStops.Add(new GradientStop(Color.Orange, 1 / 6f));
            grad1.GradientStops.Add(new GradientStop(Color.Yellow, 2 / 6f));
            grad1.GradientStops.Add(new GradientStop(Color.Green, 3 / 6f));
            grad1.GradientStops.Add(new GradientStop(Color.Cyan, 4/ 6f));
            grad1.GradientStops.Add(new GradientStop(Color.Blue, 5 / 6f));
            TextFormat tf3 = new TextFormat()
            {
                FillBrush = grad1,
                Font = Font.FromFile(Path.Combine("Resources", "Fonts", "cambriab.ttf")),
                FontSize = 48,
            };
            tl.AppendLine("Multi-stop gradient", tf3);

            var tf4 = new TextFormat(tf3);
            tf4.StrokePen = Color.GreenYellow;
            tl.AppendLine("Multi-stop gradient with outline", tf4);

            var tf5 = new TextFormat(tf4);
            tf5.Hollow = true;
            tf5.StrokePen = new Pen(Color.DarkRed, 1);
            tl.AppendLine("Text outlined with 1pt wide pen", tf5);

            // It is not necessary to call PerformLayout() for a newly created TextLayout
            // or after a call to TextLayout.Clear():
            g.DrawTextLayout(tl, PointF.Empty);

            // Done:
            doc.Save(stream);
        }
    }
}
