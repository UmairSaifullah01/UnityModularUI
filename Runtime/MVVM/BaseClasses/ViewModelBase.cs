using System;
using System.Collections.Generic;
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
				foreach (IView view in v)
				{
					views.Add(view.Id, view);
				}
			}

			foreach (var view in views)
			{
				view.Value.Init(this);
			}
		}

		public void Binder(string id, object value)
		{
			 model = new ModelBase(value);
			ModelBinder?.Invoke(id, model);
		}
		public void StringBinder(string id, string value)
		{
			 model = new ModelBase(value);
			ModelBinder?.Invoke(id, model);
		}

		public void FloatBinder(string id, float value)
		{
			 model = new ModelBase(value);
			ModelBinder?.Invoke(id, model);
		}

		public void EventBinder(string id, Action value)
		{
			model = new ModelBase(value);
			ModelBinder?.Invoke(id, model);
		}
	}


}