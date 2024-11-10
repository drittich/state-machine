using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace drittich.StateMachine
{
	/// <summary>
	/// Represents a state machine that manages transitions between states based on events.
	/// </summary>
	/// <typeparam name="TStateEnum">The enum type representing the states.</typeparam>
	/// <typeparam name="TEventEnum">The enum type representing the events.</typeparam>
	/// <typeparam name="TEventData">The type of data associated with events.</typeparam>
	public class StateMachine<TStateEnum, TEventEnum, TEventData>
		where TStateEnum : Enum
		where TEventEnum : Enum
	{
		private readonly Dictionary<(TStateEnum CurrentState, TEventEnum Event), Transition<TStateEnum, TEventEnum, TEventData>> _transitions
			= new Dictionary<(TStateEnum, TEventEnum), Transition<TStateEnum, TEventEnum, TEventData>>();

		private readonly SemaphoreSlim _stateLock = new SemaphoreSlim(1, 1);
		private readonly ILogger<StateMachine<TStateEnum, TEventEnum, TEventData>> _logger;

		/// <summary>
		/// Gets the current state of the state machine.
		/// </summary>
		public TStateEnum CurrentState { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StateMachine{TStateEnum, TEventEnum, TEventData}"/> class.
		/// </summary>
		/// <param name="initialState">The initial state of the state machine.</param>
		/// <param name="logger">The logger instance.</param>
		public StateMachine(TStateEnum initialState, ILogger<StateMachine<TStateEnum, TEventEnum, TEventData>> logger)
		{
			// Validate that the initialState is a defined enum value
			if (!Enum.IsDefined(typeof(TStateEnum), initialState))
				throw new ArgumentException("Invalid initial state", nameof(initialState));

			// Ensure the logger is not null
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));

			// Set the current state
			CurrentState = initialState;
		}



		/// <summary>
		/// Adds a transition to the state machine.
		/// </summary>
		/// <param name="transition">The transition to add.</param>
		public void AddTransition(Transition<TStateEnum, TEventEnum, TEventData> transition)
		{
			if (transition == null)
				throw new ArgumentNullException(nameof(transition));

			var key = (transition.CurrentState, transition.Event);

			if (_transitions.ContainsKey(key))
				throw new InvalidOperationException($"A transition from state {transition.CurrentState} on event {transition.Event} is already defined.");

			_transitions.Add(key, transition);
		}

		// New simplified AddTransition method
		public void AddTransition(
			TStateEnum currentState,
			TEventEnum evt,
			TStateEnum nextState,
			Func<TEventData, CancellationToken, Task> action,
			Func<TEventData, bool>? guard = null)
		{
			var transition = new Transition<TStateEnum, TEventEnum, TEventData>(currentState, evt, nextState, action, guard);
			AddTransition(transition);
		}


		/// <summary>
		/// Handles an event and transitions to the next state if possible.
		/// </summary>
		/// <param name="evt">The event that triggers the transition.</param>
		/// <param name="parameter">The event data.</param>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <returns>The new state after the transition.</returns>
		public async Task<TStateEnum> GetNextAsync(TEventEnum evt, TEventData parameter, CancellationToken cancellationToken = default)
		{
			await _stateLock.WaitAsync(cancellationToken).ConfigureAwait(false);

			try
			{
				var key = (CurrentState, evt);

				if (!_transitions.TryGetValue(key, out var transition))
				{
					_logger.LogWarning("No transition defined from state {CurrentState} on event {Event}.", CurrentState, evt);
					throw new InvalidTransitionException($"No transition defined from state {CurrentState} on event {evt}.");
				}

				if (transition.Guard != null && !transition.Guard(parameter))
				{
					_logger.LogWarning("Guard condition failed for transition from state {CurrentState} on event {Event}.", CurrentState, evt);
					throw new GuardConditionFailedException($"Guard condition failed for transition from state {CurrentState} on event {evt}.");
				}

				_logger.LogInformation("Transitioning from {CurrentState} to {NextState} on event {Event}.", CurrentState, transition.NextState, evt);

				await transition.Action(parameter, cancellationToken).ConfigureAwait(false);

				CurrentState = transition.NextState;

				return CurrentState;
			}
			finally
			{
				_stateLock.Release();
			}
		}
	}
}
