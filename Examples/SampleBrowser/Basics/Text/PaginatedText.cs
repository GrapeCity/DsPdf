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
    // This sample shows how to render a long text spanning multiple pages.
    public class PaginatedText
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            // Use TextLayout to render text:
            var tl = new TextLayout(72);
            // If not specifying formats for individual runs, we MUST provide
            // font and font size on TextLayout.DefaultFormat:
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 12;
            // First line offset 1/2":
            tl.FirstLineIndent = 72 / 2;
            //
            // All other formatting properties are left at their default values.
            // In particular, TextLayout's default resolution is 72 dpi - 
            // the same as GcPdf's, and WordWrap is true.
            //
            // Set TextLayout's area to the whole page:
            tl.MaxWidth = doc.PageSize.Width;
            tl.MaxHeight = doc.PageSize.Height;
            // ...and have it manage the page margins (1" all around):
            tl.MarginAll = tl.Resolution;
            //
            // Append the text (20 paragraphs so they would not fit on a single page)
            // (note that TextLayout interprets "\r\n" as paragraph delimiter):
            tl.Append(Common.Util.LoremIpsum(20));
            //
            // When all text has been added, we must calculate the glyphs needed to render the text,
            // and perform the layout. This can be done by a single call to PerformLayout, passing true to
            // recalculate glyphs first (even though the text won't all fit in the specified max size,
            // we only need to call PerformLayout once):
            tl.PerformLayout(true);
            // Use split options to provide widow/orphan control:
            TextSplitOptions to = new TextSplitOptions(tl);
            to.MinLinesInFirstParagraph = 2;
            to.MinLinesInLastParagraph = 2;
            // In a loop, split and render the text:
            while (true)
            {
                // 'rest' will accept the text that did not fit:
                var splitResult = tl.Split(to, out TextLayout rest);
                doc.Pages.Add().Graphics.DrawTextLayout(tl, PointF.Empty);
                if (splitResult != SplitResult.Split)
                    break;
                tl = rest;
            }
            // Done:
            doc.Save(stream);
        }
    }
}
