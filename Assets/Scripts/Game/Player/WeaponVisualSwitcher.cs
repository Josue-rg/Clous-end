using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>
    /// Muestra solo el modelo del arma equipada. Los cuatro modelos ya existen
    /// como hijos del rig creados por el builder del editor; este componente no
    /// instancia ni destruye nada, solo activa y desactiva.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WeaponVisualSwitcher : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private WeaponController controller;

        [Header("Modelos (0 = Carbón, 1 = Nogal, 2 = Estrella, 3 = Cascabel)")]
        [SerializeField] private GameObject[] visuals;

        private int lastIndex = -1;

        private void Awake()
        {
            Apply(true);
        }

        private void LateUpdate()
        {
            Apply(false);
        }

        private void Apply(bool force)
        {
            if (controller == null || visuals == null || visuals.Length == 0)
                return;

            var index = controller.CurrentIndex;

            if (!force && index == lastIndex)
                return;

            lastIndex = index;

            for (var i = 0; i < visuals.Length; i++)
            {
                if (visuals[i] != null)
                    visuals[i].SetActive(i == index);
            }
        }
    }
}
