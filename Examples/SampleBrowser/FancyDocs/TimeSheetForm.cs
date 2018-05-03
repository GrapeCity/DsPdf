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
    // This sample generates a PDF AcroForm representing a time sheet.
    // The same code is used to generated the time sheet in the TimeSheet use case sample.
    public class TimeSheetForm
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

        // Main entry point of this sample:
        public void CreatePDF(Stream stream)
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

            // Done:
            doc.Save(stream);
        }

        // data fields names:
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
            SignatureField sfEmp = new SignatureField();
            sfEmp.Name = _Names.EmpSign;
            sfEmp.Widget.Rect = new RectangleF(r.X + r.Width / 2, r.Y + _inputMargin, r.Width / 2 - _inputMargin * 2, r.Height - _inputMargin * 2);
            sfEmp.Widget.Page = page;
            sfEmp.Widget.BackColor = Color.LightSeaGreen;
            doc.AcroForm.Fields.Add(sfEmp);
            drawField("DATE: ", fields3[1, 1], _Names.EmpSignDate);

            drawField("SUPERVISOR SIGNATURE: ", fields3[0, 2], null);
            r = fields3[0, 2];
            SignatureField sfSup = new SignatureField();
            sfSup.Name = _Names.SupSign;
            sfSup.Widget.Rect = new RectangleF(r.X + r.Width / 2, r.Y + _inputMargin, r.Width / 2 - _inputMargin * 2, r.Height - _inputMargin * 2);
            sfSup.Widget.Page = page;
            sfSup.Widget.BackColor = Color.LightYellow;
            doc.AcroForm.Fields.Add(sfSup);
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
                // start time:
                var start = new DateTime(workday.Year, workday.Month, workday.Day, rand.Next(6, 12), rand.Next(0, 59), 0);
                SetFieldValue(doc, _Names.DtNames[_Names.Dows[i]][0], start.ToShortDateString());
                SetFieldValue(doc, _Names.DtNames[_Names.Dows[i]][1], start.ToShortTimeString());
                // end time:
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

            // 'Sign' the image on behalf of the employee by drawing an image representing the signature
            // (alternatively, this could be a digital signature field like the super's signature below):
            var empSignImage = Image.FromFile(Path.Combine("Resources", "ImagesBis", "signature.png"));
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
