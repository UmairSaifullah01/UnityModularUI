
using UnityEngine;


namespace THEBADDEST.DatabaseModule
{
    public class GameStatesTable : Table<GameStateRecord>
    {
        public GameStatesTable() : base()
        {
        }

        // Add your custom table logic here.
    }

    [System.Serializable]
    public class GameStateRecord
    {

        [Options(typeof(IState))]
        public string stateName;
        public GameObject stateObject;

    }
}