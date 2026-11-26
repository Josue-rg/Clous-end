using ClousEnd.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ClousEnd.EditorTools
{
    /// <summary>
    /// Construye el HUD de combate (casco, renos, arma, jefe, objetivo, rescate
    /// y avisos) sobre el Canvas del juego y le añade el componente GameHud.
    /// </summary>
    internal static class GameHudBuilder
    {
        private static readonly Color PanelColor = new Color(0.015f, 0.03f, 0.05f, 0.78f);
        private static readonly Color BarBack = new Color(0.06f, 0.09f, 0.12f, 0.92f);
        private static readonly Color HullColor = new Color(0.55f, 1f, 0.85f);
        private static readonly Color WarmColor = new Color(1f, 0.78f, 0.3f);
        private static readonly Color TextColor = new Color(0.78f, 0.88f, 0.96f);

        public static GameHud Build(RectTransform canvas, MapAssets assets)
        {
            var root = new GameObject("CombatHud", typeof(RectTransform));
            var rootRect = root.GetComponent<RectTransform>();
            rootRect.SetParent(canvas, false);
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            var objective = CreateText("Objective", rootRect, "OBJETIVO", assets.BodyBoldFont, 17f, WarmColor,
                TextAlignmentOptions.TopLeft, 3f);
            Place(objective.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(30f, -88f), new Vector2(860f, 26f));

            var reindeerLabels = new TMP_Text[4];
            BuildReindeerPanel(rootRect, assets, reindeerLabels);

            var hullFill = BuildHullBar(rootRect, assets, out var hullLabel);

            var slotLabels = new TMP_Text[4];
            BuildWeaponPanel(rootRect, assets, out var weaponLabel, out var ammoLabel, out var heatRoot,
                out var heatFill, slotLabels);

            var bossRoot = BuildBossBar(rootRect, assets, out var bossFill, out var bossLabel);

            var toastGroup = BuildToast(rootRect, assets, out var toastLabel);
            var rescueRoot = BuildRescue(rootRect, assets, out var rescueLabel);

            var hud = root.AddComponent<GameHud>();
            SerializedField.Set(hud, "objectiveLabel", objective);
            SerializedField.SetArray(hud, "reindeerLabels", reindeerLabels);
            SerializedField.SetArray(hud, "slotLabels", slotLabels);
            SerializedField.Set(hud, "weaponLabel", weaponLabel);
            SerializedField.Set(hud, "ammoLabel", ammoLabel);
            SerializedField.Set(hud, "hullFill", hullFill);
            SerializedField.Set(hud, "hullLabel", hullLabel);
            SerializedField.Set(hud, "heatRoot", heatRoot);
            SerializedField.Set(hud, "heatFill", heatFill);
            SerializedField.Set(hud, "bossRoot", bossRoot);
            SerializedField.Set(hud, "bossFill", bossFill);
            SerializedField.Set(hud, "bossLabel", bossLabel);
            SerializedField.Set(hud, "rescueRoot", rescueRoot);
            SerializedField.Set(hud, "rescueLabel", rescueLabel);
            SerializedField.Set(hud, "toastGroup", toastGroup);
            SerializedField.Set(hud, "toastLabel", toastLabel);

            return hud;
        }

        private static void BuildReindeerPanel(RectTransform parent, MapAssets assets, TMP_Text[] labels)
        {
            var panel = CreatePanel("ReindeerPanel", parent, assets.Panel, PanelColor);
            Place(panel, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f),
                new Vector2(30f, -122f), new Vector2(330f, 150f));

            for (var i = 0; i < labels.Length; i++)
            {
                var label = CreateText("Reindeer" + i, panel, "RENO", assets.BodyBoldFont, 15f,
                    TextColor, TextAlignmentOptions.Left, 2f);
                Place(label.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f),
                    new Vector2(18f, -14f - i * 33f), new Vector2(-36f, 24f));
                labels[i] = label;
            }
        }

        private static Image BuildHullBar(RectTransform parent, MapAssets assets, out TMP_Text label)
        {
            var root = CreatePanel("HullPanel", parent, assets.Panel, PanelColor);
            Place(root, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f),
                new Vector2(26f, 248f), new Vector2(380f, 66f));

            label = CreateText("HullLabel", root, "CASCO  100", assets.BodyBoldFont, 16f,
                HullColor, TextAlignmentOptions.Left, 2f);
            Place(label.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f),
                new Vector2(16f, -10f), new Vector2(-32f, 24f));

            var background = CreateImage("HullBarBack", root, assets.Panel, BarBack);
            Place(background.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f),
                new Vector2(16f, 12f), new Vector2(-32f, 20f));

            var fill = CreateImage("HullBarFill", background.rectTransform, assets.Panel, HullColor);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = 0;
            fill.fillAmount = 1f;
            Stretch(fill.rectTransform);

            return fill;
        }

        private static void BuildWeaponPanel(
            RectTransform parent,
            MapAssets assets,
            out TMP_Text weaponLabel,
            out TMP_Text ammoLabel,
            out GameObject heatRoot,
            out Image heatFill,
            TMP_Text[] slotLabels)
        {
            var panel = CreatePanel("WeaponPanel", parent, assets.Panel, PanelColor);
            Place(panel, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(1f, 0f),
                new Vector2(-26f, 26f), new Vector2(390f, 178f));

            weaponLabel = CreateText("WeaponLabel", panel, "CARBÓN", assets.DisplayFont, 28f,
                TextColor, TextAlignmentOptions.Right, 4f);
            Place(weaponLabel.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(-18f, -14f), new Vector2(-36f, 32f));

            ammoLabel = CreateText("AmmoLabel", panel, "∞", assets.BodyBoldFont, 20f,
                WarmColor, TextAlignmentOptions.Right, 2f);
            Place(ammoLabel.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 1f),
                new Vector2(-18f, -48f), new Vector2(-36f, 26f));

            var heat = CreateImage("HeatBack", panel, assets.Panel, BarBack);
            Place(heat.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f),
                new Vector2(18f, 48f), new Vector2(-36f, 10f));
            heatRoot = heat.gameObject;

            heatFill = CreateImage("HeatFill", heat.rectTransform, assets.Panel, new Color(1f, 0.72f, 0.25f));
            heatFill.type = Image.Type.Filled;
            heatFill.fillMethod = Image.FillMethod.Horizontal;
            heatFill.fillOrigin = 0;
            heatFill.fillAmount = 0f;
            Stretch(heatFill.rectTransform);

            for (var i = 0; i < slotLabels.Length; i++)
            {
                var label = CreateText("Slot" + i, panel, (i + 1) + "  —", assets.BodyFont, 12f,
                    new Color(0.68f, 0.8f, 0.9f), TextAlignmentOptions.Left, 1f);
                Place(label.rectTransform, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f),
                    new Vector2(18f + i * 92f, 14f), new Vector2(86f, 22f));
                slotLabels[i] = label;
            }
        }

        private static GameObject BuildBossBar(
            RectTransform parent,
            MapAssets assets,
            out Image fill,
            out TMP_Text label)
        {
            var root = CreatePanel("BossPanel", parent, assets.Panel, new Color(0.02f, 0.01f, 0.03f, 0.85f));
            Place(root, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -128f), new Vector2(900f, 54f));

            label = CreateText("BossLabel", root, "NICOLÁS CERO", assets.DisplayFont, 24f,
                new Color(1f, 0.55f, 0.62f), TextAlignmentOptions.Center, 5f);
            Place(label.rectTransform, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, 1f),
                new Vector2(16f, -4f), new Vector2(-32f, 28f));

            var back = CreateImage("BossBack", root, assets.Panel, BarBack);
            Place(back.rectTransform, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f),
                new Vector2(16f, 8f), new Vector2(-32f, 12f));

            fill = CreateImage("BossFill", back.rectTransform, assets.Panel, new Color(0.85f, 0.15f, 0.28f));
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillOrigin = 0;
            fill.fillAmount = 1f;
            Stretch(fill.rectTransform);

            root.gameObject.SetActive(false);
            return root.gameObject;
        }

        private static CanvasGroup BuildToast(RectTransform parent, MapAssets assets, out TMP_Text label)
        {
            var root = new GameObject("Toast", typeof(RectTransform));
            var rect = root.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            Place(rect, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f),
                new Vector2(0f, -200f), new Vector2(1000f, 44f));

            label = CreateText("ToastLabel", rect, "AVISO", assets.DisplayFont, 24f,
                new Color(0.86f, 0.97f, 1f), TextAlignmentOptions.Center, 4f);
            Stretch(label.rectTransform);

            var group = root.AddComponent<CanvasGroup>();
            group.alpha = 0f;
            group.blocksRaycasts = false;
            group.interactable = false;
            return group;
        }

        private static GameObject BuildRescue(RectTransform parent, MapAssets assets, out TMP_Text label)
        {
            var root = new GameObject("RescueWarning", typeof(RectTransform));
            var rect = root.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            Place(rect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f),
                new Vector2(0f, 170f), new Vector2(900f, 48f));

            label = CreateText("RescueLabel", rect, "¡RESCATA AL RENO!", assets.DisplayFont, 30f,
                new Color(1f, 0.42f, 0.42f), TextAlignmentOptions.Center, 4f);
            Stretch(label.rectTransform);

            root.gameObject.SetActive(false);
            return root.gameObject;
        }

        private static RectTransform CreatePanel(string name, RectTransform parent, Sprite sprite, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.raycastTarget = false;

            return rect;
        }

        private static Image CreateImage(string name, RectTransform parent, Sprite sprite, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);

            var image = go.AddComponent<Image>();
            image.sprite = sprite;
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static TextMeshProUGUI CreateText(
            string name,
            RectTransform parent,
            string content,
            TMP_FontAsset font,
            float size,
            Color color,
            TextAlignmentOptions alignment,
            float spacing)
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
            text.characterSpacing = spacing;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.overflowMode = TextOverflowModes.Overflow;
            text.raycastTarget = false;
            text.richText = false;

            return text;
        }

        private static void Place(
            RectTransform rect,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 position,
            Vector2 size)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }
    }
}
