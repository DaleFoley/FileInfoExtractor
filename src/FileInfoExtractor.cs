using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

namespace FileInfoExtractor
{
    class FileInfoExtractor
    {
        private static SortedDictionary<double, string> _directoryOrderDictionary;
        private static List<string> _directoryListing;

        private static void Main(string[] args)
        {
            string sourceTargetDirectory;
            if (args.Length > 1)
            {
                sourceTargetDirectory = ParseArgument(args);
            }
            else
            {
                sourceTargetDirectory = args.First();
            }

            _directoryListing = new List<string>();
            _directoryOrderDictionary = new SortedDictionary<double, string>();

            _directoryListing = Directory.GetDirectories(sourceTargetDirectory, "*", SearchOption.AllDirectories).ToList();

            foreach (string directory in _directoryListing)
            {
                string fileName =
                    Directory.EnumerateFiles(directory, "*.*", SearchOption.TopDirectoryOnly)
                        .FirstOrDefault(s => s.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) ||
                                    s.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) ||
                                    s.EndsWith(".wav", StringComparison.OrdinalIgnoreCase) ||
                                    s.EndsWith(".m4a", StringComparison.OrdinalIgnoreCase) ||
                                    s.EndsWith(".m3u", StringComparison.OrdinalIgnoreCase) ||
                                    s.EndsWith(".wma", StringComparison.OrdinalIgnoreCase));

                if (fileName == null) continue;

                fileName = ReplaceForwardSlashes(fileName);

                string fileSourceDirectory = Path.GetDirectoryName(fileName);

                var mediaYear = ShellObject.FromParsingName(fileName)
                    .Properties.GetProperty(SystemProperties.System.Media.Year);

                double fileYear;
                if (mediaYear.ValueAsObject != null)
                {                    
                    fileYear = Int32.Parse(ShellObject.FromParsingName(fileName)
                   .Properties.GetProperty(SystemProperties.System.Media.Year)
                   .ValueAsObject.ToString());
                }
                else
                {
                    //Couldn't get a Year from file properties, so we'll append to the end of
                    //the collection.
                    fileYear = _directoryOrderDictionary.Keys.Last() + .01;
                }

                while (_directoryOrderDictionary.ContainsKey(fileYear))
                {
                    fileYear += .01;
                }

                _directoryOrderDictionary.Add(fileYear, fileSourceDirectory);
            }

            foreach (KeyValuePair<double, string> keyValuePair in _directoryOrderDictionary)
            {
                Console.WriteLine(keyValuePair);
            }
        }

        private static string ParseArgument(string[] args)
        {
            string resolvedPath = @"";
            foreach (string argument in args)
            {
                resolvedPath += argument + " ";
            }

            return resolvedPath;
        }

        private static string ReplaceForwardSlashes(string stringToModify)
        {
            string newString = "";
            newString = stringToModify.Replace("/", "\\");

            return newString;
        }
    }
}
