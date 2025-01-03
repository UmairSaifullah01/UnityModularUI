using System;
using THEBADDEST.MVVM;
using THEBADDEST.UI;
using UnityEngine.EventSystems;

public class ButtonView : ViewBase, IPointerClickHandler
{

	Action onclickEvent;

	public override void Init(IViewModel viewModel)
	{
		base.Init(viewModel);
		this.ViewModel.ModelBinder += Bind;
	}

	void Bind(string id, IModel<object> model)
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