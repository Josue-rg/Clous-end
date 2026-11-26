using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>
    /// Barra de vida en el mundo para un enemigo. Las barras se crean en el
    /// editor como objetos de la escena; este componente solo las coloca,
    /// las orienta a la cámara y ajusta el ancho del relleno.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WorldHealthBar : MonoBehaviour
    {
        [Header("Objetivo")]
        [SerializeField] private Transform target;
        [SerializeField] private EnemyAgent agent;

        [Header("Barra")]
        [SerializeField] private Transform fill;
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private float width = 1.7f;
        [SerializeField] private float heightOffset = 4f;

        private Camera viewCamera;

        public void Configure(
            Transform barTarget,
            EnemyAgent barAgent,
            Transform barFill,
            Renderer[] barRenderers,
            float barHeightOffset,
            float barWidth)
        {
            target = barTarget;
            agent = barAgent;
            fill = barFill;
            renderers = barRenderers;
            heightOffset = barHeightOffset;
            width = Mathf.Max(0.1f, barWidth);
        }

        private void Awake()
        {
            if (renderers == null || renderers.Length == 0)
                renderers = GetComponentsInChildren<Renderer>(true);
        }

        private void LateUpdate()
        {
            if (target == null)
                return;

            if (viewCamera == null)
                viewCamera = Camera.main;

            var visible = target.gameObject.activeInHierarchy && (agent == null || agent.IsAlive);
            SetVisible(visible);

            if (!visible)
                return;

            transform.position = target.position + Vector3.up * heightOffset;

            if (viewCamera != null)
                transform.rotation = viewCamera.transform.rotation;

            if (fill == null)
                return;

            var ratio = agent != null ? Mathf.Clamp01(agent.Health01) : 1f;
            var scale = fill.localScale;
            scale.x = width * ratio;
            fill.localScale = scale;

            var position = fill.localPosition;
            position.x = -width * 0.5f + width * ratio * 0.5f;
            fill.localPosition = position;
        }

        private void SetVisible(bool visible)
        {
            if (renderers == null)
                return;

            for (var i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] != null)
                    renderers[i].enabled = visible;
            }
        }
    }
}
