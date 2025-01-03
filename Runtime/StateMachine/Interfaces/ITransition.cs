using System.Collections;


namespace THEBADDEST
{


	public interface ITransition
	{

		string      ToState    { get; }
		bool        IsAnyState { get; }
		bool        ClearStates { get; }
		IEnumerator Execute();
		

	}


}