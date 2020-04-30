//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;

namespace GcPdfWeb.Samples.Basics
{
    // This sample demonstrates the basics of rendering text in GcPdf.
    // The two main approaches are:
    // - using the MeasureString/DrawString pair, or
    // - using the TextLayout directly.
    // While the first approach may be easier in simple cases,
    // the second approach (using TextLayout) is much more powerful
    // and generally speaking yields better performance.
    // Please read the comments in code below for more details.
    // See also CharacterFormatting, PaginatedText, ParagraphAlign,
    // ParagraphFormatting, TextAlign.
    public class TextRendering
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;
            // By default, GcPdf uses 72dpi:
            const float In = 72;

            // TextFormat class is used throughout all GcPdf text rendering to specify
            // font and other character formatting:
            var tf = new TextFormat() { Font = StandardFonts.Times, FontSize = 12 };

            // 1.
            // The easiest way to render a short string on a page at an arbitrary location,
            // when you are 100% sure that the string will fit in the available space,
            // is to use the GcGraphics.DrawString() overload accepting jus the point
            // at which to draw the string:
            g.DrawString(
                "1. Test string. Please read the extensive comments in this sample's code.\r\n" +
                "(Note that line breaks are allowed even in the simplest DrawString overload.)",
                tf, new PointF(In, In));

            // 2.
            // Another overload taking a rectangle instead, plus alignment and wrapping
            // options, is also available and provides a bit more flexibility.
            // The parameters are:
            // - the text string;
            // - the text format;
            // - the layout rectangle;
            // - (optional) text alignment (the default is leading, left for LTR languages);
            // - (optional) paragraph alignment (the default is near, top for top-to-bottom flow);
            // - (optional) word wrapping (the default is true):
            g.DrawString(
                "2. A longer test string which will probably need more than the allocated " +
                "4 inches so quite possibly will wrap to show that DrawString can do that.", 
                tf,
                new RectangleF(In, In * 2, In * 4, In),
                TextAlignment.Leading,
                ParagraphAlignment.Near,
                true);

            // 3.
            // Complementary to DrawString, a MeasureString() method is available
            // (with several different overloads), and can be used in pair with
            // DrawString when more control over text layout is needed:
            const string tstr3 = "3. Test string to demo MeasureString() used with DrawString().";
            // Available layout size:
            SizeF layoutSize = new SizeF(In * 3, In * 0.8f);
            SizeF s = g.MeasureString(tstr3, tf, layoutSize, out int fitCharCount);
            // Show the passed in size in red, the measured size in blue,
            // and draw the string within the returned size as bounds:
            PointF pt = new PointF(In, In * 3);
            g.DrawRectangle(new RectangleF(pt, layoutSize), Color.Red);
            g.DrawRectangle(new RectangleF(pt, s), Color.Blue);
            g.DrawString(tstr3, tf, new RectangleF(pt, s));

            // 4.
            // A much more powerful and with better performance, way to render text
            // is to use TextLayout. (TextLayout is used anyway by DrawString/MeasureString,
            // so when you use TextLayout directly, you basically cut the work in half.)
            // A TextLayout instance represents one or more paragraphs of text, with 
            // the same paragraph formatting (character formats may be different,
            // see MultiFormattedText).
            var tl = g.CreateTextLayout();
            // To add text, use Append() or AppendLine() methods:
            tl.Append("4. First test string added to TextLayout. ", tf);
            tl.Append("Second test string added to TextLayout, continuing the same paragraph. ", tf);
            // Add a line break, effectively starting a new paragraph:
            tl.AppendLine();
            tl.Append("Third test string added to TextLayout, a new paragraph. ", tf);
            tl.Append("Fourth test string, with a different char formatting. ",
                new TextFormat(tf) { Font = StandardFonts.TimesBoldItalic, ForeColor = Color.DarkSeaGreen, });
            // Text can be added to TextLayout without explicit TextFormat:
            tl.Append("Fifth test string, using the TextLayout's default format.");
            // ...but in that case at least the Font must be specified on the
            // TextLayout's DefaultFormat, otherwise PerformLayout (below) will fail:
            tl.DefaultFormat.Font = StandardFonts.TimesItalic;
            // Specify the layout, such as max available size etc.
            // Here we only provide the max width, but many more parameters can be set:
            tl.MaxWidth = page.Size.Width - In * 2;
            // Paragraph formatting can also be set, here we set first line offset,
            // spacing between paragraphs and line spacing:
            tl.FirstLineIndent = In * 0.5f;
            tl.ParagraphSpacing = In * 0.05f;
            tl.LineSpacingScaleFactor = 0.8f;

            // When all text has been added, and layout options specified,
            // the TextLayout needs to calculate the glyphs needed to render
            // the text, and perform the layout. This can be done with a 
            // single call:
            tl.PerformLayout(true);

            // Now we can draw it on the page:
            pt = new PointF(In, In * 4);
            g.DrawTextLayout(tl, pt);
            // TextLayout provides info about the text including the measured bounds
            // and much more. Here we draw the bounding box in orange red:
            g.DrawRectangle(new RectangleF(pt, tl.ContentRectangle.Size), Color.OrangeRed);

            // 5.
            // TextLayout can be re-used to draw different paragraph(s), this can be useful
            // when you need to render a different text with the same paragraph formatting.
            // The Clear() call removes the text but preserves paragraph formatting:
            tl.Clear();
            tl.Append("5. This is text rendered re-using the same TextLayout. ");
            tl.Append("More text added to TextLayout being re-used, continuing the same paragraph. ", tf);
            tl.Append("And finally, some more text added.", tf);
            // The necessary call to calculate the glyphs and perform layout:
            tl.PerformLayout(true);
            // Render the text:
            g.DrawTextLayout(tl, new PointF(In, In * 5));

            // Done:
            doc.Save(stream);
        }
    }
}
