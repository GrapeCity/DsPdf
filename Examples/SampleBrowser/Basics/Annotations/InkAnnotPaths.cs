//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Pdf.Annotations;

namespace GcPdfWeb.Samples.Basics
{
    // Shows how to add an ink annotation to a PDF document,
    // and how to render its content using the InkAnnotation.Paths property.
    public class InkAnnotPaths
    {
        public int CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            // User name for the annotation's author:
            var user1 = "Jaime Smith";

            var rc = Common.Util.AddNote(
                "This sample creates an ink annotation and shows how to use the InkAnnotation.Paths property " +
                "to render the annotation's content. The content is specified by discrete points that should be " +
                "connected when a PDF viewer renders them. The points can be connected either by straight or " +
                "curved lines depending on the viewer implementation.",
                page);

            var inkAnnot = new InkAnnotation()
            {
                UserName = user1,
                Rect = new RectangleF(rc.Left, rc.Bottom + 20, 72 * 5, 72 * 2),
                LineWidth = 2,
                Color = Color.DarkBlue,
                Contents = "This is an ink annotation drawn via InkAnnotation.Paths."
            };
            float x0 = 80, x = 80, y = rc.Bottom + 24, h = 18, dx = 2, dy = 4, dx2 = 4, w = 10, xoff = 15;

            // Scribble 'ink annotation' text:
            
            // i
            inkAnnot.Paths.Add(new[] { new PointF(x + w/2, y), new PointF(x + w/2, y + h), new PointF(x + w, y + h*.7f) });
            inkAnnot.Paths.Add(new[] { new PointF(x + w/2 - dx, y - h/3 + dy), new PointF(x + w/2 + dx, y - h/3) });
            // n
            x += xoff;
            inkAnnot.Paths.Add(new[] { new PointF(x, y), new PointF(x, y + h), new PointF(x, y + h - dy), new PointF(x + w*0.7f, y),
                new PointF(x + w - dx/2, y + h*.6f), new PointF(x + w, y + h), new PointF(x + w + dx2, y + h*.7f) });
            // k
            x += xoff;
            inkAnnot.Paths.Add(new[] { new PointF(x, y - h/3), new PointF(x, y + h) });
            inkAnnot.Paths.Add(new[] { new PointF(x + w, y), new PointF(x + dx, y + h/2 - dy), new PointF(x, y + h/2),
                new PointF(x + dx2, y + h/2 + dy), new PointF(x + w, y + h), new PointF(x + w + dx2, y + h*.7f) });

            // a
            x += xoff * 2;
            inkAnnot.Paths.Add(new[] { new PointF(x + w, y + dy), new PointF(x + w/2, y), new PointF(x, y + h/2), new PointF(x + w/2, y + h),
                new PointF(x + w, y + dy), new PointF(x + w, y), new PointF(x + w, y + h), new PointF(x + w + dx2, y + h*.7f) });
            // n
            x += xoff;
            inkAnnot.Paths.Add(new[] { new PointF(x, y), new PointF(x, y + h), new PointF(x, y + h - dy), new PointF(x + w*0.7f, y),
                new PointF(x + w - dx/2, y + h*.6f), new PointF(x + w, y + h), new PointF(x + w + dx2, y + h*.7f) });
            // n
            x += xoff;
            inkAnnot.Paths.Add(new[] { new PointF(x, y), new PointF(x, y + h), new PointF(x, y + h - dy), new PointF(x + w*0.7f, y),
                new PointF(x + w - dx/2, y + h*.6f), new PointF(x + w, y + h), new PointF(x + w + dx2, y + h*.7f) });
            // o
            x += xoff;
            inkAnnot.Paths.Add(new[] { new PointF(x + w/2, y), new PointF(x + w/2 - dx, y), new PointF(x, y + h/2), new PointF(x + w/2, y + h),
                new PointF(x + w, y + h/2), new PointF(x + w/2 + dx, y), new PointF(x + w/2, y) });
            // t
            x += xoff;
            inkAnnot.Paths.Add(new[] { new PointF(x + w/2, y - h/3), new PointF(x + w/2, y + h), new PointF(x + w, y + h*.7f) });
            inkAnnot.Paths.Add(new[] { new PointF(x, y), new PointF(x + w, y) });
            // a
            x += xoff;
            inkAnnot.Paths.Add(new[] { new PointF(x + w, y + dy), new PointF(x + w/2, y), new PointF(x, y + h/2), new PointF(x + w/2, y + h),
                new PointF(x + w, y + dy), new PointF(x + w, y), new PointF(x + w, y + h), new PointF(x + w + dx2, y + h*.7f) });
            // t
            x += xoff;
            inkAnnot.Paths.Add(new[] { new PointF(x + w/2, y - h/3), new PointF(x + w/2, y + h), new PointF(x + w, y + h*.7f) });
            inkAnnot.Paths.Add(new[] { new PointF(x, y), new PointF(x + w, y) });
            // i
            x += xoff;
            inkAnnot.Paths.Add(new[] { new PointF(x + w/2, y), new PointF(x + w/2, y + h), new PointF(x + w, y + h*.7f) });
            inkAnnot.Paths.Add(new[] { new PointF(x + w/2 - dx, y - h/3 + dy), new PointF(x + w/2 + dx, y - h/3) });
            // o
            x += xoff;
            inkAnnot.Paths.Add(new[] { new PointF(x + w/2, y), new PointF(x + w/2 - dx, y), new PointF(x, y + h/2),
                new PointF(x + w/2, y + h), new PointF(x + w, y + h/2), new PointF(x + w/2 + dx, y), new PointF(x + w/2, y) });
            // n
            x += xoff;
            inkAnnot.Paths.Add(new[] { new PointF(x, y), new PointF(x, y + h), new PointF(x, y + h - dy), new PointF(x + w*0.7f, y),
                new PointF(x + w - dx/2, y + h*.6f), new PointF(x + w, y + h), new PointF(x + w + dx2, y + h*.7f),
                new PointF(x + w*3, y + h*.4f), new PointF(x + w + dx2, y + h + dy*2), new PointF(x0, y + h + dy)} );

            page.Annotations.Add(inkAnnot);

            // Done:
            doc.Save(stream);
            return doc.Pages.Count;
        }
    }
}
