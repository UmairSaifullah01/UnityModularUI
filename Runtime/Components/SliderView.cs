using System;
using System.Collections;
using System.Collections.Generic;
using THEBADDEST.MVVM;
using UnityEngine;
using UnityEngine.UI;


namespace THEBADDEST.UI
{


	public class SliderView : Slider, IView
	{

		[SerializeField, HideInInspector] protected RectTransform cachedRectTransform;
		/// <summary>
		/// Retrieves the transform component of the view.
		/// </summary>
		public RectTransform rectTransformObject
		{
			get
			{
				if (cachedRectTransform == null) cachedRectTransform = GetComponent<RectTransform>();
				return cachedRectTransform;
			}
		}

		public virtual string     Id        => gameObject.name;
		public         IViewModel ViewModel { get; set; }

		Action<float> onValueChangedAction;
		public virtual void Init(IViewModel viewModel)
		{
			this.ViewModel             =  viewModel;
			this.ViewModel.ModelBinder += Bind;
		}

		public virtual void Active(bool active) => gameObject.SetActive(active);

		public Transform transformObject => transform;

		protected override void OnEnable()
		{
			base.OnEnable();
			onValueChanged.AddListener(OnValueChanged);
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			onValueChanged.RemoveListener(OnValueChanged);
		}

		void OnValueChanged(float sliderValue)
		{
			onValueChangedAction?.Invoke(sliderValue);
		}
		void Bind(string id, IModel<object> model)
		{
			if (Id.Equals(id))
			{
				if (model.Data is float dataValue)
				{
					value = dataValue;
				}
				if (model.Data is Action<float> onValueChangeEvent)
				{
					onValueChangedAction = onValueChangeEvent;
				}
			}
		}
	

	}


}