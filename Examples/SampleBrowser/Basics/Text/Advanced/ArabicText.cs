using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // Renders Arabic text using a columnar layout.
    // See also MultiLang and JapaneseColumns.
    public class ArabicText
    {
        string text = "العربية أكبر لغات المجموعة السامية من حيث عدد المتحدثين، وإحدى أكثر اللغات انتشارًا في العالم، يتحدثها أكثر من 422 مليون نسمة،1 ويتوزع متحدثوها في المنطقة المعروفة باسم الوطن العربي، بالإضافة إلى العديد من المناطق الأخرى المجاورة كالأحواز وتركيا وتشاد ومالي والسنغالوارتيرياوللغة العربية أهمية قصوى لدى أتباع الديانة الإسلامية، فهي لغة مصدري التشريع الأساسيين في الإسلام: القرآن، والأحاديث النبوية المروية عن النبي محمد، ولا تتم الصلاة في الإسلام (وعبادات أخرى) إلا بإتقان بعض من كلمات هذه اللغة. والعربية هي أيضًا لغة طقسية رئيسية لدى عدد من الكنائس المسيحية في العالم العربي، كما كتبت بها الكثير من أهم الأعمال الدينية والفكرية اليهودية في العصور الوسطى. وأثّر انتشار الإسلام، وتأسيسه دولًا، أرتفعت مكانة اللغة العربية، وأصبحت لغة السياسة والعلم والأدب لقرون طويلة في الأراضي التي حكمها المسلمون، وأثرت العربية، تأثيرًا مباشرًا أو غير مباشر على كثير من اللغات الأخرى في العالم الإسلامي، كالتركية والفارسية والأرديةوالالبانية واللغات الأفريقية الاخرى واللغات الأوروبية مثل الروسية والإنجليزية والفرنسية والأسبانية والايطالية والألمانية.كما انها تدرس بشكل رسمى او غير رسمى في الدول الاسلامية والدول الأفريقية المحادية للوطن العربى.";

        public void CreatePDF(Stream stream)
        {
            var times = Font.FromFile(Path.Combine("Resources", "Fonts", "times.ttf"));
            var reds = Image.FromFile(Path.Combine("Resources", "Images", "reds.jpg"));
            var firth = Image.FromFile(Path.Combine("Resources", "Images", "firth.jpg"));
            var purples = Image.FromFile(Path.Combine("Resources", "Images", "purples.jpg"));
            var ia = new ImageAlign(ImageAlignHorz.Left, ImageAlignVert.Top, true, true, true, false, false);

            var doc = new GcPdfDocument();

            // The TextLayout that will hold and render the text:
            var tl = new TextLayout();
            tl.FirstLineIndent = 18;
            tl.ParagraphSpacing = 6;
            tl.TextAlignment = TextAlignment.Justified;
            tl.AlignmentDelayToSplit = true;
            tl.RightToLeft = true;
            var tf = new TextFormat() { Font = times, FontSize = 12 };
            // Repeat test text to fill a few pages:
            for (int i = 0; i < 12; ++i)
            {
                tl.Append(text, tf);
                tl.AppendLine();
            }

            // Layout text in 3 columns:
            // (The logic/code in this sample is identical to JapaneseColumns
            const int NCOLS = 3;
            var margin = 36f;
            var gap = 18f;
            var page = doc.NewPage();
            page.Landscape = true;
            var colWid = (page.Size.Width - margin * 2 - gap * (NCOLS - 1)) / NCOLS;
            tl.MaxWidth = page.Size.Width;
            tl.MaxHeight = page.Size.Height;
            tl.MarginTop = tl.MarginBottom = margin;
            tl.MarginRight = margin;
            tl.MarginLeft = margin + (colWid + gap) * (NCOLS - 1);
            // We can specify arbitrary rectangles for the text to flow around.
            // In this case, we add 3 areas to draw some images:
            tl.ObjectRects = new List<ObjectRect>()
            {
                new ObjectRect(page.Size.Width - margin - 240, margin, 240, 240),
                new ObjectRect(margin + 100, margin + 60, 133, 100),
                new ObjectRect(margin, page.Size.Height - margin - 300, 300, 300),
            };
            // Convert object rects to image areas, ajust to provide nice looking padding:
            var rReds = tl.ObjectRects[0].ToRectangleF();
            rReds.Inflate(-4, -3);
            var rFirth = tl.ObjectRects[1].ToRectangleF();
            rFirth.Inflate(-4, -3);
            var rPurples = tl.ObjectRects[2].ToRectangleF();
            rPurples.Inflate(-4, -3);
            page.Graphics.DrawImage(reds, rReds, null, ia);
            page.Graphics.DrawImage(firth, rFirth, null, ia);
            page.Graphics.DrawImage(purples, rPurples, null, ia);

            // THE call: it calculates the glyphs needed to draw the text, and lays it out:
            tl.PerformLayout(true);

            bool done = false;
            while (!done) // loop while there is still text to render
            {
                for (int col = 0; col < NCOLS; ++col)
                {
                    int nextcol = (col < NCOLS - 1) ? col + 1 : 0;
                    // TextSplitOptions tell TextLayout.Split() how to layout the remaining text.
                    // In this case we advance from column to column by updating top and bottom margins:
                    var tso = new TextSplitOptions(tl)
                    {
                        RestMarginRight = margin + (colWid + gap) * nextcol,
                        RestMarginLeft = margin + (colWid + gap) * (NCOLS - 1 - nextcol)
                    };
                    var split = tl.Split(tso, out TextLayout rest);
                    page.Graphics.DrawTextLayout(tl, PointF.Empty);
                    if (split != SplitResult.Split)
                    {
                        done = true;
                        break;
                    }
                    tl = rest;
                }
                if (!done)
                {
                    page = doc.NewPage();
                    page.Landscape = true;
                    // We only want to render images on the first page, so clear ObjectRects:
                    if (tl.ObjectRects != null)
                    {
                        tl.ObjectRects = null;
                        // We need to redo the layout, but no need to recalculate the glyphs:
                        tl.PerformLayout(false);
                    }
                }
            }
            // Done:
            doc.Save(stream);
        }
    }
}
