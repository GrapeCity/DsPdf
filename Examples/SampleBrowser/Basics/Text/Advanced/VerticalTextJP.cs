using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // Draws vertical right-to-left Japanese text using a layout with horizontal columns.
    // See also ArabicColumns, MultiLang and VerticalText.
    public class VerticalTextJP
    {
        const string text = "日本語（にほんご、にっぽんご）は、主として、日本列島で使用されてきた言語である。日本手話を母語とする者などを除いて、ほぼ全ての日本在住者は日本語を第一言語とする。日本国は法令上、公用語を明記していないが、事実上の公用語となっており、学校教育の「国語」で教えられる。使用者は、日本国内を主として約\uFF11億\uFF13千万人。日本語の文法体系や音韻体系を反映する手話として日本語対応手話がある。";

        public void CreatePDF(Stream stream)
        {
            var yumin = Font.FromFile(Path.Combine("Resources", "Fonts", "yumin.ttf"));
            var clouds = Image.FromFile(Path.Combine("Resources", "Images", "clouds.jpg"));
            var firth = Image.FromFile(Path.Combine("Resources", "Images", "firth.jpg"));
            var lavender = Image.FromFile(Path.Combine("Resources", "Images", "lavender.jpg"));
            var ia = new ImageAlign(ImageAlignHorz.Left, ImageAlignVert.Top, true, true, true, false, false);

            var doc = new GcPdfDocument();

            // The TextLayout that will hold and render the text:
            var tl = new TextLayout();
            tl.FirstLineIndent = 18;
            tl.ParagraphSpacing = 6;
            tl.FlowDirection = FlowDirection.VerticalRightToLeft;
            tl.TextAlignment = TextAlignment.Justified;
            tl.AlignmentDelayToSplit = true;
            var tf = new TextFormat() { Font = yumin, FontSize = 12 };
            // Repeat test text to fill a few pages:
            for (int i = 0; i < 25; ++i)
            {
                tl.Append(text, tf);
                tl.AppendLine();
            }

            // Layout text in 4 horizontal columns:
            // (The logic/code in this sample is identical to ArabicColumns):
            const int NCOLS = 4;
            var margin = 36f;
            var gap = 18f;
            var page = doc.NewPage();
            page.Landscape = true;
            var colHeight = (page.Size.Height - margin * 2 - gap * (NCOLS - 1)) / NCOLS;
            tl.MaxWidth = page.Size.Width;
            tl.MaxHeight = page.Size.Height;
            tl.MarginLeft = tl.MarginRight = margin;
            tl.MarginTop = margin;
            tl.MarginBottom = margin + (colHeight + gap) * (NCOLS - 1);
            // We can specify arbitrary rectangles for the text to flow around.
            // In this case, we add 3 areas to draw some images:
            tl.ObjectRects = new List<ObjectRect>()
            {
                new ObjectRect(page.Size.Width - margin - 267, margin, 267, 200),
                new ObjectRect(margin + 100, margin + 60, 133, 100),
                new ObjectRect(margin, page.Size.Height - margin - 301, 200, 301),
            };
            // Convert object rects to image areas, ajust to provide nice looking padding:
            var rClouds = tl.ObjectRects[0].ToRectangleF();
            rClouds.Inflate(-4, -3);
            var rFirth = tl.ObjectRects[1].ToRectangleF();
            rFirth.Inflate(-4, -3);
            var rLavender = tl.ObjectRects[2].ToRectangleF();
            rLavender.Inflate(-4, -3);
            page.Graphics.DrawImage(clouds, rClouds, null, ia);
            page.Graphics.DrawImage(firth, rFirth, null, ia);
            page.Graphics.DrawImage(lavender, rLavender, null, ia);

            // THE call: it calculates the glyphs needed to draw the text, and lays it out:
            tl.PerformLayout(true);

            bool done = false;
            while (!done) // loop while there is still text to render
            {
                for (int col = 0; col < NCOLS; ++col)
                {
                    int nextcol = (col < NCOLS - 1) ? col + 1 : 0;
                    // TextSplitOptions tell TextLayout.Split() how to layout the remaining text.
                    // In this case we advance from column to column by updating top and bottom margins:
                    var tso = new TextSplitOptions(tl)
                    {
                        RestMarginTop = margin + (colHeight + gap) * nextcol,
                        RestMarginBottom = margin + (colHeight + gap) * (NCOLS - 1 - nextcol)
                    };
                    var split = tl.Split(tso, out TextLayout rest);
                    page.Graphics.DrawTextLayout(tl, PointF.Empty);
                    if (split != SplitResult.Split)
                    {
                        done = true;
                        break;
                    }
                    tl = rest;
                }
                if (!done)
                {
                    page = doc.NewPage();
                    page.Landscape = true;
                    // We only want to render images on the first page, so clear ObjectRect:
                    if (tl.ObjectRects != null)
                    {
                        tl.ObjectRects = null;
                        // We need to redo the layout, but no need to recalculate the glyphs:
                        tl.PerformLayout(false);
                    }
                }
            }
            // Done:
            doc.Save(stream);
        }
    }
}
