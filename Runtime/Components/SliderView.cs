using System.Collections;
using System.Collections.Generic;
using THEBADDEST.MVVM;
using UnityEngine;
using UnityEngine.UI;


namespace THEBADDEST.UI
{


	public class SliderView : Slider, IView
	{

		public virtual string     Id        => gameObject.name;
		public         IViewModel ViewModel { get; set; }

		public virtual void Init(IViewModel viewModel)
		{
			this.ViewModel             =  viewModel;
			this.ViewModel.ModelBinder += Bind;
		}

		public virtual void Active(bool active) => gameObject.SetActive(active);

		public Transform transformObject => transform;

		void Bind(string id, IModel<object> model)
		{
			if (Id.Equals(id))
			{
				if (model.Data is float dataValue)
				{
					value = dataValue;
				}
			}
		}

	}


}