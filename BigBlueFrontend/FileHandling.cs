using System;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace BigBlue
{
    public static class FileHandling
    {
        public static string CleanUpPath(string path)
        {
            // don't allow assets or programs from the windows system directory; too dangerous
            if (path.IndexOf(Environment.SystemDirectory, StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                path = string.Empty;
            }
                        
            return path;
        }

        public static Uri ValidateUri(string xmlPath, string path)
        {
            if (string.IsNullOrWhiteSpace(xmlPath))
            {
                return null;
            }

            Uri mediaUri = null;

            if (xmlPath.Contains(":") == false)
            {
                mediaUri = new Uri(path + @"\" + xmlPath);
            }
            else
            {
                mediaUri = new Uri(xmlPath);
            }

            if (!string.IsNullOrWhiteSpace(mediaUri.LocalPath))
            {
                if (File.Exists(mediaUri.LocalPath))
                {
                    return mediaUri;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public static IEnumerable<string> GetFilesByPath(string searchPattern, string path)
        {
            IEnumerable<string> files = Directory
                .EnumerateFiles(path, "*." + searchPattern, SearchOption.TopDirectoryOnly)
                .ToList();

            return files;
        }

        public static FileInfo[] GetFiles(DirectoryInfo di, FileInfo[] files, string path, string searchPattern)
        {
            string[] delimitedPattern = searchPattern.Split(',');

            files = delimitedPattern
                .SelectMany(i => di.GetFiles(i, SearchOption.TopDirectoryOnly))
                .ToArray();

            return files;
        }
        
        public static IEnumerable<string> GetFiles(string path,
                       string searchPatternExpression = "",
                       SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            Regex reSearchPattern = new Regex(searchPatternExpression, RegexOptions.IgnoreCase);
            return Directory.EnumerateFiles(path, "*", searchOption)
                            .Where(file =>
                                     reSearchPattern.IsMatch(Path.GetExtension(file)));
        }

        // Takes same patterns, and executes in parallel
        public static IEnumerable<string> GetFiles(string path,
                            string[] searchPatterns,
                            SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return searchPatterns.AsParallel()
                   .SelectMany(searchPattern =>
                          Directory.EnumerateFiles(path, searchPattern, searchOption));
        }

        public static XmlNode GetNodeFromFile(string filePath, string xPath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNode node = doc.SelectSingleNode(xPath);
            return node;
        }
    }
}
