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
    // This sample shows how to render a hard-coded HTML string.
    //
    // Adding GcHtml to your projects:
    // - Public classes and extension methods that allow to render HTML
    //   are provided by the GrapeCity.Documents.Html (GcHtml) package.
    // - GcHtml supports Windows, macOS and Linux platforms.
    // - Internally, it uses one of 3 system-dependent HTML engine packages:
    //   -- GrapeCity.Documents.Html.Windows.X64
    //   -- GrapeCity.Documents.Html.Mac.X64
    //   -- GrapeCity.Documents.Html.Linux.X64
    // - GcHtml will automatically select the correct system-dependent engine package
    //   at runtime, but that package must be referenced by your project so that GcHtml
    //   can find it. You can add references to all 3 platform packages to your project,
    //   or if you only target one or two platforms, you can add just the packages for
    //   these target platforms.
    //
    // Dealing with errors when using GcHtml:
    // - If GcHtml is not doing what you expect (e.g. rendering HTML to PDF
    //   produces invalid PDFs) - check the content of the static string property
    //   GcHtmlRenderer.LastLogMessage after the call to GcHtml returns,
    //   it may explain why the error occurred (e.g. a required shared library
    //   might be missing on Linux).
    // - Sometimes the HTML rendering process can hang without any diagnostics
    //   being written to GcHtmlRenderer.LastLogMessage. GcHtml automatically kills
    //   the hanging process in this case, but there is no diagnostics.
    //   Normally such situations should not occur.
    //
    // The above notes apply to any project that uses GcHtml.
    public class HelloWorldHtml
    {
        public void CreatePDF(Stream stream)
        {
            // HTML code that represents the content to render:
            var html = "<!DOCTYPE html>" +
                "<html>" +
                "<head>" +
                "<style>" +
                "span.bold {" +
                    "font-weight: bold;" +
                "}" +
                "p.round {" +
                    "font: 36px arial, sans-serif;" +
                    "color: DarkSlateBlue;" +
                    "border: 4px solid SlateBlue;" +
                    "border-radius: 16px;" +
                    "padding: 3px 5px 3px 5px;" +
                    "text-shadow: 3px 2px LightSkyBlue;" +
                "}" +
                "</style>" +
                "</head>" +
                "<body>" +
                "<p class='round'>Hello, World, from <span class='bold'>GcHtml</span>!</p>" +
                "</body>" +
                "</html>";

            // Create a new PDF document, add a page, get graphics to draw on:
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;

            try
            {
                // Render HTML.
                // The return value from DrawHtml() indicates whether anything has been rendered.
                // The output parameter 'size' returns the actual size of the rendered content.
                var ok = g.DrawHtml(html, 72, 72,
                    new HtmlToPdfFormat(false) { MaxPageWidth = 6.5f },
                    out SizeF size);

                // If anything has been rendered, draw an extra border around the rendered content:
                if (ok)
                {
                    var rc = new RectangleF(72 - 4, 72 - 4, size.Width + 8, size.Height + 8);
                    g.DrawRoundRect(rc, 8, Color.PaleVioletRed);
                }

                // Optional diagnostics that may be useful to diagnose Chromium behavior
                // (do NOT leave such code in production, as the last log message might
                // contain harmless messages even if all is ok):
                // if (!string.IsNullOrEmpty(GcHtmlRenderer.LastLogMessage))
                // {
                //   Common.Util.AddNote(GcHtmlRenderer.LastLogMessage, page, 
                //     new RectangleF(72, 72 + size.Height + 36, page.Size.Width - 144, page.Size.Height - size.Height - 108));
                // }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error:\n{ex.Message}\nChromium last log message:\n{GcHtmlRenderer.LastLogMessage}");

            }

            // Done:
            doc.Save(stream);
        }
    }
}
