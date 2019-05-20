using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace SightProperties
{
    class Bundle
    {
        /// <summary>
        /// Get default bundle like serviceReg
        /// </summary>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<String, String>> getDefaultBundles(String _rep)
        {
            List<String> xmlFiles = getXMLFiles(_rep);

            List<Tuple<String, String>> bundles = new List<Tuple<String, String>>();
            foreach (String file in xmlFiles)
            {
                String text = File.ReadAllText(file);
                if (text.Contains("::fwServices::"))
                {
                    bundles.Add(new Tuple<String, String>("servicesReg", file));
                }
            }

            /*bundles.Add(new Tuple<String, String>("fwlauncher", "require bundle"));
            bundles.Add(new Tuple<String, String>("appXml", "require bundle"));*/

            return bundles;
        }

        /// <summary>
        /// Get bundles related to requirement
        /// </summary>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<String, String>> getRequireBundles(String _rep)
        {
            List<String> xmlFiles = getXMLFiles(_rep);

            List<Tuple<String, String>> bundles = new List<Tuple<String, String>>();
            foreach (String file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                List<String> requirements = getRequirement(doc);

                foreach (String s in requirements)
                {
                    bundles.Add(new Tuple<String, String>(s, file));
                }
            }

            return bundles;
        }

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

        /// <summary>
        /// Get bundles related to object like dataReg or arDataReg
        /// </summary>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<String, String>> getObjectsBundles(String _rep)
        {
            List<String> xmlFiles = getXMLFiles(_rep);

            List<Tuple<String, String>> bundles = new List<Tuple<String, String>>();
            foreach (String file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                List<String> objects = getObjects(doc);
                foreach (String s in objects)
                {
                    if (s.StartsWith("::fwData::") || s.StartsWith("::fwMedData::"))
                    {
                        bundles.Add(new Tuple<String, String>("dataReg", file));
                    }
                    else if (s.StartsWith("::arData::"))
                    {
                        bundles.Add(new Tuple<String, String>("arDataReg", file));
                    }
                }
            }

            return bundles;
        }

        /// <summary>
        /// Get bundles related to ogre
        /// </summary>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<String, String>> getOgreBundles(String _rep)
        {
            List<String> xmlFiles = getXMLFiles(_rep);

            List<Tuple<String, String>> bundles = new List<Tuple<String, String>>();
            foreach (String file in xmlFiles)
            {
                String text = File.ReadAllText(file);
                if (text.Contains("::fwRenderOgre::SRender"))
                {
                    bundles.Add(new Tuple<String, String>("visuOgreQt", file));
                    bundles.Add(new Tuple<String, String>("visuOgre", file));
                    bundles.Add(new Tuple<String, String>("material", file));
                }
            }

            return bundles;
        }

        /// <summary>
        /// Get bundles related to VTK
        /// </summary>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<String, String>> getVTKBundles(String _rep)
        {
            List<String> xmlFiles = getXMLFiles(_rep);

            List<Tuple<String, String>> bundles = new List<Tuple<String, String>>();
            foreach (String file in xmlFiles)
            {
                String text = File.ReadAllText(file);
                if (text.Contains("::fwRenderVTK::SRender"))
                {
                    bundles.Add(new Tuple<String, String>("visuVTKQt", file));
                    bundles.Add(new Tuple<String, String>("visuVTK", file));
                }
            }

            return bundles;
        }

        /// <summary>
        /// Get standard bundles from each xml files in a directory
        /// </summary>
        /// <param name="_rep">The directory where find bundles in xml files</param>
        /// <returns></returns>
        public static List<Tuple<String, String>> getStandardBundles(String _rep)
        {
            List<String> xmlFiles = getXMLFiles(_rep);

            List<Tuple<String, String>> bundles = new List<Tuple<String, String>>();
            foreach (String file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                List<String> services = getServices(doc);

                foreach (String s in services)
                {
                    bundles.Add(new Tuple<String, String>(extractBundle(s), file));
                }
            }

            return bundles;
        }

        /// <summary>
        /// Get bundles related to media
        /// </summary>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<String, String>> getMediaBundles(String _rep)
        {
            List<String> xmlFiles = getXMLFiles(_rep);

            List<Tuple<String, String>> bundles = new List<Tuple<String, String>>();
            foreach (String file in xmlFiles)
            {
                String text = File.ReadAllText(file);
                if (text.Contains("icon=\"media") || text.Contains("<icon>media"))
                {
                    bundles.Add(new Tuple<String, String>("media", file));
                } 
                if (text.Contains("icon=\"arMedia") || text.Contains("<icon>arMedia"))
                {
                    bundles.Add(new Tuple<String, String>("arMedia", file));
                }
                if (text.Contains("icon=\"rdMedia") || text.Contains("<icon>rdMedia"))
                {
                    bundles.Add(new Tuple<String, String>("rdMedia", file));
                }
                if (text.Contains("icon=\"perfusionMedia") || text.Contains("<icon>perfusionMedia"))
                {
                    bundles.Add(new Tuple<String, String>("perfusionMedia", file));
                }
            }

            return bundles;
        }

        /*
         * Commun
         */

        /// <summary>
        /// Get all xml file names in a directory
        /// </summary>
        /// <param name="_rep">The directory</param>
        /// <returns>The list af all file names</returns>
        static List<String> getXMLFiles(String _rep)
        {
            List<String> xmlFiles = new List<string>();
            foreach (String dir in Directory.GetDirectories(_rep))
            {
                xmlFiles.AddRange(getXMLFiles(dir));
            }
            xmlFiles.AddRange(Directory.GetFiles(_rep, "*.xml"));
            return xmlFiles;
        }

        /// <summary>
        /// Get all hpp/cpp/h/c file names in a directory
        /// </summary>
        /// <param name="_rep">The directory</param>
        /// <returns>The list af all file names</returns>
        static List<String> getLanguageFiles(String _rep)
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
        /// Get all services from an xml file
        /// </summary>
        /// <param name="_doc">The xml file</param>
        /// <returns>The list of all services</returns>
        static List<String> getServices(XmlDocument _doc)
        {
            List<String> services = new List<String>();
            XmlNodeList serviceNodes = _doc.DocumentElement.GetElementsByTagName("service");
            foreach (XmlNode serviceAtt in serviceNodes)
            {
                if (serviceAtt.Attributes["type"] != null)
                {
                    services.Add(serviceAtt.Attributes["type"].InnerText);
                }
            }
            return services;
        }

        /// <summary>
        /// Extract the bundle name from a service name
        /// </summary>
        /// <param name="_service">The name of the service</param>
        /// <returns>The name of the bundle</returns>
        static String extractBundle(String _service)
        {
            int end = _service.Substring(2).IndexOf(":");
            return _service.Substring(2, end);
        }

        /*
         * Get object from xml file
         */

        /// <summary>
        /// Get all object from an xml file
        /// </summary>
        /// <param name="_doc">The xml file</param>
        /// <returns>The list of all object</returns>
        static List<String> getObjects(XmlDocument _doc)
        {
            List<String> objects = new List<String>();
            XmlNodeList objectNodes = _doc.DocumentElement.GetElementsByTagName("object");
            foreach (XmlNode objectAtt in objectNodes)
            {
                if (objectAtt.Attributes["type"] != null)
                {
                    objects.Add(objectAtt.Attributes["type"].InnerText);
                }
            }
            return objects;
        }

        /// <summary>
        /// Get all requirement from an xml file
        /// </summary>
        /// <param name="_doc">The xml file</param>
        /// <returns>The list of all object</returns>
        static List<String> getRequirement(XmlDocument _doc)
        {
            List<String> requirements = new List<String>();
            XmlNodeList requirementNodes = _doc.DocumentElement.GetElementsByTagName("requirement");
            foreach (XmlNode requirementAtt in requirementNodes)
            {
                if (requirementAtt.Attributes["id"] != null)
                {
                    requirements.Add(requirementAtt.Attributes["id"].InnerText);
                }
            }
            return requirements;
        }

        /// <summary>
        /// Get all bundles from #include
        /// </summary>
        /// <param name="_file">The language file</param>
        /// <returns>The list of all bundles</returns>
        static List<String> getIncludes(String _file)
        {
            List<String> bundles = new List<String>();

            String text = File.ReadAllText(_file);
            Regex regex = new Regex(@"#include +<[^/\.>]*/", RegexOptions.Compiled);
            Regex regexBundle = new Regex(@"<.*", RegexOptions.Compiled);
            foreach (Match include in regex.Matches(text))
            {
                String bundle = regexBundle.Match(include.ToString()).ToString().Replace("<", string.Empty).Replace("/", string.Empty);
                if (bundle.CompareTo("boost") != 0 &&
                    bundle.CompareTo("OGRE") != 0)
                { 
                    bundles.Add(bundle);
                }
            }

            return bundles;
        }
    }
}
