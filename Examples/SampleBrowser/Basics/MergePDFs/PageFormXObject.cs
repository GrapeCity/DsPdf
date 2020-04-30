//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Pdf.Annotations;
using GrapeCity.Documents.Pdf.Graphics;

namespace GcPdfWeb.Samples
{
    // This sample shows how to create FormXObject representing a page
    // from a PDF loaded into a temporary GcPdfDocument, and render that
    // object into the current document.
    public class PageFormXObject
    {
        public int CreatePDF(Stream stream)
        {
            GcPdfDocument doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;

            var rc = Common.Util.AddNote(
                "The thumbnails below are pages from an existing document. " +
                "We load an arbitrary PDF into a temporary GcPdfDocument, " +
                "then create a FormXObject representing each of its pages, " +
                "and render those objects as thumbnails into the current document. " +
                "To show that the same FormXObject can be used more than once, " +
                "we also render each thumbnail a second time applying a mirror transform.",
                page);

            // Layout params:
            var margin = rc.Left;
            var pad = 36;
            var side = (page.Size.Width - margin * 2 - pad) / 2;
            var ip = new PointF(margin, rc.Bottom + pad);
            // Mirror transform:
            var tr = Matrix3x2.CreateScale(-1, 1) * Matrix3x2.CreateTranslation(page.Size.Width, 0);
            // Text format for the overlaid page captions:
            var color = Color.DarkRed;
            var tf = new TextFormat()
            {
                Font = StandardFonts.HelveticaBold,
                FontSize = 16,
                ForeColor = Color.FromArgb(128, color),
            };
            // Open an arbitrary PDF, load it into a temp document and loop over its pages,
            // drawing each into the current document:
            using (var fs = new FileStream(Path.Combine("Resources", "PDFs", "Wetlands.pdf"), FileMode.Open, FileAccess.Read))
            {
                var doc1 = new GcPdfDocument();
                doc1.Load(fs);
                // Create a list of FormXObject for the pages of the loaded PDF:
                var fxos = new List<FormXObject>();
                doc1.Pages.ToList().ForEach(p_ => fxos.Add(new FormXObject(doc, p_)));
                // Render the thumbnails into the current document:
                for (int i = 0; i < fxos.Count; ++i)
                {
                    if (ip.Y + side > page.Size.Height - margin)
                    {
                        page = doc.NewPage();
                        g = page.Graphics;
                        ip = new PointF(margin, margin);
                    }
                    var rcfx = new RectangleF(ip.X, ip.Y, side, side);
                    // Draw direct:
                    g.DrawForm(fxos[i], rcfx, null, ImageAlign.ScaleImage);
                    g.DrawRectangle(rcfx, color);
                    g.DrawString($"Page {i + 1}", tf, rcfx, TextAlignment.Center, ParagraphAlignment.Center, false);
                    // Draw reversed:
                    g.Transform = tr;
                    g.DrawForm(fxos[i], rcfx, null, ImageAlign.ScaleImage);
                    g.DrawRectangle(rcfx, color);
                    g.Transform = Matrix3x2.Identity;
                    rcfx.Offset(side + pad, 0);
                    g.DrawString($"Reversed page {i + 1}", tf, rcfx, TextAlignment.Center, ParagraphAlignment.Center, false);
                    //
                    ip.Y += side + pad;
                }
            }
            // Done:
            doc.Save(stream);
            return doc.Pages.Count;
        }
    }
}
