namespace THEBADDEST.MVVM
{


	/// <summary>
	/// Interface for defining a model in the MVVM pattern.
	/// </summary>
	/// <typeparam name="T">The type of data held by the model.</typeparam>
	public interface IModel<T>
	{

		/// <summary>
		/// Gets or sets the data of the model.
		/// </summary>
		T Data { get; set; }

	}


}