using System;
using System.Collections.Generic;
using THEBADDEST.MVVM;
using UnityEngine;


namespace THEBADDEST.MVVM
{


	public class ViewModelMono : MonoBehaviour, IViewModel
	{

		public Dictionary<string, IView>            views
		{
			get => ViewModel.views;
			set => ViewModel.views = value;
		}
		public event Action<string, IModel<object>> ModelBinder
		{
			add => ViewModel.ModelBinder    += value;
			remove => ViewModel.ModelBinder -= value;
		}
		
		protected ViewModelBase ViewModel;
		
		public virtual void InitViewModel()
		{
			ViewModel = new ViewModelBase(gameObject);
			ViewModel.InitViewModel();
		}
		public void Binder(string id, object value)
		{
			ViewModel.Binder(id, value);
		}
		public void StringBinder(string id, string value)
		{
			ViewModel.StringBinder(id,value);
		}

		public void FloatBinder(string id, float value)
		{
			ViewModel.FloatBinder(id,value);
		}

		public void EventBinder(string id, Action value)
		{
			ViewModel.EventBinder(id, value);
		}

	}


}

