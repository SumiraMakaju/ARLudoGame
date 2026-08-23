using UnityEngine;
using ARLudo.Core;

public class BoardDebugTest : MonoBehaviour
{
    void Start()
    {
        var board = new LudoBoard();

        Debug.Log($"Total tiles: {board.AllTiles.Count}");

        foreach (PlayerColor color in System.Enum.GetValues(typeof(PlayerColor)))
        {
            var startTile = board.StartingTiles[color];
            Debug.Log($"{color} starts at: {startTile}");
            Debug.Log($"{color} home entry at perimeter: {board.GetHomeEntryIndex(color)}");
            Debug.Log($"{color} goal: {board.GoalTiles[color]}");
        }

        var current = board.StartingTiles[PlayerColor.Red];
        int steps = 0;
        while (current != null && steps < 60)
        {
            if (current.Index == board.GetHomeEntryIndex(PlayerColor.Red)
                && current.HomeEntryBranch != null)
            {
                Debug.Log($"  Step {steps}: {current} > BRANCHING to home corridor");
                current = current.HomeEntryBranch;
            }
            else
            {
                current = current.NextTile;
            }
            steps++;
        }
        Debug.Log($"Red full path: {steps} steps (expected ~57)");
    }
}