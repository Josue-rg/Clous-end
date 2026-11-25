using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>Piezas repetidas de las zonas: hileras de edificios, farolas, nieve.</summary>
    internal static class MapZoneCommon
    {
        public static void BuildingRow(
            MapZoneContext context,
            MapAssets assets,
            float zStart,
            float zEnd,
            float side,
            MapBuildingStyle style,
            int seed,
            float minHeight,
            float maxHeight,
            float minDepth,
            float maxDepth,
            float minFrontage,
            float maxFrontage,
            float gapChance,
            WindowMeshBuilder windows,
            float litSideSign = 1f)
        {
            var random = new System.Random(seed);
            var z = zStart;
            var index = 0;

            while (z < zEnd - minFrontage)
            {
                var frontage = Mathf.Min(
                    Mathf.Lerp(minFrontage, maxFrontage, (float)random.NextDouble()),
                    zEnd - z);

                if (frontage < 8f)
                    break;

                var depth = Mathf.Lerp(minDepth, maxDepth, (float)random.NextDouble());
                var height = Mathf.Lerp(minHeight, maxHeight, (float)random.NextDouble());
                var gap = random.NextDouble() < gapChance ? 5f + (float)random.NextDouble() * 9f : 1.6f;

                var wall = style == MapBuildingStyle.Tower
                    ? Pick(assets.TowerWalls, random)
                    : style == MapBuildingStyle.Historic
                        ? Pick(assets.HistoricWalls, random)
                        : Pick(assets.TowerWalls, random);

                var trim = style == MapBuildingStyle.Historic ? assets.Metal : assets.Concrete;
                var center = new Vector3(side * (MapPalette.BuildingLine + depth * 0.5f), 0f, z + frontage * 0.5f);

                MapBuildingFactory.Create(
                    context.Buildings,
                    "SM_Bldg_" + context.Root.name + "_" + index,
                    center,
                    new Vector2(depth, frontage),
                    height,
                    style,
                    wall,
                    trim,
                    assets.SnowRoof,
                    assets.LampCool,
                    -side * litSideSign,
                    windows,
                    random);

                z += frontage + gap;
                index++;
            }
        }

        public static void BackRow(
            MapZoneContext context,
            MapAssets assets,
            float zStart,
            float zEnd,
            float side,
            MapBuildingStyle style,
            int seed,
            float minHeight,
            float maxHeight,
            WindowMeshBuilder windows = null)
        {
            var random = new System.Random(seed);
            var z = zStart + 10f;
            var index = 0;

            while (z < zEnd - 20f)
            {
                var frontage = 14f + (float)random.NextDouble() * 20f;
                var depth = 18f + (float)random.NextDouble() * 20f;
                var height = Mathf.Lerp(minHeight, maxHeight, (float)random.NextDouble());
                var distance = 48f + (float)random.NextDouble() * 46f;
                var wall = style == MapBuildingStyle.Historic
                    ? Pick(assets.HistoricWalls, random)
                    : Pick(assets.TowerWalls, random);

                MapBuildingFactory.Create(
                    context.Buildings,
                    "SM_Bldg_Back_" + context.Root.name + "_" + index,
                    new Vector3(side * distance, 0f, z + frontage * 0.5f),
                    new Vector2(depth, frontage),
                    height,
                    style,
                    wall,
                    assets.Concrete,
                    assets.SnowRoof,
                    assets.LampCool,
                    -side,
                    windows,
                    random,
                    windows != null);

                z += frontage + 8f + (float)random.NextDouble() * 14f;
                index++;
            }
        }

        public static void Lamps(
            MapZoneContext context,
            MapAssets assets,
            float zStart,
            float zEnd,
            float spacing,
            bool cool,
            float startOffset = 0f)
        {
            var material = cool ? assets.LampCool : assets.LampWarm;
            var index = 0;

            for (var z = zStart + startOffset; z < zEnd; z += spacing)
            {
                var side = index % 2 == 0 ? -1f : 1f;
                MapPropsFactory.StreetLamp(
                    context.Props,
                    new Vector3(side * 14.8f, 0.2f, z),
                    side < 0f ? 90f : -90f,
                    assets.Metal,
                    material);
                index++;
            }
        }

        public static void SnowBanks(
            MapZoneContext context,
            MapAssets assets,
            float zStart,
            float zEnd,
            int count,
            int seed)
        {
            var random = new System.Random(seed);

            for (var i = 0; i < count; i++)
            {
                var side = random.Next(2) == 0 ? -1f : 1f;
                var z = Mathf.Lerp(zStart, zEnd, (float)random.NextDouble());
                var x = side * (13.9f + (float)random.NextDouble() * 3.5f);
                var size = new Vector3(
                    3.5f + (float)random.NextDouble() * 6f,
                    0.35f + (float)random.NextDouble() * 0.65f,
                    2.5f + (float)random.NextDouble() * 4f);

                MapPropsFactory.SnowBank(
                    context.Props,
                    new Vector3(x, 0.12f, z),
                    size,
                    assets.Snow,
                    (float)random.NextDouble() * 360f);
            }
        }

        public static void ParkedCars(
            MapZoneContext context,
            MapAssets assets,
            float zStart,
            float zEnd,
            int count,
            int seed,
            Material body)
        {
            var random = new System.Random(seed);

            for (var i = 0; i < count; i++)
            {
                var side = random.Next(2) == 0 ? -1f : 1f;
                var z = Mathf.Lerp(zStart + 8f, zEnd - 8f, (float)random.NextDouble());
                var yaw = side * 90f + (random.Next(2) == 0 ? 0f : 180f);

                MapPropsFactory.ParkedCar(
                    context.Props,
                    new Vector3(side * 11.2f, 0f, z),
                    yaw,
                    body,
                    assets.Glass,
                    assets.LampWarm,
                    assets.SnowRoof);
            }
        }

        public static void Pines(
            MapZoneContext context,
            MapAssets assets,
            float zStart,
            float zEnd,
            int count,
            int seed,
            float minDistance = 26f,
            float maxDistance = 62f)
        {
            var random = new System.Random(seed);

            for (var i = 0; i < count; i++)
            {
                var side = random.Next(2) == 0 ? -1f : 1f;
                var distance = Mathf.Lerp(minDistance, maxDistance, (float)random.NextDouble());
                var z = Mathf.Lerp(zStart, zEnd, (float)random.NextDouble());
                var scale = 0.7f + (float)random.NextDouble() * 1.6f;

                MapPropsFactory.Pine(
                    context.Props,
                    new Vector3(side * distance, 0f, z),
                    scale,
                    assets.HistoricWood,
                    assets.Pine,
                    random);
            }
        }

        public static void DebrisField(
            MapZoneContext context,
            MapAssets assets,
            float zStart,
            float zEnd,
            int count,
            int seed,
            float maxSide = 11f)
        {
            var random = new System.Random(seed);

            for (var i = 0; i < count; i++)
            {
                var z = Mathf.Lerp(zStart, zEnd, (float)random.NextDouble());
                var x = Mathf.Lerp(-maxSide, maxSide, (float)random.NextDouble());
                var size = new Vector3(
                    0.5f + (float)random.NextDouble() * 2.4f,
                    0.3f + (float)random.NextDouble() * 1.2f,
                    0.5f + (float)random.NextDouble() * 2.4f);

                MapPropsFactory.Debris(
                    context.Props,
                    new Vector3(x, size.y * 0.5f, z),
                    size,
                    new Vector3(0f, 0f, 0f),
                    assets.Rubble,
                    random);
            }
        }

        private static Material Pick(Material[] materials, System.Random random)
        {
            if (materials == null || materials.Length == 0)
                return null;

            return materials[random.Next(materials.Length)];
        }
    }
}
