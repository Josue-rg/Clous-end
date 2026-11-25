using UnityEngine;
using ClousEnd.World;

namespace ClousEnd.EditorTools
{
    /// <summary>Zona 4: Refugio Aurora. Explanada, cúpula, faro y presencia Yerma al fondo.</summary>
    internal static class MapZoneRefugio
    {
        public static ZoneStreamer.Zone Build(Transform zonesRoot, MapAssets assets)
        {
            var context = new MapZoneContext("Zone_Refugio", zonesRoot);

            BuildEsplanade(context, assets);
            BuildGate(context, assets);
            BuildDome(context, assets);
            BuildLighthouse(context, assets);
            BuildControlTower(context, assets);
            BuildContainers(context, assets);
            BuildTents(context, assets);
            BuildAlienHorizon(context, assets);

            MapZoneCommon.Lamps(context, assets, 1190f, 1480f, 40f, true);
            MapZoneCommon.SnowBanks(context, assets, 1182f, 1490f, 20, 401);
            MapZoneCommon.DebrisField(context, assets, 1220f, 1470f, 14, 402, 26f);
            MapZoneCommon.Pines(context, assets, 1182f, 1490f, 14, 403, 58f, 110f);

            MapPropsFactory.Barricade(context.Props, new Vector3(-10f, 0f, 1216f), 4f,
                assets.Metal, assets.RoadPaint, assets.LampWarm);
            MapPropsFactory.Barricade(context.Props, new Vector3(10f, 0f, 1216f), -4f,
                assets.Metal, assets.RoadPaint, assets.LampWarm);
            MapPropsFactory.Barricade(context.Props, new Vector3(-9.5f, 0f, 1478f), 2f,
                assets.Metal, assets.RoadPaint, assets.LampWarm);
            MapPropsFactory.Barricade(context.Props, new Vector3(9.5f, 0f, 1478f), -2f,
                assets.Metal, assets.RoadPaint, assets.LampWarm);

            for (var i = 0; i < 6; i++)
            {
                var side = i % 2 == 0 ? -1f : 1f;
                MapPropsFactory.Container(
                    context.Props,
                    new Vector3(side * 24f, 0f, 1382f + i * 19f),
                    side * 3f,
                    assets.Metal,
                    assets.SnowRoof,
                    assets.LampWarm);
            }

            return context.Finish(assets, "Refugio", MapPalette.FinancieroEnd, MapPalette.RefugioEnd, 760f);
        }

        private static void BuildEsplanade(MapZoneContext context, MapAssets assets)
        {
            var esplanade = MapObjects.Empty("SM_Esplanade", context.Landmarks, new Vector3(0f, 0f, 1360f));

            MapObjects.Box(esplanade.transform, "Floor", new Vector3(0f, 0.03f, 0f), new Vector3(180f, 0.06f, 300f), assets.Concrete, false, false);
            MapObjects.Box(esplanade.transform, "SnowPatchA", new Vector3(-42f, 0.075f, -40f), new Vector3(64f, 0.03f, 90f), assets.Snow, false, false);
            MapObjects.Box(esplanade.transform, "SnowPatchB", new Vector3(46f, 0.075f, 62f), new Vector3(58f, 0.03f, 74f), assets.Snow, false, false);

            for (var i = 0; i < 2; i++)
            {
                var ring = MapObjects.Cylinder(
                    esplanade.transform,
                    "SM_LandingRing" + i,
                    new Vector3(i == 0 ? -24f : 24f, 0.08f, i == 0 ? 1330f : 1290f),
                    9.5f,
                    0.08f,
                    assets.RefugeGlow,
                    false,
                    false);
                ring.isStatic = true;
            }

            for (var i = 0; i < 12; i++)
            {
                var side = i % 2 == 0 ? -1f : 1f;
                MapObjects.Box(context.Props, "SM_EvacLight",
                    new Vector3(side * (22f + (i / 2) * 9f), 0.55f, 1240f + (i / 2) * 44f),
                    new Vector3(0.3f, 1.1f, 0.3f),
                    assets.LampWarm,
                    false,
                    false,
                    true);
            }
        }

        private static void BuildGate(MapZoneContext context, MapAssets assets)
        {
            var gate = MapObjects.Empty("SM_RefugeGate", context.Landmarks, new Vector3(0f, 0f, 1232f));

            MapObjects.Box(gate.transform, "PillarL", new Vector3(-17.6f, 8.5f, 0f), new Vector3(3f, 17f, 3f), assets.Concrete, true, true);
            MapObjects.Box(gate.transform, "PillarR", new Vector3(17.6f, 8.5f, 0f), new Vector3(3f, 17f, 3f), assets.Concrete, true, true);
            MapObjects.Box(gate.transform, "Beam", new Vector3(0f, 17.5f, 0f), new Vector3(38f, 1.4f, 3f), assets.Metal, false, true);
            MapObjects.Box(gate.transform, "Glow", new Vector3(0f, 15.6f, -1.6f), new Vector3(24f, 0.5f, 0.4f), assets.RefugeGlow, false, false);
        }

        private static void BuildDome(MapZoneContext context, MapAssets assets)
        {
            var dome = MapObjects.Empty("SM_RefugeDome", context.Landmarks, new Vector3(0f, 0f, 1620f));

            MapObjects.Sphere(dome.transform, "Shell", Vector3.zero, new Vector3(190f, 128f, 190f), assets.Metal, true, false);
            MapObjects.Cylinder(dome.transform, "Plinth", new Vector3(0f, 0.05f, 0f), 97f, 2.4f, assets.Concrete, false, true);
            MapObjects.Cylinder(dome.transform, "GlowRing", new Vector3(0f, 2.4f, 0f), 96f, 0.5f, assets.RefugeGlow, false, false);
            MapObjects.Cylinder(dome.transform, "RibA", new Vector3(0f, 22f, 0f), 94.4f, 0.35f, assets.RefugeGlow, false, false);
            MapObjects.Cylinder(dome.transform, "RibB", new Vector3(0f, 48f, 0f), 88.6f, 0.3f, assets.RefugeGlow, false, false);
            MapObjects.Cylinder(dome.transform, "RibC", new Vector3(0f, 74f, 0f), 79.6f, 0.3f, assets.RefugeGlow, false, false);
            MapObjects.Sphere(dome.transform, "Beacon", new Vector3(0f, 130f, 0f), Vector3.one * 4.5f, assets.RefugeGlow, false, false);

            MapObjects.Box(dome.transform, "GateFrame", new Vector3(0f, 15f, -96.5f), new Vector3(44f, 32f, 2.4f), assets.Concrete, false, true);
            MapObjects.Box(dome.transform, "GateGlow", new Vector3(0f, 14f, -97.6f), new Vector3(36f, 26f, 0.6f), assets.RefugeGlow, false, false);
            MapObjects.Box(dome.transform, "GateDivider", new Vector3(0f, 14f, -98f), new Vector3(1.6f, 26f, 0.4f), assets.Metal, false, false);
        }

        private static void BuildLighthouse(MapZoneContext context, MapAssets assets)
        {
            var lighthouse = MapObjects.Empty("SM_Lighthouse", context.Landmarks, new Vector3(78f, 0f, 1500f));

            MapObjects.Cylinder(lighthouse.transform, "Base", Vector3.zero, 7f, 22f, assets.Concrete, true, true);
            MapObjects.Cylinder(lighthouse.transform, "Mid", new Vector3(0f, 22f, 0f), 5f, 20f, assets.Metal, false, true);
            MapObjects.Cylinder(lighthouse.transform, "Top", new Vector3(0f, 42f, 0f), 3.4f, 16f, assets.Concrete, false, true);
            MapObjects.Sphere(lighthouse.transform, "Lamp", new Vector3(0f, 59.5f, 0f), Vector3.one * 6.2f, assets.RefugeGlow, false, false);
            MapObjects.Mesh(lighthouse.transform, "Crown", MapMeshFactory.Pyramid, new Vector3(0f, 62f, 0f), Vector3.zero, new Vector3(5f, 9f, 5f), assets.Metal, false);
            MapObjects.Mesh(lighthouse.transform, "Beam", MapMeshFactory.Pyramid, new Vector3(0f, 66f, 0f), Vector3.zero, new Vector3(3.4f, 34f, 3.4f), assets.AuroraCool, false);
        }

        private static void BuildControlTower(MapZoneContext context, MapAssets assets)
        {
            var tower = MapObjects.Empty("SM_ControlTower", context.Landmarks, new Vector3(-82f, 0f, 1476f));

            MapObjects.Cylinder(tower.transform, "Shaft", Vector3.zero, 4.4f, 19f, assets.Concrete, true, true);
            MapObjects.Box(tower.transform, "Cabin", new Vector3(0f, 21f, 0f), new Vector3(13f, 4.4f, 13f), assets.Glass, false, true);
            MapObjects.Box(tower.transform, "CabinRoof", new Vector3(0f, 23.5f, 0f), new Vector3(14f, 0.5f, 14f), assets.Metal, false, false);
            MapObjects.Cylinder(tower.transform, "Antenna", new Vector3(0f, 24f, 0f), 0.25f, 14f, assets.Metal, false, false);
            MapObjects.Sphere(tower.transform, "Beacon", new Vector3(0f, 38.6f, 0f), Vector3.one * 0.9f, assets.LampWarm, false, false);
        }

        private static void BuildContainers(MapZoneContext context, MapAssets assets)
        {
            var random = new System.Random(410);

            for (var i = 0; i < 12; i++)
            {
                var side = i % 2 == 0 ? -1f : 1f;
                var x = side * (28f + (i / 2) * 10f);
                var z = 1258f + (i / 2) * 34f;
                var root = MapObjects.Empty("SM_Container", context.Props, new Vector3(x, 0f, z));
                root.transform.localRotation = Quaternion.Euler(0f, (float)random.NextDouble() * 12f - 6f, 0f);

                MapObjects.Box(root.transform, "Shell", new Vector3(0f, 1.6f, 0f), new Vector3(6.4f, 3.2f, 12.4f), assets.Metal, true, true);
                MapObjects.Box(root.transform, "Snow", new Vector3(0f, 3.3f, 0f), new Vector3(6.1f, 0.25f, 12f), assets.SnowRoof, false, false);
                MapObjects.Box(root.transform, "Strip", new Vector3(-3.24f, 2.3f, 0f), new Vector3(0.12f, 0.35f, 7f), assets.LampWarm, false, false);
                root.isStatic = true;
            }
        }

        private static void BuildTents(MapZoneContext context, MapAssets assets)
        {
            for (var i = 0; i < 4; i++)
            {
                var root = MapObjects.Empty("SM_Tent", context.Props, new Vector3(-38f - i * 15f, 0f, 1320f + i * 24f));
                MapObjects.Box(root.transform, "Base", new Vector3(0f, 1f, 0f), new Vector3(9f, 2f, 9f), assets.Metal, true, true);
                MapObjects.Mesh(root.transform, "Roof", MapMeshFactory.Pyramid, new Vector3(0f, 2f, 0f), new Vector3(0f, 45f, 0f), new Vector3(12.8f, 3.4f, 12.8f), assets.SnowRoof, true);
                MapObjects.Box(root.transform, "Window", new Vector3(4.55f, 1.2f, 0f), new Vector3(0.12f, 0.8f, 3f), assets.LampWarm, false, false);
                root.isStatic = true;
            }
        }

        private static void BuildAlienHorizon(MapZoneContext context, MapAssets assets)
        {
            var horizon = MapObjects.Empty("SM_AlienHorizon", context.Landmarks, new Vector3(0f, 0f, 1760f));

            MapObjects.Mesh(horizon.transform, "SpireA", MapMeshFactory.Mountain, new Vector3(-70f, 0f, 0f), new Vector3(0f, 20f, 0f), new Vector3(150f, 300f, 150f), assets.AlienObsidian, false);
            MapObjects.Mesh(horizon.transform, "SpireB", MapMeshFactory.Mountain, new Vector3(20f, 0f, 60f), new Vector3(0f, -40f, 0f), new Vector3(190f, 360f, 190f), assets.AlienObsidian, false);
            MapObjects.Mesh(horizon.transform, "SpireC", MapMeshFactory.Mountain, new Vector3(130f, 0f, 20f), new Vector3(0f, 70f, 0f), new Vector3(120f, 240f, 120f), assets.AlienObsidian, false);

            MapObjects.Cylinder(horizon.transform, "VeinA", new Vector3(-70f, 190f, 0f), 16f, 1.2f, assets.AlienVein, false, false);
            MapObjects.Cylinder(horizon.transform, "VeinB", new Vector3(20f, 235f, 60f), 20f, 1.4f, assets.AlienVein, false, false);
            MapObjects.Sphere(horizon.transform, "Eye", new Vector3(20f, 268f, 60f), Vector3.one * 26f, assets.AlienVein, false, false);
        }
    }
}
