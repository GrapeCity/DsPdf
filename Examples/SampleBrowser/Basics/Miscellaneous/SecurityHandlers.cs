using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Pdf.Security;

namespace GcPdfWeb.Samples
{
    // This sample demonstrates the use of Security.EncryptHandler and Security.DecryptHandler.
    // Security.DecryptHandler allows to examine the security attributes of an existing PDF.
    // Security.EncryptHandler allows to specify security attributes when saving a PDF.
    // GcPdf supports Standard Security Handlers revisions 2, 3 and 4 (as defined in the PDF Spec).
    // In this sample, we use StandardSecurityHandlerRev4 which provides the most options.
    public class SecurityHandlers
    {
        public void CreatePDF(Stream stream)
        {
            // Sample passwords:
            var ownerPassword = "I'm the owner";
            var userPassword = "I'm a user";

            // Step 1: Generate a document with some security attributes:
            GcPdfDocument doc0 = new GcPdfDocument();
            var rc0 = Common.Util.AddNote(
                "Demonstrating security:\n" +
                "In this PDF, we specify certain encryption options,\n" +
                "and set owner and user passwords.",
                doc0.NewPage());

            // Create a Rev4 security handler:
            var ssh4 = new StandardSecurityHandlerRev4()
            {
                // Set some rev4 specific props:
                EncryptionAlgorithm = EncryptionAlgorithm.AES,
                EncryptStrings = true,
            };
            // StandardSecurityHandlerRev4 is derived from StandardSecurityHandlerRev3,
            // so we can do this to make sure we touch rev3-specific properties only
            // (the cast is for illustration only, you do not need it to set those props of course):
            if (ssh4 is StandardSecurityHandlerRev3 ssh3)
            {
                ssh3.EditingPermissions = EditingPermissions.AssembleDocument;
                ssh3.PrintingPermissions = PrintingPermissions.LowResolution;
            }
            // But StandardSecurityHandlerRev3 is NOT derived from StandardSecurityHandlerRev2,
            // because some properties have similar meanings but different syntax, so this:
            // if (ssh3 is StandardSecurityHandlerRev2 ssh2) { ... }
            // will NOT work.

            // Set some passwords:
            ssh4.OwnerPassword = ownerPassword;
            ssh4.UserPassword = userPassword;

            // Assign the handler we created to the document so that it is used when saving the PDF:
            doc0.Security.EncryptHandler = ssh4;

            // Save the PDF in a temp file, so that we can load it:
            var fn = Path.GetTempFileName();
            doc0.Save(fn);

            // Step 2: Load the generated PDf and examine its security attributes:
            var doc = new GcPdfDocument();
            using (var fs = new FileStream(fn, FileMode.Open, FileAccess.Read))
            {
                // User password is needed to load the document:
                doc.Load(fs, userPassword);

                // At this point we can examine doc.Security.DecryptHandler if it exists,
                // but there is NO Security.EncryptHandler:
                if (doc.Security.EncryptHandler != null)
                    throw new Exception("This should not happen.");

                var dh = doc.Security.DecryptHandler;
                if (dh is StandardSecurityHandlerRev4 dh_ssh4)
                {
                    // Make sure the loaded permissions are what we specified in Step 1:
                    Common.Util.AddNote(
                        $"Security attributes that were in the loaded PDF's DecryptHandler:\n" +
                        $"EditingPermissions: {dh_ssh4.EditingPermissions}\n" +
                        $"PrintingPermissions: {dh_ssh4.PrintingPermissions}",
                        doc.Pages[0],
                        new RectangleF(72, rc0.Bottom + 36, 72 * 6, 72 * 2));
                    // This won't work, sorry:
                    var noway = dh_ssh4.OwnerPassword;
                    if (noway != null)
                        throw new Exception("No way.");
                }
                else if (dh is StandardSecurityHandlerRev3 dh_ssh3)
                {
                    // If we didn't know that we have a Rev4 handler, we would add code here,
                }
                else if (dh is StandardSecurityHandlerRev2 dh_ssh2)
                {
                    // ... and here,
                }
                else
                {
                    // ... and done something in this case too.
                }

                // Save the new PDF - but PLEASE NOTE that because we did not set
                // the Security.EncryptHandler, the newly saved document has no security!
                doc.Save(stream);
            }
            // Clean up the temp file:
            File.Delete(fn);
        }
    }
}
