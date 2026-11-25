using UnityEngine;

namespace ClousEnd.Login
{
    /// <summary>Giro constante para el indicador de carga.</summary>
    public sealed class Spinner : MonoBehaviour
    {
        [SerializeField] private float degreesPerSecond = -220f;

        private void Update()
        {
            transform.Rotate(0f, 0f, degreesPerSecond * Time.unscaledDeltaTime, Space.Self);
        }
    }
}
