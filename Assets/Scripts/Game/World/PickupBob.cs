using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>Flota y gira un recogible. No crea nada; solo mueve su transform.</summary>
    [DisallowMultipleComponent]
    public sealed class PickupBob : MonoBehaviour
    {
        [SerializeField] private float amplitude = 0.3f;
        [SerializeField] private float speed = 1.3f;
        [SerializeField] private float rotationSpeed = 55f;

        private Vector3 origin;
        private float phase;

        private void Awake()
        {
            origin = transform.localPosition;
            phase = Random.value * Mathf.PI * 2f;
        }

        private void Update()
        {
            transform.localPosition = origin + Vector3.up * (Mathf.Sin(Time.time * speed + phase) * amplitude);
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}
