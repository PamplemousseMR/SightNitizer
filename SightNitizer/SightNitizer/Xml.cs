using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SightNitizer
{
    class Xml
    {
        /// <summary>
        /// Get default module like serviceReg
        /// </summary>
        /// <returns>The list of require modules</returns>
        public static List<Tuple<string, string>> getDefaultModules(string _rep)
        {
            List<string> xmlFiles = getXMLFiles(_rep);

            List<Tuple<string, string>> modules = new List<Tuple<string, string>>();
            foreach (string file in xmlFiles)
            {
                string text = File.ReadAllText(file);
                if (text.Contains("::fwServices::"))
                {
                    modules.Add(new Tuple<string, string>("servicesReg", file));
                }
                if (text.Contains("::fwActivities::"))
                {
                    modules.Add(new Tuple<string, string>("activities", file));
                }
            }

            return modules;
        }

        /// <summary>
        /// Get modules related to requirement
        /// </summary>
        /// <returns>The list of require modules</returns>
        public static List<Tuple<string, string>> getRequireModules(string _rep)
        {
            List<string> xmlFiles = getXMLFiles(_rep);

            List<Tuple<string, string>> modules = new List<Tuple<string, string>>();
            foreach (string file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                List<string> requirements = getRequirement(doc);

                foreach (string s in requirements)
                {
                    modules.Add(new Tuple<string, string>(s, file));
                }
            }

            return modules;
        }

        /// <summary>
        /// Get modules related to object
        /// </summary>
        /// <returns>The list of require modules</returns>
        public static List<Tuple<string, string>> getObjectsModules(string _rep)
        {
            List<string> xmlFiles = getXMLFiles(_rep);

            List<Tuple<string, string>> modules = new List<Tuple<string, string>>();
            foreach (string file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                List<string> objects = getObjects(doc);
                foreach (string s in objects)
                {
                    if (s.StartsWith("::fwData::"))
                    {
                        modules.Add(new Tuple<string, string>("fwData", file));
                    }
                    else if (s.StartsWith("::fwMedData::"))
                    {
                        modules.Add(new Tuple<string, string>("fwMedData", file));
                    }
                    else if (s.StartsWith("::arData::"))
                    {
                        modules.Add(new Tuple<string, string>("arData", file));
                    }
                    else if (s.StartsWith("::rdDataBiopsy::"))
                    {
                        modules.Add(new Tuple<string, string>("rdDataBiopsyReg", file));
                    }
                    else if (s.StartsWith("::brData::"))
                    {
                        modules.Add(new Tuple<string, string>("brDataReg", file));
                    }
                }
            }

            return modules;
        }

        /// <summary>
        /// Get modules related to qt
        /// </summary>
        /// <returns>The list of require modules</returns>
        public static List<Tuple<string, string>> getQtModules(string _rep)
        {
            List<string> xmlFiles = getXMLFiles(_rep);

            List<Tuple<string, string>> modules = new List<Tuple<string, string>>();
            foreach (string file in xmlFiles)
            {
                string text = File.ReadAllText(file);
                if (text.Contains("::fwRenderQt::SRender"))
                {
                    modules.Add(new Tuple<string, string>("scene2D", file));
                }
            }

            return modules;
        }

        /// <summary>
        /// Get modules related to ogre
        /// </summary>
        /// <returns>The list of require modules</returns>
        public static List<Tuple<string, string>> getOgreModules(string _rep)
        {
            List<string> xmlFiles = getXMLFiles(_rep);

            List<Tuple<string, string>> modules = new List<Tuple<string, string>>();
            foreach (string file in xmlFiles)
            {
                string text = File.ReadAllText(file);
                if (text.Contains("::fwRenderOgre::SRender"))
                {
                    modules.Add(new Tuple<string, string>("visuOgreQt", file));
                    modules.Add(new Tuple<string, string>("visuOgre", file));
                }
            }

            return modules;
        }

        /// <summary>
        /// Get modules related to VTK
        /// </summary>
        /// <returns>The list of require modules</returns>
        public static List<Tuple<string, string>> getVTKModules(string _rep)
        {
            List<string> xmlFiles = getXMLFiles(_rep);

            List<Tuple<string, string>> modules = new List<Tuple<string, string>>();
            foreach (string file in xmlFiles)
            {
                string text = File.ReadAllText(file);
                if (text.Contains("::fwRenderVTK::SRender"))
                {
                    modules.Add(new Tuple<string, string>("visuVTKQt", file));
                    modules.Add(new Tuple<string, string>("visuVTK", file));
                }
            }

            return modules;
        }

        /// <summary>
        /// Get standard modules from each xml files in a directory
        /// </summary>
        /// <param name="_rep">The directory where find modules in xml files</param>
        /// <returns></returns>
        public static List<Tuple<string, string>> getStandardModules(string _rep)
        {
            List<string> xmlFiles = getXMLFiles(_rep);

            List<Tuple<string, string>> modules = new List<Tuple<string, string>>();
            foreach (string file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                List<string> services = getServicesType(doc);

                foreach (string s in services)
                {
                    modules.Add(new Tuple<string, string>(extractModule(s), file));
                }
            }

            return modules;
        }

        /// <summary>
        /// Get modules related to media
        /// </summary>
        /// <returns>The list of require modules</returns>
        public static List<Tuple<string, string>> getMediaModules(string _rep)
        {
            List<string> xmlFiles = getXMLFiles(_rep);

            List<Tuple<string, string>> modules = new List<Tuple<string, string>>();
            foreach (string file in xmlFiles)
            {
                string text = File.ReadAllText(file);
                if (text.Contains("icon=\"media") || text.Contains("<icon>media"))
                {
                    modules.Add(new Tuple<string, string>("media", file));
                }
                if (text.Contains("icon=\"rdMedia") || text.Contains("<icon>rdMedia"))
                {
                    modules.Add(new Tuple<string, string>("rdMedia", file));
                }
                if (text.Contains("icon=\"perfusionMedia") || text.Contains("<icon>perfusionMedia"))
                {
                    modules.Add(new Tuple<string, string>("perfusionMedia", file));
                }
                if (text.Contains("icon=\"EUSResources") || text.Contains("<icon>EUSResources"))
                {
                    modules.Add(new Tuple<string, string>("EUSResources", file));
                }
                if (text.Contains("icon=\"surgeomicsMedia") || text.Contains("<icon>surgeomicsMedia"))
                {
                    modules.Add(new Tuple<string, string>("surgeomicsMedia", file));
                }
                if (text.Contains("icon=\"flatIcon") || text.Contains("<icon>flatIcon"))
                {
                    modules.Add(new Tuple<string, string>("flatIcon", file));
                }
                if (text.Contains("icon=\"persiaMedia") || text.Contains("<icon>persiaMedia"))
                {
                    modules.Add(new Tuple<string, string>("persiaMedia", file));
                }
            }

            return modules;
        }

        /// <summary>
        /// Get modules related to extensions
        /// </summary>
        /// <param name="_extensionModules">Module name with extensions list</param>
        /// <returns>The list of require modules</returns>
        public static List<Tuple<string, string>> getExtensionModules(string _rep, List<Tuple<string, List<string>>> _extensionModules)
        {
            List<string> xmlFiles = getXMLFiles(_rep);
            List<Tuple<string, string>> modules = new List<Tuple<string, string>>();
            foreach (string file in xmlFiles)
            {
                string text = File.ReadAllText(file);
                foreach (Tuple<string, List<string>> extensionModule in _extensionModules)
                {
                    foreach (string extension in extensionModule.Item2)
                    {
                        if (checkId(text, extension))
                        {
                            modules.Add(new Tuple<string, string>(extensionModule.Item1, file));
                        }
                    }
                }
            }

            return modules;
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
                if (selectionAtt.Attributes["filter"] != null)
                {
                    services.Add(selectionAtt.Attributes["filter"].InnerText);
                }
            }
            return services;
        }

        /// <summary>
        /// Extract the module name from a service name
        /// </summary>
        /// <param name="_service">The name of the service</param>
        /// <returns>The name of the module</returns>
        private static string extractModule(string _service)
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
                _text.Contains("value=\"" + _keyword + "\"") ||
                _text.Contains("<id>" + _keyword + "</id>") ||
                _text.Contains("config=\"" + _keyword + "\"") ||
                _text.Contains("<ioSelectorConfig>" + _keyword + "</ioSelectorConfig>") ||
                _text.Contains("<activity>" + _keyword + "</activity>") ||
                _text.Contains("<sdbIoSelectorConfig>" + _keyword + "</sdbIoSelectorConfig>"));
        }
    }
}
