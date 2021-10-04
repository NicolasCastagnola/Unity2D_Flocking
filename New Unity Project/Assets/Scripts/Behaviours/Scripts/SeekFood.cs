using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/SeekFood")]

public class SeekFood : FlockBehaviour
{
    public override Vector2 CalculateMove(FlockAgent _agent, List<Transform> _context, Flock _flock)
    {
        if (_context.Count == 0)
            return Vector2.zero;

        Vector2 cohesionMove = Vector2.zero;

        foreach (Transform item in _context)
        {
            cohesionMove += (Vector2)item.position;
        }

        cohesionMove /= _context.Count;

        cohesionMove -= (Vector2)_agent.transform.position;

        return cohesionMove;
    }
}
