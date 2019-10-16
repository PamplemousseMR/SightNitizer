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
        public static List<string> getRequirements(string _file)
        {
            string[] lines = System.IO.File.ReadAllLines(_file);
            List<string> cleanLines = new List<string>();

            bool add = false;
            foreach (string line in lines)
            {
                int comment = line.IndexOf("#");
                comment = comment < 0 ? line.Length : comment;
                string noComment = line.Substring(0, comment);

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
                    string cleanLine = Regex.Replace(noComment, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
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
        public static List<string> getDependencies(string _file)
        {
            string[] lines = System.IO.File.ReadAllLines(_file);
            List<string> cleanLines = new List<string>();

            bool add = false;
            foreach (string line in lines)
            {
                int comment = line.IndexOf("#");
                comment = comment < 0 ? line.Length : comment;
                string noComment = line.Substring(0, comment);

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
                    string cleanLine = Regex.Replace(noComment, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
                    if (cleanLine.Length > 0)
                    {
                        cleanLines.Add(cleanLine);
                    }
                }
            }
            return cleanLines;
        }

        /// <summary>
        /// Get the type of a properties file
        /// </summary>
        /// <param name="_file">The properties file</param>
        /// <returns>The type of the directory</returns>
        public static string getType(string _file)
        {
            string[] lines = System.IO.File.ReadAllLines(_file);
            string type = "Unknow";

            foreach (string line in lines)
            {
                int comment = line.IndexOf("#");
                comment = comment < 0 ? line.Length : comment;
                string noComment = line.Substring(0, comment);

                if (noComment.Contains("TYPE"))
                {
                    string cleanLine = Regex.Replace(noComment, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
                    type = cleanLine.Replace("setTYPE", "");
                }

            }
            return type;
        }

        /// <summary>
        /// Get the name of a properties file
        /// </summary>
        /// <param name="_file">The properties file</param>
        /// <returns>The name of the directory</returns>
        public static string getName(string _file)
        {
            string[] lines = System.IO.File.ReadAllLines(_file);
            string type = "Unknow";

            foreach (string line in lines)
            {
                int comment = line.IndexOf("#");
                comment = comment < 0 ? line.Length : comment;
                string noComment = line.Substring(0, comment);

                if (noComment.Contains("NAME"))
                {
                    string cleanLine = Regex.Replace(noComment, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
                    type = cleanLine.Replace("setNAME", "");
                }

            }
            return type;
        }
    }
}
