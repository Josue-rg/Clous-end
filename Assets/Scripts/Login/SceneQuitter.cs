using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ClousEnd.Login
{
    /// <summary>Cierra el juego en build y detiene el Play Mode en el editor.</summary>
    public sealed class SceneQuitter : MonoBehaviour
    {
        public void Quit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
