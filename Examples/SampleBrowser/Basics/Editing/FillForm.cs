using System;
using System.IO;
using System.Drawing;
using System.Text;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Pdf.AcroForms;

namespace GcPdfWeb.Samples
{
    // This sample loads the form created by the FormFields sample,
    // loops through all form fields found in that file,
    // and modifies the values of input fields.
    // The log of what was done (showing old and new values) is added to the form page.
    public class FillForm
    {
        public void CreatePDF(Stream stream)
        {
            GcPdfDocument doc = new GcPdfDocument();

            // The original file stream must be kept open while working with the loaded PDF, see LoadPDF for details:
            using (var fs = new FileStream(Path.Combine("Resources", "PDFs", "FormFields.pdf"), FileMode.Open, FileAccess.Read))
            {
                doc.Load(fs);
                var page = doc.Pages.Last;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Log of updates made by the FillForm sample:\r\n");

                foreach (Field fld in doc.AcroForm.Fields)
                {
                    if (fld is CombTextField ctfld)
                    {
                        sb.Append($"CombTextField.Value was '{ctfld.Value}', ");
                        ctfld.Value = "Comb text";
                        sb.AppendLine($"now '{ctfld.Value}'.");
                    }
                    else if (fld is TextField tfld)
                    {
                        sb.Append($"TextField.Value was '{tfld.Value}', ");
                        tfld.Value = $"Text updated on {DateTime.Now}";
                        sb.AppendLine($"now '{tfld.Value}'.");
                    }
                    else if (fld is CheckBoxField cfld)
                    {
                        sb.Append($"CheckBoxField.Value was '{cfld.Value}', ");
                        cfld.Value = !cfld.Value;
                        sb.AppendLine($"now '{cfld.Value}'.");
                    }
                    else if (fld is RadioButtonField rbfld)
                    {
                        sb.Append($"RadioButtonField.Value was '{rbfld.Value}', ");
                        rbfld.Value = rbfld.Widgets.Count - 1;
                        sb.AppendLine($"now '{rbfld.Value}'.");
                    }
                    else if (fld is ComboBoxField cmbfld)
                    {
                        sb.Append($"ComboBoxField selection was '{cmbfld.Items[cmbfld.SelectedIndex].Text}', ");
                        cmbfld.SelectedIndex = cmbfld.Items.Count - 1;
                        sb.AppendLine($"now '{cmbfld.Items[cmbfld.SelectedIndex].Text}'.");
                    }
                    else if (fld is ListBoxField lbfld)
                    {
                        sb.Append($"ListBoxField selection was '{lbfld.Items[lbfld.SelectedIndex].Text}', ");
                        lbfld.SelectedIndex = lbfld.Items.Count - 1;
                        sb.AppendLine($"now '{lbfld.Items[lbfld.SelectedIndex].Text}'.");
                    }
                    else if (fld is SignatureField sfld)
                    {
                        sb.AppendLine("SignatureField found.");
                    }
                    else if (fld is PushButtonField btnfld)
                    {
                        sb.AppendLine($"PushButtonField '{btnfld.Widget.ButtonAppearance.Caption}' found.");
                    }
                    else
                    {
                        sb.AppendLine($"Field '{fld}' found/");
                    }
                }

                // Add a log of what we did at the bottom of the page:
                var tl = new TextLayout();
                tl.MaxWidth = page.Size.Width;
                tl.MaxHeight = page.Size.Height;
                tl.MarginLeft = tl.MarginRight = tl.MarginBottom = 80;
                tl.ParagraphAlignment = ParagraphAlignment.Far;
                tl.Append(sb.ToString(), new TextFormat() { Font = StandardFonts.Times, FontSize = 12 });
                tl.PerformLayout(true);
                var rc = tl.ContentRectangle;
                rc.Inflate(8, 8);
                page.Graphics.FillRectangle(rc, Color.LightYellow);
                page.Graphics.DrawRectangle(rc, Color.Orange);
                page.Graphics.DrawTextLayout(tl, PointF.Empty);

                // Done:
                doc.Save(stream);
            }
        }
    }
}
