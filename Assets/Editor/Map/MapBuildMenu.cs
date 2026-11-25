using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>Accesos del menú para construir, abrir y probar el mapa.</summary>
    public static class MapBuildMenu
    {
        [MenuItem("Clous-End/Mapa/Construir escena Game (blockout)", false, 20)]
        public static void BuildFromMenu()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(MapSceneBuilder.ScenePath) != null)
            {
                var rebuild = EditorUtility.DisplayDialog(
                    "Reconstruir Game",
                    "Esto vuelve a generar Game.unity y reemplaza los cambios hechos a mano en esa escena.\n\n¿Continuar?",
                    "Reconstruir",
                    "Cancelar");

                if (!rebuild)
                    return;
            }

            MapSceneBuilder.Build();
        }

        [MenuItem("Clous-End/Mapa/Abrir escena Game", false, 21)]
        public static void OpenFromMenu()
        {
            MapSceneBuilder.Open();
        }

        [MenuItem("Clous-End/Mapa/Entrar en Play Mode", false, 22)]
        public static void PlayFromMenu()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(MapSceneBuilder.ScenePath) == null)
                MapSceneBuilder.Build();

            if (EditorSceneManager.GetActiveScene().path != MapSceneBuilder.ScenePath)
                MapSceneBuilder.Open();

            EditorApplication.isPlaying = true;
        }
    }
}
