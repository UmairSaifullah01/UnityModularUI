using THEBADDEST.DatabaseModule;
using THEBADDEST;
using UnityEngine;


namespace THEBADDEST.UI
{


	public interface IUIStateFactory
	{
		IState CreateState(string id);
		/// <summary>Returns a transition to the boot state defined in ApplicationFlow, or null if none.</summary>
		ITransition GetBootState();
	}

	public class UIStateFactory : IUIStateFactory
	{
		private readonly Transform _transform;
		private readonly Camera _uiCamera;
		ApplicationFlow applicationFlow;
		private bool enableDebugLogs;

		public UIStateFactory(Transform transform, Camera uiCamera, bool enableDebugLogs = true)
		{
			_transform = transform;
			_uiCamera = uiCamera;
			this.enableDebugLogs = enableDebugLogs;
			applicationFlow = DatabaseServiceLocator.DatabaseService().GetTable<ApplicationFlow>();
		}

		public IState CreateState(string id)
		{
			var stateContainer = applicationFlow.GetByKey(x => x.stateName, id);
			if (stateContainer == null)
			{
				UILog.Log($"State container not found for id {id}");
				return null;
			}

			var stateInstance = Object.Instantiate(stateContainer.stateObject, _transform);
			stateInstance.name = id;
			var canvas = stateInstance?.GetComponent<Canvas>();
			if (canvas == null)
			{
				canvas = stateInstance?.GetComponentInChildren<Canvas>();
				if (canvas == null)
				{
					UILog.Log($"Canvas component not found in state instance with id {id}");
					return null;
				}
			}

			canvas.worldCamera = _uiCamera;
			var state = stateInstance?.GetComponent<UIState>();
			if (state == null)
			{
				UILog.Log($"State component not found in state instance with id {id}");
				return null;
			}

			state.uiTransitions = stateContainer.GetTransitions();
			return state;
		}

		/// <summary>Returns a transition to the boot state defined in ApplicationFlow, or null if none.</summary>
		public ITransition GetBootState()
		{
			if (applicationFlow == null || string.IsNullOrEmpty(applicationFlow.BootStateName))
				return null;
			return new TransitionBase(applicationFlow.BootStateName);
		}
	}


}