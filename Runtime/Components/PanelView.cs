using System;
using THEBADDEST.MVVM;
using UnityEngine;
using THEBADDEST.Tweening;
using System.Collections;

namespace THEBADDEST.UI
{
    public class PanelView : ViewBase
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
        public Transform GetTransform() => transform;
    }

    // PanelTweenView: Variant of PanelView using Tween for open/close animation
    public class PanelTweenView : ViewBase
    {
        [SerializeField] private Tween openTween;
        [SerializeField] private Tween closeTween;
        [SerializeField] private GameObject panelContent;
        private bool isOpen = false;

        public override void Init(IViewModel viewModel)
        {
            base.Init(viewModel);
            ViewModel.ModelBinder += Bind;
        }

        protected virtual void Bind(string id, IModel<object> model)
        {
            if (Id.Equals(id) && model.Data is bool active)
            {
                if (active)
                    OpenPanel();
                else
                    ClosePanel();
            }
        }

        public void OpenPanel()
        {
            if (panelContent != null)
                panelContent.SetActive(true);
            if (openTween != null)
                StartCoroutine(PlayTween(openTween));
            isOpen = true;
        }

        public void ClosePanel()
        {
            if (closeTween != null)
                StartCoroutine(PlayTween(closeTween, deactivateAfter:true));
            else if (panelContent != null)
                panelContent.SetActive(false);
            isOpen = false;
        }

        private IEnumerator PlayTween(Tween tween, bool deactivateAfter = false)
        {
            yield return tween.Play(panelContent != null ? panelContent.transform : transform);
            if (deactivateAfter && panelContent != null)
                panelContent.SetActive(false);
        }
    }
} 