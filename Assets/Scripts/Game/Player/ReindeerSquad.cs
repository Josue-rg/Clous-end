using ClousEnd.Rendering;
using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>
    /// Los cuatro renos son las cuatro vidas. El Garfio puede agarrar a uno:
    /// el jugador tiene una ventana para matar al Garfio y rescatarlo. Si el
    /// Garfio escapa, el reno se pierde.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ReindeerSquad : MonoBehaviour
    {
        public enum ReindeerState
        {
            Alive,
            Grabbed,
            Lost
        }

        [Header("Renos")]
        [SerializeField] private string[] names = { "RODOLFO", "DONNER", "VIXEN", "COMETA" };
        [SerializeField] private GameObject[] visuals = new GameObject[4];
        [SerializeField] private VATPlayer[] playbacks = new VATPlayer[4];

        [Header("Reglas")]
        [SerializeField] private float grabDuration = 5f;
        [SerializeField] private float hurtRecoverTime = 0.7f;

        private bool[] lost;
        private float[] hurtUntil;
        private int grabbedIndex = -1;
        private float grabEndTime;
        private bool shield;

        public int GrabbedIndex => grabbedIndex;

        public bool HasShield => shield;

        public float GrabTimeRemaining =>
            grabbedIndex >= 0 ? Mathf.Max(0f, grabEndTime - Time.time) : 0f;

        public int AliveCount
        {
            get
            {
                var count = 0;

                for (var i = 0; i < 4; i++)
                {
                    if (!IsLost(i))
                        count++;
                }

                return count;
            }
        }

        public bool AllLost => AliveCount <= 0;

        public int Count => 4;

        private void Awake()
        {
            lost = new bool[4];
            hurtUntil = new float[4];
        }

        private void Update()
        {
            if (grabbedIndex >= 0 && Time.time >= grabEndTime)
                LoseGrabbed();

            if (hurtUntil == null)
                return;

            for (var i = 0; i < hurtUntil.Length; i++)
            {
                if (hurtUntil[i] > 0f && Time.time >= hurtUntil[i])
                {
                    hurtUntil[i] = 0f;

                    if (playbacks != null && i < playbacks.Length && playbacks[i] != null && !IsLost(i))
                        playbacks[i].PlayDefault();
                }
            }
        }

        public void Configure(string[] reindeerNames, GameObject[] reindeerVisuals, VATPlayer[] reindeerPlaybacks)
        {
            names = reindeerNames;
            visuals = reindeerVisuals;
            playbacks = reindeerPlaybacks;
        }

        public bool IsLost(int index)
        {
            return lost != null && index >= 0 && index < lost.Length && lost[index];
        }

        public ReindeerState GetState(int index)
        {
            if (IsLost(index))
                return ReindeerState.Lost;

            return index == grabbedIndex ? ReindeerState.Grabbed : ReindeerState.Alive;
        }

        public string GetName(int index)
        {
            return names != null && index >= 0 && index < names.Length ? names[index] : "RENO";
        }

        public Vector3 GetReindeerPosition(int index)
        {
            if (visuals == null || index < 0 || index >= visuals.Length || visuals[index] == null)
                return transform.position;

            return visuals[index].transform.position;
        }

        public Vector3 GetGrabbedPosition()
        {
            return grabbedIndex >= 0 ? GetReindeerPosition(grabbedIndex) : transform.position;
        }

        /// <summary>El Garfio elige un reno sano. Si no queda ninguno, falla.</summary>
        public bool TryGrab(out int index)
        {
            index = -1;

            if (grabbedIndex >= 0)
                return false;

            var candidates = new int[4];
            var count = 0;

            for (var i = 0; i < 4; i++)
            {
                if (!IsLost(i))
                    candidates[count++] = i;
            }

            if (count == 0)
                return false;

            index = candidates[Random.Range(0, count)];
            grabbedIndex = index;
            grabEndTime = Time.time + Mathf.Max(1f, grabDuration);

            if (visuals != null && index < visuals.Length && visuals[index] != null)
                visuals[index].SetActive(false);

            return true;
        }

        /// <summary>El jugador mata al Garfio: el reno vuelve.</summary>
        public void RescueGrabbed()
        {
            if (grabbedIndex < 0)
                return;

            var index = grabbedIndex;
            grabbedIndex = -1;

            if (visuals != null && index < visuals.Length && visuals[index] != null)
                visuals[index].SetActive(true);

            if (playbacks != null && index < playbacks.Length && playbacks[index] != null)
            {
                playbacks[index].Play("Hurt");
                hurtUntil[index] = Time.time + hurtRecoverTime;
            }
        }

        /// <summary>El Garfio escapa: el reno se pierde (o lo salva el escudo).</summary>
        public void LoseGrabbed()
        {
            if (grabbedIndex < 0)
                return;

            if (shield)
            {
                shield = false;
                RescueGrabbed();
                return;
            }

            var index = grabbedIndex;
            grabbedIndex = -1;

            if (lost != null && index < lost.Length)
                lost[index] = true;

            if (visuals != null && index < visuals.Length && visuals[index] != null)
                visuals[index].SetActive(false);
        }

        /// <summary>El pickup de escudo: revive un reno o protege al próximo.</summary>
        public void GrantShield()
        {
            for (var i = 0; i < 4; i++)
            {
                if (!IsLost(i))
                    continue;

                lost[i] = false;

                if (visuals != null && i < visuals.Length && visuals[i] != null)
                    visuals[i].SetActive(true);

                if (playbacks != null && i < playbacks.Length && playbacks[i] != null)
                    playbacks[i].PlayDefault();

                return;
            }

            shield = true;
        }

        /// <summary>Checkpoint / reintento: los cuatro renos vuelven.</summary>
        public void RestoreAll()
        {
            if (lost == null)
                lost = new bool[4];

            grabbedIndex = -1;
            shield = false;

            for (var i = 0; i < 4; i++)
            {
                lost[i] = false;

                if (visuals != null && i < visuals.Length && visuals[i] != null)
                    visuals[i].SetActive(true);

                if (playbacks != null && i < playbacks.Length && playbacks[i] != null)
                {
                    playbacks[i].PlayDefault();
                    hurtUntil[i] = 0f;
                }
            }
        }
    }
}
