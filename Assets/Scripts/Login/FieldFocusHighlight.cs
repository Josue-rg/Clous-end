using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ClousEnd.Login
{
    /// <summary>Subraya el campo activo sin depender de la lógica del formulario.</summary>
    public sealed class FieldFocusHighlight : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private Graphic underline;
        [SerializeField] private Color idleColor = new Color(0.16f, 0.23f, 0.31f, 1f);
        [SerializeField] private Color focusColor = new Color(0.85f, 0.93f, 0.98f, 1f);
        [SerializeField] private float fadeDuration = 0.12f;

        private void OnEnable()
        {
            if (underline != null)
                underline.color = idleColor;
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (underline != null)
                underline.CrossFadeColor(focusColor, fadeDuration, true, true);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (underline != null)
                underline.CrossFadeColor(idleColor, fadeDuration, true, true);
        }
    }
}
