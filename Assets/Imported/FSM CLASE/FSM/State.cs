using System;
using System.Collections.Generic;
using UnityEngine;

namespace IA2
{
	public class State<T>
    {
		public string Name => _stateName;
		public event Action<T> OnEnter = delegate {};
		public event Action OnUpdate = delegate {};
        public event Action OnLateUpdate = delegate { };
        public event Action OnFixedUpdate = delegate { };
		public event Action<T> OnExit = delegate {};

		private readonly string _stateName;
		private Dictionary<T, Transition<T>> transitions;

		public State(string name) => _stateName = name;
		public State<T> Configure(Dictionary<T, Transition<T>> transitions)
        {
			this.transitions = transitions;
			
			return this;
		}

		public Transition<T> GetTransition(T input)
        {
			return transitions[input];
	    }

		public bool CheckInput(T input, out State<T> next)
        {
			if(transitions.TryGetValue(input, out var transition))
			{
				transition.OnTransitionExecute(input);
				next = transition.TargetState;
				
				return true;
			}

			next = this;
			return false; 
        }
		public void SetCallbacks(Action<T> OnEnter = null, Action OnUpdate = null, Action OnLateUpdate = null, Action OnFixedUpdate = null, Action<T> OnExit = null)
		{
			this.OnEnter = OnEnter;
			this.OnUpdate = OnUpdate;
			this.OnLateUpdate = OnLateUpdate;
			this.OnFixedUpdate = OnFixedUpdate;
			this.OnExit = OnExit;
		}
		public void Enter(T input) => OnEnter?.Invoke(input);
		public void Update() => OnUpdate?.Invoke();
		public void LateUpdate() => OnLateUpdate?.Invoke();
		public void FixedUpdate() => OnFixedUpdate?.Invoke();
		public void Exit(T input) => OnExit?.Invoke(input);
    }
}