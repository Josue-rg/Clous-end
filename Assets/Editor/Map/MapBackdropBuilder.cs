using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>Montañas y siluetas lejanas que cierran el horizonte del mapa.</summary>
    internal static class MapBackdropBuilder
    {
        public static void Build(Transform root, MapAssets assets)
        {
            var backdrop = new GameObject("Backdrop").transform;
            backdrop.SetParent(root, false);

            var random = new System.Random(950);

            // Cordillera lateral: siempre a más de 400 m de la calzada.
            for (var side = -1; side <= 1; side += 2)
            {
                for (var i = 0; i < 8; i++)
                {
                    var z = -260f + i * 300f + ((float)random.NextDouble() - 0.5f) * 140f;
                    var x = side * (430f + (float)random.NextDouble() * 230f);
                    var height = 110f + (float)random.NextDouble() * 120f;
                    var width = 220f + (float)random.NextDouble() * 200f;
                    AddMountain(backdrop, assets, new Vector3(x, -2f, z), width, height, random);
                }
            }

            // Cierre del fondo, detrás del inicio y detrás del refugio.
            for (var i = 0; i < 4; i++)
            {
                AddMountain(backdrop, assets, new Vector3(-260f + i * 175f, -2f, -330f - (float)random.NextDouble() * 120f), 240f, 150f, random);
                AddMountain(backdrop, assets, new Vector3(-300f + i * 190f, -2f, 1930f + (float)random.NextDouble() * 120f), 260f, 170f, random);
            }

            // Skyline lejano a los lados del refugio.
            for (var i = 0; i < 18; i++)
            {
                var side = i % 2 == 0 ? -1f : 1f;
                var x = side * (150f + (i / 2) * 27f);
                var z = 1300f + (i / 2) * 62f;
                var height = 45f + (float)random.NextDouble() * 95f;

                MapObjects.Box(
                    backdrop,
                    "SM_FarTower_" + i,
                    new Vector3(x, height * 0.5f, z),
                    new Vector3(24f + (float)random.NextDouble() * 18f, height, 26f),
                    assets.HistoricWalls[i % assets.HistoricWalls.Length],
                    false,
                    false);
            }
        }

        private static void AddMountain(Transform parent, MapAssets assets, Vector3 position, float width, float height, System.Random random)
        {
            MapObjects.Mesh(
                parent,
                "SM_Mountain",
                MapMeshFactory.Mountain,
                position,
                new Vector3(0f, (float)random.NextDouble() * 360f, 0f),
                new Vector3(width, height, width * 0.78f),
                assets.Mountain,
                false);

            MapObjects.Mesh(
                parent,
                "SM_MountainSnow",
                MapMeshFactory.Mountain,
                position + Vector3.up * (height * 0.68f),
                new Vector3(0f, (float)random.NextDouble() * 360f, 0f),
                new Vector3(width * 0.42f, height * 0.33f, width * 0.42f * 0.78f),
                assets.SnowRoof,
                false);
        }
    }
}
