using THEBADDEST.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace THEBADDEST.UI
{
    public class TooltipView : UIView
    {
        [SerializeField] private Text tooltipText;
        [SerializeField] private GameObject tooltipPanel;
        public virtual string Id => gameObject.name;
        public IViewModel ViewModel { get; set; }

        public virtual void Init(IViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.ViewModel.ModelBinder += Bind;
        }

        protected virtual void Bind(string id, IModel<object> model)
        {
            if (Id.Equals(id))
            {
                if (model.Data is string str)
                {
                    tooltipText.text = str;
                }
                tooltipPanel.SetActive(!string.IsNullOrEmpty(tooltipText.text));
            }
        }

        public void ShowTooltip(string content)
        {
            tooltipText.text = content;
            tooltipPanel.SetActive(true);
        }

        public void HideTooltip()
        {
            tooltipPanel.SetActive(false);
        }

        public virtual void Active(bool active) => gameObject.SetActive(active);
        public Transform transformObject => transform;
    }
} 