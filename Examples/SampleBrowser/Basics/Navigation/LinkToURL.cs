using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Pdf.Annotations;
using GrapeCity.Documents.Pdf.Actions;

namespace GcPdfWeb.Samples
{
    // A simple way to create a link to an external URL,
    // and associate it with a text on a page.
    public class LinkToURL
    {
        public void CreatePDF(Stream stream)
        {
            GcPdfDocument doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;

            // Draw some text that will represent the link:
            var tf = new TextFormat() { Font = StandardFonts.Times, FontSize = 14 };
            var tl = new TextLayout();
            tl.MarginLeft = tl.MarginTop = tl.MarginRight = tl.MarginBottom = 72;
            tl.Append("Google google on the wall, please tell me all!", tf);
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, PointF.Empty);

            // Add a link associated with the text area:
            page.Annotations.Add(new LinkAnnotation(tl.ContentRectangle, new ActionURI("http://www.google.com")));

            // Done:
            doc.Save(stream);
        }
    }
}
