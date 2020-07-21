using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace SightNitizer
{

    class Program
    {
        static void Main(string[] _args)
        {
            /// Parse args
            if (Option.parse(_args))
            {
                // If the directory contain a properties.cmake
                if (Option.isPropertiesDirectory())
                {
                    processDirectory(Option.getDirectory());
                }
                // Else, it must be a git repository
                else if (Repository.IsValid(Option.getDirectory()))
                {
                    using (var repo = new Repository(Option.getDirectory()))
                    {
                        IEnumerable<StatusEntry> files = repo.RetrieveStatus().Modified;
                        List<string> directories = new List<string>();
                        // For each modified files
                        foreach (StatusEntry statusEntry in files)
                        {
                            // Get the full path
                            string directory = statusEntry.FilePath.Replace('/', '\\'); ;
                            while (directory != "")
                            {
                                if (File.Exists(Option.getDirectory() + "\\" + directory + "\\Properties.cmake"))
                                {
                                    // Add it if it's not already added.
                                    bool found = false;
                                    foreach (string addedDirectory in directories)
                                    {
                                        if (addedDirectory == Option.getDirectory() + "\\" + directory)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found)
                                    {
                                        directories.Add(Option.getDirectory() + "\\" + directory);
                                    }
                                    break;
                                }
                                int lastIndex = directory.LastIndexOf("\\");
                                if (lastIndex != -1)
                                {
                                    directory = directory.Substring(0, lastIndex);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }

                        if (directories.Count == 0)
                        {
                            Logs.getInstance().info("No modified files where found");
                        }
                        else
                        {
                            foreach (string directory in directories)
                            {
                                processDirectory(directory);
                            }
                        }
                    }
                }
                else
                {
                    Logs.getInstance().error("The path must be a git repository or a sight directory that contain a Properties.cmake file.");
                }
            }

            Logs logs = Logs.getInstance();
            foreach (string log in logs.getInfo())
            {
                Console.WriteLine(log);
            }

            if (logs.getErrors().Count > 0)
            {
                foreach(string log in logs.getErrors())
                {
                    Console.WriteLine(log);
                }
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Check many things in a directory that contains a Properties.cmake
        /// </summary>
        /// <param name="_directory">The directory (Module/SrcLib/App) to check</param>
        static private void processDirectory(string _directory)
        {
            /// Get sight directories informations
            string rootDirectory = Sight.getRootDirectory(_directory);
            List<string> sightDirectories = Sight.getSightDirectory(rootDirectory);

            /// Retreive modules with activities, app config and service config names
            List<Tuple<string, List<string>>> activityModules = new List<Tuple<string, List<string>>>();
            List<Tuple<string, List<string>>> appConfigModules = new List<Tuple<string, List<string>>>();
            List<Tuple<string, List<string>>> serviceConfigModules = new List<Tuple<string, List<string>>>();
            foreach (string directory in sightDirectories)
            {
                foreach (string dir in Sight.getModuleDirectories(directory))
                {
                    activityModules.Add(Sight.getActivitiesModules(dir));
                    appConfigModules.Add(Sight.getAppConfigModules(dir));
                    serviceConfigModules.Add(Sight.getServiceConfigModules(dir));
                }
            }

            /// Get current libray/module name
            string[] path = _directory.Split('\\');
            string currentName = path[path.Length - 1];

            /// Get modules list of xml files
            List<Tuple<string, string>> xmlModules = Xml.getDefaultModules(_directory);
            xmlModules.AddRange(Xml.getRequireModules(_directory));
            xmlModules.AddRange(Xml.getObjectsModules(_directory));
            xmlModules.AddRange(Xml.getOgreModules(_directory));
            xmlModules.AddRange(Xml.getQtModules(_directory));
            xmlModules.AddRange(Xml.getVTKModules(_directory));
            xmlModules.AddRange(Xml.getStandardModules(_directory));
            xmlModules.AddRange(Xml.getMediaModules(_directory));
            xmlModules.AddRange(Xml.getExtensionModules(_directory, activityModules));
            xmlModules.AddRange(Xml.getExtensionModules(_directory, appConfigModules));
            xmlModules.AddRange(Xml.getExtensionModules(_directory, serviceConfigModules));

            /// Get require modules in xml files
            List<Tuple<string, string>> xmlRequirements = Xml.getRequireModules(_directory);

            /// Get modules list of languages files
            List<Tuple<string, string>> languagesModules = Language.getIncludeModules(_directory);

            /// Get Properties.cmake
            string propertiesFile = _directory + "\\Properties.cmake";

            /// Get the type of the directory (APP/EXECUTABLE/BUNDLE/LIBRARY/TEST)
            Properties.TYPE propertiesType = Properties.getType(propertiesFile);

            /// Get the requirement list
            List<string> propertiesRequirements = Properties.getRequirements(propertiesFile);

            /// Get the dependencies list
            List<string> propertiesDependencies = Properties.getDependencies(propertiesFile);

            ///========================================================================================================
            /// In case of APP type modules
            ///========================================================================================================
            if (propertiesType == Properties.TYPE.APP)
            {
                ///========================================================================================================
                /// Check that appxml and fwlauncher are in the Properties.cmake
                ///========================================================================================================
                bool findAppXml = false;
                bool findFwlauncher = false;
                foreach (string propertiesRequirement in propertiesRequirements)
                {
                    if (findAppXml && findFwlauncher)
                    {
                        break;
                    }
                    if (propertiesRequirement == "appXml")
                    {
                        findAppXml = true;
                    }
                    else if (propertiesRequirement == "fwlauncher")
                    {
                        findFwlauncher = true;
                    }
                }
                if (!findAppXml)
                {
                    Logs.getInstance().error("The module: `appXml` was not found in the `Properties.cmake` in " + _directory + ".");
                }
                if (!findFwlauncher)
                {
                    Logs.getInstance().error("The module: `fwlauncher` was not found in the `Properties.cmake` in " + _directory + ".");
                }
            }

            ///========================================================================================================
            /// If it's not a library
            ///========================================================================================================
            if (propertiesType != Properties.TYPE.LIBRARY)
            {
                ///========================================================================================================
                /// Check that module in xml file are properly started (requirement in xml files)
                ///========================================================================================================
                foreach (string propertiesRequirement in propertiesRequirements)
                {
                    /// List of module that require a starting
                    if (propertiesRequirement == "validators" ||
                        propertiesRequirement == "filterUnknownSeries" ||
                        propertiesRequirement == "filterVRRender" ||
                        propertiesRequirement == "activities" ||
                        propertiesRequirement == "arDataReg" ||
                        propertiesRequirement == "dataReg" ||
                        propertiesRequirement == "memory" ||
                        propertiesRequirement == "preferences" ||
                        propertiesRequirement == "servicesReg" ||
                        propertiesRequirement == "ioDicomWeb" ||
                        propertiesRequirement == "ioPacs" ||
                        propertiesRequirement == "arPatchMedicalData" ||
                        propertiesRequirement == "patchMedicalData" ||
                        propertiesRequirement == "console" ||
                        propertiesRequirement == "guiQt" ||
                        propertiesRequirement == "scene2D" ||
                        propertiesRequirement == "visuOgre" ||
                        propertiesRequirement == "material" ||
                        propertiesRequirement == "materialEx" ||
                        propertiesRequirement == "visuVTKQml" ||
                        propertiesRequirement == "visuVTKQt")
                    {
                        bool find = false;
                        foreach (Tuple<string, string> requirement in xmlRequirements)
                        {
                            if (propertiesRequirement == requirement.Item1)
                            {
                                find = true;
                                break;
                            }
                        }

                        if (!find)
                        {
                            Logs.getInstance().error("The module: `" + propertiesRequirement + "` needs to be in the xml's requirements list in " + _directory + ".");
                        }
                    }
                }
            }

            ///========================================================================================================
            /// Check that modules used in xml files are in the Properties.cmake (REQUIREMENT)
            ///========================================================================================================
            foreach (Tuple<string, string> module in xmlModules)
            {
                /// Check this special libraries, it's not in the requirement list, there are included by others modules
                if (module.Item1 != currentName &&
                    module.Item1 != "fwServices" &&
                    module.Item1 != "fwActivities" &&
                    module.Item1 != "fwRenderOgre" &&
                    module.Item1 != "fwRenderVTK" &&
                    module.Item1 != "fwRenderQt")
                {
                    bool find = false;
                    foreach (string requirement in propertiesRequirements)
                    {
                        if (requirement == module.Item1)
                        {
                            find = true;
                            break;
                        }
                    }
                    if (!find)
                    {
                        Logs.getInstance().error("The module: `" + module.Item1 + "` from: `" + module.Item2 + "` was not found in the `Properties.cmake`.");
                    }
                }
            }

            ///========================================================================================================
            /// Check that this specials libraries are not in the requirement list, there are included by others modules
            ///========================================================================================================
            foreach (string module in propertiesRequirements)
            {
                if (module == "fwServices" ||
                    module == "fwActivities" ||
                    module == "fwRenderOgre" ||
                    module == "fwRenderVTK" ||
                    module == "fwRenderQt")
                {
                    Logs.getInstance().error("The library: `" + module + "` should not be in the REQUIREMENT list of the `Properties.cmake`.");
                }
            }

            ///========================================================================================================
            /// Check that library used in languages files are in the Properties.cmake (DEPENDENCIES)
            ///========================================================================================================
            foreach (Tuple<string, string> module in languagesModules)
            {
                /// Skip external libraries and the current one
                if (module.Item1 != currentName &&
                    module.Item1 != "boost" &&
                    module.Item1 != "camp" &&
                    module.Item1 != "ceres" &&
                    module.Item1 != "cppunit" &&
                    module.Item1 != "dcmtk" &&
                    module.Item1 != "Eigen" &&
                    module.Item1 != "glm" &&
                    module.Item1 != "librealsense2" &&
                    module.Item1 != "libxml" &&
                    module.Item1 != "OGRE" &&
                    module.Item1 != "GL" &&
                    module.Item1 != "OpenGL" &&
                    module.Item1 != "opencv2" &&
                    module.Item1 != "OpenNI" &&
                    module.Item1 != "pcl" &&
                    module.Item1 != "vtk" &&
                    module.Item1 != "vlc" &&
                    module.Item1 != "IPPE" &&
                    module.Item1 != "cryptopp" &&
                    module.Item1 != "glog" &&
                    module.Item1 != "odil" &&
                    module.Item1 != "sofa" &&
                    module.Item1 != "tetgen" &&
                    module.Item1 != "trakSTAR" &&
                    module.Item1 != "BulletSoftBody" &&
                    module.Item1 != "sys" &&
                    module.Item1 != "grpc++" &&
                    module.Item1 != "pybind11" &&
                    module.Item1 != "itkhdf5" &&
                    module.Item1 != "openvslam" &&
                    module.Item1 != "spdlog" &&
                    module.Item1 != "grpcpp")
                {
                    bool find = false;
                    foreach (string dependenci in propertiesDependencies)
                    {
                        if (dependenci == module.Item1)
                        {
                            find = true;
                            break;
                        }
                    }
                    if (!find)
                    {
                        Logs.getInstance().error("The library: `" + module.Item1 + "` from: `" + module.Item2 + "` was not found in the `Properties.cmake`.");
                    }
                }
            }

            ///========================================================================================================
            /// Check that modules and libraries in the Properties.cmake are used
            ///========================================================================================================
            List<Tuple<string, string>> modulesAndLibraries = xmlModules;
            modulesAndLibraries.AddRange(languagesModules);

            List<string> requirementsAnDependencies = propertiesRequirements;
            requirementsAnDependencies.AddRange(propertiesDependencies);
            foreach (string requirementOrDependency in requirementsAnDependencies)
            {
                /// Skip 'style' modules, this module is used in a weird way and will be checked below
                /// Skip 'io' modules, they just need to be include to work.
                if (!(
                    (requirementOrDependency == "style") ||
                    (requirementOrDependency.StartsWith("io"))
                    ))
                {
                    bool find = false;
                    foreach (Tuple<string, string> moduleOrLibrary in modulesAndLibraries)
                    {
                        if (requirementOrDependency == moduleOrLibrary.Item1)
                        {
                            find = true;
                            break;
                        }
                    }
                    /// If the module is 'appXml' or 'fwlauncher', it can't be used.
                    /// These module must be here only in APP type modules (checked above).
                    if (!((requirementOrDependency == "appXml" || requirementOrDependency == "fwlauncher") && propertiesType == Properties.TYPE.APP))
                    {
                        if (!find)
                        {
                            Logs.getInstance().error("The library/module: `" + requirementOrDependency + "` is not used in " + _directory + ".");
                        }
                    }
                }
            }

            ///========================================================================================================
            /// Check 'style' module here
            ///========================================================================================================
            if (requirementsAnDependencies.Contains("style"))
            {
                /// The 'style' module is used in the properties.cmake files
                string text = File.ReadAllText(propertiesFile);
                if (!text.Contains("style-0.1/"))
                {
                    Logs.getInstance().error("The module: `style` is not used in " + _directory + ".");
                }
            }

            ///========================================================================================================
            /// TODO, check IO modules, video modules
            ///========================================================================================================

            ///========================================================================================================
            /// Check that all services are used
            ///========================================================================================================
            List<string> xmlFiles = Xml.getXMLFiles(_directory);
            foreach (string file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                XmlNodeList serviceNodes = doc.DocumentElement.GetElementsByTagName("service");
                List<string> servicesUid = Xml.getServicesUid(doc);

                /// Check that each service is use in others services
                foreach (string serviceUid in servicesUid)
                {
                    Boolean find = false;
                    foreach (XmlNode serviceAtt in serviceNodes)
                    {
                        StringWriter sw = new StringWriter();
                        XmlTextWriter xw = new XmlTextWriter(sw)
                        {
                            Formatting = Formatting.Indented
                        };
                        serviceAtt.WriteTo(xw);
                        /// The first line contain the uid, we must remove it
                        string service = sw.ToString();
                        service = service.Substring(service.IndexOf(Environment.NewLine) + 1);
                        if (service.Contains(serviceUid))
                        {
                            find = true;
                            break;
                        }
                    }

                    /// If it's not used in others services, it can be just started
                    if (!find)
                    {
                        XmlNodeList startNodes = doc.DocumentElement.GetElementsByTagName("start");
                        foreach (XmlNode startAtt in startNodes)
                        {
                            if (startAtt.Attributes["uid"] != null)
                            {
                                if (startAtt.Attributes["uid"].InnerText == serviceUid)
                                {
                                    find = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!find)
                    {
                        Logs.getInstance().error("The service: `" + serviceUid + "` is not used in the file '" + file + "'.");
                    }
                }
            }

            ///========================================================================================================
            /// Check that all objects are used
            ///========================================================================================================
            foreach (string file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                List<string> objectsUid = Xml.getObjectUid(doc);

                /// Check that this object is used in a 'inout'
                foreach (string objectUid in objectsUid)
                {
                    Boolean find = false;
                    XmlNodeList inoutNodes = doc.DocumentElement.GetElementsByTagName("inout");
                    foreach (XmlNode inoutAtt in inoutNodes)
                    {
                        if (inoutAtt.Attributes["uid"] != null)
                        {
                            if (inoutAtt.Attributes["uid"].InnerText == objectUid)
                            {
                                find = true;
                                break;
                            }
                        }
                    }

                    /// Check that this object is used in a 'in'
                    if (!find)
                    {
                        XmlNodeList inNodes = doc.DocumentElement.GetElementsByTagName("in");
                        foreach (XmlNode inAtt in inNodes)
                        {
                            if (inAtt.Attributes["uid"] != null)
                            {
                                if (inAtt.Attributes["uid"].InnerText == objectUid)
                                {
                                    find = true;
                                    break;
                                }
                            }
                        }
                    }

                    /// Check that this object is used in a 'out'
                    if (!find)
                    {
                        XmlNodeList outNodes = doc.DocumentElement.GetElementsByTagName("out");
                        foreach (XmlNode outAtt in outNodes)
                        {
                            if (outAtt.Attributes["uid"] != null)
                            {
                                if (outAtt.Attributes["uid"].InnerText == objectUid)
                                {
                                    find = true;
                                    break;
                                }
                            }
                        }
                    }

                    /// Check that this object is used in a 'key'
                    if (!find)
                    {
                        XmlNodeList outNodes = doc.DocumentElement.GetElementsByTagName("key");
                        foreach (XmlNode outAtt in outNodes)
                        {
                            if (outAtt.Attributes["uid"] != null)
                            {
                                if (outAtt.Attributes["uid"].InnerText == objectUid)
                                {
                                    find = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!find)
                    {
                        Logs.getInstance().error("The object: `" + objectUid + "` is not used in the file '" + file + "'.");
                    }
                }
            }

            ///========================================================================================================
            /// Check that all channel are used
            ///========================================================================================================
            foreach (string file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                XmlNodeList connectNodes = doc.DocumentElement.GetElementsByTagName("connect");

                string text = File.ReadAllText(file);

                /// Retreive all channel uid
                List<string> channelsUid = new List<string>();
                foreach (XmlNode connectAtt in connectNodes)
                {
                    if (connectAtt.Attributes["channel"] != null)
                    {
                        string channel = connectAtt.Attributes["channel"].InnerText;
                        /// Remove the reference, the channel can be used as parameter
                        channel = channel.Replace("${", "");
                        channel = channel.Replace("}", "");
                        /// If the channel is used only once, it's not used
                        if (Regex.Matches(text, channel).Count <= 1)
                        {
                            Logs.getInstance().error("The channel: `" + channel + "` is not used in the file '" + file + "'.");
                        }
                    }
                }
            }
        }
    }
}
