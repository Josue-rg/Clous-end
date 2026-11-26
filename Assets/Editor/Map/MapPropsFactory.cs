using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>Props reutilizables del blockout: farolas, árboles, coches, vallas...</summary>
    internal static class MapPropsFactory
    {
        public static void StreetLamp(
            Transform parent,
            Vector3 position,
            float yaw,
            Material postMaterial,
            Material lightMaterial,
            bool castShadows = true)
        {
            var root = MapObjects.Empty("SM_StreetLamp", parent, position);
            root.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);

            MapObjects.Cylinder(root.transform, "Post", Vector3.zero, 0.14f, 6.4f, postMaterial, false, castShadows);
            MapObjects.Box(root.transform, "Arm", new Vector3(0f, 6.2f, 1.05f), new Vector3(0.16f, 0.16f, 2.1f), postMaterial, false, castShadows);
            MapObjects.Box(root.transform, "Head", new Vector3(0f, 6.05f, 2.05f), new Vector3(0.62f, 0.16f, 0.9f), lightMaterial, false, false);

            var collider = root.AddComponent<CapsuleCollider>();
            collider.radius = 0.18f;
            collider.height = 6.4f;
            collider.center = new Vector3(0f, 3.2f, 0f);
            root.isStatic = true;
        }

        public static void Pine(
            Transform parent,
            Vector3 position,
            float scale,
            Material trunk,
            Material pine,
            System.Random random)
        {
            var root = MapObjects.Empty("SM_Pine", parent, position);
            root.transform.localRotation = Quaternion.Euler(0f, (float)random.NextDouble() * 360f, 0f);
            root.transform.localScale = Vector3.one * scale;

            var collider = root.AddComponent<CapsuleCollider>();
            collider.radius = 0.38f;
            collider.height = 4.2f;
            collider.center = new Vector3(0f, 2.1f, 0f);

            MapObjects.Cylinder(root.transform, "Trunk", Vector3.zero, 0.28f, 2.6f, trunk, false, true);

            MapObjects.Mesh(
                root.transform,
                "PineLow",
                MapMeshFactory.Cone,
                new Vector3(0f, 1.6f, 0f),
                Vector3.zero,
                new Vector3(5.4f, 6.4f, 5.4f),
                pine,
                true);

            MapObjects.Mesh(
                root.transform,
                "PineHigh",
                MapMeshFactory.Cone,
                new Vector3(0f, 4.6f, 0f),
                new Vector3(0f, 23f, 0f),
                new Vector3(3.9f, 7.6f, 3.9f),
                pine,
                true);
        }

        public static void ParkedCar(
            Transform parent,
            Vector3 position,
            float yaw,
            Material body,
            Material glass,
            Material light,
            Material snow)
        {
            var root = MapObjects.Empty("SM_Car", parent, position);
            root.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);

            var shell = MapObjects.Box(root.transform, "Body", new Vector3(0f, 0.62f, 0f), new Vector3(1.95f, 0.85f, 4.5f), body, true, true);
            MapObjects.Box(root.transform, "Cabin", new Vector3(0f, 1.3f, -0.25f), new Vector3(1.7f, 0.65f, 2.2f), glass, false, true);
            MapObjects.Box(root.transform, "SnowRoof", new Vector3(0f, 1.67f, -0.25f), new Vector3(1.5f, 0.14f, 2f), snow, false, false);
            MapObjects.Box(root.transform, "LightL", new Vector3(-0.62f, 0.68f, 2.24f), new Vector3(0.34f, 0.22f, 0.08f), light, false, false);
            MapObjects.Box(root.transform, "LightR", new Vector3(0.62f, 0.68f, 2.24f), new Vector3(0.34f, 0.22f, 0.08f), light, false, false);

            root.isStatic = true;
        }

        public static void Truck(
            Transform parent,
            Vector3 position,
            float yaw,
            Material body,
            Material cabin,
            Material light,
            Material snow)
        {
            var root = MapObjects.Empty("SM_Truck", parent, position);
            root.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);

            MapObjects.Box(root.transform, "Cargo", new Vector3(0f, 2.1f, -1.4f), new Vector3(2.7f, 3.6f, 7.4f), body, true, true);
            MapObjects.Box(root.transform, "Cab", new Vector3(0f, 1.75f, 3.1f), new Vector3(2.55f, 2.9f, 2.1f), cabin, false, true);
            MapObjects.Box(root.transform, "SnowTop", new Vector3(0f, 3.98f, -1.4f), new Vector3(2.5f, 0.3f, 7f), snow, false, false);
            MapObjects.Box(root.transform, "LightL", new Vector3(-0.9f, 1.1f, 4.1f), new Vector3(0.4f, 0.28f, 0.1f), light, false, false);
            MapObjects.Box(root.transform, "LightR", new Vector3(0.9f, 1.1f, 4.1f), new Vector3(0.4f, 0.28f, 0.1f), light, false, false);

            root.isStatic = true;
        }

        public static void Bus(
            Transform parent,
            Vector3 position,
            float yaw,
            Material body,
            Material glass,
            Material light,
            Material snow)
        {
            var root = MapObjects.Empty("SM_Bus", parent, position);
            root.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);

            MapObjects.Box(root.transform, "Body", new Vector3(0f, 1.7f, 0f), new Vector3(2.9f, 3.2f, 11.5f), body, true, true);
            MapObjects.Box(root.transform, "WindowBand", new Vector3(0f, 2.4f, 0f), new Vector3(2.96f, 0.9f, 9.6f), glass, false, false);
            MapObjects.Box(root.transform, "SnowRoof", new Vector3(0f, 3.42f, 0f), new Vector3(2.7f, 0.26f, 11f), snow, false, false);
            MapObjects.Box(root.transform, "LightL", new Vector3(-0.95f, 0.9f, 5.78f), new Vector3(0.4f, 0.26f, 0.1f), light, false, false);
            MapObjects.Box(root.transform, "LightR", new Vector3(0.95f, 0.9f, 5.78f), new Vector3(0.4f, 0.26f, 0.1f), light, false, false);

            root.isStatic = true;
        }

        public static void Barricade(
            Transform parent,
            Vector3 position,
            float yaw,
            Material frame,
            Material stripe,
            Material light)
        {
            var root = MapObjects.Empty("SM_Barricade", parent, position);
            root.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);

            MapObjects.Box(root.transform, "Plank", new Vector3(0f, 1.05f, 0f), new Vector3(4.6f, 0.75f, 0.28f), stripe, false, true);
            MapObjects.Box(root.transform, "LegL", new Vector3(-1.9f, 0.55f, 0f), new Vector3(0.28f, 1.1f, 0.7f), frame, false, true);
            MapObjects.Box(root.transform, "LegR", new Vector3(1.9f, 0.55f, 0f), new Vector3(0.28f, 1.1f, 0.7f), frame, false, true);
            MapObjects.Box(root.transform, "Lamp", new Vector3(0f, 1.55f, 0f), new Vector3(0.3f, 0.16f, 0.16f), light, false, false);

            var collider = root.AddComponent<BoxCollider>();
            collider.size = new Vector3(4.8f, 1.6f, 0.9f);
            collider.center = new Vector3(0f, 0.8f, 0f);
            root.isStatic = true;
        }

        public static void Bench(
            Transform parent,
            Vector3 position,
            float yaw,
            Material wood,
            Material snow)
        {
            var root = MapObjects.Empty("SM_Bench", parent, position);
            root.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);

            MapObjects.Box(root.transform, "Seat", new Vector3(0f, 0.55f, 0f), new Vector3(2.3f, 0.14f, 0.62f), wood, false, true);
            MapObjects.Box(root.transform, "Back", new Vector3(0f, 0.95f, -0.28f), new Vector3(2.3f, 0.6f, 0.12f), wood, false, true);
            MapObjects.Box(root.transform, "Snow", new Vector3(0f, 0.64f, 0f), new Vector3(2.1f, 0.06f, 0.5f), snow, false, false);
            root.isStatic = true;
        }

        public static void MarketStall(
            Transform parent,
            Vector3 position,
            float yaw,
            Material wood,
            Material cloth,
            Material light)
        {
            var root = MapObjects.Empty("SM_MarketStall", parent, position);
            root.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);

            for (var x = -1; x <= 1; x += 2)
            {
                for (var z = -1; z <= 1; z += 2)
                    MapObjects.Box(root.transform, "Post", new Vector3(x * 1.6f, 1.3f, z * 0.9f), new Vector3(0.16f, 2.6f, 0.16f), wood, false, true);
            }

            MapObjects.Box(root.transform, "Roof", new Vector3(0f, 2.7f, 0f), new Vector3(4f, 0.2f, 2.5f), cloth, false, true);
            MapObjects.Box(root.transform, "Counter", new Vector3(0f, 1f, 0.55f), new Vector3(3.4f, 0.2f, 0.8f), wood, false, true);
            MapObjects.Box(root.transform, "Lantern", new Vector3(0f, 2.35f, 0f), new Vector3(0.28f, 0.28f, 0.28f), light, false, false);
            root.isStatic = true;
        }

        public static void SnowBank(
            Transform parent,
            Vector3 position,
            Vector3 size,
            Material snow,
            float yaw)
        {
            MapObjects.Sphere(parent, "SM_SnowBank", position, size, snow, false, false, true)
                .transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        }

        public static void Debris(
            Transform parent,
            Vector3 position,
            Vector3 size,
            Vector3 euler,
            Material material,
            System.Random random)
        {
            var go = MapObjects.Box(
                parent,
                "SM_Debris",
                position,
                size,
                material,
                true,
                true,
                true,
                Quaternion.Euler(euler));

            go.transform.localRotation = Quaternion.Euler(euler.x, euler.y + (float)random.NextDouble() * 60f, euler.z);
        }

        public static void Cable(
            Transform parent,
            string name,
            Vector3 start,
            Vector3 end,
            float sag,
            Material material,
            int segments = 16)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);

            var line = go.AddComponent<LineRenderer>();
            line.sharedMaterial = material;
            line.useWorldSpace = false;
            line.widthMultiplier = 0.09f;
            line.numCapVertices = 2;
            line.numCornerVertices = 0;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            line.receiveShadows = false;
            line.positionCount = segments + 1;

            for (var i = 0; i <= segments; i++)
            {
                var t = i / (float)segments;
                var point = Vector3.Lerp(start, end, t);
                point.y -= sag * Mathf.Sin(t * Mathf.PI);
                line.SetPosition(i, point);
            }

            go.isStatic = true;
        }

        public static void ChristmasTree(
            Transform parent,
            Vector3 position,
            Material trunk,
            Material pine,
            Material star,
            Material ornamentWarm,
            Material ornamentCool,
            System.Random random)
        {
            var root = MapObjects.Empty("SM_ChristmasTree", parent, position);
            MapObjects.Cylinder(root.transform, "Trunk", Vector3.zero, 1.1f, 6f, trunk, false, true);

            MapObjects.Mesh(root.transform, "Layer1", MapMeshFactory.Cone, new Vector3(0f, 2.4f, 0f), Vector3.zero, new Vector3(17f, 12f, 17f), pine, true);
            MapObjects.Mesh(root.transform, "Layer2", MapMeshFactory.Cone, new Vector3(0f, 8.4f, 0f), Vector3.zero, new Vector3(12.5f, 11f, 12.5f), pine, true);
            MapObjects.Mesh(root.transform, "Layer3", MapMeshFactory.Cone, new Vector3(0f, 14.4f, 0f), Vector3.zero, new Vector3(8f, 9f, 8f), pine, true);
            MapObjects.Mesh(root.transform, "Star", MapMeshFactory.Cone, new Vector3(0f, 22.5f, 0f), Vector3.zero, new Vector3(2.2f, 3f, 2.2f), star, false);

            for (var i = 0; i < 18; i++)
            {
                var angle = i / 18f * Mathf.PI * 2f;
                var radius = 6.2f - (i % 3) * 1.1f;
                var height = 3.5f + (i / 18f) * 16f;
                var size = 0.5f + (float)random.NextDouble() * 0.35f;

                var ornament = MapObjects.Sphere(
                    root.transform,
                    "Ornament",
                    new Vector3(Mathf.Cos(angle) * radius, height, Mathf.Sin(angle) * radius),
                    Vector3.one * size,
                    i % 2 == 0 ? ornamentWarm : ornamentCool,
                    false,
                    false,
                    false);
                ornament.isStatic = true;
            }

            var collider = root.AddComponent<BoxCollider>();
            collider.size = new Vector3(8f, 6f, 8f);
            collider.center = new Vector3(0f, 3f, 0f);
            root.isStatic = true;
        }

        public static void Fountain(
            Transform parent,
            Vector3 position,
            Material stone,
            Material ice,
            Material light)
        {
            var root = MapObjects.Empty("SM_Fountain", parent, position);
            MapObjects.Cylinder(root.transform, "Base", Vector3.zero, 6.5f, 0.7f, stone, false, true);
            MapObjects.Cylinder(root.transform, "Ice", new Vector3(0f, 0.7f, 0f), 5.8f, 0.12f, ice, false, false);
            MapObjects.Cylinder(root.transform, "Column", new Vector3(0f, 0.8f, 0f), 1.2f, 4.2f, stone, false, true);
            MapObjects.Sphere(root.transform, "Glow", new Vector3(0f, 5.4f, 0f), Vector3.one * 1.2f, light, false, false);

            var collider = root.AddComponent<CapsuleCollider>();
            collider.radius = 6.5f;
            collider.height = 1.4f;
            collider.center = new Vector3(0f, 0.7f, 0f);
            root.isStatic = true;
        }

        public static void Sign(
            Transform parent,
            Vector3 position,
            float yaw,
            Material post,
            Material panel)
        {
            var root = MapObjects.Empty("SM_Sign", parent, position);
            root.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);

            MapObjects.Cylinder(root.transform, "Post", Vector3.zero, 0.16f, 4.6f, post, false, true);
            MapObjects.Box(root.transform, "Panel", new Vector3(0f, 4.6f, 0f), new Vector3(2.6f, 1.3f, 0.12f), panel, false, false);
            root.isStatic = true;
        }

        /// <summary>Baliza de carretera: poste fino con punta reflectante.</summary>
        public static void SnowPole(
            Transform parent,
            Vector3 position,
            Material post,
            Material light)
        {
            var root = MapObjects.Empty("SM_SnowPole", parent, position);
            MapObjects.Box(root.transform, "Pole", new Vector3(0f, 1.3f, 0f), new Vector3(0.14f, 2.6f, 0.14f), post, false, false);
            MapObjects.Box(root.transform, "Tip", new Vector3(0f, 2.45f, 0f), new Vector3(0.24f, 0.42f, 0.24f), light, false, false);
            root.isStatic = true;
        }

        public static void Container(
            Transform parent,
            Vector3 position,
            float yaw,
            Material shell,
            Material snow,
            Material light)
        {
            var root = MapObjects.Empty("SM_Container", parent, position);
            root.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);

            MapObjects.Box(root.transform, "Shell", new Vector3(0f, 1.55f, 0f), new Vector3(6.2f, 3.1f, 12f), shell, true, true);
            MapObjects.Box(root.transform, "Snow", new Vector3(0f, 3.2f, 0f), new Vector3(5.9f, 0.25f, 11.6f), snow, false, false);
            MapObjects.Box(root.transform, "Strip", new Vector3(-3.14f, 2.2f, 0f), new Vector3(0.12f, 0.32f, 7f), light, false, false);
            root.isStatic = true;
        }

        public static void TruckStopCanopy(
            Transform parent,
            Vector3 position,
            float yaw,
            Material frame,
            Material roof,
            Material snow,
            Material light,
            Material shopWall,
            Material shopGlass)
        {
            var root = MapObjects.Empty("SM_TruckStop", parent, position);
            root.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);

            for (var x = -1; x <= 1; x += 2)
            {
                for (var z = -1; z <= 1; z += 2)
                    MapObjects.Box(root.transform, "Column", new Vector3(x * 7f, 3.2f, z * 5f), new Vector3(0.8f, 6.4f, 0.8f), frame, true, true);
            }

            MapObjects.Box(root.transform, "Canopy", new Vector3(0f, 6.7f, 0f), new Vector3(18f, 0.6f, 13f), roof, false, true);
            MapObjects.Box(root.transform, "Snow", new Vector3(0f, 7.1f, 0f), new Vector3(17.4f, 0.25f, 12.4f), snow, false, false);
            MapObjects.Box(root.transform, "Light", new Vector3(0f, 6.28f, 0f), new Vector3(13f, 0.18f, 0.5f), light, false, false);

            MapObjects.Box(root.transform, "Shop", new Vector3(-13f, 2.4f, 0f), new Vector3(9f, 4.8f, 7f), shopWall, true, true);
            MapObjects.Box(root.transform, "ShopGlass", new Vector3(-8.45f, 2.2f, 0f), new Vector3(0.14f, 2.6f, 5.6f), shopGlass, false, false);
            root.isStatic = true;
        }
    }
}
