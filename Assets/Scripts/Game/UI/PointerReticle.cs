using UnityEngine;
using UnityEngine.UI;

namespace ClousEnd.Game
{
    /// <summary>
    /// Mueve la retícula de la UI a la posición del ratón. Como el cursor del
    /// sistema se oculta, la retícula es la que marca dónde se dispara.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PointerReticle : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private RectTransform canvasRect;
        [SerializeField] private RectTransform reticle;
        [SerializeField] private bool hideSystemCursor = true;

        private void Awake()
        {
            if (hideSystemCursor)
                Cursor.visible = false;
        }

        private void OnDisable()
        {
            Cursor.visible = true;
        }

        private void LateUpdate()
        {
            if (input == null || canvasRect == null || reticle == null)
                return;

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRect,
                    input.PointerPosition,
                    null,
                    out var localPoint))
            {
                reticle.anchoredPosition = localPoint;
            }
        }
    }
}
