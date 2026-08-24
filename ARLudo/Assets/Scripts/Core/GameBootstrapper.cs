using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public float aiDelay = 1f;

    private LudoGameManager gameManager;
    private bool waitingForAI;
    private bool diceInProgress;

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
        
        // Initialize any LudoAgents in the scene so they can play
        foreach(var agent in Object.FindObjectsByType<ARLudo.AI.LudoAgent>(FindObjectsSortMode.None)) {
            agent.Setup(gameManager.Board, rules, playerList);
        }
        
        visualController.gameManager = gameManager;
        visualController.Initialize();

        diceController.OnDiceResult += OnDiceResult;
        diceController.OnDiceThrown += () => diceInProgress = true;

        if (hud != null)
        {
            hud.SetupRollButton(OnRollButtonPressed);
            hud.InitPlayerPanels(playerList);
            gameManager.OnTurnChanged += OnTurnChanged;
            gameManager.OnDiceRolled += v => hud.UpdateDice(v);
            gameManager.OnLegalMovesCalculated += _ => hud.ShowChoosePawn();
            gameManager.OnNoValidMoves += () => hud.ShowNoMoves();
            gameManager.OnPlayerWon += p => hud.ShowWinner(p);
            gameManager.OnPawnCaptured += _ => UpdateAllPanels();
            gameManager.OnPawnReachedGoal += _ => UpdateAllPanels();
            gameManager.OnPawnExitedYard += _ => UpdateAllPanels();
        }

        gameManager.StartGame();
    }

    void Update()
    {
        if (gameManager == null || gameManager.CurrentPhase == GamePhase.GameOver) return;
        if (waitingForAI || diceInProgress) return;

        if (gameManager.CurrentPhase == GamePhase.Rolling)
        {
             if (gameManager.CurrentPlayer.IsAI)
                 {
                     if (!waitingForAI) StartCoroutine(AIRoll());
                  }
             else if (WasTapped() && !diceInProgress)
                 {
                     hud.StopTimer();
                     diceController.ThrowDice();
                }
        }

        if (gameManager.CurrentPhase == GamePhase.ChoosingPawn)
        {
            if (gameManager.CurrentPlayer.IsAI)
                StartCoroutine(AIChoose());
            else if (WasTapped())
                TrySelectPawn();
        }
    }

    private void OnRollButtonPressed()
    {
        if (gameManager.CurrentPhase != GamePhase.Rolling) return;
        if (gameManager.CurrentPlayer.IsAI) return;
        if (diceInProgress) return;
        hud.StopTimer();
        diceController.ThrowDice();
    }

    private void OnTurnChanged(LudoPlayer player)
    {
        hud.UpdateTurn(player);
        if (!player.IsAI && rules.turnTimeLimit > 0)
        {
            hud.StartTurnTimer(rules.turnTimeLimit, () =>
            {
                if (gameManager.CurrentPhase == GamePhase.Rolling)
                    diceController.ThrowDice();
            });
        }
    }

    private void UpdateAllPanels()
    {
        foreach (var player in gameManager.Players)
            hud.UpdatePlayerPanel(player);
    }

    private IEnumerator AIRoll()
    {
        waitingForAI = true;
        yield return new WaitForSeconds(aiDelay);
        if (!diceInProgress)
            diceController.ThrowDice();
        waitingForAI = false;
    }

    private IEnumerator AIChoose()
    {
        waitingForAI = true;
        yield return new WaitForSeconds(aiDelay * 0.5f);
        
        var agent = Object.FindObjectsByType<ARLudo.AI.LudoAgent>(FindObjectsSortMode.None)
                     .FirstOrDefault(a => a.agentColor == gameManager.CurrentPlayer.Color);
                     
        if (agent != null)
        {
            agent.ResetMoveSelection();
            int instantMove = agent.RequestMove(gameManager.LastDiceValue);
            
            if (instantMove >= 0)
            {
                gameManager.SelectMove(instantMove);
            }
            else if (instantMove == -2)
            {
                while (!agent.HasSelectedMove) yield return null;
                gameManager.SelectMove(agent.GetSelectedMove());
            }
        }
        else
        {
            var moves = gameManager.GetCurrentLegalMoves();
            if (moves != null && moves.Count > 0)
                gameManager.SelectMove(Random.Range(0, moves.Count));
        }
        
        waitingForAI = false;
    }

    private void OnDiceResult(int value)
    {
        diceInProgress = false;
        gameManager.SetDiceValue(value);
    }

    private bool WasTapped()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            return true;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;
        return false;
    }

    private void TrySelectPawn()
    {
        Vector2 pos = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            pos = Touchscreen.current.primaryTouch.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var pv = hit.collider.GetComponent<PawnVisual>();
            if (pv != null && pv.Data != null)
            {
                hud.StopTimer();
                gameManager.SelectPawn(pv.Data);
            }
        }
    }
}