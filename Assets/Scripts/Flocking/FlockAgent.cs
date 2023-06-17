using UnityEngine;

namespace Flocking
{
    [RequireComponent(typeof(Collider2D))]
    public class FlockAgent : GridEntity
    {
        [HideInInspector] public Flock agentFlock;
        public Flock AgentFlock => agentFlock;
        public Collider2D AgentCollider { get; private set; }
        private void Start() => AgentCollider = GetComponent<Collider2D>();
        public void Initialize(Flock flock) => agentFlock = flock;
        public void Kill() => agentFlock.RemoveAgentFromList(this);
        public void Move(Vector2 velocity)
        {
            transform.up = velocity;
            transform.position += (Vector3)velocity * Time.deltaTime;

            MoveCallback();
        }
    }
}
