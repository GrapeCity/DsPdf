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
    // This sample shows how to add page labels to a document.
    // Page labels allow to subdivide the document into sequences of
    // logically related page ranges (e.g. preface, main body, postface).
    // In this sample consisting of 'chapters', we add a separate
    // page labeling range for each chapter.
    // The code in this sample is similar to the Outlines sample.
    public class PageLabels
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            // Text layout for main text (default GcPdf resolution is 72dpi):
            var tl = new TextLayout(72);
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 12;
            tl.FirstLineIndent = 72 / 2;
            tl.MaxWidth = doc.PageSize.Width;
            tl.MaxHeight = doc.PageSize.Height;
            tl.MarginAll = tl.Resolution;
            // Text layout for chapter headers:
            var tlCaption = new TextLayout(72);
            tlCaption.DefaultFormat.Font = StandardFonts.TimesBold;
            tlCaption.DefaultFormat.FontSize = tl.DefaultFormat.FontSize + 4;
            tlCaption.DefaultFormat.Underline = true;
            tlCaption.MaxWidth = tl.MaxWidth;
            tlCaption.MarginAll = tlCaption.Resolution;
            // Split options to control splitting of text between pages:
            TextSplitOptions to = new TextSplitOptions(tl)
            {
                RestMarginTop = tl.Resolution,
                MinLinesInFirstParagraph = 2,
                MinLinesInLastParagraph = 2
            };
            // Generate a number of "chapters", provide outline entry for each:
            const int NChapters = 20;
            for (int i = 0; i < NChapters; ++i)
            {
                // Chapter title - print as chapter header and add as outline node:
                string chapter = $"Chapter {i + 1}";

                // All it takes to add page lables is to add a PageLabelingRange
                // associated with the index of the first page in the range,
                // and the range prefix and numbering style:
                doc.PageLabelingRanges.Add(doc.Pages.Count, new PageLabelingRange($"{chapter}, p. ", NumberingStyle.DecimalArabic, 1));

                doc.Pages.Add();
                tlCaption.Clear();
                tlCaption.Append(chapter);
                tlCaption.PerformLayout(true);
                // Add outline node for the chapter:
                doc.Outlines.Add(new OutlineNode(chapter, new DestinationFitH(doc.Pages.Count - 1, tlCaption.MarginTop)));
                // Print the caption:
                doc.Pages.Last.Graphics.DrawTextLayout(tlCaption, PointF.Empty);
                // Chapter text:
                tl.Clear();
                tl.FirstLineIsStartOfParagraph = true;
                tl.LastLineIsEndOfParagraph = true;
                tl.Append(Common.Util.LoremIpsum(7));
                // Account for chapter header in the main text layout:
                tl.MarginTop = tlCaption.ContentRectangle.Bottom + 12;
                tl.PerformLayout(true);
                // Print the chapter:
                while (true)
                {
                    // 'rest' will accept the text that did not fit:
                    var splitResult = tl.Split(to, out TextLayout rest);
                    doc.Pages.Last.Graphics.DrawTextLayout(tl, PointF.Empty);
                    if (splitResult != SplitResult.Split)
                        break;
                    tl = rest;
                    var p = doc.Pages.Add();
                }
            }
            // Done:
            doc.Save(stream);
        }
    }
}
