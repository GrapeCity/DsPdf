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
            // Render a string into the page:
            g.DrawString("Hello, World!",
                // Use a standard font (the 14 standard PDF fonts are built into GcPdf
                // and are always available):
                new TextFormat() { Font = StandardFonts.Times, FontSize = 12 },
                // GcPdf page coordinates start at top left corner, using 72 dpi by default:
                new PointF(72, 72));
            // Save the PDF:
            doc.Save(stream);
        }
    }
}
