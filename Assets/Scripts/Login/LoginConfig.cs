using UnityEngine;

namespace ClousEnd.Login
{
    /// <summary>
    /// Ajustes de la pantalla de acceso. Todos los valores se editan en el Inspector.
    /// </summary>
    [CreateAssetMenu(fileName = "LoginConfig", menuName = "Clous-End/Login Config")]
    public sealed class LoginConfig : ScriptableObject
    {
        [Header("Validación del perfil")]
        [Min(1)] public int minNameLength = 2;
        [Min(1)] public int minPasswordLength = 4;
        [Min(1)] public int maxNameLength = 20;
        [Min(1)] public int maxPasswordLength = 32;
        [Tooltip("Si el perfil no existe, se crea en el primer acceso.")]
        public bool createProfileIfMissing = true;

        [Header("Transición")]
        [Tooltip("Escena que se carga al entrar. Debe estar en Build Settings.")]
        public string nextSceneName = "Game";
        [Min(0f)] public float loadingDuration = 0.9f;
        [Min(0f)] public float successHold = 1.1f;
        [Min(0f)] public float fadeDuration = 0.6f;
    }
}
