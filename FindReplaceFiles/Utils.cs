using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace FindReplaceFiles
{
    public class Utils
    {
        public static string[] GetFilesInDirectory(string dir, string fileMask, bool includeSubDirectories, string excludeMask)
        {
            SearchOption searchOption = includeSubDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            var filesInDirectory = new List<string>();
            var fileMasks = fileMask.Split(',');
            foreach (var mask in fileMasks)
            {
                filesInDirectory.AddRange(Directory.GetFiles(dir, mask.Trim(), searchOption));
            }

            filesInDirectory = filesInDirectory.Distinct().ToList();

            if (!String.IsNullOrEmpty(excludeMask))
            {
                var tempFilesInDirectory = new List<string>();
                List<string> excludeFileMasks = excludeMask.Split(',').ToList();
                excludeFileMasks = excludeFileMasks.Select(fm => WildcardToRegex(fm.Trim())).ToList();


                foreach (var excludeFileMaskRegExPattern in excludeFileMasks)
                {
                    foreach (string filePath in filesInDirectory)
                    {
                        string fileName = Path.GetFileName(filePath);
                        if (fileName == null) //Somehow it can be null. So add a check
                            continue;

                        if (!Regex.IsMatch(fileName, excludeFileMaskRegExPattern))
                            tempFilesInDirectory.Add(filePath);
                    }

                    filesInDirectory = tempFilesInDirectory;
                    tempFilesInDirectory = new List<string>();
                }
            }

            filesInDirectory.Sort();
            return filesInDirectory.ToArray();
        }

        internal static string WildcardToRegex(string pattern)
        {
            return string.Format("^{0}$", Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", "."));
        }
    }
}
