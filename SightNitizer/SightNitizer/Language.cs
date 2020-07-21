using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SightNitizer
{
    class Language
    {

        /// <summary>
        /// Get modules related to cpp/hpp/c/h files
        /// </summary>
        /// <returns>The list of require modules</returns>
        public static List<Tuple<string, string>> getIncludeModules(string _rep)
        {
            List<string> languageFiles = getLanguageFiles(_rep);

            List<Tuple<string, string>> modules = new List<Tuple<string, string>>();
            foreach (string file in languageFiles)
            {
                List<string> requirements = getIncludes(file);

                foreach (string s in requirements)
                {
                    modules.Add(new Tuple<string, string>(s, file));
                }
            }

            return modules;
        }

        /*
        * Commun utils functions
        */

        /// <summary>
        /// Get all hpp/cpp/h/c file names in a directory
        /// </summary>
        /// <param name="_rep">The directory</param>
        /// <returns>The list af all file names</returns>
        public static List<string> getLanguageFiles(string _rep)
        {
            List<string> languageFiles = new List<string>();
            foreach (string dir in Directory.GetDirectories(_rep))
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
        /// Get all modules from #include
        /// </summary>
        /// <param name="_file">The language file</param>
        /// <returns>The list of all modules</returns>
        private static List<string> getIncludes(string _file)
        {
            List<string> modules = new List<string>();

            string text = File.ReadAllText(_file);
            Regex regex = new Regex(@"#include +<[^/\.>]*/", RegexOptions.Compiled);
            Regex regexModule = new Regex(@"<.*", RegexOptions.Compiled);
            foreach (Match include in regex.Matches(text))
            {
                string module = regexModule.Match(include.ToString()).ToString().Replace("<", string.Empty).Replace("/", string.Empty);
                modules.Add(module);
            }

            return modules;
        }
    }
}
