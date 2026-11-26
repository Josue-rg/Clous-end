using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>
    /// Regalo-bomba del jefe. Se lanza hacia el trineo y explota al acercarse;
    /// el jugador puede dispararlo para desactivarlo. Vive en un pool.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GiftBomb : MonoBehaviour, IDamageable
    {
        [SerializeField] private Transform target;
        [SerializeField] private SleighHealth hull;
        [SerializeField] private float damage = 14f;
        [SerializeField] private float explosionRadius = 3.5f;
        [SerializeField] private float speed = 26f;
        [SerializeField] private float gravity = -10f;
        [SerializeField] private float homing = 2.4f;
        [SerializeField] private float lifetime = 7f;

        private bool active;
        private float lifeEnd;
        private Vector3 velocity;

        public bool IsAlive => active;

        public void Launch(Vector3 origin)
        {
            if (target == null)
                return;

            transform.position = origin;
            active = true;
            lifeEnd = Time.time + lifetime;

            var aim = target.position + Vector3.up * 1.2f;
            velocity = (aim - origin).normalized * speed + Vector3.up * 2.2f;
        }

        private void Update()
        {
            if (Time.timeScale <= 0f || !active || target == null)
                return;

            if (Time.time >= lifeEnd || transform.position.y < -10f)
            {
                Deactivate();
                return;
            }

            velocity.y += gravity * Time.deltaTime;

            var desired = (target.position + Vector3.up * 1.2f - transform.position).normalized * speed;
            velocity = Vector3.Lerp(velocity, desired, Mathf.Clamp01(homing * Time.deltaTime));

            transform.position += velocity * Time.deltaTime;

            if (velocity.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);

            if ((transform.position - target.position).sqrMagnitude <= explosionRadius * explosionRadius)
                Explode(true);
        }

        public void TakeDamage(float amount, Vector3 point, DamageType type)
        {
            if (active)
                Explode(false);
        }

        private void Explode(bool hitHull)
        {
            if (hitHull && hull != null)
                hull.TakeDamage(damage, DamageType.Explosive);

            Deactivate();
        }

        public void Deactivate()
        {
            active = false;
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            active = false;
        }
    }
}
