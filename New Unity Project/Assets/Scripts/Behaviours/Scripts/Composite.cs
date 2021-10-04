using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Composite")]
public class Composite : FlockBehaviour
{
    public FlockBehaviour[] behaviours;

    public float[] weights;
    public override Vector2 CalculateMove(FlockAgent _agent, List<Transform> _context, Flock _flock)
    {
        if (weights.Length != behaviours.Length)
        {
            Debug.LogError($"Data missmatch in" + name, this);
            return Vector2.zero;
        }

        Vector2 move = Vector2.zero;

        for (int i = 0; i < behaviours.Length; i++)
        {
            Vector2 partialMove = behaviours[i].CalculateMove(_agent, _context, _flock) * weights[i];

            if (partialMove != Vector2.zero)
            {
                if(partialMove.sqrMagnitude > Mathf.Pow(weights[i], 2))
                {
                    partialMove.Normalize();
                    partialMove *= weights[i];
                }

                move += partialMove;
            }
        }

        return move;
    }
}
