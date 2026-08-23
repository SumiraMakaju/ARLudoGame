namespace ARLudo.Core
{
    public class LudoPlayer
    {
        public const int PawnsPerPlayer = 4;

        public PlayerColor Color { get; }
        public string DisplayName { get; set; }
        public bool IsAI { get; set; }
        public bool IsRemote { get; set; }
        public LudoPawn[] Pawns { get; }
        public int ConsecutiveSixCount { get; set; }

        public LudoPlayer(PlayerColor color, string displayName, bool isAI = false, bool isRemote = false)
        {
            Color = color;
            DisplayName = displayName;
            IsAI = isAI;
            IsRemote = isRemote;
            ConsecutiveSixCount = 0;

            Pawns = new LudoPawn[PawnsPerPlayer];
            int baseId = (int)color * PawnsPerPlayer;
            for (int i = 0; i < PawnsPerPlayer; i++)
            {
                Pawns[i] = new LudoPawn(baseId + i, color, i);
            }
        }

        public int PawnsAtGoal()
        {
            int count = 0;
            foreach (var pawn in Pawns)
            {
                if (pawn.State == PawnState.AtGoal) count++;
            }
            return count;
        }

        public bool AllPawnsHome() => PawnsAtGoal() == PawnsPerPlayer;

        public int PawnsInYard()
        {
            int count = 0;
            foreach (var pawn in Pawns)
            {
                if (pawn.State == PawnState.InYard) count++;
            }
            return count;
        }
    }
}