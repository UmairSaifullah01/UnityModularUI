using UnityEngine;


namespace THEBADDEST.MVVM
{


	public interface IView
	{

		/// <summary>
		/// Gets the unique identifier for the view.
		/// </summary>
		string Id { get; }

		/// <summary>
		/// Gets or sets the ViewModel associated with the view.
		/// </summary>
		IViewModel ViewModel { get; set; }

		/// <summary>
		/// Initializes the view with a specific ViewModel.
		/// </summary>
		/// <param name="viewModel">The ViewModel to associate with the view.</param>
		void Init(IViewModel viewModel);

		/// <summary>
		/// Gets the transform of the view's GameObject.
		/// </summary>
		/// <returns>The transform of the view's GameObject.</returns>
		Transform GetTransform();

		/// <summary>
		/// Activates or deactivates the view.
		/// </summary>
		/// <param name="active">True to activate the view, false to deactivate it.</param>
		void Active(bool active);

	}


}