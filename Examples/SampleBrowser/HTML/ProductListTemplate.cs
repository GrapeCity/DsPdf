//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Html;

namespace GcPdfWeb.Samples
{
    // This sample shows how to render a report (the list of products
    // from the standard NWind sample database) using a {{mustache}}
    // HTML template.
    //
    // The data query and HTML formatting are similar to those used
    // in the ProductList sample. Unlike that sample though, here
    // we use the HTML template file ProductListTemplate.html
    // loaded from a resource, and bind it to data using {{mustache}}.
    // Changing the template file (preserving the {{mustache}} bindings)
    // can be used to easily customize the look of the report.
    //
    // This sample uses the https://www.nuget.org/packages/Stubble.Core/ package to bind data to the template.
    //
    // Please see notes in comments at the top of HelloWorldHtml
    // sample code for details on adding GcHtml to your projects.
    public class ProductListTemplate
    {
        public void CreatePDF(Stream stream)
        {
            using (var ds = new DataSet())
            {
                // Fetch data:
                ds.ReadXml(Path.Combine("Resources", "data", "GcNWind.xml"));

                DataTable dtProds = ds.Tables["Products"];
                DataTable dtSupps = ds.Tables["Suppliers"];

                var products =
                    from prod in dtProds.Select()
                    join supp in dtSupps.Select()
                    on prod["SupplierID"] equals supp["SupplierID"]
                    orderby prod["ProductName"]
                    select new
                    {
                        ProductID = prod["ProductID"],
                        ProductName = prod["ProductName"],
                        Supplier = supp["CompanyName"],
                        QuantityPerUnit = prod["QuantityPerUnit"],
                        UnitPrice = $"{prod["UnitPrice"]:C}"
                    };

                // Load the template - HTML file with {{mustache}} data references:
                var template = File.ReadAllText(Path.Combine("Resources", "Misc", "ProductListTemplate.html"));
                // Bind the template to data:
                var builder = new Stubble.Core.Builders.StubbleBuilder();
                var boundTemplate = builder.Build().Render(template, new { Query = products });
                var tmp = Path.GetTempFileName();
                // Render the bound HTML:
                using (var re = new GcHtmlRenderer(boundTemplate))
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
                    // Render the generated HTML to the temporary file:
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
