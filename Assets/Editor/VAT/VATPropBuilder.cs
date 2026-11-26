using UnityEditor;
using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>
    /// Coloca en la escena los props del paquete VAT que no forman parte del
    /// combate: arnés, cascabeles y collar sobre el jugador, y meteoritos
    /// caídos como decoración del recorrido. Todo se crea una vez en el editor
    /// y queda editable en la jerarquía.
    /// </summary>
    internal static class VATPropBuilder
    {
        private const string MeteorsRootName = "VATProps";
        private const string PlayerPropsRootName = "VATProps";

        private struct MeteorSpec
        {
            public string prefab;
            public Vector3 position;
            public Vector3 euler;
            public float scale;

            public MeteorSpec(string prefab, Vector3 position, Vector3 euler, float scale)
            {
                this.prefab = prefab;
                this.position = position;
                this.euler = euler;
                this.scale = scale;
            }
        }

        private static readonly MeteorSpec[] Meteors =
        {
            new MeteorSpec("SM_Meteor_1", new Vector3(-14f, 0.55f, 190f), new Vector3(0f, 24f, 8f), 1f),
            new MeteorSpec("SM_Meteor_2", new Vector3(14.5f, 0.45f, 205f), new Vector3(0f, -38f, -6f), 0.85f),
            new MeteorSpec("SM_Meteor_3", new Vector3(-13f, 0.5f, 610f), new Vector3(8f, 62f, 0f), 1.1f),
            new MeteorSpec("SM_Meteor_1", new Vector3(15f, 0.6f, 640f), new Vector3(0f, -12f, 10f), 0.9f),
            new MeteorSpec("SM_Meteor_2", new Vector3(-15f, 0.5f, 1070f), new Vector3(-7f, 41f, 0f), 1f),
            new MeteorSpec("SM_Meteor_3", new Vector3(13f, 0.55f, 1095f), new Vector3(0f, -55f, 12f), 0.95f),
            new MeteorSpec("SM_Meteor_1", new Vector3(-12f, 0.5f, 1240f), new Vector3(0f, 80f, 0f), 0.8f),
            new MeteorSpec("SM_Meteor_2", new Vector3(16f, 0.6f, 1490f), new Vector3(6f, -20f, -9f), 1.05f)
        };

        public static void Build(Transform gameRoot)
        {
            AddMeteors(gameRoot);

            var player = gameRoot.Find("Player");

            if (player != null)
                AddPlayerProps(player);
        }

        private static void AddMeteors(Transform gameRoot)
        {
            var root = FindOrCreate(gameRoot, MeteorsRootName);
            Clear(root);

            foreach (var spec in Meteors)
            {
                var prefab = VATCatalog.LoadPrefab(spec.prefab);

                if (prefab == null)
                {
                    Debug.LogWarning("[Clous-End VAT] Falta el prop " + spec.prefab + " para el mapa.");
                    continue;
                }

                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, root);
                instance.name = spec.prefab + "_" + Mathf.RoundToInt(spec.position.z);
                instance.transform.position = spec.position;
                instance.transform.localRotation =
                    Quaternion.Euler(spec.euler) * prefab.transform.localRotation;
                instance.transform.localScale = Vector3.one * spec.scale;
            }
        }

        private static void AddPlayerProps(Transform player)
        {
            var root = FindOrCreate(player, PlayerPropsRootName);
            Clear(root);

            AddRigProp("SM_Harness_Set", root, "Harness", new Vector3(0f, 1.8f, 5.1f), 0f);
            AddRigProp("SM_Prop_Bells", root, "Bells_L", new Vector3(-0.68f, 1.2f, 1.85f), 0f);
            AddRigProp("SM_Prop_Bells", root, "Bells_R", new Vector3(0.68f, 1.2f, 1.85f), 0f);
            AddRigProp("SM_Collar", root, "Collar", new Vector3(0.72f, 1.12f, -1.45f), 28f);
        }

        private static void AddRigProp(
            string prefabName,
            Transform parent,
            string instanceName,
            Vector3 localPosition,
            float yaw)
        {
            var prefab = VATCatalog.LoadPrefab(prefabName);

            if (prefab == null)
            {
                Debug.LogWarning("[Clous-End VAT] Falta el prop " + prefabName + " del jugador.");
                return;
            }

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.name = instanceName;
            instance.transform.localPosition = localPosition;
            instance.transform.localRotation =
                Quaternion.Euler(0f, yaw, 0f) * prefab.transform.localRotation;

            // El jugador ya tiene su CharacterController: estos adornos no
            // deben chocar ni bloquear los raycast del arma.
            foreach (var hitbox in instance.GetComponentsInChildren<Collider>(true))
                Object.DestroyImmediate(hitbox);
        }

        private static Transform FindOrCreate(Transform parent, string name)
        {
            var existing = parent.Find(name);

            if (existing != null)
                return existing;

            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            return go.transform;
        }

        private static void Clear(Transform root)
        {
            for (var i = root.childCount - 1; i >= 0; i--)
                Object.DestroyImmediate(root.GetChild(i).gameObject);
        }
    }
}
