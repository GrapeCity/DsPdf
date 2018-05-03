using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;

namespace GcPdfWeb.Samples.Basics
{
    // Shows how to add ouline entries to a document.
    // See also PaginatedText.
    public class Outlines
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
            tl.MarginLeft = tl.MarginTop = tl.MarginRight = tl.MarginBottom = tl.Resolution;
            // Text layout for chapter headers:
            var tlCaption = new TextLayout(72);
            tlCaption.DefaultFormat.Font = StandardFonts.TimesBold;
            tlCaption.DefaultFormat.FontSize = tl.DefaultFormat.FontSize + 4;
            tlCaption.DefaultFormat.Underline = true;
            tlCaption.MaxWidth = tl.MaxWidth;
            tlCaption.MarginLeft = tlCaption.MarginTop = tlCaption.MarginRight = tlCaption.MarginBottom = tlCaption.Resolution;
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
                doc.Pages.Add();
                // Chapter title - print as chapter header and add as outline node:
                string chapter = $"Chapter {i + 1}";
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
                    TextLayout rest;
                    var splitResult = tl.Split(to, out rest);
                    doc.Pages.Last.Graphics.DrawTextLayout(tl, PointF.Empty);
                    if (splitResult != SplitResult.Split)
                        break;
                    tl = rest;
                    doc.Pages.Add();
                }
            }
            // Done:
            doc.Save(stream);
        }
    }
}
