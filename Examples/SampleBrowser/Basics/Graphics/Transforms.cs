using System;
using System.IO;
using System.Drawing;
using System.Numerics;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // Shows how to use graphics transformations (GcPdfGraphics.Transform property).
    public class Transforms
    {
        // Helper method drawing a filled box with text:
        private void DrawBox(string text, GcGraphics g, RectangleF box)
        {
            g.FillRectangle(box, Color.FromArgb(80, 0, 184, 204));
            g.DrawRectangle(box, Color.FromArgb(0, 193, 213), 1);
            box.Inflate(-6, -6);
            g.DrawString(text, new TextFormat() { Font = StandardFonts.Times, FontSize = 14, }, box);
        }

        public void CreatePDF(Stream stream)
        {
            const string baseTxt = "Text drawn at (0,36) in a 4\"x2\" box";
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;
            var box = new RectangleF(0, 36, 72 * 4, 72 * 2);
            // #1:
            DrawBox($"Box 1: {baseTxt}, no transformations.", g, box);
            //
            var translate0 = Matrix3x2.CreateTranslation(72 * 1, 72 * 4);
            var scale0 = Matrix3x2.CreateScale(0.5f);

            // Transforms are applied in order from last to first.

            // #2:
            g.Transform =
                scale0 *
                translate0;
            DrawBox($"Box 2: {baseTxt}, translated by (1\",4\") and scaled by 0.5.", g, box);
            // #3:
            g.Transform =
                translate0 *
                scale0;
            DrawBox($"Box 3: {baseTxt}, scaled by 0.5 and translated by (1\",4\").", g, box);
            //
            var translate1 = Matrix3x2.CreateTranslation(72 * 3, 72 * 5);
            var scale1 = Matrix3x2.CreateScale(0.7f);
            var rotate0 = Matrix3x2.CreateRotation((float)(-70 * Math.PI) / 180f); // 70 degrees CCW
            // #4:
            g.Transform =
                rotate0 *
                translate1 *
                scale1;
            DrawBox($"Box 4: {baseTxt}, scaled by 0.7, translated by (3\",5\"), and rotated 70 degrees counterclockwise.", g, box);
            // #5:
            g.Transform =
                Matrix3x2.CreateTranslation(36, 72) *
                g.Transform;
            DrawBox($"Box 5: {baseTxt}, applied current transform (Box 4), and translated by (1/2\",1\").", g, box);
            // #6:
            g.Transform =
                // rotate0 *
                Matrix3x2.CreateSkew((float)(-45 * Math.PI) / 180f, (float)(20 * Math.PI) / 180f) *
                Matrix3x2.CreateTranslation(72 * 3, 72 * 7);
            DrawBox($"Box 6: {baseTxt}, translated by (3\",7\"), and skewed -45 degrees on axis X and 20 degrees on axis Y.", g, box);
            // #7:
            g.Transform =
                Matrix3x2.CreateRotation((float)Math.PI) *
                Matrix3x2.CreateTranslation(page.Size.Width - 72, page.Size.Height - 72);
            DrawBox($"Box 7: {baseTxt}, translated by (7.5\",10\"), and rotated by 180 degrees.", g, box);
            // We can remove any transformations on a graphics like so:
            g.Transform = Matrix3x2.Identity;

            // Done:
            doc.Save(stream);
        }
    }
}
