using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Pdf.Annotations;
using GrapeCity.Documents.Pdf.Actions;

namespace GcPdfWeb.Samples.Basics
{
    // Shows how to associate a PDF action with a JavaScript script.
    // In this example the script is associated with a link on a page.
    // Note that JavaScript may not work in some PDF viewers (e.g. built-in browser viewers).
    // See <a class="nocode a" target="_blank" href="http://www.adobe.com/content/dam/acom/en/devnet/acrobat/pdfs/js_api_reference.pdf">JavaScript for Acrobat</a> for details.
    public class JavaScriptAction
    {
        const string js =
            "var cChoice = app.popUpMenu(\"Introduction\", \" - \", \"Chapter 1\",\r\n" +
            "[ \"Chapter 2\", \"Chapter 2 Start\", \"Chapter 2 Middle\",\r\n" +
            "[\"Chapter 2 End\", \"The End\"]]);\r\n" +
            "app.alert(\"You chose the '\" + cChoice + \"' menu item\");";

        public void CreatePDF(Stream stream)
        {
            // Create a new PDF document:
            var doc = new GcPdfDocument();
            // Add a page and get its graphics to draw on:
            var g = doc.NewPage().Graphics;
            // Create the action:
            ActionJavaScript jsAction = new ActionJavaScript(js);
            // Create a text format to draw the "Hello World!" string:
            TextFormat tf = new TextFormat();
            tf.Font = StandardFonts.Times;
            tf.FontSize = 14;
            // Draw the link string in a rectangle:
            var text = "Click this to show the popup menu.";
            var rect = new RectangleF(new PointF(72, 72), g.MeasureString(text, tf));
            g.FillRectangle(rect, Color.LightGoldenrodYellow);
            g.DrawString(text, tf, rect);
            var result = new LinkAnnotation(rect, jsAction);
            doc.Pages.Last.Annotations.Add(result);
            // Add warning about this possibly not working in a browser:
            Common.Util.AddNote(
                "Note that JavaScript may not work in some PDF viewers such as built-in browser viewers.",
                doc.Pages.Last, new RectangleF(rect.X, rect.Bottom + 36, 400, 400));
            // Done:
            doc.Save(stream);
        }
    }
}
