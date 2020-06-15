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
        /// Get module directories in sight sub project
        /// </summary>
        /// <param name="_dir">The sight project</param>
        /// <returns>A list of all module directories</returns>
        public static List<string> getModuleDirectories(string _dir)
        {
            List<string> moduleDirectories = new List<string>();
            if (Directory.GetFiles(_dir, "Properties.cmake").Length < 1)
            {
                foreach (string dir in Directory.GetDirectories(_dir))
                {
                    moduleDirectories.AddRange(getModuleDirectories(dir));
                }
            }
            else
            {
                string propertiesFile = _dir + "\\Properties.cmake";
                Properties.TYPE propertiesType = Properties.getType(propertiesFile);
                if (propertiesType == Properties.TYPE.MODULE || propertiesType == Properties.TYPE.BUNDLE)
                {
                    moduleDirectories.Add(_dir);
                }
            }
            return moduleDirectories;
        }

        /// <summary>
        /// Get activities IDs related to a module
        /// </summary>
        /// <param name="_dir">The module directory</param>
        /// <returns>A tuple with the name of the module, and a list of all activities</returns>
        public static Tuple<string, List<string>> getActivitiesModules(string _dir)
        {
            string propertiesFile = _dir + "\\Properties.cmake";
            string moduleName = Properties.getName(propertiesFile);
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

            return new Tuple<string, List<string>>(moduleName, activities);
        }

        /// <summary>
        /// Get app config IDs related to a module
        /// </summary>
        /// <param name="_dir">The module directory</param>
        /// <returns>A tuple with the name of the module, and a list of all app config</returns>
        public static Tuple<string, List<string>> getAppConfigModules(string _dir)
        {
            string propertiesFile = _dir + "\\Properties.cmake";
            string moduleName = Properties.getName(propertiesFile);
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

            return new Tuple<string, List<string>>(moduleName, appConfig);
        }

        /// <summary>
        /// Get service config IDs related to a module
        /// </summary>
        /// <param name="_dir">The module directory</param>
        /// <returns>A tuple with the name of the module, and a list of all service config</returns>
        public static Tuple<string, List<string>> getServiceConfigModules(string _dir)
        {
            string propertiesFile = _dir + "\\Properties.cmake";
            string moduleName = Properties.getName(propertiesFile);
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

            return new Tuple<string, List<string>>(moduleName, serviceConfig);
        }

    }
}
