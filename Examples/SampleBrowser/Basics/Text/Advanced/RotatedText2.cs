//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using System.Numerics;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // Shows how to use GcPdfGraphics.Transform to rotate a text string
    // (alternative way using matrix multiplication).
    // See also RotatedText.
    public class RotatedText2
    {
        public void CreatePDF(Stream stream)
        {
            // Rotation angle, degrees clockwise:
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
            // Rotation center point in the middel of text bounding rectangle:
            var center = new PointF(ip.X + tl.ContentWidth / 2, ip.Y + tl.ContentHeight / 2);
            // Transformations can be combined by multiplying matrices.
            // Note that matrix multiplication is not commutative - 
            // the sequence of operands is important, and is applied from last to first
            // matrices being multiplied:
            // 3) Translate the origin back to (0,0):
            // 2) Rotate around new origin by the specified angle:
            // 1) Translate the origin from default (0,0) to rotation center:
            g.Transform =
                Matrix3x2.CreateTranslation(-center.X, -center.Y) *
                Matrix3x2.CreateRotation((float)(angle * Math.PI) / 180f) *
                Matrix3x2.CreateTranslation(center.X, center.Y);
            // Draw rotated text and bounding rectangle:
            g.DrawTextLayout(tl, ip);
            g.DrawRectangle(rect, Color.Black, 1);
            // Remove transformation and draw the bounding rectangle where the non-rotated text would have been:
            g.Transform = Matrix3x2.Identity;
            g.DrawRectangle(rect, Color.ForestGreen, 1);
            // Done:
            doc.Save(stream);
        }
    }
}
