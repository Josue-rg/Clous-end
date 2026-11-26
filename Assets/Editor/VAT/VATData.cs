using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ClousEnd.EditorTools
{
    // ------------------------------------------------------------------
    // Datos que exporta Blender en vat_<Asset>.json y lod_<Asset>.json.
    // Son la fuente de verdad de frames, fps, bounds y nombres de textura.
    // ------------------------------------------------------------------

    [Serializable]
    public sealed class VATClipData
    {
        public string name;
        public int frames;
        public float fps;
        public bool loop;
        public string texture;
        public int texture_width;
        public int texture_height;
        public int rows;
        public float[] bounds_min;
        public float[] bounds_max;
    }

    [Serializable]
    public sealed class VATAssetData
    {
        public string asset;
        public string mesh;
        public int triangles;
        public int vertices;
        public VATClipData[] clips;
    }

    [Serializable]
    public sealed class VATStaticData
    {
        public string asset;
        public string mesh;
        public int triangles;
        public int vertices;
        public bool animated;
    }

    [Serializable]
    public sealed class VATLodLevelData
    {
        public string level;
        public string mesh;
        public int triangles;
    }

    [Serializable]
    public sealed class VATCollisionData
    {
        public string mesh;
        public int triangles;
    }

    [Serializable]
    public sealed class VATLodData
    {
        public string asset;
        public int lod0_triangles;
        public VATLodLevelData[] levels;
        public VATCollisionData collision;
    }

    /// <summary>Rutas del paquete de Blender y de los assets generados en Unity.</summary>
    internal static class VATPaths
    {
        public const string ModelsRoot = "Assets/Art/Models/VAT";
        public const string StaticModelsRoot = ModelsRoot + "/Static";
        public const string GeneratedTexturesRoot = ModelsRoot + "/Generated";
        public const string MaterialsRoot = "Assets/Art/Materials/VAT";
        public const string ShaderRoot = "Assets/Shaders/VAT";
        public const string RuntimeRoot = "Assets/Scripts/Rendering";
        public const string PrefabsRoot = "Assets/Prefabs";
        public const string PreviewScenePath = "Assets/Scenes/VAT_Preview.unity";

        public const string SourceRoot = "BlenderSource/VAT";
        public const string SourceUnityPackage = "BlenderSource/UnityPackage";

        public static string ProjectRoot => Directory.GetParent(Application.dataPath).FullName;

        public static string SourceFull(string relative)
        {
            return Path.Combine(ProjectRoot, relative.Replace('/', Path.DirectorySeparatorChar));
        }

        public static string ToFull(string assetPath)
        {
            return Path.Combine(ProjectRoot, assetPath.Replace('/', Path.DirectorySeparatorChar));
        }

        public static string AssetPath(string fullPath)
        {
            var root = ProjectRoot.Replace('\\', '/') + "/";
            return fullPath.Replace('\\', '/').Replace(root, string.Empty);
        }

        public static void EnsureFolder(string assetFolder)
        {
            if (AssetDatabase.IsValidFolder(assetFolder))
                return;

            var parent = Path.GetDirectoryName(assetFolder)?.Replace('\\', '/');

            if (string.IsNullOrEmpty(parent))
                return;

            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(assetFolder));
        }
    }

    /// <summary>Decide a qué carpeta de prefabs pertenece cada asset.</summary>
    internal static class VATCatalog
    {
        private static readonly HashSet<string> Characters = new HashSet<string>
        {
            "Santa",
            "Sleigh",
            "Reindeer_Rodolfo",
            "Reindeer_Donner",
            "Reindeer_Vixen",
            "Reindeer_Cometa"
        };

        private static readonly HashSet<string> Enemies = new HashSet<string>
        {
            "Sabueso",
            "Garfio",
            "Espora",
            "Robot_Manto",
            "Robot_Torre",
            "NicolasCero",
            "Ship_Caza",
            "Ship_Recolector"
        };

        public static bool IsCharacter(string shortName) => Characters.Contains(shortName);

        public static bool IsEnemy(string shortName) => Enemies.Contains(shortName);

        public static bool IsPlayerRig(string shortName)
        {
            return shortName == "Santa" || shortName == "Sleigh" || shortName.StartsWith("Reindeer_");
        }

        public static string PrefabFolder(string shortName)
        {
            if (Characters.Contains(shortName))
                return VATPaths.PrefabsRoot + "/Characters";

            if (Enemies.Contains(shortName))
                return VATPaths.PrefabsRoot + "/Enemies";

            if (shortName.StartsWith("SM_Weapon_"))
                return VATPaths.PrefabsRoot + "/Weapons";

            return VATPaths.PrefabsRoot + "/Props";
        }

        public static string PrefabPath(string shortName)
        {
            return PrefabFolder(shortName) + "/PF_" + shortName + ".prefab";
        }

        public static GameObject LoadPrefab(string shortName)
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath(shortName));
        }

        public static bool Exists(string shortName)
        {
            return AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath(shortName)) != null;
        }
    }
}
