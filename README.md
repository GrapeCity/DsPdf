# GrapeCity Documents for PDF (GcPdf)

Jump down to [Resources](#Resources).

## Overview

**GrapeCity Documents for PDF** (aka **GcPdf**) is a collection of class libraries targeting .NET Standard 2.0, written in C#, and providing APIs that allow to create PDF files from scratch and to load, analyze and modify existing documents.

**GcPdf** runs on all platforms supported by .NET Standard, including .NET Core, ASP.NET Core, .NET Framework and so on.

**GcPdf** and supporting packages are [available on NuGet.org](https://www.nuget.org/packages?q=grapecity.documents):

- [GrapeCity.Documents.Pdf](https://www.nuget.org/packages/GrapeCity.Documents.Pdf/)
- [GrapeCity.Documents.Barcode](https://www.nuget.org/packages/GrapeCity.Documents.Barcode/)
- [GrapeCity.Documents.Common](https://www.nuget.org/packages/GrapeCity.Documents.Common/)
- [GrapeCity.Documents.Common.Windows](https://www.nuget.org/packages/GrapeCity.Common.Windows/)
- [GrapeCity.Documents.DX.Windows](https://www.nuget.org/packages/GrapeCity.DX.Windows/)

To use **GcPdf**  in an application, you need to reference (install) just the **GrapeCity.Documents.Pdf** package. It will pull in the required infrastructure packages.

To render barcodes, install the **GrapeCity.Documents.Barcode** (aka **GcBarcode**) package. It provides extension methods allowing to render barcodes when using **GcPdf**.

On a **Windows** system, you can optionally install **GrapeCity.Documents.Common.Windows**. It provides support for font linking specified in the Windows registry, and access to native Windows imaging APIs, improving performance and adding some features (e.g. TIFF support).

**GrapeCity.Documents.Common** is an infrastructure package used by **GcPdf** and **GcBarcode**. You do not need to reference it directly. 

**GrapeCity.Documents.DX.Windows** is an infrastructure package used by
**GrapeCity.Documents.Common.Windows**. You do not need to reference
it directly.

## What is here

This repository contains sample projects to help learn GcPdf features and quickly get up to speed using it.

## Licensing

To use **GcPdf** in a production environment, you need a valid license, please see [GrapeCity Licensing](https://www.grapecity.com/en/licensing/grapecity/) for details.

An unlicensed version of **GcPdf** has the following limitations:

- A header saying that an unlicensed version was used, is added to the top of all pages in the generated PDFs.
- When loading PDFs, only up to 5 first pages are loaded.


## Resources

- [GcPdf Home](https://www.grapecity.com/en/documents-api-pdf)
- [Online Sample Browser](http://demos.componentone.com/gcdocs/gcpdf)

