using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ClousEnd.Login
{
    /// <summary>
    /// Une la vista de login con el perfil local y la transición de escena.
    /// No crea objetos: solo lee referencias serializadas y cambia su estado.
    /// </summary>
    public sealed class LoginFormController : MonoBehaviour
    {
        [Header("Datos")]
        [SerializeField] private LoginConfig config;

        [Header("Referencias de UI")]
        [SerializeField] private TMP_InputField nameField;
        [SerializeField] private TMP_InputField passwordField;
        [SerializeField] private Button enterButton;
        [SerializeField] private Button exitButton;
        [SerializeField] private TMP_Text enterLabel;
        [SerializeField] private TMP_Text errorLabel;
        [SerializeField] private TMP_Text statusLabel;
        [SerializeField] private GameObject loadingIndicator;
        [SerializeField] private CanvasGroup fadeOverlay;
        [SerializeField] private RectShaker errorShake;

        [Header("Textos dinámicos")]
        [SerializeField] private string loadingLabel = "ABRIENDO EL REFUGIO…";
        [SerializeField] private string nameTooShort = "El nombre necesita al menos {0} caracteres.";
        [SerializeField] private string passwordTooShort = "La contraseña necesita al menos {0} caracteres.";
        [SerializeField] private string profileNotFound = "No encontramos ese perfil. Revisa el nombre o la contraseña.";
        [SerializeField] private string welcomeFormat = "ACCESO CONCEDIDO · BIENVENIDO, {0}";
        [SerializeField] private string demoMessage = "La escena «{0}» todavía no está en Build Settings.";
        [SerializeField] private float focusDelay = 0.9f;

        private string idleEnterLabel;
        private bool busy;

        private void Awake()
        {
            if (enterLabel != null)
                idleEnterLabel = enterLabel.text;

            SetVisible(errorLabel, false);
            SetVisible(statusLabel, false);
            SetVisible(loadingIndicator, false);
        }

        private void OnEnable()
        {
            if (nameField != null)
                nameField.onSubmit.AddListener(OnNameSubmitted);

            if (passwordField != null)
                passwordField.onSubmit.AddListener(OnPasswordSubmitted);
        }

        private void OnDisable()
        {
            if (nameField != null)
                nameField.onSubmit.RemoveListener(OnNameSubmitted);

            if (passwordField != null)
                passwordField.onSubmit.RemoveListener(OnPasswordSubmitted);
        }

        private void Start()
        {
            StartCoroutine(FocusNameFieldAfterIntro());
        }

        private IEnumerator FocusNameFieldAfterIntro()
        {
            if (focusDelay > 0f)
                yield return new WaitForSecondsRealtime(focusDelay);

            if (nameField != null && !busy)
            {
                nameField.Select();
                nameField.ActivateInputField();
            }
        }

        private void OnNameSubmitted(string _)
        {
            if (busy) return;

            if (passwordField != null)
            {
                passwordField.Select();
                passwordField.ActivateInputField();
            }
        }

        private void OnPasswordSubmitted(string _)
        {
            Submit();
        }

        /// <summary>Llamado por el botón ENTRAR AL TRINEO.</summary>
        public void Submit()
        {
            if (busy) return;

            if (config == null)
            {
                Debug.LogError("[Login] Falta asignar LoginConfig.", this);
                return;
            }

            SetVisible(errorLabel, false);
            SetVisible(statusLabel, false);

            var displayName = nameField != null ? nameField.text.Trim() : string.Empty;
            var password = passwordField != null ? passwordField.text : string.Empty;

            if (displayName.Length < config.minNameLength)
            {
                Fail(string.Format(nameTooShort, config.minNameLength), nameField);
                return;
            }

            if (password.Length < config.minPasswordLength)
            {
                Fail(string.Format(passwordTooShort, config.minPasswordLength), passwordField);
                return;
            }

            if (LocalProfileStore.Exists(displayName))
            {
                if (!LocalProfileStore.Validate(displayName, password))
                {
                    Fail(profileNotFound, passwordField);
                    return;
                }
            }
            else if (config.createProfileIfMissing)
            {
                LocalProfileStore.Save(displayName, password);
            }
            else
            {
                Fail(profileNotFound, nameField);
                return;
            }

            var savedName = LocalProfileStore.GetDisplayName(displayName);
            StartCoroutine(EnterRoutine(savedName));
        }

        private void Fail(string message, TMP_InputField fieldToFocus)
        {
            if (errorLabel != null)
                errorLabel.text = message;

            SetVisible(errorLabel, true);
            SetVisible(statusLabel, false);

            if (errorShake != null)
                errorShake.Play();

            if (fieldToFocus != null)
            {
                fieldToFocus.Select();
                fieldToFocus.ActivateInputField();
            }
        }

        private IEnumerator EnterRoutine(string displayName)
        {
            busy = true;
            SetFormEnabled(false);
            SetVisible(loadingIndicator, true);
            SetVisible(statusLabel, false);
            SetVisible(errorLabel, false);

            if (enterLabel != null)
                enterLabel.text = loadingLabel;

            if (config.loadingDuration > 0f)
                yield return new WaitForSecondsRealtime(config.loadingDuration);

            SetVisible(loadingIndicator, false);

            var hasNextScene = !string.IsNullOrWhiteSpace(config.nextSceneName)
                && Application.CanStreamedLevelBeLoaded(config.nextSceneName);

            if (enterLabel != null)
                enterLabel.text = idleEnterLabel;

            if (!hasNextScene)
            {
                SetFormEnabled(true);
                busy = false;

                if (statusLabel != null)
                {
                    statusLabel.text = string.Format(welcomeFormat, displayName) + "\n"
                        + string.Format(demoMessage, config.nextSceneName ?? "Game");
                }

                SetVisible(statusLabel, true);
                yield break;
            }

            if (fadeOverlay != null)
            {
                fadeOverlay.blocksRaycasts = true;
                yield return FadeCanvas(fadeOverlay, fadeOverlay.alpha, 1f, config.fadeDuration);
            }

            SceneManager.LoadScene(config.nextSceneName);
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

        private void SetFormEnabled(bool enabled)
        {
            if (nameField != null) nameField.interactable = enabled;
            if (passwordField != null) passwordField.interactable = enabled;
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
