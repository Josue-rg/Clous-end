using ClousEnd.Game;
using TMPro;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace ClousEnd.EditorTools
{
    internal sealed class MapUiRefs
    {
        public RectTransform Canvas;
        public RectTransform Reticle;
        public TMP_Text TimerLabel;
        public CanvasGroup CheckpointToast;
        public GameObject FinishPanel;
        public TMP_Text FinishTime;
        public TMP_Text FinishRenos;
        public Button FinishRetryButton;
        public Button FinishLobbyButton;
        public GameObject GameOverPanel;
        public TMP_Text GameOverReason;
        public Button GameOverRetryButton;
        public Button GameOverLobbyButton;
        public GameHud Hud;
        public PauseMenu Pause;
        public GameObject PausePanel;
        public Button PauseResumeButton;
        public Button PauseRestartButton;
        public Button PauseLobbyButton;
    }

    /// <summary>HUD de pruebas: retícula, controles, zona, tiempo y paneles de misión.</summary>
    internal static class MapUIBuilder
    {
        private static readonly Color PanelDark = new Color(0.004f, 0.008f, 0.016f, 0.995f);
        private static readonly Color ButtonRed = new Color(0.62f, 0.09f, 0.12f, 0.96f);
        private static readonly Color ButtonDark = new Color(0.05f, 0.09f, 0.14f, 0.92f);

        public static MapUiRefs Build(
            Transform parent,
            MapAssets assets,
            PlayerInputReader input,
            Transform viewer)
        {
            var refs = new MapUiRefs();

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

            refs.Canvas = canvasGo.GetComponent<RectTransform>();
            refs.Reticle = CreateReticle(refs.Canvas, assets);
            CreateControlsPanel(refs.Canvas, assets);
            CreateTitle(refs.Canvas, assets);
            CreateZoneTitle(refs.Canvas, assets, viewer);
            refs.TimerLabel = CreateTimer(refs.Canvas, assets);
            refs.CheckpointToast = CreateCheckpointToast(refs.Canvas, assets);
            refs.Hud = GameHudBuilder.Build(refs.Canvas, assets);

            refs.FinishPanel = CreateResultsPanel(refs.Canvas, assets, refs);
            refs.GameOverPanel = CreateGameOverPanel(refs.Canvas, assets, refs);
            CreatePausePanel(refs.Canvas, assets, refs, input);

            CreateEventSystem(parent);

            var pointer = canvasGo.AddComponent<PointerReticle>();
            SerializedField.Set(pointer, "input", input);
            SerializedField.Set(pointer, "canvasRect", refs.Canvas);
            SerializedField.Set(pointer, "reticle", refs.Reticle);
            SerializedField.Set(pointer, "hideSystemCursor", true);

            return refs;
        }

        private static void CreateEventSystem(Transform parent)
        {
            var go = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            go.transform.SetParent(parent, false);
            go.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
        }

        private static GameObject CreateResultsPanel(RectTransform canvas, MapAssets assets, MapUiRefs refs)
        {
            var panel = CreateOverlay("FinishPanel", canvas, assets);

            var title = CreateText(
                "FinishTitle",
                panel.transform,
                "MISIÓN COMPLETADA",
                assets.DisplayFont,
                58f,
                new Color(0.86f, 0.98f, 1f),
                TextAlignmentOptions.Center,
                7f);
            Center(title.rectTransform, new Vector2(1100f, 80f), new Vector2(0f, 112f));

            var line = new GameObject("FinishLine", typeof(RectTransform));
            var lineRect = line.GetComponent<RectTransform>();
            lineRect.SetParent(panel.transform, false);
            Center(lineRect, new Vector2(140f, 2f), new Vector2(0f, 64f));

            var lineImage = line.AddComponent<Image>();
            lineImage.sprite = assets.Panel;
            lineImage.color = new Color(0.42f, 1f, 0.85f, 0.95f);
            lineImage.raycastTarget = false;

            var subtitle = CreateText(
                "FinishSubtitle",
                panel.transform,
                "REFUGIO AURORA ALCANZADO",
                assets.BodyFont,
                18f,
                new Color(0.55f, 0.95f, 0.9f),
                TextAlignmentOptions.Center,
                5f);
            Center(subtitle.rectTransform, new Vector2(900f, 28f), new Vector2(0f, 36f));

            refs.FinishTime = CreateText(
                "FinishTime",
                panel.transform,
                "TIEMPO  00:00",
                assets.BodyFont,
                24f,
                new Color(0.86f, 0.92f, 1f),
                TextAlignmentOptions.Center,
                3f);
            Center(refs.FinishTime.rectTransform, new Vector2(900f, 34f), new Vector2(0f, -26f));

            refs.FinishRenos = CreateText(
                "FinishRenos",
                panel.transform,
                "RENOS  4 / 4",
                assets.BodyFont,
                16f,
                new Color(0.62f, 0.85f, 0.95f),
                TextAlignmentOptions.Center,
                3f);
            Center(refs.FinishRenos.rectTransform, new Vector2(900f, 26f), new Vector2(0f, -62f));

            refs.FinishRetryButton = CreateButton(
                "FinishRetry",
                panel.transform,
                "REINTENTAR",
                new Vector2(-132f, -150f),
                ButtonRed,
                assets);

            refs.FinishLobbyButton = CreateButton(
                "FinishLobby",
                panel.transform,
                "LOBBY",
                new Vector2(132f, -150f),
                ButtonDark,
                assets);

            var credits = CreateText(
                "FinishCredits",
                panel.transform,
                "CLOUS-END  ·  Gracias por jugar  ·  Un juego de Josué R.G.",
                assets.BodyFont,
                13f,
                new Color(0.48f, 0.62f, 0.74f),
                TextAlignmentOptions.Center,
                2f);
            Center(credits.rectTransform, new Vector2(1000f, 24f), new Vector2(0f, -226f));

            panel.SetActive(false);
            return panel;
        }

        private static GameObject CreateGameOverPanel(RectTransform canvas, MapAssets assets, MapUiRefs refs)
        {
            var panel = CreateOverlay("GameOverPanel", canvas, assets);

            var title = CreateText(
                "GameOverTitle",
                panel.transform,
                "PARTIDA TERMINADA",
                assets.DisplayFont,
                54f,
                new Color(1f, 0.55f, 0.55f),
                TextAlignmentOptions.Center,
                6f);
            Center(title.rectTransform, new Vector2(1100f, 76f), new Vector2(0f, 78f));

            var subtitle = CreateText(
                "GameOverSubtitle",
                panel.transform,
                "El trineo cayó.",
                assets.BodyFont,
                20f,
                new Color(0.8f, 0.86f, 0.94f),
                TextAlignmentOptions.Center,
                2f);
            Center(subtitle.rectTransform, new Vector2(900f, 30f), new Vector2(0f, 26f));
            refs.GameOverReason = subtitle;

            refs.GameOverRetryButton = CreateButton(
                "GameOverRetry",
                panel.transform,
                "REINTENTAR",
                new Vector2(-132f, -48f),
                ButtonRed,
                assets);

            refs.GameOverLobbyButton = CreateButton(
                "GameOverLobby",
                panel.transform,
                "LOBBY",
                new Vector2(132f, -48f),
                ButtonDark,
                assets);

            panel.SetActive(false);
            return panel;
        }

        private static void CreatePausePanel(
            RectTransform canvas,
            MapAssets assets,
            MapUiRefs refs,
            PlayerInputReader input)
        {
            var panel = CreateOverlay("PausePanel", canvas, assets);

            var title = CreateText(
                "PauseTitle",
                panel.transform,
                "PAUSA",
                assets.DisplayFont,
                56f,
                new Color(0.86f, 0.97f, 1f),
                TextAlignmentOptions.Center,
                8f);
            Center(title.rectTransform, new Vector2(900f, 80f), new Vector2(0f, 150f));

            var hint = CreateText(
                "PauseHint",
                panel.transform,
                "ESC  REANUDAR     ·     1–4  CAMBIAR ARMA     ·     R  RECARGAR     ·     CLIC IZQ  DISPARAR",
                assets.BodyFont,
                15f,
                new Color(0.62f, 0.76f, 0.88f),
                TextAlignmentOptions.Center,
                2f);
            Center(hint.rectTransform, new Vector2(1400f, 26f), new Vector2(0f, 92f));

            refs.PauseResumeButton = CreateButton(
                "PauseResume",
                panel.transform,
                "REANUDAR",
                new Vector2(-256f, -40f),
                ButtonDark,
                assets);

            refs.PauseRestartButton = CreateButton(
                "PauseRestart",
                panel.transform,
                "REINICIAR",
                new Vector2(0f, -40f),
                ButtonRed,
                assets);

            refs.PauseLobbyButton = CreateButton(
                "PauseLobby",
                panel.transform,
                "LOBBY",
                new Vector2(256f, -40f),
                ButtonDark,
                assets);

            var footer = CreateText(
                "PauseFooter",
                panel.transform,
                "CLOUS-END  ·  DISTRITO AURORA",
                assets.BodyFont,
                13f,
                new Color(0.45f, 0.58f, 0.7f),
                TextAlignmentOptions.Center,
                3f);
            Center(footer.rectTransform, new Vector2(900f, 24f), new Vector2(0f, -150f));

            panel.SetActive(false);

            var pause = canvas.gameObject.AddComponent<PauseMenu>();
            SerializedField.Set(pause, "input", input);
            SerializedField.Set(pause, "panel", panel);
            SerializedField.Set(pause, "reticleRoot", refs.Reticle.gameObject);

            UnityEventTools.AddPersistentListener(
                refs.PauseResumeButton.onClick,
                new UnityAction(pause.Resume));
            UnityEventTools.AddPersistentListener(
                refs.PauseRestartButton.onClick,
                new UnityAction(pause.RestartRun));
            UnityEventTools.AddPersistentListener(
                refs.PauseLobbyButton.onClick,
                new UnityAction(pause.ReturnToLobby));

            refs.Pause = pause;
            refs.PausePanel = panel;
        }

        private static GameObject CreateOverlay(string name, RectTransform canvas, MapAssets assets)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(canvas, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            var image = go.AddComponent<Image>();
            image.sprite = assets.Panel;
            image.color = PanelDark;
            image.raycastTarget = true;

            return go;
        }

        private static Button CreateButton(
            string name,
            Transform parent,
            string label,
            Vector2 position,
            Color background,
            MapAssets assets)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            Center(rect, new Vector2(244f, 54f), position);

            var image = go.AddComponent<Image>();
            image.sprite = assets.Panel;
            image.color = background;
            image.raycastTarget = true;

            var button = go.AddComponent<Button>();
            button.targetGraphic = image;
            button.transition = Selectable.Transition.ColorTint;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            button.colors = new ColorBlock
            {
                normalColor = Color.white,
                highlightedColor = new Color(0.84f, 0.92f, 1f),
                pressedColor = new Color(0.68f, 0.76f, 0.86f),
                selectedColor = Color.white,
                disabledColor = new Color(0.45f, 0.45f, 0.45f, 0.5f),
                colorMultiplier = 1f,
                fadeDuration = 0.08f
            };

            var text = CreateText(
                name + "Label",
                rect,
                label,
                assets.BodyBoldFont,
                16f,
                new Color(0.96f, 0.98f, 1f),
                TextAlignmentOptions.Center,
                3f);

            text.rectTransform.anchorMin = Vector2.zero;
            text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMin = Vector2.zero;
            text.rectTransform.offsetMax = Vector2.zero;
            return button;
        }

        private static TMP_Text CreateTimer(RectTransform canvas, MapAssets assets)
        {
            var text = CreateText(
                "Timer",
                canvas,
                "00:00",
                assets.DisplayFont,
                28f,
                new Color(0.74f, 0.9f, 1f, 0.92f),
                TextAlignmentOptions.TopRight,
                4f);

            var rect = text.rectTransform;
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-30f, -24f);
            rect.sizeDelta = new Vector2(220f, 40f);
            return text;
        }

        private static CanvasGroup CreateCheckpointToast(RectTransform canvas, MapAssets assets)
        {
            var go = new GameObject("CheckpointToast", typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(canvas, false);
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.pivot = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -128f);
            rect.sizeDelta = new Vector2(600f, 46f);

            var text = CreateText(
                "CheckpointLabel",
                rect,
                "CHECKPOINT",
                assets.DisplayFont,
                30f,
                new Color(0.55f, 1f, 0.88f, 0.95f),
                TextAlignmentOptions.Center,
                7f);
            text.rectTransform.anchorMin = Vector2.zero;
            text.rectTransform.anchorMax = Vector2.one;
            text.rectTransform.offsetMin = Vector2.zero;
            text.rectTransform.offsetMax = Vector2.zero;

            var group = go.AddComponent<CanvasGroup>();
            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;
            return group;
        }

        private static void Center(RectTransform rect, Vector2 size, Vector2 position)
        {
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = position;
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
                "A / D  ·  ← / →    ESQUIVAR\n" +
                "ESPACIO / CTRL    SUBIR / BAJAR\n" +
                "CLIC IZQUIERDO    DISPARAR        1–4  ARMA\n" +
                "R    RECARGAR        ESC    PAUSA\n" +
                "CLIC DERECHO + ARRASTRAR    GIRAR CÁMARA\n" +
                "RUEDA    ZOOM",
                assets.BodyFont,
                16f,
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
                "DISTRITO AURORA",
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
                "CLOUS-END  ·  MISIÓN ÚNICA",
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
