using UnityEngine;
using ClousEnd.World;

namespace ClousEnd.EditorTools
{
    /// <summary>Zona 3: distrito financiero. Torres, pasarelas, cables y el checkpoint.</summary>
    internal static class MapZoneFinanciero
    {
        public static ZoneStreamer.Zone Build(Transform zonesRoot, MapAssets assets)
        {
            var context = new MapZoneContext("Zone_Financiero", zonesRoot);

            MapZoneCommon.BuildingRow(context, assets, 826f, 1174f, -1f, MapBuildingStyle.Tower, 301,
                44f, 100f, 18f, 28f, 20f, 34f, 0.06f, context.CoolWindows);

            MapZoneCommon.BuildingRow(context, assets, 826f, 1174f, 1f, MapBuildingStyle.Tower, 302,
                40f, 104f, 18f, 28f, 20f, 34f, 0.06f, context.CoolWindows);

            MapZoneCommon.BackRow(context, assets, 820f, 1180f, -1f, MapBuildingStyle.Tower, 303, 80f, 150f, context.CoolWindows);
            MapZoneCommon.BackRow(context, assets, 820f, 1180f, 1f, MapBuildingStyle.Tower, 304, 80f, 150f, context.CoolWindows);

            BuildCheckpoint(context, assets);
            BuildSkybridge(context, assets, 900f, 36f);
            BuildSkybridge(context, assets, 1072f, 42f);
            BuildBillboard(context, assets);
            BuildCrane(context, assets);

            MapPropsFactory.Cable(context.Props, "SM_Cable_A", new Vector3(-20f, 58f, 840f), new Vector3(20f, 46f, 940f), 6f, assets.Metal, 18);
            MapPropsFactory.Cable(context.Props, "SM_Cable_B", new Vector3(20f, 64f, 950f), new Vector3(-20f, 52f, 1030f), 5f, assets.Metal, 18);
            MapPropsFactory.Cable(context.Props, "SM_Cable_C", new Vector3(-20f, 72f, 1080f), new Vector3(20f, 60f, 1160f), 7f, assets.Metal, 18);

            MapPropsFactory.Bus(context.Props, new Vector3(5.2f, 0f, 1092f), 24f,
                assets.Metal, assets.CoolWindow, assets.LampCool, assets.SnowRoof);
            MapPropsFactory.Truck(context.Props, new Vector3(-7.2f, 0f, 872f), -12f,
                assets.Concrete, assets.Metal, assets.LampWarm, assets.SnowRoof);

            MapZoneCommon.Lamps(context, assets, 830f, 1174f, 34f, true);
            MapZoneCommon.SnowBanks(context, assets, 822f, 1178f, 22, 305);
            MapZoneCommon.ParkedCars(context, assets, 836f, 1168f, 10, 306, assets.Metal);
            MapZoneCommon.DebrisField(context, assets, 900f, 1170f, 18, 307, 11f);

            return context.Finish(assets, "Financiero", MapPalette.CentroEnd, MapPalette.FinancieroEnd);
        }

        private static void BuildCheckpoint(MapZoneContext context, MapAssets assets)
        {
            var checkpoint = MapObjects.Empty("SM_Checkpoint", context.Landmarks, new Vector3(0f, 0f, 1000f));

            MapObjects.Box(checkpoint.transform, "PylonL", new Vector3(-17.5f, 15f, 0f), new Vector3(8f, 30f, 8f), assets.Concrete, true, true);
            MapObjects.Box(checkpoint.transform, "PylonR", new Vector3(17.5f, 15f, 0f), new Vector3(8f, 30f, 8f), assets.Concrete, true, true);
            MapObjects.Box(checkpoint.transform, "Beam", new Vector3(0f, 27f, 0f), new Vector3(54f, 2.2f, 4.4f), assets.Metal, false, true);
            MapObjects.Box(checkpoint.transform, "LightBar", new Vector3(0f, 9.5f, 0f), new Vector3(36f, 0.35f, 0.35f), assets.LampCool, false, false);
            MapObjects.Box(checkpoint.transform, "ScreenL", new Vector3(-13.4f, 22f, -4.1f), new Vector3(0.4f, 4.6f, 7f), assets.LampCool, false, false);
            MapObjects.Box(checkpoint.transform, "ScreenR", new Vector3(13.4f, 22f, -4.1f), new Vector3(0.4f, 4.6f, 7f), assets.LampCool, false, false);

            MapPropsFactory.Cable(checkpoint.transform, "SM_CheckpointCableA",
                new Vector3(-17.5f, 30f, 0f), new Vector3(0f, 27.5f, 0f), 0.8f, assets.Metal, 10);
            MapPropsFactory.Cable(checkpoint.transform, "SM_CheckpointCableB",
                new Vector3(17.5f, 30f, 0f), new Vector3(0f, 27.5f, 0f), 0.8f, assets.Metal, 10);
        }

        private static void BuildSkybridge(MapZoneContext context, MapAssets assets, float z, float height)
        {
            var bridge = MapObjects.Empty("SM_Skybridge", context.Landmarks, new Vector3(0f, height, z));

            MapObjects.Box(bridge.transform, "Deck", Vector3.zero, new Vector3(48f, 3.4f, 6.4f), assets.Metal, false, true);
            MapObjects.Box(bridge.transform, "Glow", new Vector3(0f, -1.8f, 0f), new Vector3(42f, 0.18f, 0.5f), assets.LampCool, false, false);
            MapObjects.Box(bridge.transform, "RailL", new Vector3(-18f, 2.1f, 0f), new Vector3(0.3f, 0.9f, 6.4f), assets.Concrete, false, true);
            MapObjects.Box(bridge.transform, "RailR", new Vector3(18f, 2.1f, 0f), new Vector3(0.3f, 0.9f, 6.4f), assets.Concrete, false, true);
        }

        private static void BuildBillboard(MapZoneContext context, MapAssets assets)
        {
            var billboard = MapObjects.Empty("SM_Billboard", context.Landmarks, new Vector3(24f, 0f, 980f));

            MapObjects.Box(billboard.transform, "Frame", new Vector3(-4.5f, 48f, 0f), new Vector3(0.8f, 15f, 24f), assets.Metal, false, true);
            MapObjects.Box(billboard.transform, "Screen", new Vector3(-5f, 48f, 0f), new Vector3(0.4f, 13f, 21f), assets.AlienVein, false, false);
            MapObjects.Box(billboard.transform, "Backing", new Vector3(-3.8f, 48f, 0f), new Vector3(0.4f, 14.4f, 22.6f), assets.Metal, false, true);
        }

        private static void BuildCrane(MapZoneContext context, MapAssets assets)
        {
            var crane = MapObjects.Empty("SM_Crane", context.Landmarks, new Vector3(52f, 0f, 1118f));

            MapObjects.Box(crane.transform, "Mast", new Vector3(0f, 32f, 0f), new Vector3(2.4f, 64f, 2.4f), assets.Metal, false, true);
            MapObjects.Box(crane.transform, "Jib", new Vector3(-14f, 62f, 0f), new Vector3(34f, 1.6f, 2f), assets.Metal, false, true);
            MapObjects.Box(crane.transform, "CounterJib", new Vector3(9f, 62f, 0f), new Vector3(10f, 1.4f, 1.8f), assets.Metal, false, true);
            MapObjects.Box(crane.transform, "Cabin", new Vector3(-2.5f, 58f, 0f), new Vector3(3.2f, 3f, 3f), assets.Concrete, false, true);
            MapObjects.Box(crane.transform, "Beacon", new Vector3(-30f, 62f, 0f), new Vector3(0.7f, 0.7f, 0.7f), assets.LampWarm, false, false);
        }
    }
}
