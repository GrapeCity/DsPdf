//
// This code is part of http://localhost:20395.
// Copyright (c) GrapeCity, Inc. All rights reserved.
//
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GrapeCity.Documents.Pdf;

namespace GcPdfSampleApp
{
    class Program
    {
        //
        // This is a very simple driver that allows to run GcPdf samples.
        // These samples are also available from the GrapeCity Documents for PDF sample browser,
        // see https://www.grapecity.com/ for details.
        //
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 1 && args[0] == "/runall")
                    RunAll();
                else
                    RunUI();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }

        static void RunUI()
        {
            var samples = GetSamples();
            samples.Sort((a, b) => a.type.Name.CompareTo(b.type.Name));
            while (true)
            {
                (Type type, MethodInfo createPdf) sample = (null, null);
                for (int i = 0; i < samples.Count; i += 10)
                {
                    List<(int, Type, MethodInfo)> ten = new List<(int, Type, MethodInfo)>();
                    Console.Clear();
                    Console.SetCursorPosition(0, 0);
                    Console.WriteLine($"Showing samples {i + 1}-{Math.Min(i + 10, samples.Count)} of {samples.Count}:");
                    for (int j = i; j < i + 10 && j < samples.Count; ++j)
                    {
                        var (type, createPdf) = samples[j];
                        ten.Add((j, type, createPdf));
                        Console.WriteLine($"{j - i}: {type.Name}");
                    }
                    Console.WriteLine("Press (0-9) to run a sample, PageUp/PageDown to page, Escape to abort...");
                    var key = Console.ReadKey(true);
                    if (key.KeyChar >= '0' && key.KeyChar <= '9')
                    {
                        int k = key.KeyChar - '0';
                        Console.WriteLine($"Running {k}: {ten[k].Item2.Name}...");
                        sample = (ten[k].Item2, ten[k].Item3);
                        break;
                    }
                    else if (key.Key == ConsoleKey.PageUp)
                        i = Math.Max(-10, i - 20);
                    else if (key.Key == ConsoleKey.Escape)
                    {
                        Console.WriteLine("Aborted.");
                        return;
                    }
                    else if (key.Key != ConsoleKey.PageDown)
                        i = Math.Max(-10, i - 10);
                }

                if (sample.type != null)
                {
                    var fname = $"{sample.type.Name}.pdf";
                    Console.WriteLine($"Press ENTER to create '{fname}' in the current directory,\nor enter an alternative file name:");
                    var t = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(t))
                        fname = t;
                    fname = Path.GetFullPath(fname);
                    MakePdf(sample, fname);

                    // Uncomment to open the generated file:
                    // ProcessStartInfo psi = new ProcessStartInfo() { FileName = fname, UseShellExecute = true };
                    // Process.Start(psi);
                }
            }
        }

        static void RunAll()
        {
            if (Directory.Exists("temp"))
                throw new Exception("Directory ./temp exists, aborting...");
            DirectoryInfo di = Directory.CreateDirectory("temp");
            if (di == null || !di.Exists)
                throw new Exception("Could not create directory ./temp, aborting...");

            var samples = GetSamples();
            foreach (var sample in samples)
                MakePdf(sample, Path.Combine(di.FullName, $"{sample.type.Name}.pdf"));

            Console.WriteLine("All samples ran, results are in ./temp.");
        }

        static string MakePdf((Type type, MethodInfo createPdf) sample, string fname)
        {
            if (sample.type == null)
                return null;

            if (File.Exists(fname))
            {
                Console.WriteLine($"'{fname}' exists.\nOVERWRITE (Y/n)?");
                var yesno = Console.ReadLine();
                if (yesno.ToLower() == "n")
                {
                    Console.WriteLine("Aborted.");
                    return null;
                }
            }
            Console.WriteLine($"Generating '{fname}'...");

            var sampleInst = Activator.CreateInstance(sample.type);
            using (FileStream fs = new FileStream(fname, FileMode.Create))
                sample.createPdf.Invoke(sampleInst, new object[] { fs });

            Console.WriteLine($"Created '{fname}' successfully.");
            return fname;
        }

        static List<(Type type, MethodInfo createPdf)> GetSamples()
        {

            List<(Type type, MethodInfo createPdf)> samples = new List<(Type type, MethodInfo createPdf)>();
            Assembly asm = typeof(Program).GetTypeInfo().Assembly;
            foreach (var t in asm.GetExportedTypes())
            {
                var createPdf = t.GetMethod("CreatePDF", new Type[] { typeof(Stream) });
                if (createPdf != null)
                    samples.Add((t, createPdf));
            }
            return samples;
        }
    }
}
