//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
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
    // In this sample we attach a few photos and two PDFs, including an AcroForm.
    // See also the FileAttachments sample that demonstrates file attachment annotations,
    // which are files associated with a specific location on a page.
    public class DocAttachments
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();

            (string, string)[] files = new (string, string)[]
            {
                ( "Images", "tudor.jpg" ),
                ( "Images", "sea.jpg" ),
                ( "Images", "puffins.jpg" ),
                ( "Images", "lavender.jpg" ),
                ( "Images", "skye.jpg" ),
                ( "Images", "fiord.jpg" ),
                ( "Images", "out.jpg" ),
                ( "PDFs", "HelloWorld.pdf" ),
                ( "PDFs", "FormFields.pdf" )
            };
            var sb = new StringBuilder();
            foreach (var f in files)
                sb.AppendLine(f.Item2);
            Common.Util.AddNote(
                "Several images and PDFs are attached to this document:\n\n" +
                sb.ToString(), page);
            foreach (var f in files)
            {
                string file = Path.Combine("Resources", f.Item1, f.Item2);
                FileSpecification fspec = FileSpecification.FromEmbeddedFile(EmbeddedFileStream.FromFile(doc, file));
                doc.EmbeddedFiles.Add(file, fspec);
            }

            // Done:
            doc.Save(stream);
        }
    }
}
