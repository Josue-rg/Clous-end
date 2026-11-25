using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClousEnd.Login
{
    /// <summary>
    /// Entrada escalonada de los bloques de la pantalla. No crea objetos:
    /// anima CanvasGroup y RectTransform que ya existen en la escena.
    /// </summary>
    public sealed class LoginIntroAnimator : MonoBehaviour
    {
        [System.Serializable]
        public sealed class IntroTarget
        {
            public CanvasGroup group;
            public RectTransform rect;
            [Min(0f)] public float delay;
            [Min(0.01f)] public float duration = 0.6f;
            public Vector2 offset = new Vector2(0f, 18f);
        }

        [SerializeField] private List<IntroTarget> targets = new List<IntroTarget>();
        [SerializeField] private float startDelay = 0.05f;
        [SerializeField] private bool playOnEnable = true;

        private readonly List<Vector2> basePositions = new List<Vector2>();

        private void Awake()
        {
            CaptureBasePositions();
        }

        private void OnEnable()
        {
            // En el editor la pantalla debe verse completa para poder editarla.
            if (!Application.isPlaying || !playOnEnable)
                return;

            ApplyStartState();
            StartCoroutine(PlayRoutine());
        }

        private void OnDisable()
        {
            if (!Application.isPlaying)
                return;

            StopAllCoroutines();
            RestoreBaseState();
        }

        private void CaptureBasePositions()
        {
            basePositions.Clear();

            foreach (var target in targets)
            {
                if (target != null && target.rect != null)
                    basePositions.Add(target.rect.anchoredPosition);
                else
                    basePositions.Add(Vector2.zero);
            }
        }

        private void ApplyStartState()
        {
            for (var i = 0; i < targets.Count; i++)
            {
                var target = targets[i];
                if (target == null || target.group == null) continue;

                target.group.alpha = 0f;

                if (target.rect != null)
                    target.rect.anchoredPosition = basePositions[i] + target.offset;
            }
        }

        private IEnumerator PlayRoutine()
        {
            if (startDelay > 0f)
                yield return new WaitForSecondsRealtime(startDelay);

            var elapsed = 0f;
            var longest = 0f;

            foreach (var target in targets)
            {
                if (target != null)
                    longest = Mathf.Max(longest, target.delay + target.duration);
            }

            while (elapsed < longest)
            {
                elapsed += Time.unscaledDeltaTime;

                for (var i = 0; i < targets.Count; i++)
                {
                    var target = targets[i];
                    if (target == null || target.group == null) continue;

                    var local = Mathf.Clamp01((elapsed - target.delay) / Mathf.Max(0.01f, target.duration));
                    var eased = Mathf.SmoothStep(0f, 1f, local);

                    target.group.alpha = eased;

                    if (target.rect != null)
                        target.rect.anchoredPosition = Vector2.LerpUnclamped(
                            basePositions[i] + target.offset,
                            basePositions[i],
                            eased);
                }

                yield return null;
            }

            RestoreBaseState();
        }

        private void RestoreBaseState()
        {
            for (var i = 0; i < targets.Count; i++)
            {
                var target = targets[i];
                if (target == null) continue;

                if (target.group != null)
                    target.group.alpha = 1f;

                if (target.rect != null && i < basePositions.Count)
                    target.rect.anchoredPosition = basePositions[i];
            }
        }
    }
}
