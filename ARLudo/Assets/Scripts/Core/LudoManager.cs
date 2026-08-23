using System;
using System.Collections.Generic;
using UnityEngine;

namespace ARLudo.Core
{
    public class LudoGameManager : MonoBehaviour
    {
        public LudoRules rules;

        public event Action<GamePhase> OnPhaseChanged;
        public event Action<LudoPlayer> OnTurnChanged;
        public event Action<int> OnDiceRolled;
        public event Action<LudoPawn, LudoTile, LudoTile> OnPawnMoved;
        public event Action<LudoPawn> OnPawnCaptured;
        public event Action<LudoPawn> OnPawnReachedGoal;
        public event Action<LudoPawn> OnPawnExitedYard;
        public event Action<LudoPlayer> OnPlayerWon;
        public event Action<List<LudoMoveResult>> OnLegalMovesCalculated;
        public event Action OnNoValidMoves;

        public GamePhase CurrentPhase { get; private set; }
        public LudoPlayer CurrentPlayer => players[currentPlayerIndex];
        public int LastDiceValue { get; private set; }
        public LudoBoard Board { get; private set; }
        public List<LudoPlayer> Players => players;

        private List<LudoPlayer> players = new();
        private LudoMoveValidator validator;
        private int currentPlayerIndex;
        private bool hasBonusTurn;
        private List<LudoMoveResult> currentLegalMoves;

        public void InitializeGame(List<LudoPlayer> playerList)
        {
            players = playerList;
            Board = new LudoBoard();
            validator = new LudoMoveValidator(Board, rules, players);
            currentPlayerIndex = 0;
            hasBonusTurn = false;
            SetPhase(GamePhase.WaitingToStart);
        }

        public void StartGame()
        {
            if (CurrentPhase != GamePhase.WaitingToStart)
                return;

            if (rules.openingRoll == OpeningRollRule.AllOpen)
                PlaceAllPawnsOnBoard();

            SetPhase(GamePhase.Rolling);
            OnTurnChanged?.Invoke(CurrentPlayer);
        }

        public void RollDice()
        {
            if (CurrentPhase != GamePhase.Rolling)
                return;

            LastDiceValue = UnityEngine.Random.Range(1, 7);
            OnDiceRolled?.Invoke(LastDiceValue);

            HandleConsecutiveSix();
        }

        public void SetDiceValue(int value)
        {
            if (CurrentPhase != GamePhase.Rolling)
                return;

            LastDiceValue = Mathf.Clamp(value, 1, 6);
            OnDiceRolled?.Invoke(LastDiceValue);

            HandleConsecutiveSix();
        }

        private void HandleConsecutiveSix()
        {
            if (LastDiceValue == 6)
            {
                CurrentPlayer.ConsecutiveSixCount++;

                if (CurrentPlayer.ConsecutiveSixCount >= 3)
                {
                    switch (rules.consecutiveSixRule)
                    {
                        case ConsecutiveSixRule.ThirdSixCancelsTurn:
                            CurrentPlayer.ConsecutiveSixCount = 0;
                            AdvanceToNextPlayer();
                            return;

                        case ConsecutiveSixRule.ThirdSixPenalty:
                            SendLastMovedPawnToYard();
                            CurrentPlayer.ConsecutiveSixCount = 0;
                            AdvanceToNextPlayer();
                            return;

                        case ConsecutiveSixRule.NoPenalty:
                            break;
                    }
                }
            }
            else
            {
                CurrentPlayer.ConsecutiveSixCount = 0;
            }

            EvaluateMoves();
        }

        private void EvaluateMoves()
        {
            currentLegalMoves = validator.GetLegalMoves(CurrentPlayer, LastDiceValue);

            if (currentLegalMoves.Count == 0)
            {
                OnNoValidMoves?.Invoke();
                AdvanceToNextPlayer();
                return;
            }

            if (currentLegalMoves.Count == 1)
            {
                ExecuteMove(currentLegalMoves[0]);
                return;
            }

            SetPhase(GamePhase.ChoosingPawn);
            OnLegalMovesCalculated?.Invoke(currentLegalMoves);
        }

        public void SelectMove(int moveIndex)
        {
            if (CurrentPhase != GamePhase.ChoosingPawn)
                return;

            if (moveIndex < 0 || moveIndex >= currentLegalMoves.Count)
                return;

            ExecuteMove(currentLegalMoves[moveIndex]);
        }

        public void SelectPawn(LudoPawn pawn)
        {
            if (CurrentPhase != GamePhase.ChoosingPawn)
                return;

            foreach (var move in currentLegalMoves)
            {
                if (move.Pawn == pawn)
                {
                    ExecuteMove(move);
                    return;
                }
            }
        }

        private void ExecuteMove(LudoMoveResult move)
        {
            SetPhase(GamePhase.Moving);
            hasBonusTurn = false;

            LudoTile fromTile = move.Pawn.CurrentTile;

            if (move.IsExitingYard)
            {
                move.Pawn.State = PawnState.OnBoard;
                move.Pawn.CurrentTile = move.DestinationTile;
                move.Pawn.TilesTraveled = 0;
                OnPawnExitedYard?.Invoke(move.Pawn);
            }
            else
            {
                move.Pawn.CurrentTile = move.DestinationTile;
                move.Pawn.TilesTraveled = move.NewTilesTraveled;
            }

            if (move.IsCapture && move.CapturedPawn != null)
            {
                move.CapturedPawn.SendToYard();
                OnPawnCaptured?.Invoke(move.CapturedPawn);

                if (rules.bonusTurnOnCapture)
                    hasBonusTurn = true;
            }

            if (move.IsReachingGoal)
            {
                move.Pawn.State = PawnState.AtGoal;
                OnPawnReachedGoal?.Invoke(move.Pawn);

                if (rules.bonusTurnOnGoal)
                    hasBonusTurn = true;

                if (CheckWinCondition(CurrentPlayer))
                {
                    SetPhase(GamePhase.GameOver);
                    OnPlayerWon?.Invoke(CurrentPlayer);
                    return;
                }
            }

            OnPawnMoved?.Invoke(move.Pawn, fromTile, move.DestinationTile);

            if (LastDiceValue == 6 && rules.bonusTurnOnSix)
                hasBonusTurn = true;

            SetPhase(GamePhase.NextTurn);
            ResolveTurn();
        }

        private void ResolveTurn()
        {
            if (hasBonusTurn)
            {
                hasBonusTurn = false;
                SetPhase(GamePhase.Rolling);
                return;
            }

            AdvanceToNextPlayer();
        }

        private void AdvanceToNextPlayer()
        {
            CurrentPlayer.ConsecutiveSixCount = 0;
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;

            while (CurrentPlayer.AllPawnsHome() && players.Count > 1)
            {
                currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            }

            SetPhase(GamePhase.Rolling);
            OnTurnChanged?.Invoke(CurrentPlayer);
        }

        private bool CheckWinCondition(LudoPlayer player)
        {
            return rules.winCondition switch
            {
                WinCondition.AllFourHome => player.AllPawnsHome(),
                WinCondition.FirstPawnHome => player.PawnsAtGoal() >= 1,
                WinCondition.TimedRush => false,
                _ => false
            };
        }

        private void PlaceAllPawnsOnBoard()
        {
            foreach (var player in players)
            {
                foreach (var pawn in player.Pawns)
                {
                    pawn.State = PawnState.OnBoard;
                    pawn.CurrentTile = Board.StartingTiles[player.Color];
                    pawn.TilesTraveled = 0;
                }
            }
        }

        private void SendLastMovedPawnToYard()
        {
            foreach (var pawn in CurrentPlayer.Pawns)
            {
                if (pawn.State == PawnState.OnBoard)
                {
                    pawn.SendToYard();
                    OnPawnCaptured?.Invoke(pawn);
                    return;
                }
            }
        }

        private void SetPhase(GamePhase phase)
        {
            CurrentPhase = phase;
            OnPhaseChanged?.Invoke(phase);
        }

        public List<LudoMoveResult> GetCurrentLegalMoves()
        {
            return currentLegalMoves;
        }
    }
}