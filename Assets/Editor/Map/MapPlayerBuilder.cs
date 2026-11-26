using ClousEnd.Game;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ClousEnd.EditorTools
{
    internal sealed class MapPlayerResult
    {
        public GameObject Player;
        public Camera Camera;
        public MapUiRefs Ui;
    }

    /// <summary>
    /// Crea el vehículo y el personaje de pruebas (cubo y cilindro), la cámara
    /// en tercera persona, el HUD y los efectos de disparo. Todo queda en la
    /// escena para que se pueda reemplazar por los modelos de Blender.
    /// </summary>
    internal static class MapPlayerBuilder
    {
        public static MapPlayerResult Build(Transform parent, MapAssets assets)
        {
            var player = new GameObject("Player");
            player.transform.SetParent(parent, false);
            player.transform.localPosition = new Vector3(0f, 9f, 8f);

            var controller = player.AddComponent<CharacterController>();
            controller.radius = 1.9f;
            controller.height = 3.4f;
            controller.center = new Vector3(0f, 1.7f, 0f);
            controller.slopeLimit = 46f;
            controller.stepOffset = 0.5f;
            controller.skinWidth = 0.08f;
            controller.minMoveDistance = 0f;

            var input = player.AddComponent<PlayerInputReader>();
            var motor = player.AddComponent<VehicleMotor>();

            BuildVehicleVisual(player.transform, assets);
            var character = BuildCharacter(player.transform, assets);
            var camera = BuildCamera(parent, character);

            var effects = BuildEffects(parent, assets);
            var ui = MapUIBuilder.Build(parent, assets, input, character);
            BuildWeather(parent, assets, player.transform);

            var aim = character.GetComponent<CharacterAim>();
            SerializedField.Set(aim, "input", input);
            SerializedField.Set(aim, "aimCamera", camera);
            SerializedField.Set(aim, "body", character);
            SerializedField.Set(aim, "aimPivot", character.Find("AimPivot"));
            SerializedField.Set(aim, "turnSharpness", 18f);
            SerializedField.Set(aim, "maxAimDistance", 520f);
            SerializedField.Set(aim, "aimMask", Physics.DefaultRaycastLayers);

            var view = camera.GetComponent<ThirdPersonCamera>();

            SerializedField.Set(motor, "input", input);
            SerializedField.Set(motor, "view", view);
            SerializedField.Set(motor, "forwardSpeed", 24f);
            SerializedField.Set(motor, "reverseSpeed", 11f);
            SerializedField.Set(motor, "strafeSpeed", 16f);
            SerializedField.Set(motor, "acceleration", 58f);
            SerializedField.Set(motor, "deceleration", 78f);
            SerializedField.Set(motor, "gravity", -30f);
            SerializedField.Set(motor, "groundedStick", -2f);
            SerializedField.Set(motor, "turnSharpness", 12f);
            SerializedField.Set(motor, "flying", true);
            SerializedField.Set(motor, "verticalSpeed", 13f);
            SerializedField.Set(motor, "verticalAcceleration", 45f);

            var weapon = character.GetComponent<HitscanWeapon>();
            SerializedField.Set(weapon, "input", input);
            SerializedField.Set(weapon, "aimCamera", camera);
            SerializedField.Set(weapon, "muzzle", character.Find("AimPivot/Muzzle"));
            SerializedField.Set(weapon, "tracer", effects.Tracer);
            SerializedField.Set(weapon, "impact", effects.Impact);
            SerializedField.Set(weapon, "range", 320f);
            SerializedField.Set(weapon, "cooldown", 0.14f);
            SerializedField.Set(weapon, "automatic", false);
            SerializedField.Set(weapon, "hitMask", Physics.DefaultRaycastLayers);

            SerializedField.Set(view, "target", character);
            SerializedField.Set(view, "input", input);
            SerializedField.Set(view, "targetOffset", new Vector3(0f, 1.35f, 0f));
            SerializedField.Set(view, "distance", 11.5f);
            SerializedField.Set(view, "minDistance", 4.5f);
            SerializedField.Set(view, "maxDistance", 17f);
            SerializedField.Set(view, "yawSensitivity", 0.18f);
            SerializedField.Set(view, "pitchSensitivity", 0.14f);
            SerializedField.Set(view, "minPitch", -6f);
            SerializedField.Set(view, "maxPitch", 62f);
            SerializedField.Set(view, "initialYaw", 0f);
            SerializedField.Set(view, "initialPitch", 18f);
            SerializedField.Set(view, "followTime", 0.08f);
            SerializedField.Set(view, "collisionRadius", 0.45f);
            SerializedField.Set(view, "collisionMask", Physics.DefaultRaycastLayers);
            SerializedField.Set(view, "zoomSensitivity", 0.02f);

            MapObjects.SetLayerRecursive(player, LayerMask.NameToLayer("Ignore Raycast"));

            return new MapPlayerResult { Player = player, Camera = camera, Ui = ui };
        }

        private static void BuildVehicleVisual(Transform player, MapAssets assets)
        {
            MapObjects.Box(
                player,
                "PH_CarBody",
                new Vector3(0f, 0.75f, 0f),
                new Vector3(3.2f, 1.4f, 4.4f),
                assets.CarPaint,
                false,
                true,
                false);

            MapObjects.Box(
                player,
                "PH_HeadlightL",
                new Vector3(-1f, 0.7f, 2.22f),
                new Vector3(0.5f, 0.2f, 0.08f),
                assets.LampWarm,
                false,
                false,
                false);

            MapObjects.Box(
                player,
                "PH_HeadlightR",
                new Vector3(1f, 0.7f, 2.22f),
                new Vector3(0.5f, 0.2f, 0.08f),
                assets.LampWarm,
                false,
                false,
                false);
        }

        private static Transform BuildCharacter(Transform player, MapAssets assets)
        {
            var pivot = MapObjects.Empty("CharacterPivot", player, new Vector3(0f, 1.55f, -0.9f)).transform;
            pivot.gameObject.AddComponent<CharacterAim>();
            pivot.gameObject.AddComponent<HitscanWeapon>();

            MapObjects.Cylinder(
                pivot,
                "PH_Character",
                Vector3.zero,
                0.42f,
                1.75f,
                assets.Character,
                false,
                true,
                false);

            var aimPivot = MapObjects.Empty("AimPivot", pivot, new Vector3(0f, 1.28f, 0f)).transform;

            MapObjects.Box(
                aimPivot,
                "PH_Gun",
                new Vector3(0f, 0f, 0.58f),
                new Vector3(0.22f, 0.22f, 1.15f),
                assets.Gunmetal,
                false,
                true,
                false);

            MapObjects.Empty("Muzzle", aimPivot, new Vector3(0f, 0f, 1.2f));
            return pivot;
        }

        private static Camera BuildCamera(Transform parent, Transform target)
        {
            var go = new GameObject("Player Camera");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = new Vector3(0f, 8f, -12f);
            go.transform.localRotation = Quaternion.Euler(10f, 0f, 0f);
            go.tag = "MainCamera";

            var camera = go.AddComponent<Camera>();
            camera.fieldOfView = 62f;
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 2200f;
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.allowHDR = true;
            camera.allowMSAA = true;
            camera.cullingMask = ~0;

            var data = go.GetComponent<UniversalAdditionalCameraData>();

            if (data == null)
                data = go.AddComponent<UniversalAdditionalCameraData>();

            data.renderPostProcessing = true;
            data.renderShadows = true;
            data.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;

            go.AddComponent<AudioListener>();
            go.AddComponent<ThirdPersonCamera>();
            return camera;
        }

        private static MapEffects BuildEffects(Transform parent, MapAssets assets)
        {
            var effects = new GameObject("Effects").transform;
            effects.SetParent(parent, false);

            var tracerGo = new GameObject("Tracer", typeof(LineRenderer));
            tracerGo.transform.SetParent(effects, false);

            var line = tracerGo.GetComponent<LineRenderer>();
            line.sharedMaterial = assets.Tracer;
            line.useWorldSpace = true;
            line.positionCount = 2;
            line.SetPosition(0, Vector3.zero);
            line.SetPosition(1, Vector3.forward);
            line.startWidth = 0.07f;
            line.endWidth = 0.07f;
            line.numCapVertices = 2;
            line.alignment = LineAlignment.View;
            line.shadowCastingMode = ShadowCastingMode.Off;
            line.receiveShadows = false;
            line.textureMode = LineTextureMode.Stretch;
            line.enabled = false;

            var tracer = tracerGo.AddComponent<TracerBeam>();
            SerializedField.Set(tracer, "lifetime", 0.07f);
            SerializedField.Set(tracer, "width", 0.07f);

            var impactGo = MapObjects.Sphere(
                effects,
                "Impact",
                new Vector3(0f, 3f, 14f),
                Vector3.one * 0.5f,
                assets.Impact,
                false,
                false,
                false);

            var impactRenderer = impactGo.GetComponent<Renderer>();

            if (impactRenderer != null)
                impactRenderer.enabled = false;

            var impact = impactGo.AddComponent<ImpactFlash>();
            SerializedField.Set(impact, "lifetime", 0.16f);
            SerializedField.Set(impact, "startScale", 0.55f);
            SerializedField.Set(impact, "endScale", 0.04f);

            return new MapEffects
            {
                Tracer = tracer,
                Impact = impact
            };
        }

        private static void BuildWeather(Transform parent, MapAssets assets, Transform player)
        {
            var go = new GameObject("Snowfall");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = player.localPosition + new Vector3(0f, 12f, 12f);

            var ps = go.AddComponent<ParticleSystem>();

            var main = ps.main;
            main.loop = true;
            main.prewarm = true;
            main.startLifetime = 13f;
            main.startSpeed = 0.35f;
            main.startSize = new ParticleSystem.MinMaxCurve(0.018f, 0.055f);
            main.startColor = new Color(0.86f, 0.93f, 1f, 0.82f);
            main.maxParticles = 5000;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.playOnAwake = true;

            var emission = ps.emission;
            emission.rateOverTime = 620f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(96f, 18f, 96f);

            var velocity = ps.velocityOverLifetime;
            velocity.enabled = true;
            velocity.space = ParticleSystemSimulationSpace.World;
            velocity.x = new ParticleSystem.MinMaxCurve(-0.55f, -0.15f);
            velocity.y = new ParticleSystem.MinMaxCurve(-1.35f, -0.85f);
            velocity.z = new ParticleSystem.MinMaxCurve(-0.08f, 0.08f);

            var noise = ps.noise;
            noise.enabled = true;
            noise.strength = 0.35f;
            noise.frequency = 0.12f;
            noise.scrollSpeed = 0.05f;

            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.sharedMaterial = assets.SnowParticle;
            renderer.maxParticleSize = 0.11f;
            renderer.sortingFudge = -5f;

            var follower = go.AddComponent<SnowFollower>();
            SerializedField.Set(follower, "target", player);
            SerializedField.Set(follower, "offset", new Vector3(0f, 12f, 12f));
        }
    }

    internal sealed class MapEffects
    {
        public TracerBeam Tracer;
        public ImpactFlash Impact;
    }
}
