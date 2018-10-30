using System;
using System.IO;
using System.Linq;
using System.Text;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;

namespace GcPdfWeb.Samples.Basics
{
    // Shows how to attach files to a PDF document.
    // See also the FileAttachments sample that demonstrates file attachment annotations,
    // which are attached to a specific location on a page.
#if !DIODOCS_V1
#endif
    public class DocAttachments
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();

            string[] files = new string[]
            {
                "tudor.jpg",
                "sea.jpg",
                "puffins.jpg",
                "lavender.jpg",
                "skye.jpg",
                "fiord.jpg",
                "out.jpg"
            };
            var sb = new StringBuilder();
            foreach (var fn in files)
                sb.AppendLine(fn);
            Common.Util.AddNote(
                "Several images from the sample's Resources/Images folder are attached to this document:\n\n" +
                sb.ToString(), page);
            foreach (string fn in files)
            {
                string file = Path.Combine("Resources", "Images", fn);
                FileSpecification fspec = FileSpecification.FromEmbeddedFile(EmbeddedFileStream.FromFile(doc, file));
                doc.EmbeddedFiles.Add(file, fspec);
            }
            // Done:
            doc.Save(stream);
        }
    }
}
