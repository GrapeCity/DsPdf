using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Pdf.AcroForms;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Common;
using GrapeCity.Documents.Drawing;
using System.Security.Cryptography.X509Certificates;

namespace GcPdfWeb.Samples
{
    // This sample implements a scenario involving generating, filling and signing a time sheet:
    // - The first step is to generate a time sheet form (AcroForm PDF).
    //   The form contains fields for employee info, working times for a week,
    //   and employee's and supervisor's signatures.
    // - The next step in a real app would involve employees filling and signing the form.
    //   In this sample, we use some randomly generated data to fill the form on behalf
    //   of an employee.
    // - We then flatten the filled form - convert the text fields filled by the employee
    //   to regular text.
    // - Finally, we digitally sign the flattened document on behalf of the employee's
    //   supervisor, and save it.
    //
    // See also TimeSheetIncremental - it is essentially the same code, but uses
    // incremental update to digitally sign the document by both employee and supervisor.
    public class TimeSheet
    {
        // Font collection to hold the fonts we need:
        private FontCollection _fc = new FontCollection();
        // The text layout used to render input fields when flattening the document:
        private TextLayout _inputTl = new TextLayout();
        // The text format used for input fields:
        private TextFormat _inputTf = new TextFormat();
        // Input fields margin:
        private float _inputMargin = 5;
        // Space for employee's signature:
        private RectangleF _empSignRect;
        // This will hold the llst of images so we can dispose them after saving the document:
        private List<IDisposable> _disposables = new List<IDisposable>();

        // Main entry point of this sample:
        public int CreatePDF(Stream stream)
        {
            // Set up a font collection with the fonts we need:
            _fc.RegisterDirectory(Path.Combine("Resources", "Fonts"));
            // Set that font collection on input fields' text layout
            // (we will also set it on all text layouts that we'll use):
            _inputTl.FontCollection = _fc;
            // Set up layout and formatting for input fields:
            _inputTl.ParagraphAlignment = ParagraphAlignment.Center;
            _inputTf.FontName = "Segoe UI";
            _inputTf.FontSize = 12;
            _inputTf.FontBold = true;

            // Create the time sheet input form
            // (in a real-life scenario, we probably would only create it once,
            // and then re-use the form PDF):
            var doc = MakeTimeSheetForm();

            // At this point, 'doc' is an empty AcroForm. 
            // In a real-life app it would be distributed to employees
            // for them to fill and send back.
            FillEmployeeData(doc);

            //
            // At this point the form is filled with employee's data.
            //

            // Supervisor data (in a real app, these would probably be fetched from a db):
            var supName = "Jane Donahue";
            var supSignDate = DateTime.Now.ToShortDateString();
            SetFieldValue(doc, _Names.EmpSuper, supName);
            SetFieldValue(doc, _Names.SupSignDate, supSignDate);

            // The next step is to 'flatten' the form: we loop over document AcroForm's fields,
            // drawing their current values in place, and then remove the fields.
            // This produces a PDF with text fields' values as part of the regular (non-editable) content:
            FlattenDoc(doc);

            // Now we digitally sign the flattened document on behalf of the 'manager':
            var pfxPath = Path.Combine("Resources", "Misc", "GcPdfTest.pfx");
            X509Certificate2 cert = new X509Certificate2(File.ReadAllBytes(pfxPath), "qq",
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            SignatureProperties sp = new SignatureProperties();
            sp.Certificate = cert;
            sp.Location = "GcPdfWeb - TimeSheet sample";
            sp.SignerName = supName;

            // Connect the signature field and signature props:
            SignatureField supSign = doc.AcroForm.Fields.First(f_ => f_.Name == _Names.SupSign) as SignatureField;
            sp.SignatureField = supSign;
            supSign.Widget.ButtonAppearance.Caption = supName;
            // Some browser PDF viewers do not show form fields, so we render a placeholder:
            supSign.Widget.Page.Graphics.DrawString("digitally signed", new TextFormat() { FontName = "Segoe UI", FontSize = 9 }, supSign.Widget.Rect);

            // Done, now save the document with supervisor signature:
            doc.Sign(sp, stream);
            // Dispose images only after the document is saved:
            _disposables.ForEach(d_ => d_.Dispose());
            return doc.Pages.Count;
        }

        // Replaces any text fields in the document with regular text:
        private void FlattenDoc(GcPdfDocument doc)
        {
            foreach (var f in doc.AcroForm.Fields)
            {
                if (f is TextField fld)
                {
                    var w = fld.Widget;
                    var g = w.Page.Graphics;
                    _inputTl.Clear();
                    _inputTl.Append(fld.Value, _inputTf);
                    _inputTl.MaxHeight = w.Rect.Height;
                    _inputTl.PerformLayout(true);
                    g.DrawTextLayout(_inputTl, w.Rect.Location);
                }
            }
            for (int i = doc.AcroForm.Fields.Count - 1; i >= 0; --i)
                if (doc.AcroForm.Fields[i] is TextField)
                    doc.AcroForm.Fields.RemoveAt(i);
        }

        // Data field names:
        static class _Names
        {
            public static readonly string[] Dows = new string[]
            {
                "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat",
            };
            public const string EmpName = "empName";
            public const string EmpTitle = "empTitle";
            public const string EmpNum = "empNum";
            public const string EmpStatus = "empStatus";
            public const string EmpDep = "empDep";
            public const string EmpSuper = "empSuper";
            public static Dictionary<string, string[]> DtNames = new Dictionary<string, string[]>()
            {
                {"Sun", new string[] { "dtSun", "tSunStart", "tSunEnd", "tSunReg", "tSunOvr", "tSunTotal" } },
                {"Mon", new string[] { "dtMon", "tMonStart", "tMonEnd", "tMonReg", "tMonOvr", "tMonTotal" } },
                {"Tue", new string[] { "dtTue", "tTueStart", "tTueEnd", "tTueReg", "tTueOvr", "tTueTotal" } },
                {"Wed", new string[] { "dtWed", "tWedStart", "tWedEnd", "tWedReg", "tWedOvr", "tWedTotal" } },
                {"Thu", new string[] { "dtThu", "tThuStart", "tThuEnd", "tThuReg", "tThuOvr", "tThuTotal" } },
                {"Fri", new string[] { "dtFri", "tFriStart", "tFriEnd", "tFriReg", "tFriOvr", "tFriTotal" } },
                {"Sat", new string[] { "dtSat", "tSatStart", "tSatEnd", "tSatReg", "tSatOvr", "tSatTotal" } },
            };
            public const string TotalReg = "totReg";
            public const string TotalOvr = "totOvr";
            public const string TotalHours = "totHours";
            public const string EmpSign = "empSign";
            public const string EmpSignDate = "empSignDate";
            public const string SupSign = "supSign";
            public const string SupSignDate = "supSignDate";
        }

        // Creates the Time Sheet form:
        private GcPdfDocument MakeTimeSheetForm()
        {
            const float marginH = 72, marginV = 48;
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;
            var ip = new PointF(marginH, marginV);

            var tl = new TextLayout() { FontCollection = _fc };

            tl.Append("TIME SHEET", new TextFormat() { FontName = "Segoe UI", FontSize = 18 });
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, ip);
            ip.Y += tl.ContentHeight + 15;

            var logo = Image.FromFile(Path.Combine("Resources", "ImagesBis", "AcmeLogo-vertical-250px.png"));
            _disposables.Add(logo);
            var s = new SizeF(250f * 0.75f, 64f * 0.75f);
            g.DrawImage(logo, new RectangleF(ip, s), null, ImageAlign.Default);
            ip.Y += s.Height + 5;

            tl.Clear();
            tl.Append("Where Business meets Technology",
                new TextFormat() { FontName = "Segoe UI", FontItalic = true, FontSize = 10 });
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, ip);
            ip.Y += tl.ContentHeight + 15;

            tl.Clear();
            tl.Append("1901, Halford Avenue,\r\nSanta Clara, California – 95051-2553,\r\nUnited States",
                new TextFormat() { FontName = "Segoe UI", FontSize = 9 });
            tl.MaxWidth = page.Size.Width - marginH * 2;
            tl.TextAlignment = TextAlignment.Trailing;
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, ip);
            ip.Y += tl.ContentHeight + 25;

            var pen = new Pen(Color.Gray, 0.5f);

            var colw = (page.Size.Width - marginH * 2) / 2;
            var fields1 = DrawTable(ip,
                new float[] { colw, colw },
                new float[] { 30, 30, 30 },
                g, pen);

            var tf = new TextFormat() { FontName = "Segoe UI", FontSize = 9 };
            tl.ParagraphAlignment = ParagraphAlignment.Center;
            tl.TextAlignment = TextAlignment.Leading;
            tl.MarginLeft = tl.MarginRight = tl.MarginTop = tl.MarginBottom = 4;

            // t_ - caption
            // b_ - bounds
            // f_ - field name, null means no field
            Action<string, RectangleF, string> drawField = (t_, b_, f_) =>
            {
                float tWidth;
                if (!string.IsNullOrEmpty(t_))
                {
                    tl.Clear();
                    tl.MaxHeight = b_.Height;
                    tl.MaxWidth = b_.Width;
                    tl.Append(t_, tf);
                    tl.PerformLayout(true);
                    g.DrawTextLayout(tl, b_.Location);
                    tWidth = tl.ContentRectangle.Right;
                }
                else
                    tWidth = 0;
                if (!string.IsNullOrEmpty(f_))
                {
                    var fld = new TextField() { Name = f_ };
                    fld.Widget.Page = page;
                    fld.Widget.Rect = new RectangleF(
                        b_.X + tWidth + _inputMargin, b_.Y + _inputMargin, 
                        b_.Width - tWidth - _inputMargin * 2, b_.Height - _inputMargin * 2);
                    fld.Widget.TextFormat = _inputTf;
                    fld.Widget.Border.Color = Color.LightSlateGray;
                    fld.Widget.Border.Width = 0.5f;
                    doc.AcroForm.Fields.Add(fld);
                }
            };

            drawField("EMPLOYEE NAME: ", fields1[0, 0], _Names.EmpName);
            drawField("TITLE: ", fields1[1, 0], _Names.EmpTitle);
            drawField("EMPLOYEE NUMBER: ", fields1[0, 1], _Names.EmpNum);
            drawField("STATUS: ", fields1[1, 1], _Names.EmpStatus);
            drawField("DEPARTMENT: ", fields1[0, 2], _Names.EmpDep);
            drawField("SUPERVISOR: ", fields1[1, 2], _Names.EmpSuper);

            ip.Y = fields1[0, 2].Bottom;

            float col0 = 100;
            colw = (page.Size.Width - marginH * 2 - col0) / 5;
            float rowh = 25;
            var fields2 = DrawTable(ip,
                new float[] { col0, colw, colw, colw, colw, colw },
                new float[] { 50, rowh, rowh, rowh, rowh, rowh, rowh, rowh, rowh },
                g, pen);

            tl.ParagraphAlignment = ParagraphAlignment.Far;
            drawField("DATE", fields2[0, 0], null);
            drawField("START TIME", fields2[1, 0], null);
            drawField("END TIME", fields2[2, 0], null);
            drawField("REGULAR HOURS", fields2[3, 0], null);
            drawField("OVERTIME HOURS", fields2[4, 0], null);
            tf.FontBold = true;
            drawField("TOTAL HOURS", fields2[5, 0], null);
            tf.FontBold = false;
            tl.ParagraphAlignment = ParagraphAlignment.Center;
            tf.ForeColor = Color.Gray;
            for (int i = 0; i < 7; ++i)
                drawField(_Names.Dows[i], fields2[0, i + 1], _Names.DtNames[_Names.Dows[i]][0]);
            tf.ForeColor = Color.Black;
            for (int row = 1; row <= 7; ++row)
                for (int col = 1; col <= 5; ++col)
                    drawField(null, fields2[col, row], _Names.DtNames[_Names.Dows[row - 1]][col]);

            tf.FontBold = true;
            drawField("WEEKLY TOTALS", fields2[0, 8], null);
            tf.FontBold = false;

            drawField(null, fields2[3, 8], _Names.TotalReg);
            drawField(null, fields2[4, 8], _Names.TotalOvr);
            drawField(null, fields2[5, 8], _Names.TotalHours);

            ip.Y = fields2[0, 8].Bottom;

            col0 = 72 * 4;
            colw = page.Size.Width - marginH * 2 - col0;
            var fields3 = DrawTable(ip,
                new float[] { col0, colw },
                new float[] { rowh + 10, rowh, rowh },
                g, pen);

            drawField("EMPLOYEE SIGNATURE: ", fields3[0, 1], null);
            var r = fields3[0, 1];
            _empSignRect = new RectangleF(r.X + r.Width / 2, r.Y, r.Width / 2 - _inputMargin * 2, r.Height);
            // For a digital employee signature, uncomment this code:
            /*
            SignatureField sf = new SignatureField();
            sf.Name = _Names.EmpSign;
            sf.Widget.Rect = new RectangleF(r.X + r.Width / 2, r.Y + _inputMargin, r.Width / 2 - _inputMargin * 2, r.Height - _inputMargin * 2);
            sf.Widget.Page = page;
            sf.Widget.BackColor = Color.LightSeaGreen;
            doc.AcroForm.Fields.Add(sf);
            */
            drawField("DATE: ", fields3[1, 1], _Names.EmpSignDate);

            drawField("SUPERVISOR SIGNATURE: ", fields3[0, 2], null);
            // Supervisor signature:
            r = fields3[0, 2];
            SignatureField sf = new SignatureField();
            sf.Name = _Names.SupSign;
            sf.Widget.Rect = new RectangleF(r.X + r.Width / 2, r.Y + _inputMargin, r.Width / 2 - _inputMargin * 2, r.Height - _inputMargin * 2);
            sf.Widget.Page = page;
            sf.Widget.BackColor = Color.LightYellow;
            doc.AcroForm.Fields.Add(sf);
            drawField("DATE: ", fields3[1, 2], _Names.SupSignDate);

            // Done:
            return doc;
        }

        // Simple table drawing method. Returns the array of table cell rectangles.
        private RectangleF[,] DrawTable(PointF loc, float[] widths, float[] heights, GcGraphics g, Pen p)
        {
            if (widths.Length == 0 || heights.Length == 0)
                throw new Exception("Table must have some columns and rows.");

            RectangleF[,] cells = new RectangleF[widths.Length, heights.Length];
            var r = new RectangleF(loc, new SizeF(widths.Sum(), heights.Sum()));
            // Draw left borders (except for 1st one):
            float x = loc.X;
            for (int i = 0; i < widths.Length; ++i)
            {
                for (int j = 0; j < heights.Length; ++j)
                {
                    cells[i, j].X = x;
                    cells[i, j].Width = widths[i];
                }
                if (i > 0)
                    g.DrawLine(x, r.Top, x, r.Bottom, p);
                x += widths[i];
            }
            // Draw top borders (except for 1st one):
            float y = loc.Y;
            for (int j = 0; j < heights.Length; ++j)
            {
                for (int i = 0; i < widths.Length; ++i)
                {
                    cells[i, j].Y = y;
                    cells[i, j].Height = heights[j];
                }
                if (j > 0)
                    g.DrawLine(r.Left, y, r.Right, y, p);
                y += heights[j];
            }
            // Draw outer border:
            g.DrawRectangle(r, p);
            //
            return cells;
        }

        // Fill in employee info and working hours with sample data:
        private void FillEmployeeData(GcPdfDocument doc)
        {
            // For the purposes of this sample, we fill the form with random data:
            SetFieldValue(doc, _Names.EmpName, "Jaime Smith");
            SetFieldValue(doc, _Names.EmpNum, "12345");
            SetFieldValue(doc, _Names.EmpDep, "Research & Development");
            SetFieldValue(doc, _Names.EmpTitle, "Senior Developer");
            SetFieldValue(doc, _Names.EmpStatus, "Full Time");
            var rand = new Random((int)DateTime.Now.Ticks);
            DateTime workday = DateTime.Now.AddDays(-15);
            while (workday.DayOfWeek != DayOfWeek.Sunday)
                workday = workday.AddDays(1);
            TimeSpan wkTot = TimeSpan.Zero, wkReg = TimeSpan.Zero, wkOvr = TimeSpan.Zero;
            for (int i = 0; i < 7; ++i)
            {
                // Start time:
                var start = new DateTime(workday.Year, workday.Month, workday.Day, rand.Next(6, 12), rand.Next(0, 59), 0);
                SetFieldValue(doc, _Names.DtNames[_Names.Dows[i]][0], start.ToShortDateString());
                SetFieldValue(doc, _Names.DtNames[_Names.Dows[i]][1], start.ToShortTimeString());
                // End time:
                var end = start.AddHours(rand.Next(8, 14)).AddMinutes(rand.Next(0, 59));
                SetFieldValue(doc, _Names.DtNames[_Names.Dows[i]][2], end.ToShortTimeString());
                var tot = end - start;
                var reg = TimeSpan.FromHours((start.DayOfWeek != DayOfWeek.Saturday && start.DayOfWeek != DayOfWeek.Sunday) ? 8 : 0);
                var ovr = tot.Subtract(reg);
                SetFieldValue(doc, _Names.DtNames[_Names.Dows[i]][3], reg.ToString(@"hh\:mm"));
                SetFieldValue(doc, _Names.DtNames[_Names.Dows[i]][4], ovr.ToString(@"hh\:mm"));
                SetFieldValue(doc, _Names.DtNames[_Names.Dows[i]][5], tot.ToString(@"hh\:mm"));
                wkTot += tot;
                wkOvr += ovr;
                wkReg += reg;
                //
                workday = workday.AddDays(1);
            }
            SetFieldValue(doc, _Names.TotalReg, wkReg.TotalHours.ToString("F"));
            SetFieldValue(doc, _Names.TotalOvr, wkOvr.TotalHours.ToString("F"));
            SetFieldValue(doc, _Names.TotalHours, wkTot.TotalHours.ToString("F"));
            SetFieldValue(doc, _Names.EmpSignDate, workday.ToShortDateString());

            // 'Sign' the time sheet on behalf of the employee by drawing an image representing the signature
            // (see TimeSheetIncremental for digitally signing by both employee and supervisor):
            var empSignImage = Image.FromFile(Path.Combine("Resources", "ImagesBis", "signature.png"));
            _disposables.Add(empSignImage);
            var ia = new ImageAlign(ImageAlignHorz.Center, ImageAlignVert.Center, true, true, true, false, false)
            { KeepAspectRatio = true };
            doc.Pages[0].Graphics.DrawImage(empSignImage, _empSignRect, null, ia);
        }

        // Sets the value of a field with the specified name:
        private void SetFieldValue(GcPdfDocument doc, string name, string value)
        {
            var fld = doc.AcroForm.Fields.First(f_ => f_.Name == name);
            if (fld != null)
                fld.Value = value;
        }
    }
}
