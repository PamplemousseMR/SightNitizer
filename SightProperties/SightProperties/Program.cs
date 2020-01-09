using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace SightProperties
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
        /// <param name="_directory">The directory (Bundle/SrcLib/App) to check</param>
        static private void processDirectory(string _directory)
        {
            /// Get sight directories informations
            string rootDirectory = Sight.getRootDirectory(_directory);
            List<string> sightDirectories = Sight.getSightDirectory(rootDirectory);

            /// Retreive bundles with activities, app config and service config names
            List<Tuple<string, List<string>>> activityBundles = new List<Tuple<string, List<string>>>();
            List<Tuple<string, List<string>>> appConfigBundles = new List<Tuple<string, List<string>>>();
            List<Tuple<string, List<string>>> serviceConfigBundles = new List<Tuple<string, List<string>>>();
            foreach (string directory in sightDirectories)
            {
                foreach (string dir in Sight.getBundleDirectories(directory))
                {
                    activityBundles.Add(Sight.getActivitiesBundles(dir));
                    appConfigBundles.Add(Sight.getAppConfigBundles(dir));
                    serviceConfigBundles.Add(Sight.getServiceConfigBundles(dir));
                }
            }

            /// Get current libray/bundle name
            string[] path = _directory.Split('\\');
            string currentName = path[path.Length - 1];

            /// Get bundles list of xml files
            List<Tuple<string, string>> xmlBundles = Xml.getDefaultBundles(_directory);
            xmlBundles.AddRange(Xml.getRequireBundles(_directory));
            xmlBundles.AddRange(Xml.getObjectsBundles(_directory));
            xmlBundles.AddRange(Xml.getOgreBundles(_directory));
            xmlBundles.AddRange(Xml.getQtBundles(_directory));
            xmlBundles.AddRange(Xml.getVTKBundles(_directory));
            xmlBundles.AddRange(Xml.getStandardBundles(_directory));
            xmlBundles.AddRange(Xml.getMediaBundles(_directory));
            xmlBundles.AddRange(Xml.getExtensionBundles(_directory, activityBundles));
            xmlBundles.AddRange(Xml.getExtensionBundles(_directory, appConfigBundles));
            xmlBundles.AddRange(Xml.getExtensionBundles(_directory, serviceConfigBundles));

            /// Get require bundles in xml files
            List<Tuple<string, string>> xmlRequirements = Xml.getRequireBundles(_directory);

            /// Get bundles list of languages files
            List<Tuple<string, string>> languagesBundles = Language.getIncludeBundles(_directory);

            /// Get Properties.cmake
            string propertiesFile = _directory + "\\Properties.cmake";

            /// Get the type of the directory (APP/EXECUTABLE/BUNDLE/LIBRARY/TEST)
            Properties.TYPE propertiesType = Properties.getType(propertiesFile);

            /// Get the requirement list
            List<string> propertiesRequirements = Properties.getRequirements(propertiesFile);

            /// Get the dependencies list
            List<string> propertiesDependencies = Properties.getDependencies(propertiesFile);

            ///========================================================================================================
            /// In case of APP type bundles
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
                    Logs.getInstance().error("The bundle: `appXml` was not found in the `Properties.cmake` in " + _directory + ".");
                }
                if (!findFwlauncher)
                {
                    Logs.getInstance().error("The bundle: `fwlauncher` was not found in the `Properties.cmake` in " + _directory + ".");
                }
            }

            ///========================================================================================================
            /// If it's not a library
            ///========================================================================================================
            if (propertiesType != Properties.TYPE.LIBRARY)
            {
                ///========================================================================================================
                /// Check that bundle in xml file are properly started (requirement in xml files)
                ///========================================================================================================
                foreach (string propertiesRequirement in propertiesRequirements)
                {
                    /// List of bundle that require a starting
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
                            Logs.getInstance().error("The bundle: `" + propertiesRequirement + "` needs to be in the xml's requirements list in " + _directory + ".");
                        }
                    }
                }
            }

            ///========================================================================================================
            /// Check that bundles used in xml files are in the Properties.cmake (REQUIREMENT)
            ///========================================================================================================
            foreach (Tuple<string, string> bundle in xmlBundles)
            {
                /// Check this special libraries, it's not in the requirement list, there are included by others bundles
                if (bundle.Item1 != currentName &&
                    bundle.Item1 != "fwServices" &&
                    bundle.Item1 != "fwActivities" &&
                    bundle.Item1 != "fwRenderOgre" &&
                    bundle.Item1 != "fwRenderVTK" &&
                    bundle.Item1 != "fwRenderQt")
                {
                    bool find = false;
                    foreach (string requirement in propertiesRequirements)
                    {
                        if (requirement == bundle.Item1)
                        {
                            find = true;
                            break;
                        }
                    }
                    if (!find)
                    {
                        Logs.getInstance().error("The bundle: `" + bundle.Item1 + "` from: `" + bundle.Item2 + "` was not found in the `Properties.cmake`.");
                    }
                }
            }

            ///========================================================================================================
            /// Check that this specials libraries are not in the requirement list, there are included by others bundles
            ///========================================================================================================
            foreach (string bundle in propertiesRequirements)
            {
                if (bundle == "fwServices" ||
                    bundle == "fwActivities" ||
                    bundle == "fwRenderOgre" ||
                    bundle == "fwRenderVTK" ||
                    bundle == "fwRenderQt")
                {
                    Logs.getInstance().error("The library: `" + bundle + "` should not be in the REQUIREMENT list of the `Properties.cmake`.");
                }
            }

            ///========================================================================================================
            /// Check that library used in languages files are in the Properties.cmake (DEPENDENCIES)
            ///========================================================================================================
            foreach (Tuple<string, string> bundle in languagesBundles)
            {
                /// Skip external libraries and the current one
                if (bundle.Item1 != currentName &&
                    bundle.Item1 != "boost" &&
                    bundle.Item1 != "camp" &&
                    bundle.Item1 != "ceres" &&
                    bundle.Item1 != "cppunit" &&
                    bundle.Item1 != "dcmtk" &&
                    bundle.Item1 != "Eigen" &&
                    bundle.Item1 != "glm" &&
                    bundle.Item1 != "librealsense2" &&
                    bundle.Item1 != "libxml" &&
                    bundle.Item1 != "OGRE" &&
                    bundle.Item1 != "GL" &&
                    bundle.Item1 != "OpenGL" &&
                    bundle.Item1 != "opencv2" &&
                    bundle.Item1 != "OpenNI" &&
                    bundle.Item1 != "pcl" &&
                    bundle.Item1 != "vtk" &&
                    bundle.Item1 != "vlc" &&
                    bundle.Item1 != "IPPE" &&
                    bundle.Item1 != "cryptopp" &&
                    bundle.Item1 != "glog" &&
                    bundle.Item1 != "odil" &&
                    bundle.Item1 != "sofa" &&
                    bundle.Item1 != "tetgen" &&
                    bundle.Item1 != "trakSTAR" &&
                    bundle.Item1 != "BulletSoftBody" &&
                    bundle.Item1 != "sys" &&
                    bundle.Item1 != "grpc++" &&
                    bundle.Item1 != "pybind11" &&
                    bundle.Item1 != "itkhdf5" &&
                    bundle.Item1 != "openvslam" &&
                    bundle.Item1 != "spdlog" &&
                    bundle.Item1 != "grpcpp")
                {
                    bool find = false;
                    foreach (string dependenci in propertiesDependencies)
                    {
                        if (dependenci == bundle.Item1)
                        {
                            find = true;
                            break;
                        }
                    }
                    if (!find)
                    {
                        Logs.getInstance().error("The library: `" + bundle.Item1 + "` from: `" + bundle.Item2 + "` was not found in the `Properties.cmake`.");
                    }
                }
            }

            ///========================================================================================================
            /// Check that bundles and libraries in the Properties.cmake are used
            ///========================================================================================================
            List<Tuple<string, string>> bundlesAndLibraries = xmlBundles;
            bundlesAndLibraries.AddRange(languagesBundles);

            List<string> requirementsAnDependencies = propertiesRequirements;
            requirementsAnDependencies.AddRange(propertiesDependencies);
            foreach (string requirementOrDependency in requirementsAnDependencies)
            {
                /// Skip 'style' bundles, this bundle is used in a weird way and will be checked below
                /// Skip 'uiTF' bundles, this bundle contains files for pre-defined TF
                if (!(
                    (requirementOrDependency == "style") ||
                    (requirementOrDependency.StartsWith("io"))
                    ))
                {
                    bool find = false;
                    foreach (Tuple<string, string> bundleOrLibrary in bundlesAndLibraries)
                    {
                        if (requirementOrDependency == bundleOrLibrary.Item1)
                        {
                            find = true;
                            break;
                        }
                    }
                    /// If the bundle is 'appXml' or 'fwlauncher', it can't be used.
                    /// These bundle must be here only in APP type bundles (checked above).
                    if (!((requirementOrDependency == "appXml" || requirementOrDependency == "fwlauncher") && propertiesType == Properties.TYPE.APP))
                    {
                        if (!find)
                        {
                            Logs.getInstance().error("The library/bundle: `" + requirementOrDependency + "` is not used in " + _directory + ".");
                        }
                    }
                }
            }

            ///========================================================================================================
            /// Check 'style' bundle here
            ///========================================================================================================
            if (requirementsAnDependencies.Contains("style"))
            {
                /// The 'style' bundle is used in the properties.cmake files
                string text = File.ReadAllText(propertiesFile);
                if (!text.Contains("style-0.1/"))
                {
                    Logs.getInstance().error("The bundle: `style` is not used in " + _directory + ".");
                }
            }

            ///========================================================================================================
            /// TODO, check IO bundles, video bundles
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
