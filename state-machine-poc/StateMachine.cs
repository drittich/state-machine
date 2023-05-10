namespace state_machine_poc
{
	public class StateMachine
	{
		private Dictionary<StateTransition, ProcessState> transitions;
		public ProcessState CurrentState { get; private set; }

		public StateMachine()
		{
			CurrentState = ProcessState.Inactive;
			transitions = new Dictionary<StateTransition, ProcessState>
			{
				{ new StateTransition(ProcessState.Inactive, Event.Exit, SomeMethodToExecute), ProcessState.Terminated },
				{ new StateTransition(ProcessState.Inactive, Event.Begin, SomeMethodToExecute), ProcessState.Active },
				// ... other transitions
			};
		}

		public ProcessState GetNext(Event command, BattleStateDto parameter)
		{
			var transitionKey = transitions.Keys.Where(t => t.CurrentState == CurrentState && t.Command == command).SingleOrDefault();
			
			if (transitionKey is null)
				throw new Exception("Invalid transition: " + CurrentState + " -> " + command);

			transitionKey.MethodToExecute(parameter);
			CurrentState = transitions[transitionKey];
			
			return CurrentState;
		}

		public static void SomeMethodToExecute(BattleStateDto parameter)
		{
			Console.WriteLine("Executing method with parameter: " + parameter);
		}
	}
}