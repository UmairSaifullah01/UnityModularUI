using System.Collections;
using UnityEngine;


namespace THEBADDEST
{


	public class GameTransition : ScriptableObject, ITransition
	{

		[SerializeField, Options(typeof(IState))] private string targetState;
		[SerializeField]                          private float  transitTime;
		/// <summary>
		/// The ID of the state to transition to.
		/// </summary>
		public string toState => targetState;
		
		
		private IStateMachine stateMachine;
		public void Init(IStateMachine stateMachine)
		{
			this.stateMachine = stateMachine;
		}


		public IEnumerator Execute()
		{
			if (transitTime > 0)
				yield return new WaitForSecondsRealtime(transitTime);
			yield break;
		}

		[Button]
		public void Run()
		{
			stateMachine?.Transition(this);
		}
	}


}