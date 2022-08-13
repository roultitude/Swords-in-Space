
using FishNet.Configuring;
using System.IO;
using UnityEngine;
using System.Xml.Serialization;

#if UNITY_EDITOR
using FishNet.Editing.PrefabCollectionGenerator;
using UnityEditor.Compilation;
using UnityEditor.Build.Reporting;
using UnityEditor;
using UnityEditor.Build;
#endif

namespace FishNet.Configuring
{


    public class CodeStripping
    //PROSTART
#if UNITY_EDITOR
    : IPreprocessBuildWithReport, IPostprocessBuildWithReport
#endif
    //PROEND
    {

        /// <summary>
        /// True if making a release build for client.
        /// </summary>
        public static bool ReleasingForClient => (Configuration.ConfigurationData.IsBuilding && !Configuration.ConfigurationData.IsHeadless && !Configuration.ConfigurationData.IsDevelopment);
        /// <summary>
        /// True if making a release build for server.
        /// </summary>
        public static bool ReleasingForServer => (Configuration.ConfigurationData.IsBuilding && Configuration.ConfigurationData.IsHeadless && !Configuration.ConfigurationData.IsDevelopment);
        /// <summary>
        /// Returns if to remove server logic.
        /// </summary>
        /// <returns></returns>
        public static bool RemoveServerLogic
        {
            get
            {
                //PROSTART
                if (!StripBuild)
                    return false;
                //Cannot remove server code if headless.
                if (Configuration.ConfigurationData.IsHeadless)
                    return false;

                return true;
                //PROSTART

                /* This is to protect non pro users from enabling this
                 * without the extra logic code.  */
#pragma warning disable CS0162 // Unreachable code detected
                return false;
#pragma warning restore CS0162 // Unreachable code detected
            }
        }
        /// <summary>
        /// Returns if to remove server logic.
        /// </summary>
        /// <returns></returns>
        public static bool RemoveClientLogic
        {
            get
            {
                //PROSTART
                if (!StripBuild)
                    return false;
                //Cannot remove server code if headless.
                if (!Configuration.ConfigurationData.IsHeadless)
                    return false;

                return true;
                //PROEND

                /* This is to protect non pro users from enabling this
                 * without the extra logic code.  */
#pragma warning disable CS0162 // Unreachable code detected
                return false;
#pragma warning restore CS0162 // Unreachable code detected
            }
        }
        /// <summary>
        /// True if building and stripping is enabled.
        /// </summary>
        public static bool StripBuild
        {
            get
            {
                //PROSTART
                if (!Configuration.ConfigurationData.IsBuilding || Configuration.ConfigurationData.IsDevelopment)
                    return false;
                //Stripping isn't enabled.
                if (!Configuration.ConfigurationData.StripReleaseBuilds)
                    return false;

                //Fall through.
                return true;
                //PROEND

                /* This is to protect non pro users from enabling this
                 * without the extra logic code.  */
#pragma warning disable CS0162 // Unreachable code detected
                return false;
#pragma warning restore CS0162 // Unreachable code detected
            }
        }
        /// <summary>
        /// Technique to strip methods.
        /// </summary>
        public static StrippingTypes StrippingType => (StrippingTypes)Configuration.ConfigurationData.StrippingType;

        private static object _compilationContext;
        public int callbackOrder => 0;
#if UNITY_EDITOR

        public void OnPreprocessBuild(BuildReport report)
        {
            Generator.IgnorePostProcess = true;
            Generator.GenerateFull();
            CompilationPipeline.compilationStarted += CompilationPipelineOnCompilationStarted;
            CompilationPipeline.compilationFinished += CompilationPipelineOnCompilationFinished;

            //PROSTART
            //Set building values.
            Configuration.ConfigurationData.IsBuilding = true;

            BuildOptions options = report.summary.options;
#if UNITY_2021_2_OR_NEWER
            Configuration.ConfigurationData.IsHeadless = (report.summary.GetSubtarget<StandaloneBuildSubtarget>() == StandaloneBuildSubtarget.Server);
#else
            Configuration.ConfigurationData.IsHeadless = options.HasFlag(BuildOptions.EnableHeadlessMode);
#endif
            Configuration.ConfigurationData.IsDevelopment = options.HasFlag(BuildOptions.Development);

            //Write to file.
            Configuration.ConfigurationData.Write(false);
            //PROEND
        }
        /* Solution for builds ending with errors and not triggering OnPostprocessBuild.
        * Link: https://gamedev.stackexchange.com/questions/181611/custom-build-failure-callback
        */
        private void CompilationPipelineOnCompilationStarted(object compilationContext)
        {
            _compilationContext = compilationContext;
        }

        private void CompilationPipelineOnCompilationFinished(object compilationContext)
        {
            if (compilationContext != _compilationContext)
                return;

            _compilationContext = null;

            CompilationPipeline.compilationStarted -= CompilationPipelineOnCompilationStarted;
            CompilationPipeline.compilationFinished -= CompilationPipelineOnCompilationFinished;

            BuildingEnded();
        }

        private void BuildingEnded()
        {
            //PROSTART
            //Set building values.
            Configuration.ConfigurationData.IsBuilding = false;
            Configuration.ConfigurationData.IsHeadless = false;
            Configuration.ConfigurationData.IsDevelopment = false;
            //Write to file.
            Configuration.ConfigurationData.Write(false);
            //PROEND

            Generator.IgnorePostProcess = false;
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            //PROSTART
            if (Configuration.ConfigurationData.IsBuilding)
                //PROEND
                BuildingEnded();
        }
#endif
        }

}
