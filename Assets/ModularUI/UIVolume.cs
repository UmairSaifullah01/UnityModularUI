using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace THEBADDEST.UI
{


	/// <summary>
	/// The <c>UIVolume</c> class represents a UI volume that acts as a state machine for managing UI states.
	/// It implements the <see cref="IStateMachine"/> interface and extends the <see cref="MonoBehaviour"/> class.
	/// </summary>
	public class UIVolume : MonoBehaviour, IStateMachine
	{

		private readonly Dictionary<string, IState> cachedStates = new Dictionary<string, IState>();
		private          IState                     currentState;
		private          IState                     previousState;
		private readonly Stack<IState>              anyStates = new Stack<IState>();
		public           bool                       isTransiting { get; private set; }

		/// <summary>
		/// Called when the script instance is being loaded.
		/// Initializes the UI volume by caching states and initializing them.
		/// </summary>
		private void Awake()
		{
			var states = GetComponentsInChildren<IState>(true);
			foreach (var state in states)
			{
				cachedStates.Add(state.GetStateName(), state);
			}

			foreach (var state in cachedStates.Values)
			{
				if (currentState == null) currentState = state;
				state.Init(this);
			}

			LoadState(currentState.StateName, Entry);
		}

		/// <summary>
		/// Called once per frame.
		/// Executes the states' logic.
		/// </summary>
		private void Update()
		{
			StatesExecution();
		}

		/// <summary>
		/// Retrieves the state with the specified ID.
		/// </summary>
		/// <param name="id">The ID of the state.</param>
		/// <returns>The state with the specified ID.</returns>
		public IState GetState(string id)
		{
			return cachedStates[id];
		}

		/// <summary>
		/// Loads the state with the specified ID and invokes the specified callback when the state is loaded.
		/// </summary>
		/// <param name="id">The ID of the state to load.</param>
		/// <param name="onStateLoad">The callback to invoke when the state is loaded.</param>
		public void LoadState(string id, Action<IState> onStateLoad)
		{
			if (cachedStates.TryGetValue(id, out IState cachedState))
			{
				onStateLoad?.Invoke(cachedState);
			}
		}

		/// <summary>
		/// Sets the current state and calls its Enter method.
		/// </summary>
		/// <param name="state">The state to set as the current state.</param>
		public void Entry(IState state)
		{
			currentState = state;
			currentState?.Enter();
		}

		/// <summary>
		/// Initiates a transition to the specified transition.
		/// </summary>
		/// <param name="transition">The transition to initiate.</param>
		public void Transition(ITransition transition)
		{
			if (!isTransiting)
			{
				isTransiting = true;
				StartCoroutine(TransitionTo(transition));
			}
		}

		/// <summary>
		/// Coroutine that performs the transition to the specified transition.
		/// </summary>
		/// <param name="transition">The transition to perform.</param>
		private IEnumerator TransitionTo(ITransition transition)
		{
			yield return new WaitForSecondsRealtime(transition.transitTime);
			currentState?.Exit();
			previousState = currentState;
			currentState  = GetState(transition.toState);
			currentState?.Enter();
			isTransiting = false;
		}

		/// <summary>
		/// Initiates an "any" transition to the specified state.
		/// </summary>
		/// <param name="state">The state to transition to.</param>
		public void AnyTransition(IState state)
		{
			anyStates.Push(state);
			state.Enter();
		}

		/// <summary>
		/// Exits the most recently entered "any" state.
		/// </summary>
		public void ExitAnyStates()
		{
			if (anyStates.Count > 0)
				anyStates.Pop().Exit();
		}

		/// <summary>
		/// Executes the logic of the current state and "any" states.
		/// </summary>
		public void StatesExecution()
		{
			currentState?.Execute();
			foreach (var anyState in anyStates)
			{
				anyState?.Execute();
			}
		}

		/// <summary>
		/// Exits the specified state and enters the previously entered state.
		/// </summary>
		/// <param name="state">The state to exit.</param>
		public void Exit(IState state)
		{
			state?.Exit();
			previousState?.Enter();
		}

	}


}