using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Pdf.Annotations;
using GrapeCity.Documents.Pdf.Actions;

namespace GcPdfWeb.Samples.Basics
{
    // This sample demonstrates how to create file attachment annotations on a page.
    // See also the DocAttachments sample that demonstrates document level file attachments.
    public class FileAttachments
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;

            var rc = Common.Util.AddNote(
                "Some files from the sample's Resources/Images folder are attached to this page.\n" +
                "Some viewers may not show attachments, so we draw rectangles to indicate their (usually clickable) locations.", page);
            var ip = new PointF(rc.X, rc.Bottom + 9);
            var attSize = new SizeF(36, 12);
            var gap = 8;
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
            foreach (string fn in files)
            {
                string file = Path.Combine("Resources", "Images", fn);
                FileAttachmentAnnotation faa = new FileAttachmentAnnotation()
                {
                    Color = Color.FromArgb(unchecked((int)0xFFc540a5)),
                    UserName = "Jaime Smith",
                    Rect = new RectangleF(ip.X, ip.Y, attSize.Width, attSize.Height),
                    Contents = $"Attached file {file}",
                    Icon = FileAttachmentAnnotationIcon.Paperclip,
                    File = FileSpecification.FromEmbeddedFile(EmbeddedFileStream.FromFile(doc, file)),
                };
                page.Annotations.Add(faa);
                g.FillRectangle(faa.Rect, Color.FromArgb(unchecked((int)0xFF40c5a3)));
                g.DrawRectangle(faa.Rect, Color.FromArgb(unchecked((int)0xFF6040c5)));
                ip.Y += attSize.Height + gap;
            }
            // Done:
            doc.Save(stream);
        }
    }
}
