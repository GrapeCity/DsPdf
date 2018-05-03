using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;
using GcPdfWeb.Samples.Common;

namespace GcPdfWeb.Samples
{
    // Creates pages of 'slides' from all images found in a directory.
    //
    // IMPORTANT NOTE: When you render an image in GcPdf multiple times (e.g. rendering
    // the same image as part of a page header on all pages), it will automatically be
    // added to a dictionary and reused throughout the document, provided you use
    // the same image object on all pages. So rather than loading the same image from
    // file (or stream) each time it is needed, it is always preferable to load the image
    // once and cache it in an image object. This applies to all image types available in
    // GcPdf (Image, RawImage, ImageWrapper).
    public class SlidePages
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            // Get a font for captions:
            var font = Font.FromFile(Path.Combine("Resources", "Fonts", "segoeui.ttf"));
            // GcPdfDocument.ImageOptions allow to control various image-related settings.
            // In particular, we can lower the JPEG quality from the default 75% to reduce the file size:
            doc.ImageOptions.JpegQuality = 50;

            // Load all images from the Resources/Images folder:
            List<Tuple<string, Image>> images = new List<Tuple<string, Image>>();
            foreach (var fname in Directory.GetFiles(Path.Combine("Resources", "Images"), "*", SearchOption.AllDirectories))
                images.Add(Tuple.Create<string, Image>(Path.GetFileName(fname), Util.ImageFromFile(fname)));
            images.Shuffle();
            // Print all images as slide sheets in a 3x4 grid with 1/2" margins all around:
            const float margin = 36;
            const int rows = 4;
            const int cols = 3;
            float gapx = 72f / 4, gapy = gapx;
            float sWidth = (doc.PageSize.Width - margin * 2 + gapx) / cols;
            float sHeight = (doc.PageSize.Height - margin * 2 + gapy) / rows;
            if (sWidth > sHeight)
            {
                gapx += sWidth - sHeight;
                sWidth = sHeight;
            }
            else
            {
                gapy += sHeight - sWidth;
                sHeight = sWidth;
            }
            const float sMargin = 72f / 6;
            // Set up image alignment that would center images within the specified area:
            ImageAlign ia = new ImageAlign(ImageAlignHorz.Center, ImageAlignVert.Center, true, true, true, false, false);
            // Text format for image captions:
            TextFormat tf = new TextFormat() { Font = font, FontSize = sMargin * 0.65f };
            // Insertion point:
            PointF ip = new PointF(margin, margin);
            var g = doc.NewPage().Graphics;
            for (int i = 0; i < images.Count(); ++i)
            {
                var rect = new RectangleF(ip, new SizeF(sWidth - gapx, sHeight - gapy));
                g.FillRectangle(rect, Color.LightGray);
                g.DrawRectangle(rect, Color.Black, 0.5f);
                rect.Inflate(-sMargin, -sMargin);
                // We get the actual rectangle where the image was drawn from the DrawImage method
                // (via an output parameter) so that we can draw a thin border exactly around the image
                // (an array is required as the image can be tiled, in which case multiple rectangles
                // will be returned):
                g.DrawImage(images[i].Item2, rect, null, ia, out RectangleF[] imageRect);
                g.DrawRectangle(imageRect[0], Color.DarkGray, 1);
                // print image file name as caption in the bottom slide margin:
                g.DrawString(Path.GetFileName(images[i].Item1), tf, 
                    new RectangleF(rect.X, rect.Bottom, rect.Width, sMargin),
                    TextAlignment.Center, ParagraphAlignment.Near, false);
                ip.X += sWidth;
                if (ip.X + sWidth > doc.PageSize.Width && i < images.Count() - 1)
                {
                    ip.X = margin;
                    ip.Y += sHeight;
                    if (ip.Y + sHeight > doc.PageSize.Height)
                    {
                        g = doc.NewPage().Graphics;
                        ip.Y = margin;
                    }
                }
            }
            // Done:
            doc.Save(stream);
        }
    }
}
