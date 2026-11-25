using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>
    /// Marca de impacto reutilizable. El objeto existe en la escena y solo se
    /// activa visualmente durante un instante; no se instancia nada al disparar.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ImpactFlash : MonoBehaviour
    {
        [SerializeField] private float lifetime = 0.16f;
        [SerializeField] private float startScale = 0.55f;
        [SerializeField] private float endScale = 0.05f;

        private Renderer cachedRenderer;
        private float hideAt;
        private bool playing;

        private void Awake()
        {
            cachedRenderer = GetComponent<Renderer>();

            if (cachedRenderer != null)
                cachedRenderer.enabled = false;
        }

        public void Play(Vector3 worldPosition)
        {
            if (cachedRenderer == null)
                cachedRenderer = GetComponent<Renderer>();

            transform.position = worldPosition;
            transform.localScale = Vector3.one * startScale;

            if (cachedRenderer != null)
                cachedRenderer.enabled = true;

            hideAt = Time.time + lifetime;
            playing = true;
        }

        private void LateUpdate()
        {
            if (!playing)
                return;

            var remaining = Mathf.Clamp01((hideAt - Time.time) / lifetime);
            var scale = Mathf.Lerp(endScale, startScale, remaining);
            transform.localScale = Vector3.one * scale;

            if (Time.time < hideAt)
                return;

            playing = false;

            if (cachedRenderer != null)
                cachedRenderer.enabled = false;
        }
    }
}
