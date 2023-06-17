using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum States { Rest, Patrol, Persuit }
public class FiniteStateMachine : MonoBehaviour
{
    IState _currentState = new BlankState();

    Dictionary<States, IState> _statesDictionary = new Dictionary<States, IState>();

    public void OnUpdate()
    {
        if (_currentState != null) _currentState.OnUpdate();
    }

    public void ChangeState(States state)
    {
        if (!_statesDictionary.ContainsKey(state)) return;

        _currentState?.OnExit();
        _currentState = _statesDictionary[state];
        _currentState?.OnEnter();

    }

    public void AddState(States id, IState state)
    {
        if (_statesDictionary.ContainsKey(id)) return;

        _statesDictionary.Add(id, state);
    }

    public void RemoveState(States id)
    {
        if (_statesDictionary.ContainsKey(id))
            _statesDictionary.Remove(id);
    }
}

#region Hunter States
public class BlankState : IState
{
    public void OnEnter() { }
    public void OnExit() { }
    public void OnUpdate() { }
}

public class Rest : IState
{
    private Hunter _hunter;
    private FiniteStateMachine _fsm;

    public Rest(Hunter hunter, FiniteStateMachine fsm)
    {
        _hunter = hunter;
        _fsm = fsm;

    }
    public void OnEnter()
    {
        if (_hunter.energy < 0)
        {
            _hunter.energy = 0;
        }
    }
    public void OnUpdate()
    {

        if (_hunter.energy >= 1)
        {
            _fsm.ChangeState(States.Patrol);
        }

        _hunter.SetRestbehaviour();
    }

    public void OnExit()
    {
        _hunter.energy = 1;
    }

}
public class Patrol : IState
{
    private Hunter _hunter;
    private FiniteStateMachine _fsm;
    private float stateTimer;
    public Patrol(Hunter hunter, FiniteStateMachine fsm)
    {
        _hunter = hunter;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        stateTimer = 10f;
    }

    public void OnUpdate()
    {
        stateTimer -= Time.deltaTime;
       
        if (_hunter.energy >= 0)
        {
            _hunter.SetPatrolBehaviour();

            if (stateTimer <= 0)
            {
                _fsm.ChangeState(States.Persuit);
            }
        }     
        else 
        {
            _fsm.ChangeState(States.Rest);
        }
    }

    public void OnExit()
    {
        if (_hunter.energy > 0.2)
        {
            _hunter.energy -= 0.2f; 
        }

    }
}

public class Persuit : IState
{
    private Hunter _hunter;
    private FiniteStateMachine _fsm;
    private float energyDrainTicks;
    private float interpolationPeriod;
    private float time;

    public Persuit(Hunter hunter, FiniteStateMachine fsm)
    {
        _hunter = hunter;
        _fsm = fsm;
    }

    public void OnEnter()
    {
        energyDrainTicks = 0.1f;
        interpolationPeriod = 2f;
        time = 0f;
    }

    public void OnUpdate()
    {
        if (_hunter.energy >= 0)
        {
            time += Time.deltaTime;

            if (time >= interpolationPeriod)
            {
                time -= interpolationPeriod;
                _hunter.energy -= energyDrainTicks;
            }

            if (_hunter.Target)
            {
                if (Vector3.Distance(_hunter.transform.position, _hunter.Target.position) <= 0.2)
                {
                    var a = _hunter.Target.GetComponent<FlockAgent>();
                    a.Kill();
                    _hunter.energy += 0.20f;
                    _hunter.proximityRadius = 2.85f;
                    _fsm.ChangeState(States.Patrol);
                    
                }
            }



            _hunter.SetPersuitBehaviour();
        }
        else
        {
            _fsm.ChangeState(States.Rest);
        }
    }

    public void OnExit()
    {
        
    }
}

#endregion