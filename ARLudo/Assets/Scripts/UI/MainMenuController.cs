using UnityEngine;
using UnityEngine.SceneManagement;

namespace ARLudo.UI
{
    public class MainMenuController : MonoBehaviour
    {
        public GameObject mainPanel;
        public GameObject matchSetupPanel;
        public GameObject rulesPanel;
        public GameObject howToPlayPanel;

        void Start()
        {
            ShowMain();
        }

        public void ShowMain()
        {
            mainPanel.SetActive(true);
            matchSetupPanel.SetActive(false);
            rulesPanel.SetActive(false);
            howToPlayPanel.SetActive(false);
        }

        public void ShowMatchSetup()
        {
            mainPanel.SetActive(false);
            matchSetupPanel.SetActive(true);
        }

        public void ShowRules()
        {
            matchSetupPanel.SetActive(false);
            rulesPanel.SetActive(true);
        }

        public void ShowHowToPlay()
        {
            mainPanel.SetActive(false);
            howToPlayPanel.SetActive(true);
        }

        public void StartGame()
        {
            SceneManager.LoadScene("LudoGame");
        }
    }
}