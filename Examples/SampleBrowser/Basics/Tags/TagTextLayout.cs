//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Pdf.Structure;
using GrapeCity.Documents.Pdf.MarkedContent;

namespace GcPdfWeb.Samples.Basics
{
    // This sample shows how to create tagged (structured) PDF and attach
    // tags to individual paragraphs in a TextLayout that is used to render
    // them together, splitting between pages.
    // The code generating the document is similar to that used in PaginatedText,
    // but adds tags.
    // To see/explore the tags, open the document in Adobe Acrobat Pro and go to
    // View | Navigation Panels | Tags.
    public class TagTextLayout
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();

            // Create a Part element, it will contain P (paragraph) elements:
            StructElement sePart = new StructElement("Part");
            doc.StructTreeRoot.Children.Add(sePart);

            // Create and set up a TextLayout to render paragraphs:
            var tl = new TextLayout(72);
            tl.DefaultFormat.Font = StandardFonts.Times;
            tl.DefaultFormat.FontSize = 12;
            tl.FirstLineIndent = 72 / 2;
            tl.MaxWidth = doc.PageSize.Width;
            tl.MaxHeight = doc.PageSize.Height;
            tl.MarginAll = tl.Resolution;
            //
            // Append the text (20 paragraphs so they would not fit on a single page)
            // (note that TextLayout interprets "\r\n" as paragraph delimiter):
            //
            // Get the text (20 paragraphs):
            var text = Common.Util.LoremIpsum(20);
            // In order to tag the individual paragraphs, we need to split the text into paragraphs,
            // and use each paragraph format's Tag property (which is not related to PDF tags, 
            // it is just an arbitrary data that can be associated with a TextFormat) to add the
            // paragraph's index to the paragraph:
            var pars = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < pars.Length; ++i)
            {
                var tf = new TextFormat(tl.DefaultFormat) { Tag = i };
                tl.AppendLine(pars[i], tf);
            }

            // Layout the text:
            tl.PerformLayout(true);
            // Use split options to provide widow/orphan control:
            TextSplitOptions to = new TextSplitOptions(tl)
            {
                MinLinesInFirstParagraph = 2,
                MinLinesInLastParagraph = 2,
            };
            // TextLayoutHandler implements ITextLayoutHandler, which
            // allows to tag the text as it is rendered:
            TextLayoutHandler tlh = new TextLayoutHandler() { ParentElement = sePart };

            // In a loop, split and render the text:
            while (true)
            {
                // 'rest' will accept the text that did not fit:
                var splitResult = tl.Split(to, out TextLayout rest);
                var page = doc.Pages.Add();
                var g = page.Graphics;
                // Tell the TextLayoutHandler which page we're on:
                tlh.Page = page;
                // ..and associate it with the graphics:
                g.TextLayoutHandler = tlh;
                // Draw the text that fits on the current page, and advance to next page unless we're done:
                g.DrawTextLayout(tl, PointF.Empty);
                if (splitResult != SplitResult.Split)
                    break;
                tl = rest;
            }
            // Mark document as tagged:
            doc.MarkInfo.Marked = true;

            // Done:
            doc.Save(stream);
        }

        // Custom class that allows to tag content as it is rendered by TextLayout:
        private class TextLayoutHandler : ITextLayoutHandler
        {
            private int _tagIndex;
            private int _currentParagraphIndex = -1;
            private StructElement _currentparagraphElement;
            public StructElement ParentElement;
            public Page Page;

            public void TextTagBegin(GcPdfGraphics graphics, TextLayout textLayout, object tag)
            {
                int paragraphIndex;
                if (tag is int)
                    paragraphIndex = (int)tag;
                else
                    paragraphIndex = -1;

                StructElement paragraphElement;
                if (_currentParagraphIndex == paragraphIndex)
                {
                    paragraphElement = _currentparagraphElement;
                }
                else
                {
                    if (paragraphIndex >= 0)
                    {
                        paragraphElement = new StructElement("P");
                        ParentElement.Children.Add(paragraphElement);
                        _currentparagraphElement = paragraphElement;
                        _currentParagraphIndex = paragraphIndex;
                    }
                    else
                    {
                        paragraphElement = null;
                        _currentparagraphElement = paragraphElement;
                        _currentParagraphIndex = paragraphIndex;
                    }
                }

                //
                if (paragraphElement != null)
                {
                    graphics.BeginMarkedContent(new TagMcid("P", _tagIndex));
                    McrContentItemLink mcil = new McrContentItemLink();
                    mcil.MCID = _tagIndex;
                    mcil.Page = Page;
                    paragraphElement.ContentItems.Add(mcil);
                    _tagIndex++;
                }
            }

            public void TextTagEnd(GcPdfGraphics graphics, TextLayout textLayout, object tag)
            {
                if (_currentparagraphElement != null)
                    graphics.EndMarkedContent();
            }

            public void AddTextArea(RectangleF bounds)
            {
            }
        }
    }
}
