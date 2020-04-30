//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // This sample demonstrates how to create a drop cap in GcPdf.
    public class DropCap
    {
        public int CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var g = doc.NewPage().Graphics;
            // Get some text and split it into first letter (drop cap) and the rest:
            var text = Common.Util.LoremIpsum(1);
            var head = text.Substring(0, 1);
            var tail = text.Substring(1);
            // Use the Times font:
            var font = Font.FromFile(Path.Combine("Resources", "Fonts", "times.ttf"));
            // Text layout for the drop cap:
            var tlHead = g.CreateTextLayout();
            tlHead.DefaultFormat.Font = font;
            tlHead.DefaultFormat.FontSize = 40;
            tlHead.Append(head);
            tlHead.PerformLayout(true);
            // Text layout for the rest of the text:
            var tlTail = g.CreateTextLayout();
            tlTail.DefaultFormat.Font = font;
            tlTail.DefaultFormat.FontSize = 12;
            // Use whole page with 1" margins all around:
            tlTail.MaxWidth = doc.Pages.Last.Size.Width - 72 * 2;
            tlTail.MaxHeight = doc.Pages.Last.Size.Height - 72 * 2;
            tlTail.Append(tail);
            // Before we layout the main body of the text, we calculate the size and position
            // of the drop cap rectangle, and add it to the main text layout's ObjectRects -
            // the list of rectangles that the main text will flow around.
            //
            // Note: While we could simply position the drop cap rectangle at the top/left of the 
            // main text, it looks better if the tops of the drop cap and the main text's glyphs
            // are aligned. For this, we need to calculate the offets of letter tops within
            // the text bounding boxes, and adjust the position of the drop cap accordingly
            // (raise it a little).
            // (For this adjustment we need the sCapHeight field which is present if the font's
            // os/2 table version 2 and higher, so we must test for that and skip this step if
            // the CapHeight is not available).
            float dy = 0;
            if (font.CapHeight != -1)
            {
                // We move the drop cap position up by the amount equal to the difference between the
                // top spacing within the Em square of the drop cap's font size and the font size of the rest of the text:
                float k = tlHead.DefaultFormat.FontSize * tlHead.Resolution * tlHead.FontScaleFactor / (font.UnitsPerEm * 72);
                dy = (font.HorizontalAscender - font.CapHeight) * k;
                k /= tlHead.DefaultFormat.FontSize;
                k *= tlTail.DefaultFormat.FontSize;
                dy -= (font.HorizontalAscender - font.SmallXHeight) * k;
            }
            // Specify the rectangle for the main text to flow around:
            tlTail.ObjectRects = new List<ObjectRect>() { new ObjectRect(0, -dy, tlHead.ContentWidth * 1.2f, tlHead.ContentHeight) };
            // Layout the main text now:
            tlTail.PerformLayout(true);
            // Draw everything:
            g.DrawTextLayout(tlHead, new PointF(72, 72 - dy));
            g.DrawTextLayout(tlTail, new PointF(72, 72));
            // Done:
            doc.Save(stream);
            return doc.Pages.Count;
        }
    }
}
