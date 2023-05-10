namespace StateMachine.CommandLine
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello, World!");

			var stateMachine = new StateMachine<MyCustomDto>();
			stateMachine.Transitions = new Dictionary<Transition<MyCustomDto>, State>
			{
				{ new Transition<MyCustomDto>(State.Inactive, Event.Exit, SomeMethodToExecute), State.Terminated },
				{ new Transition<MyCustomDto>(State.Inactive, Event.Begin, SomeMethodToExecute), State.Active },
				// ... other transitions
			};

			var bs1 = new MyCustomDto { BattleId = 1 };
			var bs2 = new MyCustomDto { BattleId = 2 };

			stateMachine.GetNext(Event.Begin, bs1);
			//stateMachine.GetNext(Event.Exit, bs2);

			Console.WriteLine("Goodbye, World!");

		}


		static void SomeMethodToExecute(MyCustomDto parameter)
		{
			Console.WriteLine("Executing method with BattleId parameter: " + parameter.BattleId);
		}

	}
}