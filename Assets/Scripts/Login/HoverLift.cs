using UnityEngine;
using UnityEngine.EventSystems;

namespace ClousEnd.Login
{
    /// <summary>Elevación sutil del botón al pasar el cursor.</summary>
    [RequireComponent(typeof(RectTransform))]
    public sealed class HoverLift : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private RectTransform target;
        [SerializeField] private float lift = 2f;
        [SerializeField] private float duration = 0.12f;

        private Vector2 basePosition;
        private float progress;
        private bool hovered;

        private void Awake()
        {
            if (target == null)
                target = (RectTransform)transform;

            basePosition = target.anchoredPosition;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            hovered = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hovered = false;
        }

        private void Update()
        {
            if (target == null) return;

            var step = Time.unscaledDeltaTime / Mathf.Max(0.01f, duration);
            progress = Mathf.MoveTowards(progress, hovered ? 1f : 0f, step);

            var eased = Mathf.SmoothStep(0f, 1f, progress);
            target.anchoredPosition = basePosition + Vector2.up * (lift * eased);
        }

        private void OnDisable()
        {
            progress = 0f;

            if (target != null)
                target.anchoredPosition = basePosition;
        }
    }
}
