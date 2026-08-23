using System.Collections.Generic;
using UnityEngine;
using ARLudo.Core;

public class GameSimulationTest : MonoBehaviour
{
    public LudoRules rules;

    void Start()
    {
        var gm = gameObject.AddComponent<LudoGameManager>();
        gm.rules = rules;

        var playerList = new List<LudoPlayer>
        {
            new LudoPlayer(PlayerColor.Red, "Red"),
            new LudoPlayer(PlayerColor.Green, "Green"),
            new LudoPlayer(PlayerColor.Yellow, "Yellow"),
            new LudoPlayer(PlayerColor.Blue, "Blue")
        };

        gm.OnDiceRolled += val => Debug.Log($"  Rolled: {val}");
        gm.OnPawnExitedYard += p => Debug.Log($"  {p.Color} pawn #{p.LocalIndex} exited yard");
        gm.OnPawnMoved += (p, from, to) => Debug.Log($"  {p.Color} pawn #{p.LocalIndex} moved to {to}");
        gm.OnPawnCaptured += p => Debug.Log($"  {p.Color} pawn #{p.LocalIndex} captured! Sent to yard");
        gm.OnPawnReachedGoal += p => Debug.Log($"  {p.Color} pawn #{p.LocalIndex} reached GOAL!");
        gm.OnNoValidMoves += () => Debug.Log($"  No valid moves, skipping");
        gm.OnTurnChanged += p => Debug.Log($"--- {p.DisplayName}'s turn ---");
        gm.OnPlayerWon += p => Debug.Log($"=== {p.DisplayName} WINS! ===");

        gm.OnLegalMovesCalculated += moves =>
        {
            int pick = Random.Range(0, moves.Count);
            gm.SelectMove(pick);
        };

        gm.InitializeGame(playerList);
        gm.StartGame();

        int maxTurns = 2000;
        int turns = 0;
        while (gm.CurrentPhase != GamePhase.GameOver && turns < maxTurns)
        {
            if (gm.CurrentPhase == GamePhase.Rolling)
                gm.RollDice();
            turns++;
        }

        Debug.Log($"Game finished in {turns} turns");
    }
}