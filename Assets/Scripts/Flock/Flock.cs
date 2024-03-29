﻿using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Flock : MonoBehaviour
{
    private const float AGENT_DENSITY = 0.08f;
    private const int STARTING_VALUE = 100;

    [SerializeField] private Composite weightController;
    public List<FlockAgent> GetTotalAgents { get; } = new List<FlockAgent>();
    public FlockAgent agentPrefab;
    public FlockBehaviour behavior;
    public SpatialGrid SpatialGrid;

    [Range(10, 500)] public int currentCount = 150;
    [Range(1f, 100f)] public float driveFactor = 10f;
    [Range(5f, 100f)] public float maxSpeed = 5f;
    [Range(1f, 2f)] public float neighborRadius = 1.5f;
    [Range(0f, 5f)] public float avoidanceRadiusMultiplier = 0.5f;
    [Range(0f, 1f)] public float seekRadiusMultiplier = 0.5f;

    private float squareMaxSpeed;
    private float squareNeighborRadius;
    public float SquareAvoidanceRadius {
        get;
        private set;
    }
    public float SquareSeekRadius {
        get;
        private set;
    }


    public void SpawnBoids(int amount)
    {
        
    }
    
    #region Slider Setters

    #region UI


    [SerializeField] Slider speedSlider;
    [SerializeField] Slider neighbourRadiusSlider;
    [SerializeField] Slider avoidanceRadiusSlider;
    [SerializeField] Slider driveFactorSlider;

    #endregion

    #region Stats
    public void SetAlingmentValue() => neighborRadius = neighbourRadiusSlider.value;
    public void SetAvoidanceValue() => avoidanceRadiusMultiplier = avoidanceRadiusSlider.value;
    public void SetCohesionValue() => driveFactor = driveFactorSlider.value;

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
    private void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborRadius * neighborRadius;
        SquareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;
        SquareSeekRadius = Mathf.Pow(seekRadiusMultiplier, 2);
        
        Spawn(STARTING_VALUE);
        
        //IA2-P2
        SpatialGrid.Initialize();
    }
    public void ResetAgents()
    {
        RemoveAllAgents();
        Spawn((STARTING_VALUE));
        GameManager.Instance.agentsDisplay.text = GetTotalAgents.Count.ToString();
    }
    private void Spawn(int quantity)
    {
        for (var i = 0; i < quantity; i++)
        {
            var newAgent = Instantiate(
                agentPrefab,
                Random.insideUnitCircle * quantity * AGENT_DENSITY,
                Quaternion.Euler(Vector3.forward * Random.Range(0f, 360f)),
                transform
            );
            
            newAgent.GetComponent<Queries>().Initialize(SpatialGrid);
            newAgent.name = "Agent " + i;
            newAgent.Initialize(this);
            
            GetTotalAgents.Add(newAgent);
        }
    }
    public void RemoveAgentFromList(FlockAgent agent)
    {
        agent.gameObject.SetActive(false);
        agent.Terminate();
        GetTotalAgents.Remove(agent);
    }
    private void RemoveAllAgents()
    {
        for (int i = STARTING_VALUE - 1; i > 0; i--)
        {
            GetTotalAgents[i].Kill();
        }

        GetTotalAgents[0].Kill();
    }
    private void Update()
    {
        foreach (var agent in GetTotalAgents)
        {
            var context = agent.GetNearby().Select(c => c.transform).ToList();
            {
                var move = behavior.CalculateMove(agent, context, this);
            
                move *= driveFactor;
            
                if (move.sqrMagnitude > squareMaxSpeed) move = move.normalized * maxSpeed;
                    
                agent.Move(move);
            }
        }
    }

}