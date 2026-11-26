using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>
    /// Datos de un arma. Todo el tuning vive aquí para poder ajustarlo en el
    /// Inspector sin tocar código. Un asset por arma en Assets/Config/Weapons.
    /// </summary>
    [CreateAssetMenu(menuName = "Clous-End/Weapon", fileName = "W_Weapon")]
    public sealed class WeaponDefinition : ScriptableObject
    {
        [Header("Identidad")]
        public string id = "weapon";
        public string displayName = "ARMA";
        public DamageType damageType = DamageType.Ballistic;

        [Header("Disparo")]
        public float damage = 6f;
        public float cooldown = 0.125f;
        public float range = 350f;
        public bool automatic = true;
        public int pellets = 1;
        public float spreadDegrees;

        [Header("Munición")]
        [Tooltip("0 = munición infinita (solo calor).")]
        public int magazine;
        public float reloadTime = 1.2f;

        [Header("Calor (solo armas infinitas)")]
        public bool usesHeat;
        public float heatPerShot = 0.09f;
        public float heatCooling = 0.5f;
        public float overheatLock = 1.2f;

        [Header("Especiales")]
        public bool pierces;
        public int maxPierceTargets = 4;
        public int chainTargets;
        public float chainRadius = 18f;

        [Header("Color del trazador")]
        public Color tracerColor = Color.white;
    }
}
