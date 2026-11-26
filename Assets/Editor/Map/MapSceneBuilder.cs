using System.Collections.Generic;
using System.Linq;
using ClousEnd.Game;
using ClousEnd.World;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClousEnd.EditorTools
{
    /// <summary>
    /// Genera una sola vez la escena Game completa: Distrito Aurora en blockout,
    /// con el vehículo y el personaje de pruebas, la cámara, el HUD y el sistema
    /// de zonas. El resultado queda guardado y se edita a mano en Unity.
    /// </summary>
    public static class MapSceneBuilder
    {
        public const string ScenePath = "Assets/Scenes/Game.unity";

        public static void Build()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            MapAssetFactory.ClearGeneratedMeshes();
            var assets = MapAssetFactory.Build();

            if (assets.WarmWindow == null || assets.CarPaint == null)
            {
                Debug.LogError("[Clous-End] Faltan materiales del mapa. Se cancela la escena.");
                return;
            }

            var root = new GameObject("Game").transform;

            var environment = new GameObject("Environment").transform;
            environment.SetParent(root, false);

            MapLightingBuilder.Build(environment, assets);
            MapBackdropBuilder.Build(environment, assets);
            MapGroundBuilder.Build(environment, assets);

            var zonesRoot = new GameObject("Zones").transform;
            zonesRoot.SetParent(environment, false);

            var zones = new List<ZoneStreamer.Zone>
            {
                MapZoneAfueras.Build(zonesRoot, assets),
                MapZoneCentro.Build(zonesRoot, assets),
                MapZoneFinanciero.Build(zonesRoot, assets),
                MapZoneRefugio.Build(zonesRoot, assets)
            };

            var player = MapPlayerBuilder.Build(root, assets);

            var goalGo = new GameObject("MissionGoal");
            goalGo.transform.SetParent(root, false);
            goalGo.transform.localPosition = new Vector3(0f, 0f, 1450f);

            var goal = goalGo.AddComponent<MissionGoal>();
            SerializedField.Set(goal, "player", player.Player.transform);
            SerializedField.Set(goal, "finishPanel", player.FinishPanel);
            SerializedField.Set(goal, "motor", player.Player.GetComponent<VehicleMotor>());
            SerializedField.Set(goal, "input", player.Player.GetComponent<PlayerInputReader>());
            SerializedField.Set(goal, "finishZ", 1450f);
            SerializedField.Set(goal, "halfWidth", 130f);
            SerializedField.Set(goal, "maxHeight", 400f);

            var worldGo = new GameObject("World");
            worldGo.transform.SetParent(root, false);

            var streamer = worldGo.AddComponent<ZoneStreamer>();
            streamer.zones = zones.ToArray();
            SerializedField.Set(streamer, "viewer", player.Player.transform);
            SerializedField.Set(streamer, "checkInterval", 0.3f);

            Selection.activeGameObject = player.Player;
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();

            AddToBuildSettings();
            Debug.Log("[Clous-End] Escena Game construida y guardada en " + ScenePath);
        }

        public static void Open()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) == null)
            {
                Debug.LogWarning("[Clous-End] Todavía no existe Game.unity. Usa Clous-End > Mapa > Construir escena Game.");
                return;
            }

            EditorSceneManager.OpenScene(ScenePath);
        }

        private static void AddToBuildSettings()
        {
            var scenes = EditorBuildSettings.scenes
                .Where(scene => scene.path != ScenePath)
                .ToList();

            var loginIndex = scenes.FindIndex(scene => scene.path == "Assets/Scenes/Login.unity");
            scenes.Insert(loginIndex >= 0 ? loginIndex + 1 : 0, new EditorBuildSettingsScene(ScenePath, true));

            if (scenes.All(scene => scene.path != "Assets/Scenes/SampleScene.unity"))
                scenes.Add(new EditorBuildSettingsScene("Assets/Scenes/SampleScene.unity", true));

            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
