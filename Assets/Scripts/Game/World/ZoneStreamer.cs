using System;
using UnityEngine;

namespace ClousEnd.World
{
    /// <summary>
    /// Activa y desactiva zonas completas del mapa según la distancia al
    /// jugador. Es la primera capa de rendimiento: lo que queda muy lejos ni
    /// se dibuja, ni colisiona, ni actualiza sombras. La niebla tapa el cambio.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ZoneStreamer : MonoBehaviour
    {
        [Serializable]
        public sealed class Zone
        {
            public string label;
            public Transform root;
            public Bounds bounds;
            public float cullDistance = 500f;
        }

        [SerializeField] private Transform viewer;

        [Tooltip("Zonas del mapa. Cada una se apaga cuando el jugador se aleja más de cullDistance.")]
        public Zone[] zones;

        [SerializeField] private float checkInterval = 0.25f;

        private float nextCheckTime;
        private bool[] zoneActive;

        private void Start()
        {
            if (zones == null || zones.Length == 0)
                return;

            zoneActive = new bool[zones.Length];

            for (var i = 0; i < zones.Length; i++)
                zoneActive[i] = zones[i].root != null && zones[i].root.gameObject.activeSelf;

            Apply(true);
        }

        private void Update()
        {
            if (viewer == null || zones == null || zones.Length == 0)
                return;

            if (Time.time < nextCheckTime)
                return;

            nextCheckTime = Time.time + Mathf.Max(0.05f, checkInterval);
            Apply(false);
        }

        private void Apply(bool force)
        {
            var viewerPosition = viewer.position;

            for (var i = 0; i < zones.Length; i++)
            {
                var zone = zones[i];

                if (zone == null || zone.root == null)
                    continue;

                var distance = zone.cullDistance;
                var shouldBeActive = zone.bounds.SqrDistance(viewerPosition) <= distance * distance;

                if (!force && shouldBeActive == zoneActive[i])
                    continue;

                zone.root.gameObject.SetActive(shouldBeActive);
                zoneActive[i] = shouldBeActive;
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (zones == null)
                return;

            foreach (var zone in zones)
            {
                if (zone == null)
                    continue;

                Gizmos.color = new Color(0.35f, 0.8f, 1f, 0.45f);
                Gizmos.DrawWireCube(zone.bounds.center, zone.bounds.size);
            }
        }
    }
}
