using System.Collections;
using System.Collections.Generic;
using Flocking;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Alignment")]
public class Alignment : FilteredFlockBehaviour
{
    public override Vector2 CalculateMove(FlockAgent agent, List<Transform> context, Flock flock)
    {
        if (context.Count == 0)
            return agent.transform.up;

        Vector2 alignmentMove = Vector2.zero;

        List<Transform> filteredContext = (filter == null) ? context : filter.Filter(agent, context);

        foreach (Transform item in filteredContext)
        {
            alignmentMove += (Vector2)item.transform.up;
        }

        alignmentMove /= context.Count;

        return alignmentMove;
    }
}
