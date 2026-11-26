using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>
    /// Genera las texturas VAT como assets RGBAHalf nativos de Unity a partir
    /// del volcado crudo que exporta Blender (export_vat_raw.py). El importador
    /// de EXR de Unity transforma los valores de posición, así que este paso lo
    /// evita y conserva los floats originales.
    /// </summary>
    public static class VATTextureBuilder
    {
        private const string RawRoot = "BlenderSource/VATRaw";

        [MenuItem("Clous-End/VAT/6 · Generar texturas VAT (float)", false, 47)]
        public static void Build()
        {
            var modelsRoot = VATPaths.ToFull(VATPaths.ModelsRoot);

            if (!Directory.Exists(modelsRoot))
            {
                Debug.LogWarning("[Clous-End VAT] No existe " + VATPaths.ModelsRoot +
                                 ". Importa primero el paquete desde BlenderSource.");
                return;
            }

            VATPaths.EnsureFolder(VATPaths.GeneratedTexturesRoot);

            var exrPaths = Directory.GetFiles(modelsRoot, "T_*_Pos.exr", SearchOption.AllDirectories);
            var generated = new Dictionary<string, Texture2D>();
            var created = 0;
            var skipped = 0;

            foreach (var exrPath in exrPaths)
            {
                var assetFolder = Path.GetFileName(Path.GetDirectoryName(exrPath));
                var name = Path.GetFileNameWithoutExtension(exrPath);
                var rawPath = Path.Combine(VATPaths.ProjectRoot, RawRoot, assetFolder, name + ".rgba32f");

                if (!File.Exists(rawPath))
                {
                    Debug.LogWarning("[Clous-End VAT] Falta el volcado crudo " + rawPath +
                                     ". Ejecuta export_vat_raw.py en Blender.");
                    skipped++;
                    continue;
                }

                var bytes = File.ReadAllBytes(rawPath);

                if (bytes.Length < 8)
                {
                    Debug.LogWarning("[Clous-End VAT] Volcado inválido: " + rawPath);
                    skipped++;
                    continue;
                }

                var width = System.BitConverter.ToInt32(bytes, 0);
                var height = System.BitConverter.ToInt32(bytes, 4);
                var expected = 8 + width * height * 16;

                if (width <= 0 || height <= 0 || bytes.Length < expected)
                {
                    Debug.LogWarning("[Clous-End VAT] Tamaño inesperado en " + rawPath);
                    skipped++;
                    continue;
                }

                var colors = new Color[width * height];

                for (var i = 0; i < colors.Length; i++)
                {
                    var offset = 8 + i * 16;
                    colors[i] = new Color(
                        System.BitConverter.ToSingle(bytes, offset),
                        System.BitConverter.ToSingle(bytes, offset + 4),
                        System.BitConverter.ToSingle(bytes, offset + 8),
                        System.BitConverter.ToSingle(bytes, offset + 12));
                }

                var assetPath = VATPaths.GeneratedTexturesRoot + "/" + name + ".asset";

                if (AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath) != null)
                    AssetDatabase.DeleteAsset(assetPath);

                var texture = new Texture2D(width, height, TextureFormat.RGBAHalf, false, true)
                {
                    name = name,
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Clamp,
                    anisoLevel = 0
                };

                texture.SetPixels(colors);
                texture.Apply(false, false);
                AssetDatabase.CreateAsset(texture, assetPath);
                generated[name] = texture;
                created++;
            }

            AssetDatabase.SaveAssets();
            UpdateMaterials(generated);
            AssetDatabase.SaveAssets();

            Debug.Log("[Clous-End VAT] Texturas VAT generadas: " + created +
                      " (omitidas: " + skipped + "). Materiales actualizados.");
        }

        private static void UpdateMaterials(Dictionary<string, Texture2D> generated)
        {
            if (generated.Count == 0)
                return;

            var guids = AssetDatabase.FindAssets("t:Material", new[] { VATPaths.MaterialsRoot });
            var updated = 0;

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var material = AssetDatabase.LoadAssetAtPath<Material>(path);

                if (material == null)
                    continue;

                var current = material.GetTexture("_PosTex");

                if (current == null || !generated.TryGetValue(current.name, out var texture))
                    continue;

                material.SetTexture("_PosTex", texture);
                EditorUtility.SetDirty(material);
                updated++;
            }

            Debug.Log("[Clous-End VAT] Materiales con textura float: " + updated + ".");
        }
    }
}
