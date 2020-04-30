//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Linq;
using System.Drawing;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Pdf.Annotations;
using GrapeCity.Documents.Pdf.Actions;

namespace GcPdfWeb.Samples.Common
{
    public static class Util
    {
        public const int LargeDocumentIterations = 1000;

        private static readonly string[] s_words = new[]
            {
                "lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit", "mauris", "id", "volutpat",
                "tellus", "ullamcorper", "sem", "praesent", "et", "ante", "laoreet", "lobortis", "nunc", "congue",
                "nisi", "donec", "ac", "tempus", "sed", "feugiat", "pulvinar", "pharetra", "turpis", "nonummy",
                "at", "felis", "eget", "molestie", "euismod", "non", "aliquet", "diam", "proin", "mi", "nibh",
                "massa", "tincidunt", "ut", "dolore", "magna", "aliquam", "erat",
            };

        private static int s_gen = 0;

        public static Random NewRandom()
        {
            return new Random((int)DateTime.Now.Ticks + Interlocked.Increment(ref s_gen));
        }

        // https://stackoverflow.com/questions/4286487/is-there-any-lorem-ipsum-generator-in-c
        public static string LoremIpsum(
            int numParagraphs = 5,
            int minSentences = 5,
            int maxSentences = 10,
            int minWords = 5,
            int maxWords = 30)
        {
            int commaMax = 10;
            var rand = NewRandom();
            int numSentences = rand.Next(maxSentences - minSentences) + minSentences + 1;
            int numWords = rand.Next(maxWords - minWords) + minWords + 1;

            int wdslen = s_words.Length;
            StringBuilder result = new StringBuilder();
            for (int p = 0; p < numParagraphs; p++)
            {
                for (int s = 0; s < numSentences; s++)
                {
                    for (int w = 0; w < numWords; w++)
                    {
                        var word = s_words[rand.Next(wdslen)];
                        if (w == 0)
                        {
                            result.Append(char.ToUpper(word[0]));
                            result.Append(word.Substring(1));
                        }
                        else
                        {
                            if (rand.Next(commaMax) == 0)
                                result.Append(',');
                            result.Append(' ');
                            result.Append(word);
                        }
                    }
                    result.Append(". ");
                }
                result.Append("\r\n");
            }
            return result.ToString();
        }

        // Gets the list of words used by the LoremIpsum method.
        public static List<string> LoremWords()
        {
            return s_words.ToList();
        }

        // https://stackoverflow.com/questions/273313/randomize-a-listt
        private static Random rng = new Random();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Adds a note to the document - a text fragment on a colored background.
        /// Used to insert meta info into sample documents.
        /// </summary>
        /// <param name="text">The text to draw.</param>
        /// <param name="page">The page to draw on.</param>
        /// <param name="bounds">The text bounds. If null, whole page with 1" margins all around is used.</param>
        /// <returns>The actual rectangle occupied by the note.</returns>
        public static RectangleF AddNote(string text, Page page, RectangleF? bounds = null)
        {
            var g = page.Graphics;
            var tl = g.CreateTextLayout();
            var pad = g.Resolution / 8f;
            tl.DefaultFormat.Font = StandardFonts.Helvetica;
            var yumin = Path.Combine("Resources", "Fonts", "yumin.ttf");
            if (File.Exists(yumin))
                tl.DefaultFormat.Font.AddLinkedFont(Font.FromFile(yumin));
            tl.DefaultFormat.FontSize = 12;
            if (!bounds.HasValue)
            {
                var margin = g.Resolution; // 1" margins by default
                bounds = new RectangleF(margin, margin, page.Size.Width - margin * 2, page.Size.Height - margin * 2);
            }
            tl.MaxWidth = bounds.Value.Width;
            tl.MaxHeight = bounds.Value.Height;
            tl.MarginAll = pad;

            var texts = Regex.Split(text, "(https?://[^ ]+)", RegexOptions.IgnoreCase);
            var runs = new List<TextRun>();
            var urls = new List<string>();
            foreach (var txt in texts)
            {
                if (txt.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    urls.Add(txt);
                    runs.Add(tl.Append(txt, new TextFormat(tl.DefaultFormat)
                    { ForeColor = Color.Blue, Underline = true, TextRunAsCluster = true, }));
                }
                else
                    tl.Append(txt);
            }
            tl.PerformLayout(true);
            for (int i = 0; i < runs.Count; ++i)
            {
                var run = runs[i];
                var rects = tl.GetTextRects(run.CodePointIndex, run.CodePointCount);
                foreach (var r in rects)
                {
                    var rc = r.ToRectangleF();
                    rc.Offset(bounds.Value.Location);
                    page.Annotations.Add(new LinkAnnotation(rc, new ActionURI(urls[i])));
                }
            }

            var rect = tl.ContentRectangle;
            rect.Offset(bounds.Value.Location);
            rect.Inflate(pad, pad);
            g.FillRectangle(rect, Color.FromArgb(213, 221, 240));
            g.DrawRectangle(rect, Color.FromArgb(59, 92, 170), 0.5f);
            g.DrawTextLayout(tl, bounds.Value.Location);

            return rect;
        }

        /// <summary>
        /// Loads an image from a disk file.
        /// <para>
        /// The only point of this method is to provide better performance on a Windows system,
        /// by using GrapeCity.Documents.Drawing.Windows.WicImage which works via the
        /// Windows Imaging Component framework. If WicImage is not supported, this method
        /// defaults to platform-independent Image.FromFile() method.
        /// </para>
        /// </summary>
        /// <param name="path">The image path.</param>
        /// <returns>The Image object.</returns>
        public static IImage ImageFromFile(string path)
        {
            if (GrapeCity.Documents.Imaging.Windows.GcWicBitmap.IsSupported)
                return new GrapeCity.Documents.Imaging.Windows.GcWicBitmap(path);
            return Image.FromFile(path);
        }
    }
}
