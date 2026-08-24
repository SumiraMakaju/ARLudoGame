using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ARLudo.Core;

namespace ARLudo.AI
{
    public class LudoAIPlayer : MonoBehaviour
    {
        public LudoAgent agent;
        public float thinkDelay = 0.5f;
        public bool useML = true;
        private LudoGameManager gameManager;

        public void Setup(LudoGameManager gm)
        {
            gameManager = gm;
        }

        public IEnumerator MakeMove(int diceValue, List<LudoMoveResult> moves)
        {
            if (thinkDelay > 0) yield return new WaitForSeconds(thinkDelay);

            if (moves == null || moves.Count == 0) yield break;
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
                    // This is the true fix: safely waits for the Python response 
                    yield return new WaitUntil(() => agent.HasSelectedMove);
                    
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

            // Fallback random move
            gameManager.SelectMove(Random.Range(0, moves.Count));
        }
    }
}