using System;

namespace IA2
{
	public class EventFSM<T>
	{
		public event Action<T> OnStateUpdated;

		public State<T> Current { get { return current; } }
		
		private State<T> current;
		
		public EventFSM(State<T> initial)
        {
			current = initial;
			current.Enter(default(T));
		}
		
		public void Terminate(){}
		public void SendInput(T input)
		{
			if (!current.CheckInput(input, out var newState)) return;
				
			current.Exit(input);
			current = newState;
			current.Enter(input);
			
			OnStateUpdated?.Invoke(input);
		}

		public void Update() => current.Update();
		public void LateUpdate() => current.LateUpdate();
		public void FixedUpdate() => current.FixedUpdate();
    }
}