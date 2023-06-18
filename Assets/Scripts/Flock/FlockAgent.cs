using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
public class FlockAgent : GridEntity
{
    [HideInInspector] public Flock agentFlock;
    public Flock AgentFlock => agentFlock;
    public Collider2D AgentCollider { get; private set; }
    private void Start() => AgentCollider = GetComponent<Collider2D>();
    public void Initialize(Flock flock)
    {
        grid = GetComponentInParent<SpatialGrid>();
        agentFlock = flock;
        //grid.AddEntity(this);
    }
    
    public void Kill() 
    {
       // OnDestroy(this);
       // OnDestroy -= grid.RemoveEntity;
        agentFlock.RemoveAgentFromList(this);
    }
    public void Move(Vector2 velocity)
    {
        transform.up = velocity;
        transform.position += (Vector3)velocity * Time.deltaTime;

        MoveCallback();
    }

    public List<GridEntity> GetNearby()
    {
        if (agentFlock == null) return null;
        return grid.Query(
                transform.position + new Vector3(-agentFlock.neighborRadius, -agentFlock.neighborRadius, 0),
                transform.position + new Vector3(agentFlock.neighborRadius, agentFlock.neighborRadius, 0),
                x => {
                    var position2d = x - transform.position;
                    position2d.z = 0;
                    return position2d.sqrMagnitude < agentFlock.neighborRadius * agentFlock.neighborRadius;
                }).ToList();
    }

}