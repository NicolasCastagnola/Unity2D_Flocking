using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class Hunter : MonoBehaviour
{
    private FiniteStateMachine _finiteStateMachine;
    private SpriteRenderer _spriteRenderer;
    public string currentState;

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
    public Transform Target { get { return _target; } }
    private Transform _target;

    private void Start()
    {
        _finiteStateMachine = GetComponent<FiniteStateMachine>();
        _spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();
        _finiteStateMachine.AddState(States.Rest, new Rest(this, _finiteStateMachine));
        _finiteStateMachine.AddState(States.Persuit, new Persuit(this, _finiteStateMachine));
        _finiteStateMachine.AddState(States.Patrol, new Patrol(this, _finiteStateMachine));


        _finiteStateMachine.ChangeState(States.Rest);
    }

    public void SetWaypoints(Transform[] waypoints)
    {
        this.waypoints = waypoints;
    }
    private void Update()
    {
        _finiteStateMachine.OnUpdate();
    }
    public void CheckProximity()
    {
        Collider2D[] proximity = Physics2D.OverlapCircleAll(transform.position, proximityRadius);
        //proximity.PrintCollection();
        var bTarget = proximity.Select(x => x.GetComponent<FlockAgent>()).Where(x => x != null);//.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).ToList().FirstOrDefault();
        bTarget.PrintCollection();
        /*if(bTarget != null)
        {
            _target = bTarget.transform;
            targetAcquiredFlag = true;
        }
        else
        {
            targetAcquiredFlag = false;
        }
        if (bTarget == null)
        {
            proximityRadius += 1f;
        }*/
        /*foreach (Collider2D collider in proximity)
        {
            FlockAgent target = collider.GetComponent<FlockAgent>();

            if(collider.GetComponent<FlockAgent>())
            {
                _target = target.transform;
                targetAcquiredFlag = true;
            }
            else
            {
                targetAcquiredFlag = false;
            }

            if (target == null)
            {
                proximityRadius += 1f;
            
         }*/
    }

    public void Persuit(Vector3 _velocity)
    {
        transform.position += _velocity * Time.deltaTime;
        transform.up = _velocity.normalized;
    }
    public void SetPersuitBehaviour()
    {
        _spriteRenderer.color = Color.red;
        currentState = "PERSUIT";

        if (!targetAcquiredFlag)
        {
            CheckProximity();
        }
        else
        {
            Persuit(CalculateTrajectory(_target));
        }
    }
    public Vector3 CalculateTrajectory(Transform target) {

        if (target != null)
        {
            Vector3 desired = target.position - transform.position;
            desired.Normalize();
            desired *= speed;
            return desired;
        }
        return default;
    }
    public void SetPatrolBehaviour()
    {
        currentState = "PATROL";

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
            Vector3 dir = waypoints[waypointIndex].transform.position - transform.position;
            dir.Normalize();
            transform.up = dir;
            transform.position += dir * speed * Time.deltaTime;
            _spriteRenderer.color = Color.yellow;
            targetAcquiredFlag = false;
        }      
    }

    public void SetRestbehaviour()
    {
        currentState = "REST";

        if (energy <= 0f)
        {
            _spriteRenderer.color = Color.green;
            speed = 0;
            StartCoroutine(TriggerRecovery());
        }
        else
        {
            speed = 5.2f;
        }
    }

    IEnumerator TriggerRecovery()
    {
        
        yield return new WaitForSeconds(_recoveryTime);

         energy = _totalEnergy;
    }
}
