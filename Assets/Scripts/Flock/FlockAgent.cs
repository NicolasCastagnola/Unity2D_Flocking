using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class FlockAgent : GridEntity
{
    [HideInInspector] public Flock agentFlock;
    public Flock AgentFlock => agentFlock;
    public void Initialize(Flock flock) => agentFlock = flock;
    public void Kill() => agentFlock.RemoveAgentFromList(this);
    public void Move(Vector2 velocity)
    {
        var agentTransform = transform;
        
        agentTransform.up = velocity;
        agentTransform.position += (Vector3)velocity * Time.deltaTime;

        UpdatePosition();
    }
    public IEnumerable<GridEntity> GetNearby()
    {
        if (agentFlock == null) return default;

        var position = transform.position;
        
        return agentFlock._spatialGrid.Query(
                position + new Vector3(-agentFlock.neighborRadius, -agentFlock.neighborRadius, 0),
                position + new Vector3(agentFlock.neighborRadius, agentFlock.neighborRadius, 0),
                x => {
                    var position2d = x - transform.position;
                    position2d.z = 0;
                    return position2d.sqrMagnitude < agentFlock.neighborRadius * agentFlock.neighborRadius;
                }).ToList();
    }

}