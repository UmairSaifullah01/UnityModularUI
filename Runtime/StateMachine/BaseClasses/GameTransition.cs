using System.Collections;
using UnityEngine;


namespace THEBADDEST
{
	
	public class GameTransition : ScriptableObject, ITransition
	{

		[SerializeField, Options(typeof(IState))] private string targetState;
		[SerializeField]                          private bool   isAnyState;
		[SerializeField]                          private bool   clearAnyStates;
		[SerializeField]                          private bool   clearAllStates;
		/// <summary>
		/// The ID of the state to transition to.
		/// </summary>
		public string ToState => targetState;
		public bool IsAnyState     => isAnyState;
		public bool ClearAnyStates => clearAnyStates;
		public bool ClearAllStates => clearAllStates;
		


		public virtual IEnumerator Execute()
		{
			yield break;
		}

		[EditorTools.Button]
		public void Run()
		{
			var stateMachine = ServiceLocator.Global.GetService<IStateMachine>();
			if(stateMachine!=null) stateMachine.Transition(this);
		}
	}


}