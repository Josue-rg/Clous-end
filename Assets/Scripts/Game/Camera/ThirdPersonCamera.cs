using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>
    /// Cámara en tercera persona detrás del personaje. La mirada se gira
    /// arrastrando con el botón derecho y la distancia se ajusta con la rueda.
    /// Expone el yaw/pitch para que el vehículo y la mira apunten con la cámara.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    public sealed class ThirdPersonCamera : MonoBehaviour
    {
        [Header("Objetivo")]
        [SerializeField] private Transform target;
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.2f, 0f);

        [Header("Órbita")]
        [SerializeField] private float distance = 9.5f;
        [SerializeField] private float minDistance = 4.5f;
        [SerializeField] private float maxDistance = 17f;
        [SerializeField] private float yawSensitivity = 0.18f;
        [SerializeField] private float pitchSensitivity = 0.14f;
        [SerializeField] private float minPitch = -6f;
        [SerializeField] private float maxPitch = 62f;
        [SerializeField] private float initialYaw = 0f;
        [SerializeField] private float initialPitch = 14f;
        [SerializeField] private float followTime = 0.08f;
        [SerializeField] private float zoomSensitivity = 0.02f;

        [Header("Colisión")]
        [SerializeField] private float collisionRadius = 0.45f;
        [SerializeField] private LayerMask collisionMask = ~0;

        private float yaw;
        private float pitch;
        private float currentDistance;
        private Vector3 smoothedPivot;
        private Vector3 pivotVelocity;
        private bool initialized;

        /// <summary>Ángulo horizontal actual de la cámara, en grados.</summary>
        public float Yaw => yaw;

        /// <summary>Ángulo vertical actual de la cámara, en grados.</summary>
        public float Pitch => pitch;

        private void Awake()
        {
            yaw = initialYaw;
            pitch = initialPitch;
            currentDistance = distance;
        }

        private void LateUpdate()
        {
            if (target == null)
                return;

            if (input != null && input.OrbitHeld)
            {
                yaw += input.OrbitDelta.x * yawSensitivity;
                pitch -= input.OrbitDelta.y * pitchSensitivity;
            }

            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            if (input != null)
                currentDistance = Mathf.Clamp(currentDistance - input.ZoomDelta * zoomSensitivity, minDistance, maxDistance);

            var pivot = target.position + targetOffset;

            if (!initialized)
            {
                smoothedPivot = pivot;
                initialized = true;
            }
            else
            {
                smoothedPivot = Vector3.SmoothDamp(smoothedPivot, pivot, ref pivotVelocity, followTime);
            }

            var rotation = Quaternion.Euler(pitch, yaw, 0f);
            var wanted = smoothedPivot - rotation * Vector3.forward * currentDistance;

            if (Physics.SphereCast(
                    smoothedPivot,
                    collisionRadius,
                    (wanted - smoothedPivot).normalized,
                    out var hit,
                    currentDistance,
                    collisionMask,
                    QueryTriggerInteraction.Ignore))
            {
                wanted = hit.point + hit.normal * collisionRadius;
            }

            transform.position = wanted;
            transform.rotation = Quaternion.LookRotation(smoothedPivot - wanted, Vector3.up);
        }
    }
}
