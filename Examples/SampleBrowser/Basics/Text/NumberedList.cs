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
    // Demonstrates how paragraphs of text can be rendered as a numbered list in GcPdf.
    // The method of rendering pages of text in this sample is taken from the PaginatedText
    // sample. See also TextRendering.
    public class NumberedList
    {
        // Encapsulate page layout constants used in the sample:
        private struct Layout
        {
            public static float Margin => 72;
            public static float ListOffset => 24;
        };

        // Utility method which pre-pends numbers to all paragraphs in a TextLayout.
        private void AddBullets(GcGraphics g, PointF pt, TextLayout tl, ref int itemNo)
        {
            var tlBullet = g.CreateTextLayout();
            tlBullet.DefaultFormat.Font = StandardFonts.Times;
            tlBullet.DefaultFormat.FontSize = 12;
            foreach (var line in tl.Lines)
            {
                if (line.FirstLineInParagraph)
                {
                    tlBullet.Clear();
                    tlBullet.Append($"{itemNo++})");
                    tlBullet.PerformLayout(true);
                    g.DrawTextLayout(tlBullet, new PointF(pt.X, pt.Y + line.Position + line.LineGap));
                }
            }
        }

        // Main entry point:
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var ip = new PointF(Layout.Margin, Layout.Margin);
            // Use TextLayout.MarginLeft to reserve space for list numbers/bullets:
            var tl = new TextLayout(72)
            {
                MaxWidth = doc.PageSize.Width - Layout.Margin * 2,
                MaxHeight = doc.PageSize.Height - Layout.Margin * 2,
                ParagraphSpacing = 8,
                MarginLeft = Layout.ListOffset,
            };
            tl.DefaultFormat.Font = StandardFonts.Times;

            // Add 20 paragraphs of text that will render as a numbered list of 20 items:
            tl.Append(Common.Util.LoremIpsum(20, 1, 6));
            // Perform layout:
            tl.PerformLayout(true);
            // Use split options to provide widow/orphan control:
            TextSplitOptions to = new TextSplitOptions(tl);
            to.MinLinesInFirstParagraph = 2;
            to.MinLinesInLastParagraph = 2;
            // In a loop, split and render the text (see PaginatedText),
            // and add list numbers:
            int itemNo = 1;
            while (true)
            {
                // 'rest' will accept the text that did not fit:
                var splitResult = tl.Split(to, out TextLayout rest);
                var g = doc.Pages.Add().Graphics;
                g.DrawTextLayout(tl, ip);
                AddBullets(g, ip, tl, ref itemNo);
                if (splitResult != SplitResult.Split)
                    break;
                tl = rest;
            }
            // Done:
            doc.Save(stream);
        }
    }
}
