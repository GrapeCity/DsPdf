using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;

namespace GcPdfWeb.Samples.Basics
{
    // Demonstrates the most basic paragraph formatting options:
    // - first line indent;
    // - line spacing.
    public class ParagraphFormatting
    {
        public void CreatePDF(Stream stream)
        {
            Func<string> makePara = () => Common.Util.LoremIpsum(1, 5, 10, 15, 30);

            var doc = new GcPdfDocument();
            var g = doc.NewPage().Graphics;
            // Using Graphics.CreateTextLayout() ensures that TextLayout's resolution
            // is set to the same value as that of the graphics (which is 72 dpi by default):
            var tl = g.CreateTextLayout();
            // Default font:
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 12;
            // Set TextLayout to the whole page:
            tl.MaxWidth = doc.PageSize.Width;
            tl.MaxHeight = doc.PageSize.Height;
            // ...and have it manage the page margins (1" all around):
            tl.MarginLeft = tl.MarginTop = tl.MarginRight = tl.MarginBottom = tl.Resolution;
            // First line offset 1/2":
            tl.FirstLineIndent = 72 / 2;
            // 1.5 line spacing:
            tl.LineSpacingScaleFactor = 1.5f;
            //
            tl.Append(makePara());
            tl.PerformLayout(true);
            // Render text at (0,0) (margins are added by TextLayout):
            g.DrawTextLayout(tl, PointF.Empty);
            //
            doc.Save(stream);
        }
    }
}
