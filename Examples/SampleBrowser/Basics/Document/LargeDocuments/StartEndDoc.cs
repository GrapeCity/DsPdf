using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // Shows how to create a large document using less memory.
    //
    // GcPdf provides two approaches to creating a PDF file:
    // - The usually more convenient approach: you build the document completely first,
    //   adding text, graphics and other elements. Then you call Save() on the document
    //   passing the name of the file, or the stream to save to. This approach allows
    //   to modify the already created content - e.g. you can insert pages anywhere
    //   in the document, or modify the already added pages.
    // - The StartDoc/EndDoc method: with this approach, you provide the stream
    //   to save to at the very beginning, before adding any content to the document,
    //   by calling the StartDoc() method on the document. All content is then written
    //   directly to that stream, and you cannot go back and update the already created pages.
    //   To complete the document you call the EndDoc() method. If you try to perform an
    //   action that is not allowed, an exception will be thrown. While this approach is
    //   somewhat limiting (e.g. Linearized cannot be set to true in this mode), it uses
    //   less memory and may be preferable especially when creating very large documents.
    //
    // This sample demonstrates the StartDoc/EndDoc approach.
    //
    // Essentially the same code, but without use of StartDoc/EndDoc, is demonstrated by the
    // LargeDocument2 sample. See also LinearizedPdf.
    public class StartEndDoc
    {
        public int CreatePDF(Stream stream)
        {
            // Number of pages to generate:
            const int N = Common.Util.LargeDocumentIterations;
            var doc = new GcPdfDocument();
            // Start creating the document by this call:
            doc.StartDoc(stream);
            // Prep a TextLayout to hold/format the text:
            var tl = new TextLayout()
            {
                MaxWidth = doc.PageSize.Width,
                MaxHeight = doc.PageSize.Height,
                MarginLeft = 72,
                MarginRight = 72,
                MarginTop = 72,
                MarginBottom = 72,
            };
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 12;
            // Start with a title page:
            tl.FirstLineIndent = 0;
            tl.Append($"Large Document\n{N} Pages of Lorem Ipsum\n\n", new TextFormat() { FontSize = 24, FontBold = true });
            tl.Append($"Generated on {DateTime.Now}.", new TextFormat() { FontSize = 14, FontItalic = true });
            tl.TextAlignment = TextAlignment.Center;
            tl.PerformLayout(true);
            doc.Pages.Add().Graphics.DrawTextLayout(tl, PointF.Empty);
            tl.Clear();
            tl.FirstLineIndent = 36;
            tl.TextAlignment = TextAlignment.Leading;
            // Generate the document:
            for (int pageIdx = 0; pageIdx < N; ++pageIdx)
            {
                tl.Append(Common.Util.LoremIpsum(1));
                tl.PerformLayout(true);
                doc.NewPage().Graphics.DrawTextLayout(tl, PointF.Empty);
                tl.Clear();
            }
            // NOTE: Certain operations (e.g. the one below) will throw an error when using StartDoc/EndDoc:
            //   doc.Pages.Insert(0);
            //
            // Done - call EndDoc instead of Save():
            doc.EndDoc();
            return doc.Pages.Count;
        }
    }
}
