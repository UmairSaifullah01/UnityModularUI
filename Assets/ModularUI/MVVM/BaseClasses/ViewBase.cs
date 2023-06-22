using System.Collections;
using System.Collections.Generic;
using THEBADDEST.MVVM;
using UnityEngine;


namespace THEBADDEST.UI
{


	/// <summary>
	/// Base class for implementing a view in the MVVM pattern.
	/// </summary>
	public class ViewBase : MonoBehaviour, IView
	{
		/// <summary>
		/// Gets the unique identifier of the view.
		/// </summary>
		public string Id => gameObject.name;

		/// <summary>
		/// Gets or sets the associated view model.
		/// </summary>
		public IViewModel ViewModel { get; set; }

		/// <summary>
		/// Initializes the view with the provided view model.
		/// </summary>
		/// <param name="viewModel">The view model to associate with the view.</param>
		public virtual void Init(IViewModel viewModel)
		{
			this.ViewModel = viewModel;
		}

		/// <summary>
		/// Activates or deactivates the view.
		/// </summary>
		/// <param name="active">The desired active state of the view.</param>
		public virtual void Active(bool active)
		{
			gameObject.SetActive(active);
		}

		/// <summary>
		/// Retrieves the transform component of the view.
		/// </summary>
		/// <returns>The transform component of the view.</returns>
		public Transform GetTransform()
		{
			return transform;
		}
	}



}