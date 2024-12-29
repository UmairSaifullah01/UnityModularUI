using System;
using System.Collections.Generic;
using THEBADDEST.MVVM;


namespace THEBADDEST.UI
{


	/**
     * The `UIPanel` class represents a base class for UI panels in a user interface system.
     * It implements the `IState` and `IViewModel` interfaces and derives from `MonoBehaviour`.
     * This class provides functionality for managing the panel's state, transitions, and view model.
     */
	public abstract class UIPanel : StateBase, IViewModel
	{

		// Event triggered when the model data needs to be bound to the view
		public event Action<string, IModel<object>> ModelBinder;
		
		// Dictionary to store the views associated with the panel
		public Dictionary<string, IView> views { get; set; }
		
		
		// Reference to the panel's model
		private ModelBase model;
		
		
		
		/**
         * Initializes the panel with the specified state machine.
         * It also initializes the view model and sets the panel's model to null.
         * @param stateMachine The state machine that manages the panel's state.
         */
		public override void Init(IStateMachine stateMachine)
		{
			base.Init(stateMachine);;
			InitViewModel();
			model = new ModelBase(null);
			gameObject.SetActive(false);
		}
		
		/**
         * Initializes the view model of the panel.
         * Retrieves and initializes the views associated with the panel.
         */
		public virtual void InitViewModel()
		{
			if (views == null)
			{
				var v = GetComponentsInChildren<IView>(true);
				views = new Dictionary<string, IView>();
				foreach (IView view in v)
				{
					views.Add(view.Id, view);
				}
			}

			foreach (var view in views.Values)
			{
				view.Init(this);
			}
		}

		/**
         * Binds the specified value to the model with the given ID and invokes the model binder event.
         * @param id The ID of the model to bind.
         * @param value The value to bind to the model.
         */
		protected void Binder(string id, object value)
		{
			model.Data = value;
			ModelBinder?.Invoke(id, model);
		}

	}


}