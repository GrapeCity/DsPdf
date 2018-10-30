using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;

namespace GcPdfWeb.Samples.Basics
{
    // This sample demonstrates how to display ellipsis
    // if a string does not fit in the allocated space.
#if !DIODOCS_V1
#endif
    public class TextTrimming
    {
        public int CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;
            const float In = 72;

            var str = "This is a long line of text which does not fit in the allocated space.";
            var wid = In * 4;
            var dy = 0.3f;

            var rc = Common.Util.AddNote(
                "TextLayout allows to display ellipsis (or other character) " +
                "at the end of a text line that did not fit in the allocated space.\n" +
                "To use trimming, set TrimmingGranularity to Character or Word " +
                "(the default is None). Trimming will kick in if WrapMode is NoWrap, " +
                "and the text is too long. Wrapped text may may also display trimming if " +
                "the layout width is too narrow to fit a single word.\n" +
                "Below are examples of text untrimmed, trimmed to character and to word. " +
                "The next line demonstrates a different trimming character (tilde in this case). " +
                "Finally, the last line shows how to trim text (respecting TrimmingGranularity) " +
                "without adding any trimming character.",
                page);
            var top = rc.Bottom + 36;

            var ip = new PointF(rc.Left, top);

            var tl = g.CreateTextLayout();
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 12;
            tl.MaxWidth = wid;
            tl.WrapMode = WrapMode.NoWrap;

            // TrimmingGranularity is None by default:
            tl.Append(str);
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, ip);
            ip.Y += tl.ContentHeight + dy;

            // Character trimming:
            tl.TrimmingGranularity = TrimmingGranularity.Character;
            // NOTE that the recalculateGlyphsBeforeLayout parameter to PerformLayout
            // may be false after the first call, as the text/font has not changed:
            tl.PerformLayout(false);
            g.DrawTextLayout(tl, ip);
            ip.Y += tl.ContentHeight + dy;

            // Word trimming:
            tl.TrimmingGranularity = TrimmingGranularity.Word;
            tl.PerformLayout(false);
            g.DrawTextLayout(tl, ip);
            ip.Y += tl.ContentHeight + dy;

            // tl.EllipsisCharCode is HorizontalEllipsis (0x2026) by default.
            // Change it to a tilde:
            tl.EllipsisCharCode = 0x007E;
            tl.PerformLayout(false);
            g.DrawTextLayout(tl, ip);
            ip.Y += tl.ContentHeight + dy;

            // Finally, we may set tl.EllipsisCharCode to 0 to trim text
            // without rendering any trimming character:
            tl.EllipsisCharCode = 0;
            tl.PerformLayout(false);
            g.DrawTextLayout(tl, ip);
            ip.Y += tl.ContentHeight + dy;

            g.DrawRectangle(new RectangleF(rc.Left, top, wid, ip.Y - top), Color.OrangeRed);

            // Done:
            doc.Save(stream);
            return doc.Pages.Count;
        }
    }
}
