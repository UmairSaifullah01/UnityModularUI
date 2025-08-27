using System;

namespace THEBADDEST
{
	/// <summary>
	/// Interface for a state machine.
	/// </summary>
	public interface IStateMachine
	{
		bool IsTransiting { get; }
		
		/// <summary>
		/// Retrieves a state by its ID.
		/// </summary>
		/// <param name="id">The ID of the state to retrieve.</param>
		/// <returns>The state with the specified ID.</returns>
		IState GetState(string id);
		
		/// <summary>
		/// Transitions from the current state to the specified transition.
		/// </summary>
		/// <param name="transition">The transition to execute.</param>
		void Transition(ITransition transition);

	
		/// <summary>
		/// Exits any active states that were transitioned with an "AnyTransition".
		/// </summary>
		void ExitAnyState();
		

		/// <summary>
		/// Exits the specified state.
		/// </summary>
		/// <param name="state">The state to exit.</param>
		void ExitState(IState state);
	
	}
}