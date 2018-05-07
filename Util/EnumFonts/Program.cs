using System;
using System.IO;
using System.Runtime.InteropServices;

using GrapeCity.Documents.Text;

namespace EnumFonts
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enumerating System Fonts (quick check)");

            int err = 0;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var fontsPath = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
                if (!string.IsNullOrEmpty(fontsPath))
                {
                    var dirInfo = new DirectoryInfo(fontsPath);
                    if (dirInfo.Exists)
                    {
                        foreach (var fileInfo in dirInfo.EnumerateFiles())
                        {
                            var ext = fileInfo.Extension;
                            if (string.Compare(ext, ".TTF", StringComparison.OrdinalIgnoreCase) == 0 ||
                                string.Compare(ext, ".TTC", StringComparison.OrdinalIgnoreCase) == 0 ||
                                string.Compare(ext, ".OTF", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                QuickParseFontFile(fileInfo.FullName, ref err);
                            }
                        }
                    }
                }
            }
            else
            {
                var paths = new string[]
                {
                    // linux directory list
                    "~/.fonts/",
                    "/usr/local/share/fonts/",
                    "/usr/share/fonts/",

                    // mac fonts
                    "~/Library/Fonts/",
                    "/Library/Fonts/",
                    "/Network/Library/Fonts/",
                    "/System/Library/Fonts/",
                    "/System Folder/Fonts/",
                };
                for (int i = 0; i < paths.Length; i++)
                {
                    var dirInfo = new DirectoryInfo(paths[i]);
                    if (dirInfo.Exists)
                    {
                        foreach (var fileInfo in dirInfo.EnumerateFiles("*", SearchOption.AllDirectories))
                        {
                            var ext = fileInfo.Extension;
                            if (string.Compare(ext, ".TTF", StringComparison.OrdinalIgnoreCase) == 0 ||
                                string.Compare(ext, ".TTC", StringComparison.OrdinalIgnoreCase) == 0 ||
                                string.Compare(ext, ".OTF", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                QuickParseFontFile(fileInfo.FullName, ref err);
                            }
                        }
                    }
                }
            }
            Console.WriteLine($"Quick check completed. Errors: {err}");
            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("Enumerating System Fonts (deep check)");

            err = 0;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var fontsPath = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
                if (!string.IsNullOrEmpty(fontsPath))
                {
                    var dirInfo = new DirectoryInfo(fontsPath);
                    if (dirInfo.Exists)
                    {
                        foreach (var fileInfo in dirInfo.EnumerateFiles())
                        {
                            var ext = fileInfo.Extension;
                            if (string.Compare(ext, ".TTF", StringComparison.OrdinalIgnoreCase) == 0 ||
                                string.Compare(ext, ".TTC", StringComparison.OrdinalIgnoreCase) == 0 ||
                                string.Compare(ext, ".OTF", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                ParseFontFile(fileInfo.FullName, ref err);
                            }
                        }
                    }
                }
            }
            else
            {
                var paths = new string[]
                {
                    // linux directory list
                    "~/.fonts/",
                    "/usr/local/share/fonts/",
                    "/usr/share/fonts/",

                    // mac fonts
                    "~/Library/Fonts/",
                    "/Library/Fonts/",
                    "/Network/Library/Fonts/",
                    "/System/Library/Fonts/",
                    "/System Folder/Fonts/",
                };
                for (int i = 0; i < paths.Length; i++)
                {
                    var dirInfo = new DirectoryInfo(paths[i]);
                    if (dirInfo.Exists)
                    {
                        foreach (var fileInfo in dirInfo.EnumerateFiles("*", SearchOption.AllDirectories))
                        {
                            var ext = fileInfo.Extension;
                            if (string.Compare(ext, ".TTF", StringComparison.OrdinalIgnoreCase) == 0 ||
                                string.Compare(ext, ".TTC", StringComparison.OrdinalIgnoreCase) == 0 ||
                                string.Compare(ext, ".OTF", StringComparison.OrdinalIgnoreCase) == 0)
                            {
                                ParseFontFile(fileInfo.FullName, ref err);
                            }
                        }
                    }
                }
            }
            Console.WriteLine($"Deep check completed. Errors: {err}");
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        static void QuickParseFontFile(string path, ref int err)
        {
            try
            {
                Console.Write($"parsing {path,-45}");

                Font.FromFile(path, true);

                Console.WriteLine("(done)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"({ex.Message})");
                err++;
            }
        }

        static void ParseFontFile(string path, ref int err)
        {
            try
            {
                Console.Write($"parsing {path,-45}");

                var fc = new FontCollection();
                fc.RegisterFont(path, true);

                Console.WriteLine("(done)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"({ex.Message})");
                err++;
            }
        }
    }
}
