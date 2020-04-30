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
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;
using GcPdfWeb.Samples.Common;
using GrapeCity.Documents.Pdf.Annotations;

namespace GcPdfWeb.Samples
{
    // This sample loads all images found in a directory, then renders each image
    // in the largest possible size on a separate page of the PDF.
    // Finally it inserts a TOC of image thumbnails linked to the large images
    // into the document.
    // See also SlidePages.
    public class ImageLinks
    {
        private class ImageInfo
        {
            public string Name;
            public IImage Image;
            public int PageIdx;
        }

        public int CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var font = Font.FromFile(Path.Combine("Resources", "Fonts", "segoeui.ttf"));
            // 1/2" page margins all around:
            const float margin = 36;

            // Load all images from the Resources/Images folder:
            List<ImageInfo> imageInfos = new List<ImageInfo>();
            foreach (var fname in Directory.GetFiles(Path.Combine("Resources", "Images"), "*", SearchOption.AllDirectories))
                imageInfos.Add(new ImageInfo() { Name = Path.GetFileName(fname), Image = Util.ImageFromFile(fname) });
            imageInfos.Shuffle();
            // Set up image alignment that would center images horizontally and align to top vertically:
            ImageAlign ia = new ImageAlign(ImageAlignHorz.Center, ImageAlignVert.Top, true, true, true, false, false);
            // Image rectangle for full-sized images - whole page:
            RectangleF rBig = new RectangleF(margin, margin, doc.PageSize.Width - margin * 2, doc.PageSize.Height - margin * 2);
            // Render all images full-size, one image per page:
            for (int i = 0; i < imageInfos.Count; ++i)
            {
                var g = doc.NewPage().Graphics;
                var ii = imageInfos[i];
                g.DrawImage(ii.Image, rBig, null, ia);
                ii.PageIdx = i;
            }
            // Insert page(s) with thumbnails into the beginning of the document as a 4x5 grid (see SlidePages):
            const int rows = 5;
            const int cols = 4;
            float gapx = 72f / 4, gapy = gapx;
            float sWidth = (doc.PageSize.Width - margin * 2 + gapx) / cols;
            float sHeight = (doc.PageSize.Height - margin * 2 + gapy) / rows;
            if (sWidth > sHeight)
            {
                gapx += sWidth - sHeight;
                sWidth = sHeight;
            }
            else
            {
                gapy += sHeight - sWidth;
                sHeight = sWidth;
            }
            const float sMargin = 72f / 6;
            // Center thumbnails vertically too:
            ia.AlignVert = ImageAlignVert.Center;
            // Text format for image captions:
            TextFormat tf = new TextFormat() { Font = font, FontSize = sMargin * 0.65f };
            // Insertion point:
            PointF ip = new PointF(margin, margin);
            var page = doc.Pages.Insert(0);
            for (int i = 0; i < imageInfos.Count(); ++i)
            {
                var ii = imageInfos[i];
                var rect = new RectangleF(ip, new SizeF(sWidth - gapx, sHeight - gapy));
                // Add a link to the page where the full-sized image is (the page index
                // will be updated when we know how many pages are in TOC, see below):
                page.Annotations.Add(new LinkAnnotation(rect, new DestinationFit(ii.PageIdx)));
                // Draw thumbnail:
                var g = page.Graphics;
                g.FillRectangle(rect, Color.LightGray);
                g.DrawRectangle(rect, Color.Black, 0.5f);
                rect.Inflate(-sMargin, -sMargin);
                g.DrawImage(ii.Image, rect, null, ia, out RectangleF[] imageRect);
                g.DrawRectangle(imageRect[0], Color.DarkGray, 1);
                // Print image file name as caption in the bottom slide margin:
                g.DrawString(ii.Name, tf, 
                    new RectangleF(rect.X, rect.Bottom, rect.Width, sMargin),
                    TextAlignment.Center, ParagraphAlignment.Near, false);
                ip.X += sWidth;
                if (ip.X + sWidth > doc.PageSize.Width)
                {
                    ip.X = margin;
                    ip.Y += sHeight;
                    if (ip.Y + sHeight > doc.PageSize.Height)
                    {
                        page = doc.Pages.Insert(doc.Pages.IndexOf(page) + 1);
                        ip.Y = margin;
                    }
                }
            }
            // We now go through all TOC pages, updating page indices in links' destinations
            // to account for the TOC pages inserted in the beginning of the document:
            int tocPages = doc.Pages.IndexOf(page) + 1;
            for (int i = 0; i < tocPages; ++i)
            {
                foreach (var ann in doc.Pages[i].Annotations)
                    if (ann is LinkAnnotation link && link.Dest is DestinationFit dest)
                        link.Dest = new DestinationFit(dest.PageIndex.Value + tocPages);
            }
            // Done:
            doc.Save(stream);
            imageInfos.ForEach((ii_) => ii_.Image.Dispose());
            return doc.Pages.Count;
        }
    }
}

