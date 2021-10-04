using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    const float AGENT_DENSITY = 0.08f;

    private List<FlockAgent> _agents = new List<FlockAgent>();

    public FlockAgent agentPrefab;

    public FlockBehaviour behaviour;

    [Header("Properties")]
    [Range(10, 500)]
    public int startingCount = 250;
    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(1f, 100f)]
    public float maxSpeed = 5f;
    [Range(1f, 10f)]
    public float neighborRadius = 1.5f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    private float squareMaxSpeed;
    private float squareNeighborRadius;
    private float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }

    void Start()
    {
        squareMaxSpeed = Mathf.Pow(maxSpeed, 2);
        squareNeighborRadius = Mathf.Pow(neighborRadius, 2);
        squareAvoidanceRadius = squareNeighborRadius * Mathf.Pow(SquareAvoidanceRadius, 2);

        for (int i = 0; i < startingCount; i++)
        {
            FlockAgent newAgent = Instantiate
                (agentPrefab,
                Random.insideUnitCircle * startingCount * AGENT_DENSITY,
                Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
                transform);
            
            newAgent.name = "Agent " + i;
            newAgent.Initialize(this);
            _agents.Add(newAgent);
        }
    }

    void Update()
    {
        foreach (FlockAgent agent in _agents)
        {
            List<Transform> context = GetNearbyObjects(agent);

            //agent.GetComponentInChildren<SpriteRenderer>().color = Color.Lerp(Color.white, Color.red, context.Count / 6f);

            Vector2 move = behaviour.CalculateMove(agent, context, this);
            move *= driveFactor;

            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }

            agent.Move(move);
        }
    }

    List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        List<Transform> context = new List<Transform>();
        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighborRadius);

        foreach (Collider2D collider in contextColliders)
        {
            if (collider != agent.AgentCollider) 
                context.Add(collider.transform);
        }

        return context;
    }   
}
