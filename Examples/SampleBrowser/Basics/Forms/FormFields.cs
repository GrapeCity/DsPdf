using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Pdf.AcroForms;
using GrapeCity.Documents.Pdf.Annotations;
using GrapeCity.Documents.Pdf.Actions;

namespace GcPdfWeb.Samples
{
    // This sample demonstrates how to create the various AcroForm fields
    // such as textbox, checkbox, push buttons and so on.
    public class FormFields
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;
            TextFormat tf = new TextFormat();
            tf.Font = StandardFonts.Times;
            tf.FontSize = 14;
            PointF ip = new PointF(72, 72);
            float fldOffset = 72 * 2;
            float fldHeight = tf.FontSize * 1.2f;
            float dY = 32;

            // Text field:
            g.DrawString("Text field:", tf, ip);
            var fldText = new TextField();
            fldText.Value = "Initial TextField value";
            fldText.Widget.Page = page;
            fldText.Widget.Rect = new RectangleF(ip.X + fldOffset, ip.Y, 72 * 3, fldHeight);
            fldText.Widget.TextFormat.Font = tf.Font;
            fldText.Widget.TextFormat.FontSize = tf.FontSize;
            doc.AcroForm.Fields.Add(fldText);
            ip.Y += dY;

            // Checkbox:
            g.DrawString("Checkbox:", tf, ip);
            var fldCheckbox = new CheckBoxField();
            fldCheckbox.Value = true;
            fldCheckbox.Widget.Page = page;
            fldCheckbox.Widget.Rect = new RectangleF(ip.X + fldOffset, ip.Y, fldHeight, fldHeight);
            doc.AcroForm.Fields.Add(fldCheckbox);
            ip.Y += dY;

            // Radio button:
            g.DrawString("Radio button:", tf, ip);
            var fldRadio = new RadioButtonField();
            fldRadio.Value = 1;
            fldRadio.Widgets.Add(new WidgetAnnotation(page, new RectangleF(ip.X + fldOffset, ip.Y, fldHeight, fldHeight)));
            fldRadio.Widgets.Add(new WidgetAnnotation(page, new RectangleF(ip.X + fldOffset, ip.Y + fldHeight * 1.2f, fldHeight, fldHeight)));
            fldRadio.Widgets.Add(new WidgetAnnotation(page, new RectangleF(ip.X + fldOffset, ip.Y + (fldHeight * 1.2f) * 2, fldHeight, fldHeight)));
            doc.AcroForm.Fields.Add(fldRadio);
            ip.Y = fldRadio.Widgets[fldRadio.Widgets.Count - 1].Rect.Y + dY;

            // CombTextField:
            g.DrawString("CombText field:", tf, ip);
            var fldCombText = new CombTextField();
            fldCombText.Value = "123";
            fldCombText.Widget.TextFormat.FontSize = 12;
            fldCombText.Widget.Rect = new RectangleF(ip.X + fldOffset, ip.Y, 72 * 3, fldHeight);
            fldCombText.Widget.Page = page;
            doc.AcroForm.Fields.Add(fldCombText);
            ip.Y += dY;

            // Combo-box:
            g.DrawString("Combo box:", tf, ip);
            var fldComboBox = new ComboBoxField();
            fldComboBox.Items.Add(new ChoiceFieldItem("ComboBox Choice 1"));
            fldComboBox.Items.Add(new ChoiceFieldItem("ComboBox Choice 2"));
            fldComboBox.Items.Add(new ChoiceFieldItem("ComboBox Choice 3"));
            fldComboBox.SelectedIndex = 1;
            fldComboBox.Widget.Rect = new RectangleF(ip.X + fldOffset, ip.Y, 72 * 3, fldHeight);
            fldComboBox.Widget.Page = page;
            doc.AcroForm.Fields.Add(fldComboBox);
            ip.Y += dY;

            // List box:
            g.DrawString("List box:", tf, ip);
            ListBoxField fldListBox = new ListBoxField();
            fldListBox.Items.Add(new ChoiceFieldItem("ListBox Choice 1"));
            fldListBox.Items.Add(new ChoiceFieldItem("ListBox Choice 2"));
            fldListBox.Items.Add(new ChoiceFieldItem("ListBox Choice 3"));
            fldListBox.SelectedIndexes = new int[] { 0, 2 };
            fldListBox.MultiSelect = true;
            fldListBox.CommitOnSelChange = true;
            fldListBox.Widget.Rect = new RectangleF(ip.X + fldOffset, ip.Y, 100, 50);
            fldListBox.Widget.Page = page;
            doc.AcroForm.Fields.Add(fldListBox);
            ip.Y = fldListBox.Widget.Rect.Bottom - fldHeight + dY;

            // Signature field:
            g.DrawString("Signature field:", tf, ip);
            var fldSignature = new SignatureField();
            fldSignature.AlternateName = "All fields locked when the document is signed";
            fldSignature.LockedFields = new SignatureLockedFields();
            fldSignature.Widget.Rect = new RectangleF(ip.X + fldOffset, ip.Y, 72 * 2, 72 - dY);
            fldSignature.Widget.TextFormat.FontSize = 8;
            fldSignature.Widget.ButtonAppearance.Caption = "Click to sign";
            fldSignature.Widget.Border = new Border() { Width = 0.5f, Color = Color.DarkSeaGreen };
            fldSignature.Widget.Page = page;
            doc.AcroForm.Fields.Add(fldSignature);
            ip.Y += 72 - fldHeight;

            // Buttons:
            g.DrawString("Push buttons:", tf, ip);

            // Submit form button:
            var btnSubmit = new PushButtonField();
            btnSubmit.Widget.Rect = new RectangleF(ip.X + fldOffset, ip.Y, 72, fldHeight);
            btnSubmit.Widget.ButtonAppearance.Caption = "Submit";
            btnSubmit.Widget.Highlighting = HighlightingMode.Invert;
            btnSubmit.Widget.Events.Activate = new ActionSubmitForm("Sample Form Submit URI");
            btnSubmit.Widget.Page = page;
            doc.AcroForm.Fields.Add(btnSubmit);
            // ip.Y += dY;

            // Reset form button:
            var btnReset = new PushButtonField();
            btnReset.Widget.Rect = new RectangleF(ip.X + fldOffset + 72 * 1.5f, ip.Y, 72, fldHeight);
            btnReset.Widget.ButtonAppearance.Caption = "Reset";
            btnReset.Widget.Highlighting = HighlightingMode.Invert;
            btnReset.Widget.Events.Activate = new ActionResetForm();
            btnReset.Widget.Page = page;
            doc.AcroForm.Fields.Add(btnReset);
            ip.Y += dY;

            // Done:
            doc.Save(stream);
        }
    }
}
