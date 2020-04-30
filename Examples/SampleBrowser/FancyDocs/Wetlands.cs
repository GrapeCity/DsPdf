//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;

namespace GcPdfWeb.Samples
{
    // This sample generates a text document with some images and text highlights,
    // using the TextLayout class to arrange text and images.
    public class Wetlands
    {
        // The main sample driver.
        public int CreatePDF(Stream stream)
        {
            GcPdfDocument doc = new GcPdfDocument();
            // This will hold the llst of images so we can dispose them after saving the document:
            List<IDisposable> disposables = new List<IDisposable>();

            // Page footer:
            var ftrImg = Image.FromFile(Path.Combine("Resources", "ImagesBis", "logo-GC-devsol.png"));
            disposables.Add(ftrImg);
            var fx = ftrImg.HorizontalResolution / 72f;
            var fy = ftrImg.VerticalResolution / 72f;
            var ftrRc = new RectangleF(
                doc.PageSize.Width / 2 - ftrImg.Width / fx / 2,
                doc.PageSize.Height - 40,
                ftrImg.Width / fx,
                ftrImg.Height / fy);

            // Color for the title:
            var colorBlue = Color.FromArgb(0x3B, 0x5C, 0xAA);
            // Color for the highlights:
            var colorRed = Color.Red;
            // The text layout used to render text:
            TextLayout tl = new TextLayout(72)
            {
                    MaxWidth = doc.PageSize.Width,
                    MaxHeight = doc.PageSize.Height,
                    MarginLeft = 72,
                    MarginRight = 72,
                    MarginTop = 72,
                    MarginBottom = 72,
            };
            tl.DefaultFormat.Font = Font.FromFile(Path.Combine("Resources", "Fonts", "segoeui.ttf"));
            tl.DefaultFormat.FontSize = 11;

            var page = doc.NewPage();
            addFtr();
            var g = page.Graphics;

            // Caption:
            tl.TextAlignment = TextAlignment.Center;
            tl.Append("Introduction\n", new TextFormat() { FontSize = 16, ForeColor = colorBlue });
            tl.Append("The Importance of Wetlands", new TextFormat() { FontSize = 13, ForeColor = colorBlue });
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, PointF.Empty);

            // Move below the caption for the first para:
            tl.MarginTop = tl.ContentHeight + 72 * 2;
            tl.Clear();
            tl.TextAlignment = TextAlignment.Leading;
            tl.ParagraphSpacing = 12;

            // For the first para we want a bigger initial letter, but no first line indent,
            // so we render it separately from the rest of the text:
            tl.Append(_paras[0].Substring(0, 1), new TextFormat(tl.DefaultFormat) { FontSize = 22 });
            addPara(_paras[0].Substring(1));
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, PointF.Empty);

            // Account for the first para, and set up the text layout
            // for the rest of the text (a TextLayout allows to render multiple paragraphs,
            // but they all must have the same paragraph format):
            tl.MarginTop = tl.ContentRectangle.Bottom;
            tl.Clear();
            tl.FirstLineIndent = 36;

            // Add remaining paragraphs:
            foreach (var para in _paras.Skip(1))
            {
                // Paragraphs starting with '::' indicate images to be rendered across the page width:
                if (para.StartsWith("::"))
                {
                    var img = Image.FromFile(Path.Combine("Resources", "ImagesBis", para.Substring(2)));
                    disposables.Add(img);
                    var w = tl.MaxWidth.Value - tl.MarginLeft - tl.MarginRight;
                    var h = (float)img.Height / (float)img.Width * w;
                    tl.AppendInlineObject(img, w, h);
                    tl.AppendLine();
                }
                else
                {
                    addPara(para);
                }
            }
            // Layout the paragraphs:
            tl.PerformLayout(true);
            // Text split options allow to implement widow and orphan control:
            var tso = new TextSplitOptions(tl)
            {
                RestMarginTop = 72,
                MinLinesInFirstParagraph = 2,
                MinLinesInLastParagraph = 2,
            };
            // Image alignment used to render the pictures:
            var ia = new ImageAlign(ImageAlignHorz.Left, ImageAlignVert.Top, true, true, true, false, false) { BestFit = true };
            // In a loop, split and render the text:
            while (true)
            {
                var splitResult = tl.Split(tso, out TextLayout rest);
                g = doc.Pages.Last.Graphics;
                doc.Pages.Last.Graphics.DrawTextLayout(tl, PointF.Empty);
                // Render all images that occurred on this page:
                foreach (var io in tl.InlineObjects)
                    doc.Pages.Last.Graphics.DrawImage((Image)io.Object, io.ObjectRect.ToRectangleF(), null, ia);
                // Break unless there is more to render:
                if (splitResult != SplitResult.Split)
                    break;
                // Assign the remaining text to the 'main' TextLayout, add a new page and continue:
                tl = rest;
                doc.Pages.Add();
                addFtr();
            }

            // Save the PDF:
            doc.Save(stream);
            // Dispose images (can be done only after saving the document):
            disposables.ForEach(d_ => d_.Dispose());
            // Done:
            return doc.Pages.Count;

            void addPara(string para)
            {
                // We implement a primitive markup to highlight some fragments in red:
                var txt = para.Split(new string[] { "<red>", "</red>" }, StringSplitOptions.None);
                for (int i = 0; i < txt.Length; ++i)
                {
                    if (i % 2 == 0)
                        tl.Append(txt[i]);
                    else
                        tl.Append(txt[i], new TextFormat(tl.DefaultFormat) { ForeColor = colorRed });
                }
                tl.AppendLine();
            }

            void addFtr()
            {
                doc.Pages.Last.Graphics.DrawImage(ftrImg, ftrRc, null, ImageAlign.StretchImage);
            }
        }

        // The list of text paragraphs to render.
        // Note:
        // - if a para starts with "::", the rest of the string is the name of an image file to insert;
        // - <red>..</red> mark up the text to highlight.
        string[] _paras = new string[]
        {
            "Originally there were in excess of <red>two point three (2.3) million</red> hectares of wetlands in southern Ontario. Today there is a mere <red>twelve percent (12%)</red> remaining (Rowntree 1979). Yet, these same areas are vital to the continued existence of a whole host of wildlife species. Grebes,herons, bitterns, rails, shorebirds, gulls, terns, and numerous smaller birds, plus the waterfowl, nest in or use wetlands for feeding and resting. About <red>ninety-five percent (95%)</red> of all furbearers are taken in water (Rowntree 1979). Reptiles and amphibians must return there to breed. ",
            "::Birdswetland.jpg",
            "Several species of game fish live or spawn in wetlands. Hundreds, if not thousands, of invertebrates that form the food of birds also rely on water for most, if not all, phases of their existence. In fact, most all species of animals we have must spend at least part of the year in wetlands. To lose any more of these vital areas is almost unthinkable.",
            "Wetlands enhance and protect water quality in lakes and streams where additional species spend their time and from which we draw our water. Water from drainage may have five (5) times more phosphates or as much as fifty (50) times more nitrates than water from marshes. These nutrient loads act as fertilizers to aquatic plants whose growth may clog rivers, foul shorelines and deplete oxygen in the water making it unsuitable for fish. Wetlands handle as much as <red>fifty percent (50%)</red> of terrestrial denitrification whereby nitrogen is returned to the atmosphere. Wetlands act as settling and filtration basins collecting silt that might build up behind dams or clog navigation channels. Vegetation in wetlands protects shorelines from damage by tides and storms. Wetlands soak up tremendous amounts of rainwater, slowing runoff and decreasing flooding that will help to decrease erosion of streambanks and prevent property damage. Water maintained in wetlands also helps to maintain ground water levels.",
            "Wetlands provide valuable renewable resources of fur, wild rice, fish, bait, cranberries, game, etc. They are rich in plant and animal life and are, therefore, ideal for scientific studies and educational field trips. The recreational potential for wetlands is immense. About <red>eighty percent (80%)</red> of Canadians value wildlife conservation and spend some three (3) billion dollars annually on nonconsumptive wildlife related activities as well as another one (1) billion on consumptive pursuits. Photography, bird-watching, canoeing, nature study, hiking, fishing and hunting are all pursued in wetlands.",
            "::palo_verde.jpg",
            "The economic value of wetlands may far exceed the returns gained from converting them to other uses. In addition to recreational potential, the farming of wildlife for economic return has proven to be viable for many species (Smith et al. 1983). Wetlands may prove valuable to more than fur, rice or cranberries in future.",
            "The greatest threats to our remaining wetlands are from agricultural drainage and industrial or housing developments (Brynaert 1983). Vast sums are expended annually by federal and provincial government agencies to implement drainage programs with little or no consideration given to wildlife values. The extensive so-called stream improvements, channeling and ditching, are very much questionable. It is essential now to introduce measures that clearly place the onus on agricultural agencies to prove that drainage projects are economically viable and that they do not jeopardize our wetland habitats (Brynaert 1983).",
            "Wetlands are important to the productivity of the entire biosphere (Sanderson 1977). They are vital to effective management of many wildlife species that depend upon these habitats. Whether a hunter or a naturalist, the preservation of wetlands is an objective that should appeal to everyone (Brynaert 1983). The entire province, country and continent have suffered a great loss in natural resources because of wetland losses. If we cannot succeed in saving wetlands, we shall not be able to meet the greater challenge of safeguarding an environment that man can continue to inhabit (Rowntree 1979).",
        };
    }
}
