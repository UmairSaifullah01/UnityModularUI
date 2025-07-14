using THEBADDEST.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace THEBADDEST.UI
{
    public class ProgressBarView : ViewBase
    {
        [SerializeField] private Image fillImage;
        public virtual string Id => gameObject.name;
        public IViewModel ViewModel { get; set; }

        public virtual void Init(IViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.ViewModel.ModelBinder += Bind;
        }

        protected virtual void Bind(string id, IModel<object> model)
        {
            if (Id.Equals(id) && model.Data is float value)
            {
                if (fillImage != null)
                    fillImage.fillAmount = Mathf.Clamp01(value);
            }
        }

        public virtual void Active(bool active) => gameObject.SetActive(active);
        public Transform GetTransform() => transform;
    }
} 