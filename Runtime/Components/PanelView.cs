using System;
using THEBADDEST.MVVM;
using UnityEngine;
using System.Collections;

namespace THEBADDEST.UI
{
    public class PanelView : UIView
    {
        [SerializeField] private Animator animator;
        [SerializeField] private string openTrigger = "Open";
        [SerializeField] private string closeTrigger = "Close";
        public virtual string Id => gameObject.name;
        public IViewModel ViewModel { get; set; }

        public virtual void Init(IViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.ViewModel.ModelBinder += Bind;
        }

        protected virtual void Bind(string id, IModel<object> model)
        {
            if (Id.Equals(id) && model.Data is bool isActive)
            {
                if (animator != null)
                {
                    animator.SetTrigger(isActive ? openTrigger : closeTrigger);
                }
                else
                {
                    gameObject.SetActive(isActive);
                }
            }
        }

        public virtual void Active(bool active) => gameObject.SetActive(active);
        public Transform transformObject => transform;
    }
} 