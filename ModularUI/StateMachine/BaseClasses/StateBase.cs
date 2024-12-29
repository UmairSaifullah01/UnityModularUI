using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace THEBADDEST
{


	/// <summary>
	/// Abstract base class for implementing states in a state machine.
	/// </summary>
	public abstract class StateBase :MonoBehaviour, IState
	{

		public virtual string        StateName    { get; protected set; }
		public         IStateMachine StateMachine { get; protected set; }
		// Dictionary to cache the transitions associated with the panel
		private Dictionary<string, ITransition> cachedTransitions = new Dictionary<string, ITransition>();

		/// <summary>
		/// Initializes the state with the given state machine.
		/// </summary>
		/// <param name="stateMachine">The state machine that owns this state.</param>
		public virtual void Init(IStateMachine stateMachine)
		{
			this.StateMachine = stateMachine;
		}

		/// <summary>
		/// Sets the transitions for this state.
		/// </summary>
		/// <param name="transitions">The transitions to set.</param>
		public void SetTransitions(params ITransition[] transitions)
		{
			foreach (var transition in transitions)
			{
				StateMachine.LoadState(transition.toState, null);
				cachedTransitions.Add(transition.toState, transition);
			}
		}

		/// <summary>
		/// Retrieves the transitions for this state.
		/// </summary>
		/// <returns>An array of transitions.</returns>
		public ITransition[] GetTransitions()
		{
			return cachedTransitions.Values.ToArray();
		}

		/// <summary>
		/// Sets the condition value for a specific transition.
		/// </summary>
		/// <param name="stateName">The name of the destination state.</param>
		/// <param name="value">The condition value to set.</param>
		public void SetTransitionCondition(string stateName, bool value)
		{
			if (cachedTransitions.TryGetValue(stateName, out ITransition transition))
			{
				transition.condition = value;
			}
		}

		/// <summary>
		/// Executes the state. Checks for executable transitions based on conditions and triggers state transitions.
		/// </summary>
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

		/// <summary>
		/// Called when entering the state.
		/// </summary>
		public virtual void Enter()
		{
			gameObject.SetActive(true);
		}

		/// <summary>
		/// Called when exiting the state.
		/// </summary>
		public virtual void Exit()
		{
			gameObject.SetActive(false);
			foreach (var transition in cachedTransitions.Values)
			{
				transition.condition = false;
			}
		}

	}


}