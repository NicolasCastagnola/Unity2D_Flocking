using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static Utils;

public class Flock : MonoBehaviour
{
    public List<FlockAgent> GetTotalAgents { get { return agents; } }
    public FlockAgent agentPrefab;
    List<FlockAgent> agents = new List<FlockAgent>();
    public FlockBehaviour behavior;

    [SerializeField] Composite weightController;

    [Range(10, 500)]
    public int startingCount = 150;
    const float AgentDensity = 0.08f;

    [Range(1f, 100f)]
    public float driveFactor = 10f;
    [Range(5f, 100f)]
    public float maxSpeed = 5f;
    [Range(1f, 2f)]
    public float neighborRadius = 1.5f;
    [Range(0f, 5f)]
    public float avoidanceRadiusMultiplier = 0.5f;
    [Range(0f, 1f)]
    public float seekRadiusMultiplier = 0.5f;


    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;
    float squareSeekRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }
    public float SquareSeekRadius { get { return squareSeekRadius; } }


    #region Slider Setters

    #region UI

    [SerializeField] Slider quantitySlider;
    [SerializeField] Slider speedSlider;
    [SerializeField] Slider neighbourRadiusSlider;
    [SerializeField] Slider avoidanceRadiusSlider;
    [SerializeField] Slider driveFactorSlider;

    #endregion

    #region Stats
    public void SetSpeedValue()
    {
        maxSpeed = speedSlider.value;
    }
    public void SetAlingmentValue()
    {
        neighborRadius = alignmentSlider.value;
    }
    public void SetAvoidanceValue()
    {
        avoidanceRadiusMultiplier = avoidanceRadiusSlider.value;
    }
    public void SetCohesionValue()
    {
        driveFactor = driveFactorSlider.value;
        Debug.Log(driveFactorSlider.value);
    }

    public void SetQuantityValue()
    {
        startingCount = (int)quantitySlider.value;
    }
    #endregion

    #region Weights
    [SerializeField] Slider cohesionSlider;
    [SerializeField] Slider alignmentSlider;
    [SerializeField] Slider avoidanceSlider;
    [SerializeField] Slider seekSlider;
    [SerializeField] Slider fleeSlider;
    public void SetCohesionWeight()
    {
        weightController.weights[0] = cohesionSlider.value;
        Debug.Log(cohesionSlider.value);
    }
    public void SetAlignmentWeight()
    {
        weightController.weights[1] = alignmentSlider.value;
    }
    public void SetAvoidanceWeight()
    {
        weightController.weights[2] = avoidanceSlider.value;
    }
    public void SetSeekWeight()
    {
        weightController.weights[4] = cohesionSlider.value;
    }
    public void SetFleeWeight()
    {
        weightController.weights[5] = cohesionSlider.value;
    }
    #endregion

    #endregion


    void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;
        squareSeekRadius = Mathf.Pow(seekRadiusMultiplier, 2);

        Spawn((int)quantitySlider.value);
    }

    public void ResetAgents()
    {
        RemoveAllAgents();
        Spawn((int)quantitySlider.value);
        GameManager.Instance.agentsDisplay.text = agents.Count.ToString();
    }


    public void Spawn(int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            FlockAgent newAgent = Instantiate(
                agentPrefab,
                Random.insideUnitCircle * quantity * AgentDensity,
                Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
                transform
                );
            newAgent.name = "Agent " + i;
            newAgent.Initialize(this);
            agents.Add(newAgent);
        }
    }

    public void RemoveAgentFromList(FlockAgent agent)
    {
        agents.Remove(agent);
    }

    public void RemoveAllAgents()
    {
        for (int i = startingCount - 1; i > 0; i--)
        {
            agents[i].Kill();
        }

        agents[0].Kill();
    }


    void Update()
    {
        foreach (FlockAgent agent in agents)
        {
            List<Transform> context = GetNearbyObjects(agent);
            Vector2 move = behavior.CalculateMove(agent, context, this);
            move *= driveFactor;
            if (move.sqrMagnitude > squareMaxSpeed)
            {
                move = move.normalized * maxSpeed;
            }

            agent.Move(move);
        }
    }


    //---------------IA2-P1------------------
    List<Transform> GetNearbyObjects(FlockAgent agent)
    {
        //List<Transform> context = new List<Transform>();



        /*foreach (Collider2D c in contextColliders)
        {
            if (c != agent.AgentCollider)
            {
                context.Add(c.transform);
            }
        }*/

        //return context;

        Collider2D[] contextColliders = Physics2D.OverlapCircleAll(agent.transform.position, neighborRadius);
        return contextColliders.Where(c => c != agent.AgentCollider).Select(c => c.transform).ToList();
    }
}
