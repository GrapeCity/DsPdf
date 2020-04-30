//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;
using GrapeCity.Documents.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // This sample demonstrates how to render short upright Latin text or numbers
    // in a block of vertical text. This is used in Chinese, Japanese and
    // Korean vertical text. In CSS this is referred to using the Japanese
    // name 縦中横 (tate chu yoko). To achieve this in GcGraphics,
    // the TextFormat.UprightInVerticalText property should be set.
    // A number of other properties on TextLayout and TextFormat 
    // allow to fine-tune the behavior, as shown in this sample.
    public class TateChuYoko
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;

            var rc = Common.Util.AddNote(
                "Vertical text often includes short runs of horizontal numbers or Latin text. " +
                "In CSS this is referred to using the Japanese name 縦中横 (tate chu yoko). " +
                "It occurs in Chinese, Japanese and Korean vertical text. " +
                "We support this by providing TextFormat.UprightInVerticalText and a few " +
                "related properties on TextLayout and TextFormat.",
                page);

            var fntJp = Font.FromFile(Path.Combine("Resources", "Fonts", "YuGothM.ttc"));
            var fntLat = Font.FromFile(Path.Combine("Resources", "Fonts", "MyriadPro-Cond.otf"));
            var hiliteFore = Color.DarkSlateBlue;
            var hiliteBack = Color.FromArgb(unchecked((int)0xffffff99));

            var tl = g.CreateTextLayout();

            // Set text flow and other layout properties:
            tl.FlowDirection = FlowDirection.VerticalRightToLeft;
            tl.MaxWidth = page.Size.Width;
            tl.MaxHeight = page.Size.Height;
            tl.MarginAll = 72;
            tl.MarginTop = rc.Bottom + 36;
            tl.ParagraphSpacing = 12;
            tl.LineSpacingScaleFactor = 1.4f;

            // g.FillRectangle(
            // new RectangleF(tl.MarginLeft, tl.MarginTop, tl.MaxWidth.Value - tl.MarginLeft - tl.MarginRight, tl.MaxHeight.Value - tl.MarginTop - tl.MarginBottom),
            // Color.AliceBlue);

            // Text format for upright text (short Latin words or numbers)
            // (GlyphWidths turns on corresponding font features, but makes a difference
            // only of those features are present in the font):
            var fUpright = new TextFormat()
            {
                Font = fntLat,
                FontSize = 14,
                UprightInVerticalText = true,
                GlyphWidths = GlyphWidths.QuarterWidths,
                TextRunAsCluster = true,
            };
            // Text format for vertical Japanese and sideways Latin text:
            var fVertical = new TextFormat(fUpright)
            {
                Font = fntJp,
                UprightInVerticalText = false,
                GlyphWidths = GlyphWidths.Default,
                TextRunAsCluster = false,
            };

            // Make sure runs of sideways text do not affect vertical spacing:
            fVertical.UseVerticalLineGapForSideways = true;
            // This commented fragment effectively would do the same as the
            // UseVerticalLineGapForSideways property setting above:
            /*
            var scale = fVertical.FontSize * tl.FontScaleFactor / fntJp.UnitsPerEm;
            if (!fVertical.FontSizeInGraphicUnits)
                scale *= tl.Resolution / 72f;
            fVertical.LineGap = fntJp.VerticalLineGap * scale;
            */

            // Two additional text formants for highlighted headers:
            var fUpHdr = new TextFormat(fUpright)
            {
                ForeColor = hiliteFore,
                BackColor = hiliteBack
            };
            var fVertHdr = new TextFormat(fVertical)
            {
                ForeColor = hiliteFore,
                BackColor = hiliteBack
            };

            tl.Append("PDF", fUpright);
            tl.Append("ファイルをコードから", fVertical);
            tl.Append("API", fUpright);
            tl.Append("を利用することで操作できます。クロスプラットフォーム環境で動作するアプリケーションの開発を支援する", fVertical);
            tl.Append("API", fUpright);
            tl.Append("ライブラリです。", fVertical);

            // Smaller font size for the rest of the text:
            fUpright.FontSize -= 2;
            fVertical.FontSize -= 2;

            // 1:
            tl.AppendParagraphBreak();
            tl.Append("PDF", fUpHdr);
            tl.Append("用の包括的な", fVertHdr);
            tl.Append("API", fUpHdr);

            tl.AppendSoftBreak();
            tl.Append("PDF", fUpright);
            tl.Append("バージョン「", fVertical);
            tl.Append("1.7", fUpright);
            tl.Append("」に準拠した", fVertical);
            tl.Append("API", fUpright);
            tl.Append("を提供し、レイアウトや機能を損なうことなく、豊富な機能を備えた", fVertical);
            tl.Append("PDF", fUpright);
            tl.Append("文書を生成、編集、保存できます。", fVertical);

            // 2:
            tl.AppendParagraphBreak();
            tl.Append("完全なテキスト描画", fVertHdr);

            tl.AppendSoftBreak();
            tl.Append("PDF", fUpright);
            tl.Append("文書にテキストの描画情報が保持されます。テキストと段落の書式、特殊文字、複数の言語、縦書き、テキスト角度などが保持さるので、完全な形でテキスト描画を再現できます。", fVertical);

            // 3:
            tl.AppendParagraphBreak();
            tl.Append(".NET Standard 2.0 準拠", fVertHdr);

            tl.AppendSoftBreak();
            tl.Append(".NET Core、.NET Framework、Xamarinで動作するアプリケーションを開発できます。Windows、macOS、Linuxなどクロスプラットフォーム環境で動作可能です。", fVertical);

            // 4:
            tl.AppendParagraphBreak();
            tl.Append("100", fUpHdr);
            tl.Append("を超える", fVertHdr);
            tl.Append("PDF", fUpHdr);
            tl.Append("操作機能", fVertHdr);

            tl.AppendSoftBreak();
            tl.Append("ページの追加や削除、ページサイズ、向きの変更だけでなく、ファイルの圧縮、", fVertical);
            tl.Append("Web", fUpright);
            tl.Append("に最適化した", fVertical);
            tl.Append("PDF", fUpright);
            tl.Append("の生成など高度な機能も", fVertical);
            tl.Append("API", fUpright);
            tl.Append("操作で実現します。また、署名からセキュリティ機能まで様々な機能を含んだ", fVertical);
            tl.Append("PDF", fUpright);
            tl.Append("フォームを生成可能です。", fVertical);

            // 5:
            tl.AppendParagraphBreak();
            tl.Append("高速、軽量アーキテクチャ", fVertHdr);

            tl.AppendSoftBreak();
            tl.Append("軽量", fVertical);
            tl.Append("API", fUpright);
            tl.Append("アーキテクチャでメモリと時間を節約できます。", fVertical);
            tl.AppendSoftBreak();
            tl.Append("また、他の生成用ツールに依存せずドキュメントを生成可能です。", fVertical);

            // 6:
            tl.AppendParagraphBreak();
            tl.Append("クラウドアプリケーション展開", fVertHdr);
            tl.Append("", fUpHdr);

            tl.AppendSoftBreak();
            tl.Append("Azure、AWSなどのサービスに配置するクラウドアプリケーションの開発で利用可能です。仮想マシン、コンテナ、サーバーレスなどの方法で配置できます。", fVertical);

            tl.PerformLayout(true);
            g.DrawTextLayout(tl, PointF.Empty);

            // Done:
            doc.Save(stream);
        }
    }
}
