using System.Collections;
using System.Collections.Generic;
using Flocking;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Avoidance")]
public class Avoidance : FilteredFlockBehaviour
{
    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        if (context.Count == 0)
            return Vector2.zero;

        Vector2 avoidanceMove = Vector2.zero;

        int n_Avoid = 0;

        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);

        foreach (Transform item in filteredContext)
        {
            if (item.GetComponent<Hunter>())
            {
                if (Vector2.SqrMagnitude(item.position - agent.transform.position) < flock.SquareAvoidanceRadius * 15)
                {
                    n_Avoid++;
                    avoidanceMove += (Vector2)(agent.transform.position - item.position);
                }
            }
            if (Vector2.SqrMagnitude(item.position - agent.transform.position) < flock.SquareAvoidanceRadius)
            {
                n_Avoid++;
                avoidanceMove += (Vector2)(agent.transform.position - item.position);
            }        
        }

        if (n_Avoid > 0)
            avoidanceMove /= n_Avoid;

        return avoidanceMove;
    }
}

