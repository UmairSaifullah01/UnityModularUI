using System;
using THEBADDEST.MVVM;
using UnityEngine;
using TMPro;

namespace THEBADDEST.UI
{
    public class InputFieldView : TMP_InputField, IView
    {
        public virtual string Id => gameObject.name;
        public IViewModel ViewModel { get; set; }
        private Action<string> onValueChangedAction;

        protected override void OnEnable()
        {
            base.OnEnable();
            onValueChanged.AddListener(OnTextChanged);
            onEndEdit.AddListener(OnEndEdit);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            onValueChanged.RemoveListener(OnTextChanged);
            onEndEdit.RemoveListener(OnEndEdit);
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
                if (model.Data is string strValue)
                {
                    text = strValue;
                }
                else if (model.Data is Action<string> action)
                {
                    onValueChangedAction = action;
                }
            }
        }

        void OnTextChanged(string newText)
        {
            onValueChangedAction?.Invoke(newText);
        }

        void OnEndEdit(string finalText)
        {
            onValueChangedAction?.Invoke(finalText);
        }

        public virtual void Active(bool active) => gameObject.SetActive(active);
        public Transform transformObject => transform;
    }
} 