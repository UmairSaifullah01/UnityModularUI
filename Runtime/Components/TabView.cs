using System;
using System.Collections.Generic;
using THEBADDEST.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace THEBADDEST.UI
{
    public class TabView : UIView
    {
        [SerializeField] private List<Button> tabButtons;
        public virtual string Id => gameObject.name;
        public IViewModel ViewModel { get; set; }
        private int selectedIndex = 0;

        public virtual void Init(IViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.ViewModel.ModelBinder += Bind;
            for (int i = 0; i < tabButtons.Count; i++)
            {
                int idx = i;
                tabButtons[i].onClick.AddListener(() => OnTabClicked(idx));
            }
        }

        protected virtual void Bind(string id, IModel<object> model)
        {
            if (Id.Equals(id) && model.Data is int idx)
            {
                SelectTab(idx);
            }
        }

        void OnTabClicked(int idx)
        {
            SelectTab(idx);
            // Optionally, notify ViewModel or raise event
        }

        void SelectTab(int idx)
        {
            selectedIndex = idx;
            for (int i = 0; i < tabButtons.Count; i++)
            {
                tabButtons[i].interactable = i != idx;
            }
        }

        public virtual void Active(bool active) => gameObject.SetActive(active);
        public Transform GetTransform() => transform;
    }
} 