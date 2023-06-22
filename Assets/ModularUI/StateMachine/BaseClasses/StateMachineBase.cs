using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace THEBADDEST
{


	/// <summary>
	/// Abstract base class for implementing a state machine in Unity.
	/// </summary>
	public abstract class StateMachineBase : MonoBehaviour, IStateMachine
	{

		/// <summary>
		/// The name of the current state.
		/// </summary>
		public string currentStateName;

		/// <summary>
		/// Dictionary to store cached states.
		/// </summary>
		protected readonly Dictionary<string, IState> cachedStates = new Dictionary<string, IState>();

		/// <summary>
		/// The current state of the state machine.
		/// </summary>
		protected IState currentState;

		/// <summary>
		/// The previous state of the state machine.
		/// </summary>
		protected IState previousState;

		/// <summary>
		/// Stack to store any-states for handling transitions.
		/// </summary>
		protected readonly Stack<IState> anyStates = new Stack<IState>();

		/// <summary>
		/// Flag indicating if the state machine is currently in a transition.
		/// </summary>
		public bool isTransiting { get; private set; }

		/// <summary>
		/// Abstract method to load a state asynchronously.
		/// </summary>
		/// <param name="id">The ID of the state to load.</param>
		/// <param name="onStateLoad">Callback invoked when the state is loaded.</param>
		public abstract void LoadState(string id, Action<IState> onStateLoad);

		/// <summary>
		/// The current any-state of the state machine.
		/// </summary>
		protected IState currentAnyState;

		/// <summary>
		/// Retrieves a state from the cached states dictionary based on its ID.
		/// </summary>
		/// <param name="id">The ID of the state to retrieve.</param>
		/// <returns>The state associated with the specified ID.</returns>
		public IState GetState(string id)
		{
			return cachedStates[id];
		}

		/// <summary>
		/// Enters a specific state.
		/// </summary>
		/// <param name="state">The state to enter.</param>
		public void Entry(IState state)
		{
			currentState = state;
			currentState?.Enter();
		}

		/// <summary>
		/// Initiates a transition to a new state based on a transition object.
		/// </summary>
		/// <param name="transition">The transition object specifying the target state.</param>
		public void Transition(ITransition transition)
		{
			if (isTransiting) return;
			isTransiting = true;
			StartCoroutine(TransitionTo(transition));
		}

		private IEnumerator TransitionTo(ITransition transition)
		{
			if (transition.transitTime > 0)
				yield return new WaitForSecondsRealtime(transition.transitTime);
			MoveToState(GetState(transition.toState));
		}

		private void MoveToState(IState state)
		{
			currentState?.Exit();
			previousState = currentState;
			currentState  = state;
			currentState?.Enter();
			isTransiting = false;
		}

		/// <summary>
		/// Initiates an any-state transition.
		/// </summary>
		/// <param name="state">The any-state to transition to.</param>
		public void AnyTransition(IState state)
		{
			currentAnyState = state;
			anyStates.Push(currentAnyState);
			currentAnyState.Enter();
		}

		/// <summary>
		/// Exits the current any-state and transitions to the previous any-state if available.
		/// </summary>
		public void ExitAnyStates()
		{
			if (anyStates.Count > 0)
			{
				currentAnyState?.Exit();
				anyStates.Pop();
				if (anyStates.Count > 0)
				{
					AnyTransition(anyStates.Pop());
				}
				else
				{
					currentAnyState = null;
				}
			}
		}

		/// <summary>
		/// Executes the current state or any-state.
		/// </summary>
		public void StatesExecution()
		{
			if (isTransiting) return;
			if (currentAnyState != null)
			{
				currentAnyState.Execute();
			}
			else
			{
				currentStateName = currentState.StateName;
				currentState?.Execute();
			}
		}

		/// <summary>
		/// Exits a specific state.
		/// </summary>
		/// <param name="state">The state to exit.</param>
		public void Exit(IState state)
		{
			state?.Exit();
			if (currentState == state)
			{
				previousState = currentState;
				currentState  = null;
			}
		}

	}


}