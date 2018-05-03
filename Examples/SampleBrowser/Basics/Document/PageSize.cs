using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Common;
using GrapeCity.Documents.Drawing;

namespace GcPdfWeb.Samples.Basics
{
    // Shows how to change page size and orientation in GcPdf.
    public class PageSize
    {
        public void CreatePDF(Stream stream)
        {
            const float in2mm = 72f / 25.4f;
            var colorOrig = Color.Red;
            var colorNew = Color.Blue;
            var doc = new GcPdfDocument();
            // The default page size is Letter (8 1/2" x 11") with portrait orientation:
            var page = doc.NewPage();
            var sOrigPageInfo = $"Original page size: {page.Size} pts ({page.Size.Width / 72f}\" * {page.Size.Height / 72f}\"),\r\npaper kind: {page.PaperKind}, landscape: {page.Landscape}.";
            // Save original page bounds:
            var rOrig = page.Bounds;
            // Change page parameters:
            page.Landscape = true;
            page.PaperKind = PaperKind.A4;
            var sNewPageInfo = $"New page size: {page.Size} pts ({page.Size.Width / in2mm}mm * {page.Size.Height / in2mm}mm),\r\npaper kind: {page.PaperKind}, landscape: {page.Landscape}.";
            // New page bounds:
            var rNew = page.Bounds;
            // Draw original and new page bounds:
            page.Graphics.DrawRectangle(rOrig, colorOrig, 6);
            page.Graphics.DrawRectangle(rNew, colorNew, 6);
            // Draw original and new page infos:
            TextFormat tf = new TextFormat();
            tf.Font = StandardFonts.Times;
            tf.FontSize = 14;
            tf.ForeColor = colorOrig;
            page.Graphics.DrawString(sOrigPageInfo, tf, new PointF(72, 72));
            tf.ForeColor = colorNew;
            page.Graphics.DrawString(sNewPageInfo, tf, new PointF(72, 72 * 2));
            // Done:
            doc.Save(stream);
        }
    }
}
