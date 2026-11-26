using System.Collections.Generic;
using ClousEnd.Login;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace ClousEnd.EditorTools
{
    /// <summary>
    /// Construye el Canvas completo del login como objetos reales y editables.
    /// Los botones, campos y textos quedan en la Jerarquía, no se crean en Play.
    /// </summary>
    internal static class LoginUIBuilder
    {
        private static readonly Color Cream = Hex("#F4F7FA");
        private static readonly Color Ice = Hex("#B7CBDD");
        private static readonly Color Dim = Hex("#748A9E");
        private static readonly Color Red = Hex("#C8202F");
        private static readonly Color Aurora = Hex("#5FE3C8");
        private static readonly Color Danger = Hex("#FF6B6B");
        private static readonly Color PanelColor = new Color(0.025f, 0.05f, 0.09f, 0.96f);
        private static readonly Color InputColor = new Color(0.012f, 0.025f, 0.045f, 0.88f);
        private static readonly Color LineColor = new Color(0.85f, 0.93f, 0.98f, 0.18f);

        private sealed class ButtonRefs
        {
            public RectTransform Root;
            public Button Button;
            public TextMeshProUGUI Label;
            public GameObject Spinner;
        }

        public static void Build(Transform parent, LoginAssets assets, LoginConfig config)
        {
            var canvasGo = new GameObject("LoginCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvasGo.transform.SetParent(parent, false);

            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            var scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            var screen = CreateRect("Screen", canvasGo.transform, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            screen.offsetMin = Vector2.zero;
            screen.offsetMax = Vector2.zero;

            CreateBackdrop(screen, assets);
            var brand = CreateBrand(screen, assets);
            var topRight = CreateTopRight(screen, assets);
            var content = CreateContent(screen, assets);
            var panel = CreatePanel(content, assets, config, out var primary, out var secondary, out var statusLabel);
            var bottomRight = CreateBottomRight(screen, assets);
            var fade = CreateFadeOverlay(screen, assets);
            CreateEventSystem(parent);

            var controller = screen.gameObject.AddComponent<LoginFormController>();
            var quitter = secondary.Root.gameObject.AddComponent<SceneQuitter>();

            SerializedField.Set(controller, "config", config);
            SerializedField.Set(controller, "enterButton", primary.Button);
            SerializedField.Set(controller, "exitButton", secondary.Button);
            SerializedField.Set(controller, "enterLabel", primary.Label);
            SerializedField.Set(controller, "statusLabel", statusLabel);
            SerializedField.Set(controller, "loadingIndicator", primary.Spinner);
            SerializedField.Set(controller, "fadeOverlay", fade);
            SerializedField.Set(controller, "loadingLabel", "ENTRANDO EN DISTRITO AURORA…");
            SerializedField.Set(controller, "demoMessage", "La escena «{0}» todavía no está en Build Settings.");

            UnityEventTools.AddPersistentListener(primary.Button.onClick, new UnityAction(controller.Submit));
            UnityEventTools.AddPersistentListener(secondary.Button.onClick, new UnityAction(quitter.Quit));

            var intro = screen.gameObject.AddComponent<LoginIntroAnimator>();
            var introTargets = new List<LoginIntroAnimator.IntroTarget>
            {
                MakeIntroTarget(brand, 0.00f, 0.55f, 14f),
                MakeIntroTarget(topRight, 0.05f, 0.50f, 10f),
                MakeIntroTarget(content.Find("Eyebrow") as RectTransform, 0.12f, 0.50f, 14f),
                MakeIntroTarget(content.Find("Title") as RectTransform, 0.18f, 0.70f, 24f),
                MakeIntroTarget(content.Find("Tagline") as RectTransform, 0.28f, 0.55f, 18f),
                MakeIntroTarget(panel, 0.36f, 0.70f, 28f),
                MakeIntroTarget(bottomRight, 0.55f, 0.50f, 10f)
            };

            var serializedIntro = new SerializedObject(intro);
            var targetsProperty = serializedIntro.FindProperty("targets");
            targetsProperty.arraySize = introTargets.Count;
            for (var i = 0; i < introTargets.Count; i++)
            {
                var element = targetsProperty.GetArrayElementAtIndex(i);
                element.FindPropertyRelative("group").objectReferenceValue = introTargets[i].group;
                element.FindPropertyRelative("rect").objectReferenceValue = introTargets[i].rect;
                element.FindPropertyRelative("delay").floatValue = introTargets[i].delay;
                element.FindPropertyRelative("duration").floatValue = introTargets[i].duration;
                element.FindPropertyRelative("offset").vector2Value = introTargets[i].offset;
            }
            serializedIntro.ApplyModifiedPropertiesWithoutUndo();

            Selection.activeGameObject = screen.gameObject;
        }

        private static void CreateBackdrop(RectTransform parent, LoginAssets assets)
        {
            var gradient = CreateImage("LeftGradient", parent, assets.LeftGradient, Color.white, Image.Type.Simple, false);
            var gradientRect = gradient.rectTransform;
            gradientRect.anchorMin = new Vector2(0f, 0f);
            gradientRect.anchorMax = new Vector2(0f, 1f);
            gradientRect.pivot = new Vector2(0f, 0.5f);
            gradientRect.anchoredPosition = Vector2.zero;
            gradientRect.sizeDelta = new Vector2(1280f, 0f);

            var vignette = CreateImage("Vignette", parent, assets.Vignette, Color.white, Image.Type.Simple, false);
            var vignetteRect = vignette.rectTransform;
            vignetteRect.anchorMin = Vector2.zero;
            vignetteRect.anchorMax = Vector2.one;
            vignetteRect.offsetMin = Vector2.zero;
            vignetteRect.offsetMax = Vector2.zero;
        }

        private static RectTransform CreateBrand(RectTransform parent, LoginAssets assets)
        {
            var brand = CreateRect("Brand", parent, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(96f, -54f), new Vector2(420f, 28f));
            brand.gameObject.AddComponent<CanvasGroup>();

            var dot = CreateImage("Dot", brand, assets.Dot, Red, Image.Type.Simple, false);
            SetTopLeft(dot.rectTransform, 0f, 7f, 9f, 9f);

            var word = CreateText(
                "Wordmark", brand, "CLOUS-END", assets.Display, 22f, Cream,
                TextAlignmentOptions.MidlineLeft, false, 0f, 3f);
            SetTopLeft(word.rectTransform, 16f, 0f, 280f, 28f);

            var divider = CreateImage("Divider", brand, assets.White, LineColor, Image.Type.Simple, false);
            SetTopLeft(divider.rectTransform, 176f, 7f, 1f, 14f);

            var protocol = CreateText(
                "Protocol", brand, "PROTOCOLO AURORA", assets.Body, 10f, Dim,
                TextAlignmentOptions.MidlineLeft, false, 6f);
            SetTopLeft(protocol.rectTransform, 190f, 4f, 220f, 20f);

            return brand;
        }

        private static RectTransform CreateTopRight(RectTransform parent, LoginAssets assets)
        {
            var root = CreateRect("TopRight", parent, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-96f, -54f), new Vector2(360f, 24f));
            root.gameObject.AddComponent<CanvasGroup>();

            var dot = CreateImage("SignalDot", root, assets.Dot, Aurora, Image.Type.Simple, false);
            SetTopRight(dot.rectTransform, 356f, 8f, 7f, 7f);

            var label = CreateText(
                "SignalLabel", root, "AURORA-9  ·  78.4 MHz", assets.Body, 10.5f, Ice,
                TextAlignmentOptions.MidlineRight, false, 3f);
            label.rectTransform.anchorMin = new Vector2(0f, 1f);
            label.rectTransform.anchorMax = new Vector2(1f, 1f);
            label.rectTransform.pivot = new Vector2(1f, 1f);
            label.rectTransform.anchoredPosition = new Vector2(0f, 0f);
            label.rectTransform.sizeDelta = new Vector2(0f, 24f);

            return root;
        }

        private static RectTransform CreateContent(RectTransform parent, LoginAssets assets)
        {
            var content = CreateRect("Content", parent, new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(96f, -20f), new Vector2(660f, 800f));

            var eyebrow = CreateText(
                "Eyebrow", content, "LA ÚLTIMA NAVIDAD", assets.BodyBold, 13f, Aurora,
                TextAlignmentOptions.MidlineLeft, false, 6f);
            SetTopLeft(eyebrow.rectTransform, 0f, 0f, 620f, 18f);
            eyebrow.gameObject.AddComponent<CanvasGroup>();

            var title = CreateText(
                "Title", content, "CLOUS-END", assets.Display, 122f, Cream,
                TextAlignmentOptions.TopLeft, false, 8f);
            SetTopLeft(title.rectTransform, 0f, 26f, 660f, 150f);
            title.gameObject.AddComponent<CanvasGroup>();

            var accent = CreateImage("TitleAccent", content, assets.White, Red, Image.Type.Simple, false);
            SetTopLeft(accent.rectTransform, 2f, 184f, 64f, 2f);

            var tagline = CreateText(
                "Tagline", content, "Cuatro renos. Un trineo.\nUna ciudad que aún respira.", assets.Body, 22f, Ice,
                TextAlignmentOptions.TopLeft, true, 0f, 8f);
            SetTopLeft(tagline.rectTransform, 0f, 208f, 540f, 72f);
            tagline.gameObject.AddComponent<CanvasGroup>();

            return content;
        }

        private static RectTransform CreatePanel(
            RectTransform content,
            LoginAssets assets,
            LoginConfig config,
            out ButtonRefs primary,
            out ButtonRefs secondary,
            out TMP_Text statusLabel)
        {
            var panel = CreateRect("LoginPanel", content, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, -312f), new Vector2(540f, 470f));
            panel.gameObject.AddComponent<CanvasGroup>();

            CreateBorder(panel, assets, LineColor);

            var background = CreateImage("Background", panel, assets.Rounded, PanelColor, Image.Type.Sliced, true);
            Stretch(background.rectTransform);

            var accent = CreateImage("Accent", panel, assets.White, Red, Image.Type.Simple, false);
            SetTopLeft(accent.rectTransform, 0f, 28f, 3f, 22f);

            var header = CreateText(
                "Header", panel, "DISTRITO AURORA", assets.BodyBold, 18f, Cream,
                TextAlignmentOptions.MidlineLeft, false, 3f);
            SetTopLeft(header.rectTransform, 28f, 28f, 484f, 24f);

            var divider = CreateImage("Divider", panel, assets.White, LineColor, Image.Type.Simple, false);
            SetTopLeft(divider.rectTransform, 28f, 78f, 484f, 1f);

            var ready = CreateText(
                "Ready", panel, "LA CIUDAD TE ESPERA", assets.Display, 34f, Cream,
                TextAlignmentOptions.MidlineLeft, false, 3f);
            SetTopLeft(ready.rectTransform, 28f, 96f, 484f, 44f);

            var tagline = CreateText(
                "Tagline", panel, "Un trineo. Cuatro renos. Una noche.", assets.Body, 14f, Ice,
                TextAlignmentOptions.MidlineLeft, false, 1f);
            SetTopLeft(tagline.rectTransform, 28f, 142f, 484f, 22f);

            var meta = CreateText(
                "Meta", panel, "4 zonas  ·  1,5 km  ·  vuelo libre", assets.Body, 11f, Dim,
                TextAlignmentOptions.MidlineLeft, false, 3f);
            SetTopLeft(meta.rectTransform, 28f, 165f, 484f, 18f);

            primary = CreatePrimaryButton(panel, assets, new Vector2(28f, -204f), new Vector2(484f, 64f));
            secondary = CreateSecondaryButton(panel, assets, new Vector2(28f, -284f), new Vector2(484f, 46f));

            statusLabel = CreateText(
                "StatusLabel", panel, string.Empty, assets.Body, 12f, Aurora,
                TextAlignmentOptions.TopLeft, true, 0f, 4f);
            SetTopLeft(statusLabel.rectTransform, 28f, 346f, 484f, 60f);
            statusLabel.gameObject.SetActive(false);

            return panel;
        }

        private static TMP_InputField CreateInputField(
            string name,
            RectTransform parent,
            LoginAssets assets,
            string placeholderText,
            TMP_InputField.ContentType contentType,
            int characterLimit,
            Vector2 position)
        {
            var root = CreateRect(name, parent, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), position, new Vector2(484f, 48f));

            CreateBorder(root, assets, LineColor);

            var background = CreateImage("Background", root, assets.Rounded, InputColor, Image.Type.Sliced, true);
            Stretch(background.rectTransform);

            var underline = CreateImage("FocusLine", root, assets.White, new Color(0.16f, 0.23f, 0.31f, 1f), Image.Type.Simple, false);
            var underlineRect = underline.rectTransform;
            underlineRect.anchorMin = new Vector2(0f, 0f);
            underlineRect.anchorMax = new Vector2(1f, 0f);
            underlineRect.pivot = new Vector2(0.5f, 0f);
            underlineRect.anchoredPosition = Vector2.zero;
            underlineRect.sizeDelta = new Vector2(0f, 2f);

            var textArea = CreateRect("Text Area", root, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            textArea.offsetMin = new Vector2(14f, 6f);
            textArea.offsetMax = new Vector2(-14f, -6f);
            textArea.gameObject.AddComponent<RectMask2D>();

            var placeholder = CreateText(
                "Placeholder", textArea, placeholderText, assets.Body, 15f, new Color(0.55f, 0.66f, 0.76f, 0.5f),
                TextAlignmentOptions.MidlineLeft, false, 0f);
            Stretch(placeholder.rectTransform);

            var text = CreateText(
                "Text", textArea, string.Empty, assets.Body, 16f, Cream,
                TextAlignmentOptions.MidlineLeft, false, 0f);
            Stretch(text.rectTransform);

            var input = root.gameObject.AddComponent<TMP_InputField>();
            input.textViewport = textArea;
            input.textComponent = text;
            input.placeholder = placeholder;
            input.targetGraphic = background;
            input.transition = Selectable.Transition.None;
            input.lineType = TMP_InputField.LineType.SingleLine;
            input.contentType = contentType;
            input.characterLimit = characterLimit;
            input.caretColor = Cream;
            input.customCaretColor = true;
            input.caretWidth = 1;
            input.selectionColor = new Color(0.37f, 0.89f, 0.78f, 0.32f);
            input.onFocusSelectAll = true;
            input.resetOnDeActivation = false;
            input.restoreOriginalTextOnEscape = true;
            input.richText = false;

            if (contentType == TMP_InputField.ContentType.Password)
                input.asteriskChar = '•';

            var focus = root.gameObject.AddComponent<FieldFocusHighlight>();
            SerializedField.Set(focus, "underline", underline);
            SerializedField.Set(focus, "idleColor", new Color(0.16f, 0.23f, 0.31f, 1f));
            SerializedField.Set(focus, "focusColor", new Color(0.85f, 0.93f, 0.98f, 1f));
            SerializedField.Set(focus, "fadeDuration", 0.12f);

            return input;
        }

        private static ButtonRefs CreatePrimaryButton(RectTransform parent, LoginAssets assets, Vector2 position, Vector2 size)
        {
            var root = CreateRect("EnterButton", parent, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), position, size);

            var glow = CreateImage("Glow", root, assets.Glow, new Color(0.78f, 0.12f, 0.18f, 0.5f), Image.Type.Simple, false);
            var glowRect = glow.rectTransform;
            glowRect.anchorMin = new Vector2(0.5f, 0.5f);
            glowRect.anchorMax = new Vector2(0.5f, 0.5f);
            glowRect.pivot = new Vector2(0.5f, 0.5f);
            glowRect.anchoredPosition = Vector2.zero;
            glowRect.sizeDelta = size + new Vector2(120f, 80f);

            var background = CreateImage("Background", root, assets.Rounded, Red, Image.Type.Sliced, true);
            Stretch(background.rectTransform);

            var highlight = CreateImage("TopHighlight", root, assets.White, new Color(1f, 0.85f, 0.82f, 0.18f), Image.Type.Simple, false);
            var highlightRect = highlight.rectTransform;
            highlightRect.anchorMin = new Vector2(0f, 1f);
            highlightRect.anchorMax = new Vector2(1f, 1f);
            highlightRect.pivot = new Vector2(0.5f, 1f);
            highlightRect.anchoredPosition = Vector2.zero;
            highlightRect.sizeDelta = new Vector2(-2f, 1f);

            var label = CreateText(
                "Label", root, "JUGAR", assets.BodyBold, 20f, Cream,
                TextAlignmentOptions.MidlineLeft, false, 2f);
            var labelRect = label.rectTransform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(24f, 0f);
            labelRect.offsetMax = new Vector2(-58f, 0f);

            var arrow = CreateImage("Arrow", root, assets.Arrow, new Color(1f, 0.95f, 0.92f, 0.9f), Image.Type.Simple, false);
            var arrowRect = arrow.rectTransform;
            arrowRect.anchorMin = new Vector2(1f, 0.5f);
            arrowRect.anchorMax = new Vector2(1f, 0.5f);
            arrowRect.pivot = new Vector2(1f, 0.5f);
            arrowRect.anchoredPosition = new Vector2(-22f, 0f);
            arrowRect.sizeDelta = new Vector2(13f, 13f);

            var spinner = CreateImage("Spinner", root, assets.Spinner, Cream, Image.Type.Simple, false);
            var spinnerRect = spinner.rectTransform;
            spinnerRect.anchorMin = new Vector2(1f, 0.5f);
            spinnerRect.anchorMax = new Vector2(1f, 0.5f);
            spinnerRect.pivot = new Vector2(1f, 0.5f);
            spinnerRect.anchoredPosition = new Vector2(-46f, 0f);
            spinnerRect.sizeDelta = new Vector2(16f, 16f);
            spinner.gameObject.SetActive(false);

            var spin = spinner.gameObject.AddComponent<Spinner>();
            SerializedField.Set(spin, "degreesPerSecond", -220f);

            var button = root.gameObject.AddComponent<Button>();
            button.targetGraphic = background;
            button.transition = Selectable.Transition.ColorTint;
            button.colors = new ColorBlock
            {
                normalColor = Color.white,
                highlightedColor = new Color(1f, 0.86f, 0.86f, 1f),
                pressedColor = new Color(0.82f, 0.78f, 0.78f, 1f),
                selectedColor = Color.white,
                disabledColor = new Color(0.55f, 0.55f, 0.55f, 0.5f),
                colorMultiplier = 1f,
                fadeDuration = 0.08f
            };

            var hover = root.gameObject.AddComponent<HoverLift>();
            SerializedField.Set(hover, "target", root);
            SerializedField.Set(hover, "lift", 2f);
            SerializedField.Set(hover, "duration", 0.12f);

            return new ButtonRefs { Root = root, Button = button, Label = label, Spinner = spinner.gameObject };
        }

        private static ButtonRefs CreateSecondaryButton(RectTransform parent, LoginAssets assets, Vector2 position, Vector2 size)
        {
            var root = CreateRect("ExitButton", parent, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), position, size);

            CreateBorder(root, assets, LineColor);

            var background = CreateImage("Background", root, assets.Rounded, new Color(0.04f, 0.075f, 0.12f, 0.82f), Image.Type.Sliced, true);
            Stretch(background.rectTransform);

            var label = CreateText(
                "Label", root, "SALIR", assets.BodyBold, 14f, Ice,
                TextAlignmentOptions.MidlineLeft, false, 4f);
            var labelRect = label.rectTransform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(24f, 0f);
            labelRect.offsetMax = new Vector2(-24f, 0f);

            var button = root.gameObject.AddComponent<Button>();
            button.targetGraphic = background;
            button.transition = Selectable.Transition.ColorTint;
            button.colors = new ColorBlock
            {
                normalColor = Color.white,
                highlightedColor = new Color(0.82f, 0.9f, 0.98f, 1f),
                pressedColor = new Color(0.62f, 0.72f, 0.82f, 1f),
                selectedColor = Color.white,
                disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.45f),
                colorMultiplier = 1f,
                fadeDuration = 0.08f
            };

            var hover = root.gameObject.AddComponent<HoverLift>();
            SerializedField.Set(hover, "target", root);
            SerializedField.Set(hover, "lift", 1f);
            SerializedField.Set(hover, "duration", 0.12f);

            return new ButtonRefs { Root = root, Button = button, Label = label, Spinner = null };
        }

        private static RectTransform CreateBottomRight(RectTransform parent, LoginAssets assets)
        {
            var root = CreateRect("BottomRight", parent, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-96f, 48f), new Vector2(340f, 40f));
            root.gameObject.AddComponent<CanvasGroup>();

            var label = CreateText(
                "HerdLabel", root, "4 RENOS EN LÍNEA", assets.BodyBold, 10f, Ice,
                TextAlignmentOptions.BottomRight, false, 3f);
            var labelRect = label.rectTransform;
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(1f, 0f);
            labelRect.pivot = new Vector2(1f, 0f);
            labelRect.anchoredPosition = new Vector2(0f, 18f);
            labelRect.sizeDelta = new Vector2(0f, 14f);

            for (var i = 0; i < 4; i++)
            {
                var dot = CreateImage("HerdDot_" + i, root, assets.Dot, i == 0 ? Red : Cream, Image.Type.Simple, false);
                SetBottomRight(dot.rectTransform, 6f + i * 14f, 0f, 7f, 7f);
            }

            return root;
        }

        private static CanvasGroup CreateFadeOverlay(RectTransform parent, LoginAssets assets)
        {
            var fade = CreateImage("FadeOverlay", parent, assets.White, Color.black, Image.Type.Simple, true);
            Stretch(fade.rectTransform);

            var group = fade.gameObject.AddComponent<CanvasGroup>();
            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;
            return group;
        }

        private static void CreateEventSystem(Transform parent)
        {
            var go = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
            go.transform.SetParent(parent, false);

            var module = go.GetComponent<InputSystemUIInputModule>();
            module.AssignDefaultActions();
        }

        private static LoginIntroAnimator.IntroTarget MakeIntroTarget(Component target, float delay, float duration, float offsetY)
        {
            var rect = target as RectTransform;
            var group = target != null ? target.GetComponent<CanvasGroup>() : null;

            return new LoginIntroAnimator.IntroTarget
            {
                group = group,
                rect = rect,
                delay = delay,
                duration = duration,
                offset = new Vector2(0f, offsetY)
            };
        }

        private static RectTransform CreateRect(
            string name,
            Transform parent,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 anchoredPosition,
            Vector2 sizeDelta)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = (RectTransform)go.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
            return rect;
        }

        private static Image CreateImage(
            string name,
            Transform parent,
            Sprite sprite,
            Color color,
            Image.Type type,
            bool raycast)
        {
            var rect = CreateRect(name, parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(10f, 10f));

            var image = rect.gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.type = type;
            image.preserveAspect = false;
            image.raycastTarget = raycast;

            return image;
        }

        private static TextMeshProUGUI CreateText(
            string name,
            Transform parent,
            string content,
            TMP_FontAsset font,
            float fontSize,
            Color color,
            TextAlignmentOptions alignment,
            bool wrap,
            float characterSpacing,
            float lineSpacing = 0f)
        {
            var rect = CreateRect(name, parent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(100f, 20f));

            var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.font = font;
            text.text = content;
            text.fontSize = fontSize;
            text.color = color;
            text.alignment = alignment;
            text.textWrappingMode = wrap ? TextWrappingModes.Normal : TextWrappingModes.NoWrap;
            text.characterSpacing = characterSpacing;
            text.lineSpacing = lineSpacing;
            text.overflowMode = TextOverflowModes.Overflow;
            text.raycastTarget = false;
            text.richText = false;

            return text;
        }

        private static void SetTopLeft(RectTransform rect, float x, float y, float width, float height)
        {
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(x, -y);
            rect.sizeDelta = new Vector2(width, height);
        }

        private static void SetTopRight(RectTransform rect, float right, float y, float width, float height)
        {
            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-right, -y);
            rect.sizeDelta = new Vector2(width, height);
        }

        private static void SetBottomLeft(RectTransform rect, float x, float y, float width, float height)
        {
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(width, height);
        }

        private static void SetBottomRight(RectTransform rect, float right, float y, float width, float height)
        {
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.anchoredPosition = new Vector2(-right, y);
            rect.sizeDelta = new Vector2(width, height);
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = Vector2.zero;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static Image CreateBorder(RectTransform parent, LoginAssets assets, Color color)
        {
            var border = CreateImage("Border", parent, assets.Rounded, color, Image.Type.Sliced, false);
            var rect = border.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(-1f, -1f);
            rect.offsetMax = new Vector2(1f, 1f);
            border.transform.SetAsFirstSibling();
            return border;
        }

        private static Color Hex(string value)
        {
            return ColorUtility.TryParseHtmlString(value, out var color) ? color : Color.magenta;
        }
    }
}
