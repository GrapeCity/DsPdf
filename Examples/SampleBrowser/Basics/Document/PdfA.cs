//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using System.Text;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Common;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Pdf.Structure;
using GrapeCity.Documents.Pdf.MarkedContent;
using GrapeCity.Documents.Pdf.Graphics;
using GrapeCity.Documents.Pdf.Annotations;

namespace GcPdfWeb.Samples.Basics
{
    // This sample shows how to create a PDF/A-3u conformant document.
    public class PdfA
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var date = new DateTime(1961, 4, 12, 6, 7, 0, DateTimeKind.Utc);

            // Mark the document as PDF/A-3u conformant:
            doc.ConformanceLevel = PdfAConformanceLevel.PdfA3u;

            var fnt = Font.FromFile(Path.Combine("Resources", "Fonts", "arial.ttf"));
            var gap = 36;

            // PDF/A-3a requires all content to be tagged so create and populate StructElement when rendering:
            StructElement sePart = new StructElement("Part");
            doc.StructTreeRoot.Children.Add(sePart);

            TextLayout tl = null;
            // Add 3 pages with sample content tagged according to PDF/A rules:
            for (int pageNo = 1; pageNo <= 3; ++pageNo)
            {
                var page = doc.Pages.Add();
                var g = page.Graphics;
                float y = 72;
                if (doc.Pages.Count == 1)
                {
                    // Create paragraph element:
                    var seParagraph = new StructElement("P") { DefaultPage = page };
                    // Add it to Part element:
                    sePart.Children.Add(seParagraph);

                    tl = g.CreateTextLayout();
                    tl.MarginAll = 72;
                    tl.MaxWidth = page.Size.Width;

                    tl.DefaultFormat.Font = fnt;
                    tl.DefaultFormat.FontBold = true;
                    tl.DefaultFormat.FontSize = 20;
                    tl.Append("PDF/A-3A Document");

                    // PerformLayout is done automatically in a new TextLayout or after a Clear():
                    //tl.PerformLayout(true);

                    // Draw TextLayout within tagged content:
                    g.BeginMarkedContent(new TagMcid("P", 0));
                    g.DrawTextLayout(tl, PointF.Empty);
                    g.EndMarkedContent();

                    y = tl.ContentRectangle.Bottom + gap;

                    seParagraph.ContentItems.Add(new McidContentItemLink(0));
                }

                // Add some sample paragraphs tagged according to PDF/A rules:
                for (int i = 1; i <= 3; ++i)
                {
                    // Create paragraph element:
                    var seParagraph = new StructElement("P") { DefaultPage = page };
                    // Add it to Part element:
                    sePart.Children.Add(seParagraph);

                    var sb = new StringBuilder();
                    sb.Append(string.Format("Paragraph {0} on page {1}: ", i, pageNo));
                    sb.Append(Common.Util.LoremIpsum(1, 2, 4, 5, 10));
                    var para = sb.ToString();

                    tl.Clear();
                    tl.DefaultFormat.FontSize = 14;
                    tl.DefaultFormat.FontBold = false;
                    tl.MarginTop = y;
                    tl.Append(para);

                    // Draw TextLayout within tagged content:
                    g.BeginMarkedContent(new TagMcid("P", i));
                    g.DrawTextLayout(tl, PointF.Empty);
                    g.EndMarkedContent();

                    y += tl.ContentHeight + gap;

                    // Add content item to paragraph StructElement:
                    seParagraph.ContentItems.Add(new McidContentItemLink(i));

                    // PDF/A-3 allows to embed files into document, but they should be associated with some document element
                    // add embedded file associated with seParagraph:
                    var ef1 = EmbeddedFileStream.FromBytes(doc, Encoding.UTF8.GetBytes(para));
                    // ModificationDate and MimeType should be specified in case of PDF/A:
                    ef1.ModificationDate = date;
                    ef1.MimeType = "text/plain";
                    var fn = string.Format("Page{0}_Paragraph{1}.txt", pageNo, i);
                    var fs1 = FileSpecification.FromEmbeddedStream(fn, ef1);
                    // Relationship should be specified in case of PDF/A:
                    fs1.Relationship = AFRelationship.Unspecified;
                    doc.EmbeddedFiles.Add(fn, fs1);
                    seParagraph.AssociatedFiles.Add(fs1);
                }
            }

            // PDF/A-3 allows transparency drawing in PDF file, add some:
            var gpage = doc.Pages[0].Graphics;
            gpage.FillRectangle(new RectangleF(20, 20, 200, 200), Color.FromArgb(40, Color.Red));

            // PDF/A-3 allows to use FormXObjects, add one with transparency:
            var r = new RectangleF(0, 0, 144, 72);
            var fxo = new FormXObject(doc, r);
            var gfxo = fxo.Graphics;
            gfxo.FillRectangle(r, Color.FromArgb(40, Color.Violet));
            TextFormat tf = new TextFormat()
            {
                Font = fnt,
                FontSize = 16,
                ForeColor = Color.FromArgb(100, Color.Black),
            };
            gfxo.DrawString("FormXObject", tf, r, TextAlignment.Center, ParagraphAlignment.Center);
            gfxo.DrawRectangle(r, Color.Blue, 3);
            gpage.DrawForm(fxo, new RectangleF(300, 250, r.Width, r.Height), null, ImageAlign.ScaleImage);

            // PDF/A-3 allows to use embedded files, but each embedded file must be associated with a document's element:
            EmbeddedFileStream ef = EmbeddedFileStream.FromFile(doc, Path.Combine("Resources", "WordDocs", "ProcurementLetter.docx"));
            // ModificationDate and MimeType should be specified for EmbeddedFile in PDF/A:
            ef.ModificationDate = date;
            ef.MimeType = "application/msword";
            var fs = FileSpecification.FromEmbeddedFile(ef);
            fs.Relationship = AFRelationship.Unspecified;
            doc.EmbeddedFiles.Add("ProcurementLetter.docx", fs);
            // Associate embedded file with the document:
            doc.AssociatedFiles.Add(fs);

            // Add an attachment associated with an annotation:
            var sa = new StampAnnotation()
            {
                UserName = "Minerva",
                Font = fnt,
                Rect = new RectangleF(300, 36, 220, 72),
            };
            sa.Flags |= AnnotationFlags.Print;
            // Use a FormXObject to represent the stamp annotation:
            var stampFxo = new FormXObject(doc, new RectangleF(PointF.Empty, sa.Rect.Size));
            var gstampFxo = stampFxo.Graphics;
            gstampFxo.FillRectangle(stampFxo.Bounds, Color.FromArgb(40, Color.Green));
            gstampFxo.DrawString("Stamp Annotation\nassociated with minerva.jpg", tf, stampFxo.Bounds, TextAlignment.Center, ParagraphAlignment.Center);
            gstampFxo.DrawRectangle(stampFxo.Bounds, Color.Green, 3);
            //
            sa.AppearanceStreams.Normal.Default = stampFxo;
            doc.Pages[0].Annotations.Add(sa);
            ef = EmbeddedFileStream.FromFile(doc, Path.Combine("Resources", "Images", "minerva.jpg"));
            ef.ModificationDate = date;
            ef.MimeType = "image/jpeg";
            fs = FileSpecification.FromEmbeddedFile(ef);
            fs.Relationship = AFRelationship.Unspecified;
            doc.EmbeddedFiles.Add("minerva.jpg", fs);
            sa.AssociatedFiles.Add(fs);

            // Mark the document as conforming to Tagged PDF conventions (required for PDF/A):
            doc.MarkInfo.Marked = true;

            // Metadata.CreatorTool and DocumentInfo.Creator should be the same for a PDF/A document:
            doc.Metadata.CreatorTool = doc.DocumentInfo.Creator;
            // A title should be specified for PDF/A document:
            doc.Metadata.Title = "GcPdf Document";
            doc.ViewerPreferences.DisplayDocTitle = true;

            // Done:
            doc.Save(stream);
        }
    }
}
