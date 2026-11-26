// CLOUS-END · Reproductor VAT
// Mueve un renderer por Vertex Animation Texture sin huesos.
// Un solo material + MaterialPropertyBlock = GPU instancing con fase propia.

using UnityEngine;

[DisallowMultipleComponent]
public class VATPlayer : MonoBehaviour
{
    [Header("Clip")]
    [Tooltip("Frames del clip (debe coincidir con el JSON del VAT).")]
    public int frames = 24;
    [Tooltip("Frames por segundo del clip original.")]
    public float fps = 30f;
    [Tooltip("Multiplicador de velocidad. 0 = congelado.")]
    public float speed = 1f;
    [Tooltip("Desfase inicial en frames para que no arranquen sincronizados.")]
    public bool randomPhase = true;
    [Range(0f, 1f)]
    public float phase01 = 0f;
    [Tooltip("Si es false, la animacion se para en el ultimo frame.")]
    public bool loop = true;

    [Header("Culling")]
    [Tooltip("Amplia los bounds para que el desplazamiento del VAT no parpadee.")]
    public bool expandBounds = true;
    public Vector3 boundsCenter = Vector3.zero;
    public Vector3 boundsExtent = new Vector3(1f, 1f, 1f);

    [Header("Debug")]
    public bool useSharedMaterialFrame = false;

    static readonly int FrameId = Shader.PropertyToID("_Frame");

    Renderer _renderer;
    MaterialPropertyBlock _block;
    float _time;
    float _phase;

    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _block = new MaterialPropertyBlock();
        _phase = phase01 * frames;
        if (randomPhase) _phase = Random.value * frames;

        // el shader usa _Frame como propiedad instanciada
        var mat = _renderer.sharedMaterial;
        if (mat != null) mat.enableInstancing = true;

        if (expandBounds)
            ApplyBounds();
    }

    void OnEnable()
    {
        _time = 0f;
        Apply(0f);
    }

    void Update()
    {
        if (frames <= 1) return;

        _time += Time.deltaTime * speed;
        float frame = _time * fps + _phase;

        if (loop)
        {
            frame = Mathf.Repeat(frame, frames);
        }
        else
        {
            frame = Mathf.Clamp(frame, 0f, frames - 1);
        }

        Apply(frame);
    }

    void Apply(float frame)
    {
        if (_renderer == null) return;

        if (useSharedMaterialFrame)
        {
            var mat = _renderer.material;
            if (mat != null) mat.SetFloat(FrameId, frame);
            return;
        }

        _renderer.GetPropertyBlock(_block);
        _block.SetFloat(FrameId, frame);
        _renderer.SetPropertyBlock(_block);
    }

    void ApplyBounds()
    {
        var mf = GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
        {
            var b = mf.sharedMesh.bounds;
            b.center = boundsCenter;
            b.extent = boundsExtent;
            mf.sharedMesh.bounds = b;
            return;
        }
        _renderer.localBounds = new Bounds(boundsCenter, boundsExtent * 2f);
    }

    /// <summary>Cambia de clip en caliente (otra textura y otro numero de frames).</summary>
    public void SetClip(Texture posTex, int newFrames, float newFps)
    {
        frames = newFrames;
        fps = newFps;
        var mat = _renderer.material;
        if (mat != null && posTex != null)
            mat.SetTexture("_PosTex", posTex);
    }

    void OnValidate()
    {
        frames = Mathf.Max(2, frames);
        if (expandBounds) ApplyBounds();
    }
}
