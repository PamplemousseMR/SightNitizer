using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SightProperties
{
    class Language
    {

        /// <summary>
        /// Get bundles related to cpp/hpp/c/h files
        /// </summary>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<String, String>> getIncludeBundles(String _rep)
        {
            List<String> languageFiles = getLanguageFiles(_rep);

            List<Tuple<String, String>> bundles = new List<Tuple<String, String>>();
            foreach (String file in languageFiles)
            {
                List<String> requirements = getIncludes(file);

                foreach (String s in requirements)
                {
                    bundles.Add(new Tuple<String, String>(s, file));
                }
            }

            return bundles;
        }

        /*
        * Commun utils fnctions
        */

        /// <summary>
        /// Get all hpp/cpp/h/c file names in a directory
        /// </summary>
        /// <param name="_rep">The directory</param>
        /// <returns>The list af all file names</returns>
        public static List<String> getLanguageFiles(String _rep)
        {
            List<String> languageFiles = new List<string>();
            foreach (String dir in Directory.GetDirectories(_rep))
            {
                languageFiles.AddRange(getLanguageFiles(dir));
            }
            languageFiles.AddRange(Directory.GetFiles(_rep, "*.hpp"));
            languageFiles.AddRange(Directory.GetFiles(_rep, "*.cpp"));
            languageFiles.AddRange(Directory.GetFiles(_rep, "*.c"));
            languageFiles.AddRange(Directory.GetFiles(_rep, "*.h"));
            return languageFiles;
        }

        /// <summary>
        /// Get all bundles from #include
        /// </summary>
        /// <param name="_file">The language file</param>
        /// <returns>The list of all bundles</returns>
        private static List<String> getIncludes(String _file)
        {
            List<String> bundles = new List<String>();

            String text = File.ReadAllText(_file);
            Regex regex = new Regex(@"#include +<[^/\.>]*/", RegexOptions.Compiled);
            Regex regexBundle = new Regex(@"<.*", RegexOptions.Compiled);
            foreach (Match include in regex.Matches(text))
            {
                String bundle = regexBundle.Match(include.ToString()).ToString().Replace("<", string.Empty).Replace("/", string.Empty);
                bundles.Add(bundle);
            }

            return bundles;
        }
    }
}
