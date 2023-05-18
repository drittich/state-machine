namespace drittich.StateMachine.Tests
{
	public class UnitTest1
	{
		[Fact]
		public async Task ShouldDoADefinedTransitionAsync()
		{
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>();
			stateMachine.Transitions = new Dictionary<Transition<MyStates, MyEvents, MyCustomDto>, MyStates>
			{
				{ new Transition<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, MyEvents.DoOtherStuff, SomeMethodToExecuteAsync), MyStates.Complete }
			};

			var bs1 = new MyCustomDto { Prop1 = 1 };

			var resultingState = await stateMachine.GetNextAsync(MyEvents.DoOtherStuff, bs1);
			Assert.Equal(MyStates.Complete, resultingState);
		}

		[Fact]
		public async Task ShouldAllowAnEmptyDtoObjectAsync()
		{
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>();
			stateMachine.Transitions = new Dictionary<Transition<MyStates, MyEvents, MyCustomDto>, MyStates>
			{
				{ new Transition<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, MyEvents.DoOtherStuff, SomeMethodToExecuteAsync), MyStates.Complete }
			};

			var bs1 = new MyCustomDto();

			var resultingState = await stateMachine.GetNextAsync(MyEvents.DoOtherStuff, bs1);
			Assert.Equal(MyStates.Complete, resultingState);
		}

		[Fact]
		public void ShouldNotAllowMultipleEndStatesForSameTransition()
		{
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>();
			stateMachine.Transitions = new Dictionary<Transition<MyStates, MyEvents, MyCustomDto>, MyStates>
			{
				{ new Transition<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, MyEvents.DoOtherStuff, SomeMethodToExecuteAsync), MyStates.Complete }
			};

			Assert.Throws<ArgumentException>(() =>
			stateMachine.Transitions.Add(new Transition<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, MyEvents.DoOtherStuff, SomeMethodToExecuteAsync), MyStates.SomeOtherState));
		}

		[Fact]
		public async Task ShouldThrowIfInvalidTransitionAsync()
		{
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>();
			stateMachine.Transitions = new Dictionary<Transition<MyStates, MyEvents, MyCustomDto>, MyStates>
			{
				{ new Transition<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, MyEvents.DoOtherStuff, SomeMethodToExecuteAsync), MyStates.Complete }
			};

			var bs1 = new MyCustomDto();

			await Assert.ThrowsAsync<InvalidOperationException>(() => stateMachine.GetNextAsync(MyEvents.NeverDefinedInTransition, bs1));
		}

		[Fact]
		public async Task ShouldDoMultipleDefinedTransitionsAsync()
		{
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>();
			stateMachine.Transitions = new Dictionary<Transition<MyStates, MyEvents, MyCustomDto>, MyStates>
			{
				{ new Transition<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, MyEvents.DoOtherStuff, SomeMethodToExecuteAsync), MyStates.SomeOtherState },
				{ new Transition<MyStates, MyEvents, MyCustomDto>(MyStates.SomeOtherState, MyEvents.DoOtherStuff, SomeMethodToExecuteAsync), MyStates.Complete }
			};

			var bs1 = new MyCustomDto { Prop1 = 1 };

			var resultingState = await stateMachine.GetNextAsync(MyEvents.DoOtherStuff, bs1);
			Assert.Equal(MyStates.SomeOtherState, resultingState);
			var resultingState2 = await stateMachine.GetNextAsync(MyEvents.DoOtherStuff, bs1);
			Assert.Equal(MyStates.Complete, resultingState2);
		}

		// Should start out with initial state
		[Fact]
		public void ShouldStartOutWithInitialState()
		{
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>();

			// validate that initial (first) state is set
			Assert.Equal(MyStates.Initial, stateMachine.CurrentState);
		}

		// Should throw if not enough states
		[Fact]
		public void ShouldThrowIfNotEnoughStates()
		{
			Assert.Throws<ArgumentException>(() => new StateMachine<NotEnoughStates, MyEvents, MyCustomDto>());
		}

		// Should throw if not enough events
		[Fact]
		public void ShouldThrowIfNoEvents()
		{
			Assert.Throws<ArgumentException>(() => new StateMachine<MyStates, NotEnoughEvents, MyCustomDto>());
		}

		private async Task SomeMethodToExecuteAsync(MyCustomDto? obj) { 
			// add a dummy await operation
			await Task.Delay(TimeSpan.FromMicroseconds(1));
		}
	}

	public class MyCustomDto
	{
		public int Prop1 { get; set; }
	}

	enum MyStates
	{
		Initial,
		SomeState,
		SomeOtherState,
		NeverDefinedInTransition,
		Complete
	}

	enum MyEvents
	{
		DoStuff,
		SomeEvent,
		NeverDefinedInTransition,
		DoOtherStuff
	}

	enum NotEnoughStates
	{
		Initial
	}

	enum NotEnoughEvents
	{
	}
}