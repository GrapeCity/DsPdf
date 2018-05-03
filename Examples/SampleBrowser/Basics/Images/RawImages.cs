using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;

namespace GcPdfWeb.Samples
{
    // Using JPEG and JPEG2000 images in GcPdf.
    //
    // IMPORTANT NOTE: When you render an image in GcPdf multiple times (e.g. rendering
    // the same image as part of a page header on all pages), it will automatically be
    // added to a dictionary and reused throughout the document, provided you use
    // the same image object on all pages. So rather than loading the same image from
    // file (or stream) each time it is needed, it is always preferable to load the image
    // once and cache it in an image object. This applies to all image types available in
    // GcPdf (Image, RawImage, ImageWrapper).
    public class RawImages
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            // PDF format allows to insert JPEG and JPEG2000 images into the document 'as is',
            // without converting to PDF native image formats. To do that in GcPdf,
            // the RawImage class can be used, as the code below demonstrates.
            //
            // Create instances of RawImage from JPEG files:
            var image = RawImage.FromFile(Path.Combine("Resources", "Images", "puffins.jpg"));
            var imageSmall = RawImage.FromFile(Path.Combine("Resources", "ImagesBis", "puffins-small.jpg"));
            // Text format used to draw captions:
            TextFormat tf = new TextFormat() { Font = StandardFonts.Times, FontSize = 12 };

            // Action to draw the image using various options:
            Action<Page, ImageAlign, bool> drawImage = (page_, ia_, clip_) =>
            {
                var rect = new RectangleF(72, 72, 72 * 4, 72 * 4);
                var clipped = clip_ ? "clipped to a 4\"x4\" rectangle" : "without clipping";
                var align = ia_ == ImageAlign.Default ? "ImageAlign.Default" :
                    ia_ == ImageAlign.CenterImage ? "ImageAlign.CenterImage" :
                    ia_ == ImageAlign.StretchImage ? "ImageAlign.StretchImage" : "custom ImageAlign";
                // Draw image caption:
                page_.Graphics.DrawString($"Page {doc.Pages.IndexOf(page_) + 1}: Image drawn at (1\",1\"), {clipped}, using {align}:", tf, new PointF(72, 36));
                var clip = clip_ ? new Nullable<RectangleF>(rect) : new Nullable<RectangleF>();
                // Draw the image:
                page_.Graphics.DrawImage(image, rect, clip, ia_, out RectangleF[] imageRects);
                // Show the image outline:
                page_.Graphics.DrawRectangle(imageRects[0], Color.Red, 1, null);
                // Show image/clip area:
                page_.Graphics.DrawRectangle(rect, Color.Blue, 1, null);
            };

            // The ImageAlign class provides various image alignment options.
            // It also defines a few static instances with some commonly used
            // combinations of options demonstrated below.

            // Page 1: draw image without clipping, with default alignment:
            drawImage(doc.NewPage(), ImageAlign.Default, false);

            // Page 2: draw image with clipping, with default alignment:
            drawImage(doc.NewPage(), ImageAlign.Default, true);

            // Page 3: draw image with clipping, with CenterImage alignment:
            drawImage(doc.NewPage(), ImageAlign.CenterImage, true);

            // Page 4: draw image without clipping and stretched image:
            drawImage(doc.NewPage(), ImageAlign.StretchImage, false);

            // Page 5: draw image without clipping, fit into the rectangle, preserving aspect ratio:
            ImageAlign ia = new ImageAlign(ImageAlignHorz.Center, ImageAlignVert.Center, true, true, true, false, false);
            drawImage(doc.NewPage(), ia, false);

            // Page 6: draw a small image tiled, without clipping, fit into the rectangle, preserving aspect ratio:
            image = imageSmall;
            ia = new ImageAlign(ImageAlignHorz.Left, ImageAlignVert.Top, false, false, true, true, true);
            drawImage(doc.NewPage(), ia, false);

            // Done:
            doc.Save(stream);
        }
    }
}
