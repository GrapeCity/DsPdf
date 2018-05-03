using System;
using System.IO;
using System.Drawing;
using System.Linq;
using GrapeCity.Documents.Common;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples
{
    // This sample loads the PDF file created by the Transforms sample,
    // finds all occurrences of a string in the loaded document,
    // and highlights these occurrences. Two points of interest about this sample:
    // - The texts in the original document are graphically transformed,
    //   but the quadrilaterals supplied by the FindText method allows to easily
    //   highlight the finds even in that case.
    // - The sample inserts a new content stream at index 0 of the page,
    //   this ensures that the highlighting is drawn UNDER the original conent.
    //   (The same approach may be used to add watermarks etc. to existing files.)
    public class FindTransformed
    {
        public void CreatePDF(Stream stream)
        {
            GcPdfDocument doc = new GcPdfDocument();

            // The original file stream must be kept open while working with the loaded PDF, see LoadPDF for details:
            using (var fs = new FileStream(Path.Combine("Resources", "PDFs", "Transforms.pdf"), FileMode.Open, FileAccess.Read))
            {
                doc.Load(fs);
                // Find all 'Text drawn at', using case-sensitive search:
                var finds = doc.FindText(
                    new FindTextParams("Text drawn at", false, true),
                    OutputRange.All);

                // Highlight all finds: first, find all pages where the text was found
                var pgIndices = finds.Select(f_ => f_.PageIndex).Distinct();
                // Loop through pages, on each page insert a new content stream at index 0,
                // so that our highlights go UNDER the original content:
                foreach (int pgIdx in pgIndices)
                {
                    var page = doc.Pages[pgIdx];
                    PageContentStream pcs = page.ContentStreams.Insert(0);
                    var g = pcs.GetGraphics(page);
                    foreach (var find in finds.Where(f_ => f_.PageIndex == pgIdx))
                    {
                        // Note the solid color used to fill the polygon:
                        g.FillPolygon(find.Bounds, Color.CadetBlue);
                        g.DrawPolygon(find.Bounds, Color.Blue);
                    }
                }
                // Done:
                doc.Save(stream);
            }
        }
    }
}
