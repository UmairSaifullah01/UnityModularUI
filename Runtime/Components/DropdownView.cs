using System.Collections.Generic;
using THEBADDEST.MVVM;
using UnityEngine;
using TMPro;

namespace THEBADDEST.UI
{
    public class DropdownView : TMP_Dropdown, IView
    {
        public virtual string Id => gameObject.name;
        public IViewModel ViewModel { get; set; }
        private System.Action<int> onValueChangedAction;

        protected override void OnEnable()
        {
            base.OnEnable();
            onValueChanged.AddListener(OnDropdownValueChanged);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            onValueChanged.RemoveListener(OnDropdownValueChanged);
        }

        public virtual void Init(IViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.ViewModel.ModelBinder += Bind;
        }

        protected virtual void Bind(string id, IModel<object> model)
        {
            if (Id.Equals(id))
            {
                if (model.Data is int intValue)
                {
                    value = intValue;
                }
                else if (model.Data is string strValue)
                {
                    int idx = options.FindIndex(opt => opt.text == strValue);
                    if (idx >= 0) value = idx;
                }
                else if (model.Data is List<string> strList)
                {
                    options.Clear();
                    foreach (var s in strList)
                        options.Add(new OptionData(s));
                    RefreshShownValue();
                }
                else if (model.Data is System.Action<int> action)
                {
                    onValueChangedAction = action;
                }
            }
        }

        void OnDropdownValueChanged(int newValue)
        {
            onValueChangedAction?.Invoke(newValue);
        }

        public virtual void Active(bool active) => gameObject.SetActive(active);
        public Transform transformObject => transform;
    }
} 