using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>
    /// Mallas reutilizables del mapa. Las que se generan se guardan como assets
    /// para que la escena las conserve al cerrar Unity.
    /// </summary>
    internal static class MapMeshFactory
    {
        private const string MeshRoot = "Assets/Art/Game/Meshes";
        private const string GeneratedRoot = MeshRoot + "/Generated";

        public static Mesh Cone => LoadOrCreate("MapCone", () => BuildCone(24, 0f, 0f));
        public static Mesh Pyramid => LoadOrCreate("MapPyramid", () => BuildCone(4, 0f, 0f));
        public static Mesh Mountain => LoadOrCreate("MapMountain", () => BuildCone(13, 0.2f, 0.1f));
        public static Mesh RoadDashes => LoadOrCreate("MapRoadDashes", BuildDashes);

        public static Mesh SaveZoneMesh(string name, Mesh mesh)
        {
            var path = GeneratedRoot + "/" + name + ".asset";
            AssetDatabase.DeleteAsset(path);
            mesh.name = name;
            AssetDatabase.CreateAsset(mesh, path);
            return AssetDatabase.LoadAssetAtPath<Mesh>(path);
        }

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

        private static Mesh BuildDashes()
        {
            var cube = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
            var combines = new List<CombineInstance>();

            for (var z = 4f; z < MapPalette.RouteEnd; z += 9f)
            {
                AddDash(combines, cube, new Vector3(-6.6f, 0f, z));
                AddDash(combines, cube, new Vector3(6.6f, 0f, z));
            }

            var mesh = new Mesh { name = "MapRoadDashes" };
            mesh.CombineMeshes(combines.ToArray(), true, true);
            mesh.RecalculateBounds();
            return mesh;
        }

        private static void AddDash(List<CombineInstance> combines, Mesh cube, Vector3 position)
        {
            combines.Add(new CombineInstance
            {
                mesh = cube,
                transform = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(0.26f, 0.02f, 4.2f))
            });
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

    /// <summary>
    /// Acumula cajas pequeñas (ventanas encendidas) en una sola malla por zona,
    /// para que toda la iluminación de fachadas cueste un draw call.
    /// </summary>
    internal sealed class WindowMeshBuilder
    {
        private readonly List<CombineInstance> instances = new List<CombineInstance>();
        private readonly Mesh cube;

        public WindowMeshBuilder()
        {
            cube = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
        }

        public int Count => instances.Count;

        /// <summary>
        /// Añade una cuadrícula de ventanas sobre la fachada que mira a la calle.
        /// facadeX es el centro X del edificio; depth es su tamaño en X.
        /// </summary>
        public void AddFacade(
            Vector3 baseCenter,
            float depth,
            float frontage,
            float height,
            float facadeSign,
            System.Random random,
            float litChance,
            float windowWidth = 1.5f,
            float windowHeight = 1.05f)
        {
            var facadeX = baseCenter.x + facadeSign * (depth * 0.5f + 0.18f);
            var floors = Mathf.FloorToInt((height - 4.2f) / 3.3f);
            var columns = Mathf.FloorToInt((frontage - 3f) / 3.4f);

            if (floors <= 0 || columns <= 0)
                return;

            var startZ = baseCenter.z - (columns - 1) * 1.7f;

            for (var floor = 0; floor < floors; floor++)
            {
                var y = 3.1f + floor * 3.3f;

                for (var column = 0; column < columns; column++)
                {
                    if (random.NextDouble() > litChance)
                        continue;

                    var z = startZ + column * 3.4f;
                    instances.Add(new CombineInstance
                    {
                        mesh = cube,
                        transform = Matrix4x4.TRS(
                            new Vector3(facadeX, y, z),
                            Quaternion.identity,
                            new Vector3(0.12f, windowHeight, windowWidth))
                    });
                }
            }
        }

        /// <summary>Ventanas de los testeros (caras que miran a lo largo de la calle).</summary>
        public void AddEndFacade(
            Vector3 baseCenter,
            float depth,
            float frontage,
            float height,
            float facadeSignZ,
            System.Random random,
            float litChance,
            float windowWidth = 1.5f,
            float windowHeight = 1.05f)
        {
            var facadeZ = baseCenter.z + facadeSignZ * (frontage * 0.5f + 0.18f);
            var floors = Mathf.FloorToInt((height - 4.2f) / 3.3f);
            var columns = Mathf.FloorToInt((depth - 3f) / 3.5f);

            if (floors <= 0 || columns <= 0)
                return;

            var startX = baseCenter.x - (columns - 1) * 1.75f;

            for (var floor = 0; floor < floors; floor++)
            {
                var y = 3.1f + floor * 3.3f;

                for (var column = 0; column < columns; column++)
                {
                    if (random.NextDouble() > litChance)
                        continue;

                    var x = startX + column * 3.5f;
                    instances.Add(new CombineInstance
                    {
                        mesh = cube,
                        transform = Matrix4x4.TRS(
                            new Vector3(x, y, facadeZ),
                            Quaternion.identity,
                            new Vector3(windowWidth, windowHeight, 0.12f))
                    });
                }
            }
        }

        /// <summary>Escaparate cálido a pie de calle.</summary>
        public void AddStorefront(Vector3 baseCenter, float depth, float frontage, float facadeSign)
        {
            var facadeX = baseCenter.x + facadeSign * (depth * 0.5f + 0.2f);

            instances.Add(new CombineInstance
            {
                mesh = cube,
                transform = Matrix4x4.TRS(
                    new Vector3(facadeX, 1.5f, baseCenter.z),
                    Quaternion.identity,
                    new Vector3(0.14f, 1.9f, Mathf.Max(2f, frontage * 0.55f)))
            });
        }

        public Mesh Build(string name)
        {
            var mesh = new Mesh { name = name, indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
            mesh.CombineMeshes(instances.ToArray(), true, true);
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
