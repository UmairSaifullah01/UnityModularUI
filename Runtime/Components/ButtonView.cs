using System;
using THEBADDEST.MVVM;
using UnityEngine;
using UnityEngine.EventSystems;
using THEBADDEST.Tweening;
namespace THEBADDEST.UI
{
	public class ButtonView : ViewBase, IPointerClickHandler
	{

		[SerializeField] protected Tween clickTween;
		protected Action onclickEvent;

		public override void Init(IViewModel viewModel)
		{
			base.Init(viewModel);
			ViewModel.ModelBinder += EventBind;
			clickTween.onComplete += () => onclickEvent?.Invoke();
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
			PerformClick();
		}

		protected virtual void PerformClick()
		{
			if (clickTween != null)
			{
				StartCoroutine(clickTween.Play(transform));
			}
			else
			{
				onclickEvent?.Invoke();
			}
		}

	}
}
