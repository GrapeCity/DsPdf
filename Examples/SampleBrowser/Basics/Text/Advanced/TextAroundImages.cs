//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;

namespace GcPdfWeb.Samples.Basics
{
    // This sample shows how to flow a large block of text around rectangular areas,
    // in this case images. It also demonstrates how to get the actual bounds
    // of an image that has been rendered on a page using a specific ImageAlign.
    public class TextAroundImages
    {
        public int CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var g = doc.NewPage().Graphics;
            //
            // We want to draw 3 images in certain arbitrary locations on the first page, 
            // and then print a text that would take 2-3 pages, and have it flow around
            // the images on the first page.
            //
            // Get the images and their rectangles. Note that we specify a square
            // area for all images - but they will be aligned within that area
            // preserving their original aspect ratios, so we will later retrieve
            // the actual rectangles where the images were drawn:
            using (var imgPuffins = Image.FromFile("Resources/Images/puffins.jpg"))
            using (var imgReds = Image.FromFile("Resources/Images/reds.jpg"))
            using (var imgLavender = Image.FromFile("Resources/Images/lavender.jpg"))
            {
                var rectPuffins = new RectangleF(100, 70, 180, 180);
                var rectReds = new RectangleF(300, 280, 180, 180);
                var rectLavender = new RectangleF(190, 510, 180, 180);
                // Set up ImageAlign that would fit and center an image within a specified area,
                // preserving the image's original aspect ratio:
                ImageAlign ia = new ImageAlign(ImageAlignHorz.Center, ImageAlignVert.Center, true, true, true, false, false);
                // Draw each image, providing an array of rectangles as an output parameter for each DrawImage call,
                // so that we get the actual rectangle taken by the image (an array is needed to handle tiled images):
                g.DrawImage(imgPuffins, rectPuffins, null, ia, out RectangleF[] rectsPuffins);
                g.DrawImage(imgReds, rectReds, null, ia, out RectangleF[] rectsReds);
                g.DrawImage(imgLavender, rectLavender, null, ia, out RectangleF[] rectsLavender);
                // Create and set up a TextLayout object to print the text:
                var tl = g.CreateTextLayout();
                tl.DefaultFormat.Font = StandardFonts.Times;
                tl.DefaultFormat.FontSize = 9;
                tl.TextAlignment = TextAlignment.Justified;
                tl.ParagraphSpacing = 72 / 8;
                tl.MaxWidth = doc.PageSize.Width;
                tl.MaxHeight = doc.PageSize.Height;
                // 1/2" margins all around
                tl.MarginAll = 72 / 2;
                // ObjectRect is the type used to specify the areas to flow around to TextLayout.
                // We set up a local function to create an ObjecRect based on an image rectangle,
                // adding some padding so that the result looks nicer:
                Func<RectangleF, ObjectRect> makeObjectRect = rect_ =>
                    new ObjectRect(rect_.X - 6, rect_.Y - 2, rect_.Width + 12, rect_.Height + 4);
                // Specify the array of ObjectRects on the TextLayout:
                tl.ObjectRects = new List<ObjectRect>()
                {
                    makeObjectRect(rectsPuffins[0]),
                    makeObjectRect(rectsReds[0]),
                    makeObjectRect(rectsLavender[0]),
                };
                // Add several paragraphs of text:
                tl.Append(Common.Util.LoremIpsum(7, 5, 6, 28, 32));
                // Calculate glyphs and lay out the text:
                tl.PerformLayout(true);
                // Split options to control splitting of text between pages.
                // We can either use the default ctor and set up values like MaxWidth etc,
                // or create a TextSplitOptions based on the TextLayout, and clear RestObjectRects:
                TextSplitOptions to = new TextSplitOptions(tl)
                {
                    RestObjectRects = null,
                    MinLinesInFirstParagraph = 2,
                    MinLinesInLastParagraph = 2
                };
                // In a loop, split and render the text:
                while (true)
                {
                    // 'rest' will accept the text that did not fit:
                    var splitResult = tl.Split(to, out TextLayout rest);
                    doc.Pages.Last.Graphics.DrawTextLayout(tl, PointF.Empty);
                    if (splitResult != SplitResult.Split)
                        break;
                    tl = rest;
                    // We only draw images on the first page:
                    tl.ObjectRects = null;
                    doc.Pages.Add();
                }
                // Done:
                doc.Save(stream);
                return doc.Pages.Count;
            }
        }
    }
}
