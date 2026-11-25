using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>Traza un disparo reutilizando un LineRenderer que ya vive en la escena.</summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(LineRenderer))]
    public sealed class TracerBeam : MonoBehaviour
    {
        [SerializeField] private float lifetime = 0.07f;
        [SerializeField] private float width = 0.07f;

        private LineRenderer line;
        private float hideAt;

        private void Awake()
        {
            line = GetComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.widthMultiplier = width;
            line.enabled = false;
        }

        public void Show(Vector3 from, Vector3 to)
        {
            if (line == null)
                line = GetComponent<LineRenderer>();

            line.widthMultiplier = width;
            line.SetPosition(0, from);
            line.SetPosition(1, to);
            line.enabled = true;
            hideAt = Time.time + lifetime;
        }

        private void LateUpdate()
        {
            if (line != null && line.enabled && Time.time >= hideAt)
                line.enabled = false;
        }
    }
}
