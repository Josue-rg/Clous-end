using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>
    /// Recoge los pickups por cercanía (sin depender de triggers). Reparte el
    /// efecto a los sistemas del jugador y avisa al HUD.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PickupCollector : MonoBehaviour
    {
        [Header("Sistemas")]
        [SerializeField] private SleighHealth health;
        [SerializeField] private ReindeerSquad squad;
        [SerializeField] private WeaponController weapons;
        [SerializeField] private GameHud hud;

        [Header("Pickups de la ruta")]
        [SerializeField] private Pickup[] pickups = new Pickup[0];
        [SerializeField] private float pickupRadius = 4.5f;

        private void Update()
        {
            if (pickups == null)
                return;

            var position = transform.position;

            for (var i = 0; i < pickups.Length; i++)
            {
                var pickup = pickups[i];

                if (pickup == null || !pickup.gameObject.activeInHierarchy || pickup.Collected)
                    continue;

                if ((pickup.transform.position - position).sqrMagnitude > pickupRadius * pickupRadius)
                    continue;

                Apply(pickup);
            }
        }

        private void Apply(Pickup pickup)
        {
            switch (pickup.Type)
            {
                case PickupType.Ammo:
                    if (weapons != null)
                        weapons.RefillAmmo();

                    Notify("MUNICIÓN RECARGADA");
                    break;

                case PickupType.HelmetKit:
                    if (health != null)
                        health.Repair(pickup.Amount);

                    Notify("CASCO REPARADO  +" + Mathf.RoundToInt(pickup.Amount));
                    break;

                case PickupType.ReindeerShield:
                    if (squad != null)
                        squad.GrantShield();

                    Notify("ESCUDO DE RENO");
                    break;

                case PickupType.WeaponEstrella:
                    UnlockWeapon(2, "ESTRELLA");
                    break;

                case PickupType.WeaponCascabel:
                    UnlockWeapon(3, "CASCABEL");
                    break;
            }

            pickup.MarkCollected();
        }

        private void UnlockWeapon(int index, string label)
        {
            if (weapons == null)
                return;

            if (weapons.IsUnlocked(index))
            {
                weapons.RefillAmmo();
                Notify(label + " · MUNICIÓN RECARGADA");
                return;
            }

            weapons.UnlockAndEquip(index);
            Notify("ARMA OBTENIDA  ·  " + label);
        }

        private void Notify(string message)
        {
            if (hud != null)
                hud.ShowToast(message);
        }
    }
}
