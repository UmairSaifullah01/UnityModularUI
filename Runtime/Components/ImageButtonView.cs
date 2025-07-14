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
			if (clickTween != null)
			{
				clickTween.onComplete += onclickEvent.Invoke;
			}
		}

		protected override void Bind(string id, IModel<object> model)
		{
			if (!this.Id.Equals(id)) return;
			base.Bind(id, model);
			if (Id.Equals(id))
			{
				if (model.Data is Action action)
				{
					onclickEvent = action;
				}
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