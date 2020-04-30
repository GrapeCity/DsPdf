//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using System.Text;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Html;

namespace GcPdfWeb.Samples
{
    // This sample shows how to insert an HTML table into a PDF
    // along with other (non-HTML) content.
    //
    // Please see notes in comments at the top of HelloWorldHtml
    // sample code for details on adding GcHtml to your projects.
    public class SimpleTable
    {
        public void CreatePDF(Stream stream)
        {
            const string TTAG = "___TABLE___";

            // HTML page template:
            const string tableTpl =
                "<!DOCTYPE html>" +
                "<html>" +
                "<head>" +
                "<style>" +

                "html * {" +
                "  font-family: 'Trebuchet MS', Arial, Helvetica, sans-serif !important;" +
                "}" +

                "h1 {" +
                "  color: #fcf3cf;" +
                "  background-color: #2471a3;" +
                "  text-align: center;" +
                "  padding: 6px;" +
                "  margin-bottom: 0px;" +
                "}" +

                "table {" +
                "  border-bottom: 1px solid #ddd;" +
                "}" +

                "thead {display: table-header-group;}" +

                "#employees {" +
                "  font-family: 'Trebuchet MS', Arial, Helvetica, sans-serif;" +
                "  border-collapse: collapse;" +
                "  width: 100%;" +
                "}" +

                "#employees td, #employees th {" +
                "  border: 0px solid #ddd;" +
                "  padding: 8px;" +
                "}" +

                "#employees tr:nth-child(even){background-color: #d4e6f1;}" +

                "#employees tr:hover {background-color: #ddd;}" +

                "#employees th {" +
                "  padding-top: 12px;" +
                "  padding-bottom: 12px;" +
                "  text-align: left;" +
                "  background-color: #2980b9;" +
                "  color: white;" +
                "}" +
                "</style>" +
                "</head>" +
                "<body>" +

                TTAG +

                "</body>" +
                "</html>";

            const string tableHead = "<h1>Employees</h1>";

            const string tableFmt =
                "<table id='employees'>" +
                "  <thead>" +
                "    <th>Name</th>" +
                "    <th>Address</th>" +
                "    <th>Country</th>" +
                "  </thead>" +
                "{0}" +
                "</table>";

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
            // Get page graphics:
            var g = page.Graphics;

            var nrc =  Common.Util.AddNote(
                "Here we build an HTML table with data fetched from an XML database, " +
                "and insert it into the current PDF page. " +
                "A footer is added below the table based on the rendered table size " +
                "returned by the GcPdfGraphics.DrawHtml() method.",
                page);

            // Get employees data from the sample NorthWind database:
            using (var ds = new DataSet())
            {
                ds.ReadXml(Path.Combine("Resources", "data", "GcNWind.xml"));
                DataTable dtEmps = ds.Tables["Employees"];
                var emps =
                    from emp in dtEmps.Select()
                    orderby emp["LastName"]
                    select new
                    {
                        Name = emp["LastName"] + ", " + emp["FirstName"],
                        Address = emp["Address"],
                        Country = emp["Country"]
                    };

                // Build the HTML table:
                var sb = new StringBuilder();
                sb.AppendLine(tableHead);
                foreach (var emp in emps)
                    sb.AppendFormat(dataRowFmt, emp.Name, emp.Address, emp.Country);

                var html = tableTpl.Replace(TTAG, string.Format(tableFmt, sb.ToString()));

                // Render HTML.
                // The return value indicates whether anything has been rendered.
                // The output parameter size returns the actual size of the rendered content:
                var ok = g.DrawHtml(html, nrc.Left, nrc.Bottom + 36,
                    new HtmlToPdfFormat(false) { MaxPageWidth = nrc.Width / 72 },
                    out SizeF size);

                Common.Util.AddNote(
                    "This text is added below the HTML table. Its position is determined " +
                    "by the rendered size returned by GcPdfGraphics.DrawHtml().",
                    page,
                    new RectangleF(nrc.Left, nrc.Bottom + size.Height + 72, nrc.Width, int.MaxValue));
            }
            // Done.
            doc.Save(stream);
        }
    }
}
