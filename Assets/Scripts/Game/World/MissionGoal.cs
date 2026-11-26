using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>
    /// Meta de la demo: al cruzar la línea del Refugio Aurora se termina la
    /// partida. No usa triggers: compara la posición del jugador, así funciona
    /// igual a pie, volando o a mucha velocidad.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MissionGoal : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private Transform player;
        [SerializeField] private GameObject finishPanel;
        [SerializeField] private VehicleMotor motor;
        [SerializeField] private PlayerInputReader input;

        [Header("Línea de meta")]
        [SerializeField] private float finishZ = 1450f;
        [SerializeField] private float halfWidth = 120f;
        [SerializeField] private float maxHeight = 400f;

        private bool completed;

        private void Update()
        {
            if (completed || player == null)
                return;

            var position = player.position;

            if (position.z < finishZ || Mathf.Abs(position.x) > halfWidth || position.y > maxHeight)
                return;

            Complete();
        }

        private void Complete()
        {
            completed = true;

            if (motor != null)
                motor.enabled = false;

            if (input != null)
                input.enabled = false;

            if (finishPanel != null)
                finishPanel.SetActive(true);

            Cursor.visible = true;
        }
    }
}
