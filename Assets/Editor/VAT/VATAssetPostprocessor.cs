using UnityEditor;
using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>
    /// Aplica los ajustes obligatorios del paquete VAT: texturas EXR sin filtro,
    /// sin compresión y en Point, y modelos FBX en metros, sin cámaras, luces ni
    /// animación de esqueleto. Si alguien reimporta a mano, estos valores vuelven.
    /// </summary>
    public sealed class VATAssetPostprocessor : AssetPostprocessor
    {
        private static bool IsVatAsset(string path)
        {
            return path.StartsWith(VATPaths.ModelsRoot + "/", System.StringComparison.Ordinal);
        }

        private void OnPreprocessTexture()
        {
            if (!IsVatAsset(assetPath))
                return;

            if (!assetPath.EndsWith("_Pos.exr", System.StringComparison.OrdinalIgnoreCase))
                return;

            var importer = (TextureImporter)assetImporter;

            importer.textureType = TextureImporterType.Default;
            importer.textureShape = TextureImporterShape.Texture2D;
            importer.sRGBTexture = false;
            importer.alphaSource = TextureImporterAlphaSource.None;
            importer.filterMode = FilterMode.Point;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.mipmapEnabled = false;
            importer.anisoLevel = 0;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.crunchedCompression = false;
            importer.npotScale = TextureImporterNPOTScale.None;
            importer.maxTextureSize = 2048;
            importer.isReadable = false;
            importer.streamingMipmaps = false;
        }

        private void OnPreprocessModel()
        {
            if (!IsVatAsset(assetPath))
                return;

            var importer = (ModelImporter)assetImporter;

            // Unidades: 1 unidad de Blender = 1 metro; el FBX trae su propia escala.
            importer.globalScale = 1f;
            importer.useFileScale = true;
            importer.bakeAxisConversion = false;

            // El FBX de Blender trae la conversión de ejes como rotación de raíz
            // (normalmente 90° en X). El constructor de prefabs copia esa rotación
            // al prefab, así que la malla y la animación quedan en Y arriba.

            importer.importCameras = false;
            importer.importLights = false;
            importer.importBlendShapes = false;
            importer.importVisibility = false;
            importer.importAnimation = false;

            importer.importNormals = ModelImporterNormals.Import;
            importer.importTangents = ModelImporterTangents.None;
            importer.meshCompression = ModelImporterMeshCompression.Off;

            // Los materiales del FBX se usan como fuente de color para los
            // materiales VAT y para los props estáticos.
            importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
            importer.materialLocation = ModelImporterMaterialLocation.InPrefab;
            importer.isReadable = false;
        }

        private void OnPostprocessModel(GameObject model)
        {
            if (!IsVatAsset(assetPath))
                return;

            // Los LOD y las colisiones no necesitan la capa UV del VAT.
            if (assetPath.Contains("_LOD") || assetPath.Contains("_COL"))
                return;

            // Los props estáticos no usan VAT.
            if (assetPath.StartsWith(VATPaths.StaticModelsRoot + "/", System.StringComparison.Ordinal))
                return;

            foreach (var filter in model.GetComponentsInChildren<MeshFilter>(true))
            {
                var mesh = filter.sharedMesh;

                if (mesh == null)
                    continue;

                if (mesh.uv2 == null || mesh.uv2.Length != mesh.vertexCount)
                {
                    Debug.LogError(
                        "[Clous-End VAT] " + assetPath + ": la malla no entró con la segunda capa UV " +
                        "(UV1/VAT). El shader leerá uv2 vacío y la animación no se verá. " +
                        "Revisa los import settings del FBX.");
                    return;
                }
            }
        }
    }
}
