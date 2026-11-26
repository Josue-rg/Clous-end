using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ClousEnd.Game
{
    /// <summary>
    /// Misión de la demo: cronómetro, checkpoint del puente, meta y game over.
    /// No crea objetos: solo lee referencias serializadas y cambia su estado.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MissionManager : MonoBehaviour
    {
        [Header("Referencias")]
        [SerializeField] private Transform player;
        [SerializeField] private VehicleMotor motor;
        [SerializeField] private PlayerInputReader input;

        [Header("Sistemas")]
        [SerializeField] private SleighHealth health;
        [SerializeField] private ReindeerSquad squad;
        [SerializeField] private BossController boss;
        [SerializeField] private EnemyDirector director;
        [SerializeField] private GameHud hud;
        [SerializeField] private PauseMenu pause;

        [Header("UI")]
        [SerializeField] private GameObject finishPanel;
        [SerializeField] private TMP_Text finishTime;
        [SerializeField] private TMP_Text finishRenos;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TMP_Text gameOverReason;
        [SerializeField] private TMP_Text timerLabel;
        [SerializeField] private CanvasGroup checkpointToast;

        [Header("Misión")]
        [SerializeField] private float finishZ = 1450f;
        [SerializeField] private float checkpointZ = 1000f;
        [SerializeField] private float halfWidth = 130f;
        [SerializeField] private float maxHeight = 400f;
        [SerializeField] private float fallLimitY = -30f;
        [SerializeField] private int reindeerAlive = 4;
        [SerializeField] private string lobbyScene = "Login";

        private Vector3 checkpointPosition;
        private float startTime;
        private float toastHoldUntil;
        private float toastFadeStart;
        private bool checkpointReached;
        private bool finished;
        private bool gameOver;

        public bool IsPlaying => !finished && !gameOver;

        private void Start()
        {
            checkpointPosition = player != null ? player.position : Vector3.zero;
            startTime = Time.time;

            if (finishPanel != null) finishPanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (checkpointToast != null) checkpointToast.alpha = 0f;

            if (hud != null)
                hud.ShowToast("VOSS: Santa, la ciudad está en llamas. Despega.", 5f);

            UpdateTimer();
        }

        private void Update()
        {
            if (finished || gameOver || player == null)
                return;

            if (health != null && health.IsDead)
            {
                TriggerGameOver("EL CASCO CEDIÓ");
                return;
            }

            if (squad != null && squad.AllLost)
            {
                TriggerGameOver("SE PERDIERON LOS RENOS");
                return;
            }

            UpdateTimer();
            UpdateToast();

            var position = player.position;

            if (!checkpointReached && position.z >= checkpointZ)
            {
                checkpointReached = true;
                checkpointPosition = new Vector3(
                    Mathf.Clamp(position.x, -8f, 8f),
                    12f,
                    checkpointZ + 14f);

                if (health != null)
                    health.RepairFull();

                ShowCheckpoint();

                if (hud != null)
                    hud.ShowToast("CHECKPOINT  ·  CASCO REPARADO", 3f);
            }

            var bossDefeated = boss == null || boss.IsDefeated;

            if (position.z >= finishZ && bossDefeated && Mathf.Abs(position.x) <= halfWidth && position.y <= maxHeight)
            {
                Finish();
                return;
            }

            if (position.y < fallLimitY)
                TriggerGameOver("EL TRINEO CAYÓ");
        }

        /// <summary>Termina la misión con el panel de resultados.</summary>
        public void Finish()
        {
            if (finished || gameOver)
                return;

            finished = true;
            StopControls();

            var elapsed = Time.time - startTime;

            if (finishTime != null)
                finishTime.text = "TIEMPO  " + FormatTime(elapsed);

            if (finishRenos != null)
            {
                var alive = squad != null ? squad.AliveCount : reindeerAlive;
                var hull = health != null ? Mathf.CeilToInt(health.Current) : 0;
                var bossText = boss == null ? "—" : boss.IsDefeated ? "SÍ" : "NO";

                finishRenos.text = "RENOS " + alive + " / 4   ·   CASCO " + hull + "%   ·   BOSS " + bossText;
            }

            if (finishPanel != null)
                finishPanel.SetActive(true);

            HideToast();
            ShowCursor();
        }

        /// <summary>Game over por caída o por perder los renos.</summary>
        public void TriggerGameOver()
        {
            TriggerGameOver("EL TRINEO CAYÓ");
        }

        /// <summary>Game over con motivo visible en el panel.</summary>
        public void TriggerGameOver(string reason)
        {
            if (gameOver || finished)
                return;

            gameOver = true;
            StopControls();

            if (gameOverReason != null)
                gameOverReason.text = reason;

            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);

            HideToast();
            ShowCursor();
        }

        /// <summary>Vuelve al último checkpoint sin recargar la escena.</summary>
        public void RetryFromCheckpoint()
        {
            if (finished || player == null)
                return;

            gameOver = false;
            Time.timeScale = 1f;

            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);

            if (health != null)
                health.RepairFull();

            if (squad != null)
                squad.RestoreAll();

            if (boss != null)
                boss.ResetBoss();

            if (director != null)
                director.ResetWaves();

            if (motor != null)
            {
                motor.enabled = true;
                motor.Teleport(checkpointPosition);
            }
            else
            {
                player.position = checkpointPosition;
            }

            if (input != null)
                input.enabled = true;

            HideToast();
            HideCursor();
            UpdateTimer();
        }

        /// <summary>Reinicia la partida desde el principio.</summary>
        public void RestartRun()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /// <summary>Vuelve al lobby.</summary>
        public void ReturnToLobby()
        {
            Time.timeScale = 1f;

            if (!string.IsNullOrWhiteSpace(lobbyScene))
                SceneManager.LoadScene(lobbyScene);
        }

        private void StopControls()
        {
            if (motor != null) motor.enabled = false;
            if (input != null) input.enabled = false;
        }

        private void UpdateTimer()
        {
            if (timerLabel != null)
                timerLabel.text = FormatTime(Time.time - startTime);
        }

        private void ShowCheckpoint()
        {
            if (checkpointToast == null)
                return;

            checkpointToast.alpha = 1f;
            toastHoldUntil = Time.time + 1.1f;
            toastFadeStart = toastHoldUntil;
        }

        private void UpdateToast()
        {
            if (checkpointToast == null || checkpointToast.alpha <= 0f)
                return;

            if (Time.time <= toastHoldUntil)
                return;

            var fade = 1f - Mathf.Clamp01((Time.time - toastFadeStart) / 0.8f);
            checkpointToast.alpha = fade;
        }

        private void HideToast()
        {
            if (checkpointToast != null)
                checkpointToast.alpha = 0f;
        }

        private static void ShowCursor()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        private static void HideCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.None;
        }

        private static string FormatTime(float seconds)
        {
            var total = Mathf.Max(0, Mathf.FloorToInt(seconds));
            return (total / 60).ToString("00") + ":" + (total % 60).ToString("00");
        }
    }
}
