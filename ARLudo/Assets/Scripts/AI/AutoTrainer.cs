using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using ARLudo.Core;

namespace ARLudo.AI
{
    public class AutoTrainer : MonoBehaviour
    {
        public LudoAIPlayer aiRed, aiGreen, aiYellow, aiBlue;
        private LudoGameManager gameManager;
        private bool isRestarting = false;
        private bool isProcessingTurn = false;

        void Start()
        {
            Time.timeScale = 50f;
            
            aiRed.thinkDelay = 0; aiGreen.thinkDelay = 0; 
            aiYellow.thinkDelay = 0; aiBlue.thinkDelay = 0;
            
            aiRed.useML = true; aiGreen.useML = true; 
            aiYellow.useML = true; aiBlue.useML = true;

            // Turn off the normal bootstrapper so it stops trying to throw the physical dice!
            var bootstrapper = FindObjectOfType<GameBootstrapper>();
            if (bootstrapper != null) bootstrapper.enabled = false;
        }

        void Update()
        {
            if (gameManager == null)
            {
                gameManager = FindObjectOfType<LudoGameManager>();
                if (gameManager == null) return;
            }

            // Check Win
            if (gameManager.CurrentPhase == GamePhase.GameOver && !isRestarting)
            {
                isRestarting = true;
                PlayerColor winner = gameManager.CurrentPlayer.Color;
                
                RewardAgent(aiRed, winner);
                RewardAgent(aiGreen, winner);
                RewardAgent(aiYellow, winner);
                RewardAgent(aiBlue, winner);
                
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                return;
            }

            // Process AI Turns
            if (!isProcessingTurn && !isRestarting)
            {
                StartCoroutine(ProcessTurn());
            }
        }

        IEnumerator ProcessTurn()
        {
            isProcessingTurn = true;

            if (gameManager.CurrentPhase == GamePhase.Rolling)
            {
                // INSTANT math dice roll, zero physics involved.
                gameManager.SetDiceValue(Random.Range(1, 7));
            }
            else if (gameManager.CurrentPhase == GamePhase.ChoosingPawn)
            {
                LudoAIPlayer currentAI = GetAI(gameManager.CurrentPlayer.Color);
                var moves = gameManager.GetCurrentLegalMoves();
                
                // Triggers ML-Agent and waits safely for Python
                yield return StartCoroutine(currentAI.MakeMove(gameManager.LastDiceValue, moves));
            }

            isProcessingTurn = false;
        }

        LudoAIPlayer GetAI(PlayerColor color)
        {
            if (color == PlayerColor.Red) return aiRed;
            if (color == PlayerColor.Green) return aiGreen;
            if (color == PlayerColor.Yellow) return aiYellow;
            return aiBlue;
        }

        void RewardAgent(LudoAIPlayer player, PlayerColor winner)
        {
            if (player.agent != null)
            {
                if (player.agent.agentColor == winner) player.agent.RewardWin();
                else player.agent.PenalizeLoss();
                player.agent.EndEpisode();
            }
        }
    }
}