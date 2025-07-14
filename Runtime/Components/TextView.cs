using System.Collections;
using System.Collections.Generic;
using THEBADDEST.MVVM;
using TMPro;
using UnityEngine;


namespace THEBADDEST.UI
{
	public class TextView : TextMeshProUGUI, IView
	{

		public virtual string     Id        => gameObject.name;
		public         IViewModel ViewModel { get; set; }


		public virtual void Init(IViewModel viewModel)
		{
			this.ViewModel             =  viewModel;
			this.ViewModel.ModelBinder += Bind;
		}

		void Bind(string id, IModel<object> model)
		{
			if (this.Id.Equals(id))
			{
				if (model.Data is string textValue)
					text = textValue;
			}
		}
	
		public virtual void Active(bool active) => gameObject.SetActive(active);

		public Transform transformObject => transform;

	}

	


}
