using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARLudo.Core;

namespace ARLudo.AI
{
    public class TrainingManager : MonoBehaviour
    {
        public LudoRules rules;
        public LudoAgent agentRed;
        public LudoAgent agentGreen;
        public LudoAgent agentYellow;
        public LudoAgent agentBlue;

        private LudoGameManager gameManager;
        private int lastRoll = 1;

        void Start()
        {
            StartCoroutine(TrainingLoop());
        }

        private LudoAgent GetAgent(PlayerColor color)
        {
            if (color == PlayerColor.Red) return agentRed;
            if (color == PlayerColor.Green) return agentGreen;
            if (color == PlayerColor.Yellow) return agentYellow;
            return agentBlue;
        }

        private IEnumerator TrainingLoop()
        {
            while (true)
            {
                // 1. Restart game if it's over or hasn't started
                if (gameManager == null || gameManager.CurrentPhase == GamePhase.GameOver)
                {
                    if (gameManager != null)
                    {
                        // Reward the winner, end episode for everyone
                        GetAgent(gameManager.CurrentPlayer.Color).RewardWin();
                        agentRed.EndEpisode();
                        agentGreen.EndEpisode();
                        agentYellow.EndEpisode();
                        agentBlue.EndEpisode();
                        Destroy(gameManager);
                    }

                    gameManager = gameObject.AddComponent<LudoGameManager>();
                    gameManager.rules = rules;

                    var players = new List<LudoPlayer> {
                        new LudoPlayer(PlayerColor.Red, "Red", true),
                        new LudoPlayer(PlayerColor.Green, "Green", true),
                        new LudoPlayer(PlayerColor.Yellow, "Yellow", true),
                        new LudoPlayer(PlayerColor.Blue, "Blue", true)
                    };

                    gameManager.InitializeGame(players);
                    
                    // Setup agents for the new game
                    agentRed.Setup(gameManager.Board, rules, players);
                    agentGreen.Setup(gameManager.Board, rules, players);
                    agentYellow.Setup(gameManager.Board, rules, players);
                    agentBlue.Setup(gameManager.Board, rules, players);

                    gameManager.StartGame();
                }

                // 2. Handle Turns Instantly
                if (gameManager.CurrentPhase == GamePhase.Rolling)
                {
                    // Instant random dice roll (no physics)
                    lastRoll = Random.Range(1, 7);
                    gameManager.SetDiceValue(lastRoll);
                }
                else if (gameManager.CurrentPhase == GamePhase.ChoosingPawn)
                {
                    LudoAgent currentAgent = GetAgent(gameManager.CurrentPlayer.Color);
                    int result = currentAgent.RequestMove(lastRoll);

                    if (result == -2)
                    {
                        // Wait across multiple frames until Python sends the action back
                        while (!currentAgent.HasSelectedMove)
                        {
                            yield return null;
                        }
                        
                        int choice = currentAgent.GetSelectedMove();
                        if (choice >= 0) gameManager.SelectMove(choice);
                        else gameManager.SelectMove(0);
                    }
                    else if (result >= 0)
                    {
                        // Auto-selected (only 1 valid move)
                        gameManager.SelectMove(result);
                    }
                    else
                    {
                        // No valid moves fallback
                        var moves = gameManager.GetCurrentLegalMoves();
                        if (moves != null && moves.Count > 0)
                            gameManager.SelectMove(0); 
                    }
                }

                yield return null;
            }
        }
    }
}