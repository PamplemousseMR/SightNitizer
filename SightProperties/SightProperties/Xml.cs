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
                if (text.Contains("::fwRenderQt::"))
                {
                    bundles.Add(new Tuple<String, String>("scene2D", file));
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
                if (text.Contains("icon=\"resources") || text.Contains("<icon>resources"))
                {
                    bundles.Add(new Tuple<String, String>("resources", file));
                }
            }

            return bundles;
        }

        /// <summary>
        /// Get bundles related to appconfig
        /// </summary>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<String, String>> getAppConfigBundles(String _rep)
        {
            List<String> xmlFiles = getXMLFiles(_rep);
            List<Tuple<String, String>> bundles = new List<Tuple<String, String>>();
            foreach (String file in xmlFiles)
            {
                String text = File.ReadAllText(file);
                if (text.Contains("id=\"ImageManager\"") ||
                    text.Contains("id=\"ModelSeriesManagerView\"") ||
                    text.Contains("id=\"ModelSeriesManagerWindow\""))
                {
                    bundles.Add(new Tuple<String, String>("dataManagerConfig", file));
                }
                if (text.Contains("id=\"TransferFunctionWithNegatoEditor\""))
                {
                    bundles.Add(new Tuple<String, String>("imageConfig", file));
                }
                if (text.Contains("id=\"OgreHistogramManager\"") ||
                    text.Contains("id=\"OgreLightManager\"") ||
                    text.Contains("id=\"OgreOrganManager\""))
                {
                    bundles.Add(new Tuple<String, String>("ogreConfig", file));
                }
                if (text.Contains("id=\"TransferFunctionWidget\""))
                {
                    bundles.Add(new Tuple<String, String>("qtSceneConfig", file));
                }
                if (text.Contains("id=\"OgreIDVRManager\""))
                {
                    bundles.Add(new Tuple<String, String>("configOgreEx", file));
                }
                if (text.Contains("id=\"landmarkModelSeriesView\"") ||
                    text.Contains("id=\"manualRegistrationView\""))
                {
                    bundles.Add(new Tuple<String, String>("modelSeriesConfig", file));
                }
            }

            return bundles;
        }

        /*
        * Commun utils fnctions
        */

        /// <summary>
        /// Get all xml file names in a directory
        /// </summary>
        /// <param name="_rep">The directory</param>
        /// <returns>The list af all file names</returns>
        private static List<String> getXMLFiles(String _rep)
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
        /// Get all services from an xml file
        /// </summary>
        /// <param name="_doc">The xml file</param>
        /// <returns>The list of all services</returns>
        private static List<String> getServices(XmlDocument _doc)
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
    }
}
