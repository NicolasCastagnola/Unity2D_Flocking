using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
[CreateAssetMenu(menuName = "Flock/Behaviour/Composite")]
public class Composite : FlockBehaviour
{
    public FlockBehaviour[] behaviours;
    [Range(0.1f, 10f)]
    public float[] weights;

    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        if (weights.Length != behaviours.Length)
        {
            Debug.LogError("Data mismatch in " + name, this);
            return Vector2.zero;
        }

        //Vector2 move = Vector2.zero;

        //---------------IA2-P1------------------

        Tuple<Vector2, int> move = behaviours.Aggregate(Tuple.Create(Vector2.zero, 0), (pos, tar) =>
        {
            Vector2 partialMove = tar.CalculateMove(agent, context, flock) * weights[pos.Item2];

            if (partialMove != Vector2.zero)
            {
                if (partialMove.sqrMagnitude > weights[pos.Item2] * weights[pos.Item2])
                {
                    partialMove.Normalize();
                    partialMove *= weights[pos.Item2];
                }
            }
            return Tuple.Create(pos.Item1 + partialMove, pos.Item2 + 1);
        });

        /*for (int i = 0; i < behaviours.Length; i++)
        {
            Vector2 partialMove = behaviours[i].CalculateMove(agent, context, flock) * weights[i];

            if (partialMove != Vector2.zero)
            {
                if (partialMove.sqrMagnitude > weights[i] * weights[i])
                {
                    partialMove.Normalize();
                    partialMove *= weights[i];
                }

                move += partialMove;
            }
        }*/

        return move.Item1;
    }
}
