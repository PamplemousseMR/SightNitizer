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
        public static string getRootDirectory(string _dir)
        {
            string sightPath = _dir;
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
        public static List<string> getSightDirectory(string _root)
        {
            List<string> sightDirectories = new List<string>();
            foreach (string dir in Directory.GetDirectories(_root))
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
        public static List<string> getBundleDirectories(string _dir)
        {
            List<string> bundleDirectories = new List<string>();
            if(Directory.GetFiles(_dir, "Properties.cmake").Length < 1)
            {
                foreach (string dir in Directory.GetDirectories(_dir))
                {
                    bundleDirectories.AddRange(getBundleDirectories(dir));
                }
            }
            else
            {
                string propertiesFile = _dir + "\\Properties.cmake";
                string propertiesType = Properties.getType(propertiesFile);
                if (propertiesType == "BUNDLE")
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
        public static Tuple<string, List<string>> getActivitiesBundles(string _dir)
        {
            string propertiesFile = _dir + "\\Properties.cmake";
            string bundleName = Properties.getName(propertiesFile);
            List<string> activities = new List<string>();

            List<string> xmlFiles = Xml.getXMLFiles(_dir);
            foreach (string file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                XmlNodeList extensionNodes = doc.DocumentElement.GetElementsByTagName("extension");
                foreach (XmlNode extensionAtt in extensionNodes)
                {
                    if (extensionAtt.Attributes["implements"] != null &&
                        extensionAtt.Attributes["implements"].InnerText == "::fwActivities::registry::Activities")
                    {
                        XmlNodeList idNodes = extensionAtt.SelectNodes("id");
                        foreach (XmlNode idAtt in idNodes)
                        {
                            activities.Add(idAtt.InnerText);
                        }
                    }
                }
            }

            return new Tuple<string, List<string>>(bundleName, activities);
        }

        /// <summary>
        /// Get app config IDs related to a bundle
        /// </summary>
        /// <param name="_dir">The bundle directory</param>
        /// <returns>A tuple with the name of the bundle, and a list of all app config</returns>
        public static Tuple<string, List<string>> getAppConfigBundles(string _dir)
        {
            string propertiesFile = _dir + "\\Properties.cmake";
            string bundleName = Properties.getName(propertiesFile);
            List<string> appConfig = new List<string>();

            List<string> xmlFiles = Xml.getXMLFiles(_dir);
            foreach (string file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                /// This extension is the root node of the XML file
                /// We don't need to loop over each nodes
                string extensionAtt = doc.DocumentElement.GetAttribute("implements");
                if (extensionAtt != null &&
                    extensionAtt == "::fwServices::registry::AppConfig")
                {
                    XmlNodeList idNodes = doc.DocumentElement.SelectNodes("id");
                    foreach (XmlNode idAtt in idNodes)
                    {
                        appConfig.Add(idAtt.InnerText);
                    }
                }
            }

            return new Tuple<string, List<string>>(bundleName, appConfig);
        }

        /// <summary>
        /// Get service config IDs related to a bundle
        /// </summary>
        /// <param name="_dir">The bundle directory</param>
        /// <returns>A tuple with the name of the bundle, and a list of all service config</returns>
        public static Tuple<string, List<string>> getServiceConfigBundles(string _dir)
        {
            string propertiesFile = _dir + "\\Properties.cmake";
            string bundleName = Properties.getName(propertiesFile);
            List<string> serviceConfig = new List<string>();

            List<string> xmlFiles = Xml.getXMLFiles(_dir);
            foreach (string file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                XmlNodeList extensionNodes = doc.DocumentElement.GetElementsByTagName("extension");
                foreach (XmlNode extensionAtt in extensionNodes)
                {
                    if (extensionAtt.Attributes["implements"] != null &&
                        extensionAtt.Attributes["implements"].InnerText == "::fwServices::registry::ServiceConfig")
                    {
                        XmlNodeList idNodes = extensionAtt.SelectNodes("id");
                        foreach (XmlNode idAtt in idNodes)
                        {
                            serviceConfig.Add(idAtt.InnerText);
                        }
                    }
                }
            }

            return new Tuple<string, List<string>>(bundleName, serviceConfig);
        }

    }
}
