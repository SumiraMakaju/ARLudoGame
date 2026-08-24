using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARLudo.Core;

namespace ARLudo.AI
{
    public class LudoAIPlayer : MonoBehaviour
    {
        public LudoAgent agent;
        public float thinkDelay = 0.8f;
        public bool useML = true;

        private LudoGameManager gameManager;

        public void Setup(LudoGameManager gm)
        {
            gameManager = gm;
        }

        public IEnumerator MakeMove(int diceValue, List<LudoMoveResult> moves)
        {
            yield return new WaitForSeconds(thinkDelay);

            if (moves == null || moves.Count == 0)
                yield break;

            if (moves.Count == 1)
            {
                gameManager.SelectMove(0);
                yield break;
            }

            if (useML && agent != null)
            {
                int result = agent.RequestMove(diceValue);
                if (result == -2)
                {
                    yield return new WaitForSeconds(0.1f);
                    int selected = agent.GetSelectedMove();
                    if (selected >= 0 && selected < moves.Count)
                    {
                        gameManager.SelectMove(selected);
                        yield break;
                    }
                }
                else if (result >= 0)
                {
                    gameManager.SelectMove(result);
                    yield break;
                }
            }

            int pick = UtilityFallback(moves);
            gameManager.SelectMove(pick);
        }

        private int UtilityFallback(List<LudoMoveResult> moves)
        {
            int best = 0;
            float bestScore = float.MinValue;

            for (int i = 0; i < moves.Count; i++)
            {
                float score = 0;
                if (moves[i].IsCapture) score += 100;
                if (moves[i].IsReachingGoal) score += 120;
                if (moves[i].IsEnteringHomeCorridor) score += 60;
                if (moves[i].IsExitingYard) score += 50;
                score += Random.Range(0f, 10f);

                if (score > bestScore)
                {
                    bestScore = score;
                    best = i;
                }
            }
            return best;
        }
    }
}