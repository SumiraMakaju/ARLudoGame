using System.Collections.Generic;

namespace ARLudo.Core
{
    public class LudoMoveValidator
    {
        private LudoBoard board;
        private LudoRules rules;
        private List<LudoPlayer> players;

        public LudoMoveValidator(LudoBoard board, LudoRules rules, List<LudoPlayer> players)
        {
            this.board = board;
            this.rules = rules;
            this.players = players;
        }

        public List<LudoMoveResult> GetLegalMoves(LudoPlayer player, int diceValue)
        {
            var moves = new List<LudoMoveResult>();

            foreach (var pawn in player.Pawns)
            {
                if (pawn.State == PawnState.AtGoal)
                    continue;

                if (pawn.State == PawnState.InYard)
                {
                    if (rules.CanOpenWithRoll(diceValue))
                    {
                        var startTile = board.StartingTiles[player.Color];
                        var result = new LudoMoveResult
                        {
                            Pawn = pawn,
                            DestinationTile = startTile,
                            NewTilesTraveled = 0,
                            IsExitingYard = true,
                            IsCapture = false,
                            CapturedPawn = null,
                            IsReachingGoal = false,
                            IsEnteringHomeCorridor = false
                        };

                        CheckForCapture(result, player);

                        if (!IsBlockedByBlockade(startTile, player))
                            moves.Add(result);
                    }
                    continue;
                }

                var moveResult = TryWalkForward(pawn, player, diceValue);
                if (moveResult != null)
                    moves.Add(moveResult);
            }

            return moves;
        }

        private LudoMoveResult TryWalkForward(LudoPawn pawn, LudoPlayer player, int steps)
        {
            LudoTile current = pawn.CurrentTile;
            int traveled = pawn.TilesTraveled;
            bool enteredHome = false;

            for (int i = 0; i < steps; i++)
            {
                int homeEntryIndex = board.GetHomeEntryIndex(player.Color);

                if (current.Index == homeEntryIndex && current.HomeEntryBranch != null && !enteredHome)
                {
                    int stepsNeededToGoal = LudoBoard.HomeCorridorLength + 1;
                    int stepsRemaining = steps - i;

                    if (traveled >= LudoBoard.PerimeterTileCount - 1)
                    {
                        current = current.HomeEntryBranch;
                        traveled++;
                        enteredHome = true;
                        continue;
                    }
                }

                if (current.NextTile == null)
                    return null;

                if (rules.blockadeRule == BlockadeRule.SafeBlockade && !enteredHome)
                {
                    if (IsBlockedByBlockade(current.NextTile, player))
                        return null;
                }

                current = current.NextTile;
                traveled++;
            }

            if (current.Type == TileType.Goal)
            {
                return new LudoMoveResult
                {
                    Pawn = pawn,
                    DestinationTile = current,
                    NewTilesTraveled = traveled,
                    IsExitingYard = false,
                    IsCapture = false,
                    CapturedPawn = null,
                    IsReachingGoal = true,
                    IsEnteringHomeCorridor = enteredHome
                };
            }

            if (current.Type == TileType.HomeCorridor || current.Type == TileType.Goal)
            {
                var result = new LudoMoveResult
                {
                    Pawn = pawn,
                    DestinationTile = current,
                    NewTilesTraveled = traveled,
                    IsExitingYard = false,
                    IsCapture = false,
                    CapturedPawn = null,
                    IsReachingGoal = false,
                    IsEnteringHomeCorridor = enteredHome
                };
                return result;
            }

            var moveResult = new LudoMoveResult
            {
                Pawn = pawn,
                DestinationTile = current,
                NewTilesTraveled = traveled,
                IsExitingYard = false,
                IsCapture = false,
                CapturedPawn = null,
                IsReachingGoal = false,
                IsEnteringHomeCorridor = enteredHome
            };

            CheckForCapture(moveResult, player);

            if (IsBlockedByBlockade(current, player))
                return null;

            return moveResult;
        }

        private void CheckForCapture(LudoMoveResult result, LudoPlayer movingPlayer)
        {
            if (rules.IsTileSafe(result.DestinationTile))
                return;

            foreach (var otherPlayer in players)
            {
                if (otherPlayer.Color == movingPlayer.Color)
                    continue;

                foreach (var otherPawn in otherPlayer.Pawns)
                {
                    if (otherPawn.State == PawnState.OnBoard && otherPawn.CurrentTile == result.DestinationTile)
                    {
                        result.IsCapture = true;
                        result.CapturedPawn = otherPawn;
                        return;
                    }
                }
            }
        }

        private bool IsBlockedByBlockade(LudoTile tile, LudoPlayer movingPlayer)
        {
            if (rules.blockadeRule != BlockadeRule.SafeBlockade)
                return false;

            foreach (var otherPlayer in players)
            {
                if (otherPlayer.Color == movingPlayer.Color)
                    continue;

                int count = 0;
                foreach (var pawn in otherPlayer.Pawns)
                {
                    if (pawn.State == PawnState.OnBoard && pawn.CurrentTile == tile)
                        count++;
                }

                if (count >= 2)
                    return true;
            }

            return false;
        }

        public bool HasAnyLegalMove(LudoPlayer player, int diceValue)
        {
            return GetLegalMoves(player, diceValue).Count > 0;
        }
    }
}