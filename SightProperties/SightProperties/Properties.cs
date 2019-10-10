using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SightProperties
{
    class Properties
    {
        /// <summary>
        /// Get the bundles list from a properties file
        /// </summary>
        /// <param name="_file">The properties file</param>
        /// <returns>The list of all bundles</returns>
        public static List<String> getRequirements(String _file)
        {
            String[] lines = System.IO.File.ReadAllLines(_file);
            List<String> cleanLines = new List<string>();

            bool add = false;
            foreach (String line in lines)
            {
                int comment = line.IndexOf("#");
                comment = comment < 0 ? line.Length : comment;
                String noComment = line.Substring(0, comment);

                if (noComment.Contains("REQUIREMENTS"))
                {
                    if (!noComment.Contains(")"))
                    {

                        add = true;
                        continue;
                    }
                }
                else if (add && noComment.Contains(")"))
                {
                    add = false;
                }

                if (add)
                {
                    String cleanLine = Regex.Replace(noComment, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
                    if (cleanLine.Length > 0)
                    {
                        cleanLines.Add(cleanLine);
                    }
                }
            }
            return cleanLines;
        }

        /// <summary>
        /// Get the library list from a properties file
        /// </summary>
        /// <param name="_file">The properties file</param>
        /// <returns>The list of all bundles</returns>
        public static List<String> getDependencies(String _file)
        {
            String[] lines = System.IO.File.ReadAllLines(_file);
            List<String> cleanLines = new List<string>();

            bool add = false;
            foreach (String line in lines)
            {
                int comment = line.IndexOf("#");
                comment = comment < 0 ? line.Length : comment;
                String noComment = line.Substring(0, comment);

                if (noComment.Contains("DEPENDENCIES"))
                {
                    if (!noComment.Contains(")"))
                    {

                        add = true;
                        continue;
                    }
                }
                else if (add && noComment.Contains(")"))
                {
                    add = false;
                }

                if (add)
                {
                    String cleanLine = Regex.Replace(noComment, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
                    if (cleanLine.Length > 0)
                    {
                        cleanLines.Add(cleanLine);
                    }
                }
            }
            return cleanLines;
        }
    }
}
