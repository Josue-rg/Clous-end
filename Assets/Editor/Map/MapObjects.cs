using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ClousEnd.EditorTools
{
    /// <summary>Helpers de construcción del mapa (cajas, cilindros, LODs).</summary>
    internal static class MapObjects
    {
        public static GameObject Empty(string name, Transform parent, Vector3 position)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            return go;
        }

        public static GameObject Box(
            Transform parent,
            string name,
            Vector3 center,
            Vector3 size,
            Material material,
            bool collider = true,
            bool castShadows = true,
            bool markStatic = true,
            Quaternion? rotation = null)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;

            if (!collider)
                RemoveCollider(go);

            go.transform.SetParent(parent, false);
            go.transform.localPosition = center;
            go.transform.localRotation = rotation ?? Quaternion.identity;
            go.transform.localScale = size;
            go.isStatic = markStatic;

            var renderer = go.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
            renderer.receiveShadows = true;
            return go;
        }

        public static GameObject Cylinder(
            Transform parent,
            string name,
            Vector3 baseCenter,
            float radius,
            float height,
            Material material,
            bool collider = false,
            bool castShadows = true,
            bool markStatic = true)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            go.name = name;

            if (!collider)
                RemoveCollider(go);

            go.transform.SetParent(parent, false);
            go.transform.localPosition = baseCenter + Vector3.up * (height * 0.5f);
            go.transform.localScale = new Vector3(radius * 2f, height * 0.5f, radius * 2f);
            go.isStatic = markStatic;

            var renderer = go.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
            renderer.receiveShadows = true;
            return go;
        }

        public static GameObject Sphere(
            Transform parent,
            string name,
            Vector3 center,
            Vector3 size,
            Material material,
            bool collider = false,
            bool castShadows = true,
            bool markStatic = true)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = name;

            if (!collider)
                RemoveCollider(go);

            go.transform.SetParent(parent, false);
            go.transform.localPosition = center;
            go.transform.localScale = size;
            go.isStatic = markStatic;

            var renderer = go.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
            renderer.receiveShadows = true;
            return go;
        }

        public static GameObject Mesh(
            Transform parent,
            string name,
            Mesh mesh,
            Vector3 position,
            Vector3 euler,
            Vector3 scale,
            Material material,
            bool castShadows = true,
            bool markStatic = true)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;
            go.transform.localRotation = Quaternion.Euler(euler);
            go.transform.localScale = scale;
            go.isStatic = markStatic;

            go.AddComponent<MeshFilter>().sharedMesh = mesh;

            var renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
            renderer.receiveShadows = true;
            return go;
        }

        /// <summary>Malla de ventanas ya combinada (una por zona).</summary>
        public static GameObject MeshRenderer(
            Transform parent,
            string name,
            Mesh mesh,
            Material material,
            bool castShadows = false)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<MeshFilter>().sharedMesh = mesh;

            var renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
            renderer.receiveShadows = true;
            go.isStatic = false;
            return go;
        }

        public static LODGroup AddLods(GameObject root, Renderer[] lod0, Renderer[] lod1)
        {
            var group = root.AddComponent<LODGroup>();
            group.SetLODs(new[]
            {
                new LOD(1f, lod0) { fadeTransitionWidth = 0.18f },
                new LOD(0.075f, lod1) { fadeTransitionWidth = 0.12f },
                new LOD(0.008f, Array.Empty<Renderer>())
            });
            group.RecalculateBounds();
            group.fadeMode = LODFadeMode.CrossFade;
            group.animateCrossFading = false;
            return group;
        }

        public static Renderer RendererOf(GameObject go)
        {
            return go.GetComponent<Renderer>();
        }

        public static Renderer[] Renderers(params GameObject[] objects)
        {
            var renderers = new Renderer[objects.Length];

            for (var i = 0; i < objects.Length; i++)
                renderers[i] = objects[i].GetComponent<Renderer>();

            return renderers;
        }

        public static void RemoveCollider(GameObject go)
        {
            var collider = go.GetComponent<Collider>();

            if (collider != null)
                UnityEngine.Object.DestroyImmediate(collider);
        }

        public static void SetLayerRecursive(GameObject go, int layer)
        {
            go.layer = layer;

            foreach (Transform child in go.transform)
                SetLayerRecursive(child.gameObject, layer);
        }

        /// <summary>Cilindro de colisión fino para farolas y postes.</summary>
        public static void AddPostCollider(GameObject go, float radius, float height)
        {
            var collider = go.AddComponent<CapsuleCollider>();
            collider.radius = radius;
            collider.height = height;
            collider.center = new Vector3(0f, height * 0.5f, 0f);
        }
    }
}
