using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>
    /// Copia el paquete de Blender (<c>BlenderSource/VAT</c>) a <c>Assets</c> y
    /// fuerza la reimportación para que el postprocesador aplique los ajustes de
    /// texturas y modelos. Es idempotente: se puede ejecutar tras reexportar.
    /// </summary>
    internal static class VATPackageImporter
    {
        private static readonly string[] ModelExtensions = { ".fbx" };
        private static readonly string[] TextureExtensions = { ".exr" };
        private static readonly string[] JsonExtensions = { ".json" };

        public static bool Import()
        {
            var sourceRoot = VATPaths.SourceFull(VATPaths.SourceRoot);

            if (!Directory.Exists(sourceRoot))
            {
                Debug.LogError("[Clous-End VAT] No existe la carpeta " + sourceRoot);
                return false;
            }

            var copied = new List<string>();

            foreach (var folder in Directory.GetDirectories(sourceRoot))
            {
                var name = Path.GetFileName(folder);

                if (string.Equals(name, "Static", StringComparison.OrdinalIgnoreCase))
                    continue;

                CopyAnimatedAsset(folder, VATPaths.ModelsRoot + "/" + name, copied);
            }

            var staticRoot = Path.Combine(sourceRoot, "Static");

            if (Directory.Exists(staticRoot))
            {
                foreach (var folder in Directory.GetDirectories(staticRoot))
                {
                    var name = Path.GetFileName(folder);
                    CopyStaticAsset(folder, VATPaths.StaticModelsRoot + "/" + name, copied);
                }
            }

            CopyRuntimeFile("VAT_Unlit.shader", VATPaths.ShaderRoot, copied);
            CopyRuntimeFile("FloatWobble.shader", VATPaths.ShaderRoot, copied);
            CopyRuntimeFile("VATPlayer.cs", VATPaths.RuntimeRoot, copied);
            CopyRuntimeFile("FloatPhase.cs", VATPaths.RuntimeRoot, copied);

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            // Refresco explícito: garantiza que el AssetPostprocessor se aplica
            // aunque los archivos ya existieran con otros ajustes.
            foreach (var path in copied)
            {
                if (!path.EndsWith(".fbx", StringComparison.OrdinalIgnoreCase) &&
                    !path.EndsWith(".exr", StringComparison.OrdinalIgnoreCase) &&
                    !path.EndsWith(".shader", StringComparison.OrdinalIgnoreCase))
                    continue;

                AssetDatabase.ImportAsset(
                    path,
                    ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("[Clous-End VAT] Paquete importado. Archivos: " + copied.Count +
                      ". Modelos en " + VATPaths.ModelsRoot + ".");
            return true;
        }

        private static void CopyAnimatedAsset(string sourceFolder, string destinationFolder, List<string> copied)
        {
            VATPaths.EnsureFolder(destinationFolder);

            CopyFiles(sourceFolder, destinationFolder, copied, ModelExtensions);
            CopyFiles(sourceFolder, destinationFolder, copied, TextureExtensions);
            CopyFiles(sourceFolder, destinationFolder, copied, JsonExtensions);

            var lodFolder = Path.Combine(sourceFolder, "LOD");

            if (!Directory.Exists(lodFolder))
                return;

            var lodDestination = destinationFolder + "/LOD";
            VATPaths.EnsureFolder(lodDestination);
            CopyFiles(lodFolder, lodDestination, copied, ModelExtensions);
            CopyFiles(lodFolder, lodDestination, copied, JsonExtensions);
        }

        private static void CopyStaticAsset(string sourceFolder, string destinationFolder, List<string> copied)
        {
            VATPaths.EnsureFolder(destinationFolder);
            CopyFiles(sourceFolder, destinationFolder, copied, ModelExtensions);
            CopyFiles(sourceFolder, destinationFolder, copied, JsonExtensions);
        }

        private static void CopyFiles(
            string sourceFolder,
            string destinationFolder,
            List<string> copied,
            string[] extensions)
        {
            foreach (var extension in extensions)
            {
                foreach (var file in Directory.GetFiles(sourceFolder, "*" + extension))
                {
                    var fileName = Path.GetFileName(file);
                    var destination = VATPaths.ToFull(destinationFolder + "/" + fileName);
                    File.Copy(file, destination, true);
                    copied.Add(destinationFolder + "/" + fileName);
                }
            }
        }

        private static void CopyRuntimeFile(string fileName, string destinationFolder, List<string> copied)
        {
            var source = VATPaths.SourceFull(VATPaths.SourceUnityPackage + "/" + fileName);

            if (!File.Exists(source))
                return;

            VATPaths.EnsureFolder(destinationFolder);

            var destinationAsset = destinationFolder + "/" + fileName;

            if (File.Exists(VATPaths.ToFull(destinationAsset)))
            {
                Debug.Log("[Clous-End VAT] " + destinationAsset + " ya existe; no se sobrescribe.");
                return;
            }

            File.Copy(source, VATPaths.ToFull(destinationAsset), false);
            copied.Add(destinationAsset);
        }
    }
}
