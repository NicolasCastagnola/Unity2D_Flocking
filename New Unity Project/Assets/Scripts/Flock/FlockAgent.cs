using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class FlockAgent : MonoBehaviour
{
    private Flock _agentFlock;
    public Flock AgentFlock { get { return _agentFlock; } }

    private Collider2D _agentCollider;
    public Collider2D AgentCollider {get { return _agentCollider; } }

    private void Start() { _agentCollider = GetComponent<Collider2D>(); }
    public void Initialize(Flock flock) { _agentFlock = flock; }

    public void Move(Vector2 _velocity)
    {
        transform.up = _velocity;
        transform.position += (Vector3)_velocity * Time.deltaTime;
    }
}
