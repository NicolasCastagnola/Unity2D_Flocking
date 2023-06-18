using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
public class FlockAgent : GridEntity
{
    [HideInInspector] public Flock agentFlock;
    public Flock AgentFlock => agentFlock;
    // public Collider2D AgentCollider { get; private set; }
    // private void Start() => AgentCollider = GetComponent<Collider2D>();
    public void Initialize(Flock flock) => agentFlock = flock;
    public void Kill() 
    {
        OnDestroy(this);
        agentFlock.RemoveAgentFromList(this);
    }

    public Action<GridEntity> OnDestroy = delegate { };
    public void Move(Vector2 velocity)
    {
        var agentTransform = transform;
        
        agentTransform.up = velocity;
        agentTransform.position += (Vector3)velocity * Time.deltaTime;

        MoveCallback();
    }
    public IEnumerable<GridEntity> GetNearby()
    {
        if (agentFlock == null) return null;

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