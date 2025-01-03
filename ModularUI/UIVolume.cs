using System;
using THEBADDEST.DatabaseModule;
using UnityEngine;


namespace THEBADDEST.UI
{
	

	/// <summary>
	/// The <c>UIVolume</c> class represents a UI volume that acts as a state machine for managing UI states.
	/// It implements the <see cref="IStateMachine"/> interface and extends the <see cref="MonoBehaviour"/> class.
	/// </summary>
	public class UIVolume : StateMachineBase
	{

		[SerializeField] Camera uiCamera;
		IUIStateFactory uiStateFactory;
		/// <summary>
		/// Called when the script instance is being loaded.
		/// Initializes the UI volume by caching states and initializing them.
		/// </summary>
		private void Awake()
		{
			gameObject.name = nameof(UIVolume);
			var states = GetComponentsInChildren<IState>(true);
			foreach (var state in states)
			{
				cachedStates.Add(state.GetStateName(), state);
			}
			foreach (var state in cachedStates.Values)
			{
				state.Init(this);
			}
			uiStateFactory= new UIStateFactory(transform, uiCamera);
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

		public override IState GetState(string id)
		{
			if (cachedStates.TryGetValue(id, out IState cachedState))
			{
				return cachedState;
			}
			
			var state=uiStateFactory.CreateState(id);
			if (state == null)
			{
				Debug.Log($"State component not found in state instance with id {id}");
			}
			state?.Init(this);
			cachedStates.Add(id,state);
			return base.GetState(id);
		}

		

	}


}