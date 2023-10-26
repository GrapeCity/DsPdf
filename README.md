# Document Solutions for PDF (DsPdf)

## Overview

**Document Solutions for PDF** (aka **DsPdf**) is a .NET class library written in C#, providing an API
that allows you to create PDF files from scratch and to load, analyze and modify existing PDF documents.

**DsPdf** is compatible with .NET 7, .NET 6, .NET 5, .NET Core 3.x and 2.x, .NET Standard 2.x, .NET Framework 4.6.1 or higher,
with all features fully supported on all operating systems that support .NET.

**DsPdf** and supporting packages are [available on NuGet.org](https://www.nuget.org/packages?q=grapecity.documents).

To use **DsPdf**  in an application, you need to reference (install) just the [GrapeCity.Documents.Pdf](https://www.nuget.org/packages/GrapeCity.Documents.Pdf/) package.
It will pull in the required infrastructure packages.

To render HTML content, install the [GrapeCity.Documents.Html](https://www.nuget.org/packages/GrapeCity.Documents.Html/)
(aka **DsHtml**) package. It allows you to save whole HTML pages as PDFs, or include HTML fragments into PDFs along with other content.

To render barcodes, install the [GrapeCity.Documents.Barcode](https://www.nuget.org/packages/GrapeCity.Documents.Barcode/)
(aka **DsBarcode**) package. It provides extension methods that allow you to add barcodes to PDFs.

On a **Windows** system, you can optionally install [GrapeCity.Documents.Common.Windows](https://www.nuget.org/packages/GrapeCity.Documents.Common.Windows/).
It provides support for font linking specified in the Windows registry, and access to native Windows imaging APIs.

## What is here

This repository contains example projects that demonstrate basic and advanced features of **DsPdf**.
The samples' code includes extensive comments that will help you learn **DsPdf** and quickly get up to speed using it.

## Licensing

To use **DsPdf** in a production environment, you need a valid license, contact us.sales@mescius.com for details.

When used without a license, **DsPdf** has the following limitations:

- A header saying that an unlicensed version was used is added at the top of all pages in the generated PDFs.
- When loading PDFs, only up to 5 first pages are loaded.

## Resources

- [DsPdf Home](https://developer.mescius.com/document-solutions/dot-net-pdf-api)
- [Online Demo Browser](https://developer.mescius.com/document-solutions/dot-net-pdf-api/demos/)
- [Documentation](https://developer.mescius.com/document-solutions/dot-net-pdf-api/docs/online/overview.html)
- [DsPdf on NuGet](https://www.nuget.org/packages/GrapeCity.Documents.Pdf/)
