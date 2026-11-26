using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>Accesos del menú para importar y revisar el paquete VAT de Blender.</summary>
    public static class VATBuildMenu
    {
        [MenuItem("Clous-End/VAT/Importar todo (paquete + prefabs)", false, 38)]
        public static void ImportAll()
        {
            if (!VATPackageImporter.Import())
                return;

            VATPrefabBuilder.BuildAll();
        }

        [MenuItem("Clous-End/VAT/1 · Importar paquete desde BlenderSource", false, 40)]
        public static void ImportPackage()
        {
            VATPackageImporter.Import();
        }

        [MenuItem("Clous-End/VAT/2 · Crear materiales y prefabs", false, 41)]
        public static void BuildPrefabs()
        {
            VATPrefabBuilder.BuildAll();
        }

        [MenuItem("Clous-End/VAT/3 · Crear escena de prueba VAT", false, 42)]
        public static void BuildPreviewScene()
        {
            VATPreviewSceneBuilder.Build();
        }

        [MenuItem("Clous-End/VAT/Abrir escena de prueba VAT", false, 43)]
        public static void OpenPreviewScene()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(VATPaths.PreviewScenePath) == null)
            {
                Debug.LogWarning("[Clous-End VAT] Todavía no existe la escena de prueba. " +
                                 "Usa 'Crear escena de prueba VAT'.");
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            EditorSceneManager.OpenScene(VATPaths.PreviewScenePath);
        }

        [MenuItem("Clous-End/VAT/Entrar en Play con la escena de prueba", false, 45)]
        public static void PlayPreview()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>(VATPaths.PreviewScenePath) == null)
                VATPreviewSceneBuilder.Build();

            if (EditorSceneManager.GetActiveScene().path != VATPaths.PreviewScenePath)
            {
                if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                    return;

                EditorSceneManager.OpenScene(VATPaths.PreviewScenePath);
            }

            EditorApplication.isPlaying = true;
        }
    }
}
