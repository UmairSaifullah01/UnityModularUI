using System.Collections;
using THEBADDEST.Tasks;
using UnityEngine;


namespace THEBADDEST
{


	/// <summary>
	/// Abstract base class for implementing states in a state machine.
	/// </summary>
	public abstract class StateBase : MonoBehaviour, IState
	{

		public virtual string StateName => this.GetType().Name;
		public IStateMachine StateMachine { get; protected set; }
		public bool Paused { get; protected set; }

		/// <summary>
		/// Initializes the state with the given state machine.
		/// </summary>
		/// <param name="stateMachine">The state machine that owns this state.</param>
		public virtual async UTask Init(IStateMachine stateMachine)
		{
			this.StateMachine = stateMachine;
		}
		

		/// <summary>
		/// Called when entering the state.
		/// </summary>
		public virtual async UTask Enter()
		{
			gameObject.SetActive(true);
		}

		/// <summary>
		/// Called when exiting the state.
		/// </summary>
		public virtual async UTask Exit()
		{
			gameObject.SetActive(false);
		}

		/// <summary>
		/// Pauses the state execution.
		/// </summary>
		public virtual async UTask Pause()
		{
			Paused = true;
		}

		/// <summary>
		/// Resumes the state execution.
		/// </summary>
		public virtual async UTask Resume()
		{
			Paused = false;
		}

	}


}