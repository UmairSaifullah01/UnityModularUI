using THEBADDEST.DatabaseModule;
using UnityEngine;


namespace THEBADDEST.UI
{
    public class ApplicationFlow : Table<UIStateRecord>
    {
        [SerializeField] private string bootStateName = "";
        
        public string BootStateName => bootStateName;
        
        public ApplicationFlow() : base()
        {
        }

#if UNITY_EDITOR
        [System.Serializable]
        public class NodeData
        {
            public string stateName;
            public Vector2 position;
        }
        [HideInInspector]
        public System.Collections.Generic.List<NodeData> nodeMetaData = new System.Collections.Generic.List<NodeData>();
#endif

        // Add your custom table logic here.
    }

    [System.Serializable]
    public class UIStateRecord
    {
        [Options(typeof(IState))]
        public string stateName;
        public GameObject stateObject;
        public UITransition[] transitions;


        public ITransition[] GetTransitions()
        {
            ITransition[] transitionsArray = new ITransition[transitions.Length];
            for (int i = 0; i < transitions.Length; i++)
            {
                transitionsArray[i] = transitions[i] as ITransition;
            }
            return transitionsArray;
        }

    }
}