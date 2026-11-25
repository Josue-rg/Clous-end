using UnityEngine;
using ClousEnd.World;

namespace ClousEnd.EditorTools
{
    /// <summary>
    /// Contexto de una zona del mapa: su raíz, sus subcarpetas y los acumuladores
    /// de ventanas. Al terminar, genera los draw calls de fachada y devuelve el
    /// descriptor que usa el ZoneStreamer.
    /// </summary>
    internal sealed class MapZoneContext
    {
        public readonly GameObject Root;
        public readonly Transform Buildings;
        public readonly Transform Props;
        public readonly Transform Landmarks;
        public readonly WindowMeshBuilder WarmWindows = new WindowMeshBuilder();
        public readonly WindowMeshBuilder CoolWindows = new WindowMeshBuilder();

        public MapZoneContext(string name, Transform parent)
        {
            Root = new GameObject(name);
            Root.transform.SetParent(parent, false);

            Buildings = new GameObject("Buildings").transform;
            Buildings.SetParent(Root.transform, false);

            Props = new GameObject("Props").transform;
            Props.SetParent(Root.transform, false);

            Landmarks = new GameObject("Landmarks").transform;
            Landmarks.SetParent(Root.transform, false);
        }

        public ZoneStreamer.Zone Finish(
            MapAssets assets,
            string label,
            float zStart,
            float zEnd,
            float cullDistance = 760f)
        {
            MapPropCombiner.Combine(this, label);

            var windowsRoot = new GameObject("Windows").transform;
            windowsRoot.SetParent(Root.transform, false);

            if (WarmWindows.Count > 0)
            {
                var mesh = MapMeshFactory.SaveZoneMesh(label + "_WarmWindows", WarmWindows.Build(label + "_WarmWindows"));
                MapObjects.MeshRenderer(windowsRoot, "WindowsWarm", mesh, assets.WarmWindow);
            }

            if (CoolWindows.Count > 0)
            {
                var mesh = MapMeshFactory.SaveZoneMesh(label + "_CoolWindows", CoolWindows.Build(label + "_CoolWindows"));
                MapObjects.MeshRenderer(windowsRoot, "WindowsCool", mesh, assets.CoolWindow);
            }

            return new ZoneStreamer.Zone
            {
                label = label,
                root = Root.transform,
                bounds = new Bounds(
                    new Vector3(0f, 60f, (zStart + zEnd) * 0.5f),
                    new Vector3(480f, 320f, Mathf.Max(1f, zEnd - zStart))),
                cullDistance = cullDistance
            };
        }
    }
}
