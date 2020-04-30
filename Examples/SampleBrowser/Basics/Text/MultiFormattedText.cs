//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // This sample shows how to use different text formats
    // (fonts, colors) in a single paragraph.
    public class MultiFormattedText
    {
        public void CreatePDF(Stream stream)
        {
            // Function to generate sample text quoting its formatting options:
            Func<TextFormat, string> makeSampleText = (tf_) =>
            {
                string boldItalic = string.Empty;
                if (tf_.Font.FontBold)
                    boldItalic = "bold ";
                if (tf_.Font.FontItalic)
                    boldItalic += "italic ";
                if (boldItalic == string.Empty)
                    boldItalic = "normal ";
                return $"This is {boldItalic}text drawn using font '{tf_.Font.FullFontName}', font size {tf_.FontSize} points, " +
                    $"text color {tf_.ForeColor}, background color {tf_.BackColor}. ";
            };
            // Font names:
            const string times = "times new roman";
            const string arial = "arial";
            // Create document and text layout:
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;
            var tl = g.CreateTextLayout();
            // Use TextLayout to layout the whole page and maintain margins:
            tl.MaxHeight = page.Size.Height;
            tl.MaxWidth = page.Size.Width;
            tl.MarginAll = 72;
            // Get some fonts:
            var fc = new FontCollection();
            fc.RegisterDirectory(Path.Combine("Resources", "Fonts"));
            var fTimes = fc.FindFamilyName(times, false, false);
            var fTimesBold = fc.FindFamilyName(times, true, false);
            var fTimesItalic = fc.FindFamilyName(times, false, true);
            var fTimesBoldItalic = fc.FindFamilyName(times, true, true);
            var fArial = fc.FindFamilyName(arial, false, false);
            // Add text to TextLayout using different fonts and font sizes:
            TextFormat tf = new TextFormat() { Font = fTimes, FontSize = 12, };
            tl.Append(makeSampleText(tf), tf);
            tf.Font = fTimesBold;
            tf.FontSize += 2;
            tl.Append(makeSampleText(tf), tf);
            tf.Font = fTimesItalic;
            tf.FontSize += 2;
            tl.Append(makeSampleText(tf), tf);
            tf.Font = fTimesBoldItalic;
            tf.FontSize += 2;
            tl.Append(makeSampleText(tf), tf);
            tf.Font = fArial;
            tf.FontSize += 2;
            tl.Append(makeSampleText(tf), tf);
            // Add text with different foreground and background colors:
            tf.Font = fTimesBold;
            tf.ForeColor = Color.Tomato;
            tl.Append(makeSampleText(tf), tf);
            tf.Font = fTimesBoldItalic;
            tf.FontSize = 16;
            tf.ForeColor = Color.SlateBlue;
            tf.BackColor = Color.Orange;
            tl.Append(makeSampleText(tf), tf);
            // Finish with plain black on transparent again:
            tl.Append("The end.", new TextFormat() { Font = fTimes, FontSize = 14, });
            // Layout and draw text:
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, PointF.Empty);
            // Done:
            doc.Save(stream);
        }
    }
}
