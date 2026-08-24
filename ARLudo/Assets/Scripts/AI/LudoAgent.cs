using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Collections.Generic;
using UnityEngine;
using ARLudo.Core;

namespace ARLudo.AI
{
    public class LudoAgent : Agent
    {
        public PlayerColor agentColor;
        private LudoBoard board;
        private LudoRules rules;
        private List<LudoPlayer> players;
        private LudoMoveValidator validator;
        private LudoPlayer agentPlayer;
        private int lastDiceValue;
        private List<LudoMoveResult> currentMoves;
        
        private int selectedMoveIndex = -1;
        public bool HasSelectedMove => selectedMoveIndex != -1;

        public void ResetMoveSelection()
        {
            selectedMoveIndex = -1;
        }

        public void Setup(LudoBoard board, LudoRules rules, List<LudoPlayer> players)
        {
            this.board = board;
            this.rules = rules;
            this.players = players;
            validator = new LudoMoveValidator(board, rules, players);

            foreach (var p in players)
            {
                if (p.Color == agentColor) { agentPlayer = p; break; }
            }
        }

        public int RequestMove(int diceValue)
        {
            lastDiceValue = diceValue;
            currentMoves = validator.GetLegalMoves(agentPlayer, diceValue);

            if (currentMoves.Count == 0) return -1;
            if (currentMoves.Count == 1) return 0;

            selectedMoveIndex = -1;
            RequestDecision();
            return -2;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(lastDiceValue / 6f);
            foreach (var pawn in agentPlayer.Pawns)
            {
                sensor.AddObservation((int)pawn.State / 2f);
                sensor.AddObservation(pawn.TilesTraveled / 57f);
            }
            foreach (var player in players)
            {
                if (player.Color == agentColor) continue;
                foreach (var pawn in player.Pawns)
                {
                    sensor.AddObservation((int)pawn.State / 2f);
                    sensor.AddObservation(pawn.TilesTraveled / 57f);
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (i < currentMoves.Count)
                {
                    sensor.AddObservation(1f);
                    sensor.AddObservation(currentMoves[i].IsExitingYard ? 1f : 0f);
                    sensor.AddObservation(currentMoves[i].IsCapture ? 1f : 0f);
                    sensor.AddObservation(currentMoves[i].IsReachingGoal ? 1f : 0f);
                }
                else
                {
                    sensor.AddObservation(0f);
                    sensor.AddObservation(0f);
                    sensor.AddObservation(0f);
                    sensor.AddObservation(0f);
                }
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            int choice = actions.DiscreteActions[0];
            if (choice >= currentMoves.Count) choice = Random.Range(0, currentMoves.Count);
            selectedMoveIndex = choice;
        }

        public int GetSelectedMove()
        {
            int move = selectedMoveIndex;
            selectedMoveIndex = -1;
            return move;
        }

        public void RewardWin() => AddReward(1.0f);
        public void PenalizeLoss() => AddReward(-1.0f);
    }
}