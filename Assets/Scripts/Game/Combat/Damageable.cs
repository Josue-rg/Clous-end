using UnityEngine;

namespace ClousEnd.Game
{
    public enum DamageType
    {
        Ballistic,
        Shotgun,
        Piercing,
        Electric,
        Explosive
    }

    /// <summary>Todo lo que puede recibir daño (enemigos, jefe, regalos-bomba).</summary>
    public interface IDamageable
    {
        bool IsAlive { get; }

        void TakeDamage(float amount, Vector3 point, DamageType type);
    }

    /// <summary>Resuelve el daño de un impacto sin depender del tipo concreto.</summary>
    public static class DamageUtility
    {
        public static bool Apply(Collider collider, float amount, Vector3 point, DamageType type)
        {
            if (collider == null)
                return false;

            var target = collider.GetComponentInParent<IDamageable>();

            if (target == null || !target.IsAlive)
                return false;

            target.TakeDamage(amount, point, type);
            return true;
        }
    }
}
