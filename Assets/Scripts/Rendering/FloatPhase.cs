// CLOUS-END · Fase aleatoria para el shader FloatWobble
// Evita que todos los aliens floten y pulsen al mismo tiempo.

using UnityEngine;

namespace ClousEnd.Rendering
{
    /// <summary>Da una fase propia a cada instancia del shader FloatWobble.</summary>
    [DisallowMultipleComponent]
    public sealed class FloatPhase : MonoBehaviour
    {
        [SerializeField] private bool randomize = true;
        [SerializeField, Range(0f, 6.283f)] private float phase;
        [SerializeField, Min(0f)] private float amplitudeScale = 1f;

        private static readonly int PhaseId = Shader.PropertyToID("_Phase");
        private static readonly int FloatAmpId = Shader.PropertyToID("_FloatAmp");
        private static readonly int WaveAmpId = Shader.PropertyToID("_WaveAmp");

        private void Awake()
        {
            var targetRenderer = GetComponent<Renderer>();

            if (targetRenderer == null)
                return;

            var block = new MaterialPropertyBlock();
            var instancePhase = randomize ? Random.value * Mathf.PI * 2f : phase;

            var shared = targetRenderer.sharedMaterial;

            if (shared != null)
                shared.enableInstancing = true;

            targetRenderer.GetPropertyBlock(block);
            block.SetFloat(PhaseId, instancePhase);

            if (!Mathf.Approximately(amplitudeScale, 1f))
            {
                var floatAmp = shared != null ? shared.GetFloat(FloatAmpId) : 0.12f;
                var waveAmp = shared != null ? shared.GetFloat(WaveAmpId) : 0.06f;
                block.SetFloat(FloatAmpId, floatAmp * amplitudeScale);
                block.SetFloat(WaveAmpId, waveAmp * amplitudeScale);
            }

            targetRenderer.SetPropertyBlock(block);
        }
    }
}
