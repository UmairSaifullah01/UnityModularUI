using System.Collections.Generic;
using THEBADDEST.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace THEBADDEST.UI
{
    public class IconView : Image, IView
    {
        [SerializeField] private List<Sprite> iconSet;
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
                if (model.Data is int idx && idx >= 0 && idx < iconSet.Count)
                {
                    sprite = iconSet[idx];
                }
                else if (model.Data is Sprite s)
                {
                    sprite = s;
                }
            }
        }

        public virtual void Active(bool active) => gameObject.SetActive(active);
        public Transform GetTransform() => transform;
    }
} 