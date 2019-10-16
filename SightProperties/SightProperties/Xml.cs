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
        /// Get bundles related to qt
        /// </summary>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<String, String>> getQtBundles(String _rep)
        {
            List<String> xmlFiles = getXMLFiles(_rep);

            List<Tuple<String, String>> bundles = new List<Tuple<String, String>>();
            foreach (String file in xmlFiles)
            {
                String text = File.ReadAllText(file);
                if (text.Contains("::fwRenderQt::SRender"))
                {
                    bundles.Add(new Tuple<String, String>("scene2D", file));
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
                if (checkId(text, "2DNegato") ||
                    checkId(text, "2DNegatoWithTF") ||
                    checkId(text, "2DSimpleConfig") ||
                    checkId(text, "2DVisualization"))
                {
                    bundles.Add(new Tuple<String, String>("2DVisualizationActivity", file));
                }
                if (checkId(text, "3DNegatoWithAcq") ||
                    checkId(text, "3DSimpleConfig") ||
                    checkId(text, "3DVisualization"))
                {
                    bundles.Add(new Tuple<String, String>("3DVisualizationActivity", file));
                }
                if (checkId(text, "Blend") ||
                    checkId(text, "ImageMix") ||
                    checkId(text, "TransferFunctionEditor"))
                {
                    bundles.Add(new Tuple<String, String>("blendActivity", file));
                }
                if (checkId(text, "calCameraCharucoView") ||
                    checkId(text, "calExtrinsicCharucoView") ||
                    checkId(text, "calExtrinsicView") ||
                    checkId(text, "calibration") ||
                    checkId(text, "calibrationCharuco") ||
                    checkId(text, "calibrationEdition") ||
                    checkId(text, "calIntrinsicCharucoView") ||
                    checkId(text, "calIntrinsicView") ||
                    checkId(text, "displayImageConfig") ||
                    checkId(text, "displayTwoImagesConfig") ||
                    checkId(text, "rgbdCalibration") ||
                    checkId(text, "videoEdition"))
                {
                    bundles.Add(new Tuple<String, String>("calibrationActivity", file));
                }
                if (checkId(text, "2DLocalPreviewConfig"))
                {
                    bundles.Add(new Tuple<String, String>("dicomAppConfig", file));
                }
                if (checkId(text, "DicomFiltering") ||
                    checkId(text, "DicomPreview"))
                {
                    bundles.Add(new Tuple<String, String>("dicomFilteringActivity", file));
                }
                if (checkId(text, "2DPacsPreviewConfig") ||
                    checkId(text, "DicomPacsReader") ||
                    checkId(text, "PacsConfigurationManager"))
                {
                    bundles.Add(new Tuple<String, String>("dicomPacsReaderActivity", file));
                }
                if (checkId(text, "DicomPacsWriter"))
                {
                    bundles.Add(new Tuple<String, String>("dicomPacsWriterActivity", file));
                }
                if (checkId(text, "2DDicomWebPreviewConfig") ||
                    checkId(text, "DicomWebReaderActivity"))
                {
                    bundles.Add(new Tuple<String, String>("DicomWebReaderActivity", file));
                }
                if (checkId(text, "DicomWebWriterActivity"))
                {
                    bundles.Add(new Tuple<String, String>("DicomWebWriterActivity", file));
                }
                if (checkId(text, "ExportSelection") ||
                    checkId(text, "ExportDicomSelection"))
                {
                    bundles.Add(new Tuple<String, String>("ioActivity", file));
                }
                if (checkId(text, "BlendEditor") ||
                    checkId(text, "LandmarkRegistration") ||
                    checkId(text, "ManualRegistrationView") ||
                    checkId(text, "Registration") ||
                    checkId(text, "SliceBlendView"))
                {
                    bundles.Add(new Tuple<String, String>("registrationActivity", file));
                }
                if (checkId(text, "toolCalibration"))
                {
                    bundles.Add(new Tuple<String, String>("toolCalibrationActivity", file));
                }
                if (checkId(text, "rgbCameraView") ||
                    checkId(text, "trackedCameraRegistration"))
                {
                    bundles.Add(new Tuple<String, String>("trackedCameraRegistrationActivity", file));
                }
                if (checkId(text, "VolumeRendering"))
                {
                    bundles.Add(new Tuple<String, String>("volumeRenderingActivity", file));
                }
                if (checkId(text, "ImageManager") ||
                    checkId(text, "ModelSeriesManagerView") ||
                    checkId(text, "ModelSeriesManagerWindow"))
                {
                    bundles.Add(new Tuple<String, String>("dataManagerConfig", file));
                }
                if (checkId(text, "TransferFunctionWithNegatoEditor"))
                {
                    bundles.Add(new Tuple<String, String>("imageConfig", file));
                }
                if (checkId(text, "OgreHistogramManager") ||
                    checkId(text, "OgreLightManager") ||
                    checkId(text, "OgreOrganManager"))
                {
                    bundles.Add(new Tuple<String, String>("ogreConfig", file));
                }
                if (checkId(text, "TransferFunctionWidget"))
                {
                    bundles.Add(new Tuple<String, String>("qtSceneConfig", file));
                }
                if (checkId(text, "ActivityCreatorConfig"))
                {
                    bundles.Add(new Tuple<String, String>("uiMedDataQt", file));
                }
                if (checkId(text, "arOrbSlamVolumeRender"))
                {
                    bundles.Add(new Tuple<String, String>("arOrbSlamActivity", file));
                }
                if (checkId(text, "arseg") ||
                    checkId(text, "basicPCRegistrationActivity") ||
                    checkId(text, "basicVideoRegistration") ||
                    checkId(text, "poseAR") ||
                    checkId(text, "poseRegistration"))
                {
                    bundles.Add(new Tuple<String, String>("arsegActivity", file));
                }
                if (checkId(text, "biopsyAnalyzer") ||
                    checkId(text, "biopsyPdfExportConfig") ||
                    checkId(text, "biopsyRecorder"))
                {
                    bundles.Add(new Tuple<String, String>("biopsyActivity", file));
                }
                if (checkId(text, "breathingRegistrationConfig") ||
                    checkId(text, "breathingVolumeEqualizerConfig"))
                {
                    bundles.Add(new Tuple<String, String>("breathingRegistrationActivity", file));
                }
                if (checkId(text, "PreOpBreathing") ||
                    checkId(text, "PreOpBreathingCommon") ||
                    checkId(text, "PreOpBreathingGenericScene") ||
                    checkId(text, "preOpEchography"))
                {
                    bundles.Add(new Tuple<String, String>("breathingSimuActivity", file));
                }
                if (checkId(text, "PreOpBreathingGenericSceneOgre") ||
                    checkId(text, "PreOpBreathingOgre") ||
                    checkId(text, "visu3DOgre") ||
                    checkId(text, "visuVideoOgre"))
                {
                    bundles.Add(new Tuple<String, String>("breathingSimuOgreActivity", file));
                }
                if (checkId(text, "compareMeshesActivity"))
                {
                    bundles.Add(new Tuple<String, String>("compareMeshesActivity", file));
                }
                if (checkId(text, "diaSegmentation"))
                {
                    bundles.Add(new Tuple<String, String>("diaSegmentationActivity", file));
                }
                if (checkId(text, "probeCalibration"))
                {
                    bundles.Add(new Tuple<String, String>("echoCalibrationActivity", file));
                }
                if (checkId(text, "echoImageGenerator") ||
                    checkId(text, "imageFromUSCTRegistration"))
                {
                    bundles.Add(new Tuple<String, String>("echoImageGeneratorActivity", file));
                }
                if (checkId(text, "echography") ||
                    checkId(text, "PerOp") ||
                    checkId(text, "PerOpStereo") ||
                    checkId(text, "visu3D") ||
                    checkId(text, "visuVideo"))
                {
                    bundles.Add(new Tuple<String, String>("echographySimuActivity", file));
                }
                if (checkId(text, "camerasHandEyeCalibration") ||
                    checkId(text, "trackingHandEyeCalibration"))
                {
                    bundles.Add(new Tuple<String, String>("handEyeActivity", file));
                }
                if (checkId(text, "RDActivitySelector") ||
                    checkId(text, "RDActivitySequencer") ||
                    checkId(text, "RDSeriesSelector"))
                {
                    bundles.Add(new Tuple<String, String>("ioRDActivity", file));
                }
                if(checkId(text, "MarkerlessPatientScanning"))
                {
                    bundles.Add(new Tuple<String, String>("markerlessPatientScanningActivity", file));
                }
                if (checkId(text, "meshRegistrationActivity"))
                {
                    bundles.Add(new Tuple<String, String>("meshRegistrationActivity", file));
                }
                if (checkId(text, "needleCalibration"))
                {
                    bundles.Add(new Tuple<String, String>("needleCalibrationActivity", file));
                }
                if (checkId(text, "odilSCPActivity") ||
                    checkId(text, "odilSCUActivity"))
                {
                    bundles.Add(new Tuple<String, String>("odilActivity", file));
                }
                if (checkId(text, "orbSlam2DepthTracking") ||
                    checkId(text, "orbSlam2Tracking"))
                {
                    bundles.Add(new Tuple<String, String>("orbSlamActivity", file));
                }
                if (checkId(text, "PatientScanning"))
                {
                    bundles.Add(new Tuple<String, String>("patientScanningActivity", file));
                }
                if (checkId(text, "periSegmentation") ||
                    checkId(text, "segmentationToBreathing"))
                {
                    bundles.Add(new Tuple<String, String>("periSegmentationActivity", file));
                }
                if (checkId(text, "ProcessingConfig"))
                {
                    bundles.Add(new Tuple<String, String>("processingActivity", file));
                }
                if (checkId(text, "PrometeusConfig"))
                {
                    bundles.Add(new Tuple<String, String>("prometeusActivity", file));
                }
                if (checkId(text, "quadMeshActivity"))
                {
                    bundles.Add(new Tuple<String, String>("quadMeshActivity", file));
                }
                if (checkId(text, "rgbdRecording"))
                {
                    bundles.Add(new Tuple<String, String>("rgbdRecordingActivity", file));
                }
                if (checkId(text, "SeshatConfig"))
                {
                    bundles.Add(new Tuple<String, String>("seshatActivity", file));
                }
                if (checkId(text, "trackedCameraCalibration"))
                {
                    bundles.Add(new Tuple<String, String>("trackedCameraCalibrationActivity", file));
                }
                if (checkId(text, "volumicActivity"))
                {
                    bundles.Add(new Tuple<String, String>("volumicActivity", file));
                }
                if (checkId(text, "OgreIDVRManager"))
                {
                    bundles.Add(new Tuple<String, String>("configOgreEx", file));
                }
                if (checkId(text, "landmarkModelSeriesView") ||
                    checkId(text, "manualRegistrationView"))
                {
                    bundles.Add(new Tuple<String, String>("modelSeriesConfig", file));
                }
                if (checkId(text, "EraserTool") ||
                    checkId(text, "PencilTool"))
                {
                    bundles.Add(new Tuple<String, String>("opData", file));
                }
                if (checkId(text, "PropagationTool"))
                {
                    bundles.Add(new Tuple<String, String>("opITK", file));
                }
            }

            return bundles;
        }

        /// <summary>
        /// Get bundles related to serviceConfig
        /// </summary>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<String, String>> getServiceConfigBundles(String _rep)
        {
            List<String> xmlFiles = getXMLFiles(_rep);
            List<Tuple<String, String>> bundles = new List<Tuple<String, String>>();
            foreach (String file in xmlFiles)
            {
                String text = File.ReadAllText(file);
                if (checkId(text, "DicomWebReaderConfig"))
                {
                    bundles.Add(new Tuple<String, String>("DicomWebReaderActivity", file));
                }
                if (checkId(text, "InfraredRealsenseGrabberConfig"))
                {
                    bundles.Add(new Tuple<String, String>("calibrationActivity", file));
                }
                if (checkId(text, "IOSelectorServiceConfigVRRenderExtDicomSeriesDBReader") ||
                    checkId(text, "DicomReaderConfig"))
                {
                    bundles.Add(new Tuple<String, String>("dicomFilteringActivity", file));
                }
                if (checkId(text, "DicomPacsReaderConfig"))
                {
                    bundles.Add(new Tuple<String, String>("dicomPacsReaderActivity", file));
                }
                if (checkId(text, "ARSDBAtomsReaderIOSelectorConfig") ||
                   checkId(text, "ARSDBAtomsWriterIOSelectorConfig") ||
                   checkId(text, "ARFullSDBReaderIOSelectorConfig") ||
                   checkId(text, "ARActivityReaderConfig") ||
                   checkId(text, "ARActivityWriterConfig"))
                {
                    bundles.Add(new Tuple<String, String>("ioARActivity", file));
                }
                if (checkId(text, "SDBReaderIOSelectorConfig") ||
                   checkId(text, "SDBAtomsReaderIOSelectorConfig") ||
                   checkId(text, "SDBAtomsWriterIOSelectorConfig") ||
                   checkId(text, "FullSDBReaderIOSelectorConfig") ||
                   checkId(text, "MDAtomsConfig") ||
                   checkId(text, "ActivityMDAtomsConfig") ||
                   checkId(text, "ActivityReaderConfig") ||
                   checkId(text, "ActivityWriterConfig") ||
                   checkId(text, "LandmarksAtomsConfig"))
                {
                    bundles.Add(new Tuple<String, String>("ioActivity", file));
                }
                if (checkId(text, "BiopsyWriterConfig"))
                {
                    bundles.Add(new Tuple<String, String>("biopsyActivity", file));
                }
                if (checkId(text, "FrameByFrameOpenCVGrabberConfig"))
                {
                    bundles.Add(new Tuple<String, String>("handEyeActivity", file));
                }
                if (checkId(text, "RDSDBAtomsReaderIOSelectorConfig") ||
                   checkId(text, "RDSDBAtomsWriterIOSelectorConfig") ||
                   checkId(text, "RDFullSDBReaderIOSelectorConfig") ||
                   checkId(text, "RDActivityReaderConfig") ||
                   checkId(text, "RDActivityWriterConfig"))
                {
                    bundles.Add(new Tuple<String, String>("ioRDActivity", file));
                }
            }

            return bundles;
        }

        /// <summary>
        /// Get bundles related to activiies
        /// </summary>
        /// <returns>The list of require bundles</returns>
        public static List<Tuple<String, String>> getActivitiesBundles(String _rep)
        {
            List<String> xmlFiles = getXMLFiles(_rep);
            List<Tuple<String, String>> bundles = new List<Tuple<String, String>>();
            foreach (String file in xmlFiles)
            {
                String text = File.ReadAllText(file);
                if (checkId(text, "2DVisualization"))
                {
                    bundles.Add(new Tuple<String, String>("2DVisualizationActivity", file));
                }
                if (checkId(text, "3DVisualization"))
                {
                    bundles.Add(new Tuple<String, String>("3DVisualizationActivity", file));
                }
                if (checkId(text, "DicomWebReaderActivity"))
                {
                    bundles.Add(new Tuple<String, String>("DicomWebReaderActivity", file));
                }
                if (checkId(text, "Blend"))
                {
                    bundles.Add(new Tuple<String, String>("blendActivity", file));
                }
                if (checkId(text, "Calibration") ||
                   checkId(text, "calibrationEdition") ||
                   checkId(text, "RGBDCalibration") ||
                   checkId(text, "CalibrationCharuco"))
                {
                    bundles.Add(new Tuple<String, String>("calibrationActivity", file));
                }
                if (checkId(text, "DicomFiltering"))
                {
                    bundles.Add(new Tuple<String, String>("dicomFilteringActivity", file));
                }
                if (checkId(text, "DicomPacsReader"))
                {
                    bundles.Add(new Tuple<String, String>("dicomPacsReaderActivity", file));
                }
                if (checkId(text, "DicomPacsWriter"))
                {
                    bundles.Add(new Tuple<String, String>("dicomPacsWriterActivity", file));
                }
                if (checkId(text, "DicomWebWriterActivity"))
                {
                    bundles.Add(new Tuple<String, String>("DicomWebWriterActivity", file));
                }
                if (checkId(text, "ImageSeriesExport") ||
                    checkId(text, "ModelSeriesExport") ||
                    checkId(text, "DicomSegmentationSurfaceExport"))
                {
                    bundles.Add(new Tuple<String, String>("ioActivity", file));
                }
                if (checkId(text, "registrationActivity"))
                {
                    bundles.Add(new Tuple<String, String>("registrationActivity", file));
                }
                if (checkId(text, "toolCalibrationActivity"))
                {
                    bundles.Add(new Tuple<String, String>("toolCalibrationActivity", file));
                }
                if (checkId(text, "TrackedCameraRegistration"))
                {
                    bundles.Add(new Tuple<String, String>("trackedCameraRegistrationActivity", file));
                }
                if (checkId(text, "VolumeRendering"))
                {
                    bundles.Add(new Tuple<String, String>("volumeRenderingActivity", file));
                }
                if (checkId(text, "arOrbSlamVolumeRenderActivity"))
                {
                    bundles.Add(new Tuple<String, String>("arOrbSlamActivity", file));
                }
                if (checkId(text, "arseg") ||
                    checkId(text, "basicPCRegistrationActivity") ||
                    checkId(text, "basicVideoRegistrationActivity") ||
                    checkId(text, "poseRegistrationActivity") ||
                    checkId(text, "poseARActivity") ||
                    checkId(text, "poseARRGBActivity"))
                {
                    bundles.Add(new Tuple<String, String>("arsegActivity", file));
                }
                if (checkId(text, "biopsyRecorder") ||
                    checkId(text, "biopsyAnalyzer") ||
                    checkId(text, "biopsyPdfExportActivity"))
                {
                    bundles.Add(new Tuple<String, String>("biopsyActivity", file));
                }
                if (checkId(text, "breathingRegistrationActivity") ||
                    checkId(text, "breathingVolumeEqualizerConfig"))
                {
                    bundles.Add(new Tuple<String, String>("breathingRegistrationActivity", file));
                }
                if (checkId(text, "PreOpBreathing"))
                {
                    bundles.Add(new Tuple<String, String>("breathingSimuActivity", file));
                }
                if (checkId(text, "PreOpBreathingOgre") ||
                    checkId(text, "PerOpOgre") ||
                    checkId(text, "PerOpStereoOgre"))
                {
                    bundles.Add(new Tuple<String, String>("breathingSimuOgreActivity", file));
                }
                if (checkId(text, "compareMeshesActivity"))
                {
                    bundles.Add(new Tuple<String, String>("compareMeshesActivity", file));
                }
                if (checkId(text, "diaSegmentation"))
                {
                    bundles.Add(new Tuple<String, String>("diaSegmentationActivity", file));
                }
                if (checkId(text, "probeCalibration"))
                {
                    bundles.Add(new Tuple<String, String>("echoCalibrationActivity", file));
                }
                if (checkId(text, "echoImageGeneratorActivity") ||
                    checkId(text, "imageFromUSCTRegistration"))
                {
                    bundles.Add(new Tuple<String, String>("echoImageGeneratorActivity", file));
                }
                if (checkId(text, "PerOp") ||
                    checkId(text, "PerOpStereo"))
                {
                    bundles.Add(new Tuple<String, String>("echographySimuActivity", file));
                }
                if (checkId(text, "camerasHandEyeCalibration") ||
                    checkId(text, "trackingHandEyeCalibration"))
                {
                    bundles.Add(new Tuple<String, String>("handEyeActivity", file));
                }
                if (checkId(text, "RDSeriesSelector") ||
                    checkId(text, "RDActivitySelector"))
                {
                    bundles.Add(new Tuple<String, String>("ioRDActivity", file));
                }
                if (checkId(text, "MarkerlessPatientScanning"))
                {
                    bundles.Add(new Tuple<String, String>("markerlessPatientScanningActivity", file));
                }
                if (checkId(text, "meshRegistrationActivity"))
                {
                    bundles.Add(new Tuple<String, String>("meshRegistrationActivity", file));
                }
                if (checkId(text, "needleCalibration"))
                {
                    bundles.Add(new Tuple<String, String>("needleCalibrationActivity", file));
                }
                if (checkId(text, "odilSCPActivity") ||
                    checkId(text, "odilSCUActivity"))
                {
                    bundles.Add(new Tuple<String, String>("odilActivity", file));
                }
                if (checkId(text, "orbSlam2TrackingActivity") ||
                    checkId(text, "orbSlam2DepthTrackingActivity"))
                {
                    bundles.Add(new Tuple<String, String>("orbSlamActivity", file));
                }
                if (checkId(text, "PatientScanning"))
                {
                    bundles.Add(new Tuple<String, String>("patientScanningActivity", file));
                }
                if (checkId(text, "periSegmentation") ||
                    checkId(text, "segmentationToBreathing"))
                {
                    bundles.Add(new Tuple<String, String>("periSegmentationActivity", file));
                }
                if (checkId(text, "Processing"))
                {
                    bundles.Add(new Tuple<String, String>("processingActivity", file));
                }
                if (checkId(text, "prometeusActivity"))
                {
                    bundles.Add(new Tuple<String, String>("prometeusActivity", file));
                }
                if (checkId(text, "quadMeshActivity"))
                {
                    bundles.Add(new Tuple<String, String>("quadMeshActivity", file));
                }
                if (checkId(text, "rgbdRecording"))
                {
                    bundles.Add(new Tuple<String, String>("rgbdRecordingActivity", file));
                }
                if (checkId(text, "SeshatActivity"))
                {
                    bundles.Add(new Tuple<String, String>("seshatActivity", file));
                }
                if (checkId(text, "trackedCameraCalibration"))
                {
                    bundles.Add(new Tuple<String, String>("trackedCameraCalibrationActivity", file));
                }
                if (checkId(text, "volumicActivity"))
                {
                    bundles.Add(new Tuple<String, String>("volumicActivity", file));
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
        public static List<String> getXMLFiles(String _rep)
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
            XmlNodeList configNodes = _doc.DocumentElement.GetElementsByTagName("config");
            foreach (XmlNode configAtt in configNodes)
            {
                if (configAtt.Attributes["service"] != null)
                {
                    services.Add(configAtt.Attributes["service"].InnerText);
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

        /// <summary>
        /// Check if a serviceDonfig, appConfig or activity is used
        /// </summary>
        /// <param name="_text">The file as text</param>
        /// <param name="_keyword">The keyword to find</param>
        /// <returns>True if the keyword is founded</returns>
        private static bool checkId(String _text, String _keyword)
        {
            return (_text.Contains("id=\"" + _keyword + "\"") || _text.Contains("<id>" + _keyword + "</id>"));
        }
    }
}
