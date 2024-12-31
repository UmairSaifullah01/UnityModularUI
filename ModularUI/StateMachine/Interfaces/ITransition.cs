using System.Collections;


namespace THEBADDEST
{


	public interface ITransition
	{

		string toState     { get; }
		
		IEnumerator Execute();

		void Run();

	}


}