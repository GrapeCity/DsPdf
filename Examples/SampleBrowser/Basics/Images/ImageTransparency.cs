using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;

namespace GcPdfWeb.Samples
{
    // This sample demonstrates the ability of GcPdf to render
    // images using a specified transparency (opacity).
#if !DIODOCS_V1
#endif
    public class ImageTransparency
    {
        public int CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;

            var rc = Common.Util.AddNote(
                "GcPdfGraphics.DrawImage() method allows to render images with a specified opacity. " +
                "Below is a random text with an image drawn on top of it using opacity 0.2 (almost transparent), " +
                "0.5 (medium transparency) and 1 (non-transparent).",
                page);

            var tl = new TextLayout();
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 12;
            tl.MaxWidth = page.Size.Width;
            tl.MaxHeight = page.Size.Height;
            tl.MarginAll = 36;
            tl.MarginTop += rc.Bottom;
            tl.Append(Common.Util.LoremIpsum(2));
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, PointF.Empty);

            using (var image = Image.FromFile(Path.Combine("Resources", "Images", "puffins.jpg")))
            {
                var imageRc = new RectangleF(tl.MarginLeft, tl.MarginTop, 144, 144);
                // Opacity 0.2:
                g.DrawImage(image, imageRc, null, ImageAlign.ScaleImage, 0.2f);
                imageRc.Offset(imageRc.Width + 36, 0);
                // Opacity 0.5:
                g.DrawImage(image, imageRc, null, ImageAlign.ScaleImage, 0.5f);
                imageRc.Offset(imageRc.Width + 36, 0);
                // Opacity 1 (default):
                g.DrawImage(image, imageRc, null, ImageAlign.ScaleImage);

                // NOTE: we must save document BEFORE disposing the image(s) used in it:
                doc.Save(stream);
            }
            return doc.Pages.Count;
        }
    }
}
