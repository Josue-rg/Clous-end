using ClousEnd.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ClousEnd.EditorTools
{
    /// <summary>HUD de pruebas: retícula, controles y nombre de zona.</summary>
    internal static class MapUIBuilder
    {
        public static GameObject Build(
            Transform parent,
            MapAssets assets,
            PlayerInputReader input,
            Transform viewer,
            out RectTransform canvasRect,
            out RectTransform reticle)
        {
            var canvasGo = new GameObject(
                "UI",
                typeof(RectTransform),
                typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster));

            canvasGo.transform.SetParent(parent, false);

            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasRect = canvasGo.GetComponent<RectTransform>();
            reticle = CreateReticle(canvasRect, assets);
            CreateControlsPanel(canvasRect, assets);
            CreateTitle(canvasRect, assets);
            CreateZoneTitle(canvasRect, assets, viewer);

            var pointer = canvasGo.AddComponent<PointerReticle>();
            SerializedField.Set(pointer, "input", input);
            SerializedField.Set(pointer, "canvasRect", canvasRect);
            SerializedField.Set(pointer, "reticle", reticle);
            SerializedField.Set(pointer, "hideSystemCursor", true);

            return canvasGo;
        }

        private static RectTransform CreateReticle(RectTransform canvas, MapAssets assets)
        {
            var root = new GameObject("Reticle", typeof(RectTransform));
            var rootRect = root.GetComponent<RectTransform>();
            rootRect.SetParent(canvas, false);
            rootRect.anchorMin = new Vector2(0.5f, 0.5f);
            rootRect.anchorMax = new Vector2(0.5f, 0.5f);
            rootRect.pivot = new Vector2(0.5f, 0.5f);
            rootRect.anchoredPosition = Vector2.zero;
            rootRect.sizeDelta = new Vector2(58f, 58f);

            CreateReticleImage("Outline", rootRect, assets.Crosshair, new Color(0f, 0f, 0f, 0.55f), 66f);
            CreateReticleImage("Crosshair", rootRect, assets.Crosshair, new Color(0.92f, 0.98f, 1f, 0.95f), 58f);
            return rootRect;
        }

        private static void CreateReticleImage(string name, RectTransform parent, Sprite sprite, Color color, float size)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(size, size);

            var image = go.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.raycastTarget = false;
            image.preserveAspect = true;
        }

        private static void CreateControlsPanel(RectTransform canvas, MapAssets assets)
        {
            var panel = new GameObject("Controls", typeof(RectTransform));
            var rect = panel.GetComponent<RectTransform>();
            rect.SetParent(canvas, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.zero;
            rect.pivot = Vector2.zero;
            rect.anchoredPosition = new Vector2(26f, 26f);
            rect.sizeDelta = new Vector2(620f, 208f);

            var background = panel.AddComponent<Image>();
            background.sprite = assets.Panel;
            background.color = new Color(0.015f, 0.025f, 0.04f, 0.72f);
            background.raycastTarget = false;

            var accent = new GameObject("Accent", typeof(RectTransform));
            var accentRect = accent.GetComponent<RectTransform>();
            accentRect.SetParent(rect, false);
            accentRect.anchorMin = new Vector2(0f, 0f);
            accentRect.anchorMax = new Vector2(0f, 1f);
            accentRect.pivot = new Vector2(0f, 0.5f);
            accentRect.anchoredPosition = Vector2.zero;
            accentRect.sizeDelta = new Vector2(3f, 0f);

            var accentImage = accent.AddComponent<Image>();
            accentImage.sprite = assets.Panel;
            accentImage.color = new Color(1f, 0.3f, 0.36f, 0.9f);
            accentImage.raycastTarget = false;

            var text = CreateText(
                "ControlsText",
                rect,
                "W / S  ·  ↑ / ↓    AVANZAR Y RETROCEDER\n" +
                "A / D  ·  ← / →    DESPLAZARSE\n" +
                "ESPACIO    SUBIR       CTRL / Q    BAJAR\n" +
                "CLIC IZQUIERDO    DISPARAR\n" +
                "CLIC DERECHO + ARRASTRAR    GIRAR CÁMARA\n" +
                "RUEDA    ZOOM",
                assets.BodyFont,
                17f,
                new Color(0.78f, 0.87f, 0.96f),
                TextAlignmentOptions.TopLeft,
                6f,
                3f);

            text.rectTransform.anchorMin = Vector2.zero;
            text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMin = new Vector2(22f, 16f);
            text.rectTransform.offsetMax = new Vector2(-16f, -16f);
        }

        private static void CreateTitle(RectTransform canvas, MapAssets assets)
        {
            var title = CreateText(
                "Title",
                canvas,
                "DISTRITO AURORA  ·  BLOCKOUT",
                assets.DisplayFont,
                26f,
                new Color(0.78f, 0.9f, 1f, 0.92f),
                TextAlignmentOptions.TopLeft,
                5f);

            var rect = title.rectTransform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(30f, -24f);
            rect.sizeDelta = new Vector2(700f, 36f);

            var subtitle = CreateText(
                "Subtitle",
                canvas,
                "CLOUS-END  ·  ESCENA DE PRUEBA",
                assets.BodyFont,
                12f,
                new Color(0.55f, 0.68f, 0.8f, 0.9f),
                TextAlignmentOptions.TopLeft,
                2f);

            var subRect = subtitle.rectTransform;
            subRect.anchorMin = new Vector2(0f, 1f);
            subRect.anchorMax = new Vector2(0f, 1f);
            subRect.pivot = new Vector2(0f, 1f);
            subRect.anchoredPosition = new Vector2(32f, -58f);
            subRect.sizeDelta = new Vector2(700f, 20f);
        }

        private static void CreateZoneTitle(RectTransform canvas, MapAssets assets, Transform viewer)
        {
            var go = new GameObject("ZoneTitle", typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(canvas, false);
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -54f);
            rect.sizeDelta = new Vector2(900f, 64f);

            var text = go.AddComponent<TextMeshProUGUI>();
            text.font = assets.DisplayFont;
            text.text = "AFUERAS";
            text.fontSize = 46f;
            text.color = new Color(0.72f, 0.95f, 1f, 0.95f);
            text.alignment = TextAlignmentOptions.Center;
            text.characterSpacing = 8f;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.overflowMode = TextOverflowModes.Overflow;
            text.raycastTarget = false;
            text.richText = false;

            var group = go.AddComponent<CanvasGroup>();
            group.alpha = 1f;
            group.blocksRaycasts = false;
            group.interactable = false;

            var hud = go.AddComponent<ZoneTitleHud>();
            SerializedField.Set(hud, "label", text);
            SerializedField.Set(hud, "group", group);
            SerializedField.Set(hud, "viewer", viewer);
            SerializedField.Set(hud, "visibleDuration", 2.4f);
            SerializedField.Set(hud, "fadeDuration", 0.8f);

            hud.zones = new[]
            {
                new ZoneTitleHud.ZoneLabel { startZ = 0f, title = "AFUERAS" },
                new ZoneTitleHud.ZoneLabel { startZ = MapPalette.AfuerasEnd, title = "CENTRO HISTÓRICO" },
                new ZoneTitleHud.ZoneLabel { startZ = MapPalette.CentroEnd, title = "DISTRITO FINANCIERO" },
                new ZoneTitleHud.ZoneLabel { startZ = MapPalette.FinancieroEnd, title = "REFUGIO AURORA" }
            };
        }

        private static TextMeshProUGUI CreateText(
            string name,
            Transform parent,
            string content,
            TMP_FontAsset font,
            float size,
            Color color,
            TextAlignmentOptions alignment,
            float characterSpacing,
            float lineSpacing = 0f)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);

            var text = go.AddComponent<TextMeshProUGUI>();
            text.font = font;
            text.text = content;
            text.fontSize = size;
            text.color = color;
            text.alignment = alignment;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.characterSpacing = characterSpacing;
            text.lineSpacing = lineSpacing;
            text.overflowMode = TextOverflowModes.Overflow;
            text.raycastTarget = false;
            text.richText = false;

            return text;
        }
    }
}
