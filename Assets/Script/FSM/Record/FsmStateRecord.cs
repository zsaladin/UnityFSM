using UnityEngine;
using System;

namespace FSM
{
    public class FsmStatesRecord : ScriptableObject
    {
        public struct State
        {
            public string Name;
            public FsmAction[] Actions;
        }
    }
}