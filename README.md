# UnityModularUI Documentation

Welcome to the documentation for UnityModularUI, a user interface system for Unity projects. This documentation provides an overview of the project structure, usage instructions, and important classes and interfaces.

## Table of Contents

- [Installation](#installation)
- [Getting Started](#getting-started)
- [UIVolume](#uivolume)
- [UIPanel](#uipanel)

## Installation

### Unity Package

1. Download the latest UnityModularUI package from [GitHub](https://github.com/UmairSaifullah01/UnityModularUI/releases).
2. Open your Unity project.
3. Go to `Assets` > `Import Package` > `Custom Package`.
4. Select the downloaded package file and click "Import".

### Git Repository

1. Clone the UnityModularUI repository from [GitHub](https://github.com/UmairSaifullah01/UnityModularUI).
2. Open your Unity project.
3. Go to `Assets` > `Import Package` > `Custom Package`.
4. Navigate to the cloned repository folder and select the package file. Click "Import".

## Getting Started

To get started with UnityModularUI, follow these steps:

1. Create a new game object in your scene and attach the `UIVolume` script to it. This script manages the UI panels in your project.

2. Create a new class that inherits from the `UIPanel` class. This will represent your custom UI panel.

3. Implement the required methods and properties in your custom panel class. You can override the `Init`, `Enter`, `Exit`, and `Execute` methods to define the panel's behavior.

4. Define the transitions for your panel by using the `SetTransitions` method. This method takes instances of the `ITransition` interface as parameters. Set the appropriate conditions for each transition.

5. Implement the `IView` interface in your custom panel class to handle view-related functionality.

6. Use the `UIVolume` script to manage the active panel in your scene. Call the `SetCurrentState` method to set the initial state of the UI system.

7. Build and run your project to see the UI panels in action.

## UIVolume

The `UIVolume` class represents a UI volume that acts as a state machine for managing UI states. It implements the `IStateMachine` interface and extends the `MonoBehaviour` class.

### Properties

- `isTransiting` (bool): Indicates whether a state transition is currently in progress. *(Read-only)*

### Methods

- `GetState(string id)`: Retrieves the state with the specified ID.
- `LoadState(string id, Action<IState> onStateLoad)`: Loads the state with the specified ID and invokes the specified callback when the state is loaded.
- `Entry(IState state)`: Sets the current state and calls its `Enter` method.
- `Transition(ITransition transition)`: Initiates a transition to the specified transition.
- `AnyTransition(IState state)`: Initiates an "any" transition to the specified state.
- `ExitAnyStates()`: Exits the most recently entered "any" state.
- `StatesExecution()`: Executes the logic of the current state and "any" states.
- `Exit(IState state)`: Exits the specified state and enters the previously entered state.

## UIPanel

The `UIPanel` class represents a base class for UI panels in the UnityModularUI system. It provides functionality for managing the panel's state, transitions, and view model.

### Properties

- `StateName` (string): The name of the panel's state. *(Read-only)*
- `StateMachine` (IStateMachine): Reference to the state machine that manages the panel's state. *(Read-only)*
- `views` (Dictionary<string, IView>): Dictionary to store the views associated with the panel.

### Methods

- `Init(IStateMachine stateMachine)`: Initializes the panel with the specified state machine.
- `SetTransitions(params ITransition[] transitions)`: Sets the transitions for the panel.
- `GetTransitions()`: Retrieves the transitions associated with the panel.
- `SetTransitionCondition(string stateName, bool value)`: Sets the condition for a specific transition.
- `Execute()`: Executes the panel's logic and checks for executable transitions.
- `Enter()`: Called when the panel enters the active state.
- `Exit()`: Called when the panel exits the active state.
- `InitViewModel()`: Initializes the view model of the panel.
- `Binder(string id, object value)`: Binds the specified value to the model with the given ID and invokes the model binder event.
