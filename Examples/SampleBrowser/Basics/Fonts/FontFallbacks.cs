//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // Fallback fonts are fonts used to draw glyphs that are not present
    // in a font specified by the application.
    // GcPdf provides a default list of fallback font families
    // that is automatically initialized, and includes large fonts
    // that are usually suitable to be used as fallbacks for many
    // languages for which some common fonts do not have the glyphs.
    // These automatically added fallback font families are available
    // via methods on the FontCollection.SystemFonts static collection.
    // You can customize the default (and system-dependent) behavior
    // by providing your own fallback fonts, and by adding them either
    // to fallbacks managed by the global FontCollection.SystemFonts,
    // by adding them to your own instance of the FontCollection,
    // or to specific fonts that you are using.
    // In this way the fallback font behavior can be finely tuned
    // and be completely system-independent.
    //
    // This sample demonstrates the basic fallback behavior -
    // clearing system fallbacks and re-adding them again.
    // Additionally, it prints the list of fallback fonts
    // found on the current system.
    public class FontFallbacks
    {
        public void CreatePDF(Stream stream)
        {
            // Set up GcPdfDocument:
            GcPdfDocument doc = new GcPdfDocument();
            GcPdfGraphics g = doc.NewPage().Graphics;

            // Set up some helper vars for rendering lines of text:
            const float margin = 36;
            // Insertion point (GcPdf's default resolution is 72dpi, use 1/2" margins all around):
            PointF ip = new PointF(margin, margin);
            // Init a text format with one of the standard fonts. Standard fonts are minimal
            // and contain very few glyphs for non-Latin characters.
            TextFormat tf = new TextFormat() { Font = StandardFonts.Courier, FontSize = 14 };

            // Get the list of fallback font families:
            string[] fallbacks = FontCollection.SystemFonts.GetFallbackFontFamilies();

            // Clear global fallbacks list:
            FontCollection.SystemFonts.ClearFallbackFontFamilies();
            FontCollection.SystemFonts.ClearFallbackFonts();

            // Now there are no global fallback fonts, so Japanese text rendered using
            // a standard font will produce 'blank boxes' instead of real Japanese characters:
            g.DrawString("A Japanese text that won't render: あなたは日本語を話せますか？", tf, ip);
            ip.Y += 36;

            // Re-add the original list of fallback font families to global SystemFonts:
            FontCollection.SystemFonts.AppendFallbackFontFamilies(fallbacks);
            // On some systems, default system fallbacks might not provide Japanese glyphs,
            // so we add our own fallback just in case:
            Font arialuni = Font.FromFile(Path.Combine("Resources", "Fonts", "arialuni.ttf"));
            FontCollection.SystemFonts.AppendFallbackFonts(arialuni);

            // Now that fallback fonts are available again, the same Japanese text will render
            // correctly as an appropriate fallback will have been found:
            g.DrawString("Same text with fallbacks available: あなたは日本語を話せますか？", tf, ip);
            ip.Y += 36;

            // Finally, we list all fallbacks and print a test line using each:
            Action<string> drawTestLine = (fnt_) =>
            {
                var tf1 = new TextFormat() { FontName = fnt_ };
                string tstr = $"{fnt_}: The quick brown fox jumps over the lazy dog.";
                var s = g.MeasureString(tstr, tf1, doc.PageSize.Width - margin * 2);
                g.DrawString(tstr, tf1, new RectangleF(ip, s));
                ip.Y += s.Height * 1.5f;
                if (ip.Y > doc.Pages.Last.Size.Height - margin * 2)
                {
                    g = doc.NewPage().Graphics;
                    ip.Y = 36;
                }
            };
            foreach (var fnt in fallbacks)
                drawTestLine(fnt);

            // Done:
            doc.Save(stream);
        }
    }
}
