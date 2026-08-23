using System.Collections.Generic;
using UnityEngine;
using ARLudo.Core;

namespace ARLudo.Visuals
{
    public class GameVisualController : MonoBehaviour
    {
        public LudoGameManager gameManager;
        public BoardReference boardReference;
        public GameObject pawnPrefab;

        private Dictionary<int, PawnVisual> pawnVisuals = new();
        private List<PawnVisual> highlightedPawns = new();

        public void Initialize()
        {
            boardReference.BuildLookup();
            foreach (var player in gameManager.Players)
            {
                Color color = BoardReference.GetPlayerColor(player.Color);
                foreach (var pawn in player.Pawns)
                {
                    Vector3 yardPos = boardReference.GetYardPosition(player.Color, pawn.LocalIndex);
                    var obj = Instantiate(pawnPrefab, yardPos, Quaternion.identity, boardReference.transform);
                    obj.name = $"Pawn_{player.Color}_{pawn.LocalIndex}";
                    var vis = obj.GetComponent<PawnVisual>();
                    vis.Initialize(pawn, color);
                    pawnVisuals[pawn.Id] = vis;
                }
            }

            gameManager.OnPawnExitedYard += p => {
                if (pawnVisuals.TryGetValue(p.Id, out var v))
                    v.MoveTo(boardReference.GetTilePosition(p.CurrentTile.Index));
            };
            gameManager.OnPawnMoved += (p, from, to) => {
                if (pawnVisuals.TryGetValue(p.Id, out var v))
                    v.MoveTo(boardReference.GetTilePosition(to.Index));
            };
            gameManager.OnPawnCaptured += p => {
                if (pawnVisuals.TryGetValue(p.Id, out var v))
                    v.MoveTo(boardReference.GetYardPosition(p.Color, p.LocalIndex));
            };
            gameManager.OnPawnReachedGoal += p => {
                if (pawnVisuals.TryGetValue(p.Id, out var v))
                    v.MoveTo(boardReference.GetTilePosition(p.CurrentTile.Index),
                        () => v.transform.localScale *= 0.5f);
            };
            gameManager.OnLegalMovesCalculated += moves => {
                ClearHighlights();
                foreach (var m in moves)
                    if (pawnVisuals.TryGetValue(m.Pawn.Id, out var v))
                    { v.SetHighlight(true); highlightedPawns.Add(v); }
            };
            gameManager.OnTurnChanged += _ => ClearHighlights();
        }

        private void ClearHighlights()
        {
            foreach (var pv in highlightedPawns) pv.SetHighlight(false);
            highlightedPawns.Clear();
        }
    }
}