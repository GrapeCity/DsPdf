using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;

namespace GcPdfWeb.Samples.Basics
{
    // This sample demonstrates how to use TextLayout.TabStops to render columns
    // of floating point numbers aligned in different ways:
    // - aligned on the decimal point via TabStopAlignment.Separator;
    // - left-aligned on the tab position using TabStopAlignment.Leading;
    // - centered around the tab position using TabStopAlignment.Center;
    // - right-aligned on the tab position using TabStopAlignment.Trailing.
    // [SampleOrdinal(55)]
    public class TabsAlignment
    {
        public void CreatePDF(Stream stream)
        {
            // Create and set up the document:
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;
            // Create and set up a TextLayout object to print the text:
            var tl = g.CreateTextLayout();
            tl.MaxWidth = page.Size.Width;
            tl.MaxHeight = page.Size.Height;
            tl.MarginLeft = tl.MarginRight = tl.MarginTop = tl.MarginBottom = 36;
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 10;
            tl.DefaultFormat.BackColor = Color.FromArgb(217, 217, 217);
            // Add tab stops with different alignment types:
            tl.TabStops = new List<TabStop>()
            {
                new TabStop(72, '.'), // this ctor creates a TabStopAlignment.Separator TabStop
                new TabStop(72 * 2.5f, TabStopAlignment.Leading),
                new TabStop(72 * 5, TabStopAlignment.Center),
                new TabStop(72 * 7.5f, TabStopAlignment.Trailing),
            };
            // Render sample text:
            tl.Append($"TabStopAlignment:\r\n\tSeparator '.'\tLeading\tCenter\tTrailing\r\n");
            double v0 = 1;
            double q = (1 + Math.Sqrt(5)) / 2;
            for (int i = 1; i < 50; ++i)
            {
                tl.Append($"\t{v0:R}\t{v0:R}\t{v0:R}\t{v0:R}\r\n");
                v0 *= q;
            }
            tl.PerformLayout(true);
            // Draw the text and images:
            g.DrawTextLayout(tl, PointF.Empty);
            // Done:
            doc.Save(stream);
        }
    }
}
