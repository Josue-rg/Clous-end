using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ClousEnd.EditorTools
{
    /// <summary>
    /// Funde todos los props estáticos de una zona por material. Pasan de cientos
    /// de renderers a una decena de draw calls; los colliders se conservan.
    /// Se hace una sola vez en el editor y queda guardado en la escena.
    /// </summary>
    internal static class MapPropCombiner
    {
        public static void Combine(MapZoneContext context, string label)
        {
            var groups = new Dictionary<Material, List<CombineInstance>>();

            foreach (var renderer in context.Props.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer is LineRenderer || renderer is ParticleSystemRenderer || !renderer.enabled)
                    continue;

                var filter = renderer.GetComponent<MeshFilter>();

                if (filter == null || filter.sharedMesh == null)
                    continue;

                var material = renderer.sharedMaterial;

                if (material == null)
                    continue;

                if (!groups.TryGetValue(material, out var list))
                {
                    list = new List<CombineInstance>();
                    groups.Add(material, list);
                }

                list.Add(new CombineInstance
                {
                    mesh = filter.sharedMesh,
                    transform = renderer.transform.localToWorldMatrix
                });

                renderer.enabled = false;
            }

            var root = new GameObject("MergedProps");
            root.transform.SetParent(context.Root.transform, false);

            foreach (var pair in groups)
            {
                var mesh = new Mesh
                {
                    name = label + "_Props_" + pair.Key.name,
                    indexFormat = IndexFormat.UInt32
                };

                mesh.CombineMeshes(pair.Value.ToArray(), true, true);
                mesh.RecalculateBounds();

                var saved = MapMeshFactory.SaveZoneMesh(label + "_Props_" + pair.Key.name, mesh);
                var go = MapObjects.MeshRenderer(root.transform, "Props_" + pair.Key.name, saved, pair.Key, true);
                go.isStatic = false;
            }
        }
    }
}
