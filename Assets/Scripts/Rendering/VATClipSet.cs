using UnityEngine;

namespace ClousEnd.Rendering
{
    /// <summary>
    /// Conjunto de clips VAT de un asset (textura, frames, fps y loop). Lo genera
    /// el importador y permite cambiar de animación sin hardcodear rutas.
    /// </summary>
    [CreateAssetMenu(menuName = "Clous-End/VAT Clip Set", fileName = "VAT_Clips")]
    public sealed class VATClipSet : ScriptableObject
    {
        public VATClipEntry[] clips = new VATClipEntry[0];

        public VATClipEntry Find(string clipName)
        {
            if (clips == null || string.IsNullOrEmpty(clipName))
                return null;

            for (var i = 0; i < clips.Length; i++)
            {
                var clip = clips[i];

                if (clip != null && string.Equals(clip.name, clipName, System.StringComparison.OrdinalIgnoreCase))
                    return clip;
            }

            return null;
        }

        public bool Has(string clipName)
        {
            return Find(clipName) != null;
        }
    }

    [System.Serializable]
    public sealed class VATClipEntry
    {
        public string name;
        public Texture2D positions;
        public int frames = 24;
        public float fps = 30f;
        public bool loop;
    }
}
