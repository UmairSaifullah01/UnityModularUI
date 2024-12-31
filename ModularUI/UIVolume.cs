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
	public class UIVolume : StateMachineBase
	{

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
				state.Init(this);
			}
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
		/// Loads the state with the specified ID and invokes the specified callback when the state is loaded.
		/// </summary>
		/// <param name="id">The ID of the state to load.</param>
		/// <param name="onStateLoad">The callback to invoke when the state is loaded.</param>
		public override  void LoadState(string id, Action<IState> onStateLoad)
		{
			if (cachedStates.TryGetValue(id, out IState cachedState))
			{
				onStateLoad?.Invoke(cachedState);
			}
		}
		
	}


}