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
		private static readonly string LogTag = "<color=orange>[UI-Volume]</color>";

		[SerializeField] Camera uiCamera;
		IUIStateFactory uiStateFactory;

		private void DebugLogError(string message)
		{
			if (!enableDebugLogs) return;
			Debug.LogError($"{LogTag} {message}");
		}
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
			uiStateFactory= new UIStateFactory(transform, uiCamera, enableDebugLogs);
		}
		
		

		public override IState GetState(string id)
		{
			if (cachedStates.TryGetValue(id, out IState cachedState) && cachedState != null)
			{
				return cachedState;
			}
			
			var state=uiStateFactory.CreateState(id);
			if (state == null)
			{
				DebugLogError($"State component not found in state instance with id {id}");
				return null;
			}
			state.Init(this);
			cachedStates.Add(id, state);
			return state;
		}

		

	}


}