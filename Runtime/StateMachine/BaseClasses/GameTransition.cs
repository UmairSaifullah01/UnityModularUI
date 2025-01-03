using System.Collections;
using UnityEngine;


namespace THEBADDEST
{
	
	public class GameTransition : ScriptableObject, ITransition
	{

		[SerializeField, Options(typeof(IState))] private string targetState;
		[SerializeField]                          private bool   isAnyState;
		[SerializeField]                          private bool   clearStates;
		/// <summary>
		/// The ID of the state to transition to.
		/// </summary>
		public string ToState => targetState;
		public bool IsAnyState  => isAnyState;
		public bool ClearStates => clearStates;


		public IEnumerator Execute()
		{
			yield break;
		}

		[Button]
		public void Run()
		{
			var stateMachine = ServiceLocator.Global.GetService<IStateMachine>();
			if(stateMachine!=null) stateMachine.Transition(this);
		}
	}


}