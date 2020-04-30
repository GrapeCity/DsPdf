//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Pdf.AcroForms;
using GrapeCity.Documents.Text;
using System.Security.Cryptography.X509Certificates;

namespace GcPdfWeb.Samples
{
    // This sample demonstrates how to create and sign a PDF with a .pfx file,
    // using a SignatureField.
    // The sample then loads the signed file back into another GcPdfDocument instance
    // and verifies the signature.
    // See also VisualSignature which is similar but adds a visual representation
    // of the signature.
    public class SignDoc
    {
        public int CreatePDF(Stream stream)
        {
            GcPdfDocument doc = new GcPdfDocument();
            Page page = doc.NewPage();
            TextFormat tf = new TextFormat() { Font = StandardFonts.Times, FontSize = 14 };
            page.Graphics.DrawString(
                "Hello, World!\r\nSigned below by GcPdfWeb SignDoc sample.",
                tf, new PointF(72, 72));

            // Init a test certificate:
            var pfxPath = Path.Combine("Resources", "Misc", "GcPdfTest.pfx");
            X509Certificate2 cert = new X509Certificate2(File.ReadAllBytes(pfxPath), "qq",
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            SignatureProperties sp = new SignatureProperties();
            sp.Certificate = cert;
            sp.Location = "GcPdfWeb Sample Browser";
            sp.SignerName = "GcPdfWeb";

            // Init a signature field to hold the signature:
            SignatureField sf = new SignatureField();
            sf.Widget.Rect = new RectangleF(72, 72 * 2, 72 * 4, 36);
            sf.Widget.Page = page;
            sf.Widget.BackColor = Color.LightSeaGreen;
            sf.Widget.TextFormat.Font = StandardFonts.Helvetica;
            sf.Widget.ButtonAppearance.Caption = $"Signer: {sp.SignerName}\r\nLocation: {sp.Location}";
            // Add the signature field to the document:
            doc.AcroForm.Fields.Add(sf);
            // Connect the signature field and signature props:
            sp.SignatureField = sf;

            // Sign and save the document:
            // NOTES:
            // - Signing and saving is an atomic operation, the two cannot be separated.
            // - The stream passed to the Sign() method must be readable.
            doc.Sign(sp, stream);

            // Rewind the stream to read the document just created 
            // into another GcPdfDocument and verify the signature:
            stream.Seek(0, SeekOrigin.Begin);
            GcPdfDocument doc2 = new GcPdfDocument();
            doc2.Load(stream);
            SignatureField sf2 = (SignatureField)doc2.AcroForm.Fields[0];
            if (!sf2.Value.VerifySignature())
                throw new Exception("Failed to verify the signature");

            // Done (the generated and signed document has already been saved to 'stream').
            return doc.Pages.Count;
        }
    }
}
