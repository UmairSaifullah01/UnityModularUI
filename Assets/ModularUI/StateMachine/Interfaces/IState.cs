namespace THEBADDEST
{


	/// <summary>
	/// Interface for defining a state in a state machine.
	/// </summary>
	public interface IState
	{
		/// <summary>
		/// The name of the state.
		/// </summary>
		string StateName { get; }

		/// <summary>
		/// The state machine that the state belongs to.
		/// </summary>
		IStateMachine StateMachine { get; }

		/// <summary>
		/// Initializes the state with a reference to the state machine.
		/// </summary>
		/// <param name="stateMachine">The state machine that the state belongs to.</param>
		void Init(IStateMachine stateMachine);
    
		/// <summary>
		/// Executes the logic of the state.
		/// </summary>
		void Execute();

		/// <summary>
		/// Actions to be performed when entering the state.
		/// </summary>
		void Enter();

		/// <summary>
		/// Actions to be performed when exiting the state.
		/// </summary>
		void Exit();
	}



}