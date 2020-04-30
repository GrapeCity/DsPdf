//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;

namespace GcPdfWeb.Samples.Basics
{
    // Ligatures (joining two or more letters into a single glyph) are supported by GcPdf,
    // provided the selected font supports them, and the corresponding font feature is on.
    // For the complete list of font features see https://www.microsoft.com/typography/otspec/featurelist.htm.
    // See also FontFeatures.
    public class Ligatures
    {
        public void CreatePDF(Stream stream)
        {
            // The list of common Latin ligatures:
            const string latinLigatures = "fi, fj, fl, ff, ffi, ffl.";
            // Set up ligature-related font features:
            // All ON:
            FontFeature[] allOn = new FontFeature[]
            {
                new FontFeature(FeatureTag.clig, true), // Contextual Ligatures
                new FontFeature(FeatureTag.dlig, true), // Discretionary Ligatures
                new FontFeature(FeatureTag.hlig, true), // Historical Ligatures
                new FontFeature(FeatureTag.liga, true), // Standard Ligatures
                new FontFeature(FeatureTag.rlig, true), // Required Ligatures
            };
            // All OFF:
            FontFeature[] allOff = new FontFeature[]
            {
                new FontFeature(FeatureTag.clig, false),
                new FontFeature(FeatureTag.dlig, false),
                new FontFeature(FeatureTag.hlig, false),
                new FontFeature(FeatureTag.liga, false),
                new FontFeature(FeatureTag.rlig, false),
            };
            var doc = new GcPdfDocument();
            var g = doc.NewPage().Graphics;
            // Text insertion point:
            PointF ip = new PointF(72, 72);
            TextFormat tf = new TextFormat();
            tf.Font = Font.FromFile(Path.Combine("Resources", "Fonts", "times.ttf"));
            tf.FontSize = 20;
            g.DrawString($"Common Latin ligatures, font {tf.Font.FontFamilyName}", tf, ip);
            ip.Y += 36;
            // Turn all ligature features OFF:
            tf.FontFeatures = allOff;
            g.DrawString($"All ligature features OFF: {latinLigatures}", tf, ip);
            ip.Y += 36;
            // Turn all ligature features ON:
            tf.FontFeatures = allOn;
            g.DrawString($"All ligature features ON: {latinLigatures}", tf, ip);
            ip.Y += 72;
            // Repeat with a different font:
            tf.Font = Font.FromFile(Path.Combine("Resources", "Fonts", "Gabriola.ttf"));
            g.DrawString($"Common Latin ligatures, font {tf.Font.FontFamilyName}", tf, ip);
            ip.Y += 36;
            // Turn all ligature features OFF:
            tf.FontFeatures = allOff;
            g.DrawString($"All ligature features OFF: {latinLigatures}", tf, ip);
            ip.Y += 36;
            // Turn all ligature features ON:
            tf.FontFeatures = allOn;
            g.DrawString($"All ligature features ON: {latinLigatures}", tf, ip);
            // Done:
            doc.Save(stream);
        }
    }
}
