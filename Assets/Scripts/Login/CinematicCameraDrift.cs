using UnityEngine;

namespace ClousEnd.Login
{
    /// <summary>Deriva muy lenta de cámara para que el diorama respire.</summary>
    public sealed class CinematicCameraDrift : MonoBehaviour
    {
        [Header("Posición")]
        [SerializeField] private float positionAmplitude = 0.22f;
        [SerializeField] private float positionFrequency = 0.055f;

        [Header("Rotación")]
        [SerializeField] private float rotationAmplitude = 0.35f;
        [SerializeField] private float rotationFrequency = 0.04f;

        [SerializeField] private Vector2 noiseSeeds = new Vector2(3.17f, 9.41f);

        private Vector3 basePosition;
        private Quaternion baseRotation;
        private float time;

        private void Awake()
        {
            basePosition = transform.localPosition;
            baseRotation = transform.localRotation;
        }

        private void LateUpdate()
        {
            time += Time.unscaledDeltaTime;

            var px = (Mathf.PerlinNoise(noiseSeeds.x, time * positionFrequency) - 0.5f) * 2f;
            var py = (Mathf.PerlinNoise(noiseSeeds.y, time * positionFrequency) - 0.5f) * 2f;

            var rx = (Mathf.PerlinNoise(noiseSeeds.y + 4.2f, time * rotationFrequency) - 0.5f) * 2f;
            var ry = (Mathf.PerlinNoise(noiseSeeds.x + 7.8f, time * rotationFrequency) - 0.5f) * 2f;

            transform.localPosition = basePosition + new Vector3(px, py, 0f) * positionAmplitude;
            transform.localRotation = baseRotation * Quaternion.Euler(
                rx * rotationAmplitude,
                ry * rotationAmplitude,
                0f);
        }
    }
}
