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
		/// Initializes the ViewModel.
		/// </summary>
		void InitViewModel();

	}


}