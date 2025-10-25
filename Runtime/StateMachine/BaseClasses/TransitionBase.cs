using THEBADDEST.Tasks;


namespace THEBADDEST
{


	public class TransitionBase : ITransition
	{

		/// <summary>
		/// The ID of the state to transition to.
		/// </summary>
		public string ToState { get; }
		public bool IsAnyState     { get; }
		public bool ClearAnyStates { get; }
		public bool ClearAllStates { get; }

		/// <summary>
		/// The duration of the transition in seconds.
		/// </summary>
		public float transitTime { get; }

		public async UTask Execute()
		{
			if (transitTime > 0)
				await UTask.Delay(transitTime);
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
		public TransitionBase(string to, float exitTime = 0, bool isAnyState = false,bool clearAnyStates=false,bool clearAllStates = false)
		{
			this.ToState     = to;
			this.transitTime = exitTime;
			this.IsAnyState  = isAnyState;
			this.ClearAnyStates = clearAnyStates; 
			this.ClearAllStates = clearAllStates; 
		}


	}


}