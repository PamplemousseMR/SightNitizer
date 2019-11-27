using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SightProperties
{
    class Xml
    {
        /// <summary>
        /// Get default bundle like serviceReg
        /// </summary>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<string, string>> getDefaultBundles(string _rep)
        {
            List<string> xmlFiles = getXMLFiles(_rep);

            List<Tuple<string, string>> bundles = new List<Tuple<string, string>>();
            foreach (string file in xmlFiles)
            {
                string text = File.ReadAllText(file);
                if (text.Contains("::fwServices::"))
                {
                    bundles.Add(new Tuple<string, string>("servicesReg", file));
                }
                if (text.Contains("::fwActivities::"))
                {
                    bundles.Add(new Tuple<string, string>("activities", file));
                }
            }

            return bundles;
        }

        /// <summary>
        /// Get bundles related to requirement
        /// </summary>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<string, string>> getRequireBundles(string _rep)
        {
            List<string> xmlFiles = getXMLFiles(_rep);

            List<Tuple<string, string>> bundles = new List<Tuple<string, string>>();
            foreach (string file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                List<string> requirements = getRequirement(doc);

                foreach (string s in requirements)
                {
                    bundles.Add(new Tuple<string, string>(s, file));
                }
            }

            return bundles;
        }

        /// <summary>
        /// Get bundles related to object like dataReg or arDataReg
        /// </summary>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<string, string>> getObjectsBundles(string _rep)
        {
            List<string> xmlFiles = getXMLFiles(_rep);

            List<Tuple<string, string>> bundles = new List<Tuple<string, string>>();
            foreach (string file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                List<string> objects = getObjects(doc);
                foreach (string s in objects)
                {
                    if (s.StartsWith("::fwData::") || s.StartsWith("::fwMedData::"))
                    {
                        bundles.Add(new Tuple<string, string>("dataReg", file));
                    }
                    else if (s.StartsWith("::arData::"))
                    {
                        bundles.Add(new Tuple<string, string>("arDataReg", file));
                    }
                    else if (s.StartsWith("::rdDataBiopsy::"))
                    {
                        bundles.Add(new Tuple<string, string>("rdDataBiopsyReg", file));
                    }
                    else if (s.StartsWith("::brData::"))
                    {
                        bundles.Add(new Tuple<string, string>("brDataReg", file));
                    }
                }
            }

            return bundles;
        }

        /// <summary>
        /// Get bundles related to qt
        /// </summary>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<string, string>> getQtBundles(string _rep)
        {
            List<string> xmlFiles = getXMLFiles(_rep);

            List<Tuple<string, string>> bundles = new List<Tuple<string, string>>();
            foreach (string file in xmlFiles)
            {
                string text = File.ReadAllText(file);
                if (text.Contains("::fwRenderQt::SRender"))
                {
                    bundles.Add(new Tuple<string, string>("scene2D", file));
                }
            }

            return bundles;
        }

        /// <summary>
        /// Get bundles related to ogre
        /// </summary>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<string, string>> getOgreBundles(string _rep)
        {
            List<string> xmlFiles = getXMLFiles(_rep);

            List<Tuple<string, string>> bundles = new List<Tuple<string, string>>();
            foreach (string file in xmlFiles)
            {
                string text = File.ReadAllText(file);
                if (text.Contains("::fwRenderOgre::SRender"))
                {
                    bundles.Add(new Tuple<string, string>("visuOgreQt", file));
                    bundles.Add(new Tuple<string, string>("visuOgre", file));
                    bundles.Add(new Tuple<string, string>("material", file));
                }
                if (text.Contains("::visuOgreExAdaptor::SIDVRRender"))
                {
                    bundles.Add(new Tuple<string, string>("materialEx", file));
                }
            }

            return bundles;
        }

        /// <summary>
        /// Get bundles related to VTK
        /// </summary>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<string, string>> getVTKBundles(string _rep)
        {
            List<string> xmlFiles = getXMLFiles(_rep);

            List<Tuple<string, string>> bundles = new List<Tuple<string, string>>();
            foreach (string file in xmlFiles)
            {
                string text = File.ReadAllText(file);
                if (text.Contains("::fwRenderVTK::SRender"))
                {
                    bundles.Add(new Tuple<string, string>("visuVTKQt", file));
                    bundles.Add(new Tuple<string, string>("visuVTK", file));
                }
            }

            return bundles;
        }

        /// <summary>
        /// Get standard bundles from each xml files in a directory
        /// </summary>
        /// <param name="_rep">The directory where find bundles in xml files</param>
        /// <returns></returns>
        public static List<Tuple<string, string>> getStandardBundles(string _rep)
        {
            List<string> xmlFiles = getXMLFiles(_rep);

            List<Tuple<string, string>> bundles = new List<Tuple<string, string>>();
            foreach (string file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                List<string> services = getServicesType(doc);

                foreach (string s in services)
                {
                    bundles.Add(new Tuple<string, string>(extractBundle(s), file));
                }
            }

            return bundles;
        }

        /// <summary>
        /// Get bundles related to media
        /// </summary>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<string, string>> getMediaBundles(string _rep)
        {
            List<string> xmlFiles = getXMLFiles(_rep);

            List<Tuple<string, string>> bundles = new List<Tuple<string, string>>();
            foreach (string file in xmlFiles)
            {
                string text = File.ReadAllText(file);
                if (text.Contains("icon=\"media") || text.Contains("<icon>media"))
                {
                    bundles.Add(new Tuple<string, string>("media", file));
                }
                if (text.Contains("icon=\"arMedia") || text.Contains("<icon>arMedia"))
                {
                    bundles.Add(new Tuple<string, string>("arMedia", file));
                }
                if (text.Contains("icon=\"rdMedia") || text.Contains("<icon>rdMedia"))
                {
                    bundles.Add(new Tuple<string, string>("rdMedia", file));
                }
                if (text.Contains("icon=\"perfusionMedia") || text.Contains("<icon>perfusionMedia"))
                {
                    bundles.Add(new Tuple<string, string>("perfusionMedia", file));
                }
                if (text.Contains("icon=\"EUSResources") || text.Contains("<icon>EUSResources"))
                {
                    bundles.Add(new Tuple<string, string>("EUSResources", file));
                }
                if (text.Contains("icon=\"surgeomicsMedia") || text.Contains("<icon>surgeomicsMedia"))
                {
                    bundles.Add(new Tuple<string, string>("surgeomicsMedia", file));
                }
            }

            return bundles;
        }

        /// <summary>
        /// Get bundles related to extensions
        /// </summary>
        /// <param name="_extensionBundles">Bundle name with extensions list</param>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<string, string>> getExtensionBundles(string _rep, List<Tuple<string, List<string>>> _extensionBundles)
        {
            List<string> xmlFiles = getXMLFiles(_rep);
            List<Tuple<string, string>> bundles = new List<Tuple<string, string>>();
            foreach (string file in xmlFiles)
            {
                string text = File.ReadAllText(file);
                foreach (Tuple<string, List<string>> extensionBundle in _extensionBundles)
                {
                    foreach (string extension in extensionBundle.Item2)
                    {
                        if (checkId(text, extension))
                        {
                            bundles.Add(new Tuple<string, string>(extensionBundle.Item1, file));
                        }
                    }
                }
            }

            return bundles;
        }

        /// <summary>
        /// Get all services uid from an xml file
        /// </summary>
        /// <param name="_doc">The xml file</param>
        /// <returns>The list of all services uid</returns>
        public static List<string> getServicesUid(XmlDocument _doc)
        {
            XmlNodeList serviceNodes = _doc.DocumentElement.GetElementsByTagName("service");
            List<string> servicesUid = new List<string>();
            foreach (XmlNode serviceAtt in serviceNodes)
            {
                if (serviceAtt.Attributes["uid"] != null)
                {
                    servicesUid.Add(serviceAtt.Attributes["uid"].InnerText);
                }
            }
            return servicesUid;
        }

        /// <summary>
        /// Get all object uid from an xml file
        /// </summary>
        /// <param name="_doc">The xml file</param>
        /// <returns>The list of all object uid</returns>
        public static List<string> getObjectUid(XmlDocument _doc)
        {
            XmlNodeList serviceNodes = _doc.DocumentElement.GetElementsByTagName("object");
            List<string> servicesUid = new List<string>();
            foreach (XmlNode serviceAtt in serviceNodes)
            {
                if (serviceAtt.Attributes["uid"] != null)
                {
                    servicesUid.Add(serviceAtt.Attributes["uid"].InnerText);
                }
            }
            return servicesUid;
        }

        /*
        * Commun utils functions
        */

        /// <summary>
        /// Get all xml file names in a directory
        /// </summary>
        /// <param name="_rep">The directory</param>
        /// <returns>The list af all file names</returns>
        public static List<string> getXMLFiles(string _rep)
        {
            List<string> xmlFiles = new List<string>();
            foreach (string dir in Directory.GetDirectories(_rep))
            {
                xmlFiles.AddRange(getXMLFiles(dir));
            }
            xmlFiles.AddRange(Directory.GetFiles(_rep, "*.xml"));
            return xmlFiles;
        }

        /// <summary>
        /// Get all services type from an xml file
        /// </summary>
        /// <param name="_doc">The xml file</param>
        /// <returns>The list of all services type</returns>
        private static List<string> getServicesType(XmlDocument _doc)
        {
            List<string> services = new List<string>();
            XmlNodeList serviceNodes = _doc.DocumentElement.GetElementsByTagName("service");
            foreach (XmlNode serviceAtt in serviceNodes)
            {
                if (serviceAtt.Attributes["type"] != null)
                {
                    services.Add(serviceAtt.Attributes["type"].InnerText);
                }
            }
            XmlNodeList configNodes = _doc.DocumentElement.GetElementsByTagName("config");
            foreach (XmlNode configAtt in configNodes)
            {
                if (configAtt.Attributes["service"] != null)
                {
                    services.Add(configAtt.Attributes["service"].InnerText);
                }
            }
            XmlNodeList selectionNodes = _doc.DocumentElement.GetElementsByTagName("addSelection");
            foreach (XmlNode selectionAtt in selectionNodes)
            {
                if (selectionAtt.Attributes["service"] != null)
                {
                    services.Add(selectionAtt.Attributes["service"].InnerText);
                }
            }
            return services;
        }

        /// <summary>
        /// Extract the bundle name from a service name
        /// </summary>
        /// <param name="_service">The name of the service</param>
        /// <returns>The name of the bundle</returns>
        private static string extractBundle(string _service)
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
        private static List<string> getObjects(XmlDocument _doc)
        {
            List<string> objects = new List<string>();
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
        private static List<string> getRequirement(XmlDocument _doc)
        {
            List<string> requirements = new List<string>();
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
        /// Check if a serviceConfig, appConfig or activity is used
        /// </summary>
        /// <param name="_text">The file as text</param>
        /// <param name="_keyword">The keyword to find</param>
        /// <returns>True if the keyword is founded</returns>
        private static bool checkId(string _text, string _keyword)
        {
            return (_text.Contains("id=\"" + _keyword + "\"") ||
                _text.Contains("<id>" + _keyword + "</id>") ||
                _text.Contains("config=\"" + _keyword + "\"") ||
                _text.Contains("<ioSelectorConfig>" + _keyword + "</ioSelectorConfig>") ||
                _text.Contains("<activity>" + _keyword + "</activity>") ||
                _text.Contains("<sdbIoSelectorConfig>" + _keyword + "</sdbIoSelectorConfig>"));
        }
    }
}
