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
    // NOTE: This sample is obsolete as of GcPdf v3. Please see the new FormDataSubmit
    // sample for a better solution.
    //
    // This sample creates an AcroForm PDF that can be submitted to the server.
    // It relies on the server to put the submitted data into an XML,
    // import that XML into a PDF containing a similar form,
    // and send the form with loaded data back to the client.
    // Note that the produced PDF with filled form fields
    // is shown in the client browser's default PDF viewer.
    // The code is similar to FormSubmit.
    public class FormSubmitXml
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();

            var rc = Common.Util.AddNote(
                "Fill the fields in the form and click 'Submit' to send it back to the server. " +
                "The sample server will put the submitted data into an XML, feed that XML " +
                "to a PDF with a different but compatible form, and send the resulting form " +
                "filled with the submitted data back to your browser. " +
                "Note that the form with the submitted data is opened in the browser's default PDF viewer, " +
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
            btnSubmit.Widget.Events.Activate = new ActionSubmitForm("/Samples/HandleFormSubmitXml");
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
        // using the GcPdfDocument.ImportFormDataFromXML() method.
        //
        // This method is called by the samples controller when the form prepared by this sample
        // is submitted by the user. The samples controller parses the client response and builds
        // the 'values' collection filling it with the submitted field values, then calls
        // this method to prepare the XML, imports it into a newly created PDF, and returns
        // the resulting PDF to the controller, which sends it back to the client.
        public static Stream ImportFormData(List<FieldExportEntry> values)
        {
            GcPdfDocument pdf = new GcPdfDocument();
            using (var fs = new FileStream(Path.Combine("Resources", "PDFs", "ImportFormXML.pdf"), FileMode.Open, FileAccess.Read))
            {
                pdf.Load(fs);
                using (var ms = new MemoryStream())
                {
                    SaveFieldsToXML(values, ms);
                    ms.Seek(0, SeekOrigin.Begin);
                    pdf.ImportFormDataFromXML(ms);
                }
                var outMs = new MemoryStream();
                pdf.Save(outMs);
                outMs.Seek(0, SeekOrigin.Begin);
                return outMs;
            }
        }

        // Represents a form field and its value(s) for export to XML.
        public class FieldExportEntry
        {
            public string Name { get; set; }
            public List<string> Values { get; set; }
            // Note: this sample does not support child fields:
            // public List<FieldTreeNode> Children { get; set; }
        }

        // Saves the fields and their values to a stream.
        //
        // This method is similar to GcPdfDocument.ExportFormDataToXML(), with the following
        // imporant limitations:
        // - it does not support child fields (field.Children collection);
        // - it does not handle fields with names that are not valid XML names (xfdf:original).
        public static void SaveFieldsToXML(List<FieldExportEntry> values, Stream stream)
        {
            XmlWriterSettings xws = new XmlWriterSettings()
            {
                Indent = true,
                CloseOutput = false,
                Encoding = Encoding.UTF8,
            };
            using (XmlWriter xw = XmlWriter.Create(stream, xws))
            {
                xw.WriteStartElement("fields");
                xw.WriteAttributeString("xmlns", "xfdf", null, "http://ns.adobe.com/xfdf-transition/");
                foreach (var ftn in values)
                {
                    xw.WriteStartElement(ftn.Name);
                    foreach (var v in ftn.Values)
                    {
                        xw.WriteStartElement("value");
                        // NOTE: the values in the array are formed by the client PDF viewer,
                        // and it represents 'on' checkbox values as 'true', while ImportFormDataFromXML
                        // expects 'on' values to be represented as "Yes" (that's how ExportFormDataToXML
                        // works, similar to Acrobat). This here is a quick and dirty hack just for the
                        // sake of this sample:
                        xw.WriteString(v == "true" ? "Yes" : v);
                        xw.WriteEndElement();
                    }
                    xw.WriteEndElement();
                }
                xw.WriteEndElement();
            }
        }
    }
}
