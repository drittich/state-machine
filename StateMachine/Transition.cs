
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace drittich.StateMachine
{
	/// <summary>
	/// Represents a transition between states in the state machine.
	/// </summary>
	/// <typeparam name="TStateEnum">The enum type representing the states.</typeparam>
	/// <typeparam name="TEventEnum">The enum type representing the events.</typeparam>
	/// <typeparam name="TEventData">The type of data associated with events.</typeparam>
	public class Transition<TStateEnum, TEventEnum, TEventData> : IEquatable<Transition<TStateEnum, TEventEnum, TEventData>>
		where TStateEnum : Enum
		where TEventEnum : Enum
	{
		/// <summary>
		/// Gets the state from which the transition occurs.
		/// </summary>
		public TStateEnum CurrentState { get; }

		/// <summary>
		/// Gets the event that triggers the transition.
		/// </summary>
		public TEventEnum Event { get; }

		/// <summary>
		/// Gets the state to which the machine transitions.
		/// </summary>
		public TStateEnum NextState { get; }

		/// <summary>
		/// Gets the action to execute during the transition.
		/// </summary>
		public Func<TEventData, CancellationToken, Task> Action { get; }

		/// <summary>
		/// Gets the guard condition that must be satisfied for the transition to occur.
		/// </summary>
		public Func<TEventData, bool> Guard { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Transition{TStateEnum, TEventEnum, TEventData}"/> class.
		/// </summary>
		/// <param name="currentState">The state from which the transition occurs.</param>
		/// <param name="evt">The event that triggers the transition.</param>
		/// <param name="nextState">The state to which the machine transitions.</param>
		/// <param name="action">The action to execute during the transition.</param>
		/// <param name="guard">An optional guard condition that must be satisfied for the transition to occur.</param>
		public Transition(
			TStateEnum currentState,
			TEventEnum evt,
			TStateEnum nextState,
			Func<TEventData, CancellationToken, Task> action,
			Func<TEventData, bool> guard = null)
		{
			CurrentState = currentState;
			Event = evt;
			NextState = nextState;
			Action = action ?? throw new ArgumentNullException(nameof(action));
			Guard = guard;
		}

		/// <inheritdoc/>
		public bool Equals(Transition<TStateEnum, TEventEnum, TEventData> other)
		{
			if (other == null) return false;

			return EqualityComparer<TStateEnum>.Default.Equals(CurrentState, other.CurrentState)
				&& EqualityComparer<TEventEnum>.Default.Equals(Event, other.Event)
				&& EqualityComparer<TStateEnum>.Default.Equals(NextState, other.NextState);
		}

		/// <inheritdoc/>
		public override bool Equals(object obj) => Equals(obj as Transition<TStateEnum, TEventEnum, TEventData>);

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			return HashCode.Combine(CurrentState, Event, NextState);
		}
	}
}
