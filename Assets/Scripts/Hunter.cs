using System;
using System.Collections;
using IA2;
using Sirenix.OdinInspector;
using UnityEngine;

[Flags]
public enum HunterStates : ushort { None, Rest, Pursuit, Patrol }

public class Hunter : MonoBehaviour
{
    //Pursuit stuff
    private float energyDrainTicks = 0.1f;
    private float interpolationPeriod = 2f;
    private float time = 0f;
    //patrol stuff
    private float stateTimer = 10f;


  
    private EventFSM<HunterStates> _finiteStateMachine;
    private SpriteRenderer _spriteRenderer;
    public string currentStateDisplay;

    public Transform[] waypoints;
    private int waypointIndex = 0;
    private float distanceToChangeWaypoint = .2f;
    
    [Range(0f,10f)]
    public float speed;

    [Range(0f,1f)]
    public float _totalEnergy = 1;
    public float energy;
    private float _recoveryTime = 5f;
    
    [Range(1f,20f)]
    public float proximityRadius;

    public bool targetAcquiredFlag = false;
    public Transform Target { get; private set; }

    private void Awake()
    {
        _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        
        InitializeFSMCoreStates();
    }
    public void Update() => _finiteStateMachine?.Update();
    private void LateUpdate() => _finiteStateMachine?.LateUpdate();
    private void FixedUpdate() => _finiteStateMachine?.FixedUpdate();
    private void OnDestroy() => _finiteStateMachine?.Terminate();
    private void InitializeFSMCoreStates()
    {
        var Rest = new State<HunterStates>("Rest");
        var Pursuit = new State<HunterStates>("Pursuit");
        var Patrol = new State<HunterStates>("Patrol");
        
        StateConfigurer.Create(Rest)
                       .SetTransition(HunterStates.Patrol, Patrol)
                       .SetTransition(HunterStates.Pursuit, Pursuit)
                       .SetCallbacks(RestStateEnter,RestStateUpdate,null,null, RestStateOnExit)
                       .Done();

        StateConfigurer.Create(Pursuit)
                       .SetTransition(HunterStates.Rest, Rest)
                       .SetTransition(HunterStates.Patrol, Patrol)
                       .SetCallbacks(PursuitStateEnter, PursuitStateUpdate)
                       .Done();
                        
        StateConfigurer.Create(Patrol)
                       .SetTransition(HunterStates.Rest, Rest)
                       .SetTransition(HunterStates.Pursuit, Pursuit)
                       .SetCallbacks(PatrolStateEnter, PatrolStateUpdate, null, null, PatrolStateExit)
                       .Done();

        _finiteStateMachine = new EventFSM<HunterStates>(Rest);
    }

    #region RestStateBehaviours
    private void RestStateEnter(HunterStates incomingStateInput)
    {
        if (energy < 0) energy = 0;
    }
    private void RestStateUpdate()
    {
        if (energy >= 1)
            _finiteStateMachine.SendInput(HunterStates.Patrol);

        currentStateDisplay = "REST";

        if (energy <= 0f)
        {
            speed = 0;
            _spriteRenderer.color = Color.green;
            StartCoroutine(TriggerRecovery());
        }
        else speed = 5.2f;
    }
    private void RestStateOnExit(HunterStates incomingStateInput) => energy = 1;
    #endregion

    #region PursuitStateBehaviours

    private void PursuitStateEnter(HunterStates incomingStateInput)
    {
        energyDrainTicks = 0.1f;
        interpolationPeriod = 2f;
        time = 0f;
    }
    private void PursuitStateUpdate()
    {
        if (energy >= 0)
        {
            time += Time.deltaTime;

            if (time >= interpolationPeriod)
            {
                time -= interpolationPeriod;
                energy -= energyDrainTicks;
            }

            if (Target)
            {
                if (Vector3.Distance(transform.position, Target.position) <= 0.2)
                {
                    var a = Target.GetComponent<FlockAgent>();
                    a.Kill();
                    energy += 0.20f;
                    proximityRadius = 2.85f;
                    _finiteStateMachine.SendInput(HunterStates.Patrol);
                    
                }
            }

            _spriteRenderer.color = Color.red;
        
            currentStateDisplay = "PURSUIT";

            if (!targetAcquiredFlag)
            {
                CheckProximity();
            }
            else
            {
                Pursuit(CalculateTrajectory(Target));
            }
        }
        else
        {
            _finiteStateMachine.SendInput(HunterStates.Rest);
        }
    }


    #endregion

    #region PatrolStateBehaviours

    private void PatrolStateEnter(HunterStates incomingStateInput) => stateTimer = 10f;
    private void PatrolStateExit(HunterStates incomingStateInput)
    {
        if (energy > 0.2)
        {
            energy -= 0.2f; 
        }
    }
    private void PatrolStateUpdate()
    {
        stateTimer -= Time.deltaTime;
       
        if (energy >= 0)
        {
            SetPatrolBehaviour();

            if (stateTimer <= 0)
            {
                _finiteStateMachine.SendInput(HunterStates.Pursuit);
            }
        }     
        else 
        {
            _finiteStateMachine.SendInput(HunterStates.Rest);
        }
    }

  #endregion
    private void CheckProximity()
    {
        var proximity = Physics2D.OverlapCircleAll(transform.position, proximityRadius);

        foreach (var col in proximity)
        {
            var target = col.GetComponent<FlockAgent>();

            if(col.GetComponent<FlockAgent>())
            {
                Target = target.transform;
                targetAcquiredFlag = true;
            }
            else targetAcquiredFlag = false;

            if (target == null)
                proximityRadius += 1f;
        }
    }
    public void SetWaypoints(Transform[] targetWaypoints) => waypoints = targetWaypoints;
    private void Pursuit(Vector3 _velocity)
    {
        var hunterTransform = transform;
        
        hunterTransform.position += _velocity * Time.deltaTime;
        hunterTransform.up = _velocity.normalized;
    }
    private Vector3 CalculateTrajectory(Transform target) {

        if (target == null) return default;
            
        var desired = target.position - transform.position;
        desired.Normalize();
        desired *= speed;
        return desired;

    }
    public void SetPatrolBehaviour()
    {
        currentStateDisplay = "PATROL";

        if (Vector2.Distance(waypoints[waypointIndex].position, transform.position) < distanceToChangeWaypoint)
        {
            waypointIndex++;

            if (waypointIndex > waypoints.Length - 1)
            {
                waypointIndex = 0;
            }
        }
        else
        {
            var position = transform.position;
            var direction = waypoints[waypointIndex].transform.position - position;
            
            direction.Normalize();
            transform.up = direction;
            position += direction * (speed * Time.deltaTime);
            transform.position = position;
            _spriteRenderer.color = Color.yellow;
            targetAcquiredFlag = false;
        }      
    }
    private IEnumerator TriggerRecovery()
    {
        yield return new WaitForSeconds(_recoveryTime);

        energy = _totalEnergy;
    }
}