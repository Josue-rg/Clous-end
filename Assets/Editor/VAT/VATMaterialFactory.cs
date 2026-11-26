using UnityEditor;
using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>
    /// Crea los materiales VAT (uno por clip) y los materiales de los props.
    /// Se actualizan en cada construcción para que el color del FBX y el EXR
    /// asignado siempre estén sincronizados con el JSON.
    /// </summary>
    internal static class VATMaterialFactory
    {
        public const string VatShaderName = "ClousEnd/VAT_Unlit";

        // La luz clave del mapa (MapLightingBuilder) es Euler(38, 26, 0).
        // El shader espera la dirección que apunta HACIA la luz.
        private static readonly Vector4 KeyLight = DirectionTowardsLight(38f, 26f);
        private static readonly int PosTexId = Shader.PropertyToID("_PosTex");
        private static readonly int FramesId = Shader.PropertyToID("_Frames");
        private static readonly int UvFramesId = Shader.PropertyToID("_UVFrames");
        private static readonly int VatAmountId = Shader.PropertyToID("_VATAmount");
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int AmbientId = Shader.PropertyToID("_Ambient");
        private static readonly int LightDirId = Shader.PropertyToID("_LightDir");
        private static readonly int RimId = Shader.PropertyToID("_Rim");

        public static Material GetOrCreateVat(
            string name,
            Color baseColor,
            Texture posTex,
            int frames,
            int uvFrames,
            bool animated)
        {
            var shader = Shader.Find(VatShaderName);

            if (shader == null)
            {
                Debug.LogError("[Clous-End VAT] No se encontró el shader " + VatShaderName +
                               ". Ejecuta primero 'Importar paquete desde BlenderSource'.");
                return null;
            }

            var material = LoadOrCreate(name, shader);

            if (material == null)
                return null;

            material.SetColor(BaseColorId, baseColor);
            material.SetColor("_Color", baseColor);
            material.SetTexture(PosTexId, posTex);
            material.SetFloat(FramesId, Mathf.Max(2, frames));
            material.SetFloat(UvFramesId, uvFrames);
            material.SetFloat(VatAmountId, animated ? 1f : 0f);
            material.SetFloat("_FlipY", 0f);
            material.SetFloat(AmbientId, 0.5f);
            material.SetVector(LightDirId, KeyLight);
            material.SetFloat(RimId, 0.45f);
            material.enableInstancing = true;

            EditorUtility.SetDirty(material);
            return material;
        }

        public static Material GetOrCreateLit(string name, Color baseColor)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit");

            if (shader == null)
            {
                Debug.LogError("[Clous-End VAT] No se encontró el shader URP Lit.");
                return null;
            }

            var material = LoadOrCreate(name, shader);

            if (material == null)
                return null;

            material.SetColor(BaseColorId, baseColor);
            material.SetColor("_Color", baseColor);
            material.SetFloat("_Smoothness", 0.24f);
            material.SetFloat("_Metallic", 0.05f);
            material.enableInstancing = true;

            EditorUtility.SetDirty(material);
            return material;
        }

        public static Material GetOrCreateUnlit(string name, Color baseColor)
        {
            var shader = Shader.Find("Universal Render Pipeline/Unlit");

            if (shader == null)
            {
                Debug.LogError("[Clous-End VAT] No se encontró el shader URP Unlit.");
                return null;
            }

            var material = LoadOrCreate(name, shader);

            if (material == null)
                return null;

            material.SetColor(BaseColorId, baseColor);
            material.SetColor("_Color", baseColor);
            material.enableInstancing = true;

            EditorUtility.SetDirty(material);
            return material;
        }

        /// <summary>Lee el color plano que guardó Blender en el material del FBX.</summary>
        public static Color ReadSourceColor(Material source)
        {
            if (source == null)
                return new Color(0.72f, 0.72f, 0.74f, 1f);

            if (source.HasProperty(BaseColorId))
                return source.GetColor(BaseColorId);

            if (source.HasProperty("_Color"))
                return source.GetColor("_Color");

            return new Color(0.72f, 0.72f, 0.74f, 1f);
        }

        private static Material LoadOrCreate(string name, Shader shader)
        {
            VATPaths.EnsureFolder(VATPaths.MaterialsRoot);

            var path = VATPaths.MaterialsRoot + "/" + name + ".mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (existing != null)
            {
                if (existing.shader != shader)
                    existing.shader = shader;

                return existing;
            }

            var material = new Material(shader) { name = name };
            AssetDatabase.CreateAsset(material, path);
            return material;
        }

        private static Vector4 DirectionTowardsLight(float pitch, float yaw)
        {
            var forward = Quaternion.Euler(pitch, yaw, 0f) * Vector3.forward;
            var towardsLight = -forward;
            return new Vector4(towardsLight.x, towardsLight.y, towardsLight.z, 0f);
        }
    }
}
