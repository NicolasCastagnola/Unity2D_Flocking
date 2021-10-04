using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/Avoidance")]
public class Avoidance : FilteredFlockBehaviour
{
    public override Vector2 CalculateMove(FlockAgent _agent, List<Transform> _context, Flock _flock)
    {
        if (_context.Count == 0)
            return Vector2.zero;

        Vector2 avoidanceMove = Vector2.zero;

        int n_Avoid = 0;

        List<Transform> filteredContext = (filter == null) ? _context : filter.Filter(_agent, _context);

        foreach (Transform item in _context)
        {
            if (Vector2.SqrMagnitude(item.position - _agent.transform.position) < _flock.SquareAvoidanceRadius )
            {
                n_Avoid++;
                avoidanceMove += (Vector2)(item.transform.position - item.position) ;
            }
        }

        if (n_Avoid > 0)       
            avoidanceMove /= n_Avoid;
        
        return avoidanceMove;
    }
}

