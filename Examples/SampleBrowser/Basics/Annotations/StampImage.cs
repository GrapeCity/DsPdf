//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Pdf.Annotations;
using GrapeCity.Documents.Pdf.Graphics;
using System.Numerics;

namespace GcPdfWeb.Samples.Basics
{
    // This sample demonstrates how to add a custom appearance stream
    // to an annotation using a FormXObject.
    // In the sample, an existing PDF is loaded, and then we loop over
    // the document's pages. On each page, a StampAnnotation is created, and a FormXObject
    // which is assigned to the annotation's normal default appearance stream.
    // A semi-transparent PNG image is then drawn on the FormXObject's
    // Graphics using regular GcGraphics features (Transform and DrawImage).
    public class StampImage
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            // Load an existing PDF to which we will add a stamp annotation
            // (see LoadPDF for details on loading documents):
            var jsFile = Path.Combine("Resources", "PDFs", "The-Rich-History-of-JavaScript.pdf");
            using (var fs = new FileStream(jsFile, FileMode.Open, FileAccess.Read))
            {
                doc.Load(fs);
                var rect = new RectangleF(PointF.Empty, doc.Pages[0].Size);
                // Create a FormXObject to use as the stamp appearance:
                var fxo = new FormXObject(doc, rect);
                // Get an image from the resources, and draw it on the FormXObject's graphics
                // (see Transformations for details on using GcGraphics.Transform):
                using (var image = Image.FromFile(Path.Combine("Resources", "ImagesBis", "draft-copy-450x72.png")))
                {
                    var center = new Vector2(fxo.Bounds.Width / 2, fxo.Bounds.Height / 2);
                    fxo.Graphics.Transform =
                        Matrix3x2.CreateRotation((float)(-55 * Math.PI) / 180f, center) *
                        Matrix3x2.CreateScale(6, center);
                    fxo.Graphics.DrawImage(image, fxo.Bounds, null, ImageAlign.CenterImage);
                    // Loop over pages, add a stamp to each page:
                    foreach (var page in doc.Pages)
                    {
                        // Create a StampAnnotation over the whole page:
                        var stamp = new StampAnnotation()
                        {
                            Icon = StampAnnotationIcon.Draft.ToString(),
                            Name = "draft",
                            Page = page,
                            Rect = rect,
                            UserName = "Jaime Smith"
                        };
                        // Re-use the same FormXObject on all pages:
                        stamp.AppearanceStreams.Normal.Default = fxo;
                    }
                    // Done:
                    doc.Save(stream);
                }
            }
        }
    }
}
