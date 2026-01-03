using System;
using THEBADDEST.MVVM;
using THEBADDEST.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;


namespace THEBADDEST.UI
{


	public class ButtonView : UIView, IPointerClickHandler
	{

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

		protected virtual async void PerformClick()
		{
			if (UIUtils.WaitBetweenClick())
				return;

			await PlayEffectAsync(transform);
			onclickEvent?.Invoke();
		}

		protected virtual async UTask PlayEffectAsync(Transform target)
		{
			// Override to play effect
		}

	}


}