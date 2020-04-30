//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;

namespace GcPdfWeb.Samples
{
    // This sample demonstrates how to draw round rectangles
    // using dedicated DrawRoundRect/FillRoundRect methods.
    // It also shows how the same result may be achieved with
    // graphics paths.
    public class RoundRectangle
    {
        public int CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;

            var rc = Common.Util.AddNote(
                "GcPdfGraphics has dedicated methods to easily draw and fill rectangles with rounded corners. " +
                "This sample also shows how the same result may be achieved using graphics paths. " +
                "While they are not really needed for drawing round rectangles, graphics paths allow " +
                "to draw and fill arbitrary figures with complex geometries.",
                page);

            // Rounded rectangle's radii:
            float rx = 36, ry = 24;

            // Using dedicated methods to draw and fill round rectangles:
            var rEasy = new RectangleF(rc.Left, rc.Bottom + 36, 144, 72);
            g.FillRoundRect(rEasy, rx, ry, Color.PaleGreen);
            g.DrawRoundRect(rEasy, rx, ry, Color.Blue, 4);
            // Add a label:
            var tf = new TextFormat() { Font = StandardFonts.Times, FontSize = 14 };
            g.DrawString("The easy way.", tf, rEasy, TextAlignment.Center, ParagraphAlignment.Center, false);

            // Using graphics path to achieve the same result:
            var rHard = rEasy;
            rHard.Offset(0, rEasy.Height + 36);
            var path = MakeRoundRect(g, rHard, rx, ry);
            g.FillPath(path, Color.PaleVioletRed);
            g.DrawPath(path, Color.Purple, 4);
            // Add a label:
            g.DrawString("The hard way.", tf, rHard, TextAlignment.Center, ParagraphAlignment.Center, false);

            // Done:
            doc.Save(stream);
            return doc.Pages.Count;
        }

        // This method shows how to create a graphics path that may be used
        // to fill or draw arbitrary shapes on a GcGraphics.
        private IPath MakeRoundRect(GcGraphics g, RectangleF rc, float rx, float ry)
        {
            var path = g.CreatePath();
            var sz = new SizeF(rx, ry);
            // Start from horizontal top left:
            path.BeginFigure(new PointF(rc.Left + rx, rc.Top));
            path.AddLine(new PointF(rc.Right - rx, rc.Top));
            path.AddArc(new ArcSegment() { Point = new PointF(rc.Right, rc.Top + ry), SweepDirection = SweepDirection.Clockwise, Size = sz });
            path.AddLine(new PointF(rc.Right, rc.Bottom - ry));
            path.AddArc(new ArcSegment() { Point = new PointF(rc.Right - rx, rc.Bottom), SweepDirection = SweepDirection.Clockwise, Size = sz });
            path.AddLine(new PointF(rc.Left + rx, rc.Bottom));
            path.AddArc(new ArcSegment() { Point = new PointF(rc.Left, rc.Bottom - ry), SweepDirection = SweepDirection.Clockwise, Size = sz });
            path.AddLine(new PointF(rc.Left, rc.Top + ry));
            path.AddArc(new ArcSegment() { Point = new PointF(rc.Left + rx, rc.Top), SweepDirection = SweepDirection.Clockwise, Size = sz });
            path.EndFigure(FigureEnd.Closed);
            return path;
        }
    }
}
