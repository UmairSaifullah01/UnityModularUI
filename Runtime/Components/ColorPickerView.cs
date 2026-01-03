using System;
using THEBADDEST.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace THEBADDEST.UI
{
    public class ColorPickerView : UIView
    {
        [SerializeField] private Image colorPreview;
        public virtual string Id => gameObject.name;
        public IViewModel ViewModel { get; set; }
        public Action<Color> OnColorChanged;

        public virtual void Init(IViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.ViewModel.ModelBinder += Bind;
        }

        protected virtual void Bind(string id, IModel<object> model)
        {
            if (Id.Equals(id) && model.Data is Color color)
            {
                SetColor(color);
            }
        }

        public void SetColor(Color color)
        {
            if (colorPreview != null)
                colorPreview.color = color;
            OnColorChanged?.Invoke(color);
        }

        public virtual void Active(bool active) => gameObject.SetActive(active);
        public Transform GetTransform() => transform;
    }
} 