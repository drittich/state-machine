namespace state_machine_poc
{
	public class StateMachine<T>
	{
		public Dictionary<StateTransition<T>, ProcessState> Transitions = new();
		public ProcessState CurrentState { get; private set; }

		public StateMachine()
		{
			CurrentState = ProcessState.Inactive;
		}

		public ProcessState GetNext(Event command, T parameter)
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