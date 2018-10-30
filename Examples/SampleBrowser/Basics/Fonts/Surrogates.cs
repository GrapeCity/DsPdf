using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // This sample renders a variety of interesting Unicode characters
    // including surrogate pairs. It also implicitly uses the automatic
    // font fallback (font substitution) feature built into GcPdf.
    // Note that this sample may produce different results on different
    // systems, as it relies on system-provided fallback fonts.
    // For a platform- and system-independent version of this sample see SurrogatesPort.
    // See also FontFallbacks.
    public class Surrogates
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var g = doc.NewPage().Graphics;
            // For most of the 'interesting' chars demoed in this sample,
            // fallback fonts (see FontFallbacks) will be automatically used,
            // so we can just use a standard font for the captions:
            var font = StandardFonts.Helvetica;

            TextFormat tf = new TextFormat() { Font = font, FontSize = 12 };
            TextFormat tf1 = new TextFormat(tf) { FontSize = 14 };

            g.DrawString("Some Interesting Unicode Characters (relies on system fallbacks)",
                new TextFormat(tf) { Underline = true, FontSize = tf.FontSize + 2 },
                new RectangleF(0, 36, doc.PageSize.Width, float.MaxValue),
                TextAlignment.Center);

            // Set up text insertion point and its advance function:
            PointF ip = new PointF(72, 54);
            Func<bool, PointF> nextIp = (caption_) => { ip.Y += caption_ ? 30 : 20; return ip; };

            // Draw the strings:
            g.DrawString("Surrogate Pairs:", tf, nextIp(true));
            g.DrawString("\uD867\uDEDB \uD840\uDC0B \uD834\uDD1E \uD834\uDD61 \uD83D\uDC04", tf1, nextIp(false));

            g.DrawString("Currency Symbols:", tf, nextIp(true));
            g.DrawString("\u0024 \u20A0 \u20A1 \u20A2 \u20A3 \u20A4 \u20AC \u20B9 \x20BD", tf1, nextIp(false));

            g.DrawString("Mathematical Operators:", tf, nextIp(true));
            g.DrawString("\u221A \u222B \u2211 \u2210 \u2264 \u2265 \u2202 \u2208", tf1, nextIp(false));

            g.DrawString("CJK Ideographs Extension A:", tf, nextIp(true));
            g.DrawString("\u3400 \u3401 \u3402 \u3403 \u3404 \u3405 \u3406 \u3407", tf1, nextIp(false));

            g.DrawString("Letterlike Symbols:", tf, nextIp(true));
            g.DrawString("\u2110 \u2111 \u2112 \u2113 \u2114 \u2115 \u211B \u211C", tf1, nextIp(false));

            g.DrawString("Private Use Area:", tf, nextIp(true));
            g.DrawString("\uE000 \uE001 \uE010 \uE011 \uE012 \uE013 \uE014 \uE015", tf1, nextIp(false));

            g.DrawString("Arrows:", tf, nextIp(true));
            g.DrawString("\u2190 \u2191 \u2192 \u2193 \u21B0 \u21E6 \u21CB \u21A9", tf1, nextIp(false));

            g.DrawString("Dingbats:", tf, nextIp(true));
            g.DrawString("\u2714 \u2717 \u275B \u275C \u2706 \u2707 \u2708 \u2709", tf1, nextIp(false));

            g.DrawString("Braille Patterns:", tf, nextIp(true));
            g.DrawString("\u2830 \u2831 \u2832 \u2833 \u2834 \u2835 \u2836 \u2837", tf1, nextIp(false));

            g.DrawString("Geometric Shapes:", tf, nextIp(true));
            g.DrawString("\u25D0 \u25D1 \u25D2 \u25D3 \u25A4 \u25F0 \u25BC \u25CE", tf1, nextIp(false));

            g.DrawString("Latin Extended A:", tf, nextIp(true));
            g.DrawString("\u0100 \u0101 \u0102 \u0103 \u0104 \u0105 \u0106 \u0107", tf1, nextIp(false));

            g.DrawString("Miscellaneous Symbols:", tf, nextIp(true));
            g.DrawString("\u2600 \u2601 \u2602 \u2603 \u2604 \u2605 \u2606 \u2607 \u2608 \u2609 \u2614 \u2615 \u26F0", tf1, nextIp(false));

            // Done:
            doc.Save(stream);
        }
    }
}
