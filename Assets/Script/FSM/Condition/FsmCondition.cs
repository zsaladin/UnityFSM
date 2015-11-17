using UnityEngine;
using System.Collections;

namespace FSM
{
    [System.Serializable]
    public abstract class FsmCondition
    {
        public FsmState State { get; private set; }

        public FsmCondition(FsmState state)
        {
            State = state;
        }

        public abstract bool MeetCondition();
    }
}