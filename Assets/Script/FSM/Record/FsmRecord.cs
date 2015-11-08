using UnityEngine;
using System.Collections.Generic;

namespace FSM
{
    public class FsmRecord : ScriptableObject
    {
        public string Name;
        public List<FsmState> States;
    }
}
