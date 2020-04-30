//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.IO.Compression;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples
{
    // Shows how to set various document properties such as PDF version, document info and metadata.
    public class DocProperties
    {
        public void CreatePDF(Stream stream)
        {
            // Create a new PDF document:
            var doc = new GcPdfDocument();
            // While normally the PDF Version written into the document is determined automatically
            // (as the minimal version needed for the features actually used in this document),
            // it can be explicitly set:
            doc.PdfVersion = "1.7";
            // By default, GcPdfDocument uses CompressionLevel.Fastest to reduce the file size.
            // Setting the level to CompressionLevel.NoCompression will result in uncompressed PDF:
            doc.CompressionLevel = CompressionLevel.NoCompression;
            // By default, font subsets containing just the glyphs used in the document,
            // are embedded. This can be changed to embed whole fonts (which may result in
            // huge file size) or to not embed fonts as we do here (embedding can also
            // be controlled for individual fonts using the GcPdfDocument.Fonts collection):
            doc.FontEmbedMode = FontEmbedMode.NotEmbed;
            // Document properties such as title, author etc. can be set on GcPdfDocument.DocumentInfo,
            // but it should be noted that similar properties can be set in the GcPdfDocument.Metadata
            // and in most PDF readers properties set in metadata (see below) take precedence.
            // Here we set some of the properties available in DocumentInfo:
            doc.DocumentInfo.Title = "GcPdf Document Info Sample";
            doc.DocumentInfo.Author = "Jaime Smith";
            doc.DocumentInfo.Subject = "GcPdfDocument.DocumentInfo";
            doc.DocumentInfo.Producer = "GcPdfWeb Producer";
            doc.DocumentInfo.Creator = "GcPdfWeb Creator";
            // For sample sake, set CreationDate to a date 10 years in the future:
            doc.DocumentInfo.CreationDate = DateTime.Now.AddYears(10);
            // Document metadata is available via the GcPdfDocument.Metadata property.
            // It provides a number of predefined accessors, such as:
            doc.Metadata.Contributors.Add("contributor 1");
            doc.Metadata.Contributors.Add("contributor 2");
            doc.Metadata.Contributors.Add("contributor 3");
            doc.Metadata.Copyright = "GrapeCity Inc.";
            doc.Metadata.Creators.Add("Creator 1");
            doc.Metadata.Creators.Add("Creator 2");
            doc.Metadata.Creators.Add("Creator 3");
            doc.Metadata.Description = "Sample document description";
            doc.Metadata.Keywords.Add("Keyword1");
            doc.Metadata.Keywords.Add("Keyword2");
            doc.Metadata.Keywords.Add("Keyword3");
            doc.Metadata.Source = "Sourced by GcPdfWeb";
            // Arbitrary metadata can also be added using the AddProperty method:
            doc.Metadata.AddProperty("http://purl.org/dc/elements/1.1/", "language", "English");
            // Finally, add a page and some text to the document and save it:
            Common.Util.AddNote("GcPdf Document Properties sample.", doc.NewPage());
            // Done:
            doc.Save(stream);
        }
    }
}
