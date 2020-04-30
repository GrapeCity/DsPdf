//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Pdf.AcroForms;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Common;
using GrapeCity.Documents.Drawing;
using System.Numerics;
using GcPdfWeb.Samples.Common;

namespace GcPdfWeb.Samples
{
    // This sample generates a two-page data sheet about the new
    // GrapeCity Documents product line.
    public class GcDocsDataSheet
    {
        private FontCollection _fc = new FontCollection();

        Color _darkGray = Color.FromArgb(64, 64, 64);
        Color _lightGray = Color.FromArgb(232, 232, 232);
        Color _blue = Color.FromArgb(0x3B, 0x5C, 0xAA);
        List<IDisposable> _disposables = new List<IDisposable>();

        // Main entry point of this sample:
        public int CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();

            _fc.RegisterDirectory(Path.Combine("Resources", "Fonts"));
            // First page:
            Page1(doc);
            // Second page:
            Page2(doc);
            // Save the PDF:
            doc.Save(stream);
            // Dispose images after the document has been saved:
            _disposables.ForEach(d_ => d_.Dispose());
            // Done:
            return doc.Pages.Count;
        }

        IImage GetImage(string path)
        {
            var image = Util.ImageFromFile(path);
            _disposables.Add(image);
            return image;
        }

        void Page1(GcPdfDocument doc)
        {
            var page = doc.Pages.Add();
            var g = page.Graphics;
            var tl = new TextLayout(g.Resolution) { FontCollection = _fc };

            var gclogo = GetImage(Path.Combine("Resources", "ImagesBis", "gc-logo-100px.png"));
            var gclogoRc = new RectangleF(36, 0, 72 * 1.8f, 72);
            g.DrawImage(gclogo, gclogoRc, null,
                new ImageAlign(ImageAlignHorz.Left, ImageAlignVert.Center, true, true, true, false, false), out RectangleF[] rcs);
            g.DrawLine(rcs[0].Right + 10, rcs[0].Top, rcs[0].Right + 10, rcs[0].Bottom, _darkGray, 1);

            tl.Clear();
            tl.ParagraphAlignment = ParagraphAlignment.Center;
            tl.MaxHeight = gclogoRc.Height;
            tl.Append("Developer Solutions",
                new TextFormat() { FontName = "open sans", FontSize = 16, ForeColor = _darkGray });
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, new PointF(gclogoRc.Right + 20, gclogoRc.Y));

            var back = GetImage(Path.Combine("Resources", "ImagesBis", "GCDocs-datasheet-sm.png"));
            var backRcClip = new RectangleF(0, 72, page.Size.Width, page.Size.Width - 72 * 1.75f);
            var backRc = new RectangleF(-72, -72 * 4, page.Size.Width + 72 * 4, page.Size.Height + 72 * 4);
            g.DrawImage(back, backRc, backRcClip, ImageAlign.StretchImage);
            g.FillRectangle(new RectangleF(0, backRcClip.Bottom, page.Size.Width, page.Size.Height - backRcClip.Bottom), _lightGray);
            g.DrawLine(backRcClip.X, backRcClip.Bottom, backRcClip.Right, backRcClip.Bottom, Color.White, 1, null);
            g.DrawLine(backRcClip.X, backRcClip.Bottom + 1, backRcClip.Right, backRcClip.Bottom + 1, _darkGray, 1, null);

            var blueRc = new RectangleF(0, backRcClip.Y, page.Size.Width, 72 * 4);
            g.FillRectangle(blueRc, Color.FromArgb(220, _blue));

            blueRc.Inflate(0, -36);
            g.FillRectangle(new RectangleF(blueRc.Location, new SizeF(10, blueRc.Height)), Color.White);

            blueRc.Inflate(-36, 0);
            tl.Clear();
            tl.ParagraphAlignment = ParagraphAlignment.Near;
            tl.MaxWidth = blueRc.Width;
            tl.MaxHeight = blueRc.Height;
            tl.Append("NEW PRODUCT LINE",
                new TextFormat() { FontName = "open sans semibold", FontSize = 20, ForeColor = Color.White });
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, blueRc.Location);

            var midRc = new RectangleF(blueRc.X, blueRc.Y + tl.ContentHeight, blueRc.Width, blueRc.Height - tl.ContentHeight);

            tl.Clear();
            tl.ParagraphAlignment = ParagraphAlignment.Far;
            tl.Append(
                "Take total control of your digital documents with this NEW collection of ultra-fast, low-footprint document APIs for .NET Standard 2.0. These intuitive, extensible APIs " +
                "allow you to create, load, modify, and save Excel spreadsheets and PDF files in any .NET Standard 2.0 application",
                new TextFormat() { FontName = "open sans light", FontSize = 14, ForeColor = Color.White });
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, blueRc.Location);

            midRc.Height -= tl.ContentHeight;
            midRc.Inflate(0, -20);

            var hex = GetImage(Path.Combine("Resources", "ImagesBis", "gcd-hex-logo-white.png"));
            var hexRc = new RectangleF(midRc.Location, new SizeF(midRc.Height, midRc.Height));
            g.DrawImage(hex, hexRc, null, ImageAlign.StretchImage);

            tl.Clear();
            tl.ParagraphAlignment = ParagraphAlignment.Center;
            tl.MaxHeight = midRc.Height;
            tl.Append("GrapeCity Documents",
                new TextFormat() { FontName = "open sans semibold", FontSize = 26, ForeColor = Color.White });
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, new PointF(midRc.X + midRc.Height + 10, midRc.Y));

            var pointRc = new RectangleF(0, backRcClip.Bottom, page.Size.Width / 2, (page.Size.Height - backRcClip.Bottom) / 2 - 12);
            tl.ParagraphAlignment = ParagraphAlignment.Near;
            tl.MaxWidth = pointRc.Width;
            tl.MaxHeight = pointRc.Height;
            tl.MarginLeft = 80;
            tl.MarginTop = 25;
            tl.MarginBottom = 0;

            addPoint(GetImage(Path.Combine("Resources", "ImagesBis", "ico-hex-.NET.png")),
                "Expand the reach of modern apps",
                "With full support for .NET Standard 2.0, you can target multiple platforms, devices, and cloud with one code base.");

            pointRc.Offset(0, pointRc.Height);
            addPoint(GetImage(Path.Combine("Resources", "ImagesBis", "ico-hex-code.png")), 
                "Comprehensive, highly programmable",
                "Do more with your Excel spreadsheets and PDFs: these APIs support Windows, Mac, Linux, and a wide variety of features for your documents.");

            pointRc.Offset(pointRc.Width, -pointRc.Height);
            tl.MarginRight = 30;
            addPoint(GetImage(Path.Combine("Resources", "ImagesBis", "ico-hex-speed.png")), 
                "High-speed, small footprint",
                "The API architecture is designed to generate large, optimized documents, fast—while remaining lightweight and extensible.");

            pointRc.Offset(0, pointRc.Height);
            addPoint(GetImage(Path.Combine("Resources", "ImagesBis", "ico-hex-nodependences.png")), 
                "No dependencies",
                "Generate and edit digital documents with no Acrobat or Excel dependencies.");

            g.FillRectangle(new RectangleF(0, page.Size.Height - 16, page.Size.Width, 16), _darkGray);

            drawCircle(new PointF(page.Size.Width - 160, backRcClip.Bottom - 105));

            void addPoint(IImage img, string caption, string text)
            {
                var imgRc = new RectangleF(pointRc.X + 20, pointRc.Y + tl.MarginTop, 48, 48);
                g.DrawImage(img, imgRc, null, new ImageAlign() { AlignHorz = ImageAlignHorz.Center, AlignVert = ImageAlignVert.Center, BestFit = true });
                tl.Clear();
                tl.AppendLine(caption, new TextFormat() { FontName = "open sans semibold", FontSize = 11 });
                tl.Append(text, new TextFormat() { FontName = "open sans light", FontSize = 11 });
                tl.PerformLayout(true);
                if (!tl.ContentHeightFitsInBounds)
                    throw new Exception("Unexpected: text overflow.");
                g.DrawTextLayout(tl, pointRc.Location);
            }

            void drawCircle(PointF p)
            {
                float D = 128;
                float angle = (float)(16 * Math.PI) / 180f;
                g.Transform =
                    Matrix3x2.CreateTranslation(-D / 2, -D / 2) *
                    Matrix3x2.CreateRotation(angle) *
                    Matrix3x2.CreateTranslation(p.X + D / 2, p.Y + D / 2);

                var r = new RectangleF(PointF.Empty, new SizeF(D, D));
                for (int i = 0; i < 3; ++i)
                {
                    g.FillEllipse(r, Color.FromArgb(30 + i * 10, _darkGray));
                    r.Inflate(-1, -1);
                }
                g.FillEllipse(r, _darkGray);
                r.Inflate(-1, -1);
                g.FillEllipse(r, Color.White);
                r.Inflate(-6, -6);
                g.FillEllipse(r, _darkGray);

                tl.Clear();
                tl.MaxHeight = tl.MaxWidth = D;
                tl.MarginLeft = tl.MarginRight = tl.MarginTop = tl.MarginBottom = 0;
                tl.TextAlignment = TextAlignment.Center;
                tl.ParagraphAlignment = ParagraphAlignment.Center;
                tl.ParagraphSpacing = -4;
                var tf = new TextFormat() { FontName = "open sans light", FontSize = 18, ForeColor = Color.White };
                tl.Append("DEPLOY\nTO", tf);
                tl.Append(" AZURE\n", new TextFormat(tf) { FontName = "open sans semibold" });
                tl.Append(" ");
                tl.PerformLayout(true);
                g.DrawTextLayout(tl, PointF.Empty);

                g.Transform = Matrix3x2.Identity;
            }
        }

        void Page2(GcPdfDocument doc)
        {
            var page = doc.Pages.Add();
            var g = page.Graphics;
            var tl = new TextLayout(g.Resolution) { FontCollection = _fc };
            var col0X = 36;
            var colWidth = page.Size.Width / 3 - 40;
            var colGap = 20;
            var vgap = 10;
            var ser1Y = 100;
            var ser2Y = 72 * 6;
            var h = 45;

            List<(string caption, List<string> points)> gcpdf = new List<(string, List<string>)>()
            {
                ("Advanced Text Handling", new List<string> {
                    "Standard PDF Fonts, Truetype Fonts, Open type Fonts, WOFF Fonts, system font loading, font embedding, fallback and linked fonts, EUDC fonts",
                    "Advanced text rendering features",
                    "Special character support",
                }),
                ("Cross-Platform, Cross-Framework compatibility", new List<string>
                {
                    ".NET Standard 2.0",
                    ".NET Core 2.0",
                    ".NET Framework",
                    "Mono",
                    "Xamarin iOS",
                    "VSTO-style API"
                }),
                ("Security", new List<string>
                {
                    "Encryption and decrpyption",
                    "User and owner passwords",
                    "AllowCopyContent, AllowEditAnnotations, AllowEditContent, AllowPrint",
                    "Digital Signatures"
                }),
                ("Annotations", new List<string>
                {
                    "Figures",
                    "Comments",
                    "Text",
                    "Signatures",
                    "Stamps",
                    "Modify, extract or delete annotations from existing PDFs"
                }),
                ("Fillable Form Fields", new List<string>
                {
                    "Textbox",
                    "Checkbox",
                    "Combobox",
                    "Listbox",
                    "Radio button",
                    "Push button",
                    "Signature field",
                    "Modify, extract or delete form fields from existing PDFs"
                }),
                ("Navigation", new List<string>
                {
                    "Outlines",
                    "Hyperlinks"
                }),
                ("Additional Features", new List<string>
                {
                    "50 barcodes and properties",
                    "Create PDF/A files",
                    "Maintain document history with document properties",
                    "Generate linearized PDFs for fast web view",
                    "Full image and graphic support on all platforms",
                    "Add and delete pages",
                    "Chage page sizes",
                    "Page orientation"
                }),
            };

            List<(string caption, List<string> points)> gcexcel = new List<(string, List<string>)>()
            {
                ("Fast and Efficient", new List<string>
                {
                    "Lightweight",
                    "Optimized for processing large Excel documents quickly"
                }),
                ("Cross-Platform, Cross-Framework compatibility", new List<string>
                {
                    ".NET Standard 2.0",
                    ".NET Core 2.0",
                    ".NET Framework",
                    "Mono",
                    "Xamarin.iOS",
                    "VSTO-style API",
                }),
                ("Data Visualization", new List<string>
                {
                    "Shapes and pictures",
                    "Slicers",
                    "Sparklines",
                    "Charts",
                }),
                ("Powerful Calculation Engine", new List<string>
                {
                    "450+ Excel functions",
                    "Calculate",
                    "Query",
                    "Generate",
                    "Sorting",
                    "Filtering",
                    "Grouping",
                }),
                ("Seamless Excel Compatibility", new List<string>
                {
                    "Import and export Excel files",
                    "Export to PDF",
                    "Encrypt files",
                    "Workbooks and worksheets",
                    "Cell range operations",
                    "Pivot and Excel tables",
                    "Data validation",
                    "Annotations",
                    "Comments",
                }),
                ("Conditional Formatting Rules", new List<string>
                {
                    "Cell value",
                    "Average",
                    "Color scale",
                    "Data bar",
                    "Icon sets",
                    "Top and Bottom",
                    "Unique",
                    "Expression",
                }),
                ("Flexible Themes And Components", new List<string>
                {
                    "Customizable themes",
                    "Configurable components",
                    "Summary data",
                    "Custom styles",
                    "Embedded drawing objects",
                    "Integrated calculation engine",
                }),
            };

            addHeader(45,
                "GrapeCity Documents for PDF",
                "This high-speed, feature-rich PDF document API for .NET Standard 2.0 gives you total " +
                "control of your PDF documents, with no dependencies on Adobe Acrobat.Generate, " +
                "edit, and store feature - rich PDF documents without compromising design or features.");

            PointF ipt = new PointF(col0X, ser1Y);
            foreach (var (caption, points) in gcpdf)
            {
                var rc = addList(ipt, caption, points.ToArray());
                if (rc.Bottom < ser2Y - 120)
                    ipt = new PointF(rc.X, rc.Bottom + vgap);
                else
                    ipt = new PointF(rc.X + colWidth + colGap, ser1Y);
            }

            addHeader(ser2Y,
                "GrapeCity Documents for Excel",
                "Generate high-performance Excel spreadsheets with no dependencies on Excel! " +
                "Generate, convert, calculate, format, and parse spreadsheets in any app.");

            var topY = ser2Y + h + 10;
            ipt = new PointF(col0X, topY);
            foreach (var (caption, points) in gcexcel)
            {
                var rc = addList(ipt, caption, points.ToArray());
                if (rc.Bottom < page.Size.Height - 100)
                    ipt = new PointF(rc.X, rc.Bottom + vgap);
                else
                    ipt = new PointF(rc.X + colWidth + colGap, topY);
            }

            var hdrRc = new RectangleF(28, 0, page.Size.Width - 28 * 2, 36);
            g.FillRectangle(hdrRc, _darkGray);
            var w = hdrRc.Width / 7;
            string[] hdrs = new string[] { "Create", "Load", "Edit", "Save", "Analyze" };
            var hdrTf = new TextFormat() { FontName = "open sans", FontSize = 12, ForeColor = Color.White };
            var trc = new RectangleF(hdrRc.X + w, hdrRc.Y, w, hdrRc.Height);
            for (int i = 0; i < hdrs.Length; ++i)
            {
                g.DrawString(hdrs[i], hdrTf, trc, TextAlignment.Center, ParagraphAlignment.Center, false);
                if (i < hdrs.Length - 1)
                    g.DrawLine(trc.Right, trc.Top + 12, trc.Right, trc.Bottom - 12, Color.White, 1);
                trc.Offset(w, 0);
            }
            var ftrRc = new RectangleF(0, page.Size.Height - 36, page.Size.Width, 36);
            g.FillRectangle(ftrRc, _darkGray);
            var ftr0 = "GrapeCity.com";
            var ftr1 = "© 2018 GrapeCity, Inc.All rights reserved.All other product and brand names are trademarks and/or registered trademarks of their respective holders.";
            ftrRc.Inflate(-col0X, -5);
            hdrTf.FontSize = 12;
            g.DrawString(ftr0, hdrTf, ftrRc, TextAlignment.Leading, ParagraphAlignment.Near, false);
            hdrTf.FontSize = 6;
            g.DrawString(ftr1, hdrTf, ftrRc, TextAlignment.Leading, ParagraphAlignment.Far, false);
            ftrRc.Inflate(0, -5);
            g.DrawImage(GetImage(Path.Combine("Resources", "ImagesBis", "logo-GC-white.png")), ftrRc, null,
                new ImageAlign() { AlignHorz = ImageAlignHorz.Right, AlignVert = ImageAlignVert.Center, BestFit = true });

            void addHeader(float y, string caption, string text)
            {
                var bluerc = new RectangleF(0, y, 28, h);
                g.FillRectangle(bluerc, _blue);
                var caprc = new RectangleF(bluerc.Right, y, 72 * 2.75f, h);
                g.FillRectangle(caprc, _lightGray);
                caprc.X = col0X;
                caprc.Width -= col0X - bluerc.Width;
                g.DrawString(caption, new TextFormat() { FontName = "open sans semibold", FontSize = 12 }, caprc, TextAlignment.Leading, ParagraphAlignment.Center, false);
                var textrc = new RectangleF(caprc.Right, caprc.Top, page.Size.Width - caprc.Right, caprc.Height);
                textrc.Inflate(-10, 0);
                g.DrawString(text, new TextFormat() { FontName = "open sans light", FontSize = 9 }, textrc, TextAlignment.Leading, ParagraphAlignment.Center, true);
            }

            RectangleF addList(PointF pt, string caption, params string[] items)
            {
                var tf = new TextFormat() { FontName = "open sans light", FontSize = 9 };
                var ret = new RectangleF(pt, SizeF.Empty);
                tl.Clear();
                tl.MaxWidth = colWidth;
                tl.AppendLine(caption, new TextFormat() { FontName = "open sans", FontBold = true, FontSize = 9 });
                tl.PerformLayout(true);
                g.DrawTextLayout(tl, pt);
                ret.Width = tl.ContentWidth;
                ret.Height = tl.ContentHeight;
                pt.Y += ret.Height;
                tl.Clear();
                var itemPrefix = "\u2022  ";
                tl.FirstLineIndent = -g.MeasureStringWithTrailingWhitespace(itemPrefix, tf).Width;
                foreach (var item in items)
                    tl.AppendLine(itemPrefix + item, tf);
                tl.PerformLayout(true);
                g.DrawTextLayout(tl, pt);
                ret.Width = Math.Max(ret.Width, tl.ContentWidth);
                ret.Height += tl.ContentHeight;
                return ret;
            }
        }
    }
}
