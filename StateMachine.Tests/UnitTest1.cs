namespace StateMachine.Tests
{
	public class UnitTest1
	{
		[Fact]
		public void ShouldThrowIfInitialStateNotSet()
		{
			var stateMachine = new StateMachine<MyCustomDto>();
			stateMachine.Transitions = new Dictionary<Transition<MyCustomDto>, State>
			{
				{ new Transition<MyCustomDto>(State.Inactive, Event.Exit, SomeMethodToExecute), State.Terminated },
				{ new Transition<MyCustomDto>(State.Inactive, Event.Begin, SomeMethodToExecute), State.Active },
				// ... other transitions
			};

			var bs1 = new MyCustomDto { Prop1 = 1 };
			var bs2 = new MyCustomDto { Prop1 = 2 };

			stateMachine.GetNext(Event.Begin, bs1);
			stateMachine.GetNext(Event.Exit, bs2);
		}

		[Fact]
		public void ShouldThrowIfInvalidTransition()
		{
			var stateMachine = new StateMachine<MyCustomDto>();
			stateMachine.Transitions = new Dictionary<Transition<MyCustomDto>, State>
			{
				{ new Transition<MyCustomDto>(State.Inactive, Event.Exit, SomeMethodToExecute), State.Terminated },
				{ new Transition<MyCustomDto>(State.Inactive, Event.Begin, SomeMethodToExecute), State.Active },
				// ... other transitions
			};

			var bs1 = new MyCustomDto { Prop1 = 1 };
			var bs2 = new MyCustomDto { Prop1 = 2 };

			var resultingState = stateMachine.GetNext(Event.Begin, bs1);
			Assert.Equal(State.Active , resultingState);
			stateMachine.GetNext(Event.Exit, bs2);
		}

		[Fact]
		public void ShouldAllowMultipleValidTransitions()
		{
			var stateMachine = new StateMachine<MyCustomDto>();
			stateMachine.Transitions = new Dictionary<Transition<MyCustomDto>, State>
			{
				{ new Transition<MyCustomDto>(State.Inactive, Event.Exit, SomeMethodToExecute), State.Terminated },
				{ new Transition<MyCustomDto>(State.Inactive, Event.Begin, SomeMethodToExecute), State.Active },
				// ... other transitions
			};
			var bs1 = new MyCustomDto { Prop1 = 1 };
			var bs2 = new MyCustomDto { Prop1 = 2 };
			stateMachine.GetNext(Event.Begin, bs1);
			stateMachine.GetNext(Event.Exit, bs2);

		}

		[Fact]
		public void ShouldProperlyPassDto()
		{
			var stateMachine = new StateMachine<MyCustomDto>();
			stateMachine.Transitions = new Dictionary<Transition<MyCustomDto>, State>
			{
				{ new Transition<MyCustomDto>(State.Inactive, Event.Exit, SomeMethodToExecute), State.Terminated },
				{ new Transition<MyCustomDto>(State.Inactive, Event.Begin, SomeMethodToExecute), State.Active },
				// ... other transitions
			};

			var bs1 = new MyCustomDto { Prop1 = 1 };
			var bs2 = new MyCustomDto { Prop1 = 2 };

			stateMachine.GetNext(Event.Begin, bs1);
			stateMachine.GetNext(Event.Exit, bs2);
		}

		private void SomeMethodToExecute(MyCustomDto obj) { }
	}

	public class MyCustomDto
	{
		public int Prop1 { get; set; }
	}


}