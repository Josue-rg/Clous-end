using ClousEnd.Login;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ClousEnd.EditorTools
{
    /// <summary>Cielo, cámara, luces, terreno, montañas y nieve del login.</summary>
    internal static class LoginEnvironmentBuilder
    {
        public static Camera Build(Transform root, LoginAssets assets)
        {
            var camera = CreateCamera(root);
            CreateLights(root);
            CreateVolume(root, assets);
            CreateSkyAndFog(assets);
            CreateGround(root, assets);
            CreateMountains(root, assets);
            CreateMoon(root, assets);
            CreateAlienSpire(root, assets);
            CreateSnowfall(root, assets);
            LoginVillageBuilder.Build(root, assets);
            return camera;
        }

        private static Camera CreateCamera(Transform root)
        {
            var go = new GameObject("Main Camera");
            go.tag = "MainCamera";
            go.transform.SetParent(root, false);
            go.transform.localPosition = LoginPalette.CameraPosition;
            go.transform.localRotation = Quaternion.Euler(LoginPalette.CameraEuler);

            var camera = go.AddComponent<Camera>();
            camera.fieldOfView = LoginPalette.CameraFieldOfView;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 500f;
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.allowHDR = true;
            camera.allowMSAA = true;

            var data = go.GetComponent<UniversalAdditionalCameraData>();
            if (data == null)
                data = go.AddComponent<UniversalAdditionalCameraData>();

            data.renderPostProcessing = true;
            data.renderShadows = true;
            data.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;

            go.AddComponent<AudioListener>();

            var drift = go.AddComponent<CinematicCameraDrift>();
            SerializedField.Set(drift, "positionAmplitude", 0.24f);
            SerializedField.Set(drift, "positionFrequency", 0.055f);
            SerializedField.Set(drift, "rotationAmplitude", 0.32f);
            SerializedField.Set(drift, "rotationFrequency", 0.04f);

            return camera;
        }

        private static void CreateLights(Transform root)
        {
            var keyGo = new GameObject("Moon Key Light");
            keyGo.transform.SetParent(root, false);
            keyGo.transform.localRotation = Quaternion.Euler(32f, 18f, 0f);

            var key = keyGo.AddComponent<Light>();
            key.type = LightType.Directional;
            key.color = new Color(0.72f, 0.82f, 0.98f);
            key.intensity = 1.2f;
            key.shadows = LightShadows.Soft;
            key.shadowStrength = 0.58f;
            key.shadowNormalBias = 0.25f;

            RenderSettings.sun = key;

            var rimGo = new GameObject("Rim Light");
            rimGo.transform.SetParent(root, false);
            rimGo.transform.localRotation = Quaternion.Euler(18f, 200f, 0f);

            var rim = rimGo.AddComponent<Light>();
            rim.type = LightType.Directional;
            rim.color = new Color(0.28f, 0.42f, 0.68f);
            rim.intensity = 0.35f;
            rim.shadows = LightShadows.None;

            var villageGo = new GameObject("Village Warm Light");
            villageGo.transform.SetParent(root, false);
            villageGo.transform.localPosition = new Vector3(0f, 7f, 34f);

            var village = villageGo.AddComponent<Light>();
            village.type = LightType.Point;
            village.color = new Color(1f, 0.63f, 0.32f);
            village.intensity = 2.4f;
            village.range = 34f;
            village.shadows = LightShadows.None;
        }

        private static void CreateVolume(Transform root, LoginAssets assets)
        {
            var go = new GameObject("Global Volume");
            go.transform.SetParent(root, false);

            var volume = go.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 1f;
            volume.weight = 1f;
            volume.sharedProfile = assets.Volume;
        }

        private static void CreateSkyAndFog(LoginAssets assets)
        {
            RenderSettings.skybox = assets.Sky;
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.15f, 0.21f, 0.34f);
            RenderSettings.ambientEquatorColor = new Color(0.07f, 0.1f, 0.16f);
            RenderSettings.ambientGroundColor = new Color(0.02f, 0.03f, 0.05f);
            RenderSettings.ambientIntensity = 1.12f;
            RenderSettings.reflectionIntensity = 0.55f;

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogDensity = 0.0062f;
            RenderSettings.fogColor = new Color(0.045f, 0.075f, 0.125f);
        }

        private static void CreateGround(Transform root, LoginAssets assets)
        {
            LoginSceneObjects.Primitive(
                "SnowGround",
                PrimitiveType.Plane,
                root,
                new Vector3(0f, 0f, 45f),
                Vector3.zero,
                new Vector3(42f, 1f, 42f),
                assets.SnowGround);

            LoginSceneObjects.Primitive(
                "VillagePath",
                PrimitiveType.Cube,
                root,
                new Vector3(0f, 0.025f, 22f),
                Vector3.zero,
                new Vector3(7f, 0.05f, 58f),
                assets.IcePath);

            LoginSceneObjects.Primitive(
                "Plaza",
                PrimitiveType.Cylinder,
                root,
                new Vector3(12.5f, 0.04f, 7f),
                Vector3.zero,
                new Vector3(10f, 0.03f, 10f),
                assets.IcePath);

            var drifts = new[]
            {
                new Vector4(-7f, 0.35f, 12f, 4.5f),
                new Vector4(7f, 0.28f, 17f, 3.8f),
                new Vector4(-12f, 0.45f, 29f, 5.2f),
                new Vector4(18f, 0.4f, 21f, 4.4f),
                new Vector4(-27f, 0.5f, 40f, 6f),
                new Vector4(35f, 0.45f, 43f, 5f),
                new Vector4(-4f, 0.32f, 72f, 5.5f),
                new Vector4(30f, 0.38f, 76f, 4.8f)
            };

            for (var i = 0; i < drifts.Length; i++)
            {
                var d = drifts[i];
                LoginSceneObjects.Primitive(
                    "SnowDrift_" + i,
                    PrimitiveType.Sphere,
                    root,
                    new Vector3(d.x, d.y * 0.35f, d.z),
                    new Vector3(0f, i * 31f, 0f),
                    new Vector3(d.w, d.y, d.w * 0.55f),
                    assets.SnowGround);
            }
        }

        private static void CreateMountains(Transform root, LoginAssets assets)
        {
            var mountains = new[]
            {
                new Vector4(-95f, 55f, 185f, 150f),
                new Vector4(-20f, 62f, 215f, 130f),
                new Vector4(65f, 70f, 200f, 165f),
                new Vector4(140f, 46f, 185f, 120f),
                new Vector4(0f, 78f, 245f, 185f),
                new Vector4(-165f, 40f, 180f, 110f)
            };

            for (var i = 0; i < mountains.Length; i++)
            {
                var m = mountains[i];
                LoginSceneObjects.Mesh(
                    "Mountain_" + i,
                    LoginMeshFactory.Mountain,
                    root,
                    new Vector3(m.x, -1.5f, m.z),
                    new Vector3(0f, i * 37f, 0f),
                    new Vector3(m.w, m.y, m.w * 0.72f),
                    assets.Mountain,
                    false);

                LoginSceneObjects.Mesh(
                    "MountainSnow_" + i,
                    LoginMeshFactory.PineCone,
                    root,
                    new Vector3(m.x, -1.5f + m.y * 0.665f, m.z),
                    new Vector3(0f, i * 37f, 0f),
                    new Vector3(m.w * 0.34f, m.y * 0.34f, m.w * 0.34f * 0.72f),
                    assets.SnowRoof,
                    false);
            }
        }

        private static void CreateMoon(Transform root, LoginAssets assets)
        {
            LoginSceneObjects.Primitive(
                "MoonGlow",
                PrimitiveType.Quad,
                root,
                new Vector3(24f, 21f, 113.5f),
                Vector3.zero,
                new Vector3(46f, 46f, 1f),
                assets.MoonGlow,
                false);

            LoginSceneObjects.Primitive(
                "Moon",
                PrimitiveType.Sphere,
                root,
                new Vector3(24f, 21f, 115f),
                Vector3.zero,
                Vector3.one * 13f,
                assets.Moon,
                false);
        }

        private static void CreateAlienSpire(Transform root, LoginAssets assets)
        {
            LoginSceneObjects.Mesh(
                "AlienSpire_A",
                LoginMeshFactory.PineCone,
                root,
                new Vector3(54f, 0f, 150f),
                new Vector3(0f, 15f, 0f),
                new Vector3(5f, 72f, 5f),
                assets.Alien,
                false);

            LoginSceneObjects.Mesh(
                "AlienSpire_B",
                LoginMeshFactory.PineCone,
                root,
                new Vector3(66f, 0f, 163f),
                new Vector3(0f, -8f, 0f),
                new Vector3(3f, 50f, 3f),
                assets.Alien,
                false);

            LoginSceneObjects.Primitive(
                "AlienRing",
                PrimitiveType.Cylinder,
                root,
                new Vector3(54f, 45f, 150f),
                Vector3.zero,
                new Vector3(5.4f, 0.06f, 5.4f),
                assets.LightCool,
                false);
        }

        private static void CreateSnowfall(Transform root, LoginAssets assets)
        {
            var go = new GameObject("Snowfall");
            go.transform.SetParent(root, false);
            go.transform.localPosition = new Vector3(0f, 11f, 25f);

            var ps = go.AddComponent<ParticleSystem>();

            var main = ps.main;
            main.loop = true;
            main.prewarm = true;
            main.startLifetime = 15f;
            main.startSpeed = 0.4f;
            main.startSize = new ParticleSystem.MinMaxCurve(0.02f, 0.055f);
            main.startColor = new Color(0.86f, 0.93f, 1f, 0.85f);
            main.maxParticles = 7000;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.playOnAwake = true;

            var emission = ps.emission;
            emission.rateOverTime = 600f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(110f, 8f, 110f);

            var velocity = ps.velocityOverLifetime;
            velocity.enabled = true;
            velocity.space = ParticleSystemSimulationSpace.World;
            velocity.x = new ParticleSystem.MinMaxCurve(-0.55f, -0.15f);
            velocity.y = new ParticleSystem.MinMaxCurve(-1.25f, -0.8f);
            velocity.z = new ParticleSystem.MinMaxCurve(-0.05f, 0.05f);

            var noise = ps.noise;
            noise.enabled = true;
            noise.strength = 0.38f;
            noise.frequency = 0.12f;
            noise.scrollSpeed = 0.04f;

            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.sharedMaterial = assets.SnowParticle;
            renderer.maxParticleSize = 0.12f;
            renderer.sortingFudge = -5f;
        }
    }
}
