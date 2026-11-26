using System;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ClousEnd.EditorTools
{
    internal sealed class MapAssets
    {
        public Material Sky;
        public Material SnowParticle;

        public Material Snow;
        public Material SnowRoof;
        public Material Road;
        public Material RoadPaint;
        public Material Sidewalk;
        public Material Concrete;
        public Material Glass;
        public Material Metal;
        public Material Rubble;
        public Material Pine;
        public Material Mountain;
        public Material HistoricWood;

        public Material[] HistoricWalls;
        public Material[] TowerWalls;

        public Material WarmWindow;
        public Material CoolWindow;
        public Material AlienWindow;

        public Material LampWarm;
        public Material LampCool;
        public Material RefugeGlow;
        public Material AlienObsidian;
        public Material AlienVein;

        public Material AuroraCool;
        public Material AuroraWarm;
        public Material Tracer;
        public Material Impact;

        public Material CarPaint;
        public Material Character;
        public Material Gunmetal;

        public VolumeProfile Volume;
        public TMP_FontAsset BodyFont;
        public TMP_FontAsset BodyBoldFont;
        public TMP_FontAsset DisplayFont;
        public Sprite Crosshair;
        public Sprite Panel;
    }

    /// <summary>
    /// Crea (o actualiza) los materiales, el perfil de postproceso y los sprites
    /// de UI del mapa. Los materiales se actualizan en cada reconstrucción para
    /// que los cambios de paleta se vean sin borrar assets a mano.
    /// </summary>
    internal static class MapAssetFactory
    {
        private const string ArtRoot = "Assets/Art/Game";
        private const string MaterialRoot = ArtRoot + "/Materials";
        private const string TextureRoot = ArtRoot + "/Textures";
        private const string MeshRoot = ArtRoot + "/Meshes";
        private const string GeneratedRoot = MeshRoot + "/Generated";
        private const string LoginRoot = "Assets/Art/Login";

        public static MapAssets Build()
        {
            EnsureFolders();

            var assets = new MapAssets
            {
                Sky = LoadMaterial(LoginRoot + "/Materials/M_Sky.mat"),
                SnowParticle = LoadMaterial(LoginRoot + "/Materials/M_SnowParticle.mat"),

                Snow = Lit("M_Map_Snow", MapPalette.Snow, 0.22f, 0f),
                SnowRoof = Lit("M_Map_SnowRoof", MapPalette.SnowRoof, 0.28f, 0f),
                Road = Lit("M_Map_Road", MapPalette.Road, 0.32f, 0f),
                RoadPaint = Lit("M_Map_RoadPaint", MapPalette.RoadPaint, 0.24f, 0f, MapPalette.RoadPaint, 0.55f),
                Sidewalk = Lit("M_Map_Sidewalk", MapPalette.Sidewalk, 0.28f, 0f),
                Concrete = Lit("M_Map_Concrete", MapPalette.Concrete, 0.24f, 0f),
                Glass = Lit("M_Map_Glass", MapPalette.Glass, 0.85f, 0.55f),
                Metal = Lit("M_Map_Metal", MapPalette.Metal, 0.5f, 0.7f),
                Rubble = Lit("M_Map_Rubble", MapPalette.Rubble, 0.18f, 0f),
                Pine = Lit("M_Map_Pine", MapPalette.Hex("#10231B"), 0.16f, 0f),
                Mountain = Lit("M_Map_Mountain", MapPalette.Hex("#2B3A52"), 0.12f, 0f),
                HistoricWood = Lit("M_Map_HistoricWood", MapPalette.HistoricWood, 0.26f, 0f),

                WarmWindow = Unlit("M_Map_WarmWindow", MapPalette.WarmLight * 1.35f),
                CoolWindow = Unlit("M_Map_CoolWindow", MapPalette.CoolLight * 1.3f),
                AlienWindow = Unlit("M_Map_AlienWindow", MapPalette.AlienLight * 1.6f),

                LampWarm = Unlit("M_Map_LampWarm", MapPalette.WarmLight * 2.4f),
                LampCool = Unlit("M_Map_LampCool", MapPalette.CoolLight * 2.2f),
                RefugeGlow = Unlit("M_Map_RefugeGlow", MapPalette.RefugeLight * 2.2f),
                AlienObsidian = Lit("M_Map_AlienObsidian", MapPalette.AlienObsidian, 0.42f, 0.45f),
                AlienVein = Unlit("M_Map_AlienVein", MapPalette.AlienLight * 2.6f),

                CarPaint = Lit("M_Map_CarPaint", MapPalette.CarPaint, 0.62f, 0.35f),
                Character = Lit("M_Map_Character", MapPalette.Character, 0.24f, 0f),
                Gunmetal = Lit("M_Map_Gunmetal", MapPalette.Gunmetal, 0.52f, 0.65f),

                BodyFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/TMP/Lato-Regular SDF.asset"),
                BodyBoldFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/TMP/Lato-Bold SDF.asset"),
                DisplayFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Art/Fonts/TMP/BebasNeue SDF.asset"),
                Crosshair = CreateCrosshairSprite(),
                Panel = AssetDatabase.LoadAssetAtPath<Sprite>(LoginRoot + "/Textures/ui-white.png")
            };

            assets.AuroraCool = Additive("M_Map_AuroraCool", LoadTexture(LoginRoot + "/Textures/ui-glow.png"), new Color(0.3f, 1f, 0.78f, 0.5f));
            assets.AuroraWarm = Additive("M_Map_AuroraWarm", LoadTexture(LoginRoot + "/Textures/ui-glow.png"), new Color(0.7f, 0.3f, 1f, 0.42f));
            assets.Tracer = Unlit("M_Map_Tracer", new Color(4.2f, 3.1f, 1.6f, 1f));
            assets.Impact = Unlit("M_Map_Impact", new Color(6f, 4.2f, 2.4f, 1f));
            assets.HistoricWalls = BuildWallSet("M_Map_Historic_", MapPalette.HistoricWalls);
            assets.TowerWalls = BuildWallSet("M_Map_Tower_", MapPalette.TowerWalls);
            assets.Volume = CreateVolumeProfile(ArtRoot + "/MapVolumeProfile.asset");

            AssetDatabase.SaveAssets();
            return assets;
        }

        public static void ClearGeneratedMeshes()
        {
            if (AssetDatabase.IsValidFolder(GeneratedRoot))
                AssetDatabase.DeleteAsset(GeneratedRoot);

            EnsureFolder(GeneratedRoot);
        }

        private static Material[] BuildWallSet(string prefix, Color[] colors)
        {
            var materials = new Material[colors.Length];

            for (var i = 0; i < colors.Length; i++)
                materials[i] = Lit(prefix + (i + 1), colors[i], 0.24f, 0f);

            return materials;
        }

        private static void EnsureFolders()
        {
            EnsureFolder(ArtRoot);
            EnsureFolder(MaterialRoot);
            EnsureFolder(TextureRoot);
            EnsureFolder(MeshRoot);
            EnsureFolder(GeneratedRoot);
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return;

            var parent = Path.GetDirectoryName(path)?.Replace('\\', '/');

            if (string.IsNullOrEmpty(parent))
                return;

            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }

        private static Material LoadMaterial(string path)
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (material == null)
                Debug.LogWarning("[Clous-End] Falta el material " + path);

            return material;
        }

        private static Texture2D LoadTexture(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private static Material Lit(
            string name,
            Color baseColor,
            float smoothness,
            float metallic,
            Color? emission = null,
            float emissionIntensity = 0f)
        {
            var material = LoadOrCreateMaterial(name, "Universal Render Pipeline/Lit");

            if (material == null)
                return null;

            material.SetColor("_BaseColor", baseColor);
            material.SetColor("_Color", baseColor);
            material.SetFloat("_Smoothness", smoothness);
            material.SetFloat("_Metallic", metallic);
            material.enableInstancing = true;

            if (emission.HasValue && emissionIntensity > 0f)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", emission.Value * emissionIntensity);
                material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
                material.SetTexture("_EmissionMap", null);
            }
            else
            {
                material.DisableKeyword("_EMISSION");
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        private static Material Unlit(string name, Color hdrColor)
        {
            var material = LoadOrCreateMaterial(name, "Universal Render Pipeline/Unlit");

            if (material == null)
                return null;

            material.SetColor("_BaseColor", hdrColor);
            material.SetColor("_Color", hdrColor);
            EditorUtility.SetDirty(material);
            return material;
        }

        private static Material Additive(string name, Texture2D texture, Color color)
        {
            var material = LoadOrCreateMaterial(name, "Universal Render Pipeline/Unlit");

            if (material == null)
                return null;

            material.SetTexture("_BaseMap", texture);
            material.SetColor("_BaseColor", color);
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_Blend", 2f);
            material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            material.SetFloat("_DstBlend", (float)BlendMode.One);
            material.SetFloat("_ZWrite", 0f);
            material.SetFloat("_Cull", 2f);
            material.SetFloat("_AlphaClip", 0f);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetShaderPassEnabled("ShadowCaster", false);
            material.renderQueue = (int)RenderQueue.Transparent;
            material.enableInstancing = true;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static Material LoadOrCreateMaterial(string name, string shaderName)
        {
            var path = MaterialRoot + "/" + name + ".mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            var shader = Shader.Find(shaderName);

            if (shader == null)
            {
                Debug.LogError("[Clous-End] No se encontró el shader " + shaderName);
                return null;
            }

            if (existing != null)
            {
                if (existing.shader != shader)
                    existing.shader = shader;

                return existing;
            }

            var material = new Material(shader) { name = name };
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static Sprite CreateCrosshairSprite()
        {
            const string path = TextureRoot + "/ui-crosshair.png";
            var existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (existing == null)
            {
                const int size = 64;
                var texture = new Texture2D(size, size, TextureFormat.RGBA32, false, false);

                for (var y = 0; y < size; y++)
                {
                    for (var x = 0; x < size; x++)
                        texture.SetPixel(x, y, CrosshairPixel(x, y));
                }

                texture.Apply();
                File.WriteAllBytes(path, texture.EncodeToPNG());
                UnityEngine.Object.DestroyImmediate(texture);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            }

            var importer = AssetImporter.GetAtPath(path) as TextureImporter;

            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.mipmapEnabled = false;
                importer.filterMode = FilterMode.Bilinear;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.maxTextureSize = 128;
                importer.sRGBTexture = true;
                importer.spritePixelsPerUnit = 64f;

                var settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteMeshType = SpriteMeshType.FullRect;
                importer.SetTextureSettings(settings);
                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static Color CrosshairPixel(int x, int y)
        {
            var nx = x + 0.5f - 32f;
            var ny = y + 0.5f - 32f;
            var radius = Mathf.Sqrt(nx * nx + ny * ny);

            var ring = radius > 19f && radius < 21.5f;
            var dot = radius < 1.9f;
            var ticks = (Mathf.Abs(nx) < 1.4f && Mathf.Abs(ny) > 24f && Mathf.Abs(ny) < 31f)
                        || (Mathf.Abs(ny) < 1.4f && Mathf.Abs(nx) > 24f && Mathf.Abs(nx) < 31f);

            return ring || dot || ticks ? new Color(1f, 1f, 1f, 0.92f) : Color.clear;
        }

        private static VolumeProfile CreateVolumeProfile(string path)
        {
            if (AssetDatabase.LoadAssetAtPath<VolumeProfile>(path) != null)
                AssetDatabase.DeleteAsset(path);

            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(profile, path);

            var bloom = profile.Add<Bloom>(true);
            bloom.active = true;
            bloom.intensity.Override(0.75f);
            bloom.threshold.Override(1.05f);
            bloom.scatter.Override(0.6f);
            bloom.tint.Override(new Color(0.86f, 0.94f, 1f));

            var vignette = profile.Add<Vignette>(true);
            vignette.active = true;
            vignette.intensity.Override(0.26f);
            vignette.smoothness.Override(0.5f);
            vignette.color.Override(new Color(0.004f, 0.008f, 0.016f));

            var tonemapping = profile.Add<Tonemapping>(true);
            tonemapping.active = true;
            tonemapping.mode.Override(TonemappingMode.Neutral);

            var color = profile.Add<ColorAdjustments>(true);
            color.active = true;
            color.postExposure.Override(0.65f);
            color.contrast.Override(8f);
            color.saturation.Override(18f);

            var grain = profile.Add<FilmGrain>(true);
            grain.active = true;
            grain.intensity.Override(0.1f);
            grain.response.Override(0.8f);
            grain.type.Override(FilmGrainLookup.Medium3);

            var whiteBalance = profile.Add<WhiteBalance>(true);
            whiteBalance.active = true;
            whiteBalance.temperature.Override(-10f);
            whiteBalance.tint.Override(4f);

            EditorUtility.SetDirty(profile);
            AssetDatabase.SaveAssets();
            return profile;
        }
    }
}
