using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // Demonstrates the basics of character formatting in GcPdf.
    // 
    // Character formatting in GcPdf is done via GrapeCity.Documents.Text.TextFormat class.
    // An instance of that class with the required formatting options
    // is passed to most text rendering methods available in GcPdf (e.g. DrawString).
    // Rendering text with different character formatting in the same paragraph
    // is done by using TextLayout/DrawTextLayout.
    // See also TextRendering, MultiFormattedText, ParagraphAlign,
    // ParagraphFormatting, TextAlign.
    public class CharacterFormatting
    {
        public int CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;
            const float In = 72f, vStep = In / 2;
            PointF ip = new PointF(In, In);

            // 1. The only mandatory property that must be set on a TextFormat is Font:
            TextFormat tf = new TextFormat() { Font = StandardFonts.Times };

            g.DrawString(
                "1. The only mandatory property that must always be set on a TextFormat is Font.\r\n" +
                "Even FontSize is optional, and defaults to 12pts.", 
                tf, ip);
            ip.Y += vStep * 2;

            // 2. Standard font properties are available:
            tf.Underline = true;
            tf.Strikethrough = true;
            tf.FontSize = 10;
            g.DrawString(
                "2. Standard properties are available, here we turn Underline and Strikethrough on, and set FontSize to 10.",
                tf, ip);
            ip.Y += vStep;

            // 3. TextFormat.FontStyle allows to emulate bold and/or italic styles
            // using a regular font (see also BoldItalicEmulation):
            tf.Underline = tf.Strikethrough = false;
            tf.FontStyle = FontStyle.BoldItalic;
            tf.FontSize = 12;
            g.DrawString(
                "3. Using TextFormat.FontStyle.BoldItalic to emulate bold italic style.",
                tf, ip);
            ip.Y += vStep;

            // 4. Other properties include foreground and background colors:
            tf.FontStyle = FontStyle.Regular;
            tf.ForeColor = Color.DarkSlateBlue;
            tf.BackColor = Color.PaleGreen;
            g.DrawString(
                "4. Using TextFormat.ForeColor and TextFormat.BackColor to colorize the text.",
                tf, ip);
            ip.Y += vStep;

            // 5. Different text formats may be mixed in the same paragraph.
            // For that, TextLayout and GcPdfGraphics.DrawTextLayout must be used:
            TextLayout tl = new TextLayout();
            tl.Append(
                "5. Different text formats can be easily mixed in the same paragraph ",
                new TextFormat() { Font = StandardFonts.Times });
            tl.Append(
                "when the paragraph is built with TextLayout",
                new TextFormat() { Font = StandardFonts.TimesBold, BackColor = Color.PaleTurquoise });
            tl.Append(
                " as this sample paragraph shows. ",
                new TextFormat() { Font = StandardFonts.HelveticaBoldItalic, ForeColor = Color.DarkOrange });
            tl.Append(
                "Various other options are available on TextFormat, including ",
                new TextFormat() { Font = StandardFonts.Times, ForeColor = Color.DarkSlateBlue });
            tl.Append(
                "GlyphAdvanceFactor",
                new TextFormat() { Font = StandardFonts.TimesBoldItalic, Underline = true });
            tl.Append(
                " (spreading glyphs out ",
                new TextFormat() { Font = StandardFonts.Times, GlyphAdvanceFactor = 1.5f, ForeColor = Color.BlueViolet });
            tl.Append(
                "or putting them closer together),  ",
                new TextFormat() { Font = StandardFonts.Times, GlyphAdvanceFactor = 0.8f, ForeColor = Color.BlueViolet });
            tl.Append(
                "TransverseOffset",
                new TextFormat() { Font = StandardFonts.TimesBoldItalic, Underline = true });
            tl.Append(
                " (lowering the glyphs below the base line, ",
                new TextFormat() { Font = StandardFonts.Times, TransverseOffset = -5, ForeColor = Color.MediumVioletRed });
            tl.Append(
                "or raising them above it)",
                new TextFormat() { Font = StandardFonts.Times, TransverseOffset = 5, ForeColor = Color.MediumVioletRed });
            tl.Append(
                " and more (for example, specific fonts' features are accessible via TextFormat.FontFeatures).",
                new TextFormat() { Font = StandardFonts.Times, FontFeatures = new FontFeature[] { new FontFeature(FeatureTag.clig) } });

            // For this sample, we just set the max width of the text layout,
            // in a real app you would probably set at least the MaxHeight too:
            tl.MaxWidth = page.Size.Width - In * 2;
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, ip);
            // Done:
            doc.Save(stream);
            return doc.Pages.Count;
        }
    }
}
