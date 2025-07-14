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
            if (Id.Equals(id) && model.Data is string strValue)
            {
                text = strValue;
            }
        }

        void OnTextChanged(string newText)
        {
            // Optionally, notify ViewModel or raise event
        }

        void OnEndEdit(string finalText)
        {
            // Optionally, notify ViewModel or raise event
        }

        public virtual void Active(bool active) => gameObject.SetActive(active);
        public Transform GetTransform() => transform;
    }
} 