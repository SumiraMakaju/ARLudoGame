namespace ARLudo.Core
{
    public class LudoPawn
    {
        public int Id { get; }
        public PlayerColor Color { get; }
        public int LocalIndex { get; }
        public PawnState State { get; set; }
        public LudoTile CurrentTile { get; set; }
        public int TilesTraveled { get; set; }

        public LudoPawn(int id, PlayerColor color, int localIndex)
        {
            Id = id;
            Color = color;
            LocalIndex = localIndex;
            State = PawnState.InYard;
            CurrentTile = null;
            TilesTraveled = 0;
        }

        public void SendToYard()
        {
            State = PawnState.InYard;
            CurrentTile = null;
            TilesTraveled = 0;
        }

        public override string ToString()
        {
            return $"Pawn[{Id}] {Color}#{LocalIndex} {State}" +
                   (CurrentTile != null ? $" @ {CurrentTile}" : "") +
                   $" (traveled: {TilesTraveled})";
        }
    }
}