namespace ARLudo.Core
{
    public enum PlayerColor
    {
        Red = 0,
        Green = 1,
        Yellow = 2,
        Blue = 3
    }

    public enum TileType
    {
        Normal,
        SafeStar,
        StartingTile,
        HomeCorridor,
        Goal
    }

    public enum GamePhase
    {
        WaitingToStart,
        Rolling,
        ChoosingPawn,
        Moving,
        NextTurn,
        GameOver
    }

    public enum PawnState
    {
        InYard,
        OnBoard,
        AtGoal
    }

    public enum OpeningRollRule
    {
        OnlySix,
        OneOrSix,
        AllOpen
    }

    public enum ConsecutiveSixRule
    {
        ThirdSixCancelsTurn,
        ThirdSixPenalty,
        NoPenalty
    }

    public enum BlockadeRule
    {
        Passable,
        SafeBlockade
    }

    public enum WinCondition
    {
        AllFourHome,
        FirstPawnHome,
        TimedRush
    }

    public enum SafeZoneRule
    {
        ClassicEightStars,
        StartingTilesOnly,
        NoSafeZones
    }

    public enum FUN{
        //planning
    }
}