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
    // This sample shows how to render a web page specified by a URL
    // to a PDF. Here we render the C# source of the WordIndex sample.
    // Similar to HtmlRenderPage0, this sample directly uses GcHtmlRenderer,
    // but shows how to specify options such as page orientation, margins,
    // headers and footers.
    //
    // For reference, the following markup extensions can be used in the
    // header and title templates (all except title are used in this sample):
    // - <span class="date">       - Formatted date
    // - <span class="title">      - Document title
    // - <span class="url">        - Document location
    // - <span class="pageNumber"> - Current page number
    // - <span class="totalPages"> - Total number of pages
    //
    // Note that in headers/footers images cannot be specified as links,
    // but base64 encoded data is supported as shown in this sample.
    //
    // Also note that for background colors in headers/footers to appear,
    // the following webkit CSS extension should be specified:
    // -webkit-print-color-adjust: exact;
    //
    // Please see notes in comments at the top of HelloWorldHtml
    // sample code for details on adding GcHtml to your projects.
    public class HtmlSettings
    {
        public void CreatePDF(Stream stream)
        {
            // Get a temporary file where the web page will be rendered:
            var tmp = Path.GetTempFileName();
            // The Uri of the web page to render:
            var uri = new Uri(@"https://www.grapecity.com/documents-api-pdf/demos/view-source-cs/WordIndex/");
            // Image used in the footer template:
            var image = @"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAFAAAABQCAYAAACOEfKtAAAACXBIWXMAAA7EAAAOxAGVKw4bAAAF8WlUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4gPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNS42LWMxNDggNzkuMTY0MDM2LCAyMDE5LzA4LzEzLTAxOjA2OjU3ICAgICAgICAiPiA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIgeG1sbnM6cGhvdG9zaG9wPSJodHRwOi8vbnMuYWRvYmUuY29tL3Bob3Rvc2hvcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RFdnQ9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZUV2ZW50IyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgMjEuMCAoV2luZG93cykiIHhtcDpDcmVhdGVEYXRlPSIyMDE5LTAzLTI4VDEzOjExOjE1LTA0OjAwIiB4bXA6TW9kaWZ5RGF0ZT0iMjAyMC0wMS0yMFQxMjo0NjowNi0wNTowMCIgeG1wOk1ldGFkYXRhRGF0ZT0iMjAyMC0wMS0yMFQxMjo0NjowNi0wNTowMCIgZGM6Zm9ybWF0PSJpbWFnZS9wbmciIHBob3Rvc2hvcDpDb2xvck1vZGU9IjMiIHBob3Rvc2hvcDpJQ0NQcm9maWxlPSJzUkdCIElFQzYxOTY2LTIuMSIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDoyNTcxMWNkZC1kZWVmLTU0NDUtYjMwMi0xNTJjNTMzMmE3NDciIHhtcE1NOkRvY3VtZW50SUQ9ImFkb2JlOmRvY2lkOnBob3Rvc2hvcDo1MzdlNGI5OS0zN2M3LTMyNDItYjQzNi1jN2I2YzM4N2JjOGIiIHhtcE1NOk9yaWdpbmFsRG9jdW1lbnRJRD0ieG1wLmRpZDpmNGZmNTI4Ny0yOWE4LTU2NGYtYjQ3YS05ZGZkNjg1NDNiMjciPiA8eG1wTU06SGlzdG9yeT4gPHJkZjpTZXE+IDxyZGY6bGkgc3RFdnQ6YWN0aW9uPSJjcmVhdGVkIiBzdEV2dDppbnN0YW5jZUlEPSJ4bXAuaWlkOmY0ZmY1Mjg3LTI5YTgtNTY0Zi1iNDdhLTlkZmQ2ODU0M2IyNyIgc3RFdnQ6d2hlbj0iMjAxOS0wMy0yOFQxMzoxMToxNS0wNDowMCIgc3RFdnQ6c29mdHdhcmVBZ2VudD0iQWRvYmUgUGhvdG9zaG9wIDIxLjAgKFdpbmRvd3MpIi8+IDxyZGY6bGkgc3RFdnQ6YWN0aW9uPSJzYXZlZCIgc3RFdnQ6aW5zdGFuY2VJRD0ieG1wLmlpZDoyNTcxMWNkZC1kZWVmLTU0NDUtYjMwMi0xNTJjNTMzMmE3NDciIHN0RXZ0OndoZW49IjIwMjAtMDEtMjBUMTI6NDY6MDYtMDU6MDAiIHN0RXZ0OnNvZnR3YXJlQWdlbnQ9IkFkb2JlIFBob3Rvc2hvcCAyMS4wIChXaW5kb3dzKSIgc3RFdnQ6Y2hhbmdlZD0iLyIvPiA8L3JkZjpTZXE+IDwveG1wTU06SGlzdG9yeT4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz5fbzkiAAALSElEQVR4nN2ceZBU1RXGf2/pdbae7qFBhmEfVtlEEUkpVFCKWIpGklhapUYTE4lWRTGmlMQNK0qW0lRFjUlc4pYFDcYYRBHBYAIiRkVRBFkGZmCAYfaZnt7ee/njDktvM72994Z8VVQxr+875/bX595z7rn3HskwDE7FnGtewmYMB5YCC3v/LwH1wJvAI8Be+7oGm57/ZsLfsk39SAcFQdB+4IfAeMADuIFa4BZgD/A44LSpjykYKAReD3QAt2bRdklv25vM7FC2sJvA2cAO4GnA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";

            // Create a GcHtmlRenderer with the source Uri
            // (note that GcHtmlRenderer ctor and other HTML rendering methods accept either a Uri
            // specifying the HTML page to render, or a string which represents the actual HTML):
            using (var re = new GcHtmlRenderer(uri))
            {
                // PdfSettings allow to provide options for HTML to PDF conversion:
                // - PageRanges allows to skip the first page which is empty in this case.
                // - PageWidth/PageHeight allow to customize page size (defaults are used here for demo).
                // - Margins specify page margins (the default is no margins).
                // - IgnoreCSSPageSize makes sure page size specified here is used.
                // - Landscape allows to change page orientation.
                // - Scale allows to enlarge or reduce the render size (default is 1).
                // - To add custom headers, DisplayHeaderFooter needs to be set to true.
                // - HeaderTemplate/FooterTemplate allow to specify custom page headers.
                var pdfSettings = new PdfSettings()
                {
                    PageRanges = "2-100",
                    PageWidth = 8.5f,
                    PageHeight = 11f,
                    Margins = new Margins(0.2f, 1, 0.2f, 1),
                    IgnoreCSSPageSize = true,
                    Landscape = true,
                    DisplayHeaderFooter = true,
                    HeaderTemplate = "<div style='-webkit-print-color-adjust:exact;background-color:#395daa;color:white;" +
                        "padding:0.1in;font-size:12em;width:1000px;margin-left:0.2in;margin-right:0.2in'>" +
                        "<span style='float:left'>Page <span class='pageNumber'></span> of <span class='totalPages'></span></span>" +
                        "<span style='float:right'>Document created on <span class='date'></span>" +
                        "</div>",
                    FooterTemplate = "<div style='font-size:12em;width:1000px;margin-left:0.2in;margin-right:0.2in'>" +
                        $"<span>Document location: <span class='url'></span><img style='float:right;' width='40' height='40' src='{image}'></img></div>"
                };
                // Render the source Web page to the temporary file:
                re.RenderToPdf(tmp, pdfSettings);
            }
            // Copy the created PDF from the temp file to target stream:
            using (var ts = File.OpenRead(tmp))
                ts.CopyTo(stream);
            // Clean up:
            File.Delete(tmp);
            // Done.
        }
    }
}
