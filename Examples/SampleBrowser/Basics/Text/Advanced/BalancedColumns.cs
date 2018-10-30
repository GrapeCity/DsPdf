using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // Creates a multi-column text layout with balanced columns.
    // The heart of this sample is the TextLayout.SplitAndBalance() method
    // which allows to split a text between multiple columns,
    // AND balance those columns so that their heights are similar,
    // thus allowing to produce magazine- and newsparer-like text layouts.
    public class BalancedColumns
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var font = StandardFonts.Times;
            var fontSize = 12;
            // 1/2" margins all around (72 dpi is the default resolution used by GcPdf):
            var margin = 72 / 2;
            var pageWidth = doc.PageSize.Width;
            var pageHeight = doc.PageSize.Height;
            var cW = pageWidth - margin * 2;
            // Text format for the chapter titles:
            var tlCaption = new TextLayout();
            tlCaption.DefaultFormat.Font = font;
            tlCaption.DefaultFormat.FontSize = fontSize + 4;
            tlCaption.DefaultFormat.Underline = true;
            tlCaption.MaxWidth = pageWidth;
            tlCaption.MaxHeight = pageHeight;
            tlCaption.MarginLeft = tlCaption.MarginTop = tlCaption.MarginRight = tlCaption.MarginBottom = margin;
            tlCaption.TextAlignment = TextAlignment.Center;
            // Height of chapter caption (use a const for simplicity):
            const float captionH = 24;
            // Text layout for main document body (default GcPdf resolution is 72dpi):
            var tl = new TextLayout();
            tl.DefaultFormat.Font = font;
            tl.DefaultFormat.FontSize = fontSize;
            tl.FirstLineIndent = 72 / 2;
            tl.MaxWidth = pageWidth;
            tl.MaxHeight = pageHeight;
            tl.MarginLeft = tl.MarginRight = tl.MarginBottom = margin;
            tl.MarginTop = margin + captionH;
            tl.ColumnWidth = cW * 0.3f;
            tl.TextAlignment = TextAlignment.Justified;
            tl.AlignmentDelayToSplit = true;
            // Array of PageSplitArea's which control additional columns (1st column is controlled by
            // the 'main' TextLayout, for each additional one a PageSplitArea must be provided - 
            // it will create and return a TextLayout that can then be used to render the column):
            var psas = new PageSplitArea[]
            {
                new PageSplitArea(tl) { MarginLeft = tl.MarginLeft + (cW * 0.35f) },
                new PageSplitArea(tl) { ColumnWidth = -cW * 0.3f }
            };
            // Split options to control splitting text between pages:
            TextSplitOptions tso = new TextSplitOptions(tl)
            {
                RestMarginTop = margin,
                MinLinesInFirstParagraph = 2,
                MinLinesInLastParagraph = 2
            };
            // Generate a number of "chapters", provide outline entry for each:
            const int NChapters = 20;
            doc.Pages.Add();
            for (int i = 0; i < NChapters; ++i)
            {
                // Print chapter header across all columns:
                string chapter = $"Chapter {i + 1}";
                tlCaption.Clear();
                tlCaption.Append(chapter);
                tlCaption.PerformLayout(true);
                doc.Pages.Last.Graphics.DrawTextLayout(tlCaption, PointF.Empty);
                // Add outline node for the chapter:
                doc.Outlines.Add(new OutlineNode(chapter, new DestinationFitV(doc.Pages.Count - 1, null)));
                //
                // Clear last chapter's text and add new chapter:
                tl.FirstLineIsStartOfParagraph = true;
                tl.LastLineIsEndOfParagraph = true;
                tl.Clear();
                tl.Append(Common.Util.LoremIpsum(5, 7, 9, 15, 25));
                tl.PerformLayout(true);
                // Variable to hold last chapter end's bottom coord:
                float contentBottom = 0f;
                // Print the chapter:
                var tls = new TextLayoutSplitter(tl);
                while (true)
                {
                    var tlCol0 = tls.SplitAndBalance(psas, tso);
                    var g = doc.Pages.Last.Graphics;
                    g.DrawTextLayout(tlCol0, PointF.Empty);
                    g.DrawTextLayout(psas[0].TextLayout, PointF.Empty);
                    g.DrawTextLayout(psas[1].TextLayout, PointF.Empty);
                    if (tls.SplitResult != SplitResult.Split)
                    {
                        // End of chapter, find out how much height left on page for next chapter:
                        contentBottom = tl.ContentY + tl.ContentHeight;
                        contentBottom = Math.Max(contentBottom, psas[0].TextLayout.ContentRectangle.Bottom);
                        contentBottom = Math.Max(contentBottom, psas[1].TextLayout.ContentRectangle.Bottom);
                        // Done printing chapter:
                        break;
                    }
                    // Continue printing chapter on new page:
                    psas[0].MarginTop = psas[1].MarginTop = margin;
                    doc.Pages.Add();
                }
                // Next chapter - find out if we have enough space left on current page to start new chapter:
                if (contentBottom + captionH < pageHeight * 0.8f)
                {
                    // Start new chapter on current page:
                    contentBottom += pageHeight * 0.05f;
                    tlCaption.MarginTop = contentBottom;
                    tl.MarginTop = psas[0].MarginTop = psas[1].MarginTop = contentBottom + captionH;
                }
                else if (i < NChapters - 1)
                {
                    // Start new chapter on new page:
                    tlCaption.MarginTop = margin;
                    tl.MarginTop = psas[0].MarginTop = psas[1].MarginTop = margin + captionH;
                    doc.Pages.Add();
                }
            }
            // Done:
            doc.Save(stream);
        }
    }
}
