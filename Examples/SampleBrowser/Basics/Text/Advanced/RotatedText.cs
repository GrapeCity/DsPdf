using System;
using System.IO;
using System.Drawing;
using System.Numerics;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // Shows how to use GcPdfGraphics.Transform to rotate a text string.
    // See also RotatedText2.
    public class RotatedText
    {
        public void CreatePDF(Stream stream)
        {
            // Rotation angle, degrees clockwise
            float angle = -45;
            //
            var doc = new GcPdfDocument();
            var g = doc.NewPage().Graphics;
            // Create a text layout, pick a font and font size:
            TextLayout tl = g.CreateTextLayout();
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 24;
            // Add a text, and perform layout:
            tl.Append("Rotated text.");
            tl.PerformLayout(true);
            // Text insertion point at (1",1"):
            var ip = new PointF(72, 72);
            // Now that we have text size, create text rectangle with top left at insertion point:
            var rect = new RectangleF(ip.X, ip.Y, tl.ContentWidth, tl.ContentHeight);
            // Rotate the text around its bounding rect's center:
            // we now have the text size, and can rotate it about its center:
            g.Transform = Matrix3x2.CreateRotation((float)(angle * Math.PI) / 180f, new Vector2(ip.X + tl.ContentWidth / 2, ip.Y + tl.ContentHeight / 2));
            // Draw rotated text and bounding rectangle:
            g.DrawTextLayout(tl, ip);
            g.DrawRectangle(rect, Color.Black, 1);
            // Remove rotation and draw the bounding rectangle where the non-rotated text would have been:
            g.Transform = Matrix3x2.Identity;
            g.DrawRectangle(rect, Color.ForestGreen, 1);
            //
            doc.Save(stream);
        }
    }
}
