using UnityEngine;

namespace ClousEnd.Game
{
    public enum PickupType
    {
        Ammo,
        HelmetKit,
        ReindeerShield,
        WeaponEstrella,
        WeaponCascabel
    }

    /// <summary>Recogible de la ruta. El colector del jugador lo desactiva al pasar.</summary>
    [DisallowMultipleComponent]
    public sealed class Pickup : MonoBehaviour
    {
        [SerializeField] private PickupType type = PickupType.Ammo;
        [SerializeField] private float amount = 25f;

        public PickupType Type => type;

        public float Amount => amount;

        public bool Collected { get; private set; }

        public void Configure(PickupType pickupType, float pickupAmount)
        {
            type = pickupType;
            amount = pickupAmount;
        }

        public void MarkCollected()
        {
            Collected = true;
            gameObject.SetActive(false);
        }
    }
}
