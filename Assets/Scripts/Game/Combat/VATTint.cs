using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>
    /// Tiñe un renderer VAT sin duplicar materiales. Se usa para los renos de
    /// obsidiana del jefe: oscurece el color y añade emisión violeta.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class VATTint : MonoBehaviour
    {
        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int EmissionId = Shader.PropertyToID("_Emission");

        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private Color tint = new Color(0.045f, 0.035f, 0.06f, 1f);
        [SerializeField] private Color emission = new Color(0.22f, 0.04f, 0.42f, 1f);

        private void Awake()
        {
            Apply();
        }

        public void Configure(Renderer renderer, Color color, Color glow)
        {
            targetRenderer = renderer;
            tint = color;
            emission = glow;
        }

        public void Apply()
        {
            if (targetRenderer == null)
                targetRenderer = GetComponentInChildren<Renderer>(true);

            if (targetRenderer == null)
                return;

            var block = new MaterialPropertyBlock();
            targetRenderer.GetPropertyBlock(block);
            block.SetColor(BaseColorId, tint);
            block.SetColor(EmissionId, emission);
            targetRenderer.SetPropertyBlock(block);
        }
    }
}
