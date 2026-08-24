using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ARLudo.Core;

namespace ARLudo.UI
{
    public class MatchSetupController : MonoBehaviour
    {
        public TMP_Text humanCountText;
        public TMP_Text aiCountText;
        public Button humanMinusBtn;
        public Button humanPlusBtn;
        public Button aiMinusBtn;
        public Button aiPlusBtn;
        public TMP_Dropdown presetDropdown;
        public Button customizeRulesBtn;
        public Button startGameBtn;

        private int humanCount = 1;
        private int aiCount = 3;

        void Start()
        {
            humanMinusBtn.onClick.AddListener(() => ChangeHuman(-1));
            humanPlusBtn.onClick.AddListener(() => ChangeHuman(1));
            aiMinusBtn.onClick.AddListener(() => ChangeAI(-1));
            aiPlusBtn.onClick.AddListener(() => ChangeAI(1));
            
            if (startGameBtn != null)
            {
                startGameBtn.onClick.AddListener(() => {
                    GameSettingsData.HumanPlayers = GetHumanCount();
                    GameSettingsData.AIPlayers = GetAICount();
                    UnityEngine.SceneManagement.SceneManager.LoadScene("LudoGame");
                });
            }
            
            UpdateDisplay();
        }

        void ChangeHuman(int delta)
        {
            humanCount = Mathf.Clamp(humanCount + delta, 0, 4);
            aiCount = Mathf.Clamp(aiCount, 0, 4 - humanCount);
            if (humanCount + aiCount < 2) aiCount = 2 - humanCount;
            UpdateDisplay();
        }

        void ChangeAI(int delta)
        {
            aiCount = Mathf.Clamp(aiCount + delta, 0, 4);
            humanCount = Mathf.Clamp(humanCount, 0, 4 - aiCount);
            if (humanCount + aiCount < 2) humanCount = 2 - aiCount;
            UpdateDisplay();
        }

        void UpdateDisplay()
        {
            humanCountText.text = humanCount.ToString();
            aiCountText.text = aiCount.ToString();
        }

        public int GetHumanCount() => humanCount;
        public int GetAICount() => aiCount;
        public int GetSelectedPreset() => presetDropdown.value;
    }
}