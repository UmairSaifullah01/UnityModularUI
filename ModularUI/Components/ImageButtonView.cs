using System;
using THEBADDEST.MVVM;
using UnityEngine.EventSystems;


namespace THEBADDEST.UI
{


	public class ImageButtonView : ImageView, IPointerClickHandler
	{

		Action onclickEvent;

		public override void Init(IViewModel viewModel)
		{
			base.Init(viewModel);
			ViewModel.ModelBinder += EventBind;
		}

		void EventBind(string id, IModel<object> model)
		{
			if (!this.Id.Equals(id)) return;
			if (model.Data is Action action)
			{
				onclickEvent = action;
			}
		}


		public void OnPointerClick(PointerEventData eventData)
		{
			onclickEvent?.Invoke();
		}

	}


}