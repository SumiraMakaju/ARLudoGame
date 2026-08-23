using System.Collections.Generic;

namespace ARLudo.Core
{
    public class LudoBoard
    {
        public const int PerimeterTileCount = 52;
        public const int TilesPerQuadrant = 13;
        public const int HomeCorridorLength = 5;

        private static readonly Dictionary<PlayerColor, int> StartingTileIndices = new()
        {
            { PlayerColor.Red, 1 },
            { PlayerColor.Green, 14 },
            { PlayerColor.Yellow, 27 },
            { PlayerColor.Blue, 40 }
        };

        private static readonly Dictionary<PlayerColor, int> HomeEntryIndices = new()
        {
            { PlayerColor.Red, 50 },
            { PlayerColor.Green, 11 },
            { PlayerColor.Yellow, 24 },
            { PlayerColor.Blue, 37 }
        };

        private static readonly HashSet<int> StarTileIndices = new() { 8, 21, 34, 47 };

        private static readonly Dictionary<PlayerColor, int> HomeBaseIndex = new()
        {
            { PlayerColor.Red, 100 },
            { PlayerColor.Green, 200 },
            { PlayerColor.Yellow, 300 },
            { PlayerColor.Blue, 400 }
        };

        public Dictionary<int, LudoTile> AllTiles { get; } = new();
        public LudoTile[] PerimeterTiles { get; } = new LudoTile[PerimeterTileCount];
        public Dictionary<PlayerColor, LudoTile[]> HomeCorridors { get; } = new();
        public Dictionary<PlayerColor, LudoTile> GoalTiles { get; } = new();
        public Dictionary<PlayerColor, LudoTile> StartingTiles { get; } = new();

        public LudoBoard()
        {
            BuildPerimeter();
            BuildHomeCorridors();
            LinkPerimeterRing();
            LinkHomeEntries();
            MarkStartingTiles();
        }

        private void BuildPerimeter()
        {
            for (int i = 0; i < PerimeterTileCount; i++)
            {
                TileType type = TileType.Normal;

                if (StarTileIndices.Contains(i))
                    type = TileType.SafeStar;

                foreach (var kvp in StartingTileIndices)
                {
                    if (kvp.Value == i)
                    {
                        type = TileType.StartingTile;
                        break;
                    }
                }

                var tile = new LudoTile(i, type);
                PerimeterTiles[i] = tile;
                AllTiles[i] = tile;
            }
        }

        private void BuildHomeCorridors()
        {
            foreach (PlayerColor color in System.Enum.GetValues(typeof(PlayerColor)))
            {
                int baseIdx = HomeBaseIndex[color];
                var corridor = new LudoTile[HomeCorridorLength + 1];

                for (int i = 0; i < HomeCorridorLength; i++)
                {
                    int idx = baseIdx + i;
                    var tile = new LudoTile(idx, TileType.HomeCorridor, color);
                    corridor[i] = tile;
                    AllTiles[idx] = tile;
                }

                int goalIdx = baseIdx + HomeCorridorLength;
                var goalTile = new LudoTile(goalIdx, TileType.Goal, color);
                corridor[HomeCorridorLength] = goalTile;
                AllTiles[goalIdx] = goalTile;
                GoalTiles[color] = goalTile;

                for (int i = 0; i < corridor.Length - 1; i++)
                {
                    corridor[i].NextTile = corridor[i + 1];
                }

                HomeCorridors[color] = corridor;
            }
        }

        private void LinkPerimeterRing()
        {
            for (int i = 0; i < PerimeterTileCount; i++)
            {
                int nextIdx = (i + 1) % PerimeterTileCount;
                PerimeterTiles[i].NextTile = PerimeterTiles[nextIdx];
            }
        }

        private void LinkHomeEntries()
        {
            foreach (PlayerColor color in System.Enum.GetValues(typeof(PlayerColor)))
            {
                int entryIdx = HomeEntryIndices[color];
                LudoTile entryTile = PerimeterTiles[entryIdx];
                LudoTile firstCorridorTile = HomeCorridors[color][0];
                entryTile.HomeEntryBranch = firstCorridorTile;
            }
        }

        private void MarkStartingTiles()
        {
            foreach (PlayerColor color in System.Enum.GetValues(typeof(PlayerColor)))
            {
                int startIdx = StartingTileIndices[color];
                LudoTile startTile = PerimeterTiles[startIdx];
                startTile.StartingTileForColor = color;
                StartingTiles[color] = startTile;
            }
        }

        public int GetStartingTileIndex(PlayerColor color) => StartingTileIndices[color];

        public int GetHomeEntryIndex(PlayerColor color) => HomeEntryIndices[color];

        public bool IsSafeTile(LudoTile tile)
        {
            return tile.Type == TileType.SafeStar
                || tile.Type == TileType.StartingTile
                || tile.Type == TileType.HomeCorridor
                || tile.Type == TileType.Goal;
        }
    }
}