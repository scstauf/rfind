using System;
using System.Collections.Generic;

namespace RecursiveFind
{
    struct RFindOptions
    {
        public string Path;
        public string SearchTerm;
        public string Extension;
        public string Contains;
        public bool ShowHelp;

        public RFindOptions(string[] args)
        {
            var optPath = BuildOptFlags("p", "path");
            var optSearch = BuildOptFlags("s", "search");
            var optExt = BuildOptFlags("e", "extension", "ext");
            var optContains = BuildOptFlags("c", "contains");

            Path = Environment.CurrentDirectory;
            SearchTerm = string.Empty;
            Extension = "*.*";
            Contains = string.Empty;
            ShowHelp = false;

            foreach (var arg in args)
            {
                if (IsOpt(arg, optPath))
                {
                    Path = GetOpt(arg).ToLower();
                }
                else if (IsOpt(arg, optSearch))
                {
                    SearchTerm = GetOpt(arg).ToLower();
                }
                else if (IsOpt(arg, optExt))
                {
                    Extension = GetOpt(arg).ToLower();
                }
                else if (IsOpt(arg, optContains))
                {
                    Contains = GetOpt(arg).ToLower();
                }
                else if (IsHelp(arg))
                {
                    ShowHelp = true;
                }
            }
        }

        private string GetOpt(string option)
        {
            var value = string.Empty;

            if (option.IndexOf('=') < 0)
            {
                return value;
            }

            return option.Split('=')[1].Replace("\"", "");
        }

        private bool IsOpt(string test, params string[] options)
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

        private bool IsHelp(string test, params string[] options)
        {
            var help = test;

            if (help.IndexOf('=') < 0)
            {
                help += "=help";
            }

            return IsOpt(help, BuildOptFlags("h", "help", "?"));
        }

        private string StripSymbols(string str)
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
    }
}
