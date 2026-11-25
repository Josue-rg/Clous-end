using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClousEnd.EditorTools
{
    /// <summary>
    /// Genera la escena completa de login una sola vez. El resultado queda guardado
    /// y se puede editar en Jerarquía, Inspector y Scene View.
    /// </summary>
    public static class LoginSceneBuilder
    {
        public const string ScenePath = "Assets/Scenes/Login.unity";

        public static void Build()
        {
            // La escena se crea primero: al cerrar la escena anterior Unity puede
            // descargar assets que todavía no están referenciados por nada.
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            var assets = LoginAssetFactory.Build();

            if (assets.Config == null)
            {
                Debug.LogError("[Clous-End] No se pudo crear LoginConfig. Se cancela la escena.");
                return;
            }

            var environment = new GameObject("Environment").transform;
            var camera = LoginEnvironmentBuilder.Build(environment, assets);

            var ui = new GameObject("UI").transform;
            LoginUIBuilder.Build(ui, assets, assets.Config);

            if (camera != null)
                camera.transform.SetAsFirstSibling();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();

            AddToBuildSettings();
            Debug.Log("[Clous-End] Escena de login construida y guardada en " + ScenePath);
        }

        public static void Open()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) == null)
            {
                Debug.LogWarning("[Clous-End] Todavía no existe Login.unity. Usa Clous-End > Login > Construir escena de Login.");
                return;
            }

            EditorSceneManager.OpenScene(ScenePath);
        }

        private static void AddToBuildSettings()
        {
            var scenes = EditorBuildSettings.scenes
                .Where(scene => scene.path != ScenePath)
                .ToList();

            scenes.Insert(0, new EditorBuildSettingsScene(ScenePath, true));

            if (scenes.All(scene => scene.path != "Assets/Scenes/SampleScene.unity"))
                scenes.Add(new EditorBuildSettingsScene("Assets/Scenes/SampleScene.unity", true));

            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
