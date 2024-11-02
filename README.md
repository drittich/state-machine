# State Machine

A simple, extensible finite state machine that allows you to define states, events, transitions, and pass event data through to your transition actions.

## Example Usage

The `StateMachine` class lets you define your own states, events, transitions, and a data transfer object (DTO) to pass data with events. This data can then be provided to the action that runs when a transition occurs.

You need to:

- Define enums for your states and events.
- Create a DTO class for event data.
- Initialize the state machine with an initial state and a logger.
- Add transitions that specify how the state machine moves from one state to another in response to events.

### Define States and Events

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
```

### Create a DTO for Event Data

```csharp
public class MyDto
{
    public int Prop1 { get; set; }
}
```

### Initialize the State Machine

```csharp
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

// Create a logger (use NullLogger if you don't need logging)
ILogger<StateMachine<MyStates, MyEvents, MyDto>> logger = new NullLogger<StateMachine<MyStates, MyEvents, MyDto>>();

// Initialize the state machine with the initial state
var sm = new StateMachine<MyStates, MyEvents, MyDto>(MyStates.Initial, logger);
```

### Define the Transitions

```csharp
// Add transitions to the state machine
sm.AddTransition(new Transition<MyStates, MyEvents, MyDto>(
    currentState: MyStates.Initial,
    evt: MyEvents.SomethingHappened,
    nextState: MyStates.SomeState,
    action: SomeMethodToExecuteAsync
));

sm.AddTransition(new Transition<MyStates, MyEvents, MyDto>(
    currentState: MyStates.SomeState,
    evt: MyEvents.SomethingElseHappened,
    nextState: MyStates.Complete,
    action: SomeOtherMethodToExecuteAsync
));
```

### Define the Action Methods

```csharp
using System.Threading;
using System.Threading.Tasks;

// Action method for the first transition
async Task SomeMethodToExecuteAsync(MyDto data, CancellationToken cancellationToken)
{
    // Your action code here
    await Task.Delay(100, cancellationToken);
    Console.WriteLine("Executed SomeMethodToExecuteAsync");
}

// Action method for the second transition
async Task SomeOtherMethodToExecuteAsync(MyDto data, CancellationToken cancellationToken)
{
    // Your action code here
    await Task.Delay(100, cancellationToken);
    Console.WriteLine("Executed SomeOtherMethodToExecuteAsync");
}
```

### Execute Transitions

```csharp
var data = new MyDto { Prop1 = 1 };

// Execute the first transition
var resultingState = await sm.GetNextAsync(MyEvents.SomethingHappened, data);
// resultingState is MyStates.SomeState

// Execute the second transition
var resultingState2 = await sm.GetNextAsync(MyEvents.SomethingElseHappened, data);
// resultingState2 is MyStates.Complete
```

### Handle Invalid Transitions

If an invalid transition is attempted (no transition is defined for the current state and event), an InvalidTransitionException is thrown.

```csharp
try
{
    // Attempt an invalid transition
    var resultingState3 = await sm.GetNextAsync(MyEvents.SomeOtherRandomEvent, data);
}
catch (InvalidTransitionException ex)
{
    // Handle the exception
    Console.WriteLine($"Invalid transition: {ex.Message}");
}
```

## Additional Features

### Guard Conditions

You can add guard conditions to transitions to control whether the transition should occur based on the event data.

```csharp
sm.AddTransition(new Transition<MyStates, MyEvents, MyDto>(
    currentState: MyStates.SomeState,
    evt: MyEvents.SomeOtherRandomEvent,
    nextState: MyStates.Complete,
    action: SomeOtherMethodToExecuteAsync,
    guard: data => data.Prop1 > 0
));
```

If the guard condition returns false, a GuardConditionFailedException is thrown, and the transition does not occur.

### Cancellation Support

The action methods accept a CancellationToken, allowing transitions to be canceled if needed.

```csharp
var cts = new CancellationTokenSource();
cts.CancelAfter(500); // Cancel after 500ms

try
{
    await sm.GetNextAsync(MyEvents.SomethingHappened, data, cts.Token);
}
catch (TaskCanceledException)
{
    Console.WriteLine("Transition was canceled.");
}
```

### Logging

The state machine uses ILogger to log information about transitions, warnings, and errors.

- **Information:** Successful transitions.
- **Warning:** Undefined transitions or guard condition failures.
- **Error:** Exceptions thrown during actions.

## Exception Handling

- **InvalidTransitionException:** Thrown when no transition is defined for the current state and event.
- **GuardConditionFailedException:** Thrown when a guard condition evaluates to false.
- **TaskCanceledException:** Thrown when a transition is canceled via a CancellationToken.
- **InvalidOperationException:** Thrown when attempting to add a duplicate transition.

## Thread Safety

The `StateMachine` class is thread-safe and can handle concurrent transition attempts appropriately. It ensures that only one transition occurs at a time, maintaining the integrity of the `CurrentState`.

## Complete Example

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

// Define states and events
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

// DTO for event data
public class MyDto
{
    public int Prop1 { get; set; }
}

class Program
{
    static async Task Main(string[] args)
    {
        // Create a logger
        ILogger<StateMachine<MyStates, MyEvents, MyDto>> logger = new NullLogger<StateMachine<MyStates, MyEvents, MyDto>>();

        // Initialize the state machine
        var sm = new StateMachine<MyStates, MyEvents, MyDto>(MyStates.Initial, logger);

        // Define transitions
        sm.AddTransition(new Transition<MyStates, MyEvents, MyDto>(
            currentState: MyStates.Initial,
            evt: MyEvents.SomethingHappened,
            nextState: MyStates.SomeState,
            action: SomeMethodToExecuteAsync
        ));

        sm.AddTransition(new Transition<MyStates, MyEvents, MyDto>(
            currentState: MyStates.SomeState,
            evt: MyEvents.SomethingElseHappened,
            nextState: MyStates.Complete,
            action: SomeOtherMethodToExecuteAsync
        ));

        // Event data
        var data = new MyDto { Prop1 = 1 };

        // Execute transitions
        var resultingState = await sm.GetNextAsync(MyEvents.SomethingHappened, data);
        Console.WriteLine($"State after first transition: {resultingState}");

        var resultingState2 = await sm.GetNextAsync(MyEvents.SomethingElseHappened, data);
        Console.WriteLine($"State after second transition: {resultingState2}");

        // Handle invalid transition
        try
        {
            await sm.GetNextAsync(MyEvents.SomeOtherRandomEvent, data);
        }
        catch (InvalidTransitionException ex)
        {
            Console.WriteLine($"Invalid transition: {ex.Message}");
        }
    }

    // Action methods
    static async Task SomeMethodToExecuteAsync(MyDto data, CancellationToken cancellationToken)
    {
        await Task.Delay(100, cancellationToken);
        Console.WriteLine("Executed SomeMethodToExecuteAsync");
    }

    static async Task SomeOtherMethodToExecuteAsync(MyDto data, CancellationToken cancellationToken)
    {
        await Task.Delay(100, cancellationToken);
        Console.WriteLine("Executed SomeOtherMethodToExecuteAsync");
    }
}
```

## Installation
To use the `StateMachine` class in your project, include the source code or compile it into a library that you can reference.

## Dependencies
- **.NET Standard 2.0** or higher.
- `Microsoft.Extensions.Logging.Abstractions` for logging interfaces.


Install via NuGet:

```bash
Install-Package Microsoft.Extensions.Logging.Abstractions
```

## License
This project is licensed under the MIT License.

## Contributing
Contributions are welcome! Please submit a pull request or open an issue to discuss improvements or features.

## Contact
For questions or support, please open an issue on the GitHub repository.

***

**Note:** Replace `SomeMethodToExecuteAsync` and `SomeOtherMethodToExecuteAsync` with your actual action methods. The DTO MyDto should contain the data relevant to your application.