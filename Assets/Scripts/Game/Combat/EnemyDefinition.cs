using UnityEngine;

namespace ClousEnd.Game
{
    public enum EnemyKind
    {
        Sabueso,
        Garfio,
        Espora,
        Manto,
        Torre,
        Ship,
        Obsidian
    }

    /// <summary>Datos de un enemigo. Un asset por tipo en Assets/Config/Enemies.</summary>
    [CreateAssetMenu(menuName = "Clous-End/Enemy", fileName = "E_Enemy")]
    public sealed class EnemyDefinition : ScriptableObject
    {
        [Header("Identidad")]
        public EnemyKind kind = EnemyKind.Sabueso;
        public string displayName = "ENEMIGO";

        [Header("Combate")]
        public float maxHealth = 30f;
        public float damage = 10f;
        public float attackRange = 12f;
        public float attackCooldown = 3f;
        public float telegraphTime = 0.6f;

        [Header("Movimiento")]
        public float moveSpeed = 20f;
        public float chargeSpeed = 38f;
        public float chargeDuration = 0.8f;
        public float hitRadius = 5f;

        [Header("Presentación")]
        [Tooltip("Escala visual del modelo. Afecta también a su collider.")]
        public float modelScale = 1f;

        [Header("Garfio")]
        public float grabDuration = 5f;

        [Header("Lectura")]
        public Color markerColor = new Color(1f, 0.82f, 0.2f);
    }
}
