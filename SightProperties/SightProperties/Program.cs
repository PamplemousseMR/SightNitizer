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

            /// Get bundles list
            List<Tuple<String, String>> bundles = Bundle.getDefaultBundles(Option.getDirectory());
            bundles.AddRange(Bundle.getRequireBundles(Option.getDirectory()));
            bundles.AddRange(Bundle.getObjectsBundles(Option.getDirectory()));
            bundles.AddRange(Bundle.getOgreBundles(Option.getDirectory()));
            bundles.AddRange(Bundle.getVTKBundles(Option.getDirectory()));
            bundles.AddRange(Bundle.getStandardBundles(Option.getDirectory()));
            bundles.AddRange(Bundle.getMediaBundles(Option.getDirectory()));
            bundles.AddRange(Bundle.getIncludeBundles(Option.getDirectory()));

            /// Get Properties.cmake
            String properties = Option.getDirectory() + "\\Properties.cmake";
            /// Check properties.cmake
            CheckProperties.checkProperties(properties, bundles);

            /// Check bundles that need to be started
            List<String> requirements = CheckProperties.getBundles(properties);
            List<Tuple<String, String>> startedRequirements = Bundle.getRequireBundles(Option.getDirectory());
            foreach(String requirement in requirements)
            {
                if (requirement.CompareTo("validators") == 0 ||
                    requirement.CompareTo("filterUnknownSeries") == 0 ||
                    requirement.CompareTo("filterVRRender") == 0 ||
                    requirement.CompareTo("activities") == 0 ||
                    //requirement.CompareTo("appXml") == 0 ||
                    requirement.CompareTo("arDataReg") == 0 ||
                    requirement.CompareTo("dataReg") == 0 ||
                    requirement.CompareTo("memory") == 0 ||
                    requirement.CompareTo("preferences") == 0 ||
                    requirement.CompareTo("servicesReg") == 0 ||
                    requirement.CompareTo("ioDicomWeb") == 0 ||
                    requirement.CompareTo("ioPacs") == 0 ||
                    requirement.CompareTo("arPatchMedicalData") == 0 ||
                    requirement.CompareTo("patchMedicalData") == 0 ||
                    requirement.CompareTo("console") == 0 ||
                    requirement.CompareTo("guiQt") == 0 ||
                    requirement.CompareTo("scene2D") == 0 ||
                    requirement.CompareTo("visuOgre") == 0 ||
                    requirement.CompareTo("material") == 0 ||
                    requirement.CompareTo("visuVTKQml") == 0 ||
                    requirement.CompareTo("visuVTKQt") == 0)
                {
                    bool find = false;
                    foreach (Tuple<String, String> startedRequirement in startedRequirements)
                    {
                        if (requirement.CompareTo(startedRequirement.Item1) == 0)
                        {
                            find = true;
                            break;
                        }
                    }

                    if (!find)
                    {
                        Console.WriteLine("The bundle: `" + requirement + "` need to be in the xml's requirements list");
                    }
                }
            }
        }
    }
}
