using System.Collections;
using UnityEngine;


namespace THEBADDEST
{


	public class TransitionBase : ITransition
	{

		/// <summary>
		/// The ID of the state to transition to.
		/// </summary>
		public string toState { get; }

		/// <summary>
		/// The duration of the transition in seconds.
		/// </summary>
		public float transitTime { get; }

		public IEnumerator Execute()
		{
			if (transitTime > 0)
				yield return new WaitForSecondsRealtime(transitTime);
			yield break;
		}

		public void Run()
		{
			condition=true;
		}

		/// <summary>
		/// The condition that determines if the transition should be taken.
		/// </summary>
		public virtual bool condition { get; set; }

		/// <summary>
		/// Creates a new instance of the TransitionBase class.
		/// </summary>
		/// <param name="to">The ID of the state to transition to.</param>
		/// <param name="exitTime">The duration of the transition in seconds.</param>
		public TransitionBase(string to, float exitTime = 0)
		{
			this.toState     = to;
			this.transitTime = exitTime;
		}


	}


}