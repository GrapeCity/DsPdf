//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System.IO;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Pdf.AcroForms;

namespace GcPdfWeb.Samples
{
    // This sample shows how to find and remove signature fields from a PDF.
    // The code in this sample is almost identical to the code in RemoveSignatures,
    // which also finds all signature fields, but removes just the signatures,
    // leaving the fields.
    // The PDF used in this sample was created by TimeSheet.
    public class RemoveSignatureFields
    {
        public int CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            using (var fs = new FileStream(Path.Combine("Resources", "PDFs", "TimeSheet.pdf"), FileMode.Open, FileAccess.Read))
            {
                doc.Load(fs);

                // Fields can be children of other fields, so we use
                // a recursive method to iterate through the whole tree:
                removeSignatureFields(doc.AcroForm.Fields);

                // Done:
                doc.Save(stream);
                return doc.Pages.Count;

                void removeSignatureFields(FieldCollection fields)
                {
                    for (int i = fields.Count - 1; i >= 0; --i)
                    {
                        removeSignatureFields(fields[i].Children);
                        // Note: if we just wanted to remove the signature
                        // without removing the field itself, we would do:
                        // ((SignatureField)fields[i]).Value = null;
                        if (fields[i] is SignatureField)
                            fields.RemoveAt(i);
                    }
                }
            }
        }
    }
}
