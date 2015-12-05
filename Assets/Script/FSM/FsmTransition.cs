using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace FSM
{
    [System.Serializable]
    public class FsmTransition
    {
        public FsmState State { get; set; }
        public FsmState NextState { get; set; }
        public List<FsmCondition> Conditions { get; set; }

        public bool MeetConditions()
        {
            return Conditions.All(item => item.MeetCondition());
        }
    }
}