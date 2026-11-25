using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>
    /// Orienta al personaje y al pivote del arma hacia el punto que marca el
    /// ratón en pantalla. El cuerpo gira en horizontal y el pivote del arma en
    /// vertical, para que el disparo salga hacia la retícula.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CharacterAim : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private Camera aimCamera;
        [SerializeField] private Transform body;
        [SerializeField] private Transform aimPivot;

        [Header("Ajustes")]
        [SerializeField] private float turnSharpness = 18f;
        [SerializeField] private float maxAimDistance = 500f;
        [SerializeField] private LayerMask aimMask = ~0;

        /// <summary>Punto del mundo que está debajo de la retícula.</summary>
        public Vector3 AimPoint { get; private set; }

        private void LateUpdate()
        {
            if (input == null || aimCamera == null || body == null || aimPivot == null)
                return;

            var ray = aimCamera.ScreenPointToRay(input.PointerPosition);
            AimPoint = ray.origin + ray.direction * maxAimDistance;

            if (Physics.Raycast(ray, out var hit, maxAimDistance, aimMask, QueryTriggerInteraction.Ignore))
                AimPoint = hit.point;

            var toAim = AimPoint - body.position;
            toAim.y = 0f;

            if (toAim.sqrMagnitude > 0.05f)
            {
                body.rotation = Quaternion.Slerp(
                    body.rotation,
                    Quaternion.LookRotation(toAim, Vector3.up),
                    1f - Mathf.Exp(-turnSharpness * Time.deltaTime));
            }

            var direction = AimPoint - aimPivot.position;

            if (direction.sqrMagnitude > 0.05f)
                aimPivot.rotation = Quaternion.LookRotation(direction, Vector3.up);
        }
    }
}
