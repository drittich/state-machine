namespace StateMachine
{
	public class StateMachine<TEventData>
	{
		public Dictionary<StateTransition<TEventData>, ProcessState> Transitions = new();
		public ProcessState CurrentState { get; private set; }

		public StateMachine()
		{
			CurrentState = ProcessState.Inactive;
		}

		public ProcessState GetNext(Event command, TEventData parameter)
		{
			var transitionKey = Transitions.Keys.Where(t => t.CurrentState == CurrentState && t.Command == command).SingleOrDefault();
			
			if (transitionKey is null)
				throw new Exception("Invalid transition: " + CurrentState + " -> " + command);

			transitionKey.MethodToExecute(parameter);
			CurrentState = Transitions[transitionKey];
			
			return CurrentState;
		}

	}
}