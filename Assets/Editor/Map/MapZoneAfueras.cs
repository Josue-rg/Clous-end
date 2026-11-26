using UnityEngine;
using ClousEnd.World;

namespace ClousEnd.EditorTools
{
    /// <summary>Zona 1: afueras y autopista. Naves industriales, pinos y camiones.</summary>
    internal static class MapZoneAfueras
    {
        public static ZoneStreamer.Zone Build(Transform zonesRoot, MapAssets assets)
        {
            var context = new MapZoneContext("Zone_Afueras", zonesRoot);

            BuildStartGate(context, assets);

            MapZoneCommon.BuildingRow(context, assets, 40f, 332f, -1f, MapBuildingStyle.Industrial, 101,
                9f, 20f, 14f, 24f, 15f, 26f, 0.1f, context.WarmWindows);

            MapZoneCommon.BuildingRow(context, assets, 34f, 332f, 1f, MapBuildingStyle.Industrial, 102,
                9f, 22f, 14f, 24f, 15f, 26f, 0.12f, context.WarmWindows);

            MapZoneCommon.BackRow(context, assets, 20f, 332f, -1f, MapBuildingStyle.Industrial, 103, 24f, 52f, context.WarmWindows);
            MapZoneCommon.BackRow(context, assets, 20f, 332f, 1f, MapBuildingStyle.Industrial, 104, 24f, 56f, context.WarmWindows);

            // Naves grandes.
            Warehouse(context, assets, new Vector3(-62f, 0f, 86f), new Vector3(36f, 13f, 48f));
            Warehouse(context, assets, new Vector3(66f, 0f, 264f), new Vector3(42f, 15f, 56f));

            // Contenedores en las aceras (fuera de la calzada pero visibles).
            MapPropsFactory.Container(context.Props, new Vector3(16.2f, 0f, 118f), 4f, assets.Metal, assets.SnowRoof, assets.LampWarm);
            MapPropsFactory.Container(context.Props, new Vector3(-16.2f, 0f, 236f), -6f, assets.Concrete, assets.SnowRoof, assets.LampWarm);
            MapPropsFactory.Container(context.Props, new Vector3(16.2f, 0f, 298f), 2f, assets.Metal, assets.SnowRoof, assets.LampCool);

            MapZoneCommon.Pines(context, assets, 8f, 334f, 58, 105, 24f, 112f);
            MapZoneCommon.Lamps(context, assets, 30f, 330f, 30f, false, 4f);
            MapZoneCommon.SnowBanks(context, assets, 4f, 336f, 42, 106);
            MapZoneCommon.ParkedCars(context, assets, 44f, 320f, 18, 107, assets.CarPaint);
            MapZoneCommon.DebrisField(context, assets, 36f, 326f, 26, 108, 12f);

            for (var i = 0; i < 12; i++)
            {
                var z = 26f + i * 26f;
                MapPropsFactory.SnowPole(context.Props, new Vector3(-13.7f, 0.2f, z), assets.Metal, assets.LampWarm);
                MapPropsFactory.SnowPole(context.Props, new Vector3(13.7f, 0.2f, z + 12f), assets.Metal, assets.LampWarm);
            }

            // Camiones en la calzada: son los primeros obstáculos a esquivar.
            MapPropsFactory.Truck(context.Props, new Vector3(6.6f, 0f, 206f), 8f,
                assets.Metal, assets.Concrete, assets.LampWarm, assets.SnowRoof);
            MapPropsFactory.Truck(context.Props, new Vector3(-6.4f, 0f, 292f), 186f,
                assets.Concrete, assets.Metal, assets.LampWarm, assets.SnowRoof);

            MapPropsFactory.Barricade(context.Props, new Vector3(7.4f, 0f, 152f), 6f,
                assets.Metal, assets.RoadPaint, assets.LampWarm);
            MapPropsFactory.Barricade(context.Props, new Vector3(-8.2f, 0f, 248f), -8f,
                assets.Metal, assets.RoadPaint, assets.LampWarm);

            MapPropsFactory.Sign(context.Props, new Vector3(-15.4f, 0.2f, 74f), 90f, assets.Metal, assets.LampCool);
            MapPropsFactory.Sign(context.Props, new Vector3(15.4f, 0.2f, 186f), -90f, assets.Metal, assets.LampWarm);

            AlienShard(context, assets, new Vector3(88f, 0f, 196f), 34f, 5f);

            return context.Finish(assets, "Afueras", MapPalette.RouteStart, MapPalette.AfuerasEnd);
        }

        private static void BuildStartGate(MapZoneContext context, MapAssets assets)
        {
            var gate = MapObjects.Empty("SM_StartGate", context.Landmarks, new Vector3(0f, 0f, 26f));

            MapObjects.Box(gate.transform, "PillarL", new Vector3(-15.4f, 10f, 0f), new Vector3(1.6f, 20f, 1.6f), assets.Concrete, true, true);
            MapObjects.Box(gate.transform, "PillarR", new Vector3(15.4f, 10f, 0f), new Vector3(1.6f, 20f, 1.6f), assets.Concrete, true, true);
            MapObjects.Box(gate.transform, "Beam", new Vector3(0f, 20.6f, 0f), new Vector3(32.5f, 1.4f, 1.9f), assets.Metal, true, true);
            MapObjects.Box(gate.transform, "Panel", new Vector3(0f, 18.4f, -0.75f), new Vector3(21f, 0.9f, 0.3f), assets.LampCool, false, false);
            MapObjects.Box(gate.transform, "Snow", new Vector3(0f, 21.45f, 0f), new Vector3(32.5f, 0.35f, 2.1f), assets.SnowRoof, false, false);
        }

        private static void Warehouse(MapZoneContext context, MapAssets assets, Vector3 position, Vector3 size)
        {
            var root = MapObjects.Empty("SM_Warehouse", context.Buildings, position);
            var facadeSign = position.x < 0f ? 1f : -1f;

            MapObjects.Box(root.transform, "Body", new Vector3(0f, size.y * 0.5f, 0f), size, assets.Metal, true, true);
            MapObjects.Box(root.transform, "Roof", new Vector3(0f, size.y + 0.25f, 0f), new Vector3(size.x + 0.6f, 0.5f, size.z + 0.6f), assets.SnowRoof, false, false);
            MapObjects.Box(root.transform, "Door", new Vector3(facadeSign * (size.x * 0.5f + 0.05f), 4.4f, 0f), new Vector3(0.12f, 8.4f, 9f), assets.LampWarm, false, false);
            MapObjects.Box(root.transform, "Banner", new Vector3(facadeSign * (size.x * 0.5f + 0.12f), size.y - 1.8f, 0f), new Vector3(0.12f, 1.1f, size.z * 0.7f), assets.LampCool, false, false);
            MapObjects.Box(root.transform, "WindowStrip", new Vector3(facadeSign * (size.x * 0.5f + 0.14f), 9.4f, 0f), new Vector3(0.12f, 1.4f, size.z * 0.86f), assets.WarmWindow, false, false);
        }

        private static void AlienShard(MapZoneContext context, MapAssets assets, Vector3 position, float height, float radius)
        {
            var root = MapObjects.Empty("SM_AlienShard", context.Landmarks, position);

            MapObjects.Mesh(
                root.transform,
                "Spike",
                MapMeshFactory.Mountain,
                Vector3.zero,
                new Vector3(0f, 18f, 0f),
                new Vector3(radius * 2f, height, radius * 2f),
                assets.AlienObsidian,
                true);

            MapObjects.Cylinder(root.transform, "Vein", new Vector3(0f, height * 0.34f, 0f), radius * 1.05f, 0.5f, assets.AlienVein, false, false);
            MapObjects.Cylinder(root.transform, "Vein2", new Vector3(0f, height * 0.62f, 0f), radius * 0.7f, 0.35f, assets.AlienVein, false, false);
        }
    }
}
