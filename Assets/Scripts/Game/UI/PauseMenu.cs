using UnityEngine;

namespace ClousEnd.Game
{
    /// <summary>Pausa con Escape. Congela el tiempo y muestra el panel de pausa.</summary>
    [DisallowMultipleComponent]
    public sealed class PauseMenu : MonoBehaviour
    {
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private MissionManager mission;
        [SerializeField] private GameObject panel;
        [SerializeField] private GameObject reticleRoot;

        public bool IsPaused { get; private set; }

        private void Awake()
        {
            Time.timeScale = 1f;

            if (panel != null)
                panel.SetActive(false);
        }

        private void Update()
        {
            if (input == null || mission == null || panel == null)
                return;

            if (!input.isActiveAndEnabled)
                return;

            if (!input.PausePressed)
                return;

            if (IsPaused)
                Resume();
            else if (mission.IsPlaying)
                Pause();
        }

        public void Pause()
        {
            IsPaused = true;
            Time.timeScale = 0f;

            if (panel != null)
                panel.SetActive(true);

            if (reticleRoot != null)
                reticleRoot.SetActive(false);

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        public void Resume()
        {
            IsPaused = false;
            Time.timeScale = 1f;

            if (panel != null)
                panel.SetActive(false);

            if (reticleRoot != null)
                reticleRoot.SetActive(true);

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
        }

        public void RestartRun()
        {
            Time.timeScale = 1f;

            if (mission != null)
                mission.RestartRun();
        }

        public void ReturnToLobby()
        {
            Time.timeScale = 1f;

            if (mission != null)
                mission.ReturnToLobby();
        }
    }
}
