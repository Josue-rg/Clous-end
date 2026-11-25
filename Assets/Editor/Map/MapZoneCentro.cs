using UnityEngine;
using ClousEnd.World;

namespace ClousEnd.EditorTools
{
    /// <summary>Zona 2: centro histórico. Plaza, iglesia, árbol de Navidad y mercadillo.</summary>
    internal static class MapZoneCentro
    {
        public static ZoneStreamer.Zone Build(Transform zonesRoot, MapAssets assets)
        {
            var context = new MapZoneContext("Zone_Centro", zonesRoot);

            MapZoneCommon.BuildingRow(context, assets, 344f, 430f, -1f, MapBuildingStyle.Historic, 201,
                10f, 24f, 14f, 22f, 15f, 24f, 0.1f, context.WarmWindows);

            MapZoneCommon.BuildingRow(context, assets, 616f, 816f, -1f, MapBuildingStyle.Historic, 202,
                10f, 26f, 14f, 23f, 15f, 24f, 0.1f, context.WarmWindows);

            MapZoneCommon.BuildingRow(context, assets, 344f, 816f, 1f, MapBuildingStyle.Historic, 203,
                10f, 26f, 14f, 23f, 15f, 24f, 0.1f, context.WarmWindows);

            MapZoneCommon.BackRow(context, assets, 340f, 430f, -1f, MapBuildingStyle.Historic, 204, 18f, 36f, context.WarmWindows);
            MapZoneCommon.BackRow(context, assets, 616f, 820f, -1f, MapBuildingStyle.Historic, 205, 18f, 36f, context.WarmWindows);
            MapZoneCommon.BackRow(context, assets, 340f, 820f, 1f, MapBuildingStyle.Historic, 206, 18f, 38f, context.WarmWindows);

            // Fondo de la plaza.
            MapBuildingFactory.Create(
                context.Buildings,
                "SM_Bldg_PlazaBackA",
                new Vector3(-92f, 0f, 468f),
                new Vector2(30f, 34f),
                26f,
                MapBuildingStyle.Historic,
                assets.HistoricWalls[1],
                assets.Metal,
                assets.SnowRoof,
                assets.LampWarm,
                1f,
                context.WarmWindows,
                new System.Random(207));

            MapBuildingFactory.Create(
                context.Buildings,
                "SM_Bldg_PlazaBackB",
                new Vector3(-96f, 0f, 566f),
                new Vector2(34f, 38f),
                30f,
                MapBuildingStyle.Historic,
                assets.HistoricWalls[3],
                assets.Metal,
                assets.SnowRoof,
                assets.LampWarm,
                1f,
                context.WarmWindows,
                new System.Random(208));

            BuildPlaza(context, assets);
            BuildChurch(context, assets);

            for (var i = 0; i < 4; i++)
            {
                var z = 386f + i * 118f;
                MapPropsFactory.Cable(context.Props, "SM_StringLights_" + i,
                    new Vector3(-18f, 8.5f, z), new Vector3(18f, 8.5f, z), 1.6f, assets.LampWarm, 12);
            }

            MapZoneCommon.Lamps(context, assets, 350f, 816f, 38f, false);
            MapZoneCommon.SnowBanks(context, assets, 342f, 818f, 26, 209);
            MapZoneCommon.ParkedCars(context, assets, 352f, 810f, 12, 210, assets.Metal);
            MapZoneCommon.Pines(context, assets, 342f, 818f, 22, 211, 52f, 96f);
            MapZoneCommon.DebrisField(context, assets, 600f, 816f, 12, 212, 10f);

            return context.Finish(assets, "Centro", MapPalette.AfuerasEnd, MapPalette.CentroEnd);
        }

        private static void BuildPlaza(MapZoneContext context, MapAssets assets)
        {
            var plaza = MapObjects.Empty("SM_Plaza", context.Landmarks, new Vector3(-44f, 0f, 520f));

            MapObjects.Box(plaza.transform, "Floor", new Vector3(0f, 0.03f, 0f), new Vector3(58f, 0.06f, 178f), assets.Sidewalk, false, false);
            MapObjects.Box(plaza.transform, "Frost", new Vector3(0f, 0.07f, 0f), new Vector3(44f, 0.04f, 150f), assets.SnowRoof, false, false);

            MapPropsFactory.Fountain(plaza.transform, new Vector3(14f, 0f, -58f), assets.Concrete, assets.Glass, assets.RefugeGlow);
            MapPropsFactory.ChristmasTree(
                plaza.transform,
                new Vector3(14f, 0f, 36f),
                assets.HistoricWood,
                assets.Pine,
                assets.LampWarm,
                assets.LampWarm,
                assets.LampCool,
                new System.Random(220));

            var random = new System.Random(221);

            for (var i = 0; i < 5; i++)
            {
                var side = i % 2 == 0 ? -1f : 1f;
                MapPropsFactory.MarketStall(
                    context.Props,
                    new Vector3(-44f + side * 20f, 0f, 452f + i * 24f),
                    side * 90f,
                    assets.HistoricWood,
                    assets.CarPaint,
                    assets.LampWarm);
            }

            for (var i = 0; i < 6; i++)
            {
                var angle = i / 6f * Mathf.PI * 2f;
                MapPropsFactory.Bench(
                    context.Props,
                    new Vector3(-30f + Mathf.Cos(angle) * 15f, 0f, 556f + Mathf.Sin(angle) * 15f),
                    -angle * Mathf.Rad2Deg + 90f,
                    assets.HistoricWood,
                    assets.SnowRoof);
            }

            for (var i = 0; i < 6; i++)
            {
                var angle = i / 6f * Mathf.PI * 2f;
                MapPropsFactory.StreetLamp(
                    context.Props,
                    new Vector3(-44f + Mathf.Cos(angle) * 24f, 0f, 520f + Mathf.Sin(angle) * 34f),
                    angle * Mathf.Rad2Deg + 90f,
                    assets.Metal,
                    assets.LampWarm);
            }

            for (var i = 0; i < 10; i++)
            {
                var x = -78f + (float)random.NextDouble() * 8f;
                var z = 438f + (float)random.NextDouble() * 158f;
                MapPropsFactory.Pine(context.Props, new Vector3(x, 0f, z), 0.8f + (float)random.NextDouble() * 0.9f,
                    assets.HistoricWood, assets.Pine, random);
            }
        }

        private static void BuildChurch(MapZoneContext context, MapAssets assets)
        {
            var church = MapObjects.Empty("SM_Church", context.Landmarks, new Vector3(-56f, 0f, 690f));

            MapObjects.Box(church.transform, "Nave", new Vector3(0f, 8f, 0f), new Vector3(30f, 16f, 44f), assets.Concrete, true, true);
            MapObjects.Mesh(
                church.transform,
                "Roof",
                MapMeshFactory.Pyramid,
                new Vector3(0f, 16f, 0f),
                new Vector3(0f, 45f, 0f),
                new Vector3(30f * 1.414f, 9f, 44f * 1.414f),
                assets.SnowRoof,
                false);

            MapObjects.Box(church.transform, "Tower", new Vector3(13f, 22f, 18f), new Vector3(10f, 44f, 10f), assets.Concrete, true, true);
            MapObjects.Box(church.transform, "TowerCap", new Vector3(13f, 44.6f, 18f), new Vector3(11.6f, 1.2f, 11.6f), assets.Metal, false, true);
            MapObjects.Mesh(
                church.transform,
                "Spire",
                MapMeshFactory.Pyramid,
                new Vector3(13f, 45f, 18f),
                new Vector3(0f, 45f, 0f),
                new Vector3(11.6f * 1.414f, 13f, 11.6f * 1.414f),
                assets.Metal,
                false);

            MapObjects.Sphere(church.transform, "RoseWindow", new Vector3(18.05f, 28f, 18f), new Vector3(0.4f, 3.6f, 3.6f), assets.LampWarm, false, false);
            MapObjects.Box(church.transform, "TowerWindow", new Vector3(18.05f, 19f, 18f), new Vector3(0.18f, 7f, 1.4f), assets.LampWarm, false, false);
            MapObjects.Box(church.transform, "Door", new Vector3(15.05f, 3.2f, 0f), new Vector3(0.2f, 6.4f, 5f), assets.LampWarm, false, false);
            MapObjects.Box(church.transform, "DoorFrame", new Vector3(15.12f, 4.6f, 0f), new Vector3(0.12f, 9.2f, 6.6f), assets.Metal, false, false);

            MapObjects.Cylinder(church.transform, "BellTowerRing", new Vector3(13f, 40f, 18f), 7.2f, 0.5f, assets.LampWarm, false, false);
        }
    }
}
