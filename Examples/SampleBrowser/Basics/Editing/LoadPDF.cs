using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples
{
    // This sample loads the PDF file created by the HelloWorld sample,
    // adds a short text note to the first page, and saves the result.
    public class LoadPDF
    {
        public void CreatePDF(Stream stream)
        {
            GcPdfDocument doc = new GcPdfDocument();
            // IMPORTANT: when working with an existing PDF file using GcPdfDocument.Load() method,
            // the stream passed to that method MUST REMAIN OPEN while working with the document.
            // This is because Load() does not load the whole PDF document into memory right away,
            // instead it loads the various parts of the PDF as needed.
            // The stream is only used for reading, and the original file itself is not modified.
            // To save the changes you need to call one of the GcPdfDocument.Save() overloads
            // as usual.
            using (var fs = new FileStream(Path.Combine("Resources", "PDFs", "HelloWorld.pdf"), FileMode.Open, FileAccess.Read))
            {
                doc.Load(fs);
                // Add note to the (only) page in the doc:
                var page = doc.Pages.Last;
                Common.Util.AddNote(
                    "This text was added to the original \"Hello World\" PDF",
                    page,
                    new RectangleF(72, 72 * 3, page.Size.Width - 72 * 2, 72));
                // Done:
                doc.Save(stream);
            }
        }
    }
}
