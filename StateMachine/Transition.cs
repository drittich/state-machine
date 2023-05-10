using System;
using System.Collections.Generic;

namespace StateMachine
{
	public class Transition<TEventData, TStateEnum, TEventEnum> 
		where TStateEnum : Enum
		where TEventEnum : Enum
	{
		public TStateEnum CurrentState { get; }
		public TEventEnum Event { get; }
		public Action<TEventData> Action { get; }

		public Transition(TStateEnum currentState, TEventEnum evt, Action<TEventData> action)
		{
			CurrentState = currentState;
			Event = evt;
			Action = action;
		}

		public override bool Equals(object? obj)
		{
			return obj is Transition<TEventData, TStateEnum, TEventEnum> transition &&
				   EqualityComparer<TStateEnum>.Default.Equals(CurrentState, transition.CurrentState) &&
				   EqualityComparer<TEventEnum>.Default.Equals(Event, transition.Event) &&
				   EqualityComparer<Action<TEventData>>.Default.Equals(Action, transition.Action);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(CurrentState, Event, Action);
		}
	}
}
