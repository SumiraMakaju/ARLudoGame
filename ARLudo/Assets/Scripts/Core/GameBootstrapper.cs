using System.Collections.Generic;
using UnityEngine;
using ARLudo.Core;
using ARLudo.Visuals;
using ARLudo.Dice;
using ARLudo.UI;

public class GameBootstrapper : MonoBehaviour
{
    public LudoRules rules;
    public GameVisualController visualController;
    public DiceController diceController;
    public GameHUD hud;
    public int humanPlayers = 1;
    public int aiPlayers = 3;

    private LudoGameManager gameManager;

    void Start()
    {
        gameManager = gameObject.AddComponent<LudoGameManager>();
        gameManager.rules = rules;

        var playerList = new List<LudoPlayer>();
        var colors = new[] { PlayerColor.Red, PlayerColor.Green, PlayerColor.Yellow, PlayerColor.Blue };
        int total = Mathf.Clamp(humanPlayers + aiPlayers, 2, 4);
        for (int i = 0; i < total; i++)
        {
            bool isAI = i >= humanPlayers;
            playerList.Add(new LudoPlayer(colors[i], isAI ? $"Bot {colors[i]}" : $"Player {colors[i]}", isAI));
        }

        gameManager.InitializeGame(playerList);
        visualController.gameManager = gameManager;
        visualController.Initialize();

        diceController.OnDiceResult += OnDiceResult;
        gameManager.OnTurnChanged += p => hud.UpdateTurn(p);
        gameManager.OnDiceRolled += v => hud.UpdateDice(v);
        gameManager.OnLegalMovesCalculated += _ => hud.ShowChoosePawn();
        gameManager.OnNoValidMoves += () => hud.ShowNoMoves();
        gameManager.OnPlayerWon += p => hud.ShowWinner(p);

        gameManager.StartGame();
    }

    void Update()
    {
        if (gameManager == null || gameManager.CurrentPhase == GamePhase.GameOver) return;

        if (gameManager.CurrentPhase == GamePhase.Rolling)
        {
            if (gameManager.CurrentPlayer.IsAI)
            {
                if (!diceController.IsRolling)
                    diceController.ThrowDice();
            }
            else if (Input.GetKeyDown(KeyCode.Space) && !diceController.IsRolling)
            {
                diceController.ThrowDice();
            }
        }

        if (gameManager.CurrentPhase == GamePhase.ChoosingPawn)
        {
            if (gameManager.CurrentPlayer.IsAI)
            {
                var moves = gameManager.GetCurrentLegalMoves();
                if (moves != null && moves.Count > 0)
                    gameManager.SelectMove(Random.Range(0, moves.Count));
            }
            else if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    var pv = hit.collider.GetComponent<PawnVisual>();
                    if (pv != null && pv.Data != null)
                        gameManager.SelectPawn(pv.Data);
                }
            }
        }
    }

    private void OnDiceResult(int value)
    {
        gameManager.SetDiceValue(value);
    }
}