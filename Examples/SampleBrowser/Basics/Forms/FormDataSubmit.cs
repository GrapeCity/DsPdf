//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Pdf.AcroForms;
using GrapeCity.Documents.Pdf.Actions;
using GrapeCity.Documents.Pdf.Annotations;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples
{
    // This sample creates an AcroForm PDF that can be submitted to the server.
    // The server then uses the (new in GcPdf v3) GcPdfDocument.ImportFormDataFromCollection()
    // method to import the submitted data into a PDF that contains a similarly structured
    // PDF form, and sends the form filled with user provided data back to the client.
    //
    // Note that the produced PDF with filled form fields
    // is shown in the client browser's default PDF viewer.
    //
    // This sample is similar to the now obsolete FormSubmitXml sample,
    // but the server side is much simpler as it uses the new ImportFormDataFromCollection()
    // method that accepts a data structure very similar to how data is sent from the client form,
    // so almost no code is needed to manipulate that data.
    // See also the FormSubmit sample.
    public class FormDataSubmit
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();

            var rc = Common.Util.AddNote(
                "Fill the fields in the form and click 'Submit' to send it back to the server. " +
                "The sample server will use the GcPdfDocument.ImportFormDataFromCollection() method " +
                "to import the submitted data into a different but compatible PDF form, " +
                "and the filled form will be sent back to your browser. " +
                "Note that the filled form is opened in the browser's default PDF viewer, " +
                "and does not have the 'Submit' and 'Reset' buttons.",
                page);

            var g = page.Graphics;
            TextFormat tf = new TextFormat() { Font = StandardFonts.Times, FontSize = 14 };
            PointF ip = new PointF(72, rc.Bottom + 36);
            float fldOffset = 72 * 2 + 46;
            float fldHeight = tf.FontSize * 1.2f;
            float dY = 32;

            // Text field:
            g.DrawString("First name:", tf, ip);
            var fldFirstName = new TextField() { Name = "FirstName", Value = "John" };
            fldFirstName.Widget.Page = page;
            fldFirstName.Widget.Rect = new RectangleF(ip.X + fldOffset, ip.Y, 72 * 3, fldHeight);
            fldFirstName.Widget.TextFormat.Font = tf.Font;
            fldFirstName.Widget.TextFormat.FontSize = tf.FontSize;
            doc.AcroForm.Fields.Add(fldFirstName);
            ip.Y += dY;

            // Text field:
            g.DrawString("Last name:", tf, ip);
            var fldLastName = new TextField() { Name = "LastName", Value = "Smith" };
            fldLastName.Widget.Page = page;
            fldLastName.Widget.Rect = new RectangleF(ip.X + fldOffset, ip.Y, 72 * 3, fldHeight);
            fldLastName.Widget.TextFormat.Font = tf.Font;
            fldLastName.Widget.TextFormat.FontSize = tf.FontSize;
            doc.AcroForm.Fields.Add(fldLastName);
            ip.Y += dY;

            // Checkbox:
            g.DrawString("Subscribe to Mailing List:", tf, ip);
            var fldCheckbox = new CheckBoxField() { Name = "Subscribe", Value = true };
            fldCheckbox.Widget.Page = page;
            fldCheckbox.Widget.Rect = new RectangleF(ip.X + fldOffset, ip.Y, fldHeight, fldHeight);
            doc.AcroForm.Fields.Add(fldCheckbox);
            ip.Y += dY;

            // Multiline TextBox:
            g.DrawString("Additional information:", tf, ip);
            var fldAdditionalInfo = new TextField() { Name = "AdditionalInfo", Multiline = true };
            fldAdditionalInfo.Widget.Page = page;
            fldAdditionalInfo.Widget.Rect = new RectangleF(ip.X + fldOffset, ip.Y, 72 * 3, fldHeight * 2);
            fldAdditionalInfo.Widget.TextFormat.Font = tf.Font;
            fldAdditionalInfo.Widget.TextFormat.FontSize = tf.FontSize;
            doc.AcroForm.Fields.Add(fldAdditionalInfo);
            ip.Y += dY * 2;

            // Submit form button:
            var btnSubmit = new PushButtonField();
            btnSubmit.Widget.Rect = new RectangleF(ip.X + fldOffset, ip.Y, 72, fldHeight);
            btnSubmit.Widget.ButtonAppearance.Caption = "Submit";
            btnSubmit.Widget.Highlighting = HighlightingMode.Invert;
            btnSubmit.Widget.Page = page;

            // The URL for the submission:
            btnSubmit.Widget.Events.Activate = new ActionSubmitForm("/Samples/HandleFormDataSubmit");
            doc.AcroForm.Fields.Add(btnSubmit);

            // Reset form button:
            var btnReset = new PushButtonField();
            btnReset.Widget.Rect = new RectangleF(ip.X + fldOffset + 72 * 1.5f, ip.Y, 72, fldHeight);
            btnReset.Widget.ButtonAppearance.Caption = "Reset";
            btnReset.Widget.Highlighting = HighlightingMode.Invert;
            btnReset.Widget.Page = page;
            btnReset.Widget.Events.Activate = new ActionResetForm();
            doc.AcroForm.Fields.Add(btnReset);
            ip.Y += dY;

            // Done:
            doc.Save(stream);
        }

        //
        // NOTE: the code below is used by the web sample browser controller when the form
        // prepared by this sample is submitted, it is not directly called by the CreatePDF() method.
        //

        // Creates a GcPdfDocument, loads an AcroForm PDF into it, and fills it with data
        // using the GcPdfDocument.ImportFormDataFromCollection() method.
        //
        // This method is called by the samples controller when the form prepared by this sample
        // is submitted by the user. The controller method converts the IFormCollection that it
        // receives to an array or key value pairs, where keys are field names and values are
        // lists of string values, then calls this method to import the values into the
        // compatible ImportFormXML.pdf PDF form. That form is then sent back to the client.
        //
        // The controller code that calls this method looks like this:
        //
        // public IActionResult HandleFormDataSubmit(IFormCollection fields)
        // {
        //   var values = fields.ToList();
        //   var fieldValues = values.Select(kvp_ => new KeyValuePair<string, IList<string>>(kvp_.Key, kvp_.Value.ToArray())).ToArray();
        //   var ms = Samples.FormDataSubmit.ImportFormData(fieldValues);
        //   var result = new FileStreamResult(ms, "application/pdf");
        //   return result;
        // }
        public static Stream ImportFormData(KeyValuePair<string, IList<string>>[] fieldValues)
        {
            GcPdfDocument pdf = new GcPdfDocument();
            using (var fs = new FileStream(Path.Combine("Resources", "PDFs", "ImportFormFromCollection.pdf"), FileMode.Open, FileAccess.Read))
            {
                // Load compatible empty form:
                pdf.Load(fs);
                // Import submitted data:
                pdf.ImportFormDataFromCollection(fieldValues);
                // Done:
                var outMs = new MemoryStream();
                pdf.Save(outMs);
                outMs.Seek(0, SeekOrigin.Begin);
                return outMs;
            }
        }
    }
}
