using System.Collections.Generic;
using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>
    /// Las cuatro armas del juego: disparo hitscan con calor, escopeta, railgun
    /// que perfora y cadena eléctrica. No instancia nada: usa el trazador y el
    /// impacto que ya existen en la escena.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class WeaponController : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private Camera aimCamera;
        [SerializeField] private Transform muzzle;
        [SerializeField] private TracerBeam tracer;
        [SerializeField] private ImpactFlash impact;
        [SerializeField] private LayerMask hitMask = ~0;

        [Header("Armas (0 = Carbón, 1 = Nogal, 2 = Estrella, 3 = Cascabel)")]
        [SerializeField] private WeaponDefinition[] definitions;

        private int current;
        private int[] ammo;
        private bool[] unlocked;
        private float nextShotTime;
        private float heat;
        private float heatLockUntil;
        private bool overheated;
        private bool reloading;
        private float reloadEndTime;

        public WeaponDefinition CurrentDefinition =>
            definitions != null && current >= 0 && current < definitions.Length ? definitions[current] : null;

        public int CurrentAmmo => ammo != null && current < ammo.Length ? ammo[current] : 0;

        public int CurrentMagazine => CurrentDefinition != null ? CurrentDefinition.magazine : 0;

        public float Heat01 => CurrentDefinition != null && CurrentDefinition.usesHeat ? heat : 0f;

        public bool IsOverheated => overheated;

        public bool IsReloading => reloading;

        public int CurrentIndex => current;

        public int WeaponCount => definitions != null ? definitions.Length : 0;

        public const int MaxWeapons = 4;

        private void Awake()
        {
            if (hitMask.value == 0)
                hitMask = Physics.DefaultRaycastLayers;

            var count = definitions != null ? definitions.Length : 0;
            ammo = new int[Mathf.Max(MaxWeapons, count)];
            unlocked = new bool[Mathf.Max(MaxWeapons, count)];

            for (var i = 0; i < ammo.Length; i++)
                ammo[i] = definitions != null && i < definitions.Length && definitions[i] != null
                    ? definitions[i].magazine
                    : 0;

            unlocked[0] = true;

            if (unlocked.Length > 1)
                unlocked[1] = true;

            current = 0;
        }

        private void Update()
        {
            if (Time.timeScale <= 0f || input == null || aimCamera == null || definitions == null || definitions.Length == 0)
                return;

            if (!input.isActiveAndEnabled)
                return;

            var slot = input.WeaponSlot;

            if (slot > 0)
                TryEquip(slot - 1);

            if (input.ReloadPressed)
                StartReload();

            CoolHeat();
            UpdateReload();

            var definition = CurrentDefinition;

            if (definition == null)
                return;

            var wantsToFire = definition.automatic ? input.FireHeld : input.FirePressed;

            if (!wantsToFire || Time.time < nextShotTime || reloading || overheated)
                return;

            if (definition.magazine > 0 && CurrentAmmo <= 0)
            {
                StartReload();
                return;
            }

            if (definition.usesHeat && heat + definition.heatPerShot > 1f)
            {
                overheated = true;
                heatLockUntil = Time.time + definition.overheatLock;
                return;
            }

            Fire(definition);
            nextShotTime = Time.time + definition.cooldown;
        }

        private void Fire(WeaponDefinition definition)
        {
            if (definition.magazine > 0)
                ammo[current] = Mathf.Max(0, ammo[current] - 1);

            if (definition.usesHeat)
                heat = Mathf.Min(1f, heat + definition.heatPerShot);

            var pellets = Mathf.Max(1, definition.pellets);
            // El disparo sale por el rayo que pasa por el puntero, no por el
            // centro de la cámara. Así se puede apuntar a los lados sin girar
            // la cámara con el botón derecho.
            var pointer = input != null
                ? input.PointerPosition
                : new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            var aimRay = aimCamera.ScreenPointToRay(pointer);

            for (var pellet = 0; pellet < pellets; pellet++)
            {
                var direction = Spread(aimRay.direction, definition.spreadDegrees);
                var ray = new Ray(aimRay.origin, direction);
                var endPoint = ray.origin + ray.direction * definition.range;

                var hits = Physics.RaycastAll(
                    ray,
                    definition.range,
                    hitMask,
                    QueryTriggerInteraction.Collide);

                System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                var damaged = 0;

                for (var i = 0; i < hits.Length; i++)
                {
                    var hit = hits[i];

                    if (hit.collider == null)
                        continue;

                    var target = hit.collider.GetComponentInParent<IDamageable>();

                    if (target == null || !target.IsAlive)
                    {
                        endPoint = hit.point;
                        break;
                    }

                    target.TakeDamage(definition.damage, hit.point, definition.damageType);
                    endPoint = hit.point;
                    damaged++;

                    if (definition.chainTargets > 0)
                        Chain(hit.point, definition, target);

                    if (!definition.pierces || damaged >= Mathf.Max(1, definition.maxPierceTargets))
                        break;
                }

                if (tracer != null && muzzle != null)
                    tracer.Show(muzzle.position, endPoint);

                if (impact != null)
                    impact.Play(endPoint);
            }
        }

        private void Chain(Vector3 center, WeaponDefinition definition, IDamageable primary)
        {
            var overlaps = Physics.OverlapSphere(
                center,
                definition.chainRadius,
                hitMask,
                QueryTriggerInteraction.Collide);

            var damaged = new HashSet<IDamageable> { primary };
            var targets = 0;

            for (var i = 0; i < overlaps.Length && targets < definition.chainTargets; i++)
            {
                var other = overlaps[i] != null ? overlaps[i].GetComponentInParent<IDamageable>() : null;

                if (other == null || !other.IsAlive || !damaged.Add(other))
                    continue;

                other.TakeDamage(definition.damage * 0.55f, overlaps[i].ClosestPoint(center), DamageType.Electric);
                targets++;
            }
        }

        private static Vector3 Spread(Vector3 direction, float spreadDegrees)
        {
            if (spreadDegrees <= 0.01f)
                return direction;

            var radius = spreadDegrees * Mathf.Deg2Rad;
            var offset = Random.insideUnitCircle * radius;
            var right = Vector3.Cross(Vector3.up, direction).normalized;
            var up = Vector3.Cross(direction, right);
            return (direction + right * offset.x + up * offset.y).normalized;
        }

        private void CoolHeat()
        {
            var definition = CurrentDefinition;

            if (definition != null && definition.usesHeat)
                heat = Mathf.Max(0f, heat - definition.heatCooling * Time.deltaTime);

            if (overheated && Time.time >= heatLockUntil)
            {
                overheated = false;
                heat = Mathf.Min(heat, 0.45f);
            }
        }

        private void UpdateReload()
        {
            if (!reloading || Time.time < reloadEndTime)
                return;

            reloading = false;

            if (ammo != null && current < ammo.Length)
                ammo[current] = CurrentMagazine;
        }

        private void StartReload()
        {
            var definition = CurrentDefinition;

            if (definition == null || definition.magazine <= 0 || reloading)
                return;

            if (CurrentAmmo >= definition.magazine)
                return;

            reloading = true;
            reloadEndTime = Time.time + Mathf.Max(0.1f, definition.reloadTime);
        }

        public void Configure(WeaponDefinition[] weaponDefinitions)
        {
            definitions = weaponDefinitions;
        }

        public void TryEquip(int index)
        {
            if (index < 0 || index >= definitions.Length || index >= unlocked.Length)
                return;

            if (!unlocked[index] || index == current)
                return;

            reloading = false;
            overheated = false;
            current = index;
            nextShotTime = Time.time + 0.12f;
        }

        public bool IsUnlocked(int index)
        {
            return unlocked != null && index >= 0 && index < unlocked.Length && unlocked[index];
        }

        public WeaponDefinition GetDefinition(int index)
        {
            return definitions != null && index >= 0 && index < definitions.Length ? definitions[index] : null;
        }

        public string GetWeaponName(int index)
        {
            var definition = GetDefinition(index);
            return definition != null ? definition.displayName : "—";
        }

        public void UnlockAndEquip(int index)
        {
            if (index < 0 || index >= unlocked.Length || index >= definitions.Length)
                return;

            unlocked[index] = true;

            if (ammo != null && index < ammo.Length)
                ammo[index] = Mathf.Max(1, definitions[index].magazine);

            TryEquip(index);
        }

        public void RefillAmmo()
        {
            if (definitions == null)
                return;

            for (var i = 0; i < definitions.Length; i++)
            {
                if (definitions[i] != null && definitions[i].magazine > 0 && ammo != null)
                    ammo[i] = definitions[i].magazine;
            }

            reloading = false;
        }
    }
}
