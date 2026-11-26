using System;
using System.Collections.Generic;
using System.IO;
using ClousEnd.Rendering;
using UnityEditor;
using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>
    /// Construye los materiales y los prefabs editables a partir de los FBX, los
    /// EXR y los JSON de Blender. Los prefabs llevan MeshRenderer, VATPlayer,
    /// LODGroup, collider y el clip por defecto ya conectado.
    /// </summary>
    internal static class VATPrefabBuilder
    {
        private const float MaxPoseHeightRatio = 5f;
        private const float MinPoseHeightRatio = 0.2f;

        private sealed class ModelInfo
        {
            public Mesh Mesh;
            public Vector3 RootScale = Vector3.one;
            public Quaternion RootRotation = Quaternion.identity;
            public Material[] SourceMaterials = Array.Empty<Material>();
        }

        public static void BuildAll()
        {
            var sourceRoot = VATPaths.SourceFull(VATPaths.SourceRoot);

            if (!Directory.Exists(sourceRoot))
            {
                Debug.LogError("[Clous-End VAT] No existe " + sourceRoot +
                               ". Ejecuta 'Importar paquete desde BlenderSource'.");
                return;
            }

            VATPaths.EnsureFolder(VATPaths.MaterialsRoot);
            EnsurePrefabFolders();

            var built = 0;
            var failed = 0;

            foreach (var folder in Directory.GetDirectories(sourceRoot))
            {
                var folderName = Path.GetFileName(folder);

                if (string.Equals(folderName, "Static", StringComparison.OrdinalIgnoreCase))
                    continue;

                try
                {
                    if (BuildAnimatedAsset(folder, folderName))
                        built++;
                    else
                        failed++;
                }
                catch (Exception exception)
                {
                    failed++;
                    Debug.LogError("[Clous-End VAT] Error construyendo " + folderName + ": " + exception);
                }
            }

            var staticRoot = Path.Combine(sourceRoot, "Static");

            if (Directory.Exists(staticRoot))
            {
                foreach (var folder in Directory.GetDirectories(staticRoot))
                {
                    try
                    {
                        if (BuildStaticAsset(folder))
                            built++;
                        else
                            failed++;
                    }
                    catch (Exception exception)
                    {
                        failed++;
                        Debug.LogError("[Clous-End VAT] Error construyendo " + Path.GetFileName(folder) + ": " + exception);
                    }
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[Clous-End VAT] Prefabs creados: " + built + ", omitidos: " + failed +
                      ". Revisa la consola si algún asset no encontró su FBX o su EXR.");
        }

        private static bool BuildAnimatedAsset(string folder, string folderName)
        {
            var jsonPath = FindJson(folder, "vat_*.json");

            if (jsonPath == null)
            {
                Debug.LogWarning("[Clous-End VAT] " + folderName + " no tiene vat_*.json; se omite.");
                return false;
            }

            var data = JsonUtility.FromJson<VATAssetData>(File.ReadAllText(jsonPath));

            if (data == null || string.IsNullOrEmpty(data.asset) || data.clips == null || data.clips.Length == 0)
            {
                Debug.LogWarning("[Clous-End VAT] " + jsonPath + " no tiene clips válidos; se omite.");
                return false;
            }

            var shortName = ShortName(data.asset);
            var fbxPath = VATPaths.ModelsRoot + "/" + folderName + "/" + data.mesh;

            if (!File.Exists(VATPaths.ToFull(fbxPath)))
                fbxPath = FirstModelIn(folder, folderName);

            var info = InspectModel(fbxPath);

            if (info == null)
            {
                Debug.LogError("[Clous-End VAT] No se pudo cargar el modelo de " + shortName + " (" + fbxPath + ").");
                return false;
            }

            var skeletonScale = ResolveRootScale(info, data.clips[0]);
            var sourceColors = ReadSourceColors(info.SourceMaterials);
            var subMeshCount = Mathf.Max(1, info.Mesh.subMeshCount);

            var maxFrames = 2;

            foreach (var clip in data.clips)
            {
                if (clip != null)
                    maxFrames = Mathf.Max(maxFrames, clip.frames);
            }

            var clipMaterials = new Dictionary<string, Material[]>();
            VATClipData defaultClip = null;

            foreach (var clip in data.clips)
            {
                if (clip == null || string.IsNullOrEmpty(clip.name))
                    continue;

                var texture = LoadClipTexture(folderName, clip);

                if (texture == null)
                {
                    Debug.LogError("[Clous-End VAT] Falta la textura " + clip.texture + " de " + shortName + ".");
                    return false;
                }

                clipMaterials[clip.name] = BuildClipMaterials(
                    shortName,
                    clip,
                    texture,
                    sourceColors,
                    subMeshCount,
                    maxFrames);

                if (clipMaterials[clip.name] == null)
                {
                    Debug.LogError("[Clous-End VAT] No se pudieron crear los materiales del clip " +
                                   clip.name + " de " + shortName + ".");
                    return false;
                }

                if (defaultClip == null)
                    defaultClip = clip;
            }

            if (defaultClip == null)
                return false;

            var firstClip = defaultClip;
            var clipSet = BuildClipSet(shortName, data.clips, folderName);
            var root = BuildAnimatedRoot(shortName, info, skeletonScale, clipMaterials[firstClip.name], out var lod0Renderer);
            var unitsPerMeter = 1f / Mathf.Max(0.0001f, Mathf.Abs(root.transform.localScale.z));
            var bounds = ConvertBounds(firstClip, unitsPerMeter);
            var player = root.AddComponent<VATPlayer>();

            player.Configure(
                firstClip.frames,
                firstClip.fps,
                firstClip.loop,
                !VATCatalog.IsPlayerRig(shortName),
                0f,
                bounds.center,
                bounds.extents);
            player.SetRenderer(lod0Renderer);
            player.SetClipSet(clipSet, firstClip.name);

            if (!VATCatalog.IsPlayerRig(shortName))
                AddLods(root, folder, folderName, shortName, info, sourceColors, lod0Renderer);

            if (!VATCatalog.IsPlayerRig(shortName))
                AddCollider(root, folderName, shortName, info);

            SavePrefab(root, shortName);
            return true;
        }

        private static bool BuildStaticAsset(string folder)
        {
            var jsonPath = FindJson(folder, "asset_*.json");

            if (jsonPath == null)
                return false;

            var data = JsonUtility.FromJson<VATStaticData>(File.ReadAllText(jsonPath));

            if (data == null || string.IsNullOrEmpty(data.asset))
                return false;

            var shortName = data.asset;
            var folderName = Path.GetFileName(folder);
            var fbxPath = VATPaths.StaticModelsRoot + "/" + folderName + "/" + data.mesh;

            if (!File.Exists(VATPaths.ToFull(fbxPath)))
                fbxPath = FirstModelIn(folder, folderName);

            var info = InspectModel(fbxPath);

            if (info == null)
            {
                Debug.LogError("[Clous-End VAT] No se pudo cargar el prop " + shortName + " (" + fbxPath + ").");
                return false;
            }

            var sourceColors = ReadSourceColors(info.SourceMaterials);
            var subMeshCount = Mathf.Max(1, info.Mesh.subMeshCount);
            var materials = new Material[subMeshCount];

            for (var i = 0; i < subMeshCount; i++)
            {
                var color = sourceColors[Mathf.Min(i, sourceColors.Length - 1)];
                var materialName = sourceColors.Length > 1
                    ? "M_" + shortName + "_" + i
                    : "M_" + shortName;

                materials[i] = VATMaterialFactory.GetOrCreateLit(materialName, color);

                if (materials[i] == null)
                    return false;
            }

            var root = BuildMeshRoot(shortName, info, info.RootScale, materials, true);

            var collider = root.AddComponent<BoxCollider>();
            var meshBounds = info.Mesh.bounds;
            collider.center = meshBounds.center;
            collider.size = meshBounds.size;

            SavePrefab(root, shortName);
            return true;
        }

        private static GameObject BuildAnimatedRoot(
            string shortName,
            ModelInfo info,
            Vector3 rootScale,
            Material[] materials,
            out MeshRenderer lod0Renderer)
        {
            var root = new GameObject(shortName);
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = info.RootRotation;
            root.transform.localScale = rootScale;

            var lod0 = new GameObject("LOD0");
            lod0.transform.SetParent(root.transform, false);

            var filter = lod0.AddComponent<MeshFilter>();
            filter.sharedMesh = info.Mesh;

            lod0Renderer = lod0.AddComponent<MeshRenderer>();
            lod0Renderer.sharedMaterials = materials;
            lod0Renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            lod0Renderer.receiveShadows = true;
            lod0Renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;

            return root;
        }

        private static GameObject BuildMeshRoot(
            string shortName,
            ModelInfo info,
            Vector3 rootScale,
            Material[] materials,
            bool isStatic)
        {
            var root = new GameObject(shortName);
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = info.RootRotation;
            root.transform.localScale = rootScale;

            var filter = root.AddComponent<MeshFilter>();
            filter.sharedMesh = info.Mesh;

            var renderer = root.AddComponent<MeshRenderer>();
            renderer.sharedMaterials = materials;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            renderer.receiveShadows = true;

            if (!isStatic)
                renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.BlendProbes;

            return root;
        }

        private static VATClipSet BuildClipSet(string shortName, VATClipData[] clips, string folderName)
        {
            var path = VATPaths.MaterialsRoot + "/VAT_" + shortName + ".asset";
            var set = AssetDatabase.LoadAssetAtPath<VATClipSet>(path);

            if (set == null)
            {
                set = ScriptableObject.CreateInstance<VATClipSet>();
                AssetDatabase.CreateAsset(set, path);
            }

            var entries = new List<VATClipEntry>();

            for (var i = 0; i < clips.Length; i++)
            {
                var clip = clips[i];

                if (clip == null || string.IsNullOrEmpty(clip.name))
                    continue;

                entries.Add(new VATClipEntry
                {
                    name = clip.name,
                    positions = LoadClipTexture(folderName, clip),
                    frames = Mathf.Max(2, clip.frames),
                    fps = Mathf.Max(0.01f, clip.fps),
                    loop = clip.loop
                });
            }

            set.clips = entries.ToArray();
            EditorUtility.SetDirty(set);
            return set;
        }

        private static Material[] BuildClipMaterials(            string shortName,
            VATClipData clip,
            Texture2D texture,
            Color[] sourceColors,
            int subMeshCount,
            int uvFrames)
        {
            var materials = new Material[subMeshCount];

            if (sourceColors.Length <= 1)
            {
                var single = VATMaterialFactory.GetOrCreateVat(
                    "M_" + shortName + "_" + clip.name,
                    sourceColors[0],
                    texture,
                    clip.frames,
                    uvFrames,
                    true);

                if (single == null)
                    return null;

                for (var i = 0; i < materials.Length; i++)
                    materials[i] = single;

                return materials;
            }

            for (var i = 0; i < subMeshCount; i++)
            {
                materials[i] = VATMaterialFactory.GetOrCreateVat(
                    "M_" + shortName + "_" + clip.name + "_" + i,
                    sourceColors[Mathf.Min(i, sourceColors.Length - 1)],
                    texture,
                    clip.frames,
                    uvFrames,
                    true);

                if (materials[i] == null)
                    return null;
            }

            return materials;
        }

        private static void AddLods(
            GameObject root,
            string sourceFolder,
            string folderName,
            string shortName,
            ModelInfo info,
            Color[] sourceColors,
            Renderer lod0Renderer)
        {
            var lodFolder = Path.Combine(sourceFolder, "LOD");
            var lodJsonPath = FindJson(lodFolder, "lod_*.json");

            if (lodJsonPath == null)
                return;

            var data = JsonUtility.FromJson<VATLodData>(File.ReadAllText(lodJsonPath));

            if (data == null || data.levels == null || data.levels.Length == 0)
                return;

            var lods = new List<LOD>();
            lods.Add(new LOD(0.05f, new[] { lod0Renderer }));

            var thresholds = new[] { 0.015f, 0.004f };
            var subMeshCount = Mathf.Max(1, info.Mesh.subMeshCount);

            for (var levelIndex = 0; levelIndex < data.levels.Length && levelIndex < thresholds.Length; levelIndex++)
            {
                var level = data.levels[levelIndex];
                var mesh = LoadMainMesh(VATPaths.ModelsRoot + "/" + folderName + "/LOD/" + level.mesh);

                if (mesh == null)
                {
                    Debug.LogWarning("[Clous-End VAT] " + shortName + ": no se encontró " + level.mesh + ".");
                    continue;
                }

                var lodGo = new GameObject(level.level);
                lodGo.transform.SetParent(root.transform, false);

                var filter = lodGo.AddComponent<MeshFilter>();
                filter.sharedMesh = mesh;

                var renderer = lodGo.AddComponent<MeshRenderer>();
                var lodMaterials = new Material[Mathf.Max(1, mesh.subMeshCount, subMeshCount)];

                for (var i = 0; i < lodMaterials.Length; i++)
                {
                    var color = sourceColors[Mathf.Min(i, sourceColors.Length - 1)];
                    var materialName = sourceColors.Length > 1
                        ? "M_" + shortName + "_LOD_" + i
                        : "M_" + shortName + "_LOD";

                    lodMaterials[i] = VATMaterialFactory.GetOrCreateVat(
                        materialName,
                        color,
                        null,
                        2,
                        0,
                        false);
                }

                renderer.sharedMaterials = lodMaterials;
                lods.Add(new LOD(thresholds[levelIndex], new Renderer[] { renderer }));
            }

            if (lods.Count < 2)
                return;

            var group = root.AddComponent<LODGroup>();
            group.fadeMode = LODFadeMode.None;
            group.animateCrossFading = false;
            group.SetLODs(lods.ToArray());
            group.RecalculateBounds();
        }

        private static void AddCollider(
            GameObject root,
            string folderName,
            string shortName,
            ModelInfo info)
        {
            var collisionPath = VATPaths.ModelsRoot + "/" + folderName + "/LOD/SK_" + shortName + "_COL.fbx";
            var collisionMesh = LoadMainMesh(collisionPath);

            if (collisionMesh != null)
            {
                var collider = root.AddComponent<MeshCollider>();
                collider.sharedMesh = collisionMesh;
                collider.convex = true;
                return;
            }

            // Sin malla _COL: una caja del tamaño de la malla evita que el
            // enemigo atraviese el corredor de vuelo mientras se añade el arte.
            var bounds = info.Mesh.bounds;
            var box = root.AddComponent<BoxCollider>();
            box.center = bounds.center;
            box.size = bounds.size;
        }

        private static void SavePrefab(GameObject root, string shortName)
        {
            var folder = VATCatalog.PrefabFolder(shortName);
            VATPaths.EnsureFolder(folder);
            PrefabUtility.SaveAsPrefabAsset(root, VATCatalog.PrefabPath(shortName));
            UnityEngine.Object.DestroyImmediate(root);
        }

        private static ModelInfo InspectModel(string fbxPath)
        {
            if (string.IsNullOrEmpty(fbxPath) || !File.Exists(VATPaths.ToFull(fbxPath)))
                return null;

            var model = AssetDatabase.LoadAssetAtPath<GameObject>(fbxPath);

            if (model == null)
                return null;

            var mesh = LoadMainMesh(fbxPath);

            if (mesh == null)
                return null;

            var totalVertices = 0;

            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(fbxPath))
            {
                if (asset is Mesh other)
                    totalVertices += other.vertexCount;
            }

            if (totalVertices > 0 && mesh.vertexCount < totalVertices * 0.9f)
            {
                Debug.LogWarning("[Clous-End VAT] " + fbxPath + " contiene varias mallas. " +
                                 "El prefab usa solo la mayor (" + mesh.name + "); si faltan piezas, " +
                                 "hay que unirlas en Blender antes de exportar.");
            }

            var materials = Array.Empty<Material>();

            foreach (var renderer in model.GetComponentsInChildren<MeshRenderer>(true))
            {
                var filter = renderer.GetComponent<MeshFilter>();

                if (filter == null || filter.sharedMesh != mesh)
                    continue;

                materials = renderer.sharedMaterials;
                break;
            }

            if (materials.Length == 0)
            {
                var renderer = model.GetComponentInChildren<MeshRenderer>(true);

                if (renderer != null)
                    materials = renderer.sharedMaterials;
            }

            return new ModelInfo
            {
                Mesh = mesh,
                RootScale = model.transform.localScale,
                RootRotation = model.transform.localRotation,
                SourceMaterials = materials
            };
        }

        private static Mesh LoadMainMesh(string fbxPath)
        {
            if (string.IsNullOrEmpty(fbxPath) || !File.Exists(VATPaths.ToFull(fbxPath)))
                return null;

            Mesh best = null;

            foreach (var asset in AssetDatabase.LoadAllAssetsAtPath(fbxPath))
            {
                if (!(asset is Mesh mesh))
                    continue;

                if (best == null || mesh.vertexCount > best.vertexCount)
                    best = mesh;
            }

            return best;
        }

        private static Color[] ReadSourceColors(Material[] sourceMaterials)
        {
            if (sourceMaterials == null || sourceMaterials.Length == 0)
                return new[] { new Color(0.72f, 0.72f, 0.74f, 1f) };

            var colors = new Color[sourceMaterials.Length];

            for (var i = 0; i < sourceMaterials.Length; i++)
                colors[i] = VATMaterialFactory.ReadSourceColor(sourceMaterials[i]);

            return colors;
        }

        private static Vector3 ResolveRootScale(ModelInfo info, VATClipData clip)
        {
            var scale = info.RootScale;

            if (clip == null || clip.bounds_min == null || clip.bounds_max == null ||
                clip.bounds_min.Length < 3 || clip.bounds_max.Length < 3)
                return scale;

            var jsonHeight = clip.bounds_max[2] - clip.bounds_min[2];
            var meshHeight = info.Mesh.bounds.size.z * Mathf.Abs(scale.z);

            if (jsonHeight <= 0.0001f || meshHeight <= 0.0001f)
                return scale;

            var ratio = jsonHeight / meshHeight;

            // La pose de reposo no tiene por qué medir lo mismo que la pose
            // animada. Solo corregimos diferencias de unidades (cm/pulgadas).
            if (ratio > MinPoseHeightRatio && ratio < MaxPoseHeightRatio)
                return scale;

            Debug.LogWarning("[Clous-End VAT] " + info.Mesh.name + ": la escala importada no coincide con el JSON (" +
                             meshHeight.ToString("0.00") + " m vs " + jsonHeight.ToString("0.00") +
                             " m). Se corrige por " + ratio.ToString("0.###") + ".");

            return new Vector3(scale.x * ratio, scale.y * ratio, scale.z * ratio);
        }

        private static Bounds ConvertBounds(VATClipData clip, float unitsPerMeter)
        {
            if (clip.bounds_min == null || clip.bounds_max == null ||
                clip.bounds_min.Length < 3 || clip.bounds_max.Length < 3)
            {
                return new Bounds(Vector3.zero, Vector3.one);
            }

            var min = clip.bounds_min;
            var max = clip.bounds_max;

            // El JSON está en coordenadas de Blender y la malla conserva esos
            // ejes (Z arriba). El prefab aplica la rotación de raíz del FBX, así
            // que los bounds locales se usan tal cual, sin cambiar de eje.
            var center = new Vector3(
                (min[0] + max[0]) * 0.5f,
                (min[1] + max[1]) * 0.5f,
                (min[2] + max[2]) * 0.5f) * unitsPerMeter;

            var extents = new Vector3(
                max[0] - min[0],
                max[1] - min[1],
                max[2] - min[2]) * (0.5f * unitsPerMeter);

            extents += Vector3.one * (0.1f * unitsPerMeter);
            return new Bounds(center, extents * 2f);
        }

        private static Texture2D LoadClipTexture(string folderName, VATClipData clip)
        {
            var path = VATPaths.ModelsRoot + "/" + folderName + "/" + clip.texture;
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private static string FindJson(string folder, string pattern)
        {
            if (!Directory.Exists(folder))
                return null;

            var files = Directory.GetFiles(folder, pattern);

            if (files.Length == 0 && pattern == "vat_*.json")
                files = Directory.GetFiles(folder, "*.json");

            return files.Length > 0 ? files[0] : null;
        }

        private static string FirstModelIn(string folder, string folderName)
        {
            foreach (var file in Directory.GetFiles(folder, "*.fbx"))
            {
                var name = Path.GetFileName(file);

                if (name.Contains("_LOD") || name.Contains("_COL"))
                    continue;

                return VATPaths.ModelsRoot + "/" + folderName + "/" + name;
            }

            return null;
        }

        private static string ShortName(string assetName)
        {
            return assetName.StartsWith("SK_", StringComparison.Ordinal)
                ? assetName.Substring(3)
                : assetName;
        }

        private static void EnsurePrefabFolders()
        {
            VATPaths.EnsureFolder(VATPaths.PrefabsRoot + "/Characters");
            VATPaths.EnsureFolder(VATPaths.PrefabsRoot + "/Enemies");
            VATPaths.EnsureFolder(VATPaths.PrefabsRoot + "/Weapons");
            VATPaths.EnsureFolder(VATPaths.PrefabsRoot + "/Props");
        }
    }
}
