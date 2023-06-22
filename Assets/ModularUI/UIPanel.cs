using System;
using System.Collections.Generic;
using System.Linq;
using THEBADDEST.MVVM;
using UnityEngine;


namespace THEBADDEST.UI
{


	/**
     * The `UIPanel` class represents a base class for UI panels in a user interface system.
     * It implements the `IState` and `IViewModel` interfaces and derives from `MonoBehaviour`.
     * This class provides functionality for managing the panel's state, transitions, and view model.
     */
	public abstract class UIPanel : MonoBehaviour, IState, IViewModel
	{
		
		// Name of the panel's state
		public abstract string StateName { get; protected set; }
		
		
		// Reference to the state machine that manages the panel's state
		public IStateMachine StateMachine { get; private set; }

		// Event triggered when the model data needs to be bound to the view
		public event Action<string, IModel<object>> ModelBinder;

		
		// Dictionary to store the views associated with the panel
		public Dictionary<string, IView> views { get; set; }

		
		
		// Reference to the panel's model
		private ModelBase model;
		
		// Dictionary to cache the transitions associated with the panel
		private Dictionary<string, ITransition> cachedTransitions = new Dictionary<string, ITransition>();
		
		/**
         * Initializes the panel with the specified state machine.
         * It also initializes the view model and sets the panel's model to null.
         * @param stateMachine The state machine that manages the panel's state.
         */
		public virtual void Init(IStateMachine stateMachine)
		{
			this.StateMachine = stateMachine;
			InitViewModel();
			model = new ModelBase(null);
			gameObject.SetActive(false);
		}

		/**
         * Sets the transitions for the panel.
         * @param transitions The transitions associated with the panel.
         */
		public void SetTransitions(params ITransition[] transitions)
		{
			foreach (var transition in transitions)
			{
				StateMachine.LoadState(transition.toState, null);
				cachedTransitions.Add(transition.toState, transition);
			}
		}

		/**
         * Retrieves the transitions associated with the panel.
         * @return An array of transitions.
         */
		public ITransition[] GetTransitions()
		{
			return cachedTransitions.Values.ToArray();
		}

		/**
         * Sets the condition for a specific transition.
         * @param stateName The name of the state to set the condition for.
         * @param value The condition value.
         */
		public void SetTransitionCondition(string stateName, bool value)
		{
			if (cachedTransitions.TryGetValue(stateName, out ITransition transition))
			{
				transition.condition = value;
			}
		}

		/**
         * Executes the panel's logic and checks for executable transitions.
         * If a transition's condition is satisfied, the panel transitions to the corresponding state.
         */
		public virtual void Execute()
		{
			foreach (var transition in cachedTransitions.Values)
			{
				if (transition.Decision())
				{
					StateMachine.Transition(transition);
					break;
				}
			}
		}

		/**
         * Called when the panel enters the active state.
         * Activates the panel's game object.
         */
		public virtual void Enter()
		{
			gameObject.SetActive(true);
		}

		/**
         * Called when the panel exits the active state.
         * Deactivates the panel's game object and resets the transition conditions.
         */
		public virtual void Exit()
		{
			gameObject.SetActive(false);
			foreach (var transition in cachedTransitions.Values)
			{
				transition.condition = false;
			}
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