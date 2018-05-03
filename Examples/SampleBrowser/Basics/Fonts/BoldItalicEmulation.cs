using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // Sample shows how to control bold and/or italic emulation when using normal fonts.
    public class BoldItalicEmulation
    {
        public void CreatePDF(Stream stream)
        {
            var fc = new FontCollection();
            fc.RegisterDirectory(Path.Combine("Resources", "Fonts"));
            var doc = new GcPdfDocument();
            var g = doc.NewPage().Graphics;
            var rc = Common.Util.AddNote(
                "TextFormat.FontStyle allows to turn on Bold and/or Italic emulation\n" +
                "on a regular (not a bold/italic) font.", doc.Pages.Last);
            // Text insertion point:
            PointF ip = new PointF(rc.Left, rc.Bottom + 36);
            TextFormat tf = new TextFormat();
            // We specifically get a non-bold/non-italic version of the font:
            tf.Font = fc.FindFamilyName("Times New Roman", false, false);
            tf.FontSize = 16;
            g.DrawString($"Regular Times font: {tf.Font.FullFontName}", tf, ip);
            ip.Y += 36;
            // Draw some strings using the same (regular) font but emulating bold and italic:
            tf.FontStyle = FontStyle.Bold;
            g.DrawString($"Bold emulation using font: {tf.Font.FullFontName}", tf, ip);
            ip.Y += 36;
            tf.FontStyle = FontStyle.Italic;
            g.DrawString($"Italic emulation using font: {tf.Font.FullFontName}", tf, ip);
            ip.Y += 36;
            tf.FontStyle = FontStyle.BoldItalic;
            g.DrawString($"Bold+Italic emulation using font: {tf.Font.FullFontName}", tf, ip);
            ip.Y += 36;
            //
            // Now we render some strings using the "real" bold/italic variants of the font:
            tf.FontStyle = FontStyle.Regular;
            tf.Font = fc.FindFamilyName("Times New Roman", true, false);
            g.DrawString($"Using real bold font: {tf.Font.FullFontName}", tf, ip);
            ip.Y += 36;
            tf.Font = fc.FindFamilyName("Times New Roman", false, true);
            g.DrawString($"Using real italic font: {tf.Font.FullFontName}", tf, ip);
            ip.Y += 36;
            tf.Font = fc.FindFamilyName("Times New Roman", true, true);
            g.DrawString($"Using real bold italic font: {tf.Font.FullFontName}", tf, ip);
            ip.Y += 36;
            // Done:
            doc.Save(stream);
        }
    }
}
