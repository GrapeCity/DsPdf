using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Pdf.AcroForms;
using GrapeCity.Documents.Text;
using System.Security.Cryptography.X509Certificates;

namespace GcPdfWeb.Samples
{
    // This sample generates and signs a PDF (using code that is similar to SignDoc sample),
    // and then signs the generated PDF with a second signature without invalidating the original
    // signature, by using incremental update (default when using the Sign() method).
    public class SignIncremental
    {
        public int CreatePDF(Stream stream)
        {
            GcPdfDocument doc = new GcPdfDocument();
            // Load a signed document (we use code similar to the SignDoc sample):
            doc.Load(CreateAndSignPdf());
            // Init a second certificate:
            var pfxPath = Path.Combine("Resources", "Misc", "JohnDoe.pfx");
            X509Certificate2 cert = new X509Certificate2(File.ReadAllBytes(pfxPath), "secret",
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            SignatureProperties sp2 = new SignatureProperties()
            {
                Certificate = cert,
                Location = "GcPdfWeb Sample Browser",
                SignerName = "Jaime Smith",
            };
            // Find the 2nd (not yet filled) signature field:
            var sfld2 = doc.AcroForm.Fields["SecondSignature"] as SignatureField;
            // Connect the signature field and signature props:
            sp2.SignatureField = sfld2 ?? throw new Exception("Unexpected: could not find 'SecondSignature' field");
            // Sign and save the document:
            doc.Sign(sp2, stream);

            // Rewind the stream to read the document just created 
            // into another GcPdfDocument and verify all signatures:
            stream.Seek(0, SeekOrigin.Begin);
            GcPdfDocument doc2 = new GcPdfDocument();
            doc2.Load(stream);
            foreach (var fld in doc2.AcroForm.Fields)
                if (fld is SignatureField sfld)
                    if (!sfld.Value.VerifySignature())
                        throw new Exception($"Failed to verify signature for field {sfld.Name}");

            // Done (the generated and signed document has already been saved to 'stream').
            return doc.Pages.Count;
        }

        // This method is almost exactly the same as the SignDoc sample,
        // but adds a second signature field (does not sign it though):
        private Stream CreateAndSignPdf()
        {
            GcPdfDocument doc = new GcPdfDocument();
            Page page = doc.NewPage();
            TextFormat tf = new TextFormat() { Font = StandardFonts.Times, FontSize = 14 };
            page.Graphics.DrawString(
                "Hello, World!\r\nSigned below TWICE by GcPdfWeb SignIncremental sample.\r\n(Note that some browser built-in viewers may not show the signatures.)",
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
            // Add the signature field to the document:
            doc.AcroForm.Fields.Add(sf);
            // Connect the signature field and signature props:
            sp.SignatureField = sf;

            // Add a second signature field:
            SignatureField sf2 = new SignatureField() { Name = "SecondSignature" };
            sf2.Widget.Rect = new RectangleF(72, 72 * 3, 72 * 4, 36);
            sf2.Widget.Page = page;
            sf2.Widget.BackColor = Color.LightYellow;
            // Add the signature field to the document:
            doc.AcroForm.Fields.Add(sf2);

            var ms = new MemoryStream();
            doc.Sign(sp, ms);
            return ms;
        }
    }
}
