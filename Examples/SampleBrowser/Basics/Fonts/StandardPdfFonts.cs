using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // PDF specification lists 14 standard fonts that should always be available.
    // GcPdf has those fonts built in, and allows to use them directly as this sample demonstrates.
    public class StandardPdfFonts
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var g = doc.NewPage().Graphics;
            // Insertion point (GcPdf's default resolution is 72dpi, use 1" margins all around):
            const float margin = 72;
            PointF ip = new PointF(margin, margin);
            TextFormat tf = new TextFormat() { FontSize = 12 };
            Action<string, Font> drawText = (tag_, fnt_) =>
            {
                tf.Font = fnt_;
                string tstr = $"{tag_} ({fnt_.FullFontName}): The quick brown fox jumps over the lazy dog.";
                var s = g.MeasureString(tstr, tf, doc.PageSize.Width - margin * 2);
                g.DrawString(tstr, tf, new RectangleF(ip, s));
                ip.Y += s.Height * 1.5f;
            };
            // Draw samples of all 14 standard fonts:
            drawText("Helvetica", StandardFonts.Helvetica);
            drawText("HelveticaItalic", StandardFonts.HelveticaItalic);
            drawText("HelveticaBold", StandardFonts.HelveticaBold);
            drawText("HelveticaBoldItalic", StandardFonts.HelveticaBoldItalic);
            drawText("Times", StandardFonts.Times);
            drawText("TimesItalic", StandardFonts.TimesItalic);
            drawText("TimesBold", StandardFonts.TimesBold);
            drawText("TimesBoldItalic", StandardFonts.TimesBoldItalic);
            drawText("Courier", StandardFonts.Courier);
            drawText("CourierItalic", StandardFonts.CourierItalic);
            drawText("CourierBold", StandardFonts.CourierBold);
            drawText("CourierBoldItalic", StandardFonts.CourierBoldItalic);
            drawText("Symbol", StandardFonts.Symbol);
            drawText("ZapfDingbats", StandardFonts.ZapfDingbats);
            // Done:
            doc.Save(stream);
        }
    }
}
