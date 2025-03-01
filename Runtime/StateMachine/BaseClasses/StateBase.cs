﻿using System.Collections;
using UnityEngine;


namespace THEBADDEST
{


	/// <summary>
	/// Abstract base class for implementing states in a state machine.
	/// </summary>
	public abstract class StateBase :MonoBehaviour, IState
	{

		public virtual string        StateName    => this.GetType().Name;
		public         IStateMachine StateMachine { get; protected set; }
		/// <summary>
		/// Initializes the state with the given state machine.
		/// </summary>
		/// <param name="stateMachine">The state machine that owns this state.</param>
		public virtual void Init(IStateMachine stateMachine)
		{
			this.StateMachine = stateMachine;
		}

		/// <summary>
		/// Executes the state. Checks for executable transitions based on conditions and triggers state transitions.
		/// </summary>
		public virtual void Execute()
		{
			
		}

		/// <summary>
		/// Called when entering the state.
		/// </summary>
		public virtual IEnumerator Enter()
		{
			gameObject.SetActive(true);
			yield break;
		}

		/// <summary>
		/// Called when exiting the state.
		/// </summary>
		public virtual IEnumerator Exit()
		{
			gameObject.SetActive(false);
			yield break;
		}

	}


}