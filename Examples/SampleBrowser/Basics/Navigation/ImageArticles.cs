//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Pdf.Articles;
using GrapeCity.Documents.Imaging;
using GrapeCity.Documents.Drawing;
using GcPdfWeb.Samples.Common;

namespace GcPdfWeb.Samples
{
    // This sample shows how to create article threads in a PDF document.
    // An article thread is a sequence of related pages or page areas that can be
    // navigated sequentially (forward or back) in a supporting PDF viewer.
    // In this sample we load a number of photos from a folder and render them
    // one per page in a random order.
    // Some photos are associated (via known file names) with a specific subject
    // (buildings, art, etc.), and we put all images associated with
    // each subject into a subject-specific article thread (images that are not
    // associated with any known subject are put in the 'Miscellaneous' thread).
    // In addition we create 3 threads for different aspect ratios (horizontal,
    // vertical and square), and add each image to the appropriate aspect article.
    // See section 'Article threads' in https://helpx.adobe.com/ca/acrobat/using/navigating-pdf-pages.html
    // for details on how to navigate article threads in Acrobat
    // (our JavaScript PDF viewer provides a similar UI for this).
    public class ImageArticles
    {
        // Article names:
        static class ArticleNames
        {
            public static string Landscape => "Subject: landscape";
            public static string Art => "Subject: art";
            public static string Flora => "Subject: flora";
            public static string Buildings => "Subject: buildings";
            public static string Misc => "Subject: Miscellaneous";
            public static string AspectHorz => "Aspect: horizontal";
            public static string AspectVert => "Aspect: vertical";
            public static string AspectSquare => "Aspect: square";
        }

        // Associate known image file names with appropriate subjects:
        static readonly Dictionary<string, string> _subjects = new Dictionary<string, string>()
        {
            {"aurora.jpg", ArticleNames.Landscape },
            {"chairs.jpg", ArticleNames.Buildings },
            {"clouds.jpg", ArticleNames.Landscape },
            {"colosseum.jpg", ArticleNames.Art },
            {"deadwood.jpg", ArticleNames.Flora },
            {"door.jpg", ArticleNames.Buildings },
            {"ferns.jpg", ArticleNames.Flora },
            {"fiord.jpg", ArticleNames.Landscape },
            {"firth.jpg", ArticleNames.Landscape },
            {"lady.jpg", ArticleNames.Art },
            {"lavender.jpg", ArticleNames.Flora },
            {"maple.jpg", ArticleNames.Buildings },
            {"minerva.jpg", ArticleNames.Art },
            {"out.jpg", ArticleNames.Landscape },
            {"pines.jpg", ArticleNames.Flora },
            {"purples.jpg", ArticleNames.Flora },
            {"reds.jpg", ArticleNames.Flora },
            {"road.jpg", ArticleNames.Landscape },
            {"rome.jpg", ArticleNames.Art },
            {"roofs.jpg", ArticleNames.Buildings },
            {"sea.jpg", ArticleNames.Landscape },
            {"skye.jpg", ArticleNames.Landscape },
            {"tudor.jpg", ArticleNames.Buildings },
            {"windswept.jpg", ArticleNames.Flora },
            // Images not in this list are 'misc'.
        };

        // Class to hold image info:
        private class ImageInfo
        {
            public string Name;
            public IImage Image;
            public string Subject;
            public string Aspect;
        }

        public int CreatePDF(Stream stream)
        {
            // Load images and their associated infos:
            var imageInfos = new List<ImageInfo>();
            foreach (var fname in Directory.GetFiles(Path.Combine("Resources", "Images"), "*", SearchOption.AllDirectories))
            {
                var image = Util.ImageFromFile(fname);
                var aspect = image.Width > image.Height ? ArticleNames.AspectHorz :
                    (image.Width < image.Height ? ArticleNames.AspectVert : ArticleNames.AspectSquare);
                var name = Path.GetFileName(fname);
                _subjects.TryGetValue(name, out string subject);
                if (string.IsNullOrEmpty(subject))
                    subject = ArticleNames.Misc;
                imageInfos.Add(new ImageInfo()
                {
                    Name = name,
                    Image = image,
                    Subject = subject,
                    Aspect = aspect
                });
            }
            // Randomize the order of images in the PDF:
            imageInfos.Shuffle();

            // Keys are article thread names (from ArticleNames),
            // values are ArticleThread objects to be added to the PDF:
            var articles = new Dictionary<string, ArticleThread>();
            foreach (var subject in _subjects.Values.Distinct())
                articles.Add(subject,
                    new ArticleThread()
                    {
                        Info = new DocumentInfo() { Title = subject }
                    });
            articles.Add(ArticleNames.Misc,
                new ArticleThread()
                {
                    Info = new DocumentInfo() { Title = ArticleNames.Misc }
                });
            var horizontals = new ArticleThread()
            {
                Info = new DocumentInfo() { Title = ArticleNames.AspectHorz }
            };
            var verticals = new ArticleThread()
            {
                Info = new DocumentInfo() { Title = ArticleNames.AspectVert }
            };
            var squares = new ArticleThread()
            {
                Info = new DocumentInfo() { Title = ArticleNames.AspectSquare }
            };

            // Create the document:
            var doc = new GcPdfDocument();

            // Add images (one per page) to the PDF and article threads:
            var ia = new ImageAlign(ImageAlignHorz.Center, ImageAlignVert.Top, true, true, true, false, false);
            for (int i = 0; i < imageInfos.Count; ++i)
            {
                var page = doc.NewPage();
                var ii = imageInfos[i];
                RectangleF rc = new RectangleF(72, 72, doc.PageSize.Width - 144, doc.PageSize.Height - 144);
                // Note that we get the actual image bounds to precisely specify the page area in the thread:
                page.Graphics.DrawImage(ii.Image, rc, null, ia, out RectangleF[] imageBounds);
                var bounds = imageBounds[0];
                // Add the image to proper subject and aspect threads:
                articles[ii.Subject].Beads.Add(new ArticleBead() { Page = page, Bounds = bounds });
                if (ii.Aspect == ArticleNames.AspectHorz)
                    horizontals.Beads.Add(new ArticleBead() { Page = page, Bounds = bounds });
                else if (ii.Aspect == ArticleNames.AspectVert)
                    verticals.Beads.Add(new ArticleBead() { Page = page, Bounds = bounds });
                else
                    squares.Beads.Add(new ArticleBead() { Page = page, Bounds = bounds });
            }
            // Add subject and aspect article threads to the PDF:
            foreach (var article in articles.Select(a_ => a_.Value))
                doc.ArticleThreads.Add(article);
            doc.ArticleThreads.Add(horizontals);
            doc.ArticleThreads.Add(verticals);
            doc.ArticleThreads.Add(squares);

            // Done:
            doc.Save(stream);
            return doc.Pages.Count;
        }
    }
}
