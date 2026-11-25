using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>Construye el pueblo, la iglesia, el árbol y el bosque del diorama.</summary>
    internal static class LoginVillageBuilder
    {
        private struct HouseSpec
        {
            public string Name;
            public Vector3 Position;
            public float Yaw;
            public Vector3 Size;
            public bool Plaster;
            public bool Smoke;
        }

        public static void Build(Transform root, LoginAssets assets)
        {
            var houses = new[]
            {
                new HouseSpec { Name = "House_01", Position = new Vector3(-18f, 0f, 22f), Yaw = 8f,  Size = new Vector3(5.0f, 3.2f, 4.5f), Plaster = false, Smoke = true },
                new HouseSpec { Name = "House_02", Position = new Vector3(-12f, 0f, 36f), Yaw = -12f, Size = new Vector3(4.2f, 2.9f, 4.0f), Plaster = true,  Smoke = false },
                new HouseSpec { Name = "House_03", Position = new Vector3(-23f, 0f, 48f), Yaw = 4f,  Size = new Vector3(5.5f, 3.6f, 5.0f), Plaster = false, Smoke = true },
                new HouseSpec { Name = "House_04", Position = new Vector3(13f, 0f, 27f),  Yaw = -8f, Size = new Vector3(4.8f, 3.0f, 4.2f), Plaster = true,  Smoke = false },
                new HouseSpec { Name = "House_05", Position = new Vector3(22f, 0f, 39f),  Yaw = 14f, Size = new Vector3(5.2f, 3.4f, 4.8f), Plaster = false, Smoke = true },
                new HouseSpec { Name = "House_06", Position = new Vector3(11f, 0f, 53f),  Yaw = -4f, Size = new Vector3(4.4f, 2.9f, 4.0f), Plaster = false, Smoke = false },
                new HouseSpec { Name = "House_07", Position = new Vector3(-7f, 0f, 64f),  Yaw = 6f,  Size = new Vector3(5.0f, 3.3f, 4.6f), Plaster = true,  Smoke = true }
            };

            foreach (var spec in houses)
                CreateHouse(root, assets, spec);

            CreateChurch(root, assets);
            CreateGrandTree(root, assets);
            CreatePines(root, assets);
            CreateFences(root, assets);
            CreateSnowman(root, assets);
        }

        private static void CreateHouse(Transform parent, LoginAssets assets, HouseSpec spec)
        {
            var house = LoginSceneObjects.Empty(spec.Name, parent, spec.Position, new Vector3(0f, spec.Yaw, 0f));
            var w = spec.Size.x;
            var h = spec.Size.y;
            var d = spec.Size.z;

            LoginSceneObjects.Primitive(
                "Body",
                PrimitiveType.Cube,
                house.transform,
                new Vector3(0f, h * 0.5f, 0f),
                Vector3.zero,
                spec.Size,
                spec.Plaster ? assets.Plaster : assets.WoodWarm);

            var roofHeight = h * 0.42f;
            var roofAngle = Mathf.Atan2(roofHeight, d * 0.5f) * Mathf.Rad2Deg;
            var slabLength = Mathf.Sqrt(d * 0.5f * d * 0.5f + roofHeight * roofHeight) + 0.2f;

            LoginSceneObjects.Primitive(
                "RoofFront",
                PrimitiveType.Cube,
                house.transform,
                new Vector3(0f, h + roofHeight * 0.5f, -d * 0.25f),
                new Vector3(-roofAngle, 0f, 0f),
                new Vector3(w + 0.6f, 0.18f, slabLength),
                assets.SnowRoof);

            LoginSceneObjects.Primitive(
                "RoofBack",
                PrimitiveType.Cube,
                house.transform,
                new Vector3(0f, h + roofHeight * 0.5f, d * 0.25f),
                new Vector3(roofAngle, 0f, 0f),
                new Vector3(w + 0.6f, 0.18f, slabLength),
                assets.SnowRoof);

            LoginSceneObjects.Primitive(
                "RoofRidge",
                PrimitiveType.Cube,
                house.transform,
                new Vector3(0f, h + roofHeight, 0f),
                Vector3.zero,
                new Vector3(w + 0.7f, 0.16f, 0.4f),
                assets.SnowRoof);

            var chimney = new Vector3(w * 0.3f, h + roofHeight * 0.8f, d * 0.16f);
            LoginSceneObjects.Primitive(
                "Chimney",
                PrimitiveType.Cube,
                house.transform,
                chimney,
                Vector3.zero,
                new Vector3(0.65f, 2.0f, 0.65f),
                assets.Stone);

            LoginSceneObjects.Primitive(
                "Door",
                PrimitiveType.Cube,
                house.transform,
                new Vector3(0f, 1.05f, -d * 0.5f - 0.07f),
                Vector3.zero,
                new Vector3(1.05f, 2.1f, 0.12f),
                assets.WoodDark);

            LoginSceneObjects.Primitive(
                "DoorLight",
                PrimitiveType.Sphere,
                house.transform,
                new Vector3(0f, 2.35f, -d * 0.5f - 0.18f),
                Vector3.zero,
                Vector3.one * 0.12f,
                assets.LightWarm,
                false);

            CreateWindow(house.transform, assets, new Vector3(-w * 0.27f, 1.65f, -d * 0.5f), false);
            CreateWindow(house.transform, assets, new Vector3(w * 0.27f, 1.65f, -d * 0.5f), false);

            LoginSceneObjects.Primitive(
                "WindowPool",
                PrimitiveType.Quad,
                house.transform,
                new Vector3(0f, 0.06f, -d * 0.5f - 1.7f),
                new Vector3(90f, 0f, 0f),
                new Vector3(w * 1.5f, 3.0f, 1f),
                assets.WindowPool,
                false);

            for (var i = 0; i < 6; i++)
            {
                var t = i / 5f;
                var material = i % 3 == 0 ? assets.LightRed : assets.LightWarm;
                LoginSceneObjects.Primitive(
                    "EaveLight_" + i,
                    PrimitiveType.Sphere,
                    house.transform,
                    new Vector3(Mathf.Lerp(-w * 0.42f, w * 0.42f, t), h + 0.02f, -d * 0.5f - 0.12f),
                    Vector3.zero,
                    Vector3.one * 0.1f,
                    material,
                    false);
            }

            LoginSceneObjects.Primitive(
                "SnowDrift",
                PrimitiveType.Sphere,
                house.transform,
                new Vector3(0f, 0.22f, -d * 0.5f - 0.9f),
                Vector3.zero,
                new Vector3(w * 1.05f, 0.38f, 1.4f),
                assets.SnowGround);

            if (spec.Smoke)
            {
                CreateSmoke(house.transform, chimney + new Vector3(0f, 1.15f, 0f), assets);
                CreateWarmLight(house.transform, new Vector3(0f, 2.4f, -d * 0.5f - 1.2f), 2.2f, 10f);
            }
        }

        private static void CreateWarmLight(Transform parent, Vector3 localPosition, float intensity, float range)
        {
            var go = new GameObject("WarmLight");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;

            var light = go.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = LoginPalette.WarmLight;
            light.intensity = intensity;
            light.range = range;
            light.shadows = LightShadows.None;
        }

        private static void CreateWindow(Transform parent, LoginAssets assets, Vector3 position, bool side)
        {
            var euler = side ? new Vector3(0f, 90f, 0f) : Vector3.zero;

            LoginSceneObjects.Primitive(
                "WindowFrame",
                PrimitiveType.Cube,
                parent,
                position + new Vector3(0f, 0f, -0.035f),
                euler,
                new Vector3(1.12f, 1.12f, 0.08f),
                assets.WoodDark);

            LoginSceneObjects.Primitive(
                "WindowGlow",
                PrimitiveType.Quad,
                parent,
                position + new Vector3(0f, 0f, -0.09f),
                euler,
                new Vector3(0.88f, 0.88f, 1f),
                assets.WindowWarm,
                false);
        }

        private static void CreateChurch(Transform parent, LoginAssets assets)
        {
            var church = LoginSceneObjects.Empty("Church", parent, new Vector3(30f, 0f, 55f), new Vector3(0f, -12f, 0f));

            LoginSceneObjects.Primitive(
                "Body",
                PrimitiveType.Cube,
                church.transform,
                new Vector3(0f, 3f, 0f),
                Vector3.zero,
                new Vector3(9f, 6f, 12f),
                assets.Plaster);

            var roofHeight = 2.6f;
            var depth = 12f;
            var roofAngle = Mathf.Atan2(roofHeight, depth * 0.5f) * Mathf.Rad2Deg;
            var slabLength = Mathf.Sqrt(depth * 0.5f * depth * 0.5f + roofHeight * roofHeight) + 0.2f;

            LoginSceneObjects.Primitive(
                "RoofFront",
                PrimitiveType.Cube,
                church.transform,
                new Vector3(0f, 6f + roofHeight * 0.5f, -depth * 0.25f),
                new Vector3(-roofAngle, 0f, 0f),
                new Vector3(9.6f, 0.2f, slabLength),
                assets.SnowRoof);

            LoginSceneObjects.Primitive(
                "RoofBack",
                PrimitiveType.Cube,
                church.transform,
                new Vector3(0f, 6f + roofHeight * 0.5f, depth * 0.25f),
                new Vector3(roofAngle, 0f, 0f),
                new Vector3(9.6f, 0.2f, slabLength),
                assets.SnowRoof);

            LoginSceneObjects.Primitive(
                "Tower",
                PrimitiveType.Cube,
                church.transform,
                new Vector3(-5f, 7.5f, -6.2f),
                Vector3.zero,
                new Vector3(3f, 15f, 3f),
                assets.Stone);

            LoginSceneObjects.Mesh(
                "Spire",
                LoginMeshFactory.PineCone,
                church.transform,
                new Vector3(-5f, 15f, -6.2f),
                Vector3.zero,
                new Vector3(3.4f, 7f, 3.4f),
                assets.SnowRoof);

            LoginSceneObjects.Primitive(
                "CrossVertical",
                PrimitiveType.Cube,
                church.transform,
                new Vector3(-5f, 22.4f, -6.2f),
                Vector3.zero,
                new Vector3(0.14f, 1.5f, 0.14f),
                assets.WoodDark);

            LoginSceneObjects.Primitive(
                "CrossHorizontal",
                PrimitiveType.Cube,
                church.transform,
                new Vector3(-5f, 22.8f, -6.2f),
                Vector3.zero,
                new Vector3(0.85f, 0.14f, 0.14f),
                assets.WoodDark);

            LoginSceneObjects.Primitive(
                "ClockFace",
                PrimitiveType.Cylinder,
                church.transform,
                new Vector3(-5f, 11.2f, -7.78f),
                new Vector3(90f, 0f, 0f),
                new Vector3(2.2f, 0.06f, 2.2f),
                assets.Moon,
                false);

            LoginSceneObjects.Primitive(
                "ClockHandMinute",
                PrimitiveType.Cube,
                church.transform,
                new Vector3(-5f, 11.45f, -7.88f),
                Vector3.zero,
                new Vector3(0.06f, 0.85f, 0.04f),
                assets.WoodDark,
                false);

            LoginSceneObjects.Primitive(
                "ClockHandHour",
                PrimitiveType.Cube,
                church.transform,
                new Vector3(-4.75f, 11.15f, -7.88f),
                new Vector3(0f, 0f, -55f),
                new Vector3(0.05f, 0.6f, 0.04f),
                assets.WoodDark,
                false);

            for (var i = -1; i <= 1; i++)
            {
                LoginSceneObjects.Primitive(
                    "NaveWindowFrame_" + i,
                    PrimitiveType.Cube,
                    church.transform,
                    new Vector3(i * 2.2f, 3.4f, -6.05f),
                    Vector3.zero,
                    new Vector3(1.35f, 3.5f, 0.08f),
                    assets.WoodDark);

                LoginSceneObjects.Primitive(
                    "NaveWindowGlow_" + i,
                    PrimitiveType.Quad,
                    church.transform,
                    new Vector3(i * 2.2f, 3.4f, -6.1f),
                    Vector3.zero,
                    new Vector3(1.1f, 3.2f, 1f),
                    assets.WindowWarm,
                    false);
            }

            LoginSceneObjects.Primitive(
                "ChurchDoor",
                PrimitiveType.Cube,
                church.transform,
                new Vector3(0f, 1.5f, -6.08f),
                Vector3.zero,
                new Vector3(1.9f, 3f, 0.14f),
                assets.WoodDark);

            LoginSceneObjects.Primitive(
                "ChurchLightPool",
                PrimitiveType.Quad,
                church.transform,
                new Vector3(0f, 0.06f, -8.6f),
                new Vector3(90f, 0f, 0f),
                new Vector3(8f, 4f, 1f),
                assets.WindowPool,
                false);

            LoginSceneObjects.Primitive(
                "BelfryLeft",
                PrimitiveType.Quad,
                church.transform,
                new Vector3(-5.8f, 13.2f, -7.8f),
                Vector3.zero,
                new Vector3(0.7f, 1.5f, 1f),
                assets.WindowWarm,
                false);

            LoginSceneObjects.Primitive(
                "BelfryRight",
                PrimitiveType.Quad,
                church.transform,
                new Vector3(-4.2f, 13.2f, -7.8f),
                Vector3.zero,
                new Vector3(0.7f, 1.5f, 1f),
                assets.WindowWarm,
                false);

            var lightGo = new GameObject("ChurchLight");
            lightGo.transform.SetParent(church.transform, false);
            lightGo.transform.localPosition = new Vector3(0f, 3.2f, -8.5f);
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = LoginPalette.WarmLight;
            light.intensity = 2.6f;
            light.range = 16f;
            light.shadows = LightShadows.None;
        }

        private static void CreateGrandTree(Transform parent, LoginAssets assets)
        {
            var tree = LoginSceneObjects.Empty("GrandTree", parent, new Vector3(12.5f, 0f, 7f), new Vector3(0f, 12f, 0f));

            LoginSceneObjects.Primitive(
                "Trunk",
                PrimitiveType.Cylinder,
                tree.transform,
                new Vector3(0f, 2.4f, 0f),
                Vector3.zero,
                new Vector3(0.7f, 2.4f, 0.7f),
                assets.WoodDark);

            LoginSceneObjects.Mesh(
                "Layer_1",
                LoginMeshFactory.PineCone,
                tree.transform,
                new Vector3(0f, 2.0f, 0f),
                Vector3.zero,
                new Vector3(7f, 5.2f, 7f),
                assets.Pine);

            LoginSceneObjects.Mesh(
                "Layer_2",
                LoginMeshFactory.PineCone,
                tree.transform,
                new Vector3(0f, 4.8f, 0f),
                Vector3.zero,
                new Vector3(5.6f, 4.5f, 5.6f),
                assets.Pine);

            LoginSceneObjects.Mesh(
                "Layer_3",
                LoginMeshFactory.PineCone,
                tree.transform,
                new Vector3(0f, 7.2f, 0f),
                Vector3.zero,
                new Vector3(4.0f, 4.0f, 4.0f),
                assets.Pine);

            LoginSceneObjects.Mesh(
                "Layer_4",
                LoginMeshFactory.PineCone,
                tree.transform,
                new Vector3(0f, 9.4f, 0f),
                Vector3.zero,
                new Vector3(2.4f, 3.2f, 2.4f),
                assets.Pine);

            for (var i = 0; i < 48; i++)
            {
                var t = i / 47f;
                var angle = t * Mathf.PI * 2f * 4.5f + 0.4f;
                var radius = Mathf.Lerp(3.7f, 1.25f, t);
                var y = Mathf.Lerp(2.2f, 12.2f, t);

                var material = i % 7 == 0 ? assets.LightRed
                    : i % 11 == 0 ? assets.LightCool
                    : assets.LightWarm;

                LoginSceneObjects.Primitive(
                    "TreeLight_" + i,
                    PrimitiveType.Sphere,
                    tree.transform,
                    new Vector3(Mathf.Cos(angle) * radius, y, Mathf.Sin(angle) * radius),
                    Vector3.zero,
                    Vector3.one * 0.18f,
                    material,
                    false);
            }

            LoginSceneObjects.Primitive(
                "TreeWindowPool",
                PrimitiveType.Quad,
                tree.transform,
                new Vector3(0f, 0.06f, 0f),
                new Vector3(90f, 0f, 0f),
                new Vector3(13f, 10f, 1f),
                assets.WindowPool,
                false);

            LoginSceneObjects.Primitive(
                "Star",
                PrimitiveType.Sphere,
                tree.transform,
                new Vector3(0f, 12.9f, 0f),
                Vector3.zero,
                Vector3.one * 0.6f,
                assets.LightWarm,
                false);

            var giftMaterials = new[] { assets.GiftRed, assets.GiftGreen, assets.WoodWarm };
            var giftPositions = new[]
            {
                new Vector3(-2.6f, 0.35f, -1.4f),
                new Vector3(-1.4f, 0.28f, -2.6f),
                new Vector3(0.2f, 0.4f, -2.9f),
                new Vector3(1.8f, 0.3f, -2.2f),
                new Vector3(2.8f, 0.36f, -0.6f),
                new Vector3(2.0f, 0.26f, 1.1f)
            };

            for (var i = 0; i < giftPositions.Length; i++)
            {
                var size = 0.65f + i % 3 * 0.12f;
                LoginSceneObjects.Primitive(
                    "Gift_" + i,
                    PrimitiveType.Cube,
                    tree.transform,
                    giftPositions[i],
                    new Vector3(0f, i * 17f, 0f),
                    new Vector3(size, size, size),
                    giftMaterials[i % giftMaterials.Length]);
            }

            var lightGo = new GameObject("TreeLight");
            lightGo.transform.SetParent(tree.transform, false);
            lightGo.transform.localPosition = new Vector3(0f, 6.5f, 0f);
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = LoginPalette.WarmLight;
            light.intensity = 4f;
            light.range = 24f;
            light.shadows = LightShadows.None;

            var topLightGo = new GameObject("TreeTopLight");
            topLightGo.transform.SetParent(tree.transform, false);
            topLightGo.transform.localPosition = new Vector3(0f, 10.5f, 0f);
            var topLight = topLightGo.AddComponent<Light>();
            topLight.type = LightType.Point;
            topLight.color = new Color(1f, 0.72f, 0.4f);
            topLight.intensity = 2.2f;
            topLight.range = 14f;
            topLight.shadows = LightShadows.None;
        }

        private static void CreatePines(Transform parent, LoginAssets assets)
        {
            var positions = new[]
            {
                new Vector3(-30f, 0f, 14f), new Vector3(-23f, 0f, 24f), new Vector3(-35f, 0f, 33f),
                new Vector3(-16f, 0f, 45f), new Vector3(-27f, 0f, 58f), new Vector3(-41f, 0f, 49f),
                new Vector3(-13f, 0f, 12f), new Vector3(-33f, 0f, 72f), new Vector3(-4f, 0f, 79f),
                new Vector3(35f, 0f, 18f), new Vector3(41f, 0f, 31f), new Vector3(31f, 0f, 44f),
                new Vector3(45f, 0f, 57f), new Vector3(24f, 0f, 69f), new Vector3(38f, 0f, 74f),
                new Vector3(21f, 0f, 14f), new Vector3(49f, 0f, 46f), new Vector3(9f, 0f, 84f),
                new Vector3(19f, 0f, 88f), new Vector3(-8f, 0f, 90f)
            };

            for (var i = 0; i < positions.Length; i++)
            {
                var scale = 2.2f + (i * 37 % 100) / 100f * 2.2f;
                CreatePine(parent, assets, "Pine_" + i, positions[i], scale, i * 23f);
            }
        }

        private static void CreatePine(Transform parent, LoginAssets assets, string name, Vector3 position, float scale, float yaw)
        {
            var tree = LoginSceneObjects.Empty(name, parent, position, new Vector3(0f, yaw, 0f));

            LoginSceneObjects.Primitive(
                "Trunk",
                PrimitiveType.Cylinder,
                tree.transform,
                new Vector3(0f, 0.5f * scale, 0f),
                Vector3.zero,
                new Vector3(0.16f * scale, 0.5f * scale, 0.16f * scale),
                assets.WoodDark);

            LoginSceneObjects.Mesh(
                "CanopyLower",
                LoginMeshFactory.PineCone,
                tree.transform,
                new Vector3(0f, 0.65f * scale, 0f),
                Vector3.zero,
                new Vector3(1.85f * scale, 2.6f * scale, 1.85f * scale),
                assets.Pine);

            LoginSceneObjects.Mesh(
                "CanopyUpper",
                LoginMeshFactory.PineCone,
                tree.transform,
                new Vector3(0f, 2.45f * scale, 0f),
                Vector3.zero,
                new Vector3(1.3f * scale, 2.1f * scale, 1.3f * scale),
                assets.Pine);

            LoginSceneObjects.Mesh(
                "SnowCap",
                LoginMeshFactory.PineCone,
                tree.transform,
                new Vector3(0f, 4.05f * scale, 0f),
                Vector3.zero,
                new Vector3(0.9f * scale, 0.95f * scale, 0.9f * scale),
                assets.SnowRoof);

            LoginSceneObjects.Primitive(
                "BaseSnow",
                PrimitiveType.Sphere,
                tree.transform,
                new Vector3(0f, 0.16f * scale, 0f),
                Vector3.zero,
                new Vector3(1.6f * scale, 0.3f * scale, 1.6f * scale),
                assets.SnowGround);
        }

        private static void CreateFences(Transform parent, LoginAssets assets)
        {
            for (var side = -1; side <= 1; side += 2)
            {
                var root = LoginSceneObjects.Empty("Fence_" + side, parent, Vector3.zero, Vector3.zero);

                for (var i = 0; i < 7; i++)
                {
                    var z = -6f + i * 3.4f;
                    var x = side * 4.7f;

                    LoginSceneObjects.Primitive(
                        "Post_" + i,
                        PrimitiveType.Cube,
                        root.transform,
                        new Vector3(x, 0.6f, z),
                        Vector3.zero,
                        new Vector3(0.14f, 1.2f, 0.14f),
                        assets.WoodDark);

                    LoginSceneObjects.Primitive(
                        "PostSnow_" + i,
                        PrimitiveType.Cube,
                        root.transform,
                        new Vector3(x, 1.24f, z),
                        Vector3.zero,
                        new Vector3(0.2f, 0.09f, 0.2f),
                        assets.SnowRoof);

                    if (i < 6)
                    {
                        LoginSceneObjects.Primitive(
                            "Rail_" + i,
                            PrimitiveType.Cube,
                            root.transform,
                            new Vector3(x, 0.72f, z + 1.7f),
                            Vector3.zero,
                            new Vector3(0.1f, 0.1f, 3.4f),
                            assets.WoodDark);
                    }
                }

                var lanternX = side * 4.7f;
                var lanternZ = 8.4f;

                LoginSceneObjects.Primitive(
                    "LanternArm",
                    PrimitiveType.Cube,
                    root.transform,
                    new Vector3(lanternX - side * 0.32f, 2.5f, lanternZ),
                    Vector3.zero,
                    new Vector3(0.7f, 0.08f, 0.08f),
                    assets.WoodDark);

                LoginSceneObjects.Primitive(
                    "LanternGlow",
                    PrimitiveType.Sphere,
                    root.transform,
                    new Vector3(lanternX - side * 0.62f, 2.38f, lanternZ),
                    Vector3.zero,
                    Vector3.one * 0.18f,
                    assets.LightWarm,
                    false);
            }

            var lightGo = new GameObject("FenceLight");
            lightGo.transform.SetParent(parent, false);
            lightGo.transform.localPosition = new Vector3(4.1f, 2.6f, 8.4f);
            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = LoginPalette.WarmLight;
            light.intensity = 2.2f;
            light.range = 10f;
            light.shadows = LightShadows.None;
        }

        private static void CreateSnowman(Transform parent, LoginAssets assets)
        {
            var snowman = LoginSceneObjects.Empty("Snowman", parent, new Vector3(7.2f, 0f, 15f), new Vector3(0f, -16f, 0f));

            LoginSceneObjects.Primitive(
                "Body", PrimitiveType.Sphere, snowman.transform,
                new Vector3(0f, 0.68f, 0f), Vector3.zero, Vector3.one * 1.4f,
                assets.SnowGround);

            LoginSceneObjects.Primitive(
                "Head", PrimitiveType.Sphere, snowman.transform,
                new Vector3(0f, 1.68f, 0f), Vector3.zero, Vector3.one * 0.86f,
                assets.SnowGround);

            LoginSceneObjects.Primitive(
                "EyeLeft", PrimitiveType.Sphere, snowman.transform,
                new Vector3(-0.16f, 1.82f, -0.38f), Vector3.zero, Vector3.one * 0.075f,
                assets.WoodDark, false);

            LoginSceneObjects.Primitive(
                "EyeRight", PrimitiveType.Sphere, snowman.transform,
                new Vector3(0.16f, 1.82f, -0.38f), Vector3.zero, Vector3.one * 0.075f,
                assets.WoodDark, false);

            LoginSceneObjects.Mesh(
                "CarrotNose", LoginMeshFactory.PineCone, snowman.transform,
                new Vector3(0f, 1.7f, -0.43f), new Vector3(-90f, 0f, 0f),
                new Vector3(0.14f, 0.42f, 0.14f), assets.Carrot, false);

            for (var i = 0; i < 3; i++)
            {
                LoginSceneObjects.Primitive(
                    "Button_" + i, PrimitiveType.Sphere, snowman.transform,
                    new Vector3(0f, 0.78f + i * 0.3f, -0.65f), Vector3.zero, Vector3.one * 0.075f,
                    assets.WoodDark, false);
            }

            LoginSceneObjects.Primitive(
                "HatBrim", PrimitiveType.Cylinder, snowman.transform,
                new Vector3(0f, 2.12f, 0f), Vector3.zero, new Vector3(0.95f, 0.06f, 0.95f),
                assets.Stone);

            LoginSceneObjects.Primitive(
                "HatTop", PrimitiveType.Cylinder, snowman.transform,
                new Vector3(0f, 2.4f, 0f), Vector3.zero, new Vector3(0.6f, 0.55f, 0.6f),
                assets.Stone);

            LoginSceneObjects.Primitive(
                "HatBand", PrimitiveType.Cylinder, snowman.transform,
                new Vector3(0f, 2.16f, 0f), Vector3.zero, new Vector3(0.63f, 0.08f, 0.63f),
                assets.GiftRed);

            LoginSceneObjects.Primitive(
                "Scarf", PrimitiveType.Cube, snowman.transform,
                new Vector3(0f, 1.3f, -0.34f), Vector3.zero, new Vector3(0.95f, 0.14f, 0.2f),
                assets.GiftRed);

            LoginSceneObjects.Primitive(
                "ScarfTail", PrimitiveType.Cube, snowman.transform,
                new Vector3(0.48f, 1.05f, -0.34f), new Vector3(0f, 0f, -12f), new Vector3(0.16f, 0.42f, 0.16f),
                assets.GiftRed);

            LoginSceneObjects.Primitive(
                "ArmLeft", PrimitiveType.Cylinder, snowman.transform,
                new Vector3(-0.72f, 1.28f, 0f), new Vector3(0f, 0f, 62f), new Vector3(0.07f, 0.55f, 0.07f),
                assets.WoodDark);

            LoginSceneObjects.Primitive(
                "ArmRight", PrimitiveType.Cylinder, snowman.transform,
                new Vector3(0.72f, 1.28f, 0f), new Vector3(0f, 0f, -62f), new Vector3(0.07f, 0.55f, 0.07f),
                assets.WoodDark);
        }

        private static void CreateSmoke(Transform parent, Vector3 localPosition, LoginAssets assets)
        {
            var go = new GameObject("ChimneySmoke");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;

            var ps = go.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.loop = true;
            main.startLifetime = 8f;
            main.startSpeed = 0.35f;
            main.startSize = new ParticleSystem.MinMaxCurve(0.8f, 1.8f);
            main.startColor = new Color(0.36f, 0.4f, 0.45f, 0.16f);
            main.maxParticles = 80;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.playOnAwake = true;

            var emission = ps.emission;
            emission.rateOverTime = 7f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 9f;
            shape.radius = 0.35f;
            shape.rotation = new Vector3(-90f, 0f, 0f);

            var velocity = ps.velocityOverLifetime;
            velocity.enabled = true;
            velocity.space = ParticleSystemSimulationSpace.World;
            velocity.x = new ParticleSystem.MinMaxCurve(-0.18f, 0.18f);
            velocity.y = new ParticleSystem.MinMaxCurve(0.45f, 0.8f);
            velocity.z = new ParticleSystem.MinMaxCurve(-0.08f, 0.08f);

            var noise = ps.noise;
            noise.enabled = true;
            noise.strength = 0.32f;
            noise.frequency = 0.25f;
            noise.scrollSpeed = 0.08f;

            var renderer = go.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.sharedMaterial = assets.SnowParticle;
            renderer.maxParticleSize = 2f;
            renderer.sortingFudge = 4f;
        }
    }
}
