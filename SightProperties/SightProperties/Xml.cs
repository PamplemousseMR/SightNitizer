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
                if (text.Contains("::fwActivities::"))
                {
                    bundles.Add(new Tuple<String, String>("activities", file));
                }
            }

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
                    else if (s.StartsWith("::rdDataBiopsy::"))
                    {
                        bundles.Add(new Tuple<String, String>("rdDataBiopsyReg", file));
                    }
                    else if (s.StartsWith("::brData::"))
                    {
                        bundles.Add(new Tuple<String, String>("brDataReg", file));
                    }
                }
            }

            return bundles;
        }

        /// <summary>
        /// Get bundles related to qt
        /// </summary>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<String, String>> getQtBundles(String _rep)
        {
            List<String> xmlFiles = getXMLFiles(_rep);

            List<Tuple<String, String>> bundles = new List<Tuple<String, String>>();
            foreach (String file in xmlFiles)
            {
                String text = File.ReadAllText(file);
                if (text.Contains("::fwRenderQt::SRender"))
                {
                    bundles.Add(new Tuple<String, String>("scene2D", file));
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
                if (text.Contains("::visuOgreExAdaptor::SIDVRRender"))
                {
                    bundles.Add(new Tuple<String, String>("materialEx", file));
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

                List<String> services = getServicesType(doc);

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
                if (text.Contains("icon=\"resources") || text.Contains("<icon>resources"))
                {
                    bundles.Add(new Tuple<String, String>("resources", file));
                }
            }

            return bundles;
        }

        /// <summary>
        /// Get bundles related to extensions
        /// </summary>
        /// <param name="_extensionBundles">Bundle name with extensions list</param>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<String, String>> getExtensionBundles(String _rep, List<Tuple<String, List<String>>> _extensionBundles)
        {
            List<String> xmlFiles = getXMLFiles(_rep);
            List<Tuple<String, String>> bundles = new List<Tuple<String, String>>();
            foreach (String file in xmlFiles)
            {
                String text = File.ReadAllText(file);
                foreach(Tuple<String, List<String>> extensionBundle in _extensionBundles)
                {
                    foreach (String extension in extensionBundle.Item2)
                    {
                        if (checkId(text, extension))
                        {
                            bundles.Add(new Tuple<String, String>(extensionBundle.Item1, file));
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
        public static List<String> getServicesUid(XmlDocument _doc)
        {
            XmlNodeList serviceNodes = _doc.DocumentElement.GetElementsByTagName("service");
            List<String> servicesUid = new List<String>();
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
        public static List<String> getObjectUid(XmlDocument _doc)
        {
            XmlNodeList serviceNodes = _doc.DocumentElement.GetElementsByTagName("object");
            List<String> servicesUid = new List<String>();
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
        public static List<String> getXMLFiles(String _rep)
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
        /// Get all services type from an xml file
        /// </summary>
        /// <param name="_doc">The xml file</param>
        /// <returns>The list of all services type</returns>
        private static List<String> getServicesType(XmlDocument _doc)
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
        private static String extractBundle(String _service)
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
        private static List<String> getObjects(XmlDocument _doc)
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
        private static List<String> getRequirement(XmlDocument _doc)
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
        /// Check if a serviceConfig, appConfig or activity is used
        /// </summary>
        /// <param name="_text">The file as text</param>
        /// <param name="_keyword">The keyword to find</param>
        /// <returns>True if the keyword is founded</returns>
        private static bool checkId(String _text, String _keyword)
        {
            return (_text.Contains("id=\"" + _keyword + "\"") || 
                _text.Contains("<id>" + _keyword + "</id>") ||
                _text.Contains("config=\"" + _keyword + "\"") ||
                _text.Contains("<ioSelectorConfig>" + _keyword + "</ioSelectorConfig>") ||
                _text.Contains("<sdbIoSelectorConfig>" + _keyword + "</sdbIoSelectorConfig>"));
        }
    }
}
