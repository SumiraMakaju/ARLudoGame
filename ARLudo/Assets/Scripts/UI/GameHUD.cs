using UnityEngine;
using UnityEngine.UI;
using ARLudo.Core;

namespace ARLudo.UI
{
    public class GameHUD : MonoBehaviour
    {
        public Text turnText;
        public Text diceText;
        public Text instructionText;
        public Image turnColorIndicator;

        public void UpdateTurn(LudoPlayer player)
        {
            turnText.text = player.DisplayName;
            turnColorIndicator.color = Visuals.BoardReference.GetPlayerColor(player.Color);
            instructionText.text = player.IsAI ? "AI thinking..." : "Press SPACE to roll";
            diceText.text = "";
        }

        public void UpdateDice(int value)
        {
            diceText.text = value.ToString();
        }

        public void ShowChoosePawn()
        {
            instructionText.text = "Click a glowing pawn";
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