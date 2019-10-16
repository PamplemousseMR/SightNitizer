using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SightProperties
{
    class Sight
    {

        /// <summary>
        /// Get the root directory of the sight project
        /// </summary>
        /// <returns>The path of sight root</returns>
        public static String getRootDirectory(String _dir)
        {
            String sightPath = _dir;
            while (Directory.GetFiles(sightPath, ".sight").Length < 1 &&
                Directory.GetFiles(sightPath, ".fw4spl").Length < 1)
            {
                sightPath = Directory.GetParent(sightPath).FullName;
            }
            return Directory.GetParent(sightPath).FullName;
        }

        /// <summary>
        /// Get sight directories (contain a '.sight' file)
        /// </summary>
        /// <param name="_root">The root directory of the project</param>
        /// <returns>A list of all sight sub project</returns>
        public static List<String> getSightDirectory(String _root)
        {
            List<String> sightDirectories = new List<string>();
            foreach (String dir in Directory.GetDirectories(_root))
            {
                if (Directory.GetFiles(dir, ".sight").Length == 1 ||
                    Directory.GetFiles(dir, ".fw4spl").Length == 1)
                {
                    sightDirectories.Add(dir);
                }
            }
            return sightDirectories;
        }

        /// <summary>
        /// Get bundle directories in sight sub project
        /// </summary>
        /// <param name="_dir">The sight project</param>
        /// <returns>A list of all bundle directories</returns>
        public static List<String> getBundleDirectories(String _dir)
        {
            List<String> bundleDirectories = new List<string>();
            if(Directory.GetFiles(_dir, "Properties.cmake").Length < 1)
            {
                foreach (String dir in Directory.GetDirectories(_dir))
                {
                    bundleDirectories.AddRange(getBundleDirectories(dir));
                }
            }
            else
            {
                String propertiesFile = _dir + "\\Properties.cmake";
                String propertiesType = Properties.getType(propertiesFile);
                if (propertiesType.CompareTo("BUNDLE") == 0)
                {
                    bundleDirectories.Add(_dir);
                }
            }
            return bundleDirectories;
        }

        /// <summary>
        /// Get activities IDs related to a bundle
        /// </summary>
        /// <param name="_dir">The bundle directory</param>
        /// <returns>A tuple with the name of the bundle, and a list of all activities</returns>
        public static Tuple<String, List<String>> getActivitiesBundles(String _dir)
        {
            String propertiesFile = _dir + "\\Properties.cmake";
            String bundleName = Properties.getName(propertiesFile);
            List<String> activities = new List<String>();

            List<String> xmlFiles = Xml.getXMLFiles(_dir);
            foreach (String file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                XmlNodeList extensionNodes = doc.DocumentElement.GetElementsByTagName("extension");
                foreach (XmlNode extensionAtt in extensionNodes)
                {
                    if (extensionAtt.Attributes["implements"] != null &&
                        extensionAtt.Attributes["implements"].InnerText.CompareTo("::fwActivities::registry::Activities") == 0)
                    {
                        XmlNodeList idNodes = extensionAtt.SelectNodes("id");
                        foreach (XmlNode idAtt in idNodes)
                        {
                            activities.Add(idAtt.InnerText);
                        }
                    }
                }
            }

            return new Tuple<String, List<String>>(bundleName, activities);
        }

        /// <summary>
        /// Get app config IDs related to a bundle
        /// </summary>
        /// <param name="_dir">The bundle directory</param>
        /// <returns>A tuple with the name of the bundle, and a list of all app config</returns>
        public static Tuple<String, List<String>> getAppConfigBundles(String _dir)
        {
            String propertiesFile = _dir + "\\Properties.cmake";
            String bundleName = Properties.getName(propertiesFile);
            List<String> appConfig = new List<String>();

            List<String> xmlFiles = Xml.getXMLFiles(_dir);
            foreach (String file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                /// This extension is the root node of the XML file
                /// We don't need to loop over each nodes
                String extensionAtt = doc.DocumentElement.GetAttribute("implements");
                if (extensionAtt != null &&
                    extensionAtt.CompareTo("::fwServices::registry::AppConfig") == 0)
                {
                    XmlNodeList idNodes = doc.DocumentElement.SelectNodes("id");
                    foreach (XmlNode idAtt in idNodes)
                    {
                        appConfig.Add(idAtt.InnerText);
                    }
                }
            }

            return new Tuple<String, List<String>>(bundleName, appConfig);
        }

        /// <summary>
        /// Get service config IDs related to a bundle
        /// </summary>
        /// <param name="_dir">The bundle directory</param>
        /// <returns>A tuple with the name of the bundle, and a list of all service config</returns>
        public static Tuple<String, List<String>> getServiceConfigBundles(String _dir)
        {
            String propertiesFile = _dir + "\\Properties.cmake";
            String bundleName = Properties.getName(propertiesFile);
            List<String> serviceConfig = new List<String>();

            List<String> xmlFiles = Xml.getXMLFiles(_dir);
            foreach (String file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                XmlNodeList extensionNodes = doc.DocumentElement.GetElementsByTagName("extension");
                foreach (XmlNode extensionAtt in extensionNodes)
                {
                    if (extensionAtt.Attributes["implements"] != null &&
                        extensionAtt.Attributes["implements"].InnerText.CompareTo("::fwServices::registry::ServiceConfig") == 0)
                    {
                        XmlNodeList idNodes = extensionAtt.SelectNodes("id");
                        foreach (XmlNode idAtt in idNodes)
                        {
                            serviceConfig.Add(idAtt.InnerText);
                        }
                    }
                }
            }

            return new Tuple<String, List<String>>(bundleName, serviceConfig);
        }

    }
}
