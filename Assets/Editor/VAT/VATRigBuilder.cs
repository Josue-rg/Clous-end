using ClousEnd.Game;
using UnityEditor;
using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>
    /// Sustituye los placeholders del jugador por los modelos del paquete VAT:
    /// trineo, Santa y los cuatro renos. Solo se ejecuta en el editor; el
    /// resultado queda como prefabs instanciados y editables en la escena.
    /// </summary>
    internal static class VATRigBuilder
    {
        public const string SleighName = "Sleigh";
        public const string SantaName = "Santa";
        public const string SquadName = "ReindeerSquad";
        public const string WeaponRackName = "WeaponRack";

        public static readonly Vector3 SleighOffset = new Vector3(0f, 0.05f, 0f);
        public static readonly Vector3 SantaOffset = new Vector3(0f, -1f, 0f);
        public static readonly Vector3 SquadOffset = new Vector3(0f, 0.6f, 4.6f);

        private static readonly string[] ReindeerNames =
        {
            "Reindeer_Rodolfo",
            "Reindeer_Donner",
            "Reindeer_Vixen",
            "Reindeer_Cometa"
        };

        private static readonly string[] WeaponNames =
        {
            "SM_Weapon_Carbon",
            "SM_Weapon_Nogal",
            "SM_Weapon_Estrella",
            "SM_Weapon_Cascabel"
        };

        private static readonly Vector3[] ReindeerOffsets =
        {
            new Vector3(-0.9f, 0f, 0f),
            new Vector3(0.9f, 0f, 0f),
            new Vector3(-0.9f, 0f, 3.6f),
            new Vector3(0.9f, 0f, 3.6f)
        };

        public static bool HasCorePrefabs =>
            VATCatalog.Exists("Sleigh") && VATCatalog.Exists("Santa");

        /// <summary>Instancia el trineo, Santa y los renos bajo el jugador.</summary>
        public static void Build(Transform player, Transform characterPivot)
        {
            RemoveExisting(player);

            Spawn(SleighName, player, SleighOffset);

            if (characterPivot != null)
            {
                Spawn(SantaName, characterPivot, SantaOffset);
                BuildWeaponRack(characterPivot);
            }

            var squad = new GameObject(SquadName);
            squad.transform.SetParent(player, false);
            squad.transform.localPosition = SquadOffset;
            squad.transform.localRotation = Quaternion.identity;

            for (var i = 0; i < ReindeerNames.Length; i++)
            {
                var instance = Spawn(ReindeerNames[i], squad.transform, ReindeerOffsets[i]);

                if (instance == null)
                    continue;

                var playback = instance.GetComponent<ClousEnd.Rendering.VATPlayer>();

                if (playback != null)
                    playback.SetPhase(i * 0.25f);
            }

            Debug.Log("[Clous-End VAT] Rig del jugador montado: trineo, Santa y " +
                      ReindeerNames.Length + " renos.");
        }

        /// <summary>Borra las instancias VAT previas para que el proceso sea repetible.</summary>
        public static void RemoveExisting(Transform player)
        {
            DestroyChild(player, SleighName);
            DestroyChild(player, SquadName);

            var characterPivot = player.Find("CharacterPivot");

            if (characterPivot != null)
            {
                DestroyChild(characterPivot, SantaName);
                var aimPivot = characterPivot.Find("AimPivot");

                if (aimPivot != null)
                    DestroyChild(aimPivot, WeaponRackName);
            }
        }

        /// <summary>
        /// Coloca los cuatro modelos de arma en la mano y añade el conmutador que
        /// muestra solo la equipada. No crea nada en runtime: las armas viven en
        /// el rig como hijos desactivados.
        /// </summary>
        private static void BuildWeaponRack(Transform characterPivot)
        {
            var aimPivot = characterPivot.Find("AimPivot");

            if (aimPivot == null)
            {
                Debug.LogWarning("[Clous-End VAT] El personaje no tiene AimPivot; no se colocan las armas.");
                return;
            }

            var rack = new GameObject(WeaponRackName);
            rack.transform.SetParent(aimPivot, false);
            rack.transform.localPosition = new Vector3(0f, 0f, 0.35f);
            rack.transform.localRotation = Quaternion.identity;

            var visuals = new GameObject[WeaponNames.Length];

            for (var i = 0; i < WeaponNames.Length; i++)
            {
                var instance = Spawn(WeaponNames[i], rack.transform, Vector3.zero);

                if (instance == null)
                    continue;

                // El arma es solo visual: sin collider para no chocar con el
                // CharacterController ni bloquear los raycast del jugador.
                foreach (var hitbox in instance.GetComponentsInChildren<Collider>(true))
                    Object.DestroyImmediate(hitbox);

                instance.SetActive(false);
                visuals[i] = instance;
            }

            if (visuals.Length > 0 && visuals[0] != null)
                visuals[0].SetActive(true);

            var controller = characterPivot.GetComponent<WeaponController>();
            var switcher = characterPivot.GetComponent<WeaponVisualSwitcher>();

            if (switcher == null)
                switcher = characterPivot.gameObject.AddComponent<WeaponVisualSwitcher>();

            SerializedField.Set(switcher, "controller", controller);
            SerializedField.SetArray(switcher, "visuals", visuals);

            Debug.Log("[Clous-End VAT] Armas colocadas en la mano: " + WeaponNames.Length + ".");
        }

        private static GameObject Spawn(
            string prefabName,
            Transform parent,
            Vector3 localPosition)
        {
            var prefab = VATCatalog.LoadPrefab(prefabName);

            if (prefab == null)
            {
                Debug.LogWarning("[Clous-End VAT] Falta el prefab " + VATCatalog.PrefabPath(prefabName) +
                                 ". Ejecuta 'Crear materiales y prefabs'.");
                return null;
            }

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.name = prefabName;
            instance.transform.localPosition = localPosition;
            return instance;
        }

        private static void DestroyChild(Transform parent, string childName)
        {
            var child = parent.Find(childName);

            if (child != null)
                Object.DestroyImmediate(child.gameObject);
        }
    }
}
