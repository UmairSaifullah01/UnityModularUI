using System.Collections;


namespace THEBADDEST
{


	public interface ITransition
	{

		string      ToState    { get; }
		bool        IsAnyState { get; }
		bool        ClearAnyStates { get; }
		bool        ClearAllStates { get; }
		IEnumerator Execute();
		

	}


}