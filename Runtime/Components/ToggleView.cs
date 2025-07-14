using THEBADDEST.MVVM;
using UnityEngine;
using System;
using THEBADDEST.Tasks;
using UnityEngine.EventSystems;


namespace THEBADDEST.UI
{


	public class ToggleView : ViewBase, IPointerClickHandler
	{
		[SerializeField]  Transform onObject;
		[SerializeField]           Transform offObject;
		
		Action<bool> onValueChanged;
		

		public override void Init(IViewModel viewModel)
		{
			base.Init(viewModel);
			viewModel.ModelBinder+= Bind;
		}
		public void OnPointerClick(PointerEventData eventData)
		{
			PerformClick();
		}
		void Bind(string id, IModel<object> model)
		{
			if (!this.Id.Equals(id)) return;
			if (model.Data is Action<bool> action)
			{
				onValueChanged = action;
			}

			if (model.Data is float value)
			{
				var isOn= Mathf.Approximately(value, 1);
				onObject.gameObject.SetActive(isOn);
				offObject.gameObject.SetActive(!isOn);
			}
		}

		protected virtual async UTask PlayEffectAsync(Transform target)
		{
		
		}

		async void PerformClick()
		{
			if (onObject.gameObject.activeSelf)
			{
				onObject.gameObject.SetActive(false);
				offObject.gameObject.SetActive(true);
				await PlayEffectAsync(offObject);
			}
			else
			{
				onObject.gameObject.SetActive(true);
				offObject.gameObject.SetActive(false);
				await PlayEffectAsync(onObject);
			}

			onValueChanged?.Invoke(onObject.gameObject.activeSelf);
		}

	}
}