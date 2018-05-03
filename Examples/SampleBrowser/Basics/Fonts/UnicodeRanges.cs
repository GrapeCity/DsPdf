using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // This sample lists Unicode ranges available in each system font.
    public class UnicodeRanges
    {
        public int CreatePDF(Stream stream)
        {
            // Setup:
            GcPdfDocument doc = new GcPdfDocument();
            TextLayout tl = new TextLayout()
            {
                MaxWidth = doc.PageSize.Width,
                MaxHeight = doc.PageSize.Height,
                MarginLeft = 72,
                MarginRight = 72,
                MarginTop = 72,
                MarginBottom = 72,
            };
            tl.DefaultFormat.FontSize = 7;
            var tfH = new TextFormat() { Font = StandardFonts.TimesBold, FontSize = 12 };
            var tfP = new TextFormat() { Font = StandardFonts.Times, FontSize = 11 };

            // Loop through all system fonts,
            // list Unicode ranges provided by each font:
            foreach (Font font in FontCollection.SystemFonts)
            {
                tl.AppendLine($"{font.FontFileName} [{font.FullFontName}] [{font.FontFamilyName}]", tfH);
                var shot = font.CreateFontTables(TableTag.OS2);
                tl.AppendLine(shot.GetUnicodeRanges(), tfP);
                tl.AppendLine();
            }

            // Split and render TextLayout as shown in the PaginatedText sample:
            TextSplitOptions to = new TextSplitOptions(tl)
            {
                MinLinesInFirstParagraph = 2,
                MinLinesInLastParagraph = 2
            };
            tl.PerformLayout(true);
            while (true)
            {
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
