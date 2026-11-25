using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>Paleta y medidas del diorama. Cambiar aquí y reconstruir la escena.</summary>
    internal static class LoginPalette
    {
        public static readonly Color Snow = Hex("#D7E3F0");
        public static readonly Color SnowRoof = Hex("#EAF2FB");
        public static readonly Color WoodDark = Hex("#3A2A22");
        public static readonly Color WoodWarm = Hex("#6A4630");
        public static readonly Color Stone = Hex("#3E4147");
        public static readonly Color Ice = Hex("#7FA2B8");
        public static readonly Color Mountain = Hex("#536A88");
        public static readonly Color Pine = Hex("#10231B");
        public static readonly Color Alien = Hex("#05070C");
        public static readonly Color WarmLight = Hex("#FFC46A");
        public static readonly Color RedLight = Hex("#FF2E3C");
        public static readonly Color CoolLight = Hex("#5FE3C8");
        public static readonly Color MoonLight = Hex("#AFCBFF");

        public static readonly Vector3 CameraPosition = new Vector3(0f, 4.4f, -20f);
        public static readonly Vector3 CameraEuler = new Vector3(4f, 0f, 0f);
        public const float CameraFieldOfView = 42f;

        public static Color Hex(string value)
        {
            return ColorUtility.TryParseHtmlString(value, out var color) ? color : Color.magenta;
        }
    }
}
