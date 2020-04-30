//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // This sample lists all fonts found in a loaded PDF,
    // prints some info for each font, and indicates whether a Font object
    // can be created from the font in the PDF.
    public class ListFonts
    {
        public int CreatePDF(Stream stream)
        {
            var sourcePDF = "CompleteJavaScriptBook.pdf";

            GcPdfDocument doc = new GcPdfDocument();
            var page = doc.NewPage();

            var rc = Common.Util.AddNote(
                "This sample loads an arbitrary PDF into a temporary GcPdfDocument, " +
                "and lists all fonts found in that document, with some of their properties. " +
                "It also tries to create a Font object from each of those PDF fonts, " +
                "and reports whether this operation succeeded.",
                page);

            // Text layout to render the text:
            var tab = 24;
            var tl = page.Graphics.CreateTextLayout();
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 12;
            tl.MaxWidth = doc.PageSize.Width;
            tl.MaxHeight = doc.PageSize.Height;
            tl.MarginAll = rc.Left;
            tl.MarginTop = rc.Bottom + 36;
            tl.TabStops = new List<TabStop>()
            {
                new TabStop(tab)
            };
            tl.FirstLineIndent = -tab;
            tl.MarginRight = 144;

            // Text split options for widow/orphan control:
            TextSplitOptions to = new TextSplitOptions(tl)
            {
                KeepParagraphLinesTogether = true,
                MinLinesInFirstParagraph = 2,
                MinLinesInLastParagraph = 2,
                RestMarginTop = rc.Left,
            };

            // Open an arbitrary PDF, load it into a temp document and get all fonts:
            using (var fs = new FileStream(Path.Combine("Resources", "PDFs", sourcePDF), FileMode.Open, FileAccess.Read))
            {
                var doc1 = new GcPdfDocument();
                doc1.Load(fs);

                var fonts = doc1.GetFonts();

                tl.AppendLine($"Total of {fonts.Count} fonts found in {sourcePDF}:");
                tl.AppendLine();
                int i = 0;
                foreach (var font in fonts)
                {
                    var nativeFont = font.CreateNativeFont();
                    tl.Append($"{i}:\tBaseFont: {font.BaseFont}; IsEmbedded: {font.IsEmbedded}.");
                    tl.AppendParagraphBreak();
                    if (nativeFont != null)
                        tl.AppendLine($"\tCreateNativeFont succeeded, family: {nativeFont.FontFamilyName}; bold: {nativeFont.FontBold}; italic: {nativeFont.FontItalic}.");
                    else
                        tl.AppendLine("\tCreateNativeFont failed");
                    tl.AppendLine();
                    ++i;
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
