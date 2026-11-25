using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>Suelo, calzada, aceras, líneas y nieve de todo el corredor.</summary>
    internal static class MapGroundBuilder
    {
        private const float CenterZ = 750f;

        public static void Build(Transform root, MapAssets assets)
        {
            var ground = new GameObject("Ground").transform;
            ground.SetParent(root, false);

            MapObjects.Box(
                ground,
                "SM_Ground",
                new Vector3(0f, -0.5f, 800f),
                new Vector3(1400f, 1f, 2500f),
                assets.Snow,
                true,
                true);

            MapObjects.Box(
                ground,
                "SM_Road",
                new Vector3(0f, 0.02f, CenterZ),
                new Vector3(MapPalette.RoadWidth, 0.04f, 1560f),
                assets.Road,
                false,
                false);

            MapObjects.Box(ground, "SM_EdgeL", new Vector3(-12.4f, 0.045f, CenterZ), new Vector3(0.22f, 0.02f, 1550f), assets.RoadPaint, false, false);
            MapObjects.Box(ground, "SM_EdgeR", new Vector3(12.4f, 0.045f, CenterZ), new Vector3(0.22f, 0.02f, 1550f), assets.RoadPaint, false, false);

            MapObjects.MeshRenderer(ground, "SM_RoadDashes", MapMeshFactory.RoadDashes, assets.RoadPaint)
                .transform.localPosition = new Vector3(0f, 0.055f, 0f);

            for (var side = -1; side <= 1; side += 2)
            {
                MapObjects.Box(
                    ground,
                    side < 0 ? "SM_SidewalkL" : "SM_SidewalkR",
                    new Vector3(side * 16f, 0.1f, CenterZ),
                    new Vector3(MapPalette.SidewalkWidth, 0.2f, 1550f),
                    assets.Sidewalk,
                    true,
                    false);
            }

            // Dunas de nieve que rompen la planitud del borde del mapa.
            var random = new System.Random(900);

            for (var i = 0; i < 42; i++)
            {
                var side = random.Next(2) == 0 ? -1f : 1f;
                var x = side * (22f + (float)random.NextDouble() * 240f);
                var z = -80f + (float)random.NextDouble() * 1700f;

                // La plaza y la iglesia se quedan despejadas.
                if (x < -10f && z > 410f && z < 740f)
                    continue;

                var size = new Vector3(
                    8f + (float)random.NextDouble() * 34f,
                    0.6f + (float)random.NextDouble() * 3.4f,
                    8f + (float)random.NextDouble() * 30f);

                MapPropsFactory.SnowBank(ground, new Vector3(x, 0.05f, z), size, assets.Snow, (float)random.NextDouble() * 360f);
            }
        }
    }
}
