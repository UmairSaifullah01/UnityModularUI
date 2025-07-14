using System.Collections.Generic;
using THEBADDEST.MVVM;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace THEBADDEST.UI
{
    
    public class ScrollDataView : ScrollRect, IView
    {
        [SerializeField] private GameObject itemPrefab;
        [SerializeField] private Transform contentRoot;
        private List<GameObject> items = new List<GameObject>();
        public virtual string Id => gameObject.name;
        public IViewModel ViewModel { get; set; }

        // ScrollData model
        public class ScrollData
        {
            public GameObject Prefab;
            public List<GameObject> Data;
            public Action<GameObject> OnItemSetup;
        }

        public virtual void Init(IViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.ViewModel.ModelBinder += Bind;
        }

        protected virtual void Bind(string id, IModel<object> model)
        {
            if (Id.Equals(id) && model.Data is ScrollData scrollData && scrollData.Data != null)
            {
                foreach (var item in items)
                    Destroy(item);
                items.Clear();
                foreach (var goData in scrollData.Data)
                {
                    var go = Instantiate(scrollData.Prefab != null ? scrollData.Prefab : itemPrefab, contentRoot);
                    if (scrollData.OnItemSetup != null)
                        scrollData.OnItemSetup.Invoke(go);
                    items.Add(go);
                }
            }
        }

        public virtual void Active(bool active) => gameObject.SetActive(active);
        public Transform transformObject => transform;
    }
} 