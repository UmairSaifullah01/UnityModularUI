using System;
using THEBADDEST.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace THEBADDEST.UI
{
    public class StepperView : UIView
    {
        [SerializeField] private Button plusButton;
        [SerializeField] private Button minusButton;
        [SerializeField] private Text valueText;
        public virtual string Id => gameObject.name;
        public IViewModel ViewModel { get; set; }
        private float value = 0;
        public Action<float> OnValueChanged;

        public virtual void Init(IViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.ViewModel.ModelBinder += Bind;
            plusButton.onClick.AddListener(OnPlus);
            minusButton.onClick.AddListener(OnMinus);
        }

        protected virtual void Bind(string id, IModel<object> model)
        {
            if (Id.Equals(id))
            {
                if (model.Data is float f)
                {
                    value = f;
                    UpdateValueText();
                }
                else if (model.Data is int i)
                {
                    value = i;
                    UpdateValueText();
                }
            }
        }

        void OnPlus()
        {
            if (UIUtils.WaitBetweenClick())
                return;

            value++;
            UpdateValueText();
            OnValueChanged?.Invoke(value);
        }

        void OnMinus()
        {
            if (UIUtils.WaitBetweenClick())
                return;

            value--;
            UpdateValueText();
            OnValueChanged?.Invoke(value);
        }

        void UpdateValueText()
        {
            if (valueText != null)
                valueText.text = value.ToString();
        }

        public virtual void Active(bool active) => gameObject.SetActive(active);
        public Transform transformObject => transform;
    }
} 