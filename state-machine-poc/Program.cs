using System;
using System.Collections.Generic;

namespace state_machine_poc
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello, World!");

			var stateMachine = new StateMachine<BattleStateDto>();
			stateMachine.Transitions = new Dictionary<StateTransition<BattleStateDto>, ProcessState>
			{
				{ new StateTransition<BattleStateDto>(ProcessState.Inactive, Event.Exit, SomeMethodToExecute), ProcessState.Terminated },
				{ new StateTransition<BattleStateDto>(ProcessState.Inactive, Event.Begin, SomeMethodToExecute), ProcessState.Active },
				// ... other transitions
			};

			var bs1 = new BattleStateDto { BattleId = 1 };
			var bs2 = new BattleStateDto { BattleId = 2 };

			stateMachine.GetNext(Event.Begin, bs1);
			stateMachine.GetNext(Event.Exit, bs2);

			Console.WriteLine("Goodbye, World!");

		}


		public static void SomeMethodToExecute(BattleStateDto parameter)
		{
			Console.WriteLine("Executing method with BattleId parameter: " + parameter.BattleId);
		}
	}


}