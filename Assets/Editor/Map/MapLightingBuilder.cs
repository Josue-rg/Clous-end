using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ClousEnd.EditorTools
{
    /// <summary>Luna, luces prácticas, niebla y postproceso del mapa.</summary>
    internal static class MapLightingBuilder
    {
        public static void Build(Transform root, MapAssets assets)
        {
            var lighting = new GameObject("Lighting").transform;
            lighting.SetParent(root, false);

            var moonGo = new GameObject("Moon Key Light");
            moonGo.transform.SetParent(lighting, false);
            moonGo.transform.localRotation = Quaternion.Euler(38f, 26f, 0f);

            var moon = moonGo.AddComponent<Light>();
            moon.type = LightType.Directional;
            moon.color = new Color(0.78f, 0.86f, 1f);
            moon.intensity = 2.1f;
            moon.shadows = LightShadows.Soft;
            moon.shadowStrength = 0.5f;
            moon.shadowNormalBias = 0.35f;
            moon.shadowBias = 0.06f;
            RenderSettings.sun = moon;

            PointLight(lighting, "Plaza Tree Light", new Vector3(-30f, 14f, 556f), new Color(1f, 0.7f, 0.38f), 5.5f, 64f);
            PointLight(lighting, "Plaza Fountain Light", new Vector3(-30f, 7f, 462f), new Color(0.42f, 0.88f, 1f), 3.2f, 42f);
            PointLight(lighting, "Checkpoint Light", new Vector3(0f, 14f, 1000f), new Color(0.45f, 0.92f, 1f), 4.4f, 68f);
            PointLight(lighting, "Refuge Light", new Vector3(0f, 14f, 1330f), new Color(0.4f, 1f, 0.82f), 4.2f, 78f);

            var volumeGo = new GameObject("Global Volume");
            volumeGo.transform.SetParent(lighting, false);

            var volume = volumeGo.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 1f;
            volume.weight = 1f;
            volume.sharedProfile = assets.Volume;

            if (assets.Sky != null)
                RenderSettings.skybox = assets.Sky;

            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.28f, 0.36f, 0.5f);
            RenderSettings.ambientEquatorColor = new Color(0.16f, 0.21f, 0.3f);
            RenderSettings.ambientGroundColor = new Color(0.07f, 0.09f, 0.13f);
            RenderSettings.ambientIntensity = 1.65f;
            RenderSettings.reflectionIntensity = 0.55f;

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.0022f;
            RenderSettings.fogColor = MapPalette.Fog;
        }

        private static void PointLight(Transform parent, string name, Vector3 position, Color color, float intensity, float range)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = position;

            var light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = color;
            light.intensity = intensity;
            light.range = range;
            light.shadows = LightShadows.None;
        }
    }
}
