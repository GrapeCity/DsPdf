using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrapeCity.Documents.Pdf;
using GrapeCity.Documents.Text;
using GrapeCity.Documents.Drawing;

namespace GcPdfWeb.Samples.Basics
{
    // Demonstrates how different styles of numbered lists can be rendered in GcPdf.
    // See also NumberedList.
    public class NumberedList2
    {
        // Encapsulate page layout constants used in the sample:
        private struct Layout
        {
            // All around page margins:
            public static float Margin => 72;
            // List offset relative to item number:
            public static float ListOffset => 24;
            // List level indent:
            public static float ListIndent => 30;
        };

        // Define simple tree type:
        private class Node
        {
            public Node(string text)
            {
                Text = text;
            }
            public string Text { get; }
            public List<Node> Children = new List<Node>();
        }

        // Renders a list of nodes as a numbered list.
        private PointF RenderNodes(ref Page page, PointF pt, List<Node> nodes, int level)
        {
            TextLayout tlBullet = new TextLayout();
            tlBullet.DefaultFormat.Font = StandardFonts.Times;
            tlBullet.MarginLeft = Layout.ListIndent * level;

            TextLayout tlText = new TextLayout();
            tlText.DefaultFormat.Font = StandardFonts.Times;
            tlText.MarginLeft = Layout.ListOffset + Layout.ListIndent * level;

            for (int i = 0; i < nodes.Count; ++i)
            {
                var g = page.Graphics;
                // Prep item text:
                tlText.Clear();
                tlText.Append(nodes[i].Text);
                tlText.PerformLayout(true);
                if (pt.Y + tlText.ContentHeight > page.Size.Height - Layout.Margin)
                {
                    page = page.Doc.NewPage();
                    g = page.Graphics;
                    pt.Y = Layout.Margin;
                }
                // Prep item number:
                tlBullet.Clear();
                tlBullet.Append(ItemIdxToString(i, level, tlBullet));
                tlBullet.PerformLayout(true);
                // Render item:
                g.DrawTextLayout(tlBullet, pt);
                g.DrawTextLayout(tlText, pt);
                // increase insertion point:
                pt.Y += tlText.ContentHeight;
                // Render children:
                if (nodes[i].Children.Count > 0)
                    pt = RenderNodes(ref page, pt, nodes[i].Children, level + 1);
            }
            return pt;
        }

        // Convert item index to item number representation, in a 
        // Arabic -> Latin letters -> Roman numbers loop.
        // Roman numbers are right-aligned, others are left-aligned.
        private string ItemIdxToString(int itemIdx, int level, TextLayout tl)
        {
            switch (level % 3)
            {
                case 0:
                    tl.MarginLeft = Layout.ListIndent * level;
                    tl.MaxWidth = null;
                    tl.TextAlignment = TextAlignment.Leading;
                    return $"{itemIdx + 1}.";
                case 1:
                    tl.MarginLeft = Layout.ListIndent * level;
                    tl.MaxWidth = null;
                    tl.TextAlignment = TextAlignment.Leading;
                    return $"{(char)('a' + itemIdx)}.";
                case 2:
                    tl.MarginLeft = 0;
                    var font = tl.DefaultFormat.Font;
                    tl.MaxWidth = Layout.ListIndent * level + tl.DefaultFormat.FontSize;
                    tl.TextAlignment = TextAlignment.Trailing;
                    return $"{IntToRoman(itemIdx + 1)}.";
                default:
                    throw new Exception("Unexpected.");
            }
        }

        // From http://dotnet-snippets.com/snippet/roman-numerals/667
        private string IntToRoman(int number)
        {
            StringBuilder result = new StringBuilder();
            int[] digitsValues = { 1, 4, 5, 9, 10, 40, 50, 90, 100, 400, 500, 900, 1000 };
            string[] romanDigits = { "I", "IV", "V", "IX", "X", "XL", "L", "XC", "C", "CD", "D", "CM", "M" };
            while (number > 0)
            {
                for (int i = digitsValues.Count() - 1; i >= 0; i--)
                    if (number / digitsValues[i] >= 1)
                    {
                        number -= digitsValues[i];
                        result.Append(romanDigits[i].ToLower());
                        break;
                    }
            }
            return result.ToString();
        }

        // Main:
        public void CreatePDF(Stream stream)
        {
            var doc = new GcPdfDocument();
            var page = doc.NewPage();
            RenderNodes(ref page, new PointF(Layout.Margin, Layout.Margin), _arthrapods.Children, 0);
            // Done:
            doc.Save(stream);
        }

        // Sample tree data:
        private Node _arthrapods = new Node("Animalia Arthrapoda")
        {
            Children = new List<Node>()
            {
                new Node("Insecta")
                {
                    Children = new List<Node>()
                    {
                        new Node("Archaeognatha")
                        {
                            Children = new List<Node>()
                            {
                                new Node("Protozoa")
                            },
                        },
                        new Node("Thysanura")
                        {
                            Children = new List<Node>()
                            {
                                new Node("Silverfish")
                            },
                        },
                        new Node("Ephemeoptera")
                        {
                            Children = new List<Node>()
                            {
                                new Node("Mafly")
                            },
                        },
                        new Node("Odonata")
                        {
                            Children = new List<Node>()
                            {
                                new Node("Dragonfly"),
                                new Node("Azure Damzelfly"),
                                new Node("Emerald Damzelfly"),
                            },
                        },
                        new Node("Orthoptera")
                        {
                            Children = new List<Node>()
                            {
                                new Node("Grasshopper"),
                                new Node("Cricket"),
                                new Node("Cave Cricket"),
                            },
                        },
                        new Node("Phasmatodea")
                        {
                            Children = new List<Node>()
                            {
                                new Node("Walking Stick"),
                                new Node("Leaf Bug"),
                            },
                        },
                        new Node("Mantodea")
                        {
                            Children = new List<Node>()
                            {
                                new Node("Praying Mantis"),
                            },
                        },
                        new Node("Blattoeda")
                        {
                            Children = new List<Node>()
                            {
                                new Node("Cockroach"),
                            },
                        },
                        new Node("Isoptera")
                        {
                            Children = new List<Node>()
                            {
                                new Node("Termite"),
                            },
                        },
                        new Node("Phithiraptera")
                        {
                            Children = new List<Node>()
                            {
                                new Node("Bird Lice"),
                                new Node("Human Lice"),
                                new Node("Pubic Lice"),
                            },
                        },
                        new Node("Hemiptera")
                        {
                            Children = new List<Node>()
                            {
                                new Node("Cicada"),
                                new Node("Pond Skater"),
                                new Node("Tree Hopper"),
                                new Node("Stink Bug"),
                                new Node("Thrip"),
                                new Node("Alderfly"),
                            },
                        },
                        new Node("Siphonatera")
                        {
                            Children = new List<Node>()
                            {
                                new Node("Flea"),
                            },
                        },
                        // Coleoptera etc skipped
                    },
                },
                new Node("Myriapoda")
                {
                    Children = new List<Node>()
                    {
                        new Node("Chilopoda")
                        {
                            Children  = new List<Node>()
                            {
                                new Node("Centipede"),
                            },
                        },
                        new Node("Diplopoda")
                        {
                            Children  = new List<Node>()
                            {
                                new Node("Millipede"),
                                new Node("Pitbug"),
                            },
                        },
                    },
                },
                new Node("Crustacea")
                {
                    Children = new List<Node>()
                    {
                        new Node("Branchiopod")
                        {
                            Children  = new List<Node>()
                            {
                                new Node("Brine Shrimp"),
                                new Node("Water Flea"),
                            },
                        },
                        new Node("Maxillopod")
                        {
                            Children  = new List<Node>()
                            {
                                new Node("Cyclopoid"),
                                new Node("Calgid"),
                                new Node("Barnacles"),
                            },
                        },
                        new Node("Malacostracan")
                        {
                            Children  = new List<Node>()
                            {
                                new Node("Krill"),
                                new Node("Prawn"),
                                new Node("Shrimp"),
                                new Node("Cancrid Crab"),
                                new Node("Fidder Crab"),
                                new Node("Spider Crab"),
                                new Node("Lobster"),
                                new Node("Hermit Crab"),
                                new Node("Cray Fish"),
                            },
                        },
                    },
                },
                new Node("Pycnogonida")
                {
                    Children = new List<Node>()
                    {
                        new Node("Pycnogonida")
                        {
                            Children  = new List<Node>()
                            {
                                new Node("Sea Spider"),
                            },
                        },
                    },
                },
                new Node("Merostomata")
                {
                    Children = new List<Node>()
                    {
                        new Node("Merostomata")
                        {
                            Children  = new List<Node>()
                            {
                                new Node("Horseshoe Spider"),
                            },
                        },
                    },
                },
                new Node("Arachnida")
                {
                    Children = new List<Node>()
                    {
                        new Node("Scorpiones")
                        {
                            Children  = new List<Node>()
                            {
                                new Node("Buthid"),
                                new Node("Imperial Scorpion"),
                            },
                        },
                        new Node("Pseudoscorpions")
                        {
                            Children  = new List<Node>()
                            {
                                new Node("Neobisiid"),
                                new Node("Cheliferid"),
                            },
                        },
                        new Node("Solfigugae")
                        {
                            Children  = new List<Node>()
                            {
                                new Node("Wind Scorpion"),
                            },
                        },
                        new Node("Acari")
                        {
                            Children  = new List<Node>()
                            {
                                new Node("Tick"),
                                new Node("Mite"),
                            },
                        },
                        new Node("Araneae")
                        {
                            Children  = new List<Node>()
                            {
                                new Node("Crib Weaver"),
                                new Node("Funnel-web Spider"),
                                new Node("Funnel Weaver"),
                                new Node("Water Spider"),
                                new Node("Jumping Spider"),
                                new Node("Wolf Spider"),
                                new Node("Nursery-web Spider"),
                                new Node("Crab Spider"),
                                new Node("Black Widow"),
                                new Node("Tiger Oriental Tarantula"),
                                new Node("Mexican Red-legged Tarantula"),
                            },
                        },
                    },
                },
            },
        };
    }
}
