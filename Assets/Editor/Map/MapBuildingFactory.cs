using System.Collections.Generic;
using UnityEngine;

namespace ClousEnd.EditorTools
{
    internal enum MapBuildingStyle
    {
        Industrial,
        Historic,
        Tower,
        Refuge
    }

    /// <summary>
    /// Fábrica de edificios del blockout. Cada edificio tiene un LOD0 con
    /// detalles y un LOD1 con solo el volumen, para que la ciudad lejana se
    /// simplifique antes de que el ZoneStreamer la apague.
    /// </summary>
    internal static class MapBuildingFactory
    {
        public static GameObject Create(
            Transform parent,
            string name,
            Vector3 baseCenter,
            Vector2 footprint,
            float height,
            MapBuildingStyle style,
            Material wall,
            Material trim,
            Material snow,
            Material beacon,
            float facadeSign,
            WindowMeshBuilder windows,
            System.Random random,
            bool withWindows = true)
        {
            var root = MapObjects.Empty(name, parent, baseCenter);
            var width = Mathf.Max(6f, footprint.x);
            var depth = Mathf.Max(6f, footprint.y);

            var mass = MapObjects.Box(
                root.transform,
                "Mass",
                new Vector3(0f, height * 0.5f, 0f),
                new Vector3(width, height, depth),
                wall,
                true,
                true,
                false);

            var trimBand = MapObjects.Box(
                root.transform,
                "TopTrim",
                new Vector3(0f, height - 0.5f, 0f),
                new Vector3(width + 0.5f, 0.9f, depth + 0.5f),
                trim,
                false,
                true,
                false);

            var lod0 = new List<Renderer>
            {
                mass.GetComponent<Renderer>(),
                trimBand.GetComponent<Renderer>()
            };

            AddRoof(root.transform, style, width, depth, height, trim, snow, beacon, lod0, random, facadeSign);

            if (withWindows && windows != null)
            {
                var chance = style == MapBuildingStyle.Historic ? 0.62f
                    : style == MapBuildingStyle.Industrial ? 0.42f
                    : 0.5f;

                windows.AddFacade(
                    baseCenter,
                    width,
                    depth,
                    height,
                    facadeSign,
                    random,
                    chance,
                    style == MapBuildingStyle.Industrial ? 2f : 1.45f,
                    style == MapBuildingStyle.Historic ? 1.15f : 1.35f);

                // Testeros: son las caras que más se ven al mirar a lo largo de la calle.
                windows.AddEndFacade(baseCenter, width, depth, height, 1f, random, chance * 0.72f, 1.45f, 1.15f);
                windows.AddEndFacade(baseCenter, width, depth, height, -1f, random, chance * 0.72f, 1.45f, 1.15f);

                if (style == MapBuildingStyle.Historic && random.NextDouble() < 0.65)
                    windows.AddStorefront(baseCenter, width, depth, facadeSign);
            }

            MapObjects.AddLods(root, lod0.ToArray(), new[] { mass.GetComponent<Renderer>() });
            return root;
        }

        private static void AddRoof(
            Transform parent,
            MapBuildingStyle style,
            float width,
            float depth,
            float height,
            Material trim,
            Material snow,
            Material beacon,
            List<Renderer> lod0,
            System.Random random,
            float facadeSign)
        {
            if (style == MapBuildingStyle.Historic)
            {
                var roofHeight = 2.8f + (float)random.NextDouble() * 3.2f;
                var roof = MapObjects.Mesh(
                    parent,
                    "Roof",
                    MapMeshFactory.Pyramid,
                    new Vector3(0f, height, 0f),
                    new Vector3(0f, 45f, 0f),
                    new Vector3(width * 1.414f, roofHeight, depth * 1.414f),
                    snow,
                    false,
                    false);
                lod0.Add(roof.GetComponent<Renderer>());

                var chimneyHeight = 2.5f + (float)random.NextDouble() * 1.8f;
                var chimney = MapObjects.Box(
                    parent,
                    "Chimney",
                    new Vector3(width * 0.24f * facadeSign, height + chimneyHeight * 0.5f, depth * 0.22f),
                    new Vector3(1.2f, chimneyHeight, 1.2f),
                    trim,
                    false,
                    true,
                    false);
                lod0.Add(chimney.GetComponent<Renderer>());

                var cap = MapObjects.Box(
                    parent,
                    "ChimneySnow",
                    new Vector3(width * 0.24f * facadeSign, height + chimneyHeight + 0.15f, depth * 0.22f),
                    new Vector3(1.45f, 0.3f, 1.45f),
                    snow,
                    false,
                    false,
                    false);
                lod0.Add(cap.GetComponent<Renderer>());

                if (random.NextDouble() < 0.5)
                    AddAwning(parent, width, depth, facadeSign, trim, lod0);

                return;
            }

            var roofCap = MapObjects.Box(
                parent,
                "RoofCap",
                new Vector3(0f, height + 0.22f, 0f),
                new Vector3(width + 0.35f, 0.45f, depth + 0.35f),
                snow,
                false,
                false,
                false);
            lod0.Add(roofCap.GetComponent<Renderer>());

            if (style == MapBuildingStyle.Tower)
            {
                if (random.NextDouble() > 0.55)
                    return;

                var antennaHeight = 5f + (float)random.NextDouble() * 9f;
                var antenna = MapObjects.Cylinder(
                    parent,
                    "Antenna",
                    new Vector3(width * 0.2f, height + 0.4f, depth * 0.15f),
                    0.14f,
                    antennaHeight,
                    trim,
                    false,
                    false,
                    false);
                lod0.Add(antenna.GetComponent<Renderer>());

                var light = MapObjects.Box(
                    parent,
                    "Beacon",
                    new Vector3(width * 0.2f, height + antennaHeight + 0.3f, depth * 0.15f),
                    new Vector3(0.55f, 0.55f, 0.55f),
                    beacon,
                    false,
                    false,
                    false);
                lod0.Add(light.GetComponent<Renderer>());
                return;
            }

            if (random.NextDouble() >= 0.4)
                return;

            var vent = MapObjects.Box(
                parent,
                "RoofVent",
                new Vector3(width * 0.22f, height + 0.9f, depth * 0.2f),
                new Vector3(1.6f, 1.2f, 1.6f),
                trim,
                false,
                false,
                false);
            lod0.Add(vent.GetComponent<Renderer>());
        }

        private static void AddAwning(
            Transform parent,
            float width,
            float depth,
            float facadeSign,
            Material trim,
            List<Renderer> lod0)
        {
            var awning = MapObjects.Box(
                parent,
                "Awning",
                new Vector3(facadeSign * (width * 0.5f + 0.75f), 3.6f, 0f),
                new Vector3(1.7f, 0.12f, depth * 0.55f),
                trim,
                false,
                true,
                false,
                Quaternion.Euler(0f, 0f, facadeSign * -16f));
            lod0.Add(awning.GetComponent<Renderer>());
        }
    }
}
