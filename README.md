# GrapeCity Documents for PDF (GcPdf)

## Overview

**GrapeCity Documents for PDF** (aka **GcPdf**) is a collection of class libraries targeting .NET Standard 2.0,
written in C#, and providing APIs that allow you to create PDF files from scratch and to load, analyze and modify existing documents.

**GcPdf** runs on all platforms supported by .NET Standard, including .NET Core, ASP.NET Core, .NET Framework and so on,
and on all operating systems (Windows, macOS and Linux).

**GcPdf** and supporting packages are [available on NuGet.org](https://www.nuget.org/packages?q=grapecity.documents).

To use **GcPdf**  in an application, you need to reference (install) just the [GrapeCity.Documents.Pdf](https://www.nuget.org/packages/GrapeCity.Documents.Pdf/) package.
It will pull in the required infrastructure packages.

To render HTML content, install the [GrapeCity.Documents.Html](https://www.nuget.org/packages/GrapeCity.Documents.Html/)
(aka **GcHtml**) package. It allows to save whole HTML pages as PDFs, or include HTML fragments into PDFs along with other content.

To render barcodes, install the [GrapeCity.Documents.Barcode](https://www.nuget.org/packages/GrapeCity.Documents.Barcode/)
(aka **GcBarcode**) package. It provides extension methods that allow to add barcodes to PDFs.

On a **Windows** system, you can optionally install [GrapeCity.Documents.Common.Windows](https://www.nuget.org/packages/GrapeCity.Documents.Common.Windows/).
It provides support for font linking specified in the Windows registry, and access to native Windows imaging APIs,
improving performance and adding some features (e.g. TIFF support).

## What is here

This repository contains example projects that demonstrate basic and advanced features of **GcPdf**.
The samples' code includes extensive comments that will help you learn **GcPdf** and quickly get up to speed using it.

## Licensing

To use **GcPdf** in a production environment, you need a valid license,
please see [GrapeCity Licensing](https://www.grapecity.com/en/licensing/grapecity/) for details.

When used without a license, **GcPdf** has the following limitations:

- A header saying that an unlicensed version was used is added at the top of all pages in the generated PDFs.
- When loading PDFs, only up to 5 first pages are loaded.

## Resources

- [GcPdf Home](https://www.grapecity.com/en/documents-api-pdf)
- [Online Sample Browser](https://www.grapecity.com/documents-api-pdf/demos/)
- [Documentation](https://www.grapecity.com/documents-api-pdf/docs/online/overview.html)
- [GcPdf on NuGet](https://www.nuget.org/packages/GrapeCity.Documents.Pdf/)
