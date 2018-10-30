using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // Shows how to render private use Unicode characters (PUA) with custom EUDC fonts (.tte).
    public class EUDC
    {
        public void CreatePDF(Stream stream)
        {
            // Test string using EUDC codes and two regular chars (& and !): 0xE620 0xE621 0xE622 0xE624 & 0xE623 !
            const string tstr = "&!";
            // Set up:
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;
            var tf = new TextFormat() { FontSize = 20 };
            var rc = Common.Util.AddNote(
                "This sample demonstrates how to render private use Unicode characters (PUA) with custom EUDC fonts (.tte).\n" +
                "A GrapeCity.Documents.Text.Font can be created from an EUDC .tte file, " +
                "and linked to one or more fonts using Font.AddEudcFont() method.",
                page);
            const float dy = 36;
            var ip = new PointF(rc.X, rc.Bottom + dy / 2);

            // Use FontCollection to allow fetching fonts by family names:
            var fc = new FontCollection();

            // Assign the font collection to the graphics so that MeasureString/DrawString
            // methods on the graphics can find fallback fonts:
            g.FontCollection = fc;

            // Register some regular fonts with the FontCollection:
            fc.RegisterFont(Path.Combine("Resources", "Fonts", "arial.ttf"));
            fc.RegisterFont(Path.Combine("Resources", "Fonts", "times.ttf"));
            fc.RegisterFont(Path.Combine("Resources", "Fonts", "yumin.ttf"));
            fc.RegisterFont(Path.Combine("Resources", "Fonts", "msgothic.ttc"));
            fc.RegisterFont(Path.Combine("Resources", "Fonts", "YuGothR.ttc"));

            // Tell the font collection to use Yu Mincho as a fallback:
            fc.AppendFallbackFonts(fc.FindFamilyName("Yu Mincho"));

            // Using Arial font renders the test string as empty rectangles, as suitable glyphs are not present in Arial:
            tf.Font = fc.FindFamilyName("Arial", false, false);
            g.DrawString($"Arial: {tstr} (no EUDC font has been linked yet)", tf, ip);
            ip.Y += dy;

            // Load two custome EUDC fonts:
            var eudcF0 = Font.FromFile(Path.Combine("Resources", "Fonts", "Eudc0.tte"));
            var eudcF1 = Font.FromFile(Path.Combine("Resources", "Fonts", "Eudc1.tte"));

            // Link one EUDC font to Arial - now in strings rendered with Arial, EUDC chars will be looked up in this font:
            var font = fc.FindFamilyName("Arial");
            font.AddEudcFont(eudcF0);
            // Ditto for Yu Mincho font:
            font = fc.FindFamilyName("Yu Mincho");
            font.AddEudcFont(eudcF0);
            // Link another EUDC font to Yu Gothic:
            font = fc.FindFamilyName("Yu Gothic");
            font.AddEudcFont(eudcF1);

            // Render strings with EUDC chars using fonts to which our custom EUDC font is linked:
            tf.Font = fc.FindFamilyName("Arial", false, false);
            g.DrawString($"Arial, linked with Eudc0.tte: {tstr}", tf, ip);
            ip.Y += dy;
            tf.Font = fc.FindFileName("times.ttf");
            g.DrawString($"Times, fallback via Yu Mincho: {tstr}", tf, ip);
            ip.Y += dy;
            tf.Font = fc.FindFamilyName("MS Gothic");
            g.DrawString($"MS Gothic, fallback via Yu Mincho: {tstr}", tf, ip);
            ip.Y += dy;
            tf.Font = fc.FindFamilyName("Yu Gothic");
            g.DrawString($"Yu Gothic, linked with Eudc1.tte: {tstr}", tf, ip);
            ip.Y += dy;

            // FontCollection adds some services (like font lookup by family name),
            // but EUDC fonts can be linked to fonts that are not in a collection:
            font = Font.FromFile(Path.Combine("Resources", "Fonts", "Gabriola.ttf"));
            font.AddEudcFont(eudcF0);
            tf.Font = font;
            g.DrawString($"Gabriola Font, linked with Eudc0.tte: {tstr}", tf, ip);
            ip.Y += dy;
            // Done:
            doc.Save(stream);
        }
    }
}
