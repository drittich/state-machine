namespace StateMachine
{
	public class StateMachine<TEventData>
	{
		public Dictionary<Transition<TEventData>, State> Transitions = new();
		public State CurrentState { get; private set; }

		public StateMachine()
		{
			CurrentState = State.Inactive;
		}

		public State GetNext(Event command, TEventData parameter)
		{
			var transitionKey = Transitions.Keys.Where(t => t.CurrentState == CurrentState && t.Command == command).SingleOrDefault();
			
			if (transitionKey is null)
				throw new Exception("Invalid transition: " + CurrentState + " -> " + command);

			transitionKey.Action(parameter);
			CurrentState = Transitions[transitionKey];
			
			return CurrentState;
		}
	}
}