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

namespace GcPdfWeb.Samples
{
    // This sample demonstrates how to extract text from an existing PDF.
    // It loads an arbitrary PDF into a temporary GcPdfDocument, then
    // retrieves text from each page of that document using the Page.GetText() method,
    // adds all those texts to a TextLayout and renders it into the current document.
    // An alternative to Page.GetText() is the method GcPdfDocument.GetText()
    // which retrieves the text from the whole document at once.
    public class ExtractText
    {
        public int CreatePDF(Stream stream)
        {
            GcPdfDocument doc = new GcPdfDocument();
            var page = doc.NewPage();

            var rc = Common.Util.AddNote(
                "This sample loads an arbitrary PDF into a temporary GcPdfDocument, " +
                "then retrieves text from each page of the loaded document using the Page.GetText() method, " +
                "adds all those texts to a TextLayout and renders it into the current document. " +
                "An alternative to Page.GetText() is the method GcPdfDocument.GetText() " +
                "which retrieves the text from the whole document at once.",
                page);

            // Text format for captions:
            var tf = new TextFormat()
            {
                Font = Font.FromFile(Path.Combine("Resources", "Fonts", "yumin.ttf")),
                FontSize = 14,
                ForeColor = Color.Blue
            };
            // Text layout to render the text:
            var tl = new TextLayout(72);
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 12;
            tl.MaxWidth = doc.PageSize.Width;
            tl.MaxHeight = doc.PageSize.Height;
            tl.MarginAll = rc.Left;
            tl.MarginTop = rc.Bottom + 36;

            // Text split options for widow/orphan control:
            TextSplitOptions to = new TextSplitOptions(tl)
            {
                MinLinesInFirstParagraph = 2,
                MinLinesInLastParagraph = 2,
                RestMarginTop = rc.Left,
            };

            // Open an arbitrary PDF, load it into a temp document and get all page texts:
            using (var fs = new FileStream(Path.Combine("Resources", "PDFs", "Wetlands.pdf"), FileMode.Open, FileAccess.Read))
            {
                var doc1 = new GcPdfDocument();
                doc1.Load(fs);

                // Get the texts of the loaded document's pages:
                var texts = new List<string>();
                doc1.Pages.ToList().ForEach(p_ => texts.Add(p_.GetText()));

                // Add texts and captions to the text layout:
                for (int i = 0; i < texts.Count; ++i)
                {
                    tl.AppendLine(string.Format("Text from page {0} of the loaded document:", i + 1), tf);
                    tl.AppendLine(texts[i]);
                }
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
            }
            // Done:
            doc.Save(stream);
            return doc.Pages.Count;
        }
    }
}
