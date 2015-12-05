using UnityEngine;
using System.Collections.Generic;

namespace FSM
{
    [System.Serializable]
    public class FsmRecord
    {
        public string Name;
        public State EntryState;
        public List<State> States = new List<State>();
        
        [System.Serializable]
        public class State
        {
            public string Name = ""; 
            public List<FsmAction> Actions = new List<FsmAction>();
            public List<FsmTransition> Transitions = new List<FsmTransition> ();
            public Rect Rect;
        }
    }

    
}
