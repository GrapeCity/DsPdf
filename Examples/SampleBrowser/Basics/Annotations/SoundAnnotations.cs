//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Drawing;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Pdf.Annotations;

namespace GcPdfWeb.Samples.Basics
{
    // This sample shows how to add sound annotations to a PDF document.
    public class SoundAnnotations
    {
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            var g = page.Graphics;
            // User names for annotations' authors:
            var user1 = "Aiff Ding";
            var user2 = "Wav Dong";

            TextFormat tf = new TextFormat() { Font = StandardFonts.Helvetica, FontSize = 10 };
            var noteWidth = 72 * 3;
            var gap = 8;

            var rc = Common.Util.AddNote(
                "This sample demonstrates adding sound annotations using GcPdf. " +
                "The track associated with an annotation can be played in a viewer that supports it. " +
                "PDF supports AIFF and WAV tracks in sound annotations.",
                page);

            // AIFF sound annotation:
            var ip = new PointF(rc.X, rc.Bottom + gap);
            rc = Common.Util.AddNote("A red sound annotation is placed to the right of this note. Double click the icon to play the sound.",
                page, new RectangleF(ip.X, ip.Y, noteWidth, 100));
            var aiffAnnot = new SoundAnnotation()
            {
                UserName = user1,
                Contents = "Sound annotation with an AIFF track.",
                Rect = new RectangleF(rc.Right, rc.Top, 24, 24),
                Icon = SoundAnnotationIcon.Speaker,
                Color = Color.Red,
                Sound = SoundObject.FromFile(Path.Combine("Resources", "Sounds", "ding.aiff"), AudioFormat.Aiff)
            };
            page.Annotations.Add(aiffAnnot);

            // WAV sound annotation:
            ip = new PointF(rc.X, rc.Bottom + gap);
            rc = Common.Util.AddNote("A blue sound annotation is placed to the right of this note. Double click the icon to play the sound.",
                page, new RectangleF(ip.X, ip.Y, noteWidth, 100));
            var wavAnnot = new SoundAnnotation()
            {
                UserName = user2,
                Contents = "Sound annotation with a WAV track.",
                Rect = new RectangleF(rc.Right, rc.Top, 24, 24),
                Icon = SoundAnnotationIcon.Mic,
                Color = Color.Blue,
                Sound = SoundObject.FromFile(Path.Combine("Resources", "Sounds", "dong.wav"), AudioFormat.Wav)
            };
            page.Annotations.Add(wavAnnot);

            // Done:
            doc.Save(stream);
        }
    }
}
