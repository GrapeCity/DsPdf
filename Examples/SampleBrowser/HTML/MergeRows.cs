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
    // This sample shows how to build and render a table-based report
    // grouped by the first column, with that column's cells with same
    // values merged.
    // This sample uses a JavaScript code in the HTML to actually
    // merge the cells, demonstrating the use of JavaScript when
    // rendering HTML to PDF.
    // Note that the sample limits the number of rows so that
    // the whole table fits on a single page.
    //
    // Please see notes in comments at the top of HelloWorldHtml
    // sample code for details on adding GcHtml to your projects.
    public class MergeRows
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
                "  color: #1a5276;" +
                "  background-color: #d2b4de;" +
                "  text-align: center;" +
                "  padding: 6px;" +
                "}" +

                "thead {display: table-header-group;}" +

                "#products {" +
                "  font-family: 'Trebuchet MS', Arial, Helvetica, sans-serif;" +
                "  border-collapse: collapse;" +
                "  width: 100%;" +
                "}" +

                "#products td, #products th {" +
                "  border: 1px solid #ddd;" +
                "  padding: 8px;" +
                "}" +

                "#products tr:hover {background-color: #ddd;}" +

                "#products th {" +
                "  padding-top: 12px;" +
                "  padding-bottom: 12px;" +
                "  text-align: left;" +
                "  background-color: #a569bd;" +
                "  color: white;" +
                "}" +
                "</style>" +

                "</head>" +
                "<body onload='mergeRows()'>" +

                // solution from https://stackoverflow.com/questions/56587070/merge-neighbouring-html-table-cells-with-same-value-using-js
                "<script>" +
                "function mergeRows() {" +
                "  const table = document.querySelector('table');" +
                "  let headerCell = null;" +
                "  for (let row of table.rows)" +
                "  {" +
                "    const firstCell = row.cells[0];" +
                "    if (headerCell === null || firstCell.innerText !== headerCell.innerText)" +
                "    {" +
                "      headerCell = firstCell;" +
                "    }" +
                "    else" +
                "    {" +
                "      headerCell.rowSpan++;" +
                "      firstCell.remove();" +
                "    }" +
                "  }" +
                "}" +
                "</script>" +

                TTAG +

                "</body>" +
                "</html>";

            const string tableHead = "<h1>Products by Suppliers</h1>";

            const string tableFmt =
                "<table id='products'>" +
                "  <thead>" +
                "    <th>Supplier</th>" +
                "    <th>Description</th>" +
                "    <th>Quantity Per Unit</th>" +
                "    <th>Unit Price</th>" +
                "  </thead>" +
                "{0}" +
                "</table>";

            const string dataRowFmt =
                "  <tr>" +
                "    <td>{0}</td>" +
                "    <td>{1}</td>" +
                "    <td>{2}</td>" +
                "    <td align='right'>{3:C}</td>" +
                "  </tr>";

            using (var ds = new DataSet())
            {
                ds.ReadXml(Path.Combine("Resources", "data", "GcNWind.xml"));

                DataTable dtProds = ds.Tables["Products"];
                DataTable dtSupps = ds.Tables["Suppliers"];

                var products =
                    (from prod in dtProds.Select()
                     join supp in dtSupps.Select()
                 on prod["SupplierID"] equals supp["SupplierID"]
                     orderby supp["CompanyName"]
                     select new
                     {
                         ProductName = prod["ProductName"],
                         Supplier = supp["CompanyName"],
                         QuantityPerUnit = prod["QuantityPerUnit"],
                         UnitPrice = prod["UnitPrice"]
                     }).Take(16);

                var sb = new StringBuilder();
                sb.AppendLine(tableHead);
                foreach (var prod in products)
                    sb.AppendFormat(dataRowFmt, prod.Supplier, prod.ProductName, prod.QuantityPerUnit, prod.UnitPrice);

                var html = tableTpl.Replace(TTAG, string.Format(tableFmt, sb.ToString()));

                var tmp = Path.GetTempFileName();
                using (var re = new GcHtmlRenderer(html))
                {
                    // PdfSettings allow to provide options for HTML to PDF conversion:
                    var pdfSettings = new PdfSettings()
                    {
                        Margins = new Margins(0.2f, 1, 0.2f, 1),
                        IgnoreCSSPageSize = true,
                        DisplayHeaderFooter = true,
                        HeaderTemplate = "<div style='color:#1a5276; font-size:12px; width:1000px; margin-left:0.2in; margin-right:0.2in'>" +
                            "<span style='float:left;'>Product Price List</span>" +
                            "<span style='float:right'>Page <span class='pageNumber'></span> of <span class='totalPages'></span></span>" +
                            "</div>",
                        FooterTemplate = "<div style='color: #1a5276; font-size:12em; width:1000px; margin-left:0.2in; margin-right:0.2in;'>" +
                            "<span>(c) GrapeCity, Inc. All Rights Reserved.</span>" +
                            "<span style='float:right'>Generated on <span class='date'></span></span></div>"
                    };
                    // Render the source Web page to the temporary file:
                    re.RenderToPdf(tmp, pdfSettings);
                }
                // Copy the created PDF from the temp file to target stream:
                using (var ts = File.OpenRead(tmp))
                    ts.CopyTo(stream);
                // Clean up:
                File.Delete(tmp);
            }
            // Done.
        }
    }
}
