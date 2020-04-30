//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
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
    // Essentially the same code, but without using StartDoc/EndDoc, is demonstrated by the
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
            var tl = new TextLayout(72)
            {
                MaxWidth = doc.PageSize.Width,
                MaxHeight = doc.PageSize.Height,
                MarginAll = 72,
            };
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 12;
            // Start with a title page:
            tl.FirstLineIndent = 0;
            var fnt = Font.FromFile(Path.Combine("Resources", "Fonts", "yumin.ttf"));
            var tf0 = new TextFormat() { FontSize = 24, FontBold = true, Font = fnt };
            tl.Append(string.Format("Large Document\n{0} Pages of Lorem Ipsum\n\n", N), tf0);
            var tf1 = new TextFormat(tf0) { FontSize = 14, FontItalic = true };
            tl.Append(string.Format("Generated on {0}.", DateTime.Now), tf1);
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
