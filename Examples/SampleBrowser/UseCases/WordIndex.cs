using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Common;
using GrapeCity.Documents.Pdf.Annotations;

namespace GcPdfWeb.Samples.Basics
{
    // This sample loads an existing PDF, and using a predefined list of key words,
    // builds an alphabetical index of those words linked to pages where they occur
    // in the document. The generated index pages are appended to the original document,
    // and saved in a new PDF.
    // The index is rendered in two balanced columns, using the technique
    // demonstrated in the BalancedColumns sample.
    //
    // NOTE: if you download this sample and run it locally on your own system 
    // without a valid GcPdf license, only the first five pages of the sample PDF
    // will be loaded, and the index will be generated for those five pages only.
    public class WordIndex
    {
        // Font collection to hold the fonts we need:
        private FontCollection _fc = new FontCollection();
        // Font family used throughout this sample (this is not case-sensitive):
        private const string _fontFamily = "segoe ui";

        // Main sample entry:
        public int CreatePDF(Stream stream)
        {
            // Set up a font collection with the fonts we need:
            _fc.RegisterDirectory(Path.Combine("Resources", "Fonts"));

            // Get the PDF to add index to:
            string tfile = Path.Combine("Resources", "PDFs", "CompleteJavaScriptBook.pdf");

            // The list of words on which we will build the index:
            // var words = _keywords.Select(w_ => w_.ToLower()).Distinct().Where(w_ => !string.IsNullOrEmpty(w_));
            var words = _keywords.Distinct(StringComparer.InvariantCultureIgnoreCase).Where(w_ => !string.IsNullOrEmpty(w_));

            // Load the PDF and add the index:
            using (var fs = new FileStream(tfile, FileMode.Open, FileAccess.Read))
            {
                var doc = new GcPdfDocument();
                doc.Load(fs);
                //
                int origPageCount = doc.Pages.Count;
                // Build and add the index:
                AddWordIndex(doc, words);
                // Open document on the first index page by default
                // (does not work in browser viewers, but works in Acrobat):
                doc.OpenAction = new DestinationFit(origPageCount);
                // Done:
                doc.Save(stream);
                return doc.Pages.Count;
            }
        }

        // The list of words to build the index on:
        private readonly string[] _keywords = new string[]
        {
            "JavaScript", "Framework", "MVC", "npm", "URL", "CDN", "HTML5", "CSS", "ES2015", "web",
            "Node.js", "API", "model", "view", "controller", "data management", "UI", "HTML",
            "API", "function", "var", "component", "design pattern", "React.js", "Angular", "AJAX",
            "DOM", "TypeScript", "ECMAScript", "CLI", "Wijmo", "CoffeeScript", "Elm",
            "plugin", "VueJS", "Knockout", "event", "AngularJS", "pure JS", "data binding", "OOP", "GrapeCity",
            "gauge", "JSX", "mobile", "desktop", "Vue", "template", "server-side", "client-side",
            "SPEC", "RAM", "ECMA",
        };

        // Adds a word index to the end of the passed document:
        private void AddWordIndex(GcPdfDocument doc, IEnumerable<string> words)
        {
            var tStart = DateTime.Now;

            // Words and page indices where they occur, sorted on words:
            SortedDictionary<string, List<int>> index = new SortedDictionary<string, List<int>>();

            // Here the main loop building the index is on key words.
            // An alternative would be to loop over the pages.
            // Depending on the relative sizes of the keyword dictionary vs
            // the number of pages in the document, one or the other might be better,
            // but this is beyond the scope of this sample.
            foreach (string word in words)
            {
                bool wholeWord = word.IndexOf(' ') == -1;
                var ft = doc.FindText(new FindTextParams(word, wholeWord, false), OutputRange.All);
                var pgs = ft.Select(fp_ => fp_.PageIndex).Distinct();
                // Very simplistic way of also finding plurals:
                if (wholeWord && !word.EndsWith('s'))
                {
                    var ftpl = doc.FindText(new FindTextParams(word + "s", wholeWord, false), OutputRange.All);
                    pgs = pgs.Concat(ftpl.Select(fp_ => fp_.PageIndex).Distinct());
                }
                if (pgs.Any())
                {
                    var sorted = pgs.ToList();
                    sorted.Sort();
                    index.Add(word, sorted);
                }
            }

            // Prepare to render the index. The whole index is built
            // in a single TextLayout instance, set up to render it
            // in two columns per page.
            // The main rendering loop uses the TextLayout.SplitAndBalance method 
            // using the approach demonstrated in BalancedColumns sample.
            // The complication here is that we need to associate a link to the
            // relevant page with each page number rendered, see linkIndices below.
            // Set up the TextLayout:
            const float margin = 72;
            var pageWidth = doc.PageSize.Width;
            var pageHeight = doc.PageSize.Height;
            var cW = pageWidth - margin * 2;
            // Caption (index letter) format:
            var tfCap = new TextFormat()
            {
                FontName = _fontFamily,
                FontBold = true,
                FontSize = 16,
                LineGap = 24,
            };
            // Index word and pages format:
            var tfRun = new TextFormat()
            {
                FontName = _fontFamily,
                FontSize = 10,
            };
            // Page headers/footers:
            var tfHdr = new TextFormat()
            {
                FontName = _fontFamily,
                FontItalic = true,
                FontSize = 10,
            };
            var tl = new TextLayout()
            {
                FontCollection = _fc,
                FirstLineIndent = -18, // hanging indent
                MaxWidth = pageWidth,
                MaxHeight = pageHeight,
                MarginLeft = margin,
                MarginRight = margin,
                MarginBottom = margin,
                MarginTop = margin,
                ColumnWidth = cW * 0.46f,
                TextAlignment = TextAlignment.Leading,
                ParagraphSpacing = 4,
                LineGapBeforeFirstLine = false,
            };

            // The list of text runs created for page numbers:
            List<Tuple<TextRun, int>> pgnumRuns = new List<Tuple<TextRun, int>>();
            // This loop builds the index on the TextLayout, saving the text runs
            // created for each page number rendered. Note that at this point 
            // (prior to the PerformLayout(true) call) the text runs do not contain any info
            // about their code points and render locations, so we can only save the text runs here.
            // Later they will be used to add links to referenced pages in the PDF:
            char litera = ' ';
            foreach (KeyValuePair<string, List<int>> kvp in index)
            {
                var word = kvp.Key;
                var pageIndices = kvp.Value;
                if (Char.ToUpper(word[0]) != litera)
                {
                    litera = Char.ToUpper(word[0]);
                    tl.Append($"{litera}\u2029", tfCap);
                }
                tl.Append(word, tfRun);
                tl.Append("  ", tfRun);
                for (int i = 0; i < pageIndices.Count; ++i)
                {
                    var from = pageIndices[i];
                    var tr = tl.Append((from + 1).ToString(), tfRun);
                    pgnumRuns.Add(Tuple.Create(tr, from));
                    // We merge sequential pages into "..-M":
                    int k = i;
                    for (int j = i + 1; j < pageIndices.Count && pageIndices[j] == pageIndices[j - 1] + 1; ++j)
                        k = j;
                    if (k > i + 1)
                    {
                        tl.Append("-", tfRun);
                        var to = pageIndices[k];
                        tr = tl.Append((to + 1).ToString(), tfRun);
                        pgnumRuns.Add(Tuple.Create(tr, to));
                        i = k; // fast forward
                    }
                    if (i < pageIndices.Count - 1)
                        tl.Append(", ", tfRun);
                    else
                        tl.AppendLine(tfRun);
                }
            }
            // This calculates the glyphs and lays out the whole index.
            // The tl.SplitAndBalance() call in the loop below does not require redoing the layout:
            tl.PerformLayout(true);

            //
            // Now we are ready to split and render the text layout, and also add links to page numbers.
            //

            // Split areas and options - see BalancedColumns for details:
            var psas = new PageSplitArea[] {
                new PageSplitArea(tl) { MarginLeft = tl.MarginLeft + (cW * 0.54f) },
            };
            TextSplitOptions tso = new TextSplitOptions(tl)
            {
                KeepParagraphLinesTogether = true,
            };

            // First original code point index in the current column:
            int cpiStart = 0;
            // Max+1 original code point index in the current column:
            int cpiEnd = 0;
            // Current index in pgnumRuns:
            int pgnumRunsIdx = 0;
            // Split and render the index in 2 columns:
            for (var page = doc.Pages.Add(); ; page = doc.Pages.Add())
            {
                var g = page.Graphics;
                // Add a simple page  header:
                g.DrawString($"Index generated by GcPdf on {tStart}", tfHdr,
                    new RectangleF(margin, 0, pageWidth - margin * 2, margin),
                    TextAlignment.Center, ParagraphAlignment.Center, false);
                // 'rest' will accept the text that did not fit on this page:
                var splitResult = tl.SplitAndBalance(psas, tso, out TextLayout rest);
                // Render text:
                g.DrawTextLayout(tl, PointF.Empty);
                g.DrawTextLayout(psas[0].TextLayout, PointF.Empty);
                // Add links from page numbers to pages:
                linkIndices(tl, page);
                linkIndices(psas[0].TextLayout, page);
                // Are we done yet?
                if (splitResult != SplitResult.Split)
                    break;
                tl = rest;
            }
            // Done:
            return;

            // Method to add links to actual pages over page numbers in the current column:
            void linkIndices(TextLayout tl_, Page page_)
            {
                cpiEnd += tl_.CodePointCount;
                for (; pgnumRunsIdx < pgnumRuns.Count; ++pgnumRunsIdx)
                {
                    var run = pgnumRuns[pgnumRunsIdx];
                    var textRun = run.Item1;
                    int cpi = textRun.CodePointIndex;
                    if (cpi >= cpiEnd)
                        break;
                    cpi -= cpiStart;
                    var rects = tl_.GetTextRects(cpi, textRun.CodePointCount);
                    System.Diagnostics.Debug.Assert(rects.Count > 0);
                    page_.Annotations.Add(new LinkAnnotation(rects[0].ToRectangleF(), new DestinationFit(run.Item2)));
                }
                cpiStart += tl_.CodePointCount;
            }
        }

        // Creates a sample document with 100 pages of 'lorem ipsum':
        private string MakeDocumentToIndex()
        {
            const int N = 100;
            string tfile = Path.GetTempFileName();
            using (var fsOut = new FileStream(tfile, FileMode.Open, FileAccess.ReadWrite))
            {
                var tdoc = new GcPdfDocument();
                // See StartEndDoc for details on StartDoc/EndDoc mode:
                tdoc.StartDoc(fsOut);
                // Prep a TextLayout to hold/format the text:
                var tl = new TextLayout();
                tl.FontCollection = _fc;
                tl.DefaultFormat.FontName = _fontFamily;
                tl.DefaultFormat.FontSize = 12;
                // Use TextLayout to layout the whole page including margins:
                tl.MaxHeight = tdoc.PageSize.Height;
                tl.MaxWidth = tdoc.PageSize.Width;
                tl.MarginLeft = tl.MarginTop = tl.MarginRight = tl.MarginBottom = 72;
                tl.FirstLineIndent = 72 / 2;
                // Generate the document:
                for (int pageIdx = 0; pageIdx < N; ++pageIdx)
                {
                    tl.Append(Common.Util.LoremIpsum(1));
                    tl.PerformLayout(true);
                    tdoc.NewPage().Graphics.DrawTextLayout(tl, PointF.Empty);
                    tl.Clear();
                }
                tdoc.EndDoc();
            }
            return tfile;
        }
    }
}
