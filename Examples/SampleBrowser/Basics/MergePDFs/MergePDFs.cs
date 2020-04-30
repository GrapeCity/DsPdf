//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Pdf.Annotations;

namespace GcPdfWeb.Samples
{
    // This sample shows how to merge two existing PDFs into a single document.
    // The method GcPdfDocument.MergeWithDocument() provides this feature,
    // and allows to insert all or some pages from another PDF into the current
    // document. The simplest form of this method is demonstrated in this sample,
    // appending a whole PDF to the current document.
    public class MergePDFs
    {
        public int CreatePDF(Stream stream)
        {
            GcPdfDocument doc = new GcPdfDocument();

            using (var fs0 = new FileStream(Path.Combine("Resources", "PDFs", "The-Rich-History-of-JavaScript.pdf"), FileMode.Open, FileAccess.Read))
            using (var fs1 = new FileStream(Path.Combine("Resources", "PDFs", "CompleteJavaScriptBook.pdf"), FileMode.Open, FileAccess.Read))
            {
                doc.Load(fs0);
                // Save page count for the navigaion link added below:
                var pgNo = doc.Pages.Count;
                var doc1 = new GcPdfDocument();
                doc1.Load(fs1);
                doc.MergeWithDocument(doc1, new MergeDocumentOptions());

                // Insert a note at the beginning of the document:
                var page = doc.Pages.Insert(0);
                var rc = Common.Util.AddNote(
                    "GcPdfDocument.MergeWithDocument() method allows to add to the current document all or some pages " +
                    "from another document.\n" +
                    "In this sample we load one PDF, append another whole PDF to it, and save the result.\n" +
                    "Click this note to jump directly to the first page of the 2nd document.",
                    page);

                // Link the note to the first page of the second document:
                page.Annotations.Add(new LinkAnnotation(rc, new DestinationFit(pgNo + 1)));
                // Done (target document must be saved BEFORE the source is disposed):
                doc.Save(stream);
            }
            return doc.Pages.Count;
        }
    }
}
