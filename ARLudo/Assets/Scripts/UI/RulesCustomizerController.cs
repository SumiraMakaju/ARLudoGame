using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ARLudo.Core;

namespace ARLudo.UI
{
    public class RulesCustomizerController : MonoBehaviour
    {
        public TMP_Dropdown openingRollDropdown;
        public Toggle bonusTurnOnSixToggle;
        public TMP_Dropdown consecutiveSixDropdown;
        public Toggle bonusTurnOnCaptureToggle;
        public Toggle bonusTurnOnGoalToggle;
        public TMP_Dropdown safeZoneDropdown;
        public Toggle captureRequiredToggle;
        public TMP_Dropdown blockadeDropdown;
        public TMP_Dropdown winConditionDropdown;
        public Slider turnTimerSlider;
        public TMP_Text turnTimerValueText;
        public Button applyBtn;
        public Button backBtn;

        private LudoRules targetRules;

        void Start()
        {
            turnTimerSlider.onValueChanged.AddListener(v =>
                turnTimerValueText.text = v > 0 ? $"{v:0}s" : "OFF");

            applyBtn.onClick.AddListener(ApplyToRules);
        }

        void OnEnable()
        {
            if (GameSettingsData.SelectedRules == null)
            {
                GameSettingsData.SelectedRules = ScriptableObject.CreateInstance<LudoRules>();
            }
            Setup(GameSettingsData.SelectedRules);
        }

        public void Setup(LudoRules rules)
        {
            targetRules = rules;
            LoadFromRules();
        }

        void LoadFromRules()
        {
            if (targetRules == null) return;
            openingRollDropdown.value = (int)targetRules.openingRoll;
            bonusTurnOnSixToggle.isOn = targetRules.bonusTurnOnSix;
            consecutiveSixDropdown.value = (int)targetRules.consecutiveSixRule;
            bonusTurnOnCaptureToggle.isOn = targetRules.bonusTurnOnCapture;
            bonusTurnOnGoalToggle.isOn = targetRules.bonusTurnOnGoal;
            safeZoneDropdown.value = (int)targetRules.safeZoneRule;
            captureRequiredToggle.isOn = targetRules.captureRequiredBeforeHome;
            blockadeDropdown.value = (int)targetRules.blockadeRule;
            winConditionDropdown.value = (int)targetRules.winCondition;
            turnTimerSlider.value = targetRules.turnTimeLimit;
            turnTimerValueText.text = targetRules.turnTimeLimit > 0
                ? $"{targetRules.turnTimeLimit:0}s" : "OFF";
        }

        void ApplyToRules()
        {
            if (targetRules == null) return;
            targetRules.openingRoll = (OpeningRollRule)openingRollDropdown.value;
            targetRules.bonusTurnOnSix = bonusTurnOnSixToggle.isOn;
            targetRules.consecutiveSixRule = (ConsecutiveSixRule)consecutiveSixDropdown.value;
            targetRules.bonusTurnOnCapture = bonusTurnOnCaptureToggle.isOn;
            targetRules.bonusTurnOnGoal = bonusTurnOnGoalToggle.isOn;
            targetRules.safeZoneRule = (SafeZoneRule)safeZoneDropdown.value;
            targetRules.captureRequiredBeforeHome = captureRequiredToggle.isOn;
            targetRules.blockadeRule = (BlockadeRule)blockadeDropdown.value;
            targetRules.winCondition = (WinCondition)winConditionDropdown.value;
            targetRules.turnTimeLimit = turnTimerSlider.value;
        }
    }
}