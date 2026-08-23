namespace ARLudo.Core
{
    public class LudoTile
    {
        public int Index { get; }
        public TileType Type { get; }
        public PlayerColor? Owner { get; }
        public LudoTile NextTile { get; set; }
        public LudoTile HomeEntryBranch { get; set; }
        public PlayerColor? StartingTileForColor { get; set; }

        public LudoTile(int index, TileType type, PlayerColor? owner = null)
        {
            Index = index;
            Type = type;
            Owner = owner;
            NextTile = null;
            HomeEntryBranch = null;
            StartingTileForColor = null;
        }

        public override string ToString()
        {
            string desc = $"Tile[{Index}] {Type}";
            if (Owner.HasValue) desc += $" ({Owner.Value})";
            if (StartingTileForColor.HasValue) desc += $" [Start:{StartingTileForColor.Value}]";
            return desc;
        }
    }
}