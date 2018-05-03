using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Common;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples
{
    // This sample loads the PDF file created by the BalancedColumns sample,
    // finds all occurrences of the words 'lorem' and 'ipsum' in the loaded document,
    // and highlights these two words using different colors.
    public class FindText
    {
        public void CreatePDF(Stream stream)
        {
            GcPdfDocument doc = new GcPdfDocument();

            // The original file stream must be kept open while working with the loaded PDF, see LoadPDF for details:
            using (var fs = new FileStream(Path.Combine("Resources", "PDFs", "BalancedColumns.pdf"), FileMode.Open, FileAccess.Read))
            {
                doc.Load(fs);
                // Find all 'lorem', using case-insensitive word search:
                var findsLorem = doc.FindText(
                    new FindTextParams("lorem", true, false),
                    OutputRange.All);
                // Ditto for 'ipsum':
                var findsIpsum = doc.FindText(
                    new FindTextParams("ipsum", true, false),
                    OutputRange.All);

                // Highlight all 'lorem' using semi-transparent orange red:
                foreach (var find in findsLorem)
                    doc.Pages[find.PageIndex].Graphics.FillPolygon(find.Bounds, Color.FromArgb(100, Color.OrangeRed));
                // Put a violet red border around all 'ipsum':
                foreach (var find in findsIpsum)
                    doc.Pages[find.PageIndex].Graphics.DrawPolygon(find.Bounds, Color.MediumVioletRed);

                // Done:
                doc.Save(stream);
            }
        }
    }
}
