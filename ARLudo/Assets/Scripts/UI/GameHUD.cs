using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ARLudo.Core;
using ARLudo.Visuals;

namespace ARLudo.UI
{
    public class GameHUD : MonoBehaviour
    {
        public TMP_Text turnText;
        public TMP_Text diceText;
        public TMP_Text instructionText;
        public Image turnColorIndicator;
        public Button rollButton;
        public Image timerBar;
        public GameObject gameOverPanel;
        public TMP_Text winnerText;
        public PlayerPanel[] playerPanels;

        private float turnTimeLimit;
        private float turnTimer;
        private bool timerRunning;
        private System.Action onTimerExpired;
        private bool currentPlayerIsAI;

        void Update()
        {
            if (!timerRunning) return;
            turnTimer -= Time.deltaTime;
            if (timerBar != null)
                timerBar.fillAmount = Mathf.Clamp01(turnTimer / turnTimeLimit);
            if (turnTimer <= 0)
            {
                timerRunning = false;
                onTimerExpired?.Invoke();
            }
        }

        public void SetupRollButton(System.Action onRoll)
        {
            rollButton.onClick.RemoveAllListeners();
            rollButton.onClick.AddListener(() => onRoll());
        }

        public void ShowRollButton(bool show)
        {
            rollButton.gameObject.SetActive(show);
        }

        public void UpdateTurn(LudoPlayer player)
        {
            currentPlayerIsAI = player.IsAI;
            turnText.text = player.DisplayName;
            turnColorIndicator.color = BoardReference.GetPlayerColor(player.Color);
            instructionText.text = player.IsAI ? "AI thinking..." : "Tap Roll!";
            diceText.text = "";
            ShowRollButton(!player.IsAI);
            HighlightPlayerPanel(player.Color);

            // Reset timer bar for AI turns so stale human-turn progress doesn't linger
            if (player.IsAI)
                ResetTimerBar();
        }

        public void UpdateDice(int value)
        {
            diceText.text = value.ToString();
            ShowRollButton(false);
        }

        public void ShowChoosePawn()
        {
            instructionText.text = currentPlayerIsAI ? "AI thinking..." : "Tap a glowing pawn";
        }

        public void ShowNoMoves()
        {
            instructionText.text = currentPlayerIsAI ? "AI thinking..." : "No valid moves";
        }

        public void ShowWinner(LudoPlayer player)
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                winnerText.text = player.DisplayName + "\nWINS!";
                winnerText.color = BoardReference.GetPlayerColor(player.Color);
            }
            ShowRollButton(false);
            ResetTimerBar();
        }

        public void StartTurnTimer(float duration, System.Action onExpired)
        {
            turnTimeLimit = duration;
            turnTimer = duration;
            timerRunning = true;
            onTimerExpired = onExpired;
            if (timerBar != null)
                timerBar.fillAmount = 1f;
        }

        public void StopTimer()
        {
            timerRunning = false;
        }

        public void ResetTimerBar()
        {
            timerRunning = false;
            if (timerBar != null)
                timerBar.fillAmount = 0f;
        }

        public void InitPlayerPanels(System.Collections.Generic.List<LudoPlayer> players)
        {
            for (int i = 0; i < playerPanels.Length; i++)
            {
                if (i < players.Count)
                {
                    playerPanels[i].gameObject.SetActive(true);
                    playerPanels[i].Setup(players[i]);
                }
                else
                {
                    playerPanels[i].gameObject.SetActive(false);
                }
            }
        }

        public void UpdatePlayerPanel(LudoPlayer player)
        {
            foreach (var panel in playerPanels)
            {
                if (panel.gameObject.activeSelf && panel.PlayerColor == player.Color)
                    panel.UpdateInfo(player);
            }
        }

        public void HighlightPlayerPanel(PlayerColor color)
        {
            foreach (var panel in playerPanels)
            {
                if (panel.gameObject.activeSelf)
                    panel.SetActive(panel.PlayerColor == color);
            }
        }
    }
}