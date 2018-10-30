using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Common;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Pdf.Annotations;
using GrapeCity.Documents.Pdf.AcroForms;

namespace GcPdfWeb.Samples.OrderReturnForm
{
    // Creates a "Goods return or exchange form" AcroForm with multiple input fields and a complex layout.
    public class GoodsReturnForm
    {
        // Page margins:
        const float MarginLeft = 32;
        const float MarginTop = 32;
        const float MarginRight = 32;
        const float MarginBottom = 32;
        //
        const float TableCaptionHeight = 20;
        readonly float TableSampleHeight = Textbox.Height;
        const float SmallTextVOff = -0.5f;
        // Section delimiting line:
        float CaptionLineThickness = 2.5f;
        // Struct to hold a text style:
        struct TextStyle
        {
            public Font Font;
            public float FontSize;
            public Color ForeColor;
            public float GlyphAdvanceFactor;
        }
        // Various styles used throughout the form:
        static TextStyle TsTitle = new TextStyle()
        {
            Font = Font.FromFile(Path.Combine("Resources", "Fonts", "SitkaB.ttc")),
            FontSize = 30,
            ForeColor = Color.FromArgb(0xff, 0x3b, 0x5c, 0xaa),
            GlyphAdvanceFactor = 0.93f,
        };
        static TextStyle TsCaption = new TextStyle()
        {
            Font = TsTitle.Font,
            FontSize = 14,
            ForeColor = Color.FromArgb(0xff, 0x3b, 0x5c, 0xaa),
            GlyphAdvanceFactor = 0.93f,
        };
        static TextStyle TsBold = new TextStyle()
        {
            Font = Font.FromFile(Path.Combine("Resources", "Fonts", "arialbd.ttf")),
            FontSize = 9,
            ForeColor = Color.Black,
            GlyphAdvanceFactor = 1,
        };
        static TextStyle TsNormal = new TextStyle()
        {
            Font = Font.FromFile(Path.Combine("Resources", "Fonts", "arial.ttf")),
            FontSize = 8f,
            ForeColor = Color.Black,
            GlyphAdvanceFactor = 0.922f,
        };
        static TextStyle TsSmall = new TextStyle()
        {
            Font = TsNormal.Font,
            FontSize = 5,
            ForeColor = Color.FromArgb(0x0F, 0x0F, 0x0F),
            GlyphAdvanceFactor = 1.1f,
        };
        // Input field styles:
        struct Textbox
        {
            static public Font Font = TsNormal.Font;
            static public float FontSize = 12;
            static public float Height;
            static public float BaselineOffset;
            static public float LabelSpacing = 2;
        }
        struct Checkbox
        {
            static public Font Font = TsNormal.Font;
            static public float FontSize = TsNormal.FontSize - 2;
            static public float Height;
            static public float BaselineOffset;
            static public float LabelSpacing = 3;
        }
        // The document being created:
        private GcPdfDocument _doc;
        // Insertion point:
        private PointF _ip = new PointF(MarginLeft, MarginTop);
        // If non-null, DrawText use this to align text to last baseline:
        private float? _lastBaselineOffset = null;
        // Shortcuts to current values:
        private int CurrPageIdx => _doc.Pages.Count - 1;
        private Page CurrPage => _doc.Pages[CurrPageIdx];
        private GcGraphics CurrGraphics => CurrPage.Graphics;

        // Static ctor:
        static GoodsReturnForm()
        {
            // Init Textbox:
            TextLayout tl = new TextLayout() { Resolution = 72 };
            tl.Append("Qwerty");
            tl.DefaultFormat.Font = Textbox.Font;
            tl.DefaultFormat.FontSize = Textbox.FontSize;
            tl.PerformLayout(true);
            Textbox.Height = tl.ContentHeight;
            Textbox.BaselineOffset = tl.Lines[0].GlyphRuns[0].BaselineOffset;
            // Init Checkbox:
            tl.Clear();
            tl.Append("Qwerty");
            tl.DefaultFormat.Font = Checkbox.Font;
            tl.DefaultFormat.FontSize = Checkbox.FontSize;
            tl.PerformLayout(true);
            Checkbox.Height = tl.ContentHeight;
            Checkbox.BaselineOffset = tl.Lines[0].GlyphRuns[0].BaselineOffset;
        }

        // The main entry point:
        public void CreatePDF(Stream stream)
        {
            Acme();
            _doc.Save(stream);
        }

        // Sets or advances the insertion point vertically:
        private void SetY(float? abs, float? offset)
        {
            if (abs.HasValue)
                _ip.Y = abs.Value;
            if (offset.HasValue)
                _ip.Y += offset.Value;
            _lastBaselineOffset = null;
        }

        // Creates the PDF form:
        private void Acme()
        {
            _doc = new GcPdfDocument();
            _doc.NewPage();
            var pageWidth = CurrPage.Size.Width;

            // Main caption:
            SetY(null, -2);
            var cr = DrawText("ACME Inc.", TsTitle);
            SetY(null, _lastBaselineOffset - CaptionLineThickness / 2);
            DrawGreenLine(MarginLeft, cr.Left - CaptionLineThickness);
            DrawGreenLine(cr.Right + CaptionLineThickness, pageWidth - MarginRight);

            // 'return and exchange form':
            SetY(cr.Bottom, 10);
            cr = DrawText("Return and Exchange Form", TsCaption);

            SetY(null, CaptionLineThickness + 14);
            cr = DrawText("Please type in the appropriate information below, then print this form.", TsBold);
            _ip.X = pageWidth - 150;
            cr = DrawText("Have Any Questions?", TsBold);

            SetY(null, 10);
            _ip.X = MarginLeft;
            cr = DrawText("(Or you may print the form and complete it by hand.)", TsNormal);
            _ip.X = pageWidth - 150;
            cr = DrawText("Please call us at 800-123-4567.", TsNormal);

            // Step 1 - line 1:
            SetY(null, 18);
            _ip.X = MarginLeft;
            cr = DrawText("Step 1", TsCaption);
            _ip.X = cr.Right + 10;
            cr = DrawText("Original Order #", TsBold);
            _ip.X = cr.Right + 4;
            cr = DrawText("(if available):", TsNormal);
            _ip.X = cr.Right + Textbox.LabelSpacing;
            cr = DrawTextbox(120);
            _ip.X = cr.Right + 6;
            cr = DrawText("Estimated Order Date:", TsBold);
            _ip.X = cr.Right + Textbox.LabelSpacing;
            cr = DrawTextbox(pageWidth - MarginRight - _ip.X);
            SetY(null, 17);
            DrawGreenLine();
            // Step 1 - line 2:
            SetY(null, 10);
            _ip.X = MarginLeft;
            cr = DrawText("Originally Purchased by:", TsBold);
            _ip.X = cr.Right + 20;
            cr = DrawCheckbox("Address Change");
            float col1right = pageWidth / 2 - 10;
            float col2left = col1right + 20;
            _ip.X = col2left;
            cr = DrawText("Send Refund or Exchange to:", TsBold);
            _ip.X = cr.Right + 2;
            cr = DrawText("(If different from left)", TsNormal);
            // Step 1 - line 3:
            SetY(cr.Bottom, 10);
            _ip.X = MarginLeft;
            cr = DrawText("Name:", TsNormal);
            _ip.X = cr.Right + Textbox.LabelSpacing;
            cr = DrawTextbox(col1right - _ip.X);
            _ip.X = col2left;
            cr = DrawText("Name:", TsNormal);
            _ip.X = cr.Right + Textbox.LabelSpacing;
            cr = DrawTextbox(pageWidth - MarginRight - _ip.X);
            // Step 1 - line 4:
            SetY(cr.Bottom, 4 + 4);
            _ip.X = MarginLeft;
            cr = DrawText("Address:", TsNormal);
            _ip.X = cr.Right + Textbox.LabelSpacing;
            cr = DrawTextbox(col1right - _ip.X);
            _ip.X = col2left;
            cr = DrawText("Address:", TsNormal);
            _ip.X = cr.Right + Textbox.LabelSpacing;
            cr = DrawTextbox(pageWidth - MarginRight - _ip.X);
            // Step 1 - line 5:
            SetY(cr.Bottom, 4 + 0.5f);
            _ip.X = MarginLeft;
            cr = DrawTextbox(col1right - _ip.X);
            _ip.X = col2left;
            cr = DrawTextbox(pageWidth - MarginRight - _ip.X);
            // Step 1 - line 6 (city state zip):
            SetY(cr.Bottom, 4 + 0.5f);
            _ip.X = MarginLeft;
            cr = DrawTextbox(160);
            _ip.X = cr.Right + 4;
            float oState = _ip.X - MarginLeft;
            cr = DrawTextbox(40);
            _ip.X = cr.Right + 4;
            float oZip = _ip.X - MarginLeft;
            cr = DrawTextbox(col1right - _ip.X);
            //
            _ip.X = col2left;
            cr = DrawTextbox(160);
            _ip.X = cr.Right + 4;
            cr = DrawTextbox(40);
            _ip.X = cr.Right + 4;
            cr = DrawTextbox(pageWidth - MarginRight - _ip.X);
            // small text
            SetY(cr.Bottom, SmallTextVOff);
            _ip.X = MarginLeft;
            cr = DrawText("(City)", TsSmall);
            _ip.X = MarginLeft + oState;
            cr = DrawText("(State)", TsSmall);
            _ip.X = MarginLeft + oZip;
            cr = DrawText("(Zip)", TsSmall);
            //
            _ip.X = col2left;
            cr = DrawText("(City)", TsSmall);
            _ip.X = col2left + oState;
            cr = DrawText("(State)", TsSmall);
            _ip.X = col2left + oZip;
            cr = DrawText("(Zip)", TsSmall);
            // Step 1 - line 7 (daytime):
            SetY(cr.Bottom, 4 - 0.5f);
            _ip.X = MarginLeft;
            cr = DrawText("Phone: (", TsNormal);
            _ip.X = cr.Right;
            cr = DrawTextbox(30);
            _ip.X = cr.Right;
            cr = DrawText(")", TsNormal);
            _ip.X += 3;
            cr = DrawTextbox(80);
            float oDay = cr.Left - MarginLeft + 10;
            // (evening)
            _ip.X = cr.Right + 3;
            cr = DrawText("(", TsNormal);
            _ip.X = cr.Right;
            cr = DrawTextbox(30);
            _ip.X = cr.Right;
            cr = DrawText(")", TsNormal);
            _ip.X += 3;
            cr = DrawTextbox(col1right - _ip.X);
            float oEve = cr.Left - MarginLeft + 10;
            // 
            _ip.X = col2left;
            cr = DrawText("Phone: (", TsNormal);
            _ip.X = cr.Right;
            cr = DrawTextbox(30);
            _ip.X = cr.Right;
            cr = DrawText(")", TsNormal);
            _ip.X += 3;
            cr = DrawTextbox(80);
            // (evening)
            _ip.X = cr.Right + 3;
            cr = DrawText("(", TsNormal);
            _ip.X = cr.Right;
            cr = DrawTextbox(30);
            _ip.X = cr.Right;
            cr = DrawText(")", TsNormal);
            _ip.X += 3;
            cr = DrawTextbox(pageWidth - MarginRight - _ip.X);
            // small text
            SetY(cr.Bottom, SmallTextVOff);
            _ip.X = MarginLeft + oDay;
            cr = DrawText("(Daytime)", TsSmall);
            _ip.X = MarginLeft + oEve;
            cr = DrawText("(Evening)", TsSmall);
            _ip.X = col2left + oDay;
            cr = DrawText("(Daytime)", TsSmall);
            _ip.X = col2left + oEve;
            cr = DrawText("(Evening)", TsSmall);
            // Step 1 - email
            SetY(cr.Bottom, 4 - 0.5f);
            _ip.X = MarginLeft;
            cr = DrawText("Email Address:", TsNormal);
            _ip.X = cr.Right + Textbox.LabelSpacing;
            cr = DrawTextbox(col1right - _ip.X);
            _ip.X = col2left;
            cr = DrawText("Email Address:", TsNormal);
            _ip.X = cr.Right + Textbox.LabelSpacing;
            cr = DrawTextbox(pageWidth - MarginRight - _ip.X);
            // Options:
            SetY(null, 16);
            _ip.X = MarginLeft;
            cr = DrawText("Please select one of the following options:", TsBold);
            SetY(cr.Bottom, 2);
            cr = DrawCheckbox("Exchange for another item(s).");
            SetY(cr.Bottom, 2);
            cr = DrawCheckbox("Send me an ACME Gift Card for the amount of the refund.");
            SetY(cr.Bottom, 2);
            cr = DrawCheckbox("Reimburse my original method of payment. " +
                "(Gift recipients who select this option will receive a merchandise only gift card.)");

            // Step 2:
            SetY(null, 18);
            _ip.X = MarginLeft;
            cr = DrawText("Step 2–Returns", TsCaption);
            _ip.X = cr.Right + 10;
            cr = DrawText("In the form below please indicate the item(s) you are returning, " +
                "including a reason code.", TsNormal);
            SetY(null, 17);
            DrawGreenLine();
            SetY(null, 10);
            cr = DrawReturnsTable();
            SetY(cr.Bottom, 10);
            cr = DrawReasonCodes();

            // Step 3:
            SetY(null, 25);
            _ip.X = MarginLeft;
            cr = DrawText("Step 3–Exchanges", TsCaption);
            _ip.X = cr.Right + 10;
            SetY(null, -5);
            cr = DrawText(
                "For the fastest service, call Customer Service at 800-123-4567 to request a QuickExchange " +
                "or place a new order online or by phone. We'll ship your new item right away. " +
                "Note: If you use our QuickExchange option, you do not need to fill out Step 3.",
                TsNormal);
            SetY(null, 22);
            DrawGreenLine();

            SetY(null, 10);
            cr = DrawExchangesTable();

            // Step 4:
            SetY(null, 18);
            _ip.X = MarginLeft;
            cr = DrawText("Step 4", TsCaption);
            SetY(null, 17);
            DrawGreenLine();

            SetY(null, 10);
            _ip.X = MarginLeft;
            float oCc = col2left - 30;
            cr = DrawText("Method of Payment:", TsBold);
            _ip.X = oCc;
            cr = DrawText("Credit Card Information:", TsBold);
            SetY(cr.Bottom, 2);
            _ip.X = MarginLeft;
            cr = DrawText("If the total of your exchange or new order exceeds the value of your\r\n" +
                "return, please provide a method of payment. (Select one)", TsNormal);
            _ip.X = oCc;
            cr = DrawCheckbox("ACME® Visa®");
            float oCcOff = 90;
            _ip.X += oCcOff;
            cr = DrawCheckbox("MasterCard®");
            _ip.X += oCcOff;
            cr = DrawCheckbox("JCB Card™");

            SetY(cr.Bottom, 2);
            _ip.X = oCc;
            cr = DrawCheckbox("VISA");
            _ip.X += oCcOff;
            cr = DrawCheckbox("American Express");
            _ip.X += oCcOff;
            cr = DrawCheckbox("Discover®/Novus® Cards");

            SetY(cr.Bottom, 4);
            _ip.X = MarginLeft;
            cr = DrawCheckbox("Credit Card");
            SetY(cr.Bottom, 2);
            cr = DrawCheckbox("Check or Money Order enclosed");
            SetY(cr.Bottom, 2);
            cr = DrawCheckbox("Gift Card, Gift Certificate or ACME Visa coupon dollars.\r\n" +
                "Enter # below (for Gift Cards, please include PIN).");

            _ip.X = oCc;
            cr = DrawText("Card Number:", TsNormal);
            _ip.X = cr.Right + Textbox.LabelSpacing;
            cr = DrawTextbox(180);
            _ip.X = cr.Right + 4;
            cr = DrawTextbox(pageWidth - MarginRight - _ip.X);
            // small text
            SetY(cr.Bottom, SmallTextVOff);
            _ip.X = cr.Left;
            cr = DrawText("Exp. Date (MM/YY)", TsSmall);

            SetY(cr.Bottom, 10);
            _ip.X = MarginLeft;
            cr = DrawText("Number:", TsNormal);
            _ip.X = cr.Right + Textbox.LabelSpacing;
            cr = DrawTextbox(140);
            float tbBottom = cr.Bottom;
            _ip.X = cr.Right + 4;
            cr = DrawTextbox(60);
            float oPin = cr.Left;
            _ip.X = oCc;
            cr = DrawText("Signature:", TsNormal);
            CurrGraphics.DrawLine(new PointF(cr.Right, cr.Bottom),
                new PointF(pageWidth - MarginRight, cr.Bottom), Color.Black, 0.5f);
            // small text
            SetY(tbBottom, SmallTextVOff);
            _ip.X = oPin;
            cr = DrawText("PIN", TsSmall);
        }

        private void DrawGreenLine(float? from = null, float? to = null)
        {
            var page = CurrPage;
            if (!from.HasValue)
                from = MarginLeft;
            if (!to.HasValue)
                to = page.Size.Width - MarginRight;
            var g = page.Graphics;
            var pen = new Pen(TsTitle.ForeColor, CaptionLineThickness);
            g.DrawLine(new PointF(from.Value, _ip.Y), new PointF(to.Value, _ip.Y), pen);
        }

        private RectangleF DrawText(string text, TextStyle ts)
        {
            var page = CurrPage;
            TextLayout tl = page.Graphics.CreateTextLayout();
            tl.MaxWidth = page.Size.Width - MarginRight - _ip.X;
            if (ts.FontSize == TsTitle.FontSize) // patch
                tl.TextAlignment = TextAlignment.Center;
            tl.DefaultFormat.Font = ts.Font;
            tl.DefaultFormat.FontSize = ts.FontSize;
            tl.DefaultFormat.GlyphAdvanceFactor = ts.GlyphAdvanceFactor;
            tl.DefaultFormat.ForeColor = ts.ForeColor;
            tl.Append(text);
            tl.PerformLayout(true);

            var line = tl.Lines[tl.Lines.Count - 1];
            var run = line.GlyphRuns[0];
            var baselineOffset = run.BaselineOffset;

            var p = _lastBaselineOffset.HasValue ? 
                new PointF(_ip.X, _ip.Y + _lastBaselineOffset.Value - baselineOffset) : _ip;
            page.Graphics.DrawTextLayout(tl, p);
            if (!_lastBaselineOffset.HasValue)
                _lastBaselineOffset = baselineOffset; // within one 'line', keep using the first offset

            return new RectangleF(_ip.X + tl.ContentX, _ip.Y + tl.ContentY, tl.ContentWidth, tl.ContentHeight);
        }

        private RectangleF DrawTextbox(float width, bool inTable = false)
        {
            var fld = new TextField();
            fld.Widget.Page = CurrPage;
            var p = _lastBaselineOffset.HasValue ? 
                new PointF(_ip.X, _ip.Y + _lastBaselineOffset.Value - Textbox.BaselineOffset) : _ip;
            fld.Widget.Rect = new RectangleF(p.X, p.Y, width, Textbox.Height);
            if (inTable)
                fld.Widget.Border = null;
            else
                fld.Widget.Border.Style = BorderStyle.Underline;
            fld.Widget.TextFormat.Font = Textbox.Font;
            fld.Widget.TextFormat.FontSize = Textbox.FontSize;
            _doc.AcroForm.Fields.Add(fld);
            if (!_lastBaselineOffset.HasValue)
                _lastBaselineOffset = Textbox.BaselineOffset;
            return fld.Widget.Rect;
        }

        private RectangleF DrawCheckbox(string text)
        {
            var fld = new CheckBoxField();
            fld.Widget.Page = CurrPage;
            var p = _lastBaselineOffset.HasValue ? 
                new PointF(_ip.X, _ip.Y + _lastBaselineOffset.Value - Checkbox.BaselineOffset) : _ip;
            fld.Widget.Rect = new RectangleF(p.X, p.Y, Checkbox.Height, Checkbox.Height);
            _doc.AcroForm.Fields.Add(fld);
            if (!_lastBaselineOffset.HasValue)
                _lastBaselineOffset = Checkbox.BaselineOffset;
            var pSave = _ip;
            _ip.X = fld.Widget.Rect.Right + Checkbox.LabelSpacing;
            var r = DrawText(text, TsNormal);
            _ip = pSave;
            return new RectangleF(fld.Widget.Rect.X, r.Y, r.Right - fld.Widget.Rect.Left, r.Height);
        }

        private RectangleF DrawReturnsTable()
        {
            float[] widths = new float[]
            {
                55,
                60,
                60,
                35,
                35,
                200,
                50,
                0
            };
            string[] captions = new string[]
            {
                "Reason Code",
                "Item #",
                "Color",
                "Size",
                "Quantity",
                "Item Name",
                "Pirce",
                "Total",
            };
            string[] samples = new string[]
            {
                "23",
                "KK123456",
                "Navy",
                "8",
                "1",
                "Example Item Only",
                "59.00",
                "59.00",
            };

            return DrawTable(widths, captions, samples, 4);
        }

        private RectangleF DrawExchangesTable()
        {
            // This table has two special extra titles spanning two tolumns.
            // To achieve this, we:
            // - force the column titles in those 4 columns to print as '2nd paragraph',
            //   thus leaving an empty line for the span title;
            // - print the span titles here as a special case.
            float[] widths = new float[]
            {
                50,
                25,
                25,
                25,
                25,
                60,
                150,
                50,
                40,
                25,
                35,
                0
            };
            string[] captions = new string[]
            {
                "Item",
                "Style",
                "\r\n1st",
                "\r\n2nd",
                "Size",
                "Sleeve Length\r\n& Inseam",
                "Item Name",
                "\r\nCharacters",
                "\r\nStyle",
                "Qty.",
                "Price",
                "Total"
            };
            string[] samples = new string[]
            {
                "LH123456",
                "Plain",
                "Tan",
                "Olive",
                "8",
                "28",
                "Example Item Only",
                "Amanda",
                "Block",
                "1",
                "49.95",
                "49.95"
            };

            var cr = DrawTable(widths, captions, samples, 4);

            // print 2 spanning titles:
            var g = CurrGraphics;
            TextLayout tl = g.CreateTextLayout();
            tl.ParagraphAlignment = ParagraphAlignment.Near;
            tl.TextAlignment = TextAlignment.Center;
            tl.DefaultFormat.Font = TsNormal.Font;
            tl.DefaultFormat.FontSize = TsNormal.FontSize;
            tl.DefaultFormat.GlyphAdvanceFactor = TsNormal.GlyphAdvanceFactor;
            tl.DefaultFormat.ForeColor = Color.White;
            tl.WrapMode = WrapMode.NoWrap;
            // Color Choice
            var width = widths[2] + widths[3];
            tl.MaxWidth = width;
            tl.Append("Color Choice");
            tl.PerformLayout(true);
            var pt = new PointF(cr.Left + widths[0] + widths[1], cr.Top);
            g.DrawTextLayout(tl, pt);
            Pen pen = new Pen(Color.White, 0.5f);
            var pt1 = new PointF(pt.X + 0.5f, pt.Y + TableCaptionHeight / 2);
            var pt2 = new PointF(pt1.X + width, pt1.Y);
            g.DrawLine(pt1, pt2, pen);
            pt1 = new PointF(pt.X + widths[2] + 0.5f, pt.Y + TableCaptionHeight / 2);
            pt2 = new PointF(pt1.X, pt.Y + TableCaptionHeight);
            g.DrawLine(pt1, pt2, pen);
            pt1 = new PointF(pt.X + 0.5f, pt.Y);
            pt2 = new PointF(pt1.X, pt.Y + TableCaptionHeight);
            g.DrawLine(pt1, pt2, pen);
            pt1 = new PointF(pt.X + width + 0.5f, pt.Y);
            pt2 = new PointF(pt1.X, pt.Y + TableCaptionHeight);
            g.DrawLine(pt1, pt2, pen);

            // Monogramming
            width = widths[7] + widths[8];
            tl.Inlines.Clear();
            tl.MaxWidth = width;
            tl.Append("Monogramming");
            tl.PerformLayout(true);
            pt = new PointF(cr.Left + widths[0] + widths[1] + widths[2] + widths[3] + widths[4] + widths[5] + widths[6], cr.Top);
            g.DrawTextLayout(tl, pt);
            pt1 = new PointF(pt.X + 0.5f, pt.Y + TableCaptionHeight / 2);
            pt2 = new PointF(pt1.X + width, pt1.Y);
            g.DrawLine(pt1, pt2, pen);
            pt1 = new PointF(pt.X + widths[7] + 0.5f, pt.Y + TableCaptionHeight / 2);
            pt2 = new PointF(pt1.X, pt.Y + TableCaptionHeight);
            g.DrawLine(pt1, pt2, pen);
            pt1 = new PointF(pt.X + 0.5f, pt.Y);
            pt2 = new PointF(pt1.X, pt.Y + TableCaptionHeight);
            g.DrawLine(pt1, pt2, pen);
            pt1 = new PointF(pt.X + width + 0.5f, pt.Y);
            pt2 = new PointF(pt1.X, pt.Y + TableCaptionHeight);
            g.DrawLine(pt1, pt2, pen);

            return cr;
        }

        private RectangleF DrawTable(float[] widths, string[] captions, string[] samples, int rowCount)
        {
            System.Diagnostics.Debug.Assert(captions.Length == widths.Length && samples.Length == widths.Length);

            var ipSave = _ip;
            Pen p = new Pen(Color.Black, 0.5f);

            var g = CurrGraphics;
            TextLayout tl = g.CreateTextLayout();
            tl.ParagraphAlignment = ParagraphAlignment.Center;
            tl.TextAlignment = TextAlignment.Center;
            tl.DefaultFormat.Font = TsNormal.Font;
            tl.DefaultFormat.FontSize = TsNormal.FontSize;
            tl.DefaultFormat.GlyphAdvanceFactor = TsNormal.GlyphAdvanceFactor;
            tl.DefaultFormat.ForeColor = Color.White;
            tl.WrapMode = WrapMode.NoWrap;
            tl.MaxHeight = TableCaptionHeight;
            float totW = 0;
            for (int i = 0; i < widths.Length; ++i)
            {
                if (i == widths.Length - 1)
                {
                    widths[i] = CurrPage.Size.Width - MarginLeft - MarginRight - totW - 1;
                    totW += 1;
                }
                totW += widths[i];
            }
            g.FillRectangle(new RectangleF(MarginLeft, _ip.Y, totW, TableCaptionHeight), Color.Black);
            var pt = new PointF(MarginLeft, _ip.Y);
            for (int i = 0; i < widths.Length; ++i)
            {
                tl.MaxWidth = widths[i];
                tl.Append(captions[i]);
                tl.PerformLayout(true);
                g.DrawTextLayout(tl, pt);
                pt.X = pt.X + widths[i];
                tl.Inlines.Clear();
            }
            tl.DefaultFormat.ForeColor = Color.Teal;
            tl.MaxHeight = TableSampleHeight;
            pt = new PointF(MarginLeft, _ip.Y + TableCaptionHeight);
            for (int i = 0; i < widths.Length; ++i)
            {
                tl.MaxWidth = widths[i];
                tl.Append(samples[i]);
                tl.PerformLayout(true);
                g.DrawTextLayout(tl, pt);
                pt.X = pt.X + widths[i];
                tl.Inlines.Clear();
            }
            SetY(_ip.Y + TableCaptionHeight + TableSampleHeight, 0.5f);
            for (int row = 0; row < rowCount; ++row)
            {
                _ip.X = MarginLeft + 1;
                for (int i = 0; i < widths.Length; ++i)
                {
                    var cr = DrawTextbox(widths[i] - 1, true);
                    _ip.X = cr.Right + 1;
                }
                g.DrawLine(new PointF(MarginLeft, _ip.Y - 0.5f), new PointF(MarginLeft + totW, _ip.Y - 0.5f), p);
                SetY(null, Textbox.Height + 1);
            }
            var totH = TableCaptionHeight + TableSampleHeight + (Textbox.Height + 1) * rowCount;
            _ip.X = MarginLeft + 0.5f;
            for (int i = 0; i < widths.Length - 1; ++i)
            {
                _ip.X += widths[i];
                g.DrawLine(new PointF(_ip.X, ipSave.Y), new PointF(_ip.X, ipSave.Y + totH), p);
            }

            var rect = new RectangleF(MarginLeft, ipSave.Y, totW, totH);
            g.DrawRectangle(rect, p);

            return rect;
        }

        private RectangleF DrawReasonCodes()
        {
            float startX = 150;
            float capOff = 16;
            float colOff = 110;
            var ipSave = _ip;

            _ip.X = startX;
            var cr = DrawText("01", TsNormal);
            _ip.X += capOff;
            cr = DrawText("Unsatisfactory", TsNormal);
            _ip.X = startX + colOff;
            cr = DrawText("33", TsNormal);
            _ip.X += capOff;
            cr = DrawText("Did not like color", TsNormal);
            _ip.X = startX + colOff * 2;
            cr = DrawText("23", TsNormal);
            _ip.X += capOff;
            cr = DrawText("Ordered wrong size", TsNormal);
            _ip.X = startX + colOff * 3;
            cr = DrawText("51", TsNormal);
            _ip.X += capOff;
            cr = DrawText("Shipping damage", TsNormal);
            SetY(null, TsNormal.FontSize + 2);
            _ip.X = startX;
            cr = DrawText("02", TsNormal);
            _ip.X += capOff;
            cr = DrawText("Defective construction", TsNormal);
            _ip.X = startX + colOff;
            cr = DrawText("21", TsNormal);
            _ip.X += capOff;
            cr = DrawText("Too small", TsNormal);
            _ip.X = startX + colOff * 2;
            cr = DrawText("25", TsNormal);
            _ip.X += capOff;
            cr = DrawText("Too short", TsNormal);
            _ip.X = startX + colOff * 3;
            cr = DrawText("52", TsNormal);
            _ip.X += capOff;
            cr = DrawText("Wrong item shipped", TsNormal);

            _ip.X = MarginLeft + 10;
            cr = DrawText("Reason Codes", TsBold);
            float lineX = cr.Right + 20;

            SetY(null, TsNormal.FontSize + 2);
            _ip.X = startX;
            cr = DrawText("31", TsNormal);
            _ip.X += capOff;
            cr = DrawText("Did not like styling", TsNormal);
            _ip.X = startX + colOff;
            cr = DrawText("22", TsNormal);
            _ip.X += capOff;
            cr = DrawText("Too large", TsNormal);
            _ip.X = startX + colOff * 2;
            cr = DrawText("36", TsNormal);
            _ip.X += capOff;
            cr = DrawText("Too long", TsNormal);

            var rect = new RectangleF(MarginLeft, ipSave.Y, CurrPage.Size.Width, cr.Bottom - ipSave.Y);
            CurrGraphics.DrawLine(lineX, rect.Top, lineX, rect.Bottom, Color.Black, 0.5f);

            return rect;
        }
    }
}
