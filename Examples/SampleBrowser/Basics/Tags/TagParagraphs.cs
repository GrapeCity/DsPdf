using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Pdf.Structure;
using GrapeCity.Documents.Pdf.MarkedContent;

namespace GcPdfWeb.Samples.Basics
{
    // This sample shows how to create tagged (structured) PDF.
    // To see/explore the tags, open the document in Adobe Acrobat Pro and go to
    // View | Navigation Panels | Tags.
    public class TagParagraphs
    {
        public int CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var rnd = Common.Util.NewRandom();
            int pageCount = rnd.Next(3, 7);

            // Create Part element, it will contain P (paragraph) elements
            StructElement sePart = new StructElement("Part");
            doc.StructTreeRoot.Children.Add(sePart);

            // Add some pages, on each page add some paragraphs and tag them:
            for (int pageIndex = 0; pageIndex < pageCount; ++pageIndex)
            {
                // Add page:
                var page = doc.Pages.Add();
                var g = page.Graphics;
                const float margin = 36;
                const float dy = 18;

                // Add some paragraphs:
                int paraCount = rnd.Next(1, 5);
                float y = margin;
                for (int i = 0; i < paraCount; ++i)
                {
                    // Create paragraph element:
                    StructElement seParagraph = new StructElement("P") { DefaultPage = page };
                    // Add it to Part element:
                    sePart.Children.Add(seParagraph);

                    // Create paragraph:
                    var tl = g.CreateTextLayout();
                    tl.DefaultFormat.Font = StandardFonts.Helvetica;
                    tl.DefaultFormat.FontSize = 12;
                    tl.Append(Common.Util.LoremIpsum(1, 1, 5, 5, 10));
                    tl.MaxWidth = page.Size.Width;
                    tl.MarginLeft = tl.MarginRight = margin;
                    tl.PerformLayout(true);

                    // Draw TextLayout within tagged content:
                    g.BeginMarkedContent(new TagMcid("P", i));
                    g.DrawTextLayout(tl, new PointF(0, y));
                    g.EndMarkedContent();

                    y += tl.ContentHeight + dy;

                    // Add content item to paragraph StructElement:
                    seParagraph.ContentItems.Add(new McidContentItemLink(i));
                }
            }

            // Mark document as tagged:
            doc.MarkInfo.Marked = true;

            // Done:
            doc.Save(stream);
            return doc.Pages.Count;
        }
    }
}
