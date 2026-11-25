using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>
    /// Disparo hitscan de pruebas: traza un rayo desde la cámara hasta el punto
    /// de la retícula y dibuja un trazador y un impacto que ya existen en la
    /// escena (no se instancia nada en tiempo de ejecución).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class HitscanWeapon : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private Camera aimCamera;
        [SerializeField] private Transform muzzle;
        [SerializeField] private TracerBeam tracer;
        [SerializeField] private ImpactFlash impact;

        [Header("Ajustes")]
        [SerializeField] private float range = 320f;
        [SerializeField] private float cooldown = 0.14f;
        [SerializeField] private bool automatic;
        [SerializeField] private LayerMask hitMask = ~0;

        private float nextShotTime;

        private void Update()
        {
            if (input == null || aimCamera == null)
                return;

            var wantsToFire = automatic ? input.FireHeld : input.FirePressed;

            if (!wantsToFire || Time.time < nextShotTime)
                return;

            nextShotTime = Time.time + cooldown;
            Fire();
        }

        private void Fire()
        {
            var ray = aimCamera.ScreenPointToRay(input.PointerPosition);
            var endPoint = ray.origin + ray.direction * range;

            if (Physics.Raycast(ray, out var hit, range, hitMask, QueryTriggerInteraction.Ignore))
                endPoint = hit.point;

            if (tracer != null && muzzle != null)
                tracer.Show(muzzle.position, endPoint);

            if (impact != null)
                impact.Play(endPoint);
        }
    }
}
