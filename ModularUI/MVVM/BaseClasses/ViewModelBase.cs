using System;
using System.Collections.Generic;
using UnityEngine;


namespace THEBADDEST.MVVM
{

	
	public class ViewModelBase : MonoBehaviour,IViewModel
	{

		public event Action<string, IModel<object>> ModelBinder;
		public Dictionary<string, IView>            views          { get; set; }

		public virtual void InitViewModel()
		{
			if (views == null)
			{
				var v = GetComponentsInChildren<IView>(true);
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
		protected void StringBinder(string id, string value)
		{
			var model = new ModelBase(value);
			ModelBinder?.Invoke(id, model);
		}

		protected void FloatBinder(string id, float value)
		{
			var model = new ModelBase(value);
			ModelBinder?.Invoke(id, model);
		}

		protected void EventBinder(string id, Action value)
		{
			var model = new ModelBase(value);
			ModelBinder?.Invoke(id, model);
		}
	}


}