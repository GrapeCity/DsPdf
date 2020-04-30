//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using System.Text;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Html;

namespace GcPdfWeb.Samples
{
    // This sample shows how to insert an HTML table with a varying number of rows
    // that might not fit on a single page, into a PDF document starting at an arbitrary
    // position on the page (all data rows must have the same height though).
    // We first create a table with a single data row, measure its height,
    // then create a similar table but with two data rows and measure it too.
    // This allows us to find out the header and data rows' heights, and render
    // the table to a PDF starting at the desired position on the page, and split
    // it into additional pages as needed.
    //
    // Please see notes in comments at the top of HelloWorldHtml
    // sample code for details on adding GcHtml to your projects.
    public class DynamicTable
    {
        public int CreatePDF(Stream stream)
        {
            // This tag is used to insert the prepared table HTML code
            // into the HTML page template that defines the CSS styles etc.
            // (Using this tag allows to use string.Format when building
            // the table HTML code, as otherwise curly braces in style
            // definitions would interfere with format specifiers.)
            const string TTAG = "___TABLE___";

            // The HTML page template:
            const string tableTpl =
                "<!DOCTYPE html>" +
                "<html>" +
                "<head>" +
                "<style>" +
                "#employees {" +
                "  font-family: 'Trebuchet MS', Arial, Helvetica, sans-serif;" +
                "  border-collapse: collapse;" +
                "  width: 100%;" +
                "}" +

                "#employees td, #employees th {" +
                "  border: 1px solid #ddd;" +
                "  padding: 8px;" +
                "}" +

                "#employees tr:nth-child(even){background-color: #f2f2f2;}" +

                "#employees tr:hover {background-color: #ddd;}" +

                "#employees th {" +
                "  padding-top: 12px;" +
                "  padding-bottom: 12px;" +
                "  text-align: left;" +
                "  background-color: #3377ff;" +
                "  color: white;" +
                "}" +
                "</style>" +
                "</head>" +
                "<body>" +

                TTAG +

                "</body>" +
                "</html>";

            // The table HTML code format:
            const string tableFmt =
                "<table id='employees'>" +
                "  <tr>" +
                "    <th>Index</th>" +
                "    <th>Lorem</th>" +
                "    <th>Ipsum</th>" +
                "  </tr>" +
                "{0}" +
                "</table>";

            // The table row HTML code format:
            const string dataRowFmt =
                "  <tr>" +
                "    <td>{0}</td>" +
                "    <td>{1}</td>" +
                "    <td>{2}</td>" +
                "  </tr>";

            // Create a new PDF document:
            var doc = new GcPdfDocument();
            // Add a page:
            var page = doc.NewPage();
            // Add a page, get its graphics:
            var g = page.Graphics;

            // Set up HTML to PDF formatting options.
            // The most important are the size limits, in this case
            // we do not limit the height as we will adjust it programmatically.
            // Note that in HtmlToPdfFormat, sizes are specified in inches:
            var hf = new HtmlToPdfFormat(false) { MaxPageWidth = page.Size.Width / 72 };

            // HTML code for a single data row (with sample data):
            var dataRow = string.Format(dataRowFmt, "a", "b", "c");
            // HTML page with a table that has a single data row:
            var thtml = tableTpl.Replace(TTAG, string.Format(tableFmt, dataRow));
            // Measure the HTML for the current GcPdfGraphics:
            var s1 = g.MeasureHtml(thtml, hf);
            // Same HTML page but with two data rows:
            thtml = tableTpl.Replace(TTAG, string.Format(tableFmt, dataRow + dataRow));
            // Measure the new HTML:
            var s2 = g.MeasureHtml(thtml, hf);
            // Calculate data row and header row heights:
            var rowHeight = s2.Height - s1.Height;
            var headerHeight = s1.Height - rowHeight;

            // Add a note at the top of the first page:
            var nrc = Common.Util.AddNote(
                "Here we render an HTML table with an unknown number of rows " +
                "that starts at a specified position on the first page, " +
                "and may span multiple pages.",
                page);

            // Set up for building the table with random data:
            var lorems = Common.Util.LoremWords();
            var rnd = Common.Util.NewRandom();
            var sb = new StringBuilder();

            // Page layout parameters:
            var marginx = nrc.Left;
            var marginy = nrc.Top;
            var x = marginx;
            var y = nrc.Bottom + 36;
            var tbottom = nrc.Bottom + 36 + headerHeight;
            // A random number of data rows to render:
            int nrows = rnd.Next(100, 200);
            // Generate and render the table, adding continuation pages as needed:
            for (int i = 0; i < nrows; ++i)
            {
                sb.AppendFormat(dataRowFmt, i, lorems[rnd.Next(lorems.Count)], lorems[rnd.Next(lorems.Count)]);
                tbottom += rowHeight;
                var lastPage = i == nrows - 1;
                if (tbottom >= page.Size.Height - 72 || lastPage)
                {
                    var html = tableTpl.Replace(TTAG, string.Format(tableFmt, sb.ToString()));
                    var ok = g.DrawHtml(html, x, y,
                        new HtmlToPdfFormat(false) { MaxPageWidth = (page.Size.Width - marginx * 2) / 72 },
                        out SizeF size);
                    if (!lastPage)
                    {
                        page = doc.NewPage();
                        g = page.Graphics;
                        y = 72;
                        tbottom = y + headerHeight;
                        sb.Clear();
                    }
                }
            }

            // Done:
            doc.Save(stream);
            return doc.Pages.Count;
        }
    }
}
