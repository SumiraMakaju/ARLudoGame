using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ARLudo.Core;
using ARLudo.Visuals;

namespace ARLudo.UI
{
    public class PlayerPanel : MonoBehaviour
    {
        public Image backgroundImage;
        public TMP_Text nameText;
        public TMP_Text statusText;
        public Image colorDot;
        public Image activeGlow;

        public PlayerColor PlayerColor { get; private set; }

        public void Setup(LudoPlayer player)
        {
            PlayerColor = player.Color;
            nameText.text = player.DisplayName;
            Color c = BoardReference.GetPlayerColor(player.Color);
            colorDot.color = c;
            backgroundImage.color = new Color(c.r, c.g, c.b, 0.15f);
            UpdateInfo(player);
        }

        public void UpdateInfo(LudoPlayer player)
        {
            int inYard = player.PawnsInYard();
            int atGoal = player.PawnsAtGoal();
            int onBoard = LudoPlayer.PawnsPerPlayer - inYard - atGoal;
            statusText.text = $"Board:{onBoard}  Home:{atGoal}  Yard:{inYard}";
        }

        public void SetActive(bool active)
        {
            if (activeGlow != null)
                activeGlow.gameObject.SetActive(active);
            backgroundImage.color = new Color(
                backgroundImage.color.r,
                backgroundImage.color.g,
                backgroundImage.color.b,
                active ? 0.35f : 0.15f
            );
        }
    }
}