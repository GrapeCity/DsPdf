using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Barcode;

namespace GcPdfWeb.Samples
{
    // This sample prints a set of shipping labels that include some barcodes.
    // Note also the use of tab stops to vertically align data.
    public class ShippingLabels
    {
        // Client info. This sample prints one shipping label for each client.
        class Client
        {
            public string Name;
            public string Addr;
            public string City;
            public string Country;
            public Client(string name, string addr, string city, string country)
            {
                Name = name;
                Addr = addr;
                City = city;
                Country = country;
            }
        }

        // The clients base.
        static readonly List<Client> s_clients = new List<Client>()
        {
            new Client("Simons bistro", "Vinb�ltet 34", "K�benhavn", "Denmark"),
            new Client("Richter Supermarkt", "Starenweg 5", "Gen�ve", "Switzerland"),
            new Client("Bon app'", "12, rue des Bouchers", "Marseille", "France"),
            new Client("Rattlesnake Canyon Grocery", "2817 Milton Dr.", "Albuquerque", "USA"),
            new Client("Lehmanns Marktstand", "Magazinweg 7", "Frankfurt a.M.", "Germany"),
            new Client("LILA-Supermercado", "Carrera 52 con Ave. Bol�var #65-98 Llano Largo", "Barquisimeto", "Venezuela"),
            new Client("Ernst Handel", "Kirchgasse 6", "Graz", "Austria"),
            new Client("Pericles Comidas cl�sicas", "Calle Dr. Jorge Cash 321", "M�xico D.F.", "Mexico"),
            new Client("Drachenblut Delikatessen", "Walserweg 21", "Aachen", "Germany"),
            new Client("Queen Cozinha", "Alameda dos Can�rios, 891", "S�o Paulo", "Brazil"),
            new Client("Tortuga Restaurante", "Avda. Azteca 123", "M�xico D.F.", "Mexico"),
            new Client("Save-a-lot Markets", "187 Suffolk Ln.", "Boise", "USA"),
            new Client("Franchi S.p.A.", "Via Monte Bianco 34", "Torino", "Italy"),
            // new Client("", "", "", ""),
        };

        // The main sample driver.
        public void CreatePDF(Stream stream)
        {
            GcPdfDocument doc = new GcPdfDocument();
            Init(doc);

            // Loop over clients, print up to 4 labels per page:
            int i = 0;
            for (int pg = 0; pg < (s_clients.Count + 3) / 4; ++pg)
            {
                Page page = doc.NewPage();
                PrintLabel(s_clients[i++], page, new RectangleF(hmargin, vmargin, _labelWidth, _labelHeight));
                if (i < s_clients.Count)
                    PrintLabel(s_clients[i++], page, new RectangleF(hmargin + _labelWidth, vmargin, _labelWidth, _labelHeight));
                if (i < s_clients.Count)
                    PrintLabel(s_clients[i++], page, new RectangleF(hmargin, vmargin + _labelHeight, _labelWidth, _labelHeight));
                if (i < s_clients.Count)
                    PrintLabel(s_clients[i++], page, new RectangleF(hmargin + _labelWidth, vmargin + _labelHeight, _labelWidth, _labelHeight));
            }
            // Done:
            doc.Save(stream);
            Term();
        }

        // Consts and vars used to render the labels:
        const float hmargin = 24, vmargin = 36;
        float _labelWidth, _labelHeight;
        Pen _pBold, _pNorm;
        Font _fontReg, _fontBold;
        TextFormat _tfSmall, _tfSmallB, _tfLarge;
        List<TabStop> _tsHeader, _tsFrom, _tsCodes;
        Image _logo;
        ImageAlign _ia;
        GcBarcode bcTop, bcBottom;

        // Init variables used to render labels:
        void Init(GcPdfDocument doc)
        {
            _labelWidth = (doc.PageSize.Width - hmargin * 2) / 2;
            _labelHeight = _labelWidth;
            _pBold = new Pen(Color.Black, 2);
            _pNorm = new Pen(Color.Black, 0.5f);
            _logo = Image.FromFile(Path.Combine("Resources", "ImagesBis", "AcmeLogo-vertical-250px.png"));
            _fontReg = Font.FromFile(Path.Combine("Resources", "Fonts", "segoeui.ttf"));
            _fontBold = Font.FromFile(Path.Combine("Resources", "Fonts", "segoeuib.ttf"));
            _tfSmall = new TextFormat() { Font = _fontReg, FontSize = 8 };
            _tfSmallB = new TextFormat() { Font = _fontBold, FontSize = 8 };
            _tfLarge = new TextFormat() { Font = _fontBold, FontSize = 14 };

            _ia = new ImageAlign(ImageAlignHorz.Right, ImageAlignVert.Center, true, true, true, false, false);
            _tsHeader = new List<TabStop>() { new TabStop(24, TabStopAlignment.Leading), new TabStop(108, TabStopAlignment.Leading) };
            _tsFrom = new List<TabStop>() { new TabStop(12, TabStopAlignment.Leading), new TabStop(72, TabStopAlignment.Leading) };
            _tsCodes = new List<TabStop>() { new TabStop(_labelWidth / 8, TabStopAlignment.Center) };

            bcTop = new GcBarcode() {
                TextFormat = _tfSmall,
                CodeType = CodeType.Code_128_B,
                HorizontalAlignment = ImageAlignHorz.Center,
                VerticalAlignment = ImageAlignVert.Center,
            };
            bcTop.Options.CaptionPosition = BarCodeCaptionPosition.Below;
            bcBottom = new GcBarcode()
            {
                TextFormat = _tfSmall,
                CodeType = CodeType.Code_128auto,
                HorizontalAlignment = ImageAlignHorz.Center,
                VerticalAlignment = ImageAlignVert.Center,
            };
            bcBottom.Options.CaptionPosition = BarCodeCaptionPosition.Below;
        }

        void Term()
        {
            _logo?.Dispose();
            _logo = null;
        }

        void PrintLabel(Client client, Page page, RectangleF bounds)
        {
            // Used to randomize some sample data:
            var rnd = Common.Util.NewRandom();

            // Sample shipper/sender data:
            var shipper = rnd.Next(2) == 0 ? "United Package" : "Speedy Express";
            (string name, string addr, string city, string country, string zip) sender =
                (name: "ACME Inc.", addr: "1 Main Street", city: "Metropolis", country: "USA", zip: "34567");
            var shuttle = rnd.Next(10000, 15000).ToString();
            var area = rnd.Next(1, 12).ToString();
            var tour = rnd.Next(0, 20).ToString();

            var g = page.Graphics;

            var tl = new TextLayout();
            tl.DefaultFormat.Font = _tfSmall.Font;
            tl.DefaultFormat.FontSize = _tfSmall.FontSize;
            tl.LineSpacingScaleFactor = 0.75f;
            tl.ParagraphAlignment = ParagraphAlignment.Center;

            // Header
            var hHeader = bounds.Height / 2 / 5;
            var rHeader = new RectangleF(bounds.X, bounds.Y, bounds.Width, hHeader);
            tl.TabStops = _tsHeader;
            tl.Append($"\t{shipper}");
            tl.Append("\tCMR", _tfSmallB);
            tl.Append($"\n\t\t{DateTime.Now.ToShortDateString()}");
            tl.MaxHeight = rHeader.Height;
            tl.MaxWidth = rHeader.Width;
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, rHeader.Location);

            var rLogo = rHeader;
            rLogo.Inflate(-5, -5);
            g.DrawImage(_logo, rLogo,  null, _ia);

            // From
            var hFrom = hHeader;
            var rFrom = new RectangleF(bounds.X, rHeader.Bottom, bounds.Width, hFrom);
            tl.Clear();
            tl.TabStops = _tsFrom;
            tl.Append($"\tFrom:\t{sender.name}\n\t\t{sender.addr}\n\t\t{sender.city}, {sender.country} {sender.zip}");
            tl.MaxHeight = rFrom.Height;
            tl.MaxWidth = rFrom.Width;
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, rFrom.Location);

            // To
            var hTo = bounds.Height / 2 / 3;
            var rTo = new RectangleF(bounds.X, rFrom.Bottom, bounds.Width, hTo);
            tl.Clear();
            tl.TabStops = _tsFrom;
            tl.Append($"\tTo:\t{client.Name}\n\t\t{client.Addr}\n\t\t{client.City}\n\t\t{client.Country}");
            tl.MaxHeight = rTo.Height;
            tl.MaxWidth = rTo.Width;
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, rTo.Location);

            // Codes
            var hCodes = bounds.Height / 2 / (15f / 4);
            var rCodes = new RectangleF(bounds.X, rTo.Bottom, bounds.Width, hCodes);
            tl.TabStops = _tsCodes;
            tl.Clear();
            tl.AppendLine("\tShuttle");
            tl.Append($"\t{shuttle}", _tfLarge);
            tl.MaxHeight = rCodes.Height;
            tl.MaxWidth = rCodes.Width / 4;
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, rCodes.Location);

            tl.Clear();
            tl.AppendLine("\tArea");
            tl.Append($"\t{area}", _tfLarge);
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, new PointF(rCodes.X + rCodes.Width / 4, rCodes.Y));

            tl.Clear();
            tl.AppendLine("\tException");
            tl.Append("\t ", _tfLarge);
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, new PointF(rCodes.X + rCodes.Width / 4 * 2, rCodes.Y));

            tl.Clear();
            tl.AppendLine("\tTour");
            tl.Append($"\t{tour}", _tfLarge);
            tl.PerformLayout(true);
            g.DrawTextLayout(tl, new PointF(rCodes.X + rCodes.Width / 4 * 3, rCodes.Y));

            // Barcodes
            var hBarcodes = bounds.Height / 2;
            var rBcTop = new RectangleF(bounds.X, rCodes.Bottom, bounds.Width, hBarcodes / 2);
            var rBcBottom = new RectangleF(bounds.X, rBcTop.Bottom, bounds.Width, hBarcodes / 2);

            bcTop.Text = client.Country;
            g.DrawBarcode(bcTop, rBcTop);

            // Make up a longish 'code':
            var code = $"{client.Name[0]}{client.Addr[0]}{client.City[0]}{client.Country[0]}".ToUpper();
            bcBottom.Text = $"{code}{client.GetHashCode().ToString("X12")}";
            g.DrawBarcode(bcBottom, rBcBottom);

            // Lines:
            g.DrawLine(rHeader.Left, rHeader.Bottom, rHeader.Right, rHeader.Bottom, _pNorm);
            g.DrawLine(rFrom.Left, rFrom.Bottom, rFrom.Right, rFrom.Bottom, _pNorm);
            g.DrawLine(rTo.Left, rTo.Bottom, rTo.Right, rTo.Bottom, _pNorm);
            g.DrawLine(rCodes.Left, rCodes.Bottom, rCodes.Right, rCodes.Bottom, _pNorm);

            g.DrawLine(rCodes.Left + rCodes.Width / 4, rCodes.Top, rCodes.Left + rCodes.Width / 4, rCodes.Bottom, _pNorm);
            g.DrawLine(rCodes.Left + rCodes.Width / 4 * 2, rCodes.Top, rCodes.Left + rCodes.Width / 4 * 2, rCodes.Bottom, _pNorm);
            g.DrawLine(rCodes.Left + rCodes.Width / 4 * 3, rCodes.Top, rCodes.Left + rCodes.Width / 4 * 3, rCodes.Bottom, _pNorm);

            g.DrawRectangle(bounds, _pBold);
        }
    }
}
