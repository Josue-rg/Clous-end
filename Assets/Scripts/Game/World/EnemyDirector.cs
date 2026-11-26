using System;
using UnityEngine;

namespace ClousEnd.Game
{
    [Serializable]
    public sealed class EnemyWave
    {
        public string name = "OLEADA";
        public float triggerZ = 150f;
        public EnemyAgent[] agents = new EnemyAgent[0];
    }

    /// <summary>
    /// Activa y desactiva oleadas precolocadas según la Z del jugador. No
    /// instancia enemigos en runtime: los pools viven en la escena.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EnemyDirector : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private EnemyWave[] waves = new EnemyWave[0];
        [SerializeField] private float activateAhead = 120f;

        private bool[] active;

        private void Start()
        {
            active = new bool[waves != null ? waves.Length : 0];

            for (var i = 0; i < active.Length; i++)
                Deactivate(waves[i]);
        }

        private void Update()
        {
            if (player == null || waves == null)
                return;

            var z = player.position.z;

            for (var i = 0; i < waves.Length; i++)
            {
                var wave = waves[i];

                if (wave == null)
                    continue;

                if (!active[i] && z >= wave.triggerZ - activateAhead)
                {
                    Activate(wave);
                    active[i] = true;
                }
            }
        }

        public void SetWaves(EnemyWave[] newWaves)
        {
            waves = newWaves;
        }

        /// <summary>Vuelve a dejar todas las oleadas dormidas (checkpoint).</summary>
        public void ResetWaves()
        {
            if (waves == null)
                return;

            for (var i = 0; i < waves.Length; i++)
            {
                Deactivate(waves[i]);

                if (active != null && i < active.Length)
                    active[i] = false;
            }
        }

        private static void Activate(EnemyWave wave)
        {
            if (wave.agents == null)
                return;

            for (var i = 0; i < wave.agents.Length; i++)
            {
                var agent = wave.agents[i];

                if (agent == null)
                    continue;

                agent.gameObject.SetActive(true);
                agent.ResetAgent();
            }
        }

        private static void Deactivate(EnemyWave wave)
        {
            if (wave == null || wave.agents == null)
                return;

            for (var i = 0; i < wave.agents.Length; i++)
            {
                var agent = wave.agents[i];

                if (agent != null)
                    agent.gameObject.SetActive(false);
            }
        }
    }
}
