using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;

namespace GcPdfWeb.Samples.Basics
{
    // This sample shows how to insert arbitrary objecs (images in this sample)
    // into a block of text so that those objects keep their positions relative
    // to the surrounding text, and are laid out exactly like other text runs,
    // and can participate in text flow.
    public class InlineImages
    {
        public int CreatePDF(Stream stream)
        {
            // Get the images to use as inline objects:
            using (var imgPuffins = Image.FromFile("Resources/ImagesBis/puffins-small.jpg"))
            using (var imgFerns = Image.FromFile("Resources/ImagesBis/ferns-small.jpg"))
            {
                // The image alignment to use:
                var ia = new ImageAlign(ImageAlignHorz.Center, ImageAlignVert.Bottom, true, true, true, false, false);
                // Create and set up the document:
                var doc = new GcPdfDocument();
                var page = doc.NewPage();
                var g = page.Graphics;
                // Create and set up a TextLayout object to print the text:
                var tl = g.CreateTextLayout();
                tl.MaxWidth = page.Size.Width;
                tl.MaxHeight = page.Size.Height;
                tl.MarginLeft = tl.MarginRight = tl.MarginTop = tl.MarginBottom = 36;
                tl.DefaultFormat.Font = StandardFonts.Times;
                tl.DefaultFormat.FontSize = 12;
                tl.DefaultFormat.BackColor = Color.LightGoldenrodYellow;
                tl.TextAlignment = TextAlignment.Justified;
                // Create inline objects using the images and arbitrary sizes:
                var ioPuffins = new InlineObject(imgPuffins, 36, 24);
                var ioFerns = new InlineObject(imgFerns, 36, 24);
                // Build up the text:
                tl.Append(
                    "The 'Inline objects' feature of the TextLayout class allows to insert arbitrary objects " +
                    "into a block of text. Those objects are then treated exactly like other text runs, " +
                    "and keep their positions relative to the surrounding text. " +
                    "In this sample, we insert some images into the text as inline objects, " +
                    "use the TextLayout class to position them along with text, and draw them " +
                    "using the GcGraphics.DrawImage method. "
                    );
                tl.Append("Here are some puffins: ");
                tl.Append(ioPuffins);
                tl.Append(" and here are some ferns: ");
                tl.Append(ioFerns);
                tl.Append(" The end.");
                //
                System.Diagnostics.Debug.Assert(tl.InlineObjects.Count == 0, "InlineObjects is filled by RecalculateGlyphs");
                // This method fetches and measures the glyphs needed to render the text,
                // because we draw the same text a few times with different layout,
                // we call this once before the loop below:
                tl.RecalculateGlyphs();
                //
                System.Diagnostics.Debug.Assert(tl.InlineObjects.Count == 2, "InlineObjects is filled by RecalculateGlyphs");
                // In a loop, draw the text and inline images in 3 different locations
                // and bounds on the page:
                for (int i = 0; i < 3; ++i)
                {
                    tl.MarginTop = tl.ContentRectangle.Bottom + 36;
                    tl.MarginLeft = 36 + 72 * i;
                    tl.MarginRight = 36 + 72 * i;
                    // Note passing 'false' here, we do not need to recalc the glyphs because
                    // the text has not changed:
                    tl.PerformLayout(false);
                    // Draw the text and images:
                    g.DrawTextLayout(tl, PointF.Empty);
                    foreach (var io in tl.InlineObjects)
                        g.DrawImage((Image)io.Object, io.ObjectRect.ToRectangleF(), null, ia);
                }
                // Done:
                doc.Save(stream);
                return doc.Pages.Count;
            }
        }
    }
}
