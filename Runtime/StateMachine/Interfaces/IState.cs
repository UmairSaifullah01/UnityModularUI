using System.Collections;
using THEBADDEST.Tasks;


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
		/// Indicates whether the state is currently paused.
		/// </summary>
		bool Paused { get; }

		/// <summary>
		/// Initializes the state with a reference to the state machine.
		/// </summary>
		/// <param name="stateMachine">The state machine that the state belongs to.</param>
		UTask Init(IStateMachine stateMachine);
		

		/// <summary>
		/// Actions to be performed when entering the state.
		/// </summary>
		UTask Enter();

		/// <summary>
		/// Actions to be performed when exiting the state.
		/// </summary>
		UTask Exit();

		/// <summary>
		/// Pauses the state execution.
		/// </summary>
		UTask Pause();

		/// <summary>
		/// Resumes the state execution.
		/// </summary>
		UTask Resume();
	}



}