using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // Creates a simple 3-column text layout.
    // For a slightly more complex but practically more useful way
    // to render text in columns, see BalancedColumns.
    public class MultiColumnText
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var g = doc.NewPage().Graphics;
            var tl = g.CreateTextLayout();
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 12;
            tl.TextAlignment = TextAlignment.Justified;
            tl.AlignmentDelayToSplit = true;
            tl.FirstLineIndent = 72 / 2;
            tl.ParagraphSpacing = 72 / 8;
            // Add some text (note that TextLayout interprets "\r\n" as paragraph delimiter):
            tl.Append(Common.Util.LoremIpsum(20));
            // Set up columns:
            const int colCount = 3;
            const float margin = 72 / 2; // 1/2" margins all around
            const float colGap = margin / 4; // 1/4" gap between columns
            float colWidth = (doc.Pages.Last.Size.Width - margin * 2) / colCount - colGap * (colCount - 1);
            tl.MaxWidth = colWidth;
            tl.MaxHeight = doc.Pages.Last.Size.Height - margin * 2;
            // Calculate glyphs and perform layout for the whole text:
            tl.PerformLayout(true);
            // In a loop, split and render the text in the current column:
            int col = 0;
            while (true)
            {
                // The TextLayout that will hold the rest of the text which did not fit in the current layout:
                var splitResult = tl.Split(null, out TextLayout rest);
                g.DrawTextLayout(tl, new PointF(margin + col * (colWidth + colGap), margin));
                if (splitResult != SplitResult.Split)
                    break;
                tl = rest;
                if (++col == colCount)
                {
                    doc.Pages.Add();
                    g = doc.Pages.Last.Graphics;
                    col = 0;
                }
            }
            // Done:
            doc.Save(stream);
        }
    }
}
