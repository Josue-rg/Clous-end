using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>
    /// Movimiento del vehículo de pruebas. Avanza con W/S (o flechas) y se
    /// desplaza lateralmente con A/D (o flechas), igual que el trineo del juego.
    /// En modo vuelo asciende con Espacio y desciende con Ctrl izquierdo / Q;
    /// el resto del tiempo se mantiene suspendido.
    /// Todos los valores se ajustan en el Inspector.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterController))]
    public sealed class VehicleMotor : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private ThirdPersonCamera view;

        [Header("Velocidad")]
        [SerializeField] private float forwardSpeed = 45f;
        [SerializeField] private float reverseSpeed = 20f;
        [SerializeField] private float strafeSpeed = 28f;
        [SerializeField] private float acceleration = 95f;
        [SerializeField] private float deceleration = 105f;

        [Header("Vuelo")]
        [SerializeField] private bool flying = true;
        [SerializeField] private float verticalSpeed = 18f;
        [SerializeField] private float verticalAcceleration = 55f;

        [Header("Gravedad")]
        [SerializeField] private float gravity = -30f;
        [SerializeField] private float groundedStick = -2f;

        [Header("Orientación")]
        [SerializeField] private float turnSharpness = 14f;

        private CharacterController controller;
        private Vector3 planarVelocity;
        private float verticalVelocity;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        /// <summary>Mueve el vehículo al punto indicado, resetea la vista y anula su inercia.</summary>
        public void Teleport(Vector3 position)
        {
            if (controller == null)
                controller = GetComponent<CharacterController>();

            if (controller != null)
                controller.enabled = false;

            transform.position = position;
            transform.rotation = Quaternion.identity;

            if (controller != null)
                controller.enabled = true;

            planarVelocity = Vector3.zero;
            verticalVelocity = 0f;

            if (view != null)
                view.ResetView();
        }

        /// <summary>Corta el movimiento sin mover el vehículo.</summary>
        public void ResetMotion()
        {
            planarVelocity = Vector3.zero;
            verticalVelocity = 0f;
        }

        private void Update()
        {
            if (input == null)
                return;

            var yaw = view != null ? view.Yaw : transform.eulerAngles.y;
            var basis = Quaternion.Euler(0f, yaw, 0f);
            var forward = basis * Vector3.forward;
            var right = basis * Vector3.right;

            var move = input.Move;
            var wanted = forward * (move.y >= 0f ? move.y * forwardSpeed : move.y * reverseSpeed)
                         + right * (move.x * strafeSpeed);

            if (wanted.sqrMagnitude > forwardSpeed * forwardSpeed)
                wanted = wanted.normalized * forwardSpeed;

            var rate = wanted.sqrMagnitude > planarVelocity.sqrMagnitude ? acceleration : deceleration;
            planarVelocity = Vector3.MoveTowards(planarVelocity, wanted, rate * Time.deltaTime);

            if (flying)
            {
                var wantedVertical = 0f;

                if (input.AscendHeld)
                    wantedVertical += verticalSpeed;

                if (input.DescendHeld)
                    wantedVertical -= verticalSpeed;

                verticalVelocity = Mathf.MoveTowards(
                    verticalVelocity,
                    wantedVertical,
                    verticalAcceleration * Time.deltaTime);
            }
            else
            {
                if (controller.isGrounded && verticalVelocity < 0f)
                    verticalVelocity = groundedStick;

                verticalVelocity += gravity * Time.deltaTime;
            }

            controller.Move((planarVelocity + Vector3.up * verticalVelocity) * Time.deltaTime);

            var targetRotation = Quaternion.Euler(0f, yaw, 0f);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                1f - Mathf.Exp(-turnSharpness * Time.deltaTime));
        }
    }
}
