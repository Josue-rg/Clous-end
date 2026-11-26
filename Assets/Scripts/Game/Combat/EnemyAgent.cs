using ClousEnd.Rendering;
using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>
    /// Comportamiento de los cinco enemigos y de las naves: se acercan al
    /// jugador, anuncian el ataque y atacan según su tipo. El Garfio agarra a un
    /// reno y el resto embiste, explota o dispara. No crea objetos: vive en una
    /// instancia precolocada que el director activa y desactiva.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class EnemyAgent : MonoBehaviour, IDamageable
    {
        private enum State
        {
            Dormant,
            Approach,
            Telegraph,
            Charge,
            Grab,
            Dead
        }

        [Header("Datos")]
        [SerializeField] private EnemyDefinition definition;

        [Header("Referencias")]
        [SerializeField] private Transform target;
        [SerializeField] private SleighHealth hull;
        [SerializeField] private ReindeerSquad squad;
        [SerializeField] private VATPlayer playback;
        [SerializeField] private LineRenderer beam;

        [Header("Formación")]
        [Tooltip("Desplazamiento respecto al jugador al que vuela este enemigo.")]
        [SerializeField] private Vector3 anchorOffset = new Vector3(0f, 5f, 25f);
        [SerializeField] private float deathDelay = 1.1f;
        [SerializeField] private float rotationSharpness = 7f;

        private State state;
        private float health;
        private float stateEnd;
        private float nextAttackTime;
        private float deathEnd;
        private Vector3 home;
        private Vector3 chargeDirection;
        private Quaternion baseRotation;
        private bool initialized;

        public bool IsAlive => state != State.Dead && state != State.Dormant && health > 0f;

        public EnemyDefinition Definition => definition;

        public float Health01 => definition != null && definition.maxHealth > 0f
            ? Mathf.Clamp01(health / definition.maxHealth)
            : 0f;

        private void Awake()
        {
            baseRotation = transform.rotation;
            home = transform.position;

            if (definition != null)
                health = definition.maxHealth;

            state = State.Dormant;
        }

        public void ResetAgent()
        {
            if (Time.timeScale <= 0f || definition == null || target == null)
            {
                gameObject.SetActive(false);
                return;
            }

            if (!initialized)
            {
                baseRotation = transform.rotation;
                home = transform.position;
                initialized = true;
            }

            health = definition.maxHealth;
            state = State.Approach;
            stateEnd = 0f;
            nextAttackTime = Time.time + 0.6f;
            deathEnd = 0f;
            transform.position = home;
            transform.rotation = baseRotation;

            SetCollidersEnabled(true);

            if (beam != null)
                beam.enabled = false;

            if (playback != null)
                playback.PlayDefault();
        }

        private void Update()
        {
            if (Time.timeScale <= 0f || definition == null || target == null)
                return;

            if (state == State.Dead)
            {
                if (Time.time >= deathEnd && gameObject.activeSelf)
                    gameObject.SetActive(false);

                return;
            }

            if (state == State.Dormant)
                return;

            FaceTarget();

            switch (state)
            {
                case State.Approach:
                    UpdateApproach();
                    break;

                case State.Telegraph:
                    UpdateTelegraph();
                    break;

                case State.Charge:
                    UpdateCharge();
                    break;

                case State.Grab:
                    UpdateGrab();
                    break;
            }
        }

        private void UpdateApproach()
        {
            var desired = target.position + anchorOffset;
            var distanceToTarget = Vector3.Distance(transform.position, target.position);

            if (distanceToTarget <= definition.attackRange && Time.time >= nextAttackTime)
            {
                BeginTelegraph();
                return;
            }

            transform.position = Vector3.MoveTowards(
                transform.position,
                desired,
                definition.moveSpeed * Time.deltaTime);
        }

        private void BeginTelegraph()
        {
            state = State.Telegraph;
            stateEnd = Time.time + Mathf.Max(0.1f, definition.telegraphTime);

            if (beam != null && UsesBeam)
            {
                beam.enabled = true;
                UpdateBeam();
            }
        }

        private bool UsesBeam =>
            definition.kind == EnemyKind.Manto ||
            definition.kind == EnemyKind.Ship ||
            definition.kind == EnemyKind.Garfio;

        private void UpdateTelegraph()
        {
            if (beam != null && UsesBeam)
                UpdateBeam();

            if (Time.time < stateEnd)
                return;

            if (beam != null)
                beam.enabled = false;

            switch (definition.kind)
            {
                case EnemyKind.Manto:
                case EnemyKind.Ship:
                    FireAtTarget();
                    break;

                case EnemyKind.Garfio:
                    BeginGrab();
                    break;

                default:
                    BeginCharge();
                    break;
            }
        }

        private void FireAtTarget()
        {
            if (Vector3.Distance(transform.position, target.position) <= definition.attackRange * 1.3f && hull != null)
                hull.TakeDamage(definition.damage, DamageType.Ballistic);

            state = State.Approach;
            nextAttackTime = Time.time + definition.attackCooldown;
        }

        private void BeginGrab()
        {
            if (squad == null || !squad.TryGrab(out _))
            {
                state = State.Approach;
                nextAttackTime = Time.time + definition.attackCooldown;
                return;
            }

            state = State.Grab;
            nextAttackTime = Time.time + definition.attackCooldown;

            if (beam != null)
                beam.enabled = true;
        }

        private void UpdateGrab()
        {
            if (squad == null)
                return;

            if (beam != null)
            {
                beam.SetPosition(0, transform.position + Vector3.up * 1.2f);
                beam.SetPosition(1, squad.GetGrabbedPosition());
            }

            if (squad.GrabbedIndex < 0)
                Despawn();
        }

        private void BeginCharge()
        {
            chargeDirection = (target.position - transform.position).normalized;
            state = State.Charge;
            stateEnd = Time.time + Mathf.Max(0.2f, definition.chargeDuration);
            nextAttackTime = Time.time + definition.attackCooldown;
        }

        private void UpdateCharge()
        {
            transform.position += chargeDirection * (definition.chargeSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.position) <= definition.hitRadius)
            {
                if (hull != null)
                    hull.TakeDamage(definition.damage, DamageType.Explosive);

                if (definition.kind == EnemyKind.Sabueso ||
                    definition.kind == EnemyKind.Espora ||
                    definition.kind == EnemyKind.Obsidian)
                {
                    Die();
                    return;
                }

                state = State.Approach;
                return;
            }

            if (Time.time >= stateEnd)
                state = State.Approach;
        }

        private void UpdateBeam()
        {
            if (beam == null)
                return;

            beam.SetPosition(0, transform.position + Vector3.up * 1.2f);
            beam.SetPosition(1, target.position + Vector3.up * 1.2f);
        }

        private void FaceTarget()
        {
            var direction = target.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.01f)
                return;

            var facing = Quaternion.LookRotation(direction.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                facing * baseRotation,
                1f - Mathf.Exp(-rotationSharpness * Time.deltaTime));
        }

        public void TakeDamage(float amount, Vector3 point, DamageType type)
        {
            if (state == State.Dead || state == State.Dormant || amount <= 0f)
                return;

            health -= amount;

            if (health <= 0f)
                Die();
        }

        private void Die()
        {
            if (state == State.Dead)
                return;

            if (state == State.Grab && squad != null)
                squad.RescueGrabbed();

            state = State.Dead;
            SetCollidersEnabled(false);

            if (beam != null)
                beam.enabled = false;

            var played = playback != null && playback.Play("Death");
            deathEnd = Time.time + (played ? Mathf.Max(0.4f, deathDelay) : 0.15f);
        }

        private void Despawn()
        {
            state = State.Dormant;

            if (beam != null)
                beam.enabled = false;

            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            if (state == State.Grab && squad != null)
                squad.RescueGrabbed();
        }

        private void SetCollidersEnabled(bool enabled)
        {
            var colliders = GetComponentsInChildren<Collider>(true);

            for (var i = 0; i < colliders.Length; i++)
                colliders[i].enabled = enabled;
        }
    }
}
