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
		private readonly Transform _transform;
		private readonly Camera    _uiCamera;
		GameStatesTable            _gameStatesTable;
		public UIStateFactory(Transform transform, Camera uiCamera)
		{
			_transform              = transform;
			_uiCamera               = uiCamera;
			_gameStatesTable          = DatabaseServiceLocator.DatabaseService().GetTable<GameStatesTable>();
		}

		public IState CreateState(string id)
		{
			var stateContainer = _gameStatesTable.GetByKey(x => x.stateName, id);

			if (stateContainer == null)
			{
				Debug.Log($"State container not found for id {id}");
				return null;
			}

			var stateInstance = Object.Instantiate(stateContainer.stateObject, _transform);
			stateInstance.name = id;

			var canvas = stateInstance?.GetComponent<Canvas>();
			if (canvas == null)
			{
				Debug.Log($"Canvas component not found in state instance with id {id}");
				return null;
			}

			canvas.worldCamera = _uiCamera;

			var state = stateInstance?.GetComponent<IState>();
			if (state == null)
			{
				Debug.Log($"State component not found in state instance with id {id}");
				return null;
			}
			return state;
		}
	}


}