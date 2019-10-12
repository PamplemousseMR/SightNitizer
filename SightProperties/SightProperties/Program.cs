using System;
using System.Collections.Generic;

namespace SightProperties
{
    class Program
    {
        static void Main(String[] _args)
        {
            /// Parse args
            if(!Option.parse(_args))
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
                if(!findAppXml)
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
                /// Check this special bundle, it's not in the requirement list, tere are include by others bundles
                if (bundle.Item1.CompareTo(currentName) != 0 &&
                    bundle.Item1.CompareTo("fwServices") != 0 &&
                    bundle.Item1.CompareTo("fwRenderOgre") != 0 &&
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
                    bundle.Item1.CompareTo("OGRE") != 0 &&
                    bundle.Item1.CompareTo("GL") != 0 &&
                    bundle.Item1.CompareTo("OpenGL") != 0 &&
                    bundle.Item1.CompareTo("glm") != 0 &&
                    bundle.Item1.CompareTo("cppunit") != 0 &&
                    bundle.Item1.CompareTo("opencv2") != 0 &&
                    bundle.Item1.CompareTo("ceres") != 0)
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
            foreach(String requirementOrDependency in requirementsAnDependencies)
            {
                /// Skip IO bundles, these bundles are used by SIOSelector
                if (!requirementOrDependency.StartsWith("io"))
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
                    /// If the bundle is appXml or fwlauncher, it can't be used.
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
        }
    }
}
