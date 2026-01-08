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