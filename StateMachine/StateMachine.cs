using System;
using System.Collections.Generic;
using System.Linq;

namespace StateMachine
{
	public class StateMachine<TStateEnum, TEventEnum, TEventData> 
		where TStateEnum: Enum
		where TEventEnum : Enum
	{
		public Dictionary<Transition<TStateEnum, TEventEnum, TEventData>, TStateEnum> Transitions = new Dictionary<Transition<TStateEnum, TEventEnum, TEventData>, TStateEnum>();
		public TStateEnum CurrentState { get; private set; }

		public StateMachine()
		{
			// By convention the first value of TStateEnum is the initial state
			// and the last value is the final state, so ensure that TStateEnum
			// has at least two values.
			if (typeof(TStateEnum).GetEnumValues().Length < 2)
				throw new ArgumentException("TStateEnum must have at least two values");

			// We need at least one event
			if (typeof(TEventEnum).GetEnumValues().Length < 1)
				throw new ArgumentException("TEventEnum must have at least one value");

			// Set CurrentState to the first value of TStateEnum
			var firstStateEnumValue = (TStateEnum)typeof(TStateEnum).GetEnumValues().GetValue(0)!;
			CurrentState = firstStateEnumValue;
		}

		public TStateEnum GetNext(TEventEnum evt, TEventData parameter)
		{
			var definedTransition = Transitions.Keys
				.SingleOrDefault(t => Convert.ToInt32(t.CurrentState) == Convert.ToInt32(CurrentState) && Convert.ToInt32(t.Event) == Convert.ToInt32(evt));

			if (definedTransition is null)
				throw new InvalidOperationException($"Invalid transition: {CurrentState} -> {evt}");

			definedTransition.Action(parameter);
			CurrentState = Transitions[definedTransition];

			return CurrentState;
		}
	}
}
