using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace SightProperties
{
    class Program
    {
        static void Main(String[] _args)
        {
            /// Parse args
            if (!Option.parse(_args))
            {
                Environment.Exit(1);
            }

            /// Get current libray/bundle name
            String[] path = Option.getDirectory().Split('\\');
            String currentName = path[path.Length - 1];

            /// Get bundles list of xml files
            List<Tuple<String, String>> xmlBundles = Xml.getDefaultBundles(Option.getDirectory());
            xmlBundles.AddRange(Xml.getRequireBundles(Option.getDirectory()));
            xmlBundles.AddRange(Xml.getObjectsBundles(Option.getDirectory()));
            xmlBundles.AddRange(Xml.getOgreBundles(Option.getDirectory()));
            xmlBundles.AddRange(Xml.getQtBundles(Option.getDirectory()));
            xmlBundles.AddRange(Xml.getVTKBundles(Option.getDirectory()));
            xmlBundles.AddRange(Xml.getStandardBundles(Option.getDirectory()));
            xmlBundles.AddRange(Xml.getMediaBundles(Option.getDirectory()));
            xmlBundles.AddRange(Xml.getAppConfigBundles(Option.getDirectory()));
            xmlBundles.AddRange(Xml.getServiceConfigBundles(Option.getDirectory()));
            xmlBundles.AddRange(Xml.getActivitiesBundles(Option.getDirectory()));

            /// Get require bundles in xml files
            List<Tuple<String, String>> xmlRequirements = Xml.getRequireBundles(Option.getDirectory());

            /// Get bundles list of languages files
            List<Tuple<String, String>> languagesBundles = Language.getIncludeBundles(Option.getDirectory());

            /// Get Properties.cmake
            String propertiesFile = Option.getDirectory() + "\\Properties.cmake";

            /// Get the type of the directory (APP/EXECUTABLE/BUNDLE/LIBRARY/TEST)
            String propertiesType = Properties.getType(propertiesFile);

            /// Get the requirement list
            List<String> propertiesRequirements = Properties.getRequirements(propertiesFile);

            /// Get the dependencies list
            List<String> propertiesDependencies = Properties.getDependencies(propertiesFile);

            ///========================================================================================================
            /// In case of APP type bundles
            ///========================================================================================================
            if (propertiesType.CompareTo("APP") == 0)
            {
                ///========================================================================================================
                /// Check that appxml and fwlauncher are in the Properties.cmake
                ///========================================================================================================
                bool findAppXml = false;
                bool findFwlauncher = false;
                foreach (String propertiesRequirement in propertiesRequirements)
                {
                    if (findAppXml && findFwlauncher)
                    {
                        break;
                    }
                    if (propertiesRequirement.CompareTo("appXml") == 0)
                    {
                        findAppXml = true;
                    }
                    else if (propertiesRequirement.CompareTo("fwlauncher") == 0)
                    {
                        findFwlauncher = true;
                    }
                }
                if (!findAppXml)
                {
                    Console.WriteLine("The bundle: `appXml` was not found in the file: `" + propertiesFile + "`");
                }
                if (!findFwlauncher)
                {
                    Console.WriteLine("The bundle: `fwlauncher` was not found in the file: `" + propertiesFile + "`");
                }

                ///========================================================================================================
                /// Check that bundle in xml file are properly started (requirement in xml files)
                ///========================================================================================================
                foreach (String propertiesRequirement in propertiesRequirements)
                {
                    /// List of bundle that require a starting
                    if (propertiesRequirement.CompareTo("validators") == 0 ||
                        propertiesRequirement.CompareTo("filterUnknownSeries") == 0 ||
                        propertiesRequirement.CompareTo("filterVRRender") == 0 ||
                        propertiesRequirement.CompareTo("activities") == 0 ||
                        propertiesRequirement.CompareTo("arDataReg") == 0 ||
                        propertiesRequirement.CompareTo("dataReg") == 0 ||
                        propertiesRequirement.CompareTo("memory") == 0 ||
                        propertiesRequirement.CompareTo("preferences") == 0 ||
                        propertiesRequirement.CompareTo("servicesReg") == 0 ||
                        propertiesRequirement.CompareTo("ioDicomWeb") == 0 ||
                        propertiesRequirement.CompareTo("ioPacs") == 0 ||
                        propertiesRequirement.CompareTo("arPatchMedicalData") == 0 ||
                        propertiesRequirement.CompareTo("patchMedicalData") == 0 ||
                        propertiesRequirement.CompareTo("console") == 0 ||
                        propertiesRequirement.CompareTo("guiQt") == 0 ||
                        propertiesRequirement.CompareTo("scene2D") == 0 ||
                        propertiesRequirement.CompareTo("visuOgre") == 0 ||
                        propertiesRequirement.CompareTo("material") == 0 ||
                        propertiesRequirement.CompareTo("visuVTKQml") == 0 ||
                        propertiesRequirement.CompareTo("visuVTKQt") == 0)
                    {
                        bool find = false;
                        foreach (Tuple<String, String> requirement in xmlRequirements)
                        {
                            if (propertiesRequirement.CompareTo(requirement.Item1) == 0)
                            {
                                find = true;
                                break;
                            }
                        }

                        if (!find)
                        {
                            Console.WriteLine("The bundle: `" + propertiesRequirement + "` need to be in the xml's requirements list");
                        }
                    }
                }
            }

            ///========================================================================================================
            /// Check that bundles used in xml files are in the Properties.cmake (REQUIREMENT)
            ///========================================================================================================
            foreach (Tuple<String, String> bundle in xmlBundles)
            {
                /// Check this special bundle, it's not in the requirement list, there are included by others bundles
                if (bundle.Item1.CompareTo(currentName) != 0 &&
                    bundle.Item1.CompareTo("fwServices") != 0 &&
                    bundle.Item1.CompareTo("fwRenderOgre") != 0 &&
                    bundle.Item1.CompareTo("fwRenderVTK") != 0 &&
                    bundle.Item1.CompareTo("fwActivities") != 0 &&
                    bundle.Item1.CompareTo("fwRenderQt") != 0)
                {
                    bool find = false;
                    foreach (String requirement in propertiesRequirements)
                    {
                        if (requirement.CompareTo(bundle.Item1) == 0)
                        {
                            find = true;
                            break;
                        }
                    }
                    if (!find)
                    {
                        Console.WriteLine("The bundle: `" + bundle.Item1 + "` from: `" + bundle.Item2 + "` was not found in the file: `" + propertiesFile + "`");
                    }
                }
            }

            ///========================================================================================================
            /// Check that library used in languages files are in the Properties.cmake (DEPENDENCIES)
            ///========================================================================================================
            foreach (Tuple<String, String> bundle in languagesBundles)
            {
                /// Skip external libraries and the current one
                if (bundle.Item1.CompareTo(currentName) != 0 &&
                    bundle.Item1.CompareTo("boost") != 0 &&
                    bundle.Item1.CompareTo("camp") != 0 &&
                    bundle.Item1.CompareTo("ceres") != 0 &&
                    bundle.Item1.CompareTo("cppunit") != 0 &&
                    bundle.Item1.CompareTo("dcmtk") != 0 &&
                    bundle.Item1.CompareTo("Eigen") != 0 &&
                    bundle.Item1.CompareTo("glm") != 0 &&
                    bundle.Item1.CompareTo("librealsense2") != 0 &&
                    bundle.Item1.CompareTo("libxml") != 0 &&
                    bundle.Item1.CompareTo("OGRE") != 0 &&
                    bundle.Item1.CompareTo("GL") != 0 &&
                    bundle.Item1.CompareTo("OpenGL") != 0 &&
                    bundle.Item1.CompareTo("opencv2") != 0 &&
                    bundle.Item1.CompareTo("OpenNI") != 0 &&
                    bundle.Item1.CompareTo("pcl") != 0 &&
                    bundle.Item1.CompareTo("vtk") != 0 &&
                    bundle.Item1.CompareTo("vlc") != 0 &&
                    bundle.Item1.CompareTo("IPPE") != 0 &&
                    bundle.Item1.CompareTo("cryptopp") != 0 &&
                    bundle.Item1.CompareTo("glog") != 0 &&
                    bundle.Item1.CompareTo("odil") != 0 &&
                    bundle.Item1.CompareTo("sofa") != 0 &&
                    bundle.Item1.CompareTo("tetgen") != 0 &&
                    bundle.Item1.CompareTo("trakSTAR") != 0 &&
                    bundle.Item1.CompareTo("BulletSoftBody") != 0 &&
                    bundle.Item1.CompareTo("sys") != 0 &&
                    bundle.Item1.CompareTo("grpc++") != 0 &&
                    bundle.Item1.CompareTo("pybind11") != 0 &&
                    bundle.Item1.CompareTo("itkhdf5") != 0)
                {
                    bool find = false;
                    foreach (String dependenci in propertiesDependencies)
                    {
                        if (dependenci.CompareTo(bundle.Item1) == 0)
                        {
                            find = true;
                            break;
                        }
                    }
                    if (!find)
                    {
                        Console.WriteLine("The library: `" + bundle.Item1 + "` from: `" + bundle.Item2 + "` was not found in the file: `" + propertiesFile + "`");
                    }
                }
            }

            ///========================================================================================================
            /// Check that bundles and libraries in the Properties.cmake are used
            ///========================================================================================================
            List<Tuple<String, String>> bundlesAndLibraries = xmlBundles;
            bundlesAndLibraries.AddRange(languagesBundles);

            List<String> requirementsAnDependencies = propertiesRequirements;
            requirementsAnDependencies.AddRange(propertiesDependencies);
            foreach (String requirementOrDependency in requirementsAnDependencies)
            {
                /// Skip 'IO' bundles, these bundles are used by SIOSelector
                /// Skip 'style' bundles, this bundle is used in a weird way and will be checked bellow
                /// Skip 'uiTF' bundles, this bundle contains files for pre-defined TF
                if (!(
                    (requirementOrDependency.CompareTo("style") == 0) ||
                    (requirementOrDependency.CompareTo("uiTF") == 0)
                    ))
                {
                    bool find = false;
                    foreach (Tuple<String, String> bundleOrLibrary in bundlesAndLibraries)
                    {
                        if (requirementOrDependency.CompareTo(bundleOrLibrary.Item1) == 0)
                        {
                            find = true;
                            break;
                        }
                    }
                    /// If the bundle is 'appXml' or 'fwlauncher', it can't be used.
                    /// These bundle must be here only in APP type bundles.
                    if (!((requirementOrDependency.CompareTo("appXml") == 0 || requirementOrDependency.CompareTo("fwlauncher") == 0) && propertiesType.CompareTo("APP") == 0))
                    {
                        if (!find)
                        {
                            Console.WriteLine("The library/bundle: `" + requirementOrDependency + "` is not used");
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
                String text = File.ReadAllText(propertiesFile);
                if (!text.Contains("style-0.1"))
                {
                    Console.WriteLine("The bundle: `style` is not used");
                }
            }

            ///========================================================================================================
            /// TODO, check IO bundles, video bundles, ioTF
            ///========================================================================================================

            ///========================================================================================================
            /// Check that all services are used
            ///========================================================================================================
            List<String> xmlFiles = Xml.getXMLFiles(Option.getDirectory());
            foreach (String file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                XmlNodeList serviceNodes = doc.DocumentElement.GetElementsByTagName("service");

                /// Retreive all services uid
                List<String> servicesUid = new List<String>();
                foreach (XmlNode serviceAtt in serviceNodes)
                {
                    if (serviceAtt.Attributes["uid"] != null)
                    {
                        servicesUid.Add(serviceAtt.Attributes["uid"].InnerText);
                    }
                }

                /// Check that each service is use in others services
                foreach (String serviceUid in servicesUid)
                {
                    Boolean find = false;
                    foreach (XmlNode serviceAtt in serviceNodes)
                    {
                        StringWriter sw = new StringWriter();
                        XmlTextWriter xw = new XmlTextWriter(sw);
                        xw.Formatting = Formatting.Indented;
                        serviceAtt.WriteTo(xw);
                        /// The first line contain the uid, we must remove it
                        String service = sw.ToString();
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
                                if (startAtt.Attributes["uid"].InnerText.CompareTo(serviceUid) == 0)
                                {
                                    find = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!find)
                    {
                        Console.WriteLine("The service: `" + serviceUid + "` is not used in the file '" + file + "'");
                    }
                }
            }

            ///========================================================================================================
            /// Check that all objects are used
            ///========================================================================================================
            foreach (String file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                XmlNodeList objectNodes = doc.DocumentElement.GetElementsByTagName("object");

                /// Retreive all objects uid
                List<String> objectsUid = new List<String>();
                foreach (XmlNode objectAtt in objectNodes)
                {
                    if (objectAtt.Attributes["uid"] != null)
                    {
                        objectsUid.Add(objectAtt.Attributes["uid"].InnerText);
                    }
                }

                /// Check that this object is used in a 'inout'
                foreach (String objectUid in objectsUid)
                {
                    Boolean find = false;
                    XmlNodeList inoutNodes = doc.DocumentElement.GetElementsByTagName("inout");
                    foreach (XmlNode inoutAtt in inoutNodes)
                    {
                        if (inoutAtt.Attributes["uid"] != null)
                        {
                            if (inoutAtt.Attributes["uid"].InnerText.CompareTo(objectUid) == 0)
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
                                if (inAtt.Attributes["uid"].InnerText.CompareTo(objectUid) == 0)
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
                                if (outAtt.Attributes["uid"].InnerText.CompareTo(objectUid) == 0)
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
                                if (outAtt.Attributes["uid"].InnerText.CompareTo(objectUid) == 0)
                                {
                                    find = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (!find)
                    {
                        Console.WriteLine("The object: `" + objectUid + "` is not used in the file '" + file + "'");
                    }
                }
            }

            ///========================================================================================================
            /// Check that all channel are used
            ///========================================================================================================
            foreach (String file in xmlFiles)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(file);

                XmlNodeList connectNodes = doc.DocumentElement.GetElementsByTagName("connect");

                String text = File.ReadAllText(file);

                /// Retreive all channel uid
                List<String> channelsUid = new List<String>();
                foreach (XmlNode connectAtt in connectNodes)
                {
                    if (connectAtt.Attributes["channel"] != null)
                    {
                        String channel = connectAtt.Attributes["channel"].InnerText;
                        /// Remove the reference, the channel can be used as parameter
                        channel = channel.Replace("${", "");
                        channel = channel.Replace("}", "");
                        /// If the channel is used only once, it's not used
                        if (Regex.Matches(text, channel).Count <= 1)
                        {
                            Console.WriteLine("The channel: `" + channel + "` is not used in the file '" + file + "'");
                        }
                    }
                }
            }
        }
    }
}
