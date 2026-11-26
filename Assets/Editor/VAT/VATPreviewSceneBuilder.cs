using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace ClousEnd.EditorTools
{
    /// <summary>
    /// Escena de revisión del paquete VAT: coloca todos los prefabs en filas,
    /// con cámara, luz y suelo, para comprobar escala, orientación, colores y
    /// animación sin tocar la escena Game. Se regenera desde el menú.
    /// </summary>
    internal static class VATPreviewSceneBuilder
    {
        public static void Build()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            if (!VATCatalog.Exists("Santa"))
            {
                Debug.LogWarning("[Clous-End VAT] Todavía no hay prefabs. " +
                                 "Ejecuta 'Crear materiales y prefabs' antes de la escena de prueba.");
                return;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var root = new GameObject("VAT Preview").transform;

            BuildGround(root);
            BuildLighting(root);
            BuildCamera(root);
            BuildCharacters(root);
            BuildEnemies(root);
            BuildProps(root);

            RenderSettings.fog = false;
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.32f, 0.38f, 0.48f);
            RenderSettings.ambientEquatorColor = new Color(0.18f, 0.22f, 0.28f);
            RenderSettings.ambientGroundColor = new Color(0.07f, 0.08f, 0.11f);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, VATPaths.PreviewScenePath);
            AssetDatabase.SaveAssets();

            Debug.Log("[Clous-End VAT] Escena de prueba creada en " + VATPaths.PreviewScenePath +
                      ". Pulsa Play para ver la animación.");
        }

        private static void BuildGround(Transform root)
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.SetParent(root, false);
            ground.transform.localPosition = new Vector3(0f, 0f, 9f);
            ground.transform.localScale = new Vector3(8f, 1f, 7f);

            var collider = ground.GetComponent<Collider>();

            if (collider != null)
                Object.DestroyImmediate(collider);

            var material = VATMaterialFactory.GetOrCreateUnlit(
                "M_VAT_Preview_Ground",
                new Color(0.22f, 0.27f, 0.34f, 1f));

            if (material != null)
                ground.GetComponent<MeshRenderer>().sharedMaterial = material;

            GameObjectUtility.SetStaticEditorFlags(ground, StaticEditorFlags.BatchingStatic);
        }

        private static void BuildLighting(Transform root)
        {
            var lightGo = new GameObject("Preview Key Light");
            lightGo.transform.SetParent(root, false);
            lightGo.transform.localRotation = Quaternion.Euler(38f, 26f, 0f);

            var light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.97f, 0.92f);
            light.intensity = 1.85f;
            light.shadows = LightShadows.Soft;
            light.shadowStrength = 0.55f;
            RenderSettings.sun = light;

            var sky = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Login/Materials/M_Sky.mat");

            if (sky != null)
                RenderSettings.skybox = sky;
        }

        private static void BuildCamera(Transform root)
        {
            var cameraGo = new GameObject("Preview Camera");
            cameraGo.transform.SetParent(root, false);
            cameraGo.transform.localPosition = new Vector3(0f, 9f, -19f);
            cameraGo.transform.localRotation = Quaternion.Euler(12f, 0f, 0f);
            cameraGo.tag = "MainCamera";

            var camera = cameraGo.AddComponent<Camera>();
            camera.fieldOfView = 55f;
            camera.nearClipPlane = 0.2f;
            camera.farClipPlane = 300f;
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.allowHDR = true;
            camera.allowMSAA = true;
            camera.cullingMask = ~0;

            var data = cameraGo.GetComponent<UniversalAdditionalCameraData>();

            if (data == null)
                data = cameraGo.AddComponent<UniversalAdditionalCameraData>();

            data.renderPostProcessing = false;
            data.renderShadows = true;
            data.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;

            cameraGo.AddComponent<AudioListener>();
        }

        private static void BuildCharacters(Transform root)
        {
            var group = Group(root, "Characters");

            Place(group, "Santa", new Vector3(-10f, 0f, 0f));
            Place(group, "Sleigh", new Vector3(-5.5f, 0.05f, 0f));

            Place(group, "Reindeer_Rodolfo", new Vector3(-1f, 0f, 0f));
            Place(group, "Reindeer_Donner", new Vector3(2.6f, 0f, 0f));
            Place(group, "Reindeer_Vixen", new Vector3(6.2f, 0f, 0f));
            Place(group, "Reindeer_Cometa", new Vector3(9.8f, 0f, 0f));
        }

        private static void BuildEnemies(Transform root)
        {
            var group = Group(root, "Enemies");

            Place(group, "Sabueso", new Vector3(-12f, 0f, 8f));
            Place(group, "Garfio", new Vector3(-7.5f, 0f, 8f));
            Place(group, "Espora", new Vector3(-3.5f, 0f, 8f));
            Place(group, "Robot_Manto", new Vector3(0.5f, 0f, 8f));
            Place(group, "Robot_Torre", new Vector3(4.5f, 0f, 8f));
            Place(group, "NicolasCero", new Vector3(10.5f, 0f, 10f));
            Place(group, "Ship_Caza", new Vector3(-6f, 2.4f, 14f));
            Place(group, "Ship_Recolector", new Vector3(4f, 3.2f, 14f));
        }

        private static void BuildProps(Transform root)
        {
            var group = Group(root, "Props");
            var names = new[]
            {
                "SM_Weapon_Carbon", "SM_Weapon_Nogal", "SM_Weapon_Estrella", "SM_Weapon_Cascabel",
                "SM_GiftBox_Weapon", "SM_Harness_Set", "SM_Collar", "SM_Prop_Bells",
                "SM_Pickup_Ammo", "SM_Pickup_HelmetKit", "SM_Pickup_ReindeerShield",
                "SM_Meteor_1", "SM_Meteor_2", "SM_Meteor_3"
            };

            for (var i = 0; i < names.Length; i++)
            {
                var column = i % 5;
                var row = i / 5;
                Place(group, names[i], new Vector3(-12f + column * 5f, 0f, 18f + row * 3f));
            }
        }

        private static Transform Group(Transform root, string name)
        {
            var group = new GameObject(name).transform;
            group.SetParent(root, false);
            return group;
        }

        private static void Place(Transform parent, string prefabName, Vector3 position)
        {
            var prefab = VATCatalog.LoadPrefab(prefabName);

            if (prefab == null)
            {
                Debug.LogWarning("[Clous-End VAT] Falta el prefab " + prefabName + " para la escena de prueba.");
                return;
            }

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
            instance.name = prefabName;
            instance.transform.localPosition = position;

            // Los modelos miran a +Z (avanzan hacia el fondo). Se giran 180°
            // para que la escena de revisión muestre el frente.
            instance.transform.localRotation = Quaternion.Euler(0f, 180f, 0f) * instance.transform.localRotation;
        }
    }
}
