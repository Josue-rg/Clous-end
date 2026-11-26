// CLOUS-END · Fase aleatoria para el shader FloatWobble
// Evita que todos los aliens floten y pulsen al mismo tiempo.

using UnityEngine;

[DisallowMultipleComponent]
public class FloatPhase : MonoBehaviour
{
    public bool randomize = true;
    [Range(0f, 6.283f)]
    public float phase = 0f;
    public float amplitudeScale = 1f;

    static readonly int PhaseId = Shader.PropertyToID("_Phase");
    static readonly int FloatAmpId = Shader.PropertyToID("_FloatAmp");
    static readonly int WaveAmpId = Shader.PropertyToID("_WaveAmp");

    Renderer _renderer;
    MaterialPropertyBlock _block;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _block = new MaterialPropertyBlock();

        float p = randomize ? Random.value * Mathf.PI * 2f : phase;

        var mat = _renderer.sharedMaterial;
        if (mat != null) mat.enableInstancing = true;

        _renderer.GetPropertyBlock(_block);
        _block.SetFloat(PhaseId, p);
        if (!Mathf.Approximately(amplitudeScale, 1f))
        {
            float fa = mat != null ? mat.GetFloat(FloatAmpId) : 0.12f;
            float wa = mat != null ? mat.GetFloat(WaveAmpId) : 0.06f;
            _block.SetFloat(FloatAmpId, fa * amplitudeScale);
            _block.SetFloat(WaveAmpId, wa * amplitudeScale);
        }
        _renderer.SetPropertyBlock(_block);
    }
}
