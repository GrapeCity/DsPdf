//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Pdf.Annotations;
using GrapeCity.Documents.Pdf.TextMap;
using GrapeCity.Documents.Pdf.AcroForms;

namespace GcPdfWeb.Samples
{
    // This sample demonstrates the use of GcPdfDocument.Redact() method.
    // It loads the PDF generated by the FindAndRedact sample, in which
    // certain areas are marked for redaction, and applies those redacts.
    // The original PDF without redact annotations is created by TimeSheet.
    public class ApplyRedact
    {
        public int CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            using (var fs = new FileStream(Path.Combine("Resources", "PDFs", "find-and-redact.pdf"), FileMode.Open, FileAccess.Read))
            {
                // Load the PDF containing redact annotations (areas marked for redaction):
                doc.Load(fs);

                // Apply the redacts:
                doc.Redact();

                // Done:
                doc.Save(stream);
                return doc.Pages.Count;
            }
        }
    }
}