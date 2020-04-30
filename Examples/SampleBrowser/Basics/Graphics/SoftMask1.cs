//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Pdf.Graphics;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;

namespace GcPdfWeb.Samples
{
    // This sample demonstrates how to use GcPdfGraphics.SoftMask
    // to draw semi-transparently and specify clipping.
    public class SoftMask1
    {
        public int CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;

            var rc = Common.Util.AddNote(
                "GcPdfGraphics has a SoftMask property, which allows to create a mask with a FormXObject, " +
                "draw on that object's Graphics using any supported drawing methods " +
                "(including semi-transparent drawing), and then use the result as a mask when drawing " +
                "on the document's pages. Only the alpha channel from the mask is used. " +
                "Solid areas do not mask, transparent areas mask completely, " +
                "semi-transparent areas mask in inverse proportion to the alpha value.",
                page);

            var rMask = new RectangleF(0, 0, 72 * 5, 72 * 2);
            var rDoc = new RectangleF(rc.Left, rc.Bottom + 36, rMask.Width, rMask.Height);

            var softMask = SoftMask.Create(doc, rDoc);
            var smGraphics = softMask.FormXObject.Graphics;
            smGraphics.FillEllipse(rMask, Color.FromArgb(128, Color.Black));
            smGraphics.DrawString("SOLID TEXT",
                new TextFormat() { Font = StandardFonts.HelveticaBold, FontSize = 52, ForeColor = Color.Black },
                new RectangleF(rMask.X, rMask.Y, rMask.Width, rMask.Height),
                TextAlignment.Center, ParagraphAlignment.Center, false);
            var rt = rMask;
            rt.Inflate(-8, -8);
            // Color on the mask does not matter, only alpha channel is important:
            smGraphics.DrawEllipse(rt, Color.Red);

            g.SoftMask = softMask;
            g.DrawImage(Image.FromFile(Path.Combine("Resources", "Images", "reds.jpg")), rDoc, null, ImageAlign.StretchImage);
            // NOTE: it looks like some PDF viewers (such as built-in browser viewers)
            // do not handle changing soft masks correctly unless the mask is reset prior
            // to assigning a new one, hence this:
            g.SoftMask = SoftMaskBase.None;

            rDoc.Offset(0, rDoc.Height + 12);
            rDoc.Width = rc.Width;
            rDoc.Height = 36;
            rMask.Height = rDoc.Height;

            for (var alpha = 0f; alpha <= 255; alpha += 255f / 8)
            {
                softMask = SoftMask.Create(doc, rDoc);
                smGraphics = softMask.FormXObject.Graphics;
                smGraphics.DrawString($"Text drawn on mask with alpha {(int)alpha}.",
                    new TextFormat() { Font = StandardFonts.HelveticaBold, FontSize = 24, ForeColor = Color.FromArgb((int)alpha, Color.Black) },
                    new RectangleF(rMask.X, rMask.Y, rMask.Width, rMask.Height),
                    TextAlignment.Leading, ParagraphAlignment.Center, false);
                g.SoftMask = softMask;
                g.DrawImage(Image.FromFile(Path.Combine("Resources", "Images", "reds.jpg")), rDoc, null, ImageAlign.StretchImage);
                g.SoftMask = SoftMaskBase.None;
                rDoc.Offset(0, rDoc.Height);
            }

            // Done:
            doc.Save(stream);
            return doc.Pages.Count;
        }
    }
}
