//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;

namespace GcPdfWeb.Samples.Basics
{
    // Demonstrates a simple way to generate left/centered/right aligned page headers and footers.
    public class PageHeaders
    {
        // The document being generated:
        private GcPdfDocument _doc;

        // Utility method to draw a part of a page header or footer.
        // Parameters:
        // - text: The part's text.
        // - tf: The text format to use.
        // - pageIdx: The page index.
        // - header: True if this is a header, false if a footer.
        // - horzAlign: Horizontal alignment (left/center/right).
        private void RenderHeader(string text, TextFormat tf, int pageIdx, bool header, TextAlignment horzAlign)
        {
            var page = _doc.Pages[pageIdx];
            TextLayout tl = new TextLayout(page.Graphics.Resolution);
            tl.MaxWidth = page.Size.Width;
            tl.MaxHeight = page.Size.Height;
            // 1" margins, adjust as needed:
            tl.MarginLeft = tl.MarginRight = 72;
            // 1/3" spacing above top/below bottom header, adjust as needed:
            tl.MarginTop = tl.MarginBottom = 72 / 3;
            // Vertical alignment:
            tl.ParagraphAlignment = header ? ParagraphAlignment.Near : ParagraphAlignment.Far;
            // Horizontal alignment:
            tl.TextAlignment = horzAlign;
            tl.Append(text, tf);
            // NOTE: if some part of a header or footer is static, we could cache the corresponding TextLayout
            // object and save some cycles by just drawing that cached TextLayout on each page w/out anything else:
            tl.PerformLayout(true);
            // Draw the header at (0,0) (header located by margins and alignment):
            page.Graphics.DrawTextLayout(tl, PointF.Empty);
        }

        // The main program.
        public void CreatePDF(Stream stream)
        {
            _doc = new GcPdfDocument();
            var page = _doc.NewPage();
            // Add a note about flipping landscape:
            var noteRect = Common.Util.AddNote(
                "We flip page orientation in this sample only to show that these page headers can adapt to the changing page size.",
                page);
            // Prepare a TextLayout with some long text and print it (see PaginatedText for details):
            var tl = page.Graphics.CreateTextLayout();
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 12;
            tl.MaxWidth = _doc.PageSize.Width;
            tl.MaxHeight = _doc.PageSize.Height;
            tl.MarginAll = tl.Resolution;
            tl.MarginTop = noteRect.Bottom + 18;
            // Add sample text:
            tl.Append(Common.Util.LoremIpsum(20));
            // Calculate glyphs and perform layout (see also PerformLayout call in the loop below):
            tl.PerformLayout(true);
            // In a loop, split and render the text:
            while (true)
            {
                var splitResult = tl.Split(null, out TextLayout rest);
                page.Graphics.DrawTextLayout(tl, PointF.Empty);
                if (splitResult != SplitResult.Split)
                    break;
                tl = rest;
                tl.MarginTop = tl.Resolution;
                page = _doc.Pages.Add();
                // For sample sake, toggle page orientation:
                page.Landscape = !_doc.Pages[_doc.Pages.Count - 2].Landscape;
                // Update layout size to reflect the new page orientation:
                tl.MaxWidth = page.Size.Width;
                tl.MaxHeight = page.Size.Height;
                // Because we changed layout size, we must perform layout again -
                // but can do it without recalculating glyphs:
                tl.PerformLayout(false);
            }
            // Render the headers in a separate loop (so that we can provide 'Page X of Y' header):
            TextFormat tf = new TextFormat() { Font = StandardFonts.Helvetica, FontSize = 10, ForeColor = Color.Gray };
            var now = DateTime.Now.ToString();
            for (int pageIdx = 0; pageIdx < _doc.Pages.Count; ++pageIdx)
            {
                RenderHeader(now, tf, pageIdx, true, TextAlignment.Leading);
                RenderHeader("Easy Page Headers Sample", tf, pageIdx, true, TextAlignment.Center);
                RenderHeader($"Page {pageIdx + 1} of {_doc.Pages.Count}", tf, pageIdx, true, TextAlignment.Trailing);
                RenderHeader("Page footer - left", tf, pageIdx, false, TextAlignment.Leading);
                RenderHeader("GcPdf", tf, pageIdx, false, TextAlignment.Center);
                RenderHeader("Page footer - right", tf, pageIdx, false, TextAlignment.Trailing);
            }
            // Done:
            _doc.Save(stream);
        }
    }
}
