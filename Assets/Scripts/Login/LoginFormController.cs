using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ClousEnd.Login
{
    /// <summary>
    /// Lobby simple: un solo botón JUGAR que carga la escena del juego.
    /// No pide nombre ni contraseña. No crea objetos: solo lee referencias
    /// serializadas y cambia su estado.
    /// </summary>
    public sealed class LoginFormController : MonoBehaviour
    {
        [Header("Datos")]
        [SerializeField] private LoginConfig config;

        [Header("Referencias de UI")]
        [SerializeField] private Button enterButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private TMP_Text enterLabel;
        [SerializeField] private TMP_Text statusLabel;
        [SerializeField] private GameObject loadingIndicator;
        [SerializeField] private CanvasGroup fadeOverlay;

        [Header("Textos")]
        [SerializeField] private string loadingLabel = "ENTRANDO EN DISTRITO AURORA…";
        [SerializeField] private string demoMessage = "La escena «{0}» todavía no está en Build Settings.";

        private string idleEnterLabel;
        private bool busy;

        private void Awake()
        {
            if (enterLabel != null)
                idleEnterLabel = enterLabel.text;

            SetVisible(statusLabel, false);
            SetVisible(loadingIndicator, false);
        }

        /// <summary>Llamado por el botón JUGAR.</summary>
        public void Submit()
        {
            if (busy)
                return;

            if (config == null)
            {
                Debug.LogError("[Login] Falta asignar LoginConfig.", this);
                return;
            }

            StartCoroutine(EnterRoutine());
        }

        private IEnumerator EnterRoutine()
        {
            busy = true;
            SetButtons(false);
            SetVisible(loadingIndicator, true);
            SetVisible(statusLabel, false);

            if (enterLabel != null)
                enterLabel.text = loadingLabel;

            if (config.loadingDuration > 0f)
                yield return new WaitForSecondsRealtime(config.loadingDuration);

            SetVisible(loadingIndicator, false);

            var nextScene = config.nextSceneName;
            var hasNextScene = !string.IsNullOrWhiteSpace(nextScene)
                && Application.CanStreamedLevelBeLoaded(nextScene);

            if (enterLabel != null)
                enterLabel.text = idleEnterLabel;

            if (!hasNextScene)
            {
                SetButtons(true);
                busy = false;

                if (statusLabel != null)
                    statusLabel.text = string.Format(demoMessage, nextScene ?? "Game");

                SetVisible(statusLabel, true);
                yield break;
            }

            if (fadeOverlay != null)
            {
                fadeOverlay.blocksRaycasts = true;
                yield return FadeCanvas(fadeOverlay, fadeOverlay.alpha, 1f, config.fadeDuration);
            }

            SceneManager.LoadScene(nextScene);
        }

        private IEnumerator FadeCanvas(CanvasGroup group, float from, float to, float duration)
        {
            if (duration <= 0f)
            {
                group.alpha = to;
                yield break;
            }

            var elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                group.alpha = Mathf.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
                yield return null;
            }

            group.alpha = to;
        }

        private void SetButtons(bool enabled)
        {
            if (enterButton != null) enterButton.interactable = enabled;
            if (exitButton != null) exitButton.interactable = enabled;
        }

        private static void SetVisible(Component target, bool visible)
        {
            if (target != null)
                target.gameObject.SetActive(visible);
        }

        private static void SetVisible(GameObject target, bool visible)
        {
            if (target != null)
                target.SetActive(visible);
        }
    }
}
