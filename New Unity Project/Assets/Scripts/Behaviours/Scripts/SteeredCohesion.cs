using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/SteeredCohesion")]
public class SteeredCohesion : FilteredFlockBehaviour
{
    private Vector2 _currentVelocity;
    public float agentSmoothTime = 0.3f;

    public override Vector2 CalculateMove(FlockAgent _agent, List<Transform> _context, Flock _flock)
    {
        if (_context.Count == 0) { return Vector2.zero; }

        Vector2 cohesionMove = Vector2.zero;

        List<Transform> filteredContext = (filter == null) ? _context : filter.Filter(_agent, _context);

        foreach (Transform item in _context)
        {
            cohesionMove += (Vector2)item.position;
        }

        cohesionMove /= _context.Count;

        cohesionMove -= (Vector2)_agent.transform.position;

        cohesionMove = Vector2.SmoothDamp(_agent.transform.up, cohesionMove, ref _currentVelocity, agentSmoothTime);

        return cohesionMove;
    }
}
