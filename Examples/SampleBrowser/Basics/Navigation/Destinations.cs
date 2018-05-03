using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Pdf.Annotations;
using GrapeCity.Documents.Pdf.Actions;
using GcPdfWeb.Samples.Common;

namespace GcPdfWeb.Samples.Basics
{
    // Demonstrates how to create destinations and associate them with
    // outline nodes or links in the document body.
    public class Destinations
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();

            var page1 = doc.NewPage();
            var page2 = doc.NewPage();
            var page3 = doc.NewPage();

            var mainNote = Util.AddNote(
                "This is page 1.\n\n\n" +
                "Demo of various destination types.\n\n" +
                "Destinations attached to nodes in the document's outline tree:\n" +
                "  - Page 1: goes to this page, fits whole page\n" +
                "  - Page 2: goes to page 2, fits whole page\n" +
                "    -- Note 2, zoom 200%: goes to note 2 on page 2, zooms to 200%\n" +
                "    -- Zoom 100%: zooms to whole page 2\n" +
                "    -- Back to page 1: goes back to this page\n" +
                "  - Page 3: goes to page 3, fits whole page\n" +
                "    -- Zoom to note on page 3: zooms to whole note on page 3\n" +
                "    -- Back to page 1: goes back to this page\n" +
                "  - Named destinations: keyed by names in NamedDestinations dictionary:\n" +
                "    -- page1\n" +
                "    -- page2\n" +
                "    -- page3\n\n" +
                "Destinations associated with areas in the document body:",
                page1);
            Util.AddNote(
                "This is page 2.",
                page2);
            var noteOnPage2 = Util.AddNote(
                "Note 2 on page 2.",
                page2, new RectangleF(300, 400, 200, 300));
            Util.AddNote(
                "This is page 3",
                page3);
            var noteOnPage3 = Util.AddNote(
                "This is a somewhat longer\n(even though not really long)\nnote on page 3.",
                page3, new RectangleF(200, 440, 200, 300));

            // Destinations in the outline tree:

            // DestinationFit: fits whole page:
            OutlineNode on1 = new OutlineNode("Page 1", new DestinationFit(0));
            doc.Outlines.Add(on1);

            OutlineNode on2 = new OutlineNode("Page 2", new DestinationFit(1));
            doc.Outlines.Add(on2);
            // DestinationXYZ: allows to specify top/left coordinates and the zoom level (1 is 100%):
            on2.Children.Add(new OutlineNode("Note 2, zoom 200%", new DestinationXYZ(1, noteOnPage2.X, noteOnPage2.Y, 2)));
            on2.Children.Add(new OutlineNode("Zoom 100%", new DestinationXYZ(1, null, null, 1)));
            // Add a link back to page 1:
            on2.Children.Add(new OutlineNode("Back to page 1", new DestinationFit(0)));

            OutlineNode on3 = new OutlineNode("Page 3", new DestinationFit(2));
            doc.Outlines.Add(on3);
            // DestinationFitR: fits a rectangle on page:
            on3.Children.Add(new OutlineNode("Zoom to note on page 3", new DestinationFitR(2, noteOnPage3)));
            // Add links back to page 1 & 2:
            on3.Children.Add(new OutlineNode("Go to page 2", new DestinationFit(1)));
            on3.Children.Add(new OutlineNode("Go to page 1", new DestinationFit(0)));

            // Destinations in the document body (reusing destinations from the outlines):
            // Go to page 2:
            var rc = Util.AddNote("Go to page 2", page1, new RectangleF(72, mainNote.Bottom + 18, 200, 72));
            page1.Annotations.Add(new LinkAnnotation(rc, on2.Dest));
            // Go to page 3:
            rc = Util.AddNote("Go to page 3", page1, new RectangleF(72, rc.Bottom + 18, 200, 72));
            page1.Annotations.Add(new LinkAnnotation(rc, on3.Dest));

            // Destinations can also be named and added to the document's NamedDestinations dictionary.
            doc.NamedDestinations.Add("page1", new DestinationFit(0));
            doc.NamedDestinations.Add("page2", new DestinationFit(1));
            doc.NamedDestinations.Add("page3", new DestinationFit(2));
            // Then they can be referenced using DestinationRef class, here we add them to the outline:
            OutlineNode onNamed = new OutlineNode("Named destinations", (DestinationBase)null);
            doc.Outlines.Add(onNamed);
            onNamed.Children.Add(new OutlineNode("Named: page1", new DestinationRef("page1")));
            onNamed.Children.Add(new OutlineNode("Named: page2", new DestinationRef("page2")));
            onNamed.Children.Add(new OutlineNode("Named: page3", new DestinationRef("page3")));

            // Done:
            doc.Save(stream);
        }
    }
}
