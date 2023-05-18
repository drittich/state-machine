# state-machine
A simple convention-based finite state machine that lets you pass event data through to your transition actions.

## Example Usage

StateMachine expects you to define your own states, events, transitions, and a DTO so that data can easily be passed with events so that they can then be provided to the resulting action that will run when a transition occurs. It will automatically select the first value in your states enum as the initial state of the state machine. (If you don't want this you can define and execute a transition immediately after creating the state machine.)

E.g.,

```csharp
enum MyStates
{
    Initial,
    SomeState,
    SomeOtherState,
    Complete
}

enum MyEvents
{
    SomethingHappened,
    SomethingElseHappened,
    SomeOtherRandomEvent
}

public class MyDto {
    public int Prop1 { get; set; }    
}
  
...
  
// create the state machine
var sm = new StateMachine(MyStates, MyEvents, MyDto>();

// specify the allowed transitions
stateMachine.Transitions = new Dictionary<Transition<MyStates, MyEvents, MyCustomDto>, MyStates>
{
    { new Transition<MyStates, MyEvents, MyCustomDto>(MyStates.Initial, MyEvents.SomethingHappened, SomeMethodToExecuteAsync), MyStates.SomeState },
    { new Transition<MyStates, MyEvents, MyCustomDto>(MyStates.SomeState, MyEvents.SomethingElseHappened, SomeMethodToExecuteAsync), MyStates.Complete }
};

var data = new MyDto { Prop1 = 1 };

// Execute a transition.
// This will transition the state as per the specification above, and call method SomeMethodToExecuteAsync,
// passing the parameter `data` to it.
var resultingState = await sm.GetNextAsync(MyEvents.SomethingHappened, data);  // new state is `MyStates.SomeState`

// Invalid transitions will result in an exception
var resultingState2 = sm.GetNextAsync(MyEvents.SomeOtherRandomEvent, data);  // since no transition defined for `SomeOtherRandomEvent`, wil throw
```
