using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace drittich.StateMachine
{
	public class Transition<TStateEnum, TEventEnum, TEventData>
		where TStateEnum : Enum
		where TEventEnum : Enum
	{
		public TStateEnum CurrentState { get; }
		public TEventEnum Event { get; }
		public Func<TEventData, Task> Action { get; }

		public Transition(TStateEnum currentState, TEventEnum evt, Func<TEventData, Task> action)
		{
			CurrentState = currentState;
			Event = evt;
			Action = action;
		}

		public override bool Equals(object? obj)
		{
			return obj is Transition<TStateEnum, TEventEnum, TEventData> transition &&
				   EqualityComparer<TStateEnum>.Default.Equals(CurrentState, transition.CurrentState) &&
				   EqualityComparer<TEventEnum>.Default.Equals(Event, transition.Event) &&
				   EqualityComparer<Func<TEventData, Task>>.Default.Equals(Action, transition.Action);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(CurrentState, Event, Action);
		}
	}
}
