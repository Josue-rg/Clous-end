using UnityEngine;

namespace ClousEnd.EditorTools
{
    /// <summary>
    /// Paleta, medidas y cortes del blockout de Distrito Aurora. Cambiar aquí
    /// y reconstruir la escena Game desde el menú Clous-End.
    /// </summary>
    internal static class MapPalette
    {
        // Ruta (metros).
        public const float RoadWidth = 26f;
        public const float SidewalkWidth = 6f;
        public const float BuildingLine = RoadWidth * 0.5f + SidewalkWidth;
        public const float RouteStart = 0f;
        public const float RouteEnd = 1500f;

        // Cortes de zona en Z.
        public const float AfuerasEnd = 340f;
        public const float CentroEnd = 820f;
        public const float FinancieroEnd = 1180f;
        public const float RefugioEnd = RouteEnd;

        // Colores base.
        public static readonly Color Road = Hex("#22262D");
        public static readonly Color RoadPaint = Hex("#C6D2DE");
        public static readonly Color Sidewalk = Hex("#3F4650");
        public static readonly Color Snow = Hex("#D8E4F2");
        public static readonly Color SnowRoof = Hex("#EAF3FC");
        public static readonly Color Concrete = Hex("#6A727D");
        public static readonly Color HistoricPlaster = Hex("#C9B393");
        public static readonly Color HistoricBrick = Hex("#8A5C48");
        public static readonly Color HistoricWood = Hex("#543C2C");
        public static readonly Color TowerA = Hex("#2B3644");
        public static readonly Color TowerB = Hex("#36424F");
        public static readonly Color TowerC = Hex("#414D5B");
        public static readonly Color Glass = Hex("#101821");
        public static readonly Color Metal = Hex("#3A4046");
        public static readonly Color Rubble = Hex("#494D53");
        public static readonly Color WarmLight = Hex("#FFC46A");
        public static readonly Color CoolLight = Hex("#8FDFFF");
        public static readonly Color AlienLight = Hex("#C43CFF");
        public static readonly Color RefugeLight = Hex("#6BFFD9");
        public static readonly Color AlienObsidian = Hex("#05070C");
        public static readonly Color AuroraCool = Hex("#4DFFC4");
        public static readonly Color AuroraWarm = Hex("#B44BFF");
        public static readonly Color CarPaint = Hex("#8C1626");
        public static readonly Color Character = Hex("#E8D9C5");
        public static readonly Color Gunmetal = Hex("#23262B");
        public static readonly Color Fog = Hex("#070D16");

        public static readonly Color[] HistoricWalls =
        {
            HistoricPlaster,
            HistoricBrick,
            Hex("#A79277"),
            Hex("#8C6A56"),
            Hex("#C2B29A")
        };

        public static readonly Color[] TowerWalls =
        {
            TowerA,
            TowerB,
            TowerC,
            Hex("#232D39"),
            Hex("#394654")
        };

        public static Color Hex(string value)
        {
            return ColorUtility.TryParseHtmlString(value, out var color) ? color : Color.magenta;
        }
    }
}
