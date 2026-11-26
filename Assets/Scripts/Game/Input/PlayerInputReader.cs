using UnityEngine;
using UnityEngine.InputSystem;

namespace ClousEnd.Game
{
    /// <summary>
    /// Única fuente de entrada del jugador. El resto de sistemas leen estas
    /// propiedades para no duplicar lecturas del teclado ni del ratón.
    /// Usa el Input System nuevo (el proyecto tiene el Input Manager desactivado).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlayerInputReader : MonoBehaviour
    {
        [Header("Cursor")]
        [SerializeField] private bool hideSystemCursor = true;

        /// <summary>W/S o flechas arriba/abajo en Y; A/D o flechas izquierda/derecha en X.</summary>
        public Vector2 Move { get; private set; }

        /// <summary>Posición del ratón en píxeles de pantalla.</summary>
        public Vector2 PointerPosition { get; private set; }

        /// <summary>Delta del ratón mientras se mantiene el botón derecho.</summary>
        public Vector2 OrbitDelta { get; private set; }

        /// <summary>Rueda del ratón.</summary>
        public float ZoomDelta { get; private set; }

        public bool FirePressed { get; private set; }
        public bool FireHeld { get; private set; }
        public bool OrbitHeld { get; private set; }

        /// <summary>1–4 según la tecla numérica; 0 si no se pulsa ninguna.</summary>
        public int WeaponSlot { get; private set; }

        public bool ReloadPressed { get; private set; }
        public bool PausePressed { get; private set; }

        /// <summary>Espacio: subir el vehículo (modo vuelo).</summary>
        public bool AscendHeld { get; private set; }

        /// <summary>Ctrl izquierdo o Q: bajar el vehículo (modo vuelo).</summary>
        public bool DescendHeld { get; private set; }

        private void Awake()
        {
            ApplyCursorState(hideSystemCursor);
        }

        private void OnDisable()
        {
            ApplyCursorState(false);
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;

            if (keyboard == null || mouse == null)
            {
                Move = Vector2.zero;
                OrbitDelta = Vector2.zero;
                ZoomDelta = 0f;
                FirePressed = false;
                FireHeld = false;
                OrbitHeld = false;
                AscendHeld = false;
                DescendHeld = false;
                WeaponSlot = 0;
                ReloadPressed = false;
                PausePressed = false;
                return;
            }

            var move = Vector2.zero;

            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                move.y += 1f;

            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                move.y -= 1f;

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                move.x += 1f;

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                move.x -= 1f;

            Move = Vector2.ClampMagnitude(move, 1f);

            PointerPosition = mouse.position.ReadValue();
            ZoomDelta = mouse.scroll.ReadValue().y;

            OrbitHeld = mouse.rightButton.isPressed;
            OrbitDelta = OrbitHeld ? mouse.delta.ReadValue() : Vector2.zero;

            FirePressed = mouse.leftButton.wasPressedThisFrame;
            FireHeld = mouse.leftButton.isPressed;

            AscendHeld = keyboard.spaceKey.isPressed;
            DescendHeld = keyboard.leftCtrlKey.isPressed || keyboard.qKey.isPressed;

            WeaponSlot = 0;

            if (keyboard.digit1Key.wasPressedThisFrame) WeaponSlot = 1;
            else if (keyboard.digit2Key.wasPressedThisFrame) WeaponSlot = 2;
            else if (keyboard.digit3Key.wasPressedThisFrame) WeaponSlot = 3;
            else if (keyboard.digit4Key.wasPressedThisFrame) WeaponSlot = 4;

            ReloadPressed = keyboard.rKey.wasPressedThisFrame;
            PausePressed = keyboard.escapeKey.wasPressedThisFrame;
        }

        private static void ApplyCursorState(bool hidden)
        {
            Cursor.visible = !hidden;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
