using System;
using System.Collections.Generic;
using THEBADDEST.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace THEBADDEST.UI
{
    public class ScrollViewView : ScrollRect, IView
    {
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private Transform contentRoot;
        private List<GameObject> items = new List<GameObject>();
        public virtual string Id => gameObject.name;
        public IViewModel ViewModel { get; set; }

        public virtual void Init(IViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.ViewModel.ModelBinder += Bind;
        }

        protected virtual void Bind(string id, IModel<object> model)
        {
            if (Id.Equals(id) && model.Data is IEnumerable<object> collection)
            {
                foreach (var item in items)
                    Destroy(item);
                items.Clear();
                foreach (var data in collection)
                {
                    var go = Instantiate(itemPrefab, contentRoot);
                    // Optionally bind data to item view here
                    items.Add(go);
                }
            }
        }

        public virtual void Active(bool active) => gameObject.SetActive(active);
        public Transform GetTransform() => transform;
    }
} 