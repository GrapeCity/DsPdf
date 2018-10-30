using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples
{
    // This sample shows how to create, initialize and use the FontCollection class,
    // which is the recommended way to manage fonts and use them when rendering texts
    // in GrapeCity Documents for PDF.
    // 
    // The main points to keep in mind, and the recommended steps to follow,
    // when using FontCollection with GcPdf:
    //
    // 1. Create an instance of the FontCollection class.
    //    FontCollection is not a static class, you need an instance of it to use.
    //    Also, it is a regular .NET collection of Font objects, so all usual 
    //    collection manipulation methods (Add, Insert, Remove etc) can be used on it.
    //
    // 2. Populate the font collection with fonts using any of the follwing methods:
    //    - RegisterDirectory(): registers all fonts found in a specified directory;
    //    - RegisterFont(): registers font(s) found in a specified file;
    //    - Add(Font): adds a font instance that you created.
    //    Note that registering directories or fonts with a font collection is a fast 
    //    and light-weight operation. The font collection does not actually load all font data
    //    when directories or individual fonts are registered with it. Instead, it loads only
    //    the minimal info so that it can find and provide fonts quickly and efficiently
    //    when needed.
    //
    // 3. Assign your instance of the font collection to TextLayout.FontCollection (and to 
    //    GcGraphics.FontCollection if using GcGraphics.MeasureString/DrawString) so that
    //    the correct fonts can be found.
    //
    // 4. In your text rendering code, select fonts by specifying font names (TextFormat.FontName,
    //    the names must match exactly but the case is not important), and font bold and italic
    //    flags (TextFormat.FontBold/FontItalic). If a suitable bold/italic version of the requested
    //    font is found in the collection, it will be used; otherwise font emulation will be applied.
    //
    // 5. FontCollection methods and properties are thread-safe, so once your font collection
    //    has been populated, you can cache and share it between sessions and/or modules
    //    of your application. You do need to exercise caution when modifying and accessing
    //    the font collection simultaneously from different threads though, as it may change
    //    between a check of some condition on the collection, and action on that check.
    //    For such cases the FontCollection.SyncRoot property is provided, and should be used.
    //
    // The code in this sample illustrates most of the points above. 
    public class FontCollectionTest
    {
        public void CreatePDF(Stream stream)
        {
            // Create a FontCollection instance:
            FontCollection fc = new FontCollection();
            // Populate it with fonts from the specified directory:
            fc.RegisterDirectory(Path.Combine("Resources", "Fonts"));

            // Generate a sample document using the font collection to provide fonts:
            var doc = new GcPdfDocument();
            var page = doc.Pages.Add();
            var g = page.Graphics;

            // For TextLayout/TextFormat to be able to use a font collection, it must be
            // associated with it like so:
            var tl = new TextLayout() { FontCollection = fc };

            // Render some strings using the different fonts from our collection:
            var tf = new TextFormat() { FontName = "times new roman", FontSize = 16 };
            tl.Append("Using FontCollection to manage fonts and render text\n\n", tf);
            tf.FontSize = 12;
            tl.Append("Text rendered using Times New Roman regular font. \n", tf);
            // Setting a font style (bold or italic) will tell the font collection
            // to search for a suitable font (if it is not found, emulation will be used):
            tf.FontItalic = true;
            tl.Append("Text rendered using Times New Roman italic font. \n", tf);
            // Text format is applied to a text run when the text is appended,
            // so we can re-use the same format, modifying its properties
            // to render differently formatted texts:
            tf.FontBold = true;
            tl.Append("Text rendered using Times New Roman bold italic font. \n", tf);
            tf.FontItalic = false;
            tl.Append("Text rendered using Times New Roman bold font. \n", tf);
            tf.FontName = "segoe ui";
            tl.Append("Text rendered using Segoe UI bold font. \n", tf);
            tf.FontBold = false;
            tl.Append("Text rendered using Segoe UI regular font. \n", tf);

            // Apply page settings to the page layout and render the page:
            tl.MaxWidth = page.Size.Width;
            tl.MaxHeight = page.Size.Height;
            tl.MarginLeft = tl.MarginTop = tl.MarginRight = tl.MarginBottom = 72;
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, PointF.Empty);

            // If using GcGraphics.DrawString/MeasureString, this will allow the TextLayout
            // created internally by GcGraphics to find the specified fonts in the font collection:
            g.FontCollection = fc;

            // Use GcGraphics.DrawString to show that the font collection is also used
            // by the graphics once the FontCollection has been set on it:
            g.DrawString("Text rendered using Segoe UI bold, drawn by GcGraphics.DrawString() method.",
                new TextFormat() { FontName = "segoe ui", FontBold = true, FontSize = 10 },
                new PointF(72, tl.ContentRectangle.Bottom + 12));

            // Done:
            doc.Save(stream);
        }
    }
}
