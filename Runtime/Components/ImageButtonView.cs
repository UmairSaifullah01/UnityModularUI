using System;
using THEBADDEST.MVVM;
using THEBADDEST.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;



namespace THEBADDEST.UI
{


	public class ImageButtonView : ImageView, IPointerClickHandler
	{

		[SerializeField]Tween  clickTween;
		Action                onclickEvent;

		public override void Init(IViewModel viewModel)
		{
			base.Init(viewModel);
			ViewModel.ModelBinder += EventBind;
			if (clickTween != null)
			{
				clickTween.onComplete += onclickEvent.Invoke;
			}
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
			if (clickTween != null)
			{
				StartCoroutine(clickTween.Play(transform));
			}
			else{
				onclickEvent?.Invoke();
			}
		}

	}


}