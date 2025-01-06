using System;
using System.Collections;
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
				StartCoroutine(PlayTween(transform));
			}
			else
			{
				onclickEvent?.Invoke();
			}
		}

		protected virtual IEnumerator PlayTween(Transform target)
		{
			yield return clickTween.Play(target);
			onclickEvent?.Invoke();
		}

	}
}
