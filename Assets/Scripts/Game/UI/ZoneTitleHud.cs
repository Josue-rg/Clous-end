using System;
using TMPro;
using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>
    /// Muestra el nombre de la zona del mapa cuando el jugador entra en ella
    /// y lo desvanece a los pocos segundos. Los cortes se ajustan en el Inspector.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ZoneTitleHud : MonoBehaviour
    {
        [Serializable]
        public sealed class ZoneLabel
        {
            public float startZ;
            public string title;
        }

        [SerializeField] private TMP_Text label;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private Transform viewer;
        [SerializeField] private float visibleDuration = 2.4f;
        [SerializeField] private float fadeDuration = 0.7f;

        [Tooltip("Ordenadas de menor a mayor por startZ.")]
        public ZoneLabel[] zones;

        private int currentIndex = -1;
        private float shownAt;

        private void Start()
        {
            Refresh(true);
        }

        private void Update()
        {
            if (group == null)
                return;

            Refresh(false);

            var elapsed = Time.time - shownAt;

            if (elapsed <= visibleDuration)
            {
                group.alpha = 1f;
                return;
            }

            group.alpha = 1f - Mathf.Clamp01((elapsed - visibleDuration) / Mathf.Max(0.01f, fadeDuration));
        }

        private void Refresh(bool force)
        {
            if (viewer == null || label == null || zones == null || zones.Length == 0)
                return;

            var index = 0;

            for (var i = 0; i < zones.Length; i++)
            {
                if (zones[i] == null)
                    continue;

                if (viewer.position.z >= zones[i].startZ)
                    index = i;
            }

            if (!force && index == currentIndex)
                return;

            currentIndex = index;
            label.text = zones[index].title;
            shownAt = Time.time;

            if (group != null)
                group.alpha = 1f;
        }
    }
}
