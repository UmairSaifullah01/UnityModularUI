using System;
using System.Collections.Generic;
using THEBADDEST.MVVM;
using UnityEngine;

namespace THEBADDEST.UI
{
	/// <summary>
	/// UI component for displaying toast notifications.
	/// Implements IViewModel for MVVM pattern integration.
	/// </summary>
	public class ToasterUI : MonoBehaviour, IViewModel
	{
		[SerializeField] private Canvas canvas;
		
		/// <summary>
		/// Camera to use for the canvas. Set this before calling InitViewModel().
		/// </summary>
		public Camera UICamera { get; set; }
		
		public Dictionary<string, IView> views
		{
			get => ViewModel.views;
			set => ViewModel.views = value;
		}
		
		public event Action<string, IModel<object>> ModelBinder
		{
			add => ViewModel.ModelBinder += value;
			remove => ViewModel.ModelBinder -= value;
		}
		
		private ViewModelBase ViewModel { get; set; }

		/// <summary>
		/// Initializes the ViewModel and sets up the canvas camera.
		/// </summary>
		public void InitViewModel()
		{
			if (canvas == null)
			{
				canvas = GetComponent<Canvas>();
				if (canvas == null)
				{
					canvas = gameObject.AddComponent<Canvas>();
					canvas.renderMode = RenderMode.ScreenSpaceOverlay;
				}
			}
			
			if (UICamera != null)
			{
				canvas.renderMode = RenderMode.ScreenSpaceCamera;
				canvas.worldCamera = UICamera;
			}
			
			ViewModel = new ViewModelBase(gameObject);
			ViewModel.InitViewModel();
		}
		
		public void Binder(string id, object value)
		{
			ViewModel?.Binder(id, value);
		}
		
		public void StringBinder(string id, string value)
		{
			ViewModel?.StringBinder(id, value);
		}

		public void FloatBinder(string id, float value)
		{
			ViewModel?.FloatBinder(id, value);
		}

		public void EventBinder(string id, Action value)
		{
			ViewModel?.EventBinder(id, value);
		}
	}
}

