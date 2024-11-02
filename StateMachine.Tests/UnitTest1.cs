using System.Collections.Concurrent;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Moq;

namespace drittich.StateMachine.Tests
{
	/// <summary>
	/// Unit tests for the StateMachine class.
	/// </summary>
	public class UnitTest1
	{
		/// <summary>
		/// Verifies that the state machine correctly performs a defined transition when an event occurs.
		/// </summary>
		[Fact]
		public async Task ShouldDoADefinedTransitionAsync()
		{
			// Arrange
			var logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, logger);

			stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.Initial,
				evt: MyEvents.DoOtherStuff,
				nextState: MyStates.Complete,
				action: SomeMethodToExecuteAsync
			));

			var dto = new MyCustomDto { Prop1 = 1 };

			// Act
			var resultingState = await stateMachine.GetNextAsync(MyEvents.DoOtherStuff, dto);

			// Assert
			Assert.Equal(MyStates.Complete, resultingState);
		}

		/// <summary>
		/// Ensures that the state machine allows transitions even when the event data is empty or has default values.
		/// </summary>
		[Fact]
		public async Task ShouldAllowAnEmptyDtoObjectAsync()
		{
			// Arrange
			var logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, logger);

			stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.Initial,
				evt: MyEvents.DoOtherStuff,
				nextState: MyStates.Complete,
				action: SomeMethodToExecuteAsync
			));

			var dto = new MyCustomDto();

			// Act
			var resultingState = await stateMachine.GetNextAsync(MyEvents.DoOtherStuff, dto);

			// Assert
			Assert.Equal(MyStates.Complete, resultingState);
		}

		/// <summary>
		/// Confirms that the state machine prevents adding multiple transitions for the same state and event combination.
		/// </summary>
		[Fact]
		public void ShouldNotAllowMultipleEndStatesForSameTransition()
		{
			// Arrange
			var logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, logger);

			stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.Initial,
				evt: MyEvents.DoOtherStuff,
				nextState: MyStates.Complete,
				action: SomeMethodToExecuteAsync
			));

			// Act & Assert
			Assert.Throws<InvalidOperationException>(() =>
				stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
					currentState: MyStates.Initial,
					evt: MyEvents.DoOtherStuff,
					nextState: MyStates.SomeOtherState,
					action: SomeMethodToExecuteAsync
				))
			);
		}

		/// <summary>
		/// Checks that the state machine throws an InvalidTransitionException when an undefined transition is attempted.
		/// </summary>
		[Fact]
		public async Task ShouldThrowIfInvalidTransitionAsync()
		{
			// Arrange
			var logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, logger);

			stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.Initial,
				evt: MyEvents.DoOtherStuff,
				nextState: MyStates.Complete,
				action: SomeMethodToExecuteAsync
			));

			var dto = new MyCustomDto();

			// Act & Assert
			await Assert.ThrowsAsync<InvalidTransitionException>(async () =>
				await stateMachine.GetNextAsync(MyEvents.NeverDefinedInTransition, dto)
			);
		}

		/// <summary>
		/// Verifies that the state machine can perform multiple sequential transitions correctly.
		/// </summary>
		[Fact]
		public async Task ShouldDoMultipleDefinedTransitionsAsync()
		{
			// Arrange
			var logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, logger);

			stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.Initial,
				evt: MyEvents.DoOtherStuff,
				nextState: MyStates.SomeOtherState,
				action: SomeMethodToExecuteAsync
			));

			stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.SomeOtherState,
				evt: MyEvents.DoOtherStuff,
				nextState: MyStates.Complete,
				action: SomeMethodToExecuteAsync
			));

			var dto = new MyCustomDto { Prop1 = 1 };

			// Act
			var resultingState1 = await stateMachine.GetNextAsync(MyEvents.DoOtherStuff, dto);
			var resultingState2 = await stateMachine.GetNextAsync(MyEvents.DoOtherStuff, dto);

			// Assert
			Assert.Equal(MyStates.SomeOtherState, resultingState1);
			Assert.Equal(MyStates.Complete, resultingState2);
		}

		/// <summary>
		/// Ensures that the state machine initializes with the correct initial state.
		/// </summary>
		[Fact]
		public void ShouldStartOutWithInitialState()
		{
			// Arrange
			var initialState = MyStates.Initial;
			var logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(initialState, logger);

			// Act & Assert
			Assert.Equal(initialState, stateMachine.CurrentState);
		}

		/// <summary>
		/// Confirms that transitions with guard conditions occur when the guard condition evaluates to true.
		/// </summary>
		[Fact]
		public async Task ShouldTransitionWhenGuardConditionIsTrue()
		{
			// Arrange
			var logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, logger);

			stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.Initial,
				evt: MyEvents.DoStuff,
				nextState: MyStates.SomeState,
				action: SomeMethodToExecuteAsync,
				guard: data => data.Prop1 > 0
			));

			var dto = new MyCustomDto { Prop1 = 1 };

			// Act
			var resultingState = await stateMachine.GetNextAsync(MyEvents.DoStuff, dto);

			// Assert
			Assert.Equal(MyStates.SomeState, resultingState);
		}

		/// <summary>
		/// Verifies that transitions do not occur when the guard condition evaluates to false.
		/// </summary>
		[Fact]
		public async Task ShouldNotTransitionWhenGuardConditionIsFalse()
		{
			// Arrange
			var logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, logger);

			stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.Initial,
				evt: MyEvents.DoStuff,
				nextState: MyStates.SomeState,
				action: SomeMethodToExecuteAsync,
				guard: data => data.Prop1 > 0
			));

			var dto = new MyCustomDto { Prop1 = 0 };

			// Act & Assert
			await Assert.ThrowsAsync<GuardConditionFailedException>(async () =>
				await stateMachine.GetNextAsync(MyEvents.DoStuff, dto)
			);

			// Ensure state has not changed
			Assert.Equal(MyStates.Initial, stateMachine.CurrentState);
		}

		/// <summary>
		/// Checks that the state machine correctly handles cancellation during a transition.
		/// </summary>
		[Fact]
		public async Task ShouldCancelTransitionWhenCancellationTokenIsTriggered()
		{
			// Arrange
			var logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, logger);

			stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.Initial,
				evt: MyEvents.DoStuff,
				nextState: MyStates.SomeState,
				action: async (data, cancellationToken) =>
				{
					// Simulate a long-running operation
					await Task.Delay(1000, cancellationToken);
				}
			));

			var dto = new MyCustomDto { Prop1 = 1 };
			var cts = new CancellationTokenSource();
			cts.CancelAfter(100); // Cancel after 100ms

			// Act & Assert
			await Assert.ThrowsAsync<TaskCanceledException>(async () =>
				await stateMachine.GetNextAsync(MyEvents.DoStuff, dto, cts.Token)
			);

			// Ensure state has not changed
			Assert.Equal(MyStates.Initial, stateMachine.CurrentState);
		}

		/// <summary>
		/// Ensures that if the action associated with a transition throws an exception, the state machine does not change state.
		/// </summary>
		[Fact]
		public async Task ShouldNotChangeStateWhenActionThrowsException()
		{
			// Arrange
			var logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, logger);

			stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.Initial,
				evt: MyEvents.DoStuff,
				nextState: MyStates.SomeState,
				action: async (data, cancellationToken) =>
				{
					await Task.Delay(1, cancellationToken);
					throw new InvalidOperationException("Action failed");
				}
			));

			var dto = new MyCustomDto { Prop1 = 1 };

			// Act & Assert
			await Assert.ThrowsAsync<InvalidOperationException>(async () =>
				await stateMachine.GetNextAsync(MyEvents.DoStuff, dto)
			);

			// Ensure state has not changed
			Assert.Equal(MyStates.Initial, stateMachine.CurrentState);
		}

		/// <summary>
		/// Verifies that the state machine handles concurrent transition attempts in a thread-safe manner.
		/// </summary>
		[Fact]
		public async Task ShouldHandleConcurrentTransitionsCorrectly()
		{
			// Arrange
			var logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, logger);

			stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.Initial,
				evt: MyEvents.DoOtherStuff,
				nextState: MyStates.SomeOtherState,
				action: async (data, cancellationToken) =>
				{
					await Task.Delay(10, cancellationToken); // Simulate work
				}
			));

			var tasks = new Task[10];
			var dto = new MyCustomDto { Prop1 = 1 };

			var exceptions = new ConcurrentBag<Exception>();

			// Act
			for (int i = 0; i < tasks.Length; i++)
			{
				tasks[i] = Task.Run(async () =>
				{
					try
					{
						await stateMachine.GetNextAsync(MyEvents.DoOtherStuff, dto);
					}
					catch (Exception ex)
					{
						exceptions.Add(ex);
					}
				});
			}

			await Task.WhenAll(tasks);

			// Assert
			// Verify that only one task succeeded and others failed with InvalidTransitionException
			Assert.Equal(MyStates.SomeOtherState, stateMachine.CurrentState);
			Assert.Equal(tasks.Length - 1, exceptions.Count);

			foreach (var ex in exceptions)
			{
				Assert.IsType<InvalidTransitionException>(ex);
			}
		}

		/// <summary>
		/// Checks that the state machine logs the transition information appropriately.
		/// </summary>
		[Fact]
		public async Task ShouldLogTransitionInformation()
		{
			// Arrange
			var loggerMock = new Mock<ILogger<StateMachine<MyStates, MyEvents, MyCustomDto>>>();
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, loggerMock.Object);

			stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.Initial,
				evt: MyEvents.DoStuff,
				nextState: MyStates.SomeState,
				action: SomeMethodToExecuteAsync
			));

			var dto = new MyCustomDto { Prop1 = 1 };

			// Act
			await stateMachine.GetNextAsync(MyEvents.DoStuff, dto);

			// Assert
			loggerMock.Verify(
				x => x.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Transitioning from Initial to SomeState on event DoStuff.")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()
				),
				Times.Once
			);
		}

		/// <summary>
		/// Ensures that the state machine can handle transitions when the event data is null.
		/// </summary>
		[Fact]
		public async Task ShouldHandleNullEventData()
		{
			// Arrange
			var logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, logger);

			stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.Initial,
				evt: MyEvents.DoStuff,
				nextState: MyStates.SomeState,
				action: SomeMethodToExecuteAsync
			));

			// Act
			var resultingState = await stateMachine.GetNextAsync(MyEvents.DoStuff, null);

			// Assert
			Assert.Equal(MyStates.SomeState, resultingState);
		}

		/// <summary>
		/// Confirms that the state machine can perform transitions even when the action delegate does nothing.
		/// </summary>
		[Fact]
		public async Task ShouldTransitionWithoutAction()
		{
			// Arrange
			var logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, logger);

			// Action that does nothing
			Func<MyCustomDto, CancellationToken, Task> noOpAction = (data, cancellationToken) => Task.CompletedTask;

			stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.Initial,
				evt: MyEvents.DoStuff,
				nextState: MyStates.SomeState,
				action: noOpAction
			));

			var dto = new MyCustomDto { Prop1 = 1 };

			// Act
			var resultingState = await stateMachine.GetNextAsync(MyEvents.DoStuff, dto);

			// Assert
			Assert.Equal(MyStates.SomeState, resultingState);
		}

		/// <summary>
		/// Verifies that adding a duplicate transition (same state and event) results in an exception.
		/// </summary>
		[Fact]
		public void ShouldThrowWhenAddingDuplicateTransition()
		{
			// Arrange
			var logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, logger);

			var transition = new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.Initial,
				evt: MyEvents.DoStuff,
				nextState: MyStates.SomeState,
				action: SomeMethodToExecuteAsync
			);

			stateMachine.AddTransition(transition);

			// Act & Assert
			Assert.Throws<InvalidOperationException>(() =>
				stateMachine.AddTransition(transition)
			);
		}

		/// <summary>
		/// Checks that the state machine throws an exception when a transition is attempted from a state with no defined transitions.
		/// </summary>
		[Fact]
		public async Task ShouldThrowWhenTransitionNotDefinedFromCurrentState()
		{
			// Arrange
			var logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(MyStates.SomeState, logger);

			// No transitions are added for MyStates.SomeState

			var dto = new MyCustomDto { Prop1 = 1 };

			// Act & Assert
			await Assert.ThrowsAsync<InvalidTransitionException>(async () =>
				await stateMachine.GetNextAsync(MyEvents.DoStuff, dto)
			);
		}

		/// <summary>
		/// Ensures that the state machine can handle a complex sequence of transitions.
		/// </summary>
		[Fact]
		public async Task ShouldHandleComplexSequenceOfTransitions()
		{
			// Arrange
			var logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, logger);

			// Define a sequence of transitions
			stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.Initial,
				evt: MyEvents.DoStuff,
				nextState: MyStates.SomeState,
				action: SomeMethodToExecuteAsync
			));

			stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.SomeState,
				evt: MyEvents.DoOtherStuff,
				nextState: MyStates.SomeOtherState,
				action: SomeMethodToExecuteAsync
			));

			stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.SomeOtherState,
				evt: MyEvents.SomeEvent,
				nextState: MyStates.Complete,
				action: SomeMethodToExecuteAsync
			));

			var dto = new MyCustomDto { Prop1 = 1 };

			// Act
			await stateMachine.GetNextAsync(MyEvents.DoStuff, dto);
			await stateMachine.GetNextAsync(MyEvents.DoOtherStuff, dto);
			var finalState = await stateMachine.GetNextAsync(MyEvents.SomeEvent, dto);

			// Assert
			Assert.Equal(MyStates.Complete, finalState);
		}

		/// <summary>
		/// Verifies that adding a transition with a null action delegate results in an exception.
		/// </summary>
		[Fact]
		public void ShouldThrowWhenAddingTransitionWithNullAction()
		{
			// Arrange
			var logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, logger);

			// Act & Assert
			Assert.Throws<ArgumentNullException>(() =>
				stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
					currentState: MyStates.Initial,
					evt: MyEvents.DoStuff,
					nextState: MyStates.SomeState,
					action: null
				))
			);
		}

		/// <summary>
		/// Checks that the state machine's state can be persisted and restored correctly.
		/// </summary>
		[Fact]
		public async Task ShouldPersistAndRestoreState()
		{
			// Arrange
			var logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();
			var originalStateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, logger);

			// Add transitions to the original state machine
			originalStateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.Initial,
				evt: MyEvents.DoStuff,
				nextState: MyStates.SomeState,
				action: SomeMethodToExecuteAsync
			));

			var dto = new MyCustomDto { Prop1 = 1 };

			// Act
			await originalStateMachine.GetNextAsync(MyEvents.DoStuff, dto);

			// Serialize state
			var serializedState = originalStateMachine.CurrentState.ToString();

			// Restore state
			var restoredState = Enum.Parse<MyStates>(serializedState);
			var restoredStateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(restoredState, logger);

			// Add transitions to the restored state machine
			restoredStateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.SomeState,
				evt: MyEvents.DoOtherStuff,
				nextState: MyStates.Complete,
				action: SomeMethodToExecuteAsync
			));

			// Act on the restored state machine
			var finalState = await restoredStateMachine.GetNextAsync(MyEvents.DoOtherStuff, dto);

			// Assert
			Assert.Equal(MyStates.Complete, finalState);
		}

		/// <summary>
		/// Confirms that complex guard conditions are evaluated correctly.
		/// </summary>
		[Fact]
		public async Task ShouldEvaluateComplexGuardConditionCorrectly()
		{
			// Arrange
			var logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();

			var validDto = new MyCustomDto { Prop1 = 1, Prop2 = "Valid" };
			var invalidDto = new MyCustomDto { Prop1 = -1, Prop2 = "Invalid" };

			// First, test guard condition failure
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, logger);

			stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.Initial,
				evt: MyEvents.DoStuff,
				nextState: MyStates.SomeState,
				action: SomeMethodToExecuteAsync,
				guard: data => data.Prop1 > 0 && data.Prop2 == "Valid"
			));

			// Act & Assert for invalid data
			await Assert.ThrowsAsync<GuardConditionFailedException>(async () =>
				await stateMachine.GetNextAsync(MyEvents.DoStuff, invalidDto)
			);

			// Ensure state has not changed
			Assert.Equal(MyStates.Initial, stateMachine.CurrentState);

			// Act with valid data
			var resultingState = await stateMachine.GetNextAsync(MyEvents.DoStuff, validDto);

			// Assert
			Assert.Equal(MyStates.SomeState, resultingState);
		}

		/// <summary>
		/// Verifies that initializing the state machine with an invalid initial state results in an exception.
		/// </summary>
		[Fact]
		public void ShouldThrowWhenInitializedWithInvalidInitialState()
		{
			// Arrange
			ILogger<StateMachine<MyStates, MyEvents, MyCustomDto>> logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();

			// Create an invalid state value that's not defined in MyStates enum
			MyStates invalidState = (MyStates)999;

			// Act & Assert
			Assert.Throws<ArgumentException>(() =>
				new StateMachine<MyStates, MyEvents, MyCustomDto>(initialState: invalidState, logger)
			);
		}

		/// <summary>
		/// Checks that initializing the state machine with a null logger results in an exception.
		/// </summary>
		[Fact]
		public void ShouldThrowWhenInitializedWithNullLogger()
		{
			// Arrange
			var initialState = MyStates.Initial;

			// Act & Assert
			Assert.Throws<ArgumentNullException>(() =>
				new StateMachine<MyStates, MyEvents, MyCustomDto>(initialState, logger: null)
			);
		}

		/// <summary>
		/// Ensures that actions associated with transitions are executed in the correct order.
		/// </summary>
		[Fact]
		public async Task ShouldExecuteActionsInOrder()
		{
			// Arrange
			var logger = new NullLogger<StateMachine<MyStates, MyEvents, MyCustomDto>>();
			var callOrder = new List<string>();

			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, logger);

			stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.Initial,
				evt: MyEvents.DoStuff,
				nextState: MyStates.SomeState,
				action: async (data, cancellationToken) =>
				{
					callOrder.Add("FirstAction");
					await Task.Delay(1, cancellationToken);
				}
			));

			stateMachine.AddTransition(new Transition<MyStates, MyEvents, MyCustomDto>(
				currentState: MyStates.SomeState,
				evt: MyEvents.DoOtherStuff,
				nextState: MyStates.Complete,
				action: async (data, cancellationToken) =>
				{
					callOrder.Add("SecondAction");
					await Task.Delay(1, cancellationToken);
				}
			));

			var dto = new MyCustomDto { Prop1 = 1 };

			// Act
			await stateMachine.GetNextAsync(MyEvents.DoStuff, dto);
			await stateMachine.GetNextAsync(MyEvents.DoOtherStuff, dto);

			// Assert
			Assert.Equal(new[] { "FirstAction", "SecondAction" }, callOrder);
		}

		/// <summary>
		/// Dummy asynchronous operation used as the action delegate in several tests.
		/// </summary>
		private async Task SomeMethodToExecuteAsync(MyCustomDto obj, CancellationToken cancellationToken)
		{
			await Task.Delay(1, cancellationToken);
		}
	}

	/// <summary>
	/// Custom data transfer object used for event data in the state machine tests.
	/// </summary>
	public class MyCustomDto
	{
		public int Prop1 { get; set; }
		public string Prop2 { get; set; }
	}

	/// <summary>
	/// Enumeration of possible states for the state machine.
	/// </summary>
	public enum MyStates
	{
		Initial,
		SomeState,
		SomeOtherState,
		NeverDefinedInTransition,
		Complete
	}

	/// <summary>
	/// Enumeration of possible events for the state machine.
	/// </summary>
	public enum MyEvents
	{
		DoStuff,
		SomeEvent,
		NeverDefinedInTransition,
		DoOtherStuff
	}
}
