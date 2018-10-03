using System;
using System.IO;

namespace RecursiveFind
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                RFind(new RFindOptions(args));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"RFind Error: {ex.Message}");
            }
        }

        static void RFind(RFindOptions opts)
        {
            if (opts.ShowHelp)
            {
                Help();
                return;
            }

            var parseFile = !string.IsNullOrEmpty(opts.Contains);
            var searchFiles = !string.IsNullOrEmpty(opts.SearchTerm);

            if (!searchFiles && !parseFile)
            {
                throw new Exception("No \"--search\" or \"--contains\" argument was given. Please use the \"--help\" argument.");
            }

            if (string.IsNullOrEmpty(opts.Path) || !Directory.Exists(opts.Path))
            {
                throw new Exception("\"path\" is invalid or was not supplied.");
            }
            
            foreach (var file in new DirectoryInfo(opts.Path).GetFiles(opts.Extension, SearchOption.AllDirectories))
            {
                Console.Write($"\rSearching {file.FullName}");
                if (!searchFiles || file.FullName.ToLower().Contains(opts.SearchTerm))
                {
                    if (!parseFile || ParseFile(file.FullName, opts.Contains))
                    {
                        ClearLine();
                        Console.WriteLine(file.FullName);
                    }
                }
                ClearLine();
            }
        }

        static bool ParseFile(string path, string contains)
        {
            var searchTerms = contains.Split('|');
            var countFound = 0;

            try
            {
                var content = File.ReadAllText(path);
                content = content.Replace("\t", " ").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Replace("  ", "").ToLower();

                foreach (var searchTerm in searchTerms)
                {
                    if (content.Contains(searchTerm))
                    {
                        countFound++;
                    }
                }
            }
            catch { }

            return countFound == searchTerms.Length;
        }

        static void ClearLine()
        {
            Console.Write("\r" + new string(' ', Console.WindowWidth - 1) + "\r");
        }

        static void Help()
        {
            Console.WriteLine(
                "rfind by scottyeatscode\r\n\r\n" +
                "rfind [--<path|extension|search|contains>=<values>]|[--help]"
            );
        }
    }
}
