using System;

namespace THEBADDEST
{
	/// <summary>
	/// Interface for a state machine.
	/// </summary>
	public interface IStateMachine
	{
		bool isTransiting { get; }

		/// <summary>
		/// Loads a state with the specified ID asynchronously and invokes the callback when the state is loaded.
		/// </summary>
		/// <param name="id">The ID of the state to load.</param>
		/// <param name="onStateLoad">The callback to invoke when the state is loaded.</param>
		void LoadState(string id, Action<IState> onStateLoad);

		/// <summary>
		/// Retrieves a state by its ID.
		/// </summary>
		/// <param name="id">The ID of the state to retrieve.</param>
		/// <returns>The state with the specified ID.</returns>
		IState GetState(string id);

		/// <summary>
		/// Enters the specified state.
		/// </summary>
		/// <param name="state">The state to enter.</param>
		void Entry(IState state);

		/// <summary>
		/// Transitions from the current state to the specified transition.
		/// </summary>
		/// <param name="transition">The transition to transition to.</param>
		void Transition(ITransition transition);

		/// <summary>
		/// Transitions to the specified state without any condition.
		/// </summary>
		/// <param name="state">The state to transition to.</param>
		void AnyTransition(IState state);

		/// <summary>
		/// Exits any active states that were transitioned with an "AnyTransition".
		/// </summary>
		void ExitAnyState();

		/// <summary>
		/// Executes the logic of the current state.
		/// </summary>
		void StatesExecution();

		/// <summary>
		/// Exits the specified state.
		/// </summary>
		/// <param name="state">The state to exit.</param>
		void Exit(IState state);
		/// <summary>
		/// Removes all the states from the state machine.
		/// </summary>
		void ClearStates();
	}
}