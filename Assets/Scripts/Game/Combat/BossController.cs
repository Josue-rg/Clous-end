using ClousEnd.Rendering;
using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>
    /// Nicolás Cero: tres fases (procesión con renos de obsidiana, reparto de
    /// regalos-bomba y cuerpo a cuerpo). El jefe es invulnerable hasta que
    /// caen sus dos renos. Al morir abre la barrera y termina la misión.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class BossController : MonoBehaviour, IDamageable
    {
        [Header("Referencias")]
        [SerializeField] private Transform target;
        [SerializeField] private SleighHealth hull;
        [SerializeField] private MissionManager mission;
        [SerializeField] private GameHud hud;
        [SerializeField] private GameObject model;
        [SerializeField] private VATPlayer playback;
        [SerializeField] private EnemyAgent[] obsidians = new EnemyAgent[0];
        [SerializeField] private GiftBomb[] bombs = new GiftBomb[0];
        [SerializeField] private BossGate gate;
        [SerializeField] private LineRenderer beam;

        [Header("Ajustes")]
        [SerializeField] private string bossName = "NICOLÁS CERO";
        [SerializeField] private float triggerZ = 1340f;
        [SerializeField] private float maxHealth = 850f;
        [SerializeField] private float moveAmplitude = 15f;
        [SerializeField] private float moveSpeed = 0.35f;
        [SerializeField] private float bombInterval = 2.6f;
        [SerializeField] private float phase3BombInterval = 1.6f;
        [SerializeField] private float volleyInterval = 4.5f;
        [SerializeField] private float phase3VolleyInterval = 3.2f;
        [SerializeField] private float volleyDamage = 12f;
        [SerializeField] private float volleyRange = 90f;
        [SerializeField] private float volleyTelegraph = 0.8f;
        [SerializeField] private float defeatDelay = 2.5f;

        private Vector3 center;
        private Quaternion baseRotation;
        private float health;
        private int phase;
        private bool engaged;
        private bool defeated;
        private bool shielded;
        private bool volleyPending;
        private float volleyEnd;
        private float nextBomb;
        private float nextVolley;
        private float defeatAt;
        private float attackAnimEnd;

        public bool IsEngaged => engaged;

        public bool IsDefeated => defeated;

        public float Health01 => maxHealth > 0f ? Mathf.Clamp01(health / maxHealth) : 0f;

        public string BossName => bossName;

        public bool IsAlive => engaged && !defeated;

        private void Awake()
        {
            center = transform.position;
            baseRotation = transform.rotation;

            if (model != null)
                model.SetActive(false);

            if (gate != null)
                gate.Close();

            if (beam != null)
                beam.enabled = false;
        }

        private void Update()
        {
            if (Time.timeScale <= 0f || target == null)
                return;

            if (!engaged)
            {
                if (target.position.z >= triggerZ)
                    Engage();

                return;
            }

            if (defeated)
            {
                if (model != null && model.activeSelf && Time.time >= defeatAt)
                {
                    model.SetActive(false);

                    if (mission != null)
                        mission.Finish();
                }

                return;
            }

            Move();
            UpdatePhase();
            UpdateVolley();

            if (attackAnimEnd > 0f && Time.time >= attackAnimEnd)
            {
                attackAnimEnd = 0f;

                if (playback != null)
                    playback.PlayDefault();
            }

            if (phase < 2)
                return;

            if (Time.time >= nextBomb)
            {
                LaunchBomb();
                nextBomb = Time.time + (phase >= 3 ? phase3BombInterval : bombInterval);
            }

            if (Time.time >= nextVolley)
            {
                StartVolley();
                nextVolley = Time.time + (phase >= 3 ? phase3VolleyInterval : volleyInterval);
            }
        }

        private void Engage()
        {
            engaged = true;
            defeated = false;
            shielded = true;
            phase = 1;
            health = maxHealth;
            volleyPending = false;

            if (model != null)
                model.SetActive(true);

            if (gate != null)
                gate.Close();

            if (playback != null)
                playback.PlayDefault();

            if (obsidians != null)
            {
                for (var i = 0; i < obsidians.Length; i++)
                {
                    if (obsidians[i] == null)
                        continue;

                    obsidians[i].gameObject.SetActive(true);
                    obsidians[i].ResetAgent();
                }
            }

            nextBomb = Time.time + bombInterval;
            nextVolley = Time.time + volleyInterval;

            if (hud != null)
                hud.ShowToast(bossName + " · PROCESIÓN", 3f);
        }

        private void Move()
        {
            var x = Mathf.Sin(Time.time * moveSpeed) * moveAmplitude;
            var y = Mathf.Sin(Time.time * moveSpeed * 1.7f) * 1.5f;
            transform.position = new Vector3(center.x + x, center.y + y, center.z);

            var direction = target.position - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
            {
                var facing = Quaternion.LookRotation(direction.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    facing * baseRotation,
                    1f - Mathf.Exp(-3f * Time.deltaTime));
            }
        }

        private void UpdatePhase()
        {
            if (phase == 1)
            {
                var aliveObsidians = 0;

                if (obsidians != null)
                {
                    for (var i = 0; i < obsidians.Length; i++)
                    {
                        if (obsidians[i] != null && obsidians[i].IsAlive)
                            aliveObsidians++;
                    }
                }

                if (aliveObsidians > 0)
                    return;

                phase = 2;
                shielded = false;

                if (hud != null)
                    hud.ShowToast("EL ESCUDO CAYÓ · DISPARA AL PECHO", 3f);

                return;
            }

            if (phase == 2 && health <= maxHealth * 0.4f)
            {
                phase = 3;

                if (hud != null)
                    hud.ShowToast("NICOLÁS CERO · CUERPO A CUERPO", 3f);
            }
        }

        private void StartVolley()
        {
            volleyPending = true;
            volleyEnd = Time.time + Mathf.Max(0.2f, volleyTelegraph);

            if (beam != null)
                beam.enabled = true;

            if (playback != null)
            {
                playback.Play("Attack");
                attackAnimEnd = Time.time + 0.9f;
            }
        }

        private void UpdateVolley()
        {
            if (!volleyPending)
                return;

            if (beam != null)
            {
                beam.SetPosition(0, transform.position + Vector3.up * 2f);
                beam.SetPosition(1, target.position + Vector3.up * 1.2f);
            }

            if (Time.time < volleyEnd)
                return;

            volleyPending = false;

            if (beam != null)
                beam.enabled = false;

            if (Vector3.Distance(transform.position, target.position) <= volleyRange && hull != null)
                hull.TakeDamage(volleyDamage, DamageType.Ballistic);
        }

        private void LaunchBomb()
        {
            if (bombs == null)
                return;

            for (var i = 0; i < bombs.Length; i++)
            {
                var bomb = bombs[i];

                if (bomb == null || bomb.gameObject.activeSelf)
                    continue;

                bomb.gameObject.SetActive(true);
                bomb.Launch(transform.position + Vector3.up * 2f);

                if (playback != null)
                {
                    playback.Play("Attack");
                    attackAnimEnd = Time.time + 0.9f;
                }

                return;
            }
        }

        public void TakeDamage(float amount, Vector3 point, DamageType type)
        {
            if (!engaged || defeated || shielded || amount <= 0f)
                return;

            health -= amount;

            if (health <= 0f)
                Defeat();
        }

        private void Defeat()
        {
            if (defeated)
                return;

            defeated = true;
            shielded = false;
            health = 0f;
            defeatAt = Time.time + Mathf.Max(0.5f, defeatDelay);

            if (beam != null)
                beam.enabled = false;

            if (gate != null)
                gate.Open();

            if (obsidians != null)
            {
                for (var i = 0; i < obsidians.Length; i++)
                {
                    if (obsidians[i] != null)
                        obsidians[i].gameObject.SetActive(false);
                }
            }

            if (bombs != null)
            {
                for (var i = 0; i < bombs.Length; i++)
                {
                    if (bombs[i] != null)
                        bombs[i].Deactivate();
                }
            }

            if (playback != null)
                playback.PlayDefault();

            if (hud != null)
                hud.ShowToast("NICOLÁS CERO HA CAÍDO", 3f);
        }

        /// <summary>Devuelve al jefe a su estado inicial tras un game over.</summary>
        public void ResetBoss()
        {
            engaged = false;
            defeated = false;
            shielded = true;
            phase = 1;
            health = maxHealth;
            volleyPending = false;

            if (model != null)
                model.SetActive(false);

            if (gate != null)
                gate.Close();

            if (beam != null)
                beam.enabled = false;

            if (obsidians != null)
            {
                for (var i = 0; i < obsidians.Length; i++)
                {
                    if (obsidians[i] != null)
                        obsidians[i].gameObject.SetActive(false);
                }
            }

            if (bombs != null)
            {
                for (var i = 0; i < bombs.Length; i++)
                {
                    if (bombs[i] != null)
                        bombs[i].Deactivate();
                }
            }
        }
    }
}
