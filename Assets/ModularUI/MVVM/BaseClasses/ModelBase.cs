namespace THEBADDEST.MVVM
{

	/// <summary>
	/// Base class for implementing a model in the MVVM pattern.
	/// </summary>
	public class ModelBase : IModel<object>
	{
		/// <summary>
		/// Gets or sets the data of the model.
		/// </summary>
		public object Data { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ModelBase"/> class with the specified data.
		/// </summary>
		/// <param name="data">The data to be stored in the model.</param>
		public ModelBase(object data)
		{
			Data = data;
		}
	}



}