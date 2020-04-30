//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Barcode;

namespace GcPdfWeb.Samples.Barcodes
{
    // Renders samples of all barcode symbologies supported by the GcBarcode library.
    public class SupportedBarcodes
    {
        public void CreatePDF(Stream stream)
        {
            GcPdfDocument doc = new GcPdfDocument();
            Page page = null;
            GcGraphics g = null;
            const float margin = 72 / 2;
            const float pad = 4;
            const float gap = 10;
            var ip = new PointF(margin, margin);
            Action newPage = () =>
            {
                page = doc.NewPage();
                g = page.Graphics;
                ip = new PointF(margin, margin);
            };
            newPage();

            var tfCaption = new TextFormat()
            {
                Font = StandardFonts.Times,
                FontSize = 12,
            };
            var tfBarcode = new TextFormat()
            {
                Font = StandardFonts.Helvetica,
                FontSize = 9,
            };
            GcBarcode barcode = new GcBarcode()
            {
                TextFormat = tfBarcode,
                ScaleFactor = 1.5f,
            };
            barcode.Options.CaptionPosition = BarCodeCaptionPosition.Below;
            barcode.Options.SizeOptions.NarrowWideRatio = 0;

            Action<CodeType, string, string> drawBarcode = (ct_, txt_, txt2_) =>
            {
                var caption = $"{ct_}:\r\n{txt_}";
                if (string.IsNullOrEmpty(txt2_))
                    barcode.Options.GS1Composite.Type = GS1CompositeType.None;
                else
                {
                    caption += $"\r\nDependent CCA: {txt2_}";
                    barcode.Options.GS1Composite.Type = GS1CompositeType.CCA;
                    barcode.Options.GS1Composite.Value = txt2_;
                }
                barcode.Options.CheckSumEnabled = ct_ != CodeType.Code25intlv && ct_ != CodeType.Code_2_of_5 && ct_ != CodeType.Matrix_2_of_5;
                var csize = g.MeasureString(caption, tfCaption);
                barcode.CodeType = ct_;
                barcode.Text = txt_;
                var size = g.MeasureBarcode(barcode);
                size.Height = Math.Max(size.Height, csize.Height);
                var border = new RectangleF(ip, new SizeF(page.Size.Width - margin * 2, size.Height + pad * 2));
                if (ip.Y + border.Height > page.Size.Height - margin)
                {
                    newPage();
                    border = new RectangleF(ip, border.Size);
                }
                g.DrawRectangle(border, Color.Gray);
                g.DrawString(caption, tfCaption, new PointF(border.Left + pad, border.Top + pad));
                g.DrawBarcode(barcode, new RectangleF(border.Right - size.Width - pad, border.Top + pad, size.Width, size.Height));
                ip.Y = border.Bottom + gap;
            };
            //
            drawBarcode(CodeType.Ansi39, "*GCBARCODE*", null);
            drawBarcode(CodeType.Ansi39x, "*GcPdf*", null);
            drawBarcode(CodeType.Codabar, "A12041961D", null);
            drawBarcode(CodeType.Code25intlv, "1234567890", null); // Interleaved 2 of 5 (ITF)
            drawBarcode(CodeType.Code39, "*GCBARCODE*", null);
            drawBarcode(CodeType.Code39x, "*GcPdf*", null);
            drawBarcode(CodeType.Code49, "GcBarcode+GcPdf", null);
            drawBarcode(CodeType.Code93x, "GcBarcode+GcPdf", null);
            drawBarcode(CodeType.Code_93, "GCBARCODE", null);
            drawBarcode(CodeType.Code_128_A, "GCPDF-2017", null);
            drawBarcode(CodeType.Code_128_B, "GcPdf-2017", null);
            drawBarcode(CodeType.Code_128_C, "1234567890", null);
            drawBarcode(CodeType.Code_128auto, "GcPdf-2017", null);
            drawBarcode(CodeType.Code_2_of_5, "1234567890", null);
            drawBarcode(CodeType.DataMatrix, "GcBarcode+GcPdf", null);
            drawBarcode(CodeType.QRCode, "GcBarcode+GcPdf", null);
            drawBarcode(CodeType.EAN_8, "1234567", null);
            drawBarcode(CodeType.EAN_13, "469" + "87654" + "3210", null);
            drawBarcode(CodeType.EAN128FNC1, "GcBarcode\nGcPdf", null);
            drawBarcode(CodeType.IntelligentMail, "00300999999000000001", null);
            drawBarcode(CodeType.JapanesePostal, "TOKYO-10CC-09-1978", null);
            drawBarcode(CodeType.PostNet, "152063949", null);
            drawBarcode(CodeType.RM4SCC, "SE17PB9Z", null);
            drawBarcode(CodeType.Matrix_2_of_5, "1234567890", null);
            drawBarcode(CodeType.MSI, "1234567890", null);
            drawBarcode(CodeType.MicroPDF417, "GcPdf", null);
            drawBarcode(CodeType.Pdf417, "GcPdf", null);
            drawBarcode(CodeType.RSS14, "1234567890", null);
            drawBarcode(CodeType.RSS14Stacked, "1234567890", null);
            drawBarcode(CodeType.RSS14Stacked, "1234567890", "12345");
            drawBarcode(CodeType.RSS14StackedOmnidirectional, "1234567890", null);
            drawBarcode(CodeType.RSS14Truncated, "1234567890", null);
            drawBarcode(CodeType.RSSExpanded, "12345678901234", null);
            drawBarcode(CodeType.RSSExpandedStacked, "12345678901234", null);
            drawBarcode(CodeType.RSSLimited, "1234567890", null);
            drawBarcode(CodeType.RSSLimited, "1234567890", "12345");
            drawBarcode(CodeType.UCCEAN128, "GcBarcode+GcPdf", null);
            drawBarcode(CodeType.UPC_A, "123456789012", null);
            drawBarcode(CodeType.UPC_E0, "123456789012", null);
            drawBarcode(CodeType.UPC_E1, "123456789012", null);
            //
            doc.Save(stream);
        }
    }
}
