//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
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
using GrapeCity.Documents.Pdf.Security;
using System.Security.Cryptography.X509Certificates;

namespace GcPdfWeb.Samples
{
    // This sample is almost the same as TimeSheet, with one significant difference:
    // unlike the other sample, in this one the filled form is digitally signed by
    // the employee, and the signed PDF is signed again by the supervisor using
    // incremental update (the only way to sign an already signed PDF while
    // preserving the validity of the first signature).
    //
    // NOTE: if you download this sample and run it locally on your own system,
    // you will need to have a valid license for it to work as expected, because
    // in an unlicensed version the automatically added nag page caption will
    // invalidate the employee's signature.
    public class TimeSheetIncremental
    {
        // Font collection to hold the fonts we need:
        private FontCollection _fc = new FontCollection();
        // The text layout used to render input fields when flattening the document:
        private TextLayout _inputTl = new TextLayout(72);
        // The text format used for input fields:
        private TextFormat _inputTf = new TextFormat();
        // Input fields margin:
        private float _inputMargin = 5;
        // Space for employee's signature:
        private RectangleF _empSignRect;
        //
        private Image _logo;

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
            using (var empSignedStream = FillEmployeeData(doc))
            {
                //
                // At this point 'empSignedStream' contains the form filled with employee's data and signed by them.
                //

                // Load the employee-signed document:
                doc.Load(empSignedStream);

                // Fill in supervisor data:
                var supName = "Jane Donahue";
                var supSignDate = DateTime.Now.ToShortDateString();
                SetFieldValue(doc, _Names.EmpSuper, supName);
                SetFieldValue(doc, _Names.SupSignDate, supSignDate);

                // Digitally sign the document on behalf of the supervisor:
                var pfxPath = Path.Combine("Resources", "Misc", "GcPdfTest.pfx");
                X509Certificate2 cert = new X509Certificate2(File.ReadAllBytes(pfxPath), "qq",
                    X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
                SignatureProperties sp = new SignatureProperties()
                {
                    Certificate = cert,
                    Location = "GcPdfWeb - TimeSheet Incremental",
                    SignerName = supName,
                    SignatureDigestAlgorithm = SignatureDigestAlgorithm.SHA512,
                    // Connect the signature field and signature props:
                    SignatureField = doc.AcroForm.Fields.First(f_ => f_.Name == _Names.SupSign) as SignatureField,
                };

                // Any changes to the document would invalidate the employee's signature, so we cannot do this:
                // supSign.Widget.ButtonAppearance.Caption = supName;
                //
                // Done, now save the document with supervisor signature:
                // NOTE: in order to not invalidate the employee's signature,
                // we MUST use incremental update here (which is true by default in Sign() method):
                doc.Sign(sp, stream);
                _logo.Dispose();
                return doc.Pages.Count;
            }
        }

        // Replaces any text fields in the document with regular text,
        // except the fields listed in 'excludeFields':
        private void FlattenDoc(GcPdfDocument doc, params string[] excludeFields)
        {
            foreach (var f in doc.AcroForm.Fields)
            {
                if (f is TextField fld && !excludeFields.Contains(fld.Name))
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
                if (doc.AcroForm.Fields[i] is TextField fld && !excludeFields.Contains(fld.Name))
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

            var tl = new TextLayout(g.Resolution) { FontCollection = _fc };

            tl.Append("TIME SHEET", new TextFormat() { FontName = "Segoe UI", FontSize = 18 });
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, ip);
            ip.Y += tl.ContentHeight + 15;

            _logo = Image.FromFile(Path.Combine("Resources", "ImagesBis", "AcmeLogo-vertical-250px.png"));
            var s = new SizeF(250f * 0.75f, 64f * 0.75f);
            g.DrawImage(_logo, new RectangleF(ip, s), null, ImageAlign.Default);
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
            // Employee signature:
            var r = fields3[0, 1];
            _empSignRect = new RectangleF(r.X + r.Width / 2, r.Y, r.Width / 2 - _inputMargin * 2, r.Height);
            SignatureField sf = new SignatureField() { Name = _Names.EmpSign };
            sf.Widget.Rect = new RectangleF(r.X + r.Width / 2, r.Y + _inputMargin, r.Width / 2 - _inputMargin * 2, r.Height - _inputMargin * 2);
            sf.Widget.Page = page;
            sf.Widget.BackColor = Color.LightSeaGreen;
            doc.AcroForm.Fields.Add(sf);
            drawField("DATE: ", fields3[1, 1], _Names.EmpSignDate);

            drawField("SUPERVISOR SIGNATURE: ", fields3[0, 2], null);
            // Supervisor signature:
            r = fields3[0, 2];
            sf = new SignatureField() { Name = _Names.SupSign };
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
            // Done:
            return cells;
        }

        // Fill in employee info and working hours with sample data:
        private Stream FillEmployeeData(GcPdfDocument doc)
        {
            // For the purposes of this sample, we fill the form with random data:
            var empName = "Jaime Smith";
            SetFieldValue(doc, _Names.EmpName, empName);
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

            // Digitally sign the document on behalf of the 'employee':
            var pfxPath = Path.Combine("Resources", "Misc", "JohnDoe.pfx");
            X509Certificate2 cert = new X509Certificate2(File.ReadAllBytes(pfxPath), "secret",
                X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            SignatureProperties sp = new SignatureProperties()
            {
                Certificate = cert,
                DocumentAccessPermissions = AccessPermissions.FormFillingAndAnnotations,
                Reason = "I confirm time sheet is correct.",
                Location = "TimeSheetIncremental sample",
                SignerName = empName,
            };

            // Connect the signature field and signature props:
            SignatureField empSign = doc.AcroForm.Fields.First(f_ => f_.Name == _Names.EmpSign) as SignatureField;
            sp.SignatureField = empSign;
            empSign.Widget.ButtonAppearance.Caption = empName;
            // Some browser PDF viewers do not show form fields, so we render a placeholder:
            empSign.Widget.Page.Graphics.DrawString("digitally signed", new TextFormat() { FontName = "Segoe UI", FontSize = 9 }, empSign.Widget.Rect);

            // We now 'flatten' the form: loop over document AcroForm's fields,
            // drawing their current values in place, and then remove the fields.
            // This produces a PDF with text fields' values as part of the regular
            // (non-editable) content (we leave fields filled by the supervisor):
            FlattenDoc(doc, _Names.EmpSuper, _Names.SupSignDate);

            // Done, now save the document with employee's signature:
            var ms = new MemoryStream();
            // Note that we do NOT use incremental update here (3rd parameter is false)
            // as this is not needed yet (but will be needed/used when signing by supervisor later):
            doc.Sign(sp, ms, false);
            return ms;
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
