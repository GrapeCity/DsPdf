//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System.IO;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Pdf.AcroForms;

namespace GcPdfWeb.Samples
{
    // This sample shows how to find and remove existing signatures
    // from a PDF that had been digitally signed.
    // See the RemoveSignatureFields for code that removes any
    // signature fields instead.
    // The PDF used in this sample was created by TimeSheet.
    public class RemoveSignatures
    {
        public int CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            using (var fs = new FileStream(Path.Combine("Resources", "PDFs", "TimeSheet.pdf"), FileMode.Open, FileAccess.Read))
            {
                doc.Load(fs);

                // Fields can be children of other fields, so we use
                // a recursive method to iterate through the whole tree:
                removeSignatures(doc.AcroForm.Fields);

                // Done:
                doc.Save(stream);
                return doc.Pages.Count;

                void removeSignatures(FieldCollection fields)
                {
                    foreach (var f in fields)
                    {
                        if (f is SignatureField sf)
                            sf.Value = null;
                        removeSignatures(f.Children);
                    }
                }
            }
        }
    }
}
