using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // Shows how to create a linearized PDF file.
    // Note that while the code below was used to generate the PDF shown in the sample browser,
    // the browser sends a static copy of this file, so that the web server can send it
    // in smaller chunks (all other sample PDFs are generated on the fly).
    public class LinearizedPdf
    {
        public void CreatePDF(Stream stream)
        {
            // Number of pages to generate:
            const int N = 5000;
            var doc = new GcPdfDocument();
            // To create a linearized PDF, the only thing we need to do is raise a flag:
            doc.Linearized = true;
            // Prep a TextLayout to hold/format the text:
            var page = doc.NewPage();
            var tl = page.Graphics.CreateTextLayout();
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 12;
            // Use TextLayout to layout the whole page including margins:
            tl.MaxHeight = page.Size.Height;
            tl.MaxWidth = page.Size.Width;
            tl.MarginLeft = tl.MarginTop = tl.MarginRight = tl.MarginBottom = 72;
            tl.FirstLineIndent = 72 / 2;
            // Generate the document:
            for (int pageIdx = 0; pageIdx < N; ++pageIdx)
            {
                // Note: for the sake of this sample, we do not care if a sample text does not fit on a page.
                tl.Append(Common.Util.LoremIpsum(2));
                tl.PerformLayout(true);
                doc.Pages.Last.Graphics.DrawTextLayout(tl, PointF.Empty);
                if (pageIdx < N - 1)
                {
                    doc.Pages.Add();
                    tl.Clear();
                }
            }
            // Done:
            doc.Save(stream);
        }
    }
}
