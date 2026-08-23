using UnityEngine;

namespace ARLudo.Core
{
    [CreateAssetMenu(fileName = "NewLudoRules", menuName = "ARLudo/Rule Preset")]
    public class LudoRules : ScriptableObject
    {
        public OpeningRollRule openingRoll = OpeningRollRule.OnlySix;
        public bool bonusTurnOnSix = true;
        public ConsecutiveSixRule consecutiveSixRule = ConsecutiveSixRule.ThirdSixCancelsTurn;
        public bool bonusTurnOnCapture = true;
        public bool bonusTurnOnGoal = false;
        public SafeZoneRule safeZoneRule = SafeZoneRule.ClassicEightStars;
        public bool captureRequiredBeforeHome = false;
        public BlockadeRule blockadeRule = BlockadeRule.Passable;
        public WinCondition winCondition = WinCondition.AllFourHome;
        public float timedRushDuration = 300f;
        public float turnTimeLimit = 30f;

        public bool CanOpenWithRoll(int diceValue)
        {
            return openingRoll switch
            {
                OpeningRollRule.OnlySix => diceValue == 6,
                OpeningRollRule.OneOrSix => diceValue == 1 || diceValue == 6,
                OpeningRollRule.AllOpen => true,
                _ => false
            };
        }

        public bool IsTileSafe(LudoTile tile)
        {
            if (tile.Type == TileType.HomeCorridor || tile.Type == TileType.Goal)
                return true;

            return safeZoneRule switch
            {
                SafeZoneRule.ClassicEightStars =>
                    tile.Type == TileType.SafeStar || tile.Type == TileType.StartingTile,
                SafeZoneRule.StartingTilesOnly =>
                    tile.Type == TileType.StartingTile,
                SafeZoneRule.NoSafeZones =>
                    false,
                _ => false
            };
        }
    }
}