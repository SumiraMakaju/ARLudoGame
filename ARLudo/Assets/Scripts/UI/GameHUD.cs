using UnityEngine;
using TMPro;
using UnityEngine.UI;
using ARLudo.Core;

namespace ARLudo.UI
{
    public class GameHUD : MonoBehaviour
    {
        public TMP_Text turnText;
        public TMP_Text diceText;
        public TMP_Text instructionText;
        public Image turnColorIndicator;

        public void UpdateTurn(LudoPlayer player)
        {
            turnText.text = player.DisplayName;
            turnColorIndicator.color = Visuals.BoardReference.GetPlayerColor(player.Color);
            instructionText.text = player.IsAI ? "AI thinking..." : "Tap to roll";
            diceText.text = "";
        }

        public void UpdateDice(int value)
        {
            diceText.text = value.ToString();
        }

        public void ShowChoosePawn()
        {
            instructionText.text = "Tap a glowing pawn";
        }

        public void ShowNoMoves()
        {
            instructionText.text = "No valid moves, skipping...";
        }

        public void ShowWinner(LudoPlayer player)
        {
            turnText.text = player.DisplayName + " WINS!";
            instructionText.text = "";
            diceText.text = "";
        }
    }
}