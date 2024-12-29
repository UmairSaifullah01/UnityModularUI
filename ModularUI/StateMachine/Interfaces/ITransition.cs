namespace THEBADDEST
{


	public interface ITransition
	{

		string toState     { get; }
		float  transitTime { get; }
		bool   condition   { get; set; }

		bool Decision();

	}


}