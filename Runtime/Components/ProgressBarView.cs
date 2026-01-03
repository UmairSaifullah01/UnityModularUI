using THEBADDEST.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace THEBADDEST.UI
{
    public class ProgressBarView : UIView
    {
        [SerializeField] private Image fillImage;
        private RectTransform fillRect;
        private float originalWidth;
        public virtual string Id => gameObject.name;
        public IViewModel ViewModel { get; set; }

        public virtual void Init(IViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.ViewModel.ModelBinder += Bind;
            if (fillImage != null)
            {
                fillRect = fillImage.rectTransform;
                originalWidth = fillRect.sizeDelta.x;
            }
        }

        protected virtual void Bind(string id, IModel<object> model)
        {
            if (Id.Equals(id) && model.Data is float value)
            {
                if (fillRect != null)
                {
                    float clamped = Mathf.Clamp01(value);
                    fillRect.sizeDelta = new Vector2(originalWidth * clamped, fillRect.sizeDelta.y);
                }
            }
        }

        public virtual void Active(bool active) => gameObject.SetActive(active);
        public Transform transformObject => transform;
    }
} 