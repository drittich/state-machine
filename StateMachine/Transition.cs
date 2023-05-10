using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StateMachine
{
	public class Transition<T>
	{
		public State CurrentState { get; }
		public Event Command { get; }
		public Action<T> Action { get; }

		public Transition(State currentState, Event evt, Action<T> action)
		{
			CurrentState = currentState;
			Command = evt;
			Action = action;
		}
	}
}
