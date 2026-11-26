using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>barrera de energía que cierra la arena del jefe hasta vencerlo.</summary>
    [DisallowMultipleComponent]
    public sealed class BossGate : MonoBehaviour
    {
        [SerializeField] private GameObject visuals;
        [SerializeField] private Collider blocker;

        public bool IsOpen { get; private set; }

        public void Open()
        {
            IsOpen = true;

            if (visuals != null)
                visuals.SetActive(false);

            if (blocker != null)
                blocker.enabled = false;
        }

        public void Close()
        {
            IsOpen = false;

            if (visuals != null)
                visuals.SetActive(true);

            if (blocker != null)
                blocker.enabled = true;
        }
    }
}
