namespace ARLudo.Core
{
    public class LudoMoveResult
    {
        public LudoPawn Pawn { get; set; }
        public LudoTile DestinationTile { get; set; }
        public int NewTilesTraveled { get; set; }
        public bool IsExitingYard { get; set; }
        public bool IsCapture { get; set; }
        public LudoPawn CapturedPawn { get; set; }
        public bool IsReachingGoal { get; set; }
        public bool IsEnteringHomeCorridor { get; set; }
    }
}