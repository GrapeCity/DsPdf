using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Pdf.Annotations;

namespace GcPdfWeb.Samples.Basics
{
    // Shows how to add annotations to a PDF document.
    public class Annotations
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;
            // User names for annotations' authors:
            var user1 = "Jaime Smith";
            var user2 = "Jane Donahue";

            TextFormat tf = new TextFormat() { Font = StandardFonts.Helvetica, FontSize = 10 };
            var noteWidth = 72 * 4;
            var gap = 8;

            var rc = Common.Util.AddNote(
                "This sample demonstrates some types of annotations that can be created with GcPdf.\r\n" +
                "Note that some annotation types may not display in certain viewers (such as built-in browser viewers). " +
                "To see all annotations on this page, open it in Acrobat Reader or other full-featured PDF viewer.",
                page);

            // Text annotation:
            var ip = new PointF(rc.X, rc.Bottom + gap);
            rc = Common.Util.AddNote("A red text annotation is placed to the right of this note.", page, new RectangleF(ip.X, ip.Y, noteWidth, 100));
            var textAnnot = new TextAnnotation()
            {
                UserName = user1,
                Contents = "This is annotation 1, a red one.",
                Rect = new RectangleF(rc.Right, rc.Top, 72 * 2, 72),
                Color = Color.Red,
            };
            page.Annotations.Add(textAnnot);
            // A reply to previous annotation:
            var textAnnotReply = new TextAnnotation()
            {
                UserName = user2,
                Contents = "This is a reply to the first annotation.",
                Rect = new RectangleF(rc.Right, rc.Top, 72 * 2, 72),
                ReferenceAnnotation = textAnnot,
                ReferenceType = AnnotationReferenceType.Reply,
            };
            page.Annotations.Add(textAnnotReply);

            // An initially open text annotation:
            ip = new PointF(rc.X, rc.Bottom + gap);
            rc = Common.Util.AddNote("A green text annotation that is initially open is placed to the right of this note.", page, new RectangleF(ip.X, ip.Y, noteWidth, 100));
            var textAnnotOpen = new TextAnnotation()
            {
                Open = true,
                UserName = user1,
                Contents = "This is an initially open annotation (green).",
                Rect = new RectangleF(rc.Right, rc.Top, 72 * 2, 72),
                Color = Color.Green,
            };
            page.Annotations.Add(textAnnotOpen);

            // A free text annotation (shows directly on page):
            ip = new PointF(rc.X, rc.Bottom + gap);
            rc = Common.Util.AddNote("A blue free text annotation is placed below and to the right, with a callout going from it to this note.",
                page, new RectangleF(ip.X, ip.Y, noteWidth, 100));
            var freeAnnot = new FreeTextAnnotation()
            {
                Rect = new RectangleF(rc.Right + 18, rc.Bottom + 9, 72 * 2, 72),
                CalloutLine = new PointF[]
                {
                    new PointF(rc.Left + rc.Width / 2, rc.Bottom),
                    new PointF(rc.Left + rc.Width / 2, rc.Bottom + 9 + 36),
                    new PointF(rc.Right + 18, rc.Bottom + 9 + 36),
                },
                LineWidth = 1,
                LineEndStyle = LineEndingStyle.OpenArrow,
                LineDashPattern = new float[] { 8, 4 },
                Contents = "This is a free text annotation with a callout line going to the note on the left.",
                Color = Color.LightSkyBlue,
            };
            page.Annotations.Add(freeAnnot);

            // Another free text annotation, with some rich text inside:
            ip = new PointF(rc.X, freeAnnot.Rect.Bottom + gap);
            var freeRichAnnot = new FreeTextAnnotation()
            {
                Rect = new RectangleF(ip.X - 144, ip.Y, 72 * 4, 72),
                LineWidth = 1,
                Color = Color.LightSalmon,
                RichText =
                    "<body><p>This is another <i>free text annotation</i>, with <b><i>Rich Text</i></b> content.</p>" +
                    "<p><br />Even though a <b>free text</b> annotation displays text directly on a page, " +
                    "as other annotations it can be placed outside the page's bounds.</p></body>",
            };
            page.Annotations.Add(freeRichAnnot);

            // A square annotatou around a note:
            ip = new PointF(rc.X, freeRichAnnot.Rect.Bottom + gap * 2);
            rc = Common.Util.AddNote("A square annotation drawn with a 3pt wide orange line around this note has a rich text associated with it.",
                page, new RectangleF(ip.X, ip.Y, noteWidth, 100));

            rc.Inflate(8, 8);
            var squareAnnot = new SquareAnnotation()
            {
                UserName = user2,
                Rect = rc,
                LineWidth = 3,
                Color = Color.Orange,
                RichText =
                    "<body><p>This <b><i>rich text</i></b> is associated with the square annotation around a text note.</p></body>",
            };
            page.Annotations.Add(squareAnnot);

            // Done:
            doc.Save(stream);
        }
    }
}
