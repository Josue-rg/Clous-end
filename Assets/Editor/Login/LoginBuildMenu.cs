using ClousEnd.Login;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>Accesos directos para construir, abrir y probar la pantalla de login.</summary>
    public static class LoginBuildMenu
    {
        [MenuItem("Clous-End/Login/Construir escena de Login", false, 10)]
        public static void BuildFromMenu()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(LoginSceneBuilder.ScenePath) != null)
            {
                var rebuild = EditorUtility.DisplayDialog(
                    "Reconstruir Login",
                    "Esto vuelve a generar Login.unity y reemplaza los cambios hechos a mano en esa escena.\n\n¿Continuar?",
                    "Reconstruir",
                    "Cancelar");

                if (!rebuild)
                    return;
            }

            LoginSceneBuilder.Build();
        }

        [MenuItem("Clous-End/Login/Abrir escena de Login", false, 11)]
        public static void OpenFromMenu()
        {
            LoginSceneBuilder.Open();
        }

        [MenuItem("Clous-End/Login/Abrir configuración", false, 12)]
        public static void SelectConfig()
        {
            var config = AssetDatabase.LoadAssetAtPath<LoginConfig>("Assets/Config/LoginConfig.asset");
            if (config == null)
            {
                Debug.LogWarning("[Clous-End] Aún no existe LoginConfig. Construye la escena primero.");
                return;
            }

            Selection.activeObject = config;
            EditorGUIUtility.PingObject(config);
        }

        [MenuItem("Clous-End/Login/Entrar en Play Mode", false, 13)]
        public static void PlayFromMenu()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(LoginSceneBuilder.ScenePath) == null)
            {
                LoginSceneBuilder.Build();
            }

            if (EditorSceneManager.GetActiveScene().path != LoginSceneBuilder.ScenePath)
                LoginSceneBuilder.Open();

            EditorApplication.isPlaying = true;
        }
    }
}
