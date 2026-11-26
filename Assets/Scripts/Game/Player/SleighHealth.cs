using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>Integridad del casco del trineo. El casco es la vida del jugador.</summary>
    [DisallowMultipleComponent]
    public sealed class SleighHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;

        public float Current { get; private set; }

        public float Max => maxHealth;

        public float Normalized => maxHealth > 0f ? Mathf.Clamp01(Current / maxHealth) : 0f;

        public bool IsDead { get; private set; }

        private void Awake()
        {
            Current = maxHealth;
        }

        public void TakeDamage(float amount, DamageType type)
        {
            if (IsDead || amount <= 0f)
                return;

            Current = Mathf.Max(0f, Current - amount);

            if (Current <= 0f)
                IsDead = true;
        }

        public void Repair(float amount)
        {
            if (amount <= 0f)
                return;

            Current = Mathf.Min(maxHealth, Current + amount);
            IsDead = false;
        }

        public void RepairFull()
        {
            Current = maxHealth;
            IsDead = false;
        }

        public void ResetHealth()
        {
            Current = maxHealth;
            IsDead = false;
        }
    }
}
