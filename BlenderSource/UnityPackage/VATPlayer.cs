// CLOUS-END · Reproductor VAT
// Mueve un renderer por Vertex Animation Texture sin huesos.
// Un solo material + MaterialPropertyBlock = GPU instancing con fase propia.

using UnityEngine;

namespace ClousEnd.Rendering
{
    /// <summary>
    /// Reproduce un clip de Vertex Animation Texture. No crea objetos ni usa
    /// Animator: solo escribe <c>_Frame</c> en un MaterialPropertyBlock.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class VATPlayer : MonoBehaviour
    {
        private static readonly int FrameId = Shader.PropertyToID("_Frame");
        private static readonly int FramesId = Shader.PropertyToID("_Frames");
        private static readonly int PosTexId = Shader.PropertyToID("_PosTex");

        [Header("Clip")]
        [Tooltip("Frames del clip (debe coincidir con el JSON del VAT).")]
        [SerializeField, Min(2)] private int frames = 24;
        [Tooltip("Frames por segundo del clip original.")]
        [SerializeField, Min(0.01f)] private float fps = 30f;
        [Tooltip("Multiplicador de velocidad. 0 = congelado.")]
        [SerializeField, Min(0f)] private float speed = 1f;
        [Tooltip("Desfase inicial para que dos instancias no arranquen sincronizadas.")]
        [SerializeField] private bool randomPhase = true;
        [SerializeField, Range(0f, 1f)] private float phase01;
        [Tooltip("Si es false, la animación se para en el último frame.")]
        [SerializeField] private bool loop = true;

        [Header("Render")]
        [Tooltip("Renderer del LOD0. Si está vacío se busca en el mismo objeto o en los hijos.")]
        [SerializeField] private Renderer targetRenderer;

        [Header("Culling")]
        [Tooltip("Amplía los bounds para que el desplazamiento del VAT no parpadee.")]
        [SerializeField] private bool expandBounds = true;
        [Tooltip("Centro de los bounds en espacio local (X, Y, Z de Unity).")]
        [SerializeField] private Vector3 boundsCenter = Vector3.zero;
        [Tooltip("Extensión (mitad de tamaño) de los bounds en espacio local.")]
        [SerializeField] private Vector3 boundsExtent = Vector3.one;

        private MaterialPropertyBlock block;
        private Material[] clipMaterials;
        private float elapsed;
        private float phase;

        /// <summary>Frames del clip activo.</summary>
        public int Frames => frames;

        /// <summary>El clip activo se repite al terminar.</summary>
        public bool IsLooping => loop;

        /// <summary>Frame normalizado del clip, útil para animaciones sincronizadas.</summary>
        public float NormalizedTime => frames > 0 ? Mathf.Repeat(elapsed * fps + phase, frames) / frames : 0f;

        private void Awake()
        {
            EnsureRenderer();
            phase = randomPhase ? Random.value * frames : phase01 * frames;

            if (targetRenderer != null)
            {
                foreach (var shared in targetRenderer.sharedMaterials)
                {
                    if (shared != null)
                        shared.enableInstancing = true;
                }
            }

            if (expandBounds)
                ApplyBounds();
        }

        private void OnEnable()
        {
            EnsureRenderer();
            elapsed = 0f;

            var frame = loop ? Mathf.Repeat(phase, frames) : Mathf.Clamp(phase, 0f, frames - 1f);
            Apply(frame);
        }

        private void Update()
        {
            if (frames <= 1)
                return;

            elapsed += Time.deltaTime * speed;
            var frame = elapsed * fps + phase;

            if (loop)
                frame = Mathf.Repeat(frame, frames);
            else
                frame = Mathf.Clamp(frame, 0f, frames - 1f);

            Apply(frame);
        }

        private void OnValidate()
        {
            frames = Mathf.Max(2, frames);
            fps = Mathf.Max(0.01f, fps);
            speed = Mathf.Max(0f, speed);
        }

        /// <summary>
        /// Configura el clip desde el constructor de prefabs. Mantiene los bounds
        /// en espacio local de Unity.
        /// </summary>
        public void Configure(
            int clipFrames,
            float clipFps,
            bool clipLoop,
            bool clipRandomPhase,
            float clipPhase01,
            Vector3 cullingCenter,
            Vector3 cullingExtent)
        {
            frames = Mathf.Max(2, clipFrames);
            fps = Mathf.Max(0.01f, clipFps);
            loop = clipLoop;
            randomPhase = clipRandomPhase;
            phase01 = Mathf.Repeat(clipPhase01, 1f);
            boundsCenter = cullingCenter;
            boundsExtent = cullingExtent;
        }

        /// <summary>Fija una fase concreta (0–1) sin depender del azar.</summary>
        public void SetPhase(float normalizedPhase)
        {
            randomPhase = false;
            phase01 = Mathf.Repeat(normalizedPhase, 1f);
            phase = phase01 * frames;
        }

        /// <summary>Asigna el renderer del LOD0 (lo hace el constructor de prefabs).</summary>
        public void SetRenderer(Renderer renderer)
        {
            targetRenderer = renderer;
            block = null;
            clipMaterials = null;
        }

        /// <summary>
        /// Cambia de clip en caliente. Como un clip = una textura, el renderer
        /// instancia sus materiales la primera vez; es el único caso que pierde
        /// instancing.
        /// </summary>
        public void SetClip(Texture posTex, int clipFrames, float clipFps, bool clipLoop)
        {
            EnsureRenderer();

            frames = Mathf.Max(2, clipFrames);
            fps = Mathf.Max(0.01f, clipFps);
            loop = clipLoop;

            if (posTex != null && targetRenderer != null)
            {
                if (clipMaterials == null)
                    clipMaterials = targetRenderer.materials;

                if (clipMaterials != null)
                {
                    foreach (var material in clipMaterials)
                    {
                        if (material == null)
                            continue;

                        material.SetTexture(PosTexId, posTex);
                        material.SetFloat(FramesId, frames);
                        material.enableInstancing = true;
                    }
                }
            }

            elapsed = 0f;
            phase = randomPhase ? Random.value * frames : phase01 * frames;
            Apply(loop ? Mathf.Repeat(phase, frames) : Mathf.Clamp(phase, 0f, frames - 1f));
        }

        private void EnsureRenderer()
        {
            if (targetRenderer == null)
                targetRenderer = GetComponent<Renderer>();

            if (targetRenderer == null)
                targetRenderer = GetComponentInChildren<Renderer>(true);

            if (block == null)
                block = new MaterialPropertyBlock();
        }

        private void Apply(float frame)
        {
            if (targetRenderer == null || block == null)
                return;

            targetRenderer.GetPropertyBlock(block);
            block.SetFloat(FrameId, frame);
            targetRenderer.SetPropertyBlock(block);
        }

        private void ApplyBounds()
        {
            if (targetRenderer == null)
                return;

            targetRenderer.localBounds = new Bounds(boundsCenter, boundsExtent * 2f);
        }
    }
}
