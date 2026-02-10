using System;
using System.Collections.Generic;
using THEBADDEST.MVVM;
using THEBADDEST.Tasks;
using UnityEngine;
using UnityEngine.Serialization;


namespace THEBADDEST.UI
{


	/**
     * The `UIPanel` class represents a base class for UI panels in a user interface system.
     * It implements the `IState` and `IViewModel` interfaces and derives from `MonoBehaviour`.
     * This class provides functionality for managing the panel's state, transitions, and view model.
     */
	public abstract class UIState : StateBase, IViewModel
	{
		// Event triggered when the model data needs to be bound to the view
		public event Action<string, IModel<object>> ModelBinder
		{
			add => viewModel.ModelBinder += value;
			remove => viewModel.ModelBinder -= value;
		}
		// Dictionary to store the views associated with the panel
		public Dictionary<string, IView>            views
		{
			get => viewModel?.views ?? new Dictionary<string, IView>();
			set { if (viewModel != null) viewModel.views = value; }
		}

		public ITransition[] uiTransitions{get; set;}
		
		protected ViewModelBase        viewModel;

		protected Dictionary<string, ITransition> transitions;

		/**
         * Initializes the panel with the specified state machine.
         * It also initializes the view model and sets the panel's model to null.
         * @param stateMachine The state machine that manages the panel's state.
         */
		public override async UTask Init(IStateMachine stateMachine)
		{
			transitions = new Dictionary<string, ITransition>();
			if (uiTransitions != null)
			{
				foreach (var transition in uiTransitions)
				{
					if (transition != null)
					{
						transitions.TryAdd(transition.ToState, transition);
					}
				}
			}
			await base.Init(stateMachine);
			InitViewModel();
			gameObject.SetActive(false);
		}
		
		/**
         * Initializes the view model of the panel.
         * Retrieves and initializes the views associated with the panel.
         */
		public virtual void InitViewModel()
		{
			viewModel = new ViewModelBase(gameObject);
			viewModel.InitViewModel();
		}

		/**
         * Binds the specified value to the model with the given ID and invokes the model binder event.
         * @param id The ID of the model to bind.
         * @param value The value to bind to the model.
         */
		public void Binder<T>(string id, T value)
		{
			if (viewModel == null)
			{
				return;
			}
			viewModel.Binder(id, value);
		}

		protected void Binder(string id, object value)
		{
			Binder<object>(id, value);
		}
		protected void StringBinder(string id, string value)
		{
			Binder<string>(id, value);
		}

		protected void FloatBinder(string id, float value)
		{
			Binder<float>(id, value);
		}

		protected void EventBinder(string id, Action value)
		{
			Binder<Action>(id, value);
		}

		protected void GoToState(string state)
		{
			if (transitions.TryGetValue(state, out var transition))
			{
				StateMachine.Transition(transition);
			}
		}
	}


}