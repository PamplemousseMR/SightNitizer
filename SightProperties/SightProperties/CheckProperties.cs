using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SightProperties
{
    class CheckProperties
    {
        /// <summary>
        /// Check that bundles are in the file
        /// </summary>
        /// <param name="_file">The properties file</param>
        /// <param name="_bundles">The bundle list</param>
        /// <returns>True if all bundle was found</returns>
        public static bool checkProperties(String _file, List<Tuple<String, String>> _bundles)
        {
            List<String> propertiesBundles = getBundles(_file);
            List<String> parsedProperties = new List<string>();

            bool isValid = true;
            foreach(Tuple<String, String> bundle in _bundles)
            {
                /// Check this special bundle
                if (bundle.Item1.CompareTo("fwServices") != 0)
                {
                    bool find = false;
                    foreach (String propertiesBundle in propertiesBundles)
                    {
                        if (propertiesBundle.CompareTo(bundle.Item1) == 0)
                        {
                            find = true;
                            parsedProperties.Add(propertiesBundle);
                            break;
                        }
                    }
                    if (!find)
                    {
                        Console.WriteLine("The bundle: `" + bundle.Item1 + "` from: `" + bundle.Item2 + "` was not found in the file: `" + _file + "`");
                        isValid = false;
                    }
                }
            }

            foreach (String propertiesBundle in propertiesBundles)
            {
                if(!parsedProperties.Remove(propertiesBundle))
                {
                    /// Check special bundles
                    if (propertiesBundle.CompareTo("appXml") != 0
                        && propertiesBundle.CompareTo("fwlauncher") != 0
                        && propertiesBundle.CompareTo("ogreConfig") != 0
                        && propertiesBundle.CompareTo("configOgreEx") != 0
                        && propertiesBundle.CompareTo("ioVtkGdcm") != 0) 
                    {
                        isValid = false;
                        Console.WriteLine("The bundle: `" + propertiesBundle + "` is not used");
                    }
                }
            }

            return isValid;
        }

        /// <summary>
        /// Get the bundles list from a properties file
        /// </summary>
        /// <param name="_file">The properties file</param>
        /// <returns>The list of all bundles</returns>
        public static List<String> getBundles(String _file)
        {
            String[] lines = System.IO.File.ReadAllLines(_file);
            List<String> cleanLines = new List<string>();

            bool add = false;
            foreach (String line in lines)
            {
                int comment = line.IndexOf("#");
                comment = comment < 0 ? line.Length : comment;
                String noComment = line.Substring(0, comment);

                if (noComment.Contains("REQUIREMENTS") || noComment.Contains("DEPENDENCIES"))
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
