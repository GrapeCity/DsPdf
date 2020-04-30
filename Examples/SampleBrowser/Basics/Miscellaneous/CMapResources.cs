//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples
{
    // This sample is identical to the ExtractText sample,
    // but it loads a PDF that uses less common CMaps that are not built
    // into GcPdf. But, many such CMaps are provided by the optional
    // GrapeCity.Documents.Pdf.Resources package. It is not automatically
    // referenced by GcPdf, but if a reference to it is added to a project
    // (like in this sample), it will be picked up automatically
    // by the GcPdfDocument static ctor, and the additional CMaps 
    // will become available.
    //
    // The ExternalCMapTest.pdf used by this sample contains text
    // that is encoded using one of those less common CMaps.
    // Referencing GrapeCity.Documents.Pdf.Resources allows to retrieve
    // the text from that PDF using the GetText() method. 
    // If you download this sample and REMOVE the reference 
    // to GrapeCity.Documents.Pdf.Resources, the sample will build and run,
    // but it will not be able to extract the text.
    // 
    // If needed, arbitrary CMaps can be made available to GcPdf
    // using the static GcPdfDocument.CMapProvider property.
    // 
    public class CMapResources
    {
        public int CreatePDF(Stream stream)
        {
            const string pdfName = "ExternalCMapTest.pdf";

            // This line may not be needed if the GcPdfDocument class ctor finds
            // the GrapeCity.Documents.Pdf.Resources.dll at startup
            // (this depends on the way the app is deployed):
            GcPdfDocument.CMapProvider = CMapProvider.Instance;

            GcPdfDocument doc = new GcPdfDocument();
            var page = doc.NewPage();

            var rc = Common.Util.AddNote("This sample loads a PDF into a temporary GcPdfDocument, " +
                "retrieves all text from each page of the loaded document using the Page.GetText() method, " +
                "adds all those texts to a TextLayout and renders it into the current document. " +
                "CMaps used by the PDF in this sample are provided by the optional " +
                "https://www.nuget.org/packages/GrapeCity.Documents.Pdf.Resources/ package. " +
                "Without a reference to that package most of the text in this particular PDF will not be found " +
                "as it uses the less common CMaps that are not built into GcPdf itself." +
                "\n\n" +
                "To use GrapeCity.Documents.Pdf.Resources in a project, add a reference to it, " +
                "and either make sure that GrapeCity.Documents.Pdf.Resources.dll is present in the runtime directory, " +
                "or add the line:" +
                "\n\tGcPdfDocument.CMapProvider = CMapProvider.Instance;" +
                "\nto the project's initialization code.",
                page);

            Font arialbd = Font.FromFile(Path.Combine("Resources", "Fonts", "arialbd.ttf"));
            Font segoe = Font.FromFile(Path.Combine("Resources", "Fonts", "segoeui.ttf"));
            Font arialuni = Font.FromFile(Path.Combine("Resources", "Fonts", "arialuni.ttf"));
            segoe.AddLinkedFont(arialuni);

            // Text format for captions:
            var tf = new TextFormat()
            {
                Font = arialbd,
                FontSize = 14,
                ForeColor = Color.Blue
            };
            // Text layout to render the text:
            var tl = new TextLayout(72);
            tl.DefaultFormat.Font = segoe;
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
            using (var fs = new FileStream(Path.Combine("Resources", "PDFs", pdfName), FileMode.Open, FileAccess.Read))
            {
                var doc1 = new GcPdfDocument();
                doc1.Load(fs);

                // Get the texts of the loaded document's pages:
                var texts = new List<string>();
                doc1.Pages.ToList().ForEach(p_ => texts.Add(p_.GetText()));

                // Add texts and captions to the text layout:
                for (int i = 0; i < texts.Count; ++i)
                {
                    tl.AppendLine($"Text from page {i + 1} of {pdfName}:", tf);
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
