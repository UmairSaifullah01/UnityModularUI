using System.Collections.Generic;
using System.Linq;


namespace THEBADDEST
{


	/// <summary>
	/// Abstract base class for implementing states in a state machine.
	/// </summary>
	public abstract class StateBase : IState
	{

		public  string            StateName    { get; }
		public  IStateMachine     StateMachine { get; protected set; }
		private List<ITransition> cachedTransitions = new List<ITransition>();

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
			}

			this.cachedTransitions = transitions.ToList();
		}

		/// <summary>
		/// Retrieves the transitions for this state.
		/// </summary>
		/// <returns>An array of transitions.</returns>
		public ITransition[] GetTransitions()
		{
			return cachedTransitions.ToArray();
		}

		/// <summary>
		/// Sets the condition value for a specific transition.
		/// </summary>
		/// <param name="stateName">The name of the destination state.</param>
		/// <param name="value">The condition value to set.</param>
		public void SetTransitionCondition(string stateName, bool value)
		{
			var trans                          = cachedTransitions.FirstOrDefault(x => x.toState == stateName);
			if (trans != null) trans.condition = value;
		}

		/// <summary>
		/// Executes the state. Checks for executable transitions based on conditions and triggers state transitions.
		/// </summary>
		public virtual void Execute()
		{
			var executableTransition = cachedTransitions.FirstOrDefault(x => x.condition);
			if (executableTransition != null)
			{
				StateMachine.Transition(executableTransition);
			}
		}

		/// <summary>
		/// Called when entering the state.
		/// </summary>
		public abstract void Enter();

		/// <summary>
		/// Called when exiting the state.
		/// </summary>
		public abstract void Exit();

	}


}