using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>
    /// Mantiene el emisor de nieve centrado en el jugador sin mover las
    /// partículas ya emitidas (simulación en espacio de mundo).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class SnowFollower : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 12f, 14f);

        private void LateUpdate()
        {
            if (target == null)
                return;

            transform.position = target.position + offset;
        }
    }
}
