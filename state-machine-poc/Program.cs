using System;
using System.Collections.Generic;

namespace state_machine_poc
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello, World!");

			StateMachine stateMachine = new StateMachine();
			stateMachine.GetNext(Event.Begin, null);
			stateMachine.GetNext(Event.Exit, null);

			Console.WriteLine("Goodbye, World!");

		}
	}


}