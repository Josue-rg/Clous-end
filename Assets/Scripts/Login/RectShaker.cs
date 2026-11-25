using System.Collections;
using UnityEngine;

namespace ClousEnd.Login
{
    /// <summary>Sacudida corta y reutilizable para errores de formulario.</summary>
    public sealed class RectShaker : MonoBehaviour
    {
        [SerializeField] private RectTransform target;
        [SerializeField] private float amplitude = 8f;
        [SerializeField] private float duration = 0.22f;
        [SerializeField] private int cycles = 3;

        private Coroutine routine;

        public void Play()
        {
            if (target == null)
                target = (RectTransform)transform;

            if (routine != null)
                StopCoroutine(routine);

            routine = StartCoroutine(ShakeRoutine());
        }

        private IEnumerator ShakeRoutine()
        {
            var basePosition = target.anchoredPosition;
            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                var t = Mathf.Clamp01(elapsed / duration);
                var wave = Mathf.Sin(t * Mathf.PI * 2f * cycles);
                target.anchoredPosition = basePosition + Vector2.right * (wave * amplitude * (1f - t));
                yield return null;
            }

            target.anchoredPosition = basePosition;
            routine = null;
        }
    }
}
