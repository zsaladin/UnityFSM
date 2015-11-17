using UnityEngine;
using System.Collections.Generic;

namespace FSM
{
    public class FsmRecord : ScriptableObject
    {
        public string Name;
        public State EntryState;
        public List<State> States = new List<State>();
        
        [System.Serializable]
        public class State
        {
            public string Name = "";
            public List<FsmAction> Actions = new List<FsmAction>();
            public List<FsmCondition> Conditions = new List<FsmCondition>();
            public Rect Rect;
        }
    }

    
}
