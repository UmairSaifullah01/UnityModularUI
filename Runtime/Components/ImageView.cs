using THEBADDEST.MVVM;
using UnityEngine;
using UnityEngine.UI;


namespace THEBADDEST.UI
{


	public class ImageView : Image, IView
	{

		public virtual string     Id        => gameObject.name;
		public         IViewModel ViewModel { get; set; }

		public virtual void Init(IViewModel viewModel)
		{
			this.ViewModel             =  viewModel;
			this.ViewModel.ModelBinder += Bind;
		}

		protected virtual void Bind(string id, IModel<object> model)
		{
			if (Id.Equals(id))
			{
				switch (model.Data)
				{
					case Sprite dataSprite:
						sprite = dataSprite;
						break;

					case float dataFillAmount:
						fillAmount = dataFillAmount;
						break;

					case Color dataColor:
						color = dataColor;
						break;
				}
			}
			else if ($"{this.Id}Alpha".Equals(id))
			{
				if (model.Data is float alpha)
				{
					Color c = color;
					c.a   = alpha;
					color = c;
				}
			}
		}


		public virtual void Active(bool active) => gameObject.SetActive(active);

		public Transform transformObject => transform;

	}


}