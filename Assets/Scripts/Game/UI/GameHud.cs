using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ClousEnd.Game
{
    /// <summary>
    /// HUD de combate: casco, renos, arma/munición, jefe, objetivo, rescate y
    /// avisos. Solo lee los sistemas; no crea objetos ni modifica el juego.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GameHud : MonoBehaviour
    {
        [System.Serializable]
        public struct ObjectiveBeat
        {
            public float z;
            public string text;
        }

        [Header("Sistemas")]
        [SerializeField] private Transform player;
        [SerializeField] private SleighHealth health;
        [SerializeField] private ReindeerSquad squad;
        [SerializeField] private WeaponController weapons;
        [SerializeField] private BossController boss;

        [Header("HUD")]
        [SerializeField] private TMP_Text objectiveLabel;
        [SerializeField] private TMP_Text[] reindeerLabels = new TMP_Text[4];
        [SerializeField] private TMP_Text[] slotLabels = new TMP_Text[4];
        [SerializeField] private TMP_Text weaponLabel;
        [SerializeField] private TMP_Text ammoLabel;
        [SerializeField] private Image hullFill;
        [SerializeField] private TMP_Text hullLabel;
        [SerializeField] private GameObject heatRoot;
        [SerializeField] private Image heatFill;
        [SerializeField] private GameObject bossRoot;
        [SerializeField] private Image bossFill;
        [SerializeField] private TMP_Text bossLabel;
        [SerializeField] private GameObject rescueRoot;
        [SerializeField] private TMP_Text rescueLabel;
        [SerializeField] private CanvasGroup toastGroup;
        [SerializeField] private TMP_Text toastLabel;

        [Header("Objetivos por tramo")]
        [SerializeField] private ObjectiveBeat[] beats = new ObjectiveBeat[0];

        private float toastHideAt;
        private float toastFadeStart;

        private void Awake()
        {
            if (bossRoot != null) bossRoot.SetActive(false);
            if (rescueRoot != null) rescueRoot.SetActive(false);
            if (toastGroup != null) toastGroup.alpha = 0f;
        }

        private void Update()
        {
            UpdateHull();
            UpdateReindeer();
            UpdateWeapons();
            UpdateBoss();
            UpdateObjective();
            UpdateRescue();
            UpdateToast();
        }

        public void SetBeats(ObjectiveBeat[] newBeats)
        {
            beats = newBeats;
        }

        public void ShowToast(string message, float duration = 2.4f)
        {
            if (toastLabel == null || toastGroup == null)
                return;

            toastLabel.text = message;
            toastGroup.alpha = 1f;
            toastHideAt = Time.unscaledTime + Mathf.Max(0.5f, duration);
            toastFadeStart = toastHideAt;
        }

        private void UpdateHull()
        {
            if (health == null)
                return;

            if (hullFill != null)
                hullFill.fillAmount = health.Normalized;

            if (hullLabel != null)
                hullLabel.text = "CASCO  " + Mathf.CeilToInt(health.Current);
        }

        private void UpdateReindeer()
        {
            if (squad == null || reindeerLabels == null)
                return;

            for (var i = 0; i < reindeerLabels.Length; i++)
            {
                var label = reindeerLabels[i];

                if (label == null)
                    continue;

                if (i >= squad.Count)
                {
                    label.text = string.Empty;
                    continue;
                }

                var state = squad.GetState(i);
                label.text = squad.GetName(i) + "  ·  " + StateLabel(state);

                label.color = state switch
                {
                    ReindeerSquad.ReindeerState.Lost => new Color(1f, 0.36f, 0.36f),
                    ReindeerSquad.ReindeerState.Grabbed => new Color(1f, 0.78f, 0.28f),
                    _ => new Color(0.72f, 0.92f, 1f)
                };
            }
        }

        private static string StateLabel(ReindeerSquad.ReindeerState state)
        {
            return state switch
            {
                ReindeerSquad.ReindeerState.Lost => "PERDIDO",
                ReindeerSquad.ReindeerState.Grabbed => "¡AGARRIADO!",
                _ => "SANO"
            };
        }

        private void UpdateWeapons()
        {
            if (weapons == null)
                return;

            var definition = weapons.CurrentDefinition;

            if (weaponLabel != null && definition != null)
                weaponLabel.text = definition.displayName;

            if (ammoLabel != null && definition != null)
            {
                if (weapons.IsReloading)
                {
                    ammoLabel.text = "RECARGANDO…";
                }
                else if (definition.usesHeat)
                {
                    ammoLabel.text = weapons.IsOverheated
                        ? "SOBRECALENTADO"
                        : "CALOR  " + Mathf.RoundToInt(weapons.Heat01 * 100f) + "%";
                }
                else if (definition.magazine > 0)
                {
                    ammoLabel.text = weapons.CurrentAmmo + " / " + definition.magazine;
                }
                else
                {
                    ammoLabel.text = "∞";
                }
            }

            if (heatRoot != null && definition != null)
                heatRoot.SetActive(definition.usesHeat);

            if (heatFill != null && definition != null && definition.usesHeat)
            {
                heatFill.fillAmount = weapons.Heat01;
                heatFill.color = weapons.IsOverheated
                    ? new Color(1f, 0.28f, 0.2f)
                    : new Color(1f, 0.72f, 0.25f);
            }

            if (slotLabels == null)
                return;

            for (var i = 0; i < slotLabels.Length; i++)
            {
                var label = slotLabels[i];

                if (label == null)
                    continue;

                var unlocked = weapons.IsUnlocked(i);
                label.text = (i + 1) + "  " + weapons.GetWeaponName(i);
                label.color = !unlocked
                    ? new Color(0.38f, 0.44f, 0.5f)
                    : i == weapons.CurrentIndex
                        ? new Color(1f, 0.92f, 0.7f)
                        : new Color(0.68f, 0.8f, 0.9f);
            }
        }

        private void UpdateBoss()
        {
            if (boss == null)
                return;

            var engaged = boss.IsEngaged && !boss.IsDefeated;

            if (bossRoot != null && bossRoot.activeSelf != engaged)
                bossRoot.SetActive(engaged);

            if (!engaged)
                return;

            if (bossFill != null)
                bossFill.fillAmount = boss.Health01;

            if (bossLabel != null)
                bossLabel.text = boss.BossName + "   " + Mathf.CeilToInt(boss.Health01 * 100f) + "%";
        }

        private void UpdateObjective()
        {
            if (objectiveLabel == null || player == null)
                return;

            if (boss != null && boss.IsDefeated)
            {
                objectiveLabel.text = "OBJETIVO  ·  VUELA AL CORAZÓN DE ESTRELLA";
                return;
            }

            if (boss != null && boss.IsEngaged)
            {
                objectiveLabel.text = "OBJETIVO  ·  DERROTA A NICOLÁS CERO";
                return;
            }

            var text = "OBJETIVO  ·  LLEGA AL REFUGIO AURORA";

            if (beats != null)
            {
                for (var i = 0; i < beats.Length; i++)
                {
                    if (player.position.z >= beats[i].z)
                        text = "OBJETIVO  ·  " + beats[i].text;
                }
            }

            objectiveLabel.text = text;
        }

        private void UpdateRescue()
        {
            if (rescueRoot == null || rescueLabel == null || squad == null)
                return;

            var grabbed = squad.GrabbedIndex;
            var show = grabbed >= 0;

            if (rescueRoot.activeSelf != show)
                rescueRoot.SetActive(show);

            if (!show)
                return;

            rescueLabel.text = "¡RESCATA A " + squad.GetName(grabbed) + "!   " +
                               Mathf.CeilToInt(squad.GrabTimeRemaining) + " s";
        }

        private void UpdateToast()
        {
            if (toastGroup == null || toastGroup.alpha <= 0f)
                return;

            if (Time.unscaledTime <= toastHideAt)
                return;

            var fade = 1f - Mathf.Clamp01((Time.unscaledTime - toastFadeStart) / 0.55f);
            toastGroup.alpha = fade;
        }
    }
}
