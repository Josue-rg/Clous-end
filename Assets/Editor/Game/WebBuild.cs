using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>
    /// Compila la demo WebGL para GitHub Pages: Login + Game, Gzip con
    /// decompression fallback, data caching y lienzo 1600×900.
    /// </summary>
    public static class WebBuild
    {
        private const string OutputFolder = "Builds/WebGL";

        [MenuItem("Clous-End/Build/WebGL para GitHub Pages", false, 60)]
        public static void BuildFromMenu()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
            {
                EditorUtility.DisplayDialog(
                    "Cambia a WebGL",
                    "El target activo es " + EditorUserBuildSettings.activeBuildTarget +
                    ".\n\nCambia a WebGL en File > Build Profiles y vuelve a ejecutar el menú.",
                    "Entendido");
                return;
            }

            Build();
        }

        /// <summary>Punto de entrada para batch: -executeMethod ClousEnd.EditorTools.WebBuild.Build</summary>
        public static void Build()
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
            {
                Debug.LogError("[WEB] Active build target is " + EditorUserBuildSettings.activeBuildTarget + ", expected WebGL.");
                EditorApplication.Exit(1);
                return;
            }

            PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
            PlayerSettings.WebGL.decompressionFallback = true;
            PlayerSettings.WebGL.dataCaching = true;
            PlayerSettings.defaultWebScreenWidth = 1600;
            PlayerSettings.defaultWebScreenHeight = 900;

            var scenes = new[] { "Assets/Scenes/Login.unity", "Assets/Scenes/Game.unity" };

            foreach (var scene in scenes)
            {
                if (!File.Exists(scene))
                {
                    Debug.LogError("[WEB] Missing scene " + scene);
                    EditorApplication.Exit(1);
                    return;
                }
            }

            var output = Path.Combine(VATPaths.ProjectRoot, OutputFolder);

            if (Directory.Exists(output))
                Directory.Delete(output, true);

            Directory.CreateDirectory(output);

            var options = new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = output,
                target = BuildTarget.WebGL,
                options = BuildOptions.None
            };

            var report = BuildPipeline.BuildPlayer(options);
            var summary = report.summary;

            Debug.Log("[WEB] result=" + summary.result + " size=" + summary.totalSize +
                      " errors=" + summary.totalErrors + " output=" + output);

            if (summary.result != BuildResult.Succeeded)
                EditorApplication.Exit(1);
        }
    }
}
