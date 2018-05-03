using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // This sample renders a variety of interesting Unicode characters
    // including surrogate pairs, similar to the Surrogates sample.
    // Unlike that sample though, it does not rely on any system-provided
    // fallbacks. Instead, in this sample we purposefully restrict fallback
    // font lookup to the program's own font collection, and provide
    // our own set of fallback fonts.
    // This makes the code platform and system independent, so it produces
    // exactly the same result on Windows, Linux or Mac.
    public class SurrogatesPort
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;

            // As in the Surrogates sample, we specify a standard font
            // (which lacks many of the glyphs we will be rendering),
            // and will rely on font fallback support provided by FontCollection:
            var font = StandardFonts.Helvetica;

            // Set up text formats for captions, "interesting chars" and spacing:
            TextFormat tf = new TextFormat() { Font = font, FontSize = 12 };
            TextFormat tf1 = new TextFormat(tf) { FontSize = 14 };
            TextFormat tfs = new TextFormat(tf) { FontSize = 6 };

            // Create a font collection to use:
            FontCollection fc = new FontCollection();
            // Add our own fallback fonts to use (note that order is imortant here,
            // first come first found):
            fc.AppendFallbackFonts(
                Font.FromFile(Path.Combine("Resources", "Fonts", "arialuni.ttf")),
                Font.FromFile(Path.Combine("Resources", "Fonts", "l_10646.ttf")),
                Font.FromFile(Path.Combine("Resources", "Fonts", "seguiemj.ttf")),
                Font.FromFile(Path.Combine("Resources", "Fonts", "seguisym.ttf")),
                Font.FromFile(Path.Combine("Resources", "Fonts", "simsun.ttc")),
                Font.FromFile(Path.Combine("Resources", "Fonts", "times.ttf")),
                Font.FromFile(Path.Combine("Resources", "Fonts", "YuGothR.ttc"))
                );

            // Restricting default font lookup is done in the TextLayout, so unlike the
            // {Surrogates} sample, here we cannot use DrawString, but must use
            // TextLayout and DrawTextLayout directly:
            TextLayout tl = new TextLayout()
            {
                // Specify the font collection to use:
                FontCollection = fc,
                // Restrict default fonts/fallbacks lookup to the specified collection only:
                RestrictedFontLookup = true,
                FontFallbackScope = FontFallbackScope.FontCollectionOnly,
                // Set up other props to render the text:
                MaxWidth = page.Size.Width,
                MaxHeight = page.Size.Height,
                MarginLeft = 72,
                MarginRight = 72,
                MarginTop = 36,
                MarginBottom = 36,
                TextAlignment = TextAlignment.Center,
            };

            tl.Append("Some Interesting Unicode Characters (system-independent)",
                new TextFormat(tf) { Underline = true, FontSize = tf.FontSize + 2 });
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, PointF.Empty);

            tl.MarginTop = tl.ContentRectangle.Bottom + 20;
            tl.Clear();
            tl.TextAlignment = TextAlignment.Leading;

            // Draw the strings:
            tl.Append("Surrogate Pairs:\n", tf);
            tl.Append("\uD867\uDEDB \uD840\uDC0B \uD834\uDD1E \uD834\uDD61 \uD83D\uDC04\n", tf1);
            tl.Append("\n", tfs);

            tl.Append("Currency Symbols:\n", tf);
            tl.Append("\u0024 \u20A0 \u20A1 \u20A2 \u20A3 \u20A4 \u20AC \u20B9 \x20BD\n", tf1);
            tl.Append("\n", tfs);

            tl.Append("Mathematical Operators:\n", tf);
            tl.Append("\u221A \u222B \u2211 \u2210 \u2264 \u2265 \u2202 \u2208\n", tf1);
            tl.Append("\n", tfs);

            tl.Append("CJK Ideographs Extension A:\n", tf);
            tl.Append("\u3400 \u3401 \u3402 \u3403 \u3404 \u3405 \u3406 \u3407\n", tf1);
            tl.Append("\n", tfs);

            tl.Append("Letterlike Symbols:\n", tf);
            tl.Append("\u2110 \u2111 \u2112 \u2113 \u2114 \u2115 \u211B \u211C\n", tf1);
            tl.Append("\n", tfs);

            tl.Append("Private Use Area:\n", tf);
            tl.Append("\uE000 \uE001 \uE010 \uE011 \uE012 \uE013 \uE014 \uE015\n", tf1);
            tl.Append("\n", tfs);

            tl.Append("Arrows:\n", tf);
            tl.Append("\u2190 \u2191 \u2192 \u2193 \u21B0 \u21E6 \u21CB \u21A9\n", tf1);
            tl.Append("\n", tfs);

            tl.Append("Dingbats:\n", tf);
            tl.Append("\u2714 \u2717 \u275B \u275C \u2706 \u2707 \u2708 \u2709\n", tf1);
            tl.Append("\n", tfs);

            tl.Append("Braille Patterns:\n", tf);
            tl.Append("\u2830 \u2831 \u2832 \u2833 \u2834 \u2835 \u2836 \u2837\n", tf1);
            tl.Append("\n", tfs);

            tl.Append("Geometric Shapes:\n", tf);
            tl.Append("\u25D0 \u25D1 \u25D2 \u25D3 \u25A4 \u25F0 \u25BC \u25CE\n", tf1);
            tl.Append("\n", tfs);

            tl.Append("Latin Extended A:\n", tf);
            tl.Append("\u0100 \u0101 \u0102 \u0103 \u0104 \u0105 \u0106 \u0107\n", tf1);
            tl.Append("\n", tfs);

            tl.Append("Miscellaneous Symbols:\n", tf);
            tl.Append("\u2600 \u2601 \u2602 \u2603 \u2604 \u2605 \u2606 \u2607 \u2608 \u2609 \u2614 \u2615 \u26F0\n", tf1);
            tl.Append("\n", tfs);

            tl.PerformLayout(true);
            g.DrawTextLayout(tl, PointF.Empty);
            //
            doc.Save(stream);
        }
    }
}
