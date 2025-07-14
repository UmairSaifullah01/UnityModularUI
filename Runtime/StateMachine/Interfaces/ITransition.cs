using System.Collections;
using THEBADDEST.Tasks;


namespace THEBADDEST
{


	public interface ITransition
	{

		string      ToState    { get; }
		bool        IsAnyState { get; }
		bool        ClearAnyStates { get; }
		bool        ClearAllStates { get; }

		UTask Execute();


	}


}