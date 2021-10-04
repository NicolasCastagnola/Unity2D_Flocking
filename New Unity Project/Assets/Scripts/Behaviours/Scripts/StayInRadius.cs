using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flock/Behaviour/StayinRadius")]

public class StayInRadius : FlockBehaviour
{
    public Vector2 center;
    public float radius = 15f;
    public override Vector2 CalculateMove(FlockAgent _agent, List<Transform> _context, Flock _flock)
    {
        Vector2 centerOffset = center - (Vector2)_agent.transform.position;

        float t = centerOffset.magnitude / radius;

        if (t < 0.9f) { return Vector2.zero; }
            
        return centerOffset * (Mathf.Pow(t, 2));       
    }
}
