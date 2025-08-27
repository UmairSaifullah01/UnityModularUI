using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
		[SerializeField]
		string currentStateName;

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
		public bool IsTransiting { get; private set; }

		/// <summary>
		/// The current any-state of the state machine.
		/// </summary>
		protected IState currentAnyState;

		[SerializeField]
		private bool enableDebugLogs = true;

		private static readonly string LogTag = "<color=orange>[UI-StateMachine]</color>";
		private void DebugLogState(string stateName, string action)
		{
			if (!enableDebugLogs) return;
			Debug.Log($"{LogTag} Current State : {stateName} - {action}.");
		}

		/// <summary>
		/// Retrieves a state from the cached states dictionary based on its ID.
		/// </summary>
		/// <param name="id">The ID of the state to retrieve.</param>
		/// <returns>The state associated with the specified ID.</returns>
		public virtual IState GetState(string id)
		{
			if (cachedStates.TryGetValue(id, out var state))
				return state;
			Debug.LogWarning($"State with ID {id} not found.");
			return null;
		}

		/// <summary>
		/// Initiates a transition to a new state based on a transition object.
		/// </summary>
		/// <param name="transition">The transition object specifying the target state.</param>
		public void Transition(ITransition transition)
		{
			if (IsTransiting) return;
			IsTransiting = true;
			TransitionTo(transition);
		}

		/// <summary>
		/// Initiates a transition to a new state based on a transition object using a coroutine.
		/// </summary>
		/// <param name="transition">The transition object specifying the target state.</param>
		/// <returns>An IEnumerator which can be used in a coroutine to transition to the target state.</returns>
		private async void TransitionTo(ITransition transition)
		{
			if (transition.ClearAllStates)
			{
				await ClearAllStates();
			}
			else if (transition.ClearAnyStates)
			{
				await ClearAnyStates();
			}
			else if (!transition.IsAnyState)
			{
				if (currentState != null)
				{
					await currentState.Exit();
					DebugLogState(currentState.StateName, "Exit");
				}
				previousState = currentState;
				currentState = null;
			}

			await transition.Execute();
			if (transition.IsAnyState)
			{
				// Pause the current state when entering an any-state
				if (currentState != null && !currentState.Paused)
				{
					await currentState.Pause();
					DebugLogState(currentState.StateName, "Paused");
				}
				currentAnyState = GetState(transition.ToState);
				if (currentAnyState is MonoBehaviour mbState && mbState != null)
				{
					if (!anyStates.Contains(currentAnyState))
					{
						anyStates.Push(currentAnyState);
					}

					currentStateName = currentAnyState.StateName;
					await currentAnyState.Enter();
					DebugLogState(currentAnyState.StateName, "Entered");
				}
			}
			else
			{
				currentState = GetState(transition.ToState);
				if (currentState is MonoBehaviour mbState && mbState != null)
				{
					currentStateName = currentState.StateName;
					await currentState.Enter();
					DebugLogState(currentState.StateName, "Entered");
				}
			}

			IsTransiting = false;
		}
		

		/// <summary>
		/// Exits a specific state.
		/// </summary>
		/// <param name="state">The state to exit.</param>
		public void ExitState(IState state)
		{
			if (state == null) return;
			state.Exit();
			if (currentState == state)
			{
				previousState = currentState;
				currentState = null;
				currentStateName = string.Empty;
			}
		}

		/// <summary>
		/// Exits the current any-state and transitions to the previous any-state if available.
		/// </summary>
		public async void ExitAnyState()
		{
			if (currentAnyState == null) return;
			await ExitAnyStateAsync();
		}

		private async Tasks.UTask ExitAnyStateAsync()
		{
			if (anyStates.Count > 0)
			{
				var previousAnyState = anyStates.Pop();
				if (previousAnyState is MonoBehaviour mbState && mbState != null)
				{
					await previousAnyState.Exit();
					DebugLogState(previousAnyState.StateName, "Exit");
				}
			}

			if (anyStates.Count > 0)
			{
				while (anyStates.Count > 0)
				{
					currentAnyState = anyStates.Pop();
					if (currentAnyState is MonoBehaviour mbState && mbState != null)
					{
						if (!anyStates.Contains(currentAnyState))
						{
							anyStates.Push(currentAnyState);
						}
						currentStateName = currentAnyState.StateName;
						await currentAnyState.Enter();
						DebugLogState(currentAnyState.StateName, "Entered");
						return;
					}
				}
			}
			else
			{
				currentAnyState = null;
				currentStateName = currentState == null ? string.Empty : currentState.StateName;
				// Resume the current state when all any-states are exited
				if (currentState != null && currentState.Paused)
				{
					await currentState.Resume();
					DebugLogState(currentState.StateName, "Resumed");
				}
			}
		}

		public void ClearStates()
		{
			ClearAllStates();
		}

		private async Tasks.UTask ClearAllStates()
		{
			var states = cachedStates.Values.ToArray();
			cachedStates.Clear();
			foreach (var state in states)
			{
				if (state is MonoBehaviour mbState && mbState != null)
				{
					state.Exit();
					Destroy(mbState.gameObject);
				}
			}
		}

		private async Tasks.UTask ClearAnyStates()
		{
			while (anyStates.Count > 0)
			{
				var state = anyStates.Pop();
				if (state is MonoBehaviour mbState && mbState != null)
				{
					cachedStates.Remove(state.GetStateName());
					state.Exit();
					Destroy(mbState.gameObject);
				}
			}

			anyStates.Clear();
		}

	}


}