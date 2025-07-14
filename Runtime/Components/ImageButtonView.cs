using System;
using THEBADDEST.MVVM;
using THEBADDEST.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;


namespace THEBADDEST.UI
{


	public class ImageButtonView : ImageView, IPointerClickHandler
	{

		Action onclickEvent;
		

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

		protected virtual async UTask PlayEffectAsync(Transform target)
		{
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			OnClickAsync();
		}

		private async void OnClickAsync()
		{
			await PlayEffectAsync(transform);
			onclickEvent?.Invoke();
		}

	}


}