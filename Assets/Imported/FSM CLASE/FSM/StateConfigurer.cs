using System;
using System.Collections.Generic;

namespace IA2 {
	public class StateConfigurer<T>
    {
	    private readonly State<T> instance;
	    private readonly Dictionary<T, Transition<T>> transitions = new Dictionary<T, Transition<T>>();
		public StateConfigurer(State<T> state) => instance = state;
		public StateConfigurer<T> SetTransition(T input, State<T> target)
		{
			transitions.Add(input, new Transition<T>(input, target));
			
			return this;
		}
		public void Done() => instance.Configure(transitions);
		public StateConfigurer<T> SetCallbacks(Action<T> OnEnter = null, Action OnUpdate = null, Action OnLateUpdate = null, Action OnFixedUpdate = null, Action<T> OnExit = null)
		{
			instance.SetCallbacks(OnEnter, OnUpdate, OnFixedUpdate, OnLateUpdate, OnExit);

			return this;
		}
    }

	public static class StateConfigurer
    {
		public static StateConfigurer<T> Create<T>(State<T> state)
        {
			return new StateConfigurer<T>(state);
		}
	}
}