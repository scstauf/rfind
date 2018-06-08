using System;
using System.Collections.Generic;
using System.IO;

namespace RecursiveFind
{
    class Program
    {
        static void RFind(string path, string ext, string searchTerm, string contains)
        {
            var parseFile = !string.IsNullOrEmpty(contains);
            var searchFiles = !string.IsNullOrEmpty(searchTerm);

            if (!searchFiles && !parseFile)
            {
                throw new Exception("No \"--search\" or \"--contains\" argument was given. Please use the \"--help\" argument.");
            }

            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                throw new Exception("\"path\" is invalid or was not supplied.");
            }

            var chars = new { '\\', '-', '/' };

            foreach (var file in new DirectoryInfo(path).GetFiles(ext, SearchOption.AllDirectories))
            {
                Console.Write("\r");
                if (!searchFiles || file.FullName.ToLower().Contains(searchTerm))
                {
                    if (!parseFile || ParseFile(file.FullName, contains))
                    {
                        Console.WriteLine(file.FullName);
                    }
                }
            }
        }

        static bool ParseFile(string path, string contains)
        {
            var found = false;

            try
            {
                var content = File.ReadAllText(path);
                content = content.Replace("\t", " ").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("  ", "");

                if (content.ToLower().Contains(contains))
                {
                    found = true;
                }
            }
            catch { }

            return found;
        }

        static void Help()
        {
            Console.WriteLine(
                "rfind by scottyeatscode\r\n\r\n" +
                "rfind [--<path|extension|search|contains>=<values>]|[--help]"
            );
        }

        static string GetOpt(string option)
        {
            var value = string.Empty;

            if (option.IndexOf('=') < 0)
            {
                return value;
            }

            return option.Split('=')[1].Replace("\"", "");
        }

        static bool IsOpt(string test, params string[] options)
        {
            var testNoSymbols = string.Empty;
            var optionsNoSymbols = new string[options.Length];
            var isOption = false;

            if (test.IndexOf('=') < 0)
            {
                return isOption;
            }

            testNoSymbols = StripSymbols(test.Split('=')[0]);

            for (var i = 0; i < options.Length; i++)
            {
                optionsNoSymbols[i] = StripSymbols(options[i]);
            }

            foreach (var option in optionsNoSymbols)
            {
                isOption = testNoSymbols.ToLower().Equals(option.ToLower());
                if (isOption)
                {
                    break;
                }
            }

            return isOption;
        }

        static bool IsHelp(string test, params string[] options)
        {
            var help = test;

            if (help.IndexOf('=') < 0)
            {
                help += "=help";
            }

            return IsOpt(help, BuildOptFlags("h", "help", "?"));
        }

        static string StripSymbols(string str)
        {
            const char _QuestionMark = '?';
            var stripped = new List<char>();
            var index = 0;

            foreach (var c in str)
            {
                if (char.IsLetterOrDigit(c) || c.Equals(_QuestionMark))
                {
                    stripped.Add(c);
                    index++;
                }
            }

            return new string(stripped.ToArray());
        }

        static string[] BuildOptFlags(string optShort, string optLong, string optMed = "")
        {
            var lst = new List<string>(new string[] {
                $"-{optShort}",
                $"-{optLong}",
                $"--{optShort}",
                $"--{optLong}",
                $"/{optShort}",
                $"/{optLong}"
            });

            if (!string.IsNullOrEmpty(optMed))
            {
                lst.AddRange(new string[] {
                    $"-{optMed}",
                    $"--{optMed}",
                    $"/{optMed}"
                });
            }

            return lst.ToArray();
        }

        static void Main(string[] args)
        {
            var path = Environment.CurrentDirectory;
            var searchTerm = string.Empty;
            var extension = "*.*";
            var showHelp = false;
            var contains = string.Empty;
            
            try
            {
                var optPath = BuildOptFlags("p", "path");
                var optSearch = BuildOptFlags("s", "search");
                var optExt = BuildOptFlags("e", "extension", "ext");
                var optContains = BuildOptFlags("c", "contains");

                foreach (var arg in args)
                {
                    if (IsOpt(arg, optPath))
                    {
                        path = GetOpt(arg);
                    }
                    else if (IsOpt(arg, optSearch))
                    {
                        searchTerm = GetOpt(arg);
                    }
                    else if (IsOpt(arg, optExt))
                    {
                        extension = GetOpt(arg);
                    }
                    else if (IsOpt(arg, optContains))
                    {
                        contains = GetOpt(arg);
                    }
                    else if (IsHelp(arg))
                    {
                        showHelp = true;
                    }
                }

                if (showHelp)
                {
                    Help();
                    return;
                }

                RFind(path, extension.ToLower(), searchTerm.ToLower(), contains.ToLower());
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"RFind Error: {ex.Message}");
            }
        }
    }
}
