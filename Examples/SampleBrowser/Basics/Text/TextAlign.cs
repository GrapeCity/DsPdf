using System;
using System.Drawing;
using System.IO;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // Demonstrates text alignment options (horizontal alignment for LRT text):
    // Left / Centered / Right / Justified.
    public class TextAlign
    {
        public void CreatePDF(Stream stream)
        {
            // Helper function to generate a paragraph of random text:
            Func<string> makePara = () => Common.Util.LoremIpsum(1, 3, 4, 15, 20);
            //
            var doc = new GcPdfDocument();
            var g = doc.NewPage().Graphics;
            // Using Graphics.CreateTextLayout() ensures that TextLayout's resolution
            // is set to the same value as that of the graphics (which is 72 dpi by default):
            var tl = g.CreateTextLayout();
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 12;
            // Set layout size (full page with 1" margins):
            tl.MaxWidth = doc.Pages.Last.Size.Width - 72 * 2;
            tl.MaxHeight = doc.Pages.Last.Size.Height - 72 * 2;
            // Strong text format for 'headers':
            TextFormat tf = new TextFormat(tl.DefaultFormat) { Font = StandardFonts.TimesBold };
            // Insertion point:
            PointF ip = new PointF(72, 72);
            // TextAlignment controls how text is aligned horizontally within the layout rectangle.
            // We render 5 paragraphs with different alignments.
            // Leading:
            tl.TextAlignment = TextAlignment.Leading;
            tl.Append("TextAlignment.Leading: ", tf);
            tl.Append(makePara());
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, ip);
            // Advance insertion point, adding one line's height between paragraphs:
            ip.Y += tl.ContentHeight + tl.Lines[0].Height;
            // Center:
            tl.Clear();
            tl.TextAlignment = TextAlignment.Center;
            tl.Append("TextAlignment.Center: ", tf);
            tl.Append(makePara());
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, ip);
            // Advance insertion point, adding one line's height between paragraphs:
            ip.Y += tl.ContentHeight + tl.Lines[0].Height;
            // Trailing:
            tl.Clear();
            tl.TextAlignment = TextAlignment.Trailing;
            tl.Append("TextAlignment.Trailing: ", tf);
            tl.Append(makePara());
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, ip);
            // Advance insertion point, adding one line's height between paragraphs:
            ip.Y += tl.ContentHeight + tl.Lines[0].Height;
            // Justified:
            tl.Clear();
            tl.TextAlignment = TextAlignment.Justified;
            tl.Append("TextAlignment.Justified: ", tf);
            tl.Append(makePara());
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, ip);
            // Advance insertion point, adding one line's height between paragraphs:
            ip.Y += tl.ContentHeight + tl.Lines[0].Height;
            // Distributed:
            tl.Clear();
            tl.TextAlignment = TextAlignment.Distributed;
            tl.Append("TextAlignment.Distributed: ", tf);
            tl.Append(makePara());
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, ip);
            // Done:
            doc.Save(stream);
        }
    }
}
