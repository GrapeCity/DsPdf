//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Pdf.Annotations;
using GrapeCity.Documents.Pdf.Graphics;
using GrapeCity.Documents.Pdf.TextMap;

namespace GcPdfWeb.Samples
{
    // This sample shows how to use the text map for a page in a PDF
    // to find geometric positions of text lines on the page,
    // and to locate the text at a specific position.
    // The PDF used in this sample was created by TimeSheet.
    public class TextMap
    {
        public int CreatePDF(Stream stream)
        {
            GcPdfDocument doc = new GcPdfDocument();
            var page = doc.NewPage();

            var rc = Common.Util.AddNote(
                "This sample loads the PDF created by the TimeSheet sample into a temporary GcPdfDocument, " +
                "gets the text map for the first page, and prints out the coordinates and texts of all " +
                "line fragments in the map. " +
                "It also uses the map's HitTest method to find the text at specific coordinates in the PDF " +
                "and prints the result. " +
                "The original TimeSheet.pdf used by this sample (consisting of 1 page) is appended for reference.",
                page);

            // Setup text formatting and layout:
            var tf = new TextFormat()
            {
                Font = StandardFonts.Times,
                FontSize = 13
            };
            var tfFound = new TextFormat()
            {
                Font = StandardFonts.TimesBold,
                FontSize = 14,
                ForeColor = Color.DarkBlue
            };
            var tl = new TextLayout(72)
            {
                MaxWidth = doc.PageSize.Width,
                MaxHeight = doc.PageSize.Height,
                MarginAll = rc.Left,
                MarginTop = rc.Bottom + 36,
                TabStops = new List<TabStop>() { new TabStop(72 * 2) },
            };
            TextSplitOptions to = new TextSplitOptions(tl)
            {
                MinLinesInFirstParagraph = 2,
                MinLinesInLastParagraph = 2,
                RestMarginTop = rc.Left,
            };

            // Open an arbitrary PDF, load it into a temp document and use the map to find some texts:
            using (var fs = new FileStream(Path.Combine("Resources", "PDFs", "TimeSheet.pdf"), FileMode.Open, FileAccess.Read))
            {
                var doc1 = new GcPdfDocument();
                doc1.Load(fs);
                var tmap = doc1.Pages[0].GetTextMap();

                // We retrieve the text at a specific (known to us) geometric location on the page:
                float tx0 = 2.1f, ty0 = 3.37f, tx1 = 3.1f, ty1 = 3.5f;
                HitTestInfo htiFrom = tmap.HitTest(tx0 * 72, ty0 * 72);
                HitTestInfo htiTo = tmap.HitTest(ty0 * 72, ty1 * 72);
                tmap.GetFragment(htiFrom.Pos, htiTo.Pos, out TextMapFragment range1, out string text1);
                tl.AppendLine($"Looked for text inside rectangle x={tx0:F2}\", y={ty0:F2}\", width={tx1-tx0:F2}\", height={ty1-ty0:F2}\", found:", tf);
                tl.AppendLine(text1, tfFound);
                tl.AppendLine();

                // Get all text fragments and their locations on the page:
                tl.AppendLine("List of all texts found on the page", tf);
                tmap.GetFragment(out TextMapFragment range, out string text);
                foreach (TextLineFragment tlf in range)
                {
                    var coords = tmap.GetCoords(tlf);
                    tl.Append($"Text at ({coords.B.X / 72:F2}\",{coords.B.Y / 72:F2}\"):\t", tf);
                    tl.AppendLine(tmap.GetText(tlf), tfFound);
                }

                // Print the results:
                tl.PerformLayout(true);
                while (true)
                {
                    // 'rest' will accept the text that did not fit:
                    var splitResult = tl.Split(to, out TextLayout rest);
                    doc.Pages.Last.Graphics.DrawTextLayout(tl, PointF.Empty);
                    if (splitResult != SplitResult.Split)
                        break;
                    tl = rest;
                    doc.NewPage();
                }

                // Append the original document for reference:
                doc.MergeWithDocument(doc1, new MergeDocumentOptions());
            }
            // Done:
            doc.Save(stream);
            return doc.Pages.Count;
        }
    }
}
