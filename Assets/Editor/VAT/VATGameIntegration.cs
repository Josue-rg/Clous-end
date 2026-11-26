using System.IO;
using ClousEnd.Game;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>
    /// Integra los prefabs VAT en la escena Game ya construida, sin regenerar el
    /// mapa: borra los placeholders del jugador y coloca trineo, Santa y renos
    /// conservando los pivotes, el CharacterController y los scripts.
    /// </summary>
    internal static class VATGameIntegration
    {
        private const string GameScenePath = "Assets/Scenes/Game.unity";

        [MenuItem("Clous-End/VAT/4 · Integrar modelos en la escena Game", false, 44)]
        public static void Integrate()
        {
            if (!VATRigBuilder.HasCorePrefabs)
            {
                Debug.LogWarning("[Clous-End VAT] Faltan los prefabs del trineo o de Santa. " +
                                 "Ejecuta 'Crear materiales y prefabs' primero.");
                return;
            }

            if (!File.Exists(VATPaths.ToFull(GameScenePath)))
            {
                Debug.LogWarning("[Clous-End VAT] No existe " + GameScenePath +
                                 ". Construye antes el mapa desde Clous-End > Mapa.");
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            var scene = EditorSceneManager.OpenScene(GameScenePath, OpenSceneMode.Single);
            var motor = Object.FindAnyObjectByType<VehicleMotor>();

            if (motor == null)
            {
                Debug.LogError("[Clous-End VAT] No se encontró el jugador (VehicleMotor) en la escena Game.");
                return;
            }

            var player = motor.transform;
            var characterPivot = player.Find("CharacterPivot");

            if (characterPivot == null)
            {
                Debug.LogError("[Clous-End VAT] El jugador no tiene CharacterPivot; la escena no tiene " +
                               "la estructura esperada por el blockout.");
                return;
            }

            RemovePlaceholders(player, characterPivot);
            VATRigBuilder.Build(player, characterPivot);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, GameScenePath);
            AssetDatabase.SaveAssets();

            Debug.Log("[Clous-End VAT] Escena Game actualizada con los modelos de Blender. " +
                      "Revisa la altura del trineo y el hueco de Santa en el Inspector y dale Play.");
        }

        private static void RemovePlaceholders(Transform player, Transform characterPivot)
        {
            DestroyChild(player, "PH_CarBody");
            DestroyChild(player, "PH_HeadlightL");
            DestroyChild(player, "PH_HeadlightR");
            DestroyChild(characterPivot, "PH_Character");

            var aimPivot = characterPivot.Find("AimPivot");

            if (aimPivot != null)
                DestroyChild(aimPivot, "PH_Gun");
        }

        private static void DestroyChild(Transform parent, string name)
        {
            var child = parent.Find(name);

            if (child != null)
                Object.DestroyImmediate(child.gameObject);
        }
    }
}
