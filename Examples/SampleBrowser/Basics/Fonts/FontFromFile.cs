using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // This short sample demonstrates how a Font can be loaded from a file
    // and used in your code to render text.
    // The sample relies on font files Gabriola.ttf and timesbi.ttf to exist in the
    // Resources/Fonts folder.
    // 
    // NOTE 1: When Font.FromFile() is used, the actual data is loaded on demand,
    // so that usually a Font instance will not take too much space.
    // The situation is different for fonts created using Font.FromArray()
    // and Font.FromStream() methods - in those cases the whole font is
    // immediately loaded into memory. The font will still be parsed
    // only on demand, but memory consumption is slightly higher,
    // so using Font.FromFile() should generally be preferred.
    //
    // NOTE 2: When different Font instances (created using any of the static ctors
    // mentioned above) are used to render text in a PDF, each instance will result
    // in embedding a separate subset of glyphs even if the glyphs are the same,
    // because GcPdf has no way of knowing that two different Font instances 
    // represent the same physical font. So either make sure that only one Font instance
    // is created for each physical font, or better yet use the FontCollection class
    // to add the fonts you need, and specify them via TextFormat.FontName.
    public class FontFromFile
    {
        public void CreatePDF(Stream stream)
        {
            Font gabriola = Font.FromFile(Path.Combine("Resources", "Fonts", "Gabriola.ttf"));
            if (gabriola == null)
                throw new Exception("Could not load font Gabriola");

            // Now that we have our font, use it to render some text:
            TextFormat tf = new TextFormat() { Font = gabriola, FontSize = 16 };
            GcPdfDocument doc = new GcPdfDocument();
            GcPdfGraphics g = doc.NewPage().Graphics;
            g.DrawString($"Sample text drawn with font {gabriola.FontFamilyName}.", tf, new PointF(72, 72));
            // We can change the font size:
            tf.FontSize += 4;
            g.DrawString("The quick brown fox jumps over the lazy dog.", tf, new PointF(72, 72 * 2));
            // We can force GcPdf to emulate bold or italic style with a non-bold (non-italic) font, e.g.:
            tf.FontStyle = FontStyle.Bold;
            g.DrawString("This line prints with the same font, using emulated bold style.", tf, new PointF(72, 72 * 3));
            // But of course rather than emulated, it is much better to use real bold/italic fonts.
            // So finally, get a real bold italic font and print a line with it:
            Font timesbi = Font.FromFile(Path.Combine("Resources", "Fonts", "timesbi.ttf"));
            tf.Font = timesbi ?? throw new Exception("Could not load font timesbi");
            tf.FontStyle = FontStyle.Regular;
            g.DrawString($"This line prints with {timesbi.FullFontName}.", tf, new PointF(72, 72 * 4));
            // Done:
            doc.Save(stream);
        }
    }
}
