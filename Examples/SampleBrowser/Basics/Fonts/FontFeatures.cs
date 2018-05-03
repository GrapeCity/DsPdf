using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // A simple demonstration of some interesting font features in action.
    // For the complete list of font features see <a class="nocode a" target="_blank" href="https://www.microsoft.com/typography/otspec/featurelist.htm">featurelist.htm</a>.
    // See also Ligatures.
    public class FontFeatures
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var g = doc.NewPage().Graphics;
            //
            var font = Font.FromFile(Path.Combine("Resources", "Fonts", "Gabriola.ttf"));
            var tf = new TextFormat() { Font = font, FontSize = 20 };
            //
            var tl = g.CreateTextLayout();
            tl.AppendLine("Line with no custom font features.", tf);
            //
            tf.FontFeatures = new FontFeature[] { new FontFeature(FeatureTag.ss03) };
            tl.AppendLine("Line with font feature ss03 enabled.", tf);
            //
            tf.FontFeatures = new FontFeature[] { new FontFeature(FeatureTag.ss05) };
            tl.AppendLine("Line with font feature ss05 enabled.", tf);
            //
            tf.FontFeatures = new FontFeature[] { new FontFeature(FeatureTag.ss07) };
            tl.AppendLine("Line with font feature ss07 enabled.", tf);
            //
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, new PointF(72, 72));
            // Done:
            doc.Save(stream);
        }
    }
}
