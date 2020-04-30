//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // This sample draws some test texts in a number of different languages,
    // including right-to-left and Far Eastern ones.
    // See also PaginatedText.
    public class MultiLang
    {
        // All sizes, distances etc are in points (1/72", GcPdf's default):
        private const float c_CaptionToText = 5;
        private const float c_TextToCaption = 18;
        private const float c_Margin = 36;
        // Private data:
        private GcPdfDocument _doc;
        private TextLayout _captionLayout;
        private TextLayout _textLayout;
        private float _ipY;

        // Method drawing a language's caption and test text:
        private void DrawText(string caption, string text, Font font, bool rtl)
        {
            _captionLayout.Clear();
            _captionLayout.Append(caption);
            _captionLayout.PerformLayout(true);

            _textLayout.Clear();
            _textLayout.DefaultFormat.Font = font;
            _textLayout.RightToLeft = rtl;
            _textLayout.Append(text);
            _textLayout.PerformLayout(true);
            // Add new page if needed:
            GcGraphics g;
            if (_doc.Pages.Count == 0 || _ipY + tlHeight(_captionLayout) + tlHeight(_textLayout) + c_CaptionToText > _doc.PageSize.Height - c_Margin)
            {
                _ipY = c_Margin;
                g = _doc.Pages.Add().Graphics;
            }
            else
                g = _doc.Pages.Last.Graphics;
            // Draw caption:
            g.FillRectangle(new RectangleF(c_Margin, _ipY, _captionLayout.MaxWidth.Value, tlHeight(_captionLayout)), Color.SteelBlue);
            g.DrawTextLayout(_captionLayout, new PointF(c_Margin, _ipY));
            _ipY += tlHeight(_captionLayout);
            _ipY += c_CaptionToText;
            // Draw test text:
            g.DrawRectangle(new RectangleF(c_Margin, _ipY, _textLayout.MaxWidth.Value, tlHeight(_textLayout)), Color.LightSteelBlue, 0.5f);
            g.DrawTextLayout(_textLayout, new PointF(c_Margin, _ipY));
            _ipY += tlHeight(_textLayout);
            _ipY += c_TextToCaption;
            return;

            float tlHeight(TextLayout tl)
            {
                return tl.MarginTop + tl.ContentHeight + tl.MarginBottom;
            }
        }

        public void CreatePDF(Stream stream)
        {
            Font arialbd = Font.FromFile(Path.Combine("Resources", "Fonts", "arialbd.ttf"));
            Font malgun = Font.FromFile(Path.Combine("Resources", "Fonts", "malgun.ttf"));
            Font segoe = Font.FromFile(Path.Combine("Resources", "Fonts", "segoeui.ttf"));
            // Add Arial Unicode MS for Chinese, Hindi and Japanese fallbacks:
            Font arialuni = Font.FromFile(Path.Combine("Resources", "Fonts", "arialuni.ttf"));
            segoe.AddLinkedFont(arialuni);
            malgun.AddLinkedFont(arialuni);
            // Create document:
            _doc = new GcPdfDocument();
            // Init text layouts:
            _captionLayout = new TextLayout(72)
            {
                MaxWidth = _doc.PageSize.Width - c_Margin * 2,
                MarginLeft = 4,
                MarginRight = 4,
                MarginTop = 2,
                MarginBottom = 2,
            };
            _captionLayout.DefaultFormat.Font = arialbd;
            _captionLayout.DefaultFormat.FontSize = 12;
            _captionLayout.DefaultFormat.ForeColor = Color.AliceBlue;
            //
            _textLayout = new TextLayout(72)
            {
                FontFallbackScope = FontFallbackScope.None,
                MaxWidth = _doc.PageSize.Width - c_Margin * 2,
                MarginLeft = 6,
                MarginRight = 6,
                MarginTop = 6,
                MarginBottom = 6,
            };
            _textLayout.DefaultFormat.FontSize = 10;
            // Draw texts in a loop:
            Dictionary<string, Font> fonts = new Dictionary<string, Font>() { { "segoe", segoe }, { "malgun", malgun } };
            for (int i = 0; i < s_texts.GetLength(0); ++i)
            {
                string lang = s_texts[i, 0];
                string text = s_texts[i, 1];
                Font font = fonts[s_texts[i, 2]];
                bool rtl = !string.IsNullOrEmpty(s_texts[i, 3]);
                DrawText(lang, text, font, rtl);
            }
            // Done:
            _doc.Save(stream);
        }

        // 0 - Language tag
        // 1 - Test string
        // 2 - Font to use
        // 3 - If not null/empty - the language is RTL
        string[,] s_texts = new string[,]
        {
            {
                "Arabic",
                "العربية أكبر لغات المجموعة السامية من حيث عدد المتحدثين، وإحدى أكثر اللغات انتشارًا في العالم، يتحدثها أكثر من 422 مليون نسمة،1 ويتوزع متحدثوها في المنطقة المعروفة باسم الوطن العربي، بالإضافة إلى العديد من المناطق الأخرى المجاورة كالأحواز وتركيا وتشاد ومالي والسنغالوارتيرياوللغة العربية أهمية قصوى لدى أتباع الديانة الإسلامية، فهي لغة مصدري التشريع الأساسيين في الإسلام: القرآن، والأحاديث النبوية المروية عن النبي محمد، ولا تتم الصلاة في الإسلام (وعبادات أخرى) إلا بإتقان بعض من كلمات هذه اللغة. والعربية هي أيضًا لغة طقسية رئيسية لدى عدد من الكنائس المسيحية في العالم العربي، كما كتبت بها الكثير من أهم الأعمال الدينية والفكرية اليهودية في العصور الوسطى. وأثّر انتشار الإسلام، وتأسيسه دولًا، أرتفعت مكانة اللغة العربية، وأصبحت لغة السياسة والعلم والأدب لقرون طويلة في الأراضي التي حكمها المسلمون، وأثرت العربية، تأثيرًا مباشرًا أو غير مباشر على كثير من اللغات الأخرى في العالم الإسلامي، كالتركية والفارسية والأرديةوالالبانية واللغات الأفريقية الاخرى واللغات الأوروبية مثل الروسية والإنجليزية والفرنسية والأسبانية والايطالية والألمانية.كما انها تدرس بشكل رسمى او غير رسمى في الدول الاسلامية والدول الأفريقية المحادية للوطن العربى.",
                "segoe",
                "rtl"
            },
            {
                "Belarusian",
                "БЕЛАРУ́СКАЯ МО́ВА, мова беларусаў уваходзіць у сям’ю індаеўрапейскіх моў, яе славянскай групы і ўсходнеславянскіх моваў падгрупы, на якой размаўляюць у Беларусі і па ўсім свеце, галоўным чынам у Расіі, Украіне, Польшчы. Б.м. падзяляе шмат граматычных і лексічных уласцівасцяў з іншымі ўсходнеславянскімі мовамі (гл.таксама: Іншыя назвы беларускай мовы і Узаемныя ўплывы усходнеславянскіх моваў).",
                "segoe",
                null
            },
            {
                "Chinese",
                "漢語，又称中文、汉文、國文、國语、华文、华语，英文統一用「Chinese」，常用作简称现代标准汉语，古代的汉语称为文言文。属于汉藏语系分析语，有声调。汉语的文字系统汉字是一种意音文字，表意的同時也具一定的表音功能。漢語包含書面語以及口語兩部分。一般意義上，“漢語”這個詞，多指現代標準漢語，以北京语音为标准音、以北方话为基础方言、以典范的现代白话文著作为语法规范。[2]在中國大陆漢語語言文字課本中稱為語文。需要注意的是，中国大陆之“普通话”、台灣之“國語”、新马地区之“华语”，在某些漢字的取音上是有一定程度的差异的，而且口語讀音也出現不少分野，亦有一些汉字的读音在三者中根本不同。而大部分較中立的學者認為，漢語是眾多漢方言的統稱，是一個稱謂，可以指代任何其中一種方言（如粵語、吳語、客家話……），而非單一是語言。",
                "malgun",
                null
            },
            {
                "Czech",
                "Čeština je západoslovanský jazyk, nejvíce příbuzný se slovenštinou, poté polštinou a lužickou srbštinou. Patří mezi slovanské jazyky, do rodiny jazyků indoevropských. Vyvinula se ze západních nářečí praslovanštiny na konci 10. století. Česky psaná literatura se objevuje od 14. století. První písemné památky jsou však již z 12. století.",
                "segoe",
                null
            },
            {
                "English",
                "English is a West Germanic language originating in England and is the first language for most people in the United Kingdom, the United States, Canada, Australia, New Zealand, Ireland and the Anglophone Caribbean. It is used extensively as a second language and as an official language throughout the world, especially in Commonwealth countries and in many international organisations.",
                "segoe",
                null
            },
            {
                "French",
                "Le français est une langue romane parlée principalement en France, dont elle est originaire (la « langue d'oïl »), ainsi qu'au Canada (principalement au Québec, dans le nord et l'est du Nouveau-Brunswick et dans l'est et le nord-est de l'Ontario), en Belgique (en Région wallonne et en Région de Bruxelles-Capitale) et en Suisse (en Romandie). Le français est parlé comme deuxième ou troisième langue dans d'autres régions du monde, comme dans la République démocratique du Congo, le plus peuplé des pays de la francophonie[1] et l'un des 29 [2] pays ayant le français pour langue officielle ou co-officielle, ou encore au Magreb. Ces pays ayant pour la plupart fait partie des anciens empires coloniaux français et belge.",
                "segoe",
                null
            },
            {
                "German",
                "Die deutsche Sprache gehört zum westlichen Zweig der germanischen Sprachen. Die hochdeutsche Standardsprache wird auch als Standarddeutsch, außerhalb der sprachwissenschaftlichen Fachsprache als Schriftdeutsch oder einfach Hochdeutsch bezeichnet. Der (zusammenhängende) deutsche Sprachraum umfasst viele mitteleuropäische Staaten (Deutschland, Österreich, Belgien (Ostbelgien), Schweiz (Deutschschweiz), Liechtenstein, Luxemburg) und Regionen (Südtirol, Elsass und Lothringen, zahlreiche Gemeinden Polens) in anderen Staaten. Aufgrund von Auswanderungen werden Deutsch und seine Mundarten auch anderswo gesprochen.",
                "segoe",
                null
            },
            {
                "Greek",
                "Η ελληνική αποτελεί τη μητρική γλώσσα περίπου 12 εκατομμυρίων ανθρώπων, κυρίως στην Ελλάδα και στην Κύπρο. Αποτελεί επίσης την μητρική γλώσσα αυτοχθόνων πληθυσμών στην Αλβανία, στη Βουλγαρία, στην ΠΓΔΜ και στην Τουρκία. Εξαιτίας της μετανάστευσης η γλώσσα μιλιέται ακόμα σε χώρες-προορισμούς ελληνόφωνων πληθυσμών μεταξύ των οποίων η Αυστραλία, ο Καναδάς, η Γερμανία, το Ηνωμένο Βασίλειο, η Ρωσία, η Σερβία και οι Ηνωμένες Πολιτείες. Συνολικά υπολογίζεται ότι ο συνολικός αριθμός ανθρώπων που μιλάνε τα ελληνικά σαν πρώτη ή δεύτερη γλώσσα είναι γύρω στα 20 εκατομμύρια.",
                "segoe",
                null
            },
            {
                "Hebrew",
                "השם עבר מופיע בתנ\"ך כשמו של סבו של אברהם אבינו. המושג \"עברי\" נזכר בתנ\"ך פעמים רבות, אולם שפתם של העברים אינה נקראת עברית. כיום מכנים את שפת התנ\"ך \"לשון המקרא\" (או \"לשון הקודש\") כדי להבדיל אותה מלשון חז\"ל המכונה גם \"לשון חכמים\", שהיא בעצם ניב מאוחר של עברית. המונח כתב עברי מציין בלשונם של חז\"ל דווקא את הכתב הארמי על שם \"עבר הנהר\". הקובץ המפורסם ביותר שנכתב בשפה העברית הוא התנ\"ך, אם כי בו עצמו לא נזכר שמה של השפה. עם זאת, במלכים ב' יח, כו, ובישעיהו לו, יא, מסופר כי שליחי חזקיהו המלך מבקשים מרבשקה, שליחו של סנחריב מלך אשור, לדבר עמם ב\"ארמית\" ולא ב\"יהודית\", כדי שהעם (שכנראה לא דיבר ארמית) לא יבין את דבריהם, ונראה שזה היה שמה של השפה, או לפחות שמו של הניב שדובר באזור ירושלים.",
                "segoe",
                "rtl"
            },
            {
                "Hindi",
                "हिन्दी संवैधानिक रूप से भारत की प्रथम राजभाषा है और भारत की सबसे ज्यादा बोली और समझी जानेवाली भाषा भी है। हिन्दी और इसकी बोलियाँ उत्तर एवं मध्य भारत के विविध प्रांतों में बोली जाती हैं । २६ जनवरी १९६५ को हिन्दी को भारत की आधिकारिक भाषा का दर्जा दिया गया।",
                "segoe",
                null
            },
            {
                "Italian",
                "L'italiano è una lingua appartenente al gruppo delle lingue romanze della famiglia delle lingue indoeuropee. Convive con un gran numero di idiomi neo-romanzi e ha delle varianti regionali, per via dell'influenza che su di esso esercitano le lingue regionali. L'italiano è lingua ufficiale dell'Italia, di San Marino, della Svizzera (insieme al francese e al tedesco; mentre il romancio è lingua nazionale ma ufficiale soltanto nel Canton Grigioni), della Città del Vaticano (insieme al latino) e del Sovrano Militare Ordine di Malta. È seconda lingua, coufficiale insieme col croato, nella Regione Istriana (Croazia) e, insieme con lo sloveno, nelle città di Pirano, Isola d'Istria e Capodistria in Slovenia. Pur non figurando tra le lingue parlate in questi paesi, e non essendo quindi utilizzato a livello ufficiale, l'italiano è inoltre ampiamente compreso nella restante parte della Venezia Giulia ceduta alla Jugoslavia nel 1947, nel Principato di Monaco, a Malta, in Corsica e nel Nizzardo (Francia) e, in misura minore, in Albania, Libia, Eritrea, Stati Uniti d'America nordorientali, Argentina e Brasile meridionale.",
                "segoe",
                null
            },
            {
                "Japanese",
                "日本語（にほんご、にっぽんご）は、主として、日本列島で使用されてきた言語である。日本手話を母語とする者などを除いて、ほぼ全ての日本在住者は日本語を第一言語とする。日本国は法令上、公用語を明記していないが、事実上の公用語となっており、学校教育の「国語」で教えられる。使用者は、日本国内を主として約1億3千万人。日本語の文法体系や音韻体系を反映する手話として日本語対応手話がある。",
                "malgun",
                null
            },
            {
                "Korean",
                "한국어(韓國語)는 한반도 등지에서 주로 사용하는 언어로, 대한민국(남한)에서는 한국어, 한국말, 국어(國語)라고 부른다. 조선민주주의인민공화국(북한), 중국(조선족위주)을 비롯한 등지에서는 조선말, 조선어(朝鮮語)로, 카자흐스탄을 비롯 중앙아시아의 고려인들 사이에서는 고려말(高麗말)로 불린다.",
                "malgun",
                null
            },
            {
                "Portuguese",
                "A língua portuguesa, com mais de 170 milhões de falantes nativos, é a quinta língua mais falada no mundo e a terceira mais falada no mundo ocidental. É o idioma oficial de Angola, Brasil, Cabo Verde, Guiné-Bissau, Guiné Equatorial, Macau, Moçambique, Portugal, São Tomé e Príncipe e Timor-Leste, sendo também falada nos antigos territórios da Índia Portuguesa (Goa, Damão, Diu e Dadrá e Nagar-Aveli), além de ter também estatuto oficial na União Europeia, no Mercosul e na União Africana.",
                "segoe",
                null
            },
            {
                "Russian",
                "Русский язык — один из восточнославянских языков, один из крупнейших языков мира, в том числе самый распространённый из славянских языков и самый распространённый язык Европы, как географически, так и по числу носителей языка как родного (хотя значительная, и географически бо́льшая, часть русского языкового ареала находится в Азии).",
                "segoe",
                null
            },
            {
                "Spanish",
                "El español o castellano es una lengua romance del grupo ibérico. Desde el punto de vista estrictamente lingüístico, el español es una familia de cincuenta y ocho lenguas o variedades, que constituyen una cadena de solidaridad lingüística, con eslabones contiguos o eslabones más separados. Es uno de los seis idiomas oficiales de la ONU y, tras el chino mandarín, es la lengua más hablada del mundo por el número de hablantes que la tienen como lengua materna. Es también idioma oficial en varias de las principales organizaciones político-económicas internacionales (UE, UA, TLCAN y UNASUR, entre otras). Lo hablan como primera y segunda lengua entre 450 y 500 millones de personas, pudiendo ser la segunda lengua más hablada considerando los que lo hablan como primera y segunda lengua. Por otro lado, el español es el segundo idioma más estudiado en el mundo tras el inglés, con al menos 17,8 millones de estudiantes, si bien otras fuentes indican que se superan los 46 millones de estudiantes distribuidos en 90 países.",
                "segoe",
                null
            },
            {
                "Thai",
                "ภาษาไทย เป็นภาษาราชการของประเทศไทย และภาษาแม่ของชาวไทย และชนเชื้อสายอื่นในประเทศไทย ภาษาไทยเป็นภาษาในกลุ่มภาษาไต ซึ่งเป็นกลุ่มย่อยของตระกูลภาษาไท-กะได สันนิษฐานว่า ภาษาในตระกูลนี้มีถิ่นกำเนิดจากทางตอนใต้ของประเทศจีน และนักภาษาศาสตร์บางท่านเสนอว่า ภาษาไทยน่าจะมีความเชื่อมโยงกับ ตระกูลภาษาออสโตร-เอเชียติก ตระกูลภาษาออสโตรนีเซียน ตระกูลภาษาจีน-ทิเบต",
                "segoe",
                null
            },
            {
                "Turkish",
                "Türkçe, (Türkiye Türkçesi olarak da bilinir) Ural-Altay dil ailesine bağlı Türk dillerinden ve Oğuz Grubu'na mensup bir dildir. Türkiye, Kıbrıs, Balkanlar ve Orta Avrupa ülkeleri başta olmak üzere geniş bir coğrafyada konuşulmaktadır. Türkiye Cumhuriyeti,Kuzey Kıbrıs Türk Cumhuriyeti ve Kıbrıs Cumhuriyeti'nin resmî; Makedonya ve Kosova'nın ise tanınmış bölgesel dilidir. Türkçe, farklı ağızlara ayrılmış bir dildir. Ancak bu ağızlardan İstanbul lehçesi, sivrileşerek yazı dili haline gelmiştir. Türkçe, 8 ünlü harf sayısıyla beraber zengin bir dil olmasının yanı sıra özne-nesne-yüklem şeklindeki cümle kuruluşlarıyla bilinmektedir.",
                "segoe",
                null
            },
            {
                "Ukrainian",
                "Українська мова належить до індоєвропейської мовної родини, слов'янської групи і разом з російською та білоруською до східнослов'янської підгрупи. Найближчою генеалогічно до української є білоруська мова, адже обидві ці мови походять від давньоукраїнської і почали окремо розвиватися, починаючи з 15-17 століть. Українська мова має три групи діалектів. Декілька південно-західних говорів було кодифіковано паралельно з літературною українською мовою, кількома зарубіжними мовознавцями ці кодифіковані говірки вважаються окремою русинською мовою.",
                "segoe",
                null
            }
        };
    }
}
