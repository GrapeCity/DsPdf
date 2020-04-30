//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Html;

namespace GcPdfWeb.Samples
{
    // This sample shows the simplest way to render a web page
    // specified by a URL to a PDF (here we render the Google home page).
    //
    // In this sample we directly use the GcHtmlRenderer class
    // to render the page to a temporary PDF file, which is then
    // returned in the output stream.
    // A more flexible way that allows to easily add HTML conetnt
    // to a PDF file along with other content is via the extension
    // methods GcPdfGraphics.MeasureHtml()/GcPdfGraphics.DrawHtml()
    // as demonstrated by HelloWorldHtml and other samples.
    //
    // Please see notes in comments at the top of HelloWorldHtml
    // sample code for details on adding GcHtml to your projects.
    public class RenderPage0
    {
        public void CreatePDF(Stream stream)
        {
            // Get a temporary file where the web page will be rendered:
            var tmp = Path.GetTempFileName();
            // The Uri of the web page to render:
            var uri = new Uri("http://www.google.com");
            // Create a GcHtmlRenderer with the source Uri
            // (note that GcHtmlRenderer ctor and other HTML rendering methods accept either a Uri
            // specifying the HTML page to render, or a string which represents the actual HTML):
            using (var re = new GcHtmlRenderer(uri))
                // Render the source Web page to the temporary file:
                re.RenderToPdf(tmp);
            // Copy the created PDF from the temp file to target stream:
            using (var ts = File.OpenRead(tmp))
                ts.CopyTo(stream);
            // Clean up:
            File.Delete(tmp);
            // Done.
        }
    }
}
