using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>Helpers de construcción para que los builders sean legibles.</summary>
    internal static class LoginSceneObjects
    {
        public static GameObject Empty(string name, Transform parent, Vector3 localPosition, Vector3 localEuler)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localRotation = Quaternion.Euler(localEuler);
            return go;
        }

        public static GameObject Primitive(
            string name,
            PrimitiveType type,
            Transform parent,
            Vector3 localPosition,
            Vector3 localEuler,
            Vector3 localScale,
            Material material,
            bool castShadows = true)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;

            var collider = go.GetComponent<Collider>();
            if (collider != null)
                Object.DestroyImmediate(collider);

            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localRotation = Quaternion.Euler(localEuler);
            go.transform.localScale = localScale;

            var renderer = go.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial = material;
                renderer.shadowCastingMode = castShadows
                    ? UnityEngine.Rendering.ShadowCastingMode.On
                    : UnityEngine.Rendering.ShadowCastingMode.Off;
                renderer.receiveShadows = true;
            }

            go.isStatic = true;
            return go;
        }

        public static GameObject Mesh(
            string name,
            Mesh mesh,
            Transform parent,
            Vector3 localPosition,
            Vector3 localEuler,
            Vector3 localScale,
            Material material,
            bool castShadows = true)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            go.transform.localRotation = Quaternion.Euler(localEuler);
            go.transform.localScale = localScale;

            go.AddComponent<MeshFilter>().sharedMesh = mesh;

            var renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = castShadows
                ? UnityEngine.Rendering.ShadowCastingMode.On
                : UnityEngine.Rendering.ShadowCastingMode.Off;
            renderer.receiveShadows = true;

            go.isStatic = true;
            return go;
        }
    }
}
