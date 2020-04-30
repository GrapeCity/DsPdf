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
    // This sample shows how to use the text map to find specific content 
    // in a PDF and mark it for redaction.
    // Check out the ApplyRedact sample to see how the redact annotations
    // added by this sample can be applied to actually erase the data.
    // Also check out the samples in GcPdfViewer section to see how
    // redact annotations (added programmatically as in this sample,
    // or via the viewer's UI) can be applied selectively or all together.
    // The PDF used in this sample was created by TimeSheet.
    public class FindAndRedact
    {
        public int CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            using (var fs = new FileStream(Path.Combine("Resources", "PDFs", "TimeSheet.pdf"), FileMode.Open, FileAccess.Read))
            {
                doc.Load(fs);

                // Note: Acrobat does not allow to apply redactions in a digitally signed
                // document, so first we find and remove any existing signatures:
                removeSignatureFields(doc.AcroForm.Fields);

                // Loop through pages, removing anything that looks like a short date:
                foreach (var page in doc.Pages)
                {
                    var tmap = page.GetTextMap();
                    foreach (ITextLine tline in tmap)
                    {
                        if (Regex.Match(tline.Text.Trim(), @"\d+[/-]\w+[/-]\d").Success)
                        {
                            var redact = new RedactAnnotation()
                            {
                                Rect = tline.GetCoords().ToRect(),
                                MarkBorderColor = Color.Red,
                                MarkFillColor = Color.Yellow,
                                Page = page
                            };
                            // If we hadn't already set redact.Page = page, we could do this:
                            // page.Annotations.Add(redact);
                        }
                    }
                }
                // Done:
                doc.Save(stream);
                return doc.Pages.Count;

                // This code is from the RemoveSignatureFields sample:
                void removeSignatureFields(FieldCollection fields)
                {
                    for (int i = fields.Count - 1; i >= 0; --i)
                    {
                        removeSignatureFields(fields[i].Children);
                        if (fields[i] is SignatureField)
                            fields.RemoveAt(i);
                    }
                }
            }
        }
    }
}
