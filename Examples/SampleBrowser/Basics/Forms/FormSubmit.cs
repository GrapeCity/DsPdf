//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Pdf.AcroForms;
using GrapeCity.Documents.Pdf.Actions;
using GrapeCity.Documents.Pdf.Annotations;
using GrapeCity.Documents.Text;
using System.Drawing;
using System.IO;

namespace GcPdfWeb.Samples
{
    // This sample demonstrates how to create an AcroForm PDF that the user can submit.
    // Here we submit it To the sample server, which receives the data and sends it back
    // in a special form.
    public class FormSubmit
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();

            var rc = Common.Util.AddNote(
                "In this sample the Submit button is associated with the ActionSubmitForm action, " +
                "with the URL pointing to a POST handler running on our sample server. " +
                "When the form is submitted, that handler receives a collection of form field names " +
                "and field values from the filled form, and sends it back in a simple HTML page. " +
                "If you download this sample, to successfully run it you will need to set up your own " +
                "handler, and change the Submit button action's URL to point to that handler.",
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
            btnSubmit.Widget.Events.Activate = new ActionSubmitForm("/Samples/HandleFormSubmitFields");
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
    }
}
