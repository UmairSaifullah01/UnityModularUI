using System;
using System.Collections.Generic;


namespace THEBADDEST.MVVM
{


	public interface IViewModel
	{

		/// <summary>
		/// Event that binds an IModel to a specified key.
		/// </summary>
		event Action<string, IModel<object>> ModelBinder;


		/// <summary>
		/// Dictionary of views associated with their respective keys.
		/// </summary>
		Dictionary<string, IView> views { get; set; }
		

		/// <summary>
		/// Binds a value to a specified key in the model.
		/// </summary>
		/// <typeparam name="T">The type of the value to bind.</typeparam>
		/// <param name="id">The unique identifier for the binding.</param>
		/// <param name="value">The value to bind.</param>
		void Binder<T>(string id, T value);

		/// <summary>
		/// Initializes the ViewModel.
		/// </summary>
		void InitViewModel();

	}


}