using System;
using System.Collections;
using IA2;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;
using TMPro;

[Flags]
public enum HunterStates : byte
{
    None = 0, 
    Rest = 1, 
    Pursuit = 2, 
    Patrol = 3,
    Die = 4
}

//IA2-P3
public class Hunter : GridEntity
{
    private Transform Target;
    private bool _isRecovering;
    
    [ShowInInspector, ReadOnly, TabGroup("States")] public string CurrentStateDisplay => _finiteStateMachine?.Current.Name;
    [ShowInInspector, ReadOnly, TabGroup("States")] private EventFSM<HunterStates> _finiteStateMachine;
    
    [ShowInInspector, ReadOnly, Header("Pursuit Properties"), TabGroup("States")] private float time;
    [SerializeField, TabGroup("States")] private float energyDrainTicks = 0.1f;
    [SerializeField, TabGroup("States")] private float interpolationPeriod = 2f;
    
    [ShowInInspector, ReadOnly, Header("Patrol Properties"), TabGroup("States")] private float patrolCurrentTime = 10f;

    [TabGroup("Waypoints Properties"), ShowInInspector, ReadOnly] private Transform[] _waypoints;
    [ShowInInspector, ReadOnly, TabGroup("Waypoints Properties")] private int waypointIndex;
    [SerializeField, TabGroup("Waypoints Properties")]private float distanceToChangeWaypoint = .2f;
    
    [TabGroup("Hunter Properties"),SerializeField, Required] private SpriteRenderer _spriteRenderer;
    [TabGroup("Hunter Properties"), Range(1f,20f)] public float proximityRadius;
    [TabGroup("Hunter Properties"), Range(0f,10f)] public float speed;
    [TabGroup("Hunter Properties"), Range(0f,10f)] public float recoveryTime = 5f;
    [TabGroup("Hunter Properties"), Range(0f,1f)] public float _totalEnergy = 1;
    [TabGroup("Hunter Properties")] public TMP_Text stateName;
    [ShowInInspector, ReadOnly, TabGroup("Hunter Properties")] public float energy;
    [ShowInInspector, ReadOnly, TabGroup("Hunter Properties")] private bool targetAcquiredFlag;
    
    public Hunter Initialize(Transform[] waypoints)
    {
        GameManager.Instance.SpatialGrid.RegisterEntity(this);
        
        _waypoints = waypoints;
        
        InitializeFSMCoreStates();
        
        return this;
    }
    public void Terminate()
    {
        _finiteStateMachine.OnStateUpdated -= StateUpdated;
        _finiteStateMachine?.Terminate();
    }
    public void Update() => _finiteStateMachine?.Update();
    private void LateUpdate() => _finiteStateMachine?.LateUpdate();
    private void FixedUpdate() => _finiteStateMachine?.FixedUpdate();
    private void InitializeFSMCoreStates()
    {
        var Rest = new State<HunterStates>("Rest");
        var Pursuit = new State<HunterStates>("Pursuit");
        var Patrol = new State<HunterStates>("Patrol");
        var Die = new State<HunterStates>("Die");
        
        StateConfigurer.Create(Rest)
                       .SetTransition(HunterStates.Patrol, Patrol)
                       .SetTransition(HunterStates.Pursuit, Pursuit)
                       .SetCallbacks(RestStateEnter,RestStateUpdate,null,null, RestStateOnExit)
                       .Done();

        StateConfigurer.Create(Pursuit)
                       .SetTransition(HunterStates.Rest, Rest)
                       .SetTransition(HunterStates.Patrol, Patrol)
                       .SetCallbacks(PursuitStateEnter, PursuitStateUpdate,null,null, PursuitStateExit)
                       .Done();
                        
        StateConfigurer.Create(Patrol)
                       .SetTransition(HunterStates.Rest, Rest)
                       .SetTransition(HunterStates.Pursuit, Pursuit)
                       .SetCallbacks(PatrolStateEnter, PatrolStateUpdate, null, null, PatrolStateExit)
                       .Done();
        
        StateConfigurer.Create(Die)
                       .SetCallbacks(DieStateEnter)
                       .Done();

        _finiteStateMachine = new EventFSM<HunterStates>(Patrol);
        _finiteStateMachine.OnStateUpdated += StateUpdated;
        
    }
    private void DieStateEnter(HunterStates obj) => Terminate();
    private void StateUpdated(HunterStates incomingNewState) => stateName.text = incomingNewState.ToString();

    #region RestStateBehaviours
    private void RestStateEnter(HunterStates incomingStateInput)
    {
        if (energy < 0) energy = 0;
    }
    private void RestStateUpdate()
    {
        UpdatePosition();
        
        if (energy > 1) _finiteStateMachine.SendInput(HunterStates.Patrol);

        if (energy <= 0f && !_isRecovering)
        {
            speed = 0;
            _spriteRenderer.color = Color.green;
            StartCoroutine(TriggerRecovery());
        }

    }
    private void RestStateOnExit(HunterStates incomingStateInput)
    { 
    }
    #endregion

    #region PursuitStateBehaviours

    private void PursuitStateEnter(HunterStates incomingStateInput)
    {
    }
    private void PursuitStateUpdate()
    {
        UpdatePosition();

        if (energy == 0)
        {
            _finiteStateMachine.SendInput(HunterStates.Rest);
            return;
        }
        
        if (energy > 0)
        {
            time += Time.deltaTime;

            if (time >= interpolationPeriod)
            {
                time -= interpolationPeriod;
                energy -= energyDrainTicks;
                time = 0;
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
            
            if (!targetAcquiredFlag) CheckProximity();
            
            else Pursuit(CalculateTrajectory(Target));
     
        }
    }
    private void PursuitStateExit(HunterStates obj)
    {
        
    }
    #endregion
    [Button]
    public void SetStateMachineNewState(HunterStates hunterStates) => _finiteStateMachine?.SendInput(hunterStates);

    #region PatrolStateBehaviours

    private void PatrolStateEnter(HunterStates incomingStateInput)
    {
        if (energy <= 0)
        {
            _finiteStateMachine.SendInput(HunterStates.Rest);
            return;
        }
        
        patrolCurrentTime = 10f;
    }
    private void PatrolStateExit(HunterStates incomingStateInput)
    {
        if (energy > 0.2) energy -= 0.2f; 
    }
    private void PatrolStateUpdate()
    {
        patrolCurrentTime -= Time.deltaTime;
       
        if (energy >= 0)
        {
            SetPatrolBehaviour();
            
            UpdatePosition();
            
            if (patrolCurrentTime <= 0)
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
        //---------------IA2-P1------------------
        
        var position = transform.position;

        var context = GameManager.Instance.SpatialGrid.Query(
            position + new Vector3(-proximityRadius, -proximityRadius, 0),
            position + new Vector3(proximityRadius, proximityRadius, 0),
            x =>
            {
                var position2d = x - transform.position;
                position2d.z = 0;
                return position2d.sqrMagnitude < proximityRadius * proximityRadius;
            }).ToList();


        var closestTarget = context.Select(x => x.GetComponent<FlockAgent>())
                               .Where(x => x != null)
                               .OrderBy(x => Vector3.Distance(transform.position, x.transform.position))
                               .FirstOrDefault();

        if(closestTarget != null)
        {
            Target = closestTarget.transform;
            targetAcquiredFlag = true;
        }
        else
        {
            targetAcquiredFlag = false;
            proximityRadius += 1f;
        }
    }
    private void Pursuit(Vector3 _velocity)
    {
        var hunterTransform = transform;
        
        hunterTransform.position += _velocity * Time.deltaTime;
        hunterTransform.up = _velocity.normalized;
    }
    private Vector3 CalculateTrajectory(Transform target)
    {
        if (target == null) return default;
            
        var desired = target.position - transform.position;
        desired.Normalize();
        desired *= speed;
        desired.z = 0;
        
        return desired;
    }
    private void SetPatrolBehaviour()
    {
        if (Vector2.Distance(_waypoints[waypointIndex].position, transform.position) < distanceToChangeWaypoint)
        {
            waypointIndex++;

            if (waypointIndex > _waypoints.Length - 1) waypointIndex = 0;
        }
        else
        {
            var position = transform.position;
            var direction = _waypoints[waypointIndex].transform.position - position;
            
            direction.Normalize();
            transform.up = direction;
            position += direction * (speed * Time.deltaTime);
            position.z = 0;
            transform.position = position;
            _spriteRenderer.color = Color.yellow;
            targetAcquiredFlag = false;
        }      
    }
    private IEnumerator TriggerRecovery()
    {
        _isRecovering = true;
        
        yield return new WaitForSeconds(recoveryTime);
        
        _isRecovering = false;

        energy = _totalEnergy;
        speed = 5.2f;
    }
}