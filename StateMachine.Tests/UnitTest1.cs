namespace StateMachine.Tests
{
	public class UnitTest1
	{
		[Fact]
		public void ShouldDoADefinedTransition()
		{
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>();
			stateMachine.Transitions = new Dictionary<Transition<MyStates, MyEvents, MyCustomDto>, MyStates>
			{
				{ new Transition<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, MyEvents.DoOtherStuff, SomeMethodToExecute), MyStates.Complete }
			};

			var bs1 = new MyCustomDto { Prop1 = 1 };

			var resultingState = stateMachine.GetNext(MyEvents.DoOtherStuff, bs1);
			Assert.Equal(MyStates.Complete, resultingState);
		}

		[Fact]
		public void ShouldAllowAnEmptyDtoObject()
		{
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>();
			stateMachine.Transitions = new Dictionary<Transition<MyStates, MyEvents, MyCustomDto>, MyStates>
			{
				{ new Transition<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, MyEvents.DoOtherStuff, SomeMethodToExecute), MyStates.Complete }
			};

			var bs1 = new MyCustomDto();

			var resultingState = stateMachine.GetNext(MyEvents.DoOtherStuff, bs1);
			Assert.Equal(MyStates.Complete, resultingState);
		}

		[Fact]
		public void ShouldNotAllowMultipleEndStatesForSameTransition()
		{
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>();
			stateMachine.Transitions = new Dictionary<Transition<MyStates, MyEvents, MyCustomDto>, MyStates>
			{
				{ new Transition<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, MyEvents.DoOtherStuff, SomeMethodToExecute), MyStates.Complete }
			};

			Assert.Throws<ArgumentException>(() =>
			stateMachine.Transitions.Add(new Transition<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, MyEvents.DoOtherStuff, SomeMethodToExecute), MyStates.SomeOtherState));
		}

		[Fact]
		public void ShouldThrowIfInvalidTransition()
		{
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>();
			stateMachine.Transitions = new Dictionary<Transition<MyStates, MyEvents, MyCustomDto>, MyStates>
			{
				{ new Transition<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, MyEvents.DoOtherStuff, SomeMethodToExecute), MyStates.Complete }
			};

			var bs1 = new MyCustomDto();

			Assert.Throws<InvalidOperationException>(() => stateMachine.GetNext(MyEvents.NeverDefinedInTransition, bs1));
		}

		[Fact]
		public void ShouldDoMultipleDefinedTransitions()
		{
			var stateMachine = new StateMachine<MyStates, MyEvents, MyCustomDto>();
			stateMachine.Transitions = new Dictionary<Transition<MyStates, MyEvents, MyCustomDto>, MyStates>
			{
				{ new Transition<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, MyEvents.DoOtherStuff, SomeMethodToExecute), MyStates.SomeOtherState },
				{ new Transition<MyStates, MyEvents, MyCustomDto>(MyStates.SomeOtherState, MyEvents.DoOtherStuff, SomeMethodToExecute), MyStates.Complete }
			};

			var bs1 = new MyCustomDto { Prop1 = 1 };

			var resultingState = stateMachine.GetNext(MyEvents.DoOtherStuff, bs1);
			Assert.Equal(MyStates.SomeOtherState, resultingState);
			var resultingState2 = stateMachine.GetNext(MyEvents.DoOtherStuff, bs1);
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

		private void SomeMethodToExecute(MyCustomDto? obj) { }
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