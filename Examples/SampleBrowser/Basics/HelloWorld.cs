using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples
{
    // One of the simplest ways to create a "Hello, World!" PDF with GcPdf.
    public class HelloWorld
    {
        public void CreatePDF(Stream stream)
        {
            // Create a new PDF document:
            GcPdfDocument doc = new GcPdfDocument();
            // Add a page, get its graphics:
            GcPdfGraphics g = doc.NewPage().Graphics;
            // Draw a string on the page.
            // Notes:
            // - For simplicity, here we are using a standard PDF font
            //   (the 14 standard fonts' metrics are built into GcPdf and are always available);
            // - GcPdf coordinates start at top left corner of a page, using 72 dpi by default:
            g.DrawString("Hello, World!",
                new TextFormat() { Font = StandardFonts.Times, FontSize = 12 },
                new PointF(72, 72));
            // Save the PDF:
            doc.Save(stream);
        }
    }
}
