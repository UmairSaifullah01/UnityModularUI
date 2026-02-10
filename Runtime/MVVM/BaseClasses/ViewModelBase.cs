using System;
using System.Collections.Generic;
using THEBADDEST.UI;
using UnityEngine;


namespace THEBADDEST.MVVM
{

	
	public class ViewModelBase :IViewModel
	{

		public event Action<string, IModel<object>> ModelBinder;
		public Dictionary<string, IView>            views          { get; set; }

		protected GameObject gameObject;
// Reference to the panel's model
		private ModelBase model;
		public ViewModelBase(GameObject gameObject)
		{
			this.gameObject = gameObject;
		}
		public virtual void InitViewModel()
		{
			if (views == null)
			{
				var v = gameObject.GetComponentsInChildren<IView>(true);
				views = new Dictionary<string, IView>();
				
				// Check for duplicate IDs and log them
				CheckForDuplicates(v);
				
				// Add views to dictionary (only first occurrence of each ID will be added)
				foreach (IView view in v)
				{
					if (!views.ContainsKey(view.Id))
					{
						views.TryAdd(view.Id, view);
					}
				}
			}

			foreach (var view in views)
			{
				view.Value.Init(this);
			}
		}

		public void Binder<T>(string id, T value)
		{
			model = new ModelBase(value);
			ModelBinder?.Invoke(id, model);
		}

		public void Binder(string id, object value)
		{
			Binder<object>(id, value);
		}
		public void StringBinder(string id, string value)
		{
			Binder<string>(id, value);
		}

		public void FloatBinder(string id, float value)
		{
			Binder<float>(id, value);
		}

		public void EventBinder(string id, Action value)
		{
			Binder<Action>(id, value);
		}

		/// <summary>
		/// Checks for duplicate view IDs and logs warnings if any are found.
		/// </summary>
		/// <param name="views">Array of views to check for duplicates.</param>
		protected virtual void CheckForDuplicates(IView[] views)
		{
			var idGroups = new Dictionary<string, List<IView>>();
			foreach (IView view in views)
			{
				if (!idGroups.ContainsKey(view.Id))
				{
					idGroups[view.Id] = new List<IView>();
				}
				idGroups[view.Id].Add(view);
			}
			
			// Log duplicates if found
			foreach (var group in idGroups)
			{
				if (group.Value.Count > 1)
				{
					var viewNames = new List<string>();
					foreach (var view in group.Value)
					{
						viewNames.Add(view.transformObject.gameObject.name);
					}
					UILog.LogWarning($"Duplicate view IDs detected! ID: '{group.Key}' is used by {group.Value.Count} views: {string.Join(", ", viewNames)}");
				}
			}
		}
	}


}