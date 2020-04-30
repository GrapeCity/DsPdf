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
    // This sample shows how to prevent a page break between a paragraph
    // and the next one when splitting a TextLayout.
    // Splitting of text in this sample is similar to that in PaginatedText,
    // see comments in PaginatedText for more info on text handling.
    public class KeepWithNext
    {
        public int CreatePDF(Stream stream)
        {
            const int NPAR = 40;
            var doc = new GcPdfDocument();
            var tl = new TextLayout(72)
            {
                FirstLineIndent = 72 / 2,
                MaxWidth = doc.PageSize.Width,
                MaxHeight = doc.PageSize.Height,
                MarginAll = 72,

            };
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 12;
            // Text format for paragraphs kept together with next one:
            var tf = new TextFormat(tl.DefaultFormat)
            {
                FontSize = tl.DefaultFormat.FontSize + 2,
                FontBold = true
            };
            // We add a number of random 'lorem ipsum' paragraphs to this document,
            // adding a 'caption' before each paragraph which is kept together
            // with the following 'lorem ipsum' paragraph:
            for (int i = 0; i < NPAR; i++)
            {
                // 'Caption' kept together with the next paragraph:
                tl.Append("Caption kept together with the next paragraph. No page break after this.", tf);
                // AppendParagraphBreak adds a paragraph break but prevents a page break between the two paragraphs:
                tl.AppendParagraphBreak();
                // Random paragraph after 'caption':
                tl.Append(Common.Util.LoremIpsum(1));
            }
            tl.PerformLayout(true);
            // We force all paragraph lines to stay on the same page,
            // this makes it more obvious that 'caption' and following paragraph
            // are kept on the same page:
            TextSplitOptions to = new TextSplitOptions(tl)
            {
                KeepParagraphLinesTogether = true,
            };
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
            return doc.Pages.Count;
        }
    }
}
