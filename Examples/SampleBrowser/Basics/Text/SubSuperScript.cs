//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // This sample shows how to render subscript and superscript text.
    public class SubSuperScript
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;

            var rc = Common.Util.AddNote(
                "Demo of the TextFormat.Subscript and TextFormat.Superscript properties. " +
                "We draw a random 'lorem ipsum' paragraph, rendering all instances of 'lorem' as subscript, " +
                "and all instances of 'ipsum' as superscript.",
                page);

            // Get a random 'lorem ipsum' paragraph:
            var para = Common.Util.LoremIpsum(1, 18, 20, 20, 20);

            // Split the paragraph into 'lorem', 'ipsum' and everything else:
            const string sub = "lorem";
            const string super = "ipsum";
            var frags = Regex.Split(para, $"({sub})|({super})");

            var font = Font.FromFile(Path.Combine("Resources", "Fonts", "times.ttf"));

            // Create text formats for subscript and superscript:
            var tfSub = new TextFormat() { Font = font, FontSize = 12, Subscript = true };
            var tfSuper = new TextFormat(tfSub) { Subscript = false, Superscript = true };

            // Add text to a TextLayout using special formats for 'lorem' and 'ipsum':
            var tl = g.CreateTextLayout();
            tl.DefaultFormat.Font = font;
            tl.DefaultFormat.FontSize = 12;
            foreach (var frag in frags)
            {
                if (frag == sub)
                    tl.Append(frag, tfSub);
                else if (frag == super)
                    tl.Append(frag, tfSuper);
                else
                    tl.Append(frag);
            }

            // Set layout properties and render the text:
            tl.MaxWidth = page.Size.Width;
            tl.MaxHeight = page.Size.Height - rc.Height;
            tl.MarginAll = 72;
            tl.MarginTop = rc.Bottom + 36;
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, PointF.Empty);

            // Done:
            doc.Save(stream);
        }
    }
}
