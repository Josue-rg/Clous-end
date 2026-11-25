using System;
using System.IO;
using ClousEnd.Login;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.TextCore.LowLevel;

namespace ClousEnd.EditorTools
{
    internal sealed class LoginAssets
    {
        public TMP_FontAsset Body;
        public TMP_FontAsset BodyBold;
        public TMP_FontAsset Display;
        public Sprite LeftGradient;
        public Sprite Vignette;
        public Sprite Glow;
        public Sprite Arrow;
        public Sprite Spinner;
        public Sprite Dot;
        public Sprite Rounded;
        public Sprite White;
        public Material Sky;
        public Material SnowGround;
        public Material SnowRoof;
        public Material Plaster;
        public Material WoodDark;
        public Material WoodWarm;
        public Material Stone;
        public Material IcePath;
        public Material Mountain;
        public Material Pine;
        public Material WindowWarm;
        public Material LightWarm;
        public Material LightRed;
        public Material LightCool;
        public Material Moon;
        public Material Alien;
        public Material GiftRed;
        public Material GiftGreen;
        public Material Carrot;
        public Material SnowParticle;
        public Material WindowPool;
        public Material MoonGlow;
        public VolumeProfile Volume;
        public LoginConfig Config;
    }

    /// <summary>
    /// Crea una sola vez los assets del login (fuentes, materiales, sprites, perfil de postproceso).
    /// Si un asset ya existe, se reutiliza para no pisar los ajustes hechos a mano.
    /// </summary>
    internal static class LoginAssetFactory
    {
        private const string ArtRoot = "Assets/Art/Login";
        private const string TextureRoot = ArtRoot + "/Textures";
        private const string MaterialRoot = ArtRoot + "/Materials";
        private const string MeshRoot = ArtRoot + "/Meshes";
        private const string FontRoot = "Assets/Art/Fonts/TMP";
        private const string ConfigRoot = "Assets/Config";

        public const string SkyShaderName = "ClousEnd/Skybox";
        public const string SnowShaderName = "ClousEnd/Snowflake";

        public static LoginAssets Build()
        {
            EnsureFolders();
            EnsureTmpEssentials();

            var assets = new LoginAssets
            {
                Body = CreateFont("Assets/Art/Fonts/Source/Lato-Regular.ttf", FontRoot + "/Lato-Regular SDF.asset", "Lato-Regular SDF"),
                BodyBold = CreateFont("Assets/Art/Fonts/Source/Lato-Bold.ttf", FontRoot + "/Lato-Bold SDF.asset", "Lato-Bold SDF"),
                Display = CreateFont("Assets/Art/Fonts/Source/BebasNeue-Regular.ttf", FontRoot + "/BebasNeue SDF.asset", "BebasNeue SDF"),

                LeftGradient = CreateSprite("ui-left-gradient", 512, 8, GradientPixel),
                Vignette = CreateSprite("ui-vignette", 512, 512, VignettePixel),
                Glow = CreateSprite("ui-glow", 256, 256, GlowPixel),
                Arrow = CreateSprite("ui-arrow", 64, 64, ArrowPixel),
                Spinner = CreateSprite("ui-spinner", 64, 64, SpinnerPixel),
                Dot = CreateSprite("ui-dot", 32, 32, DotPixel),
                Rounded = CreateSprite("ui-rounded", 64, 64, RoundedPixel, new Vector4(16f, 16f, 16f, 16f)),
                White = CreateSprite("ui-white", 4, 4, (x, y) => Color.white),

                SnowGround = CreateMaterial("M_SnowGround", "Universal Render Pipeline/Lit", Hex("#D7E3F0"), 0.16f, 0f, Color.black, 0f),
                SnowRoof = CreateMaterial("M_SnowRoof", "Universal Render Pipeline/Lit", Hex("#EAF2FB"), 0.22f, 0f, Color.black, 0f),
                Plaster = CreateMaterial("M_Plaster", "Universal Render Pipeline/Lit", Hex("#BFAE96"), 0.2f, 0f, Color.black, 0f),
                WoodDark = CreateMaterial("M_WoodDark", "Universal Render Pipeline/Lit", Hex("#3A2A22"), 0.28f, 0f, Color.black, 0f),
                WoodWarm = CreateMaterial("M_WoodWarm", "Universal Render Pipeline/Lit", Hex("#6A4630"), 0.32f, 0f, Color.black, 0f),
                Stone = CreateMaterial("M_Stone", "Universal Render Pipeline/Lit", Hex("#3E4147"), 0.2f, 0f, Color.black, 0f),
                IcePath = CreateMaterial("M_IcePath", "Universal Render Pipeline/Lit", Hex("#7FA2B8"), 0.8f, 0.05f, Color.black, 0f),
                Mountain = CreateMaterial("M_Mountain", "Universal Render Pipeline/Lit", Hex("#536A88"), 0.08f, 0f, Color.black, 0f),
                Pine = CreateMaterial("M_Pine", "Universal Render Pipeline/Lit", Hex("#10231B"), 0.18f, 0f, Color.black, 0f),
                WindowWarm = CreateMaterial("M_WindowWarm", "Universal Render Pipeline/Lit", Hex("#2A1A0D"), 0.1f, 0f, Hex("#FFB15C"), 4.2f),
                LightWarm = CreateMaterial("M_LightWarm", "Universal Render Pipeline/Lit", Hex("#1A1008"), 0.1f, 0f, Hex("#FFC46A"), 5f),
                LightRed = CreateMaterial("M_LightRed", "Universal Render Pipeline/Lit", Hex("#1A0808"), 0.1f, 0f, Hex("#FF2E3C"), 5f),
                LightCool = CreateMaterial("M_LightCool", "Universal Render Pipeline/Lit", Hex("#081218"), 0.1f, 0f, Hex("#5FE3C8"), 4f),
                Moon = CreateMaterial("M_Moon", "Universal Render Pipeline/Lit", Hex("#DCE9FF"), 0.3f, 0f, Hex("#AFCBFF"), 2.6f),
                Alien = CreateMaterial("M_Alien", "Universal Render Pipeline/Lit", Hex("#05070C"), 0.5f, 0.35f, Color.black, 0f),
                GiftRed = CreateMaterial("M_GiftRed", "Universal Render Pipeline/Lit", Hex("#7A1020"), 0.35f, 0f, Color.black, 0f),
                GiftGreen = CreateMaterial("M_GiftGreen", "Universal Render Pipeline/Lit", Hex("#123A2A"), 0.35f, 0f, Color.black, 0f),
                Carrot = CreateMaterial("M_Carrot", "Universal Render Pipeline/Lit", Hex("#C56A2A"), 0.4f, 0f, Color.black, 0f),

                SnowParticle = CreateParticleMaterial(),
                Sky = CreateSkyMaterial()
            };

            if (assets.Display != null && assets.Body != null)
                AddFallback(assets.Display, assets.Body);

            if (assets.Glow != null)
            {
                assets.WindowPool = CreateAdditiveMaterial(
                    "M_WindowPool",
                    assets.Glow.texture,
                    new Color(1f, 0.55f, 0.25f, 0.55f));

                assets.MoonGlow = CreateAdditiveMaterial(
                    "M_MoonGlow",
                    assets.Glow.texture,
                    new Color(0.55f, 0.7f, 1f, 0.5f));
            }

            assets.Volume = CreateVolumeProfile(ArtRoot + "/LoginVolumeProfile.asset");
            assets.Config = CreateConfig(ConfigRoot + "/LoginConfig.asset");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return assets;
        }

        private static void EnsureFolders()
        {
            EnsureFolder(ArtRoot);
            EnsureFolder(TextureRoot);
            EnsureFolder(MaterialRoot);
            EnsureFolder(MeshRoot);
            EnsureFolder(FontRoot);
            EnsureFolder(ConfigRoot);
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;

            var parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            if (string.IsNullOrEmpty(parent)) return;

            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }

        private static void EnsureTmpEssentials()
        {
            if (AssetDatabase.FindAssets("t:TMP_Settings").Length > 0)
                return;

            const string packagePath = "Packages/com.unity.ugui/Package Resources/TMP Essential Resources.unitypackage";

            if (!File.Exists(packagePath))
            {
                Debug.LogError("[Clous-End] No se encontró TMP Essential Resources. Impórtalo desde Window > TextMeshPro.");
                return;
            }

#pragma warning disable 0618
            AssetDatabase.ImportPackage(packagePath, false);
#pragma warning restore 0618
            AssetDatabase.Refresh();
        }

        private static TMP_FontAsset CreateFont(string sourcePath, string assetPath, string assetName)
        {
            var existing = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
            if (existing != null)
                return existing;

            var font = AssetDatabase.LoadAssetAtPath<Font>(sourcePath);
            if (font == null)
            {
                Debug.LogError($"[Clous-End] No se encontró la fuente {sourcePath}.");
                return null;
            }

            var fontAsset = TMP_FontAsset.CreateFontAsset(
                font,
                90,
                9,
                GlyphRenderMode.SDFAA,
                1024,
                1024,
                AtlasPopulationMode.Dynamic,
                true);

            fontAsset.name = assetName;
            AssetDatabase.CreateAsset(fontAsset, assetPath);

            if (fontAsset.atlasTextures != null)
            {
                foreach (var atlas in fontAsset.atlasTextures)
                {
                    if (atlas == null) continue;
                    atlas.name = assetName + " Atlas";
                    AssetDatabase.AddObjectToAsset(atlas, fontAsset);
                }
            }

            if (fontAsset.material != null)
            {
                fontAsset.material.name = assetName + " Material";
                AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
            }

            EditorUtility.SetDirty(fontAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
            return AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
        }

        private static void AddFallback(TMP_FontAsset font, TMP_FontAsset fallback)
        {
            if (font == null || fallback == null) return;
            if (font == fallback) return;

            font.fallbackFontAssetTable ??= new System.Collections.Generic.List<TMP_FontAsset>();
            if (font.fallbackFontAssetTable.Contains(fallback)) return;

            font.fallbackFontAssetTable.Add(fallback);
            EditorUtility.SetDirty(font);
        }

        private static Sprite CreateSprite(
            string name,
            int width,
            int height,
            Func<int, int, Color> pixels,
            Vector4 border = default)
        {
            var path = TextureRoot + "/" + name + ".png";
            var existing = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            if (existing == null)
            {
                var texture = new Texture2D(width, height, TextureFormat.RGBA32, false, false);

                for (var y = 0; y < height; y++)
                {
                    for (var x = 0; x < width; x++)
                        texture.SetPixel(x, y, pixels(x, y));
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
                importer.maxTextureSize = 1024;
                importer.sRGBTexture = true;
                importer.spritePixelsPerUnit = 100f;

                var settings = new TextureImporterSettings();
                importer.ReadTextureSettings(settings);
                settings.spriteMeshType = SpriteMeshType.FullRect;
                importer.SetTextureSettings(settings);

                if (border != Vector4.zero)
                    importer.spriteBorder = border;

                importer.SaveAndReimport();
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static Color GradientPixel(int x, int y)
        {
            var t = x / 511f;
            var alpha = Mathf.Pow(1f - t, 1.35f);
            return new Color(0f, 0f, 0f, alpha);
        }

        private static Color VignettePixel(int x, int y)
        {
            var nx = (x / 511f - 0.5f) * 2f;
            var ny = (y / 511f - 0.5f) * 2f;
            var radius = Mathf.Clamp01(Mathf.Sqrt(nx * nx + ny * ny) / 1.414f);
            var alpha = Mathf.SmoothStep(0.28f, 1f, radius) * 0.9f;
            return new Color(0f, 0f, 0f, alpha);
        }

        private static Color GlowPixel(int x, int y)
        {
            var nx = (x / 255f - 0.5f) * 2f;
            var ny = (y / 255f - 0.5f) * 2f;
            var radius = Mathf.Clamp01(Mathf.Sqrt(nx * nx + ny * ny));
            var alpha = Mathf.Pow(1f - radius, 3.2f) * 0.9f;
            return new Color(1f, 1f, 1f, alpha);
        }

        private static Color ArrowPixel(int x, int y)
        {
            var px = x + 0.5f;
            var py = y + 0.5f;

            var inside = px >= 16f && px <= 48f && py >= 12f && py <= 52f
                && (px - 16f) / 32f <= 1f - Mathf.Abs(py - 32f) / 20f;

            return inside ? Color.white : Color.clear;
        }

        private static Color SpinnerPixel(int x, int y)
        {
            var nx = x / 63f - 0.5f;
            var ny = y / 63f - 0.5f;
            var radius = Mathf.Sqrt(nx * nx + ny * ny);
            var angle = Mathf.Atan2(ny, nx);

            var inRing = radius > 0.31f && radius < 0.42f;
            var inGap = angle > -0.35f && angle < 1.15f;

            return inRing && !inGap ? Color.white : Color.clear;
        }

        private static Color DotPixel(int x, int y)
        {
            var nx = x / 31f - 0.5f;
            var ny = y / 31f - 0.5f;
            var radius = Mathf.Sqrt(nx * nx + ny * ny) * 2f;
            var alpha = 1f - Mathf.SmoothStep(0.85f, 1f, radius);
            return new Color(1f, 1f, 1f, alpha);
        }

        private static Color RoundedPixel(int x, int y)
        {
            const float half = 31.5f;
            const float radius = 16f;

            var qx = Mathf.Abs(x - half) - (32f - radius);
            var qy = Mathf.Abs(y - half) - (32f - radius);
            var outside = new Vector2(Mathf.Max(qx, 0f), Mathf.Max(qy, 0f)).magnitude;
            var inside = Mathf.Min(Mathf.Max(qx, qy), 0f);
            var distance = outside + inside - radius;
            var alpha = 1f - Mathf.Clamp01(distance);

            return new Color(1f, 1f, 1f, alpha);
        }

        private static Material CreateMaterial(
            string name,
            string shaderName,
            Color baseColor,
            float smoothness,
            float metallic,
            Color emission,
            float emissionIntensity)
        {
            var path = MaterialRoot + "/" + name + ".mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
                return existing;

            var shader = Shader.Find(shaderName);
            if (shader == null)
            {
                Debug.LogError($"[Clous-End] No se encontró el shader {shaderName}.");
                return null;
            }

            var material = new Material(shader) { name = name };
            material.SetColor("_BaseColor", baseColor);
            material.SetColor("_Color", baseColor);
            material.SetFloat("_Smoothness", smoothness);
            material.SetFloat("_Metallic", metallic);
            material.enableInstancing = true;

            if (emissionIntensity > 0f)
            {
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", emission * emissionIntensity);
                material.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None;
            }

            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static Material CreateAdditiveMaterial(string name, Texture2D texture, Color color)
        {
            var path = MaterialRoot + "/" + name + ".mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
                return existing;

            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
            {
                Debug.LogError("[Clous-End] No se encontró el shader URP Unlit.");
                return null;
            }

            var material = new Material(shader) { name = name };
            material.SetTexture("_BaseMap", texture);
            material.SetColor("_BaseColor", color);
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_Blend", 2f);
            material.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.One);
            material.SetFloat("_ZWrite", 0f);
            material.SetFloat("_Cull", 2f);
            material.SetFloat("_AlphaClip", 0f);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetShaderPassEnabled("ShadowCaster", false);
            material.renderQueue = (int)RenderQueue.Transparent;

            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static Material CreateParticleMaterial()
        {
            var path = MaterialRoot + "/M_SnowParticle.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
                return existing;

            var shader = Shader.Find(SnowShaderName);
            if (shader == null)
            {
                Debug.LogError("[Clous-End] No se encontró el shader de copo de nieve.");
                return null;
            }

            var material = new Material(shader) { name = "M_SnowParticle" };
            material.SetColor("_Color", new Color(0.86f, 0.93f, 1f, 0.9f));
            material.SetFloat("_Softness", 0.45f);
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static Material CreateSkyMaterial()
        {
            var path = MaterialRoot + "/M_Sky.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
                return existing;

            var shader = Shader.Find(SkyShaderName);
            if (shader == null)
            {
                Debug.LogError("[Clous-End] No se encontró el shader de cielo.");
                return null;
            }

            var material = new Material(shader) { name = "M_Sky" };
            material.SetColor("_ZenithColor", new Color(0.012f, 0.028f, 0.075f));
            material.SetColor("_HorizonColor", new Color(0.055f, 0.11f, 0.19f));
            material.SetColor("_GroundColor", new Color(0.008f, 0.012f, 0.025f));
            material.SetColor("_AuroraA", new Color(0.10f, 0.85f, 0.62f));
            material.SetColor("_AuroraB", new Color(0.45f, 0.18f, 0.85f));
            material.SetFloat("_AuroraIntensity", 1.15f);
            material.SetFloat("_StarIntensity", 1.35f);
            material.SetFloat("_StarDensity", 0.995f);
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static VolumeProfile CreateVolumeProfile(string path)
        {
            var existing = AssetDatabase.LoadAssetAtPath<VolumeProfile>(path);
            if (existing != null)
                return existing;

            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            AssetDatabase.CreateAsset(profile, path);

            var bloom = profile.Add<Bloom>(true);
            bloom.active = true;
            bloom.intensity.Override(0.85f);
            bloom.threshold.Override(1f);
            bloom.scatter.Override(0.72f);
            bloom.tint.Override(new Color(1f, 0.9f, 0.8f));

            var vignette = profile.Add<Vignette>(true);
            vignette.active = true;
            vignette.intensity.Override(0.33f);
            vignette.smoothness.Override(0.45f);
            vignette.color.Override(new Color(0.005f, 0.01f, 0.02f));

            var tonemapping = profile.Add<Tonemapping>(true);
            tonemapping.active = true;
            tonemapping.mode.Override(TonemappingMode.ACES);

            var color = profile.Add<ColorAdjustments>(true);
            color.active = true;
            color.postExposure.Override(0.05f);
            color.contrast.Override(8f);
            color.saturation.Override(-4f);

            var grain = profile.Add<FilmGrain>(true);
            grain.active = true;
            grain.intensity.Override(0.12f);
            grain.response.Override(0.8f);
            grain.type.Override(FilmGrainLookup.Medium3);

            var whiteBalance = profile.Add<WhiteBalance>(true);
            whiteBalance.active = true;
            whiteBalance.temperature.Override(-8f);
            whiteBalance.tint.Override(6f);

            EditorUtility.SetDirty(profile);
            AssetDatabase.SaveAssets();
            return profile;
        }

        private static LoginConfig CreateConfig(string path)
        {
            var existing = AssetDatabase.LoadAssetAtPath<LoginConfig>(path);
            if (existing != null)
                return existing;

            var config = ScriptableObject.CreateInstance<LoginConfig>();
            AssetDatabase.CreateAsset(config, path);
            return config;
        }

        private static Color Hex(string hex)
        {
            return ColorUtility.TryParseHtmlString(hex, out var color) ? color : Color.magenta;
        }
    }
}
