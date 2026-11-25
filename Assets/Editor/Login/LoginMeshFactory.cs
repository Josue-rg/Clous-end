using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>Mallas simples reutilizables para el diorama. Se guardan como assets editables.</summary>
    internal static class LoginMeshFactory
    {
        private const string MeshRoot = "Assets/Art/Login/Meshes";

        public static Mesh PineCone => LoadOrCreate("PineCone", () => BuildCone(24, 0f, 0f));
        public static Mesh Mountain => LoadOrCreate("Mountain", () => BuildCone(11, 0.16f, 0.1f));

        private static Mesh LoadOrCreate(string name, System.Func<Mesh> create)
        {
            var path = MeshRoot + "/" + name + ".asset";
            var existing = AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if (existing != null)
                return existing;

            var mesh = create();
            mesh.name = name;
            AssetDatabase.CreateAsset(mesh, path);
            return mesh;
        }

        private static Mesh BuildCone(int segments, float radiusJitter, float topJitter)
        {
            var vertices = new List<Vector3>();
            var uvs = new List<Vector2>();
            var triangles = new List<int>();

            for (var i = 0; i < segments; i++)
            {
                var angle = i / (float)segments * Mathf.PI * 2f;
                var jitter = 1f + (Mathf.PerlinNoise(i * 1.37f, 0.5f) - 0.5f) * 2f * radiusJitter;
                var x = Mathf.Cos(angle) * 0.5f * jitter;
                var z = Mathf.Sin(angle) * 0.5f * jitter;

                vertices.Add(new Vector3(x, 0f, z));
                uvs.Add(new Vector2(i / (float)segments, 0f));
            }

            var topX = (Mathf.PerlinNoise(3.2f, 7.1f) - 0.5f) * 2f * topJitter;
            var topZ = (Mathf.PerlinNoise(8.4f, 1.9f) - 0.5f) * 2f * topJitter;
            var topIndex = vertices.Count;
            vertices.Add(new Vector3(topX, 1f, topZ));
            uvs.Add(new Vector2(0.5f, 1f));

            for (var i = 0; i < segments; i++)
            {
                var next = (i + 1) % segments;
                triangles.Add(i);
                triangles.Add(topIndex);
                triangles.Add(next);
            }

            var centerIndex = vertices.Count;
            vertices.Add(Vector3.zero);
            uvs.Add(new Vector2(0.5f, 0.5f));

            for (var i = 0; i < segments; i++)
            {
                var next = (i + 1) % segments;
                triangles.Add(centerIndex);
                triangles.Add(next);
                triangles.Add(i);
            }

            var mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
