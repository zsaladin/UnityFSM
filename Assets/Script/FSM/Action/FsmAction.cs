using UnityEngine;
using System.Collections;

namespace FSM
{
    [System.Serializable]
    public class FsmAction
    {
        public FsmState State { get; private set; }

        public FsmAction(FsmState state)
        {
            State = state;
        }

        public virtual void OnStart() { }
        public virtual void OnUpdate() { }
        public virtual void OnExit() { }
    }
}