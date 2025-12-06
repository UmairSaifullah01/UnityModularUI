using THEBADDEST.DatabaseModule;
using UnityEngine;


namespace THEBADDEST.UI
{


	public interface IUIStateFactory
	{

		IState CreateState(string id);

	}

	public class UIStateFactory : IUIStateFactory
	{
		private static readonly string LogTag = "<color=orange>[UI-StateFactory]</color>";

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
				DebugLog($"State container not found for id {id}");
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
					DebugLog($"Canvas component not found in state instance with id {id}");
					return null;
				}
			}

			canvas.worldCamera = _uiCamera;
			var state = stateInstance?.GetComponent<UIState>();
			if (state == null)
			{
				DebugLog($"State component not found in state instance with id {id}");
				return null;
			}

			state.uiTransitions = stateContainer.GetTransitions();
			return state;
		}

		private void DebugLog(string message)
		{
			if (!enableDebugLogs) return;
			Debug.Log($"{LogTag} {message}");
		}

	}


}